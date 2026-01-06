namespace StonebotCLI.Commands.TwitchCommands {
    using StonebotCLI.Options;
    using StonebotCore;
    using System;
    using System.Collections.Generic;
    using System.IO;
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
            Dictionary<Option, string> options,
            Command? subCommand,
            CancellationToken cancellationToken
        ) {
            // TODO: check if subCommand is not null; it should always be null here

            var methodOption = ((EnumOption<Method>)_options[0]).GetValue(options);
            switch (methodOption) {
                case Method.Full:
                    return AuthorizeTwitchAsync(options, cancellationToken);
                case Method.Refresh:
                    return Interface.RefreshTwitchAuthAsync(cancellationToken);
                case Method.TryRefreshThenFull:
                    try {
                        return Interface.RefreshTwitchAuthAsync(cancellationToken);
                    } catch (Exception) {
                        // TODO: log the error
                        return AuthorizeTwitchAsync(options, cancellationToken);
                    }
            }

            return Task.CompletedTask;
        }

        private async Task AuthorizeTwitchAsync(Dictionary<Option, string> options, CancellationToken cancellationToken) {
            var html = ((StringOption)_options[1]).GetValue(options);
            if (File.Exists(html)) {
                html = await File.ReadAllTextAsync(html, cancellationToken).ConfigureAwait(false);
            }

            await Interface.AuthorizeTwitchAsync(html, cancellationToken).ConfigureAwait(false);
        }
    }
}
