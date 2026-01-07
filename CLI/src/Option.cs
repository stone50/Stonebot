namespace StonebotCLI {
    using System;
    using System.Collections.Generic;

    internal abstract class Option(string[] aliases) {
        internal readonly string[] Aliases = aliases;
    }

    internal abstract class ValueOption<TValue>(string[] aliases, TValue? defaultValue = null) : Option(aliases) where TValue : struct {
        private readonly TValue? _defaultValue = defaultValue;

        internal abstract TValue GetValue(string valueString);

        internal TValue GetValue(Dictionary<Option, string> optionMap) =>
            optionMap.TryGetValue(this, out var valueString)
                ? GetValue(valueString)
                : _defaultValue ?? throw new Exception($"The value for option {Aliases[0]} is required");
    }

    internal abstract class RefOption<TValue>(string[] aliases, TValue? defaultValue = null) : Option(aliases) where TValue : class {
        private readonly TValue? _defaultValue = defaultValue;

        internal abstract TValue GetValue(string valueString);

        internal TValue GetValue(Dictionary<Option, string> optionMap) =>
            optionMap.TryGetValue(this, out var valueString)
                ? GetValue(valueString)
                : _defaultValue ?? throw new Exception($"The value for option {Aliases[0]} is required");
    }
}
