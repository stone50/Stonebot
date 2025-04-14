namespace Stonebot.Scripts.Command {
    using Bot_Core.Models.EventSub;
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;

    internal class Command {
        public event EventHandler<PermissionLevel> PermissionLevelChanged = delegate { };
        public event EventHandler<int> UseDelayChanged = delegate { };

        public string Keyword { get; private set; }
        public PermissionLevel PermissionLevel { get => permissionLevel; set => SetPermissionLevel(value); }
        public int UseDelay { get => useDelay; set => SetUseDelay(value); }
        public DateTime LastUsed { get; private set; } = DateTime.Now;
        public Func<ChannelChatMessageEvent, Task<bool>> UseAction;

        public bool IsReadyToUse => DateTime.Now > LastUsed.AddMilliseconds(UseDelay);

        public Command(string keyword, Func<ChannelChatMessageEvent, Task<bool>> useAction) {
            Logger.Info($"{nameof(Command)} | Constructor\n{nameof(keyword)}: {keyword}");

            Keyword = keyword;
            UseAction = useAction;
        }

        public virtual async Task<bool?> Use(ChannelChatMessageEvent messageEvent) {
            var logPrefix = $"{nameof(Command)} | {nameof(Use)}";
            Logger.Info($"{logPrefix}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            if (!IsReadyToUse) {
                return false;
            }

            var userPermissionLevel = await Permission.GetHighest(messageEvent.ChatterUserId);
            if (userPermissionLevel is null) {
                Logger.Warning($"{logPrefix} | {nameof(Permission.GetHighest)} result is null.");
                return null;
            }

            if (userPermissionLevel < PermissionLevel) {
                return false;
            }

            if (!await UseAction(messageEvent)) {
                Logger.Warning($"{logPrefix} | {nameof(UseAction)} result is false.");
                return null;
            }

            LastUsed = DateTime.Now;
            return true;
        }

        public void SetPermissionLevel(PermissionLevel permissionLevel) {
            Logger.Info($"{nameof(Command)} | {nameof(SetPermissionLevel)}\n{nameof(permissionLevel)}: {permissionLevel}");

            this.permissionLevel = permissionLevel;
            Util.InvokeDeferred(PermissionLevelChanged, PermissionLevel);
        }

        public void SetUseDelay(int useDelay) {
            Logger.Info($"{nameof(Command)} | {nameof(SetUseDelay)}\n{nameof(useDelay)}: {useDelay}");

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

        public TogglableCommand(string keyword, Func<ChannelChatMessageEvent, Task<bool>> useAction) : base(keyword, useAction) => Logger.Info($"{nameof(TogglableCommand)} | Constructor\n{nameof(keyword)}: {keyword}");

        public override async Task<bool?> Use(ChannelChatMessageEvent messageEvent) {
            Logger.Info($"{nameof(TogglableCommand)} | {nameof(Use)}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            return IsEnabled ? await base.Use(messageEvent) : false;
        }

        public void SetIsEnabled(bool isEnabled) {
            Logger.Info($"{nameof(TogglableCommand)} | {nameof(SetIsEnabled)}\n{nameof(isEnabled)}: {isEnabled}");

            this.isEnabled = isEnabled;
            Util.InvokeDeferred(IsEnabledChanged, IsEnabled);
        }
    }
}
