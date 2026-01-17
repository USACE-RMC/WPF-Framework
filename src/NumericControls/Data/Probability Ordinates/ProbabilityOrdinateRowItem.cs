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

using GenericControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericControls
{
    /// <summary>
    /// Represents a data grid row item for probability ordinate values, used for specifying
    /// exceedance probability levels at which to evaluate fitted probability distributions.
    /// These ordinates are commonly used in frequency analysis and risk assessment applications.
    /// </summary>
    public class ProbabilityOrdinateRowItem : DataGridRowItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProbabilityOrdinateRowItem"/> class with default values.
        /// </summary>
        public ProbabilityOrdinateRowItem() : base(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProbabilityOrdinateRowItem"/> class with the specified probability value.
        /// </summary>
        /// <param name="observableCollection">The observable collection that contains this row item.</param>
        /// <param name="probability">The exceedance probability value, ranging from 0 to 1.</param>
        public ProbabilityOrdinateRowItem(ObservableCollection<Object> observableCollection, double probability) : base(observableCollection)
        {
            _probability = probability;
        }

        /// <summary>
        /// The backing field for the Probability property.
        /// </summary>
        private double _probability;

        /// <summary>
        /// Gets or sets the exceedance probability value.
        /// Must be between 0 and 1, where 0 represents a certain event and 1 represents an impossible event.
        /// </summary>
        public double Probability
        {
            get { return _probability; }
            set
            {
                if (_probability != value)
                {
                    _probability = value;
                    NotifyPropertyChanged(nameof(Probability));
                }
            }
        }

        /// <summary>
        /// Adds validation rules for the probability ordinate row item properties.
        /// Validates that probability values are in ascending order and within the valid range of 0 to 1.
        /// </summary>
        public override void AddValidationRules()
        {
            AddRule(nameof(Probability), () => OrderRule<double, ProbabilityOrdinateRowItem>((x) => x.Probability, nameof(Probability), true, false), "The exceedance probability values must be in ascending order.");
            AddRule(nameof(Probability), () => Probability < 0d, "The exceedance probability value must be between 0 and 1.");
            AddRule(nameof(Probability), () => Probability > 1d, "The exceedance probability value must be between 0 and 1.");
        }

        /// <summary>
        /// Determines whether the specified property should be displayed in the data grid.
        /// </summary>
        /// <param name="propertyName">The name of the property to check.</param>
        /// <returns>Always returns true, indicating all properties are displayable.</returns>
        public override bool IsGridDisplayable(string propertyName)
        {
            return true;
        }

        /// <summary>
        /// Gets the display name for the specified property to be shown in the data grid header.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The display name for the property, or null if not recognized.</returns>
        public override string PropertyDisplayName(string propertyName)
        {
            if (propertyName == nameof(Probability))
            {
                return "Probability Ordinates";
            }
            else
            {
                return null;
            }
        }
    }
}
