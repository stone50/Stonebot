namespace Stonebot.UI.Buttons {
    using Avalonia.Layout;
    using Avalonia.Threading;

    internal abstract class UserButton : SButton {
        public enum AuthorizationState {
            Authorized,
            Unauthorized,
            Loading,
        }

        public readonly MainPanel MainPanel;
        public AuthorizationState State { get => state; private set => SetState(value); }
        public abstract AuthorizationData? AuthorizationData { get; }

        public UserButton(MainPanel mainPanel) : base(MainTheme.InfoBrush3, MainTheme.InfoBrush1, MainTheme.InfoBrush2) {
            MainPanel = mainPanel;
            FontFamily = MainTheme.Font;
            FontSize = 18d;
            Foreground = MainTheme.NeutralBrush1;
            HorizontalContentAlignment = HorizontalAlignment.Left;
            VerticalContentAlignment = VerticalAlignment.Center;
            CornerRadius = new(5d);
            Padding = new(10d);
            State = AuthorizationState.Loading;
        }

        public abstract void Authorize(CancellationToken cancellationToken);

        public abstract void ClearAuthorizationData();

        public void UpdateState() => State = AuthorizationData is null ? AuthorizationState.Unauthorized : AuthorizationState.Authorized;

        protected override void OnClick() {
            switch (State) {
                case AuthorizationState.Authorized:
                    State = AuthorizationState.Loading;
                    MainPanel.RemoveAuthorizationPopup.Show(UpdateState, FireClearAuthorizationData);
                    break;
                case AuthorizationState.Unauthorized:
                    State = AuthorizationState.Loading;
                    var cancellationTokenSource = new CancellationTokenSource();
                    FireAuthorize(cancellationTokenSource.Token);
                    MainPanel.AuthorizePopup.Show(cancellationTokenSource);
                    break;
            }

            base.OnClick();
        }

        private AuthorizationState state;

        private void SetState(AuthorizationState newState) {
            if (newState == State) {
                return;
            }

            state = newState;
            switch (State) {
                case AuthorizationState.Loading:
                    Content = "...";
                    break;
                case AuthorizationState.Authorized:
                    Content = AuthorizationData!.UserLogin;
                    break;
                case AuthorizationState.Unauthorized:
                    if (MainPanel.ConnectButton.State == ConnectButton.ConnectState.Connected) {
                        MainPanel.ConnectButton.ManualClick();
                    }

                    Content = "Click to Authorize";
                    break;
            }
        }

        private void FireClearAuthorizationData() => Task.Run(() => Utils.TryElseError(() => {
            ClearAuthorizationData();
            Cache.Save();
            Dispatcher.UIThread.Invoke(UpdateState);
        }));

        private void FireAuthorize(CancellationToken cancellationToken) => Task.Run(() => {
            Utils.TryElseError(() => Authorize(cancellationToken));
            Dispatcher.UIThread.Invoke(() => {
                MainPanel.AuthorizePopup.IsVisible = false;
                UpdateState();
            });
        }, cancellationToken);
    }
}
