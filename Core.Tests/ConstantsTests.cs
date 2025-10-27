using Modbus.Core;
using Xunit;

namespace Core.Tests;

public class ConstantsTests
{
    [Fact]
    public void DefaultForwardFuncCode_HasExpectedValue()
    {
        // Assert
        Assert.Equal(11, Constants.DefaultForwardFuncCode);
    }

    [Fact]
    public void UndefinedSlaveAddress_HasExpectedValue()
    {
        // Assert
        Assert.Equal(-1, Constants.UndefinedSlaveAddress);
    }

    [Fact]
    public void UndefinedSlaveAddress_IsNegative()
    {
        // Assert
        Assert.True(Constants.UndefinedSlaveAddress < 0);
    }

    [Fact]
    public void DefaultForwardFuncCode_IsPositive()
    {
        // Assert
        Assert.True(Constants.DefaultForwardFuncCode > 0);
    }
}
