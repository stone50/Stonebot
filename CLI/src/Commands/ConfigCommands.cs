namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    internal static class ConfigCommands {
        internal static ParentCommand GetConfigCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["config", "settings"],
            subCommands: [
                GetConfigGetCommand(httpClient, portOption),
                GetConfigPatchCommand(httpClient, portOption),
            ]
        );

        private static ChildCommand GetConfigGetCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["get", "fetch"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendGetRequestAsync(httpClient, port, EndpointPaths.GetConfig, cancellationToken));

        private static ChildCommand GetConfigPatchCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["patch", "set", "assign"],
            options: [
                portOption,
                new OptionalStringOption([$"--{ConfigValueNames.TwitchBotUsername}"], ""),
                new OptionalStringOption([$"--{ConfigValueNames.TwitchBroadcasterChannel}"], ""),
                new OptionalStringOption([$"--{ConfigValueNames.TwitchClientId}"], ""),
                new OptionalStringOption([$"--{ConfigValueNames.TwitchClientSecret}"], ""),
            ],
            async (childCommand, optionMap, cancellationToken) => {
                if (!childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)) {
                    return error;
                }

                if (!childCommand.TryParseOptionValue<string>(1, optionMap, out var twitchBotUsername, out error)) {
                    return error;
                }

                if (!childCommand.TryParseOptionValue<string>(2, optionMap, out var twitchBroadcasterChannel, out error)) {
                    return error;
                }

                if (!childCommand.TryParseOptionValue<string>(3, optionMap, out var twitchClientId, out error)) {
                    return error;
                }

                if (!childCommand.TryParseOptionValue<string>(4, optionMap, out var twitchClientSecret, out error)) {
                    return error;
                }

                var bodyValues = new List<string>();
                void addBodyValue(string valueName, string value) => bodyValues.Add($"\"{valueName}\":\"{value}\"");
                if (!string.IsNullOrWhiteSpace(twitchBotUsername)) {
                    addBodyValue(ConfigValueNames.TwitchBotUsername, twitchBotUsername);
                }

                if (!string.IsNullOrWhiteSpace(twitchBroadcasterChannel)) {
                    addBodyValue(ConfigValueNames.TwitchBroadcasterChannel, twitchBroadcasterChannel);
                }

                if (!string.IsNullOrWhiteSpace(twitchClientId)) {
                    addBodyValue(ConfigValueNames.TwitchClientId, twitchClientId);
                }

                if (!string.IsNullOrWhiteSpace(twitchClientSecret)) {
                    addBodyValue(ConfigValueNames.TwitchClientSecret, twitchClientSecret);
                }

                var bodyContent = $"{{{string.Join(',', bodyValues)}}}";
                return await Utils.SendPatchRequestAsync(httpClient, port, EndpointPaths.GetConfig, bodyContent, cancellationToken);
            });
    }
}
