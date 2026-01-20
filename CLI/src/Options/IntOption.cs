namespace StonebotCLI.Options {
    using StonebotCLI.OptionValueParsers;
    using System.Collections.Generic;

    internal class IntOption(IReadOnlyCollection<string> aliases) : Option<int>(aliases, OptionValueParsers.TryParseIntOptionValue) { }

    internal class OptionalIntOption(IReadOnlyCollection<string> aliases, int defaultValue) : IntOption(aliases), IOptionalOption<int> {
        int IOptionalOption<int>.DefaultValue => defaultValue;
    }
}
