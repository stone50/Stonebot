namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class StartCommand() : Command(
        aliases: ["start", "run"],
        options: [
            new PortOption(),
            new StringOption(
                aliases: ["--daemon-file-path", "-f"],
                defaultValue: "StonebotDaemon"
            ),
        ],
        subCommands: []
    ) {
        protected override Task ExecuteAsync(
            ArgReader argReader,
            ReadOnlyDictionary<Option, string> options,
            Command? subCommand,
            CancellationToken cancellationToken
        ) {
            var port = GetValueOptionValue<PortOption, int>(_options[0], options);
            var daemonFilePath = GetRefOptionValue<StringOption, string>(_options[1], options);
            var daemonProcess = Process.Start(new ProcessStartInfo {
                FileName = daemonFilePath,
                Arguments = $"--urls=http://localhost:{port}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            });
            daemonProcess?.StandardOutput.Close();
            daemonProcess?.StandardError.Close();
            return Task.CompletedTask;
        }
    }
}
