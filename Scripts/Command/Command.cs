namespace StoneBot.Scripts.Command {
    using Bot_Core.Models.EventSub;
    using System;
    using System.Threading.Tasks;

    internal class Command {
        public bool IsEnabled = true;
        public Permission.PermissionLevel PermissionLevel = Permission.PermissionLevel.Viewer;
        public int UseDelay = 1000;
        public DateTime LastUsed { get; private set; }
        public Func<ChannelChatMessageEvent, Task> UseAction;

        public bool IsReadyToUse => DateTime.Now < LastUsed.AddMilliseconds(UseDelay);

        public Command(Func<ChannelChatMessageEvent, Task> useAction) => UseAction = useAction;

        public async Task<bool> Use(ChannelChatMessageEvent messageEvent) {
            if (!IsEnabled) {
                return false;
            }

            if (!IsReadyToUse) {
                return false;
            }

            var userPermissionLevel = await Permission.GetHighest(messageEvent.ChatterUserId);
            if (userPermissionLevel is null || userPermissionLevel < PermissionLevel) {
                return false;
            }

            LastUsed = DateTime.Now;
            await UseAction(messageEvent);
            return true;
        }
    }
}
