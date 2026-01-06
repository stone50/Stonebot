namespace StonebotCLI.Commands {
    using StonebotCLI.Commands.AuthCommands;
    using System.Collections.Generic;
    using System.IO;

    internal class AuthCommand() : Command(
        aliases: ["auth", "authorize", "login"],
        options: [],
        subCommands: [new AuthTwitchCommand()]
    ) {
        protected override void Execute(
            StringReader input,
            Dictionary<Option, string> options,
            Command? subCommand
        ) => subCommand?.HandleInput(input);
    }
}
