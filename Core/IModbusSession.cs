namespace Modbus.Core
{
    public interface IModbusSession
    {
        Response<T> SendRequest<T>(int slaveAddress, int functionCode, object data) where T : struct;
    }
}