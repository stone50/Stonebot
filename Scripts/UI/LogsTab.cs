namespace StoneBot.Scripts.UI {
    using Godot;

    internal partial class LogsTab : Control {
        private static readonly PackedScene InfoLogScene = GD.Load<PackedScene>("res://Scenes/Templates/Logs/InfoLog.tscn");

        [Export]
        private ScrollContainer ScrollContainer = null!;
        [Export]
        private Container LogsContainer = null!;

        public LogsTab() {
            Logger.InfoLogged += OnInfoLogged;
            Logger.Debug("ADDED");
        }

        private void OnInfoLogged(object? _, string message) => Util.CallDeferred(() => {
            var shouldScrollDown = ScrollContainer.ScrollVertical == ScrollContainer.GetVScrollBar().MaxValue;
            var infoLog = InfoLogScene.Instantiate<InfoLog>();
            infoLog.Init(message);
            LogsContainer.AddChild(infoLog);
            if (shouldScrollDown) {
                ScrollContainer.ScrollVertical = int.MaxValue;
            }
        });
    }
}
