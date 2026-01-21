namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System;
    using System.Net.Http;

    internal static class TwitchCommands {
        internal static ParentCommand GetTwitchCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["twitch"],
            subCommands: [
                GetTwitchAuthCommand(httpClient, portOption),
                GetTwitchAuthorizedCommand(httpClient, portOption),
                GetTwitchConfigureClientCommand(httpClient, portOption),
                GetTwitchClientConfiguredCommand(httpClient, portOption),
                GetTwitchConnectCommand(httpClient, portOption),
                GetTwitchDisconnectCommand(httpClient, portOption),
                GetTwitchConnectedCommand(httpClient, portOption),
            ]
        );

        internal enum TwitchAuthMethod {
            Full,
            Refresh,
            TryRefreshThenFull,
        }

        private static ChildCommand GetTwitchAuthCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["auth", "authorize", "login"],
            options: [
                portOption,
                new OptionalEnumOption<TwitchAuthMethod>(
                    aliases: ["--method", "-m"],
                    defaultValue: TwitchAuthMethod.TryRefreshThenFull
                ),
                new OptionalStringOption(
                    aliases: ["--html"],
                    defaultValue: "<h1>Success!</h1><p>You can close this tab</p>"
                ),
            ],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : !childCommand.TryParseOptionValue<TwitchAuthMethod>(1, optionMap, out var method, out error)
                ? error
                : !childCommand.TryParseOptionValue<string>(2, optionMap, out var html, out error)
                ? error
                : method switch {
                    TwitchAuthMethod.Full => await Utils.SendPostRequestAsync(httpClient, port, EndpointPaths.PostTwitchAuthStart, $"{{\"Html\":\"{html}\"}}", cancellationToken),
                    TwitchAuthMethod.Refresh => await Utils.SendPostRequestAsync(httpClient, port, EndpointPaths.PostTwitchAuthRefresh, null, cancellationToken),
                    TwitchAuthMethod.TryRefreshThenFull =>
                        await Utils.SendPostRequestAsync(httpClient, port, EndpointPaths.PostTwitchAuthRefresh, null, cancellationToken) == null
                        ? null
                        : await Utils.SendPostRequestAsync(httpClient, port, EndpointPaths.PostTwitchAuthStart, $"{{\"Html\":\"{html}\"}}", cancellationToken),
                    _ => throw new Exception($"Unsupported auth method: {method}"),
                });

        private static ChildCommand GetTwitchAuthorizedCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["is-authorized", "authorized"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendGetRequestAsync(httpClient, port, EndpointPaths.GetTwitchAuthorized, cancellationToken));

        private static ChildCommand GetTwitchConfigureClientCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["configure-client", "configure", "config"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendPostRequestAsync(httpClient, port, EndpointPaths.PostTwitchConfigureClient, null, cancellationToken));

        private static ChildCommand GetTwitchClientConfiguredCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["is-client-configured", "is-configured", "configured"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendGetRequestAsync(httpClient, port, EndpointPaths.GetTwitchClientConfigured, cancellationToken));

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

        private static ChildCommand GetTwitchConnectedCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["is-connected", "connected"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendGetRequestAsync(httpClient, port, EndpointPaths.GetTwitchConnected, cancellationToken));
    }
}
