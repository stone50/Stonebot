namespace StoneBot.Scripts.Timer {
    using System.Threading.Tasks;

    internal static class UseActions {
        public static async Task Quote() => await Task.Yield(); // TODO: send a random quote in chat
    }
}
