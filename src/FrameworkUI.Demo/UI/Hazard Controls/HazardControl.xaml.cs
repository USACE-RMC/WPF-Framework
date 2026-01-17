using DatabaseManager;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FrameworkInterfaces;
using OxyPlot;
using OxyPlot.Wpf;
using OxyPlotControls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Themes;


namespace FrameworkUI.Demo.UI
{
    /// <summary>
    /// A WPF user control for displaying and interacting with parametric hazard function data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This control provides visualization of parametric hazard functions including:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>Interactive OxyPlot chart with confidence intervals, mean, and mode curves</description></item>
    ///     <item><description>Frequency curve data table</description></item>
    ///     <item><description>Summary statistics display</description></item>
    ///     <item><description>Parameter sets data grid for uncertainty analysis results</description></item>
    /// </list>
    /// </remarks>
    public partial class HazardControl : UserControl
    {

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="HazardControl"/> class.
        /// </summary>
        public HazardControl()
        {
            InitializeComponent();
            // Update plot series tracker format string
            ConfidenceInterval.TrackerFormatString = "{0}" + Environment.NewLine + "{1}: {2:0.####E+0}" + Environment.NewLine + "{3}: {4:" + UserSettings.ValueStringFormat + "}";
            MeanLine.TrackerFormatString = "{0}" + Environment.NewLine + "{1}: {2:0.####E+0}" + Environment.NewLine + "{3}: {4:" + UserSettings.ValueStringFormat + "}";
            ModeLine.TrackerFormatString = "{0}" + Environment.NewLine + "{1}: {2:0.####E+0}" + Environment.NewLine + "{3}: {4:" + UserSettings.ValueStringFormat + "}";
        }

        #endregion

        #region Members

        private bool _isLoaded = false;
        private List<ParametricDistributionCurveRow> _frequencyCurveList = new List<ParametricDistributionCurveRow>();
        private List<SummaryStatistic> _summaryStatisticsList = new List<SummaryStatistic>();

        /// <summary>
        /// Gets the collection of data points for the mean line series.
        /// </summary>
        /// <value>
        /// An observable collection of <see cref="DataPoint"/> objects representing the mean curve.
        /// </value>
        public ObservableCollection<OxyPlot.DataPoint> MeanLinePoints { get; } = new ObservableCollection<OxyPlot.DataPoint>();

        /// <summary>
        /// Gets the collection of data points for the mode line series.
        /// </summary>
        /// <value>
        /// An observable collection of <see cref="DataPoint"/> objects representing the mode (user-specified) curve.
        /// </value>
        public ObservableCollection<OxyPlot.DataPoint> ModeLinePoints { get; } = new ObservableCollection<OxyPlot.DataPoint>();

        /// <summary>
        /// Gets the collection of area points for the confidence interval series.
        /// </summary>
        /// <value>
        /// An observable collection of <see cref="AreaPoint"/> objects representing the upper and lower confidence bounds.
        /// </value>
        public ObservableCollection<AreaPoint> ConfidencePoints { get; } = new ObservableCollection<AreaPoint>();

        /// <summary>
        /// Gets a value indicating whether the plot area was clicked.
        /// </summary>
        /// <value>
        /// <c>true</c> if the plot was clicked; otherwise, <c>false</c>.
        /// </value>
        public bool PlotClicked { get; private set; } = false;

        /// <summary>
        /// Occurs before a mouse click is processed on the control.
        /// </summary>
        /// <param name="plotClicked">Indicates whether the plot area was clicked.</param>
        /// <param name="toolbarClicked">Indicates whether the toolbar area was clicked.</param>
        /// <param name="plot">The plot control that was interacted with.</param>
        public event PreviewControlClickedEventHandler PreviewControlClicked;

        /// <summary>
        /// Represents the method that will handle the <see cref="PreviewControlClicked"/> event.
        /// </summary>
        /// <param name="plotClicked">Indicates whether the plot area was clicked.</param>
        /// <param name="toolbarClicked">Indicates whether the toolbar area was clicked.</param>
        /// <param name="plot">The plot control that was interacted with.</param>
        public delegate void PreviewControlClickedEventHandler(bool plotClicked, bool toolbarClicked, Plot plot);

