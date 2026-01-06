namespace StonebotCLI.Commands {
    using System.Collections.Generic;
    using System.IO;

    internal class BaseCommand() : Command(
        aliases: ["Stonebot"],
        options: [],
        subCommands: [new AuthCommand()]
    ) {
        protected override void Execute(
            StringReader input,
            Dictionary<Option, string> options,
            Command? subCommand
        ) => subCommand?.HandleInput(input);
    }
}
