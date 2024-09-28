namespace StoneBot.Scripts.UI {
    using Godot;
    using System;

    internal partial class LogsTab : Control {
        private static readonly PackedScene LogPanelScene = GD.Load<PackedScene>("res://Scenes/Templates/LogPanel.tscn");

        [Export]
        private ScrollContainer ScrollContainer = null!;
        [Export]
        private Container LogsContainer = null!;
        [Export]
        private Button ScrollToBottomButton = null!;

        private VScrollBar ScrollContainerVScrollBar = null!;

        public LogsTab() => Logger.MessageLogged += OnMessageLogged;

        public override void _Ready() {
            ScrollContainerVScrollBar = ScrollContainer.GetVScrollBar();
            ScrollContainerVScrollBar.ValueChanged += OnScrollContainerVScrollBarValueChanged;
            ScrollToBottomButton.Pressed += OnScrollToBottomButtonPressed;
        }

        private void OnScrollToBottomButtonPressed() => ScrollContainer.ScrollVertical = ScrollContainer.ScrollVertical = int.MaxValue;

        private void OnScrollContainerVScrollBarValueChanged(double value) => ScrollToBottomButton.Visible = Math.Max(0, ScrollContainerVScrollBar.MaxValue - ScrollContainerVScrollBar.Size.Y) != (int)value;

        private void OnMessageLogged(object? _, Logger.MessageLoggedArgs args) {
            var shouldScrollDown = Math.Max(0, ScrollContainerVScrollBar.MaxValue - ScrollContainerVScrollBar.Size.Y) == ScrollContainerVScrollBar.Value;
            var logPanel = LogPanelScene.Instantiate<LogPanel>();
            logPanel.Init(args.LogMessage);
            logPanel.SelfModulate = args.LogType switch {
                Logger.LogType.Debug => Colors.Purple,
                Logger.LogType.Warning => Colors.Yellow,
                Logger.LogType.Error => Colors.Red,
                _ => Colors.White,
            };
            LogsContainer.AddChild(logPanel);
            if (shouldScrollDown) {
                Util.CallDeferred(() => Util.CallDeferred(() => ScrollContainer.ScrollVertical = int.MaxValue)); // wtf godot
            }
        }
    }
}
