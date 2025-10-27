using CommandLine;

namespace Modbus.Terminal
{
    internal class PortOptions
    {
        [Option("com", Required = true, HelpText = "COM name")]
        public string Com { get; set; }

        [Option("baud", Default = 9600, HelpText = "Baud rate")]
        public int BaudRate { get; set; }

        [Option("interval", Default = 1000, HelpText = "Loop every in milliseconds")]
        public int Interval { get; set; }
    }
}