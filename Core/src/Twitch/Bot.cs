namespace StonebotCore.Twitch {
    using System;
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
            client.OnLog += OnLog;
        }

        private static void OnLog(object? sender, OnLogArgs e) =>
            Console.WriteLine($"{e.DateTime}: {e.BotUsername} - {e.Data}");
    }
}
