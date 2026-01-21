namespace StonebotCore.Twitch {
    using Microsoft.Extensions.Logging;
    using StonebotCore.PublicInterface;
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

        internal static string GetFilteredText(string text) {
            var normalizedText = text.Normalize(NormalizationForm.FormC);
            var builder = new StringBuilder(normalizedText.Length);
            var isLastCharWhitespace = false;
            foreach (var rune in normalizedText.EnumerateRunes()) {
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

                var isCurrentCharWhitespace = char.IsWhiteSpace((char)rune.Value);
                if (isCurrentCharWhitespace && isLastCharWhitespace) {
                    continue;
                }

                _ = builder.Append(isCurrentCharWhitespace ? ' ' : rune.ToString());
                isLastCharWhitespace = isCurrentCharWhitespace;
            }

            return builder.ToString().Trim();
        }
    }
}
