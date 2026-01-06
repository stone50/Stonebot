namespace StonebotCLI.Options {
    using System;

    internal class IntOption(string[] aliases, int defaultValue) : Option<int>(aliases, defaultValue) {
        internal override int GetValue(string valueString) =>
            int.TryParse(valueString, out var result)
                ? result
                : throw new Exception(
                    $"`{valueString}` is not a valid value for option `{Aliases[0]}`. " +
                    "The value must be an integer"
                );
    }
}
