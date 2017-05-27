using System;
using System.Threading.Tasks;

namespace Modbus.Core
{
    public sealed class ModbusFwdSession : IModbusSession
    {
        private readonly IModbusSession _modbusSession;
        private readonly Settings _settings;

        public ModbusFwdSession(IModbusProtocol modbusProtocol, Settings settings)
        {
            _modbusSession = new ModbusTcpSession(modbusProtocol, _settings.ParentSlaveAddress);
            _settings = settings;

            if (settings.ParentSlaveAddress == Constants.UndefinedSlaveAddress)
                throw new ArgumentException("ParentSlaveAddress must be defined", nameof(settings));
        }

        public Response<T> SendRequest<T>(int functionCode, object data) where T : struct
        {
            var builder = new RtuRequest.Builder()
                .SetFunctionCode(functionCode)
                .SetObject(data);

            return SendRequest<T>(builder);
        }

        public Response<T> SendRequest<T>(Request.BuilderBase builder) where T : struct
        {
            if (_settings.ChildSlaveAddress != Constants.UndefinedSlaveAddress)
                builder.SetSlaveAddress(_settings.ChildSlaveAddress);
            var requestBytes = builder.Build().RequestBytes;

            var parentBuilder = new TcpRequest.Builder()
                .SetFunctionCode(_settings.ParentFunctionCode)
                .SetDataBytes(requestBytes);

            return _modbusSession.SendRequest<T>(parentBuilder);
        }

        public Task<Response<T>> SendRequestAsync<T>(int functionCode, object data) where T : struct
        {
            return Task.Run(() => SendRequest<T>(functionCode, data));
        }

        public Task<Response<T>> SendRequestAsync<T>(Request.BuilderBase builder) where T : struct
        {
            return Task.Run(() => SendRequest<T>(builder));
        }

        public void Dispose()
        {
            _modbusSession?.Dispose();
        }

        public class Settings
        {
            public int ParentSlaveAddress { get; set; } = Constants.UndefinedSlaveAddress;
            public int ChildSlaveAddress { get; set; } = Constants.UndefinedSlaveAddress;
            public int ParentFunctionCode { get; set; } = Constants.DefaultForwardFuncCode;
        }
    }
}