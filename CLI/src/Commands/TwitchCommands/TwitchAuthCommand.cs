namespace StonebotCLI.Commands.TwitchCommands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System.Collections.ObjectModel;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class TwitchAuthCommand() : Command(
        aliases: ["auth", "authorize", "login"],
        options: [
            new PortOption(),
            new EnumOption<Method>(
                aliases: ["--method", "-m"],
                defaultValue: Method.TryRefreshThenFull
            ),
            new StringOption(
                aliases: ["--html"],
                defaultValue: "<h1>Success!</h1><p>You can close this tab</p>"
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
            var port = GetValueOptionValue<PortOption, int>(_options[0], options);
            var method = GetValueOptionValue<EnumOption<Method>, Method>(_options[1], options);
            var html = GetRefOptionValue<StringOption, string>(_options[2], options);
            var client = new HttpClient();
            return method switch {
                Method.Full => Utils.SendPostRequestAsync(client, port, EndpointPaths.PostTwitchAuthStart, $"{{\"Html\":\"{html}\"}}", cancellationToken),
                Method.Refresh => Utils.SendPostRequestAsync(client, port, EndpointPaths.PostTwitchAuthRefresh, null, cancellationToken),
                Method.TryRefreshThenFull => TryRefreshThenFull(client, port, html, cancellationToken),
                _ => Task.CompletedTask,
            };
        }

        private static async Task<HttpResponseMessage> TryRefreshThenFull(HttpClient client, int port, string html, CancellationToken cancellationToken) {
            var tryRefreshAuthResponse = await Utils.SendPostRequestAsync(client, port, EndpointPaths.PostTwitchAuthRefresh, null, cancellationToken);
            return tryRefreshAuthResponse.IsSuccessStatusCode
                ? tryRefreshAuthResponse
                : await Utils.SendPostRequestAsync(client, port, EndpointPaths.PostTwitchAuthStart, $"{{\"Html\":\"{html}\"}}", cancellationToken);
        }
    }
}
