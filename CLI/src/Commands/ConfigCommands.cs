namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System.Collections.Generic;
    using System.Net.Http;

    internal static class ConfigCommands {
        internal static ParentCommand GetConfigCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["config", "settings"],
            subCommands: [
                GetConfigLoadCommand(httpClient, portOption),
                GetConfigSetCommand(httpClient, portOption)
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
                    valueJsonParts.Add($"\"TwitchClientId\":\"{twitchClientId}\"");
                } else if (error.Code != ErrorCode.MissingOption) {
                    return error;
                }

                if (childCommand.TryParseOptionValue<string>(2, optionMap, out var twitchClientSecret, out error)) {
                    valueJsonParts.Add($"\"TwitchClientSecret\":\"{twitchClientSecret}\"");
                } else if (error.Code != ErrorCode.MissingOption) {
                    return error;
                }

                if (childCommand.TryParseOptionValue<string>(3, optionMap, out var twitchBotUsername, out error)) {
                    valueJsonParts.Add($"\"TwitchBotUsername\":\"{twitchBotUsername}\"");
                } else if (error.Code != ErrorCode.MissingOption) {
                    return error;
                }

                if (childCommand.TryParseOptionValue<string>(4, optionMap, out var twitchBroadcasterChannel, out error)) {
                    valueJsonParts.Add($"\"TwitchBroadcasterChannel\":\"{twitchBroadcasterChannel}\"");
                } else if (error.Code != ErrorCode.MissingOption) {
                    return error;
                }

                return await Utils.SendPatchRequestAsync(httpClient, port, EndpointPaths.PatchConfigSet, $"{{{string.Join(',', valueJsonParts)}}}", cancellationToken);
            }
        );
    }
}
