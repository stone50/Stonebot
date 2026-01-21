namespace StonebotCLI {
    using StonebotCLI.Commands;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Program {
        public static async Task<int> Main(string[] args) {
            using var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (_, args) => {
                args.Cancel = true;
                try {
                    cancellationTokenSource.Cancel();
                } catch (Exception e) {
                    Console.Error.WriteLine($"Could not cancel: {e.Message}");
                }
            };
            var error = await BaseCommands.GetBaseCommand().ExecuteAsync(new(args), cancellationTokenSource.Token).ConfigureAwait(false);
            if (error == null) {
                return 0;
            }

            Console.WriteLine(error.Message);
            return error.Code == ErrorCode.CommandExecutionFailed ? 1 : 2;
        }
    }
}
