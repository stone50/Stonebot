namespace StonebotDaemon {
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Concurrent;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    internal sealed class SubscriberRegistry(ILogger<SubscriberRegistry> logger, IHttpClientFactory httpClientFactory) {
        private readonly ConcurrentDictionary<string, string> _subscribers = [];
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Subscribers");

        internal bool TryRegister(string id, string callbackUrl) => _subscribers.TryAdd(id, callbackUrl);

        internal bool GetIsSubscriberRegistered(string id) => _subscribers.ContainsKey(id);

        internal bool TryUnregister(string id) => _subscribers.TryRemove(id, out var _);

        internal async Task SendEventToSubscribersAsync(string data) {
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            if (logger.IsEnabled(LogLevel.Information)) {
                logger.LogInformation("Sending event to subscribers: {Data}", data);
            }

            foreach (var subscriber in _subscribers) {
                var subscriberId = subscriber.Key;
                var subscriberCallbackUrl = subscriber.Value;
                HttpResponseMessage response;
                try {
                    response = await _httpClient.PostAsync(subscriberCallbackUrl, content);
                } catch (Exception e) {
                    if (logger.IsEnabled(LogLevel.Error)) {
                        logger.LogError(e, "Error sending event to {SubscriberId}", subscriberId);
                    }

                    continue;
                }

                if (!response.IsSuccessStatusCode) {
                    if (logger.IsEnabled(LogLevel.Warning)) {
                        logger.LogWarning(
                            "Failed to send event to {SubscriberId}: {StatusCode}",
                            subscriberId,
                            response.StatusCode
                        );
                    }

                    continue;
                }

                if (logger.IsEnabled(LogLevel.Information)) {
                    logger.LogInformation("Sent event to {SubscriberId}", subscriberId);
                }
            }

            if (logger.IsEnabled(LogLevel.Information)) {
                logger.LogInformation("Event sent to subscribers: {Data}", data);
            }
        }
    }
}
