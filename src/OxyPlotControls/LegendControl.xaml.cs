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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml.Linq;
using OxyPlot.Legends;
using Wpf = OxyPlot.Wpf;

namespace OxyPlotControls
{
    /// <summary>
    /// A user control that provides UI for editing legend properties of an OxyPlot chart,
    /// including title, items, area styling, and position settings.
    /// </summary>
    public partial class LegendControl : UserControl
    {
        #region Fields

        /// <summary>
        /// The XML tag name used for serializing legend properties.
        /// </summary>
        public static readonly string LegendPropertiesTag = "Legend";

        /// <summary>
        /// Flag to suppress PlotChanged events during initialization or programmatic updates.
        /// </summary>
        private bool _suppressPlotChanged = true;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when any plot property has been modified through user interaction.
        /// </summary>
        /// <remarks>
        /// This event is raised when bindings update the source (Plot) properties.
        /// Subscribe to this event to track unsaved changes and update dirty state.
        /// The event is suppressed during control initialization and when Plot property changes.
        /// </remarks>
        public event EventHandler? PlotChanged;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="Plot"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlotProperty = DependencyProperty.Register(
            nameof(Plot), typeof(Wpf.Plot), typeof(LegendControl),
            new PropertyMetadata(null, OnPlotPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ExpanderStyle"/> dependency property.
        /// </summary>
        public static DependencyProperty ExpanderStyleProperty = DependencyProperty.Register(
            nameof(ExpanderStyle), typeof(Style), typeof(LegendControl));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the OxyPlot Plot control that this control edits.
        /// </summary>
        public Wpf.Plot Plot
        {
            get { return (Wpf.Plot)GetValue(PlotProperty); }
            set { SetValue(PlotProperty, value); }
        }

        /// <summary>
        /// Gets or sets the style applied to expander controls within this control.
        /// </summary>
        public Style ExpanderStyle
        {
            get { return (Style)GetValue(ExpanderStyleProperty); }
            set { SetValue(ExpanderStyleProperty, value); }
        }

        /// <summary>
        /// Gets the available legend orientation options.
        /// </summary>
        public static List<LegendOrientation> OrientationOptions { get; } = new List<LegendOrientation>((LegendOrientation[])Enum.GetValues(typeof(LegendOrientation)));

        /// <summary>
        /// Gets the available legend item order options.
        /// </summary>
        public static List<LegendItemOrder> ItemOrderOptions { get; } = new List<LegendItemOrder>((LegendItemOrder[])Enum.GetValues(typeof(LegendItemOrder)));

        /// <summary>
        /// Gets the available legend placement options.
        /// </summary>
        public static List<LegendPlacement> PlacementOptions { get; } = new List<LegendPlacement>((LegendPlacement[])Enum.GetValues(typeof(LegendPlacement)));

        /// <summary>
        /// Gets the available legend position options.
        /// </summary>
        public static List<LegendPosition> PositionOptions { get; } = new List<LegendPosition>((LegendPosition[])Enum.GetValues(typeof(LegendPosition)));

        /// <summary>
        /// Gets the available legend symbol placement options.
        /// </summary>
        public static List<LegendSymbolPlacement> SymbolPlacementOptions { get; } = new List<LegendSymbolPlacement>((LegendSymbolPlacement[])Enum.GetValues(typeof(LegendSymbolPlacement)));

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LegendControl"/> class.
        /// </summary>
        public LegendControl()
        {
            InitializeComponent();

            // Enable PlotChanged events after control is fully loaded
            Loaded += (s, e) => _suppressPlotChanged = false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises the <see cref="PlotChanged"/> event if not suppressed.
        /// </summary>
        protected virtual void OnPlotChanged()
        {
            if (!_suppressPlotChanged)
            {
                PlotChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Called when the Plot dependency property changes.
        /// Suppresses PlotChanged events during the update and forces a layout refresh.
        /// </summary>
        /// <param name="d">The dependency object (LegendControl instance).</param>
        /// <param name="e">The event arguments containing old and new values.</param>
        private static void OnPlotPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is LegendControl control && e.NewValue != null)
            {
                // Suppress events while bindings update to new Plot
                control._suppressPlotChanged = true;

                // Force layout update to sync bindings
                control.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new Action(() =>
                {
                    control.UpdateLayout();
                    // Re-enable events after bindings have settled
                    control._suppressPlotChanged = false;
                }));
            }
        }

        /// <summary>
        /// Handles the Binding.SourceUpdated attached event.
        /// Called when any binding with NotifyOnSourceUpdated=True updates its source.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnBindingSourceUpdated(object sender, DataTransferEventArgs e)
        {
            OnPlotChanged();
        }

        /// <summary>
        /// Handles legend property ComboBox selection changes and invalidates the plot to refresh the display.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LegendPropertyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Invalidate the plot to refresh the legend display
            // Use true to force a full update including layout recalculation
            if (Plot != null)
            {
                Plot.InvalidatePlot(true);
            }
        }

        #endregion

        /// <summary>
        /// Serializes legend properties to an XML element for persistence.
        /// </summary>
        /// <param name="plot">The OxyPlot Plot control whose legend properties will be serialized.</param>
        /// <returns>An XElement containing all serialized legend properties.</returns>
        public static XElement LegendPropertiesToXElement(Wpf.Plot plot)
        {
            var legendProperties = new XElement(LegendPropertiesTag);

            var fwc = new FontWeightConverter();

            // Legend Area
            var legendAreaProperties = new XElement("Area");
            legendAreaProperties.SetAttributeValue(nameof(plot.IsLegendVisible), plot.IsLegendVisible);
            legendAreaProperties.SetAttributeValue(nameof(plot.LegendBackground), plot.LegendBackground.ToString());
            legendAreaProperties.SetAttributeValue(nameof(plot.LegendBorder), plot.LegendBorder.ToString());
            legendAreaProperties.SetAttributeValue(nameof(plot.LegendBorderThickness), plot.LegendBorderThickness.ToString("G17", CultureInfo.InvariantCulture));
            legendAreaProperties.SetAttributeValue(nameof(plot.LegendPadding), plot.LegendPadding.ToString("G17", CultureInfo.InvariantCulture));
            legendProperties.Add(legendAreaProperties);

            // Legend Position Properties
            var subTitleProperties = new XElement("Position");
            subTitleProperties.SetAttributeValue(nameof(plot.LegendPlacement), plot.LegendPlacement.ToString());
            subTitleProperties.SetAttributeValue(nameof(plot.LegendPosition), plot.LegendPosition.ToString());
            subTitleProperties.SetAttributeValue(nameof(plot.LegendOrientation), plot.LegendOrientation.ToString());
            legendProperties.Add(subTitleProperties);

            // Title Properties
            var titleProperties = new XElement("Title");
            titleProperties.SetAttributeValue(nameof(plot.LegendTitle), plot.LegendTitle);
            titleProperties.SetAttributeValue(nameof(plot.LegendTitleColor), plot.LegendTitleColor.ToString());
            titleProperties.SetAttributeValue(nameof(plot.LegendTitleFont), plot.LegendTitleFont);
            titleProperties.SetAttributeValue(nameof(plot.LegendTitleFontSize), plot.LegendTitleFontSize.ToString("G17", CultureInfo.InvariantCulture));
            titleProperties.SetAttributeValue(nameof(plot.LegendTitleFontWeight), fwc.ConvertToInvariantString(plot.LegendTitleFontWeight));
            legendProperties.Add(titleProperties);

            // Legend Item Properties
            var itemProperties = new XElement("Items");
            itemProperties.SetAttributeValue(nameof(plot.LegendTextColor), plot.LegendTextColor.ToString());
            itemProperties.SetAttributeValue(nameof(plot.LegendSymbolLength), plot.LegendSymbolLength.ToString("G17", CultureInfo.InvariantCulture));
            itemProperties.SetAttributeValue(nameof(plot.LegendSymbolMargin), plot.LegendSymbolMargin.ToString("G17", CultureInfo.InvariantCulture));
            itemProperties.SetAttributeValue(nameof(plot.LegendSymbolPlacement), plot.LegendSymbolPlacement.ToString());
            itemProperties.SetAttributeValue(nameof(plot.LegendColumnSpacing), plot.LegendColumnSpacing.ToString("G17", CultureInfo.InvariantCulture));
            itemProperties.SetAttributeValue(nameof(plot.LegendItemAlignment), plot.LegendItemAlignment.ToString());
            itemProperties.SetAttributeValue(nameof(plot.LegendItemOrder), plot.LegendItemOrder.ToString());
            itemProperties.SetAttributeValue(nameof(plot.LegendItemSpacing), plot.LegendItemSpacing.ToString("G17", CultureInfo.InvariantCulture));
            itemProperties.SetAttributeValue(nameof(plot.LegendLineSpacing), plot.LegendLineSpacing.ToString("G17", CultureInfo.InvariantCulture));
            legendProperties.Add(itemProperties);

            return legendProperties;
        }

        /// <summary>
        /// Deserializes legend properties from an XML element and applies them to the plot.
        /// </summary>
        /// <param name="plot">The OxyPlot Plot control to apply settings to.</param>
        /// <param name="element">The XElement containing serialized legend properties.</param>
        public static void XElementToLegendProperties(Wpf.Plot plot, XElement element)
        {
            // Early Exit
            if (plot == null) return;
            if (element.Name != LegendPropertiesTag) return;

            // Set up converters
            var fontWeightConverter = new FontWeightConverter();

            // Area Properties
            var areaElement = element.Element("Area");
            if (areaElement != null)
            {
                bool isLegendVisible;
                if (OxyPlotSettingsSerializer.GetBooleanAttribute(areaElement, nameof(plot.IsLegendVisible), out isLegendVisible)) plot.IsLegendVisible = isLegendVisible;

                Color legendBackground;
                if (OxyPlotSettingsSerializer.GetColorAttribute(areaElement, nameof(plot.LegendBackground), out legendBackground)) plot.LegendBackground = legendBackground;

                Color legendBorder;
                if (OxyPlotSettingsSerializer.GetColorAttribute(areaElement, nameof(plot.LegendBorder), out legendBorder)) plot.LegendBorder = legendBorder;

                double legendBorderThickness;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(areaElement, nameof(plot.LegendBorderThickness), out legendBorderThickness)) plot.LegendBorderThickness = legendBorderThickness;

                double legendPadding;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(areaElement, nameof(plot.LegendPadding), out legendPadding)) plot.LegendPadding = legendPadding;

                // Backward compatibility
                if (OxyPlotSettingsSerializer.GetBooleanAttribute(areaElement, "LegendVisible", out isLegendVisible)) plot.IsLegendVisible = isLegendVisible;
                if (OxyPlotSettingsSerializer.GetColorAttribute(areaElement, "BackgroundColor", out legendBackground)) plot.LegendBackground = legendBackground;
                if (OxyPlotSettingsSerializer.GetColorAttribute(areaElement, "BorderColor", out legendBorder)) plot.LegendBorder = legendBorder;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(areaElement, "BorderThickness", out legendBorderThickness)) plot.LegendBorderThickness = legendBorderThickness;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(areaElement, "Padding", out legendPadding)) plot.LegendPadding = legendPadding;
            }

            // Position Properties
            var positionElement = element.Element("Position");
            if (positionElement != null)
            {
                LegendPlacement legendPlacement;
                if (OxyPlotSettingsSerializer.GetEnumAttribute(positionElement, nameof(plot.LegendPlacement), out legendPlacement)) plot.LegendPlacement = legendPlacement;

                LegendPosition legendPosition;
                if (OxyPlotSettingsSerializer.GetEnumAttribute(positionElement, nameof(plot.LegendPosition), out legendPosition)) plot.LegendPosition = legendPosition;

                LegendOrientation legendOrientation;
                if (OxyPlotSettingsSerializer.GetEnumAttribute(positionElement, nameof(plot.LegendOrientation), out legendOrientation)) plot.LegendOrientation = legendOrientation;

                // Backward compatibility
                if (OxyPlotSettingsSerializer.GetEnumAttribute(positionElement, "Placement", out legendPlacement)) plot.LegendPlacement = legendPlacement;
                if (OxyPlotSettingsSerializer.GetEnumAttribute(positionElement, "Position", out legendPosition)) plot.LegendPosition = legendPosition;
                if (OxyPlotSettingsSerializer.GetEnumAttribute(positionElement, "Orientation", out legendOrientation)) plot.LegendOrientation = legendOrientation;
            }

            // Title Properties
            var titleElement = element.Element("Title");
            if (titleElement != null)
            {
                string? legendTitle;
                if (OxyPlotSettingsSerializer.GetStringAttribute(titleElement, nameof(plot.LegendTitle), out legendTitle)) plot.LegendTitle = legendTitle;

                Color legendTitleColor;
                if (OxyPlotSettingsSerializer.GetColorAttribute(titleElement, nameof(plot.LegendTitleColor), out legendTitleColor)) plot.LegendTitleColor = legendTitleColor;

                string? legendTitleFont;
                if (OxyPlotSettingsSerializer.GetStringAttribute(titleElement, nameof(plot.LegendTitleFont), out legendTitleFont)) plot.LegendTitleFont = legendTitleFont;

                double legendTitleFontSize;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(titleElement, nameof(plot.LegendTitleFontSize), out legendTitleFontSize)) plot.LegendTitleFontSize = legendTitleFontSize;

                FontWeight legendTitleFontWeight;
                if (OxyPlotSettingsSerializer.GetFontWeightAttribute(titleElement, nameof(plot.LegendTitleFontWeight), fontWeightConverter, out legendTitleFontWeight)) plot.LegendTitleFontWeight = legendTitleFontWeight;

                // Backward compatibility
                if (OxyPlotSettingsSerializer.GetStringAttribute(titleElement, "Title", out legendTitle)) plot.LegendTitle = legendTitle;
                if (OxyPlotSettingsSerializer.GetColorAttribute(titleElement, "Color", out legendTitleColor)) plot.LegendTitleColor = legendTitleColor;
                if (OxyPlotSettingsSerializer.GetStringAttribute(titleElement, "Font", out legendTitleFont)) plot.LegendTitleFont = legendTitleFont;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(titleElement, "Size", out legendTitleFontSize)) plot.LegendTitleFontSize = legendTitleFontSize;
                if (OxyPlotSettingsSerializer.GetFontWeightAttribute(titleElement, "Weight", fontWeightConverter, out legendTitleFontWeight)) plot.LegendTitleFontWeight = legendTitleFontWeight;
            }

            // Legend Item Properties
            var itemsElement = element.Element("Items");
            if (itemsElement != null)
            {
                Color legendTextColor;
                if (OxyPlotSettingsSerializer.GetColorAttribute(itemsElement, nameof(plot.LegendTextColor), out legendTextColor)) plot.LegendTextColor = legendTextColor;

                double legendSymbolLength;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(itemsElement, nameof(plot.LegendSymbolLength), out legendSymbolLength)) plot.LegendSymbolLength = legendSymbolLength;

                double legendSymbolMargin;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(itemsElement, nameof(plot.LegendSymbolMargin), out legendSymbolMargin)) plot.LegendSymbolMargin = legendSymbolMargin;

                LegendSymbolPlacement legendSymbolPlacement;
                if (OxyPlotSettingsSerializer.GetEnumAttribute(itemsElement, nameof(plot.LegendSymbolPlacement), out legendSymbolPlacement)) plot.LegendSymbolPlacement = legendSymbolPlacement;

                double legendColumnSpacing;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(itemsElement, nameof(plot.LegendColumnSpacing), out legendColumnSpacing)) plot.LegendColumnSpacing = legendColumnSpacing;

                System.Windows.HorizontalAlignment legendItemAlignment;
                if (OxyPlotSettingsSerializer.GetEnumAttribute(itemsElement, nameof(plot.LegendItemAlignment), out legendItemAlignment)) plot.LegendItemAlignment = legendItemAlignment;

                LegendItemOrder legendItemOrder;
                if (OxyPlotSettingsSerializer.GetEnumAttribute(itemsElement, nameof(plot.LegendItemOrder), out legendItemOrder)) plot.LegendItemOrder = legendItemOrder;

                double legendItemSpacing;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(itemsElement, nameof(plot.LegendItemSpacing), out legendItemSpacing)) plot.LegendItemSpacing = legendItemSpacing;

                double legendLineSpacing;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(itemsElement, nameof(plot.LegendLineSpacing), out legendLineSpacing)) plot.LegendLineSpacing = legendLineSpacing;

                // Backward compatibility
                if (OxyPlotSettingsSerializer.GetColorAttribute(itemsElement, "Color", out legendTextColor)) plot.LegendTextColor = legendTextColor;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(itemsElement, "SymbolLength", out legendSymbolLength)) plot.LegendSymbolLength = legendSymbolLength;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(itemsElement, "SymbolMargin", out legendSymbolMargin)) plot.LegendSymbolMargin = legendSymbolMargin;
                if (OxyPlotSettingsSerializer.GetEnumAttribute(itemsElement, "SymbolPlacement", out legendSymbolPlacement)) plot.LegendSymbolPlacement = legendSymbolPlacement;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(itemsElement, "ColumnSpacing", out legendColumnSpacing)) plot.LegendColumnSpacing = legendColumnSpacing;
                if (OxyPlotSettingsSerializer.GetEnumAttribute(itemsElement, "ItemAlignment", out legendItemAlignment)) plot.LegendItemAlignment = legendItemAlignment;
                if (OxyPlotSettingsSerializer.GetEnumAttribute(itemsElement, "ItemOrder", out legendItemOrder)) plot.LegendItemOrder = legendItemOrder;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(itemsElement, "ItemSpacing", out legendItemSpacing)) plot.LegendItemSpacing = legendItemSpacing;
                if (OxyPlotSettingsSerializer.GetDoubleAttribute(itemsElement, "LineSpacing", out legendLineSpacing)) plot.LegendLineSpacing = legendLineSpacing;
            }
        }
    }
}
