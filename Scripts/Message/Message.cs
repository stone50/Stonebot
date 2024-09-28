namespace StoneBot.Scripts.Message {
    using Bot_Core.Models.EventSub;
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal class Message {
        public event EventHandler<PermissionLevel> PermissionLevelChanged = delegate { };
        public event EventHandler<int> UseDelayChanged = delegate { };
        public event EventHandler<bool> IsEnabledChanged = delegate { };

        public string Keyword { get; private set; }
        public Regex Regex;
        public bool isEnabled = true;
        public bool IsEnabled { get => isEnabled; set => SetIsEnabled(value); }
        public PermissionLevel permissionLevel = PermissionLevel.Viewer;
        public PermissionLevel PermissionLevel { get => permissionLevel; set => SetPermissionLevel(value); }
        public int useDelay = 1000;
        public int UseDelay { get => useDelay; set => SetUseDelay(value); }
        public DateTime LastUsed { get; private set; }
        public Func<ChannelChatMessageEvent, PermissionLevel, Match, Task> UseAction;

        public bool IsReadyToUse => DateTime.Now > LastUsed.AddMilliseconds(UseDelay);

        public Message(string keyword, Regex regex, Func<ChannelChatMessageEvent, PermissionLevel, Match, Task> useAction) {
            Keyword = keyword;
            Regex = regex;
            UseAction = useAction;
        }

        public async Task<bool> Use(ChannelChatMessageEvent messageEvent) {
            Logger.Info($"Using message {Keyword}.");
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
            if (userPermissionLevel is null || userPermissionLevel < PermissionLevel) {
                return false;
            }

            LastUsed = DateTime.Now;
            await UseAction(messageEvent, (PermissionLevel)userPermissionLevel, match);
            return true;
        }

        public void SetIsEnabled(bool isEnabled) {
            Logger.Info($"Setting is enabled for message {Keyword}.");
            this.isEnabled = isEnabled;
            Util.InvokeDeferred(IsEnabledChanged, IsEnabled);
        }

        public void SetPermissionLevel(PermissionLevel permissionLevel) {
            Logger.Info($"Setting permission level for message {Keyword}.");
            this.permissionLevel = permissionLevel;
            Util.InvokeDeferred(PermissionLevelChanged, PermissionLevel);
        }

        public void SetUseDelay(int useDelay) {
            Logger.Info($"Setting use delay for message {Keyword}.");
            this.useDelay = useDelay;
            Util.InvokeDeferred(UseDelayChanged, UseDelay);
        }
    }
}
