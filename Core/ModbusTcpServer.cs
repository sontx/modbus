using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Modbus.Core
{
    public sealed class ModbusTcpServer : IDisposable
    {
        private TcpListener _tcpListener;

        public ModbusTcpServer(string address, int port)
        {
            var endPoint = new IPEndPoint(string.IsNullOrEmpty(address) ? IPAddress.Any : IPAddress.Parse(address), port);
            _tcpListener = new TcpListener(endPoint);
        }

        public Task WaitForConnectionsAsync(Action<IModbusSession> onAcceptAction, HandshakeFunc<DefaultHandshake> handshakeFunc)
        {
            return Task.Run(async () =>
            {
                var listner = _tcpListener;
                if (listner == null || onAcceptAction == null || handshakeFunc == null)
                    return;

                listner.Start();
                while (true)
                {
                    var tcpClient = await listner.AcceptTcpClientAsync();
                    var modbusSession = ModbusSessionFactory.CreateTcpSession(tcpClient);
                    var handshake = handshakeFunc.Invoke(modbusSession);
                    if (handshake != null)
                    {
                        ((ModbusTcpSession)modbusSession).SetSlaveAddress(handshake.SlaveAddress);
                        onAcceptAction.Invoke(modbusSession);
                    }
                    else
                    {
                        modbusSession.Dispose();
                    }
                }
            });
        }

        public void Dispose()
        {
            _tcpListener.Stop();
            _tcpListener = null;
        }
    }
}