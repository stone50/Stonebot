namespace StoneBot.Scripts {
    using Godot;
    using System.Threading.Tasks;

    internal partial class App : Node {
        private readonly Client Client;

        public App() {
            _ = Configuration.Init();

            Client = new Client();
            _ = Task.Run(async () => {
                var code = await Client.GetAuthorizationCode();
                GD.Print($"Authorization Code: {code}");
            });

        }
    }
}
