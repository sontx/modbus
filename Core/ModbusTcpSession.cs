using Modbus.Core.Exceptions;
using System.Threading.Tasks;

namespace Modbus.Core
{
    public sealed class ModbusTcpSession : IModbusSession
    {
        private static readonly int PROTOCOL_ID = 0;

        private readonly IModbusProtocol _modbusProtocol;
        private ushort _transactionId;

        public ModbusTcpSession(IModbusProtocol modbusProtocol)
        {
            _modbusProtocol = modbusProtocol;
            _transactionId = 0;
        }

        public Response<T> SendRequest<T>(int slaveAddress, int functionCode, object data) where T : struct
        {
            var builder = (TcpRequest.Builder)new TcpRequest.Builder()
                .SetSlaveAddress(slaveAddress)
                .SetFunctionCode(functionCode)
                .SetObject(data);

            return SendRequest<T>(builder);
        }

        public Response<T> SendRequest<T>(Request.BuilderBase builder) where T : struct
        {
            var tcpBuilder = (TcpRequest.Builder)builder;

            tcpBuilder
                .SetTransactionId(_transactionId++)
                .SetProtocolId(PROTOCOL_ID)
                .AutoComputeLength();

            var request = tcpBuilder.Build();

            var responseBytes =
                _modbusProtocol.SendForResult(request.RequestBytes, TcpResponse<T>.ComputeResponseBytesLength());

            if (responseBytes == null)
                return null;

            var response = (TcpResponse<T>)new TcpResponse<T>.Builder()
                .SetResponseBytes(responseBytes)
                .Build();

            CheckResponse(request, response);

            return response;
        }

        public Task<Response<T>> SendRequestAsync<T>(int slaveAddress, int functionCode, object data) where T : struct
        {
            return Task.Run(() => SendRequest<T>(slaveAddress, functionCode, data));
        }

        public Task<Response<T>> SendRequestAsync<T>(Request.BuilderBase builder) where T : struct
        {
            return Task.Run(() => SendRequest<T>(builder));
        }

        private void CheckResponse<T>(Request request, TcpResponse<T> response) where T : struct
        {
            if (_transactionId != response.TransactionId)
                throw new DataCorruptedException("Response transaction id does not match " + _transactionId);
            if (response.ProtocolId != PROTOCOL_ID)
                throw new DataCorruptedException("Response protocol id does not match " + PROTOCOL_ID);
            if (response.ComputeMessageLength() != response.Length)
                throw new DataCorruptedException("Actual message bytes length does not match " + response.Length);
            if (request.SlaveAddress != response.SlaveAddress)
                throw new MismatchDataException("Response slave address mismatch " + request.SlaveAddress);
            if (request.FunctionCode != response.FunctionCode)
                throw new MismatchDataException("Response function code mismatch " + request.FunctionCode);
        }
    }
}