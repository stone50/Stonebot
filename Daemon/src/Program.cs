namespace StonebotDaemon {
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using StonebotDaemon.Endpoints;
    using System;

    public static class Program {
        public static void Main(string[] args) {
            var envPort = Environment.GetEnvironmentVariable("STONEBOT_PORT");
            int port;
            if (string.IsNullOrWhiteSpace(envPort)) {
                port = 57043;
            } else {
                try {
                    port = int.Parse(envPort);
                } catch {
                    Console.Error.WriteLine($"Invalid STONEBOT_PORT value '{envPort}'. Must be a valid integer.");
                    return;
                }
            }

            Host.CreateDefaultBuilder(args)
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
                            _ = endpoints.MapGet("/health", RequestDelegates.GetHealth);
                            _ = endpoints.MapPost("/subscriber", RequestDelegates.PostSubscriber);
                            _ = endpoints.MapGet("/subscriber/{subscriberId}", RequestDelegates.GetSubscriber);
                            _ = endpoints.MapDelete("/subscriber/{subscriberId}", RequestDelegates.DeleteSubscriber);
                            _ = endpoints.MapPost("/auth/twitch/start", RequestDelegates.PostAuthTwitchStart);
                            _ = endpoints.MapPost("/auth/twitch", RequestDelegates.PostAuthTwitch);
                            _ = endpoints.MapPost("/auth/twitch/refresh", RequestDelegates.PostAuthTwitchRefresh);
                        });
                    });
                })
                .Build()
                .Run();
        }
    }
}
