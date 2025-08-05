namespace Stonebot {
    using Models.Data;
    using Models.EventSubMessages;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    internal static class CommandManager {
        public static readonly List<Command> Commands = [];

        public static void Init() {
            _ = Directory.CreateDirectory(Constants.CommandScriptsPath);
            if (!File.Exists(Constants.CommandManagerFilePath)) {
                return;
            }

            var fileContents = File.ReadAllText(Constants.CommandManagerFilePath);
            var data = JsonSerializer.Deserialize(fileContents, JsonContext.Default.CommandManagerData);
            foreach (var commandData in data.Commands) {
                Utils.TryElseWarn(() => Commands.Add(new(
                    commandData.Name,
                    commandData.Aliases,
                    commandData.Enabled,
                    commandData.PermissionLevel,
                    commandData.CooldownMillis
                )));
            }
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
            var contents = JsonSerializer.Serialize(new CommandManagerData() {
                Commands = [.. Commands.Select(command => new CommandManagerDataCommand() {
                    Name = command.Name,
                    Aliases = [.. command.Aliases],
                    Enabled = command.Enabled,
                    PermissionLevel = command.PermissionLevel,
                    CooldownMillis = command.CooldownMillis,
                })],
            }, JsonContext.Default.CommandManagerData);
            File.WriteAllText(Constants.CommandManagerFilePath, contents);
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
