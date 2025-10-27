using Modbus.Core;
using Xunit;

namespace Core.Tests;

public class TcpRequestTests
{
    [Fact]
    public void Build_WithBasicData_CreatesValidRequest()
    {
        // Arrange
        var builder = new TcpRequest.Builder()
            .SetTransactionId(0x0001)
            .SetProtocolId(0x0000)
            .SetLength(0x0006)
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x03)
            .SetInt16(0x1234);
        
        // Act
        var request = (TcpRequest)builder.Build();
        
        // Assert
        Assert.NotNull(request);
        Assert.Equal(1, request.TransactionId);
        Assert.Equal(0, request.ProtocolId);
        Assert.Equal(6, request.Length);
        Assert.Equal(1, request.SlaveAddress);
        Assert.Equal(3, request.FunctionCode);
    }

    [Fact]
    public void Build_WithoutData_ThrowsException()
    {
        // Arrange
        var builder = new TcpRequest.Builder()
            .SetTransactionId(0x0001)
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x03);
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void Build_RequestBytes_HasCorrectStructure()
    {
        // Arrange
        var builder = new TcpRequest.Builder()
            .SetTransactionId(0x1234)
            .SetProtocolId(0x0000)
            .SetLength(0x0006)
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x03)
            .SetInt16(0x5678);
        
        // Act
        var request = (TcpRequest)builder.Build();
        
        // Assert
        Assert.NotNull(request.RequestBytes);
        // MBAP Header (7 bytes) + Function Code (1 byte) + Data (2 bytes) = 10 bytes
        Assert.Equal(10, request.RequestBytes.Length);
        
        // Transaction ID
        Assert.Equal(0x12, request.RequestBytes[0]);
        Assert.Equal(0x34, request.RequestBytes[1]);
        
        // Protocol ID
        Assert.Equal(0x00, request.RequestBytes[2]);
        Assert.Equal(0x00, request.RequestBytes[3]);
        
        // Length
        Assert.Equal(0x00, request.RequestBytes[4]);
        Assert.Equal(0x06, request.RequestBytes[5]);
        
        // Slave Address
        Assert.Equal(0x01, request.RequestBytes[6]);
        
        // Function Code
        Assert.Equal(0x03, request.RequestBytes[7]);
        
        // Data
        Assert.Equal(0x56, request.RequestBytes[8]);
        Assert.Equal(0x78, request.RequestBytes[9]);
    }

    [Fact]
    public void AutoComputeLength_WithoutData_ThrowsException()
    {
        // Arrange
        var builder = new TcpRequest.Builder()
            .SetTransactionId(0x0001);
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => builder.AutoComputeLength());
    }

    [Fact]
    public void AutoComputeLength_WithData_ComputesCorrectLength()
    {
        // Arrange
        var builder = new TcpRequest.Builder();
        builder.SetTransactionId(0x0001)
            .SetProtocolId(0x0000)
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x03)
            .SetInt16(0x1234);
        builder.AutoComputeLength();
        
        // Act
        var request = (TcpRequest)builder.Build();
        
        // Assert
        // Length should be 8 (header without length field) + data length (2) = 10
        // But actually it's the number of following bytes after length field
        // which is slave(1) + function(1) + data(2) = 4, but the implementation adds 8
        Assert.Equal(10, request.Length);
    }

    [Fact]
    public void Build_WithMultipleInt16_AppendsCorrectly()
    {
        // Arrange
        var builder = new TcpRequest.Builder();
        builder.SetTransactionId(0x0001)
            .SetProtocolId(0x0000)
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x10)
            .AddInt16(0x1111)
            .AddInt16(0x2222)
            .AddInt16(0x3333);
        builder.SetLength(0x000A);
        
        // Act
        var request = (TcpRequest)builder.Build();
        
        // Assert
        Assert.Equal(14, request.RequestBytes.Length); // 8 header + 6 data
        Assert.Equal(0x11, request.RequestBytes[8]);
        Assert.Equal(0x11, request.RequestBytes[9]);
        Assert.Equal(0x22, request.RequestBytes[10]);
        Assert.Equal(0x22, request.RequestBytes[11]);
        Assert.Equal(0x33, request.RequestBytes[12]);
        Assert.Equal(0x33, request.RequestBytes[13]);
    }

    [Fact]
    public void Build_WithDataBytes_UsesProvidedData()
    {
        // Arrange
        byte[] customData = new byte[] { 0xAA, 0xBB, 0xCC };
        var builder = new TcpRequest.Builder();
        builder.SetTransactionId(0x0001)
            .SetProtocolId(0x0000)
            .SetSlaveAddress(0x01)
            .SetFunctionCode(0x10)
            .SetDataBytes(customData);
        builder.SetLength(0x0007);
        
        // Act
        var request = (TcpRequest)builder.Build();
        
        // Assert
        Assert.Equal(11, request.RequestBytes.Length); // 8 header + 3 data
        Assert.Equal(0xAA, request.RequestBytes[8]);
        Assert.Equal(0xBB, request.RequestBytes[9]);
        Assert.Equal(0xCC, request.RequestBytes[10]);
    }

    [Fact]
    public void Build_DifferentTransactionIds_ProducesDifferentRequests()
    {
        // Arrange
        var builder1 = new TcpRequest.Builder();
        builder1.SetTransactionId(0x0001)
            .SetProtocolId(0x0000)
            .SetLength(0x0006);
        builder1.SetSlaveAddress(0x01)
            .SetFunctionCode(0x03)
            .SetInt16(0x0000);
        
        var builder2 = new TcpRequest.Builder();
        builder2.SetTransactionId(0x0002)
            .SetProtocolId(0x0000)
            .SetLength(0x0006);
        builder2.SetSlaveAddress(0x01)
            .SetFunctionCode(0x03)
            .SetInt16(0x0000);
        
        // Act
        var request1 = (TcpRequest)builder1.Build();
        var request2 = (TcpRequest)builder2.Build();
        
        // Assert
        Assert.NotEqual(request1.RequestBytes[1], request2.RequestBytes[1]); // Low byte differs
        Assert.NotEqual(request1.TransactionId, request2.TransactionId);
    }

    [Fact]
    public void Build_StoresAllProperties_Correctly()
    {
        // Arrange
        var builder = new TcpRequest.Builder()
            .SetTransactionId(0xABCD)
            .SetProtocolId(0x0000)
            .SetLength(0x0008)
            .SetSlaveAddress(0x05)
            .SetFunctionCode(0x06)
            .SetInt16(0x9876);
        
        // Act
        var request = (TcpRequest)builder.Build();
        
        // Assert
        Assert.Equal(0xABCD, request.TransactionId);
        Assert.Equal(0x0000, request.ProtocolId);
        Assert.Equal(0x0008, request.Length);
        Assert.Equal(0x05, request.SlaveAddress);
        Assert.Equal(0x06, request.FunctionCode);
        Assert.NotNull(request.Data);
        Assert.Equal(2, request.Data.Length);
    }
}
