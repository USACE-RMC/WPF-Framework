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
    /// Interaction logic for OrderedDataSelectorControl.xaml
    /// </summary>
    public partial class OrderedDataSelectorControl : UserControl
    {

        public static DependencyProperty XAxisLabelProperty = DependencyProperty.Register(nameof(XAxisLabel), typeof(string), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata("X Axis"));
        public string XAxisLabel
        {
            get { return (string)GetValue(XAxisLabelProperty); }
            set { SetValue(XAxisLabelProperty, value); }
        }

        public static DependencyProperty YAxisLabelProperty = DependencyProperty.Register(nameof(YAxisLabel), typeof(string), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata("Y Axis"));
        public string YAxisLabel
        {
            get { return (string)GetValue(YAxisLabelProperty); }
            set { SetValue(YAxisLabelProperty, value); }
        }

        public static DependencyProperty PlotTitleProperty = DependencyProperty.Register(nameof(PlotTitle), typeof(string), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(null));
        public string PlotTitle
        {
            get { return (string)GetValue(PlotTitleProperty); }
            set { SetValue(PlotTitleProperty, value); }
        }

        public static DependencyProperty PlotLegendPositionProperty = DependencyProperty.Register(nameof(PlotLegendPosition), typeof(LegendPosition), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(LegendPosition.BottomRight));
        public LegendPosition PlotLegendPosition
        {
            get { return (LegendPosition)GetValue(PlotLegendPositionProperty); }
            set { SetValue(PlotLegendPositionProperty, value); }
        }

        public static DependencyProperty YAxisMinimumProperty = DependencyProperty.Register(nameof(YAxisMinimum), typeof(double), typeof(OrderedDataSelectorControl), new UIPropertyMetadata(double.MinValue));
        public double YAxisMinimum
        {
            get { return (double)GetValue(YAxisMinimumProperty); }
            set { SetValue(YAxisMinimumProperty, value); }
        }

        public static DependencyProperty YAxisMaximumProperty = DependencyProperty.Register(nameof(YAxisMaximum), typeof(double), typeof(OrderedDataSelectorControl), new UIPropertyMetadata(double.MaxValue));
        public double YAxisMaximum
        {
            get { return (double)GetValue(YAxisMaximumProperty); }
            set { SetValue(YAxisMaximumProperty, value); }
        }

        public static DependencyProperty XAxisMinimumProperty = DependencyProperty.Register(nameof(XAxisMinimum), typeof(double), typeof(OrderedDataSelectorControl), new UIPropertyMetadata(double.MinValue));
        public double XAxisMinimum
        {
            get { return (double)GetValue(XAxisMinimumProperty); }
            set { SetValue(XAxisMinimumProperty, value); }
        }

        public static DependencyProperty XAxisMaximumProperty = DependencyProperty.Register(nameof(XAxisMaximum), typeof(double), typeof(OrderedDataSelectorControl), new UIPropertyMetadata(double.MaxValue));
        public double XAxisMaximum
        {
            get { return (double)GetValue(XAxisMaximumProperty); }
            set { SetValue(XAxisMaximumProperty, value); }
        }

        public static DependencyProperty XColumnHeaderProperty = DependencyProperty.Register(nameof(XColumnHeader), typeof(string), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata("X Data"));
        public string XColumnHeader
        {
            get { return (string)GetValue(XColumnHeaderProperty); }
            set { SetValue(XColumnHeaderProperty, value); }
        }

        public static DependencyProperty YColumnHeaderProperty = DependencyProperty.Register(nameof(YColumnHeader), typeof(string), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata("Y Data"));
        public string YColumnHeader
        {
            get { return (string)GetValue(YColumnHeaderProperty); }
            set { SetValue(YColumnHeaderProperty, value); }
        }

        public static DependencyProperty IsStrictXProperty = DependencyProperty.Register(nameof(IsStrictX), typeof(bool), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(false));
        public bool IsStrictX
        {
            get { return (bool)GetValue(IsStrictXProperty); }
            set { SetValue(IsStrictXProperty, value); }
        }

        public static DependencyProperty IsStrictYProperty = DependencyProperty.Register(nameof(IsStrictY), typeof(bool), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(false));
        public bool IsStrictY
        {
            get { return (bool)GetValue(IsStrictYProperty); }         
            set { SetValue(IsStrictYProperty, value); }
        }

        public static DependencyProperty OrderXProperty = DependencyProperty.Register(nameof(OrderX), typeof(SortOrder), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(SortOrder.Ascending));
        public SortOrder OrderX
        {
            get { return (SortOrder)GetValue(OrderXProperty); }
            set { SetValue(OrderXProperty, value); }
        }

        public static DependencyProperty OrderYProperty = DependencyProperty.Register(nameof(OrderY), typeof(SortOrder), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(SortOrder.Ascending));
        public SortOrder OrderY
        {
            get { return (SortOrder)GetValue(OrderYProperty); }
            set { SetValue(OrderYProperty, value); }
        }

        public static DependencyProperty MaximumXProperty = DependencyProperty.Register(nameof(MaximumX), typeof(double), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MaxValue));
        public double MaximumX
        {
            get { return (double)GetValue(MaximumXProperty); }
            set { SetValue(MaximumXProperty, value);}
        }

        public static DependencyProperty MinimumXProperty = DependencyProperty.Register(nameof(MinimumX), typeof(double), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MinValue));
        public double MinimumX
        {
            get { return (double)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        public static DependencyProperty MaximumYProperty = DependencyProperty.Register(nameof(MaximumY), typeof(double), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MaxValue));
        public double MaximumY
        {
            get { return (double)GetValue(MaximumYProperty); }
            set { SetValue(MaximumYProperty, value); }
        }

        public static DependencyProperty MinimumYProperty = DependencyProperty.Register(nameof(MinimumY), typeof(double), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(double.MinValue));
        public double MinimumY
        {
            get { return (double)GetValue(MinimumYProperty); }
            set { SetValue(MinimumYProperty, value); }
        }

        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(false));
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static DependencyProperty TableWidthProperty = DependencyProperty.Register(nameof(TableWidth), typeof(GridLength), typeof(OrderedDataSelectorControl), new FrameworkPropertyMetadata(new GridLength(1d, GridUnitType.Star)));
        public GridLength TableWidth
        {
            get { return (GridLength)GetValue(TableWidthProperty); }
            set { SetValue(TableWidthProperty, value); }
        }

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
        /// Get and set the selected probability distribution.
        /// </summary>
        public OrderedPairedData SelectedOrderedData
        {
            get { return (OrderedPairedData)GetValue(SelectedOrderedDataProperty); }
            set { SetValue(SelectedOrderedDataProperty, value); }
        }

        public ObservableCollection<DataPoint> CurveLinePoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<object> CurveRows { get; private set; } = new ObservableCollection<object>();

        public event PlotPropertiesRequestedEventHandler PlotPropertiesRequested;

        public delegate void PlotPropertiesRequestedEventHandler(OxyPlot.Wpf.Plot plotRequestingProperties);

        public OrderedDataSelectorControl()
        {

            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
        }

        public void UpdateGrid()
        {
            foreach (var r in CurveRows)
                ((OrdinateRowItem)r).ForceValidation();
        }

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
