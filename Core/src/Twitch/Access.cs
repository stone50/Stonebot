namespace StonebotCore.Twitch {
    using TwitchLib.Api;
    using TwitchLib.Client;

    internal static class Access {
        internal static readonly TwitchAPI API = new();
        internal static readonly TwitchClient Client = new();
    }
}
