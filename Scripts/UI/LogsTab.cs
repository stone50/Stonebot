namespace Stonebot.Scripts.UI {
    using Bot_Core.App_Cache;
    using Godot;
    using System;
    using System.Diagnostics;

    internal partial class LogsTab : Control {
        public LogsTab() => Logger.MessageLogged += OnMessageLogged;

        public override void _Ready() {
            VisibilityChanged += OnVisiblilityChanged;
            FilesButton.Pressed += OnFilesButtonPressed;
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
        private Button FilesButton = null!;
        [Export]
        private Button ScrollToBottomButton = null!;

        private VScrollBar ScrollContainerVScrollBar = null!;

        private void OnVisiblilityChanged() {
            if (Visible && !ScrollToBottomButton.Visible) {
                DelayScrollToBottom();
            }
        }

        private void OnFilesButtonPressed() => Process.Start(new ProcessStartInfo() {
            FileName = Constants.LogsPath,
            UseShellExecute = true
        });

        private void OnScrollToBottomButtonPressed() => ScrollToBottom();

        private void OnScrollContainerVScrollBarValueChanged(double _) => UpdateScrollToBottomButton();

        private void OnScrollContainerVScrollBarChanged() => UpdateScrollToBottomButton();

        private void UpdateScrollToBottomButton() => ScrollToBottomButton.Visible = !GetShouldScrollToBottom();

        private bool GetShouldScrollToBottom() => Math.Max(0, ScrollContainerVScrollBar.MaxValue - ScrollContainerVScrollBar.Size.Y) == ScrollContainerVScrollBar.Value;

        private void OnMessageLogged(object? _, Logger.MessageLoggedArgs args) {
            // some logs are create before config is loaded, so default log limit is 1000
            if (LogsContainer.GetChildCount() == (AppCache.Config.GetWithoutRefresh()?.DisplayLogLimit ?? 1000)) {
                LogsContainer.RemoveChild(LogsContainer.GetChild(0));
            }

            var logPanel = Resources.LogPanelScene.Instantiate<LogPanel>();
            logPanel.Init(args.LogMessage);
            logPanel.SelfModulate = args.LogType switch {
                Logger.LogType.Debug => Colors.Purple,
                Logger.LogType.Warning => Colors.Yellow,
                Logger.LogType.Error => Colors.Red,
                _ => Colors.White,
            };
            LogsContainer.AddChild(logPanel);
            if (GetShouldScrollToBottom()) {
                DelayScrollToBottom();
            }
        }

        private void ScrollToBottom() => ScrollContainer.ScrollVertical = int.MaxValue;
        private void DelayScrollToBottom() => Util.CallDeferred(() => Util.CallDeferred(ScrollToBottom));
    }
}
