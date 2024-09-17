namespace StoneBot.Scripts.Core_Interface.EventSub {
    using Bot_Core;
    using Bot_Core.App_Cache;
    using Bot_Core.Models.EventSub;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class EventSub {
        // only up to 1 of status, type, and userId should be specified
        public static async Task<EventSubsData?> Get(string? status = null, string? type = null, string? userId = null) {
            var clientWrapper = await AppCache.HttpClientWrapper.Get();
            if (clientWrapper is null) {
                return null;
            }

            EventSubsData? combinedData = null;
            string? cursor = null;
            while (true) {
                var client = await clientWrapper.GetClient();
                if (client is null) {
                    return null;
                }

                var potentialData = await Util.GetMessageAs<EventSubsData>(TwitchAPI.GetEventSubs(client, status, type, userId, cursor));
                if (potentialData is null) {
                    return null;
                }

                var data = (EventSubsData)potentialData;
                if (combinedData is null) {
                    combinedData = data;
                } else {
                    var newCombinedData = (EventSubsData)combinedData;
                    newCombinedData.Data = newCombinedData.Data.Concat(data.Data).ToArray();
                    combinedData = newCombinedData;
                }

                if (data.Pagination.Cursor is null) {
                    break;
                }

                cursor = data.Pagination.Cursor;
            }

            return combinedData;
        }

        // only up to 1 of status, type, and userId should be specified
        public static async Task<bool> RemoveBy(string? status = null, string? type = null, string? userId = null) {
            var potentialData = await Get(status, type, userId);
            return potentialData is not null && await Remove(((EventSubsData)potentialData).Data);
        }

        public static async Task<bool> Remove(EventSubData[] eventSubs) {
            var clientWrapper = await AppCache.HttpClientWrapper.Get();
            if (clientWrapper is null) {
                return false;
            }

            foreach (var eventSub in eventSubs) {
                var client = await clientWrapper.GetClient();
                if (client is null) {
                    return false;
                }

                var successfulString = await Util.GetSuccessfulString(TwitchAPI.DeleteEventSub(client, eventSub.Id));
                if (successfulString is null) {
                    return false;
                }
            }

            return true;
        }
    }
}
