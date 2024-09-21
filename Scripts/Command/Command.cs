namespace StoneBot.Scripts.Command {
    using Bot_Core.Models.EventSub;
    using System;
    using System.Threading.Tasks;

    internal class Command {
        public PermissionLevel PermissionLevel = PermissionLevel.Viewer;
        public int UseDelay = 1000;
        public DateTime LastUsed { get; private set; }
        public Func<ChannelChatMessageEvent, PermissionLevel, Task> UseAction;

        public bool IsReadyToUse => DateTime.Now > LastUsed.AddMilliseconds(UseDelay);

        public Command(Func<ChannelChatMessageEvent, PermissionLevel, Task> useAction) => UseAction = useAction;

        public virtual async Task<bool> Use(ChannelChatMessageEvent messageEvent) {
            if (!IsReadyToUse) {
                return false;
            }

            var userPermissionLevel = await Permission.GetHighest(messageEvent.ChatterUserId);
            if (userPermissionLevel is null || userPermissionLevel < PermissionLevel) {
                return false;
            }

            LastUsed = DateTime.Now;
            await UseAction(messageEvent, (PermissionLevel)userPermissionLevel);
            return true;
        }
    }

    internal class TogglableCommand : Command {
        public bool IsEnabled = true;

        public TogglableCommand(Func<ChannelChatMessageEvent, PermissionLevel, Task> useAction) : base(useAction) { }

        public override async Task<bool> Use(ChannelChatMessageEvent messageEvent) => IsEnabled && await base.Use(messageEvent);
    }
}
