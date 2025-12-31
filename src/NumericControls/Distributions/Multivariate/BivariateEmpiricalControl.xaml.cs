using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Numerics.Distributions;
using OxyPlot.Wpf;

namespace NumericControls
{
    /// <summary>
    /// Interaction logic for BivariateEmpiricalControl.xaml
    /// </summary>
    public partial class BivariateEmpiricalControl : UserControl
    {
        public BivariateEmpiricalControl()
        {

            // Toolbar buttons
            _addColumnButton = new Button() { ToolTip = "Add Column(s) to the Right of the Table" };
            _insertColumnButton = new Button() { ToolTip = "Insert Column(s) into the Table", IsEnabled = false };
            _deleteColumnButton = new Button() { ToolTip = "Delete Column(s) from the Table", IsEnabled = false };
            // Context Menu Items
            _addColumnCMI = new MenuItem() { Header = "Add Column(s)" };
            _insertColumnCMI = new MenuItem() { Header = "Insert Column(s)" };
            _deleteColumnCMI = new MenuItem() { Header = "Delete Column(s)" };

            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            X1HeaderBorder.Style = DefaultX1HeaderBorderStyle();
            X1HeaderTextBlock.Style = DefaultX1HeaderTextBlockStyle();
            // 
            X2HeaderBorder.Style = DefaultX2HeaderBorderStyle();
            X2HeaderTextBlock.Style = DefaultX2HeaderTextBlockStyle();
            // 
            _dt.ColumnChanged += TableColumnChanged;
            // AddHandler _dt.RowChanged, AddressOf TableRowChanged
            _dt.TableNewRow += TableNewRowAdded;
            // 
            // Column buttons
            // 
            // Add
            _addColumnButton.Click += AddColumns_Click;
            var addImage = new Image() { Margin = new Thickness(1d), Stretch = Stretch.Uniform, Source = GenericControls.GeneralMethods.Bitmap2BitmapSource(Properties.Resources.add_column) };
            RenderOptions.SetEdgeMode(addImage, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(addImage, BitmapScalingMode.HighQuality);
            _addColumnButton.Content = addImage;
            // Insert
            _insertColumnButton.Click += InsertColumns_Click;
            var insertImage = new Image() { Margin = new Thickness(1d), Stretch = Stretch.Uniform, Source = GenericControls.GeneralMethods.Bitmap2BitmapSource(Properties.Resources.insert_column) };
            RenderOptions.SetEdgeMode(insertImage, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(insertImage, BitmapScalingMode.HighQuality);
            _insertColumnButton.Content = insertImage;
            // Delete
            _deleteColumnButton.Click += DeleteColumns_Click;
            var deleteImage = new Image() { Margin = new Thickness(1d), Stretch = Stretch.Uniform, Source = GenericControls.GeneralMethods.Bitmap2BitmapSource(Properties.Resources.delete_column) };
            RenderOptions.SetEdgeMode(deleteImage, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(deleteImage, BitmapScalingMode.HighQuality);
            _deleteColumnButton.Content = deleteImage;
            // 
            // Column Context Items
            // 
            // Add
            _addColumnCMI.Icon = new Image() { Source = GenericControls.GeneralMethods.Bitmap2BitmapSource(Properties.Resources.add_column) };
            _addColumnCMI.Click += AddColumns_Click;
            // Insert 
            _insertColumnCMI.Icon = new Image() { Source = GenericControls.GeneralMethods.Bitmap2BitmapSource(Properties.Resources.insert_column) };
            _insertColumnCMI.Click += InsertColumns_Click;
            // Delete 
            _deleteColumnCMI.Icon = new Image() { Source = GenericControls.GeneralMethods.Bitmap2BitmapSource(Properties.Resources.delete_column) };
            _deleteColumnCMI.Click += DeleteColumns_Click;
        }

        /// <summary>
        /// Dependency property for the control distribution options.
        /// </summary>
        public static DependencyProperty BivariateCDFProperty = DependencyProperty.Register(nameof(BivariateCDF), typeof(BivariateEmpirical), typeof(BivariateEmpiricalControl), new PropertyMetadata(null, BivariateCDFPropertyCallback));

        private static void BivariateCDFPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BivariateEmpiricalControl thisControl = (BivariateEmpiricalControl)d;
            // 
            if (e.OldValue != null)
            {
            }
            // 
            thisControl._dt.Clear();
            thisControl._dt.Columns.Clear();
            if (e.NewValue != null)
            {
                BivariateEmpirical newBivariate = e.NewValue as BivariateEmpirical;
                // 
                if (newBivariate.X1Values == null) return;
                if (newBivariate.X2Values == null) return;
                for (int i = 0; i <= newBivariate.X2Values.Count(); i++)
                    thisControl._dt.Columns.Add(new DataColumn($"Column{i}", typeof(double)));

                // Top Row (X2 values)
                var topRow = new object[newBivariate.X2Values.Count() + 1];
                for (int i = 1; i <= newBivariate.X2Values.Count(); i++)
                    topRow[i] = newBivariate.X2Values[i - 1];
                // Buffer.BlockCopy(newBivariate.XValues, 0, topRow, 1, newBivariate.XValues.Count)
                thisControl._dt.Rows.Add(topRow);

                // Each Row(X1 values and probabilities)
                var nextRow = new object[(topRow.Count())];
                for (int i = 0; i < newBivariate.X1Values.Count(); i++)
                {
                    nextRow[0] = newBivariate.X1Values[i];
                    for (int j = 0; j < newBivariate.X2Values.Count(); j++)
                        nextRow[j + 1] = newBivariate.ProbabilityValues[i, j];
                    // 
                    thisControl._dt.Rows.Add(nextRow);
                }
            }
            // 
            thisControl.BivariateCDFDataGrid.ItemsSource = null;
            thisControl.BivariateCDFDataGrid.ItemsSource = thisControl._dt.DefaultView;
            thisControl.UpdatePlot();
        }

        /// <summary>
        /// Gets and sets the bivariate empirical CDF.
        /// </summary>
        public BivariateEmpirical BivariateCDF
        {
            get { return (BivariateEmpirical)GetValue(BivariateCDFProperty); }
            set { SetValue(BivariateCDFProperty, value); }
        }

        private static Style DefaultX2HeaderBorderStyle()
        {
            var s = new Style(typeof(Border));
            s.Setters.Add(new Setter(Border.BackgroundProperty, SystemColors.ControlBrush));
            s.Setters.Add(new Setter(Border.BorderBrushProperty, new SolidColorBrush(Color.FromArgb(255, 104, 140, 175))));
            s.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1d, 1d, 1d, 0d)));
            s.Setters.Add(new Setter(UIElement.IsHitTestVisibleProperty, false));
            return s;
        }

