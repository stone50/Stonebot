namespace StonebotCore.Twitch {
    using Microsoft.Extensions.Logging;
    using StonebotCore.PublicInterface;
    using TwitchLib.Api;
    using TwitchLib.Client;

    internal static class Access {
        internal static ILogger<Interface.TwitchClientLog>? Logger;
        internal static readonly TwitchAPI API = new();
        internal static readonly TwitchClient Client = new();
    }
}
