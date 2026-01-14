namespace StonebotCore.Twitch {
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;
    using TwitchLib.Client.Models;

    internal static class Bot {
        internal static void ConfigureClient(ILogger<Interface.TwitchClientLog>? logger) {
            Access.Logger = logger;
            var client = Access.Client;
            var credentials = new ConnectionCredentials(
                twitchUsername: "stonebot5555",
                twitchOAuth: Access.API.Settings.AccessToken
            );
            client.Initialize(
                credentials,
                channel: "stonebot5555"
            );
            client.OnMessageReceived += OnMessageReceived;
            client.OnChatCommandReceived += OnChatCommandReceived;
        }

        private static async Task OnChatCommandReceived(object? sender, OnChatCommandReceivedArgs args) {
            var logger = Access.Logger;
            if (logger?.IsEnabled(LogLevel.Debug) ?? false) {
                logger?.LogDebug("Command: {Command}", args.Command);
                logger?.LogDebug("ChatMessage: {ChatMessage}", args.ChatMessage);
            }
        }

        private static async Task OnMessageReceived(object? sender, OnMessageReceivedArgs args) {
            var logger = Access.Logger;
            if (logger?.IsEnabled(LogLevel.Debug) ?? false) {
                logger?.LogDebug("{DateTime}: {Username} - {Message}", DateTime.UtcNow, args.ChatMessage.Username, args.ChatMessage.Message);
            }
        }
    }
}
