using Numerics.Data;
using Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NumericControls
{
    /// <summary>
    /// Interaction logic for UncertainOrderedTableEditor.xaml
    /// </summary>
    public partial class UncertainOrderedTableEditor : UserControl
    {
        public static DependencyProperty AddRemoveRowsProperty = DependencyProperty.Register(nameof(AddRemoveRows), typeof(bool), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(true, AddRemoveRows_PropertyChanged));
        private static void AddRemoveRows_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UncertainOrderedTableEditor thisControl = (UncertainOrderedTableEditor)d;
            thisControl.ValidationGrid.CanUserAddInsertDeleteRows = thisControl.AddRemoveRows;
            thisControl.ValidationGrid.PasteAddsRows = thisControl.AddRemoveRows;
            thisControl.ValidationGridToolbar.DataGrid = thisControl.ValidationGrid;
        }

        public bool AddRemoveRows
        {
            get { return (bool)GetValue(AddRemoveRowsProperty); }
            set { SetValue(AddRemoveRowsProperty, value); }
        }

        public static DependencyProperty XColumnHeaderProperty = DependencyProperty.Register(nameof(XColumnHeader), typeof(string), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata("X Data"));

        public string XColumnHeader
        {
            get { return (string)GetValue(XColumnHeaderProperty); }
            set { SetValue(XColumnHeaderProperty, value); }
        }

        public static DependencyProperty YColumnHeaderProperty = DependencyProperty.Register(nameof(YColumnHeader), typeof(string), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata("Y Data"));

        public string YColumnHeader
        {
            get { return (string)GetValue(YColumnHeaderProperty); }
            set { SetValue(YColumnHeaderProperty, value); }
        }

        public static DependencyProperty IsStrictXProperty = DependencyProperty.Register(nameof(IsStrictX), typeof(bool), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(false, DP_PropertyChanged));

        public bool IsStrictX
        {
            get { return (bool)GetValue(IsStrictXProperty); }
            set { SetValue(IsStrictXProperty, value); }
        }

        public static DependencyProperty IsStrictYProperty = DependencyProperty.Register(nameof(IsStrictY), typeof(bool), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(false, DP_PropertyChanged));

        public bool IsStrictY
        {
            get { return (bool)GetValue(IsStrictYProperty); }
            set { SetValue(IsStrictYProperty, value); }
        }

        public static DependencyProperty OrderXProperty = DependencyProperty.Register(nameof(OrderX), typeof(SortOrder), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(SortOrder.Ascending, DP_PropertyChanged));

        public SortOrder OrderX
        {
            get { return (SortOrder)GetValue(OrderXProperty); }
            set { SetValue(OrderXProperty, value); }
        }

        public static DependencyProperty OrderYProperty = DependencyProperty.Register(nameof(OrderY), typeof(SortOrder), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(SortOrder.Ascending, DP_PropertyChanged));

        public SortOrder OrderY
        {
            get { return (SortOrder)GetValue(OrderYProperty); }
            set { SetValue(OrderYProperty, value); }
        }

        public static DependencyProperty MaximumXProperty = DependencyProperty.Register(nameof(MaximumX), typeof(double), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(double.MaxValue, DP_PropertyChanged));

        public double MaximumX
        {
            get { return (double)GetValue(MaximumXProperty); }
            set { SetValue(MaximumXProperty, value); }
        }

        public static DependencyProperty MinimumXProperty = DependencyProperty.Register(nameof(MinimumX), typeof(double), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(double.MinValue, DP_PropertyChanged));

        public double MinimumX
        {
            get { return (double)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        public static DependencyProperty MaximumYProperty = DependencyProperty.Register(nameof(MaximumY), typeof(double), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(double.MaxValue, DP_PropertyChanged));

        public double MaximumY
        {
            get { return (double)GetValue(MaximumYProperty); }
            set { SetValue(MaximumYProperty, value); }
        }

        public static DependencyProperty MinimumYProperty = DependencyProperty.Register(nameof(MinimumY), typeof(double), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(double.MinValue, DP_PropertyChanged));

        public double MinimumY
        {
            get { return (double)GetValue(MinimumYProperty); }
            set { SetValue(MinimumYProperty, value); }
        }

        private static void DP_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UncertainOrderedTableEditor thisControl = (UncertainOrderedTableEditor)d;
            thisControl.Refresh();
        }

        public static DependencyProperty IsMergedHeaderVisibleProperty = DependencyProperty.Register(nameof(IsMergedHeaderVisible), typeof(bool), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(true));

        public bool IsMergedHeaderVisible
        {
            get { return (bool)GetValue(IsMergedHeaderVisibleProperty); }
            set { SetValue(IsMergedHeaderVisibleProperty, value); }
        }

        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(false));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static DependencyProperty ShowToolBarProperty = DependencyProperty.Register(nameof(ShowToolBar), typeof(bool), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(false));

        public bool ShowToolBar
        {
            get { return (bool)GetValue(ShowToolBarProperty); }
            set { SetValue(ShowToolBarProperty, value); }
        }

        public static DependencyProperty UncertainOrderedDataProperty = DependencyProperty.Register(nameof(UncertainOrderedData), typeof(UncertainOrderedPairedData), typeof(UncertainOrderedTableEditor), new PropertyMetadata(null, UncertainOrderedData_PropertyChanged));

        private static void UncertainOrderedData_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(UncertainOrderedTableEditor)) return;
            UncertainOrderedTableEditor thisControl = (UncertainOrderedTableEditor)d;
            // 
            if (e.OldValue != null)
            {
                UncertainOrderedPairedData oldValue = e.OldValue as UncertainOrderedPairedData;
                if (oldValue != null) oldValue.CollectionChanged -= thisControl.DataCollectionChanged;
            }

            foreach (DistributionRowItem row in thisControl.DistributionRows) { row.PropertyChanged -= thisControl.RowItem_PropertyChanged; }
            // 
            thisControl.DistributionRows.Clear();
            // 
            if (e.NewValue == null) return;
            UncertainOrderedPairedData newData = e.NewValue as UncertainOrderedPairedData;
            if (newData == null) return;
            newData.CollectionChanged += thisControl.DataCollectionChanged;
            // 
            thisControl.Refresh();
            thisControl.ForceGridValidation();
        }

        /// <summary>
        /// Get and set the selected probability distribution.
        /// </summary>
        public UncertainOrderedPairedData UncertainOrderedData
        {
            get { return (UncertainOrderedPairedData)GetValue(UncertainOrderedDataProperty); }
            set { SetValue(UncertainOrderedDataProperty, value); }
        }

        public static DependencyProperty ColumnHeaderStyleProperty = DependencyProperty.Register(nameof(ColumnHeaderStyle), typeof(Style), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(null));

        public Style ColumnHeaderStyle
        {
            get { return (Style)GetValue(ColumnHeaderStyleProperty); }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }

        public static DependencyProperty CellStyleProperty = DependencyProperty.Register(nameof(CellStyle), typeof(Style), typeof(UncertainOrderedTableEditor), new FrameworkPropertyMetadata(null));

        public Style CellStyle
        {
            get { return (Style)GetValue(CellStyleProperty); }
            set { SetValue(CellStyleProperty, value); }
        }

        public ObservableCollection<object> DistributionRows { get; private set; } = new ObservableCollection<object>();

        private bool _updatingData = false;

        public event ColumnsAutoGeneratedEventHandler ColumnsAutoGenerated;

        public delegate void ColumnsAutoGeneratedEventHandler(object sender, ObservableCollection<DataGridColumn> e);

        public UncertainOrderedTableEditor()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
            ColumnHeaderStyle = (Style)FindResource("WrappedColumnHeaderStyle");

            ValidationGrid.AutoGeneratedColumns += (object sender, EventArgs e) => ColumnsAutoGenerated?.Invoke(sender, ValidationGrid.Columns);
        }

        private void RowItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _updatingData = true;
            DistributionRowItem rItem = (DistributionRowItem)sender;
            int dataIndex = DistributionRows.IndexOf(rItem);
            UncertainOrderedData[dataIndex] = new UncertainOrdinate(rItem.X, rItem.Distribution.Clone());
            _updatingData = false;
        }

        private void DataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_updatingData == true) return;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        DistributionRowItem rowItem;
                        foreach (UncertainOrdinate item in e.NewItems)
                        {
                            int index = UncertainOrderedData.IndexOf(item);
                            rowItem = new DistributionRowItem(item.X, item.Y, DistributionRows, MinimumX, MaximumX, MinimumY, MaximumY, IsStrictX, IsStrictY, OrderX, OrderY);
                            rowItem.PropertyChanged += RowItem_PropertyChanged;
                            // 
                            DistributionRows.Insert(index, rowItem);
                            rowItem.ForceValidation();
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        int startIndex = e.OldStartingIndex;
                        if (startIndex == -1)
                        {
                            Refresh();
                        }
                        else
                        {
                            for (int i = 1; i <= e.OldItems.Count; i++)
                                DistributionRows.RemoveAt(startIndex);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        int index = UncertainOrderedData.IndexOf((UncertainOrdinate)e.NewItems[0]);
                        Refresh(index);
                        break;
                    }

                default:
                    {
                        Refresh();
                        break;
                    }
            }
            // 
        }

        public void Refresh()
        {
            foreach (DistributionRowItem row in DistributionRows) { row.PropertyChanged -= RowItem_PropertyChanged; }
            DistributionRows.Clear();
            // 
            if (UncertainOrderedData == null) return;
            DistributionRowItem rowItem;
            ValidationGrid.ItemsSource = null;
            foreach (var o in UncertainOrderedData)
            {
                rowItem = new DistributionRowItem(o.X, o.Y, DistributionRows, MinimumX, MaximumX, MinimumY, MaximumY, IsStrictX, IsStrictY, OrderX, OrderY);
                rowItem.PropertyChanged += RowItem_PropertyChanged;
                // 
                DistributionRows.Add(rowItem);
            }

            ValidationGrid.ItemsSource = DistributionRows;
        }

        public void Refresh(int rowIndex) // minX As Double, maxX As Double, minY As Double, maxY As Double, strictX As Boolean, strictY As Boolean, orderX As SortOrder, orderY As SortOrder)
        {
            if (UncertainOrderedData == null) return;
            if ((rowIndex >= UncertainOrderedData.Count) || (rowIndex < 0)) return;
            ((DistributionRowItem)DistributionRows[rowIndex]).PropertyChanged -= RowItem_PropertyChanged;
            // 
            var rowItem = new DistributionRowItem(UncertainOrderedData[rowIndex].X, UncertainOrderedData[rowIndex].Y, DistributionRows, MinimumX, MaximumX, MinimumY, MaximumY, IsStrictX, IsStrictY, OrderX, OrderY);
            rowItem.PropertyChanged += RowItem_PropertyChanged;
            DistributionRows[rowIndex] = rowItem;
            rowItem.ForceValidation();
        }

        public void ForceGridValidation()
        {
            foreach (var r in DistributionRows) { ((DistributionRowItem)r).ForceValidation(); }
        }

        private void DataGridColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            DataGridColumnHeader columnHeader = sender as DataGridColumnHeader;
            if (columnHeader == null) { return; }
            // 
            ValidationGrid.SelectedCells.Clear();
            foreach (var item in ValidationGrid.Items) { ValidationGrid.SelectedCells.Add(new DataGridCellInfo(item, columnHeader.Column)); }
        }

        private void ValidationGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            MergedColumnHeaderGrid.ColumnDefinitions.Clear();
            var cd = new ColumnDefinition();
            cd.SetBinding(ColumnDefinition.WidthProperty, new Binding(nameof(DataGrid.RowHeaderActualWidth)) { Source = ValidationGrid });
            MergedColumnHeaderGrid.ColumnDefinitions.Add(cd);
            foreach (var c in ValidationGrid.Columns)
            {
                cd = new ColumnDefinition();
                cd.SetBinding(ColumnDefinition.WidthProperty, new Binding(nameof(DataGridColumn.ActualWidth)) { Source = c });
                MergedColumnHeaderGrid.ColumnDefinitions.Add(cd);
                // 
                if (c.HeaderStyle == null) c.HeaderStyle = ColumnHeaderStyle;
            }
            // 
            if (XGridColumnHeader.Style == null) XGridColumnHeader.Style = ColumnHeaderStyle;
            if (YGridColumnHeader.Style == null) YGridColumnHeader.Style = ColumnHeaderStyle;
            // 
            // RaiseEvent ColumnsAutoGenerated(sender, ValidationGrid.Columns)
            if (IsMergedHeaderVisible == false)
            {
                MergedColumnHeaderGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                MergedColumnHeaderGrid.Visibility = Visibility.Visible;
            }
        }

        private void ValidationGrid_PreviewAddRows(int startRowIndex, int nRows, ref bool cancelAddRows)
        {
            cancelAddRows = true;
            // 
            var type = UncertainOrderedData.Distribution;
            var itemsToInsert = new List<UncertainOrdinate>();
            for (int i = startRowIndex; i < startRowIndex + nRows; i++)
                itemsToInsert.Add(new UncertainOrdinate(0d, UnivariateDistributionFactory.CreateDistribution(type)));
            // Let it update the UI since this is a preview event that is being canceled.
            if (UncertainOrderedData.SupressCollectionChanged == true)
            {
                UncertainOrderedData.SupressCollectionChanged = false;
                UncertainOrderedData.InsertRange(startRowIndex, itemsToInsert);
                UncertainOrderedData.SupressCollectionChanged = true;
            }
            else
            {
                UncertainOrderedData.InsertRange(startRowIndex, itemsToInsert);
            }
        }

        private void ValidationGrid_PreviewDeleteRows(List<int> rowindices, ref bool cancel)
        {
            cancel = true;
            // Let it update the UI since this is a preview event that is being canceled.
            if (UncertainOrderedData.SupressCollectionChanged == true)
            {
                UncertainOrderedData.SupressCollectionChanged = false;
                UncertainOrderedData.RemoveRange(rowindices.ToArray());
                UncertainOrderedData.SupressCollectionChanged = true;
            }
            else
            {
                UncertainOrderedData.RemoveRange(rowindices.ToArray());
            }
            //
            ForceGridValidation();
            Mouse.OverrideCursor = null;
        }

        private void ValidationGrid_PreviewPasteData(string[][] clipboardData, ref bool cancelPaste)
        {
            UncertainOrderedData.SupressCollectionChanged = true;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        private void ValidationGrid_DataPasted()
        {
            UncertainOrderedData.SupressCollectionChanged = false;
            UncertainOrderedData.Validate();
            UncertainOrderedData.RaiseCollectionChangedReset();
            if (UncertainOrderedData.Distribution == UnivariateDistributionType.PertPercentile || UncertainOrderedData.Distribution == UnivariateDistributionType.PertPercentileZ) { Refresh(); }
            ForceGridValidation();
            Mouse.OverrideCursor = null;
        }

        private void ValidationGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Exit if not clicked in empty space below bottom row.
            if (e.OriginalSource.GetType() != typeof(ScrollViewer)) { return; }
            //Commit edits
            ValidationGrid.CommitEdit();
            //Clear Selection
            ValidationGrid.UnselectAllCells();
            //Move focus to force validation redraw
            ValidationGrid.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
