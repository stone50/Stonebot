namespace StonebotDaemon.Endpoints {
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using StonebotDaemon.Subscription;
    using System;
    using System.Text.Json;

    internal static partial class RequestDelegates {
        internal static RequestDelegate PostSubscriber = async context => {
            SubscriberRegistration? registration;
            try {
                registration = await JsonSerializer.DeserializeAsync<SubscriberRegistration>(context.Request.Body);
            } catch (Exception e) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync($"Invalid JSON payload: {e.Message}");
                return;
            }

            if (registration == null) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Invalid JSON payload");
                return;
            }

            if (registration.Id == null) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing subscriber ID");
                return;
            }

            if (registration.CallbackUrl == null) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing callback URL");
                return;
            }

            var registry = context.RequestServices.GetRequiredService<SubscriberRegistry>();
            if (!registry.TryRegister(registration.Id, registration.CallbackUrl)) {
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                await context.Response.WriteAsync($"Failed to register subscriber '{registration.Id}'");
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("Subscribed");
        };

        internal static RequestDelegate GetSubscriber = async context => {
            var subscriberId = context.Request.RouteValues["subscriberId"]?.ToString();
            if (string.IsNullOrWhiteSpace(subscriberId)) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing subscriber ID");
                return;
            }

            var registry = context.RequestServices.GetRequiredService<SubscriberRegistry>();
            var isSubscriberRegistered = registry.GetIsSubscriberRegistered(subscriberId);
            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync(isSubscriberRegistered ? "Subscribed" : "Not subscribed");
        };

        internal static RequestDelegate DeleteSubscriber = async context => {
            var subscriberId = context.Request.RouteValues["subscriberId"]?.ToString();
            if (string.IsNullOrWhiteSpace(subscriberId)) {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing subscriber ID");
                return;
            }

            var registry = context.RequestServices.GetRequiredService<SubscriberRegistry>();
            if (!registry.TryUnregister(subscriberId)) {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync($"Subscriber '{subscriberId}' not found");
                return;
            }

            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("Unsubscribed");
        };
    }
}