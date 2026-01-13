namespace StonebotCLI.Commands.ConfigCommands {
    using StonebotCLI.Commands;
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class ConfigLoadCommand() : Command(
        aliases: ["load", "read"],
        options: [new PortOption()],
        subCommands: []
    ) {
        protected override Task ExecuteAsync(
            ArgReader argReader,
            ReadOnlyDictionary<Option, string> options,
            Command? subCommand,
            CancellationToken cancellationToken
        ) {
            var port = GetValueOptionValue<PortOption, int>(_options[0], options);
            return Utils.SendPostRequestAsync(new(), port, EndpointPaths.PostConfigLoad, null, cancellationToken);
        }
    }
}
