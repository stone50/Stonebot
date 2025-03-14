﻿namespace Stonebot.Scripts.Command {
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Core_Interface;
    using Message;
    using System;
    using System.Threading.Tasks;
    using Timer;

    internal static class UseActions {
        public static async Task Commands(ChannelChatMessageEvent messageEvent, PermissionLevel __) => await Chat.Send($"{messageEvent.ChatterUserName} https://github.com/stone50/Stone-Bot/wiki/Commands");

        public static async Task EnableCommand(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return;
            }

            var command = CommandHandler.GetCommand(messageParams[1]);
            if (command is not TogglableCommand togglableCommand) {
                return;
            }

            togglableCommand.IsEnabled = true;
        }

        public static async Task DisableCommand(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return;
            }

            var command = CommandHandler.GetCommand(messageParams[1]);
            if (command is not TogglableCommand togglableCommand) {
                return;
            }

            togglableCommand.IsEnabled = false;
        }

        public static async Task EnableMessage(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return;
            }

            var message = MessageHandler.GetMessage(messageParams[1]);
            if (message is null) {
                return;
            }

            message.IsEnabled = true;
        }

        public static async Task DisableMessage(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return;
            }

            var message = MessageHandler.GetMessage(messageParams[1]);
            if (message is null) {
                return;
            }

            message.IsEnabled = false;
        }

        public static async Task EnableTimer(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return;
            }

            var timer = TimerManager.GetTimer(messageParams[1]);
            if (timer is null) {
                return;
            }

            timer.IsEnabled = true;
        }

        public static async Task DisableTimer(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return;
            }

            var timer = TimerManager.GetTimer(messageParams[1]);
            if (timer is null) {
                return;
            }

            timer.IsEnabled = false;
        }

        public static async Task Quote(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            var customData = await AppCache.Data.Get();
            if (customData is null) {
                return;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                return;
            }

            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return;
            }

            if (!int.TryParse(messageParams[1], out var quoteIndex)) {
                return;
            }

            if (quoteIndex < 0 || quoteIndex >= customData.Quotes.Count) {
                return;
            }

            _ = await Chat.Send($"[{quoteIndex}] \"{customData.Quotes[quoteIndex]}\" -{broadcaster.UserName}");
        }

        public static async Task AddQuote(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            var customData = await AppCache.Data.Get();
            if (customData is null) {
                return;
            }

            var text = messageEvent.Message.Text;
            var paramIndex = text.IndexOf(' ');
            if (paramIndex == -1) {
                return;
            }

            customData.Quotes.Add(text[(paramIndex + 1)..]);
        }

        public static async Task DeleteQuote(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            var customData = await AppCache.Data.Get();
            if (customData is null) {
                return;
            }

            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return;
            }

            if (!int.TryParse(messageParams[1], out var quoteIndex)) {
                return;
            }

            if (quoteIndex < 0 || quoteIndex >= customData.Quotes.Count) {
                return;
            }

            customData.Quotes.RemoveAt(quoteIndex);
        }

        public static async Task EditQuote(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            var customData = await AppCache.Data.Get();
            if (customData is null) {
                return;
            }

            var text = messageEvent.Message.Text;
            var indexOfFirstSpace = text.IndexOf(' ');
            if (indexOfFirstSpace == -1) {
                return;
            }

            var indexOfSecondSpace = text.IndexOf(' ', indexOfFirstSpace + 1);
            if (indexOfSecondSpace == -1) {
                return;
            }

            var quoteIndexString = text.Substring(indexOfFirstSpace + 1, indexOfSecondSpace - indexOfFirstSpace - 1);
            if (!int.TryParse(quoteIndexString, out var quoteIndex)) {
                return;
            }

            if (quoteIndex < 0 || quoteIndex >= customData.Quotes.Count) {
                return;
            }

            customData.Quotes[quoteIndex] = text[(indexOfSecondSpace + 1)..];
        }

        public static async Task Feed(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            var customData = await AppCache.Data.Get();
            if (customData is null) {
                return;
            }

            var feedCommand = CommandHandler.GetCommand("feed");
            if (feedCommand is null) {
                return;
            }

            var secondsSinceLastUse = (DateTime.Now - feedCommand.LastUsed).TotalSeconds;
            if (new Random().Next(Math.Min((int)(secondsSinceLastUse * 2d), 100)) == 0) {
                customData.FeedCount = 0;
                _ = await Chat.Send($"popCat BARF2 BARF3 {messageEvent.ChatterUserName}, you fed the cat too many crayons!");
                return;
            }

            customData.FeedCount++;
            if (customData.FeedCount > customData.FeedRecord) {
                customData.FeedRecord = customData.FeedCount;
                customData.FeedRecordHolder = messageEvent.ChatterUserName;
            }

            _ = await Chat.Send($"popCat Crayon The cat has been fed {customData.FeedCount} time{(customData.FeedCount > 1 ? "s" : "")} in a row.");
        }

        public static async Task FeedRecord(ChannelChatMessageEvent __, PermissionLevel ___) {
            var customData = await AppCache.Data.Get();
            if (customData is null) {
                return;
            }

            _ = await Chat.Send($"The record is {customData.FeedRecord}, last fed by {customData.FeedRecordHolder}.");
        }

        public static async Task Hug(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            if (new Random().Next(10) == 0) {
                _ = await Chat.Send($"pedroJAM {messageEvent.ChatterUserName} pedroJAM");
                return;
            }

            _ = await Chat.Send($"catKISS {messageEvent.ChatterUserName} catKISS");
        }

        public static async Task Lurk(ChannelChatMessageEvent messageEvent, PermissionLevel __) => _ = await Chat.Send($"{messageEvent.ChatterUserName}, thank you for your presence!");

        public static async Task Discord(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            var customData = await AppCache.Data.Get();
            if (customData is null) {
                return;
            }

            _ = Chat.Send($"{messageEvent.ChatterUserName} {customData.DiscordInvite}");
        }

        public static async Task YouTube(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            var customData = await AppCache.Data.Get();
            if (customData is null) {
                return;
            }

            _ = Chat.Send($"{messageEvent.ChatterUserName} {customData.YouTubeLink}");
        }
    }
}
