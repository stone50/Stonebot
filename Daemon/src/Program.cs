namespace StonebotDaemon {
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Serilog;
    using StonebotDaemon.Endpoints;
    using StonebotDaemon.Models;
    using StonebotDaemon.Services;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using TwitchLib.Api;
    using TwitchLib.Client;

    internal static class Program {
        private static async Task Main(string[] args) {
            var dataDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Stonebot");
            var logDirPath = Path.Combine(dataDirPath, "logs");
            _ = Directory.CreateDirectory(logDirPath);
            var builder = WebApplication.CreateBuilder(args);
            _ = builder.Configuration
                .SetBasePath(dataDirPath)
                .AddJsonFile("config.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: Path.Combine(logDirPath, "stonebot-.log"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();
            _ = builder.Host
                .UseSerilog()
                .UseWindowsService()
                .UseSystemd();
            _ = builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(dataDirPath, "keys")));
            _ = builder.Services
                .Configure<Config>(builder.Configuration.GetSection("Config"))
                .AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IOptions<Config>>().Value)
                .AddSingleton<SecretService>()
                .AddSingleton(serviceProvider => {
                    var secretService = serviceProvider.GetRequiredService<SecretService>();
                    return secretService.LoadSecretsAsync(default).GetAwaiter().GetResult();
                })
                .AddSingleton<TwitchAuthState>()
                .AddSingleton<TwitchClient>()
                .AddSingleton<TwitchAPI>()
                .AddHostedService<Worker>()
                .AddSignalR();
            var app = builder.Build();
            var config = builder.Configuration.GetSection("Config").Get<Config>() ?? new Config();
            app.Urls.Add($"http://localhost:{config.Port}");
            _ = app
                .MapStatusEndpoints()
                .MapTwitchEndpoints();
            //_ = app.MapHub<StonebotHub>("/hub"); // TODO
            try {
                Log.Information("Stonebot starting on {Url}...", app.Urls.FirstOrDefault());
                await app.RunAsync().ConfigureAwait(false);
            } catch (Exception ex) {
                Log.Fatal(ex, "Stonebot terminated unexpectedly.");
            } finally {
                await Log.CloseAndFlushAsync().ConfigureAwait(false);
            }
        }
    }
}
