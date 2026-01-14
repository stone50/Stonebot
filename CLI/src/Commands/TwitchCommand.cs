namespace StonebotCLI.Commands {
    using StonebotCLI.Commands.TwitchCommands;
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class TwitchCommand() : Command(
        aliases: ["twitch"],
        options: [],
        subCommands: [
            new TwitchAuthCommand(),
            new TwitchConfigureClientCommand(),
            new TwitchConnectCommand(),
            new TwitchDisconnectCommand()
        ]
    ) {
        protected override Task ExecuteAsync(
            ArgReader argReader,
            ReadOnlyDictionary<Option, string> options,
            Command? subCommand,
            CancellationToken cancellationToken
        ) => subCommand != null
                ? subCommand.HandleInputAsync(argReader, cancellationToken)
                : throw new Exception(
                    $"Sub-command for command `{_aliases[0]}` must be one of:\n" +
                    GetAllSubCommandAliasesText()
                );
    }
}
