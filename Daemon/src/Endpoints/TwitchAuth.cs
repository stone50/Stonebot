namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using StonebotCore;
    using StonebotDaemon.Models;
    using StonebotSharedConstants;
    using System.Text.Json;

    internal static partial class RequestDelegates {
        internal static RequestDelegate PostAuthTwitchStart = async context => {
            TwitchAuth? auth = null;
            var success = await Utils.TryDo(
                async () => auth = await JsonSerializer.DeserializeAsync<TwitchAuth>(
                    context.Request.Body,
                    cancellationToken: context.RequestAborted
                ),
                context
            );
            if (!success) {
                return;
            }

            if (auth == null) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid JSON payload");
                return;
            }

            var authCache = context.RequestServices.GetRequiredService<TwitchAuthCache>();
            authCache.Html = auth.Html;
            // TODO: fix error "Internal server error: The payload was invalid. For more information go to https://aka.ms/aspnet/dataprotectionwarning"
            authCache.State = Interface.StartAuthorization($"{context.Request.Scheme}://{context.Request.Host}{EndpointPaths.GetAuthTwitch}");
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("OK");
        };

        internal static RequestDelegate GetAuthTwitch = async context => {
            var code = context.Request.Query["code"].ToString();
            if (string.IsNullOrWhiteSpace(code)) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing code");
                return;
            }

            var state = context.Request.Query["state"].ToString();
            if (string.IsNullOrWhiteSpace(state)) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing state");
                return;
            }

            var authCache = context.RequestServices.GetRequiredService<TwitchAuthCache>();
            if (state != authCache.State) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid state");
                return;
            }

            var success = await Utils.TryDo(
                () => Interface.AuthorizeTwitchFromCodeAsync(code, $"{context.Request.Scheme}://{context.Request.Host}{EndpointPaths.GetAuthTwitch}", context.RequestAborted),
                context
            );
            if (!success) {
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("OK");
            var subscriberRegistry = context.RequestServices.GetRequiredService<SubscriberRegistry>();
            await subscriberRegistry.SendEventToSubscribersAsync("Twitch authorization success");
        };

        internal static RequestDelegate PostAuthTwitchRefresh = async context => {
            var success = await Utils.TryDo(
                () => Interface.RefreshTwitchAuthAsync(context.RequestAborted),
                context
            );
            if (!success) {
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("OK");
        };
    }
}
