namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;

    internal static partial class RequestDelegates {
        internal static RequestDelegate GetHealth = async context => {
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("OK");
        };
    }
}
