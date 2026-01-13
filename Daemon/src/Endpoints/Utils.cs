namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Threading.Tasks;

    internal static class Utils {
        internal static async Task<bool> TryDo(Func<Task> action, HttpContext context) {
            try {
                await action();
            } catch (OperationCanceledException) {
                context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                await context.Response.WriteAsync("Client closed request");
                return false;
            } catch (Exception e) {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync($"Internal server error: {e.Message}");
                return false;
            }

            return true;
        }
    }
}
