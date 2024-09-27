namespace StoneBot.Scripts.Command {
    using Bot_Core.Models.EventSub;
    using Godot;
    using System;
    using System.Threading.Tasks;

    internal class Command {
        public event EventHandler<PermissionLevel> PermissionLevelChanged = delegate { };
        public event EventHandler<int> UseDelayChanged = delegate { };

        public string Keyword { get; private set; }
        private PermissionLevel permissionLevel = PermissionLevel.Viewer;
        public PermissionLevel PermissionLevel { get => permissionLevel; set => SetPermissionLevel(value); }
        private int useDelay = 1000;
        public int UseDelay { get => useDelay; set => SetUseDelay(value); }
        public DateTime LastUsed { get; private set; } = DateTime.Now;
        public Func<ChannelChatMessageEvent, PermissionLevel, Task> UseAction;

        public bool IsReadyToUse => DateTime.Now > LastUsed.AddMilliseconds(UseDelay);

        public Command(string keyword, Func<ChannelChatMessageEvent, PermissionLevel, Task> useAction) {
            Keyword = keyword;
            UseAction = useAction;
        }

        public virtual async Task<bool> Use(ChannelChatMessageEvent messageEvent) {
            if (!IsReadyToUse) {
                return false;
            }

            var userPermissionLevel = await Permission.GetHighest(messageEvent.ChatterUserId);
            if (userPermissionLevel is null || userPermissionLevel < PermissionLevel) {
                return false;
            }

            await UseAction(messageEvent, (PermissionLevel)userPermissionLevel);
            LastUsed = DateTime.Now;
            return true;
        }

        public void SetPermissionLevel(PermissionLevel permissionLevel) {
            this.permissionLevel = permissionLevel;
            Callable.From(() => PermissionLevelChanged.Invoke(this, PermissionLevel)).CallDeferred();
        }

        public void SetUseDelay(int useDelay) {
            this.useDelay = useDelay;
            Callable.From(() => UseDelayChanged.Invoke(this, UseDelay)).CallDeferred();
        }
    }

    internal class TogglableCommand : Command {
        public event EventHandler<bool> IsEnabledChanged = delegate { };

        private bool isEnabled = true;
        public bool IsEnabled { get => isEnabled; set => SetIsEnabled(value); }

        public TogglableCommand(string keyword, Func<ChannelChatMessageEvent, PermissionLevel, Task> useAction) : base(keyword, useAction) { }

        public override async Task<bool> Use(ChannelChatMessageEvent messageEvent) => IsEnabled && await base.Use(messageEvent);

        public void SetIsEnabled(bool isEnabled) {
            this.isEnabled = isEnabled;
            Callable.From(() => IsEnabledChanged.Invoke(this, IsEnabled)).CallDeferred();
        }
    }
}
