namespace StonebotCLI.Options {
    using System;

    internal class EnumOption<TEnum>(string[] aliases, TEnum? defaultValue = null) : ValueOption<TEnum>(aliases, defaultValue) where TEnum : struct, Enum {
        internal override TEnum GetValue(string valueString) {
            foreach (var enumValue in Enum.GetValues<TEnum>()) {
                if (enumValue.ToString().Equals(valueString, StringComparison.OrdinalIgnoreCase)) {
                    return enumValue;
                }
            }

            throw new Exception(
                $"`{valueString}` is not a valid value for option `{Aliases[0]}`. " +
                $"The value must be one of:\n{string.Join("\n", Enum.GetNames<TEnum>())}"
            );
        }
    }
}
