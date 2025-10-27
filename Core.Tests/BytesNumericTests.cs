using Modbus.Core.Converters;
using Xunit;

namespace Core.Tests;

public class BytesNumericTests
{
    [Theory]
    [InlineData((short)0, new byte[] { 0x00, 0x00 })]
    [InlineData((short)1, new byte[] { 0x00, 0x01 })]
    [InlineData((short)256, new byte[] { 0x01, 0x00 })]
    [InlineData((short)-1, new byte[] { 0xFF, 0xFF })]
    [InlineData((short)0x1234, new byte[] { 0x12, 0x34 })]
    public void FromInt16_Signed_ConvertsToBigEndianBytes(short value, byte[] expected)
    {
        // Act
        byte[] result = BytesNumeric.FromInt16(value);
        
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((ushort)0, new byte[] { 0x00, 0x00 })]
    [InlineData((ushort)1, new byte[] { 0x00, 0x01 })]
    [InlineData((ushort)256, new byte[] { 0x01, 0x00 })]
    [InlineData((ushort)0xFFFF, new byte[] { 0xFF, 0xFF })]
    [InlineData((ushort)0x1234, new byte[] { 0x12, 0x34 })]
    public void FromInt16_Unsigned_ConvertsToBigEndianBytes(ushort value, byte[] expected)
    {
        // Act
        byte[] result = BytesNumeric.FromInt16(value);
        
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x00 }, 0, (short)0)]
    [InlineData(new byte[] { 0x00, 0x01 }, 0, (short)1)]
    [InlineData(new byte[] { 0x01, 0x00 }, 0, (short)256)]
    [InlineData(new byte[] { 0xFF, 0xFF }, 0, (short)-1)]
    [InlineData(new byte[] { 0x12, 0x34 }, 0, (short)0x1234)]
    public void ToInt16_ConvertsFromBigEndianBytes(byte[] bytes, int offset, short expected)
    {
        // Act
        short result = BytesNumeric.ToInt16(bytes, offset);
        
        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x00 }, 0, (ushort)0)]
    [InlineData(new byte[] { 0x00, 0x01 }, 0, (ushort)1)]
    [InlineData(new byte[] { 0x01, 0x00 }, 0, (ushort)256)]
    [InlineData(new byte[] { 0xFF, 0xFF }, 0, (ushort)0xFFFF)]
    [InlineData(new byte[] { 0x12, 0x34 }, 0, (ushort)0x1234)]
    public void ToUInt16_ConvertsFromBigEndianBytes(byte[] bytes, int offset, ushort expected)
    {
        // Act
        ushort result = BytesNumeric.ToUInt16(bytes, offset);
        
        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToInt16_WithOffset_ReadsFromCorrectPosition()
    {
        // Arrange
        byte[] bytes = new byte[] { 0xFF, 0xFF, 0x12, 0x34 };
        
        // Act
        short result = BytesNumeric.ToInt16(bytes, 2);
        
        // Assert
        Assert.Equal(0x1234, result);
    }

    [Fact]
    public void ToUInt16_WithOffset_ReadsFromCorrectPosition()
    {
        // Arrange
        byte[] bytes = new byte[] { 0xFF, 0xFF, 0x12, 0x34 };
        
        // Act
        ushort result = BytesNumeric.ToUInt16(bytes, 2);
        
        // Assert
        Assert.Equal(0x1234, result);
    }

    [Fact]
    public void FromInt16_ToInt16_RoundTrip_PreservesValue()
    {
        // Arrange
        short original = 12345;
        
        // Act
        byte[] bytes = BytesNumeric.FromInt16(original);
        short result = BytesNumeric.ToInt16(bytes, 0);
        
        // Assert
        Assert.Equal(original, result);
    }

    [Fact]
    public void FromInt16_ToUInt16_RoundTrip_PreservesValue()
    {
        // Arrange
        ushort original = 54321;
        
        // Act
        byte[] bytes = BytesNumeric.FromInt16(original);
        ushort result = BytesNumeric.ToUInt16(bytes, 0);
        
        // Assert
        Assert.Equal(original, result);
    }
}
