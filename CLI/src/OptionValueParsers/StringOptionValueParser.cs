namespace StonebotCLI.OptionValueParsers {
    using System.Diagnostics.CodeAnalysis;

    internal static partial class OptionValueParsers {
        internal static bool TryParseStringOptionValue(string valueString, [MaybeNullWhen(false)] out string value, [MaybeNullWhen(true)] out Error error) {
            value =
                valueString.Length >= 2 &&
                valueString.StartsWith('"') &&
                valueString.EndsWith('"')
                    ? valueString[1..^1]
                    : valueString;
            error = default;
            return true;
        }
    }
}
