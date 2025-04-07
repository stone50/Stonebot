namespace Stonebot.Scripts.Message {
    using Bot_Core.Models.EventSub;
    using Core_Interface;
    using System.Threading.Tasks;

    internal static class UseActions {
        public static async Task<bool> Divorce(ChannelChatMessageEvent _) {
            Logger.Info("Using divorce action.");

            if (!await Chat.Send("Hahaha")) {
                Logger.Warning("Could not use divorce action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> FailedCommand(ChannelChatMessageEvent _) {
            Logger.Info("Using failed command action.");

            if (!await Chat.Send("FailFish")) {
                Logger.Warning("Could not use failed command action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> IHype(ChannelChatMessageEvent _) {
            Logger.Info("Using ihype action.");

            if (!await Chat.Send("HYPE")) {
                Logger.Warning("Could not use ihype action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> MugMoment(ChannelChatMessageEvent _) {
            Logger.Info("Using mug moment action.");

            if (!await Chat.Send("MUG MOMENT")) {
                Logger.Warning("Could not use mug moment action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Peace(ChannelChatMessageEvent _) {
            Logger.Info("Using peace action.");

            if (!await Chat.Send("PEACE")) {
                Logger.Warning("Could not use peace action because chat send attempt failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Stonebot(ChannelChatMessageEvent _) {
            Logger.Info("Using stonebot action.");

            if (!await Chat.Send("MrDestructoid")) {
                Logger.Warning("Could not use stonebot action because chat send attempt failed.");
                return false;
            }

            return true;
        }
    }
}
