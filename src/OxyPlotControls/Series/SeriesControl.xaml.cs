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
using System.Windows;
using System.Windows.Controls;
using OxyPlot.Wpf;

namespace OxyPlotControls
{
    /// <summary>
    /// A control that dynamically displays the appropriate series property control
    /// based on the type of OxyPlot series selected.
    /// </summary>
    public partial class SeriesControl : UserControl
    {
        #region Events

        /// <summary>
        /// Occurs when a series property value changes through user interaction with any child control.
        /// </summary>
        /// <remarks>
        /// This event is raised when any child series control (LineSeriesControl, BarSeriesControl, etc.)
        /// raises its PlotChanged event. It bubbles up the event to allow parent controls to refresh the plot.
        /// </remarks>
        public event EventHandler? PlotChanged;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="Series"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
            nameof(Series),
            typeof(OxyPlot.Wpf.Series),
            typeof(SeriesControl),
            new PropertyMetadata(null, InitializeControl));

        /// <summary>
        /// Gets or sets the series whose properties should be displayed.
        /// </summary>
        public OxyPlot.Wpf.Series Series
        {
            get => (OxyPlot.Wpf.Series)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ExpanderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpanderStyleProperty = DependencyProperty.Register(
            nameof(ExpanderStyle),
            typeof(Style),
            typeof(SeriesControl),
            new PropertyMetadata(null, ExpandStyleChanged));

        /// <summary>
        /// Gets or sets the style to apply to expanders in the series property controls.
        /// </summary>
        public Style ExpanderStyle
        {
            get => (Style)GetValue(ExpanderStyleProperty);
            set => SetValue(ExpanderStyleProperty, value);
        }

        #endregion

        #region Private Fields

        // Lazy loading to improve initialization times
        private GenericSeriesControl? _genericControl;
        private ScatterSeriesControl? _scatterControl;
        private LineSeriesControl? _lineControl;
        private BoxPlotSeriesControl? _boxPlotControl;
        private BarSeriesControl? _barControl;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesControl"/> class.
        /// </summary>
        public SeriesControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Raises the <see cref="PlotChanged"/> event.
        /// </summary>
        /// <remarks>
        /// This method is called when any child series control raises its PlotChanged event.
        /// </remarks>
        protected virtual void OnPlotChanged()
        {
            PlotChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the PlotChanged event from child series controls.
        /// </summary>
        /// <param name="sender">The child control that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnChildPlotChanged(object? sender, EventArgs e)
        {
            OnPlotChanged();
        }

        /// <summary>
        /// Handles changes to the Series property by displaying the appropriate property control.
        /// </summary>
        private static void InitializeControl(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(SeriesControl)) return;
            var thisControl = (SeriesControl)d;

            // Clear the properties controls
            thisControl.SeriesGrid.Children.Clear();
            if (thisControl._genericControl != null)
            {
                thisControl._genericControl.Series = null!;
                thisControl._genericControl.ExpanderStyle = thisControl.ExpanderStyle;
            }
            if (thisControl._scatterControl != null)
            {
                thisControl._scatterControl.Series = null!;
                thisControl._scatterControl.ExpanderStyle = thisControl.ExpanderStyle;
            }
            if (thisControl._lineControl != null)
            {
                thisControl._lineControl.Series = null!;
                thisControl._lineControl.ExpanderStyle = thisControl.ExpanderStyle;
            }
            if (thisControl._boxPlotControl != null)
            {
                thisControl._boxPlotControl.Series = null!;
                thisControl._boxPlotControl.ExpanderStyle = thisControl.ExpanderStyle;
            }
            if (thisControl._barControl != null)
            {
                thisControl._barControl.Series = null!;
                thisControl._barControl.ExpanderStyle = thisControl.ExpanderStyle;
            }

            // Get the new series
            if (e.NewValue == null) return;
            var wpfSeries = e.NewValue as OxyPlot.Wpf.Series;
            if (wpfSeries == null) return;

            // Bar/Column
            var barSeries = wpfSeries as BarSeriesBase;
            if (barSeries != null)
            {
                if (thisControl._barControl == null)
                {
                    thisControl._barControl = new BarSeriesControl { ExpanderStyle = thisControl.ExpanderStyle };
                    thisControl._barControl.PlotChanged += thisControl.OnChildPlotChanged;
                }
                thisControl._barControl.Series = barSeries;
                thisControl.SeriesGrid.Children.Add(thisControl._barControl);
                return;
            }

            // Line/Area/StairStep/ThreeColorLine/TwoColorLine
            var lineSeries = wpfSeries as LineSeries;
            if (lineSeries != null)
            {
                if (thisControl._lineControl == null)
                {
                    thisControl._lineControl = new LineSeriesControl { ExpanderStyle = thisControl.ExpanderStyle };
                    thisControl._lineControl.PlotChanged += thisControl.OnChildPlotChanged;
                }
                thisControl._lineControl.Series = lineSeries;
                thisControl.SeriesGrid.Children.Add(thisControl._lineControl);
                return;
            }

            // Scatter/ScatterError
            var scatterSeries = wpfSeries as ScatterSeries<OxyPlot.Series.ScatterPoint>;
            if (scatterSeries != null)
            {
                if (thisControl._scatterControl == null)
                {
                    thisControl._scatterControl = new ScatterSeriesControl { ExpanderStyle = thisControl.ExpanderStyle };
                    thisControl._scatterControl.PlotChanged += thisControl.OnChildPlotChanged;
                }
                thisControl._scatterControl.Series = scatterSeries;
                thisControl.SeriesGrid.Children.Add(thisControl._scatterControl);
                return;
            }

            // BoxPlot
            var boxPlotSeries = wpfSeries as BoxPlotSeries;
            if (boxPlotSeries != null)
            {
                if (thisControl._boxPlotControl == null)
                {
                    thisControl._boxPlotControl = new BoxPlotSeriesControl { ExpanderStyle = thisControl.ExpanderStyle };
                    thisControl._boxPlotControl.PlotChanged += thisControl.OnChildPlotChanged;
                }
                thisControl._boxPlotControl.Series = boxPlotSeries;
                thisControl.SeriesGrid.Children.Add(thisControl._boxPlotControl);
                return;
            }

            // Generic fallback
            if (thisControl._genericControl == null)
            {
                thisControl._genericControl = new GenericSeriesControl { ExpanderStyle = thisControl.ExpanderStyle };
                thisControl._genericControl.PlotChanged += thisControl.OnChildPlotChanged;
            }
            thisControl._genericControl.Series = wpfSeries;
            thisControl.SeriesGrid.Children.Add(thisControl._genericControl);
        }

        /// <summary>
        /// Handles changes to the ExpanderStyle property.
        /// </summary>
        private static void ExpandStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(SeriesControl)) return;
            var thisControl = (SeriesControl)d;

