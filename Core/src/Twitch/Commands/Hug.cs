namespace StonebotCore.Twitch.Commands {
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;
    using TwitchLib.Client.Events;

    internal class Hug() : Command("hug") {
        internal override Task ExecuteAsync(
            OnChatCommandReceivedArgs args,
            ILogger<Interface.TwitchClientLog> logger,
            CancellationToken cancellationToken
        ) {
            var chatMessage = args.ChatMessage;
            // TODO: randomly steal hug using pedroJAM
            return ReplyAsync(chatMessage, $"catKISS {chatMessage.DisplayName} catKISS");
        }
    }
}
