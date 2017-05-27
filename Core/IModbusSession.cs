using System;
using System.Threading.Tasks;

namespace Modbus.Core
{
    public interface IModbusSession : IDisposable
    {
        Response<T> SendRequest<T>(int slaveAddress, int functionCode, object data) where T : struct;

        Response<T> SendRequest<T>(Request.BuilderBase builder) where T : struct;

        Task<Response<T>> SendRequestAsync<T>(int slaveAddress, int functionCode, object data) where T : struct;

        Task<Response<T>> SendRequestAsync<T>(Request.BuilderBase builder) where T : struct;
    }
}