namespace StoneBot.Scripts.Core_Interface.EventSub {
    using Bot_Core;
    using Bot_Core.App_Cache;
    using Bot_Core.Models;
    using Godot;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class EventSub {
        // only up to 1 of status, type, and userId should be specified
        public static async Task<EventSubsData?> Get(string? status = null, string? type = null, string? userId = null) {
            var httpUserAccessTokenClient = await AppCache.HttpUserAccessTokenClient.Get();
            if (httpUserAccessTokenClient is null) {
                GD.PushWarning("Cannot get event sub because httpUserAccessTokenClient is null.");
                return null;
            }

            EventSubsData? combinedEventSubsData = null;
            string? cursor = null;
            while (true) {
                var client = await httpUserAccessTokenClient.GetClient();
                if (client is null) {
                    GD.PushWarning("Cannot get event sub because httpUserAccessTokenClient.GetClient failed.");
                    return null;
                }

                var potentialEventSubsData = await Util.ProcessHttpResponseMessage<EventSubsData>(await TwitchAPI.GetEventSubs(client, status, type, userId, cursor));
                if (potentialEventSubsData is null) {
                    GD.PushWarning("Cannot get event subs data because ProcessHttpResponseMessage failed.");
                    return null;
                }

                var eventSubsData = (EventSubsData)potentialEventSubsData;
                if (combinedEventSubsData is null) {
                    combinedEventSubsData = eventSubsData;
                } else {
                    var newCombinedEventSubsData = (EventSubsData)combinedEventSubsData;
                    newCombinedEventSubsData.Data = newCombinedEventSubsData.Data.Concat(eventSubsData.Data).ToArray();
                    combinedEventSubsData = newCombinedEventSubsData;
                }

                if (eventSubsData.Pagination.Cursor is null) {
                    break;
                }

                cursor = eventSubsData.Pagination.Cursor;
            }

            return combinedEventSubsData;
        }

        // only up to 1 of status, type, and userId should be specified
        public static async Task<bool> RemoveBy(string? status = null, string? type = null, string? userId = null) {
            var potentialEventSubsData = await Get(status, type, userId);
            if (potentialEventSubsData is null) {
                GD.PushWarning("Cannot remove event subs because Get failed.");
                return false;
            }

            var eventSubsData = (EventSubsData)potentialEventSubsData;
            if (!await Remove(eventSubsData.Data)) {
                GD.PushWarning("Cannot remove event subs because inner Remove failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Remove(EventSubData[] eventSubs) {
            var httpUserAccessTokenClient = await AppCache.HttpUserAccessTokenClient.Get();
            if (httpUserAccessTokenClient is null) {
                GD.PushWarning("Cannot get event sub because httpUserAccessTokenClient is null.");
                return false;
            }

            foreach (var eventSub in eventSubs) {
                var client = await httpUserAccessTokenClient.GetClient();
                if (client is null) {
                    GD.PushWarning("Cannot clear event subs because httpUserAccessTokenClient.GetClient failed.");
                    return false;
                }

                var responseString = await Util.VerifyHttpResponseMessage(await TwitchAPI.DeleteEventSub(client, eventSub.Id));
                if (responseString is null) {
                    GD.PushWarning($"Cannot delete event sub because VerifyHttpResponseMessage failed: {responseString}.");
                    return false;
                }
            }

            return true;
        }
    }
}
