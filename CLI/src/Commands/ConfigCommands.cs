namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;

    internal static class ConfigCommands {
        internal static ParentCommand GetConfigCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["config", "settings"],
            subCommands: [
                GetConfigLoadCommand(httpClient, portOption),
                GetConfigSetCommand(httpClient, portOption),
                GetConfigGetCommand(httpClient, portOption),
            ]
        );

        private static ChildCommand GetConfigLoadCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["load", "read"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendPostRequestAsync(httpClient, port, EndpointPaths.PostConfigLoad, null, cancellationToken));

        private static ChildCommand GetConfigSetCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["set", "assign"],
            options: [
                portOption,
                new StringOption(
                    aliases: ["--twitch-client-id", "--twitch-id"]
                ),
                new StringOption(
                    aliases: ["--twitch-client-secret", "--twitch-secret"]
                ),
                new StringOption(
                    aliases: ["--twitch-bot-username", "--twitch-user"]
                ),
                new StringOption(
                    aliases: ["--twitch-broadcaster-channel", "--twitch-channel"]
                )
            ],
            async (childCommand, optionMap, cancellationToken) => {
                if (!childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)) {
                    return error;
                }

                var valueJsonParts = new List<string>();
                if (childCommand.TryParseOptionValue<string>(1, optionMap, out var twitchClientId, out error)) {
                    valueJsonParts.Add($"\"{ConfigValueNames.TwitchClientId}\":\"{twitchClientId}\"");
                } else if (error.Code != ErrorCode.MissingOption) {
                    return error;
                }

                if (childCommand.TryParseOptionValue<string>(2, optionMap, out var twitchClientSecret, out error)) {
                    valueJsonParts.Add($"\"{ConfigValueNames.TwitchClientSecret}\":\"{twitchClientSecret}\"");
                } else if (error.Code != ErrorCode.MissingOption) {
                    return error;
                }

                if (childCommand.TryParseOptionValue<string>(3, optionMap, out var twitchBotUsername, out error)) {
                    valueJsonParts.Add($"\"{ConfigValueNames.TwitchBotUsername}\":\"{twitchBotUsername}\"");
                } else if (error.Code != ErrorCode.MissingOption) {
                    return error;
                }

                if (childCommand.TryParseOptionValue<string>(4, optionMap, out var twitchBroadcasterChannel, out error)) {
                    valueJsonParts.Add($"\"{ConfigValueNames.TwitchBroadcasterChannel}\":\"{twitchBroadcasterChannel}\"");
                } else if (error.Code != ErrorCode.MissingOption) {
                    return error;
                }

                return await Utils.SendPatchRequestAsync(httpClient, port, EndpointPaths.PatchConfigSet, $"{{{string.Join(',', valueJsonParts)}}}", cancellationToken);
            }
        );

        private static ChildCommand GetConfigGetCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["get"],
            options: [
                portOption,
                new OptionalBoolOption(
                    aliases: ["--twitch-client-id", "--twitch-id"],
                    defaultValue: false
                ),
                new OptionalBoolOption(
                    aliases: ["--twitch-client-secret", "--twitch-secret"],
                    defaultValue: false
                ),
                new OptionalBoolOption(
                    aliases: ["--twitch-bot-username", "--twitch-user"],
                    defaultValue: false
                ),
                new OptionalBoolOption(
                    aliases: ["--twitch-broadcaster-channel", "--twitch-channel"],
                    defaultValue: false
                )
            ],
            async (childCommand, optionMap, cancellationToken) => {
                if (!childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)) {
                    return error;
                }

                var queryParams = new List<string>();
                if (!childCommand.TryParseOptionValue<bool>(1, optionMap, out var shouldGetTwitchClientId, out error)) {
                    return error;
                }

                if (shouldGetTwitchClientId) {
                    queryParams.Add($"values={ConfigValueNames.TwitchClientId}");
                }

                if (!childCommand.TryParseOptionValue<bool>(2, optionMap, out var shouldGetTwitchClientSecret, out error)) {
                    return error;
                }

                if (shouldGetTwitchClientSecret) {
                    queryParams.Add($"values={ConfigValueNames.TwitchClientSecret}");
                }

                if (!childCommand.TryParseOptionValue<bool>(3, optionMap, out var shouldGetTwitchBotUsername, out error)) {
                    return error;
                }

                if (shouldGetTwitchBotUsername) {
                    queryParams.Add($"values={ConfigValueNames.TwitchBotUsername}");
                }

                if (!childCommand.TryParseOptionValue<bool>(4, optionMap, out var shouldGetTwitchBroadcasterChannel, out error)) {
                    return error;
                }

                if (shouldGetTwitchBroadcasterChannel) {
                    queryParams.Add($"values={ConfigValueNames.TwitchBroadcasterChannel}");
                }

                return queryParams.Count == 0
                    ? new(ErrorCode.MissingOption, $"Command `{childCommand.Aliases.ElementAt(0)}` requires a config value option\nValid options:\n{string.Join('\n', childCommand.Options.Select(option => string.Join(", ", option.Aliases)))}")
                    : await Utils.SendGetRequestAsync(httpClient, port, $"{EndpointPaths.GetConfig}?{string.Join('&', queryParams)}", cancellationToken);
            }
        );
    }
}
