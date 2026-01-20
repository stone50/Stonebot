namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System;
    using System.Net.Http;

    internal static partial class Commands {
        private static ParentCommand GetTwitchCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["twitch"],
            subCommands: [
                GetTwitchAuthCommand(httpClient, portOption),
                GetTwitchConfigureClientCommand(httpClient, portOption),
                GetTwitchConnectCommand(httpClient, portOption),
                GetTwitchDisconnectCommand(httpClient, portOption),
            ]
        );

        public enum TwitchAuthMethod {
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

        private static ChildCommand GetTwitchConfigureClientCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["configure-client", "configure", "config"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendPostRequestAsync(httpClient, port, EndpointPaths.PostTwitchConfigureClient, null, cancellationToken));

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
    }
}