        private static Style DefaultX2HeaderTextBlockStyle()
        {
            var s = new Style(typeof(TextBlock));
            s.Setters.Add(new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            s.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis));
            s.Setters.Add(new Setter(TextBlock.BackgroundProperty, Brushes.Transparent));
            s.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Black));
            s.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(2d)));
            s.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
            // s.Setters.Add(New Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWithOverflow))
            s.Setters.Add(new Setter(UIElement.IsHitTestVisibleProperty, false));
            return s;
        }

        private static Style DefaultX1HeaderBorderStyle()
        {
            var s = new Style(typeof(Border));
            s.Setters.Add(new Setter(Border.BackgroundProperty, SystemColors.ControlBrush));
            s.Setters.Add(new Setter(Border.BorderBrushProperty, new SolidColorBrush(Color.FromArgb(255, 104, 140, 175))));
            s.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1d, 1d, 0d, 1d)));
            s.Setters.Add(new Setter(UIElement.IsHitTestVisibleProperty, false));
            return s;
        }

        private static Style DefaultX1HeaderTextBlockStyle()
        {
            var s = new Style(typeof(TextBlock));
            s.Setters.Add(new Setter(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            s.Setters.Add(new Setter(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis));
            s.Setters.Add(new Setter(TextBlock.BackgroundProperty, Brushes.Transparent));
            s.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Black));
            s.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(2d)));
            s.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
            // s.Setters.Add(New Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWithOverflow))
            s.Setters.Add(new Setter(UIElement.IsHitTestVisibleProperty, false));
            // Vertical text
            var verticalTextTransform = new RotateTransform(270d);
            s.Setters.Add(new Setter(FrameworkElement.LayoutTransformProperty, verticalTextTransform));
            return s;
        }

        public static DependencyProperty X2HeaderBorderStyleProperty = DependencyProperty.Register(nameof(X2HeaderBorderStyle), typeof(Style), typeof(BivariateEmpiricalControl), new UIPropertyMetadata(DefaultX2HeaderBorderStyle(), X2HeaderBorderStylePropertyCallback));

        private static void X2HeaderBorderStylePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BivariateEmpiricalControl bvc = (BivariateEmpiricalControl)d;
            bvc.X2HeaderBorder.Style = e.NewValue as Style;
        }

        public Style X2HeaderBorderStyle
        {
            get { return (Style)GetValue(X2HeaderBorderStyleProperty); }
            set { SetValue(X2HeaderBorderStyleProperty, value); }
        }

        public static DependencyProperty X2HeaderTextBlockStyleProperty = DependencyProperty.Register(nameof(X2HeaderTextBlockStyle), typeof(Style), typeof(BivariateEmpiricalControl), new UIPropertyMetadata(DefaultX2HeaderTextBlockStyle(), X2HeaderTextBlockStylePropertyCallback));

        private static void X2HeaderTextBlockStylePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BivariateEmpiricalControl bvc = (BivariateEmpiricalControl)d;
            bvc.X2HeaderTextBlock.Style = e.NewValue as Style;
        }

        public Style X2HeaderTextBlockStyle
        {
            get { return (Style)GetValue(X2HeaderTextBlockStyleProperty); }
            set { SetValue(X2HeaderTextBlockStyleProperty, value); }
        }

        public static DependencyProperty X1HeaderBorderStyleProperty = DependencyProperty.Register(nameof(X1HeaderBorderStyle), typeof(Style), typeof(BivariateEmpiricalControl), new UIPropertyMetadata(DefaultX1HeaderBorderStyle(), X1HeaderBorderStylePropertyCallback));

        private static void X1HeaderBorderStylePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BivariateEmpiricalControl bvc = (BivariateEmpiricalControl)d;
            bvc.X1HeaderBorder.Style = e.NewValue as Style;
        }

        public Style X1HeaderBorderStyle
        {
            get { return (Style)GetValue(X1HeaderBorderStyleProperty); }
            set { SetValue(X1HeaderBorderStyleProperty, value); }
        }

        public static DependencyProperty X1HeaderTextBlockStyleProperty = DependencyProperty.Register(nameof(X1HeaderTextBlockStyle), typeof(Style), typeof(BivariateEmpiricalControl), new UIPropertyMetadata(DefaultX1HeaderTextBlockStyle(), X1HeaderTextBlockStylePropertyCallback));

        private static void X1HeaderTextBlockStylePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BivariateEmpiricalControl bvc = (BivariateEmpiricalControl)d;
            bvc.X1HeaderTextBlock.Style = e.NewValue as Style;
        }

        public Style X1HeaderTextBlockStyle
        {
            get { return (Style)GetValue(X1HeaderTextBlockStyleProperty); }
            set { SetValue(X1HeaderTextBlockStyleProperty, value); }
        }

        public static DependencyProperty X2HeaderProperty = DependencyProperty.Register(nameof(X2Header), typeof(string), typeof(BivariateEmpiricalControl), new FrameworkPropertyMetadata("X2 Data"));

        public string X2Header
        {
            get { return (string)GetValue(X2HeaderProperty); }
            set { SetValue(X2HeaderProperty, value); }
        }

        public static DependencyProperty X1HeaderProperty = DependencyProperty.Register(nameof(X1Header), typeof(string), typeof(BivariateEmpiricalControl), new FrameworkPropertyMetadata("X1 Data"));

        public string X1Header
        {
            get { return (string)GetValue(X1HeaderProperty); }
            set { SetValue(X1HeaderProperty, value); }
        }

        private bool _isLoaded = false;
        private DataTable _dt = new DataTable();
        private bool _pastingData = false;
        private bool _addingRows = false;
        private bool _addingColumns = false;
        private Button _addColumnButton;
        private Button _insertColumnButton;
        private Button _deleteColumnButton;
        private MenuItem _addColumnCMI;
        private MenuItem _insertColumnCMI;
        private MenuItem _deleteColumnCMI;

        private void BivariateEmpericalCDFControl_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshDataGridView();
            if (_isLoaded == false)
            {
                GridToolbar.CustomButtons.Add(_addColumnButton);
                GridToolbar.CustomButtons.Add(_insertColumnButton);
                GridToolbar.CustomButtons.Add(_deleteColumnButton);
                BivariateCDFDataGrid.CustomMenuItems.Add(_addColumnCMI);
                BivariateCDFDataGrid.CustomMenuItems.Add(_insertColumnCMI);
                BivariateCDFDataGrid.CustomMenuItems.Add(_deleteColumnCMI);
            }
            _isLoaded = true;
        }

        private void TableNewRowAdded(object sender, DataTableNewRowEventArgs e)
        {
            // This is needed to set the new row data values to zero.
            for (int i = 0; i < e.Row.ItemArray.Count(); i++)
                if (e.Row[i] == DBNull.Value) e.Row[i] = 0;
        }

        private void TableColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (_pastingData == true) return;
            if (_addingRows == true) return;
            if (_addingColumns == true) return;
            int columnIndex = e.Column.Ordinal;
            int rowIndex = _dt.Rows.IndexOf(e.Row);
            ValidateData(rowIndex, columnIndex);
            UpdateBivariateCDF();
            UpdatePlot();
        }

        private void AddColumns_Click(object sender, RoutedEventArgs e)
        {
            if (BivariateCDFDataGrid == null) return;
            if (BivariateCDFDataGrid.CanUserAddInsertDeleteRows)
            {
                var uniqueColumns = BivariateCDFDataGrid.GetColumnsWithSelectedCells();
                AddColumns(Math.Max(uniqueColumns.Count, 1));
            }
        }

        private void InsertColumns_Click(object sender, RoutedEventArgs e)
        {
            if (BivariateCDFDataGrid == null) return;
            if (BivariateCDFDataGrid.CanUserAddInsertDeleteRows)
            {
                var uniqueColumns = BivariateCDFDataGrid.GetColumnsWithSelectedCells();
                int insertAtColumn = Math.Max(uniqueColumns.Min(), 1); // ensure that the first column (y values) doesn't get pushed.
                AddColumns(Math.Max(uniqueColumns.Count, 1), insertAtColumn);
            }
        }

        private void DeleteColumns_Click(object sender, RoutedEventArgs e)
        {
            if (BivariateCDFDataGrid == null) return;
            if (BivariateCDFDataGrid.CanUserAddInsertDeleteRows)
            {
                var uniqueColumns = BivariateCDFDataGrid.GetColumnsWithSelectedCells().ToList();
                uniqueColumns.Sort();
                for (int i = uniqueColumns.Count - 1; i >= 0; i -= 1)
                    _dt.Columns.RemoveAt(uniqueColumns[i]);
                // reset column names
                ResetColumnNames();
            }
            // Refresh DataGrid source
            BivariateCDFDataGrid.ItemsSource = null;
            BivariateCDFDataGrid.ItemsSource = _dt.DefaultView;
            BivariateCDFDataGrid.Focus();
            RefreshDataGridView();
            ValidateData();
            UpdateBivariateCDF();
            UpdatePlot();
        }

        private void AddColumns(int nColumnsToAdd, int insertIndex = -1)
        {
            _addingColumns = true;
            DataColumn newColumn;
            if (insertIndex == -1)
            {
                for (int i = 1; i < nColumnsToAdd; i++)
                {
                    if (_dt.Columns.Count >= 50) break;
                    newColumn = _dt.Columns.Add($"Column{_dt.Columns.Count}", typeof(double));
                    for (int j = 0; j < _dt.Rows.Count; j++)
                        _dt.Rows[j][newColumn] = 0;
                }
            }
            else
            {
                for (int i = 1; i <= nColumnsToAdd; i++)
                {
                    if (_dt.Columns.Count >= 50) break;
                    newColumn = _dt.Columns.Add($"Column{_dt.Columns.Count}", typeof(double));
                    newColumn.SetOrdinal(insertIndex);
                    for (int j = 0; j < _dt.Rows.Count; j++)
                        _dt.Rows[j][newColumn] = 0;
                }
                // Reset column names 
                ResetColumnNames();
            }
            // Refresh DataGrid source
            BivariateCDFDataGrid.ItemsSource = null;
            BivariateCDFDataGrid.ItemsSource = _dt.DefaultView;
            // 
            BivariateCDFDataGrid.Focus();
            RefreshDataGridView();
            ValidateData();
            _addingColumns = false;
            UpdateBivariateCDF();
            UpdatePlot();
        }

        private void ResetColumnNames()
        {
            // First set all column names to temporary names
            for (int i = 0; i < _dt.Columns.Count; i++)
                _dt.Columns[i].ColumnName = $"Temp{i}";
            // Reset column names
            for (int i = 0; i < _dt.Columns.Count; i++)
                _dt.Columns[i].ColumnName = $"Column{i}";
        }

        private void BivariateCDFDataGrid_PreviewAddRows(int startRowIndex, int nRows, ref bool cancelAddRows)
        {
            _addingRows = true;
            if (startRowIndex > 0) return;
            // 
            _addingRows = false;
            cancelAddRows = true;
            if (startRowIndex < 0) return;
            // 
            if (_dt.Rows.Count == 1)
            {
                BivariateCDFDataGrid.AddRows(nRows);
            }
            else
            {
                BivariateCDFDataGrid.InsertRows(startRowIndex + 1, nRows);
            }
        }

        private void BivariateCDFDataGrid_RowsAdded(int startRowIndex, int nRows)
        {
            // RefreshDataGridView()
            for (int i = startRowIndex; i < startRowIndex + nRows; i++)
                ValidateData(i, -1);
            _addingRows = false;
            if (_pastingData == false) UpdateBivariateCDF();
        }

        private void BivariateCDFDataGrid_PreviewDeleteRows(List<int> rowindices, ref bool cancel)
        {
            if (rowindices == null) return;
            // Ensure first row does not get deleted.
            int zeroIndex = rowindices.IndexOf(0);
            if (zeroIndex >= 0) rowindices.RemoveAt(zeroIndex);
        }

        private void BivariateCDFDataGrid_RowsDeleted(List<int> rowindices)
        {
            ValidateData();
            UpdateBivariateCDF();
            UpdatePlot();
        }

        private void BivariateCDFDataGrid_PreviewPasteData(string[][] clipboardData, ref bool cancelPaste)
        {
            _pastingData = true;
            // Check to ensure cell(0,0) doesn't get pasted into
            int rowIndex, columnIndex;
            foreach (DataGridCellInfo c in BivariateCDFDataGrid.SelectedCells)
            {
                if (c.Column.DisplayIndex != 0) continue;
                rowIndex = BivariateCDFDataGrid.Items.IndexOf(c.Item);
                if (rowIndex != 0) continue;
                // 
                clipboardData[0][0] = "0";
                break;
            }
            if (BivariateCDFDataGrid.SelectedCells.Count != 1) return;

            // fill beyond selected cell
            var cellinfo = BivariateCDFDataGrid.SelectedCells[0];
            rowIndex = BivariateCDFDataGrid.Items.IndexOf(cellinfo.Item);
            columnIndex = cellinfo.Column.DisplayIndex;
            DataColumn newColumn;
            if (columnIndex + clipboardData[0].Count() > BivariateCDFDataGrid.Columns.Count - 1)
            {
                for (int i = 1; i <= clipboardData[0].Count() - (BivariateCDFDataGrid.Columns.Count - columnIndex); i++)
                {
                    newColumn = _dt.Columns.Add($"Column{_dt.Columns.Count}", typeof(double));
                    for (int j = 0; j < _dt.Rows.Count; j++)
                        _dt.Rows[j][newColumn] = 0;
                }
                BivariateCDFDataGrid.ItemsSource = null;
                BivariateCDFDataGrid.ItemsSource = _dt.DefaultView;
                BivariateCDFDataGrid.SelectedCells.Add(new DataGridCellInfo(BivariateCDFDataGrid.GetCell(rowIndex, columnIndex)));
            } 
        }

        private void BivariateCDFDataGrid_DataPasted()
        {
            RefreshDataGridView();
            ValidateData();
            _pastingData = false;
            UpdateBivariateCDF();
            UpdatePlot();
        }

        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
        private void ValidateData(int rowIndex = -1, int columnIndex = -1)
        {
            // 
            // If IsNothing(BivariateCDFDataGrid.ItemsSource) Then Exit Sub
            // Debug.Print(DirectCast(BivariateCDFDataGrid.ItemsSource, DataView).Table.Rows.Count.ToString)
            // Debug.Print(_dt.Rows.Count.ToString)
            // Debug.Print(BivariateCDFDataGrid.Items.Count.ToString)
            // BivariateCDFDataGrid.CommitEdit(DataGridEditingUnit.Cell, True)
            if (_dt.Rows.Count == 0)
                return;
            if (_dt.Columns.Count == 0)
                return;
            if (rowIndex == 0 && columnIndex == 0)
                return;
            // 
            ValidateX1Values(rowIndex, columnIndex);
            ValidateX2Values(rowIndex, columnIndex);       
            ValidateProbabilities(rowIndex, columnIndex);
        }

        private void ValidateX2Values(int rowIndex = -1, int columnIndex = -1)
        {
            if (rowIndex > 0)
                return;
            // Get tooltip messages
            string header = string.IsNullOrEmpty(X2Header) == true ? "X2 values" : $"'{X2Header}'";
            string tooltipMessage = $"{header} must be in ascending order from left to right.";
            // 
            DataGridCell cell;
            double previousValue, currentValue;
            if (columnIndex == -1)
            {
                for (int i = 2; i < _dt.Columns.Count; i++)
                {
                    cell = BivariateCDFDataGrid.GetCell(0, i);
                    if (cell == null) continue;
                    // 
                    previousValue = _dt.Rows[0][i - 1] == DBNull.Value ? 0d : (double)_dt.Rows[0][i - 1];
                    currentValue = _dt.Rows[0][i] == DBNull.Value ? 0d : (double)_dt.Rows[0][i];
                    // 
                    SetCellValidation(cell, previousValue <= currentValue, tooltipMessage);
                }
            }
            else
            {
                if (columnIndex == 0) return;
                if (columnIndex >= _dt.Columns.Count) return;
                // 
                bool checkCurrent = true;
                bool checkNext = true;
                // 
                if (columnIndex == 1) checkCurrent = false;
                if (columnIndex + 1 >= _dt.Columns.Count) checkNext = false;
                // 
                if (checkCurrent == true)
                {
                    cell = BivariateCDFDataGrid.GetCell(0, columnIndex);
                    if (cell != null)
                    {
                        previousValue = _dt.Rows[0][columnIndex - 1] == DBNull.Value ? 0d : (double)_dt.Rows[0][columnIndex - 1];
                        currentValue = _dt.Rows[0][columnIndex] == DBNull.Value ? 0d : (double)_dt.Rows[0][columnIndex];
                        SetCellValidation(cell, previousValue <= currentValue, tooltipMessage);
                    }
                }
                // 
                if (checkNext == true)
                {
                    cell = BivariateCDFDataGrid.GetCell(0, columnIndex + 1);
                    if (cell != null)
                    {
                        previousValue = _dt.Rows[0][columnIndex] == DBNull.Value ? 0d : (double)_dt.Rows[0][columnIndex];
                        currentValue = _dt.Rows[0][columnIndex + 1] == DBNull.Value ? 0d : (double)_dt.Rows[0][columnIndex + 1];
                        SetCellValidation(cell, previousValue <= currentValue, tooltipMessage);
                    }
                }
            }
        }

        private void ValidateX1Values(int rowIndex = -1, int columnIndex = -1)
        {
            if (columnIndex > 0) return;
            // Get tooltip messages
            string header = string.IsNullOrEmpty(X1Header) == true ? "X1 values" : $"'{X1Header}'";
            string tooltipMessage = $"{header} must be in ascending order from top to bottom.";
            // 
            DataGridCell cell;
            double previousValue, currentValue;
            if (rowIndex == -1)
            {
                for (int i = 2; i < _dt.Rows.Count; i++)
                {
                    cell = BivariateCDFDataGrid.GetCell(i, 0);
                    if (cell == null) continue;
                    previousValue = _dt.Rows[i - 1][0] == DBNull.Value ? 0d : (double)_dt.Rows[i - 1][0];
                    currentValue = _dt.Rows[i][0] == DBNull.Value ? 0d : (double)_dt.Rows[i][0];
                    SetCellValidation(cell, previousValue <= currentValue, tooltipMessage, true);
                }
            }
            else
            {
                if (rowIndex == 0) return;
                if (rowIndex >= _dt.Rows.Count) return;
                // 
                bool checkCurrent = true;
                bool checkNext = true;
                // 
                if (rowIndex == 1) checkCurrent = false;
                if (rowIndex + 1 >= _dt.Rows.Count) checkNext = false;
                // 
                if (checkCurrent == true)
                {
                    cell = BivariateCDFDataGrid.GetCell(rowIndex, 0);
                    if (cell != null)
                    {
                        previousValue = _dt.Rows[rowIndex - 1][0] == DBNull.Value ? 0d : (double)_dt.Rows[rowIndex - 1][0];
                        currentValue = _dt.Rows[rowIndex][0] == DBNull.Value ? 0d : (double)_dt.Rows[rowIndex][0];
                        SetCellValidation(cell, previousValue <= currentValue, tooltipMessage, true);
                    }
                }
                // 
                if (checkNext == true)
                {
                    cell = BivariateCDFDataGrid.GetCell(rowIndex + 1, 0);
                    if (cell != null)
                    {
                        previousValue = _dt.Rows[rowIndex][0] == DBNull.Value ? 0d : (double)_dt.Rows[rowIndex][0];
                        currentValue = _dt.Rows[rowIndex + 1][0] == DBNull.Value ? 0d : (double)_dt.Rows[rowIndex + 1][0];
                        SetCellValidation(cell, previousValue <= currentValue, tooltipMessage, true);
                    }
                }
            }
        }

        private void ValidateProbabilities(int rowIndex = -1, int columnIndex = -1)
        {
            if (columnIndex == 0 || rowIndex == 0)
                return;
            // Get tooltip messages
            string rangeTooltipMessage = "Probability values must be between 0 and 1.";
            string headerX1 = string.IsNullOrEmpty(X1Header) == true ? "X1 values" : $"'{X1Header}'";
            string ascendingX1TooltipMessage = $"Probability values must increase with increasing {headerX1}.";
            //
            string headerX2 = string.IsNullOrEmpty(X2Header) == true ? "X2 values" : $"'{X2Header}'";
            string ascendingX2TooltipMessage = $"Probability values must increase with increasing {headerX2}.";
            // Validate
            if (rowIndex == -1 && columnIndex == -1)
            {
                // validate all cells
                for (int i = 1; i < _dt.Columns.Count; i++)
                {
                    for (int j = 1; j < _dt.Rows.Count; j++)
                        ValidateProbability(j, i, rangeTooltipMessage, ascendingX1TooltipMessage, ascendingX2TooltipMessage);
                }
            }
            else if (rowIndex == -1 && columnIndex != -1)
            {
                if (columnIndex >= _dt.Rows.Count) return;
                // validate entire column
                for (int j = 1; j < _dt.Rows.Count; j++)
                {
                    ValidateProbability(j, columnIndex, rangeTooltipMessage, ascendingX1TooltipMessage, ascendingX2TooltipMessage);
                    ValidateProbability(j, columnIndex + 1, rangeTooltipMessage, ascendingX1TooltipMessage, ascendingX2TooltipMessage);
                }
            }
            else if (rowIndex != -1 && columnIndex == -1)
            {
                if (rowIndex >= _dt.Rows.Count) return;
                // validate entire row
                for (int i = 1; i < _dt.Columns.Count; i++)
                {
                    ValidateProbability(rowIndex, i, rangeTooltipMessage, ascendingX1TooltipMessage, ascendingX2TooltipMessage);
                    ValidateProbability(rowIndex + 1, i, rangeTooltipMessage, ascendingX1TooltipMessage, ascendingX2TooltipMessage);
                }
            }
            else
            {
                ValidateProbability(rowIndex, columnIndex, rangeTooltipMessage, ascendingX1TooltipMessage, ascendingX2TooltipMessage);
                ValidateProbability(rowIndex + 1, columnIndex, rangeTooltipMessage, ascendingX1TooltipMessage, ascendingX2TooltipMessage);
                ValidateProbability(rowIndex, columnIndex + 1, rangeTooltipMessage, ascendingX1TooltipMessage, ascendingX2TooltipMessage);
            }
        }

        private void ValidateProbability(int rowIndex, int columnIndex, string rangeMessage, string ascendingYMessage, string ascendingXMessage)
        {
            var cell = BivariateCDFDataGrid.GetCell(rowIndex, columnIndex);
            if (cell != null)
            {
                double currentValue, previousTopValue, previousLeftValue;
                // validate probability
                currentValue = _dt.Rows[rowIndex][columnIndex] == DBNull.Value ? -1 : (double)_dt.Rows[rowIndex][columnIndex];
                SetCellValidation(cell, currentValue <= 1d && currentValue >= 0d, rangeMessage);
                // 
                if (rowIndex > 1)
                {
                    previousTopValue = _dt.Rows[rowIndex - 1][columnIndex] == DBNull.Value ? 0d : (double)_dt.Rows[rowIndex - 1][columnIndex];
                    SetCellValidation(cell, previousTopValue <= currentValue, ascendingYMessage);
                }
                if (columnIndex > 1)
                {
                    previousLeftValue = _dt.Rows[rowIndex][columnIndex - 1] == DBNull.Value ? -1 : (double)_dt.Rows[rowIndex][columnIndex - 1];
                    SetCellValidation(cell, previousLeftValue <= currentValue, ascendingXMessage);
                }
            }
        }

        private void SetCellValidation(DataGridCell cell, bool isValid, string tooltip, bool isY = false)
        {
            // Setting the background colors and borders is a real pain with this control. The first column has to be handled differently to render reasonably well. 
            // I could not figure out how to get the first column to look consistent with the other columns so I am leaving the border off for the first column...it's very annoying. 
            if (isValid == false)
            {
                cell.Background = new SolidColorBrush(Color.FromArgb(255, 247, 182, 175));
                if (isY == false)
                {
                    cell.BorderThickness = new Thickness(1d);
                    cell.BorderBrush = Brushes.Red;
                    cell.Margin = new Thickness(0d);
                }
                else
                {
                    cell.Padding = new Thickness(1d);
                }
                cell.ToolTip = tooltip;
            }
            else
            {
                cell.ClearValue(Control.BorderThicknessProperty);
                cell.ClearValue(Control.BorderBrushProperty);
                cell.ClearValue(Control.BackgroundProperty);
                cell.ClearValue(Control.PaddingProperty);
                cell.ClearValue(FrameworkElement.MarginProperty);
                cell.ClearValue(FrameworkElement.ToolTipProperty);
            }
        }

        private void UpdateBivariateCDF()
        {
            if (BivariateCDF == null) return;

            // X1 values
            var x1Vals = new double[_dt.Rows.Count - 1];
            for (int i = 1; i < _dt.Rows.Count; i++)
                x1Vals[i - 1] = _dt.Rows[i][0] == DBNull.Value ? 0d : (double)_dt.Rows[i][0];

            // X2 values
            var x2Vals = new double[_dt.Columns.Count - 1];
            for (int i = 1; i < _dt.Columns.Count; i++)
                x2Vals[i - 1] = _dt.Rows[0][i] == DBNull.Value ? 0d : (double)_dt.Rows[0][i];

            // Probabilities
            var pVals = new double[_dt.Rows.Count - 1, _dt.Columns.Count - 1];
            for (int i = 1; i < _dt.Rows.Count; i++)
            {
                for (int j = 1; j < _dt.Columns.Count; j++)
                    pVals[i - 1, j - 1] = _dt.Rows[i][j] == DBNull.Value ? 0d : (double)_dt.Rows[i][j];
            }
            BivariateCDF.SetParameters(x1Vals, x2Vals, pVals);
            GetBindingExpression(BivariateCDFProperty).UpdateSource();
        }

        private List<LineSeries> _lineSeriesList = new List<LineSeries>();

        
    private void UpdatePlot()
        {
            // If the plot is nothing, then exit
            if (Plot == null) return;

            // Clear plot series
            Plot.Series.Clear();

            // Create a new line series for each column in the data grid
            var colorHexCodes = GenericControls.GeneralMethods.RandomColorsShortList;

            for (int i = 1; i < _dt.Columns.Count; i++)
            {
                var xyList = new List<Point>();
                for (int j = 1; j < _dt.Rows.Count; j++)
                {
                    xyList.Add(new Point(_dt.Rows[j][0] == DBNull.Value ? 0d : (double)_dt.Rows[j][0], _dt.Rows[j][i] == DBNull.Value ? 0d : (double)_dt.Rows[j][i]));
                }

                var newLineSeries = new LineSeries()
                {
                    Name = "series_" + i.ToString(),// (_dt.Rows[0][i] == DBNull.Value ? 0d : (double)_dt.Rows[0][i]).ToString(),
                    Title = (_dt.Rows[0][i] == DBNull.Value ? 0d : (double)_dt.Rows[0][i]).ToString(),
                    LineStyle = OxyPlot.LineStyle.Solid,
                    Color = (Color)ColorConverter.ConvertFromString(colorHexCodes[i-1]),
                    StrokeThickness = 1.5d,
                    ItemsSource = xyList,
                    DataFieldX = nameof(Point.X),
                    DataFieldY = nameof(Point.Y),
                    TrackerFormatString = "{0}" + Environment.NewLine + "{1}: {2:0}" + Environment.NewLine + "{3}: {4:0.000000}"
                };

                Plot.Series.Add(newLineSeries);
            }

        }

        private void BivariateCDFDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Width = new DataGridLength(1d, DataGridLengthUnitType.Star);
            e.Column.MinWidth = 35d;

            if (e.PropertyName == "Column0")
            {
                // CType(e.Column, DataGridTextColumn).ElementStyle = CType(Resources("ColumnStyle"), Style)
                // CType(e.Column, DataGridTextColumn).EditingElementStyle = CType(Resources("ColumnEditStyle"), Style)
                ((DataGridTextColumn)e.Column).CellStyle = (Style)Resources["BoldCellStyle"];
            }
            else
            {
                //(e.Column as DataGridTextColumn).Binding.StringFormat = "{0:0.#####E+00}";
                (e.Column as DataGridTextColumn).Binding.TargetNullValue = Double.NaN.ToString();
                (e.Column as DataGridTextColumn).HeaderStyle = (Style)Resources["headerTemplate"];
            }
        }

        private void RefreshDataGridView()
        {
            // Top Row
            var topRow = BivariateCDFDataGrid.GetRow(0);
            if (topRow == null) return;
            topRow.FontWeight = FontWeights.Bold;
            topRow.Foreground = (SolidColorBrush)Resources["HazardColor"];
            topRow.BorderThickness = new Thickness(0d, 0d, 0d, 2d);
            topRow.BorderBrush = new SolidColorBrush(Colors.Black);
            // Upper left cell
            var upperLeftCell = BivariateCDFDataGrid.GetCell(0, 0);
            upperLeftCell.Style = (Style)Resources["UpperLeftCellStyle"];
            // DirectCast(upperLeftCell.Content, TextBlock).Focusable = False
        }

        private void BivariateCDFDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // Ensure that the top left cell can't be edited.
            if (e.Column.DisplayIndex == 0 && e.Row.GetIndex() == 0) e.Cancel = true;
        }

        private void BivariateCDFDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (BivariateCDFDataGrid.SelectedCells.Count <= 0)
            {
                _insertColumnButton.IsEnabled = false;
                _deleteColumnButton.IsEnabled = false;
            }
            else
            {
                _insertColumnButton.IsEnabled = true;
                _deleteColumnButton.IsEnabled = true;
            }
        }

        private void BivariateCDFDataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // If BivariateCDFDataGrid.Columns.Count = 0 Then Exit Sub
            // XHeaderBorder.Margin = New Thickness(BivariateCDFDataGrid.Columns(0).ActualWidth, XHeaderBorder.Margin.Top, XHeaderBorder.Margin.Right, XHeaderBorder.Margin.Bottom)
            // Dim topRow = BivariateCDFDataGrid.GetRow(0)
            // If topRow Is Nothing Then Exit Sub
            // YHeaderBorder.Margin = New Thickness(YHeaderBorder.Margin.Left, topRow.ActualHeight, YHeaderBorder.Margin.Right, YHeaderBorder.Margin.Bottom)
        }

        private void BivariateCDFDataGrid_LayoutUpdated(object sender, EventArgs e)
        {
            if (BivariateCDFDataGrid.Columns.Count == 0) return;
            X2HeaderBorder.Margin = new Thickness(BivariateCDFDataGrid.Columns[0].ActualWidth, X2HeaderBorder.Margin.Top, X2HeaderBorder.Margin.Right, X2HeaderBorder.Margin.Bottom);
            var topRow = BivariateCDFDataGrid.GetRow(0);
            if (topRow == null) return;
            X1HeaderBorder.Margin = new Thickness(X1HeaderBorder.Margin.Left, topRow.ActualHeight, X1HeaderBorder.Margin.Right, X1HeaderBorder.Margin.Bottom);
        }


    }
}
