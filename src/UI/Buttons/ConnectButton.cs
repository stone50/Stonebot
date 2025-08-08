namespace Stonebot.UI.Buttons {
    using Avalonia.Threading;
    using System;

    internal class ConnectButton : SButtonBase {
        public enum ConnectState {
            Connected,
            Disconnected,
            Connecting,
            Disconnecting,
        }

        public readonly MainPanel MainPanel;
        public ConnectState State { get => state; private set => SetState(value); }

        public ConnectButton(MainPanel mainPanel) {
            MainPanel = mainPanel;
            CornerRadius = new(50d);
            MaxHeight = 50d;
            MinWidth = 220d;
            WebSocketClient.ClosedUnexpectedly += OnWebSocketClientClosedUnexpectedly;
            State = ConnectState.Disconnected;
        }

        public void Disconnect() {
            State = ConnectState.Disconnecting;
            var disconnectCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.WebSocketClientDisconnectTimeoutSeconds));
            FireDisconnect(disconnectCancellationTokenSource.Token);
        }

        protected override void OnClick() {
            switch (State) {
                case ConnectState.Connected:
                    Disconnect();
                    break;
                case ConnectState.Disconnected:
                    Connect();
                    break;
            }

            base.OnClick();
        }

        private ConnectState state;

        private void Connect() {
            if (Cache.BroadcasterAuthorizationData is null) {
                MainPanel.BroadcasterButton.Background = MainTheme.DangerBrush2;
                return;
            }

            if (Cache.ChatterAuthorizationData is null) {
                MainPanel.ChatterButton.Background = MainTheme.DangerBrush2;
                return;
            }

            State = ConnectState.Connecting;
            var connectCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Config.WebSocketConnectTimeoutSeconds));
            FireConnect(connectCancellationTokenSource.Token);
        }

        private void FireConnect(CancellationToken cancellationToken) => Utils.FireTryElseError(() => {
            try {
                WebSocketClient.Connect(cancellationToken);
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Connected);
            } catch {
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Disconnected);
                throw;
            }
        }, cancellationToken);

        private void FireDisconnect(CancellationToken cancellationToken) => Utils.FireTryElseError(() => {
            try {
                WebSocketClient.Close(cancellationToken);
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Disconnected);
            } catch {
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Connected);
                throw;
            }
        }, cancellationToken);

        private void OnWebSocketClientClosedUnexpectedly(object? sender, EventArgs args) => Dispatcher.UIThread.Invoke(() => SetState(ConnectState.Disconnected));

        private void SetState(ConnectState newState) {
            if (newState == State) {
                return;
            }

            state = newState;
            switch (State) {
                case ConnectState.Connected:
                    Content = "Disconnect";
                    break;
                case ConnectState.Disconnected:
                    Content = "Connect";
                    break;
                case ConnectState.Connecting:
                    Content = "Connecting...";
                    break;
                case ConnectState.Disconnecting:
                    Content = "Disconnecting...";
                    break;
            }

            UpdateBackground();
        }

        protected override void UpdateBackground() => Background =
            State == ConnectState.Disconnected
                ? IsPressed
                    ? MainTheme.SuccessBrush3
                : IsPointerOver
                    ? MainTheme.SuccessBrush1
                    : MainTheme.SuccessBrush2
            : IsPressed
                ? MainTheme.DangerBrush3
            : IsPointerOver
                ? MainTheme.DangerBrush1
                : MainTheme.DangerBrush2;
    }
}
