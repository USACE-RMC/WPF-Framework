/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace GenericControls
{

    /// <summary>
    /// A wrapper class that exposes minimum and maximum values as dependency properties for use in WPF bindings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class RangeWrapper : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="Maximum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(RangeWrapper), new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// gets/sets the maximum allowable value.
        /// </summary>
        public double Maximum
        {
            get
            {
                return (double)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Minimum"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(RangeWrapper), new FrameworkPropertyMetadata(double.MinValue));
        /// <summary>
        /// gets/sets the minimum allowable value.
        /// </summary>
        public double Minimum
        {
            get
            {
                return (double)GetValue(MinimumProperty);
            }
            set
            {
                SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="BoundsAreExclusive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BoundsAreExclusiveProperty = DependencyProperty.Register(nameof(BoundsAreExclusive), typeof(bool), typeof(RangeWrapper), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets/sets whether the bounds (Minimum and Maximum) are exclusive.
        /// When true, values equal to the bounds are considered invalid (value must be strictly between min and max).
        /// When false (default), values equal to the bounds are valid.
        /// </summary>
        public bool BoundsAreExclusive
        {
            get
            {
                return (bool)GetValue(BoundsAreExclusiveProperty);
            }
            set
            {
                SetValue(BoundsAreExclusiveProperty, value);
            }
        }

    }

    /// <summary>
    /// A validation rule that checks whether a numeric input falls within a specified range defined by a <see cref="RangeWrapper"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class RangeValidationRule : ValidationRule
    {
        /// <summary>
        /// Gets/sets the wrapper object containing minimum and maximum values for validation.
        /// </summary>
        public RangeWrapper Wrapper { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidationRule"/> class.
        /// </summary>
        public RangeValidationRule()
        {
        }

        /// <summary>
        /// Validates whether the input value is within the range specified by the <see cref="Wrapper"/>
        /// </summary>
        /// <param name="value">The value from the binding target to validate.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double numericValue = 0d;
            string stringValue = value?.ToString();

            if (string.IsNullOrEmpty(stringValue))
            {
                return ValidationResult.ValidResult;
            }

            if (!NumberFormatHelper.TryParseDouble(stringValue, out numericValue))
            {
                return new ValidationResult(false, "Invalid number format.");
            }

            if (Wrapper.BoundsAreExclusive)
            {
                if (numericValue <= Wrapper.Minimum || numericValue >= Wrapper.Maximum)
                {
                    return new ValidationResult(false, $"Number must be greater than '{NumberFormatHelper.FormatDouble(Wrapper.Minimum)}' and less than '{NumberFormatHelper.FormatDouble(Wrapper.Maximum)}'.");
                }
            }
            else if (numericValue < Wrapper.Minimum || numericValue > Wrapper.Maximum)
            {
                return new ValidationResult(false, $"Number must be greater than or equal to '{NumberFormatHelper.FormatDouble(Wrapper.Minimum)}' and less than or equal to '{NumberFormatHelper.FormatDouble(Wrapper.Maximum)}'.");
            }

            return ValidationResult.ValidResult;
        }
    }

    /// <summary>
    /// A proxy class used to expose data context from a non-visual element into XAML binding scenarios.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class BindingProxy : Freezable
    {
        /// <summary>
        /// Creates a new instance of <see cref="BindingProxy"/>
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        /// <summary>
        /// gets/sets the data object to expose through the proxy.
        /// </summary>
        public object Data
        {
            get
            {
                return GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Data"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
    }
}