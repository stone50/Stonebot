namespace StonebotCLI.Commands.TwitchCommands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class TwitchDisconnectCommand() : Command(
        aliases: ["disconnect", "stop", "exit", "quit", "close"],
        options: [new PortOption()],
        subCommands: []
    ) {
        protected override async Task ExecuteAsync(
            ArgReader argReader,
            ReadOnlyDictionary<Option, string> options,
            Command? subCommand,
            CancellationToken cancellationToken
        ) {
            var port = GetValueOptionValue<PortOption, int>(_options[0], options);
            var response = await Utils.SendPostRequestAsync(new(), port, EndpointPaths.PostTwitchDisconnect, null, cancellationToken);
            _ = response.EnsureSuccessStatusCode();
        }
    }
}
