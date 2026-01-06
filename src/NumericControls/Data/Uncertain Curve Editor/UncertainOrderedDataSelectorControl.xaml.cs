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

using Numerics.Data;
using Numerics.Distributions;
using OxyPlot;
using OxyPlot.Wpf;
using OxyplotControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace NumericControls
{
    /// <summary>
    /// A user control that combines a data table editor with a plot for visualizing uncertain ordered paired data.
    /// Allows users to select distribution types and edit distribution parameters for each data point.
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
    public partial class UncertainOrderedDataSelectorControl : UserControl
    {
        public static DependencyProperty XAxisLabelProperty = DependencyProperty.Register(nameof(XAxisLabel), typeof(string), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata("X Axis"));

        public string XAxisLabel
        {
            get { return (string)GetValue(XAxisLabelProperty); }
            set { SetValue(XAxisLabelProperty, value); }
        }

        public static DependencyProperty YAxisLabelProperty = DependencyProperty.Register(nameof(YAxisLabel), typeof(string), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata("Y Axis"));

        public string YAxisLabel
        {
            get { return (string)GetValue(YAxisLabelProperty); }
            set { SetValue(YAxisLabelProperty, value); }
        }

        public static DependencyProperty PlotTitleProperty = DependencyProperty.Register(nameof(PlotTitle), typeof(string), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(null));

        public string PlotTitle
        {
            get { return (string)GetValue(PlotTitleProperty); }
            set { SetValue(PlotTitleProperty, value); }
        }

        public static DependencyProperty PlotLegendPositionProperty = DependencyProperty.Register(nameof(PlotLegendPosition), typeof(global::OxyPlot.LegendPosition), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(LegendPosition.BottomRight));

        public LegendPosition PlotLegendPosition
        {
            get { return (LegendPosition)GetValue(PlotLegendPositionProperty); }
            set {  SetValue(PlotLegendPositionProperty, value); }
        }

        public static DependencyProperty YAxisMinimumProperty = DependencyProperty.Register(nameof(YAxisMinimum), typeof(double), typeof(UncertainOrderedDataSelectorControl), new UIPropertyMetadata(double.MinValue));
        public double YAxisMinimum
        {
            get { return (double)GetValue(YAxisMinimumProperty); }
            set { SetValue(YAxisMinimumProperty, value); }
        }

        public static DependencyProperty YAxisMaximumProperty = DependencyProperty.Register(nameof(YAxisMaximum), typeof(double), typeof(UncertainOrderedDataSelectorControl), new UIPropertyMetadata(double.MaxValue));
        public double YAxisMaximum
        {
            get { return (double)GetValue(YAxisMaximumProperty); }
            set { SetValue(YAxisMaximumProperty, value); }
        }

        public static DependencyProperty XAxisMinimumProperty = DependencyProperty.Register(nameof(XAxisMinimum), typeof(double), typeof(UncertainOrderedDataSelectorControl), new UIPropertyMetadata(double.MinValue));
        public double XAxisMinimum
        {
            get { return (double)GetValue(XAxisMinimumProperty); }
            set { SetValue(XAxisMinimumProperty, value); }
        }

        public static DependencyProperty XAxisMaximumProperty = DependencyProperty.Register(nameof(XAxisMaximum), typeof(double), typeof(UncertainOrderedDataSelectorControl), new UIPropertyMetadata(double.MaxValue));
        public double XAxisMaximum
        {
            get { return (double)GetValue(XAxisMaximumProperty); }
            set { SetValue(XAxisMaximumProperty, value); }
        }

        public static DependencyProperty XColumnHeaderProperty = DependencyProperty.Register(nameof(XColumnHeader), typeof(string), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata("X Data"));
        public string XColumnHeader
        {
            get
            { return (string)GetValue(XColumnHeaderProperty); }
            set { SetValue(XColumnHeaderProperty, value); }
        }

        public static DependencyProperty YColumnHeaderProperty = DependencyProperty.Register(nameof(YColumnHeader), typeof(string), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata("Y Data"));
        public string YColumnHeader
        {
            get { return (string)GetValue(YColumnHeaderProperty); }
            set { SetValue(YColumnHeaderProperty, value); }
        }

        public static DependencyProperty IsStrictXProperty = DependencyProperty.Register(nameof(IsStrictX), typeof(bool), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(false));
        public bool IsStrictX
        {
            get { return (bool)GetValue(IsStrictXProperty); }
            set { SetValue(IsStrictXProperty, value); }
        }

        public static DependencyProperty IsStrictYProperty = DependencyProperty.Register(nameof(IsStrictY), typeof(bool), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(false));
        public bool IsStrictY
        {
            get { return (bool)GetValue(IsStrictYProperty); }
            set { SetValue(IsStrictYProperty, value); }
        }

        public static DependencyProperty OrderXProperty = DependencyProperty.Register(nameof(OrderX), typeof(SortOrder), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(SortOrder.Ascending));
        public SortOrder OrderX
        {
            get { return (SortOrder)GetValue(OrderXProperty); }
            set { SetValue(OrderXProperty, value); }
        }

        public static DependencyProperty OrderYProperty = DependencyProperty.Register(nameof(OrderY), typeof(SortOrder), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(SortOrder.Ascending));
        public SortOrder OrderY
        {
            get { return (SortOrder)GetValue(OrderYProperty); }
            set { SetValue(OrderYProperty, value); }
        }

        public static DependencyProperty MaximumXProperty = DependencyProperty.Register(nameof(MaximumX), typeof(double), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MaxValue));
        public double MaximumX
        {
            get { return (double)GetValue(MaximumXProperty); }
            set { SetValue(MaximumXProperty, value); }
        }

        public static DependencyProperty MinimumXProperty = DependencyProperty.Register(nameof(MinimumX), typeof(double), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MinValue));
        public double MinimumX
        {
            get { return (double)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        public static DependencyProperty MaximumYProperty = DependencyProperty.Register(nameof(MaximumY), typeof(double), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MaxValue));
        public double MaximumY
        {
            get { return (double)GetValue(MaximumYProperty); }
            set { SetValue(MaximumYProperty, value); }
        }

        public static DependencyProperty MinimumYProperty = DependencyProperty.Register(nameof(MinimumY), typeof(double), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MinValue));
        public double MinimumY
        {
            get { return (double)GetValue(MinimumYProperty); }
            set { SetValue(MinimumYProperty, value); }
        }

        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(false));
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static DependencyProperty TableWidthProperty = DependencyProperty.Register(nameof(TableWidth), typeof(GridLength), typeof(UncertainOrderedDataSelectorControl), new FrameworkPropertyMetadata(new GridLength(1d, GridUnitType.Star)));
        public GridLength TableWidth
        {
            get { return (GridLength)GetValue(TableWidthProperty); }
            set { SetValue(TableWidthProperty, value); }
        }

        public static DependencyProperty SelectedUncertainOrderedDataProperty = DependencyProperty.Register(nameof(SelectedUncertainOrderedData), typeof(UncertainOrderedPairedData), typeof(UncertainOrderedDataSelectorControl), new PropertyMetadata(new UncertainOrderedPairedData(false, SortOrder.Ascending, false, SortOrder.Ascending, UnivariateDistributionType.Deterministic), SetData));

        private static void SetData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(UncertainOrderedDataSelectorControl)) return;
            UncertainOrderedDataSelectorControl thisControl = (UncertainOrderedDataSelectorControl)d;
            if (e.NewValue == null)
            {
                thisControl.CurveUncertaintyComboBox.SelectedIndex = -1;
                return;
            }
            // 
            // GenerateCurveSet(Curve)
            UncertainOrderedPairedData newCurve = e.NewValue as UncertainOrderedPairedData;
            if (newCurve == null)
            {
                thisControl.CurveUncertaintyComboBox.SelectedIndex = -1;
                return;
            }
            // 
            int index = -1;
            for (int i = 0; i < thisControl.Distributions.Count; i++)
            {
                if (thisControl.Distributions[i].Data.Distribution == newCurve.Distribution)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                thisControl.CurveUncertaintyComboBox.SelectedIndex = -1;
                return;
            }
            // 
            thisControl.CurveUncertaintyComboBox.SelectionChanged -= thisControl.CurveUncertaintyComboBox_SelectionChanged;
            thisControl.CurveUncertaintyComboBox.SelectedIndex = -1;
            thisControl.CurveUncertaintyComboBox.SelectionChanged += thisControl.CurveUncertaintyComboBox_SelectionChanged;
            // 
            var newItem = new DistributionDataItem(newCurve, newCurve.Distribution, thisControl.MinimumX, thisControl.MaximumX, thisControl.MinimumY, thisControl.MaximumY, thisControl.IsStrictX, thisControl.IsStrictY, thisControl.OrderX, thisControl.OrderY);
            newItem.DataChanged += (int dataIndex) =>
            {
                if (thisControl._pastingData == false)
                    thisControl.GetBindingExpression(SelectedUncertainOrderedDataProperty).UpdateSource();
                thisControl.UpdatePlot(dataIndex);
            };
            thisControl.Distributions[index] = newItem;
            // 
            thisControl.CurveUncertaintyComboBox.SelectedIndex = index;
            // Update lines based on distribution type
            thisControl.MeanLine.Visibility = Visibility.Visible;
            thisControl.MedianLine.Visibility = Visibility.Visible;
            thisControl.ModeLine.Visibility = Visibility.Visible;
            thisControl.MaximumLine.Visibility = Visibility.Visible;
            thisControl.MinimumLine.Visibility = Visibility.Visible;
            thisControl.UncertaintyBoundsArea.Visibility = Visibility.Visible;
            switch (newCurve.Distribution)
            {
                case UnivariateDistributionType.Uniform:
                    {
                        thisControl.MeanLine.Visibility = Visibility.Collapsed;
                        thisControl.MedianLine.Visibility = Visibility.Collapsed;
                        thisControl.ModeLine.Visibility = Visibility.Collapsed;
                        break;
                    }

                case UnivariateDistributionType.Deterministic:
                    {
                        thisControl.MaximumLine.Visibility = Visibility.Collapsed;
                        thisControl.MinimumLine.Visibility = Visibility.Collapsed;
                        thisControl.MedianLine.Visibility = Visibility.Collapsed;
                        thisControl.ModeLine.Visibility = Visibility.Collapsed;
                        thisControl.UncertaintyBoundsArea.Visibility = Visibility.Collapsed;
                        break;
                    }
            }
        }

        /// <summary>
        /// Get and set the selected probability distribution.
        /// </summary>
        public UncertainOrderedPairedData SelectedUncertainOrderedData
        {
            get { return (UncertainOrderedPairedData)GetValue(SelectedUncertainOrderedDataProperty); }
            set { SetValue(SelectedUncertainOrderedDataProperty, value); }
        }

        private ObservableCollection<DistributionDataItem> Distributions { get; set; } = new ObservableCollection<DistributionDataItem>();
       
        /// <summary>
        /// Dependency property for the control distribution options.
        /// </summary>
        public static DependencyProperty DistributionOptionsProperty = DependencyProperty.Register(nameof(DistributionOptions), typeof(List<UnivariateDistributionType>), typeof(UncertainOrderedDataSelectorControl), new PropertyMetadata(DefaultDistributionOptions, DistributionOptionsCallback));

        /// <summary>
        /// Gets and sets the distribution options.
        /// </summary>
        public List<UnivariateDistributionType> DistributionOptions
        {
            get { return (List<UnivariateDistributionType>)GetValue(DistributionOptionsProperty); }
            set { SetValue(DistributionOptionsProperty, value); }
        }

        /// <summary>
        /// When the dependency property changes, this sets the distribution options.
        /// </summary>
        private static void DistributionOptionsCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UncertainOrderedDataSelectorControl thisControl = (UncertainOrderedDataSelectorControl)d;
            thisControl.Distributions.Clear();
            UnivariateDistributionBase distribution;
            if ((e.NewValue == null) || (e.NewValue.GetType() != typeof(List<UnivariateDistributionType>)))
            {
                thisControl.DistributionOptions = DefaultDistributionOptions;
            }
            else
            {
                foreach (var dist in (List<UnivariateDistributionType>)e.NewValue)
                {
                    distribution = UnivariateDistributionFactory.CreateDistribution(dist);
                    if (distribution == null) continue;
                    // 
                    var ordinates = new List<UncertainOrdinate>();
                    for (int i = 0; i <= 5; i++)
                        ordinates.Add(new UncertainOrdinate(i * 2, distribution));
                    var distCurve = new UncertainOrderedPairedData(ordinates, thisControl.IsStrictX, thisControl.OrderX, thisControl.IsStrictY, thisControl.OrderY, dist);
                    // 
                    var distItem = new DistributionDataItem(distCurve, dist, thisControl.MinimumX, thisControl.MaximumX, thisControl.MinimumY, thisControl.MaximumY, thisControl.IsStrictX, thisControl.IsStrictY, thisControl.OrderX, thisControl.OrderY);
                    thisControl.Distributions.Add(distItem);
                }
            }
            // 
        }

        /// <summary>
        /// Currently does not support bivariate, univariate, or kernel density.
        /// </summary>
        /// <returns></returns>
        public static List<UnivariateDistributionType> DefaultDistributionOptions
        {
            get
            {
                return ((UnivariateDistributionType[])Enum.GetValues(typeof(UnivariateDistributionType))).Where(o => (o != UnivariateDistributionType.Bernoulli) & 
                                                                                                                     (o != UnivariateDistributionType.Beta) & 
                                                                                                                     (o != UnivariateDistributionType.Binomial) &
                                                                                                                     (o != UnivariateDistributionType.Cauchy) &
                                                                                                                     (o != UnivariateDistributionType.ChiSquared) &
                                                                                                                     (o != UnivariateDistributionType.CompetingRisks) &
                                                                                                                     (o != UnivariateDistributionType.Empirical) &
                                                                                                                     (o != UnivariateDistributionType.Geometric) &
                                                                                                                     (o != UnivariateDistributionType.InverseChiSquared) &
                                                                                                                     (o != UnivariateDistributionType.InverseGamma) &
                                                                                                                     (o != UnivariateDistributionType.KappaFour) & 
                                                                                                                     (o != UnivariateDistributionType.KernelDensity) &
                                                                                                                     (o != UnivariateDistributionType.Mixture) &
                                                                                                                     (o != UnivariateDistributionType.NoncentralT) &
                                                                                                                     (o != UnivariateDistributionType.UniformDiscrete) &
                                                                                                                     (o != UnivariateDistributionType.Poisson)).ToList();
            }
        }

        public ObservableCollection<DataPoint> MinimumLinePoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> MaximumLinePoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> MeanLinePoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> MedianLinePoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> ModeLinePoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<AreaPoint> AreaPoints { get; private set; } = new ObservableCollection<AreaPoint>();

        public event PlotPropertiesRequestedEventHandler PlotPropertiesRequested;

        public delegate void PlotPropertiesRequestedEventHandler(Plot targetPlot, bool openProperties, OxyplotPropertiesControl.PropertyEXP propertyExpander, object selectedObject);

        private bool _pastingData = false;
        private bool _isLoaded = false;

        public UncertainOrderedDataSelectorControl()
        {

            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.

            DistributionOptions = DefaultDistributionOptions;
            // Bind the combobox
            CurveUncertaintyComboBox.ItemsSource = Distributions;
            CurveUncertaintyComboBox.DisplayMemberPath = "DistributionName";
        }

        private void CurveUncertaintyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurveUncertaintyComboBox.SelectedIndex != -1)
            {
                ValidationGrid.ItemsSource = ((DistributionDataItem)CurveUncertaintyComboBox.SelectedItem).DistributionRows;
                SelectedUncertainOrderedData = ((DistributionDataItem)CurveUncertaintyComboBox.SelectedItem).Data;
            }
            else
            {
                ValidationGrid.ItemsSource = null;
            }
            // 
            UpdatePlot();
            UpdateGrid();
        }

        public void UpdateGrid()
        {
            if (CurveUncertaintyComboBox.SelectedIndex == -1) return;
            foreach (var r in ((DistributionDataItem)CurveUncertaintyComboBox.SelectedItem).DistributionRows)
                ((DistributionRowItem)r).ForceValidation();
        }

        public void UpdatePlot(int dataIndex = -1)
        {
            if ((CurveUncertaintyComboBox.SelectedIndex == -1) || (CurveUncertaintyComboBox.SelectedItem.GetType() != typeof(DistributionDataItem)))
            {
                MinimumLinePoints.Clear();
                MaximumLinePoints.Clear();
                MeanLinePoints.Clear();
                MedianLinePoints.Clear();
                ModeLinePoints.Clear();
                AreaPoints.Clear();
                return;
            }

            var rows = ((DistributionDataItem)CurveUncertaintyComboBox.SelectedItem).DistributionRows;
            // 
            DistributionRowItem rowItem;
            if ((dataIndex == -1) || (dataIndex >= rows.Count) || (dataIndex >= MinimumLinePoints.Count))
            {
                MinimumLinePoints.Clear();
                MaximumLinePoints.Clear();
                MeanLinePoints.Clear();
                MedianLinePoints.Clear();
                ModeLinePoints.Clear();
                AreaPoints.Clear();
                for (int i = 0; i < rows.Count; i++)
                {
                    if (rows[i].GetType() != typeof(DistributionRowItem)) continue;
                    rowItem = (DistributionRowItem)rows[i];
                    // 
                    MinimumLinePoints.Add(new DataPoint(rowItem.X, rowItem.Minimum));
                    MaximumLinePoints.Add(new DataPoint(rowItem.X, rowItem.Maximum));
                    MeanLinePoints.Add(new DataPoint(rowItem.X, rowItem.Mean));
                    MedianLinePoints.Add(new DataPoint(rowItem.X, rowItem.Distribution.Median));
                    ModeLinePoints.Add(new DataPoint(rowItem.X, rowItem.Distribution.Mode));
                    AreaPoints.Add(new AreaPoint(MinimumLinePoints[i], MaximumLinePoints[i]));
                }
            }
            else
            {
                rowItem = (DistributionRowItem)rows[dataIndex];
                MinimumLinePoints[dataIndex] = new DataPoint(rowItem.X, rowItem.Minimum);
                MaximumLinePoints[dataIndex] = new DataPoint(rowItem.X, rowItem.Maximum);
                MeanLinePoints[dataIndex] = new DataPoint(rowItem.X, rowItem.Mean);
                MedianLinePoints[dataIndex] = new DataPoint(rowItem.X, rowItem.Distribution.Median);
                ModeLinePoints[dataIndex] = new DataPoint(rowItem.X, rowItem.Distribution.Mode);
                AreaPoints[dataIndex] = new AreaPoint(MinimumLinePoints[dataIndex], MaximumLinePoints[dataIndex]);
                Plot.InvalidatePlot(true);
            }
        }

        private void DataGridColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            DataGridColumnHeader columnHeader = sender as DataGridColumnHeader;
            if (columnHeader == null) return;
            // 
            ValidationGrid.SelectedCells.Clear();
            foreach (var item in ValidationGrid.Items)
                ValidationGrid.SelectedCells.Add(new DataGridCellInfo(item, columnHeader.Column));
        }

        private void OxyplotToolbar_PropertiesCalled(Plot targetPlot, bool openProperties, OxyplotPropertiesControl.PropertyEXP propertyExpander, object selectedObject)
        {
            PlotPropertiesRequested?.Invoke(targetPlot, openProperties, propertyExpander, selectedObject);
        }

        private void ValidationGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            MergedColumnHeaderGrid.ColumnDefinitions.Clear();
            var cd = new ColumnDefinition();
            cd.SetBinding(ColumnDefinition.WidthProperty, new Binding("RowHeaderActualWidth") { ElementName = "ValidationGrid" });
            MergedColumnHeaderGrid.ColumnDefinitions.Add(cd);
            foreach (var c in ValidationGrid.Columns)
            {
                cd = new ColumnDefinition();
                cd.SetBinding(ColumnDefinition.WidthProperty, new Binding("ActualWidth") { Source = c });
                MergedColumnHeaderGrid.ColumnDefinitions.Add(cd);
            }
        }

        private void ValidationGrid_PreviewAddRows(int startRowIndex, int nRows, ref bool cancelAddRows)
        {
            cancelAddRows = true;
            // 
            if (CurveUncertaintyComboBox.SelectedIndex == -1) return;
            // 
            DistributionDataItem selectedItem = (DistributionDataItem)CurveUncertaintyComboBox.SelectedItem;
            var type = selectedItem.Data.Distribution;
            for (int i = startRowIndex; i < startRowIndex + nRows; i++)
                selectedItem.Data.Insert(i, new UncertainOrdinate(0d, UnivariateDistributionFactory.CreateDistribution(type)));
            // Refresh the view
            selectedItem.Refresh(); // MinimumX, MaximumX, MinimumY, MaximumY, IsStrictX, IsStrictY, OrderX, OrderY)
            ValidationGrid.Items.Refresh();
            UpdateGrid();
            UpdatePlot();
            if (_pastingData == false)
                GetBindingExpression(SelectedUncertainOrderedDataProperty).UpdateSource();
        }

        private void ValidationGrid_RowsAdded(int startrow, int numrows)
        {
            UpdateGrid();
            UpdatePlot();
            if (_pastingData == false)
                GetBindingExpression(SelectedUncertainOrderedDataProperty).UpdateSource();
        }

        private void ValidationGrid_RowsDeleted(List<int> rowindices)
        {
            if (CurveUncertaintyComboBox.SelectedIndex == -1) return;
            DistributionDataItem selectedItem = (DistributionDataItem)CurveUncertaintyComboBox.SelectedItem;
            // 
            // Refresh the view
            rowindices.Sort();
            for (int i = rowindices.Count - 1; i >= 0; i -= 1)
                selectedItem.Data.RemoveAt(rowindices[i]);
            // 
            UpdateGrid();
            UpdatePlot();
            GetBindingExpression(SelectedUncertainOrderedDataProperty).UpdateSource();
        }

        private void ValidationGrid_DataPasted()
        {
            _pastingData = false;
            UpdateGrid();
            UpdatePlot();
            GetBindingExpression(SelectedUncertainOrderedDataProperty).UpdateSource();
            Mouse.OverrideCursor = null;
        }

        private void ValidationGrid_PreviewPasteData(string[][] clipboardData, ref bool cancelPaste)
        {
            _pastingData = true;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        private void UncertainOrderedDataSelectorControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoaded == false)
            {
                var dummyCollection = new ObservableCollection<object>();
                dummyCollection.Add(new DistributionRowItem(0d, new Deterministic(0d), dummyCollection, 0d, 10d, 0d, 10d, false, false, OrderX, OrderY));
                ValidationGrid.ItemsSource = dummyCollection;
                dummyCollection.Clear();
            }

            _isLoaded = true;
        }
    }

    /// <summary>
    /// Represents a point defining an area on a plot with X and Y coordinate bounds.
    /// Used to visualize uncertainty bounds between minimum and maximum distribution values.
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
    public class AreaPoint
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }

        public AreaPoint(DataPoint p1, DataPoint p2)
        {
            X1 = p1.X;
            X2 = p2.X;
            Y1 = p1.Y;
            Y2 = p2.Y;
        }
    }
}

