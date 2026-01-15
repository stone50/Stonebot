namespace StonebotDaemon {
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Events;
    using StonebotDaemon.Endpoints;
    using StonebotSharedConstants;
    using System;
    using System.IO;

    public static class Program {
        public static void Main(string[] args) {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production;
            var isDevelopment = environment == Environments.Development;
            var outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}";
            var exceptionTemplate = isDevelopment ? "{Exception}" : "{Exception:Message}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(isDevelopment ? LogEventLevel.Verbose : LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft", isDevelopment ? LogEventLevel.Information : LogEventLevel.Warning)
                .WriteTo.File(
                    path: Path.Join(FilePaths.StonebotDataDirPath, "logs", "stonebot-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 10,
                    outputTemplate: outputTemplate.Replace("{Exception}", exceptionTemplate),
                    shared: true
                )
                .CreateLogger();
            var envPort = Environment.GetEnvironmentVariable("STONEBOT_PORT");
            int port;
            if (string.IsNullOrWhiteSpace(envPort)) {
                port = Port.Default;
            } else {
                try {
                    port = int.Parse(envPort);
                } catch {
                    Console.Error.WriteLine($"Invalid STONEBOT_PORT value '{envPort}'. Must be a valid integer.");
                    return;
                }
            }

            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseWindowsService()
                .UseSystemd()
                .ConfigureServices(services => {
                    _ = services.AddSingleton<SubscriberRegistry>();
                    _ = services.AddSingleton<TwitchAuthCache>();
                    _ = services.AddHostedService<Worker>();
                    _ = services.AddHttpClient("Subscribers");
                })
                .ConfigureWebHostDefaults(webBuilder => {
                    _ = webBuilder.UseUrls($"http://localhost:{port}");
                    _ = webBuilder.Configure(app => {
                        _ = app.UseRouting();
                        _ = app.UseEndpoints(endpoints => {
                            _ = endpoints.MapGet(EndpointPaths.GetHealth, RequestDelegates.GetHealth);
                            _ = endpoints.MapPost(EndpointPaths.PostStop, RequestDelegates.PostStop);
                            _ = endpoints.MapPost(EndpointPaths.PostSubscriber, RequestDelegates.PostSubscriber);
                            _ = endpoints.MapGet(EndpointPaths.GetSubscriber, RequestDelegates.GetSubscriber);
                            _ = endpoints.MapDelete(EndpointPaths.DeleteSubscriber, RequestDelegates.DeleteSubscriber);
                            _ = endpoints.MapPost(EndpointPaths.PostTwitchAuthStart, RequestDelegates.PostTwitchAuthStart);
                            // TODO: add endpoint for serving a favicon
                            _ = endpoints.MapGet(EndpointPaths.GetTwitchAuth, RequestDelegates.GetTwitchAuth);
                            _ = endpoints.MapPost(EndpointPaths.PostTwitchAuthRefresh, RequestDelegates.PostTwitchAuthRefresh);
                            _ = endpoints.MapPost(EndpointPaths.PostConfigLoad, RequestDelegates.PostConfigLoad);
                            _ = endpoints.MapPatch(EndpointPaths.PatchConfigSet, RequestDelegates.PatchConfigSet);
                            // TODO: add endpoint for getting config values
                            _ = endpoints.MapPost(EndpointPaths.PostTwitchConfigureClient, RequestDelegates.PostTwitchConfigureClient);
                            _ = endpoints.MapPost(EndpointPaths.PostTwitchConnect, RequestDelegates.PostTwitchConnect);
                            _ = endpoints.MapPost(EndpointPaths.PostTwitchDisconnect, RequestDelegates.PostTwitchDisconnect);
                        });
                    });
                })
                .Build()
                .Run();
        }
    }
}
