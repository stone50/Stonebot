namespace Stonebot.Scripts {
    using Godot;

    internal static class Resources {
        public static readonly Texture2D EnableIcon = GD.Load<Texture2D>("res://Icons/enable.svg");
        public static readonly Texture2D DisableIcon = GD.Load<Texture2D>("res://Icons/disable.svg");

        public static readonly PackedScene LogPanelScene = GD.Load<PackedScene>("res://Scenes/Templates/LogPanel.tscn");
        public static readonly PackedScene CommandPanelScene = GD.Load<PackedScene>("res://Scenes/Templates/CommandPanel.tscn");
        public static readonly PackedScene TogglableCommandPanelScene = GD.Load<PackedScene>("res://Scenes/Templates/TogglableCommandPanel.tscn");
        public static readonly PackedScene MessagePanelScene = GD.Load<PackedScene>("res://Scenes/Templates/MessagePanel.tscn");
        public static readonly PackedScene TimerPanelScene = GD.Load<PackedScene>("res://Scenes/Templates/TimerPanel.tscn");
    }
}
