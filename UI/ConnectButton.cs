namespace Stonebot.UI {
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Threading;
    using System;
    using Twitch;

    internal class ConnectButton : Button {
        public ConnectButton() {
            WebSocketClient.ClosedUnexpectedly += OnWebSocketClientClosedUnexpectedly;
            SetState(ConnectState.Disconnected);
            Click += OnUserClick;
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

        private async void OnUserClick(object? sender, RoutedEventArgs args) {
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
                    cancellationTokenSource?.Cancel();
                    State = ConnectState.Disconnected;
                    cancellationTokenSource = null;
                    break;
                case ConnectState.Disconnecting:
                    cancellationTokenSource?.Cancel();
                    State = ConnectState.Connected;
                    cancellationTokenSource = null;
                    break;
            }
        }

        private void SetState(ConnectState newState) {
            if (newState == State) {
                return;
            }

            switch (newState) {
                case ConnectState.Connected:
                    Content = "Connected";
                    Background = MainTheme.SuccessBrush1;
                    break;
                case ConnectState.Disconnected:
                    Content = "Connect";
                    Background = MainTheme.DangerBrush1;
                    break;
                case ConnectState.Connecting:
                    Content = "Cancel";
                    Background = MainTheme.DangerBrush1;
                    break;
                case ConnectState.Disconnecting:
                    Content = "Cancel";
                    Background = MainTheme.SuccessBrush1;
                    break;
            }

            state = newState;
        }
    }
}
