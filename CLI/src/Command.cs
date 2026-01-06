namespace StonebotCLI {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    internal abstract class Command(
        string[] aliases,
        Option[] options,
        Command[] subCommands
    ) {
        public readonly string[] Aliases = aliases;
        public readonly Option[] Options = options;
        public readonly Command[] SubCommands = subCommands;

        // TODO: have this return a task, and add the ability for the user to cancel
        protected abstract void Execute(
            StringReader input,
            Dictionary<Option, string> options,
            Command? subCommand
        );

        public void HandleInput(StringReader input) {
            var options = ParseOptions(input);
            var subCommand = ParseSubCommand(input);
            Execute(input, options, subCommand);
        }

        protected static TValue GetOptionValue<TValue>(
            Option<TValue> option,
            Dictionary<Option, string> inputOptionMap
        ) =>
            inputOptionMap.TryGetValue(option, out var valueString)
                ? option.GetValue(valueString)
                : option.DefaultValue;

        protected static TValue GetOptionValue<TValue>(
            Option option,
            Dictionary<Option, string> inputOptionMap
        ) =>
            inputOptionMap.TryGetValue(option, out var valueString)
                ? ((Option<TValue>)option).GetValue(valueString)
                : ((Option<TValue>)option).DefaultValue;

        private Dictionary<Option, string> ParseOptions(StringReader input) {
            var options = new Dictionary<Option, string>();
            while (input.Peek() == '-') {
                _ = input.Read();
                var option = ParseOption(input);
                options.Add(option.Key, option.Value);
            }

            return options;
        }

        private KeyValuePair<Option, string> ParseOption(StringReader input) {
            var key = ParseOptionKey(input);
            var option = GetOptionFromAlias(key);
            var optionValueString = ParseOptionValue(input);
            return new(option, optionValueString);
        }

        private string ParseOptionKey(StringReader input) {
            var keyBuilder = new StringBuilder();
            int next;
            while (true) {
                next = input.Read();
                if (next == -1) {
                    throw new Exception(
                        $"`{keyBuilder}` is not a valid option format for command `{Aliases[0]}`. " +
                        "The option must be in the format -<name>=<value>"
                    );
                }

                var c = (char)next;
                if (c == '=') {
                    break;
                }

                _ = keyBuilder.Append(c);
            }

            return keyBuilder.ToString();
        }

        private Option GetOptionFromAlias(string optionAlias) {
            foreach (var option in Options) {
                if (option.Aliases.Contains(optionAlias)) {
                    return option;
                }
            }

            throw new Exception(
                $"`{optionAlias}` is not a valid option for command `{Aliases[0]}`. " +
                $"The option must be one of:\n{GetAllOptionAliasesText()}"
            );
        }

        private string ParseOptionValue(StringReader input) {
            var valueBuilder = new StringBuilder();
            var isInsideQuote = false;
            var isEscaping = false;
            int next;
            while (true) {
                next = input.Read();
                if (next == -1) {
                    if (isInsideQuote) {
                        throw new Exception(
                            $"`{valueBuilder}` is not a valid option value for command `{Aliases[0]}`. " +
                            "The option value must have a closing quotation mark"
                        );
                    }

                    if (isEscaping) {
                        throw new Exception(
                            $"`{valueBuilder}` is not a valid option value for command `{Aliases[0]}`. " +
                            "The option value must escape one of:\n\"\n\\"
                        );
                    }

                    break;
                }

                var c = (char)next;
                if (isEscaping) {
                    if (c is not '"' and not '\\') {
                        throw new Exception(
                            $"`{valueBuilder}` is not a valid option format for command `{Aliases[0]}`. " +
                            "The option value must escape one of:\n\"\n\\"
                        );
                    }

                    _ = valueBuilder.Append(c);
                    isEscaping = false;
                    continue;
                }

                if (c == '\\') {
                    isEscaping = true;
                    continue;
                }

                if (c == '\"') {
                    isInsideQuote = !isInsideQuote;
                    continue;
                }

                if (!isInsideQuote && char.IsWhiteSpace(c)) {
                    break;
                }

                _ = valueBuilder.Append(c);
            }

            return valueBuilder.ToString();
        }

        private Command? ParseSubCommand(StringReader input) {
            var builder = new StringBuilder();
            int next;
            while (true) {
                next = input.Read();
                if (next == -1) {
                    break;
                }

                var c = (char)next;
                if (char.IsWhiteSpace(c)) {
                    break;
                }

                _ = builder.Append(c);
            }

            var subCommandName = builder.ToString();
            if (subCommandName == "") {
                return null;
            }

            foreach (var subCommand in SubCommands) {
                if (subCommand.Aliases.Contains(subCommandName)) {
                    return subCommand;
                }
            }

            throw new Exception(
                $"`{subCommandName}` is not a valid sub-command for command `{Aliases[0]}`. " +
                $"The sub-command must be one of:\n{GetAllSubCommandAliasesText()}"
            );
        }

        private string GetAllOptionAliasesText() {
            var builder = new StringBuilder();
            foreach (var option in Options) {
                _ = builder.Append(string.Join(", ", option.Aliases));
                _ = builder.Append('\n');
            }

            return builder.ToString();
        }

        private string GetAllSubCommandAliasesText() {
            var builder = new StringBuilder();
            foreach (var subCommand in SubCommands) {
                _ = builder.Append(string.Join(", ", subCommand.Aliases));
                _ = builder.Append('\n');
            }

            return builder.ToString();
        }
    }
}