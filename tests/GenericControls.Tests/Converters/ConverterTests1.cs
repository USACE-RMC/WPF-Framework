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
using System.Windows.Media;
using Xunit;

namespace GenericControls.Tests.Converters
{
    /// <summary>
    /// Tests for converter classes in the GenericControls library.
    /// </summary>
    public class ConverterTests1
    {
        private readonly CultureInfo _culture = CultureInfo.InvariantCulture;

        #region ReverseBooleanConverter Tests

        [Fact]
        public void ReverseBooleanConverter_Convert_TrueReturnsFalse()
        {
            var converter = new ReverseBooleanConverter();
            var result = converter.Convert(true, typeof(bool), null, _culture);
            Assert.Equal(false, result);
        }

        [Fact]
        public void ReverseBooleanConverter_Convert_FalseReturnsTrue()
        {
            var converter = new ReverseBooleanConverter();
            var result = converter.Convert(false, typeof(bool), null, _culture);
            Assert.Equal(true, result);
        }

        [Fact]
        public void ReverseBooleanConverter_Convert_NullReturnsNull()
        {
            var converter = new ReverseBooleanConverter();
            var result = converter.Convert(null, typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void ReverseBooleanConverter_Convert_InvalidTypeReturnsNull()
        {
            var converter = new ReverseBooleanConverter();
            var result = converter.Convert("not a bool", typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void ReverseBooleanConverter_Convert_IntReturnsNull()
        {
            var converter = new ReverseBooleanConverter();
            var result = converter.Convert(1, typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void ReverseBooleanConverter_ConvertBack_TrueReturnsFalse()
        {
            var converter = new ReverseBooleanConverter();
            var result = converter.ConvertBack(true, typeof(bool), null, _culture);
            Assert.Equal(false, result);
        }

        [Fact]
        public void ReverseBooleanConverter_ConvertBack_FalseReturnsTrue()
        {
            var converter = new ReverseBooleanConverter();
            var result = converter.ConvertBack(false, typeof(bool), null, _culture);
            Assert.Equal(true, result);
        }

        [Fact]
        public void ReverseBooleanConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new ReverseBooleanConverter();
            var result = converter.ConvertBack(null, typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void ReverseBooleanConverter_ConvertBack_InvalidTypeReturnsNull()
        {
            var converter = new ReverseBooleanConverter();
            var result = converter.ConvertBack("string", typeof(bool), null, _culture);
            Assert.Null(result);
        }

        #endregion

        #region BooleanToVisibilityConverter Tests

        [Fact]
        public void BooleanToVisibilityConverter_DefaultValues()
        {
            var converter = new BooleanToVisibilityConverter();
            Assert.Equal(Visibility.Visible, converter.TrueValue);
            Assert.Equal(Visibility.Collapsed, converter.FalseValue);
        }

        [Fact]
        public void BooleanToVisibilityConverter_Convert_TrueReturnsVisible()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.Convert(true, typeof(Visibility), null, _culture);
            Assert.Equal(Visibility.Visible, result);
        }

        [Fact]
        public void BooleanToVisibilityConverter_Convert_FalseReturnsCollapsed()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.Convert(false, typeof(Visibility), null, _culture);
            Assert.Equal(Visibility.Collapsed, result);
        }

        [Fact]
        public void BooleanToVisibilityConverter_Convert_NullReturnsFalseValue()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.Convert(null, typeof(Visibility), null, _culture);
            Assert.Equal(Visibility.Collapsed, result);
        }

        [Fact]
        public void BooleanToVisibilityConverter_Convert_InvalidTypeReturnsFalseValue()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.Convert("not a bool", typeof(Visibility), null, _culture);
            Assert.Equal(Visibility.Collapsed, result);
        }

        [Fact]
        public void BooleanToVisibilityConverter_Convert_CustomValues()
        {
            var converter = new BooleanToVisibilityConverter
            {
                TrueValue = Visibility.Hidden,
                FalseValue = Visibility.Visible
            };
            Assert.Equal(Visibility.Hidden, converter.Convert(true, typeof(Visibility), null, _culture));
            Assert.Equal(Visibility.Visible, converter.Convert(false, typeof(Visibility), null, _culture));
        }

        [Fact]
        public void BooleanToVisibilityConverter_ConvertBack_VisibleReturnsTrue()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.ConvertBack(Visibility.Visible, typeof(bool), null, _culture);
            Assert.Equal(true, result);
        }

        [Fact]
        public void BooleanToVisibilityConverter_ConvertBack_CollapsedReturnsFalse()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, _culture);
            Assert.Equal(false, result);
        }

        [Fact]
        public void BooleanToVisibilityConverter_ConvertBack_HiddenReturnsFalse()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.ConvertBack(Visibility.Hidden, typeof(bool), null, _culture);
            Assert.Equal(false, result);
        }

