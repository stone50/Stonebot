namespace Stonebot.UI.CustomControls.Buttons.UserButtons {
    using Avalonia.Interactivity;
    using Avalonia.Threading;
    using Popups;

    internal abstract class UserButton : InfoButton {
        public enum AuthorizationState {
            Authorized,
            Unauthorized,
            Loading,
        }

        public AuthorizationState State { get => state; private set => SetState(value); }

        public UserButton(RemoveAuthorizationPopup removeAuthorizationPopup, CancelPopup cancelAuthorizationPopup) {
            State = AuthorizationState.Loading;
            Click += GetOnClick(removeAuthorizationPopup, cancelAuthorizationPopup);
        }

        public void Update() => State = AuthorizationData is null ? AuthorizationState.Unauthorized : AuthorizationState.Authorized;

        protected abstract AuthorizationData? AuthorizationData { get; }

        protected abstract void CreateAccessToken(CancellationToken cancellation);

        protected abstract void ClearAuthorizationData();

        private AuthorizationState state;

        private void SetState(AuthorizationState newState) {
            if (State == newState) {
                return;
            }

            state = newState;
            switch (State) {
                case AuthorizationState.Authorized:
                    Content = AuthorizationData!.UserLogin;
                    break;
                case AuthorizationState.Unauthorized:
                    Content = "Click to Authorize";
                    break;
                case AuthorizationState.Loading:
                    Content = "...";
                    break;
            }
        }

        private EventHandler<RoutedEventArgs> GetOnClick(RemoveAuthorizationPopup removeAuthorizationPopup, CancelPopup cancelAuthorizationPopup) => (_, _) => {
            switch (State) {
                case AuthorizationState.Authorized:
                    State = AuthorizationState.Loading;
                    removeAuthorizationPopup.Show(() => State = AuthorizationState.Authorized, () => {
                        ClearAuthorizationData();
                        _ = Utils.FireTryElseError(() => {
                            Cache.Save();
                            _ = Dispatcher.UIThread.Invoke(() => State = AuthorizationState.Unauthorized);
                        });
                    });
                    break;
                case AuthorizationState.Unauthorized:
                    State = AuthorizationState.Loading;
                    Authorize(cancelAuthorizationPopup);
                    break;
            }
        };

        private void Authorize(CancelPopup cancelAuthorizationPopup) {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var createAccessTokenTask = Utils.FireTryElseError(() => {
                CreateAccessToken(cancellationToken);
                Dispatcher.UIThread.Invoke(() => {
                    State = AuthorizationState.Authorized;
                    cancelAuthorizationPopup.Hide();
                });
            }, cancellationToken);
            cancelAuthorizationPopup.Show(() => {
                cancellationTokenSource.Cancel();
                Utils.Sync(createAccessTokenTask);
                State = AuthorizationState.Unauthorized;
            });
        }
    }
}
