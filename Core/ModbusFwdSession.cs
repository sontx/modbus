using System;
using System.Threading.Tasks;

namespace Modbus.Core
{
    public sealed class ModbusFwdSession : IModbusSession
    {
        private readonly IModbusSession _modbusSession;
        private readonly Settings _settings;

        public SessionState State { get; private set; }

        public ModbusFwdSession(IModbusProtocol modbusProtocol, Settings settings)
        {
            _modbusSession = new ModbusTcpSession(modbusProtocol, _settings.ParentSlaveAddress);
            _settings = settings;

            if (settings.ParentSlaveAddress == Constants.UndefinedSlaveAddress)
                throw new ArgumentException("ParentSlaveAddress must be defined", nameof(settings));

            State = settings.ChildSlaveAddress != Constants.UndefinedSlaveAddress
                ? SessionState.Identified
                : SessionState.Unidentified;
        }

        public Response<T> SendRequest<T>(int functionCode, object data) where T : struct
        {
            if (State == SessionState.Unidentified)
                throw new InvalidOperationException("Slave address must be defined");

            var builder = new RtuRequest.Builder()
                .SetFunctionCode(functionCode)
                .SetObject(data);

            return SendRequest<T>(builder);
        }

        public Response<T> SendRequest<T>(Request.BuilderBase builder) where T : struct
        {
            if (State == SessionState.Expired)
                throw new InvalidOperationException("This session already expired");

            if (State == SessionState.Identified)
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
            State = SessionState.Expired;
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