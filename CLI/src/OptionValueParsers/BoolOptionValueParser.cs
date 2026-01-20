namespace StonebotCLI.OptionValueParsers {
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal static partial class OptionValueParsers {
        internal static bool TryParseBoolOptionValue(string valueString, [MaybeNullWhen(false)] out bool value, [MaybeNullWhen(true)] out Error error) {
            if (
                valueString.Equals("t", StringComparison.OrdinalIgnoreCase) ||
                valueString.Equals("true", StringComparison.OrdinalIgnoreCase)
            ) {
                value = true;
                error = default;
                return true;
            }

            if (
                valueString.Equals("f", StringComparison.OrdinalIgnoreCase) ||
                valueString.Equals("false", StringComparison.OrdinalIgnoreCase)
            ) {
                value = false;
                error = default;
                return true;
            }

            value = default;
            error = new(ErrorCode.InvalidOptionValue, $"Option value `{valueString}` is not valid\nValid values:\nt, true, f, false");
            return false;
        }
    }
}
