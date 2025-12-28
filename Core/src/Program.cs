namespace StonebotCore {
    using System;
    using TwitchLib.Client;
    using TwitchLib.Client.Enums;
    using TwitchLib.Client.Events;
    using TwitchLib.Client.Extensions;
    using TwitchLib.Client.Models;
    using TwitchLib.Communication.Clients;
    using TwitchLib.Communication.Models;

    public static class Program {
        public static void Main(string[] args) {
            _ = new Bot();
            _ = Console.ReadLine();
        }
    }

    internal class Bot {
        private readonly TwitchClient client;

        public Bot() {
            var credentials = new ConnectionCredentials("stonebot5555", "access_token");
            client = new TwitchClient();
            client.Initialize(credentials, "stone5555");

            client.OnLog += Client_OnLog;

            _ = client.Connect();
        }

        private void Client_OnLog(object sender, OnLogArgs e) => Console.WriteLine($"{e.DateTime}: {e.BotUsername} - {e.Data}");

        private void Client_OnConnected(object sender, OnConnectedArgs e) => Console.WriteLine($"Connected to {e.AutoJoinChannel}");

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e) {
            Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            //client.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }
    }
}