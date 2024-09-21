namespace StoneBot.Scripts.Command {
    using Bot_Core.Models.EventSub;
    using Core_Interface;
    using System;
    using System.Threading.Tasks;

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

            var commandKeyword = messageParams[1];
            if (commandKeyword is "enablecommand" or "disablecommand") {
                return;
            }

            if (!CommandHandler.Commands.TryGetValue(commandKeyword, out var command)) {
                return;
            }

            command.IsEnabled = true;
        }

        public static async Task DisableCommand(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            await Task.Yield();
            var messageParams = messageEvent.Message.Text.Split(' ');
            if (messageParams.Length != 2) {
                return;
            }

            var commandKeyword = messageParams[1];
            if (commandKeyword is "enablecommand" or "disablecommand") {
                return;
            }

            if (!CommandHandler.Commands.TryGetValue(commandKeyword, out var command)) {
                return;
            }

            command.IsEnabled = false;
        }

        // TODO: add commands:
        // enablemessage
        // disablemessage
        // enabletimer
        // disabletimer
        // feed

        public static async Task Hug(ChannelChatMessageEvent messageEvent, PermissionLevel __) {
            if (new Random().Next(10) == 0) {
                _ = await Chat.Send($"RaccAttack Oh no! A raccoon stole the hug, {messageEvent.ChatterUserName}!");
                return;
            }

            _ = await Chat.Send($"catKISS {messageEvent.ChatterUserName}, thank you for hugging the cat!");
        }

        public static async Task Lurk(ChannelChatMessageEvent messageEvent, PermissionLevel __) => _ = await Chat.Send($"{messageEvent.BroadcasterUserName}, thank you for your presence!");
    }
}
