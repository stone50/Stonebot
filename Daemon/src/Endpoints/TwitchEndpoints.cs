namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using StonebotDaemon.Models;
    using StonebotDaemon.Services;
    using StonebotSharedConstants;
    using System;
    using System.Text;
    using System.Threading;
    using TwitchLib.Api;
    using TwitchLib.Api.Core.Enums;
    using TwitchLib.Client;
    using TwitchLib.Client.Models;

    internal static class TwitchEndpoints {
        private class TwitchEndpointsLogging { }
        // TODO
        private const string _redirectHtml = """
            <!DOCTYPE html>
            <html>
                <body>
                    <h1>Hello, World!</h1>
                </body>
            </html>
            """;

        internal static IEndpointRouteBuilder MapTwitchEndpoints(this IEndpointRouteBuilder endpoints) {
            var group = endpoints.MapGroup("/twitch");

            _ = group.MapGet("/status", (TwitchClient twitchClient) => Utils.GetOkResult(new { twitchClient.IsConnected }));

            _ = group.MapPost("/connect", async (Config config, Secrets secrets, TwitchClient twitchClient) => {
                if (twitchClient.IsConnected) {
                    return Utils.GetOkResult("Stonebot is already connected.");
                }

                if (string.IsNullOrWhiteSpace(config.TwitchBotUsername)) {
                    return Utils.GetConfigurationRequiredResult(ConfigValueNames.TwitchBotUsername);
                }

                if (string.IsNullOrWhiteSpace(config.TwitchBroadcasterChannel)) {
                    return Utils.GetConfigurationRequiredResult(ConfigValueNames.TwitchBroadcasterChannel);
                }

                if (string.IsNullOrWhiteSpace(secrets.TwitchAccessToken)) {
                    return Utils.GetConfigurationRequiredResult(ConfigValueNames.TwitchAccessToken);
                }

                var credentials = new ConnectionCredentials(
                    twitchUsername: config.TwitchBotUsername,
                    twitchOAuth: secrets.TwitchAccessToken
                );
                if (!twitchClient.IsInitialized) {
                    twitchClient.Initialize(credentials, config.TwitchBroadcasterChannel);
                    twitchClient.OnChatCommandReceived += async (sender, args) => { }; // TODO
                } else {
                    twitchClient.SetConnectionCredentials(credentials);
                }

                var isConnected = await twitchClient.ConnectAsync().ConfigureAwait(false);
                return isConnected
                    ? Utils.GetOkResult("Stonebot is connected.")
                    : Utils.GetProblemResult(
                        $"Failed to connect to Twitch. The bot username, broacaster channel, or access token may be bad.",
                        StatusCodes.Status502BadGateway
                    );
            });

            _ = group.MapPost("/disconnect", async (TwitchClient twitchClient) => {
                if (!twitchClient.IsConnected) {
                    return Utils.GetOkResult("Stonebot is already disconnected.");
                }

                await twitchClient.DisconnectAsync().ConfigureAwait(false);
                return Utils.GetOkResult("Stonebot is disconnected.");
            });

            var authGroup = group.MapGroup("/auth");

            _ = authGroup.MapGet("/status", async (Secrets secrets, TwitchAPI twitchAPI) => {
                var validateAccessTokenResponse = string.IsNullOrWhiteSpace(secrets.TwitchAccessToken)
                    ? null
                    : await twitchAPI.Auth.ValidateAccessTokenAsync(secrets.TwitchAccessToken).ConfigureAwait(false);
                return Utils.GetOkResult(new {
                    IsAuthorized = validateAccessTokenResponse != null,
                    CanRefresh = !string.IsNullOrWhiteSpace(secrets.TwitchRefreshToken),
                    validateAccessTokenResponse?.ClientId,
                    validateAccessTokenResponse?.Login,
                    validateAccessTokenResponse?.Scopes,
                    validateAccessTokenResponse?.UserId,
                    validateAccessTokenResponse?.ExpiresIn
                });
            });

            _ = authGroup.MapPost("/refresh", async (Secrets secrets, Config config, TwitchAPI twitchAPI, SecretService secretService, ILogger<TwitchEndpointsLogging> logger, CancellationToken cancellationToken) => {
                if (string.IsNullOrWhiteSpace(secrets.TwitchRefreshToken)) {
                    return Utils.GetConfigurationRequiredResult(ConfigValueNames.TwitchRefreshToken);
                }

                try {
                    var refreshResponse = await twitchAPI.Auth.RefreshAuthTokenAsync(secrets.TwitchRefreshToken, secrets.TwitchClientSecret, config.TwitchClientId).ConfigureAwait(false);
                    secrets.TwitchAccessToken = refreshResponse.AccessToken;
                    secrets.TwitchRefreshToken = refreshResponse.RefreshToken;
                } catch (Exception ex) {
                    logger.LogError(ex, "Failed to refresh auth token. The refresh token may be bad.");
                    return Utils.GetProblemResult(
                        "Failed to refresh auth token. The refresh token may be bad.",
                        StatusCodes.Status502BadGateway
                    );
                }

                try {
                    await secretService.SaveSecretsAsync(secrets, cancellationToken).ConfigureAwait(false);
                } catch (Exception ex) {
                    logger.LogError(ex, "Failed to save secrets to disk.");
                    return Utils.GetProblemResult(
                        "Failed to save secrets to disk.",
                        StatusCodes.Status500InternalServerError
                    );
                }

                return Utils.GetOkResult("Twitch authorization refreshed.");
            });

            _ = authGroup.MapGet("/url", (Config config, TwitchAPI twitchAPI, TwitchAuthState twitchAuthState) => {
                if (string.IsNullOrWhiteSpace(config.TwitchClientId)) {
                    return Utils.GetConfigurationRequiredResult(ConfigValueNames.TwitchClientId);
                }

                twitchAuthState.State = GetAuthState();
                return Utils.GetOkResult(new {
                    Url = twitchAPI.Auth.GetAuthorizationCodeUrl(
                        $"http://localhost:{config.Port}/twitch/auth/redirect",
                        [
                            AuthScopes.User_Bot,
                            AuthScopes.Channel_Bot,
                            AuthScopes.Chat_Read,
                            AuthScopes.Chat_Edit,
                        ],
                        forceVerify: true,
                        twitchAuthState.State,
                        config.TwitchClientId
                    )
                });
            });

            _ = authGroup.MapGet("/redirect", (HttpRequest request, ILogger<TwitchEndpointsLogging> logger) => {
                if (logger.IsEnabled(LogLevel.Debug)) {
                    logger.LogDebug("Redirect request: {Request}", request);
                }

                return Results.Content(_redirectHtml);
            });

            return endpoints;
        }

        private static string GetAuthState() {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new StringBuilder(30);
            for (var i = 0; i < 30; ++i) {
                _ = result.Append(chars[random.Next(chars.Length)]);
            }

            return result.ToString();
        }
    }
}
