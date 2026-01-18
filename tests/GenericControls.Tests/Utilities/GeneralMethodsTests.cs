/*
* Unit tests for GeneralMethods from GenericControls
*/

using Xunit;

namespace GenericControls.Tests.Utilities;

/// <summary>
/// Unit tests for the <see cref="GeneralMethods"/> class.
/// </summary>
public class GeneralMethodsTests
{
    #region IsNumericType Tests - Primitive Types

    [Theory]
    [InlineData(typeof(byte), true)]
    [InlineData(typeof(sbyte), true)]
    [InlineData(typeof(short), true)]
    [InlineData(typeof(ushort), true)]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(uint), true)]
    [InlineData(typeof(long), true)]
    [InlineData(typeof(ulong), true)]
    [InlineData(typeof(float), true)]
    [InlineData(typeof(double), true)]
    [InlineData(typeof(decimal), true)]
    public void IsNumericType_NumericTypes_ReturnsTrue(Type type, bool expected)
    {
        // Act
        var result = GeneralMethods.IsNumericType(type);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(typeof(bool))]
    [InlineData(typeof(char))]
    [InlineData(typeof(string))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(object))]
    [InlineData(typeof(Guid))]
    public void IsNumericType_NonNumericTypes_ReturnsFalse(Type type)
    {
        // Act
        var result = GeneralMethods.IsNumericType(type);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsNumericType Tests - Nullable Types

    [Theory]
    [InlineData(typeof(byte?), true)]
    [InlineData(typeof(sbyte?), true)]
    [InlineData(typeof(short?), true)]
    [InlineData(typeof(ushort?), true)]
    [InlineData(typeof(int?), true)]
    [InlineData(typeof(uint?), true)]
    [InlineData(typeof(long?), true)]
    [InlineData(typeof(ulong?), true)]
    [InlineData(typeof(float?), true)]
    [InlineData(typeof(double?), true)]
    [InlineData(typeof(decimal?), true)]
    public void IsNumericType_NullableNumericTypes_ReturnsTrue(Type type, bool expected)
    {
        // Act
        var result = GeneralMethods.IsNumericType(type);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(typeof(bool?))]
    [InlineData(typeof(char?))]
    [InlineData(typeof(DateTime?))]
    [InlineData(typeof(Guid?))]
    public void IsNumericType_NullableNonNumericTypes_ReturnsFalse(Type type)
    {
        // Act
        var result = GeneralMethods.IsNumericType(type);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsNumericType Tests - Null

    [Fact]
    public void IsNumericType_NullType_ReturnsFalse()
    {
        // Act
        var result = GeneralMethods.IsNumericType(null!);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsNumericType Tests - Complex Types

    [Fact]
    public void IsNumericType_EnumType_ReturnsFalse()
    {
        // Act
        var result = GeneralMethods.IsNumericType(typeof(DayOfWeek));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNumericType_ArrayType_ReturnsFalse()
    {
        // Act
        var result = GeneralMethods.IsNumericType(typeof(int[]));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNumericType_ListType_ReturnsFalse()
    {
        // Act
        var result = GeneralMethods.IsNumericType(typeof(List<int>));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNumericType_CustomClass_ReturnsFalse()
    {
        // Act
        var result = GeneralMethods.IsNumericType(typeof(GeneralMethodsTests));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNumericType_CustomStruct_ReturnsFalse()
    {
        // Act
        var result = GeneralMethods.IsNumericType(typeof(CustomStruct));

        // Assert
        Assert.False(result);
    }

    private struct CustomStruct
    {
        public int Value;
    }

    #endregion

    #region IsNumericType Tests - Edge Cases

    [Fact]
    public void IsNumericType_IntPtr_ReturnsFalse()
    {
        // Act
        var result = GeneralMethods.IsNumericType(typeof(IntPtr));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNumericType_UIntPtr_ReturnsFalse()
    {
        // Act
        var result = GeneralMethods.IsNumericType(typeof(UIntPtr));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNumericType_TimeSpan_ReturnsFalse()
    {
        // Act
        var result = GeneralMethods.IsNumericType(typeof(TimeSpan));

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Random Color Lists Tests

    [Fact]
    public void RandomColorsShortList_HasExpectedLength()
    {
        // Assert
        Assert.Equal(64, GeneralMethods.RandomColorsShortList.Length);
    }

    [Fact]
    public void RandomColorsLongList_HasExpectedLength()
    {
        // Assert
        Assert.Equal(1024, GeneralMethods.RandomColorsLongList.Length);
    }

    [Fact]
    public void RandomColorsShortList_ContainsValidHexColors()
    {
        // Assert
        foreach (var color in GeneralMethods.RandomColorsShortList)
        {
            Assert.StartsWith("#", color);
            Assert.Equal(7, color.Length); // #RRGGBB format
        }
    }

    [Fact]
    public void RandomColorsLongList_ContainsValidHexColors()
    {
        // Assert
        foreach (var color in GeneralMethods.RandomColorsLongList)
        {
            Assert.StartsWith("#", color);
            Assert.Equal(7, color.Length); // #RRGGBB format
        }
    }

    [Fact]
    public void RandomColorsShortList_FirstColorIsBlack()
    {
        // Assert - The first color should be black
        Assert.Equal("#000000", GeneralMethods.RandomColorsShortList[0]);
    }

    [Fact]
    public void RandomColorsLongList_FirstColorIsBlack()
    {
        // Assert - The first color should be black
        Assert.Equal("#000000", GeneralMethods.RandomColorsLongList[0]);
    }

    [Fact]
    public void RandomColorsShortList_AllColorsAreUnique()
    {
        // Arrange & Act
        var distinctColors = GeneralMethods.RandomColorsShortList.Distinct().Count();

        // Assert - All colors should be unique
        Assert.Equal(GeneralMethods.RandomColorsShortList.Length, distinctColors);
    }

    #endregion

    #region Type Verification Tests

    [Fact]
    public void IsNumericType_AllIntegerTypes_ReturnsTrue()
    {
        // Arrange
        var integerTypes = new[]
        {
            typeof(byte), typeof(sbyte),
            typeof(short), typeof(ushort),
            typeof(int), typeof(uint),
            typeof(long), typeof(ulong)
        };

        // Act & Assert
        foreach (var type in integerTypes)
        {
            Assert.True(GeneralMethods.IsNumericType(type), $"{type.Name} should be numeric");
        }
    }

    [Fact]
    public void IsNumericType_AllFloatingPointTypes_ReturnsTrue()
    {
        // Arrange
        var floatingTypes = new[] { typeof(float), typeof(double), typeof(decimal) };

        // Act & Assert
        foreach (var type in floatingTypes)
        {
            Assert.True(GeneralMethods.IsNumericType(type), $"{type.Name} should be numeric");
        }
    }

    #endregion

    #region Consistency Tests

    [Fact]
    public void IsNumericType_CalledMultipleTimes_ReturnsSameResult()
    {
        // Arrange
        var type = typeof(int);

        // Act
        var result1 = GeneralMethods.IsNumericType(type);
        var result2 = GeneralMethods.IsNumericType(type);
        var result3 = GeneralMethods.IsNumericType(type);

        // Assert
        Assert.True(result1);
        Assert.Equal(result1, result2);
        Assert.Equal(result2, result3);
    }

    [Fact]
    public void IsNumericType_NullableIntAndInt_BothReturnTrue()
    {
        // Act
        var intResult = GeneralMethods.IsNumericType(typeof(int));
        var nullableIntResult = GeneralMethods.IsNumericType(typeof(int?));

        // Assert
        Assert.True(intResult);
        Assert.True(nullableIntResult);
    }

    #endregion
}
