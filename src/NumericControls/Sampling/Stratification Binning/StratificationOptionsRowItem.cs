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
using Numerics.Sampling;

namespace NumericControls
{
    /// <summary>
    /// Represents a row item for defining stratification options in a data grid.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class StratificationOptionsRowItem : DataGridRowItem
    {
        private double _start;
        private double _end;
        private int _numberOfBins;
        private int _maxBins;
        private bool _isProbability = true;

        /// <summary>
        /// The lower bound of the starting bin.
        /// </summary>
        public double Start
        {
            get { return _start; }
            set
            {
                if (_start != value)
                {
                    _start = value;
                    NotifyPropertyChanged(nameof(Start));
                }
            }
        }

        /// <summary>
        /// The upper bound of the end bin.
        /// </summary>
        public double End
        {
            get { return _end; }
            set
            {
                if (_end != value)
                {
                    _end = value;
                    NotifyPropertyChanged(nameof(End));
                }
            }
        }

        /// <summary>
        /// The number of bins. Must be greater than 1.
        /// </summary>
        public int NumberOfBins
        {
            get { return _numberOfBins; }
            set
            {
                if (_numberOfBins != value)
                {
                    _numberOfBins = value;
                    NotifyPropertyChanged(nameof(NumberOfBins));
                }
            }
        }

        /// <summary>
        /// The maximum number of bins.
        /// </summary>
        public int MaxBins
        {
            get { return _maxBins; }
            set
            {
                if (_maxBins != value)
                {
                    _maxBins = value;
                    NotifyPropertyChanged(nameof(MaxBins));
                }
            }
        }

        /// <summary>
        /// Determines whether or not the values are probabilities.
        /// </summary>
        public bool IsProbability
        {
            get { return _isProbability; }
            set
            {
                if (_isProbability != value)
                {
                    _isProbability = value;
                    NotifyPropertyChanged(nameof(IsProbability));
                }
            }
        }

        /// <summary>
        /// Create empty row item.
        /// </summary>
        public StratificationOptionsRowItem() : base(null)
        {
            _start = 0d;
            _end = 0d;
            _numberOfBins = 0;
            _maxBins = 1000;
            _isProbability = false;
        }

        /// <summary>
        /// Create new row item with specific options.
        /// </summary>
        /// <param name="stratificationOptions">Stratification options.</param>
        /// <param name="maximumBins">Max number of bins.</param>
        /// <param name="list">List of stratification options.</param>
        public StratificationOptionsRowItem(StratificationOptions stratificationOptions, int maximumBins, ObservableCollection<object> list) : base(list)
        {
            _start = stratificationOptions.LowerBound;
            _end = stratificationOptions.UpperBound;
            _numberOfBins = stratificationOptions.NumberOfBins;
            _isProbability = stratificationOptions.IsProbability;
            _maxBins = maximumBins;
            // Must add this rule here so that the maximum number of bins is appropriately defined. The AddValidationRules() Method is called inside the MyBase.New() method.
            AddRule(nameof(NumberOfBins), () => NumberOfBins > MaxBins, $"Number of bins must be less than or equal to {MaxBins}.");
        }

        /// <summary>
        /// Returns stratification options given the row item inputs.
        /// </summary>
        public StratificationOptions GetStratificationOptions()
        {
            return new StratificationOptions(_start, _end, _numberOfBins, _isProbability);
        }

        /// <summary>
        /// Add validation rules for the row item.
        /// </summary>
        public override void AddValidationRules()
        {
            // Order rules
            AddRule(nameof(Start), () => OrderRule<double, StratificationOptionsRowItem>(o => o.Start, nameof(Start)), "Start values must be in ascending order.");
            AddRule(nameof(End), () => OrderRule<double, StratificationOptionsRowItem>(o => o.End, nameof(End)), "End values must be in ascending order.");
            // Greater than/less than rules
            AddRule(nameof(Start), () => Start >= End, "Start value must be less than the end value.", new[] { nameof(End) });
            AddRule(nameof(End), () => Start >= End, "End value must be greater than the start value.", new[] { nameof(Start) });
            // Probability rules
            AddRule(nameof(Start), () =>
            {
                if (_isProbability == true)
                {
                    if (Start < 0.0d) return true;
                }
                return false;
            }, "Start value must be greater than or equal to 0.");
            AddRule(nameof(End), () =>
            {
                if (_isProbability == true)
                {
                    if (End > 1.0d) return true;
                }
                return false;
            }, "End value must be less than or equal to 1.");
            // 
            AddRule(nameof(NumberOfBins), () => NumberOfBins < 2, "Number of bins must be greater than 1.");
        }

        /// <summary>
        /// Change column headers if necessary.
        /// </summary>
        /// <param name="propertyName">Row item property name.</param>
        public override string PropertyDisplayName(string propertyName)
        {
            if (propertyName == nameof(NumberOfBins)) return "# Bins";
            return propertyName;
        }

        /// <summary>
        /// Determines which properties are displayed in the data grid.
        /// </summary>
        /// <param name="propertyName">Row item property name.</param>
        public override bool IsGridDisplayable(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Start): return true;
                case nameof(End): return true;
                case nameof(NumberOfBins): return true;
                default: return false;
            }
        }

    }
}
