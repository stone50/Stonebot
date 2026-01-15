namespace StonebotCore.Twitch {
    using Microsoft.Extensions.Logging;
    using System.Text;
    using TwitchLib.Client.Models;

    internal static class Bot {
        internal static void ConfigureClient(ILogger<Interface.TwitchClientLog>? logger) {
            Access.Logger = logger;
            var client = Access.Client;
            var config = StonebotCore.Access.Config;
            var credentials = new ConnectionCredentials(
                twitchUsername: config.TwitchBotUsername,
                twitchOAuth: Access.API.Settings.AccessToken
            );
            client.Initialize(
                credentials,
                channel: config.TwitchBroadcasterChannel
            );
            client.OnChatCommandReceived += CommandManager.OnChatCommandReceived;
        }

        internal static string GetVisibleCharacters(string input) {
            var normalizedInput = input.Normalize(NormalizationForm.FormC);
            var builder = new StringBuilder(normalizedInput.Length);
            foreach (var rune in normalizedInput.EnumerateRunes()) {
                if (
                    rune.Value is
                    0x034F or // combining grapheme joiner
                    0x200B or // zero width space
                    0x200C or // zero width non-joiner
                    0x200D or // zero width joiner
                    0x2060 or // word joiner
                    0xFEFF    // zero width no-break space/BOM
                ) {
                    continue;
                }

                _ = builder.Append(rune.ToString());
            }

            return builder.ToString().Trim();
        }
    }
}
