namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System;
    using System.Net;
    using System.Net.Http;

    internal static class BaseCommands {
        internal static ParentCommand GetBaseCommand() {
            var handler = new SocketsHttpHandler {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                MaxConnectionsPerServer = 1,
                AutomaticDecompression = DecompressionMethods.None
            };
            var httpClient = new HttpClient(handler, true);
            var portOption = new OptionalIntOption(
                aliases: ["--port", "-p"],
                defaultValue: Port.Default
            );

            return new(
                aliases: ["StonebotCLI"],
                subCommands: [
                    DaemonCommands.GetStartCommand(portOption),
                    DaemonCommands.GetStatusCommand(httpClient, portOption),
                    DaemonCommands.GetStopCommand(httpClient, portOption),
                    ConfigCommands.GetConfigCommand(httpClient, portOption),
                    TwitchCommands.GetTwitchCommand(httpClient, portOption),
                ]
            );
        }
    }
}
