namespace StonebotCLI {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class ParentCommand(IReadOnlyCollection<string> aliases, IReadOnlyCollection<Command> subCommands) : Command(aliases) {
        internal readonly IReadOnlyCollection<Command> SubCommands = subCommands;

        internal override async Task<Error?> ExecuteAsync(ArgReader argReader, CancellationToken cancellationToken) {
            if (!argReader.TryRead(out var subCommandName)) {
                return new(ErrorCode.MissingSubCommand, $"Command `{Aliases.ElementAt(0)}` requires a sub-command\nValid sub-commands:\n{string.Join('\n', SubCommands.Select(subCmd => string.Join(", ", subCmd.Aliases)))}");
            }

            var subCommand = SubCommands.FirstOrDefault(subCommand => subCommand.Aliases.Contains(subCommandName));
            return subCommand == null
                ? new(ErrorCode.InvalidSubCommand, $"Sub-command `{subCommandName}` is not valid\nValid sub-commands:\n{string.Join('\n', SubCommands.Select(subCmd => string.Join(", ", subCmd.Aliases)))}")
                : await subCommand.ExecuteAsync(argReader, cancellationToken);
        }
    }
}
