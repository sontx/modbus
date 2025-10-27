using Modbus.Core.Converters;
using System.Runtime.InteropServices;
using Xunit;

namespace Core.Tests;

public class BytesStructureTests
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct SimpleStruct
    {
        public byte field1;
        
        [Endian(Endianness.BigEndian)]
        public ushort field2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct ComplexStruct
    {
        public byte count;
        
        [Endian(Endianness.BigEndian)]
        public short value1;
        
        [Endian(Endianness.BigEndian)]
        public ushort value2;
    }

    [Fact]
    public void ToBytes_WithSimpleStruct_ConvertsToBigEndian()
    {
        // Arrange
        var data = new SimpleStruct
        {
            field1 = 0x42,
            field2 = 0x1234
        };
        
        // Act
        byte[] result = BytesStructure.ToBytes(data);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Length);
        Assert.Equal(0x42, result[0]);
        Assert.Equal(0x12, result[1]); // Big endian
        Assert.Equal(0x34, result[2]);
    }

    [Fact]
    public void FromBytes_WithSimpleStruct_ConvertsFromBigEndian()
    {
        // Arrange
        byte[] bytes = new byte[] { 0x42, 0x12, 0x34 };
        
        // Act
        var result = BytesStructure.FromBytes<SimpleStruct>(bytes);
        
        // Assert
        Assert.Equal(0x42, result.field1);
        Assert.Equal(0x1234, result.field2);
    }

    [Fact]
    public void ToBytes_FromBytes_RoundTrip_PreservesValues()
    {
        // Arrange
        var original = new ComplexStruct
        {
            count = 5,
            value1 = -1000,
            value2 = 40000
        };
        
        // Act
        byte[] bytes = BytesStructure.ToBytes(original);
        var result = BytesStructure.FromBytes<ComplexStruct>(bytes);
        
        // Assert
        Assert.Equal(original.count, result.count);
        Assert.Equal(original.value1, result.value1);
        Assert.Equal(original.value2, result.value2);
    }

    [Fact]
    public void ToBytes_WithComplexStruct_ProducesCorrectByteLayout()
    {
        // Arrange
        var data = new ComplexStruct
        {
            count = 0x03,
            value1 = 0x1234,
            value2 = 0x5678
        };
        
        // Act
        byte[] result = BytesStructure.ToBytes(data);
        
        // Assert
        Assert.Equal(5, result.Length);
        Assert.Equal(0x03, result[0]);
        Assert.Equal(0x12, result[1]); // value1 high byte
        Assert.Equal(0x34, result[2]); // value1 low byte
        Assert.Equal(0x56, result[3]); // value2 high byte
        Assert.Equal(0x78, result[4]); // value2 low byte
    }

    [Fact]
    public void FromBytes_WithComplexStruct_ParsesCorrectly()
    {
        // Arrange
        byte[] bytes = new byte[] { 0x03, 0x12, 0x34, 0x56, 0x78 };
        
        // Act
        var result = BytesStructure.FromBytes<ComplexStruct>(bytes);
        
        // Assert
        Assert.Equal(0x03, result.count);
        Assert.Equal(0x1234, result.value1);
        Assert.Equal(0x5678, result.value2);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    private struct StructWithArray
    {
        public byte length;
        
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 2)]
        [Endian(Endianness.BigEndian)]
        public ushort[] values;
    }

    [Fact]
    public void ToBytes_WithArrayField_ConvertsArrayToBigEndian()
    {
        // Arrange
        var data = new StructWithArray
        {
            length = 2,
            values = new ushort[] { 0x1234, 0x5678 }
        };
        
        // Act
        byte[] result = BytesStructure.ToBytes(data);
        
        // Assert
        Assert.Equal(5, result.Length);
        Assert.Equal(0x02, result[0]);
        Assert.Equal(0x12, result[1]);
        Assert.Equal(0x34, result[2]);
        Assert.Equal(0x56, result[3]);
        Assert.Equal(0x78, result[4]);
    }

    [Fact]
    public void FromBytes_WithArrayField_ParsesArrayCorrectly()
    {
        // Arrange
        byte[] bytes = new byte[] { 0x02, 0x12, 0x34, 0x56, 0x78 };
        
        // Act
        var result = BytesStructure.FromBytes<StructWithArray>(bytes);
        
        // Assert
        Assert.Equal(2, result.length);
        Assert.NotNull(result.values);
        Assert.Equal(2, result.values.Length);
        Assert.Equal(0x1234, result.values[0]);
        Assert.Equal(0x5678, result.values[1]);
    }
}
