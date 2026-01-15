namespace StonebotCLI {
    using StonebotCLI.Commands;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class Program {
        private static readonly BaseCommand _baseCommand = new();

        public static async Task<int> Main(string[] args) {
            using var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) => {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            try {
                await _baseCommand.HandleInputAsync(new(args), cancellationTokenSource.Token).ConfigureAwait(false);
                return 0;
            } catch (OperationCanceledException) {
                return 130;
            } catch (Exception e) {
                Console.Error.WriteLine(e.Message);
                return 1;
            }
        }
    }
}
