namespace StoneBot.Scripts.Message {
    using Bot_Core.Models.EventSub;
    using Core_Interface;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal static class UseActions {
        public static async Task Divorce(ChannelChatMessageEvent _, PermissionLevel __, Match ___) => await Chat.Send("Hahaha");
        public static async Task FailedCommand(ChannelChatMessageEvent _, PermissionLevel __, Match ___) => await Chat.Send("FailFish");
        public static async Task IHype(ChannelChatMessageEvent _, PermissionLevel __, Match ___) => await Chat.Send("HYPE");
        public static async Task MugMoment(ChannelChatMessageEvent _, PermissionLevel __, Match ___) => await Chat.Send("MUG MOMENT");
        public static async Task Peace(ChannelChatMessageEvent _, PermissionLevel __, Match ___) => await Chat.Send("PEACE");
        public static async Task Stonebot(ChannelChatMessageEvent _, PermissionLevel __, Match ___) => await Chat.Send("MrDestructoid");
    }
}
