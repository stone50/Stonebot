namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;

    internal static partial class RequestDelegates {
        internal sealed class StopEndpoints { }

        internal static readonly Func<
            IHostApplicationLifetime,
            ILogger<StopEndpoints>,
            IResult
        > PostStop = (
            lifetime,
            logger
        ) => {
            logger.LogInformation("Stopping");
            lifetime.StopApplication();
            return Results.Accepted(value: "Stopping");
        };
    }
}
