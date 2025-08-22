namespace Stonebot.Scripting {
    using Models.EventSubMessages;
    using System.Collections.Generic;

    internal static class CommandManager {
        public static readonly List<Command> Commands = [];

        public static void Init() {
            _ = Directory.CreateDirectory(Constants.CommandScriptsPath);
            if (File.Exists(Constants.CommandManagerFilePath)) {
                Load();
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
            using var writer = new BinaryWriter(File.Open(Constants.CommandManagerFilePath, FileMode.Create));
            writer.Write(Commands.Count);
            foreach (var command in Commands) {
                writer.Write(command.Name);
                writer.Write(command.Aliases.Count);
                foreach (var alias in command.Aliases) {
                    writer.Write(alias);
                }

                writer.Write(command.Enabled);
                writer.Write((int)command.PermissionLevel);
                writer.Write(command.CooldownSeconds);
            }
        }

        private static void Load() {
            using var reader = new BinaryReader(File.Open(Constants.CommandManagerFilePath, FileMode.Open));
            var numCommands = reader.ReadInt32();
            for (var c = 0; c < numCommands; ++c) {
                var name = reader.ReadString();
                var numAliases = reader.ReadInt32();
                var aliases = new string[numAliases];
                for (var a = 0; a < numAliases; ++a) {
                    aliases[a] = reader.ReadString();
                }

                var enabled = reader.ReadBoolean();
                var permissionLevel = (UserPermission.Level)reader.ReadInt32();
                var cooldownSeconds = reader.ReadInt32();
                Commands.Add(new(
                    name,
                    aliases,
                    enabled,
                    permissionLevel,
                    cooldownSeconds
                ));
            }
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
