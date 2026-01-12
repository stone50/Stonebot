namespace StonebotCLI.Commands.TwitchCommands {
    using StonebotCLI.Options;
    using System.Collections.ObjectModel;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class TwitchAuthCommand() : Command(
        aliases: ["authorize", "auth", "login"],
        options: [
            new PortOption(),
            new EnumOption<Method>(
                aliases: ["--method", "-m"],
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

        protected override async Task ExecuteAsync(
            ArgReader argReader,
            ReadOnlyDictionary<Option, string> options,
            Command? subCommand,
            CancellationToken cancellationToken
        ) {
            var port = GetValueOptionValue<PortOption, int>(_options[0], options);
            var method = GetValueOptionValue<EnumOption<Method>, Method>(_options[1], options);
            var html = GetRefOptionValue<StringOption, string>(_options[2], options);
            using var client = new HttpClient();
            switch (method) {
                case Method.Full:
                    var fullAuthResponse = await Utils.SendPostRequestAsync(client, port, "/auth/twitch/start", $"{{\"Html\":\"{html}\"}}");
                    _ = fullAuthResponse.EnsureSuccessStatusCode();
                    break;
                case Method.Refresh:
                    var refreshAuthResponse = await Utils.SendPostRequestAsync(client, port, "/auth/twitch/refresh");
                    _ = refreshAuthResponse.EnsureSuccessStatusCode();
                    break;
                case Method.TryRefreshThenFull:
                    var tryRefreshAuthResponse = await Utils.SendPostRequestAsync(client, port, "/auth/twitch/refresh");
                    if (tryRefreshAuthResponse.IsSuccessStatusCode) {
                        break;
                    }

                    var authResponse = await Utils.SendPostRequestAsync(client, port, "/auth/twitch/start", $"{{\"Html\":\"{html}\"}}");
                    _ = authResponse.EnsureSuccessStatusCode();
                    break;
            }
        }
    }
}
