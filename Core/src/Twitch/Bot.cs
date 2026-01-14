namespace StonebotCore.Twitch {
    using Microsoft.Extensions.Logging;
    using TwitchLib.Client.Models;

    internal static class Bot {
        internal static void ConfigureClient(ILogger<Interface.TwitchClientLog>? logger) {
            Access.Logger = logger;
            var client = Access.Client;
            var config = StonebotCore.Access.Config;
            var credentials = new ConnectionCredentials(
                twitchUsername: config.TwitchBotUsername,
                twitchOAuth: Access.API.Settings.AccessToken
            );
            client.Initialize(
                credentials,
                channel: config.TwitchBroadcasterChannel
            );
            client.OnChatCommandReceived += CommandManager.OnChatCommandReceived;
        }
    }
}
