namespace StonebotDaemon.TwitchMessageHandling {
    using Microsoft.Extensions.Logging;
    using StonebotDaemon.TwitchMessageHandling.Commands;
    using System.Text;
    using System.Threading.Tasks;
    using TwitchLib.Client;
    using TwitchLib.Client.Events;

    internal class TwitchMessageHandler(TwitchClient twitchClient, ILogger<TwitchMessageHandler> logger) {
        private readonly ILogger<TwitchMessageHandler> _logger = logger;
        private readonly TwitchClient _twitchClient = twitchClient;
        private readonly Command[] _commands = {
            new LurkCommand()
        };

        internal Task OnChatCommand(object? sender, OnChatCommandReceivedArgs args) {
            foreach (var command in _commands) {
                if (command.Keyword == args.Command.Name) {
                    return command.ExecuteAsync(_twitchClient, args, default);
                }
            }

            return Task.CompletedTask;
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
