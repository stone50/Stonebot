namespace StoneBot.Scripts {
    using Godot;

    internal partial class App : Node {
        //public override void _Ready() => Task.Run(Meta.Startup);

        public override void _Notification(int what) {
            if (what == NotificationWMCloseRequest) {
                //Task.Run(Meta.Shutdown).Wait();
                GetTree().Quit();
            }
        }
    }
}
