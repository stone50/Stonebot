namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;
    using System;

    internal static partial class RequestDelegates {
        internal static Delegate PostStop = async (HttpContext context, IHostApplicationLifetime lifetime) => {
            context.Response.StatusCode = StatusCodes.Status202Accepted;
            await context.Response.WriteAsync("Stopping");
            lifetime.StopApplication();
        };
    }
}
