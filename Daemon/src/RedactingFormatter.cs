namespace StonebotDaemon {
    using Serilog.Events;
    using Serilog.Formatting;
    using System.IO;
    using System.Text.RegularExpressions;

    internal sealed partial class RedactingFormatter(ITextFormatter innerFormatter) : ITextFormatter {
        private readonly ITextFormatter _innerFormatter = innerFormatter;
        private static readonly Regex _authRegex = GetAuthRegex();

        public void Format(LogEvent logEvent, TextWriter output) {
            using var writer = new StringWriter();
            _innerFormatter.Format(logEvent, writer);
            var message = writer.ToString();
            var redactedMessage = _authRegex.Replace(message, "***REDACTED***");
            output.Write(redactedMessage);
        }

        [GeneratedRegex(@"(?<=code=|state=|client_secret=|refresh_token=|""access_token"":""|""refresh_token"":"")[a-zA-Z0-9]+", RegexOptions.Compiled)]
        private static partial Regex GetAuthRegex();
    }
}
