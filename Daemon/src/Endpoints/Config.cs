namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using StonebotCore;
    using StonebotDaemon.Models;
    using System.Text.Json;

    internal static partial class RequestDelegates {
        internal static RequestDelegate PostConfigLoad = async context => {
            var success = await Utils.TryDo(
                () => Interface.LoadConfigAsync(context.RequestAborted),
                context
            );
            if (!success) {
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("OK");
        };

        internal static RequestDelegate PatchConfigSet = async context => {
            Config? config = null;
            var success = await Utils.TryDo(
                async () => config = await JsonSerializer.DeserializeAsync<Config>(
                    context.Request.Body,
                    cancellationToken: context.RequestAborted
                ),
                context
            );
            if (!success) {
                return;
            }

            if (config == null) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid JSON payload");
                return;
            }

            if (config.ClientId != null) {
                success = await Utils.TryDo(
                    () => Interface.SetTwitchClientIdAsync(config.ClientId, context.RequestAborted),
                    context
                );
                if (!success) {
                    return;
                }
            }

            if (config.ClientSecret != null) {
                success = await Utils.TryDo(
                    () => Interface.SetTwitchClientSecretAsync(config.ClientSecret, context.RequestAborted),
                    context
                );
                if (!success) {
                    return;
                }
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("OK");
        };
    }
}
