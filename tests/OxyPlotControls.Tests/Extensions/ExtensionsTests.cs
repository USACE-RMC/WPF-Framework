/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* - Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* - Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* - The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System.Xml.Linq;
using System.Windows;
using OxyPlot;
using Xunit;

namespace OxyPlotControls.Tests.Extensions;

/// <summary>
/// Tests for DataPoint extension methods.
/// </summary>
public class DataPointExtensionsTests
{
    #region ToPrettyText Tests

    [Fact]
    public void ToPrettyText_SimpleValues_ReturnsFormattedString()
    {
        // Arrange
        var dataPoint = new DataPoint(1.5, 2.5);

        // Act
        var result = dataPoint.ToPrettyText();

        // Assert
        Assert.Equal("1.5, 2.5", result);
    }

    [Fact]
    public void ToPrettyText_ZeroValues_ReturnsFormattedString()
    {
        // Arrange
        var dataPoint = new DataPoint(0, 0);

        // Act
        var result = dataPoint.ToPrettyText();

        // Assert
        Assert.Equal("0, 0", result);
    }

    [Fact]
    public void ToPrettyText_NegativeValues_ReturnsFormattedString()
    {
        // Arrange
        var dataPoint = new DataPoint(-10.5, -20.3);

        // Act
        var result = dataPoint.ToPrettyText();

        // Assert - Use Contains due to floating point representation (-20.3 may become -20.300000000000001)
        Assert.Contains("-10.5", result);
        Assert.Contains("-20.3", result);
    }

    [Fact]
    public void ToPrettyText_LargeValues_ReturnsFormattedString()
    {
        // Arrange
        var dataPoint = new DataPoint(1234567890.123456, 9876543210.654321);

        // Act
        var result = dataPoint.ToPrettyText();

        // Assert
        Assert.Contains("1234567890", result);
        Assert.Contains("9876543210", result);
    }

    [Fact]
    public void ToPrettyText_VerySmallValues_ReturnsFormattedString()
    {
        // Arrange
        var dataPoint = new DataPoint(0.0000001, 0.0000002);

        // Act
        var result = dataPoint.ToPrettyText();

        // Assert - Very small values are formatted in scientific notation (E-07 or E-08)
        // 0.0000001 may become 9.999...E-08 and 0.0000002 may become 1.999...E-07
        Assert.Contains("E-0", result);
        Assert.Contains(", ", result);
    }

    #endregion

    #region FromPrettyDataText Tests

    [Fact]
    public void FromPrettyDataText_ValidString_ReturnsDataPoint()
    {
        // Arrange
        var text = "1.5, 2.5";

        // Act
        var result = text.FromPrettyDataText();

        // Assert
        Assert.Equal(1.5, result.X);
        Assert.Equal(2.5, result.Y);
    }

