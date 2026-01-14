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

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DatabaseManager;
using Microsoft.Win32;

namespace DatabaseControls
{
    /// <summary>
    /// A WPF control for viewing and interacting with database tables.
    /// </summary>
    public partial class TableViewer : UserControl
    {
        #region Enums

        private enum SelectionMode : byte
        {
            CellSelect = 0,
            RowSelect = 1,
            ColumnSelect = 2,
            None = 3,
            All = 4,
            EditSelect = 5
        }

        private enum SortOrder : byte
        {
            Ascending = 2,
            Descending = 1,
            None = 0
        }

        #endregion

        #region Fields

        private int _visibleRowCount;
        private bool _isLoaded = false;
        private int _activeCellDataColumnIndex = 0;
        private int _activeCellVirtualRowIndex = 0;
        private int _mouseDownVirtualRowIndex;
        private int _mouseDownColumnIndex;
        private SelectionMode _mouseSelectionMode = SelectionMode.None;
        private List<int> _selectedDataRowIndices = new List<int>();
        private List<int> _selectedColumnIndices = new List<int>();
        private SortedDictionary<int, SortedSet<int>> _selectedCellIndices = new SortedDictionary<int, SortedSet<int>>();
        private bool _selectedRowsOnly = false;
        private string _attributeSelectorString = "";
        private int[]? _rowOffset;
        private int[]? _rowId;
        private int[]? _sortedSelectedRowOffsets;
        private SortOrder _columnSortOrder;
        private SortOrder[]? _columnsSortedOrder;
        private GridLength[]? _columnWidths;
        private readonly TextBox _cellEditTextBox;
        private string _fieldCalculatorString = "";
        private HashSet<int> _readOnlyColumns = new HashSet<int>();

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="DataView"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DataViewProperty = DependencyProperty.Register(
            nameof(DataView), typeof(DataTableView), typeof(TableViewer),
            new UIPropertyMetadata(null, new PropertyChangedCallback(LoadView)));

        /// <summary>
        /// Identifies the <see cref="AllCellsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllCellsSelectedProperty = DependencyProperty.Register(
            nameof(AllCellsSelected), typeof(bool), typeof(TableViewer), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="RowHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeightProperty = DependencyProperty.Register(
            nameof(RowHeight), typeof(double), typeof(TableViewer), new UIPropertyMetadata(23.0));

