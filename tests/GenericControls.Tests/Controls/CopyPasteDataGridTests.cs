/*
* Unit tests for CopyPasteDataGrid utility methods from GenericControls
*/

using Xunit;

namespace GenericControls.Tests.Controls;

/// <summary>
/// Unit tests for the <see cref="CopyPasteDataGrid"/> class static methods.
/// </summary>
public class CopyPasteDataGridTests
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
        var result = CopyPasteDataGrid.IsNumericType(type);

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
        var result = CopyPasteDataGrid.IsNumericType(type);

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
        var result = CopyPasteDataGrid.IsNumericType(type);

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
        var result = CopyPasteDataGrid.IsNumericType(type);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsNumericType Tests - Null

    [Fact]
    public void IsNumericType_NullType_ReturnsFalse()
    {
        // Act
        var result = CopyPasteDataGrid.IsNumericType(null!);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsNumericType Tests - Complex Types

    [Fact]
    public void IsNumericType_EnumType_ReturnsTrue()
    {
        // Enums have numeric underlying types (Int32 by default), so Type.GetTypeCode()
        // returns the underlying type code, making them appear numeric
        // Act
        var result = CopyPasteDataGrid.IsNumericType(typeof(DayOfWeek));

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNumericType_ArrayType_ReturnsFalse()
    {
        // Act
        var result = CopyPasteDataGrid.IsNumericType(typeof(int[]));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNumericType_ListType_ReturnsFalse()
    {
        // Act
        var result = CopyPasteDataGrid.IsNumericType(typeof(List<int>));

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsDoubleType Tests

    [Fact]
    public void IsDoubleType_Double_ReturnsTrue()
    {
        // Act
        var result = CopyPasteDataGrid.IsDoubleType(typeof(double));

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(typeof(float))]
    [InlineData(typeof(decimal))]
    [InlineData(typeof(int))]
    [InlineData(typeof(long))]
    [InlineData(typeof(byte))]
    public void IsDoubleType_OtherNumericTypes_ReturnsFalse(Type type)
    {
        // Act
        var result = CopyPasteDataGrid.IsDoubleType(type);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(object))]
    public void IsDoubleType_NonNumericTypes_ReturnsFalse(Type type)
    {
        // Act
        var result = CopyPasteDataGrid.IsDoubleType(type);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDoubleType_NullType_ReturnsFalse()
    {
        // Act
        var result = CopyPasteDataGrid.IsDoubleType(null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDoubleType_NullableDouble_ReturnsFalse()
    {
        // Note: The implementation only checks for double, not Nullable<double>
        // Act
        var result = CopyPasteDataGrid.IsDoubleType(typeof(double?));

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Consistency Tests

    [Fact]
    public void IsNumericType_CalledMultipleTimes_ReturnsSameResult()
    {
        // Arrange
        var type = typeof(int);

        // Act
        var result1 = CopyPasteDataGrid.IsNumericType(type);
        var result2 = CopyPasteDataGrid.IsNumericType(type);
        var result3 = CopyPasteDataGrid.IsNumericType(type);

        // Assert
        Assert.True(result1);
        Assert.Equal(result1, result2);
        Assert.Equal(result2, result3);
    }

    [Fact]
    public void IsDoubleType_CalledMultipleTimes_ReturnsSameResult()
    {
        // Arrange
        var type = typeof(double);

        // Act
        var result1 = CopyPasteDataGrid.IsDoubleType(type);
        var result2 = CopyPasteDataGrid.IsDoubleType(type);
        var result3 = CopyPasteDataGrid.IsDoubleType(type);

        // Assert
        Assert.True(result1);
        Assert.Equal(result1, result2);
        Assert.Equal(result2, result3);
    }

    #endregion

    #region Comparison Between GeneralMethods and CopyPasteDataGrid

    [Theory]
    [InlineData(typeof(byte))]
    [InlineData(typeof(sbyte))]
    [InlineData(typeof(short))]
    [InlineData(typeof(ushort))]
    [InlineData(typeof(int))]
    [InlineData(typeof(uint))]
    [InlineData(typeof(long))]
    [InlineData(typeof(ulong))]
    [InlineData(typeof(float))]
    [InlineData(typeof(double))]
    [InlineData(typeof(decimal))]
    public void IsNumericType_SameAsGeneralMethods_ForNumericTypes(Type type)
    {
        // Act
        var copyPasteResult = CopyPasteDataGrid.IsNumericType(type);
        var generalMethodsResult = GeneralMethods.IsNumericType(type);

        // Assert - Both implementations should agree
        Assert.Equal(generalMethodsResult, copyPasteResult);
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(bool))]
    [InlineData(typeof(DateTime))]
    [InlineData(typeof(object))]
    public void IsNumericType_SameAsGeneralMethods_ForNonNumericTypes(Type type)
    {
        // Act
        var copyPasteResult = CopyPasteDataGrid.IsNumericType(type);
        var generalMethodsResult = GeneralMethods.IsNumericType(type);

        // Assert - Both implementations should agree
        Assert.Equal(generalMethodsResult, copyPasteResult);
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
            Assert.True(CopyPasteDataGrid.IsNumericType(type), $"{type.Name} should be numeric");
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
            Assert.True(CopyPasteDataGrid.IsNumericType(type), $"{type.Name} should be numeric");
        }
    }

    [Fact]
    public void IsDoubleType_OnlyDoubleReturnsTrue()
    {
        // Arrange - All numeric types
        var numericTypes = new[]
        {
            typeof(byte), typeof(sbyte),
            typeof(short), typeof(ushort),
            typeof(int), typeof(uint),
            typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
        };

        // Act & Assert
        foreach (var type in numericTypes)
        {
            var result = CopyPasteDataGrid.IsDoubleType(type);
            if (type == typeof(double))
            {
                Assert.True(result, $"{type.Name} should return true");
            }
            else
            {
                Assert.False(result, $"{type.Name} should return false");
            }
        }
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void IsNumericType_IntPtr_ReturnsFalse()
    {
        // Act
        var result = CopyPasteDataGrid.IsNumericType(typeof(IntPtr));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNumericType_UIntPtr_ReturnsFalse()
    {
        // Act
        var result = CopyPasteDataGrid.IsNumericType(typeof(UIntPtr));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDoubleType_IntPtr_ReturnsFalse()
    {
        // Act
        var result = CopyPasteDataGrid.IsDoubleType(typeof(IntPtr));

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsNumericType_CustomStruct_ReturnsFalse()
    {
        // Act
        var result = CopyPasteDataGrid.IsNumericType(typeof(CustomTestStruct));

        // Assert
        Assert.False(result);
    }

    private struct CustomTestStruct
    {
        public int Value;
    }

    #endregion
}