            if (thisControl._genericControl != null)
                thisControl._genericControl.ExpanderStyle = thisControl.ExpanderStyle;
            if (thisControl._scatterControl != null)
                thisControl._scatterControl.ExpanderStyle = thisControl.ExpanderStyle;
            if (thisControl._lineControl != null)
                thisControl._lineControl.ExpanderStyle = thisControl.ExpanderStyle;
            if (thisControl._boxPlotControl != null)
                thisControl._boxPlotControl.ExpanderStyle = thisControl.ExpanderStyle;
            if (thisControl._barControl != null)
                thisControl._barControl.ExpanderStyle = thisControl.ExpanderStyle;
        }

        /// <summary>
        /// Closes all expanders in the currently displayed series control.
        /// </summary>
        public void CloseExpanders()
        {
            _genericControl?.CloseExpanders();
            _scatterControl?.CloseExpanders();
            _lineControl?.CloseExpanders();
            _boxPlotControl?.CloseExpanders();
            _barControl?.CloseExpanders();
        }

        /// <summary>
        /// Expands the specified expansion zone in the currently displayed series control.
        /// </summary>
        /// <param name="expansionZone">The property expansion zone to expand.</param>
        public void Expand(OxyPlotPropertiesControl.PropertyEXP expansionZone)
        {
            if (_genericControl != null && _genericControl.Series != null)
                _genericControl.Expand(expansionZone);
            if (_scatterControl != null && _scatterControl.Series != null)
                _scatterControl.Expand(expansionZone);
            if (_lineControl != null && _lineControl.Series != null)
                _lineControl.Expand(expansionZone);
            if (_boxPlotControl != null && _boxPlotControl.Series != null)
                _boxPlotControl.Expand(expansionZone);
            if (_barControl != null && _barControl.Series != null)
                _barControl.Expand(expansionZone);
        }
    }
}
