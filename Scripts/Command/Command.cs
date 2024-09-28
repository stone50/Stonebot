namespace StoneBot.Scripts.Command {
    using Bot_Core.Models.EventSub;
    using System;
    using System.Threading.Tasks;

    internal class Command {
        public event EventHandler<PermissionLevel> PermissionLevelChanged = delegate { };
        public event EventHandler<int> UseDelayChanged = delegate { };

        public string Keyword { get; private set; }
        public PermissionLevel PermissionLevel { get => permissionLevel; set => SetPermissionLevel(value); }
        public int UseDelay { get => useDelay; set => SetUseDelay(value); }
        public DateTime LastUsed { get; private set; } = DateTime.Now;
        public Func<ChannelChatMessageEvent, PermissionLevel, Task> UseAction;

        public bool IsReadyToUse => DateTime.Now > LastUsed.AddMilliseconds(UseDelay);

        public Command(string keyword, Func<ChannelChatMessageEvent, PermissionLevel, Task> useAction) {
            Keyword = keyword;
            UseAction = useAction;
        }

        public virtual async Task<bool> Use(ChannelChatMessageEvent messageEvent) {
            Logger.Info($"Using command {Keyword}.");
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
            Logger.Info($"Setting permission level of command {Keyword}.");
            this.permissionLevel = permissionLevel;
            Util.InvokeDeferred(PermissionLevelChanged, PermissionLevel);
        }

        public void SetUseDelay(int useDelay) {
            Logger.Info($"Setting use delay of command {Keyword}.");
            this.useDelay = useDelay;
            Util.InvokeDeferred(UseDelayChanged, UseDelay);
        }

        private PermissionLevel permissionLevel = PermissionLevel.Viewer;
        private int useDelay = 1000;
    }

    internal class TogglableCommand : Command {
        public event EventHandler<bool> IsEnabledChanged = delegate { };

        private bool isEnabled = true;
        public bool IsEnabled { get => isEnabled; set => SetIsEnabled(value); }

        public TogglableCommand(string keyword, Func<ChannelChatMessageEvent, PermissionLevel, Task> useAction) : base(keyword, useAction) { }

        public override async Task<bool> Use(ChannelChatMessageEvent messageEvent) => IsEnabled && await base.Use(messageEvent);

        public void SetIsEnabled(bool isEnabled) {
            Logger.Info($"Setting is enabled of command {Keyword}.");
            this.isEnabled = isEnabled;
            Util.InvokeDeferred(IsEnabledChanged, IsEnabled);
        }
    }
}
