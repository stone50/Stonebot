namespace StonebotCLI.Commands.ConfigCommands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class ConfigSetCommand() : Command(
        aliases: ["set", "assign"],
        options: [
            new PortOption(),
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
        subCommands: []
    ) {
        protected override async Task ExecuteAsync(
            ArgReader argReader,
            ReadOnlyDictionary<Option, string> options,
            Command? subCommand,
            CancellationToken cancellationToken
        ) {
            var port = GetValueOptionValue<PortOption, int>(_options[0], options);
            var valueJsonParts = new List<string>();
            if (options.ContainsKey(_options[1])) {
                var clientId = GetRefOptionValue<StringOption, string>(_options[1], options);
                valueJsonParts.Add($"\"TwitchClientId\":\"{clientId}\"");
            }

            if (options.ContainsKey(_options[2])) {
                var clientSecret = GetRefOptionValue<StringOption, string>(_options[2], options);
                valueJsonParts.Add($"\"TwitchClientSecret\":\"{clientSecret}\"");
            }

            if (options.ContainsKey(_options[3])) {
                var botUsername = GetRefOptionValue<StringOption, string>(_options[3], options);
                valueJsonParts.Add($"\"TwitchBotUsername\":\"{botUsername}\"");
            }

            if (options.ContainsKey(_options[4])) {
                var broadcasterChannel = GetRefOptionValue<StringOption, string>(_options[4], options);
                valueJsonParts.Add($"\"TwitchBroadcasterChannel\":\"{broadcasterChannel}\"");
            }

            var bodyContent = $"{{{string.Join(',', valueJsonParts)}}}";
            var response = await Utils.SendPatchRequestAsync(new(), port, EndpointPaths.PatchConfigSet, bodyContent, cancellationToken);
            _ = response.EnsureSuccessStatusCode();
        }
    }
}
