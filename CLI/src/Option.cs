namespace StonebotCLI {
    using System.Collections.Generic;

    internal abstract class Option(string[] aliases) {
        internal readonly string[] Aliases = aliases;
    }

    internal abstract class Option<TValue>(string[] aliases, TValue defaultValue) : Option(aliases) {
        internal readonly TValue DefaultValue = defaultValue;

        internal abstract TValue GetValue(string valueString);

        internal TValue GetValue(Dictionary<Option, string> optionMap) =>
            optionMap.TryGetValue(this, out var valueString)
                ? GetValue(valueString)
                : DefaultValue;
    }
}
