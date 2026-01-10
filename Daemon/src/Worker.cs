namespace StonebotDaemon {
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class Worker(ILogger<Worker> logger) : BackgroundService {
        protected override Task ExecuteAsync(CancellationToken cancellationToken) {
            logger.LogInformation("Background service started.");
            _ = cancellationToken.Register(() => logger.LogInformation("Background service stopping."));
            return Task.CompletedTask;
        }
    }
}
