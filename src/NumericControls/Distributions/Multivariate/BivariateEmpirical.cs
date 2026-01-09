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
using System.ComponentModel;

namespace NumericControls.Distributions.Multivariate
{
    /// <summary>
    /// Represents a bivariate empirical distribution for modeling paired data with cumulative distribution functions.
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
    public class BivariateEmpirical : INotifyPropertyChanged
    {

        private BivariateEmpirical _bivariateEmpiricalCDF;

        /// <summary>
        /// Gets or sets the bivariate empirical cumulative distribution function.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b> Authors: </b>
        /// <list type="bullet">
        ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
        /// </list>
        /// </para>
        /// </remarks>
        public BivariateEmpirical BivariateEmpiricalCDF
        {
            get { return _bivariateEmpiricalCDF; }
            set 
            {
                _bivariateEmpiricalCDF = value;
                RaisePropertyChange(nameof(BivariateEmpiricalCDF));
            }
        }

        /// <summary>
        /// Gets or sets the array of X1 values (distribution 1). Points on the cumulative curve are specified
        /// with increasing value and increasing probability.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b> Authors: </b>
        /// <list type="bullet">
        ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
        /// </list>
        /// </para>
        /// </remarks>
        public ObservableCollection<double> X1Values { get; set; }

        /// <summary>
        /// Gets or sets the array of X2 values (distribution 2). Points on the cumulative curve are specified
        /// with increasing value and increasing probability.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b> Authors: </b>
        /// <list type="bullet">
        ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
        /// </list>
        /// </para>
        /// </remarks>
        public ObservableCollection<double> X2Values { get; set; }


        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b> Authors: </b>
        /// <list type="bullet">
        ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
        /// </list>
        /// </para>
        /// </remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <remarks>
        /// <para>
        /// <b> Authors: </b>
        /// <list type="bullet">
        ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
        /// </list>
        /// </para>
        /// </remarks>
        public void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
