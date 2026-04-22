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

using System.Collections.ObjectModel;
using GenericControls;

namespace NumericControls
{
    /// <summary>
    /// Represents a validating row item for univariate distribution data entry in a data grid,
    /// providing validation for X values and probabilities.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class UnivariateDistributionValidatingRow : DataGridRowItem
    {

        private double _xMax;
        private double _xMin;
        private double _x;
        private double _p;

        /// <summary>
        /// Gets or sets the X value (random variable value).
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b> Authors: </b>
        /// <list type="bullet">
        ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
        /// </list>
        /// </para>
        /// </remarks>
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                NotifyPropertyChanged(nameof(X));
            }
        }

        /// <summary>
        /// Gets or sets the probability value associated with the X value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b> Authors: </b>
        /// <list type="bullet">
        ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
        /// </list>
        /// </para>
        /// </remarks>
        public double P
        {
            get { return _p; }

            set
            {
                _p = value;
                NotifyPropertyChanged(nameof(P));
            }
        }

        /// <summary>
        /// Gets or sets the minimum allowed X value for validation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b> Authors: </b>
        /// <list type="bullet">
        ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
        /// </list>
        /// </para>
        /// </remarks>
        public double MinX
        {
            get { return _xMin; }
            set
            {
                if (_xMin != value)
                {
                    _xMin = value;
                    NotifyPropertyChanged(nameof(MinX));
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum allowed X value for validation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b> Authors: </b>
        /// <list type="bullet">
        ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
        /// </list>
        /// </para>
        /// </remarks>
        public double MaxX
        {
            get { return _xMax; }
            set
            {
                if (_xMax != value)
                {
                    _xMax = value;
                    NotifyPropertyChanged(nameof(MaxX));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the UnivariateDistributionValidatingRow class with default values.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b> Authors: </b>
        /// <list type="bullet">
        ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
        /// </list>
        /// </para>
        /// </remarks>
        public UnivariateDistributionValidatingRow() : base(null)
        {
            // 
            _x = 0d;
            _p = 0d;
            _xMin = double.MinValue;
            _xMax = double.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the UnivariateDistributionValidatingRow class with specified values and validation constraints.
        /// </summary>
        /// <param name="xValue">The X value (random variable value).</param>
        /// <param name="pValue">The probability value.</param>
        /// <param name="xMin">The minimum allowed X value.</param>
        /// <param name="xMax">The maximum allowed X value.</param>
        /// <param name="theList">The observable collection this row belongs to.</param>
        /// <remarks>
        /// <para>
        /// <b> Authors: </b>
        /// <list type="bullet">
        ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
        /// </list>
        /// </para>
        /// </remarks>
        public UnivariateDistributionValidatingRow(double xValue, double pValue, double xMin, double xMax, ObservableCollection<object> theList) : base(theList)
        {
            // 
            _x = xValue;
            _p = pValue;
            _xMin = xMin;
            _xMax = xMax;
        }

        /// <inheritdoc/>
        public override void AddValidationRules()
        {
            AddRule(nameof(P), () => P > 1d, "Probability must be less than or equal to 1");
            AddRule(nameof(P), () => P < 0d, "Probability must be greater than or equal to 0");
            AddRule(nameof(X), () => X > MaxX, "X must be less than or equal to " + MaxX);
            AddRule(nameof(X), () => X < MinX, "X must be greater than or equal to " + MinX);
            AddRule(nameof(P), () => OrderRule<double, UnivariateDistributionValidatingRow>(o => o.P, nameof(P)), "Probability must be in ascending order");
            AddRule(nameof(X), () => OrderRule<double, UnivariateDistributionValidatingRow>(o => o.X, nameof(X)), "X values must be in ascending order");
        }

        /// <inheritdoc/>
        public override string PropertyDisplayName(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(X): return "X";
                case nameof(P): return "Probability";
                default: return propertyName;
            }
        }

        /// <inheritdoc/>
        public override bool IsGridDisplayable(string propertyName)
        {
            if ((propertyName == nameof(X)) || (propertyName == nameof(P))) return true;
            return false;
        }

    }
}
