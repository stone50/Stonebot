namespace Stonebot.Scripts.Command {
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Core_Interface;
    using Message;
    using System;
    using System.Threading.Tasks;
    using Timer;

    internal static class UseActions {
        public static async Task<bool> Commands(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using commands action.");

            if (!await Chat.Send($"{messageEvent.ChatterUserName} https://github.com/stone50/Stone-Bot/wiki/Commands")) {
                Logger.Warning("Could not use command action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> EnableCommand(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using enable command action.");

            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return true;
            }

            var command = CommandHandler.GetCommand(messageParams[1]);
            if (command is not TogglableCommand togglableCommand) {
                return true;
            }

            togglableCommand.IsEnabled = true;
            return true;
        }

        public static async Task<bool> DisableCommand(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using disable command action");

            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return true;
            }

            var command = CommandHandler.GetCommand(messageParams[1]);
            if (command is not TogglableCommand togglableCommand) {
                return true;
            }

            togglableCommand.IsEnabled = false;
            return true;
        }

        public static async Task<bool> EnableMessage(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using enable message action.");

            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return true;
            }

            var message = MessageHandler.GetMessage(messageParams[1]);
            if (message is null) {
                return true;
            }

            message.IsEnabled = true;
            return true;
        }

        public static async Task<bool> DisableMessage(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using disable message action.");

            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return true;
            }

            var message = MessageHandler.GetMessage(messageParams[1]);
            if (message is null) {
                return true;
            }

            message.IsEnabled = false;
            return true;
        }

        public static async Task<bool> EnableTimer(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using enable timer action.");

            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return true;
            }

            var timer = TimerManager.GetTimer(messageParams[1]);
            if (timer is null) {
                return true;
            }

            timer.IsEnabled = true;
            return true;
        }

        public static async Task<bool> DisableTimer(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using disable timer action.");

            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return true;
            }

            var timer = TimerManager.GetTimer(messageParams[1]);
            if (timer is null) {
                return true;
            }

            timer.IsEnabled = false;
            return true;
        }

        public static async Task<bool> Quote(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using quote action.");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning("Could not use quote action because data get attempt failed.");
                return false;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning("Could not use quote action because broadcaster get attempt failed.");
                return false;
            }

            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return true;
            }

            if (!int.TryParse(messageParams[1], out var quoteIndex)) {
                return true;
            }

            if (quoteIndex < 0 || quoteIndex >= customData.Quotes.Count) {
                return true;
            }

            if (!await Chat.Send($"[{quoteIndex}] \"{customData.Quotes[quoteIndex]}\" -{broadcaster.UserName}")) {
                Logger.Warning("Could not use quote action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> AddQuote(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using add quote action.");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning("Could not use add quote action because data get attempt failed.");
                return false;
            }

            var text = messageEvent.Message.Text;
            var paramIndex = text.IndexOf(' ');
            if (paramIndex == -1) {
                return true;
            }

            customData.Quotes.Add(text[(paramIndex + 1)..]);
            return true;
        }

        public static async Task<bool> DeleteQuote(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using delete quote action.");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning("Could not use delete quote action because data get attempt failed.");
                return false;
            }

            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return true;
            }

            if (!int.TryParse(messageParams[1], out var quoteIndex)) {
                return true;
            }

            if (quoteIndex < 0 || quoteIndex >= customData.Quotes.Count) {
                return true;
            }

            customData.Quotes.RemoveAt(quoteIndex);
            return true;
        }

        public static async Task<bool> EditQuote(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using edit quote action.");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning("Could not use edit quote action because data get attempt failed.");
                return false;
            }

            var text = messageEvent.Message.Text;
            var indexOfFirstSpace = text.IndexOf(' ');
            if (indexOfFirstSpace == -1) {
                return true;
            }

            var indexOfSecondSpace = text.IndexOf(' ', indexOfFirstSpace + 1);
            if (indexOfSecondSpace == -1) {
                return true;
            }

            var quoteIndexString = text.Substring(indexOfFirstSpace + 1, indexOfSecondSpace - indexOfFirstSpace - 1);
            if (!int.TryParse(quoteIndexString, out var quoteIndex)) {
                return true;
            }

            if (quoteIndex < 0 || quoteIndex >= customData.Quotes.Count) {
                return true;
            }

            customData.Quotes[quoteIndex] = text[(indexOfSecondSpace + 1)..];
            return true;
        }

        public static async Task<bool> Feed(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using feed action.");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning("Could not use feed action because data get attempt failed.");
                return false;
            }

            var feedCommand = CommandHandler.GetCommand("feed");
            if (feedCommand is null) {
                Logger.Warning("Could not use feed action because command handler get command attempt failed.");
                return false;
            }

            var secondsSinceLastUse = (DateTime.Now - feedCommand.LastUsed).TotalSeconds;
            if (new Random().Next(Math.Min((int)(secondsSinceLastUse * 2d), 100)) == 0) {
                customData.FeedCount = 0;
                if (!await Chat.Send($"popCat BARF2 BARF3 {messageEvent.ChatterUserName}, you fed the cat too many crayons!")) {
                    Logger.Warning("Could not use feed action because chat send attempt failed.");
                    return false;
                }

                return true;
            }

            customData.FeedCount++;
            if (customData.FeedCount > customData.FeedRecord) {
                customData.FeedRecord = customData.FeedCount;
                customData.FeedRecordHolder = messageEvent.ChatterUserName;
            }

            if (!await Chat.Send($"popCat Crayon The cat has been fed {customData.FeedCount} time{(customData.FeedCount > 1 ? "s" : "")} in a row.")) {
                Logger.Warning("Could not use feed action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> FeedRecord(ChannelChatMessageEvent __) {
            Logger.Info("Using feed record action.");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning("Could not use feed record action because data get attempt failed.");
                return false;
            }

            if (!await Chat.Send($"The record is {customData.FeedRecord}, last fed by {customData.FeedRecordHolder}.")) {
                Logger.Warning("Could not use feed record action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Hug(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using hug action.");

            if (new Random().Next(10) == 0) {
                if (!await Chat.Send($"pedroJAM {messageEvent.ChatterUserName} pedroJAM")) {
                    Logger.Warning("Could not use hug action because chat send attempt failed.");
                    return false;
                }

                return true;
            }

            if (!await Chat.Send($"catKISS {messageEvent.ChatterUserName} catKISS")) {
                Logger.Warning("Could not use hug action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Lurk(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using lurk action.");

            if (!await Chat.Send($"{messageEvent.ChatterUserName}, thank you for your presence!")) {
                Logger.Warning("Could not use lurk action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Discord(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using discord action.");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning("Could not use discord action because data get attempt failed.");
                return false;
            }

            if (!await Chat.Send($"{messageEvent.ChatterUserName} {customData.DiscordInvite}")) {
                Logger.Warning("Could not use discord action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> YouTube(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Using youtube action.");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning("Could not use youtube action because data get attempt failed.");
                return false;
            }

            if (!await Chat.Send($"{messageEvent.ChatterUserName} {customData.YouTubeLink}")) {
                Logger.Warning("Could not use youtube action because chat send attempt failed.");
                return false;
            }

            return true;
        }
    }
}
