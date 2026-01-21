namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using StonebotCore.PublicInterface;
    using StonebotDaemon.Models;
    using StonebotSharedConstants;
    using System;
    using System.Collections.Generic;
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
            ).ConfigureAwait(false);
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
                ).ConfigureAwait(false);
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
                ).ConfigureAwait(false);
                if (result != null) {
                    return result;
                }

                logger.LogDebug("Twitch client secret set");
            }

            if (config.TwitchBotUsername != null) {
                logger.LogDebug("Setting Twitch bot username");
                var result = await Utils.TryDo(
                    action: ct => Interface.SetTwitchBotUsernameAsync(config.TwitchBotUsername, ct),
                    failMessage: "Could not set Twitch bot username",
                    logger,
                    cancellationToken
                ).ConfigureAwait(false);
                if (result != null) {
                    return result;
                }

                logger.LogDebug("Twitch bot username set");
            }

            if (config.TwitchBroadcasterChannel != null) {
                logger.LogDebug("Setting Twitch broadcaster channel");
                var result = await Utils.TryDo(
                    action: ct => Interface.SetTwitchBroadcasterChannelAsync(config.TwitchBroadcasterChannel, ct),
                    failMessage: "Could not set Twitch broadcaster channel",
                    logger,
                    cancellationToken
                ).ConfigureAwait(false);
                if (result != null) {
                    return result;
                }

                logger.LogDebug("Twitch broadcaster channel set");
            }

            logger.LogInformation("Config value(s) set");
            return Results.Ok("Config value(s) set");
        };

        internal static readonly Func<
            ILogger<ConfigEndpoints>,
            string[]?,
            CancellationToken,
            Task<IResult>
        > GetConfig = async (
            logger,
            values,
            cancellationToken
        ) => {
            logger.LogInformation("Getting config value(s)");

            if (values == null) {
                logger.LogWarning("No config values requested");
                return Results.BadRequest("No config values requested");
            }

            if (logger.IsEnabled(LogLevel.Debug)) {
                logger.LogDebug("Requested value(s): {@Values}", values);
            }

            if (values == null || values.Length == 0) {
                logger.LogWarning("No config values requested");
                return Results.BadRequest("Request must contain at least one config value");
            }

            var json = new Dictionary<string, string>();
            foreach (var valueName in values) {
                switch (valueName) {
                    case ConfigValueNames.TwitchClientId:
                        logger.LogDebug("Getting Twitch client ID");
                        json.Add(ConfigValueNames.TwitchClientId, Interface.GetTwitchClientId());
                        logger.LogDebug("Twitch client ID gotten");
                        break;
                    case ConfigValueNames.TwitchClientSecret:
                        logger.LogDebug("Getting Twitch client secret");
                        json.Add(ConfigValueNames.TwitchClientSecret, await Interface.GetTwitchClientSecretAsync(cancellationToken));
                        logger.LogDebug("Twitch client secret gotten");
                        break;
                    case ConfigValueNames.TwitchBotUsername:
                        logger.LogDebug("Getting Twitch bot username");
                        json.Add(ConfigValueNames.TwitchBotUsername, Interface.GetTwitchBotUsername());
                        logger.LogDebug("Twitch bot username gotten");
                        break;
                    case ConfigValueNames.TwitchBroadcasterChannel:
                        logger.LogDebug("Getting Twitch broadcaster channel");
                        json.Add(ConfigValueNames.TwitchBroadcasterChannel, Interface.GetTwitchBroadcasterChannel());
                        logger.LogDebug("Twitch broadcaster channel gotten");
                        break;
                    default:
                        if (logger.IsEnabled(LogLevel.Information)) {
                            logger.LogInformation("Config value not found: {ValueName}", valueName);
                        }

                        return Results.NotFound($"Config value not found: {valueName}");
                }
            }

            logger.LogInformation("Config value(s) gotten");
            return Results.Ok(json);
        };
    }
}
