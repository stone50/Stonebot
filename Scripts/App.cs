namespace StoneBot.Scripts {
    using Godot;
    using Middleware.EventSub;
    using Models;
    using System.Threading.Tasks;

    internal partial class App : Node {
        public App() => _ = Task.Run(Init);

        public async Task<bool> Init() {
            _ = await PrintEventSubs();
            _ = await EventSub.RemoveBy();
            _ = await PrintEventSubs();
            _ = await ChannelChatMessage.Connect();
            _ = await PrintEventSubs();
            return true;
        }

        private static async Task<bool> PrintEventSubs() {
            var potentialEventSubs = await EventSub.Get();
            if (potentialEventSubs is null) {
                GD.PushWarning("Cannot print event subs because Get failed.");
                return false;
            }

            var eventSubs = (EventSubsData)potentialEventSubs;
            GD.Print($"EventSubs: {eventSubs.Data.Length}");
            foreach (var eventSub in eventSubs.Data) {
                GD.Print($"EventSub: {{{eventSub.Id}, {eventSub.Status}}}");
            }

            return true;
        }
    }
}
