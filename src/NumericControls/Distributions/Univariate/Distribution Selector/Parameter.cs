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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericControls
{
    /// <summary>
    /// Represents a distribution parameter with validation support.
    /// Used by distribution selector controls to display and edit distribution parameters.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class implements <see cref="INotifyPropertyChanged"/> to support WPF data binding.
    /// When the <see cref="Value"/> property changes, the control can automatically update the
    /// associated probability distribution and re-validate the parameters.
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create a parameter for the mean of a Normal distribution
    /// var meanParam = new Parameter("Mean", "μ (Mean)", 100.0);
    /// meanParam.PropertyChanged += (s, e) => {
    ///     if (e.PropertyName == nameof(Parameter.Value))
    ///         UpdateDistribution();
    /// };
    /// </code>
    /// </example>
    public class Parameter : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the display name of the parameter, typically shown in the UI.
        /// </summary>
        /// <value>A human-readable name for the parameter (e.g., "μ (Mean)").</value>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the internal name of the parameter, used for programmatic identification.
        /// </summary>
        /// <value>The parameter's internal identifier (e.g., "Mean").</value>
        public string Name { get; private set; }

        private double _value;
        private bool _isValid = true;
        private string _errorMessage = null;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the numeric value of the parameter.
        /// </summary>
        /// <value>The parameter's current numeric value.</value>
        /// <remarks>
        /// When this value changes, the <see cref="PropertyChanged"/> event is raised,
        /// allowing bound UI elements to update automatically.
        /// </remarks>
        public double Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the parameter value is valid.
        /// </summary>
        /// <value>
        /// <c>true</c> if the parameter value is within acceptable bounds; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This property is used to trigger visual validation feedback in the UI.
        /// When set to <c>false</c>, the parameter cell typically displays with a red background.
        /// </remarks>
        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (value != _isValid)
                {
                    _isValid = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
                }
            }
        }

        /// <summary>
        /// Gets or sets the error message describing why the parameter is invalid.
        /// </summary>
        /// <value>
        /// A descriptive error message when <see cref="IsValid"/> is <c>false</c>; otherwise, <c>null</c>.
        /// </value>
        /// <remarks>
        /// This message is typically displayed as a tooltip when the user hovers over
        /// an invalid parameter cell in the UI.
        /// </remarks>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (value != _errorMessage)
                {
                    _errorMessage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="name">The internal name of the parameter.</param>
        /// <param name="displayName">The display name shown in the UI.</param>
        /// <param name="value">The initial numeric value of the parameter.</param>
        public Parameter(string name, string displayName, double value)
        {
            Name = name;
            DisplayName = displayName;
            Value = value;
            IsValid = true;
        }
    }
}
