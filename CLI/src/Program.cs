namespace StonebotCLI {
    using StonebotCLI.Commands;
    using System;

    public static class Program {
        private static readonly BaseCommand baseCommand = new();

        public static void Main(string[] args) {
            Console.WriteLine("Welcome to Stonebot!");
            Console.WriteLine("To quit, use one of:\nq, quit, exit");
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
                    input.Equals("q", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("exit", StringComparison.OrdinalIgnoreCase)
                ) {
                    break;
                }

                try {
                    baseCommand.HandleInput(new(input));
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
