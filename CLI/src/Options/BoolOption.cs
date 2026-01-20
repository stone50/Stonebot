namespace StonebotCLI.Options {
    using OptionValueParsers;
    using System.Collections.Generic;

    internal class BoolOption(IReadOnlyCollection<string> aliases) : Option<bool>(aliases, OptionValueParsers.TryParseBoolOptionValue) { }

    internal class OptionalBoolOption(IReadOnlyCollection<string> aliases, bool defaultValue) : BoolOption(aliases), IOptionalOption<bool> {
        bool IOptionalOption<bool>.DefaultValue => defaultValue;
    }
}
