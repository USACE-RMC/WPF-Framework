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

using OxyPlot;

namespace OxyPlotControls
{
    /// <summary>
    /// Defines the color values for an OxyPlot chart theme.
    /// Themes control chart infrastructure colors (background, axes, text, gridlines, legend, annotations)
    /// while preserving user-controlled series colors.
    /// </summary>
    public class OxyPlotTheme
    {
        // General
        public OxyColor Background { get; set; }
        public OxyColor PlotAreaBackground { get; set; }
        public OxyColor PlotAreaBorderColor { get; set; }

        // Title & Subtitle
        public OxyColor TitleColor { get; set; }
        public OxyColor SubtitleColor { get; set; }

        // Axes
        public OxyColor AxisTitleColor { get; set; }
        public OxyColor AxisLineColor { get; set; }
        public OxyColor AxisTextColor { get; set; }
        public OxyColor AxisTickColor { get; set; }
        public OxyColor MajorGridlineColor { get; set; }
        public OxyColor MinorGridlineColor { get; set; }

        // Legend
        public OxyColor LegendTitleColor { get; set; }
        public OxyColor LegendTextColor { get; set; }
        public OxyColor LegendBackground { get; set; }
        public OxyColor LegendBorderColor { get; set; }

        // Annotations
        public OxyColor AnnotationTextColor { get; set; }
        public OxyColor AnnotationStrokeColor { get; set; }
    }
}
