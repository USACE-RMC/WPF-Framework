using Numerics.Data.Statistics;
using Numerics.Distributions;
using Numerics.Sampling;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NumericControls.Distributions.Univariate
{
    /// <summary>
    /// Interaction logic for DistributionSelectorControl.xaml
    /// </summary>
    public partial class DistributionSelectorControl : UserControl
    {

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

            // DistributionOptions = DefaultDistributionOptions
            InitializeControl();
        }

        /// <summary>
        /// Initial the control elements.
        /// </summary>
        private void InitializeControl()
        {

            // Create summary statistics list
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
        /// Dependency property for expanding the distribution pdf plot.
        /// </summary>
        public static DependencyProperty ExpandPlotProperty = DependencyProperty.Register(nameof(ExpandPlot), typeof(bool), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets and sets the distribution options.
        /// </summary>
        public bool ExpandPlot
        {
            get => (bool)GetValue(ExpandPlotProperty);
            set => SetValue(ExpandPlotProperty, value);
        }

        public static DependencyProperty ExpanderStyleProperty = DependencyProperty.Register(nameof(ExpanderStyle), typeof(Style), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(null));
        public Style ExpanderStyle
        {
            get { return (Style)GetValue(ExpanderStyleProperty); }
            set { SetValue(ExpanderStyleProperty, value); }
        }

        /// <summary>
        /// Dependency property for showing the distribution summary statistics.
        /// </summary>
        public static DependencyProperty ShowStatisticsProperty = DependencyProperty.Register(nameof(ShowStatistics), typeof(bool), typeof(DistributionSelectorControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets and sets the distribution options.
        /// </summary>
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
        private static void SampleDataProperty_Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) { return; }
            if (d.GetType() != typeof(DistributionSelectorControl)) { return; }
            DistributionSelectorControl thisControl = (DistributionSelectorControl)d;
            //if (thisControl.DistributionCombobox == null) { return; }
            // 
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
                if (DistributionCanEstimate() == true)
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
                //var mode = Numerics.Mathematics.RootFinding.Brent.Solve((x) => { return stats[0] - (summaryPercentiles[0] + summaryPercentiles[6] + x) / 3; }, summaryPercentiles[0], summaryPercentiles[6]);
                var mode = histogram.Mode;

                SummaryStatisticsList[0].DataStat = summaryPercentiles[0].ToString("N4");
                SummaryStatisticsList[1].DataStat = summaryPercentiles[6].ToString("N4");
                SummaryStatisticsList[2].DataStat = stats[0].ToString("N4");
                SummaryStatisticsList[3].DataStat = mode.ToString("N4");
                SummaryStatisticsList[4].DataStat = stats[1].ToString("N4");
                SummaryStatisticsList[5].DataStat = stats[2].ToString("N4");
                SummaryStatisticsList[6].DataStat = stats[3].ToString("N4");
                SummaryStatisticsList[7].DataStat = summaryPercentiles[1].ToString("N4");
                SummaryStatisticsList[8].DataStat = summaryPercentiles[2].ToString("N4");
                SummaryStatisticsList[9].DataStat = summaryPercentiles[3].ToString("N4");
                SummaryStatisticsList[10].DataStat = summaryPercentiles[4].ToString("N4");
                SummaryStatisticsList[11].DataStat = summaryPercentiles[5].ToString("N4");

                SummaryStatisticsList.Add(new SummaryStatistic("RMSE", "", " - "));
                SummaryStatisticsList.Add(new SummaryStatistic("Chi-Squared", "", " - "));
                SummaryStatisticsList.Add(new SummaryStatistic("K-S", "", " - "));

                SummaryTable.Items.Refresh();

            }

            Plot.InvalidatePlot(true);
            UpdatePDFPlot();
        }

        /// <summary>
        /// The distribution combobox selection has changed. Perform action.
        /// </summary>
        private void DistributionCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //_fitted = false;

            // clear the parameter list
            for (int i = ParameterList.Count - 1; i >= 0; i--)
            {
                ParameterList[i].PropertyChanged -= ParameterPropertyChanged;
                ParameterList.RemoveAt(i);
            }

            if (DistributionCombobox.SelectedIndex == -1)
            {
                //SelectedDistribution = null;
                UpdatePDFPlot();
                UpdateDistributionStats();
                return;
            }
            // 
            UnivariateDistributionBase currentDistribution = (UnivariateDistributionBase)DistributionCombobox.SelectedItem;

            if (currentDistribution.Type == UnivariateDistributionType.Empirical)
            {
                // Dim uniEmp = DirectCast(currentDistribution, UnivariateEmpiricalCDF)
                // Dim minParam As New Parameter("Min", "Minimum", uniEmp.Min)
                // AddHandler minParam.PropertyChanged, Sub(s As Object, pce As PropertyChangedEventArgs) If pce.PropertyName = "Value" Then SetDistributionParameters()
                // ParameterList.Add(minParam)
                // Dim maxParam As New Parameter("Max", "Maximum", uniEmp.Max)
                // AddHandler maxParam.PropertyChanged, Sub(s As Object, pce As PropertyChangedEventArgs) If pce.PropertyName = "Value" Then SetDistributionParameters()
                // ParameterList.Add(maxParam)
                // UnivariateGrid.Visibility = Visibility.Visible
                // UnivariateDataControl.Distribution = uniEmp
            }
            else if (currentDistribution.Type == UnivariateDistributionType.KernelDensity) { }
            else
            {
                string[,] paramString = currentDistribution.ParametersToString;
                string[] paramNames = currentDistribution.GetParameterPropertyNames;
                // Update parameter grid
                for (int i = 0; i < paramString.GetLength(0); i++)
                {
                    if (double.TryParse(paramString[i, 1], out double paramValue) == false) { paramValue = double.NaN; }
                    Parameter param = new Parameter(paramNames[i], paramString[i, 0], paramValue);
                    param.PropertyChanged += ParameterPropertyChanged;
                    ParameterList.Add(param);
                }
            }

            // Update selected distribution
            if (_propertyChanging == false) { SetDistributionParameters(false); }

            //show fit options (filtered by those that implement IEstimate()
            if (SampleData != null && SampleData.Length > 0 && DistributionCanEstimate() == true)
            {
                EstimateParametersButton.Visibility = Visibility.Visible;
            }
            else
            {
                EstimateParametersButton.Visibility = Visibility.Collapsed;
            }
        }

        private bool DistributionCanEstimate()
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

        private void ClearValidation()
        {
            foreach (Parameter p in ParameterList)
            {
                p.IsValid = true;
                p.ErrorMessage = null;
            }
        }

        private void SetInvalidParameter(string parameterName, string errorMessage)
        {
            Parameter param = ParameterList.FirstOrDefault(o => string.Equals(o.Name, parameterName, StringComparison.OrdinalIgnoreCase));
            if (param == null) { return; }
            param.IsValid = false;
            param.ErrorMessage = errorMessage;
        }


        //private List<double> _sampleData;
        //private bool _addSampleData = false;
        //private bool _fitted = false;
        //public bool FitToData { get; set; } = false;



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

                // If SelectedDistribution.Type = ContinuousDistributionBase.DistributionType.Deterministic Then
                // You need to handle the deterministic plot for PDF. I don't think the range will work for it. 
                // You will want to do a column chart series for it. 
                // The CDF graph work fine though.
                // I created an override function for the deterministic CreatePDFGraph function.
                if (SelectedDistribution.Type == UnivariateDistributionType.Empirical)
                {
                    List<StratificationBin> range = Stratify.XValues(new StratificationOptions(SelectedDistribution.InverseCDF(0.001d), SelectedDistribution.InverseCDF(0.999d), 500));
                    PDFgraph = SelectedDistribution.CreatePDFGraph(range);
                }
                else if (SelectedDistribution.Type == UnivariateDistributionType.KernelDensity)
                {
                    PDFgraph = null;
                }
                else if (SelectedDistribution.Type == UnivariateDistributionType.Deterministic)
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
                // 
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
        /// Update the summary statistics.
        /// </summary>
        private void UpdateDistributionStats()
        {
            // Distribution
            if ((SelectedDistribution == null) || (SelectedDistribution.ParametersValid == false))
            {
                SummaryStatisticsList[0].DistStat = double.NaN.ToString("N4");
                SummaryStatisticsList[1].DistStat = double.NaN.ToString("N4");
                SummaryStatisticsList[2].DistStat = double.NaN.ToString("N4");
                SummaryStatisticsList[3].DistStat = double.NaN.ToString("N4");
                SummaryStatisticsList[4].DistStat = double.NaN.ToString("N4");
                SummaryStatisticsList[5].DistStat = double.NaN.ToString("N4");
                SummaryStatisticsList[6].DistStat = double.NaN.ToString("N4");
                SummaryStatisticsList[7].DistStat = double.NaN.ToString("N4");
                SummaryStatisticsList[8].DistStat = double.NaN.ToString("N4");
                SummaryStatisticsList[9].DistStat = double.NaN.ToString("N4");
                SummaryStatisticsList[10].DistStat = double.NaN.ToString("N4");
                SummaryStatisticsList[11].DistStat = double.NaN.ToString("N4");
                SummaryTable.Items.Refresh();
            }
            else
            {
                SummaryStatisticsList[0].DistStat = SelectedDistribution.Minimum.ToString("N4");
                SummaryStatisticsList[1].DistStat = SelectedDistribution.Maximum.ToString("N4");
                SummaryStatisticsList[2].DistStat = SelectedDistribution.Mean.ToString("N4");
                SummaryStatisticsList[3].DistStat = SelectedDistribution.Mode.ToString("N4");
                SummaryStatisticsList[4].DistStat = SelectedDistribution.StandardDeviation.ToString("N4");
                SummaryStatisticsList[5].DistStat = SelectedDistribution.Skewness.ToString("N4");
                SummaryStatisticsList[6].DistStat = SelectedDistribution.Kurtosis.ToString("N4");
                SummaryStatisticsList[7].DistStat = SelectedDistribution.InverseCDF(0.05d).ToString("N4");
                SummaryStatisticsList[8].DistStat = SelectedDistribution.InverseCDF(0.25d).ToString("N4");
                SummaryStatisticsList[9].DistStat = SelectedDistribution.InverseCDF(0.5d).ToString("N4");
                SummaryStatisticsList[10].DistStat = SelectedDistribution.InverseCDF(0.75d).ToString("N4");
                SummaryStatisticsList[11].DistStat = SelectedDistribution.InverseCDF(0.95d).ToString("N4");

                // Add Goodness of fit stats
                if (SampleData != null && DistributionCanEstimate() == true)
                {
                    var data = SampleData.ToArray();
                    Array.Sort(data);

                    var rmse = GoodnessOfFit.RMSE(SampleData, SelectedDistribution);
                    var chi = GoodnessOfFit.ChiSquared(data, SelectedDistribution);
                    var ks = GoodnessOfFit.KolmogorovSmirnov(data, SelectedDistribution);
                    
                    SummaryStatisticsList[12].DistStat = rmse.ToString("N4");
                    SummaryStatisticsList[13].DistStat = chi.ToString("N4");
                    SummaryStatisticsList[14].DistStat = ks.ToString("N4");
                }


                SummaryTable.Items.Refresh();
            }
        }

        private void EstimateParametersButton_Click(object sender, RoutedEventArgs e)
        {
            if (DistributionCanEstimate() == true)
            {
                var estimatedDistribution = SelectedDistribution.Clone();
                if (estimatedDistribution.Type == UnivariateDistributionType.Pert)
                {
                    ((Pert)estimatedDistribution).Estimate(SampleData, ParameterEstimationMethod.MethodOfPercentiles);
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
