namespace StonebotDaemon {
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using StonebotDaemon.Endpoints;
    using StonebotSharedConstants;
    using System;

    public static class Program {
        public static void Main(string[] args) {
            // TODO: configure logger
            Log.Logger = new LoggerConfiguration().WriteTo.File("logs/temp.txt", rollingInterval: RollingInterval.Day).CreateLogger();

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
                            _ = endpoints.MapPost(EndpointPaths.PostAuthTwitchStart, RequestDelegates.PostAuthTwitchStart);
                            _ = endpoints.MapGet(EndpointPaths.GetAuthTwitch, RequestDelegates.GetAuthTwitch);
                            _ = endpoints.MapPost(EndpointPaths.PostAuthTwitchRefresh, RequestDelegates.PostAuthTwitchRefresh);
                            _ = endpoints.MapPost(EndpointPaths.PostConfigLoad, RequestDelegates.PostConfigLoad);
                            _ = endpoints.MapPatch(EndpointPaths.PatchConfigSet, RequestDelegates.PatchConfigSet);
                        });
                    });
                })
                .Build()
                .Run();
        }
    }
}
