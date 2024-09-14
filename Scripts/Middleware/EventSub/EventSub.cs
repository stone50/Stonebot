namespace StoneBot.Scripts.Middleware.EventSub {
    using App_Cache;
    using Godot;
    using Models;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class EventSub {
        // only up to 1 of status, type, and userId should be specified
        public static async Task<EventSubsData<TConnectionData>?> Get<TConnectionData>(string? status = null, string? type = null, string? userId = null) where TConnectionData : IConditionData {
            var httpUserAccessTokenClient = await AppCache.HttpUserAccessTokenClient.Get();
            if (httpUserAccessTokenClient is null) {
                GD.PushWarning("Cannot get event sub because httpUserAccessTokenClient is null.");
                return null;
            }

            EventSubsData<TConnectionData>? combinedEventSubsData = null;
            string? cursor = null;
            while (true) {
                var client = await httpUserAccessTokenClient.GetClient();
                if (client is null) {
                    GD.PushWarning("Cannot get event sub because httpUserAccessTokenClient.GetClient failed.");
                    return null;
                }

                var potentialEventSubsData = await Util.ProcessHttpResponseMessage<EventSubsData<TConnectionData>>(await TwitchAPI.GetEventSubs(client, status, type, userId, cursor));
                if (potentialEventSubsData is null) {
                    GD.PushWarning("Cannot get event subs data because ProcessHttpResponseMessage failed.");
                    return null;
                }

                var eventSubsData = (EventSubsData<TConnectionData>)potentialEventSubsData;
                combinedEventSubsData ??= eventSubsData;
                var newCombinedEventSubsData = (EventSubsData<TConnectionData>)combinedEventSubsData;
                newCombinedEventSubsData.Data = newCombinedEventSubsData.Data.Concat(eventSubsData.Data).ToArray();
                combinedEventSubsData = newCombinedEventSubsData;
                if (eventSubsData.Pagination.Cursor is null) {
                    break;
                }

                cursor = eventSubsData.Pagination.Cursor;
            }

            return combinedEventSubsData;
        }

        // only up to 1 of status, type, and userId should be specified
        public static async Task<bool> RemoveBy<TConnectionData>(string? status = null, string? type = null, string? userId = null) where TConnectionData : IConditionData {
            var potentialEventSubsData = await Get<TConnectionData>(status, type, userId);
            if (potentialEventSubsData is null) {
                GD.PushWarning("Cannot remove event subs because Get failed.");
                return false;
            }

            var eventSubsData = (EventSubsData<TConnectionData>)potentialEventSubsData;
            if (!await Remove(eventSubsData.Data)) {
                GD.PushWarning("Cannot remove event subs because inner Remove failed.");
                return false;
            }

            return true;
        }

        public static async Task<bool> Remove<TConnectionData>(EventSubData<TConnectionData>[] eventSubs) where TConnectionData : IConditionData {
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
