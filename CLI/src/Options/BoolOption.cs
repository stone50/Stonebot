namespace StonebotCLI.Options {
    using System;

    internal class BoolOption(
        string[] aliases,
        bool defaultValue
    ) : Option<bool>(
        aliases,
        defaultValue
    ) {
        public override bool GetValue(string valueString) =>
            valueString.Equals("t", StringComparison.OrdinalIgnoreCase) ||
            valueString.Equals("true", StringComparison.OrdinalIgnoreCase) ||
            (
                valueString.Equals("f", StringComparison.OrdinalIgnoreCase) ||
                valueString.Equals("false", StringComparison.OrdinalIgnoreCase)
                    ? false
                    : throw new Exception(
                        $"`{valueString}` is not a valid value for option `-{Aliases[0]}`. " +
                        "The value must be one of:\nt, true\nf, false"
                    )
            );
    }
}
