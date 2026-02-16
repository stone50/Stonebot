namespace StonebotDaemon {
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class Worker(ILogger<Worker> logger) : BackgroundService {
        protected override Task ExecuteAsync(CancellationToken cancellationToken) {
            logger.LogInformation("Stonebot started.");
            _ = cancellationToken.Register(() => logger.LogInformation("Stonebot stopping..."));
            return Task.CompletedTask;
        }
    }
}
