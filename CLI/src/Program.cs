namespace StonebotCLI {
    using StonebotCore.Twitch;
    using System;

    public static class Program {
        public static void Main(string[] args) {
            Console.WriteLine("Hello, world! This is StonebotCLI!");

            Auth.AuthorizeAsync().GetAwaiter().GetResult();
        }
    }
}
