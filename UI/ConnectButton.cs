namespace Stonebot.UI {
    using Avalonia.Threading;
    using System;
    using Twitch;

    internal class ConnectButton : SButtonBase {
        public ConnectButton() : base() {
            WebSocketClient.ClosedUnexpectedly += OnWebSocketClientClosedUnexpectedly;
            SetState(ConnectState.Disconnected);
        }

        protected override async void OnClick() {
            switch (State) {
                case ConnectState.Connected:
                    State = ConnectState.Disconnecting;
                    cancellationTokenSource = new CancellationTokenSource();
                    try {
                        await WebSocketClient.CloseAsync(cancellationTokenSource.Token);
                        State = ConnectState.Disconnected;
                        await EventSub.DeleteEventSubsAsync(cancellationTokenSource.Token);
                    } catch (OperationCanceledException) {
                        State = ConnectState.Connected;
                    } catch (Exception e) {
                        State = ConnectState.Connected;
                        Logger.Error(e);
                    }

                    break;
                case ConnectState.Disconnected:
                    State = ConnectState.Connecting;
                    cancellationTokenSource = new CancellationTokenSource();
                    try {
                        if (await WebSocketClient.TryConnectAsync(cancellationTokenSource.Token)) {
                            await EventSub.SubscribeToChannelChatMessageAsync(cancellationTokenSource.Token);
                            State = ConnectState.Connected;
                        } else {
                            State = ConnectState.Disconnected;
                        }
                    } catch (OperationCanceledException) {
                        State = ConnectState.Disconnected;
                    } catch (Exception e) {
                        State = ConnectState.Disconnected;
                        Logger.Error(e);
                    }

                    break;
                case ConnectState.Connecting:
                    await cancellationTokenSource!.CancelAsync();
                    State = ConnectState.Disconnected;
                    cancellationTokenSource = null;
                    break;
                case ConnectState.Disconnecting:
                    await cancellationTokenSource!.CancelAsync();
                    State = ConnectState.Connected;
                    cancellationTokenSource = null;
                    break;
            }

            base.OnClick();
        }

        private enum ConnectState {
            Connected,
            Disconnected,
            Connecting,
            Disconnecting,
        }

        private ConnectState State { get => state; set => SetState(value); }
        private ConnectState state;
        private CancellationTokenSource? cancellationTokenSource;

        private void OnWebSocketClientClosedUnexpectedly(object? sender, EventArgs args) => Dispatcher.UIThread.Invoke(() => {
            cancellationTokenSource?.Cancel();
            SetState(ConnectState.Disconnected);
            cancellationTokenSource = null;
        });

        private void SetState(ConnectState newState) {
            if (newState == State) {
                return;
            }

            switch (newState) {
                case ConnectState.Connected:
                    Content = "Disconnect";
                    break;
                case ConnectState.Disconnected:
                    Content = "Connect";
                    break;
                case ConnectState.Connecting:
                    Content = "Cancel";
                    break;
                case ConnectState.Disconnecting:
                    Content = "Cancel";
                    break;
            }

            state = newState;
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
