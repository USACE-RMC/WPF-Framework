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

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GenericControls
{
    /// <summary>
    /// An extended DataGrid with built-in copy/paste functionality, row manipulation, and sorting capabilities.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class CopyPasteDataGrid : DataGrid, INotifyPropertyChanged
    {

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="CopyPasteDataGrid"/> class and sets up context menus.
        /// </summary>
        public CopyPasteDataGrid()
        {

            CanUserResizeColumns = false;
            SelectionMode = DataGridSelectionMode.Extended;
            SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;
            CanUserDeleteRows = false;
            CanUserAddRows = false;
            CanUserResizeRows = false;
            CanUserSortColumns = false;
            CanUserReorderColumns = false;
            CanUserAddInsertDeleteRows = true;
            RowHeight = 24d;
            VerticalContentAlignment = VerticalAlignment.Center;



            // Add context menu handler
            ContextMenuOpening += ContextMenu_Opening;

            // Add Copy/Paste Context Menu
            // Add Row(s)
            _addRowsCMI = new MenuItem() { Name = "AddRows", Header = "Add Row(s)" };
            _addRowsCMI.Icon = CreateVectorMenuIcon("AddRowIcon");
            _addRowsCMI.Click += (sender, e) =>
                {
                    var uniqueRows = GetRowsWithSelectedCells();
                    AddRows(Math.Max(uniqueRows.Count, 1));
                };
            _CopyPasteContextMenu.Items.Add(_addRowsCMI);
            // Insert Row(s)
            _insertRowsCMI = new MenuItem() { Name = "InsertRows", Header = "Insert Row(s)" };
            _insertRowsCMI.Icon = CreateVectorMenuIcon("InsertRowIcon");
            _insertRowsCMI.Click += (sender, e) => InsertRows();
            _CopyPasteContextMenu.Items.Add(_insertRowsCMI);
            // Delete Row(s)
            _deleteRowsCMI = new MenuItem() { Name = "DeleteRows", Header = "Delete Row(s)" };
            _deleteRowsCMI.Icon = CreateVectorMenuIcon("DeleteRowIcon");
            _deleteRowsCMI.Click += (sender, e) => DeleteRows();
            _CopyPasteContextMenu.Items.Add(_deleteRowsCMI);

            // Set up custom menu items
            // Separator
            _customSeparatorCM = new Separator() { Name = "CustomItemsSeparator", Visibility = Visibility.Collapsed };
            _CopyPasteContextMenu.Items.Add(_customSeparatorCM);

            _customContextItems.CollectionChanged += (sender, e) =>
                {
                    if (e.OldItems is not null)
                    {
                        foreach (MenuItem item in e.OldItems)
                            _CopyPasteContextMenu.Items.Remove(item);
                    }
                    if (e.NewItems is not null)
                    {
                        foreach (MenuItem item in e.NewItems)
                            _CopyPasteContextMenu.Items.Insert(3 + _customContextItems.Count, item);
                    }
                    // 
                    if (_customContextItems.Count == 0)
                    {
                        _customSeparatorCM.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        _customSeparatorCM.Visibility = Visibility.Visible;
                    }
                };


            // Separator
            _seperatorCM = new Separator() { Name = "Separator" };
            _CopyPasteContextMenu.Items.Add(_seperatorCM);
            // Select All
            _selectAllCMI = new MenuItem() { Name = "SelectAll", Header = "Select All" };
            _selectAllCMI.Icon = CreateVectorMenuIcon("SelectAllIcon");
            _selectAllCMI.Click += SelectAll_Click;
            _CopyPasteContextMenu.Items.Add(_selectAllCMI);
            // Copy
            _copyCMI = new MenuItem() { Name = "Copy", Header = "Copy" };
            _copyCMI.Icon = CreateVectorMenuIcon("CopyIcon");
            _copyCMI.Click += Copy_Click;
            _CopyPasteContextMenu.Items.Add(_copyCMI);
            // Copy w / Headers
            _copyWHeadersCMI = new MenuItem() { Name = "CopyWHeaders", Header = "Copy w/ Headers" };
            _copyWHeadersCMI.Icon = CreateVectorMenuIcon("CopyWithHeadersIcon");
            _copyWHeadersCMI.Click += CopyWithHeaders_Click;
            _CopyPasteContextMenu.Items.Add(_copyWHeadersCMI);
            // Paste
            _pasteCMI = new MenuItem() { Name = "Paste", Header = "Paste" };
            _pasteCMI.Icon = CreateVectorMenuIcon("PasteIcon");
            _pasteCMI.Click += Paste_Click;
            _CopyPasteContextMenu.Items.Add(_pasteCMI);

            // Add sort context menu
            // Sort ASC
            _sortASCCMI = new MenuItem() { Name = "SortASC", Header = new TextBlock() { Text = "Sort Ascending", TextAlignment = TextAlignment.Left } };
            _sortASCCMI.HorizontalContentAlignment = HorizontalAlignment.Left;
            _sortASCCMI.Icon = CreateVectorMenuIcon("SortAscFilterIcon");
            _sortASCCMI.Click += SortAscending;
            _sortContextMenu.Items.Add(_sortASCCMI);
            // Sort DSC
            _sortDSCCMI = new MenuItem() { Name = "SortDSC", Header = new TextBlock() { Text = "Sort Descending", TextAlignment = TextAlignment.Left } };
            _sortDSCCMI.Icon = CreateVectorMenuIcon("SortDescFilterIcon");
            _sortDSCCMI.Click += SortDescending;
            _sortContextMenu.Items.Add(_sortDSCCMI);
            // Clear Sort
            _clearSortCMI = new MenuItem() { Name = "ClearSort", Header = new TextBlock() { Text = "Clear Sort", TextAlignment = TextAlignment.Left } };
            _clearSortCMI.Icon = CreateVectorMenuIcon("ClearFilterIcon");
            _clearSortCMI.Click += (sender, e) => ClearSort();
            _sortContextMenu.Items.Add(_clearSortCMI);


            // These should be false. 
            CanUserDeleteRows = false;
            CanUserAddRows = false;

            // This live updates the data grid with changes to the source.
            Items.IsLiveSorting = true;
            Loaded += Me_Loaded;
            PreviewMouseRightButtonUp += CopyPasteDataGrid_PreviewMouseRightButtonUp;
            KeyDown += Me_KeyDown;

        }

        #endregion

        #region Members

        #region Context Menu Items

        // Add/Insert/Delete & Copy/Paste Context Menu
        private MenuItem _addRowsCMI;
        private MenuItem _insertRowsCMI;
        private MenuItem _deleteRowsCMI;
        private Separator _seperatorCM;
        private MenuItem _selectAllCMI;
        private MenuItem _copyCMI;
        private MenuItem _copyWHeadersCMI;
        private MenuItem _pasteCMI;
        private ContextMenu _CopyPasteContextMenu = new ContextMenu() { Focusable = false };

        // Sort Context Menu
        private MenuItem _sortASCCMI;
        private MenuItem _sortDSCCMI;
        private MenuItem _clearSortCMI;
        private ContextMenu _sortContextMenu = new ContextMenu() { Focusable = false };
        private DataGridColumn _rightClickedColumn;

        // Custom menu items
        private Separator _customSeparatorCM;
        private ObservableCollection<MenuItem> _customContextItems = new ObservableCollection<MenuItem>();

        #endregion

        #region Properties

        private bool _canUserAddInsertDeleteRows = true;

        /// <summary>
        /// Determines whether the user can add, insert, or delete rows.
        /// </summary>
        [Category("Rows")]
        [Description("Determines whether the user can add, insert, or delete rows.")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool CanUserAddInsertDeleteRows
        {
            get
            {
                return _canUserAddInsertDeleteRows;
            }
            set
            {
                if (_canUserAddInsertDeleteRows != value)
                {
                    _canUserAddInsertDeleteRows = value;
                    if (_canUserAddInsertDeleteRows == false)
                    {
                        if (_seperatorCM is not null)
                            _seperatorCM.Visibility = Visibility.Collapsed;
                        if (_addRowsCMI is not null)
                            _addRowsCMI.Visibility = Visibility.Collapsed;
                        if (_insertRowsCMI is not null)
                            _insertRowsCMI.Visibility = Visibility.Collapsed;
                        if (_deleteRowsCMI is not null)
                            _deleteRowsCMI.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (_seperatorCM is not null)
                            _seperatorCM.Visibility = Visibility.Visible;
                        if (_addRowsCMI is not null)
                            _addRowsCMI.Visibility = Visibility.Visible;
                        if (_insertRowsCMI is not null)
                            _insertRowsCMI.Visibility = Visibility.Visible;
                        if (_deleteRowsCMI is not null)
                            _deleteRowsCMI.Visibility = Visibility.Visible;
                    }
                    // 
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUserAddInsertDeleteRows)));
                }
            }
        }

        /// <summary>
        /// Determines whether rows are automatically added when pasting.
        /// </summary>
        [Category("Rows")]
        [Description("Determines whether rows are automatically added when pasting.")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool PasteAddsRows { get; set; } = true;

        /// <summary>
        /// Gets or sets the row type for the data grid.
        /// </summary>
        public Type RowType { get; set; } = null;

        /// <summary>
        /// Determines whether to show the sort context menu.
        /// </summary>
        public bool ShowSortContextMenu { get; set; } = true;

        /// <summary>
        /// Gets the collection of custom menu items to be displayed in the context menu.
        /// </summary>
        public ObservableCollection<MenuItem> CustomMenuItems
        {
            get
            {
                return _customContextItems;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs before data is pasted, allowing cancellation.
        /// </summary>
        public event PreviewPasteDataEventHandler PreviewPasteData;

        /// <summary>
        /// Represents the method that will handle the <see cref="PreviewPasteData"/> event.
        /// </summary>
        /// <param name="clipboardData">The clipboard data to be pasted.</param>
        /// <param name="cancelPaste">Set to true to cancel the paste operation.</param>
        public delegate void PreviewPasteDataEventHandler(string[][] clipboardData, ref bool cancelPaste);

        /// <summary>
        /// Occurs after data has been pasted.
        /// </summary>
        public event DataPastedEventHandler DataPasted;

        /// <summary>
        /// Represents the method that will handle the <see cref="DataPasted"/> event.
        /// </summary>
        public delegate void DataPastedEventHandler();

        /// <summary>
        /// Occurs before rows are added, allowing cancellation.
        /// </summary>
        public event PreviewAddRowsEventHandler PreviewAddRows;

        /// <summary>
        /// Represents the method that will handle the <see cref="PreviewAddRows"/> event.
        /// </summary>
        /// <param name="startRowIndex">The index at which rows will be added.</param>
        /// <param name="nRows">The number of rows to add.</param>
        /// <param name="cancelAddRows">Set to true to cancel the add operation.</param>
        public delegate void PreviewAddRowsEventHandler(int startRowIndex, int nRows, ref bool cancelAddRows);

        /// <summary>
        /// Occurs after rows have been added.
        /// </summary>
        public event RowsAddedEventHandler RowsAdded;

        /// <summary>
        /// Represents the method that will handle the <see cref="RowsAdded"/> event.
        /// </summary>
        /// <param name="startRowIndex">The index at which rows were added.</param>
        /// <param name="nRows">The number of rows that were added.</param>
        public delegate void RowsAddedEventHandler(int startRowIndex, int nRows);

        /// <summary>
        /// Occurs before rows are deleted, allowing cancellation.
        /// </summary>
        public event PreviewDeleteRowsEventHandler PreviewDeleteRows;

        /// <summary>
        /// Represents the method that will handle the <see cref="PreviewDeleteRows"/> event.
        /// </summary>
        /// <param name="rowindices">The indices of rows to be deleted.</param>
        /// <param name="cancel">Set to true to cancel the delete operation.</param>
        public delegate void PreviewDeleteRowsEventHandler(List<int> rowindices, ref bool cancel);

        /// <summary>
        /// Occurs after rows have been deleted.
        /// </summary>
        public event RowsDeletedEventHandler RowsDeleted;

        /// <summary>
        /// Represents the method that will handle the <see cref="RowsDeleted"/> event.
        /// </summary>
        /// <param name="rowindices">The indices of the rows that were deleted.</param>
        public delegate void RowsDeletedEventHandler(List<int> rowindices);

        /// <summary>
        /// Occurs when a column is sorted.
        /// </summary>
        public event EventHandler<ValueEventArgs<DataGridColumn>> Sorted;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Creates a menu icon from a vector icon resource key in IconDictionary.
        /// </summary>
        /// <param name="resourceKey">The resource key for the vector icon.</param>
        /// <returns>A ContentControl containing the vector icon, or null if not found.</returns>
        private static object CreateVectorMenuIcon(string resourceKey)
        {
            var icon = Application.Current?.TryFindResource(resourceKey);
            if (icon != null)
            {
                return new ContentControl
                {
                    Content = icon,
                    Width = 16,
                    Height = 16
                };
            }
            return null;
        }

        /// <summary>
        /// Determines what functionality is available when the datagrid is loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Me_Loaded(object sender, RoutedEventArgs e)
        {

            // If the datagrid.IsReadOnly is true then CanUserAddInsertDeleteRows is false.
            // The paste button is disabled as well. 
            if (IsReadOnly == true)
            {
                // Disallow editing
                CanUserAddInsertDeleteRows = false;
                // Disable the paste button
                _pasteCMI.IsEnabled = false;
                _pasteCMI.Visibility = Visibility.Collapsed;
            }

            if (CanSelectMultipleItems == false)
            {
                _selectAllCMI.IsEnabled = false;
                _selectAllCMI.Visibility = Visibility.Collapsed;
            }

        }

        #region Context Menu

        /// <summary>
        /// Determines the column that has been right-clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CopyPasteDataGrid_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            DependencyObject DepObject = (DependencyObject)e.OriginalSource;
            while (DepObject is not null && !(DepObject is DataGridColumnHeader) && !(DepObject is DataGridRow))
                DepObject = VisualTreeHelper.GetParent(DepObject);
            if (DepObject is null)
            {
                return;
            }
            if (DepObject is DataGridColumnHeader)
            {
                _rightClickedColumn = ((DataGridColumnHeader)DepObject).Column;
                ContextMenu = _sortContextMenu;
                _CopyPasteContextMenu.Visibility = Visibility.Collapsed;
                _sortContextMenu.Visibility = Visibility.Visible;
            }
            else
            {
                ContextMenu = _CopyPasteContextMenu;
                _CopyPasteContextMenu.Visibility = Visibility.Visible;
                _sortContextMenu.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles context menu opening and configures menu item availability.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ContextMenu_Opening(object sender, ContextMenuEventArgs e)
        {
            if (_CopyPasteContextMenu.Visibility == Visibility.Visible)
            {
                if (SelectedCells.Count <= 0)
                {
                    // If no cells are selected, 
                    // then disable the buttons and exit sub.
                    _insertRowsCMI.IsEnabled = false;
                    _deleteRowsCMI.IsEnabled = false;
                    _copyCMI.IsEnabled = false;
                    _copyWHeadersCMI.IsEnabled = false;
                    _pasteCMI.IsEnabled = false;
                    return;
                }
                else
                {
                    _addRowsCMI.IsEnabled = CanUserAddInsertDeleteRows;
                    _insertRowsCMI.IsEnabled = CanUserAddInsertDeleteRows;
                    _deleteRowsCMI.IsEnabled = CanUserAddInsertDeleteRows;
                    _copyCMI.IsEnabled = true;
                    _copyWHeadersCMI.IsEnabled = true;
                    _pasteCMI.IsEnabled = true;
                }

                // If there is nothing on the clipboard, then disable the paste button.
                try
                {
                    string[][] clipboardData = Clipboard.GetText().Split('\n').Select(row => row.Split('\t').Select(Clipboardcell => Clipboardcell.Length > 0 && Clipboardcell[Clipboardcell.Length - 1] == '\r' ? Clipboardcell.Substring(0, Clipboardcell.Length - 1) : Clipboardcell).ToArray()).Where(a => a.Any(b => b.Length > 0)).ToArray();
                    if (clipboardData.Length == 0)
                    {
                        _pasteCMI.IsEnabled = false;
                    }
                    else
                    {
                        _pasteCMI.IsEnabled = true;
                    }
                }
                catch (Exception)
                {
                    _pasteCMI.IsEnabled = false;
                }
            }

            else
            {
                if (ShowSortContextMenu == false)
                    e.Handled = true;
                if (_rightClickedColumn is null)
                    e.Handled = true;
            }
        }

        #endregion

        #region Select All, Copy, Paste

        /// <summary>
        /// Selects all the cells in the data grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (CanSelectMultipleItems == false)
                return;
            SelectAllCells();
        }

        /// <summary>
        /// Copies the selected cells to the clipboard.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            ApplicationCommands.Copy.Execute(null, this);
        }

        /// <summary>
        /// Copies the selected cells to the clipboard with column headers.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CopyWithHeaders_Click(object sender, RoutedEventArgs e)
        {
            ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, this);
            ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
        }

        /// <summary>
        /// Handles keyboard input and pastes clipboard data when Ctrl+V is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Me_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) | Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.V)
                    PasteClipboard();
            }
        }

        /// <summary>
        /// Pastes data from the clipboard into the data grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            PasteClipboard();
        }

        /// <summary>
        /// Pastes data from the clipboard into the selected cells of the data grid.
        /// </summary>
        public void PasteClipboard()
        {
            try
            {
                if (SelectedCells.Count <= 0)
                    return;
                string[][] clipboardData = Clipboard.GetText().Split('\n').Select(row => row.Split('\t').Select(cell => cell.Length > 0 && cell[cell.Length - 1] == '\r' ? cell.Substring(0, cell.Length - 1) : cell).ToArray()).Where(a => a.Any(b => b.Length > 0)).ToArray();
                bool cancelPaste = false;
                PreviewPasteData?.Invoke(clipboardData, ref cancelPaste);
                if (cancelPaste == true)
                    return;
                int rowIndex, columnIndex;

                if (SelectedCells.Count == 1)
                {

                    // fill beyond selected cell
                    var cellinfo = SelectedCells[0];
                    rowIndex = Items.IndexOf(cellinfo.Item);
                    columnIndex = cellinfo.Column.DisplayIndex;

                    for (int i = 0, loopTo = clipboardData.Count() - 1; i <= loopTo; i++)
                    {
                        if (rowIndex + i > Items.Count - 1)
                        {
                            if (CanUserAddInsertDeleteRows == false || PasteAddsRows == false)
                                break;
                            // Clear sorting
                            ClearSort();
                            // Add rows
                            AddRows(clipboardData.Count() - i);
                        }
                        for (int j = 0, loopTo1 = clipboardData[i].Count() - 1; j <= loopTo1; j++)
                        {
                            if (columnIndex + j > Columns.Count - 1)
                                continue;
                            if (Columns[columnIndex + j].IsReadOnly)
                                continue;
                            var rowType = Items[rowIndex + i].GetType();
                            if (rowType == typeof(DataRowView))
                            {
                                // source is likely a data table
                                ((DataRowView)Items[rowIndex + i]).Row[columnIndex + j] = clipboardData[i][j];
                            }
                            else
                            {
                                Binding binding = (Columns[columnIndex + j] as DataGridBoundColumn).Binding as Binding;
                                var y = rowType.GetProperty(binding.Path.Path);
                                try
                                {
                                    if (IsDoubleType(y.PropertyType))
                                    {
                                        if (string.IsNullOrEmpty(clipboardData[i][j]) || string.IsNullOrEmpty(clipboardData[i][j]) || clipboardData[i][j].Length > 0 && clipboardData[i][j].Substring(0, 1) == " ")
                                        {
                                            y.SetValue(Items[rowIndex + i], Convert.ChangeType(double.NaN, y.PropertyType), null);
                                        }
                                        else if (clipboardData[i][j] == "-inf" || clipboardData[i][j] == "-Inf" || clipboardData[i][j] == "-infinity" || clipboardData[i][j] == "-Infinity")
                                        {
                                            y.SetValue(Items[rowIndex + i], Convert.ChangeType(double.NegativeInfinity, y.PropertyType), null);
                                        }
                                        else if (clipboardData[i][j] == "+inf" || clipboardData[i][j] == "+Inf" || clipboardData[i][j] == "+infinity" || clipboardData[i][j] == "+Infinity")
                                        {
                                            y.SetValue(Items[rowIndex + i], Convert.ChangeType(double.PositiveInfinity, y.PropertyType), null);
                                        }
                                        else
                                        {
                                            y.SetValue(Items[rowIndex + i], Convert.ChangeType(clipboardData[i][j], y.PropertyType), null);
                                        }
                                    }
                                    else
                                    {
                                        y.SetValue(Items[rowIndex + i], Convert.ChangeType(clipboardData[i][j], y.PropertyType), null);
                                    }
                                }
                                catch (Exception)
                                {
                                    if (y is not null && IsNumericType(y.PropertyType))
                                        y.SetValue(Items[rowIndex + i], Convert.ChangeType(IsDoubleType(y.PropertyType) ? double.NaN : 0d, y.PropertyType), null);
                                }
                            }
                        }
                    }
                }
                else if (SelectedCells.Count > 1)
                {
                    // Test for continuous selection
                    List<int> Rows = new List<int>(), Columns = new List<int>();
                    foreach (DataGridCellInfo cellInfo in SelectedCells)
                    {
                        rowIndex = Items.IndexOf(cellInfo.Item);
                        columnIndex = cellInfo.Column.DisplayIndex;
                        Rows.Add(rowIndex);
                        Columns.Add(columnIndex);
                    }
                    // 
                    Rows.Sort();
                    int RowMax = Rows[Rows.Count - 1];
                    rowIndex = Rows[0];
                    Columns.Sort();
                    columnIndex = Columns[0];
                    int ColumnMax = Columns[Columns.Count - 1];
                    DataGridCell CellCheck;
                    for (int i = rowIndex, loopTo2 = RowMax; i <= loopTo2; i++)
                    {
                        for (int j = columnIndex, loopTo3 = ColumnMax; j <= loopTo3; j++)
                        {
                            CellCheck = GetCell(i, j);
                            if (CellCheck.IsSelected == false)
                            {
                                Mouse.OverrideCursor = null;
                                MessageBox.Show("Invalid selection, selected cells must be continuous.", "Invalid Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                                return;
                            }
                        }
                    }
                    // set the clipboard data
                    for (int i = 0, loopTo4 = clipboardData.Count() - 1; i <= loopTo4; i++)
                    {
                        if (rowIndex + i > RowMax)
                            break;
                        // 
                        for (int j = 0, loopTo5 = clipboardData[i].Count() - 1; j <= loopTo5; j++)
                        {
                            if (columnIndex + j > ColumnMax)
                                continue;
                            if (this.Columns[columnIndex + j].IsReadOnly)
                                continue;
                            var rowType = Items[rowIndex + i].GetType();
                            if (rowType == typeof(DataRowView))
                            {
                                // source is likely a data table
                                ((DataRowView)Items[rowIndex + i]).Row[columnIndex + j] = clipboardData[i][j];
                            }
                            else
                            {
                                Binding binding = (this.Columns[columnIndex + j] as DataGridBoundColumn).Binding as Binding;
                                var y = rowType.GetProperty(binding.Path.Path);
                                try
                                {
                                    if (IsDoubleType(y.PropertyType))
                                    {
                                        if (string.IsNullOrEmpty(clipboardData[i][j]) || string.IsNullOrEmpty(clipboardData[i][j]) || clipboardData[i][j].Length > 0 && clipboardData[i][j].Substring(0, 1) == " ")
                                        {
                                            y.SetValue(Items[rowIndex + i], Convert.ChangeType(double.NaN, y.PropertyType), null);
                                        }
                                        else if (clipboardData[i][j] == "-inf" || clipboardData[i][j] == "-Inf" || clipboardData[i][j] == "-infinity" || clipboardData[i][j] == "-Infinity")
                                        {
                                            y.SetValue(Items[rowIndex + i], Convert.ChangeType(double.NegativeInfinity, y.PropertyType), null);
                                        }
                                        else if (clipboardData[i][j] == "+inf" || clipboardData[i][j] == "+Inf" || clipboardData[i][j] == "+infinity" || clipboardData[i][j] == "+Infinity")
                                        {
                                            y.SetValue(Items[rowIndex + i], Convert.ChangeType(double.PositiveInfinity, y.PropertyType), null);
                                        }
                                        else
                                        {
                                            y.SetValue(Items[rowIndex + i], Convert.ChangeType(clipboardData[i][j], y.PropertyType), null);
                                        }
                                    }
                                    else
                                    {
                                        y.SetValue(Items[rowIndex + i], Convert.ChangeType(clipboardData[i][j], y.PropertyType), null);
                                    }
                                }
                                catch (Exception)
                                {
                                    if (IsNumericType(y.PropertyType))
                                        y.SetValue(Items[rowIndex + i], Convert.ChangeType(IsDoubleType(y.PropertyType) ? double.NaN : 0d, y.PropertyType), null);
                                }
                            }
                        }
                    }
                }
                //
                Items.Refresh();
                DataPasted?.Invoke();
            }
            //
            catch (Exception)
            {
                Mouse.OverrideCursor = null;
                MessageBox.Show("Error pasting data from clipboard.", "Error in paste from clipboard", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        #endregion

        #region Add, Insert, Delete

        /// <summary>
        /// Adds a single row to the end of the data grid with specified data.
        /// </summary>
        /// <param name="rowData">A dictionary mapping property names to their values for the new row.</param>
        public void AddRow(Dictionary<string, object> rowData)
        {
            int insertAtRow = Items.Count;
            // Exit if there are no items to create an instance from.
            if (RowType == null)
            {
                if (Items.Count == 0)
                    return;
                RowType = Items[0].GetType();
            }
            // 
            try
            {
                if (RowType == typeof(DataRowView))
                {
                    // source is likely a datatable
                    if (Items.Count == 0)
                        return;
                    var table = ((DataRowView)Items[0]).DataView.Table;
                    var newRow = table.NewRow();
                    foreach (var v in rowData)
                        newRow[v.Key] = v.Value;
                    newRow.EndEdit();
                }
                else
                {
                    // get item source (must implement ilist)
                    CollectionView cv = ItemsSource as CollectionView;
                    IList itemList;
                    if (!(cv == null))
                    {
                        itemList = (IList)cv.SourceCollection;
                    }
                    else
                    {
                        itemList = (IList)ItemsSource;
                    }
                    // 
                    var obj = Activator.CreateInstance(RowType);
                    foreach (var v in rowData)
                    {
                        var y = RowType.GetProperty(v.Key);
                        try
                        {
                            y.SetValue(obj, Convert.ChangeType(v.Value, y.PropertyType), null);
                        }
                        catch (Exception)
                        {
                            if (IsNumericType(y.PropertyType))
                                y.SetValue(obj, Convert.ChangeType("0", y.PropertyType), null);
                        }
                    }
                    // Add rows equal to the number of rows to add (minimum of 1).
                    // For i As Int32 = 1 To rowCount
                    itemList.Add(obj);
                    // Next
                } // Activator.CreateInstance(RowType))
                  // Raise the rows added event
                RowsAdded?.Invoke(insertAtRow, 1);
            }
            // 
            // Items.Refresh()
            // 
            catch (Exception ex)
            {
                MessageBox.Show("Hey developer this shouldn't happen you did something wrong:" + Environment.NewLine + ex.Message, "Insert Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }


        /// <summary>
        /// Adds the specified number of rows to the end of the data grid.
        /// </summary>
        /// <param name="nRowsToAdd">The number of rows to add.</param>
        public void AddRows(int nRowsToAdd)
        {
            // Count of rows to be added and insert point, if nRowsToAdd is zero or less then just add one row
            int rowCount = Math.Max(nRowsToAdd, 1);
            int insertAtRow = Items.Count;
            // raise preview event and cancel add if requested.
            bool cancelAdd = false;
            PreviewAddRows?.Invoke(insertAtRow, rowCount, ref cancelAdd);
            if (cancelAdd == true)
                return;
            // exit if there are no items to create an instance from.
            if (RowType == null)
            {
                if (Items.Count == 0)
                    return;
                RowType = Items[0].GetType();
            }
            try
            {
                if (RowType == typeof(DataRowView))
                {
                    // source is likely a datatable
                    if (Items.Count == 0)
                        return;
                    var table = ((DataRowView)Items[0]).DataView.Table;
                    table.Rows.Add(table.NewRow());
                }
                else
                {
                    // get item source (must implement ilist)
                    CollectionView cv = ItemsSource as CollectionView;
                    IList itemList;
                    if (!(cv == null))
                    {
                        itemList = (IList)cv.SourceCollection;
                    }
                    else
                    {
                        itemList = (IList)ItemsSource;
                    }
                    // Add rows equal to the number of rows to add (minimum of 1).
                    for (int i = 1, loopTo = rowCount; i <= loopTo; i++)
                        itemList.Add(Activator.CreateInstance(RowType));
                }
                // 
                RowsAdded?.Invoke(insertAtRow, rowCount);
            }
            // Items.Refresh()
            catch (Exception ex)
            {
                MessageBox.Show("Hey developer this shouldn't happen you did something wrong:" + Environment.NewLine + ex.Message, "Insert Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        /// <summary>
        /// Inserts new rows at the position of the first selected cell.
        /// The number of rows inserted equals the number of unique selected rows.
        /// </summary>
        public void InsertRows()
        {
            if (SelectedCells.Count <= 0)
                return;
            var uniqueRows = GetRowsWithSelectedCells();
            // Count of rows to be added and insert point
            int rowCount = Math.Max(uniqueRows.Count, 1);
            int insertAtRow = uniqueRows.Min();
            // 
            InsertRows(insertAtRow, rowCount);
        }

        /// <summary>
        /// Inserts a specified number of new rows at the given index.
        /// </summary>
        /// <param name="startRowIndex">The index at which to insert the new rows.</param>
        /// <param name="rowCount">The number of rows to insert.</param>
        public void InsertRows(int startRowIndex, int rowCount)
        {
            // Get list of sorted rows
            // get item source (must implement ilist)
            CollectionView cv = ItemsSource as CollectionView;
            IList itemList;
            if (!(cv == null))
            {
                itemList = (IList)cv.SourceCollection;
            }
            else
            {
                itemList = (IList)ItemsSource;
            }
            // 
            var rowList = new List<object>();
            var sortedList = CollectionViewSource.GetDefaultView(itemList).GetEnumerator();
            for (int i = 0, loopTo = itemList.Count - 1; i <= loopTo; i++)
            {
                sortedList.MoveNext();
                rowList.Add(sortedList.Current);
            }
            // 
            if (startRowIndex > itemList.Count - 1)
                startRowIndex = itemList.Count - 1;
            int insertAtRowSorted = itemList.IndexOf(rowList[startRowIndex]);
            // 
            // raise preview event and cancel add if requested.
            bool cancelAdd = false;
            PreviewAddRows?.Invoke(insertAtRowSorted, rowCount, ref cancelAdd);
            if (cancelAdd == true)
                return;
            if (RowType == null)
            {
                if (Items.Count == 0)
                    return;
                RowType = Items[0].GetType();
            }
            try
            {
                if (RowType == typeof(DataRowView))
                {
                    // source is likely a datatable
                    if (Items.Count == 0)
                        return;
                    var table = ((DataRowView)Items[0]).DataView.Table;
                    for (int i = 1, loopTo1 = rowCount; i <= loopTo1; i++)
                        table.Rows.InsertAt(table.NewRow(), insertAtRowSorted);
                }
                else
                {
                    for (int i = 1, loopTo2 = rowCount; i <= loopTo2; i++)
                        itemList.Insert(insertAtRowSorted, Activator.CreateInstance(RowType));
                }
                // 
                RowsAdded?.Invoke(insertAtRowSorted, rowCount);
            }
            // Items.Refresh()
            catch (Exception ex)
            {
                MessageBox.Show("Hey developer this shouldn't happen you did something wrong:" + Environment.NewLine + ex.Message, "Insert Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        /// <summary>
        /// Deletes all rows that have selected cells from the data grid.
        /// </summary>
        public void DeleteRows()
        {
            if (SelectedCells.Count <= 0)
                return;
            var UniqueRows = GetRowsWithSelectedCells().ToList();
            var UniqueSortedRows = new List<int>();
            // If the rows are not sorted, then the unique rows 
            // and unique sorted row list is the same.
            // 
            var cancel = default(bool);
            PreviewDeleteRows?.Invoke(UniqueRows, ref cancel);
            if (cancel == true)
                return;
            // 
            // Get list of sorted rows
            // get item source (must implement ilist)
            CollectionView cv = ItemsSource as CollectionView;
            IList itemList;
            if (!(cv == null))
            {
                itemList = (IList)cv.SourceCollection;
            }
            else
            {
                itemList = (IList)ItemsSource;
            }
            var rowList = new List<object>();
            var sortedList = CollectionViewSource.GetDefaultView(itemList).GetEnumerator();
            for (int i = 0, loopTo = itemList.Count - 1; i <= loopTo; i++)
            {
                sortedList.MoveNext();
                rowList.Add(sortedList.Current);
            }
            // 
            // Get list of sorted row indeces
            UniqueRows.Sort();
            for (int i = 0, loopTo1 = UniqueRows.Count - 1; i <= loopTo1; i++)
            {
                int index = itemList.IndexOf(rowList[UniqueRows[i]]);
                UniqueSortedRows.Add(index);
            }
            // 
            // Perform delete
            UniqueSortedRows.Sort();
            for (int i = UniqueSortedRows.Count - 1; i >= 0; i -= 1)
                itemList.RemoveAt(UniqueSortedRows[i]);

            RowsDeleted?.Invoke(UniqueSortedRows);
            // Items.Refresh()
        }

        /// <summary>
        /// Gets the unique row indexes that have cells selected.
        /// </summary>
        /// <returns>A set of unique row indices containing selected cells.</returns>
        public HashSet<int> GetRowsWithSelectedCells()
        {
            var uniqueRows = new HashSet<int>();
            int rowIndex;
            foreach (DataGridCellInfo cellInfo in SelectedCells)
            {
                rowIndex = Items.IndexOf(cellInfo.Item);
                if (uniqueRows.Contains(rowIndex) == false)
                    uniqueRows.Add(rowIndex);
            }
            return uniqueRows;
        }

        /// <summary>
        /// Gets the unique column indexes that have cells selected.
        /// </summary>
        /// <returns>A set of unique column indices containing selected cells.</returns>
        public HashSet<int> GetColumnsWithSelectedCells()
        {
            var uniqueColumns = new HashSet<int>();
            foreach (DataGridCellInfo cellInfo in SelectedCells)
            {
                if (uniqueColumns.Contains(cellInfo.Column.DisplayIndex) == false)
                    uniqueColumns.Add(cellInfo.Column.DisplayIndex);
            }
            return uniqueColumns;
        }

        #endregion

        #region Support

        /// <summary>
        /// Gets the data grid row at a specified index.
        /// </summary>
        /// <param name="index">The index of the data grid row to be returned.</param>
        /// <returns>The <see cref="DataGridRow"/> at the specified index, or null if not found.</returns>
        public DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)ItemContainerGenerator.ContainerFromIndex(index);
            if (row is null && index < Items.Count)
            {
                // May be virtualized, bring into view and try again.
                UpdateLayout();
                ScrollIntoView(Items[index]);
                row = (DataGridRow)ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        /// <summary>
        /// Gets the data grid cell at specified row and column indices.
        /// </summary>
        /// <param name="rowIndex">The row index of the data grid cell to be returned.</param>
        /// <param name="column">The column index of the data grid cell to be returned.</param>
        /// <returns>The <see cref="DataGridCell"/> at the specified position, or null if not found.</returns>
        public DataGridCell GetCell(int rowIndex, int column)
        {
            var row = GetRow(rowIndex);
            if (row is not null)
            {
                var presenter = GetTheVisualChild<DataGridCellsPresenter>(row);
                if (presenter is null)
                {
                    ScrollIntoView(row, Columns[column]);
                    presenter = GetTheVisualChild<DataGridCellsPresenter>(row);
                }
                if (presenter is null)
                    return null;
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                return cell;
            }
            return null;
        }

        /// <summary>
        /// Gets the first visual child of the specified type from a parent visual element.
        /// </summary>
        /// <typeparam name="T">The type of visual child to find.</typeparam>
        /// <param name="parent">The parent visual element to search.</param>
        /// <returns>The first visual child of type T, or null if not found.</returns>
        public T GetTheVisualChild<T>(Visual parent) where T : Visual
        {
            T child = null;
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0, loopTo = numVisuals - 1; i <= loopTo; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child is null)
                {
                    child = GetTheVisualChild<T>(v);
                }
                if (child is not null)
                {
                    break;
                }
            }
            return child;
        }

        /// <summary>
        /// Determines if a type is numeric. Nullable numeric types are considered numeric.
        /// </summary>
        /// <param name="typeToTest">The type to test for being numeric.</param>
        /// <returns>True if the type is numeric; otherwise, false.</returns>
        /// <remarks>
        /// Boolean is not considered numeric.
        /// <see href="http://stackoverflow.com/questions/124411/using-net-how-can-i-determine-if-a-type-is-a-numeric-valuetype"/>
        /// </remarks>
        public static bool IsNumericType(Type typeToTest)
        {
            if (typeToTest is null)
            {
                return false;
            }
            switch (Type.GetTypeCode(typeToTest))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    {
                        return true;
                    }
                case TypeCode.Object:
                    {
                        if (typeToTest.IsGenericType && typeToTest.GetGenericTypeDefinition() == typeof(object))
                        {
                            return IsNumericType(Nullable.GetUnderlyingType(typeToTest));
                        }
                        return false;
                    }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified type is a double type.
        /// </summary>
        /// <param name="typeToTest">The type to test.</param>
        /// <returns>True if the type is double; otherwise, false.</returns>
        public static bool IsDoubleType(Type typeToTest)
        {
            if (typeToTest is null)
            {
                return false;
            }
            switch (Type.GetTypeCode(typeToTest))
            {
                case TypeCode.Double:
                    {
                        return true;
                    }
            }
            return false;
        }



        #endregion

        #region Sort

        /// <summary>
        /// Sorts the right-clicked column in ascending order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void SortAscending(object sender, RoutedEventArgs e)
        {
            // Clear current sort descriptions
            Items.SortDescriptions.Clear();
            // Add the new sort description
            Items.SortDescriptions.Add(new SortDescription(_rightClickedColumn.SortMemberPath, ListSortDirection.Ascending));
            // Apply sort
            foreach (var col in Columns)
                col.SortDirection = default;
            _rightClickedColumn.SortDirection = ListSortDirection.Ascending;
            // Refresh items to display sort
            Items.Refresh();
        }

        /// <summary>
        /// Sorts the right-clicked column in descending order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void SortDescending(object sender, RoutedEventArgs e)
        {
            // Clear current sort descriptions
            Items.SortDescriptions.Clear();
            // Add the new sort description
            Items.SortDescriptions.Add(new SortDescription(_rightClickedColumn.SortMemberPath, ListSortDirection.Descending));
            // Apply sort
            foreach (var col in Columns)
                col.SortDirection = default;
            _rightClickedColumn.SortDirection = ListSortDirection.Descending;
            // Refresh items to display sort
            Items.Refresh();
        }

        /// <summary>
        /// Clears all sorting applied to the data grid.
        /// </summary>
        public void ClearSort()
        {
            // Clear current sort descriptions
            Items.SortDescriptions.Clear();
            // Clear sort
            foreach (var col in Columns)
                col.SortDirection = default;
            // Refresh items to display sort
            Items.Refresh();
        }

        /// <summary>
        /// Raises the <see cref="Sorted"/> event when a column is sorted.
        /// </summary>
        /// <param name="eventArgs">The event data containing the column being sorted.</param>
        protected override void OnSorting(DataGridSortingEventArgs eventArgs)
        {
            base.OnSorting(eventArgs);
            var column = eventArgs.Column;
            Sorted?.Invoke(this, new ValueEventArgs<DataGridColumn>(column));
        }

        /// <summary>
        /// Event arguments class that carries a value of a specified type.
        /// </summary>
        /// <typeparam name="T">The type of the value carried by the event arguments.</typeparam>
        public class ValueEventArgs<T> : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ValueEventArgs{T}"/> class.
            /// </summary>
            /// <param name="value">The value to pass with the event.</param>
            public ValueEventArgs(T value)
            {
                Value = value;
            }

            /// <summary>
            /// Gets or sets the value associated with the event.
            /// </summary>
            public T Value { get; set; }
        }

        #endregion

        #endregion

    }
}