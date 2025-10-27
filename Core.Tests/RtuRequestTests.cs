using Modbus.Core;
using Xunit;

namespace Core.Tests;

public class RtuRequestTests
{
    [Fact]
    public void Build_WithBasicData_CreatesValidRequest()
    {
        // Arrange
        var builder = new RtuRequest.Builder()
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x03)
            .SetInt16(0x1234);
        
        // Act
        var request = builder.Build();
        
        // Assert
        Assert.NotNull(request);
        Assert.Equal(1, request.SlaveAddress);
        Assert.Equal(3, request.FunctionCode);
        Assert.NotNull(request.Data);
        Assert.Equal(2, request.Data.Length);
    }

    [Fact]
    public void Build_WithoutData_ThrowsException()
    {
        // Arrange
        var builder = new RtuRequest.Builder()
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x03);
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Build_RequestBytes_IncludesCrc16()
    {
        // Arrange
        var builder = new RtuRequest.Builder()
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x03)
            .SetInt16(0x0000);
        
        // Act
        var request = builder.Build();
        
        // Assert
        Assert.NotNull(request.RequestBytes);
        // Should be: slave(1) + function(1) + data(2) + crc(2) = 6 bytes
        Assert.Equal(6, request.RequestBytes.Length);
        Assert.Equal(0x01, request.RequestBytes[0]); // Slave address
        Assert.Equal(0x03, request.RequestBytes[1]); // Function code
    }

    [Fact]
    public void Build_WithSetDataBytes_UsesProvidedData()
    {
        // Arrange
        byte[] customData = new byte[] { 0x11, 0x22, 0x33, 0x44 };
        var builder = new RtuRequest.Builder()
            .SetSlaveAddress(0x05)
            .SetFunctionCode(0x10)
            .SetDataBytes(customData);
        
        // Act
        var request = builder.Build();
        
        // Assert
        Assert.Equal(customData, request.Data);
        Assert.Equal(8, request.RequestBytes.Length); // 1+1+4+2
    }

    [Fact]
    public void Build_WithAddInt16_AppendsMultipleValues()
    {
        // Arrange
        var builder = new RtuRequest.Builder()
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x10)
            .AddInt16(0x1111)
            .AddInt16(0x2222)
            .AddInt16(0x3333);
        
        // Act
        var request = builder.Build();
        
        // Assert
        Assert.NotNull(request.Data);
        Assert.Equal(6, request.Data.Length);
        // Check big-endian encoding
        Assert.Equal(0x11, request.Data[0]);
        Assert.Equal(0x11, request.Data[1]);
        Assert.Equal(0x22, request.Data[2]);
        Assert.Equal(0x22, request.Data[3]);
        Assert.Equal(0x33, request.Data[4]);
        Assert.Equal(0x33, request.Data[5]);
    }

    [Fact]
    public void Build_SetInt16_StoresInBigEndian()
    {
        // Arrange
        var builder = new RtuRequest.Builder()
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x03)
            .SetInt16(0x1234);
        
        // Act
        var request = builder.Build();
        
        // Assert
        Assert.Equal(0x12, request.Data[0]); // High byte first
        Assert.Equal(0x34, request.Data[1]); // Low byte second
    }

    [Fact]
    public void Build_CrcValidation_ProducesCorrectCrc()
    {
        // Arrange - known modbus message
        var builder = new RtuRequest.Builder()
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x03)
            .SetInt16(0x0000)
            .AddInt16(0x0002);
        
        // Act
        var request = builder.Build();
        
        // Assert
        // For message: 01 03 00 00 00 02, CRC should be C4 0B
        Assert.Equal(8, request.RequestBytes.Length);
        Assert.Equal(0xC4, request.RequestBytes[6]); // CRC low byte
        Assert.Equal(0x0B, request.RequestBytes[7]); // CRC high byte
    }

    [Fact]
    public void Builder_SetSlaveAddress_SetsCorrectValue()
    {
        // Arrange & Act
        var builder = new RtuRequest.Builder()
            .SetSlaveAddress(0x42);
        
        // Assert
        Assert.Equal(0x42, builder.SlaveAddress);
    }

    [Fact]
    public void Builder_SetFunctionCode_SetsCorrectValue()
    {
        // Arrange & Act
        var builder = new RtuRequest.Builder()
            .SetFunctionCode(0x06);
        
        // Assert
        Assert.Equal(0x06, builder.FunctionCode);
    }

    [Fact]
    public void Build_DifferentSlaveAddresses_ProducesDifferentRequests()
    {
        // Arrange
        var builder1 = new RtuRequest.Builder()
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x03)
            .SetInt16(0x0000);
        
        var builder2 = new RtuRequest.Builder()
            .SetSlaveAddress(0x02)
            .SetFunctionCode(0x03)
            .SetInt16(0x0000);
        
        // Act
        var request1 = builder1.Build();
        var request2 = builder2.Build();
        
        // Assert
        Assert.NotEqual(request1.RequestBytes[0], request2.RequestBytes[0]);
        // CRC should also be different
        Assert.False(request1.RequestBytes.SequenceEqual(request2.RequestBytes));
    }
}
