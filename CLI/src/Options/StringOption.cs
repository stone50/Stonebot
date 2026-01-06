namespace StonebotCLI.Options {
    internal class StringOption(
        string[] aliases,
        string defaultValue
    ) : Option<string>(
        aliases,
        defaultValue
    ) {
        public override string GetValue(string valueString) =>
            valueString[0] == '\"' && valueString[^1] == '\"'
                ? valueString[1..^1]
                : valueString;
    }
}
