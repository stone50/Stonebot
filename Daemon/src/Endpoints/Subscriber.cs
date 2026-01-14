namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using StonebotDaemon.Models;
    using System;

    internal static partial class RequestDelegates {
        internal sealed class SubscriberEndpoints { }

        internal static readonly Func<
            SubscriberRegistration?,
            SubscriberRegistry,
            ILogger<SubscriberEndpoints>,
            IResult
        > PostSubscriber = (
            registration,
            registry,
            logger
        ) => {
            logger.LogInformation("Subscribing");
            if (logger.IsEnabled(LogLevel.Debug)) {
                logger.LogDebug("Registration: {@Registration}", registration);
            }

            if (registration == null) {
                logger.LogInformation("Invalid JSON payload");
                return Results.BadRequest("Invalid JSON payload");
            }

            if (registration.Id == null) {
                logger.LogInformation("Missing subscriber ID");
                return Results.BadRequest("Missing subscriber ID");
            }

            if (registration.CallbackUrl == null) {
                logger.LogInformation("Missing callback URL");
                return Results.BadRequest("Missing callback URL");
            }

            if (!registry.TryRegister(registration.Id, registration.CallbackUrl)) {
                if (logger.IsEnabled(LogLevel.Information)) {
                    logger.LogInformation("Failed to register subscriber '{RegistrationId}'", registration.Id);
                }

                return Results.Conflict($"Failed to register subscriber '{registration.Id}'");
            }

            logger.LogDebug("Subscriber registered");
            logger.LogInformation("Subscribed");
            return Results.Ok("Subscribed");
        };

        internal static readonly Func<
            string,
            SubscriberRegistry,
            ILogger<SubscriberEndpoints>,
            IResult
        > GetSubscriber = (
            subscriberId,
            registry,
            logger
        ) => {
            logger.LogInformation("Getting subscriber");
            if (string.IsNullOrWhiteSpace(subscriberId)) {
                logger.LogInformation("Missing subscriber ID");
                return Results.BadRequest("Missing subscriber ID");
            }

            if (logger.IsEnabled(LogLevel.Debug)) {
                logger.LogDebug("Subscriber ID: {subscriberId}", subscriberId);
            }

            var isSubscriberRegistered = registry.GetIsSubscriberRegistered(subscriberId);
            var message = isSubscriberRegistered ? "Subscribed" : "Not subscribed";
            if (logger.IsEnabled(LogLevel.Information)) {
                logger.LogInformation("Subscriber gotten: {Message}", message);
            }

            return Results.Ok(message);
        };

        internal static readonly Func<
            string,
            HttpContext,
            SubscriberRegistry,
            ILogger<SubscriberEndpoints>,
            IResult
        > DeleteSubscriber = (
            subscriberId,
            context,
            registry,
            logger
        ) => {
            logger.LogInformation("Deleting subscriber");
            if (string.IsNullOrWhiteSpace(subscriberId)) {
                logger.LogInformation("Missing subscriber ID");
                return Results.BadRequest("Missing subscriber ID");
            }

            if (logger.IsEnabled(LogLevel.Debug)) {
                logger.LogDebug("Subscriber ID: {SubscriberId}", subscriberId);
            }

            if (!registry.TryUnregister(subscriberId)) {
                if (logger.IsEnabled(LogLevel.Information)) {
                    logger.LogInformation("Subscriber '{SubscriberId}' not found", subscriberId);
                }

                return Results.NotFound($"Subscriber '{subscriberId}' not found");
            }

            logger.LogDebug("Subscriber unregistered");
            logger.LogInformation("Subscriber deleted");
            return Results.Ok("Unsubscribed");
        };
    }
}
