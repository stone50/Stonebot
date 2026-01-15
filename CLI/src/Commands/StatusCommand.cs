namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class StatusCommand() : Command(
        aliases: ["status", "health"],
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
            try {
                return Utils.SendGetRequestAsync(new(), port, EndpointPaths.GetHealth, cancellationToken);
            } catch {
                Console.WriteLine(" - Not running");
                return Task.CompletedTask;
            }
        }
    }
}
