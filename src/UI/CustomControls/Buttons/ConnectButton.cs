namespace Stonebot.UI.CustomControls.Buttons {
    using Avalonia.Interactivity;
    using Avalonia.Threading;
    using Helpers;
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
            State = ConnectState.Connecting;
            _ = TaskHelper.FireTryElse(() => {
                WebSocketClient.Connect();
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Connected);
            }, e => {
                Logger.Error(e);
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Disconnected);
            });
        }

        private void TryDisconnect() {
            State = ConnectState.Disconnecting;
            _ = TaskHelper.FireTryElse(() => {
                WebSocketClient.Close();
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Disconnected);
            }, e => {
                Logger.Error(e);
                _ = Dispatcher.UIThread.Invoke(() => State = ConnectState.Connected);
            });
        }

        private void OnWebSocketClientClosedUnexpectedly(object? sender, EventArgs args) => Dispatcher.UIThread.Invoke(() => SetState(ConnectState.Disconnected));
    }
}
