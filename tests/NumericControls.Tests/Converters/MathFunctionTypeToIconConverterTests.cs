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

using System.Globalization;
using System.Windows;
using Numerics.Data;
using NumericControls;
using Xunit;

namespace NumericControls.Tests.Converters
{
    /// <summary>
    /// Tests for <see cref="MathFunctionTypeToIconConverter"/>.
    /// </summary>
    public class MathFunctionTypeToIconConverterTests
    {
        /// <summary>
        /// Tests that GetIconResourceKey returns "PlusIcon" for Add function type.
        /// </summary>
        [Fact]
        public void GetIconResourceKey_Add_ReturnsPlusIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Add);

            // Assert
            Assert.Equal("PlusIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Subtract_ReturnsMinusIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Subtract);

            // Assert
            Assert.Equal("MinusIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Multiply_ReturnsMultiplyIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Multiply);

            // Assert
            Assert.Equal("MultiplyIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Divide_ReturnsDivideIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Divide);

            // Assert
            Assert.Equal("DivideIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Exponentiate_ReturnsExpIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Exponentiate);

            // Assert
            Assert.Equal("ExpIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Logarithm_ReturnsLogIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Logarithm);

            // Assert
            Assert.Equal("LogIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Inverse_ReturnsInvertIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Inverse);

            // Assert
            Assert.Equal("InvertIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Replace_ReturnsReplaceIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Replace);

            // Assert
            Assert.Equal("ReplaceIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Interpolate_ReturnsInterpolateIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Interpolate);

            // Assert
            Assert.Equal("InterpolateIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_AllFunctionTypes_ReturnValidKeys()
        {
            // Act & Assert
            foreach (MathFunctionType functionType in Enum.GetValues(typeof(MathFunctionType)))
            {
                var result = MathFunctionTypeToIconConverter.GetIconResourceKey(functionType);
                Assert.NotNull(result);
                Assert.NotEmpty(result);
            }
        }
    }
}
