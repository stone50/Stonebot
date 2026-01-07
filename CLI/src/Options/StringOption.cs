namespace StonebotCLI.Options {
    internal class StringOption(string[] aliases, string? defaultValue = null) : RefOption<string>(aliases, defaultValue) {
        internal override string GetValue(string valueString) =>
            valueString.Length >= 2 &&
            valueString.StartsWith('"') &&
            valueString.EndsWith('"')
                ? valueString[1..^1]
                : valueString;
    }
}
