namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;

    internal static partial class RequestDelegates {
        internal sealed class HealthEndpoints { }

        internal static readonly Func<
            ILogger<HealthEndpoints>,
            IResult
        > GetHealth = logger => {
            logger.LogInformation("Getting health");
            logger.LogInformation("Health gotten: OK");
            return Results.Ok("OK");
        };
    }
}
