namespace Stonebot.UI.Buttons {
    using Avalonia.Media;
    using Avalonia.Threading;

    internal abstract class UserButton : SButton {
        public enum AuthorizationState {
            Authorized,
            Unauthorized,
            Loading,
        }

        public readonly RemoveAuthorizationPopup RemoveAuthorizationPopup;
        public AuthorizationState State { get => state; private set => SetState(value); }
        public abstract AuthorizationData? AuthorizationData { get; }

        public UserButton(RemoveAuthorizationPopup removeAuthorizationPopup, IImmutableBrush defaultBrush, IImmutableBrush hoveredBrush, IImmutableBrush pressedBrush) : base(defaultBrush, hoveredBrush, pressedBrush) {
            RemoveAuthorizationPopup = removeAuthorizationPopup;
            State = AuthorizationState.Loading;
        }

        public abstract void CreateAuthorizationData(CancellationToken cancellationToken);

        public abstract void ClearAuthorizationData();

        public void UpdateState() => State = AuthorizationData is null ? AuthorizationState.Unauthorized : AuthorizationState.Authorized;

        protected override void OnClick() {
            switch (State) {
                case AuthorizationState.Authorized:
                    State = AuthorizationState.Loading;
                    RemoveAuthorizationPopup.Show(UpdateState, FireClearAuthorizationData);
                    break;
                case AuthorizationState.Unauthorized:
                    // TODO: create a popup to inform user to authorize, or allow for cancellation
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
    }
}
