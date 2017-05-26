using System.Threading.Tasks;
using Modbus.Core.Exceptions;

namespace Modbus.Core
{
    public sealed class ModbusRtuSession : IModbusSession
    {
        private readonly IModbusProtocol _modbusProtocol;

        public ModbusRtuSession(IModbusProtocol modbusProtocol)
        {
            _modbusProtocol = modbusProtocol;
        }

        public Response<T> SendRequest<T>(int slaveAddress, int functionCode, object data) where T : struct
        {
            var request = new RtuRequest.Builder()
                .SetSlaveAddress(slaveAddress)
                .SetFunctionCode(functionCode)
                .SetObject(data)
                .Build();

            var responseBytes =
                _modbusProtocol.SendForResult(request.RequestBytes, RtuResponse<T>.ComputeResponseBytesLength());

            if (responseBytes == null)
                return null;

            var response = new RtuResponse<T>.Builder()
                .SetResponseBytes(responseBytes)
                .Build();

            CheckResponse(slaveAddress, functionCode, responseBytes, response);

            return response;
        }

        public Task<Response<T>> SendRequestAsync<T>(int slaveAddress, int functionCode, object data) where T : struct
        {
            return Task.Run(() => SendRequest<T>(slaveAddress, functionCode, data));
        }

        private void CheckResponse<T>(int slaveAddress, int functionCode, byte[] responseBytes, Response<T> response)
            where T : struct
        {
            Checksum<T>(responseBytes);

            if (slaveAddress != response.SlaveAddress)
                throw new MismatchDataException("Response slave address mismatch with " + slaveAddress);
            if (functionCode != response.FunctionCode)
                throw new MismatchDataException("Response function code mismatch with " + functionCode);
        }

        private void Checksum<T>(byte[] responseBytes) where T : struct
        {
            var crc16 = Core.Checksum.ComputeCrc16(responseBytes, 0, responseBytes.Length - 2);
            if (crc16[0] != responseBytes[responseBytes.Length - 2] || crc16[1] != responseBytes[responseBytes.Length - 1])
                throw new DataCorruptedException("Checksum fail");
        }
    }
}