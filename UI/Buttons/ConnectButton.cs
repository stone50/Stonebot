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

        public ConnectState State { get => state; private set => SetState(value); }

        public ConnectButton(MainWindow mainWindow) : base(mainWindow) {
            WebSocketClient.ClosedUnexpectedly += OnWebSocketClientClosedUnexpectedly;
            State = ConnectState.Disconnected;
        }

        protected override void OnClick() {
            switch (State) {
                case ConnectState.Connected:
                    State = ConnectState.Disconnecting;
                    var disconnectCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.WebSocketClientDisconnectTimeoutSeconds));
                    FireDisconnect(disconnectCancellationTokenSource.Token);
                    break;
                case ConnectState.Disconnected:
                    State = ConnectState.Connecting;
                    var connectCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.WebSocketClientConnectTimeoutSeconds));
                    FireConnect(connectCancellationTokenSource.Token);
                    break;
            }

            base.OnClick();
        }

        private ConnectState state;

        private void FireConnect(CancellationToken cancellationToken) => Task.Run(() => Utils.TryElseError(() => {
            try {
                if (Cache.BroadcasterAuthorizationData is null) {
                    Dispatcher.UIThread.Invoke(() => {
                        MainWindow.BroadcasterButton.Background = MainTheme.DangerBrush2;
                        State = ConnectState.Disconnected;
                    });
                    return;
                }

                if (Cache.ChatterAuthorizationData is null) {
                    Dispatcher.UIThread.Invoke(() => {
                        MainWindow.ChatterButton.Background = MainTheme.DangerBrush2;
                        State = ConnectState.Disconnected;
                    });
                    return;
                }

                WebSocketClient.Connect(cancellationToken);
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Connected);
            } catch {
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Disconnected);
                throw;
            }
        }), cancellationToken);

        private void FireDisconnect(CancellationToken cancellationToken) => Task.Run(() => Utils.TryElseError(() => {
            try {
                WebSocketClient.Close(cancellationToken);
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Disconnected);
            } catch {
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Connected);
            }
        }), cancellationToken);

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
