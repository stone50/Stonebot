namespace Stonebot.Scripts.Message {
    using Bot_Core.Models.EventSub;
    using Core_Interface;
    using System.Threading.Tasks;

    internal static class UseActions {
        public static async Task<bool> Divorce(ChannelChatMessageEvent _) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(Divorce)}";
            Logger.Info(logPrefix);

            if (!await Chat.Send("Hahaha")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> FailedCommand(ChannelChatMessageEvent _) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(FailedCommand)}";
            Logger.Info(logPrefix);

            if (!await Chat.Send("FailFish")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> IHype(ChannelChatMessageEvent _) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(IHype)}";
            Logger.Info(logPrefix);

            if (!await Chat.Send("HYPE")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> MugMoment(ChannelChatMessageEvent _) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(MugMoment)}";
            Logger.Info(logPrefix);

            if (!await Chat.Send("MUG MOMENT")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Peace(ChannelChatMessageEvent _) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(Peace)}";
            Logger.Info(logPrefix);

            if (!await Chat.Send("PEACE")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Stonebot(ChannelChatMessageEvent _) {
            var logPrefix = $"{nameof(UseActions)} | {nameof(Stonebot)}";
            Logger.Info(logPrefix);

            if (!await Chat.Send("MrDestructoid")) {
                Logger.Warning($"{logPrefix} | {nameof(Chat.Send)} result is false.");
                return false;
            }

            return true;
        }
    }
}
