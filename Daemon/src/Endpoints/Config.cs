namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using StonebotCore;
    using StonebotDaemon.Models;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal static partial class RequestDelegates {
        internal sealed class ConfigEndpoints { }

        internal static readonly Func<
            ILogger<ConfigEndpoints>,
            CancellationToken,
            Task<IResult>
        > PostConfigLoad = async (
            logger,
            cancellationToken
        ) => {
            logger.LogInformation("Loading config");
            var result = await Utils.TryDo(
                action: ct => Interface.LoadConfigAsync(ct),
                failMessage: "Could not load config",
                logger,
                cancellationToken
            );
            if (result != null) {
                return result;
            }

            logger.LogInformation("Config loaded");
            return Results.Ok("Config loaded");
        };

        internal static readonly Func<
            Config?,
            ILogger<ConfigEndpoints>,
            CancellationToken,
            Task<IResult>
        > PatchConfigSet = async (
            config,
            logger,
            cancellationToken
        ) => {
            logger.LogInformation("Setting config value(s)");
            if (config == null) {
                logger.LogDebug("Invalid JSON payload");
                return Results.BadRequest("Invalid JSON payload");
            }

            if (config.TwitchClientId != null) {
                logger.LogDebug("Setting Twitch client ID");
                var result = await Utils.TryDo(
                    action: ct => Interface.SetTwitchClientIdAsync(config.TwitchClientId, ct),
                    failMessage: "Could not set Twitch client ID",
                    logger,
                    cancellationToken
                );
                if (result != null) {
                    return result;
                }

                logger.LogDebug("Twitch client ID set");
            }

            if (config.TwitchClientSecret != null) {
                logger.LogDebug("Setting Twitch client secret");
                var result = await Utils.TryDo(
                    action: ct => Interface.SetTwitchClientSecretAsync(config.TwitchClientSecret, ct),
                    failMessage: "Could not set Twitch client secret",
                    logger,
                    cancellationToken
                );
                if (result != null) {
                    return result;
                }

                logger.LogDebug("Twitch client secret set");
            }

            logger.LogInformation("Config value(s) set");
            return Results.Ok("Config value(s) set");
        };
    }
}
