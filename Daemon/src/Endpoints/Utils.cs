namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal static class Utils {
        internal static async Task<IResult?> TryDo(
            Func<CancellationToken, Task> action,
            string failMessage,
            ILogger logger,
            CancellationToken cancellationToken
        ) {
            try {
                await action(cancellationToken).ConfigureAwait(false);
                return null;
            } catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
                logger.LogInformation("Client closed request");
                return Results.Empty;
            } catch (Exception e) {
                if (logger.IsEnabled(LogLevel.Information)) {
                    logger.LogInformation("{failMessage}: {exceptionMessage}", failMessage, e.Message);
                }

                if (logger.IsEnabled(LogLevel.Debug)) {
                    logger.LogDebug("{exception}", e);
                }

                return Results.InternalServerError($"{failMessage}: {e.Message}");
            }
        }

        internal static IResult? TryDo(
            Action action,
            string failMessage,
            ILogger logger
        ) {
            try {
                action();
            } catch (Exception e) {
                if (logger.IsEnabled(LogLevel.Information)) {
                    logger.LogInformation("{failMessage}: {exceptionMessage}", failMessage, e.Message);
                }

                if (logger.IsEnabled(LogLevel.Debug)) {
                    logger.LogDebug("{exception}", e);
                }

                return Results.InternalServerError($"{failMessage}: {e.Message}");
            }

            return null;
        }
    }
}
