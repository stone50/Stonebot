namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using StonebotCore;
    using StonebotDaemon.Models;
    using StonebotSharedConstants;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal static partial class RequestDelegates {
        internal sealed class TwitchAuthEndpoints { }

        internal static readonly Func<
            TwitchAuth?,
            TwitchAuthCache,
            HttpRequest,
            ILogger<TwitchAuthEndpoints>,
            IResult
        > PostTwitchAuthStart = (
            auth,
            authCache,
            request,
            logger
        ) => {
            logger.LogInformation("Starting Twitch authorization");
            if (auth == null) {
                logger.LogInformation("Invalid JSON payload");
                return Results.BadRequest("Invalid JSON payload");
            }

            authCache.Html = auth.Html;
            var result = Utils.TryDo(
                action: () => authCache.State = Interface.StartTwitchAuthorization($"http://localhost:{request.Host.Port ?? Port.Default}{EndpointPaths.GetTwitchAuth}"),
                failMessage: "Could not start Twitch authorization",
                logger
            );
            if (result != null) {
                return result;
            }

            logger.LogDebug("Twtich authorization started");
            logger.LogInformation("Twitch authorization started");
            return Results.Ok("Twitch authorization started");
        };

        internal static readonly Func<
            string?,
            string?,
            HttpRequest,
            TwitchAuthCache,
            SubscriberRegistry,
            ILogger<TwitchAuthEndpoints>,
            CancellationToken,
            Task<IResult>
        > GetTwitchAuth = async (
            code,
            state,
            request,
            authCache,
            registry,
            logger,
            cancellationToken
        ) => {
            logger.LogInformation("Getting Twitch authorization");
            if (string.IsNullOrWhiteSpace(code)) {
                logger.LogInformation("Missing code");
                return Results.Content(
                    "<h1>Error</h1><h2>Missing code</h2><p>You can close this tab</p>",
                    "text/html; charset=utf-8",
                    statusCode: 400
                );
            }

            if (string.IsNullOrWhiteSpace(state)) {
                logger.LogInformation("Missing state");
                return Results.Content(
                    "<h1>Error</h1><h2>Missing state</h2><p>You can close this tab</p>",
                    "text/html; charset=utf-8",
                    statusCode: 400
                );
            }

            if (state != authCache.State) {
                logger.LogInformation("Invalid state");
                return Results.Content(
                    "<h1>Error</h1><h2>Invalid state</h2><p>You can close this tab</p>",
                    "text/html; charset=utf-8",
                    statusCode: 400
                );
            }

            var result = await Utils.TryDo(
                action: ct => Interface.AuthorizeTwitchFromCodeAsync(
                    code,
                    $"http://localhost:{request.Host.Port ?? Port.Default}{EndpointPaths.GetTwitchAuth}",
                    ct
                ),
                failMessage: "Could not authorize Twitch",
                logger,
                cancellationToken
            );
            if (result != null) {
                var errorMessage = result is IValueHttpResult valueResult ? valueResult.Value : "Something went wrong";
                var statusCode = result is IStatusCodeHttpResult statusCodeResult ? statusCodeResult.StatusCode : 500;
                return Results.Content(
                    $"<h1>Error</h1><h2>{errorMessage}</h2><p>You can close this tab</p>",
                    "text/html",
                    statusCode: statusCode
                );
            }

            logger.LogDebug("Authorized Twitch");
            logger.LogInformation("Twitch authorization gotten");
            await registry.SendEventToSubscribersAsync("Twitch authorization success");
            return Results.Content(
                authCache.Html ?? "<h1>Success!</h1><p>You can close this tab</p>",
                "text/html; charset=utf-8",
                statusCode: 200
            );
        };

        internal static readonly Func<
            ILogger<TwitchAuthEndpoints>,
            CancellationToken,
            Task<IResult>
        > PostTwitchAuthRefresh = async (
            logger,
            cancellationToken
        ) => {
            logger.LogInformation("Refreshing Twitch authorization");
            var result = await Utils.TryDo(
                action: ct => Interface.RefreshTwitchAuthAsync(ct),
                failMessage: "Could not refresh Twitch authorization",
                logger,
                cancellationToken
            );
            if (result != null) {
                return result;
            }

            logger.LogDebug("Refreshed Twitch authorization");
            logger.LogInformation("Twitch authorization refreshed");
            return Results.Ok("Twitch authorization refreshed");
        };
    }
}
