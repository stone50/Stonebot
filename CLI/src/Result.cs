namespace StonebotCLI {
    internal enum ErrorCode {
        MissingSubCommand,
        InvalidSubCommand,
        MissingOption,
        InvalidOptionFormat,
        InvalidOption,
        InvalidOptionValue,
        CommandExecutionFailed,
    }

    internal sealed class Error(ErrorCode code, string message) {
        internal readonly ErrorCode Code = code;
        internal readonly string Message = message;
    }
}
