namespace StoneBot.Scripts.Message {
    using Bot_Core.Models.EventSub;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal static class MessageHandler {
        public static Dictionary<string, Message> Messages = new() {
            { "divorce", new(new("divorce", RegexOptions.IgnoreCase), UseActions.Divorce) },
            { "ihype", new(new("^\\s*i\\s*$", RegexOptions.IgnoreCase), UseActions.IHype) },
            { "mugmoment", new(new("mug", RegexOptions.IgnoreCase), UseActions.MugMoment) },
            { "peace", new(new("peace", RegexOptions.IgnoreCase), UseActions.Peace) },
            { "stonebot", new(new("stonebot", RegexOptions.IgnoreCase), UseActions.Stonebot) },
            { "failedcommand", new(new("^!"), UseActions.FailedCommand) },
        };

        public static async Task<bool> Handle(ChannelChatMessageEvent messageEvent) {
            foreach (var message in Messages) {
                if (await message.Value.Use(messageEvent)) {
                    return true;
                }
            }

            return false;
        }
    }
}
