namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System.Net.Http;

    internal static class TwitchCommands {
        internal static ParentCommand GetTwitchCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["twitch"],
            subCommands: [
                GetTwitchStatusCommand(httpClient, portOption),
                GetTwitchConnectCommand(httpClient, portOption),
                GetTwitchDisconnectCommand(httpClient, portOption),
                GetTwitchAuthCommand(httpClient, portOption),
            ]
        );

        internal static ParentCommand GetTwitchAuthCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["auth"],
            subCommands: [
                GetTwitchAuthStatusCommand(httpClient, portOption),
                GetTwitchAuthRefreshCommand(httpClient, portOption),
                GetTwitchAuthUrlCommand(httpClient, portOption),
            ]
        );

        private static ChildCommand GetTwitchStatusCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["status"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendGetRequestAsync(httpClient, port, EndpointPaths.GetTwitchStatus, cancellationToken));

        private static ChildCommand GetTwitchConnectCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["connect", "start", "run"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendPostRequestAsync(httpClient, port, EndpointPaths.PostTwitchConnect, null, cancellationToken));

        private static ChildCommand GetTwitchDisconnectCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["disconnect", "stop", "exit", "quit", "close"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendPostRequestAsync(httpClient, port, EndpointPaths.PostTwitchDisconnect, null, cancellationToken));

        private static ChildCommand GetTwitchAuthStatusCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["status"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendGetRequestAsync(httpClient, port, EndpointPaths.GetTwitchAuthStatus, cancellationToken));

        private static ChildCommand GetTwitchAuthRefreshCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["refresh", "renew"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendPostRequestAsync(httpClient, port, EndpointPaths.PostTwitchAuthRefresh, null, cancellationToken));

        private static ChildCommand GetTwitchAuthUrlCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["url", "uri"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendGetRequestAsync(httpClient, port, EndpointPaths.GetTwitchAuthUrl, cancellationToken));
    }
}
