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
using System.Xml.Linq;
using OxyPlot.Wpf;
using OxyPlot.Wpf.Serialization;

namespace OxyPlotControls
{
    /// <summary>
    /// Provides serialization and deserialization functionality for OxyPlot plot settings.
    /// Delegates to <see cref="PlotSerializer"/> for the actual implementation.
    /// </summary>
    public static class OxyPlotSettingsSerializer
    {
        /// <summary>
        /// The XML tag name used for the root element containing OxyPlot properties.
        /// </summary>
        public static readonly string OxyplotPropertiesTag = PlotSerializer.OxyplotPropertiesTag;

        /// <summary>
        /// Serializes all plot properties to an XElement for persistence.
        /// </summary>
        /// <param name="plot">The OxyPlot Plot control to serialize.</param>
        /// <returns>An XElement containing all serialized plot properties including general settings, legend, axes, annotations, and series.</returns>
        public static XElement ToXelement(Plot plot)
        {
            return PlotSerializer.ToXElement(plot);
        }

        /// <summary>
        /// Deserializes plot properties from an XElement and applies them to the plot.
        /// </summary>
        /// <param name="plot">The OxyPlot Plot control to apply settings to.</param>
        /// <param name="element">The XElement containing serialized plot properties.</param>
        public static void FromXelement(Plot plot, XElement element)
        {
            PlotSerializer.FromXElement(plot, element);
        }
    }
}
