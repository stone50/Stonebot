namespace StonebotCLI.Options {
    using StonebotCLI.OptionValueParsers;
    using System.Collections.Generic;

    internal class StringOption(IReadOnlyCollection<string> aliases) : Option<string>(aliases, OptionValueParsers.TryParseStringOptionValue) { }

    internal class OptionalStringOption(IReadOnlyCollection<string> aliases, string defaultValue) : StringOption(aliases), IOptionalOption<string> {
        string IOptionalOption<string>.DefaultValue => defaultValue;
    }
}
