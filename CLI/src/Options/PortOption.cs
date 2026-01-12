namespace StonebotCLI.Options {
    internal class PortOption() : IntOption(
        aliases: ["--port", "-p"],
        defaultValue: 57043
    ) { }
}
