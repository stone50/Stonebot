namespace Stonebot.Scripts.Command {
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Core_Interface;
    using Message;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Timer;

    internal static class UseActions {
        public static async Task<bool> Commands(ChannelChatMessageEvent messageEvent) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(Commands)}";
            Logger.Info($"{logPrefix}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            if (!await Chat.Send($"{messageEvent.ChatterUserName} https://github.com/stone50/Stone-Bot/wiki/Commands")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> EnableCommand(ChannelChatMessageEvent messageEvent) {
            Logger.Info($"{nameof(UseActions)} | {nameof(EnableCommand)}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

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
            Logger.Info($"{nameof(UseActions)} | {nameof(DisableCommand)}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

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
            Logger.Info($"{nameof(UseActions)} | {nameof(EnableMessage)}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

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
            Logger.Info($"{nameof(UseActions)} | {nameof(DisableMessage)}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

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
            Logger.Info($"{nameof(UseActions)} | {nameof(EnableTimer)}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

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
            Logger.Info($"{nameof(UseActions)} | {nameof(DisableTimer)}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

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
            var logPrefix = $"{nameof(UseActions)} | {nameof(Quote)}";
            Logger.Info($"{logPrefix}\n{messageEvent}: {JsonSerializer.Serialize(messageEvent)}");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Data.Get)} result is null.");
                return false;
            }

            var broadcaster = await AppCache.Broadcaster.Get();
            if (broadcaster is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Broadcaster.Get)} result is null.");
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
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> AddQuote(ChannelChatMessageEvent messageEvent) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(AddQuote)}";
            Logger.Info($"{logPrefix}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Data.Get)} result is null.");
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
            var logPrefix = $"{nameof(UseActions)} | {nameof(DeleteQuote)}";
            Logger.Info($"{logPrefix}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Data.Get)} result is null.");
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
            var logPrefix = $"{nameof(UseActions)} | {nameof(EditQuote)}";
            Logger.Info($"{logPrefix}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Data.Get)} result is null.");
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
            var logPrefix = $"{nameof(UseActions)} | {nameof(Feed)}";
            Logger.Info($"{logPrefix}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Data.Get)} result is null.");
                return false;
            }

            var feedCommand = CommandHandler.GetCommand("feed");
            if (feedCommand is null) {
                Logger.Warning($"{logPrefix} | {nameof(CommandHandler.GetCommand)} result is null.");
                return false;
            }

            var secondsSinceLastUse = (DateTime.Now - feedCommand.LastUsed).TotalSeconds;
            if (new Random().Next(Math.Min((int)(secondsSinceLastUse * 2d), 100)) == 0) {
                customData.FeedCount = 0;
                if (!await Chat.Send($"popCat BARF2 BARF3 {messageEvent.ChatterUserName}, you fed the cat too many crayons!")) {
                    Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
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
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> FeedRecord(ChannelChatMessageEvent __) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(Feed)}";
            Logger.Info(logPrefix);

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Data.Get)} result is null.");
                return false;
            }

            if (!await Chat.Send($"The record is {customData.FeedRecord}, last fed by {customData.FeedRecordHolder}.")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Hug(ChannelChatMessageEvent messageEvent) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(Hug)}";
            Logger.Info($"{logPrefix}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            if (new Random().Next(10) == 0) {
                if (!await Chat.Send($"pedroJAM {messageEvent.ChatterUserName} pedroJAM")) {
                    Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                    return false;
                }

                return true;
            }

            if (!await Chat.Send($"catKISS {messageEvent.ChatterUserName} catKISS")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Lurk(ChannelChatMessageEvent messageEvent) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(Lurk)}";
            Logger.Info($"{logPrefix}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            if (!await Chat.Send($"{messageEvent.ChatterUserName}, thank you for your presence!")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Discord(ChannelChatMessageEvent messageEvent) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(Discord)}";
            Logger.Info($"{logPrefix}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Data.Get)} result is null.");
                return false;
            }

            if (!await Chat.Send($"{messageEvent.ChatterUserName} {customData.DiscordInvite}")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> YouTube(ChannelChatMessageEvent messageEvent) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(YouTube)}";
            Logger.Info($"{logPrefix}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            var customData = await AppCache.Data.Get();
            if (customData is null) {
                Logger.Warning($"{logPrefix} | {nameof(AppCache.Data.Get)} result is null.");
                return false;
            }

            if (!await Chat.Send($"{messageEvent.ChatterUserName} {customData.YouTubeLink}")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }
    }
}
