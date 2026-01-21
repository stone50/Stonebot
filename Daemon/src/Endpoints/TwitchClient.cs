namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using StonebotCore.PublicInterface;
    using System;
    using System.Threading.Tasks;

    internal static partial class RequestDelegates {
        internal sealed class TwitchClientEndpoints { }

        internal static readonly Func<
            ILoggerFactory,
            ILogger<TwitchClientEndpoints>,
            IResult
        > PostTwitchConfigureClient = (
            loggerFactory,
            logger
        ) => {
            logger.LogInformation("Configuring Twitch client");
            var twitchClientLogger = loggerFactory.CreateLogger<Interface.TwitchClientLog>();
            Interface.ConfigureTwitchClient(twitchClientLogger);
            logger.LogInformation("Twitch client configured");
            return Results.Ok("Twitch client configured");
        };

        internal static readonly Func<
            ILogger<TwitchClientEndpoints>,
            Task<IResult>
        > GetTwitchClientConfigured = async logger => {
            logger.LogInformation("Getting if Twitch client is configured");
            var json = new { IsTwitchClientConfigured = Interface.GetIsTwitchClientConfigured() };
            if (logger.IsEnabled(LogLevel.Information)) {
                logger.LogInformation("If Twitch client is configured gotten: {@Json}", json);
            }

            return Results.Ok(json);
        };

        internal static readonly Func<
            ILogger<TwitchClientEndpoints>,
            Task<IResult>
        > PostTwitchConnect = async logger => {
            logger.LogInformation("Connecting to Twitch");
            await Interface.ConnectTwtichAsync().ConfigureAwait(false);
            logger.LogInformation("Connected to Twitch");
            return Results.Ok("Connected to Twitch");
        };

        internal static readonly Func<
            ILogger<TwitchClientEndpoints>,
            Task<IResult>
        > PostTwitchDisconnect = async logger => {
            logger.LogInformation("Disconnecting from Twitch");
            await Interface.DisconnectTwtichAsync().ConfigureAwait(false);
            logger.LogInformation("Disconnected from Twitch");
            return Results.Ok("Disconnected from Twitch");
        };

        internal static readonly Func<
            ILogger<TwitchClientEndpoints>,
            Task<IResult>
        > GetTwitchConnected = async logger => {
            logger.LogInformation("Getting if Twitch is connected");
            var json = new { IsTwitchConnected = Interface.GetIsTwitchConnected() };
            if (logger.IsEnabled(LogLevel.Information)) {
                logger.LogInformation("If Twitch is connected gotten: {@Json}", json);
            }

            return Results.Ok(json);
        };
    }
}
