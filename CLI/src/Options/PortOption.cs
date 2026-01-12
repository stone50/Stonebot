namespace StonebotCLI.Options {
    using StonebotSharedConstants;

    internal class PortOption() : IntOption(
        aliases: ["--port", "-p"],
        defaultValue: Port.Default
    ) { }
}
