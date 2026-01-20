namespace StonebotCLI {
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    internal abstract class Option(IReadOnlyCollection<string> aliases) {
        internal readonly IReadOnlyCollection<string> Aliases = aliases;
    }

    internal delegate bool TryParseOptionValue<TValue>(string valueString, [MaybeNullWhen(false)] out TValue value, [MaybeNullWhen(true)] out Error error);

    internal abstract class Option<TValue>(IReadOnlyCollection<string> aliases, TryParseOptionValue<TValue> tryParseValue) : Option(aliases) {
        internal readonly TryParseOptionValue<TValue> TryParseValue = tryParseValue;
    }

    internal interface IOptionalOption<TValue> {
        internal TValue DefaultValue { get; }
    }
}
