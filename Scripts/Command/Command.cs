namespace Stonebot.Scripts.Command {
    using Bot_Core.Models.EventSub;
    using System;
    using System.Threading.Tasks;

    internal class Command(string keyword, Func<ChannelChatMessageEvent, Task> useAction) {
        public event EventHandler<PermissionLevel> PermissionLevelChanged = delegate { };
        public event EventHandler<int> UseDelayChanged = delegate { };

        public string Keyword { get; private set; } = keyword;
        public PermissionLevel PermissionLevel { get => permissionLevel; set => SetPermissionLevel(value); }
        public int UseDelay { get => useDelay; set => SetUseDelay(value); }
        public DateTime LastUsed { get; private set; } = DateTime.Now;
        public Func<ChannelChatMessageEvent, Task> UseAction = useAction;

        public bool IsReadyToUse => DateTime.Now > LastUsed.AddMilliseconds(UseDelay);

        public virtual async Task<bool?> Use(ChannelChatMessageEvent messageEvent) {
            Logger.Info($"Using command {Keyword}.");

            if (!IsReadyToUse) {
                return false;
            }

            var userPermissionLevel = await Permission.GetHighest(messageEvent.ChatterUserId);
            if (userPermissionLevel is null) {
                Logger.Warning($"Could not use command {Keyword} because permission get highest attempt failed. Message event chatter user id: {messageEvent.ChatterUserId}.");
                return null;
            }

            if (userPermissionLevel < PermissionLevel) {
                return false;
            }

            await UseAction(messageEvent);
            LastUsed = DateTime.Now;
            return true;
        }

        public void SetPermissionLevel(PermissionLevel permissionLevel) {
            Logger.Info($"Setting permission level of command {Keyword}. Permission level: {permissionLevel}.");

            this.permissionLevel = permissionLevel;
            Util.InvokeDeferred(PermissionLevelChanged, PermissionLevel);
        }

        public void SetUseDelay(int useDelay) {
            Logger.Info($"Setting use delay of command {Keyword}. Use delay: {useDelay}.");

            this.useDelay = useDelay;
            Util.InvokeDeferred(UseDelayChanged, UseDelay);
        }

        private PermissionLevel permissionLevel = PermissionLevel.Viewer;
        private int useDelay = 1000;
    }

    internal class TogglableCommand(string keyword, Func<ChannelChatMessageEvent, Task> useAction) : Command(keyword, useAction) {
        public event EventHandler<bool> IsEnabledChanged = delegate { };

        private bool isEnabled = true;
        public bool IsEnabled { get => isEnabled; set => SetIsEnabled(value); }

        public override async Task<bool?> Use(ChannelChatMessageEvent messageEvent) => IsEnabled ? await base.Use(messageEvent) : false;

        public void SetIsEnabled(bool isEnabled) {
            Logger.Info($"Setting is enabled of command {Keyword}. Is enabled: {isEnabled}.");

            this.isEnabled = isEnabled;
            Util.InvokeDeferred(IsEnabledChanged, IsEnabled);
        }
    }
}
