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

using System;
using Xunit;
using DatabaseManager;

namespace DatabaseManager.Tests
{
    /// <summary>
    /// Tests for the DatabaseManager base class static methods.
    /// </summary>
    public class DatabaseManagerTests
    {
        #region IsNumericType Tests

        /// <summary>
        /// Verifies that IsNumericType returns true for all numeric types.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <param name="expected">The expected result.</param>
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
        public void IsNumericType_WithNumericTypes_ReturnsTrue(Type type, bool expected)
        {
            var result = DatabaseManager.IsNumericType(type);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that IsNumericType returns false for non-numeric types.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(char), false)]
        [InlineData(typeof(DateTime), false)]
        [InlineData(typeof(object), false)]
        public void IsNumericType_WithNonNumericTypes_ReturnsFalse(Type type, bool expected)
        {
            var result = DatabaseManager.IsNumericType(type);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that IsNumericType returns false when passed null.
        /// </summary>
        [Fact]
        public void IsNumericType_WithNull_ReturnsFalse()
        {
            var result = DatabaseManager.IsNumericType(null);
            Assert.False(result);
        }

        /// <summary>
        /// Verifies that IsNumericType returns true for nullable numeric types.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData(typeof(int?), true)]
        [InlineData(typeof(double?), true)]
        [InlineData(typeof(decimal?), true)]
        [InlineData(typeof(long?), true)]
        [InlineData(typeof(float?), true)]
        [InlineData(typeof(byte?), true)]
        [InlineData(typeof(short?), true)]
        public void IsNumericType_WithNullableNumericTypes_ReturnsTrue(Type type, bool expected)
        {
            var result = DatabaseManager.IsNumericType(type);
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifies that IsNumericType returns false for nullable non-numeric types.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <param name="expected">The expected result.</param>
        [Theory]
        [InlineData(typeof(bool?), false)]
        [InlineData(typeof(char?), false)]
        [InlineData(typeof(DateTime?), false)]
        public void IsNumericType_WithNullableNonNumericTypes_ReturnsFalse(Type type, bool expected)
        {
            var result = DatabaseManager.IsNumericType(type);
            Assert.Equal(expected, result);
        }

        #endregion
    }
}
