using Modbus.Core.Streams;
using System.IO.Ports;
using System.Net.Sockets;

namespace Modbus.Core
{
    public static class ModbusSessionFactory
    {
        public static IModbusSession CreateRtuSession(SerialPort serialPort)
        {
            return new ModbusRtuSession(new ModbusProtocolImpl(new SerialStream(serialPort)));
        }

        public static IModbusSession CreateTcpSession(TcpClient tcpClient)
        {
            return new ModbusTcpSession(new ModbusProtocolImpl(new TcpStream(tcpClient)));
        }
    }
}