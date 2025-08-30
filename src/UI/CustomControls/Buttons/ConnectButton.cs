namespace Stonebot.UI.CustomControls.Buttons {
    using Avalonia.Interactivity;
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

        public ConnectButton() {
            CornerRadius = new(50d);
            MaxHeight = 50d;
            MinWidth = 220d;

            State = ConnectState.Disconnected;
            Click += OnClick;
            WebSocketClient.ClosedUnexpectedly += OnWebSocketClientClosedUnexpectedly;
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

        private ConnectState state;

        private void SetState(ConnectState newState) {
            if (State == newState) {
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

        private void OnClick(object? sender, RoutedEventArgs e) {
            switch (State) {
                case ConnectState.Connected:
                    TryDisconnect();
                    break;
                case ConnectState.Disconnected:
                    TryConnect();
                    break;
            }
        }

        private void TryConnect() {
            if (!Cache.IsAuthorized) {
                return;
            }

            if (string.IsNullOrEmpty(Config.BroadcasterUsername)) {
                return;
            }

            State = ConnectState.Connecting;
            try {
                if (string.IsNullOrEmpty(Cache.BroadcasterId)) {
                    State = ConnectState.Disconnected;
                    return;
                }
            } catch (Exception e) {
                Logger.Warn(e);
                State = ConnectState.Disconnected;
                return;
            }

            try {
                WebSocketClient.Connect();
                State = ConnectState.Connected;
            } catch (Exception e) {
                Logger.Error(e);
                State = ConnectState.Disconnected;
            }
        }

        private void TryDisconnect() {
            State = ConnectState.Disconnecting;
            try {
                WebSocketClient.Close();
                State = ConnectState.Disconnected;
            } catch (Exception e) {
                Logger.Error(e);
                State = ConnectState.Connected;
            }
        }

        private void OnWebSocketClientClosedUnexpectedly(object? sender, EventArgs args) => Dispatcher.UIThread.Invoke(() => SetState(ConnectState.Disconnected));
    }
}
