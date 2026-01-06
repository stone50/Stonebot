namespace StonebotCLI.Commands.AuthCommands {
    using StonebotCLI.Options;
    using System.Collections.Generic;
    using System.IO;

    internal class AuthTwitchCommand() : Command(
        aliases: ["twitch"],
        options: [
            new EnumOption<Method>(
                aliases: ["m", "method"],
                defaultValue: Method.TryRefreshThenFull
            ),
        ],
        subCommands: []
    ) {
        public enum Method {
            Full,
            Refresh,
            TryRefreshThenFull,
        }

        protected override void Execute(
            StringReader input,
            Dictionary<Option, string> options,
            Command? subCommand
        ) {
            // TODO: check if subCommand is not null; it should always be null here

            var methodOption = GetOptionValue<Method>(Options[0], options);
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
        }
    }
}
