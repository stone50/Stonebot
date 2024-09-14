namespace StoneBot.Scripts {
    using Godot;
    using Middleware.EventSub;
    using Models;
    using System.Threading.Tasks;

    internal partial class App : Node {
        public App() => _ = Task.Run(Init);

        public async Task<bool> Init() {
            var potentialEventSubs = await EventSub.Get<IConditionData>();
            if (potentialEventSubs is null) {
                GD.PushWarning("Cannot init app because Get failed.");
                return false;
            }

            var eventSubs = (EventSubsData<IConditionData>)potentialEventSubs;
            GD.Print($"EventSubs: {eventSubs.Data.Length}");
            foreach (var eventSub in eventSubs.Data) {
                GD.Print($"EventSub: {eventSub.Id}");
            }

            return true;
        }
    }
}
