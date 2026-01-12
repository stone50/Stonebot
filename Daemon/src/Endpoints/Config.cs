namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using StonebotCore;

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
            // TODO
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("OK");
        };
    }
}
