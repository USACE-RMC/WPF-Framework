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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Numerics.Data;
using OxyPlot;

namespace NumericControls
{
    /// <summary>
    /// A user control that combines a data table editor with a plot for visualizing ordered paired data.
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
    public partial class OrderedDataSelectorControl : UserControl
    {

        /// <summary>
        /// Dependency property for the XAxisLabel property.
        /// </summary>
        public static DependencyProperty XAxisLabelProperty = DependencyProperty.Register(nameof(XAxisLabel), typeof(string), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata("X Axis"));

        /// <summary>
        /// Gets or sets the label for the X axis of the plot.
        /// </summary>
        public string XAxisLabel
        {
            get { return (string)GetValue(XAxisLabelProperty); }
            set { SetValue(XAxisLabelProperty, value); }
        }

        /// <summary>
        /// Dependency property for the YAxisLabel property.
        /// </summary>
        public static DependencyProperty YAxisLabelProperty = DependencyProperty.Register(nameof(YAxisLabel), typeof(string), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata("Y Axis"));

        /// <summary>
        /// Gets or sets the label for the Y axis of the plot.
        /// </summary>
        public string YAxisLabel
        {
            get { return (string)GetValue(YAxisLabelProperty); }
            set { SetValue(YAxisLabelProperty, value); }
        }

