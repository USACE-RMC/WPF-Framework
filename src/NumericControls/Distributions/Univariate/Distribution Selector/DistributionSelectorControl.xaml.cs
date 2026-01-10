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
using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Sampling;
using OxyPlot.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NumericControls.Distributions.Univariate
{
    /// <summary>
    /// A comprehensive WPF user control for selecting, configuring, and visualizing univariate probability distributions.
    /// Provides interactive parameter editing, PDF visualization, and statistical comparison with sample data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="DistributionSelectorControl"/> provides a complete interface for working with
    /// probability distributions, including:
    /// </para>
    /// <list type="bullet">
    /// <item><description>A dropdown to select from available distribution types (Normal, LogNormal, Triangular, etc.)</description></item>
    /// <item><description>A parameter grid for editing distribution parameters with real-time validation</description></item>
    /// <item><description>An interactive PDF (Probability Density Function) plot</description></item>
    /// <item><description>A summary statistics table comparing distribution and sample data</description></item>
    /// <item><description>Automatic parameter estimation from sample data (when supported)</description></item>
    /// <item><description>Goodness-of-fit statistics (RMSE, Chi-Squared, Kolmogorov-Smirnov)</description></item>
    /// </list>
    /// <para>
    /// <b>Internationalization:</b> All numeric formatting and parsing uses <see cref="GenericControls.NumberFormatHelper"/>
    /// for culture-aware number handling, supporting international decimal and thousands separators.
    /// </para>
    /// <para>
    /// <b>Theming:</b> The control supports runtime theme switching via the Themes library.
    /// Use <see cref="BackgroundColor"/> to customize the control's background, or let it inherit
    /// from the application's current theme.
    /// </para>
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
    /// <example>
    /// <code>
    /// &lt;!-- XAML usage with data binding --&gt;
    /// &lt;uni:DistributionSelectorControl
    ///     SelectedDistribution="{Binding MyDistribution, Mode=TwoWay}"
    ///     SampleData="{Binding MySampleData}"
    ///     ShowPlot="True"
    ///     ShowStatistics="True"/&gt;
    /// </code>
    /// </example>
    /// <seealso cref="DistributionWithSelectorControl"/>
    /// <seealso cref="DistributionSelectorPopup"/>
    /// <seealso cref="Parameter"/>
    /// <seealso cref="SummaryStatistic"/>
    public partial class DistributionSelectorControl : UserControl
    {
        /// <summary>
        /// The histogram series used to display sample data distribution.
        /// </summary>
        private HistogramSeries _histogramSeries = new HistogramSeries()
        {
            Name = "Histogram",
            Title = "Input Data Histogram",
            FillColor = Color.FromArgb(225, 184, 243, 205),
            StrokeColor = Color.FromArgb(255, 53, 59, 122),
            StrokeThickness = 1,
            RenderInLegend = false
        };

        /// <summary>
        /// Construct a distribution selector control using the default, full list of continuous distribution types.
        /// </summary>
        public DistributionSelectorControl()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            InitializeControl();
        }

        /// <summary>
        /// Initial the control elements.
        /// </summary>
        private void InitializeControl()
        {

            // Create summary statistics list (12 base stats + 3 goodness of fit stats = 15 total)
            SummaryStatisticsList.Add(new SummaryStatistic("Minimum", "", ""));
            SummaryStatisticsList.Add(new SummaryStatistic("Maximum", "", ""));
            SummaryStatisticsList.Add(new SummaryStatistic("Mean", "", ""));
            SummaryStatisticsList.Add(new SummaryStatistic("Mode", "", ""));
            SummaryStatisticsList.Add(new SummaryStatistic("Std Dev", "", ""));
            SummaryStatisticsList.Add(new SummaryStatistic("Skewness", "", ""));
            SummaryStatisticsList.Add(new SummaryStatistic("Kurtosis", "", ""));
            SummaryStatisticsList.Add(new SummaryStatistic("5%", "", ""));
            SummaryStatisticsList.Add(new SummaryStatistic("25%", "", ""));
            SummaryStatisticsList.Add(new SummaryStatistic("50%", "", ""));
            SummaryStatisticsList.Add(new SummaryStatistic("75%", "", ""));
            SummaryStatisticsList.Add(new SummaryStatistic("95%", "", ""));

            // Bind the summary and parameter data tables
            ParametersTable.ItemsSource = ParameterList;
            SummaryTable.ItemsSource = SummaryStatisticsList;

            // OxyPlot
            Plot.ActualController.UnbindAll();
        }

        private ObservableCollection<Parameter> ParameterList { get; set; } = new ObservableCollection<Parameter>();
        private List<SummaryStatistic> SummaryStatisticsList { get; set; } = new List<SummaryStatistic>();

        /// <summary>
        /// Dependency property for the control distribution options.
        /// </summary>
        public static DependencyProperty DistributionsProperty = DependencyProperty.Register(nameof(Distributions), typeof(IList<UnivariateDistributionBase>), typeof(DistributionSelectorControl), new PropertyMetadata(DefaultDistributions));

        /// <summary>
        /// Gets and sets the distribution options.
        /// </summary>
        public IList<UnivariateDistributionBase> Distributions
        {
            get => (IList<UnivariateDistributionBase>)GetValue(DistributionsProperty);
            set => SetValue(DistributionsProperty, value);
        }

        /// <summary>
        /// Dependency property for the selected distribution.
        /// </summary>
        public static DependencyProperty SelectedDistributionProperty = DependencyProperty.Register(nameof(SelectedDistribution), typeof(UnivariateDistributionBase), typeof(DistributionSelectorControl), new PropertyMetadata(null, SelectedDistributionProperty_Callback));

        /// <summary>
        /// Property changed callback for the SelectedDistribution dependency property.
        /// Updates the UI to reflect the new distribution selection.
        /// </summary>
        /// <param name="d">The dependency object on which the property changed.</param>
        /// <param name="e">Event arguments containing the old and new property values.</param>
        private static void SelectedDistributionProperty_Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) { return; }
            if (d.GetType() != typeof(DistributionSelectorControl)) { return; }
            DistributionSelectorControl thisControl = (DistributionSelectorControl)d;
            if (thisControl.DistributionCombobox == null) { return; }
            // 
            if (e.NewValue == null) { thisControl.DistributionCombobox.SelectedIndex = -1; }
            else if (thisControl._distributionChanging == false)
            {
                UnivariateDistributionBase newDistribution = e.NewValue as UnivariateDistributionBase;
                bool isContained = false;
                for (int i = 0; i < thisControl.Distributions.Count; i++)
                {
                    if (thisControl.Distributions[i].Type == newDistribution.Type)
                    {
                        isContained = true;
                        if (thisControl.Distributions[i].Equals(newDistribution) == false)
                        {
                            thisControl.Distributions[i] = newDistribution;
                            thisControl.DistributionCombobox.Items.Refresh();
                        }
                        //
                        thisControl._propertyChanging = true;
                        thisControl.DistributionCombobox.SelectedItem = thisControl.Distributions[i];
                        thisControl.UpdatePDFPlot();
                        thisControl.UpdateDistributionStats();
                        thisControl._propertyChanging = false;
                        break;
                    }
                }
                //
                if (isContained == false) { thisControl.SelectedDistribution = null; }
            }
        }

        private bool _propertyChanging = false;

        /// <summary>
        /// Get and set the selected probability distribution.
        /// </summary>
        public UnivariateDistributionBase SelectedDistribution
        {
            get => (UnivariateDistributionBase)GetValue(SelectedDistributionProperty);
            set => SetValue(SelectedDistributionProperty, value);
        }

        /// <summary>
        /// Dependency property for the control background color.
        /// </summary>
        public static DependencyProperty BackgroundColorProperty = DependencyProperty.Register(nameof(BackgroundColor), typeof(Brush), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Get and set the distribution selector background color.
        /// </summary>
        public Brush BackgroundColor
        {
            get => (Brush)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        /// <summary>
        /// Dependency property to show the plot axis title.
        /// </summary>
        public static DependencyProperty ShowAxisProperty = DependencyProperty.Register(nameof(ShowAxis), typeof(bool), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get and set the distribution selector plot axis title visibility.
        /// </summary>
        public bool ShowAxis
        {
            get => (bool)GetValue(ShowAxisProperty);
            set => SetValue(ShowAxisProperty, value);
        }

        /// <summary>
        /// Dependency property to show the plot axis title.
        /// </summary>
        public static DependencyProperty ShowAxisTitleProperty = DependencyProperty.Register(nameof(ShowAxisTitle), typeof(bool), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Get and set the distribution selector plot axis title visibility.
        /// </summary>
        public bool ShowAxisTitle
        {
            get => (bool)GetValue(ShowAxisTitleProperty);
            set => SetValue(ShowAxisTitleProperty, value);
        }

        /// <summary>
        /// Dependency property to show the plot label title.
        /// </summary>
        public static DependencyProperty ShowAxisLabelProperty = DependencyProperty.Register(nameof(ShowAxisLabel), typeof(bool), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Get and set the distribution selector plot axis label visibility.
        /// </summary>
        public bool ShowAxisLabel
        {
            get => (bool)GetValue(ShowAxisLabelProperty);
            set => SetValue(ShowAxisLabelProperty, value);
        }

        private bool _showTickLines = true;

        /// <summary>
        /// Gets or sets a value indicating whether tick lines are displayed on the plot axes.
        /// </summary>
        /// <value><c>true</c> to show tick lines; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool ShowTickLines
        {
            get => _showTickLines;
            set
            {
                _showTickLines = value;
                if (value == true)
                {
                    Xaxis.TickStyle = OxyPlot.Axes.TickStyle.Outside;
                    Yaxis.TickStyle = OxyPlot.Axes.TickStyle.Outside;
                }
                else
                {
                    Xaxis.TickStyle = OxyPlot.Axes.TickStyle.None;
                    Yaxis.TickStyle = OxyPlot.Axes.TickStyle.None;
                }
            }
        }

        /// <summary>
        /// Dependency property for showing the distribution pdf plot.
        /// </summary>
        public static DependencyProperty ShowPlotProperty = DependencyProperty.Register(nameof(ShowPlot), typeof(bool), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets and sets the plot visibility.
        /// </summary>
        public bool ShowPlot
        {
            get => (bool)GetValue(ShowPlotProperty);
            set => SetValue(ShowPlotProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ExpandPlot"/> dependency property.
        /// </summary>
        public static DependencyProperty ExpandPlotProperty = DependencyProperty.Register(nameof(ExpandPlot), typeof(bool), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the plot expander is expanded by default.
        /// </summary>
        /// <value><c>true</c> if the plot is expanded; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool ExpandPlot
        {
            get => (bool)GetValue(ExpandPlotProperty);
            set => SetValue(ExpandPlotProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ExpanderStyle"/> dependency property.
        /// </summary>
        public static DependencyProperty ExpanderStyleProperty = DependencyProperty.Register(nameof(ExpanderStyle), typeof(Style), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style applied to the expander controls.
        /// </summary>
        public Style ExpanderStyle
        {
            get { return (Style)GetValue(ExpanderStyleProperty); }
            set { SetValue(ExpanderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShowStatistics"/> dependency property.
        /// </summary>
        public static DependencyProperty ShowStatisticsProperty = DependencyProperty.Register(nameof(ShowStatistics), typeof(bool), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the summary statistics table is visible.
        /// </summary>
        /// <value><c>true</c> to show summary statistics; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool ShowStatistics
        {
            get => (bool)GetValue(ShowStatisticsProperty);
            set => SetValue(ShowStatisticsProperty, value);
        }

        /// <summary>
        /// Dependency property for showing the distribution title.
        /// </summary>
        public static DependencyProperty DistributionTitleProperty = DependencyProperty.Register(nameof(DistributionTitle), typeof(string), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(""));

        /// <summary>
        /// Gets and sets the distribution title.
        /// </summary>
        public string DistributionTitle
        {
            get => (string)GetValue(DistributionTitleProperty);
            set => SetValue(DistributionTitleProperty, value);
        }

        /// <summary>
        /// Dependency property for showing the distribution title.
        /// </summary>
        public static DependencyProperty SampleDataProperty = DependencyProperty.Register(nameof(SampleData), typeof(double[]), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(null, SampleDataProperty_Callback));

        /// <summary>
        /// Property changed callback for the SampleData dependency property.
        /// Updates the histogram and statistics when sample data changes.
        /// </summary>
        /// <param name="d">The dependency object on which the property changed.</param>
        /// <param name="e">Event arguments containing the old and new property values.</param>
        private static void SampleDataProperty_Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) { return; }
            if (d.GetType() != typeof(DistributionSelectorControl)) { return; }
            DistributionSelectorControl thisControl = (DistributionSelectorControl)d;
            thisControl.UpdateHistogram();
            thisControl.UpdateDistributionStats();
        }

        /// <summary>
        /// Gets and sets the distribution title.
        /// </summary>
        public double[] SampleData
        {
            get => (double[])GetValue(SampleDataProperty);
            set => SetValue(SampleDataProperty, value);
        }

        /// <summary>
        /// Currently does not support bivariate, univariate, or kernel density.
        /// </summary>
        /// <returns></returns>
        public static List<UnivariateDistributionBase> DefaultDistributions
        {
            get
            {
                List<UnivariateDistributionBase> distributions = new List<UnivariateDistributionBase>();
                foreach (UnivariateDistributionType distributionType in (UnivariateDistributionType[])Enum.GetValues(typeof(UnivariateDistributionType)))
                {
                    if (distributionType == UnivariateDistributionType.Empirical) { continue; }
                    if (distributionType == UnivariateDistributionType.KernelDensity) { continue; }
                    if (distributionType == UnivariateDistributionType.UserDefined) { continue; }
                    // 
                    UnivariateDistributionBase distributionToAdd = UnivariateDistributionFactory.CreateDistribution(distributionType);
                    if (distributionToAdd != null) { distributions.Add(distributionToAdd); }
                }
                // 
                return distributions;
            }
        }

        /// <summary>
        /// Updates the histogram display with sample data.
        /// Creates bins, normalizes frequencies, and updates statistics columns.
        /// </summary>
        private void UpdateHistogram()
        {
            //Update Histogram and show fit options (filtered by those that implement IEstimate()
            if (SampleData == null || SampleData.Length == 0)
            {
                Plot.Series.Remove(_histogramSeries);
                _histogramSeries.ItemsSource = null;
                EstimateParametersButton.Visibility = Visibility.Collapsed;
                DataStatsColumn.Visibility = Visibility.Collapsed;
            }
            else
            {
                var histogram = new Histogram(SampleData);
                var histItems = new List<OxyPlot.Series.HistogramItem>();
                double sum = 0;
                for (int i = 0; i < histogram.NumberOfBins; i++) { sum += histogram[i].Frequency * histogram.BinWidth; }
                for (int i = 0; i < histogram.NumberOfBins; i++)
                {
                    histItems.Add(new OxyPlot.Series.HistogramItem(histogram[i].LowerBound, histogram[i].UpperBound, histogram[i].Frequency * histogram.BinWidth / sum));
                }

                _histogramSeries.ItemsSource = histItems;

                if (Plot.Series.Contains(_histogramSeries) == false) { Plot.Series.Insert(0, _histogramSeries); }
                //show fit options (filtered by those that implement IEstimate()
                if (DistributionCanBeEstimated() == true)
                {
                    EstimateParametersButton.Visibility = Visibility.Visible;
                }
                else
                {
                    EstimateParametersButton.Visibility = Visibility.Collapsed;
                }
                // Data Stats
                DataStatsColumn.Visibility = Visibility.Visible;
                var summaryPercentiles = Numerics.Data.Statistics.Statistics.SevenNumberSummary(SampleData);
                var stats = Numerics.Data.Statistics.Statistics.ProductMoments(SampleData);
                var mode = histogram.Mode;

                // Use culture-aware formatting via NumberFormatHelper
                SummaryStatisticsList[0].DataStat = NumberFormatHelper.FormatDouble(summaryPercentiles[0], 4, false);
                SummaryStatisticsList[1].DataStat = NumberFormatHelper.FormatDouble(summaryPercentiles[6], 4, false);
                SummaryStatisticsList[2].DataStat = NumberFormatHelper.FormatDouble(stats[0], 4, false);
                SummaryStatisticsList[3].DataStat = NumberFormatHelper.FormatDouble(mode, 4, false);
                SummaryStatisticsList[4].DataStat = NumberFormatHelper.FormatDouble(stats[1], 4, false);
                SummaryStatisticsList[5].DataStat = NumberFormatHelper.FormatDouble(stats[2], 4, false);
                SummaryStatisticsList[6].DataStat = NumberFormatHelper.FormatDouble(stats[3], 4, false);
                SummaryStatisticsList[7].DataStat = NumberFormatHelper.FormatDouble(summaryPercentiles[1], 4, false);
                SummaryStatisticsList[8].DataStat = NumberFormatHelper.FormatDouble(summaryPercentiles[2], 4, false);
                SummaryStatisticsList[9].DataStat = NumberFormatHelper.FormatDouble(summaryPercentiles[3], 4, false);
                SummaryStatisticsList[10].DataStat = NumberFormatHelper.FormatDouble(summaryPercentiles[4], 4, false);
                SummaryStatisticsList[11].DataStat = NumberFormatHelper.FormatDouble(summaryPercentiles[5], 4, false);

                if (DistributionCanBeEstimated() && SummaryStatisticsList.Count != 15)
                {
                    // Add goodness of fit stats
                    SummaryStatisticsList.Add(new SummaryStatistic("RMSE", "", " - "));
                    SummaryStatisticsList.Add(new SummaryStatistic("Chi-Squared", "", " - "));
                    SummaryStatisticsList.Add(new SummaryStatistic("K-S", "", " - "));
                }
                else
                {
                    // Remove goodness of fit stats
                    for (int i = SummaryStatisticsList.Count - 1; i >= 12; i--)
                    {
                        SummaryStatisticsList.RemoveAt(i);
                    }
                }

                SummaryTable.Items.Refresh();

            }

            Plot.InvalidatePlot(true);
            UpdatePDFPlot();
        }

        /// <summary>
        /// Handles mouse down events on the main grid to clear DataGrid focus when clicking outside.
        /// </summary>
        private void Grid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Check if the click is outside the ParametersTable
            if (!IsMouseOverElement(ParametersTable, e))
            {
                // Clear selection and move focus away from the DataGrid
                ParametersTable.UnselectAllCells();
                Keyboard.ClearFocus();
            }
        }

        /// <summary>
        /// Checks if the mouse is over a specific element.
        /// </summary>
        private bool IsMouseOverElement(UIElement element, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (element == null) return false;
            var position = e.GetPosition(element);
            var bounds = new Rect(0, 0, element.RenderSize.Width, element.RenderSize.Height);
            return bounds.Contains(position);
        }

        /// <summary>
        /// The distribution combobox selection has changed. Perform action.
        /// </summary>
        private void DistributionCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // clear the parameter list
            for (int i = ParameterList.Count - 1; i >= 0; i--)
            {
                ParameterList[i].PropertyChanged -= ParameterPropertyChanged;
                ParameterList.RemoveAt(i);
            }

            if (DistributionCombobox.SelectedIndex == -1)
            {
                UpdatePDFPlot();
                UpdateDistributionStats();
                return;
            }
            // 
            UnivariateDistributionBase currentDistribution = (UnivariateDistributionBase)DistributionCombobox.SelectedItem;

            if (currentDistribution.Type == UnivariateDistributionType.Empirical) 
            {
                // Enter in min, max limits for empirical
                // Enter x values and probabilities for empirical
                // - must be comma delimited within brackets {,,}
                // - need to make sure parsing is culture-aware
            }
            else if (currentDistribution.Type == UnivariateDistributionType.KernelDensity) 
            {
                // Assume Gaussian kernel for now
                // Use default bandwidth
                // Enter sample data for kernel density
                // - must be comma delimited within brackets {,,}
                // - need to make sure parsing is culture-aware
            }
            else
            {
                string[,] paramString = currentDistribution.ParametersToString;
                string[] paramNames = currentDistribution.GetParameterPropertyNames;
                // Update parameter grid with culture-aware parsing
                for (int i = 0; i < paramString.GetLength(0); i++)
                {
                    // Use NumberFormatHelper for culture-aware parsing of parameter values
                    if (NumberFormatHelper.TryParseDouble(paramString[i, 1], out double paramValue) == false) { paramValue = double.NaN; }
                    Parameter param = new Parameter(paramNames[i], paramString[i, 0], paramValue);
                    param.PropertyChanged += ParameterPropertyChanged;
                    ParameterList.Add(param);
                }
            }

            // Update selected distribution
            if (_propertyChanging == false) { SetDistributionParameters(false); }

            //show fit options (filtered by those that implement IEstimate()
            if (SampleData != null && SampleData.Length > 0 && DistributionCanBeEstimated() == true)
            {
                EstimateParametersButton.Visibility = Visibility.Visible;
            }
            else
            {
                EstimateParametersButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Determines whether the currently selected distribution supports parameter estimation.
        /// </summary>
        /// <returns><c>true</c> if the distribution implements parameter estimation; otherwise, <c>false</c>.</returns>
        private bool DistributionCanBeEstimated()
        {
            if (SelectedDistribution != null)
            {
                if (SelectedDistribution.Type == UnivariateDistributionType.Deterministic) { return true; }
                if (SelectedDistribution.Type == UnivariateDistributionType.Normal) { return true; }
                if (SelectedDistribution.Type == UnivariateDistributionType.Triangular) { return true; }
                if (SelectedDistribution.Type == UnivariateDistributionType.Pert) { return true; }
                if (SelectedDistribution.Type == UnivariateDistributionType.LnNormal) { return true; }
                if (SelectedDistribution.Type == UnivariateDistributionType.TruncatedNormal) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Handles property changes on parameter objects.
        /// Triggers distribution parameter updates when a parameter value changes.
        /// </summary>
        /// <param name="sender">The parameter that raised the event.</param>
        /// <param name="e">Event arguments containing the property name.</param>
        private void ParameterPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Parameter.Value)) { SetDistributionParameters(true); }
        }

        /// <summary>
        /// Set the probability distribution parameters.
        /// </summary>
        private void SetDistributionParameters(bool setParameters)
        {
            if (DistributionCombobox.SelectedIndex == -1) { return; }
            ClearValidation();

            UnivariateDistributionBase currentDistribution = null;
            try
            {
                // Get the selected distribution and set the parameters from the parameter list
                currentDistribution = (UnivariateDistributionBase)DistributionCombobox.SelectedItem;

                if (currentDistribution.Type == UnivariateDistributionType.Empirical || currentDistribution.Type == UnivariateDistributionType.KernelDensity)
                {
                    throw new NotImplementedException();
                    // Dim univDist = DirectCast(currentDistribution, UnivariateEmpiricalCDF)
                    // univDist.SetParameters(univDist.GetXValues, univDist.GetPValues, ParameterList(0).Value, ParameterList(1).Value)
                }
                else
                {
                    if (setParameters) { currentDistribution.SetParameters(ParameterList.Select(o => o.Value).ToArray()); }
                    if (currentDistribution.ParametersValid == false)
                    {
                        ArgumentOutOfRangeException ex = currentDistribution.ValidateParameters(ParameterList.Select(o => o.Value).ToArray(), false);
                        SetInvalidParameter(ex.ParamName, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(ArgumentOutOfRangeException))
                {
                    SetInvalidParameter(((ArgumentOutOfRangeException)ex).ParamName, ((ArgumentOutOfRangeException)ex).Message);
                }
            }
            // 
            if (_propertyChanging == false)
            {
                _distributionChanging = true;
                SelectedDistribution = currentDistribution.Clone();
                _distributionChanging = false;
            }
            // Update the plot and summary statistics
            UpdatePDFPlot();
            UpdateDistributionStats();
        }
        private bool _distributionChanging = false;

        /// <summary>
        /// Clears validation errors from all parameters in the parameter list.
        /// </summary>
        private void ClearValidation()
        {
            foreach (Parameter p in ParameterList)
            {
                p.IsValid = true;
                p.ErrorMessage = null;
            }
        }

        /// <summary>
        /// Marks a specific parameter as invalid and sets its error message.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to mark as invalid.</param>
        /// <param name="errorMessage">The validation error message to display.</param>
        private void SetInvalidParameter(string parameterName, string errorMessage)
        {
            Parameter param = ParameterList.FirstOrDefault(o => string.Equals(o.Name, parameterName, StringComparison.OrdinalIgnoreCase));
            if (param == null) { return; }
            param.IsValid = false;
            param.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Update the PDF plot.
        /// </summary>
        private void UpdatePDFPlot()
        {
            OxyPlot.Series.AreaSeries areaSeries = (OxyPlot.Series.AreaSeries)PDF.InternalSeries;

            if (SampleData == null || SampleData.Length == 0)
            { PDF.Fill = (Color)ColorConverter.ConvertFromString("#FF3F8BD6"); }
            else
            { PDF.Fill = Color.FromArgb(100, 63, 139, 214); }

            areaSeries.Points.Clear();
            areaSeries.Points2.Clear();
            if ((SelectedDistribution != null) && SelectedDistribution.ParametersValid)
            {
                InvalidDistributionTextBlock.Visibility = Visibility.Collapsed;
                Plot.Visibility = Visibility.Visible;
                double[,] PDFgraph;

                if (SelectedDistribution.Type == UnivariateDistributionType.Deterministic)
                {
                    PDFgraph = new double[5, 2];
                    PDFgraph[0, 0] = 0d;
                    PDFgraph[0, 1] = 0d;
                    PDFgraph[1, 0] = SelectedDistribution.Mean;
                    PDFgraph[1, 1] = 0d;
                    PDFgraph[2, 0] = SelectedDistribution.Mean;
                    PDFgraph[2, 1] = 1d;
                    PDFgraph[3, 0] = SelectedDistribution.Mean;
                    PDFgraph[3, 1] = 0d;
                    PDFgraph[4, 0] = SelectedDistribution.Mean * 2d;
                    PDFgraph[4, 1] = 0d;
                }
                else
                {
                    List<StratificationBin> range = Stratify.XValues(new StratificationOptions(SelectedDistribution.InverseCDF(0.001d), SelectedDistribution.InverseCDF(0.999d), 500));
                    PDFgraph = SelectedDistribution.CreatePDFGraph(range);
                }
                 
                // Create PDF Plot
                OxyPlot.Series.AreaSeries arSeries = (OxyPlot.Series.AreaSeries)PDF.InternalSeries;
                arSeries.Points.Clear();
                arSeries.Points2.Clear();
                for (int i = 0; i < PDFgraph.GetLength(0); i++)
                {
                    arSeries.Points.Add(new OxyPlot.DataPoint(PDFgraph[i, 0], PDFgraph[i, 1]));
                    arSeries.Points2.Add(new OxyPlot.DataPoint(PDFgraph[i, 0], 0d));
                }
            }
            else
            {
                InvalidDistributionTextBlock.Visibility = Visibility.Visible;
                Plot.Visibility = Visibility.Hidden;
                InvalidDistributionTextBlock.Text = SelectedDistribution == null ? "No distribution has been selected." : "Selected distribution has invalid parameters.";
            }
            // 
            Plot.InvalidatePlot();
        }

        /// <summary>
        /// Update the summary statistics with culture-aware formatting.
        /// </summary>
        private void UpdateDistributionStats()
        {
            // Distribution
            if ((SelectedDistribution == null) || (SelectedDistribution.ParametersValid == false))
            {
                // Use culture-aware formatting for NaN values
                string nanValue = NumberFormatHelper.FormatDouble(double.NaN, 4, false);
                SummaryStatisticsList[0].DistStat = nanValue;
                SummaryStatisticsList[1].DistStat = nanValue;
                SummaryStatisticsList[2].DistStat = nanValue;
                SummaryStatisticsList[3].DistStat = nanValue;
                SummaryStatisticsList[4].DistStat = nanValue;
                SummaryStatisticsList[5].DistStat = nanValue;
                SummaryStatisticsList[6].DistStat = nanValue;
                SummaryStatisticsList[7].DistStat = nanValue;
                SummaryStatisticsList[8].DistStat = nanValue;
                SummaryStatisticsList[9].DistStat = nanValue;
                SummaryStatisticsList[10].DistStat = nanValue;
                SummaryStatisticsList[11].DistStat = nanValue;

                if (DistributionCanBeEstimated() && SummaryStatisticsList.Count == 15)
                {
                    // Reset goodness of fit stats
                    SummaryStatisticsList[12].DistStat = " - ";
                    SummaryStatisticsList[13].DistStat = " - ";
                    SummaryStatisticsList[14].DistStat = " - ";
                }

                SummaryTable.Items.Refresh();
            }
            else
            {
                // Use culture-aware formatting via NumberFormatHelper
                SummaryStatisticsList[0].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.Minimum, 4, false);
                SummaryStatisticsList[1].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.Maximum, 4, false);
                SummaryStatisticsList[2].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.Mean, 4, false);
                SummaryStatisticsList[3].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.Mode, 4, false);
                SummaryStatisticsList[4].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.StandardDeviation, 4, false);
                SummaryStatisticsList[5].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.Skewness, 4, false);
                SummaryStatisticsList[6].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.Kurtosis, 4, false);
                SummaryStatisticsList[7].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.InverseCDF(0.05d), 4, false);
                SummaryStatisticsList[8].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.InverseCDF(0.25d), 4, false);
                SummaryStatisticsList[9].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.InverseCDF(0.5d), 4, false);
                SummaryStatisticsList[10].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.InverseCDF(0.75d), 4, false);
                SummaryStatisticsList[11].DistStat = NumberFormatHelper.FormatDouble(SelectedDistribution.InverseCDF(0.95d), 4, false);

                // Add Goodness of fit stats with culture-aware formatting
                if (SampleData != null && DistributionCanBeEstimated() == true)
                {
                    var data = SampleData.ToArray();
                    Array.Sort(data);

                    var rmse = GoodnessOfFit.RMSE(SampleData, SelectedDistribution);
                    var chi = GoodnessOfFit.ChiSquared(data, SelectedDistribution);
                    var ks = GoodnessOfFit.KolmogorovSmirnov(data, SelectedDistribution);

                    if (SummaryStatisticsList.Count == 12)
                    {
                        // Add goodness of fit stats
                        SummaryStatisticsList.Add(new SummaryStatistic("RMSE", "", " - "));
                        SummaryStatisticsList.Add(new SummaryStatistic("Chi-Squared", "", " - "));
                        SummaryStatisticsList.Add(new SummaryStatistic("K-S", "", " - "));
                    }

                    SummaryStatisticsList[12].DistStat = NumberFormatHelper.FormatDouble(rmse, 4, false);
                    SummaryStatisticsList[13].DistStat = NumberFormatHelper.FormatDouble(chi, 4, false);
                    SummaryStatisticsList[14].DistStat = NumberFormatHelper.FormatDouble(ks, 4, false);
                }
                else
                {
                    if (SummaryStatisticsList.Count == 15)
                    {
                        // Reset goodness of fit stats when not applicable
                        SummaryStatisticsList[12].DistStat = " - ";
                        SummaryStatisticsList[13].DistStat = " - ";
                        SummaryStatisticsList[14].DistStat = " - ";
                    }
                }

                SummaryTable.Items.Refresh();
            }
        }

        /// <summary>
        /// Handles the estimate parameters button click event.
        /// Estimates distribution parameters from sample data using appropriate estimation methods.
        /// </summary>
        /// <param name="sender">The button that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void EstimateParametersButton_Click(object sender, RoutedEventArgs e)
        {
            if (DistributionCanBeEstimated() == true)
            {
                var estimatedDistribution = SelectedDistribution.Clone();
                if (estimatedDistribution.Type == UnivariateDistributionType.Pert)
                {
                    ((Pert)estimatedDistribution).Estimate(SampleData, ParameterEstimationMethod.MethodOfPercentiles);
                }
                else if (estimatedDistribution.Type == UnivariateDistributionType.Empirical)
                {
                    var pp = PlottingPositions.Weibull(SampleData.Length);
                    var x = SampleData.Clone() as double[];
                    Array.Sort(x);
                    ((EmpiricalDistribution)estimatedDistribution).SetParameters(x, pp);
                }
                else if (estimatedDistribution.Type == UnivariateDistributionType.KernelDensity)
                {
                    ((KernelDensity)estimatedDistribution).SetSampleData(SampleData);
                }
                else
                {
                    ((IEstimation)estimatedDistribution).Estimate(SampleData, ParameterEstimationMethod.MethodOfMoments);
                }

                SelectedDistribution = estimatedDistribution;
            }
            //
            UpdatePDFPlot();
        }
    }
}
