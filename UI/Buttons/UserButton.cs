namespace Stonebot.UI.Buttons {
    using Avalonia.Media;
    using Avalonia.Threading;

    internal abstract class UserButton : SButton {
        public enum AuthorizationState {
            Authorized,
            Unauthorized,
            Loading,
        }

        public AuthorizationState State { get => state; private set => SetState(value); }
        public abstract AuthorizationData? AuthorizationData { get; }

        public UserButton(
            MainWindow mainWindow,
            IImmutableBrush defaultBrush,
            IImmutableBrush hoveredBrush,
            IImmutableBrush pressedBrush
        ) : base(mainWindow, defaultBrush, hoveredBrush, pressedBrush) => State = AuthorizationState.Loading;

        public abstract void Authorize(CancellationToken cancellationToken);

        public abstract void ClearAuthorizationData();

        public void UpdateState() => State = AuthorizationData is null ? AuthorizationState.Unauthorized : AuthorizationState.Authorized;

        protected override void OnClick() {
            switch (State) {
                case AuthorizationState.Authorized:
                    State = AuthorizationState.Loading;
                    MainWindow.RemoveAuthorizationPopup.Show(UpdateState, FireClearAuthorizationData);
                    break;
                case AuthorizationState.Unauthorized:
                    State = AuthorizationState.Loading;
                    var cancellationTokenSource = new CancellationTokenSource();
                    FireAuthorize(cancellationTokenSource.Token);
                    MainWindow.AuthorizePopup.Show(cancellationTokenSource);
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
                    Content = ". . .";
                    break;
                case AuthorizationState.Authorized:
                    Content = AuthorizationData!.UserLogin;
                    break;
                case AuthorizationState.Unauthorized:
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
                MainWindow.AuthorizePopup.IsVisible = false;
                UpdateState();
            });
        }, cancellationToken);
    }
}