        /// <summary>
        /// Identifies the <see cref="Element"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            nameof(Element),
            typeof(HazardElement),
            typeof(HazardControl),
            new PropertyMetadata(null, ElementPropertyChanged));

        /// <summary>
        /// Handles changes to the <see cref="Element"/> dependency property.
        /// </summary>
        /// <param name="d">The dependency object on which the property changed.</param>
        /// <param name="e">Event arguments containing the old and new values.</param>
        private static void ElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(HazardControl)) return;

            var thisControl = (HazardControl)d;

            if (e.OldValue != null)
            {
                var oldElement = e.OldValue as HazardElement;
                if (oldElement != null)
                {
                    oldElement.ParentCollection.PreviewObjectSaved -= thisControl.PreviewSaved;
                    oldElement.PropertyChanged -= thisControl.HazardFunctionPropertyChanged;
                }
            }

            if (e.NewValue == null) return;

            var newElement = e.NewValue as HazardElement;
            if (newElement == null) return;

            newElement.ParentCollection.PreviewObjectSaved += thisControl.PreviewSaved;
            newElement.PropertyChanged += thisControl.HazardFunctionPropertyChanged;

            thisControl.LoadPlotSettings();
            thisControl.UpdatePlot();
        }

        /// <summary>
        /// Gets or sets the parametric hazard element bound to this control.
        /// </summary>
        /// <value>
        /// The <see cref="ParametricHazard"/> instance containing the hazard function data and settings.
        /// </value>
        public HazardElement Element
        {
            get => (HazardElement)GetValue(ElementProperty);
            set => SetValue(ElementProperty, value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Loaded event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void HazardFunctionControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isLoaded)
            {
                LoadPlotSettings();
                UpdatePlot();
                SetColumnStringFormats();
                BindFrequencyCurveTable();
                SetFrequencyCurveTableColumnHeaders();
                BindSummaryStatisticsDataGrid();
                BindParameterSetDataGrid();

                if (Element != null && Element.IsUncertain)
                {
                    UpperColumn.Visibility = Visibility.Visible;
                    LowerColumn.Visibility = Visibility.Visible;
                    PredictiveColumn.Visibility = Visibility.Visible;
                    ParameterSetsTab.Visibility = Visibility.Visible;
                }
                else
                {
                    ParameterSetsTab.Visibility = Visibility.Collapsed;
                    UpperColumn.Visibility = Visibility.Collapsed;
                    LowerColumn.Visibility = Visibility.Collapsed;
                    PredictiveColumn.Visibility = Visibility.Collapsed;
                }

                // Subscribe to theme changes to refresh plot when theme changes
                ThemeService.Instance.ThemeChanged += OnThemeChanged;
            }

            _isLoaded = true;
        }

        /// <summary>
        /// Handles the Unloaded event of the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void HazardFunctionControl_Unloaded(object sender, RoutedEventArgs e)
        {
            // Unsubscribe from theme changes to prevent memory leaks
            ThemeService.Instance.ThemeChanged -= OnThemeChanged;
        }

        /// <summary>
        /// Handles theme change events to refresh the plot.
        /// </summary>
        /// <remarks>
        /// OxyPlot controls need to be explicitly invalidated when WPF themes change
        /// because they use a custom rendering pipeline that doesn't automatically
        /// respond to resource dictionary changes.
        /// </remarks>
        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            // Dispatch the plot invalidation to ensure the visual tree has updated
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (Plot != null)
                {
                    Plot.InvalidatePlot(true);
                }
            }));
        }

        /// <summary>
        /// Handles the preview save event to persist chart settings before saving.
        /// </summary>
        /// <param name="sender">The object being saved.</param>
        /// <param name="cancel">A reference parameter that can be set to <c>true</c> to cancel the save operation.</param>
        private void PreviewSaved(ISave sender, ref bool cancel)
        {
            if (_isLoaded)
            {

            }
        }

        /// <summary>
        /// Handles property changed events from the bound hazard function element.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the property name.</param>
        private void HazardFunctionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Element.IsEstimated))
            {
                UpdatePlot();
                BindFrequencyCurveTable();
                BindSummaryStatisticsDataGrid();
                BindParameterSetDataGrid();
            }

            if (e.PropertyName == nameof(Element.ParentDistribution))
            {
                UpdatePlot();
                BindFrequencyCurveTable();
                SetFrequencyCurveTableColumnHeaders();
                BindSummaryStatisticsDataGrid();
            }

            if (e.PropertyName == nameof(Element.ConfidenceIntervalWidth))
            {
                UpdatePlot();
                BindFrequencyCurveTable();
                SetFrequencyCurveTableColumnHeaders();
                BindSummaryStatisticsDataGrid();
            }

            if (e.PropertyName == nameof(Element.IsUncertain))
            {
                if (Element != null && Element.IsUncertain)
                {
                    UpperColumn.Visibility = Visibility.Visible;
                    LowerColumn.Visibility = Visibility.Visible;
                    PredictiveColumn.Visibility = Visibility.Visible;
                    ParameterSetsTab.Visibility = Visibility.Visible;
                }
                else
                {
                    ParameterSetsTab.Visibility = Visibility.Collapsed;
                    UpperColumn.Visibility = Visibility.Collapsed;
                    LowerColumn.Visibility = Visibility.Collapsed;
                    PredictiveColumn.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Plot Methods

        private bool _probabilityOnX = true;

        /// <summary>
        /// Loads and applies the saved plot settings from the element's chart settings.
        /// </summary>
        /// <remarks>
        /// This method restores axis configurations, bindings, and series settings from XML.
        /// It also handles backward compatibility for logarithmic axis conversion.
        /// </remarks>
        private void LoadPlotSettings()
        {
            try
            {
                // This is a hack to make the log-scale plot behave when the series data is negative
                // Without this hack, when the axis is later changed to linear, the negative values will not render correctly
                // We need to fix this in OxyPlot later
                OxyPlot.Wpf.Axis oldAxis = null;
                OxyPlot.Wpf.Axis newAxis = null;

                foreach (var axis in Plot.Axes)
                {
                    if (axis.Key == "Hazard")
                    {
                        oldAxis = axis;
                        newAxis = AxisControl.ConvertAxisToLogarithmicAxis(oldAxis);
                        break;
                    }
                }

                Plot.Axes.Remove(oldAxis);
                Plot.Axes.Add(newAxis);
                Plot.InvalidatePlot();

                if (Element == null || Element.PlotSettings == null) return;

               // OxyPlotControls.FromXElement(Plot, System.Xml.Linq.XElement.Parse(Element.ChartSettings));

                foreach (var series in Plot.Series)
                {
                    if (series.Name == ModeLine.Name) series.ItemsSource = ModeLinePoints;
                    if (series.Name == MeanLine.Name) series.ItemsSource = MeanLinePoints;
                    if (series.Name == ConfidenceInterval.Name) series.ItemsSource = ConfidencePoints;

                    if (_probabilityOnX)
                    {
                        series.TrackerFormatString = "{0}" + Environment.NewLine + "{1}: {2:0.####E+0}" + Environment.NewLine + "{3}: {4:" + UserSettings.ValueStringFormat + "}";
                    }
                    else
                    {
                        series.TrackerFormatString = "{0}" + Environment.NewLine + "{1}: {2:" + UserSettings.ValueStringFormat + "}" + Environment.NewLine + "{3}: {4:0.####E+0}";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                // Couldn't parse the chart settings xml. It doesn't necessarily mean an error since it could have never been set.
            }
        }

        /// <summary>
        /// Updates the plot with the current hazard function results.
        /// </summary>
        /// <remarks>
        /// This method clears existing series data and repopulates the plot with:
        /// <list type="bullet">
        ///     <item><description>Confidence interval area (if uncertainty is enabled)</description></item>
        ///     <item><description>Mean curve line (if uncertainty is enabled)</description></item>
        ///     <item><description>Mode (user-specified) curve line</description></item>
        /// </list>
        /// The axis orientation is determined by the <see cref="ProbabilityAxisCheckBox"/> state.
        /// </remarks>
        public void UpdatePlot()
        {
            if (Plot == null) return;

            Plot.Series.Clear();
            ConfidencePoints.Clear();
            MeanLinePoints.Clear();
            ModeLinePoints.Clear();

            if (Element.Results == null) return;

            if (Element.IsEstimated)
            {
                for (int i = 0; i < Element.ProbabilityOrdinates.Count; i++)
                {
                    double aep = Element.ProbabilityOrdinates[i];

                    if (Element.IsUncertain)
                    {
                        ConfidencePoints.Add(new AreaPoint(
                            new OxyPlot.DataPoint(aep, Element.Results.ConfidenceIntervals[i, 0]),
                            new OxyPlot.DataPoint(aep, Element.Results.ConfidenceIntervals[i, 1])));
                        MeanLinePoints.Add(new OxyPlot.DataPoint(aep, Element.Results.MeanCurve[i]));
                    }
                    ModeLinePoints.Add(new OxyPlot.DataPoint(aep, Element.Results.ModeCurve[i]));
                }

                if (Element.IsUncertain)
                {
                    ConfidenceInterval.Title = (Element.ConfidenceIntervalWidth * 100).ToString("F0") + "% Confidence Interval";
                    Plot.Series.Add(ConfidenceInterval);
                    Plot.Series.Add(MeanLine);
                }

                Plot.Series.Add(ModeLine);
            }

            Plot.InvalidatePlot(true);
        }

        /// <summary>
        /// Handles the PreviewMouseDown event to detect clicks on the plot or toolbar.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void HazardFunctionControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var plotHitResult = VisualTreeHelper.HitTest(Plot, e.GetPosition(Plot));
            var toolbarHitResult = VisualTreeHelper.HitTest(PlotToolbar, e.GetPosition(PlotToolbar));

            PreviewControlClicked?.Invoke(plotHitResult != null, toolbarHitResult != null, Plot);
            PlotClicked = plotHitResult != null;
        }

        /// <summary>
        /// Handles the LostFocus event to reset the plot clicked state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void HazardFunctionControl_LostFocus(object sender, RoutedEventArgs e)
        {
            PlotClicked = false;
        }

        #endregion

        #region Frequency Curves

        /// <summary>
        /// Sets the column headers for the confidence interval columns based on the configured interval width.
        /// </summary>
        /// <remarks>
        /// The column headers are formatted as percentiles (e.g., "95.0%-ile" and "5.0%-ile" for a 90% confidence interval).
        /// </remarks>
        private void SetFrequencyCurveTableColumnHeaders()
        {
            double alpha = (1 - Element.ConfidenceIntervalWidth) / 2;
            UpperColumn.Header = ((1 - alpha) * 100).ToString("F1") + "%-ile";
            LowerColumn.Header = (alpha * 100).ToString("F1") + "%-ile";
        }

        /// <summary>
        /// Sets the string format for value columns in the frequency curve table.
        /// </summary>
        private void SetColumnStringFormats()
        {
            // Update the column binding string format
            UpperColumn.Binding.StringFormat = "{0:" + UserSettings.ValueStringFormat + "}";
            LowerColumn.Binding.StringFormat = "{0:" + UserSettings.ValueStringFormat + "}";
            PredictiveColumn.Binding.StringFormat = "{0:" + UserSettings.ValueStringFormat + "}";
            ModeColumn.Binding.StringFormat = "{0:" + UserSettings.ValueStringFormat + "}";
        }

        /// <summary>
        /// Binds the frequency curve data to the frequency curve table.
        /// </summary>
        /// <remarks>
        /// Populates the table with probability ordinates and corresponding hazard values
        /// including upper/lower confidence bounds, mean, and mode values when available.
        /// </remarks>
        public void BindFrequencyCurveTable()
        {
            FrequencyCurveTable.ItemsSource = null;
            _frequencyCurveList.Clear();

            if (Element.Results == null) return;

            if (Element.IsEstimated)
            {
                for (int i = 0; i < Element.ProbabilityOrdinates.Count; i++)
                {
                    double aep = Element.ProbabilityOrdinates[i];

                    if (Element.IsUncertain)
                    {
                        double upper = Element.Results.ConfidenceIntervals[i, 1];
                        double lower = Element.Results.ConfidenceIntervals[i, 0];
                        double mean = Element.Results.MeanCurve[i];
                        double mode = Element.Results.ModeCurve[i];
                        _frequencyCurveList.Add(new ParametricDistributionCurveRow(aep, upper, lower, mean, mode));
                    }
                    else
                    {
                        double mode = Element.Results.ModeCurve[i];
                        _frequencyCurveList.Add(new ParametricDistributionCurveRow(aep, double.NaN, double.NaN, double.NaN, mode));
                    }
                }
            }
            else
            {
                for (int i = 0; i < Element.ProbabilityOrdinates.Count; i++)
                {
                    double aep = Element.ProbabilityOrdinates[i];
                    _frequencyCurveList.Add(new ParametricDistributionCurveRow(aep, double.NaN, double.NaN, double.NaN, double.NaN));
                }
            }

            FrequencyCurveTable.ItemsSource = _frequencyCurveList;
            FrequencyCurveTable.Items.Refresh();
        }

        #endregion

        #region Summary Statistics

        /// <summary>
        /// Represents a summary statistic with a name and value.
        /// </summary>
        public class SummaryStatistic
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SummaryStatistic"/> class.
            /// </summary>
            /// <param name="name">The name of the statistic.</param>
            /// <param name="value">The value of the statistic.</param>
            public SummaryStatistic(string name, double value)
            {
                Name = name;
                Value = value;
            }

            /// <summary>
            /// Gets the name of the statistic.
            /// </summary>
            /// <value>The statistic name (e.g., "Mean", "Std Dev", "Skewness").</value>
            public string Name { get; }

            /// <summary>
            /// Gets or sets the value of the statistic.
            /// </summary>
            /// <value>The numeric value of the statistic.</value>
            public double Value { get; set; }
        }

        /// <summary>
        /// Binds the summary statistics data to the summary statistics data grid.
        /// </summary>
        /// <remarks>
        /// Displays distribution parameters and computed statistics including:
        /// minimum, maximum, mean, standard deviation, skewness, and kurtosis.
        /// Values are displayed as NaN when the element has not been estimated.
        /// </remarks>
        public void BindSummaryStatisticsDataGrid()
        {
            SummaryStatisticsTable.ItemsSource = null;
            _summaryStatisticsList.Clear();

            if (Element.IsEstimated)
            {
                // Get parameters
                string[,] parms = Element.ParentDistribution.ParametersToString;
                double[] parVals = Element.ParentDistribution.GetParameters;

                // Create summary statistics list
                for (int i = 0; i < Element.ParentDistribution.NumberOfParameters; i++)
                {
                    _summaryStatisticsList.Add(new SummaryStatistic(parms[i, 0], parVals[i]));
                }

                _summaryStatisticsList.Add(new SummaryStatistic("Minimum", Math.Round(Element.ParentDistribution.Minimum, UserSettings.DefaultValueDigits)));
                _summaryStatisticsList.Add(new SummaryStatistic("Maximum", Math.Round(Element.ParentDistribution.Maximum, UserSettings.DefaultValueDigits)));
                _summaryStatisticsList.Add(new SummaryStatistic("Mean", Math.Round(Element.ParentDistribution.Mean, UserSettings.DefaultValueDigits)));
                _summaryStatisticsList.Add(new SummaryStatistic("Std Dev", Math.Round(Element.ParentDistribution.StandardDeviation, UserSettings.DefaultValueDigits)));
                _summaryStatisticsList.Add(new SummaryStatistic("Skewness", Element.ParentDistribution.Skewness));
                _summaryStatisticsList.Add(new SummaryStatistic("Kurtosis", Element.ParentDistribution.Kurtosis));
            }
            else
            {
                // Get parameters
                string[,] parms = Element.ParentDistribution.ParametersToString;

                // Create summary statistics list
                for (int i = 0; i < Element.ParentDistribution.NumberOfParameters; i++)
                {
                    _summaryStatisticsList.Add(new SummaryStatistic(parms[i, 0], double.NaN));
                }

                _summaryStatisticsList.Add(new SummaryStatistic("Minimum", double.NaN));
                _summaryStatisticsList.Add(new SummaryStatistic("Maximum", double.NaN));
                _summaryStatisticsList.Add(new SummaryStatistic("Mean", double.NaN));
                _summaryStatisticsList.Add(new SummaryStatistic("Std Dev", double.NaN));
                _summaryStatisticsList.Add(new SummaryStatistic("Skewness", double.NaN));
                _summaryStatisticsList.Add(new SummaryStatistic("Kurtosis", double.NaN));
            }

            // Bind the data grid
            SummaryStatisticsTable.ItemsSource = _summaryStatisticsList;
            SummaryStatisticsTable.Items.Refresh();
        }

        /// <summary>
        /// Handles the LoadingRow event to add separator borders to specific rows.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridRowEventArgs"/> instance containing the row data.</param>
        /// <remarks>
        /// Adds a top border to the "Minimum" row to visually separate distribution parameters
        /// from computed statistics.
        /// </remarks>
        private void SummaryStatisticsTable_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (((SummaryStatistic)e.Row.DataContext).Name == "Minimum")
            {
                e.Row.BorderThickness = new Thickness(0, 2, 0, 0);
                e.Row.BorderBrush = new SolidColorBrush(Colors.Black);
            }
            else
            {
                e.Row.BorderThickness = new Thickness(0, 0, 0, 0);
            }
        }

        #endregion

        #region Parameter Sets

        /// <summary>
        /// Binds the parameter sets from the bootstrap analysis to the parameter sets data grid.
        /// </summary>
        /// <remarks>
        /// Creates a DataTable with columns for each distribution parameter and populates it
        /// with the parameter values from each bootstrap realization. The number of columns
        /// varies based on the number of parameters in the parent distribution (2, 3, or 4).
        /// </remarks>
        private void BindParameterSetDataGrid()
        {
            if (Element.Results == null) return;

            if (!Element.IsUncertain)
            {
                ParameterSetTableViewer.DataView = null;
            }
            else
            {
                string[,] parms = Element.ParentDistribution.ParametersToString;
                var dt = new DataTable("ParameterSetTable");

                dt.Columns.Add(new DataColumn(parms[0, 0], typeof(double))); // Location Header

                if (Element.ParentDistribution.NumberOfParameters == 2)
                {
                    dt.Columns.Add(new DataColumn(parms[1, 0], typeof(double)));

                    if (Element.Results.ParameterSets != null && Element.IsEstimated)
                    {
                        foreach (var pSet in Element.Results.ParameterSets)
                        {
                            dt.Rows.Add(pSet.Values[0], pSet.Values[1]);
                        }
                    }
                }
                else if (Element.ParentDistribution.NumberOfParameters == 3)
                {
                    dt.Columns.Add(new DataColumn(parms[1, 0], typeof(double)));
                    dt.Columns.Add(new DataColumn(parms[2, 0], typeof(double)));

                    if (Element.Results.ParameterSets != null && Element.IsEstimated)
                    {
                        foreach (var pSet in Element.Results.ParameterSets)
                        {
                            dt.Rows.Add(pSet.Values[0], pSet.Values[1], pSet.Values[2]);
                        }
                    }
                }
                else if (Element.ParentDistribution.NumberOfParameters == 4)
                {
                    dt.Columns.Add(new DataColumn(parms[1, 0], typeof(double)));
                    dt.Columns.Add(new DataColumn(parms[2, 0], typeof(double)));
                    dt.Columns.Add(new DataColumn(parms[3, 0], typeof(double)));

                    if (Element.Results.ParameterSets != null && Element.IsEstimated)
                    {
                        foreach (var pSet in Element.Results.ParameterSets)
                        {
                            dt.Rows.Add(pSet.Values[0], pSet.Values[1], pSet.Values[2], pSet.Values[3]);
                        }
                    }
                }

                ParameterSetTableViewer.DataView = new InMemoryReader(dt).GetTableManager(dt.TableName);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the tab control to refresh the parameter set viewer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((TabControl)sender).SelectedIndex == 2)
            {
                ParameterSetTableViewer.RefreshView();
            }
        }

        #endregion

    }
}
