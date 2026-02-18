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
        private sealed class TwitchEndpointsLogging { }
        private const string _redirectHtml = """
            <!doctype html>
            <html lang="en">
              <head>
                <meta charset="UTF-8" />
                <title>Stonebot Authotization {{STATUS_TITLE}}</title>
                <link
                  href="https://fonts.googleapis.com/css2?family=JetBrains+Mono:wght@400;700&display=swap"
                  rel="stylesheet"
                />
                <style>
                  :root {
                      --bg: #0e0e10;
                      --surface: #18181b;
                      --text: #efeff1;
                      --text-dim: #adadb8;
                      --status-color: {{STATUS_COLOR}};
                  }
                  body {
                      background-color: var(--bg);
                      color: var(--text);
                      font-family: 'JetBrains Mono', monospace;
                      display: flex;
                      align-items: center;
                      justify-content: center;
                      height: 100vh;
                      margin: 0;
                  }
                  .container {
                      max-width: 720px;
                      border: 1px solid #2d2d31;
                      background: var(--surface);
                      padding: 0;
                      border-radius: 2px;
                  }
                  .status-bar {
                      height: 4px;
                      background: var(--status-color);
                      width: 100%;
                  }
                  .content {
                      padding: 40px 40px;
                  }
                  .bot-icon {
                      height: 64px;
                      width: auto;
                      display: block;
                      margin-bottom: 24px;
                  }
                  h1 {
                      font-size: 24px;
                      margin: 0 0 16px 0;
                      font-weight: 700;
                      letter-spacing: -0.03em;
                  }
                  p {
                      font-size: 14px;
                      line-height: 1.6;
                      color: var(--text-dim);
                      margin: 5px;
                  }
                </style>
              </head>
              <body>
                <div class="container">
                  <div class="status-bar"></div>
                  <div class="content">
                    <img src="../../favicon.ico" class="bot-icon">
                    <h1>{{STATUS_TITLE}}</h1>
                    <p>{{MESSAGE}}</p>
                    <p>You can close this tab.</p>
                  </div>
                </div>
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
                    return Utils.GetProblemResult(
                        "Failed to connect. The access token is empty. Authorization is required.",
                        StatusCodes.Status428PreconditionRequired
                    );
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
                        "Failed to connect to Twitch. The bot username, broacaster channel, or access token may be bad.",
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
                    return Utils.GetProblemResult(
                        "Failed to refresh auth token. The refresh token is empty. Initial authorization is required.",
                        StatusCodes.Status428PreconditionRequired
                    );
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

            _ = authGroup.MapGet("/redirect", async (
                Config config,
                Secrets secrets,
                SecretService secretService,
                TwitchAPI twitchAPI,
                TwitchAuthState twitchAuthState,
                string code,
                string state,
                CancellationToken cancellationToken,
                ILogger<TwitchEndpointsLogging> logger
            ) => {
                var storedState = twitchAuthState.State;
                twitchAuthState.State = string.Empty;
                if (string.IsNullOrWhiteSpace(config.TwitchClientId)) {
                    return GetRedirectHtmlResult(false, $"The config value `{ConfigValueNames.TwitchClientId}` is empty.");
                }

                if (string.IsNullOrWhiteSpace(secrets.TwitchClientSecret)) {
                    return GetRedirectHtmlResult(false, "The client secret is empty.");
                }

                if (string.IsNullOrWhiteSpace(code)) {
                    return GetRedirectHtmlResult(false, "The authorization code is empty.");
                }

                if (string.IsNullOrWhiteSpace(state)) {
                    return GetRedirectHtmlResult(false, "The state is empty.");
                }

                if (state != storedState) {
                    return GetRedirectHtmlResult(false, "The received state does not match the sent state.");
                }

                try {
                    var authCodeResponse = await twitchAPI.Auth.GetAccessTokenFromCodeAsync(code, secrets.TwitchClientSecret, $"http://localhost:{config.Port}/twitch/auth/redirect", config.TwitchClientId).ConfigureAwait(false);
                    secrets.TwitchAccessToken = authCodeResponse.AccessToken;
                    secrets.TwitchRefreshToken = authCodeResponse.RefreshToken;
                } catch (Exception ex) {
                    logger.LogError(ex, "Failed to get access token. The authorization code, client secret, or client id may be bad.");
                    return GetRedirectHtmlResult(false, "Failed to get access token. The authorization code, client secret, or client id may be bad.");
                }

                try {
                    await secretService.SaveSecretsAsync(secrets, cancellationToken).ConfigureAwait(false);
                } catch (Exception ex) {
                    logger.LogError(ex, "Failed to save secrets to disk.");
                    return GetRedirectHtmlResult(false, "Failed to save secrets to disk.");
                }

                return GetRedirectHtmlResult(true, "Completed authorization successfully.");
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

        private static IResult GetRedirectHtmlResult(bool isSuccess, string message) {
            var statusTitle = isSuccess ? "Success" : "Failure";
            var statusColor = isSuccess ? "#4ade80" : "#ff4a4a";
            return Results.Content(
                _redirectHtml
                    .Replace("{{STATUS_TITLE}}", statusTitle)
                    .Replace("{{STATUS_COLOR}}", statusColor)
                    .Replace("{{MESSAGE}}", message),
                "text/html"
            );
        }
    }
}
