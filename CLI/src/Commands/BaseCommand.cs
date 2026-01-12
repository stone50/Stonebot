namespace StonebotCLI.Commands {
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class BaseCommand() : Command(
        aliases: ["StonebotCLI"],
        options: [],
        subCommands: [
            new StartCommand(),
            new StopCommand(),
            new StatusCommand(),
            new TwitchCommand()
        ]
    ) {
        protected override Task ExecuteAsync(
            ArgReader argReader,
            ReadOnlyDictionary<Option, string> options,
            Command? subCommand,
            CancellationToken cancellationToken
        ) =>
            subCommand != null
                ? subCommand.HandleInputAsync(argReader, cancellationToken)
                : throw new Exception(
                    $"Sub-command for command `{_aliases[0]}` must be one of:\n" +
                    GetAllSubCommandAliasesText()
                );
    }
}
