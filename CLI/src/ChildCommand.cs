namespace StonebotCLI {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed partial class ChildCommand(
        IReadOnlyCollection<string> aliases,
        IReadOnlyCollection<Option> options,
        Func<ChildCommand, IReadOnlyDictionary<Option, string>, CancellationToken, Task<Error?>> executeAsync
    ) : Command(aliases) {
        internal readonly IReadOnlyCollection<Option> Options = options;
        private static readonly Regex _optionRegex = OptionRegex();

        internal bool TryParseOptionValue<TValue>(int optionIndex, IReadOnlyDictionary<Option, string> optionMap, [MaybeNullWhen(false)] out TValue value, [MaybeNullWhen(true)] out Error error) {
            var option = Options.ElementAt(optionIndex);
            if (option is not Option<TValue> typedOption) {
                throw new ArgumentException($"Invalid option type. Expected \"{typeof(Option<TValue>)}\" or derived, got \"{option.GetType()}\".");
            }

            if (optionMap.TryGetValue(option, out var valueString)) {
                return typedOption.TryParseValue(valueString, out value, out error);
            }

            if (option is not IOptionalOption<TValue> optionalOption) {
                value = default;
                error = new(ErrorCode.MissingOption, $"Command `{Aliases.ElementAt(0)}` requires option `{option.Aliases.ElementAt(0)}`");
                return false;
            }

            value = optionalOption.DefaultValue;
            error = default;
            return true;
        }

        internal override async Task<Error?> ExecuteAsync(ArgReader argReader, CancellationToken cancellationToken) {
            var optionMap = new Dictionary<Option, string>();
            while (argReader.TryRead(out var optionString)) {
                var match = _optionRegex.Match(optionString);
                if (!match.Success) {
                    return new(ErrorCode.InvalidOptionFormat, $"Option `{optionString}` does not match the format <option>=<value>");
                }

                var optionName = match.Groups[1].Value;
                var valueString = match.Groups[2].Value;
                var option = Options.FirstOrDefault(opt => opt.Aliases.Contains(optionName));
                if (option == null) {
                    return new(ErrorCode.InvalidOption, $"Option `{optionName}` is not valid for command `{Aliases.ElementAt(0)}`\nValid options:\n{string.Join('\n', Options.Select(opt => string.Join(", ", opt.Aliases)))}");
                }

                optionMap.Add(option, valueString);
            }

            return await executeAsync(this, new ReadOnlyDictionary<Option, string>(optionMap), cancellationToken);
        }

        [GeneratedRegex(@"(-.+?)=(.+)", RegexOptions.Compiled)]
        private static partial Regex OptionRegex();
    }
}
