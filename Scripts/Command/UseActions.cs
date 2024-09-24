namespace StoneBot.Scripts.Command {
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using Core_Interface;
    using Message;
    using System;
    using System.Threading.Tasks;
    using Timer;

    internal static class UseActions {
        public static async Task Commands(ChannelChatMessageEvent _, PermissionLevel __) =>
            // TODO: create a wiki page on github with a list of the commands, then send the link in chat
            await Chat.Send("Command coming soon!");

        public static async Task EnableCommand(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return;
            }

            if (!CommandHandler.Commands.TryGetValue(messageParams[1], out var command)) {
                return;
            }

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

            if (!CommandHandler.Commands.TryGetValue(messageParams[1], out var command)) {
                return;
            }

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

            if (!MessageHandler.Messages.TryGetValue(messageParams[1], out var message)) {
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

            if (!MessageHandler.Messages.TryGetValue(messageParams[1], out var message)) {
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

            if (!TimerManager.Timers.TryGetValue(messageParams[1], out var timer)) {
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

            if (!TimerManager.Timers.TryGetValue(messageParams[1], out var timer)) {
                return;
            }

            timer.IsEnabled = false;
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

        public static async Task Feed(ChannelChatMessageEvent __, PermissionLevel ___) {
            var customData = await AppCache.Data.Get();
            if (customData is null) {
                return;
            }

            customData.FeedCount++;
            _ = await Chat.Send($"popCat Crayon The cat has been fed {customData.FeedCount} time{(customData.FeedCount > 1 ? "s" : "")}.");
        }

        public static async Task Hug(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            if (new Random().Next(10) == 0) {
                _ = await Chat.Send($"pedroJAM {messageEvent.ChatterUserName} pedroJAM");
                return;
            }

            _ = await Chat.Send($"catKISS {messageEvent.ChatterUserName} catKISS");
        }

        public static async Task Lurk(ChannelChatMessageEvent messageEvent, PermissionLevel __) => _ = await Chat.Send($"{messageEvent.BroadcasterUserName}, thank you for your presence!");
    }
}
