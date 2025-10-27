using Modbus.Core;
using Xunit;

namespace Core.Tests;

public class ChecksumTests
{
    [Fact]
    public void ComputeCrc16_WithValidMessage_ReturnsCorrectCrc()
    {
        // Arrange
        byte[] message = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x02 };
        
        // Act
        byte[] crc = Checksum.ComputeCrc16(message, 0, message.Length);
        
        // Assert
        Assert.NotNull(crc);
        Assert.Equal(2, crc.Length);
        // Expected CRC16 for this message is 0xC40B (low byte first)
        Assert.Equal(0xC4, crc[0]);
        Assert.Equal(0x0B, crc[1]);
    }

    [Fact]
    public void ComputeCrc16_WithDifferentMessage_ReturnsDifferentCrc()
    {
        // Arrange
        byte[] message1 = new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x02 };
        byte[] message2 = new byte[] { 0x01, 0x04, 0x00, 0x00, 0x00, 0x02 };
        
        // Act
        byte[] crc1 = Checksum.ComputeCrc16(message1, 0, message1.Length);
        byte[] crc2 = Checksum.ComputeCrc16(message2, 0, message2.Length);
        
        // Assert
        Assert.NotEqual(crc1[0], crc2[0]);
    }

    [Fact]
    public void ComputeCrc16_WithOffset_ComputesFromOffset()
    {
        // Arrange
        byte[] message = new byte[] { 0xFF, 0xFF, 0x01, 0x03, 0x00, 0x00, 0x00, 0x02 };
        
        // Act - length parameter is exclusive upper bound
        byte[] crc = Checksum.ComputeCrc16(message, 2, message.Length);
        
        // Assert
        Assert.NotNull(crc);
        Assert.Equal(2, crc.Length);
        // Should be same as message without leading 0xFF bytes
        Assert.Equal(0xC4, crc[0]);
        Assert.Equal(0x0B, crc[1]);
    }

    [Fact]
    public void ComputeCrc16_WithSingleByte_ReturnsValidCrc()
    {
        // Arrange
        byte[] message = new byte[] { 0x01 };
        
        // Act
        byte[] crc = Checksum.ComputeCrc16(message, 0, message.Length);
        
        // Assert
        Assert.NotNull(crc);
        Assert.Equal(2, crc.Length);
    }

    [Fact]
    public void ComputeCrc16_WithEmptyRange_ReturnsInitialCrc()
    {
        // Arrange
        byte[] message = new byte[] { 0x01, 0x02, 0x03 };
        
        // Act
        byte[] crc = Checksum.ComputeCrc16(message, 0, 0);
        
        // Assert
        Assert.NotNull(crc);
        Assert.Equal(2, crc.Length);
        // Initial CRC value is 0xFFFF
        Assert.Equal(0xFF, crc[0]);
        Assert.Equal(0xFF, crc[1]);
    }
}
