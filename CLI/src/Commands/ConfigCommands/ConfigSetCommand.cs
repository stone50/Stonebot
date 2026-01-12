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
                aliases: ["--client-id", "--id"]
            ),
            new StringOption(
                aliases: ["--client-secret", "--secret"]
            )
        ],
        subCommands: []
    ) {
        protected override Task ExecuteAsync(
            ArgReader argReader,
            ReadOnlyDictionary<Option, string> options,
            Command? subCommand,
            CancellationToken cancellationToken
        ) {
            var port = GetValueOptionValue<PortOption, int>(_options[0], options);
            var valueJsonParts = new List<string>();
            if (options.ContainsKey(_options[1])) {
                var clientId = GetRefOptionValue<StringOption, string>(_options[1], options);
                valueJsonParts.Add($"\"ClientId\":\"{clientId}\"");
            }

            if (options.ContainsKey(_options[2])) {
                var clientSecret = GetRefOptionValue<StringOption, string>(_options[2], options);
                valueJsonParts.Add($"\"ClientSecret\":\"{clientSecret}\"");
            }

            var bodyContent = $"{{{string.Join(',', valueJsonParts)}}}";
            return Utils.SendPatchRequestAsync(new(), port, EndpointPaths.PatchConfigSet, bodyContent, cancellationToken);
        }
    }
}
