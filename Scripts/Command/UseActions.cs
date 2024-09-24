namespace StoneBot.Scripts.Command {
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

        // TODO: add commands:
        // addquote
        // deletequote
        // editquote
        // quote

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

        // TODO: add commands:
        // feed

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
