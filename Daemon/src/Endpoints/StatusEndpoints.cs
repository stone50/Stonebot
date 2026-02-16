namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Hosting;

    internal static class StatusEndpoints {
        internal static IEndpointRouteBuilder MapStatusEndpoints(this IEndpointRouteBuilder endpoints) {
            _ = endpoints.MapGet("/status", () => Utils.GetOkResult(new { status = "OK" }));
            _ = endpoints.MapPost("/stop", (IHostApplicationLifetime lifetime) => {
                lifetime.StopApplication();
                return Results.Accepted(value: new { message = "Stopping Stonebot..." });
            });
            return endpoints;
        }
    }
}
