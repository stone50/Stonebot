namespace StonebotCLI.Commands.TwitchCommands {
    using StonebotCLI.Options;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TwitchAuthCommand() : Command(
        aliases: ["auth", "authorize", "login"],
        options: [
            new EnumOption<Method>(
                aliases: ["-m", "--method"],
                defaultValue: Method.TryRefreshThenFull
            ),
            new StringOption(
                aliases: ["--html"],
                defaultValue: "You can close this tab"
            ),
        ],
        subCommands: []
    ) {
        public enum Method {
            Full,
            Refresh,
            TryRefreshThenFull,
        }

        protected override Task ExecuteAsync(
            ArgReader argReader,
            ReadOnlyDictionary<Option, string> options,
            Command? subCommand,
            CancellationToken cancellationToken
        ) {
            // TODO: check if subCommand is not null; it should always be null here

            var methodOption = ((EnumOption<Method>)_options[0]).GetValue(options);
            switch (methodOption) {
                case Method.Full:
                    // TODO
                    break;
                case Method.Refresh:
                    // TODO
                    break;
                case Method.TryRefreshThenFull:
                    // TODO
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
