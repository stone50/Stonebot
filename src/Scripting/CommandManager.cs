namespace Stonebot.Scripting {
    using Models.EventSubMessages;
    using System.Collections.Generic;

    internal static class CommandManager {
        public static readonly List<Command> Commands = [];

        public static void Init() {
            _ = Directory.CreateDirectory(Constants.CommandScriptsPath);
            Load();
        }

        public static bool TryUseCommand(EventSubNotificationMessagePayloadEvent channelChatMessageEvent) {
            var keyword = GetCommandKeyword(channelChatMessageEvent.Message.Text);
            foreach (var command in Commands) {
                if (command.Name != keyword) {
                    continue;
                }

                _ = command.TryProc(channelChatMessageEvent);
                return true;
            }

            foreach (var command in Commands) {
                if (!command.Aliases.Contains(keyword)) {
                    continue;
                }

                _ = command.TryProc(channelChatMessageEvent);
                return true;
            }

            return false;
        }



        public static void Save() {
            // TODO
        }

        private static void Load() {
            // TODO
        }

        private static string GetCommandKeyword(string text) {
            var keywordEndIndex = text.Length - 1;
            for (var i = 1; i < text.Length; ++i) {
                if (!char.IsLetter(text[i])) {
                    keywordEndIndex = i - 1;
                    break;
                }
            }

            return text.Substring(1, keywordEndIndex);
        }
    }
}
