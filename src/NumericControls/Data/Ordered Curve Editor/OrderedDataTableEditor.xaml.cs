using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Numerics.Data;

namespace NumericControls
{
    /// <summary>
    /// Interaction logic for OrderedDataTableEditor.xaml
    /// </summary>
    public partial class OrderedDataTableEditor : UserControl
    {
        public OrderedDataTableEditor()
        {
            InitializeComponent();
        }

        public static DependencyProperty OrderedDataProperty = DependencyProperty.Register(nameof(OrderedData), typeof(OrderedPairedData), typeof(OrderedDataTableEditor), new PropertyMetadata(new OrderedPairedData(false, SortOrder.Ascending, false, SortOrder.Ascending), SetData));

        private static void SetData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(OrderedDataTableEditor)) return;
            OrderedDataTableEditor thisControl = (OrderedDataTableEditor)d;
            if (e.NewValue == null)
            {
                thisControl.CurveRows.Clear();
                return;
            }
            // 
            OrderedPairedData newCurve = e.NewValue as OrderedPairedData;
            if (newCurve == null)
            {
                thisControl.CurveRows.Clear();
                return;
            }
            // Define the data
            thisControl.CurveRows.Clear();
            // 
            OrdinateRowItem rowItem;
            foreach (Ordinate o in newCurve)
            {
                rowItem = new OrdinateRowItem(o.X, o.Y, thisControl.XColumnHeader, thisControl.YColumnHeader, thisControl.CurveRows, thisControl.MinimumX, thisControl.MaximumX, thisControl.MinimumY, thisControl.MaximumY, newCurve.StrictX, newCurve.StrictY, newCurve.OrderX, newCurve.OrderY);
                rowItem.PropertyChanged += thisControl.RowItemPropertyChanged;
                thisControl.CurveRows.Add(rowItem);
                // .ValidationGrid.ItemsSource = .CurveRows
            }
        }

        private void RowItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OrdinateRowItem rItem = (OrdinateRowItem)sender;
            int dataIndex = CurveRows.IndexOf(rItem);
            OrderedData[dataIndex] = rItem.GetOrdinate();
        }

        /// <summary>
        /// Get and set the selected probability distribution.
        /// </summary>
        public OrderedPairedData OrderedData
        {
            get { return (OrderedPairedData)GetValue(OrderedDataProperty); }
            set { SetValue(OrderedDataProperty, value); }
        }

        public static DependencyProperty MaximumXProperty = DependencyProperty.Register(nameof(MaximumX), typeof(double), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata(double.MaxValue));
        public double MaximumX
        {
            get { return (double)GetValue(MaximumXProperty); }
            set { SetValue(MaximumXProperty, value); }
        }

        public static DependencyProperty MinimumXProperty = DependencyProperty.Register(nameof(MinimumX), typeof(double), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata(double.MinValue));
        public double MinimumX
        {
            get { return (double)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        public static DependencyProperty MaximumYProperty = DependencyProperty.Register(nameof(MaximumY), typeof(double), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata(double.MaxValue));
        public double MaximumY
        {
            get { return (double)GetValue(MaximumYProperty); }
            set { SetValue(MaximumYProperty, value); }
        }

        public static DependencyProperty MinimumYProperty = DependencyProperty.Register(nameof(MinimumY), typeof(double), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata(double.MinValue));
        public double MinimumY
        {
            get { return (double)GetValue(MinimumYProperty); }
            set { SetValue(MinimumYProperty, value); }
        }

        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata(false));
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static DependencyProperty XColumnHeaderProperty = DependencyProperty.Register(nameof(XColumnHeader), typeof(string), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata("X Data"));
        public string XColumnHeader
        {
            get { return (string)GetValue(XColumnHeaderProperty); }
            set { SetValue(XColumnHeaderProperty, value); }
        }

        public static DependencyProperty YColumnHeaderProperty = DependencyProperty.Register(nameof(YColumnHeader), typeof(string), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata("Y Data"));
        public string YColumnHeader
        {
            get { return (string)GetValue(YColumnHeaderProperty); }
            set { SetValue(YColumnHeaderProperty, value); }
        }

        public ObservableCollection<object> CurveRows { get; private set; } = new ObservableCollection<object>();

        public void UpdateGrid()
        {
            foreach (var r in CurveRows)
                ((OrdinateRowItem)r).ForceValidation();
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

        private void ValidationGrid_RowsAdded(int startrow, int numrows)
        {
            for (int i = startrow; i < startrow + numrows; i++)
                OrderedData.Insert(i, ((OrdinateRowItem)CurveRows[i]).GetOrdinate());
            UpdateGrid();
        }

        private void ValidationGrid_RowsDeleted(List<int> rowindices)
        {
            // Refresh the source
            rowindices.Sort();
            for (int i = rowindices.Count - 1; i >= 0; i -= 1)
                OrderedData.RemoveAt(rowindices[i]);
            UpdateGrid();
        }

        private void ValidationGrid_DataPasted()
        {
            UpdateGrid();
        }

        private void ValidationGrid_PreviewAddRows(int startRowIndex, int nRows, ref bool cancelAddRows)
        {
            cancelAddRows = true;
            for (int i = startRowIndex; i < startRowIndex + nRows; i++)
            {
                var r = new OrdinateRowItem(0d, 0d, XColumnHeader, YColumnHeader, CurveRows, MinimumX, MaximumX, MinimumY, MaximumY, OrderedData.StrictX, OrderedData.StrictY, OrderedData.OrderX, OrderedData.OrderY);
                r.PropertyChanged += RowItemPropertyChanged;
                CurveRows.Insert(i, r);
            }
            ValidationGrid_RowsAdded(startRowIndex, nRows);
        }
    }
}
