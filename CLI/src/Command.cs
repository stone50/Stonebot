namespace StonebotCLI {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    internal abstract partial class Command(string[] aliases, Option[] options, Command[] subCommands) {
        protected readonly string[] _aliases = aliases;
        protected readonly Option[] _options = options;
        private readonly Command[] _subCommands = subCommands;
        private static readonly Regex _optionRegex = OptionRegex();

        protected abstract Task ExecuteAsync(
            ArgReader argReader,
            ReadOnlyDictionary<Option, string> optionMap,
            Command? subCommand,
            CancellationToken cancellationToken
        );

        public Task HandleInputAsync(ArgReader argReader, CancellationToken cancellationToken) {
            var optionMap = ParseOptions(argReader);
            var subCommand = ParseSubCommand(argReader);
            return ExecuteAsync(argReader, optionMap, subCommand, cancellationToken);
        }

        protected static TValue GetRefOptionValue<TOption, TValue>(
             Option option,
             ReadOnlyDictionary<Option, string> options
         ) where TValue : class where TOption : RefOption<TValue> =>
             ((TOption)option).GetValue(options);

        protected static TValue GetValueOptionValue<TOption, TValue>(
            Option option,
            ReadOnlyDictionary<Option, string> options
        ) where TValue : struct where TOption : ValueOption<TValue> =>
            ((TOption)option).GetValue(options);

        private ReadOnlyDictionary<Option, string> ParseOptions(ArgReader argReader) {
            var optionMap = new Dictionary<Option, string>();
            while (argReader.Peek()?.StartsWith('-') ?? false) {
                var option = ParseOption(argReader);
                optionMap.Add(option.Key, option.Value);
            }

            return new(optionMap);
        }

        private KeyValuePair<Option, string> ParseOption(ArgReader argReader) {
            var input = argReader.Read()!;
            var match = _optionRegex.Match(input);
            if (!match.Success) {
                throw new Exception(
                    $"`{input}` is not a valid option format. " +
                    "The option must be of the format <option name>=<option value>"
                );
            }

            var option = GetOptionFromAlias(match.Groups[1].Value);
            return new(option, match.Groups[2].Value);
        }

        private Option GetOptionFromAlias(string alias) {
            foreach (var option in _options) {
                if (Array.Exists(option.Aliases, a => a.Equals(alias, StringComparison.OrdinalIgnoreCase))) {
                    return option;
                }
            }

            throw new Exception(
                $"`{alias}` is not a valid option for command `{_aliases[0]}`. " +
                $"The option must be one of:\n{GetAllOptionAliasesText()}"
            );
        }

        private string GetAllOptionAliasesText() {
            var builder = new StringBuilder();
            foreach (var option in _options) {
                _ = builder.Append(string.Join(", ", option.Aliases));
                _ = builder.Append('\n');
            }

            return builder.ToString();
        }

        private Command? ParseSubCommand(ArgReader argReader) {
            var alias = argReader.Read();
            if (alias == null) {
                return null;
            }

            foreach (var subCommand in _subCommands) {
                if (Array.Exists(subCommand._aliases, a => a.Equals(alias, StringComparison.OrdinalIgnoreCase))) {
                    return subCommand;
                }
            }

            throw new Exception(
                $"`{alias}` is not a valid sub-command for command `{_aliases[0]}`. " +
                $"The sub-command must be one of:\n{GetAllSubCommandAliasesText()}"
            );
        }

        protected string GetAllSubCommandAliasesText() {
            var builder = new StringBuilder();
            foreach (var subCommand in _subCommands) {
                _ = builder.Append(string.Join(", ", subCommand._aliases));
                _ = builder.Append('\n');
            }

            return builder.ToString();
        }

        [GeneratedRegex(@"(.*)=(.*)", RegexOptions.Compiled)]
        private static partial Regex OptionRegex();
    }
}
