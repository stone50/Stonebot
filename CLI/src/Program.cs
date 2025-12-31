namespace StonebotCLI {
    using System;
    using System.Threading.Tasks;

    public static class Program {
        public static async Task Main(string[] args) {
            Console.WriteLine("Welcome to Stonebot!");
            Console.WriteLine("Use `quit` or `exit` to exit");
            Console.WriteLine();
            while (true) {
                Console.Write(">Stonebot:");

                var readLine = Console.ReadLine();
                if (readLine == null) {
                    break;
                }

                var input = readLine.Trim();
                if (Console.IsInputRedirected) {
                    Console.WriteLine(input);
                }

                if (
                    input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("exit", StringComparison.OrdinalIgnoreCase)
                ) {
                    break;
                }

                // TODO: process input
            }
        }
    }
}
