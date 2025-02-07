namespace Stonebot.Scripts.UI {
    using Godot;
    using System;

    internal partial class LogsTab : Control {
        public LogsTab() => Logger.MessageLogged += OnMessageLogged;

        public override void _Ready() {
            VisibilityChanged += OnVisiblilityChanged;
            ScrollContainerVScrollBar = ScrollContainer.GetVScrollBar();
            ScrollContainerVScrollBar.ValueChanged += OnScrollContainerVScrollBarValueChanged;
            ScrollContainerVScrollBar.Changed += OnScrollContainerVScrollBarChanged;
            ScrollToBottomButton.Pressed += OnScrollToBottomButtonPressed;
        }

        [Export]
        private ScrollContainer ScrollContainer = null!;
        [Export]
        private Container LogsContainer = null!;
        [Export]
        private Button ScrollToBottomButton = null!;

        private VScrollBar ScrollContainerVScrollBar = null!;

        private void OnVisiblilityChanged() {
            if (Visible && !ScrollToBottomButton.Visible) {
                DelayScrollToBottom();
            }
        }

        private void OnScrollToBottomButtonPressed() => ScrollToBottom();

        private void OnScrollContainerVScrollBarValueChanged(double _) => UpdateScrollToBottomButton();

        private void OnScrollContainerVScrollBarChanged() => UpdateScrollToBottomButton();

        private void UpdateScrollToBottomButton() => ScrollToBottomButton.Visible = Math.Max(0, ScrollContainerVScrollBar.MaxValue - ScrollContainerVScrollBar.Size.Y) != (int)ScrollContainerVScrollBar.Value;

        private void OnMessageLogged(object? _, Logger.MessageLoggedArgs args) {
            var shouldScrollDown = Math.Max(0, ScrollContainerVScrollBar.MaxValue - ScrollContainerVScrollBar.Size.Y) == ScrollContainerVScrollBar.Value;
            var logPanel = Resources.LogPanelScene.Instantiate<LogPanel>();
            logPanel.Init(args.LogMessage);
            logPanel.SelfModulate = args.LogType switch {
                Logger.LogType.Debug => Colors.Purple,
                Logger.LogType.Warning => Colors.Yellow,
                Logger.LogType.Error => Colors.Red,
                _ => Colors.White,
            };
            LogsContainer.AddChild(logPanel);
            if (shouldScrollDown) {
                DelayScrollToBottom();
            }
        }

        private void ScrollToBottom() => ScrollContainer.ScrollVertical = int.MaxValue;
        private void DelayScrollToBottom() => Util.CallDeferred(() => Util.CallDeferred(ScrollToBottom));
    }
}
