namespace StoneBot.Scripts.Command {
    using System;
    using System.Threading.Tasks;

    internal class Command {
        public string Keyword;
        public int UseDelay;
        public DateTime LastUsed { get; private set; }
        public Func<string, Task> UseAction;

        public Command(string keyword, Func<string, Task> useAction) {
            Keyword = keyword;
            UseAction = useAction;
        }

        public async Task Use(string message) {
            if (DateTime.Now < LastUsed.AddMilliseconds(UseDelay)) {
                return;
            }

            LastUsed = DateTime.Now;
            await UseAction(message);
        }
    }
}
