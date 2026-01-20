namespace StonebotCLI.Options {
    using StonebotCLI.OptionValueParsers;
    using System;
    using System.Collections.Generic;

    internal class EnumOption<TEnum>(IReadOnlyCollection<string> aliases) : Option<TEnum>(aliases, OptionValueParsers.TryParseEnumOptionValue) where TEnum : struct, Enum { }

    internal class OptionalEnumOption<TEnum>(IReadOnlyCollection<string> aliases, TEnum defaultValue) : EnumOption<TEnum>(aliases), IOptionalOption<TEnum> where TEnum : struct, Enum {
        TEnum IOptionalOption<TEnum>.DefaultValue => defaultValue;
    }
}
