namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using StonebotDaemon.Models;
    using StonebotDaemon.Services;
    using StonebotSharedConstants;
    using System;
    using System.IO;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class ConfigEndpoints {
        private sealed class ConfigEndpointsLogging { }

        internal static IEndpointRouteBuilder MapConfigEndpoints(this IEndpointRouteBuilder endpoints) {
            var group = endpoints.MapGroup("/config");

            _ = group.MapGet("/", (Config config) => Utils.GetOkResult(new {
                config.TwitchBotUsername,
                config.TwitchBroadcasterChannel,
                config.TwitchClientId,
            }));

            _ = group.MapPatch("/", async (Config config, Secrets secrets, SecretService secretService, JsonElement updates, ILogger<ConfigEndpointsLogging> logger, CancellationToken cancellationToken) => {
                var newConfig = new Config() {
                    Port = config.Port,
                    TwitchBotUsername = config.TwitchBotUsername,
                    TwitchBroadcasterChannel = config.TwitchBroadcasterChannel,
                    TwitchClientId = config.TwitchClientId,
                };
                var newSecrets = new Secrets() {
                    TwitchClientSecret = secrets.TwitchClientSecret,
                    TwitchAccessToken = secrets.TwitchAccessToken,
                    TwitchRefreshToken = secrets.TwitchRefreshToken,
                    LocalApiKey = secrets.LocalApiKey,
                };
                var shouldSaveConfig = false;
                var shouldSaveSecrets = false;
                foreach (var property in updates.EnumerateObject()) {
                    switch (property.Name) {
                        case ConfigValueNames.TwitchBotUsername:
                            var newTwitchBotUsername = property.Value.GetString();
                            if (newTwitchBotUsername == null) {
                                break;
                            }

                            if (newConfig.TwitchBotUsername == newTwitchBotUsername) {
                                break;
                            }

                            newConfig.TwitchBotUsername = newTwitchBotUsername;
                            shouldSaveConfig = true;
                            break;
                        case ConfigValueNames.TwitchBroadcasterChannel:
                            var newTwitchBroadcasterChannel = property.Value.GetString();
                            if (newTwitchBroadcasterChannel == null) {
                                break;
                            }

                            if (newConfig.TwitchBroadcasterChannel == newTwitchBroadcasterChannel) {
                                break;
                            }

                            newConfig.TwitchBroadcasterChannel = newTwitchBroadcasterChannel;
                            shouldSaveConfig = true;
                            break;
                        case ConfigValueNames.TwitchClientId:
                            var newTwitchClientId = property.Value.GetString();
                            if (newTwitchClientId == null) {
                                break;
                            }

                            if (newConfig.TwitchClientId == newTwitchClientId) {
                                break;
                            }

                            newConfig.TwitchClientId = newTwitchClientId;
                            shouldSaveConfig = true;
                            if (!string.IsNullOrWhiteSpace(newSecrets.TwitchAccessToken)) {
                                newSecrets.TwitchAccessToken = string.Empty;
                                shouldSaveSecrets = true;
                            }

                            if (!string.IsNullOrWhiteSpace(newSecrets.TwitchRefreshToken)) {
                                newSecrets.TwitchRefreshToken = string.Empty;
                                shouldSaveSecrets = true;
                            }

                            break;
                        case ConfigValueNames.TwitchClientSecret:
                            var newTwitchClientSecret = property.Value.GetString();
                            if (newTwitchClientSecret == null) {
                                break;
                            }

                            if (newSecrets.TwitchClientSecret == newTwitchClientSecret) {
                                break;
                            }

                            newSecrets.TwitchClientSecret = newTwitchClientSecret;
                            shouldSaveSecrets = true;
                            newSecrets.TwitchAccessToken = string.Empty;
                            newSecrets.TwitchRefreshToken = string.Empty;
                            break;
                    }
                }

                var saveConfigTask = shouldSaveConfig ? SaveConfigAsync(newConfig, cancellationToken) : Task.CompletedTask;
                var saveSecretsTask = shouldSaveSecrets ? secretService.SaveSecretsAsync(newSecrets, cancellationToken) : Task.CompletedTask;
                var saveTasks = Task.WhenAll(saveConfigTask, saveSecretsTask);
                try {
                    await saveTasks.ConfigureAwait(false);
                } catch {
                    if (saveTasks.Exception != null) {
                        foreach (var innerEx in saveTasks.Exception.Flatten().InnerExceptions) {
                            logger.LogError(innerEx, "Failed to save config value(s) to disk.");
                        }
                    }

                    return Utils.GetProblemResult(
                        "Failed to save config value(s) to disk.",
                        StatusCodes.Status500InternalServerError
                    );
                }

                return Utils.GetOkResult("Configuration updated. Stonebot will need to be restarted for the changes to take affect.");
            });

            return endpoints;
        }

        private static async Task SaveConfigAsync(Config config, CancellationToken cancellationToken) {
            var configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stonebot", "config.json");
            using var fileStream = File.Create(configFilePath);
            await JsonSerializer.SerializeAsync(fileStream, config, cancellationToken: cancellationToken).ConfigureAwait(false);
            await fileStream.DisposeAsync().ConfigureAwait(false);
        }
    }
}
