namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using StonebotCore;
    using StonebotDaemon.Models;
    using System;
    using System.Text.Json;

    internal static partial class RequestDelegates {
        internal static RequestDelegate PostAuthTwitchStart = async context => {
            TwitchAuth? auth;
            try {
                auth = await JsonSerializer.DeserializeAsync<TwitchAuth>(context.Request.Body);
            } catch (Exception e) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync($"Invalid JSON payload: {e.Message}");
                return;
            }

            if (auth == null) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid JSON payload");
                return;
            }

            var authCache = context.RequestServices.GetRequiredService<TwitchAuthCache>();
            authCache.Html = auth.Html;
            authCache.State = Interface.StartAuthorization($"{context.Request.PathBase}/auth/twitch");
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("OK");
        };

        internal static RequestDelegate PostAuthTwitch = async context => {
            var code = context.Request.RouteValues["code"]?.ToString();
            if (string.IsNullOrWhiteSpace(code)) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing code");
                return;
            }

            var state = context.Request.RouteValues["state"]?.ToString();
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

            try {
                await Interface.AuthorizeTwitchFromCodeAsync(code, $"{context.Request.PathBase}/auth/twitch", context.RequestAborted);
            } catch (OperationCanceledException) {
                context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                await context.Response.WriteAsync("Client closed request");
                return;
            } catch (Exception e) {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync($"Internal server error: {e.Message}");
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("OK");
            var subscriberRegistry = context.RequestServices.GetRequiredService<SubscriberRegistry>();
            await subscriberRegistry.SendEventToSubscribersAsync("Twitch authorization success");
        };

        internal static RequestDelegate PostAuthTwitchRefresh = async context => {
            try {
                await Interface.RefreshTwitchAuthAsync(context.RequestAborted);
            } catch (OperationCanceledException) {
                context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                await context.Response.WriteAsync("Client closed request");
                return;
            } catch (Exception e) {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync($"Internal server error: {e.Message}");
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("OK");
        };
    }
}
