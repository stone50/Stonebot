namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using System;
    using System.Collections.ObjectModel;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class StatusCommand() : Command(
        aliases: ["status", "health"],
        options: [new PortOption()],
        subCommands: []
    ) {
        protected override async Task ExecuteAsync(
            ArgReader argReader,
            ReadOnlyDictionary<Option, string> options,
            Command? subCommand,
            CancellationToken cancellationToken
        ) {
            var port = GetValueOptionValue<PortOption, int>(_options[0], options);
            HttpResponseMessage response;
            try {
                response = await Utils.SendGetRequestAsync(new(), port, "/health", cancellationToken);
                Console.WriteLine(response.IsSuccessStatusCode ? "Running" : "Not running");
            } catch {
                Console.WriteLine("Not running");
            }
        }
    }
}