        [Fact]
        public void BooleanToVisibilityConverter_ConvertBack_NullReturnsFalse()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.ConvertBack(null, typeof(bool), null, _culture);
            Assert.Equal(false, result);
        }

        [Fact]
        public void BooleanToVisibilityConverter_ConvertBack_InvalidTypeReturnsFalse()
        {
            var converter = new BooleanToVisibilityConverter();
            var result = converter.ConvertBack("string", typeof(bool), null, _culture);
            Assert.Equal(false, result);
        }

        [Fact]
        public void BooleanToVisibilityConverter_ConvertBack_CustomTrueValue()
        {
            var converter = new BooleanToVisibilityConverter
            {
                TrueValue = Visibility.Hidden
            };
            var result = converter.ConvertBack(Visibility.Hidden, typeof(bool), null, _culture);
            Assert.Equal(true, result);
        }

        #endregion

        #region BooleanToColorConverter Tests

        [Fact]
        public void BooleanToColorConverter_DefaultValues()
        {
            var converter = new BooleanToColorConverter();
            Assert.Equal(Colors.Black, converter.TrueValue);
            Assert.Equal(Colors.Transparent, converter.FalseValue);
        }

        [Fact]
        public void BooleanToColorConverter_Convert_TrueReturnsTrueColor()
        {
            var converter = new BooleanToColorConverter();
            var result = converter.Convert(true, typeof(Color), null, _culture);
            Assert.Equal(Colors.Black, result);
        }

        [Fact]
        public void BooleanToColorConverter_Convert_FalseReturnsFalseColor()
        {
            var converter = new BooleanToColorConverter();
            var result = converter.Convert(false, typeof(Color), null, _culture);
            Assert.Equal(Colors.Transparent, result);
        }

        [Fact]
        public void BooleanToColorConverter_Convert_NullReturnsNull()
        {
            var converter = new BooleanToColorConverter();
            var result = converter.Convert(null, typeof(Color), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToColorConverter_Convert_InvalidTypeReturnsNull()
        {
            var converter = new BooleanToColorConverter();
            var result = converter.Convert("string", typeof(Color), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToColorConverter_Convert_CustomColors()
        {
            var converter = new BooleanToColorConverter
            {
                TrueValue = Colors.Red,
                FalseValue = Colors.Blue
            };
            Assert.Equal(Colors.Red, converter.Convert(true, typeof(Color), null, _culture));
            Assert.Equal(Colors.Blue, converter.Convert(false, typeof(Color), null, _culture));
        }

        [Fact]
        public void BooleanToColorConverter_ConvertBack_TrueColorReturnsTrue()
        {
            var converter = new BooleanToColorConverter();
            var result = converter.ConvertBack(Colors.Black, typeof(bool), null, _culture);
            Assert.Equal(true, result);
        }

        [Fact]
        public void BooleanToColorConverter_ConvertBack_FalseColorReturnsFalse()
        {
            var converter = new BooleanToColorConverter();
            var result = converter.ConvertBack(Colors.Transparent, typeof(bool), null, _culture);
            Assert.Equal(false, result);
        }

        [Fact]
        public void BooleanToColorConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new BooleanToColorConverter();
            var result = converter.ConvertBack(null, typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToColorConverter_ConvertBack_UnknownColorReturnsNull()
        {
            var converter = new BooleanToColorConverter();
            var result = converter.ConvertBack(Colors.Green, typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToColorConverter_ConvertBack_InvalidTypeReturnsNull()
        {
            var converter = new BooleanToColorConverter();
            var result = converter.ConvertBack("string", typeof(bool), null, _culture);
            Assert.Null(result);
        }

        #endregion

        #region BooleanToBrushConverter Tests

        [Fact]
        public void BooleanToBrushConverter_DefaultValues()
        {
            var converter = new BooleanToBrushConverter();
            Assert.Equal(Brushes.Black, converter.TrueValue);
            Assert.Equal(Brushes.Transparent, converter.FalseValue);
        }

        [Fact]
        public void BooleanToBrushConverter_Convert_TrueReturnsTrueBrush()
        {
            var converter = new BooleanToBrushConverter();
            var result = converter.Convert(true, typeof(Brush), null, _culture);
            Assert.Equal(Brushes.Black, result);
        }

        [Fact]
        public void BooleanToBrushConverter_Convert_FalseReturnsFalseBrush()
        {
            var converter = new BooleanToBrushConverter();
            var result = converter.Convert(false, typeof(Brush), null, _culture);
            Assert.Equal(Brushes.Transparent, result);
        }

        [Fact]
        public void BooleanToBrushConverter_Convert_NullReturnsNull()
        {
            var converter = new BooleanToBrushConverter();
            var result = converter.Convert(null, typeof(Brush), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToBrushConverter_Convert_InvalidTypeReturnsNull()
        {
            var converter = new BooleanToBrushConverter();
            var result = converter.Convert(123, typeof(Brush), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToBrushConverter_Convert_CustomBrushes()
        {
            var converter = new BooleanToBrushConverter
            {
                TrueValue = Brushes.Red,
                FalseValue = Brushes.Blue
            };
            Assert.Equal(Brushes.Red, converter.Convert(true, typeof(Brush), null, _culture));
            Assert.Equal(Brushes.Blue, converter.Convert(false, typeof(Brush), null, _culture));
        }

        [Fact]
        public void BooleanToBrushConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new BooleanToBrushConverter();
            var result = converter.ConvertBack(null, typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToBrushConverter_ConvertBack_InvalidTypeReturnsNull()
        {
            var converter = new BooleanToBrushConverter();
            var result = converter.ConvertBack("string", typeof(bool), null, _culture);
            Assert.Null(result);
        }

        #endregion

        #region BooleanToTextConverter Tests

        [Fact]
        public void BooleanToTextConverter_DefaultValues()
        {
            var converter = new BooleanToTextConverter();
            Assert.Equal("", converter.TrueValue);
            Assert.Equal("", converter.FalseValue);
        }

        [Fact]
        public void BooleanToTextConverter_Convert_TrueReturnsTrueText()
        {
            var converter = new BooleanToTextConverter
            {
                TrueValue = "Yes",
                FalseValue = "No"
            };
            var result = converter.Convert(true, typeof(string), null, _culture);
            Assert.Equal("Yes", result);
        }

        [Fact]
        public void BooleanToTextConverter_Convert_FalseReturnsFalseText()
        {
            var converter = new BooleanToTextConverter
            {
                TrueValue = "Yes",
                FalseValue = "No"
            };
            var result = converter.Convert(false, typeof(string), null, _culture);
            Assert.Equal("No", result);
        }

        [Fact]
        public void BooleanToTextConverter_Convert_NullReturnsNull()
        {
            var converter = new BooleanToTextConverter();
            var result = converter.Convert(null, typeof(string), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToTextConverter_Convert_InvalidTypeReturnsNull()
        {
            var converter = new BooleanToTextConverter();
            var result = converter.Convert(42, typeof(string), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToTextConverter_ConvertBack_TrueTextReturnsTrue()
        {
            var converter = new BooleanToTextConverter
            {
                TrueValue = "Yes",
                FalseValue = "No"
            };
            var result = converter.ConvertBack("Yes", typeof(bool), null, _culture);
            Assert.Equal(true, result);
        }

        [Fact]
        public void BooleanToTextConverter_ConvertBack_FalseTextReturnsFalse()
        {
            var converter = new BooleanToTextConverter
            {
                TrueValue = "Yes",
                FalseValue = "No"
            };
            var result = converter.ConvertBack("No", typeof(bool), null, _culture);
            Assert.Equal(false, result);
        }

        [Fact]
        public void BooleanToTextConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new BooleanToTextConverter();
            var result = converter.ConvertBack(null, typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToTextConverter_ConvertBack_UnknownTextReturnsNull()
        {
            var converter = new BooleanToTextConverter
            {
                TrueValue = "Yes",
                FalseValue = "No"
            };
            var result = converter.ConvertBack("Maybe", typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToTextConverter_ConvertBack_NonStringValueUsesToString()
        {
            var converter = new BooleanToTextConverter
            {
                TrueValue = "42",
                FalseValue = "0"
            };
            var result = converter.ConvertBack(42, typeof(bool), null, _culture);
            Assert.Equal(true, result);
        }

        [Fact]
        public void BooleanToTextConverter_ConvertBack_EmptyStringMatchesDefaultTrue()
        {
            var converter = new BooleanToTextConverter(); // Defaults are empty strings
            var result = converter.ConvertBack("", typeof(bool), null, _culture);
            // Both TrueValue and FalseValue are empty, TrueValue is checked first
            Assert.Equal(true, result);
        }

        #endregion

        #region BooleanToDoubleConverter Tests

        [Fact]
        public void BooleanToDoubleConverter_DefaultValues()
        {
            var converter = new BooleanToDoubleConverter();
            Assert.Equal(0d, converter.TrueValue);
            Assert.Equal(0d, converter.FalseValue);
        }

        [Fact]
        public void BooleanToDoubleConverter_Convert_TrueReturnsTrueValue()
        {
            var converter = new BooleanToDoubleConverter
            {
                TrueValue = 1.0,
                FalseValue = 0.0
            };
            var result = converter.Convert(true, typeof(double), null, _culture);
            Assert.Equal(1.0, result);
        }

        [Fact]
        public void BooleanToDoubleConverter_Convert_FalseReturnsFalseValue()
        {
            var converter = new BooleanToDoubleConverter
            {
                TrueValue = 1.0,
                FalseValue = 0.0
            };
            var result = converter.Convert(false, typeof(double), null, _culture);
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void BooleanToDoubleConverter_Convert_NullReturnsNull()
        {
            var converter = new BooleanToDoubleConverter();
            var result = converter.Convert(null, typeof(double), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToDoubleConverter_Convert_InvalidTypeReturnsNull()
        {
            var converter = new BooleanToDoubleConverter();
            var result = converter.Convert("string", typeof(double), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToDoubleConverter_Convert_CustomValues()
        {
            var converter = new BooleanToDoubleConverter
            {
                TrueValue = 100.5,
                FalseValue = -50.25
            };
            Assert.Equal(100.5, converter.Convert(true, typeof(double), null, _culture));
            Assert.Equal(-50.25, converter.Convert(false, typeof(double), null, _culture));
        }

        [Fact]
        public void BooleanToDoubleConverter_ConvertBack_TrueValueReturnsTrue()
        {
            var converter = new BooleanToDoubleConverter
            {
                TrueValue = 1.0,
                FalseValue = 0.0
            };
            var result = converter.ConvertBack(1.0, typeof(bool), null, _culture);
            Assert.Equal(true, result);
        }

        [Fact]
        public void BooleanToDoubleConverter_ConvertBack_FalseValueReturnsFalse()
        {
            var converter = new BooleanToDoubleConverter
            {
                TrueValue = 1.0,
                FalseValue = 0.0
            };
            var result = converter.ConvertBack(0.0, typeof(bool), null, _culture);
            Assert.Equal(false, result);
        }

        [Fact]
        public void BooleanToDoubleConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new BooleanToDoubleConverter();
            var result = converter.ConvertBack(null, typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToDoubleConverter_ConvertBack_UnknownValueReturnsNull()
        {
            var converter = new BooleanToDoubleConverter
            {
                TrueValue = 1.0,
                FalseValue = 0.0
            };
            var result = converter.ConvertBack(2.0, typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void BooleanToDoubleConverter_ConvertBack_StringValueParsed()
        {
            var converter = new BooleanToDoubleConverter
            {
                TrueValue = 1.0,
                FalseValue = 0.0
            };
            var result = converter.ConvertBack("1.0", typeof(bool), null, _culture);
            Assert.Equal(true, result);
        }

        [Fact]
        public void BooleanToDoubleConverter_ConvertBack_InvalidStringReturnsNull()
        {
            var converter = new BooleanToDoubleConverter();
            var result = converter.ConvertBack("not a number", typeof(bool), null, _culture);
            Assert.Null(result);
        }

        #endregion

        #region VisibilityToBooleanConverter Tests

        [Fact]
        public void VisibilityToBooleanConverter_DefaultValues()
        {
            var converter = new VisibilityToBooleanConverter();
            Assert.True(converter.VisibleValue);
            Assert.False(converter.CollapsedValue);
            Assert.False(converter.HiddenValue);
        }

        [Fact]
        public void VisibilityToBooleanConverter_Convert_VisibleReturnsTrue()
        {
            var converter = new VisibilityToBooleanConverter();
            var result = converter.Convert(Visibility.Visible, typeof(bool), null, _culture);
            Assert.Equal(true, result);
        }

        [Fact]
        public void VisibilityToBooleanConverter_Convert_CollapsedReturnsFalse()
        {
            var converter = new VisibilityToBooleanConverter();
            var result = converter.Convert(Visibility.Collapsed, typeof(bool), null, _culture);
            Assert.Equal(false, result);
        }

        [Fact]
        public void VisibilityToBooleanConverter_Convert_HiddenReturnsFalse()
        {
            var converter = new VisibilityToBooleanConverter();
            var result = converter.Convert(Visibility.Hidden, typeof(bool), null, _culture);
            Assert.Equal(false, result);
        }

        [Fact]
        public void VisibilityToBooleanConverter_Convert_NullReturnsNull()
        {
            var converter = new VisibilityToBooleanConverter();
            var result = converter.Convert(null, typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void VisibilityToBooleanConverter_Convert_InvalidTypeReturnsNull()
        {
            var converter = new VisibilityToBooleanConverter();
            var result = converter.Convert("string", typeof(bool), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void VisibilityToBooleanConverter_Convert_CustomValues()
        {
            var converter = new VisibilityToBooleanConverter
            {
                VisibleValue = false,
                CollapsedValue = true,
                HiddenValue = true
            };
            Assert.Equal(false, converter.Convert(Visibility.Visible, typeof(bool), null, _culture));
            Assert.Equal(true, converter.Convert(Visibility.Collapsed, typeof(bool), null, _culture));
            Assert.Equal(true, converter.Convert(Visibility.Hidden, typeof(bool), null, _culture));
        }

        [Fact]
        public void VisibilityToBooleanConverter_ConvertBack_TrueReturnsVisible()
        {
            var converter = new VisibilityToBooleanConverter();
            var result = converter.ConvertBack(true, typeof(Visibility), null, _culture);
            Assert.Equal(Visibility.Visible, result);
        }

        [Fact]
        public void VisibilityToBooleanConverter_ConvertBack_FalseReturnsCollapsed()
        {
            var converter = new VisibilityToBooleanConverter();
            var result = converter.ConvertBack(false, typeof(Visibility), null, _culture);
            // Note: Both Collapsed and Hidden are false by default, Collapsed checked first
            Assert.Equal(Visibility.Collapsed, result);
        }

        [Fact]
        public void VisibilityToBooleanConverter_ConvertBack_CustomConfig()
        {
            var converter = new VisibilityToBooleanConverter
            {
                VisibleValue = false,
                HiddenValue = true,
                CollapsedValue = false
            };
            var result = converter.ConvertBack(true, typeof(Visibility), null, _culture);
            Assert.Equal(Visibility.Hidden, result);
        }

        #endregion

        #region ColorToByteConverter Tests

        [Fact]
        public void ColorToByteConverter_DefaultComponent()
        {
            var converter = new ColorToByteConverter();
            Assert.Equal(ColorComponent.R, converter.Component);
        }

        [Fact]
        public void ColorToByteConverter_Convert_ExtractsRedComponent()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.R };
            var brush = new SolidColorBrush(Color.FromArgb(255, 128, 64, 32));
            var result = converter.Convert(brush, typeof(byte), null, _culture);
            Assert.Equal((byte)128, result);
        }

        [Fact]
        public void ColorToByteConverter_Convert_ExtractsGreenComponent()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.G };
            var brush = new SolidColorBrush(Color.FromArgb(255, 128, 64, 32));
            var result = converter.Convert(brush, typeof(byte), null, _culture);
            Assert.Equal((byte)64, result);
        }

        [Fact]
        public void ColorToByteConverter_Convert_ExtractsBlueComponent()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.B };
            var brush = new SolidColorBrush(Color.FromArgb(255, 128, 64, 32));
            var result = converter.Convert(brush, typeof(byte), null, _culture);
            Assert.Equal((byte)32, result);
        }

        [Fact]
        public void ColorToByteConverter_Convert_ExtractsAlphaComponent()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.A };
            var brush = new SolidColorBrush(Color.FromArgb(200, 128, 64, 32));
            var result = converter.Convert(brush, typeof(byte), null, _culture);
            Assert.Equal((byte)200, result);
        }

        [Fact]
        public void ColorToByteConverter_Convert_NullReturnsNull()
        {
            var converter = new ColorToByteConverter();
            var result = converter.Convert(null, typeof(byte), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void ColorToByteConverter_Convert_InvalidTypeReturnsNull()
        {
            var converter = new ColorToByteConverter();
            var result = converter.Convert("string", typeof(byte), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void ColorToByteConverter_Convert_ColorObjectReturnsNull()
        {
            var converter = new ColorToByteConverter();
            var result = converter.Convert(Colors.Red, typeof(byte), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void ColorToByteConverter_ConvertBack_UpdatesRedComponent()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.R };
            var originalBrush = new SolidColorBrush(Color.FromArgb(255, 100, 64, 32));
            converter.Convert(originalBrush, typeof(byte), null, _culture); // Sets internal state
            var result = converter.ConvertBack(200, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(200, result.Color.R);
            Assert.Equal(64, result.Color.G);
            Assert.Equal(32, result.Color.B);
        }

        [Fact]
        public void ColorToByteConverter_ConvertBack_UpdatesGreenComponent()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.G };
            var originalBrush = new SolidColorBrush(Color.FromArgb(255, 128, 64, 32));
            converter.Convert(originalBrush, typeof(byte), null, _culture);
            var result = converter.ConvertBack(200, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(128, result.Color.R);
            Assert.Equal(200, result.Color.G);
            Assert.Equal(32, result.Color.B);
        }

        [Fact]
        public void ColorToByteConverter_ConvertBack_UpdatesBlueComponent()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.B };
            var originalBrush = new SolidColorBrush(Color.FromArgb(255, 128, 64, 32));
            converter.Convert(originalBrush, typeof(byte), null, _culture);
            var result = converter.ConvertBack(200, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(128, result.Color.R);
            Assert.Equal(64, result.Color.G);
            Assert.Equal(200, result.Color.B);
        }

        [Fact]
        public void ColorToByteConverter_ConvertBack_UpdatesAlphaComponent()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.A };
            var originalBrush = new SolidColorBrush(Color.FromArgb(255, 128, 64, 32));
            converter.Convert(originalBrush, typeof(byte), null, _culture);
            var result = converter.ConvertBack(100, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(100, result.Color.A);
            Assert.Equal(128, result.Color.R);
            Assert.Equal(64, result.Color.G);
            Assert.Equal(32, result.Color.B);
        }

        [Fact]
        public void ColorToByteConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new ColorToByteConverter();
            var result = converter.ConvertBack(null, typeof(SolidColorBrush), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void ColorToByteConverter_ConvertBack_NoColorSetReturnsTransparent()
        {
            var converter = new ColorToByteConverter();
            var result = converter.ConvertBack(100, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(0, result.Color.A);
        }

        [Fact]
        public void ColorToByteConverter_ConvertBack_StringValueParsed()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.R };
            var originalBrush = new SolidColorBrush(Color.FromArgb(255, 100, 64, 32));
            converter.Convert(originalBrush, typeof(byte), null, _culture);
            var result = converter.ConvertBack("200", typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(200, result.Color.R);
        }

        [Fact]
        public void ColorToByteConverter_ConvertBack_ValueClampedToByteRange()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.R };
            var originalBrush = new SolidColorBrush(Color.FromArgb(255, 100, 64, 32));
            converter.Convert(originalBrush, typeof(byte), null, _culture);

            // Value > 255 should result in byteValue = 0 (doesn't satisfy the condition)
            var result = converter.ConvertBack(300, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(0, result.Color.R);
        }

        [Fact]
        public void ColorToByteConverter_ConvertBack_NegativeValueResultsInZero()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.R };
            var originalBrush = new SolidColorBrush(Color.FromArgb(255, 100, 64, 32));
            converter.Convert(originalBrush, typeof(byte), null, _culture);
            var result = converter.ConvertBack(-10, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(0, result.Color.R);
        }

        [Fact]
        public void ColorToByteConverter_ConvertBack_DoubleValueRounded()
        {
            var converter = new ColorToByteConverter { Component = ColorComponent.R };
            var originalBrush = new SolidColorBrush(Color.FromArgb(255, 100, 64, 32));
            converter.Convert(originalBrush, typeof(byte), null, _culture);
            var result = converter.ConvertBack(150.7, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(151, result.Color.R); // Rounded
        }

        #endregion

        #region ColorToSolidBrushConverter Tests

        [Fact]
        public void ColorToSolidBrushConverter_Convert_ColorToBrush()
        {
            var converter = new ColorToSolidBrushConverter();
            var result = converter.Convert(Colors.Red, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(Colors.Red, result.Color);
        }

        [Fact]
        public void ColorToSolidBrushConverter_Convert_NullReturnsNull()
        {
            var converter = new ColorToSolidBrushConverter();
            var result = converter.Convert(null, typeof(SolidColorBrush), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void ColorToSolidBrushConverter_Convert_CustomColor()
        {
            var converter = new ColorToSolidBrushConverter();
            var color = Color.FromArgb(128, 50, 100, 150);
            var result = converter.Convert(color, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(128, result.Color.A);
            Assert.Equal(50, result.Color.R);
            Assert.Equal(100, result.Color.G);
            Assert.Equal(150, result.Color.B);
        }

        [Fact]
        public void ColorToSolidBrushConverter_ConvertBack_BrushToColor()
        {
            var converter = new ColorToSolidBrushConverter();
            var brush = new SolidColorBrush(Colors.Blue);
            var result = converter.ConvertBack(brush, typeof(Color), null, _culture);
            Assert.Equal(Colors.Blue, result);
        }

        [Fact]
        public void ColorToSolidBrushConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new ColorToSolidBrushConverter();
            var result = converter.ConvertBack(null, typeof(Color), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void ColorToSolidBrushConverter_RoundTrip()
        {
            var converter = new ColorToSolidBrushConverter();
            var originalColor = Color.FromArgb(200, 75, 125, 175);
            var brush = converter.Convert(originalColor, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            var resultColor = (Color)converter.ConvertBack(brush, typeof(Color), null, _culture);
            Assert.Equal(originalColor, resultColor);
        }

        #endregion

        #region DrawingColorToSolidColorBrushConverter Tests

        [Fact]
        public void DrawingColorToSolidColorBrushConverter_Convert_DrawingColorToBrush()
        {
            var converter = new DrawingColorToSolidColorBrushConverter();
            var drawingColor = System.Drawing.Color.FromArgb(255, 100, 150, 200);
            var result = converter.Convert(drawingColor, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(100, result.Color.R);
            Assert.Equal(150, result.Color.G);
            Assert.Equal(200, result.Color.B);
        }

        [Fact]
        public void DrawingColorToSolidColorBrushConverter_Convert_NullReturnsNull()
        {
            var converter = new DrawingColorToSolidColorBrushConverter();
            var result = converter.Convert(null, typeof(SolidColorBrush), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void DrawingColorToSolidColorBrushConverter_Convert_InvalidTypeReturnsNull()
        {
            var converter = new DrawingColorToSolidColorBrushConverter();
            var result = converter.Convert("string", typeof(SolidColorBrush), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void DrawingColorToSolidColorBrushConverter_Convert_WpfColorReturnsNull()
        {
            var converter = new DrawingColorToSolidColorBrushConverter();
            var result = converter.Convert(Colors.Red, typeof(SolidColorBrush), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void DrawingColorToSolidColorBrushConverter_Convert_PreservesAlpha()
        {
            var converter = new DrawingColorToSolidColorBrushConverter();
            var drawingColor = System.Drawing.Color.FromArgb(128, 50, 100, 150);
            var result = converter.Convert(drawingColor, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(128, result.Color.A);
        }

        [Fact]
        public void DrawingColorToSolidColorBrushConverter_ConvertBack_BrushToDrawingColor()
        {
            var converter = new DrawingColorToSolidColorBrushConverter();
            var brush = new SolidColorBrush(Color.FromArgb(255, 50, 100, 150));
            var result = converter.ConvertBack(brush, typeof(System.Drawing.Color), null, _culture);
            Assert.IsType<System.Drawing.Color>(result);
            var drawingColor = (System.Drawing.Color)result;
            Assert.Equal(50, drawingColor.R);
            Assert.Equal(100, drawingColor.G);
            Assert.Equal(150, drawingColor.B);
        }

        [Fact]
        public void DrawingColorToSolidColorBrushConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new DrawingColorToSolidColorBrushConverter();
            var result = converter.ConvertBack(null, typeof(System.Drawing.Color), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void DrawingColorToSolidColorBrushConverter_ConvertBack_InvalidTypeReturnsNull()
        {
            var converter = new DrawingColorToSolidColorBrushConverter();
            var result = converter.ConvertBack("string", typeof(System.Drawing.Color), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void DrawingColorToSolidColorBrushConverter_RoundTrip()
        {
            var converter = new DrawingColorToSolidColorBrushConverter();
            var originalColor = System.Drawing.Color.FromArgb(200, 75, 125, 175);
            var brush = converter.Convert(originalColor, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            var resultColor = (System.Drawing.Color)converter.ConvertBack(brush, typeof(System.Drawing.Color), null, _culture);
            Assert.Equal(originalColor.A, resultColor.A);
            Assert.Equal(originalColor.R, resultColor.R);
            Assert.Equal(originalColor.G, resultColor.G);
            Assert.Equal(originalColor.B, resultColor.B);
        }

        #endregion

        #region GridlineColorLightConverter Tests

        [Fact]
        public void GridlineColorLightConverter_Convert_NullReturnsDefaultBrush()
        {
            var converter = new GridlineColorLightConverter();
            var result = converter.Convert(null, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(51, result.Color.A);
            Assert.Equal(0, result.Color.R);
            Assert.Equal(0, result.Color.G);
            Assert.Equal(0, result.Color.B);
        }

        [Fact]
        public void GridlineColorLightConverter_Convert_InvalidTypeReturnsDefaultBrush()
        {
            var converter = new GridlineColorLightConverter();
            var result = converter.Convert("string", typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(51, result.Color.A);
        }

        [Fact]
        public void GridlineColorLightConverter_Convert_FullyOpaqueBrushGetsAlpha51()
        {
            var converter = new GridlineColorLightConverter();
            var brush = new SolidColorBrush(Color.FromArgb(255, 100, 150, 200));
            var result = converter.Convert(brush, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(51, result.Color.A);
            Assert.Equal(100, result.Color.R);
            Assert.Equal(150, result.Color.G);
            Assert.Equal(200, result.Color.B);
        }

        [Fact]
        public void GridlineColorLightConverter_Convert_SemiTransparentBrushGetsAlpha51()
        {
            var converter = new GridlineColorLightConverter();
            var brush = new SolidColorBrush(Color.FromArgb(100, 100, 150, 200));
            var result = converter.Convert(brush, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(51, result.Color.A);
        }

        [Fact]
        public void GridlineColorLightConverter_Convert_VeryTransparentBrushGetsReducedAlpha()
        {
            var converter = new GridlineColorLightConverter();
            var brush = new SolidColorBrush(Color.FromArgb(50, 100, 150, 200));
            var result = converter.Convert(brush, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            // Alpha = 50 * 0.2 = 10
            Assert.Equal(10, result.Color.A);
            Assert.Equal(100, result.Color.R);
            Assert.Equal(150, result.Color.G);
            Assert.Equal(200, result.Color.B);
        }

        [Fact]
        public void GridlineColorLightConverter_Convert_AlphaExactly51GetsAlpha51()
        {
            var converter = new GridlineColorLightConverter();
            var brush = new SolidColorBrush(Color.FromArgb(51, 100, 150, 200));
            var result = converter.Convert(brush, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(51, result.Color.A);
        }

        [Fact]
        public void GridlineColorLightConverter_Convert_AlphaZeroStaysZero()
        {
            var converter = new GridlineColorLightConverter();
            var brush = new SolidColorBrush(Color.FromArgb(0, 100, 150, 200));
            var result = converter.Convert(brush, typeof(SolidColorBrush), null, _culture) as SolidColorBrush;
            Assert.NotNull(result);
            Assert.Equal(0, result.Color.A);
        }

        [Fact]
        public void GridlineColorLightConverter_ConvertBack_ThrowsNotImplementedException()
        {
            var converter = new GridlineColorLightConverter();
            Assert.Throws<NotImplementedException>(() =>
                converter.ConvertBack(null, typeof(SolidColorBrush), null, _culture));
        }

        #endregion

        #region FontToFontFamilyConverter Tests

        [Fact]
        public void FontToFontFamilyConverter_Convert_StringToFontFamily()
        {
            var converter = new FontToFontFamilyConverter();
            var result = converter.Convert("Arial", typeof(FontFamily), null, _culture) as FontFamily;
            Assert.NotNull(result);
            Assert.Equal("Arial", result.Source);
        }

        [Fact]
        public void FontToFontFamilyConverter_Convert_NullReturnsNull()
        {
            var converter = new FontToFontFamilyConverter();
            var result = converter.Convert(null, typeof(FontFamily), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void FontToFontFamilyConverter_Convert_IntUsesToString()
        {
            var converter = new FontToFontFamilyConverter();
            var result = converter.Convert(123, typeof(FontFamily), null, _culture) as FontFamily;
            Assert.NotNull(result);
            Assert.Equal("123", result.Source);
        }

        [Fact]
        public void FontToFontFamilyConverter_ConvertBack_FontFamilyToString()
        {
            var converter = new FontToFontFamilyConverter();
            var fontFamily = new FontFamily("Times New Roman");
            var result = converter.ConvertBack(fontFamily, typeof(string), null, _culture);
            Assert.Equal("Times New Roman", result);
        }

        [Fact]
        public void FontToFontFamilyConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new FontToFontFamilyConverter();
            var result = converter.ConvertBack(null, typeof(string), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void FontToFontFamilyConverter_RoundTrip()
        {
            var converter = new FontToFontFamilyConverter();
            var originalName = "Courier New";
            var fontFamily = converter.Convert(originalName, typeof(FontFamily), null, _culture) as FontFamily;
            var resultName = converter.ConvertBack(fontFamily, typeof(string), null, _culture);
            Assert.Equal(originalName, resultName);
        }

        #endregion

        #region FontFamilyToFontStringConverter Tests

        [Fact]
        public void FontFamilyToFontStringConverter_Convert_FontFamilyToString()
        {
            var converter = new FontFamilyToFontStringConverter();
            var fontFamily = new FontFamily("Verdana");
            var result = converter.Convert(fontFamily, typeof(string), null, _culture);
            Assert.Equal("Verdana", result);
        }

        [Fact]
        public void FontFamilyToFontStringConverter_Convert_NullReturnsNull()
        {
            var converter = new FontFamilyToFontStringConverter();
            var result = converter.Convert(null, typeof(string), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void FontFamilyToFontStringConverter_ConvertBack_StringToFontFamily()
        {
            var converter = new FontFamilyToFontStringConverter();
            var result = converter.ConvertBack("Georgia", typeof(FontFamily), null, _culture) as FontFamily;
            Assert.NotNull(result);
            Assert.Equal("Georgia", result.Source);
        }

        [Fact]
        public void FontFamilyToFontStringConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new FontFamilyToFontStringConverter();
            var result = converter.ConvertBack(null, typeof(FontFamily), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void FontFamilyToFontStringConverter_ConvertBack_IntUsesToString()
        {
            var converter = new FontFamilyToFontStringConverter();
            var result = converter.ConvertBack(456, typeof(FontFamily), null, _culture) as FontFamily;
            Assert.NotNull(result);
            Assert.Equal("456", result.Source);
        }

        [Fact]
        public void FontFamilyToFontStringConverter_RoundTrip()
        {
            var converter = new FontFamilyToFontStringConverter();
            var fontFamily = new FontFamily("Tahoma");
            var name = converter.Convert(fontFamily, typeof(string), null, _culture) as string;
            var resultFamily = converter.ConvertBack(name, typeof(FontFamily), null, _culture) as FontFamily;
            Assert.Equal(fontFamily.Source, resultFamily!.Source);
        }

        #endregion

        #region IntToDoubleConverter Tests

        [Fact]
        public void IntToDoubleConverter_Convert_IntToDouble()
        {
            var converter = new IntToDoubleConverter();
            var result = converter.Convert(42, typeof(double), null, _culture);
            Assert.Equal(42.0, result);
        }

        [Fact]
        public void IntToDoubleConverter_Convert_NullReturnsNull()
        {
            var converter = new IntToDoubleConverter();
            var result = converter.Convert(null, typeof(double), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void IntToDoubleConverter_Convert_ZeroReturnsZeroDouble()
        {
            var converter = new IntToDoubleConverter();
            var result = converter.Convert(0, typeof(double), null, _culture);
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void IntToDoubleConverter_Convert_NegativeInt()
        {
            var converter = new IntToDoubleConverter();
            var result = converter.Convert(-100, typeof(double), null, _culture);
            Assert.Equal(-100.0, result);
        }

        [Fact]
        public void IntToDoubleConverter_Convert_MaxInt()
        {
            var converter = new IntToDoubleConverter();
            var result = converter.Convert(int.MaxValue, typeof(double), null, _culture);
            Assert.Equal((double)int.MaxValue, result);
        }

        [Fact]
        public void IntToDoubleConverter_Convert_MinInt()
        {
            var converter = new IntToDoubleConverter();
            var result = converter.Convert(int.MinValue, typeof(double), null, _culture);
            Assert.Equal((double)int.MinValue, result);
        }

        [Fact]
        public void IntToDoubleConverter_ConvertBack_DoubleToInt()
        {
            var converter = new IntToDoubleConverter();
            var result = converter.ConvertBack(42.0, typeof(int), null, _culture);
            Assert.Equal(42, result);
        }

        [Fact]
        public void IntToDoubleConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new IntToDoubleConverter();
            var result = converter.ConvertBack(null, typeof(int), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void IntToDoubleConverter_ConvertBack_DoubleRoundedToInt()
        {
            var converter = new IntToDoubleConverter();
            var result = converter.ConvertBack(42.9, typeof(int), null, _culture);
            Assert.Equal(43, result); // Convert.ToInt32 uses banker's rounding
        }

        [Fact]
        public void IntToDoubleConverter_ConvertBack_ZeroDouble()
        {
            var converter = new IntToDoubleConverter();
            var result = converter.ConvertBack(0.0, typeof(int), null, _culture);
            Assert.Equal(0, result);
        }

        [Fact]
        public void IntToDoubleConverter_ConvertBack_NegativeDouble()
        {
            var converter = new IntToDoubleConverter();
            var result = converter.ConvertBack(-50.5, typeof(int), null, _culture);
            Assert.Equal(-50, result);
        }

        #endregion

        #region DoubleToStringConverter Tests

        [Fact]
        public void DoubleToStringConverter_Convert_StringToDouble()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.Convert("42.5", typeof(double), null, _culture);
            Assert.Equal(42.5, result);
        }

        [Fact]
        public void DoubleToStringConverter_Convert_NullReturnsNull()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.Convert(null, typeof(double), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void DoubleToStringConverter_Convert_IntegerString()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.Convert("100", typeof(double), null, _culture);
            Assert.Equal(100.0, result);
        }

        [Fact]
        public void DoubleToStringConverter_Convert_NegativeString()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.Convert("-75.25", typeof(double), null, _culture);
            Assert.Equal(-75.25, result);
        }

        [Fact]
        public void DoubleToStringConverter_Convert_ScientificNotation()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.Convert("1.5E2", typeof(double), null, _culture);
            Assert.Equal(150.0, result);
        }

        [Fact]
        public void DoubleToStringConverter_Convert_InvalidStringReturnsZero()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.Convert("not a number", typeof(double), null, _culture);
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void DoubleToStringConverter_Convert_EmptyStringReturnsZero()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.Convert("", typeof(double), null, _culture);
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void DoubleToStringConverter_Convert_DoubleValue()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.Convert(123.456, typeof(double), null, _culture);
            Assert.Equal(123.456, result);
        }

        [Fact]
        public void DoubleToStringConverter_ConvertBack_DoubleToString()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.ConvertBack(42.5, typeof(string), null, _culture);
            Assert.Equal("42.5", result);
        }

        [Fact]
        public void DoubleToStringConverter_ConvertBack_NullReturnsNull()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.ConvertBack(null, typeof(string), null, _culture);
            Assert.Null(result);
        }

        [Fact]
        public void DoubleToStringConverter_ConvertBack_IntegerDouble()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.ConvertBack(100.0, typeof(string), null, _culture);
            Assert.Equal("100", result);
        }

        [Fact]
        public void DoubleToStringConverter_ConvertBack_NegativeValue()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.ConvertBack(-75.25, typeof(string), null, _culture);
            Assert.Equal("-75.25", result);
        }

        [Fact]
        public void DoubleToStringConverter_ConvertBack_Zero()
        {
            var converter = new DoubleToStringConverter();
            var result = converter.ConvertBack(0.0, typeof(string), null, _culture);
            Assert.Equal("0", result);
        }

        [Fact]
        public void DoubleToStringConverter_RoundTrip()
        {
            var converter = new DoubleToStringConverter();
            var original = "123.456";
            var doubleValue = (double)converter.Convert(original, typeof(double), null, _culture);
            var result = converter.ConvertBack(doubleValue, typeof(string), null, _culture);
            Assert.Equal("123.456", result);
        }

        #endregion
    }
}
