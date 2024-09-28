namespace StoneBot.Scripts {
    using Core_Interface;
    using Godot;
    using System.Threading.Tasks;

    internal partial class App : Node {
        public override void _Ready() => Task.Run(Meta.Startup);

        public override void _Notification(int what) {
            if (what == NotificationWMCloseRequest) {
                Task.Run(Meta.Shutdown).Wait();
                GetTree().Quit();
            }
        }
    }
}
