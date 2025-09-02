namespace Stonebot.UI.CustomControls.Buttons {
    using Avalonia.Interactivity;
    using Avalonia.Layout;
    using Models.Responses;
    using Popups;
    using System;
    using System.Diagnostics;
    using Twitch;

    internal class AuthorizeButton : InfoButton {
        public enum AuthState {
            Authorized,
            Unauthorized,
            Loading,
        }

        public AuthState State { get => state; private set => SetState(value); }

        public AuthorizeButton(CancelOkPopup clearAuthPopup, ConfirmAuthPopup confirmAuthPopup) {
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Center;
            Width = 280d;

            State = AuthState.Loading;
            Click += GetOnClick(clearAuthPopup, confirmAuthPopup);
        }

        public void Update() => State = Cache.IsAuthorized ? AuthState.Authorized : AuthState.Unauthorized;

        private AuthState state;

        private void SetState(AuthState newState) {
            if (State == newState) {
                return;
            }

            state = newState;
            switch (State) {
                case AuthState.Authorized:
                    Content = Cache.GetChatterDisplayName();
                    break;
                case AuthState.Unauthorized:
                    Content = "Click to Authorize";
                    break;
                case AuthState.Loading:
                    Content = "...";
                    break;
            }
        }

        private EventHandler<RoutedEventArgs> GetOnClick(CancelOkPopup clearAuthPopup, ConfirmAuthPopup cancelAuthorizationPopup) => (_, _) => {
            switch (State) {
                case AuthState.Authorized:
                    TryClearAuthData(clearAuthPopup);
                    break;
                case AuthState.Unauthorized:
                    TryAuthorize(cancelAuthorizationPopup);
                    break;
            }
        };

        private void TryClearAuthData(CancelOkPopup clearAuthPopup) {
            State = AuthState.Loading;
            clearAuthPopup.Show(
                () => State = AuthState.Authorized,
                () => {
                    try {
                        Cache.ClearAuthData();
                        State = AuthState.Unauthorized;
                    } catch (Exception e) {
                        Logger.Error(e);
                        State = AuthState.Authorized;
                    }
                }
            );
        }

        private void TryAuthorize(ConfirmAuthPopup confirmAuthPopup) {
            State = AuthState.Loading;
            PostDeviceCodeResponse deviceCodeResponse;
            try {
                deviceCodeResponse = Auth.GetDeviceCode();
            } catch (Exception e) {
                Logger.Error(e);
                State = AuthState.Unauthorized;
                return;
            }

            try {
                _ = Process.Start(new ProcessStartInfo {
                    FileName = deviceCodeResponse.VerificationUri,
                    UseShellExecute = true,
                });
            } catch (Exception e) {
                Logger.Error(e);
            }

            confirmAuthPopup.Show(deviceCodeResponse.UserCode, deviceCodeResponse.VerificationUri,
                () => State = AuthState.Unauthorized,
                () => {
                    try {
                        Cache.LoadNewAccessToken(deviceCodeResponse.DeviceCode);
                        State = AuthState.Authorized;
                    } catch (Exception e) {
                        Logger.Error(e);
                        State = AuthState.Unauthorized;
                    }
                }
            );

        }
    }
}
