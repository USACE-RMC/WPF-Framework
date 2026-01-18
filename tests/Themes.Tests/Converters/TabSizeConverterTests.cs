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
using Themes;
using System.Globalization;

namespace Themes.Tests.Converters
{
    /// <summary>
    /// Tests for TabSizeConverter.
    /// Note: This converter requires a WPF TabControl as input, making it difficult to test
    /// in isolation. These tests verify basic behavior like ConvertBack throwing.
    /// Full integration testing would require WPF context.
    /// </summary>
    public class TabSizeConverterTests
    {
        #region ConvertBack Tests

        /// <summary>
        /// Verifies that ConvertBack throws a NotImplementedException when called.
        /// </summary>
        [Fact]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            var converter = new TabSizeConverter();

            Assert.Throws<NotImplementedException>(() =>
                converter.ConvertBack(50.0, new[] { typeof(object), typeof(double) }, null, CultureInfo.InvariantCulture));
        }

        #endregion

        #region Convert Edge Cases

        /// <summary>
        /// Verifies that Convert throws a NullReferenceException when provided with null values.
        /// </summary>
        [Fact]
        public void Convert_NullValues_ThrowsNullReferenceException()
        {
            var converter = new TabSizeConverter();

            // The converter casts values[0] to TabControl, so null values will throw
            Assert.Throws<NullReferenceException>(() =>
                converter.Convert(null!, typeof(double), null, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Verifies that Convert throws an IndexOutOfRangeException when provided with an empty values array.
        /// </summary>
        [Fact]
        public void Convert_EmptyValues_ThrowsIndexOutOfRangeException()
        {
            var converter = new TabSizeConverter();

            // The converter accesses values[0], so empty array throws IndexOutOfRange
            Assert.Throws<IndexOutOfRangeException>(() =>
                converter.Convert(Array.Empty<object>(), typeof(double), null, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Verifies that Convert throws an InvalidCastException when the first value is not a TabControl.
        /// </summary>
        [Fact]
        public void Convert_NonTabControlValue_ThrowsInvalidCastException()
        {
            var converter = new TabSizeConverter();

            // The converter casts values[0] to TabControl
            Assert.Throws<InvalidCastException>(() =>
                converter.Convert(new object[] { 100.0, 5 }, typeof(double), null, CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
