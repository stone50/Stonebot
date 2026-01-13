namespace StonebotCore.Twitch {
    using System;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;
    using TwitchLib.Client.Models;

    internal static class Bot {

        internal static void ConfigureClient() {
            var client = Access.Client;
            var credentials = new ConnectionCredentials(
                twitchUsername: "stonebot5555",
                twitchOAuth: Access.API.Settings.AccessToken
            );
            client.Initialize(
                credentials,
                channel: "stone5555"
            );
            client.OnMessageReceived += OnMessageReceived;
        }

        private static async Task OnMessageReceived(object? sender, OnMessageReceivedArgs args) =>
            Console.WriteLine($"{DateTime.UtcNow}: {args.ChatMessage.Username} - {args.ChatMessage.Message}");
    }
}
