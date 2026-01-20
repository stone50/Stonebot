namespace StonebotCLI {
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal abstract class Command(IReadOnlyCollection<string> aliases) {
        internal readonly IReadOnlyCollection<string> Aliases = aliases;

        internal abstract Task<Error?> ExecuteAsync(ArgReader argReader, CancellationToken cancellationToken);
    }
}