        /// <summary>
        /// Dependency property for the PlotTitle property.
        /// </summary>
        public static DependencyProperty PlotTitleProperty = DependencyProperty.Register(nameof(PlotTitle), typeof(string), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the title of the plot.
        /// </summary>
        public string PlotTitle
        {
            get { return (string)GetValue(PlotTitleProperty); }
            set { SetValue(PlotTitleProperty, value); }
        }

        /// <summary>
        /// Dependency property for the PlotLegendPosition property.
        /// </summary>
        public static DependencyProperty PlotLegendPositionProperty = DependencyProperty.Register(nameof(PlotLegendPosition), typeof(LegendPosition), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(LegendPosition.BottomRight));

        /// <summary>
        /// Gets or sets the position of the legend on the plot.
        /// </summary>
        public LegendPosition PlotLegendPosition
        {
            get { return (LegendPosition)GetValue(PlotLegendPositionProperty); }
            set { SetValue(PlotLegendPositionProperty, value); }
        }

        /// <summary>
        /// Dependency property for the YAxisMinimum property.
        /// </summary>
        public static DependencyProperty YAxisMinimumProperty = DependencyProperty.Register(nameof(YAxisMinimum), typeof(double), typeof(OrderedDataSelectorControl), new UIPropertyMetadata(double.MinValue));

        /// <summary>
        /// Gets or sets the minimum value for the Y axis.
        /// </summary>
        public double YAxisMinimum
        {
            get { return (double)GetValue(YAxisMinimumProperty); }
            set { SetValue(YAxisMinimumProperty, value); }
        }

        /// <summary>
        /// Dependency property for the YAxisMaximum property.
        /// </summary>
        public static DependencyProperty YAxisMaximumProperty = DependencyProperty.Register(nameof(YAxisMaximum), typeof(double), typeof(OrderedDataSelectorControl), new UIPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or sets the maximum value for the Y axis.
        /// </summary>
        public double YAxisMaximum
        {
            get { return (double)GetValue(YAxisMaximumProperty); }
            set { SetValue(YAxisMaximumProperty, value); }
        }

        /// <summary>
        /// Dependency property for the XAxisMinimum property.
        /// </summary>
        public static DependencyProperty XAxisMinimumProperty = DependencyProperty.Register(nameof(XAxisMinimum), typeof(double), typeof(OrderedDataSelectorControl), new UIPropertyMetadata(double.MinValue));

        /// <summary>
        /// Gets or sets the minimum value for the X axis.
        /// </summary>
        public double XAxisMinimum
        {
            get { return (double)GetValue(XAxisMinimumProperty); }
            set { SetValue(XAxisMinimumProperty, value); }
        }

        /// <summary>
        /// Dependency property for the XAxisMaximum property.
        /// </summary>
        public static DependencyProperty XAxisMaximumProperty = DependencyProperty.Register(nameof(XAxisMaximum), typeof(double), typeof(OrderedDataSelectorControl), new UIPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or sets the maximum value for the X axis.
        /// </summary>
        public double XAxisMaximum
        {
            get { return (double)GetValue(XAxisMaximumProperty); }
            set { SetValue(XAxisMaximumProperty, value); }
        }

        /// <summary>
        /// Dependency property for the XColumnHeader property.
        /// </summary>
        public static DependencyProperty XColumnHeaderProperty = DependencyProperty.Register(nameof(XColumnHeader), typeof(string), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata("X Data"));

        /// <summary>
        /// Gets or sets the column header text for the X data column.
        /// </summary>
        public string XColumnHeader
        {
            get { return (string)GetValue(XColumnHeaderProperty); }
            set { SetValue(XColumnHeaderProperty, value); }
        }

        /// <summary>
        /// Dependency property for the YColumnHeader property.
        /// </summary>
        public static DependencyProperty YColumnHeaderProperty = DependencyProperty.Register(nameof(YColumnHeader), typeof(string), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata("Y Data"));

        /// <summary>
        /// Gets or sets the column header text for the Y data column.
        /// </summary>
        public string YColumnHeader
        {
            get { return (string)GetValue(YColumnHeaderProperty); }
            set { SetValue(YColumnHeaderProperty, value); }
        }

        /// <summary>
        /// Dependency property for the IsStrictX property.
        /// </summary>
        public static DependencyProperty IsStrictXProperty = DependencyProperty.Register(nameof(IsStrictX), typeof(bool), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether X values must be strictly ordered (no duplicates allowed).
        /// </summary>
        public bool IsStrictX
        {
            get { return (bool)GetValue(IsStrictXProperty); }
            set { SetValue(IsStrictXProperty, value); }
        }

        /// <summary>
        /// Dependency property for the IsStrictY property.
        /// </summary>
        public static DependencyProperty IsStrictYProperty = DependencyProperty.Register(nameof(IsStrictY), typeof(bool), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether Y values must be strictly ordered (no duplicates allowed).
        /// </summary>
        public bool IsStrictY
        {
            get { return (bool)GetValue(IsStrictYProperty); }
            set { SetValue(IsStrictYProperty, value); }
        }

        /// <summary>
        /// Dependency property for the OrderX property.
        /// </summary>
        public static DependencyProperty OrderXProperty = DependencyProperty.Register(nameof(OrderX), typeof(SortOrder), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(SortOrder.Ascending));

        /// <summary>
        /// Gets or sets the sort order for X values.
        /// </summary>
        public SortOrder OrderX
        {
            get { return (SortOrder)GetValue(OrderXProperty); }
            set { SetValue(OrderXProperty, value); }
        }

        /// <summary>
        /// Dependency property for the OrderY property.
        /// </summary>
        public static DependencyProperty OrderYProperty = DependencyProperty.Register(nameof(OrderY), typeof(SortOrder), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(SortOrder.Ascending));

        /// <summary>
        /// Gets or sets the sort order for Y values.
        /// </summary>
        public SortOrder OrderY
        {
            get { return (SortOrder)GetValue(OrderYProperty); }
            set { SetValue(OrderYProperty, value); }
        }

        /// <summary>
        /// Dependency property for the MaximumX property.
        /// </summary>
        public static DependencyProperty MaximumXProperty = DependencyProperty.Register(nameof(MaximumX), typeof(double), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or sets the maximum allowed X value for data entry.
        /// </summary>
        public double MaximumX
        {
            get { return (double)GetValue(MaximumXProperty); }
            set { SetValue(MaximumXProperty, value); }
        }

        /// <summary>
        /// Dependency property for the MinimumX property.
        /// </summary>
        public static DependencyProperty MinimumXProperty = DependencyProperty.Register(nameof(MinimumX), typeof(double), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MinValue));

        /// <summary>
        /// Gets or sets the minimum allowed X value for data entry.
        /// </summary>
        public double MinimumX
        {
            get { return (double)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        /// <summary>
        /// Dependency property for the MaximumY property.
        /// </summary>
        public static DependencyProperty MaximumYProperty = DependencyProperty.Register(nameof(MaximumY), typeof(double), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or sets the maximum allowed Y value for data entry.
        /// </summary>
        public double MaximumY
        {
            get { return (double)GetValue(MaximumYProperty); }
            set { SetValue(MaximumYProperty, value); }
        }

        /// <summary>
        /// Dependency property for the MinimumY property.
        /// </summary>
        public static DependencyProperty MinimumYProperty = DependencyProperty.Register(nameof(MinimumY), typeof(double), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MinValue));

        /// <summary>
        /// Gets or sets the minimum allowed Y value for data entry.
        /// </summary>
        public double MinimumY
        {
            get { return (double)GetValue(MinimumYProperty); }
            set { SetValue(MinimumYProperty, value); }
        }

        /// <summary>
        /// Dependency property for the IsReadOnly property.
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the data grid is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Dependency property for the TableWidth property.
        /// </summary>
        public static DependencyProperty TableWidthProperty = DependencyProperty.Register(nameof(TableWidth), typeof(GridLength), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(new GridLength(1d, GridUnitType.Star)));

        /// <summary>
        /// Gets or sets the width of the data table in the grid layout.
        /// </summary>
        public GridLength TableWidth
        {
            get { return (GridLength)GetValue(TableWidthProperty); }
            set { SetValue(TableWidthProperty, value); }
        }

        /// <summary>
        /// Dependency property for the SelectedOrderedData property.
        /// </summary>
        public static DependencyProperty SelectedOrderedDataProperty = DependencyProperty.Register(nameof(SelectedOrderedData), typeof(OrderedPairedData), typeof(OrderedDataSelectorControl), new PropertyMetadata(new OrderedPairedData(false, SortOrder.Ascending, false, SortOrder.Ascending), SetData));

        private static void SetData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(OrderedDataSelectorControl)) return;
            OrderedDataSelectorControl thisControl = (OrderedDataSelectorControl)d;
            if (e.NewValue == null) return;
            //
            OrderedPairedData newCurve = e.NewValue as OrderedPairedData;
            if (newCurve == null) return;
            // Define the data
            thisControl.CurveRows.Clear();
            //
            OrdinateRowItem rowItem;
            foreach (Ordinate o in newCurve)
            {
                rowItem = new OrdinateRowItem(o.X, o.Y, thisControl.XColumnHeader, thisControl.YColumnHeader, thisControl.CurveRows, thisControl.MinimumX, thisControl.MaximumX, thisControl.MinimumY, thisControl.MaximumY, thisControl.IsStrictX, thisControl.IsStrictY, thisControl.OrderX, thisControl.OrderY);
                rowItem.PropertyChanged += (object sender, PropertyChangedEventArgs pe) =>
                {
                    OrdinateRowItem rItem = (OrdinateRowItem)sender;
                    int dataIndex = thisControl.CurveRows.IndexOf(rItem);
                    thisControl.SelectedOrderedData[dataIndex] = rItem.GetOrdinate();
                    thisControl.UpdatePlot(dataIndex);
                    // RaiseEvent DataChanged(dataIndex)
                };
                thisControl.CurveRows.Add(rowItem);
            }

            thisControl.ValidationGrid.ItemsSource = thisControl.CurveRows;
            thisControl.UpdatePlot();
        }

        /// <summary>
        /// Gets or sets the selected ordered paired data.
        /// </summary>
        public OrderedPairedData SelectedOrderedData
        {
            get { return (OrderedPairedData)GetValue(SelectedOrderedDataProperty); }
            set { SetValue(SelectedOrderedDataProperty, value); }
        }

        /// <summary>
        /// Gets the collection of data points for the plot curve.
        /// </summary>
        public ObservableCollection<DataPoint> CurveLinePoints { get; private set; } = new ObservableCollection<DataPoint>();

        /// <summary>
        /// Gets the collection of curve row items for the data grid.
        /// </summary>
        public ObservableCollection<object> CurveRows { get; private set; } = new ObservableCollection<object>();

        /// <summary>
        /// Event raised when plot properties are requested.
        /// </summary>
        public event PlotPropertiesRequestedEventHandler PlotPropertiesRequested;

        /// <summary>
        /// Delegate for the PlotPropertiesRequested event.
        /// </summary>
        /// <param name="plotRequestingProperties">The plot control requesting properties.</param>
        public delegate void PlotPropertiesRequestedEventHandler(OxyPlot.Wpf.Plot plotRequestingProperties);

        /// <summary>
        /// Initializes a new instance of the OrderedDataSelectorControl class.
        /// </summary>
        public OrderedDataSelectorControl()
        {

            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
        }

        /// <summary>
        /// Updates the data grid by forcing validation on all rows.
        /// </summary>
        public void UpdateGrid()
        {
            foreach (var r in CurveRows)
                ((OrdinateRowItem)r).ForceValidation();
        }

        /// <summary>
        /// Updates the plot with the current data.
        /// </summary>
        /// <param name="dataIndex">Optional index of the specific data point to update. If -1, all points are updated.</param>
        public void UpdatePlot(int dataIndex = -1)
        {
            OrdinateRowItem rowItem;
            if (dataIndex == -1 || dataIndex >= CurveRows.Count || dataIndex >= CurveLinePoints.Count)
            {
                CurveLinePoints.Clear();
                for (int i = 0; i < CurveRows.Count; i++)
                {
                    if (CurveRows[i].GetType() != typeof(OrdinateRowItem))
                        continue;
                    rowItem = (OrdinateRowItem)CurveRows[i];
                    //
                    CurveLinePoints.Add(new DataPoint(rowItem.X, rowItem.Y));
                }
            }
            else
            {
                rowItem = (OrdinateRowItem)CurveRows[dataIndex];
                CurveLinePoints[dataIndex] = new DataPoint(rowItem.X, rowItem.Y);
            }
            //
            Plot.InvalidatePlot(true);
        }

        private void DataGridColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Primitives.DataGridColumnHeader columnHeader = sender as System.Windows.Controls.Primitives.DataGridColumnHeader;
            if (columnHeader == null) return;
            //
            ValidationGrid.SelectedCells.Clear();
            foreach (var item in ValidationGrid.Items)
                ValidationGrid.SelectedCells.Add(new DataGridCellInfo(item, columnHeader.Column));
        }

        private void OxyplotToolbar_PropertiesCalled(OxyPlot.Wpf.Plot targetPlot, bool openProperties, OxyplotControls.OxyplotPropertiesControl.PropertyEXP propertyExpander, object selectedObject)
        {
            PlotPropertiesRequested?.Invoke(targetPlot);
        }

        private void ValidationGrid_RowsAdded(int startrow, int numrows)
        {
            for (int i = startrow; i < startrow + numrows; i++)
                SelectedOrderedData.Insert(i, ((OrdinateRowItem)CurveRows[i]).GetOrdinate());
            UpdateGrid();
            UpdatePlot();
        }

        private void ValidationGrid_RowsDeleted(List<int> rowindices)
        {
            //
            // Refresh the source
            rowindices.Sort();
            for (int i = rowindices.Count - 1; i >= 0; i -= 1)
                SelectedOrderedData.RemoveAt(rowindices[i]);
            //
            UpdateGrid();
            UpdatePlot();
        }

        private void ValidationGrid_DataPasted()
        {
            UpdateGrid();
            UpdatePlot();
        }

        private void ValidationGrid_PreviewAddRows(int startRowIndex, int nRows, ref bool cancelAddRows)
        {
            cancelAddRows = true;
            //
            for (int i = startRowIndex; i < startRowIndex + nRows; i++)
                CurveRows.Insert(i, new OrdinateRowItem(0d, 0d, XColumnHeader, YColumnHeader, CurveRows, MinimumX, MaximumX, MinimumY, MaximumY, IsStrictX, IsStrictY, OrderX, OrderY));
            //
            ValidationGrid_RowsAdded(startRowIndex, nRows);
        }


        // Private Sub UpdateSource()
        // Dim updatedOrdinates(CurveRows.Count - 1) As Ordinate
        // For i As Int32 = 0 To CurveRows.Count - 1
        // updatedOrdinates(i) = DirectCast(CurveRows(i), OrdinateRowItem).GetOrdinate
        // Next
        // SelectedOrderedData = New OrderedPairedData(updatedOrdinates, IsStrictX, OrderX, IsStrictY, OrderY)
        // End Sub
    }
}
