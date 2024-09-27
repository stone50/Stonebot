namespace StoneBot.Scripts.Message {
    using Bot_Core.Models.EventSub;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal static class MessageHandler {
        public static Message[] Messages = new Message[] {
            new("failedcommand", new("^!"), UseActions.FailedCommand),
            new("divorce", new("divorce", RegexOptions.IgnoreCase), UseActions.Divorce),
            new("ihype", new("^\\s*i\\s*$", RegexOptions.IgnoreCase), UseActions.IHype),
            new("mugmoment", new("mug", RegexOptions.IgnoreCase), UseActions.MugMoment),
            new("peace", new("peace", RegexOptions.IgnoreCase), UseActions.Peace),
            new("stonebot", new("stonebot", RegexOptions.IgnoreCase), UseActions.Stonebot)
        };

        public static async Task<bool> Handle(ChannelChatMessageEvent messageEvent) {
            foreach (var message in Messages) {
                if (await message.Use(messageEvent)) {
                    return true;
                }
            }

            return false;
        }

        public static Message? GetMessage(string keyword) => Messages.FirstOrDefault(message => message.Keyword == keyword);
    }
}
