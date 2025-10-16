namespace Stonebot.Scripting.Python {
    using Avalonia.Threading;
    using Models.EventSubMessages;
    using Resources;
    using Stonebot.UI;
    using System.Diagnostics.CodeAnalysis;
    using Twitch;

#pragma warning disable IDE1006 // Naming Styles
    public class ScriptInterface(EventSubNotificationMessagePayloadEvent channelChatMessageEvent, UserPermission.Level permissionLevel) {
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ScriptInterface))]
        public readonly MessageData message_data = new(channelChatMessageEvent);
        public readonly ChatterPermission chatter_permission = new(permissionLevel);

        public static void log(params object?[] messages) => Logger.Info(messages);

        public static void log_warn(params object?[] messages) => Logger.Warn(messages);

        public static void log_error(params object?[] messages) => Logger.Error(messages);

        public static ChatResponse chat(string message, string? reply_parent_message_id = null) => new(Chat.Send(message, reply_parent_message_id));

        public ChatResponse reply(string message) => chat(message, message_data.message_id);

        public static bool? is_command_enabled(string command_name_or_alias) => GetCommand(command_name_or_alias)?.Enabled;

        public static string[]? get_command_name_and_aliases(string command_name_or_alias) {
            var command = GetCommand(command_name_or_alias);
            return command == null ? null : [command.Name, .. command.Aliases];
        }

        public static void enable_command(string command_name_or_alias) {
            var command = GetCommand(command_name_or_alias);
            if (command == null) {
                return;
            }

            if (command.Enabled) {
                return;
            }

            command.Enabled = true;
            UpdateInteractionGrid();
        }

        public static void disable_command(string command_name_or_alias) {
            var command = GetCommand(command_name_or_alias);
            if (command == null) {
                return;
            }

            if (!command.Enabled) {
                return;
            }

            command.Enabled = false;
            UpdateInteractionGrid();
        }

        internal static void Init() {
            _ = Directory.CreateDirectory(Constants.ScriptsTypeHintsPackagePath);
            File.WriteAllText(Constants.ScriptsTypeHintsFilePath, Embedded.ScriptsTypeHintsPyi);
        }

        private static void UpdateInteractionGrid() => Dispatcher.UIThread.Invoke(InteractionGrid.Update);

        private static Command? GetCommand(string nameOrAlias) => CommandManager.Commands.Find(command => command.Name == nameOrAlias || command.Aliases.Contains(nameOrAlias));
    }
#pragma warning restore IDE1006 // Naming Styles
}
