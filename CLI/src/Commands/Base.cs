namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System.Net.Http;

    internal static partial class Commands {
        internal static ParentCommand GetBaseCommand() {
            var httpClient = new HttpClient();
            var portOption = new OptionalIntOption(
                aliases: ["--port", "-p"],
                defaultValue: Port.Default
            );

            return new(
                aliases: ["StonebotCLI"],
                subCommands: [
                    GetStartCommand(portOption),
                    GetStatusCommand(httpClient, portOption),
                    GetStopCommand(httpClient, portOption),
                    GetConfigCommand(httpClient, portOption),
                    GetTwitchCommand(httpClient, portOption),
                ]
            );
        }
    }
}