        /// <summary>
        /// Identifies the <see cref="ColumnHeaderHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderHeightProperty = DependencyProperty.Register(
            nameof(ColumnHeaderHeight), typeof(GridLength), typeof(TableViewer), new UIPropertyMetadata(GridLength.Auto));

        /// <summary>
        /// Identifies the <see cref="ColumnHeaderTextblockStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderTextblockStyleProperty = DependencyProperty.Register(
            nameof(ColumnHeaderTextblockStyle), typeof(Style), typeof(TableViewer), new PropertyMetadata(GetDefaultColumnHeaderTextblockStyle()));

        /// <summary>
        /// Identifies the <see cref="ColumnHeaderBorderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnHeaderBorderStyleProperty = DependencyProperty.Register(
            nameof(ColumnHeaderBorderStyle), typeof(Style), typeof(TableViewer), new PropertyMetadata(GetDefaultColumnHeaderBorderStyle()));

        /// <summary>
        /// Identifies the <see cref="RowHeaderTextblockStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeaderTextblockStyleProperty = DependencyProperty.Register(
            nameof(RowHeaderTextblockStyle), typeof(Style), typeof(TableViewer), new PropertyMetadata(GetDefaultRowHeaderTextblockStyle()));

        /// <summary>
        /// Identifies the <see cref="RowHeaderBorderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowHeaderBorderStyleProperty = DependencyProperty.Register(
            nameof(RowHeaderBorderStyle), typeof(Style), typeof(TableViewer), new PropertyMetadata(GetDefaultRowHeaderBorderStyle()));

        /// <summary>
        /// Identifies the <see cref="ShowRowHeaders"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowRowHeadersProperty = DependencyProperty.Register(
            nameof(ShowRowHeaders), typeof(bool), typeof(TableViewer), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="Editable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register(
            nameof(Editable), typeof(bool), typeof(TableViewer), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="HasFieldCalculator"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasFieldCalculatorProperty = DependencyProperty.Register(
            nameof(HasFieldCalculator), typeof(bool), typeof(TableViewer), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="HasSaveButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasSaveButtonProperty = DependencyProperty.Register(
            nameof(HasSaveButton), typeof(bool), typeof(TableViewer), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="HasUndoRedoButtons"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasUndoRedoButtonsProperty = DependencyProperty.Register(
            nameof(HasUndoRedoButtons), typeof(bool), typeof(TableViewer), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="RowSelectable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowSelectableProperty = DependencyProperty.Register(
            nameof(RowSelectable), typeof(bool), typeof(TableViewer), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CellTextblockStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellTextblockStyleProperty = DependencyProperty.Register(
            nameof(CellTextblockStyle), typeof(Style), typeof(TableViewer), new PropertyMetadata(GetDefaultCellTextblockStyle()));

        /// <summary>
        /// Identifies the <see cref="SelectedColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            nameof(SelectedColor), typeof(Brush), typeof(TableViewer), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(240, 0, 120, 215))));

        /// <summary>
        /// Identifies the <see cref="ActiveCellForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveCellForegroundProperty = DependencyProperty.Register(
            nameof(ActiveCellForeground), typeof(Brush), typeof(TableViewer), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Identifies the <see cref="ActiveCellBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveCellBackgroundProperty = DependencyProperty.Register(
            nameof(ActiveCellBackground), typeof(Brush), typeof(TableViewer), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 21, 107, 176))));

        /// <summary>
        /// Identifies the <see cref="DeSelectedColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DeSelectedColorProperty = DependencyProperty.Register(
            nameof(DeSelectedColor), typeof(Brush), typeof(TableViewer), new UIPropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the <see cref="SelectedForegroundColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundColorProperty = DependencyProperty.Register(
            nameof(SelectedForegroundColor), typeof(Brush), typeof(TableViewer), new UIPropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Identifies the <see cref="DeSelectedForegroundColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DeSelectedForegroundColorProperty = DependencyProperty.Register(
            nameof(DeSelectedForegroundColor), typeof(Brush), typeof(TableViewer), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Identifies the <see cref="RowColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowColorProperty = DependencyProperty.Register(
            nameof(RowColor), typeof(Brush), typeof(TableViewer), new UIPropertyMetadata(Brushes.White));

        /// <summary>
        /// Identifies the <see cref="AlternateRowColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AlternateRowColorProperty = DependencyProperty.Register(
            nameof(AlternateRowColor), typeof(Brush), typeof(TableViewer), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 243, 249, 247))));

        /// <summary>
        /// Identifies the <see cref="RowLineColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowLineColorProperty = DependencyProperty.Register(
            nameof(RowLineColor), typeof(Brush), typeof(TableViewer), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 53, 59, 122))));

        /// <summary>
        /// Identifies the <see cref="RowLineThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RowLineThicknessProperty = DependencyProperty.Register(
            nameof(RowLineThickness), typeof(double), typeof(TableViewer), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="ColumnLineColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnLineColorProperty = DependencyProperty.Register(
            nameof(ColumnLineColor), typeof(Brush), typeof(TableViewer), new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(255, 53, 59, 122))));

        /// <summary>
        /// Identifies the <see cref="ColumnLineThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnLineThicknessProperty = DependencyProperty.Register(
            nameof(ColumnLineThickness), typeof(double), typeof(TableViewer), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Identifies the <see cref="ColumnSelectable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColumnSelectableProperty = DependencyProperty.Register(
            nameof(ColumnSelectable), typeof(bool), typeof(TableViewer), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="CellSelectable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CellSelectableProperty = DependencyProperty.Register(
            nameof(CellSelectable), typeof(bool), typeof(TableViewer), new UIPropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="AutoFitColumns"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoFitColumnsProperty = DependencyProperty.Register(
            nameof(AutoFitColumns), typeof(bool), typeof(TableViewer), new UIPropertyMetadata(false));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the data source for the table viewer.
        /// </summary>
        /// <value>The <see cref="DataTableView"/> containing the data to display.</value>
        public DataTableView DataView
        {
            get => (DataTableView)GetValue(DataViewProperty);
            set => SetValue(DataViewProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether only selected rows are currently being displayed.
        /// </summary>
        /// <value><c>true</c> if showing selected rows only; otherwise, <c>false</c>.</value>
        public bool ShowingSelectedRowsOnly => _selectedRowsOnly;

        /// <summary>
        /// Gets the list of currently selected row indices in the data table.
        /// </summary>
        /// <value>A list of zero-based row indices that are currently selected.</value>
        public List<int> GetSelectedRows => _selectedDataRowIndices;

        /// <summary>
        /// Gets the row index of the currently active cell.
        /// </summary>
        /// <value>The zero-based row index of the active cell in the virtual (visible) grid.</value>
        public int ActiveCellRowIndex => _activeCellVirtualRowIndex;

        /// <summary>
        /// Gets the column index of the currently active cell.
        /// </summary>
        /// <value>The zero-based column index of the active cell in the data table.</value>
        public int ActiveCellColumnIndex => _activeCellDataColumnIndex;

        /// <summary>
        /// Gets or sets a value indicating whether all cells in the table are selected.
        /// </summary>
        /// <value><c>true</c> if all cells are selected; otherwise, <c>false</c>.</value>
        public bool AllCellsSelected
        {
            get => (bool)GetValue(AllCellsSelectedProperty);
            set => SetValue(AllCellsSelectedProperty, value);
        }

        /// <summary>
        /// Gets or sets the height of each data row in pixels.
        /// </summary>
        /// <value>The row height in pixels. Default is 23.0.</value>
        public double RowHeight
        {
            get => (double)GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the height of the column header row.
        /// </summary>
        /// <value>The column header height as a <see cref="GridLength"/>. Default is Auto.</value>
        public GridLength ColumnHeaderHeight
        {
            get => (GridLength)GetValue(ColumnHeaderHeightProperty);
            set => SetValue(ColumnHeaderHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the style applied to column header text blocks.
        /// </summary>
        /// <value>The <see cref="Style"/> for column header TextBlock elements.</value>
        public Style ColumnHeaderTextblockStyle
        {
            get => (Style)GetValue(ColumnHeaderTextblockStyleProperty);
            set => SetValue(ColumnHeaderTextblockStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the style applied to column header borders.
        /// </summary>
        /// <value>The <see cref="Style"/> for column header Border elements.</value>
        public Style ColumnHeaderBorderStyle
        {
            get => (Style)GetValue(ColumnHeaderBorderStyleProperty);
            set => SetValue(ColumnHeaderBorderStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the style applied to row header text blocks.
        /// </summary>
        /// <value>The <see cref="Style"/> for row header TextBlock elements.</value>
        public Style RowHeaderTextblockStyle
        {
            get => (Style)GetValue(RowHeaderTextblockStyleProperty);
            set => SetValue(RowHeaderTextblockStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the style applied to row header borders.
        /// </summary>
        /// <value>The <see cref="Style"/> for row header Border elements.</value>
        public Style RowHeaderBorderStyle
        {
            get => (Style)GetValue(RowHeaderBorderStyleProperty);
            set => SetValue(RowHeaderBorderStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether row headers are visible.
        /// </summary>
        /// <value><c>true</c> to show row headers; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool ShowRowHeaders
        {
            get => (bool)GetValue(ShowRowHeadersProperty);
            set => SetValue(ShowRowHeadersProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether cells can be edited.
        /// </summary>
        /// <value><c>true</c> if the table is editable; otherwise, <c>false</c>. Default is <c>false</c>.</value>
        public bool Editable
        {
            get => (bool)GetValue(EditableProperty);
            set => SetValue(EditableProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the field calculator button is visible.
        /// </summary>
        /// <value><c>true</c> to show the field calculator; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool HasFieldCalculator
        {
            get => (bool)GetValue(HasFieldCalculatorProperty);
            set => SetValue(HasFieldCalculatorProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the save button is visible.
        /// </summary>
        /// <value><c>true</c> to show the save button; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool HasSaveButton
        {
            get => (bool)GetValue(HasSaveButtonProperty);
            set => SetValue(HasSaveButtonProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the undo/redo buttons are visible.
        /// </summary>
        /// <value><c>true</c> to show undo/redo buttons; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool HasUndoRedoButtons
        {
            get => (bool)GetValue(HasUndoRedoButtonsProperty);
            set => SetValue(HasUndoRedoButtonsProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether entire rows can be selected.
        /// </summary>
        /// <value><c>true</c> to allow row selection; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool RowSelectable
        {
            get => (bool)GetValue(RowSelectableProperty);
            set => SetValue(RowSelectableProperty, value);
        }

        /// <summary>
        /// Gets or sets the style applied to cell text blocks.
        /// </summary>
        /// <value>The <see cref="Style"/> for cell TextBlock elements.</value>
        public Style CellTextblockStyle
        {
            get => (Style)GetValue(CellTextblockStyleProperty);
            set => SetValue(CellTextblockStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the background color for selected cells.
        /// </summary>
        /// <value>The <see cref="Brush"/> used to fill selected cells.</value>
        public Brush SelectedColor
        {
            get => (Brush)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground color for the active cell.
        /// </summary>
        /// <value>The <see cref="Brush"/> used for the active cell's text.</value>
        public Brush ActiveCellForeground
        {
            get => (Brush)GetValue(ActiveCellForegroundProperty);
            set => SetValue(ActiveCellForegroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the background color for the active cell.
        /// </summary>
        /// <value>The <see cref="Brush"/> used to fill the active cell.</value>
        public Brush ActiveCellBackground
        {
            get => (Brush)GetValue(ActiveCellBackgroundProperty);
            set => SetValue(ActiveCellBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the background color for deselected cells.
        /// </summary>
        /// <value>The <see cref="Brush"/> used to fill deselected cells.</value>
        public Brush DeSelectedColor
        {
            get => (Brush)GetValue(DeSelectedColorProperty);
            set => SetValue(DeSelectedColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground color for selected cells.
        /// </summary>
        /// <value>The <see cref="Brush"/> used for selected cell text.</value>
        public Brush SelectedForegroundColor
        {
            get => (Brush)GetValue(SelectedForegroundColorProperty);
            set => SetValue(SelectedForegroundColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground color for deselected cells.
        /// </summary>
        /// <value>The <see cref="Brush"/> used for deselected cell text.</value>
        public Brush DeSelectedForegroundColor
        {
            get => (Brush)GetValue(DeSelectedForegroundColorProperty);
            set => SetValue(DeSelectedForegroundColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the background color for odd-numbered rows.
        /// </summary>
        /// <value>The <see cref="Brush"/> used to fill odd rows.</value>
        public Brush RowColor
        {
            get => (Brush)GetValue(RowColorProperty);
            set => SetValue(RowColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the background color for even-numbered rows.
        /// </summary>
        /// <value>The <see cref="Brush"/> used to fill even rows.</value>
        public Brush AlternateRowColor
        {
            get => (Brush)GetValue(AlternateRowColorProperty);
            set => SetValue(AlternateRowColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of horizontal row separator lines.
        /// </summary>
        /// <value>The <see cref="Brush"/> used for row lines.</value>
        public Brush RowLineColor
        {
            get => (Brush)GetValue(RowLineColorProperty);
            set => SetValue(RowLineColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the thickness of horizontal row separator lines.
        /// </summary>
        /// <value>The line thickness in pixels. Default is 1.0.</value>
        public double RowLineThickness
        {
            get => (double)GetValue(RowLineThicknessProperty);
            set => SetValue(RowLineThicknessProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of vertical column separator lines.
        /// </summary>
        /// <value>The <see cref="Brush"/> used for column lines.</value>
        public Brush ColumnLineColor
        {
            get => (Brush)GetValue(ColumnLineColorProperty);
            set => SetValue(ColumnLineColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the thickness of vertical column separator lines.
        /// </summary>
        /// <value>The line thickness in pixels. Default is 1.0.</value>
        public double ColumnLineThickness
        {
            get => (double)GetValue(ColumnLineThicknessProperty);
            set => SetValue(ColumnLineThicknessProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether entire columns can be selected.
        /// </summary>
        /// <value><c>true</c> to allow column selection; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool ColumnSelectable
        {
            get => (bool)GetValue(ColumnSelectableProperty);
            set => SetValue(ColumnSelectableProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether individual cells can be selected.
        /// </summary>
        /// <value><c>true</c> to allow cell selection; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool CellSelectable
        {
            get => (bool)GetValue(CellSelectableProperty);
            set => SetValue(CellSelectableProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether column widths are automatically adjusted to fit content.
        /// </summary>
        /// <value><c>true</c> to auto-fit columns; otherwise, <c>false</c>. Default is <c>false</c>.</value>
        public bool AutoFitColumns
        {
            get => (bool)GetValue(AutoFitColumnsProperty);
            set => SetValue(AutoFitColumnsProperty, value);
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the set of selected row indices changes.
        /// </summary>
        public event Action<List<int>>? SelectedRowIndicesChanged;

        /// <summary>
        /// Occurs when the active cell location changes.
        /// </summary>
        public event Action? ActiveCellLocationChanged;

        /// <summary>
        /// Occurs when the right mouse button is released on a row header.
        /// Provides the context menu and row index for custom menu handling.
        /// </summary>
        public event Action<ContextMenu, int>? RowRightButtonUp;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TableViewer"/> class.
        /// </summary>
        public TableViewer()
        {
            InitializeComponent();
            _cellEditTextBox = new TextBox { Padding = new Thickness(0, 3, 0, 2) };
            _cellEditTextBox.PreviewKeyDown += PreviewEditText;
            _cellEditTextBox.LostFocus += EditTextLostFocus;
            EditorToolbar.IsEnabled = false;
        }

        #endregion

        #region Loading

        private static void LoadView(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null || d.GetType() != typeof(TableViewer)) return;
            var thisControl = (TableViewer)d;

            if (e.OldValue != null && e.OldValue is DataTableView oldView)
            {
                oldView.RowsAdded -= thisControl.TableViewRowsAdded;
                oldView.RowsDeleted -= thisControl.TableViewRowsDeleted;
                oldView.ColumnsAdded -= thisControl.TableViewColumnsAdded;
                oldView.ColumnsDeleted -= thisControl.TableViewColumnsDeleted;
            }

            if (e.NewValue == null || !(e.NewValue is DataTableView))
            {
                thisControl._selectedCellIndices = new SortedDictionary<int, SortedSet<int>>();
                thisControl._selectedColumnIndices = new List<int>();
                thisControl._selectedDataRowIndices = new List<int>();
                thisControl._activeCellDataColumnIndex = 0;
                thisControl._activeCellVirtualRowIndex = 0;
                thisControl._selectedRowsOnly = false;
                thisControl.ColumnHeadersGrid.SizeChanged -= thisControl.ColumnsGridSizeChanged;
                thisControl.GridPanel.Children.Clear();
                thisControl.RowHeadersGrid.Children.Clear();
                thisControl.RowColorGrid.Children.Clear();
                thisControl.ColumnHeadersGrid.Children.Clear();
                thisControl.GridPanel.RowDefinitions.Clear();
                thisControl.RowHeadersGrid.RowDefinitions.Clear();
                thisControl.RowColorGrid.RowDefinitions.Clear();
                thisControl.ColumnHeadersGrid.ColumnDefinitions.Clear();
                thisControl.GridPanel.ColumnDefinitions.Clear();
                thisControl.GridLinesCanvas.Children.Clear();
                thisControl.SelectionToolbar.IsEnabled = false;
                thisControl.EditorToolbar.IsEnabled = false;
            }
            else
            {
                var newView = (DataTableView)e.NewValue;
                newView.RowsAdded += thisControl.TableViewRowsAdded;
                newView.RowsDeleted += thisControl.TableViewRowsDeleted;
                newView.ColumnsAdded += thisControl.TableViewColumnsAdded;
                newView.ColumnsDeleted += thisControl.TableViewColumnsDeleted;

                if (!newView.ParentDatabase.DataBaseOpen) newView.ParentDatabase.Open();

                thisControl._columnSortOrder = SortOrder.None;
                thisControl._columnsSortedOrder = new SortOrder[newView.ColumnNames.Count()];

                thisControl.Refresh();

                if (thisControl.ShowRowHeaders)
                {
                    double pixelsPerDip = VisualTreeHelper.GetDpi(thisControl).PixelsPerDip;
                    thisControl.RowHeadersColumnDefinition.Width = new GridLength(
                        (int)(new FormattedText(newView.NumberOfRows.ToString(), CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                            12, Brushes.Black, pixelsPerDip).Width) + 3);
                }

                thisControl.SelectionToolbar.IsEnabled = true;
                thisControl.EditorToolbar.IsEnabled = true;
            }
        }

        /// <summary>
        /// Marks the specified columns as read-only, preventing edits to their cells.
        /// </summary>
        /// <param name="columnIndices">Array of zero-based column indices to mark as read-only.</param>
        public void SetColumnsAsReadOnly(int[] columnIndices)
        {
            foreach (var idx in columnIndices) _readOnlyColumns.Add(idx);
        }

        /// <summary>
        /// Marks the specified columns as read-only by column name, preventing edits to their cells.
        /// </summary>
        /// <param name="columnNames">Array of column names to mark as read-only.</param>
        public void SetColumnsAsReadOnly(string[] columnNames)
        {
            foreach (var name in columnNames)
                _readOnlyColumns.Add(Array.IndexOf(DataView.ColumnNames, name));
        }

        #endregion

        #region Database Events

        private void TableViewColumnsDeleted(int[] columnIndices)
        {
            Array.Sort(columnIndices);
            for (int i = columnIndices.Length - 1; i >= 0; i--)
            {
                _selectedColumnIndices.Remove(columnIndices[i]);
                for (int j = 0; j < _selectedColumnIndices.Count; j++)
                    if (_selectedColumnIndices[j] > columnIndices[i]) _selectedColumnIndices[j] -= 1;

                if (_activeCellDataColumnIndex >= columnIndices[i]) _activeCellDataColumnIndex -= 1;
                if (_activeCellDataColumnIndex < 0) _activeCellDataColumnIndex = 0;

                if (_columnsSortedOrder![columnIndices[i]] != SortOrder.None) RemoveSort();

                var updated = new SortOrder[_columnsSortedOrder.Length - 1];
                for (int j = 0; j < columnIndices[i]; j++) updated[j] = _columnsSortedOrder[j];
                for (int j = columnIndices[i]; j < _columnsSortedOrder.Length - 1; j++) updated[j] = _columnsSortedOrder[j + 1];
                _columnsSortedOrder = updated;
            }
            UpdateUndoRedoButtons();
            NumberOfColumnsChanged();
        }

        private void TableViewColumnsAdded(int[] columnIndices)
        {
            Array.Sort(columnIndices);
            for (int i = 0; i < columnIndices.Length; i++)
            {
                for (int j = 0; j < _selectedColumnIndices.Count; j++)
                    if (_selectedColumnIndices[j] >= columnIndices[i]) _selectedColumnIndices[j] += 1;

                if (_activeCellDataColumnIndex >= columnIndices[i]) _activeCellDataColumnIndex += 1;
                if (_activeCellDataColumnIndex > DataView.ColumnNames.Count() - 1)
                    _activeCellDataColumnIndex = DataView.ColumnNames.Count() - 1;

                var updated = new SortOrder[_columnsSortedOrder!.Length + 1];
                for (int j = 0; j < columnIndices[i]; j++) updated[j] = _columnsSortedOrder[j];
                updated[columnIndices[i]] = SortOrder.None;
                for (int j = columnIndices[i] + 1; j < updated.Length; j++) updated[j] = _columnsSortedOrder[j - 1];
                _columnsSortedOrder = updated;
            }
            UpdateUndoRedoButtons();
            NumberOfColumnsChanged();
        }

        private void TableViewRowsAdded(int[] rowIndices)
        {
            Array.Sort(rowIndices);
            for (int i = 0; i < rowIndices.Length; i++)
            {
                for (int j = 0; j < _selectedDataRowIndices.Count; j++)
                    if (_selectedDataRowIndices[j] >= rowIndices[i]) _selectedDataRowIndices[j] += 1;

                if (_activeCellVirtualRowIndex >= rowIndices[i]) _activeCellVirtualRowIndex += 1;
                if (_activeCellVirtualRowIndex > DataView.NumberOfRows - 1)
                    _activeCellVirtualRowIndex = DataView.NumberOfRows - 1;
            }
            NumberOfRowsChanged();
            UpdateUndoRedoButtons();
        }

        private void TableViewRowsDeleted(int[] rowIndices)
        {
            Array.Sort(rowIndices);
            for (int i = rowIndices.Length - 1; i >= 0; i--)
            {
                _selectedDataRowIndices.Remove(rowIndices[i]);
                for (int j = 0; j < _selectedDataRowIndices.Count; j++)
                    if (_selectedDataRowIndices[j] > rowIndices[i]) _selectedDataRowIndices[j] -= 1;

                _selectedCellIndices.Remove(rowIndices[i]);

                if (_activeCellVirtualRowIndex >= rowIndices[i]) _activeCellVirtualRowIndex -= 1;
                if (_activeCellVirtualRowIndex < 0) _activeCellVirtualRowIndex = 0;
            }
            NumberOfRowsChanged();
            UpdateUndoRedoButtons();
        }

        #endregion

        #region Refresh Methods

        private void Refresh()
        {
            if (DataView == null) return;
            if (!DataView.ParentDatabase.DataBaseOpen) DataView.ParentDatabase.Open();

            _selectedCellIndices = new SortedDictionary<int, SortedSet<int>>();
            _selectedColumnIndices = new List<int>();
            _selectedDataRowIndices = new List<int>();
            _activeCellDataColumnIndex = 0;
            _activeCellVirtualRowIndex = 0;
            _selectedRowsOnly = false;

            _rowId = new int[DataView.NumberOfRows];
            _rowOffset = new int[_rowId.Length];
            for (int i = 0; i < _rowId.Length; i++) _rowId[i] = i;
            _rowId.CopyTo(_rowOffset, 0);

            ColumnHeadersGrid.SizeChanged -= ColumnsGridSizeChanged;

            GridPanel.Children.Clear();
            RowHeadersGrid.Children.Clear();
            RowColorGrid.Children.Clear();
            ColumnHeadersGrid.Children.Clear();
            GridPanel.RowDefinitions.Clear();
            RowHeadersGrid.RowDefinitions.Clear();
            RowColorGrid.RowDefinitions.Clear();
            ColumnHeadersGrid.ColumnDefinitions.Clear();

            _visibleRowCount = GetMaxRows(false);
            if (_visibleRowCount < 0) _visibleRowCount = 0;
            if (_visibleRowCount > DataView.NumberOfRows) _visibleRowCount = DataView.NumberOfRows;

            VerticalScrollbar.ValueChanged -= VerticalScrollBar_ValueChanged;
            VerticalScrollbar.Maximum = DataView.NumberOfRows - _visibleRowCount;
            VerticalScrollbar.ValueChanged += VerticalScrollBar_ValueChanged;
            VerticalScrollbar.ViewportSize = _visibleRowCount;

            if (_columnsSortedOrder == null || _columnsSortedOrder.Length != DataView.ColumnNames.Count())
                Array.Resize(ref _columnsSortedOrder, DataView.ColumnNames.Count());

            GridPanel.ColumnDefinitions.Clear();
            GridLinesCanvas.Children.Clear();
            CreateColumns();

            for (int i = 0; i < _columnsSortedOrder.Length; i++)
            {
                if (_columnsSortedOrder[i] == SortOrder.Ascending)
                    ((ColumnHeader)ColumnHeadersGrid.Children[i * 2]).AddSorter(true);
                else if (_columnsSortedOrder[i] == SortOrder.Descending)
                    ((ColumnHeader)ColumnHeadersGrid.Children[i * 2]).AddSorter(false);
            }

            var lengthBinding = new Binding("ActualWidth") { ElementName = "GridPanel" };
            double rowDistanceFromTop = RowHeight * GridPanel.RowDefinitions.Count - (RowLineThickness / 2);
            var rowLine = new Line
            {
                SnapsToDevicePixels = true, X1 = 0, Y1 = rowDistanceFromTop, Y2 = rowDistanceFromTop,
                StrokeThickness = RowLineThickness, Stroke = RowLineColor
            };
            BindingOperations.SetBinding(rowLine, Line.X2Property, lengthBinding);
            GridLinesCanvas.Children.Add(rowLine);

            LoadRows();
            this.UpdateLayout();
            DeSelectAllCells();
            SetSelectedCells();
            SetActiveCell();
            RefreshColumnWidths();
            UpdateRowHeaders();
        }

        /// <summary>
        /// Refreshes the view by adjusting visible rows based on the control's size.
        /// </summary>
        public void RefreshView()
        {
            if (!_isLoaded || DataView == null) return;
            int newNumberRows = GetMaxRows();
            if (newNumberRows != _visibleRowCount)
            {
                _visibleRowCount = newNumberRows;
                if (_selectedRowsOnly)
                {
                    if (_visibleRowCount > _selectedDataRowIndices.Count) _visibleRowCount = _selectedDataRowIndices.Count;
                    VerticalScrollbar.Maximum = _selectedDataRowIndices.Count - _visibleRowCount;
                }
                else
                {
                    if (_visibleRowCount > DataView.NumberOfRows) _visibleRowCount = DataView.NumberOfRows;
                    VerticalScrollbar.Maximum = DataView.NumberOfRows - _visibleRowCount;
                }
                VerticalScrollbar.ViewportSize = _visibleRowCount;
                LoadRows();
                SetSelectedCells();
                UpdateRowHeaders();
            }
            RefreshColumnWidths();
        }

        private void NumberOfRowsChanged()
        {
            _visibleRowCount = GetMaxRows();
            VerticalScrollbar.ValueChanged -= VerticalScrollBar_ValueChanged;

            if (_selectedRowsOnly)
            {
                if (_visibleRowCount > _selectedDataRowIndices.Count) _visibleRowCount = _selectedDataRowIndices.Count;
                VerticalScrollbar.Maximum = _selectedDataRowIndices.Count - _visibleRowCount;
            }
            else
            {
                if (_visibleRowCount > DataView.NumberOfRows) _visibleRowCount = DataView.NumberOfRows;
                VerticalScrollbar.Maximum = DataView.NumberOfRows - _visibleRowCount;
            }

            if ((int)Math.Floor(VerticalScrollbar.Value) > (int)VerticalScrollbar.Maximum)
                VerticalScrollbar.Value = (int)VerticalScrollbar.Maximum;

            VerticalScrollbar.ValueChanged += VerticalScrollBar_ValueChanged;

            _rowId = new int[DataView.NumberOfRows];
            _rowOffset = new int[_rowId.Length];

            if (_columnSortOrder == SortOrder.None)
            {
                for (int i = 0; i < _rowId.Length; i++) _rowId[i] = i;
                _rowId.CopyTo(_rowOffset, 0);
            }
            else
            {
                for (int i = 0; i < _columnsSortedOrder!.Length; i++)
                {
                    if (_columnsSortedOrder[i] == SortOrder.Ascending) SortColumn(i, true);
                    else if (_columnsSortedOrder[i] == SortOrder.Descending) SortColumn(i, false);
                }
            }

            VerticalScrollbar.ViewportSize = _visibleRowCount;
            LoadRows();

            if (ShowRowHeaders)
            {
                double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
                RowHeadersColumnDefinition.Width = new GridLength(
                    (int)(new FormattedText(DataView.NumberOfRows.ToString(), CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                        12, Brushes.Black, pixelsPerDip).Width) + 3);
            }

            UpdateRowHeaders();
            SetSelectedCells();
            if (DataView.NumberOfRows > 0) SetActiveCell(_activeCellVirtualRowIndex, _activeCellDataColumnIndex);
        }

        private void NumberOfColumnsChanged()
        {
            ColumnHeadersGrid.SizeChanged -= ColumnsGridSizeChanged;
            GridPanel.Children.Clear();
            RowHeadersGrid.Children.Clear();
            RowColorGrid.Children.Clear();
            ColumnHeadersGrid.Children.Clear();
            GridPanel.RowDefinitions.Clear();
            RowHeadersGrid.RowDefinitions.Clear();
            RowColorGrid.RowDefinitions.Clear();
            ColumnHeadersGrid.ColumnDefinitions.Clear();
            GridPanel.ColumnDefinitions.Clear();
            GridLinesCanvas.Children.Clear();
            CreateColumns();

            for (int i = 0; i < _columnsSortedOrder!.Length; i++)
            {
                if (_columnsSortedOrder[i] == SortOrder.Ascending)
                    ((ColumnHeader)ColumnHeadersGrid.Children[i * 2]).AddSorter(true);
                else if (_columnsSortedOrder[i] == SortOrder.Descending)
                    ((ColumnHeader)ColumnHeadersGrid.Children[i * 2]).AddSorter(false);
            }

            var lengthBinding = new Binding("ActualWidth") { ElementName = "GridPanel" };
            double rowDistanceFromTop = RowHeight * GridPanel.RowDefinitions.Count - (RowLineThickness / 2);
            var rowLine = new Line
            {
                SnapsToDevicePixels = true, X1 = 0, Y1 = rowDistanceFromTop, Y2 = rowDistanceFromTop,
                StrokeThickness = RowLineThickness, Stroke = RowLineColor
            };
            BindingOperations.SetBinding(rowLine, Line.X2Property, lengthBinding);
            GridLinesCanvas.Children.Add(rowLine);

            LoadRows();
            SetSelectedCells();
            RefreshColumnWidths();
            UpdateRowHeaders();
        }

        private int GetMaxRows(bool updateLayout = true)
        {
            if (updateLayout) this.UpdateLayout();
            int maxNumberRows;
            var parentWindow = Window.GetWindow(this);

            if ((parentWindow != null && parentWindow.WindowState == WindowState.Maximized) ||
                HorizontalScrollViewer.ActualHeight < HorizontalScrollViewer.ViewportHeight)
                maxNumberRows = (int)Math.Floor((HorizontalScrollViewer.ActualHeight - ColumnHeadersGrid.ActualHeight) / RowHeight) + 1;
            else
                maxNumberRows = (int)Math.Floor((HorizontalScrollViewer.ViewportHeight - ColumnHeadersGrid.ActualHeight) / RowHeight) + 1;

            if (VerticalScrollbar.Value == VerticalScrollbar.Maximum) maxNumberRows -= 1;
            return maxNumberRows < 0 ? 0 : maxNumberRows;
        }

        #endregion

        #region Grid Setup

        private void CreateColumns()
        {
            ColumnHeadersGrid.SizeChanged += ColumnsGridSizeChanged;
            _columnWidths = new GridLength[DataView.ColumnNames.Count()];
            double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            for (int i = 0; i < DataView.ColumnNames.Count(); i++)
            {
                ColumnDefinition colDef;
                if (AutoFitColumns)
                {
                    colDef = new ColumnDefinition();
                    _columnWidths[i] = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    int columnWidth = (int)(new FormattedText(DataView.ColumnNames[i], CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                        12, Brushes.Black, pixelsPerDip).Width) + 12;
                    colDef = new ColumnDefinition { Width = new GridLength(columnWidth) };
                    _columnWidths[i] = new GridLength(columnWidth);
                }
                ColumnHeadersGrid.ColumnDefinitions.Add(colDef);

                var header = new ColumnHeader(DataView.ColumnNames[i], DataView.ColumnTypes[i])
                {
                    HeaderTextblockStyle = ColumnHeaderTextblockStyle,
                    HeaderBorderStyle = ColumnHeaderBorderStyle
                };
                header.MouseRightButtonUp += CreateColumnContextMenu;
                Grid.SetColumn(header, i);
                ColumnHeadersGrid.Children.Add(header);

                if (i < DataView.ColumnNames.Count() - 1)
                {
                    var resizer = new GridSplitter
                    {
                        Background = Brushes.Transparent, Width = 10,
                        Margin = new Thickness(0, 0, -5, 0),
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Right
                    };
                    resizer.MouseDoubleClick += ResizeColumnSplitterDoubleClick;
                    resizer.DragDelta += ResizeColumnSplitterDragDelta;
                    resizer.DragCompleted += ResizeColumnSplitterDragComplete;
                    Grid.SetZIndex(resizer, 1);
                    Grid.SetColumn(resizer, i);
                    ColumnHeadersGrid.Children.Add(resizer);
                }

                GridPanel.ColumnDefinitions.Add(new ColumnDefinition());
            }

            if (DataView.ColumnNames.Count() == 0) return;

            var lengthBinding = new Binding(nameof(Grid.ActualHeight)) { Source = GridPanel };
            for (int i = 0; i <= DataView.ColumnNames.Count(); i++)
            {
                var gridLine = new Line
                {
                    SnapsToDevicePixels = true, X1 = 0, Y1 = 0, X2 = 0,
                    StrokeThickness = ColumnLineThickness, Stroke = ColumnLineColor
                };
                BindingOperations.SetBinding(gridLine, Line.Y2Property, lengthBinding);
                GridLinesCanvas.Children.Add(gridLine);
            }

            RefreshColumnWidths(false);
        }

        private void LoadRows()
        {
            if (GridLinesCanvas.Children.Count > DataView.ColumnNames.Count() + 2)
                GridLinesCanvas.Children.RemoveRange(DataView.ColumnNames.Count() + 2, GridLinesCanvas.Children.Count - 2 - DataView.ColumnNames.Count());

            GridPanel.Children.Clear();
            GridPanel.RowDefinitions.Clear();
            RowHeadersGrid.Children.Clear();
            RowHeadersGrid.RowDefinitions.Clear();
            RowColorGrid.Children.Clear();
            RowColorGrid.RowDefinitions.Clear();

            for (int i = 0; i < _visibleRowCount; i++) AddRow();
        }

        private void AddRow()
        {
            int rowIndex = GridPanel.RowDefinitions.Count;
            GridPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(RowHeight) });
            RowHeadersGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(RowHeight) });
            RowColorGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(RowHeight) });

            var rowColor = new Border { Background = (rowIndex % 2 == 0) ? RowColor : AlternateRowColor };
            Grid.SetRow(rowColor, rowIndex);
            RowColorGrid.Children.Add(rowColor);

            var rowHeader = new RowHeader { HeaderTextblockStyle = RowHeaderTextblockStyle, HeaderBorderStyle = RowHeaderBorderStyle };
            Grid.SetRow(rowHeader, rowIndex);
            RowHeadersGrid.Children.Add(rowHeader);

            for (int j = 0; j < DataView.ColumnNames.Count(); j++)
            {
                var cell = new Cell { CellStyle = CellTextblockStyle };
                Grid.SetRow(cell, rowIndex);
                Grid.SetColumn(cell, j);
                GridPanel.Children.Add(cell);
            }

            var lengthBinding = new Binding("ActualWidth") { ElementName = "GridPanel" };
            double rowDistanceFromTop = RowHeight * (rowIndex + 1) - (RowLineThickness / 2);
            var rowLine = new Line
            {
                SnapsToDevicePixels = true, X1 = 0, Y1 = rowDistanceFromTop, Y2 = rowDistanceFromTop,
                StrokeThickness = RowLineThickness, Stroke = RowLineColor
            };
            BindingOperations.SetBinding(rowLine, Line.X2Property, lengthBinding);
            GridLinesCanvas.Children.Add(rowLine);

            UpdateVisibleRows();
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates the text content of all visible cells from the DataView.
        /// </summary>
        public void UpdateVisibleRows()
        {
            if (DataView == null || _rowId == null) return;
            int firstRowIndex = (int)Math.Floor(VerticalScrollbar.Value);

            for (int i = 0; i < _visibleRowCount; i++)
            {
                int dataRowIndex = GetDataRowIndex(i);
                if (dataRowIndex < 0 || dataRowIndex >= DataView.NumberOfRows) continue;

                for (int j = 0; j < DataView.ColumnNames.Count(); j++)
                {
                    var cell = (Cell)GridPanel.Children[i * DataView.ColumnNames.Count() + j];
                    var value = DataView.GetCell(dataRowIndex, j);
                    cell.Text = value?.ToString() ?? "";
                }
            }
        }

        private void UpdateRowHeaders()
        {
            if (DataView == null || _rowId == null) return;
            for (int i = 0; i < _visibleRowCount; i++)
            {
                int dataRowIndex = GetDataRowIndex(i);
                if (i < RowHeadersGrid.Children.Count)
                    ((RowHeader)RowHeadersGrid.Children[i]).Text = (dataRowIndex + 1).ToString();
            }
        }

        private int GetDataRowIndex(int tableRowIndex)
        {
            int firstRowIndex = (int)Math.Floor(VerticalScrollbar.Value);
            if (_selectedRowsOnly)
            {
                if (_columnSortOrder == SortOrder.None)
                {
                    int selectedIndex = firstRowIndex + tableRowIndex;
                    return selectedIndex < _selectedDataRowIndices.Count ? _selectedDataRowIndices[selectedIndex] : -1;
                }
                else
                {
                    int sortedIndex = firstRowIndex + tableRowIndex;
                    return sortedIndex < _sortedSelectedRowOffsets!.Length ? _rowId![_sortedSelectedRowOffsets[sortedIndex]] : -1;
                }
            }
            return _rowId![firstRowIndex + tableRowIndex];
        }

        private string GetCellText(int tableRowIndex, int columnIndex)
        {
            int dataRowIndex = GetDataRowIndex(tableRowIndex);
            if (dataRowIndex < 0 || dataRowIndex >= DataView.NumberOfRows) return "";
            var value = DataView.GetCell(dataRowIndex, columnIndex);
            return value?.ToString() ?? "";
        }

        private int GetTableRowIndex(Point gridPosition)
        {
            int rowIndex = (int)Math.Floor(gridPosition.Y / RowHeight);
            if (rowIndex < 0) rowIndex = 0;
            if (rowIndex >= _visibleRowCount) rowIndex = _visibleRowCount - 1;
            return rowIndex;
        }

        private int GetTableColumnIndex(Point gridPosition)
        {
            double runningWidth = 0;
            for (int i = 0; i < DataView.ColumnNames.Count(); i++)
            {
                runningWidth += ColumnHeadersGrid.ColumnDefinitions[i].ActualWidth;
                if (gridPosition.X < runningWidth) return i;
            }
            return DataView.ColumnNames.Count() - 1;
        }

        #endregion

        #region Selection

        private void SetSelectedCells()
        {
            if (DataView == null) return;
            int firstRowIndex = (int)Math.Floor(VerticalScrollbar.Value);

            if (AllCellsSelected)
            {
                for (int i = 0; i < _visibleRowCount; i++)
                    for (int j = 0; j < DataView.ColumnNames.Count(); j++)
                        SelectCell(j, i);
                return;
            }

            foreach (var rowIndex in _selectedDataRowIndices)
            {
                int tableRow = _rowOffset![rowIndex] - firstRowIndex;
                if (tableRow >= 0 && tableRow < _visibleRowCount)
                    for (int j = 0; j < DataView.ColumnNames.Count(); j++)
                        SelectCell(j, tableRow);
            }

            foreach (var colIndex in _selectedColumnIndices)
                for (int i = 0; i < _visibleRowCount; i++)
                    SelectCell(colIndex, i);

            foreach (var kvp in _selectedCellIndices)
            {
                int tableRow = _rowOffset![kvp.Key] - firstRowIndex;
                if (tableRow >= 0 && tableRow < _visibleRowCount)
                    foreach (var colIndex in kvp.Value)
                        SelectCell(colIndex, tableRow);
            }
        }

        private void DeSelectAllCells()
        {
            for (int i = 0; i < DataView.ColumnNames.Count(); i++)
                for (int j = 0; j < GridPanel.RowDefinitions.Count; j++)
                    DeSelectCell(i, j);
        }

        private void SelectCell(int columnIndex, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _visibleRowCount) return;
            if (columnIndex < 0 || columnIndex >= DataView.ColumnNames.Count()) return;
            var cell = (Cell)GridPanel.Children[rowIndex * DataView.ColumnNames.Count() + columnIndex];
            cell.Background = SelectedColor;
            cell.Foreground = SelectedForegroundColor;
        }

        private void DeSelectCell(int columnIndex, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _visibleRowCount) return;
            if (columnIndex < 0 || columnIndex >= DataView.ColumnNames.Count()) return;
            var cell = (Cell)GridPanel.Children[rowIndex * DataView.ColumnNames.Count() + columnIndex];
            cell.Background = DeSelectedColor;
            cell.Foreground = DeSelectedForegroundColor;
        }

        private void SetActiveCell(int rowIndex = -1, int columnIndex = -1)
        {
            if (DataView == null || DataView.NumberOfRows == 0) return;
            if (rowIndex >= 0) _activeCellVirtualRowIndex = rowIndex;
            if (columnIndex >= 0) _activeCellDataColumnIndex = columnIndex;

            int firstRowIndex = (int)Math.Floor(VerticalScrollbar.Value);
            int tableRow = _activeCellVirtualRowIndex - firstRowIndex;

            if (tableRow >= 0 && tableRow < _visibleRowCount)
            {
                var cell = (Cell)GridPanel.Children[tableRow * DataView.ColumnNames.Count() + _activeCellDataColumnIndex];
                cell.Background = ActiveCellBackground;
                cell.Foreground = ActiveCellForeground;
            }
        }

        /// <summary>
        /// Sets the active cell to the specified row and column, optionally scrolling to make it visible.
        /// </summary>
        /// <param name="rowIndex">The zero-based row index of the cell to activate.</param>
        /// <param name="columnIndex">The zero-based column index of the cell to activate.</param>
        /// <param name="scroll">If <c>true</c>, scrolls the view to ensure the cell is visible.</param>
        public void SetActiveCell(int rowIndex, int columnIndex, bool scroll)
        {
            _activeCellVirtualRowIndex = rowIndex;
            _activeCellDataColumnIndex = columnIndex;
            if (scroll)
            {
                int firstRowIndex = (int)Math.Floor(VerticalScrollbar.Value);
                int lastRowIndex = firstRowIndex + _visibleRowCount - 1;
                if (rowIndex < firstRowIndex) VerticalScrollbar.Value = rowIndex;
                if (rowIndex > lastRowIndex) VerticalScrollbar.Value = rowIndex - _visibleRowCount + 1;
            }
            DeSelectAllCells();
            SetSelectedCells();
            SetActiveCell();
            ActiveCellLocationChanged?.Invoke();
        }

        /// <summary>
        /// Resets focus from the cell edit TextBox to the GridPanel if currently editing.
        /// </summary>
        public void SetEditingCell()
        {
            if (!_cellEditTextBox.IsFocused) return;
            GridPanel.Focus();
        }

        /// <summary>
        /// Programmatically updates the selected row indices and refreshes the selection display.
        /// </summary>
        /// <param name="newSelectedRowIndices">Array of row indices to select, or null to clear selection.</param>
        public void UpdateSelectedRowIndices(int[]? newSelectedRowIndices)
        {
            if (newSelectedRowIndices == null)
            {
                _selectedDataRowIndices.Clear();
            }
            else
            {
                _selectedDataRowIndices = newSelectedRowIndices.ToList();
                _selectedDataRowIndices.Sort();
            }

            DeSelectAllCells();
            if (!_selectedRowsOnly)
            {
                SetSelectedCells();
            }
            else
            {
                ShowAll_Checked(null, null);
                SetSelectedCells();
                ShowSelected_Checked(null, null);
            }

            UpdateSelectionButtonStates();
        }

        #endregion

        #region Resize Logic

        private void ResizeColumnSplitterDragComplete(object sender, DragCompletedEventArgs e) { }

        private void ResizeColumnSplitterDragDelta(object sender, DragDeltaEventArgs e)
        {
            var splitter = (GridSplitter)sender;
            int columnIndex = Grid.GetColumn(splitter);
            double newColumnWidth = ColumnHeadersGrid.ColumnDefinitions[columnIndex].ActualWidth + e.HorizontalChange;
            ResizeColumnWidth(columnIndex, (int)newColumnWidth);
        }

        private void ResizeColumnSplitterDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                var splitter = (GridSplitter)sender;
                int columnIndex = Grid.GetColumn(splitter);
                ResizeColumnWidth(columnIndex);
                Mouse.OverrideCursor = null;
            }
            catch { Mouse.OverrideCursor = null; }
        }

        /// <summary>
        /// Resizes a column to the specified width, or auto-fits to content if no width is specified.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column to resize.</param>
        /// <param name="columnWidth">The desired column width in pixels. If -1 or less than 6, the column auto-fits to its content.</param>
        public void ResizeColumnWidth(int columnIndex, int columnWidth = -1)
        {
            if (columnWidth > 5)
            {
                _columnWidths![columnIndex] = new GridLength(columnWidth);
            }
            else
            {
                var data = DataView.GetColumn(DataView.ColumnNames[columnIndex]);
                double pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;
                int max = (int)(new FormattedText(DataView.ColumnNames[columnIndex], CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                    12, Brushes.Black, pixelsPerDip).Width) + 12;

                int stringLength = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    string stringData = data[i]?.ToString() ?? "";
                    if (stringLength <= stringData.Length)
                    {
                        stringLength = stringData.Length;
                        int formattedWidth = (int)(new FormattedText(stringData, CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight, new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                            12, Brushes.Black, pixelsPerDip).Width) + 6;
                        if (formattedWidth > max) max = formattedWidth;
                    }
                }
                if (max < 6) max = 6;
                _columnWidths![columnIndex] = new GridLength(max);
            }
            RefreshColumnWidths();
        }

        private void ColumnsGridSizeChanged(object sender, SizeChangedEventArgs e) => RefreshColumnWidths();

        /// <summary>
        /// Recalculates and applies column widths to all columns in the grid.
        /// </summary>
        /// <param name="setActive">If <c>true</c>, resets the active cell styling after refresh. Default is <c>true</c>.</param>
        public void RefreshColumnWidths(bool setActive = true)
        {
            if (_columnWidths == null || DataView == null) return;
            double runningWidth = 0;
            int sumStar = 0;
            var columnWidths = new double[DataView.ColumnNames.Count()];

            for (int i = 0; i < DataView.ColumnNames.Count(); i++)
            {
                if (_columnWidths[i].IsAuto || _columnWidths[i].IsAbsolute)
                {
                    columnWidths[i] = _columnWidths[i].Value;
                    runningWidth += _columnWidths[i].Value;
                }
                else if (_columnWidths[i].IsStar)
                    sumStar += (int)_columnWidths[i].Value;
            }

            double remainingSpace = HorizontalScrollViewer.ActualWidth - ColumnLineThickness - runningWidth;
            for (int i = 0; i < DataView.ColumnNames.Count(); i++)
            {
                if (_columnWidths[i].IsStar)
                {
                    if (sumStar == 0) sumStar = (int)_columnWidths[i].Value;
                    double starredWidth = remainingSpace * _columnWidths[i].Value / sumStar;
                    if (starredWidth < 10) starredWidth = 10;
                    columnWidths[i] = starredWidth;
                }
            }

            runningWidth = 0;
            for (int i = 0; i < DataView.ColumnNames.Count(); i++)
            {
                ColumnHeadersGrid.ColumnDefinitions[i].Width = new GridLength(columnWidths[i]);
                GridPanel.ColumnDefinitions[i].Width = new GridLength(columnWidths[i]);
                runningWidth += columnWidths[i];
                ((Line)GridLinesCanvas.Children[i + 1]).X1 = runningWidth;
                ((Line)GridLinesCanvas.Children[i + 1]).X2 = runningWidth;
            }

            if (setActive) SetActiveCell();
        }

        #endregion

        #region Event Handlers

        private void TableViewer_Loaded(object sender, RoutedEventArgs e)
        {
            bool wasFalse = !_isLoaded;
            _isLoaded = true;
            if (wasFalse) RefreshView();
        }

        private void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateVisibleRows();
            UpdateRowHeaders();
            DeSelectAllCells();
            SetSelectedCells();
            SetActiveCell();
        }

        private void VerticalScrollbar_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) { }

        private void HorizontalScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e) => RefreshView();

        private void TestGridPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && VerticalScrollbar.Value > 0)
                VerticalScrollbar.Value -= 3;
            else if (e.Delta < 0 && VerticalScrollbar.Value < VerticalScrollbar.Maximum)
                VerticalScrollbar.Value += 3;
        }

        /// <summary>
        /// Handles the PreviewKeyDown event for keyboard navigation (PageUp, PageDown, Arrow keys).
        /// </summary>
        private void Grid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int firstRowDataIndex = (int)Math.Floor(VerticalScrollbar.Value);
            int lastRowDataIndex = firstRowDataIndex + _visibleRowCount - 1;
            int rowIndex = _activeCellVirtualRowIndex - firstRowDataIndex;

            if (e.Key == Key.PageDown)
            {
                if (lastRowDataIndex == DataView.NumberOfRows - 1) return;
                if (_cellEditTextBox.IsFocused) GridPanel.Focus();
                VerticalScrollbar.Value += _visibleRowCount;
                int maxRows = _selectedRowsOnly ? _selectedDataRowIndices.Count : DataView.NumberOfRows;
                if ((_activeCellVirtualRowIndex + _visibleRowCount) < maxRows - _visibleRowCount)
                    SetActiveCell(_activeCellVirtualRowIndex + _visibleRowCount, _activeCellDataColumnIndex);
                else
                    SetActiveCell((int)Math.Floor(VerticalScrollbar.Value) + rowIndex, _activeCellDataColumnIndex);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.PageUp)
            {
                if (firstRowDataIndex == 0) return;
                if (_cellEditTextBox.IsFocused) GridPanel.Focus();
                VerticalScrollbar.Value -= _visibleRowCount;
                if ((firstRowDataIndex - _visibleRowCount) >= 0)
                    SetActiveCell(_activeCellVirtualRowIndex - _visibleRowCount, _activeCellDataColumnIndex);
                else
                    SetActiveCell((int)Math.Floor(VerticalScrollbar.Value) + rowIndex, _activeCellDataColumnIndex);
                e.Handled = true;
                return;
            }

            if (_cellEditTextBox.IsFocused) return;

            if (e.Key == Key.Left)
            {
                if (_activeCellDataColumnIndex > 0) SetActiveCell(_activeCellVirtualRowIndex, _activeCellDataColumnIndex - 1);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Down)
            {
                int maxRows = _selectedRowsOnly ? _selectedDataRowIndices.Count : DataView.NumberOfRows;
                if (_activeCellVirtualRowIndex < maxRows - 1)
                    SetActiveCell(_activeCellVirtualRowIndex + 1, _activeCellDataColumnIndex);
                if (_activeCellVirtualRowIndex < firstRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex;
                if (_activeCellVirtualRowIndex > lastRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1;
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Right)
            {
                if (_activeCellDataColumnIndex < DataView.ColumnNames.Count() - 1)
                    SetActiveCell(_activeCellVirtualRowIndex, _activeCellDataColumnIndex + 1);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Up)
            {
                if (_activeCellVirtualRowIndex > 0)
                    SetActiveCell(_activeCellVirtualRowIndex - 1, _activeCellDataColumnIndex);
                if (_activeCellVirtualRowIndex < firstRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex;
                if (_activeCellVirtualRowIndex > lastRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1;
                e.Handled = true;
                return;
            }
        }

        /// <summary>
        /// Handles the KeyDown event for Tab, Enter, copy/paste, and cell editing.
        /// </summary>
        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (_cellEditTextBox.IsFocused) return;
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl) return;
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift) return;

            int firstRowDataIndex = (int)Math.Floor(VerticalScrollbar.Value);
            int lastRowDataIndex = firstRowDataIndex + _visibleRowCount - 1;

            if (e.Key == Key.Tab)
            {
                if (_activeCellDataColumnIndex < DataView.ColumnNames.Count() - 1)
                    SetActiveCell(_activeCellVirtualRowIndex, _activeCellDataColumnIndex + 1);
                if (_activeCellVirtualRowIndex < firstRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex;
                if (_activeCellVirtualRowIndex > lastRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1;
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                int maxRows = _selectedRowsOnly ? _selectedDataRowIndices.Count : DataView.NumberOfRows;
                if (_activeCellVirtualRowIndex < maxRows - 1)
                    SetActiveCell(_activeCellVirtualRowIndex + 1, _activeCellDataColumnIndex);
                if (_activeCellVirtualRowIndex < firstRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex;
                if (_activeCellVirtualRowIndex > lastRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1;
                e.Handled = true;
                return;
            }

            // Filter out non-character keys
            if ((e.Key < Key.NumPad0 || e.Key > Key.Divide) && (e.Key < Key.D0 || e.Key > Key.D9) &&
                (e.Key < Key.A || e.Key > Key.Z) && (e.Key < Key.Oem1 || e.Key > Key.Oem3) &&
                (e.Key < Key.OemOpenBrackets || e.Key > Key.OemQuotes) && e.Key != Key.Space)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                Clipboard.Clear();
                return;
            }

            // Copy/Paste Logic
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.V && Editable && !_selectedRowsOnly)
                {
                    if (Clipboard.ContainsText())
                    {
                        if (_activeCellVirtualRowIndex < firstRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex;
                        if (_activeCellVirtualRowIndex > lastRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1;
                        Paste();
                    }
                }
                else if (e.Key == Key.C)
                {
                    Copy();
                }
                else if (e.Key == Key.A)
                {
                    AllCellsSelected = true;
                    _selectedDataRowIndices.Clear();
                    _selectedCellIndices.Clear();
                    _selectedColumnIndices.Clear();
                    DeSelectAllCells();
                    SetSelectedCells();
                }
                else if (e.Key == Key.Z && Undo.IsEnabled && Undo.Visibility == Visibility.Visible)
                {
                    UndoLastEdit();
                }
                else if (e.Key == Key.Y && Redo.IsEnabled && Redo.Visibility == Visibility.Visible)
                {
                    RedoLastEdit();
                }
                return;
            }

            // Enter cell edit logic
            if (Editable && !_readOnlyColumns.Contains(_activeCellDataColumnIndex))
            {
                if (_activeCellVirtualRowIndex < firstRowDataIndex)
                {
                    VerticalScrollbar.Value = _activeCellVirtualRowIndex;
                    firstRowDataIndex = (int)Math.Floor(VerticalScrollbar.Value);
                }
                if (_activeCellVirtualRowIndex > lastRowDataIndex)
                {
                    VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1;
                    firstRowDataIndex = (int)Math.Floor(VerticalScrollbar.Value);
                }
                if (_activeCellVirtualRowIndex < 0) return;

                int rowIndex = _activeCellVirtualRowIndex - firstRowDataIndex;
                _mouseSelectionMode = SelectionMode.EditSelect;
                string initialText = GetCellText(rowIndex, _activeCellDataColumnIndex);
                _cellEditTextBox.Tag = new Tuple<string, Point>(initialText, new Point(_activeCellDataColumnIndex, GetDataRowIndex(rowIndex)));
                Grid.SetColumn(_cellEditTextBox, _activeCellDataColumnIndex);
                Grid.SetRow(_cellEditTextBox, rowIndex);
                if (GridPanel.Children.Contains(_cellEditTextBox)) GridPanel.Children.Remove(_cellEditTextBox);
                GridPanel.Children.Add(_cellEditTextBox);
                _cellEditTextBox.SelectAll();
                PreviewEditText(null, e);
                _cellEditTextBox.Focus();
            }
        }

        /// <summary>
        /// Handles special keys (Enter, Tab, Escape) during cell editing.
        /// </summary>
        private void PreviewEditText(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                GridPanel.Focus();
            }
            else if (e.Key == Key.Escape)
            {
                _cellEditTextBox.Text = GetCellText(Grid.GetRow(_cellEditTextBox), Grid.GetColumn(_cellEditTextBox));
                GridPanel.Focus();
            }
        }

        /// <summary>
        /// Handles the LostFocus event for the cell edit TextBox, saving changes to the data.
        /// </summary>
        private void EditTextLostFocus(object sender, RoutedEventArgs e)
        {
            var editBox = (TextBox)sender;
            int firstRowDataIndex = (int)Math.Floor(VerticalScrollbar.Value);
            int lastRowDataIndex = firstRowDataIndex + _visibleRowCount - 1;
            if (_activeCellVirtualRowIndex < firstRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex;
            if (_activeCellVirtualRowIndex > lastRowDataIndex) VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1;

            int columnIndex, rowIndex;
            if (editBox.Tag is not Tuple<string, Point> tagTuple)
            {
                columnIndex = Grid.GetColumn(editBox);
                rowIndex = _activeCellVirtualRowIndex - firstRowDataIndex;
                rowIndex = GetDataRowIndex(rowIndex);
            }
            else
            {
                columnIndex = (int)tagTuple.Item2.X;
                rowIndex = (int)tagTuple.Item2.Y;
            }

            GridPanel.Children.Remove(editBox);
            string newText = editBox.Text;
            var originalTag = editBox.Tag as Tuple<string, Point>;
            if (originalTag != null && originalTag.Item1 != newText)
            {
                try
                {
                    DataView.EditCell(rowIndex, columnIndex, newText);
                    if (_selectedRowsOnly)
                    {
                        int selectedRowOffset;
                        if (_columnSortOrder == SortOrder.None)
                            selectedRowOffset = _rowOffset![_selectedDataRowIndices.IndexOf(rowIndex)];
                        else
                            selectedRowOffset = Array.IndexOf(_sortedSelectedRowOffsets!, _rowOffset![rowIndex]);
                        SetCellText(selectedRowOffset - firstRowDataIndex, columnIndex, newText);
                    }
                    else
                    {
                        SetCellText(_rowOffset![rowIndex] - firstRowDataIndex, columnIndex, newText);
                    }
                    UpdateUndoRedoButtons();
                    SetActiveCell();
                    _mouseSelectionMode = SelectionMode.None;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event on the row headers grid to start row selection.
        /// </summary>
        private void RowsGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_selectedRowsOnly)
            {
                _mouseSelectionMode = SelectionMode.None;
                return;
            }

            AllCellsSelected = false;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl) &&
                !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                if (_selectedDataRowIndices.Count > 0)
                {
                    _selectedDataRowIndices.Clear();
                    SelectedRowIndicesChanged?.Invoke(_selectedDataRowIndices);
                }
                _selectedCellIndices.Clear();
                _selectedColumnIndices.Clear();
                DeSelectAllCells();
            }

            Point gridPosition = e.GetPosition(GridPanel);
            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                _mouseDownVirtualRowIndex = GetTableRowIndex(gridPosition) + (int)Math.Floor(VerticalScrollbar.Value);
                _activeCellVirtualRowIndex = _mouseDownVirtualRowIndex;
                double widthSums = 0;
                int counter = 0;
                while (widthSums <= HorizontalScrollViewer.HorizontalOffset && counter < ColumnHeadersGrid.ColumnDefinitions.Count)
                {
                    widthSums += ColumnHeadersGrid.ColumnDefinitions[counter].ActualWidth;
                    counter++;
                }
                _activeCellDataColumnIndex = Math.Max(0, counter - 1);
            }

            _mouseSelectionMode = RowSelectable ? SelectionMode.RowSelect : SelectionMode.None;
            ((UIElement)sender).CaptureMouse();
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event on the row headers grid to complete row selection.
        /// </summary>
        private void RowsGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AllCellsSelected = false;
            int verticalScrollBarValue = (int)Math.Floor(VerticalScrollbar.Value);
            Point gridPosition = e.GetPosition(GridPanel);
            int mouseUpVirtualRowIndex = GetTableRowIndex(gridPosition) + verticalScrollBarValue;

            if (_mouseSelectionMode == SelectionMode.RowSelect)
            {
                int rowStep = mouseUpVirtualRowIndex < _mouseDownVirtualRowIndex ? -1 : 1;
                for (int i = _mouseDownVirtualRowIndex; rowStep > 0 ? i <= mouseUpVirtualRowIndex : i >= mouseUpVirtualRowIndex; i += rowStep)
                    _selectedDataRowIndices.Add(_rowId![i]);
                _selectedDataRowIndices = _selectedDataRowIndices.Distinct().ToList();
                _selectedDataRowIndices.Sort();
                SetSelectedCells();
                SelectedRowIndicesChanged?.Invoke(_selectedDataRowIndices);
            }

            UpdateSelectionButtonStates();
            _mouseSelectionMode = SelectionMode.None;
            ((UIElement)sender).ReleaseMouseCapture();
        }

        /// <summary>
        /// Handles the MouseMove event on the row headers grid for drag selection of rows.
        /// </summary>
        private void RowsGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Point gridPosition = e.GetPosition(GridPanel);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_mouseSelectionMode == SelectionMode.None || _mouseSelectionMode == SelectionMode.EditSelect) return;

                AllCellsSelected = false;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    DeSelectAllCells();
                    SetSelectedCells();
                }
                else
                {
                    DeSelectAllCells();
                }

                int verticalScrollBarValue = (int)Math.Floor(VerticalScrollbar.Value);
                int mouseMoveDataRowIndex = GetTableRowIndex(gridPosition) + verticalScrollBarValue;

                if (_mouseSelectionMode == SelectionMode.RowSelect)
                {
                    int rowStep = mouseMoveDataRowIndex < _mouseDownVirtualRowIndex ? -1 : 1;
                    int startRow, endRow;

                    if (_mouseDownVirtualRowIndex < verticalScrollBarValue)
                    {
                        startRow = 0;
                        endRow = mouseMoveDataRowIndex - verticalScrollBarValue;
                    }
                    else if (_mouseDownVirtualRowIndex > verticalScrollBarValue + _visibleRowCount - 1)
                    {
                        startRow = _visibleRowCount - 1;
                        endRow = mouseMoveDataRowIndex - verticalScrollBarValue;
                    }
                    else
                    {
                        startRow = _mouseDownVirtualRowIndex - verticalScrollBarValue;
                        endRow = mouseMoveDataRowIndex - verticalScrollBarValue;
                    }

                    for (int i = startRow; rowStep > 0 ? i <= endRow : i >= endRow; i += rowStep)
                        for (int j = 0; j < DataView.ColumnNames.Count(); j++)
                            SelectCell(j, i);
                }
                SetActiveCell();
            }
        }

        /// <summary>
        /// Handles the MouseRightButtonUp event on the row headers grid for context menu.
        /// </summary>
        private void RowsGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            int dataRowIndex = GetDataRowIndex(GetTableRowIndex(e.GetPosition(GridPanel)));

            if (_selectedDataRowIndices.BinarySearch(dataRowIndex) < 0)
            {
                _selectedDataRowIndices.Clear();
                _selectedDataRowIndices.Add(dataRowIndex);
                DeSelectAllCells();
                SetSelectedCells();
                SelectedRowIndicesChanged?.Invoke(_selectedDataRowIndices);
            }

            var rowMenu = new ContextMenu();
            if (Editable)
            {
                string headerText = _selectedDataRowIndices.Count == 1 ? "Delete Row" : "Delete Rows";
                var rowMenuItem = new MenuItem
                {
                    Header = headerText,
                    IsEnabled = Editable,
                    Icon = new Image { Source = new BitmapImage(new Uri("pack://application:,,/GenericControls;component/Resources/delete_row.png")) }
                };
                rowMenuItem.Click += DeleteRows;
                rowMenu.Items.Add(rowMenuItem);
            }

            rowMenu.IsOpen = true;
            RowRightButtonUp?.Invoke(rowMenu, dataRowIndex);
        }

        private void RowsGrid_MouseWheel(object sender, MouseWheelEventArgs e) => TestGridPanel_MouseWheel(sender, e);

        /// <summary>
        /// Handles the MouseLeftButtonDown event on the column headers grid to start column selection.
        /// </summary>
        private void ColumnsGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AllCellsSelected = false;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl) &&
                !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift) && !_selectedRowsOnly)
            {
                if (_selectedDataRowIndices.Count > 0)
                {
                    _selectedDataRowIndices.Clear();
                    SelectedRowIndicesChanged?.Invoke(_selectedDataRowIndices);
                }
                _selectedCellIndices.Clear();
                _selectedColumnIndices.Clear();
                DeSelectAllCells();
            }

            Point gridPosition = e.GetPosition(GridPanel);
            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                _mouseDownColumnIndex = GetTableColumnIndex(gridPosition);
                _activeCellVirtualRowIndex = (int)Math.Floor(VerticalScrollbar.Value);
                _activeCellDataColumnIndex = _mouseDownColumnIndex;
            }

            if (_selectedRowsOnly)
                _mouseSelectionMode = SelectionMode.None;
            else if (ColumnSelectable)
                _mouseSelectionMode = SelectionMode.ColumnSelect;
            else
                _mouseSelectionMode = SelectionMode.None;

            // Double-click to sort
            if (e.ClickCount == 2)
            {
                _mouseSelectionMode = SelectionMode.None;
                _mouseDownColumnIndex = GetTableColumnIndex(gridPosition);
                if (!_selectedRowsOnly) _selectedColumnIndices.Add(_mouseDownColumnIndex);
                if (_columnSortOrder == SortOrder.Ascending)
                    SortColumnDescending();
                else
                    SortColumnAscending();
                return;
            }
            ((UIElement)sender).CaptureMouse();
        }

        /// <summary>
        /// Handles the MouseMove event on the column headers grid for drag selection of columns.
        /// </summary>
        private void ColumnsGrid_MouseMove(object sender, MouseEventArgs e)
        {
            Point gridPosition = e.GetPosition(GridPanel);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_mouseSelectionMode == SelectionMode.None) return;

                AllCellsSelected = false;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    DeSelectAllCells();
                    SetSelectedCells();
                }
                else
                {
                    DeSelectAllCells();
                }

                if (_mouseSelectionMode == SelectionMode.ColumnSelect)
                {
                    int columnIndex = GetTableColumnIndex(gridPosition);
                    int columnStep = columnIndex < _mouseDownColumnIndex ? -1 : 1;
                    for (int i = _mouseDownColumnIndex; columnStep > 0 ? i <= columnIndex : i >= columnIndex; i += columnStep)
                        for (int j = 0; j < _visibleRowCount; j++)
                            SelectCell(i, j);
                }
                SetActiveCell();
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event on the column headers grid to complete column selection.
        /// </summary>
        private void ColumnsGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AllCellsSelected = false;
            Point gridPosition = e.GetPosition(GridPanel);
            int mouseUpColumnIndex = GetTableColumnIndex(gridPosition);

            if (_mouseSelectionMode == SelectionMode.ColumnSelect)
            {
                int columnStep = mouseUpColumnIndex < _mouseDownColumnIndex ? -1 : 1;
                for (int i = _mouseDownColumnIndex; columnStep > 0 ? i <= mouseUpColumnIndex : i >= mouseUpColumnIndex; i += columnStep)
                    _selectedColumnIndices.Add(i);
                _selectedColumnIndices = _selectedColumnIndices.Distinct().ToList();
                _selectedColumnIndices.Sort();
                SetSelectedCells();
            }

            UpdateSelectionButtonStates();
            _mouseSelectionMode = SelectionMode.None;
            ((UIElement)sender).ReleaseMouseCapture();
        }

        private void SelectAllLeftMouseDown(object sender, MouseButtonEventArgs e) { }
        private void SelectAllLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            AllCellsSelected = !AllCellsSelected;
            DeSelectAllCells();
            SetSelectedCells();
            SetActiveCell();
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            AllCellsSelected = !AllCellsSelected;
            _selectedDataRowIndices.Clear();
            _selectedCellIndices.Clear();
            _selectedColumnIndices.Clear();
            DeSelectAllCells();
            SetSelectedCells();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e) => Copy();
        private void CopyWithHeadersButton_Click(object sender, RoutedEventArgs e) => Copy(true);
        private void PasteButton_Click(object sender, RoutedEventArgs e) => Paste();
        private void ExportTableButton_Click(object sender, RoutedEventArgs e) => ExportTable();

        /// <summary>
        /// Handles the ShowAll button click to show all rows instead of selected rows only.
        /// </summary>
        /// <param name="sender">The source of the event, or null if called programmatically.</param>
        /// <param name="e">The event arguments, or null if called programmatically.</param>
        private void ShowAll_Checked(object? sender, RoutedEventArgs? e)
        {
            _selectedRowsOnly = false;
            double rowsAreaHeight = HorizontalScrollViewer.ActualHeight - ColumnHeadersGrid.ActualHeight;
            VerticalScrollbar.Maximum = DataView.NumberOfRows - (int)Math.Floor(rowsAreaHeight / RowHeight);
            if (_selectedDataRowIndices.Count > 0)
            {
                VerticalScrollbar.Value = _selectedDataRowIndices[0] > VerticalScrollbar.Maximum
                    ? VerticalScrollbar.Maximum - 1
                    : _selectedDataRowIndices[0];
            }
            _visibleRowCount = (int)Math.Floor(rowsAreaHeight / RowHeight);
            if (_visibleRowCount > DataView.NumberOfRows) _visibleRowCount = DataView.NumberOfRows;
            LoadRows();
            SetSelectedCells();
            UpdateRowHeaders();
            ((Image)ShowAll.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/ClearSelectionIconDisabled_22x22.png"));
            ShowAll.IsEnabled = false;
            GridPanel.Focus();
        }

        /// <summary>
        /// Handles the ShowSelected button click to show only selected rows.
        /// </summary>
        /// <param name="sender">The source of the event, or null if called programmatically.</param>
        /// <param name="e">The event arguments, or null if called programmatically.</param>
        public void ShowSelected_Checked(object? sender, RoutedEventArgs? e)
        {
            if (_selectedDataRowIndices.Count > 0)
            {
                if (_columnSortOrder != SortOrder.None)
                {
                    _sortedSelectedRowOffsets = new int[_selectedDataRowIndices.Count];
                    for (int i = 0; i < _selectedDataRowIndices.Count; i++)
                        _sortedSelectedRowOffsets[i] = _rowOffset![_selectedDataRowIndices[i]];
                    Array.Sort(_sortedSelectedRowOffsets);
                    if (_columnSortOrder == SortOrder.Descending)
                        Array.Reverse(_sortedSelectedRowOffsets);
                }

                _selectedRowsOnly = true;
                ((Image)ShowAll.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/ShowAllIcon_22x22.png"));
                ShowAll.IsEnabled = true;
                if (_visibleRowCount > _selectedDataRowIndices.Count) _visibleRowCount = _selectedDataRowIndices.Count;
                VerticalScrollbar.Maximum = _selectedDataRowIndices.Count - _visibleRowCount;
                VerticalScrollbar.Value = 0;
                LoadRows();
                _activeCellDataColumnIndex = 0;
                _activeCellVirtualRowIndex = 0;
                SetSelectedCells();
                UpdateRowHeaders();
            }
            GridPanel.Focus();
        }

        /// <summary>
        /// Handles the DeSelectAll button click to clear all selections.
        /// </summary>
        private void DeSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDataRowIndices.Count > 0)
            {
                _selectedDataRowIndices.Clear();
                SelectedRowIndicesChanged?.Invoke(_selectedDataRowIndices);
            }
            DeSelectAllCells();
            SetSelectedCells();
            ShowSelected.IsEnabled = false;
            ((Image)ShowSelected.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/ClearSelectionIconDisabled_22x22.png"));
            DeSelectAll.IsEnabled = false;
            ((Image)DeSelectAll.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/ClearSelectionIconDisabled_22x22.png"));

            if (_selectedRowsOnly)
            {
                _selectedRowsOnly = false;
                _visibleRowCount = (int)Math.Floor((HorizontalScrollViewer.ActualHeight - ColumnHeadersGrid.ActualHeight) / RowHeight);
                if (_visibleRowCount > DataView.NumberOfRows) _visibleRowCount = DataView.NumberOfRows;
                VerticalScrollbar.Maximum = DataView.NumberOfRows - _visibleRowCount;
                LoadRows();
                SetSelectedCells();
                ((Image)ShowAll.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/ClearSelectionIconDisabled_22x22.png"));
                ShowAll.IsEnabled = false;
            }
            GridPanel.Focus();
        }

        /// <summary>
        /// Handles the SelectByAttribute button click to open the attribute selector dialog.
        /// </summary>
        private void SelectByAttribute_Click(object sender, RoutedEventArgs e)
        {
            var attributeSelector = new FieldCalculator(DataView, _selectedDataRowIndices, _readOnlyColumns, null, true);
            attributeSelector.ContentRendered += SelectorRendered;

            if (attributeSelector.ShowDialog() == true)
            {
                AllCellsSelected = false;
                _selectedDataRowIndices.Clear();
                _selectedCellIndices.Clear();
                _selectedColumnIndices.Clear();
                DeSelectAllCells();
                if (!_selectedRowsOnly)
                {
                    _selectedDataRowIndices = attributeSelector.GetSelectedRows;
                }
                else
                {
                    ShowAll_Checked(null, null);
                    _selectedDataRowIndices = attributeSelector.GetSelectedRows;
                    ShowSelected_Checked(null, null!);
                }
                SetSelectedCells();
                UpdateSelectionButtonStates();
                SelectedRowIndicesChanged?.Invoke(_selectedDataRowIndices);
                _attributeSelectorString = attributeSelector.ExpressionCalculator.GetExpressionText();
                attributeSelector.ContentRendered -= SelectorRendered;
            }
            GridPanel.Focus();
        }

        private void SelectorRendered(object? sender, EventArgs e)
        {
            if (sender is FieldCalculator fc && !string.IsNullOrEmpty(_attributeSelectorString))
                fc.ExpressionCalculator.SetExpressionText(_attributeSelectorString);
        }

        private void CalculatorRendered(object? sender, EventArgs e)
        {
            if (sender is FieldCalculator fc && !string.IsNullOrEmpty(_fieldCalculatorString))
                fc.ExpressionCalculator.SetExpressionText(_fieldCalculatorString);
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event on the GridPanel for cell selection.
        /// </summary>
        private void GridPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point gridPosition = e.GetPosition(GridPanel);

            if (_selectedRowsOnly)
            {
                // Re-select the previously active cell
                int firstRowDataIndex = (int)Math.Floor(VerticalScrollbar.Value);
                int lastRowDataIndex = firstRowDataIndex + _visibleRowCount - 1;
                if (_activeCellVirtualRowIndex >= firstRowDataIndex && _activeCellVirtualRowIndex <= lastRowDataIndex)
                    SelectCell(_activeCellDataColumnIndex, _activeCellVirtualRowIndex - firstRowDataIndex);

                _mouseSelectionMode = SelectionMode.None;
                _mouseDownColumnIndex = GetTableColumnIndex(gridPosition);
                _mouseDownVirtualRowIndex = (int)Math.Floor(VerticalScrollbar.Value) + GetTableRowIndex(gridPosition);
                _activeCellDataColumnIndex = _mouseDownColumnIndex;
                _activeCellVirtualRowIndex = _mouseDownVirtualRowIndex;
                ActiveCellLocationChanged?.Invoke();
                SetActiveCell();

                if (e.ClickCount == 2 && Editable && !_readOnlyColumns.Contains(_mouseDownColumnIndex))
                {
                    _mouseSelectionMode = SelectionMode.EditSelect;
                    int rowIndex = GetTableRowIndex(gridPosition);
                    string initialText = GetCellText(rowIndex, _mouseDownColumnIndex);
                    _cellEditTextBox.Tag = new Tuple<string, Point>(initialText, new Point(_mouseDownColumnIndex, GetDataRowIndex(rowIndex)));
                    Grid.SetColumn(_cellEditTextBox, _mouseDownColumnIndex);
                    Grid.SetRow(_cellEditTextBox, rowIndex);
                    _cellEditTextBox.Text = initialText;
                    if (GridPanel.Children.Contains(_cellEditTextBox)) GridPanel.Children.Remove(_cellEditTextBox);
                    GridPanel.Children.Add(_cellEditTextBox);
                    _cellEditTextBox.SelectAll();
                    return;
                }
                ((UIElement)sender).CaptureMouse();
                return;
            }

            AllCellsSelected = false;
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl) &&
                !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                if (_selectedDataRowIndices.Count > 0)
                {
                    _selectedDataRowIndices.Clear();
                    SelectedRowIndicesChanged?.Invoke(_selectedDataRowIndices);
                }
                _selectedCellIndices.Clear();
                _selectedColumnIndices.Clear();
                DeSelectAllCells();
            }

            if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
            {
                _mouseDownVirtualRowIndex = (int)Math.Floor(VerticalScrollbar.Value) + GetTableRowIndex(gridPosition);
                _mouseDownColumnIndex = GetTableColumnIndex(gridPosition);
                _activeCellDataColumnIndex = _mouseDownColumnIndex;
                _activeCellVirtualRowIndex = _mouseDownVirtualRowIndex;
                ActiveCellLocationChanged?.Invoke();
            }

            if (CellSelectable)
                _mouseSelectionMode = SelectionMode.CellSelect;
            else if (RowSelectable)
                _mouseSelectionMode = SelectionMode.RowSelect;
            else if (ColumnSelectable)
                _mouseSelectionMode = SelectionMode.ColumnSelect;
            else
                _mouseSelectionMode = SelectionMode.None;

            if (e.ClickCount == 2 && Editable && !_readOnlyColumns.Contains(_mouseDownColumnIndex))
            {
                _mouseSelectionMode = SelectionMode.EditSelect;
                int rowIndex = GetTableRowIndex(gridPosition);
                _mouseDownVirtualRowIndex = (int)Math.Floor(VerticalScrollbar.Value) + rowIndex;
                string initialText = GetCellText(rowIndex, _mouseDownColumnIndex);
                _cellEditTextBox.Tag = new Tuple<string, Point>(initialText, new Point(_mouseDownColumnIndex, GetDataRowIndex(rowIndex)));
                Grid.SetColumn(_cellEditTextBox, _mouseDownColumnIndex);
                Grid.SetRow(_cellEditTextBox, rowIndex);
                _cellEditTextBox.Text = initialText;
                if (GridPanel.Children.Contains(_cellEditTextBox)) GridPanel.Children.Remove(_cellEditTextBox);
                GridPanel.Children.Add(_cellEditTextBox);
                _cellEditTextBox.SelectAll();
                return;
            }
            ((UIElement)sender).CaptureMouse();
        }

        /// <summary>
        /// Handles the MouseMove event on the GridPanel for drag selection.
        /// </summary>
        private void GridPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (_mouseSelectionMode == SelectionMode.None || _mouseSelectionMode == SelectionMode.EditSelect) return;

                AllCellsSelected = false;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    DeSelectAllCells();
                    SetSelectedCells();
                }
                else
                {
                    DeSelectAllCells();
                }

                int verticalScrollBarValue = (int)Math.Floor(VerticalScrollbar.Value);
                Point gridPosition = e.GetPosition(GridPanel);

                if (_mouseSelectionMode == SelectionMode.CellSelect)
                {
                    int tableRowIndex = GetTableRowIndex(gridPosition);
                    int mouseMoveDataRowIndex = tableRowIndex + verticalScrollBarValue;
                    int mouseMoveColumnIndex = GetTableColumnIndex(gridPosition);
                    int rowStep = mouseMoveDataRowIndex >= _activeCellVirtualRowIndex ? 1 : -1;
                    int columnStep = mouseMoveColumnIndex >= _activeCellDataColumnIndex ? 1 : -1;
                    int startValue = _activeCellVirtualRowIndex - verticalScrollBarValue;
                    if (startValue < 0) startValue = 0;
                    if (startValue > _visibleRowCount - 1) startValue = _visibleRowCount - 1;

                    for (int i = _activeCellDataColumnIndex; columnStep > 0 ? i <= mouseMoveColumnIndex : i >= mouseMoveColumnIndex; i += columnStep)
                        for (int j = startValue; rowStep > 0 ? j <= (mouseMoveDataRowIndex - verticalScrollBarValue) : j >= (mouseMoveDataRowIndex - verticalScrollBarValue); j += rowStep)
                            SelectCell(i, j);
                }
                else if (_mouseSelectionMode == SelectionMode.RowSelect)
                {
                    int mouseMoveDataRowIndex = GetTableRowIndex(gridPosition) + verticalScrollBarValue;
                    int rowStep = mouseMoveDataRowIndex < _mouseDownVirtualRowIndex ? -1 : 1;
                    int startRow, endRow;

                    if (_mouseDownVirtualRowIndex < verticalScrollBarValue)
                    {
                        startRow = 0;
                        endRow = mouseMoveDataRowIndex - verticalScrollBarValue;
                    }
                    else if (_mouseDownVirtualRowIndex > verticalScrollBarValue + _visibleRowCount - 1)
                    {
                        startRow = _visibleRowCount - 1;
                        endRow = mouseMoveDataRowIndex - verticalScrollBarValue;
                    }
                    else
                    {
                        startRow = _mouseDownVirtualRowIndex - verticalScrollBarValue;
                        endRow = mouseMoveDataRowIndex - verticalScrollBarValue;
                    }

                    for (int i = startRow; rowStep > 0 ? i <= endRow : i >= endRow; i += rowStep)
                        for (int j = 0; j < DataView.ColumnNames.Count(); j++)
                            SelectCell(j, i);
                }
                else if (_mouseSelectionMode == SelectionMode.ColumnSelect)
                {
                    int columnIndex = GetTableColumnIndex(gridPosition);
                    int columnStep = columnIndex < _mouseDownColumnIndex ? -1 : 1;
                    for (int i = _mouseDownColumnIndex; columnStep > 0 ? i <= columnIndex : i >= columnIndex; i += columnStep)
                        for (int j = 0; j < _visibleRowCount; j++)
                            SelectCell(i, j);
                }
                SetActiveCell();
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event on the GridPanel to complete selection.
        /// </summary>
        private void GridPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AllCellsSelected = false;
            Point gridPosition = e.GetPosition(GridPanel);
            int verticalScrollBarValue = (int)Math.Floor(VerticalScrollbar.Value);
            int mouseUpDataRowIndex = GetTableRowIndex(gridPosition) + verticalScrollBarValue;
            int mouseUpColumnIndex = GetTableColumnIndex(gridPosition);

            if (_mouseSelectionMode == SelectionMode.CellSelect)
            {
                if (_activeCellVirtualRowIndex == mouseUpDataRowIndex && _activeCellDataColumnIndex == mouseUpColumnIndex)
                {
                    if (_selectedCellIndices.ContainsKey(_rowId![mouseUpDataRowIndex]))
                        _selectedCellIndices[_rowId[mouseUpDataRowIndex]].Add(mouseUpColumnIndex);
                    else
                        _selectedCellIndices.Add(_rowId[mouseUpDataRowIndex], new SortedSet<int> { mouseUpColumnIndex });
                }
                else
                {
                    int rowStep = mouseUpDataRowIndex >= _activeCellVirtualRowIndex ? 1 : -1;
                    int columnStep = mouseUpColumnIndex >= _activeCellDataColumnIndex ? 1 : -1;

                    for (int i = _activeCellVirtualRowIndex; rowStep > 0 ? i <= mouseUpDataRowIndex : i >= mouseUpDataRowIndex; i += rowStep)
                    {
                        if (!_selectedCellIndices.ContainsKey(_rowId![i]))
                            _selectedCellIndices.Add(_rowId[i], new SortedSet<int>());
                        for (int j = _activeCellDataColumnIndex; columnStep > 0 ? j <= mouseUpColumnIndex : j >= mouseUpColumnIndex; j += columnStep)
                            _selectedCellIndices[_rowId[i]].Add(j);
                    }
                }
                SetSelectedCells();
            }
            else if (_mouseSelectionMode == SelectionMode.RowSelect)
            {
                int rowStep = mouseUpDataRowIndex < _mouseDownVirtualRowIndex ? -1 : 1;
                for (int i = _mouseDownVirtualRowIndex; rowStep > 0 ? i <= mouseUpDataRowIndex : i >= mouseUpDataRowIndex; i += rowStep)
                    _selectedDataRowIndices.Add(_rowId![i]);
                _selectedDataRowIndices = _selectedDataRowIndices.Distinct().ToList();
                _selectedDataRowIndices.Sort();
                _mouseDownColumnIndex = _activeCellDataColumnIndex;
                _mouseDownVirtualRowIndex = _activeCellVirtualRowIndex;
                SetSelectedCells();
                SelectedRowIndicesChanged?.Invoke(_selectedDataRowIndices);
            }
            else if (_mouseSelectionMode == SelectionMode.ColumnSelect)
            {
                int columnStep = mouseUpColumnIndex < _mouseDownColumnIndex ? -1 : 1;
                for (int i = _mouseDownColumnIndex; columnStep > 0 ? i <= mouseUpColumnIndex : i >= mouseUpColumnIndex; i += columnStep)
                    _selectedColumnIndices.Add(i);
                _selectedColumnIndices = _selectedColumnIndices.Distinct().ToList();
                _selectedColumnIndices.Sort();
                SetSelectedCells();
            }
            else if (_mouseSelectionMode == SelectionMode.EditSelect)
            {
                _cellEditTextBox.Focus();
            }

            if (_mouseSelectionMode != SelectionMode.EditSelect)
            {
                UpdateSelectionButtonStates();
            }

            _mouseSelectionMode = SelectionMode.None;
            ((UIElement)sender).ReleaseMouseCapture();
        }

        private void CreateColumnContextMenu(object sender, MouseButtonEventArgs e)
        {
            var header = (ColumnHeader)sender;
            _mouseDownColumnIndex = Grid.GetColumn(header);
            var menu = new ContextMenu();

            var sortAsc = new MenuItem { Header = "Sort Ascending" };
            sortAsc.Click += (s, args) => SortColumnAscending();
            menu.Items.Add(sortAsc);

            var sortDesc = new MenuItem { Header = "Sort Descending" };
            sortDesc.Click += (s, args) => SortColumnDescending();
            menu.Items.Add(sortDesc);

            var removeSort = new MenuItem { Header = "Remove Sort" };
            removeSort.Click += (s, args) => RemoveSort();
            menu.Items.Add(removeSort);

            menu.IsOpen = true;
        }

        #endregion

        #region Sorting

        private void RemoveSort()
        {
            _columnSortOrder = SortOrder.None;
            for (int i = 0; i < DataView.ColumnNames.Count(); i++)
                _columnsSortedOrder![i] = SortOrder.None;

            for (int i = 0; i < _rowId!.Length; i++)
                _rowId[i] = i;
            _rowId.CopyTo(_rowOffset!, 0);

            UpdateVisibleRows();
            if (!_selectedRowsOnly) { DeSelectAllCells(); SetSelectedCells(); }
            ((ColumnHeader)ColumnHeadersGrid.Children[_mouseDownColumnIndex * 2]).RemoveSorter();
            UpdateRowHeaders();
        }

        private void SortColumnDescending()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                SortColumn(_mouseDownColumnIndex, false);
                _columnSortOrder = SortOrder.Descending;
                for (int i = 0; i < DataView.ColumnNames.Count(); i++)
                    _columnsSortedOrder![i] = SortOrder.None;
                _columnsSortedOrder![_mouseDownColumnIndex] = SortOrder.Descending;

                UpdateVisibleRows();
                for (int i = 0; i < DataView.ColumnNames.Count(); i++)
                    ((ColumnHeader)ColumnHeadersGrid.Children[i * 2]).RemoveSorter();
                ((ColumnHeader)ColumnHeadersGrid.Children[_mouseDownColumnIndex * 2]).AddSorter(false);
                UpdateRowHeaders();
                Mouse.OverrideCursor = null;
            }
            catch { Mouse.OverrideCursor = null; }
        }

        private void SortColumnAscending()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                SortColumn(_mouseDownColumnIndex, true);
                _columnSortOrder = SortOrder.Ascending;
                for (int i = 0; i < DataView.ColumnNames.Count(); i++)
                    _columnsSortedOrder![i] = SortOrder.None;
                _columnsSortedOrder![_mouseDownColumnIndex] = SortOrder.Ascending;

                UpdateVisibleRows();
                for (int i = 0; i < DataView.ColumnNames.Count(); i++)
                    ((ColumnHeader)ColumnHeadersGrid.Children[i * 2]).RemoveSorter();
                ((ColumnHeader)ColumnHeadersGrid.Children[_mouseDownColumnIndex * 2]).AddSorter(true);
                UpdateRowHeaders();
                Mouse.OverrideCursor = null;
            }
            catch { Mouse.OverrideCursor = null; }
        }

        /// <summary>
        /// Sorts the table data by the specified column using type-specific comparison.
        /// Handles all common data types including numeric, string, boolean, and DateTime.
        /// DBNull values are sorted to the beginning (ascending) or end (descending).
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column to sort by.</param>
        /// <param name="ascending">True to sort in ascending order, false for descending.</param>
        private void SortColumn(int columnIndex, bool ascending)
        {
            var columnData = DataView.GetColumn(columnIndex);
            var columnType = DataView.ColumnTypes[columnIndex];
            List<int> idx;

            // Type-specific sorting to ensure proper comparison (especially for numeric types)
            if (columnType == typeof(byte))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<byte, int>(Convert.IsDBNull(x) ? byte.MinValue : Convert.ToByte(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else if (columnType == typeof(short))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<short, int>(Convert.IsDBNull(x) ? short.MinValue : Convert.ToInt16(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else if (columnType == typeof(ushort))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<ushort, int>(Convert.IsDBNull(x) ? ushort.MinValue : Convert.ToUInt16(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else if (columnType == typeof(int))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<int, int>(Convert.IsDBNull(x) ? int.MinValue : Convert.ToInt32(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else if (columnType == typeof(uint))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<uint, int>(Convert.IsDBNull(x) ? uint.MinValue : Convert.ToUInt32(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else if (columnType == typeof(long))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<long, int>(Convert.IsDBNull(x) ? long.MinValue : Convert.ToInt64(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else if (columnType == typeof(ulong))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<ulong, int>(Convert.IsDBNull(x) ? ulong.MinValue : Convert.ToUInt64(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else if (columnType == typeof(float))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<float, int>(Convert.IsDBNull(x) ? float.NaN : Convert.ToSingle(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else if (columnType == typeof(double))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<double, int>(Convert.IsDBNull(x) ? double.NaN : Convert.ToDouble(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else if (columnType == typeof(decimal))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<decimal, int>(Convert.IsDBNull(x) ? decimal.MinValue : Convert.ToDecimal(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else if (columnType == typeof(DateTime))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<DateTime, int>(Convert.IsDBNull(x) ? DateTime.MinValue : Convert.ToDateTime(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else if (columnType == typeof(bool))
            {
                var sorted = columnData.Select((x, i) => new KeyValuePair<bool, int>(Convert.IsDBNull(x) ? false : Convert.ToBoolean(x), i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }
            else
            {
                // Default to string comparison for unknown types
                var sorted = columnData.Select((x, i) => new KeyValuePair<string, int>(x?.ToString() ?? "", i)).OrderBy(x => x.Key).ToList();
                idx = sorted.Select(x => x.Value).ToList();
            }

            if (!ascending) idx.Reverse();

            for (int i = 0; i < idx.Count; i++)
                _rowOffset![idx[i]] = i;
            idx.CopyTo(_rowId!);
        }

        #endregion

        #region Clipboard

        private void Copy(bool includeHeaders = false)
        {
            if (DataView == null) return;
            var sb = new System.Text.StringBuilder();

            if (includeHeaders)
            {
                for (int j = 0; j < DataView.ColumnNames.Count(); j++)
                {
                    sb.Append(DataView.ColumnNames[j]);
                    if (j < DataView.ColumnNames.Count() - 1) sb.Append("\t");
                }
                sb.AppendLine();
            }

            if (AllCellsSelected)
            {
                for (int i = 0; i < DataView.NumberOfRows; i++)
                {
                    for (int j = 0; j < DataView.ColumnNames.Count(); j++)
                    {
                        sb.Append(DataView.GetCell(i, j)?.ToString() ?? "");
                        if (j < DataView.ColumnNames.Count() - 1) sb.Append("\t");
                    }
                    sb.AppendLine();
                }
            }

            Clipboard.SetText(sb.ToString());
        }

        private void Paste()
        {
            if (!Clipboard.ContainsText() || !Editable) return;
            // TODO: Implement paste logic
        }

        private void ExportTable()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|Excel Files (*.xlsx)|*.xlsx",
                DefaultExt = ".csv"
            };

            if (dialog.ShowDialog() == true)
            {
                // TODO: Implement export
            }
        }

        #endregion

        #region Undo/Redo

        /// <summary>
        /// Updates the enabled state and icons of the Undo, Redo, and Save buttons.
        /// </summary>
        private void UpdateUndoRedoButtons()
        {
            if (DataView == null) return;

            if (DataView.CanUndo())
            {
                Undo.IsEnabled = true;
                SaveButton.IsEnabled = true;
                ((Image)Undo.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/Undo.png"));
                ((Image)SaveButton.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/Save.ico"));
            }
            else
            {
                Undo.IsEnabled = false;
                SaveButton.IsEnabled = false;
                ((Image)Undo.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/UndoDisabled.png"));
                ((Image)SaveButton.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/SaveDisabled.ico"));
            }

            if (DataView.CanRedo())
            {
                Redo.IsEnabled = true;
                ((Image)Redo.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/Redo.png"));
            }
            else
            {
                Redo.IsEnabled = false;
                ((Image)Redo.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/RedoDisabled.png"));
            }
        }

        /// <summary>
        /// Handles the Undo button click event.
        /// </summary>
        private void Undo_Click(object sender, RoutedEventArgs e) => UndoLastEdit();

        /// <summary>
        /// Handles the Redo button click event.
        /// </summary>
        private void Redo_Click(object sender, RoutedEventArgs e) => RedoLastEdit();

        /// <summary>
        /// Handles the Save button click event.
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save edits?", "Apply Edits", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                try
                {
                    DataView.ApplyEdits();
                    UpdateUndoRedoButtons();
                    Mouse.OverrideCursor = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error Saving Edits", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Mouse.OverrideCursor = null;
                }
            }
        }

        /// <summary>
        /// Handles the OpenFC (Field Calculator) button click event.
        /// </summary>
        private void OpenFC_Click(object sender, RoutedEventArgs e)
        {
            var fc = new FieldCalculator(DataView, _selectedDataRowIndices, _readOnlyColumns);
            fc.ContentRendered += CalculatorRendered;

            if (fc.ShowDialog() == true)
            {
                _fieldCalculatorString = fc.ExpressionCalculator.GetExpressionText();
                UpdateVisibleRows();
                UpdateUndoRedoButtons();
                fc.ContentRendered -= CalculatorRendered;
            }
        }

        /// <summary>
        /// Undoes the last edit operation.
        /// </summary>
        private void UndoLastEdit()
        {
            if (!DataView.CanUndo()) return;
            DataView.UndoEdit();
            UpdateVisibleRows();
            UpdateUndoRedoButtons();
        }

        /// <summary>
        /// Redoes the last undone edit operation.
        /// </summary>
        private void RedoLastEdit()
        {
            if (!DataView.CanRedo()) return;
            DataView.RedoEdit();
            UpdateVisibleRows();
            UpdateUndoRedoButtons();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Sets the text of a visible cell in the GridPanel.
        /// </summary>
        private void SetCellText(int tableRowIndex, int columnIndex, string text)
        {
            if (tableRowIndex < 0 || tableRowIndex >= _visibleRowCount) return;
            if (columnIndex < 0 || columnIndex >= DataView.ColumnNames.Count()) return;
            var cell = (Cell)GridPanel.Children[tableRowIndex * DataView.ColumnNames.Count() + columnIndex];
            cell.Text = text;
        }

        /// <summary>
        /// Updates the selection button states based on current selection.
        /// </summary>
        private void UpdateSelectionButtonStates()
        {
            if (_selectedDataRowIndices.Count > 0 && !_selectedRowsOnly)
            {
                ShowSelected.IsEnabled = true;
                ((Image)ShowSelected.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/ShowSelectedIcon_22x22.png"));
                DeSelectAll.IsEnabled = true;
                ((Image)DeSelectAll.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/ClearSelectionIcon_22x22.png"));
            }
            else if (_selectedDataRowIndices.Count <= 0 && _selectedRowsOnly)
            {
                ShowAll_Checked(null, null);
            }
            else if (_selectedRowsOnly)
            {
                // Do nothing - keep current state
            }
            else
            {
                ShowSelected.IsEnabled = false;
                ((Image)ShowSelected.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/ClearSelectionIconDisabled_22x22.png"));
                DeSelectAll.IsEnabled = false;
                ((Image)DeSelectAll.Content).Source = new BitmapImage(new Uri("pack://application:,,,/DatabaseControls;component/Resources/ClearSelectionIconDisabled_22x22.png"));
            }
        }

        /// <summary>
        /// Deletes the selected rows from the data view.
        /// </summary>
        private void DeleteRows(object sender, RoutedEventArgs e)
        {
            if (_selectedDataRowIndices.Count == 0) return;

            string message = _selectedDataRowIndices.Count == 1
                ? "Are you sure you want to delete the selected row?"
                : $"Are you sure you want to delete {_selectedDataRowIndices.Count} selected rows?";

            if (MessageBox.Show(message, "Delete Rows", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    // Delete rows in reverse order to maintain correct indices
                    var sortedIndices = _selectedDataRowIndices.OrderByDescending(i => i).ToList();
                    foreach (int rowIndex in sortedIndices)
                        DataView.DeleteRow(rowIndex);

                    _selectedDataRowIndices.Clear();
                    SelectedRowIndicesChanged?.Invoke(_selectedDataRowIndices);
                    RefreshView();
                    UpdateUndoRedoButtons();
                    UpdateSelectionButtonStates();
                    Mouse.OverrideCursor = null;
                }
                catch (Exception ex)
                {
                    Mouse.OverrideCursor = null;
                    MessageBox.Show(ex.Message, "Error Deleting Rows", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Nested Classes

        private class ColumnHeader : Border
        {
            public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                nameof(Text), typeof(string), typeof(ColumnHeader), new PropertyMetadata(""));

            public string Text
            {
                get => (string)GetValue(TextProperty);
                set => SetValue(TextProperty, value);
            }

            public static readonly DependencyProperty HeaderTextblockStyleProperty = DependencyProperty.Register(
                nameof(HeaderTextblockStyle), typeof(Style), typeof(ColumnHeader), new PropertyMetadata(null));

            public Style HeaderTextblockStyle
            {
                get => (Style)GetValue(HeaderTextblockStyleProperty);
                set => SetValue(HeaderTextblockStyleProperty, value);
            }

            public static readonly DependencyProperty HeaderBorderStyleProperty = DependencyProperty.Register(
                nameof(HeaderBorderStyle), typeof(Style), typeof(ColumnHeader), new PropertyMetadata(null));

            public Style HeaderBorderStyle
            {
                get => (Style)GetValue(HeaderBorderStyleProperty);
                set => SetValue(HeaderBorderStyleProperty, value);
            }

            private readonly Viewbox _sortDownViewBox = new Viewbox { Width = 9, Visibility = Visibility.Collapsed, Margin = new Thickness(2, 0, 2, 0) };
            private readonly Viewbox _sortUpViewBox = new Viewbox { Width = 9, Visibility = Visibility.Collapsed, Margin = new Thickness(2, 0, 2, 0) };

            public ColumnHeader(string columnName, Type columnType)
            {
                var g = new Grid();
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                g.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var downArrow = new Polygon { Points = new PointCollection { new Point(0, 0), new Point(8, 0), new Point(4, 6) }, Fill = Brushes.Black };
                _sortDownViewBox.Child = downArrow;
                g.Children.Add(_sortDownViewBox);

                var upArrow = new Polygon { Points = new PointCollection { new Point(4, 0), new Point(8, 6), new Point(0, 6) }, Fill = Brushes.Black };
                _sortUpViewBox.Child = upArrow;
                g.Children.Add(_sortUpViewBox);

                var tBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = Brushes.Transparent,
                    FontWeight = FontWeights.Bold,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                tBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Text)) { Source = this });
                tBlock.SetBinding(TextBlock.StyleProperty, new Binding(nameof(HeaderTextblockStyle)) { Source = this });
                Grid.SetColumn(tBlock, 1);
                g.Children.Add(tBlock);

                SetBinding(Border.StyleProperty, new Binding(nameof(HeaderBorderStyle)) { Source = this });
                Child = g;
                Text = columnName;

                string typeName = columnType.Name;
                ToolTip = $"{columnName}\nType: {typeName}";
            }

            public void RemoveSorter()
            {
                _sortDownViewBox.Visibility = Visibility.Collapsed;
                _sortUpViewBox.Visibility = Visibility.Collapsed;
            }

            public void AddSorter(bool ascending)
            {
                _sortDownViewBox.Visibility = ascending ? Visibility.Collapsed : Visibility.Visible;
                _sortUpViewBox.Visibility = ascending ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private class Cell : Border
        {
            public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                nameof(Text), typeof(string), typeof(Cell), new PropertyMetadata(""));

            public string Text
            {
                get => (string)GetValue(TextProperty);
                set => SetValue(TextProperty, value);
            }

            public static readonly DependencyProperty CellStyleProperty = DependencyProperty.Register(
                nameof(CellStyle), typeof(Style), typeof(Cell), new PropertyMetadata(null));

            public Style CellStyle
            {
                get => (Style)GetValue(CellStyleProperty);
                set => SetValue(CellStyleProperty, value);
            }

            public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
                nameof(Foreground), typeof(Brush), typeof(Cell), new PropertyMetadata(Brushes.Black));

            public Brush Foreground
            {
                get => (Brush)GetValue(ForegroundProperty);
                set => SetValue(ForegroundProperty, value);
            }

            public Cell()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch;
                VerticalAlignment = VerticalAlignment.Stretch;
                Background = Brushes.Transparent;
                IsHitTestVisible = false;

                var tBlock = new TextBlock();
                tBlock.SetBinding(TextBlock.StyleProperty, new Binding(nameof(CellStyle)) { Source = this });
                tBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Text)) { Source = this });
                tBlock.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(Foreground)) { Source = this });
                Child = tBlock;
            }
        }

        private class RowHeader : Border
        {
            public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
                nameof(Text), typeof(string), typeof(RowHeader), new PropertyMetadata(""));

            public string Text
            {
                get => (string)GetValue(TextProperty);
                set => SetValue(TextProperty, value);
            }

            public static readonly DependencyProperty HeaderTextblockStyleProperty = DependencyProperty.Register(
                nameof(HeaderTextblockStyle), typeof(Style), typeof(RowHeader), new PropertyMetadata(null));

            public Style HeaderTextblockStyle
            {
                get => (Style)GetValue(HeaderTextblockStyleProperty);
                set => SetValue(HeaderTextblockStyleProperty, value);
            }

            public static readonly DependencyProperty HeaderBorderStyleProperty = DependencyProperty.Register(
                nameof(HeaderBorderStyle), typeof(Style), typeof(RowHeader), new PropertyMetadata(null));

            public Style HeaderBorderStyle
            {
                get => (Style)GetValue(HeaderBorderStyleProperty);
                set => SetValue(HeaderBorderStyleProperty, value);
            }

            public RowHeader()
            {
                var tBlock = new TextBlock
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(2, 0, 0, 0),
                    Background = Brushes.Transparent
                };
                tBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Text)) { Source = this });
                tBlock.SetBinding(TextBlock.StyleProperty, new Binding(nameof(HeaderTextblockStyle)) { Source = this });
                SetBinding(Border.StyleProperty, new Binding(nameof(HeaderBorderStyle)) { Source = this });
                Child = tBlock;
            }
        }

        #endregion

        #region Default Styles

        private static Style GetDefaultCellTextblockStyle()
        {
            var s = new Style(typeof(TextBlock));
            s.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
            s.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left));
            s.Setters.Add(new Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis));
            s.Setters.Add(new Setter(TextBlock.BackgroundProperty, Brushes.Transparent));
            s.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Black));
            s.Setters.Add(new Setter(TextBlock.IsHitTestVisibleProperty, false));
            s.Setters.Add(new Setter(TextBlock.MarginProperty, new Thickness(2, 0, 2, 0)));
            return s;
        }

        private static Style GetDefaultColumnHeaderTextblockStyle()
        {
            var s = new Style(typeof(TextBlock));
            s.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis));
            s.Setters.Add(new Setter(TextBlock.BackgroundProperty, Brushes.Transparent));
            s.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Black));
            s.Setters.Add(new Setter(TextBlock.FontWeightProperty, FontWeights.Bold));
            s.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWithOverflow));
            s.Setters.Add(new Setter(TextBlock.IsHitTestVisibleProperty, false));
            return s;
        }

        private static Style GetDefaultColumnHeaderBorderStyle()
        {
            var columnHeaderBackground = new LinearGradientBrush(
                new GradientStopCollection { new GradientStop(SystemColors.ControlLightLightColor, 0.25), new GradientStop(Color.FromArgb(255, 237, 238, 243), 1) },
                new Point(0, 0), new Point(0, 1));

            var s = new Style(typeof(Border));
            s.Setters.Add(new Setter(Border.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
            s.Setters.Add(new Setter(Border.VerticalAlignmentProperty, VerticalAlignment.Stretch));
            s.Setters.Add(new Setter(Border.BackgroundProperty, columnHeaderBackground));
            s.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(0.5)));
            s.Setters.Add(new Setter(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(204, 206, 219))));
            s.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1)));
            s.Setters.Add(new Setter(Border.MarginProperty, new Thickness(-0.5)));
            s.Setters.Add(new Setter(Border.MinHeightProperty, 23.0));
            s.Setters.Add(new Setter(Border.SnapsToDevicePixelsProperty, true));

            var mouseOverTrigger = new Trigger { Property = IsMouseOverProperty, Value = true };
            mouseOverTrigger.Setters.Add(new Setter(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(201, 222, 245))));
            s.Triggers.Add(mouseOverTrigger);

            return s;
        }

        private static Style GetDefaultRowHeaderTextblockStyle()
        {
            var s = new Style(typeof(TextBlock));
            s.Setters.Add(new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left));
            s.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            s.Setters.Add(new Setter(TextBlock.BackgroundProperty, Brushes.Transparent));
            s.Setters.Add(new Setter(TextBlock.ForegroundProperty, Brushes.Black));
            s.Setters.Add(new Setter(TextBlock.MarginProperty, new Thickness(2, 0, 0, 0)));
            s.Setters.Add(new Setter(TextBlock.IsHitTestVisibleProperty, false));
            return s;
        }

        private static Style GetDefaultRowHeaderBorderStyle()
        {
            var rowHeaderBackground = new LinearGradientBrush(
                new GradientStopCollection { new GradientStop(SystemColors.ControlLightLightColor, 0.25), new GradientStop(Color.FromArgb(255, 237, 238, 243), 1) },
                new Point(0, 0), new Point(1, 0));

            var s = new Style(typeof(Border));
            s.Setters.Add(new Setter(Border.HorizontalAlignmentProperty, HorizontalAlignment.Stretch));
            s.Setters.Add(new Setter(Border.VerticalAlignmentProperty, VerticalAlignment.Stretch));
            s.Setters.Add(new Setter(Border.BackgroundProperty, rowHeaderBackground));
            s.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(0)));
            s.Setters.Add(new Setter(Border.BorderBrushProperty, new SolidColorBrush(Color.FromRgb(204, 206, 219))));
            s.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(1)));
            s.Setters.Add(new Setter(Border.MarginProperty, new Thickness(0, -0.5, 0, -0.5)));
            s.Setters.Add(new Setter(Border.SnapsToDevicePixelsProperty, true));
            return s;
        }

        #endregion
    }
}
