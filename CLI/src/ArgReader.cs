namespace StonebotCLI {
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    internal sealed class ArgReader(IReadOnlyCollection<string> args) {
        private readonly IReadOnlyCollection<string> _args = args;
        private int _index = 0;

        internal bool IsAtEndOfStream => _index >= _args.Count;

        internal bool TryRead([MaybeNullWhen(false)] out string value) {
            if (IsAtEndOfStream) {
                value = default;
                return false;
            }

            value = _args.ElementAt(_index++);
            return true;
        }
    }
}
