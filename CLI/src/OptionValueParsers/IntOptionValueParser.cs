namespace StonebotCLI.OptionValueParsers {
    using System.Diagnostics.CodeAnalysis;

    internal static partial class OptionValueParsers {
        internal static bool TryParseIntOptionValue(string valueString, [MaybeNullWhen(false)] out int value, [MaybeNullWhen(true)] out Error error) {
            if (!int.TryParse(valueString, out value)) {
                value = default;
                error = new(ErrorCode.InvalidOptionValue, $"Value `{valueString}` is not valid\nValue must be an integer");
                return false;
            }

            error = default;
            return true;
        }
    }
}
