namespace StonebotCLI {
    internal sealed class ArgReader {
        private readonly string[] _args;
        private int _index = 0;

        internal ArgReader(string[] args) => _args = args;

        internal string? Peek() => _index < _args.Length ? _args[_index] : null;

        internal string? Read() => _index < _args.Length ? _args[_index++] : null;

        internal void Skip() {
            if (_index < _args.Length) {
                ++_index;
            }
        }
    }
}
