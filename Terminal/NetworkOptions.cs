using CommandLine;

namespace Modbus.Terminal
{
    internal class NetworkOptions
    {
        [Option("address", Required = true, HelpText = "Listen address")]
        public string Address { get; set; }

        [Option("port", Default = 3393, HelpText = "Listen port")]
        public int Port { get; set; }

        [Option("interval", Default = 1000, HelpText = "Loop every in milliseconds")]
        public int Interval { get; set; }
    }
}