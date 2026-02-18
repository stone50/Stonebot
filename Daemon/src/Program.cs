namespace StonebotDaemon {
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Formatting.Display;
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
            var config = builder.Configuration.Get<Config>() ?? new();
            var logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console(new RedactingFormatter(new MessageTemplateTextFormatter(logTemplate)))
                .WriteTo.File(
                    formatter: new RedactingFormatter(new MessageTemplateTextFormatter(logTemplate)),
                    path: Path.Combine(logDirPath, "stonebot-.log"),
                    rollingInterval: RollingInterval.Day
                )
                .CreateLogger();
            _ = builder.Host
                .UseSerilog()
                .UseWindowsService()
                .UseSystemd();
            _ = builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(dataDirPath, "keys")));
            _ = builder.Services
                .AddSingleton(config)
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
            _ = app
                .UseSerilogRequestLogging()
                .UseStaticFiles(new StaticFileOptions { FileProvider = new EmbeddedFileProvider(typeof(Program).Assembly, "StonebotDaemon.Resources") });
            app.Urls.Add($"http://localhost:{config.Port}");
            _ = app
                .MapConfigEndpoints()
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
