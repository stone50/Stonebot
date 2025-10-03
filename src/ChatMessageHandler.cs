namespace Stonebot {
    using Models.EventSubMessages;
    using Scripting;
    using System.Globalization;
    using System.Text;

    internal static class ChatMessageHandler {
        public static void HandleChatMessage(EventSubNotificationMessagePayloadEvent channelChatMessageEvent) {
            if (channelChatMessageEvent.ChatterId == Cache.GetChatterId()) {
                return;
            }

            if (Config.ShouldFilterChatMessages) {
                FilterMessage(channelChatMessageEvent.Message);
            }

            if (channelChatMessageEvent.Message.Text.StartsWith('!') && CommandManager.TryUseCommand(channelChatMessageEvent)) {
                return;
            }

            // TODO: handle chat message
            Logger.Debug(channelChatMessageEvent.ChatterUserName, channelChatMessageEvent.Message.Text);
        }

        private static void FilterMessage(EventSubNotificationMessagePayloadEventMessage message) {
            message.Text = GetFilteredText(message.Text);
            message.Text = message.Text.Trim();
            foreach (var fragment in message.Fragments) {
                fragment.Text = GetFilteredText(fragment.Text);
            }

            message.Fragments = GetTrimmedFragments(message.Fragments);
            if (message.Fragments.Length != 0) {
                message.Fragments[0].Text = message.Fragments[0].Text.TrimStart();
                message.Fragments[^1].Text = message.Fragments[^1].Text.TrimEnd();
            }
        }

        private static string GetFilteredText(string text) {
            var normalizedText = text.Normalize(NormalizationForm.FormC);
            var filteredStringBuilder = new StringBuilder(normalizedText.Length);
            foreach (var rune in normalizedText.EnumerateRunes()) {
                if (GetIsRuneVisible(rune)) {
                    _ = filteredStringBuilder.Append(rune.ToString());
                }
            }

            return filteredStringBuilder.ToString();
        }

        private static bool GetIsRuneVisible(Rune rune) =>
            !char.IsControl((char)rune.Value) &&
            CharUnicodeInfo.GetUnicodeCategory(rune.Value) != UnicodeCategory.Format &&
            !GetIsRuneDefaultIgnorable(rune) &&
            !IsRuneNonCharacter(rune);

        private static bool GetIsRuneDefaultIgnorable(Rune rune) => rune.Value is
            0x034F or // Combining Grapheme Joiner
            0x180E or // Mongolian Vowel Separator
            0x200B or 0x200C or 0x200D or 0x2060 or // Zero-width space / joiners / word joiner
            0xFEFF or // Byte Order Mark
            >= 0xE0000 and <= 0xE007F or // Tag characters
            >= 0xFE00 and <= 0xFE0F or >= 0xE0100 and <= 0xE01EF; // Variation Selectors

        private static bool IsRuneNonCharacter(Rune rune) =>
            rune.Value is >= 0xFDD0 and <= 0xFDEF || // Noncharacters in BMP
            (rune.Value & 0xFFFF) is 0xFFFE or 0xFFFF; // Last two codepoints of each plane

        private static EventSubNotificationMessagePayloadEventMessageFragment[] GetTrimmedFragments(EventSubNotificationMessagePayloadEventMessageFragment[] fragments) {
            var start = 0;
            var end = fragments.Length - 1;
            while (start <= end && string.IsNullOrWhiteSpace(fragments[start].Text)) {
                start++;
            }

            while (end >= start && string.IsNullOrWhiteSpace(fragments[end].Text)) {
                end--;
            }

            return fragments[start..(end + 1)];
        }
    }
}