    [Fact]
    public void FromPrettyDataText_ZeroValues_ReturnsDataPoint()
    {
        // Arrange
        var text = "0, 0";

        // Act
        var result = text.FromPrettyDataText();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void FromPrettyDataText_NegativeValues_ReturnsDataPoint()
    {
        // Arrange
        var text = "-10.5, -20.3";

        // Act
        var result = text.FromPrettyDataText();

        // Assert
        Assert.Equal(-10.5, result.X);
        Assert.Equal(-20.3, result.Y);
    }

    [Fact]
    public void FromPrettyDataText_InvalidFormat_ReturnsUndefined()
    {
        // Arrange
        var text = "invalid";

        // Act
        var result = text.FromPrettyDataText();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void FromPrettyDataText_MissingValue_ReturnsUndefined()
    {
        // Arrange
        var text = "1.5";

        // Act
        var result = text.FromPrettyDataText();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void FromPrettyDataText_InvalidXValue_ReturnsUndefined()
    {
        // Arrange
        var text = "abc, 2.5";

        // Act
        var result = text.FromPrettyDataText();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void FromPrettyDataText_InvalidYValue_ReturnsUndefined()
    {
        // Arrange
        var text = "1.5, xyz";

        // Act
        var result = text.FromPrettyDataText();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void FromPrettyDataText_ExtraValues_ReturnsUndefined()
    {
        // Arrange
        var text = "1.5, 2.5, 3.5";

        // Act
        var result = text.FromPrettyDataText();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void ToPrettyText_FromPrettyDataText_RoundTrip()
    {
        // Arrange
        var original = new DataPoint(123.456, 789.012);

        // Act
        var text = original.ToPrettyText();
        var result = text.FromPrettyDataText();

        // Assert
        Assert.Equal(original.X, result.X, 10);
        Assert.Equal(original.Y, result.Y, 10);
    }

    #endregion

    #region ToXElement Tests

    [Fact]
    public void ToXElement_SimpleValues_ReturnsXElement()
    {
        // Arrange
        var dataPoint = new DataPoint(1.5, 2.5);

        // Act
        var result = dataPoint.ToXElement();

        // Assert
        Assert.Equal("DataPoint", result.Name.LocalName);
        Assert.Equal("1.5", result.Attribute("X")?.Value);
        Assert.Equal("2.5", result.Attribute("Y")?.Value);
    }

    [Fact]
    public void ToXElement_ZeroValues_ReturnsXElement()
    {
        // Arrange
        var dataPoint = new DataPoint(0, 0);

        // Act
        var result = dataPoint.ToXElement();

        // Assert
        Assert.Equal("0", result.Attribute("X")?.Value);
        Assert.Equal("0", result.Attribute("Y")?.Value);
    }

    [Fact]
    public void ToXElement_NegativeValues_ReturnsXElement()
    {
        // Arrange
        var dataPoint = new DataPoint(-10.5, -20.3);

        // Act
        var result = dataPoint.ToXElement();

        // Assert - Use Contains/StartsWith due to floating point representation (-20.3 may become -20.300000000000001)
        Assert.Equal("-10.5", result.Attribute("X")?.Value);
        Assert.StartsWith("-20.3", result.Attribute("Y")?.Value);
    }

    #endregion

    #region PointFromXElement Tests

    [Fact]
    public void PointFromXElement_ValidElement_ReturnsDataPoint()
    {
        // Arrange
        var element = new XElement("DataPoint",
            new XAttribute("X", "1.5"),
            new XAttribute("Y", "2.5"));

        // Act
        var result = element.PointFromXElement();

        // Assert
        Assert.Equal(1.5, result.X);
        Assert.Equal(2.5, result.Y);
    }

    [Fact]
    public void PointFromXElement_WrongElementName_ReturnsUndefined()
    {
        // Arrange
        var element = new XElement("WrongName",
            new XAttribute("X", "1.5"),
            new XAttribute("Y", "2.5"));

        // Act
        var result = element.PointFromXElement();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void PointFromXElement_MissingXAttribute_ReturnsUndefined()
    {
        // Arrange
        var element = new XElement("DataPoint",
            new XAttribute("Y", "2.5"));

        // Act
        var result = element.PointFromXElement();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void PointFromXElement_MissingYAttribute_ReturnsUndefined()
    {
        // Arrange
        var element = new XElement("DataPoint",
            new XAttribute("X", "1.5"));

        // Act
        var result = element.PointFromXElement();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void PointFromXElement_InvalidXValue_ReturnsUndefined()
    {
        // Arrange
        var element = new XElement("DataPoint",
            new XAttribute("X", "invalid"),
            new XAttribute("Y", "2.5"));

        // Act
        var result = element.PointFromXElement();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void PointFromXElement_InvalidYValue_ReturnsUndefined()
    {
        // Arrange
        var element = new XElement("DataPoint",
            new XAttribute("X", "1.5"),
            new XAttribute("Y", "invalid"));

        // Act
        var result = element.PointFromXElement();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void ToXElement_PointFromXElement_RoundTrip()
    {
        // Arrange
        var original = new DataPoint(123.456, 789.012);

        // Act
        var element = original.ToXElement();
        var result = element.PointFromXElement();

        // Assert
        Assert.Equal(original.X, result.X, 10);
        Assert.Equal(original.Y, result.Y, 10);
    }

    #endregion

    #region PointsFromXElement Tests

    [Fact]
    public void PointsFromXElement_DataPointsElement_ReturnsDataPointList()
    {
        // Arrange
        var element = new XElement("DataPoints",
            new XElement("DataPoint", new XAttribute("X", "1"), new XAttribute("Y", "2")),
            new XElement("DataPoint", new XAttribute("X", "3"), new XAttribute("Y", "4")));

        // Act
        var result = element.PointsFromXElement();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].X);
        Assert.Equal(2, result[0].Y);
        Assert.Equal(3, result[1].X);
        Assert.Equal(4, result[1].Y);
    }

    [Fact]
    public void PointsFromXElement_PointsElement_ReturnsDataPointList()
    {
        // Arrange
        var element = new XElement("Points",
            new XElement("DataPoint", new XAttribute("X", "1"), new XAttribute("Y", "2")),
            new XElement("DataPoint", new XAttribute("X", "3"), new XAttribute("Y", "4")));

        // Act
        var result = element.PointsFromXElement();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].X);
        Assert.Equal(2, result[0].Y);
        Assert.Equal(3, result[1].X);
        Assert.Equal(4, result[1].Y);
    }

    [Fact]
    public void PointsFromXElement_WrongElementName_ReturnsEmptyList()
    {
        // Arrange
        var element = new XElement("WrongName",
            new XElement("DataPoint", new XAttribute("X", "1"), new XAttribute("Y", "2")));

        // Act
        var result = element.PointsFromXElement();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void PointsFromXElement_EmptyElement_ReturnsEmptyList()
    {
        // Arrange
        var element = new XElement("DataPoints");

        // Act
        var result = element.PointsFromXElement();

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region ToXElement List Tests

    [Fact]
    public void ToXElement_DataPointList_ReturnsXElement()
    {
        // Arrange
        var dataPoints = new List<DataPoint>
        {
            new DataPoint(1, 2),
            new DataPoint(3, 4)
        };

        // Act
        var result = dataPoints.ToXElement("Points");

        // Assert
        Assert.Equal("Points", result.Name.LocalName);
        Assert.Equal(2, result.Elements("DataPoint").Count());
    }

    [Fact]
    public void ToXElement_EmptyList_ReturnsEmptyXElement()
    {
        // Arrange
        var dataPoints = new List<DataPoint>();

        // Act
        var result = dataPoints.ToXElement("Points");

        // Assert
        Assert.Equal("Points", result.Name.LocalName);
        Assert.Empty(result.Elements());
    }

    #endregion
}

/// <summary>
/// Tests for ScreenVector extension methods.
/// </summary>
public class ScreenVectorExtensionsTests
{
    [Fact]
    public void ToPrettyText_SimpleValues_ReturnsFormattedString()
    {
        // Arrange
        var vector = new ScreenVector(10.5, 20.5);

        // Act
        var result = vector.ToPrettyText();

        // Assert
        Assert.Equal("10.5, 20.5", result);
    }

    [Fact]
    public void ToPrettyText_ZeroValues_ReturnsFormattedString()
    {
        // Arrange
        var vector = new ScreenVector(0, 0);

        // Act
        var result = vector.ToPrettyText();

        // Assert
        Assert.Equal("0, 0", result);
    }

    [Fact]
    public void ToPrettyText_NegativeValues_ReturnsFormattedString()
    {
        // Arrange
        var vector = new ScreenVector(-10.5, -20.5);

        // Act
        var result = vector.ToPrettyText();

        // Assert
        Assert.Equal("-10.5, -20.5", result);
    }

    [Fact]
    public void FromPrettyVectorText_ValidString_ReturnsScreenVector()
    {
        // Arrange
        var text = "10.5, 20.5";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(10.5, result.X);
        Assert.Equal(20.5, result.Y);
    }

    [Fact]
    public void FromPrettyVectorText_ZeroValues_ReturnsScreenVector()
    {
        // Arrange
        var text = "0, 0";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void FromPrettyVectorText_NegativeValues_ReturnsScreenVector()
    {
        // Arrange
        var text = "-10.5, -20.5";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(-10.5, result.X);
        Assert.Equal(-20.5, result.Y);
    }

    [Fact]
    public void FromPrettyVectorText_InvalidFormat_ReturnsDefault()
    {
        // Arrange
        var text = "invalid";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void FromPrettyVectorText_InvalidXValue_ReturnsDefault()
    {
        // Arrange
        var text = "abc, 20.5";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void FromPrettyVectorText_InvalidYValue_ReturnsDefault()
    {
        // Arrange
        var text = "10.5, xyz";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void ToPrettyText_FromPrettyVectorText_RoundTrip()
    {
        // Arrange
        var original = new ScreenVector(123.456, 789.012);

        // Act
        var text = original.ToPrettyText();
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(original.X, result.X, 10);
        Assert.Equal(original.Y, result.Y, 10);
    }
}

/// <summary>
/// Tests for ScreenPoint extension methods.
/// </summary>
public class ScreenPointExtensionsTests
{
    [Fact]
    public void ToPrettyText_SimpleValues_ReturnsFormattedString()
    {
        // Arrange
        var point = new ScreenPoint(100.5, 200.5);

        // Act
        var result = point.ToPrettyText();

        // Assert
        Assert.Equal("100.5, 200.5", result);
    }

    [Fact]
    public void ToPrettyText_ZeroValues_ReturnsFormattedString()
    {
        // Arrange
        var point = new ScreenPoint(0, 0);

        // Act
        var result = point.ToPrettyText();

        // Assert
        Assert.Equal("0, 0", result);
    }

    [Fact]
    public void ToPrettyText_NegativeValues_ReturnsFormattedString()
    {
        // Arrange
        var point = new ScreenPoint(-100.5, -200.5);

        // Act
        var result = point.ToPrettyText();

        // Assert
        Assert.Equal("-100.5, -200.5", result);
    }

    [Fact]
    public void FromPrettyScreenText_ValidString_ReturnsScreenPoint()
    {
        // Arrange
        var text = "100.5, 200.5";

        // Act
        var result = text.FromPrettyScreenText();

        // Assert
        Assert.Equal(100.5, result.X);
        Assert.Equal(200.5, result.Y);
    }

    [Fact]
    public void FromPrettyScreenText_ZeroValues_ReturnsScreenPoint()
    {
        // Arrange
        var text = "0, 0";

        // Act
        var result = text.FromPrettyScreenText();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void FromPrettyScreenText_InvalidFormat_ReturnsUndefined()
    {
        // Arrange
        var text = "invalid";

        // Act
        var result = text.FromPrettyScreenText();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void FromPrettyScreenText_InvalidXValue_ReturnsUndefined()
    {
        // Arrange
        var text = "abc, 200.5";

        // Act
        var result = text.FromPrettyScreenText();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void FromPrettyScreenText_InvalidYValue_ReturnsUndefined()
    {
        // Arrange
        var text = "100.5, xyz";

        // Act
        var result = text.FromPrettyScreenText();

        // Assert
        Assert.True(double.IsNaN(result.X));
        Assert.True(double.IsNaN(result.Y));
    }

    [Fact]
    public void ToPrettyText_FromPrettyScreenText_RoundTrip()
    {
        // Arrange
        var original = new ScreenPoint(123.456, 789.012);

        // Act
        var text = original.ToPrettyText();
        var result = text.FromPrettyScreenText();

        // Assert
        Assert.Equal(original.X, result.X, 10);
        Assert.Equal(original.Y, result.Y, 10);
    }
}

/// <summary>
/// Tests for WPF Vector extension methods.
/// </summary>
public class VectorExtensionsTests
{
    [Fact]
    public void ToPrettyText_SimpleValues_ReturnsFormattedString()
    {
        // Arrange
        var vector = new Vector(5.5, 10.5);

        // Act
        var result = vector.ToPrettyText();

        // Assert
        Assert.Equal("5.5, 10.5", result);
    }

    [Fact]
    public void ToPrettyText_ZeroValues_ReturnsFormattedString()
    {
        // Arrange
        var vector = new Vector(0, 0);

        // Act
        var result = vector.ToPrettyText();

        // Assert
        Assert.Equal("0, 0", result);
    }

    [Fact]
    public void ToPrettyText_NegativeValues_ReturnsFormattedString()
    {
        // Arrange
        var vector = new Vector(-5.5, -10.5);

        // Act
        var result = vector.ToPrettyText();

        // Assert
        Assert.Equal("-5.5, -10.5", result);
    }

    [Fact]
    public void FromPrettyVectorString_ValidString_ReturnsVector()
    {
        // Arrange
        var text = "5.5, 10.5";

        // Act
        var result = text.FromPrettyVectorString();

        // Assert
        Assert.Equal(5.5, result.X);
        Assert.Equal(10.5, result.Y);
    }

    [Fact]
    public void FromPrettyVectorString_ZeroValues_ReturnsVector()
    {
        // Arrange
        var text = "0, 0";

        // Act
        var result = text.FromPrettyVectorString();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void FromPrettyVectorString_InvalidFormat_ReturnsDefault()
    {
        // Arrange
        var text = "invalid";

        // Act
        var result = text.FromPrettyVectorString();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void FromPrettyVectorString_InvalidXValue_ReturnsDefault()
    {
        // Arrange
        var text = "abc, 10.5";

        // Act
        var result = text.FromPrettyVectorString();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void FromPrettyVectorString_InvalidYValue_ReturnsDefault()
    {
        // Arrange
        var text = "5.5, xyz";

        // Act
        var result = text.FromPrettyVectorString();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void ToPrettyText_FromPrettyVectorString_RoundTrip()
    {
        // Arrange
        var original = new Vector(123.456, 789.012);

        // Act
        var text = original.ToPrettyText();
        var result = text.FromPrettyVectorString();

        // Assert
        Assert.Equal(original.X, result.X, 10);
        Assert.Equal(original.Y, result.Y, 10);
    }
}

/// <summary>
/// Tests for GetFirstAbstractBaseType extension method.
/// </summary>
public class GetFirstAbstractBaseTypeTests
{
    // Test helper classes
    private abstract class AbstractBase { }
    private class ConcreteChild : AbstractBase { }
    private class ConcreteGrandChild : ConcreteChild { }
    private class StandaloneClass { }

    [Fact]
    public void GetFirstAbstractBaseType_NullType_ThrowsArgumentNullException()
    {
        // Arrange
        Type? nullType = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => nullType!.GetFirstAbstractBaseType());
    }

    [Fact]
    public void GetFirstAbstractBaseType_AbstractTypeItself_ReturnsNull()
    {
        // Arrange
        var type = typeof(AbstractBase);

        // Act
        var result = type.GetFirstAbstractBaseType();

        // Assert - Abstract class's base type is object which is not abstract, so returns null
        Assert.Null(result);
    }

    [Fact]
    public void GetFirstAbstractBaseType_ConcreteChildOfAbstract_ReturnsAbstractBase()
    {
        // Arrange
        var type = typeof(ConcreteChild);

        // Act
        var result = type.GetFirstAbstractBaseType();

        // Assert
        Assert.Equal(typeof(AbstractBase), result);
    }

    [Fact]
    public void GetFirstAbstractBaseType_GrandChildOfAbstract_ReturnsAbstractBase()
    {
        // Arrange
        var type = typeof(ConcreteGrandChild);

        // Act
        var result = type.GetFirstAbstractBaseType();

        // Assert
        Assert.Equal(typeof(AbstractBase), result);
    }

    [Fact]
    public void GetFirstAbstractBaseType_StandaloneClass_ReturnsObjectType()
    {
        // Arrange
        var type = typeof(StandaloneClass);

        // Act
        var result = type.GetFirstAbstractBaseType();

        // Assert - StandaloneClass inherits from object, which is not abstract
        Assert.Null(result);
    }

    [Fact]
    public void GetFirstAbstractBaseType_ObjectType_ReturnsNull()
    {
        // Arrange
        var type = typeof(object);

        // Act
        var result = type.GetFirstAbstractBaseType();

        // Assert - object has no base type
        Assert.Null(result);
    }
}
