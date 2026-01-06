namespace StonebotCLI {
    internal abstract class Option(string[] aliases) {
        public readonly string[] Aliases = aliases;
    }

    internal abstract class Option<TValue>(string[] aliases, TValue defaultValue) : Option(aliases) {
        public readonly TValue DefaultValue = defaultValue;

        public abstract TValue GetValue(string valueString);
    }
}
