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

using Xunit;

namespace OxyPlotControls.Tests.Extensions;

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
