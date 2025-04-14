namespace Stonebot.Scripts.Message {
    using Bot_Core.Models.EventSub;
    using System;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal class Message {
        public event EventHandler<PermissionLevel> PermissionLevelChanged = delegate { };
        public event EventHandler<int> UseDelayChanged = delegate { };
        public event EventHandler<bool> IsEnabledChanged = delegate { };

        public string Keyword { get; private set; }
        public Regex Regex;
        public bool IsEnabled { get => isEnabled; set => SetIsEnabled(value); }
        public PermissionLevel PermissionLevel { get => permissionLevel; set => SetPermissionLevel(value); }
        public int UseDelay { get => useDelay; set => SetUseDelay(value); }
        public DateTime LastUsed { get; private set; }
        public Func<ChannelChatMessageEvent, Task<bool>> UseAction;

        public bool IsReadyToUse => DateTime.Now > LastUsed.AddMilliseconds(UseDelay);

        public Message(string keyword, Regex regex, Func<ChannelChatMessageEvent, Task<bool>> useAction) {
            Logger.Info($"{nameof(Message)} | Constructor\n{nameof(keyword)}: {keyword}\n{nameof(regex)}: {regex}");

            Keyword = keyword;
            Regex = regex;
            UseAction = useAction;
        }

        public async Task<bool?> Use(ChannelChatMessageEvent messageEvent) {
            var logPrefix = $"{nameof(Message)} | {nameof(Use)}";
            Logger.Info($"{logPrefix}\n{nameof(messageEvent)}: {JsonSerializer.Serialize(messageEvent)}");

            if (!IsEnabled) {
                return false;
            }

            if (!IsReadyToUse) {
                return false;
            }

            var match = Regex.Match(messageEvent.Message.Text);
            if (!match.Success) {
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
                return false;
            }

            LastUsed = DateTime.Now;
            return true;
        }

        public void SetIsEnabled(bool isEnabled) {
            Logger.Info($"{nameof(Message)} | {nameof(SetIsEnabled)}\n{nameof(isEnabled)}: {isEnabled}");

            this.isEnabled = isEnabled;
            Util.InvokeDeferred(IsEnabledChanged, IsEnabled);
        }

        public void SetPermissionLevel(PermissionLevel permissionLevel) {
            Logger.Info($"{nameof(Message)} | {nameof(SetPermissionLevel)}\n{nameof(permissionLevel)}: {permissionLevel}");

            this.permissionLevel = permissionLevel;
            Util.InvokeDeferred(PermissionLevelChanged, PermissionLevel);
        }

        public void SetUseDelay(int useDelay) {
            Logger.Info($"{nameof(Message)} | {nameof(SetUseDelay)}\n{nameof(useDelay)}: {useDelay}");

            this.useDelay = useDelay;
            Util.InvokeDeferred(UseDelayChanged, UseDelay);
        }

        private bool isEnabled = true;
        private PermissionLevel permissionLevel = PermissionLevel.Viewer;
        public int useDelay = 1000;
    }
}
