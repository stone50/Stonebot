namespace StonebotCLI.OptionValueParsers {
    using System;
    using System.Diagnostics.CodeAnalysis;

    internal static partial class OptionValueParsers {
        internal static bool TryParseEnumOptionValue<TEnum>(string valueString, [MaybeNullWhen(false)] out TEnum value, [MaybeNullWhen(true)] out Error error) where TEnum : struct, Enum {
            foreach (var enumValue in Enum.GetValues<TEnum>()) {
                if (enumValue.ToString().Equals(valueString, StringComparison.OrdinalIgnoreCase)) {
                    value = enumValue;
                    error = default;
                    return true;
                }
            }

            value = default;
            error = new(ErrorCode.InvalidOptionValue, $"Value `{valueString}` is not valid\nValid values (case-insensitive):\n{string.Join(", ", Enum.GetNames<TEnum>())}");
            return false;
        }
    }
}
