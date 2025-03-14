namespace Stonebot.Scripts.Message {
    using Bot_Core.Models.EventSub;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal static partial class MessageHandler {
        public static Message[] Messages = [
            new("failedcommand", FailedCommandRegex(), UseActions.FailedCommand),
            new("divorce", DivorceRegex(), UseActions.Divorce),
            new("ihype", IHypeRegex(), UseActions.IHype),
            new("mugmoment", MugMomentRegex(), UseActions.MugMoment),
            new("peace", PeaceRegex(), UseActions.Peace),
            new("stonebot", StonebotRegex(), UseActions.Stonebot)
        ];

        public static async Task<bool> Handle(ChannelChatMessageEvent messageEvent) {
            Logger.Info("Handling message event as message.");
            foreach (var message in Messages) {
                if (await message.Use(messageEvent)) {
                    return true;
                }
            }

            return false;
        }

        public static Message? GetMessage(string keyword) => Messages.FirstOrDefault(message => message.Keyword == keyword);

        [GeneratedRegex("^!")]
        private static partial Regex FailedCommandRegex();

        [GeneratedRegex("divorce", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex DivorceRegex();

        [GeneratedRegex("^\\s*i\\s*$", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex IHypeRegex();

        [GeneratedRegex("mug", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex MugMomentRegex();

        [GeneratedRegex("peace", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex PeaceRegex();

        [GeneratedRegex("stonebot", RegexOptions.IgnoreCase, "en-US")]
        private static partial Regex StonebotRegex();
    }
}
