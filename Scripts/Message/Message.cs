namespace StoneBot.Scripts.Message {
    using Bot_Core.Models.EventSub;
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    internal class Message {
        public Regex Regex;
        public bool IsEnabled = true;
        public PermissionLevel PermissionLevel = PermissionLevel.Viewer;
        public int UseDelay = 1000;
        public DateTime LastUsed { get; private set; }
        public Func<ChannelChatMessageEvent, PermissionLevel, Match, Task> UseAction;

        public bool IsReadyToUse => DateTime.Now > LastUsed.AddMilliseconds(UseDelay);

        public Message(Regex regex, Func<ChannelChatMessageEvent, PermissionLevel, Match, Task> useAction) {
            Regex = regex;
            UseAction = useAction;
        }

        public async Task<bool> Use(ChannelChatMessageEvent messageEvent) {
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
    }
}
