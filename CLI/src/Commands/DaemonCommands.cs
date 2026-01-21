namespace StonebotCLI.Commands {
    using StonebotCLI.Options;
    using StonebotSharedConstants;
    using System;
    using System.Diagnostics;
    using System.Net.Http;

    internal static class DaemonCommands {
        internal static ChildCommand GetStartCommand(OptionalIntOption portOption) => new(
            aliases: ["start", "run"],
            options: [
                portOption,
                new OptionalStringOption(
                    aliases: ["--daemon-file-path", "--file", "-f"],
                    defaultValue: "StonebotDaemon"
                ),
            ],
            async (childCommand, optionMap, cancellationToken) => {
                if (!childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)) {
                    return error;
                }

                if (!childCommand.TryParseOptionValue<string>(1, optionMap, out var daemonFilePath, out error)) {
                    return error;
                }

                Process? daemonProcess;
                try {
                    daemonProcess = Process.Start(new ProcessStartInfo {
                        FileName = daemonFilePath,
                        Arguments = $"--urls=http://localhost:{port}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    });
                } catch (Exception e) {
                    return new(ErrorCode.CommandExecutionFailed, e.Message);
                }

                daemonProcess?.StandardOutput.Close();
                daemonProcess?.StandardError.Close();
                return null;
            }
        );

        internal static ChildCommand GetStatusCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["status", "health"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendGetRequestAsync(httpClient, port, EndpointPaths.GetHealth, cancellationToken));

        internal static ChildCommand GetStopCommand(HttpClient httpClient, OptionalIntOption portOption) => new(
            aliases: ["stop", "exit", "quit", "close"],
            options: [portOption],
            async (childCommand, optionMap, cancellationToken) =>
                !childCommand.TryParseOptionValue<int>(0, optionMap, out var port, out var error)
                ? error
                : await Utils.SendPostRequestAsync(httpClient, port, EndpointPaths.PostStop, null, cancellationToken));
    }
}
