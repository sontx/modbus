using System.Threading.Tasks;

namespace Modbus.Core
{
    /// <summary>
    /// http://www.simplymodbus.ca
    /// </summary>
    public interface IModbusProtocol
    {
        byte[] SendForResult(byte[] requestBytes, int responseBytesLength);

        Task<byte[]> SendForResultAsync(byte[] requestBytes, int responseBytesLength);
    }
}