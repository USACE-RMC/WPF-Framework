Imports DatabaseManager
Imports Microsoft.Win32
Imports OxyplotControls
Imports System.Globalization
Imports System.Windows.Controls.Primitives

Public Class TableViewer

#Region "Variables"
    'Basic Information
    Private _visibleRowCount As Int32
    Private _isLoaded As Boolean = False

    'Selection Information
    Private _activeCellDataColumnIndex As Int32 = 0
    Private _activeCellVirtualRowIndex As Int32 = 0
    Private _mouseDownVirtualRowIndex As Int32
    Private _mouseDownColumnIndex As Int32
    Private _mouseSelectionMode As SelectionMode = SelectionMode.None
    Private _selectedDataRowIndices As New List(Of Int32)
    Private _selectedColumnIndices As New List(Of Int32)
    Private _selectedCellIndices As New SortedDictionary(Of Int32, SortedSet(Of Int32)) 'key=row index in database, value=set of column indices for selected cells in the row key
    'Private _allSelected As Boolean = False
    Private _selectedRowsOnly As Boolean = False
    Private _attributeSelectorString As String = ""
    'Sorting Information
    Private _rowOffset() As Int32 'for a given database row index, where it is in the table. if no sorting then all equal zero. 
    Private _rowId() As Int32 'for a given row index in the table (index in array), the row index in the database. If no sorting then 0=0,1=1,etc.
    Private _sortedSelectedRowOffsets() As Int32 'Used when selected rows only is being used.
    Private _columnSortOrder As SortOrder 'current sort order of the table (only one column can be sorted at a time).
    Private _columnsSortedOrder() As SortOrder 'sort order For Each column, only one column can be sorted at a time.
    Private _columnWidths() As GridLength
    'Editing Information
    Private ReadOnly _cellEditTextBox As TextBox
    Private _fieldCalculatorString As String = ""

    Private _readOnlyColumns As New HashSet(Of Int32)
    'Private _visibleColumnIndices(-1) As Int32 'This needs to always be in order.
    'Private _hiddenColumnIndices As New List(Of Int32)
    'Private _frozenColumnIndices As New List(Of Int32)
#End Region

#Region "Enumerables"
    Private Enum SelectionMode As Byte
        CellSelect = 0
        RowSelect = 1
        ColumnSelect = 2
        None = 3
        All = 4
        EditSelect = 5
    End Enum
    Private Enum SortOrder As Byte
        Ascending = 2
        Descending = 1
        None = 0
    End Enum
#End Region

#Region "Properties"

    Private Shared Function GetDummyTable() As DataTableView
        Dim dt As New Data.DataTable("Temp")
        dt.Columns.Add("Column 1", GetType(String))
        dt.Rows.Add({"Row 1"})
        dt.Rows.Add({"Row 2"})
        Return New InMemoryReader(dt).GetTableManager("Temp")
    End Function
    Public Shared ReadOnly DataViewProperty As DependencyProperty = DependencyProperty.Register(NameOf(DataView), GetType(DataTableView), GetType(TableViewer), New UIPropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf LoadView)))

    Public Property DataView As DataTableView
        Get
            Return CType(GetValue(DataViewProperty), DataTableView)
        End Get
        Set(value As DataTableView)
            SetValue(DataViewProperty, value)
        End Set
    End Property

    Public ReadOnly Property ShowingSelectedRowsOnly As Boolean
        Get
            Return _selectedRowsOnly
        End Get
    End Property

    Public ReadOnly Property GetSelectedRows As List(Of Int32)
        Get
            Return _selectedDataRowIndices
        End Get
    End Property

    Public ReadOnly Property ActiveCellRowIndex As Int32
        Get
            Return _activeCellVirtualRowIndex
        End Get
    End Property

    Public ReadOnly Property ActiveCellColumnIndex As Int32
        Get
            Return _activeCellDataColumnIndex
        End Get
    End Property

    Public Shared ReadOnly AllCellsSelectedProperty As DependencyProperty = DependencyProperty.Register(NameOf(AllCellsSelected), GetType(Boolean), GetType(TableViewer), New UIPropertyMetadata(False))
    Public Property AllCellsSelected As Boolean
        Get
            Return CType(GetValue(AllCellsSelectedProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(AllCellsSelectedProperty, value)
        End Set
    End Property

    Public Shared ReadOnly RowHeightProperty As DependencyProperty = DependencyProperty.Register(NameOf(RowHeight), GetType(Double), GetType(TableViewer), New UIPropertyMetadata(CDbl(23)))
    Public Property RowHeight As Double
        Get
            Return CType(GetValue(RowHeightProperty), Double)
        End Get
        Set(value As Double)
            SetValue(RowHeightProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ColumnHeaderHeightProperty As DependencyProperty = DependencyProperty.Register(NameOf(ColumnHeaderHeight), GetType(GridLength), GetType(TableViewer), New UIPropertyMetadata(GridLength.Auto))
    Public Property ColumnHeaderHeight As GridLength
        Get
            Return CType(GetValue(ColumnHeaderHeightProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(ColumnHeaderHeightProperty, value)
        End Set
    End Property
    Public Shared ReadOnly ColumnHeaderTextblockStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(ColumnHeaderTextblockStyle), GetType(Style), GetType(TableViewer), New PropertyMetadata(GetDefaultColumnHeaderTextblockStyle()))
    Public Property ColumnHeaderTextblockStyle As Style
        Get
            Return CType(GetValue(ColumnHeaderTextblockStyleProperty), Style)
        End Get
        Set(value As Style)
            SetValue(ColumnHeaderTextblockStyleProperty, value)
        End Set
    End Property
    Public Shared ReadOnly ColumnHeaderBorderStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(ColumnHeaderBorderStyle), GetType(Style), GetType(TableViewer), New PropertyMetadata(GetDefaultColumnHeaderBorderStyle()))
    Public Property ColumnHeaderBorderStyle As Style
        Get
            Return CType(GetValue(ColumnHeaderBorderStyleProperty), Style)
        End Get
        Set(value As Style)
            SetValue(ColumnHeaderBorderStyleProperty, value)
        End Set
    End Property

    Public Shared ReadOnly RowHeaderTextblockStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(RowHeaderTextblockStyle), GetType(Style), GetType(TableViewer), New PropertyMetadata(GetDefaultRowHeaderTextblockStyle()))
    Public Property RowHeaderTextblockStyle As Style
        Get
            Return CType(GetValue(RowHeaderTextblockStyleProperty), Style)
        End Get
        Set(value As Style)
            SetValue(RowHeaderTextblockStyleProperty, value)
        End Set
    End Property
    Public Shared ReadOnly RowHeaderBorderStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(RowHeaderBorderStyle), GetType(Style), GetType(TableViewer), New PropertyMetadata(GetDefaultRowHeaderBorderStyle()))
    Public Property RowHeaderBorderStyle As Style
        Get
            Return CType(GetValue(RowHeaderBorderStyleProperty), Style)
        End Get
        Set(value As Style)
            SetValue(RowHeaderBorderStyleProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ShowRowHeadersProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowRowHeaders), GetType(Boolean), GetType(TableViewer), New UIPropertyMetadata(True))
    Public Property ShowRowHeaders As Boolean
        Get
            Return CType(GetValue(ShowRowHeadersProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowRowHeadersProperty, value)
        End Set
    End Property

    Public Shared ReadOnly EditableProperty As DependencyProperty = DependencyProperty.Register(NameOf(Editable), GetType(Boolean), GetType(TableViewer), New UIPropertyMetadata(False))
    Public Property Editable As Boolean
        Get
            Return CType(GetValue(EditableProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(EditableProperty, value)
        End Set
    End Property
    Public Shared ReadOnly HasFieldCalculatorProperty As DependencyProperty = DependencyProperty.Register(NameOf(HasFieldCalculator), GetType(Boolean), GetType(TableViewer), New UIPropertyMetadata(True))
    Public Property HasFieldCalculator As Boolean
        Get
            Return CType(GetValue(HasFieldCalculatorProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(HasFieldCalculatorProperty, value)
        End Set
    End Property
    Public Shared ReadOnly HasSaveButtonProperty As DependencyProperty = DependencyProperty.Register(NameOf(HasSaveButton), GetType(Boolean), GetType(TableViewer), New UIPropertyMetadata(True))
    Public Property HasSaveButton As Boolean
        Get
            Return CType(GetValue(HasSaveButtonProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(HasSaveButtonProperty, value)
        End Set
    End Property
    Public Shared ReadOnly HasUndoRedoButtonsProperty As DependencyProperty = DependencyProperty.Register(NameOf(HasUndoRedoButtons), GetType(Boolean), GetType(TableViewer), New UIPropertyMetadata(True))
    Public Property HasUndoRedoButtons As Boolean
        Get
            Return CType(GetValue(HasUndoRedoButtonsProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(HasUndoRedoButtonsProperty, value)
        End Set
    End Property

    Public Shared ReadOnly RowSelectableProperty As DependencyProperty = DependencyProperty.Register(NameOf(RowSelectable), GetType(Boolean), GetType(TableViewer), New UIPropertyMetadata(True))
    Public Property RowSelectable As Boolean
        Get
            Return CType(GetValue(RowSelectableProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(RowSelectableProperty, value)
        End Set
    End Property
    Public Shared ReadOnly CellTextblockStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(CellTextblockStyle), GetType(Style), GetType(TableViewer), New PropertyMetadata(GetDefaultCellTextblockStyle()))
    Public Property CellTextblockStyle As Style
        Get
            Return CType(GetValue(CellTextblockStyleProperty), Style)
        End Get
        Set(value As Style)
            SetValue(CellTextblockStyleProperty, value)
        End Set
    End Property
    Public Shared ReadOnly SelectedColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedColor), GetType(Brush), GetType(TableViewer), New UIPropertyMetadata(New SolidColorBrush(Color.FromArgb(240, 0, 120, 215))))
    Public Property SelectedColor As Brush
        Get
            Return CType(GetValue(SelectedColorProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(SelectedColorProperty, value)
        End Set
    End Property
    Public Shared ReadOnly ActiveCellForegroundProperty As DependencyProperty = DependencyProperty.Register(NameOf(ActiveCellForeground), GetType(Brush), GetType(TableViewer), New UIPropertyMetadata(New SolidColorBrush(Colors.White)))
    Public Property ActiveCellForeground As Brush
        Get
            Return CType(GetValue(ActiveCellForegroundProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(ActiveCellForegroundProperty, value)
        End Set
    End Property
    Public Shared ReadOnly ActiveCellBackgroundProperty As DependencyProperty = DependencyProperty.Register(NameOf(ActiveCellBackground), GetType(Brush), GetType(TableViewer), New UIPropertyMetadata(New SolidColorBrush(Color.FromArgb(255, 21, 107, 176))))
    Public Property ActiveCellBackground As Brush
        Get
            Return CType(GetValue(ActiveCellBackgroundProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(ActiveCellBackgroundProperty, value)
        End Set
    End Property

    Public Shared ReadOnly DeSelectedColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(DeSelectedColor), GetType(Brush), GetType(TableViewer), New UIPropertyMetadata(Brushes.Transparent))
    Public Property DeSelectedColor As Brush
        Get
            Return CType(GetValue(DeSelectedColorProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(DeSelectedColorProperty, value)
        End Set
    End Property
    Public Shared ReadOnly SelectedForegroundColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedForegroundColor), GetType(Brush), GetType(TableViewer), New UIPropertyMetadata(New SolidColorBrush(Colors.White)))
    Public Property SelectedForegroundColor As Brush
        Get
            Return CType(GetValue(SelectedForegroundColorProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(SelectedForegroundColorProperty, value)
        End Set
    End Property
    Public Shared ReadOnly DeSelectedForegroundColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(DeSelectedForegroundColor), GetType(Brush), GetType(TableViewer), New UIPropertyMetadata(Brushes.Black))
    Public Property DeSelectedForegroundColor As Brush
        Get
            Return CType(GetValue(DeSelectedForegroundColorProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(DeSelectedForegroundColorProperty, value)
        End Set
    End Property
    Public Shared ReadOnly RowColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(RowColor), GetType(Brush), GetType(TableViewer), New UIPropertyMetadata(Brushes.White))
    Public Property RowColor As Brush
        Get
            Return CType(GetValue(RowColorProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(RowColorProperty, value)
        End Set
    End Property
    Public Shared ReadOnly AlternateRowColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(AlternateRowColor), GetType(Brush), GetType(TableViewer), New UIPropertyMetadata(New SolidColorBrush(Color.FromArgb(255, 243, 249, 247))))
    Public Property AlternateRowColor As Brush
        Get
            Return CType(GetValue(AlternateRowColorProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(AlternateRowColorProperty, value)
        End Set
    End Property
    Public Shared ReadOnly RowLineColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(RowLineColor), GetType(Brush), GetType(TableViewer), New UIPropertyMetadata(New SolidColorBrush(Color.FromArgb(255, 53, 59, 122))))
    Public Property RowLineColor As Brush
        Get
            Return CType(GetValue(RowLineColorProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(RowLineColorProperty, value)
        End Set
    End Property

    Public Shared ReadOnly RowLineThicknessProperty As DependencyProperty = DependencyProperty.Register(NameOf(RowLineThickness), GetType(Double), GetType(TableViewer), New UIPropertyMetadata(CDbl(1)))
    Public Property RowLineThickness As Double
        Get
            Return CType(GetValue(RowLineThicknessProperty), Double)
        End Get
        Set(value As Double)
            SetValue(RowLineThicknessProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ColumnLineColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(ColumnLineColor), GetType(Brush), GetType(TableViewer), New UIPropertyMetadata(New SolidColorBrush(Color.FromArgb(255, 53, 59, 122))))
    Public Property ColumnLineColor As Brush
        Get
            Return CType(GetValue(ColumnLineColorProperty), Brush)
        End Get
        Set(value As Brush)
            SetValue(ColumnLineColorProperty, value)
        End Set
    End Property
    Public Shared ReadOnly ColumnLineThicknessProperty As DependencyProperty = DependencyProperty.Register(NameOf(ColumnLineThickness), GetType(Double), GetType(TableViewer), New UIPropertyMetadata(CDbl(1)))
    Public Property ColumnLineThickness As Double
        Get
            Return CType(GetValue(ColumnLineThicknessProperty), Double)
        End Get
        Set(value As Double)
            SetValue(ColumnLineThicknessProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ColumnSelectableProperty As DependencyProperty = DependencyProperty.Register(NameOf(ColumnSelectable), GetType(Boolean), GetType(TableViewer), New UIPropertyMetadata(True))

    Public Property ColumnSelectable As Boolean
        Get
            Return CType(GetValue(ColumnSelectableProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ColumnSelectableProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CellSelectableProperty As DependencyProperty = DependencyProperty.Register(NameOf(CellSelectable), GetType(Boolean), GetType(TableViewer), New UIPropertyMetadata(True))
    Public Property CellSelectable As Boolean
        Get
            Return CType(GetValue(CellSelectableProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(CellSelectableProperty, value)
        End Set
    End Property

    Public Shared ReadOnly AutoFitColumnsProperty As DependencyProperty = DependencyProperty.Register(NameOf(AutoFitColumns), GetType(Boolean), GetType(TableViewer), New UIPropertyMetadata(False))
    Public Property AutoFitColumns As Boolean
        Get
            Return CType(GetValue(AutoFitColumnsProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(AutoFitColumnsProperty, value)
        End Set
    End Property
#End Region

#Region "Events"
    'Public Event SelectionModeChanged(ByVal selectionmode As SelectionMode)
    Public Event SelectedRowIndicesChanged(selectedRowIndices As List(Of Int32))
    Public Event ActiveCellLocationChanged()
    Public Event RowRightButtonUp(rowUpMenu As ContextMenu, dataRowIndex As Int32)
    'Public Event EditsSaved()
#End Region

#Region "Initializing"
    Public Sub New()

        InitializeComponent()
        ' 
        _cellEditTextBox = New TextBox With {.Padding = New Thickness(0, 3, 0, 2)} ', .Margin = New Thickness(1, 1, 0, 0)}
        AddHandler _cellEditTextBox.PreviewKeyDown, AddressOf PreviewEditText
        AddHandler _cellEditTextBox.LostFocus, AddressOf EditTextLostFocus
        EditorToolbar.IsEnabled = False
    End Sub
#End Region

#Region "Loading and Unloading Process"
    Private Shared Sub LoadView(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(TableViewer) Then Exit Sub
        Dim thisControl = DirectCast(d, TableViewer)
        'clear old handlers
        If Not IsNothing(e.OldValue) AndAlso e.OldValue.GetType.BaseType <> GetType(DataTableView) Then
            Dim oldView As DataTableView = DirectCast(e.OldValue, DataTableView)
            RemoveHandler oldView.RowsAdded, AddressOf thisControl.TableViewRowsAdded
            RemoveHandler oldView.RowsDeleted, AddressOf thisControl.TableViewRowsDeleted
            RemoveHandler oldView.ColumnsAdded, AddressOf thisControl.TableViewColumnsAdded
            RemoveHandler oldView.ColumnsDeleted, AddressOf thisControl.TableViewColumnsDeleted
        End If
        '
        If IsNothing(e.NewValue) OrElse e.NewValue.GetType.BaseType <> GetType(DataTableView) Then
            thisControl._selectedCellIndices = New SortedDictionary(Of Int32, SortedSet(Of Integer)) 'List(Of Integer))
            thisControl._selectedColumnIndices = New List(Of Int32)
            thisControl._selectedDataRowIndices = New List(Of Int32)
            thisControl._activeCellDataColumnIndex = 0
            thisControl._activeCellVirtualRowIndex = 0
            thisControl._selectedRowsOnly = False
            '
            RemoveHandler thisControl.ColumnHeadersGrid.SizeChanged, AddressOf thisControl.ColumnsGridSizeChanged
            '
            thisControl.GridPanel.Children.Clear()
            thisControl.RowHeadersGrid.Children.Clear()
            thisControl.RowColorGrid.Children.Clear()
            thisControl.ColumnHeadersGrid.Children.Clear()
            thisControl.GridPanel.RowDefinitions.Clear()
            thisControl.RowHeadersGrid.RowDefinitions.Clear()
            thisControl.RowColorGrid.RowDefinitions.Clear()
            thisControl.ColumnHeadersGrid.ColumnDefinitions.Clear()
            '
            thisControl.GridPanel.ColumnDefinitions.Clear()
            thisControl.GridLinesCanvas.Children.Clear()
            '
            thisControl.SelectionToolbar.IsEnabled = False
            thisControl.EditorToolbar.IsEnabled = False
        Else
            Dim newView As DataTableView = DirectCast(e.NewValue, DataTableView)
            AddHandler newView.RowsAdded, AddressOf thisControl.TableViewRowsAdded
            AddHandler newView.RowsDeleted, AddressOf thisControl.TableViewRowsDeleted
            AddHandler newView.ColumnsAdded, AddressOf thisControl.TableViewColumnsAdded
            AddHandler newView.ColumnsDeleted, AddressOf thisControl.TableViewColumnsDeleted
            If newView.ParentDatabase.DataBaseOpen = False Then newView.ParentDatabase.Open()
            '
            thisControl._columnSortOrder = SortOrder.None
            ReDim thisControl._columnsSortedOrder(newView.ColumnNames.Count - 1)
            'ReDim thisControl._visibleColumnIndices(newView.ColumnNames.Count - 1)
            For i As Int32 = 0 To newView.ColumnNames.Count - 1
                thisControl._columnsSortedOrder(i) = SortOrder.None
                'thisControl._visibleColumnIndices(i) = i
            Next
            '
            thisControl.Refresh()
            If thisControl.ShowRowHeaders = True Then
                Dim pixelsPerDip = VisualTreeHelper.GetDpi(thisControl).PixelsPerDip
                thisControl.RowHeadersColumnDefinition.Width = New GridLength(CInt(New FormattedText(newView.NumberOfRows.ToString, Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, New Typeface(New FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 12, Brushes.Black, pixelsPerDip).Width) + 3)
            End If
            thisControl.SelectionToolbar.IsEnabled = True
            thisControl.EditorToolbar.IsEnabled = True
        End If
    End Sub

    Public Sub SetColumnsAsReadOnly(columnIndices() As Int32)
        For i As Int32 = 0 To columnIndices.Count - 1
            _readOnlyColumns.Add(columnIndices(i))
        Next
    End Sub
    Public Sub SetColumnsAsReadOnly(columnNames() As String)
        For i As Int32 = 0 To columnNames.Count - 1
            _readOnlyColumns.Add(Array.IndexOf(DataView.ColumnNames, columnNames(i)))
        Next
    End Sub
#Region "Database Updated Events e.g. columns/rows added"
    Private Sub TableViewColumnsDeleted(columnIndices() As Int32)
        Array.Sort(columnIndices)
        For i As Int32 = columnIndices.Count - 1 To 0 Step -1
            'update selected column indices
            _selectedColumnIndices.Remove(columnIndices(i))
            For j As Int32 = 0 To _selectedColumnIndices.Count - 1
                If _selectedColumnIndices(j) > columnIndices(i) Then _selectedColumnIndices(j) -= 1
            Next
            'Delete column from selected cells collection
            Dim keysToRemove As New List(Of Int32)
            For Each selectedCellsByRow As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
                selectedCellsByRow.Value.Remove(columnIndices(i))
                If selectedCellsByRow.Value.Count = 0 Then keysToRemove.Add(selectedCellsByRow.Key) '_selectedCellIndices.Remove(selectedCellsByRow.Key)
            Next
            For Each keyToRemove As Int32 In keysToRemove
                _selectedCellIndices.Remove(keyToRemove)
            Next
            For Each selectedCellsByRow As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
                Dim columnsToUpdate As New List(Of Int32)
                For Each selectedCellColumn As Int32 In selectedCellsByRow.Value
                    If selectedCellColumn >= columnIndices(i) Then columnsToUpdate.Add(selectedCellColumn)
                Next
                For Each columnToUpdate As Int32 In columnsToUpdate
                    selectedCellsByRow.Value.Remove(columnToUpdate)
                Next
                For Each columnToUpdate As Int32 In columnsToUpdate
                    selectedCellsByRow.Value.Add(columnToUpdate - 1)
                Next
            Next
            'Update active cell
            If _activeCellDataColumnIndex >= columnIndices(i) Then _activeCellDataColumnIndex -= 1
            If _activeCellDataColumnIndex < 0 Then _activeCellDataColumnIndex = 0
            'Update sort order
            If _columnsSortedOrder(columnIndices(i)) <> SortOrder.None Then RemoveSort()
            Dim updatedcolumnsortorder(_columnsSortedOrder.Count - 2) As SortOrder
            For j As Int32 = 0 To columnIndices(i) - 1
                updatedcolumnsortorder(j) = _columnsSortedOrder(j)
            Next
            For j As Int32 = columnIndices(i) To _columnsSortedOrder.Count - 2
                updatedcolumnsortorder(j) = _columnsSortedOrder(j + 1)
            Next
            _columnsSortedOrder = updatedcolumnsortorder
        Next
        'update undo and redo buttons if needed
        UpdateUndoRedoButtons()
        'refresh viewer to include new column
        NumberOfColumnsChanged()
    End Sub

    Private Sub TableViewColumnsAdded(columnIndices() As Int32)
        Array.Sort(columnIndices)
        For i As Int32 = 0 To columnIndices.Count - 1
            'update selected column indices
            For j As Int32 = 0 To _selectedColumnIndices.Count - 1
                If _selectedColumnIndices(j) >= columnIndices(i) Then _selectedColumnIndices(j) += 1
            Next
            'Delete column from selected cells collection
            For Each selectedCellsByRow As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
                Dim columnsToUpdate As New List(Of Int32)
                For Each selectedCellColumn As Int32 In selectedCellsByRow.Value
                    If selectedCellColumn >= columnIndices(i) Then columnsToUpdate.Add(selectedCellColumn)
                Next
                For Each columnToUpdate As Int32 In columnsToUpdate
                    selectedCellsByRow.Value.Remove(columnToUpdate)
                Next
                For Each columnToUpdate As Int32 In columnsToUpdate
                    selectedCellsByRow.Value.Add(columnToUpdate + 1)
                Next
            Next
            'Update active cell
            If _activeCellDataColumnIndex >= columnIndices(i) Then _activeCellDataColumnIndex += 1
            If _activeCellDataColumnIndex > DataView.ColumnNames.Count - 1 Then _activeCellDataColumnIndex = DataView.ColumnNames.Count - 1
            'Update sort order
            Dim updatedcolumnsortorder(_columnsSortedOrder.Count) As SortOrder
            For j As Int32 = 0 To columnIndices(i) - 1
                updatedcolumnsortorder(j) = _columnsSortedOrder(j)
            Next
            updatedcolumnsortorder(columnIndices(i)) = SortOrder.None
            For j = columnIndices(i) + 1 To updatedcolumnsortorder.Count - 1
                updatedcolumnsortorder(j) = _columnsSortedOrder(j - 1)
            Next
            _columnsSortedOrder = updatedcolumnsortorder
        Next
        'update undo and redo buttons if needed
        UpdateUndoRedoButtons()
        'refresh viewer to include new column
        NumberOfColumnsChanged()
    End Sub
    Private Sub TableViewRowsAdded(rowIndices() As Int32)
        Array.Sort(rowIndices)
        For i As Int32 = 0 To rowIndices.Count - 1
            'update selected column indices
            For j As Int32 = 0 To _selectedDataRowIndices.Count - 1
                If _selectedDataRowIndices(j) >= rowIndices(i) Then _selectedDataRowIndices(j) += 1
            Next
            'adjust row from selected cells collection
            Dim rowsToUpdate As New List(Of Int32)
            For Each selectedCellsByRow As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
                If selectedCellsByRow.Key >= rowIndices(i) Then rowsToUpdate.Add(selectedCellsByRow.Key)
            Next
            rowsToUpdate.Sort()
            rowsToUpdate.Reverse()
            '
            For Each rowToUpdate As Int32 In rowsToUpdate
                _selectedCellIndices.Add(rowToUpdate + 1, _selectedCellIndices(rowToUpdate))
                _selectedCellIndices.Remove(rowToUpdate)
            Next
            'Update active cell
            If _activeCellVirtualRowIndex >= rowIndices(i) Then _activeCellVirtualRowIndex += 1
            If _activeCellVirtualRowIndex > DataView.NumberOfRows - 1 Then _activeCellVirtualRowIndex = DataView.NumberOfRows - 1
        Next
        '
        NumberOfRowsChanged()
        UpdateUndoRedoButtons()
    End Sub
    Private Sub TableViewRowsDeleted(rowIndices() As Int32)
        Array.Sort(rowIndices)
        For i As Int32 = rowIndices.Count - 1 To 0 Step -1
            'update selected column indices
            _selectedDataRowIndices.Remove(rowIndices(i))
            For j As Int32 = 0 To _selectedDataRowIndices.Count - 1
                If _selectedDataRowIndices(j) > rowIndices(i) Then _selectedDataRowIndices(j) -= 1
            Next
            'Delete row from selected cells collection
            _selectedCellIndices.Remove(rowIndices(i))
            Dim rowsToUpdate As New List(Of Int32)
            For Each selectedCellsByRow As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
                If selectedCellsByRow.Key > rowIndices(i) Then rowsToUpdate.Add(selectedCellsByRow.Key)
            Next
            rowsToUpdate.Sort()
            '
            For Each rowToUpdate As Int32 In rowsToUpdate
                _selectedCellIndices.Add(rowToUpdate - 1, _selectedCellIndices(rowToUpdate))
                _selectedCellIndices.Remove(rowToUpdate)
            Next
            'Update active cell
            If _activeCellVirtualRowIndex >= rowIndices(i) Then _activeCellVirtualRowIndex -= 1
            If _activeCellVirtualRowIndex < 0 Then _activeCellVirtualRowIndex = 0
        Next
        '
        NumberOfRowsChanged()
        UpdateUndoRedoButtons()
    End Sub
    Private Sub NumberOfRowsChanged()
        'Set the vertical scrollbar attributes
        _visibleRowCount = GetMaxRows() 'CInt(Math.Floor(RowsAr.ActualHeight / RowHeight))
        RemoveHandler VerticalScrollbar.ValueChanged, AddressOf VerticalScrollBar_ValueChanged
        If _selectedRowsOnly = True Then
            If _visibleRowCount > _selectedDataRowIndices.Count Then _visibleRowCount = _selectedDataRowIndices.Count
            VerticalScrollbar.Maximum = _selectedDataRowIndices.Count - _visibleRowCount
        Else
            If _visibleRowCount > DataView.NumberOfRows Then _visibleRowCount = DataView.NumberOfRows
            VerticalScrollbar.Maximum = DataView.NumberOfRows - _visibleRowCount
        End If
        '
        If CInt(Math.Floor(VerticalScrollbar.Value)) > CInt(VerticalScrollbar.Maximum) Then VerticalScrollbar.Value = CInt(VerticalScrollbar.Maximum)
        AddHandler VerticalScrollbar.ValueChanged, AddressOf VerticalScrollBar_ValueChanged
        '
        'update rowid and row offsets
        ReDim _rowId(DataView.NumberOfRows - 1)
        ReDim _rowOffset(_rowId.Count - 1)
        '
        If _columnSortOrder = SortOrder.None Then
            For i = 0 To _rowId.Count - 1
                _rowId(i) = i
            Next
            _rowId.CopyTo(_rowOffset, 0)
        Else
            For i As Int32 = 0 To _columnsSortedOrder.Count - 1
                If _columnsSortedOrder(i) = SortOrder.Ascending Then
                    SortColumn(i, True)
                ElseIf _columnsSortedOrder(i) = SortOrder.Descending Then
                    SortColumn(i, False)
                End If
            Next
        End If
        '
        VerticalScrollbar.ViewportSize = _visibleRowCount
        LoadRows()
        If ShowRowHeaders = True Then
            Dim pixelsPerDip = VisualTreeHelper.GetDpi(Me).PixelsPerDip
            RowHeadersColumnDefinition.Width = New GridLength(CInt(New FormattedText(DataView.NumberOfRows.ToString, Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, New Typeface(New FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 12, Brushes.Black, pixelsPerDip).Width) + 3)
        End If
        UpdateRowHeaders()
        'UpdateVisibleRows()
        SetSelectedCells()
        If DataView.NumberOfRows > 0 Then SetActiveCell(_activeCellVirtualRowIndex, _activeCellDataColumnIndex)
    End Sub
    Private Function GetMaxRows(Optional ByVal updateLayout As Boolean = True) As Integer
        If updateLayout Then Me.UpdateLayout()
        Dim maxNumberRows As Int32
        'If GridPanel.ActualHeight > HorizontalScrollViewer.ViewportHeight AndAlso HorizontalScrollViewer.ActualHeight > HorizontalScrollViewer.ViewportHeight Then
        '    maxNumberRows = CInt(Math.Floor((HorizontalScrollViewer.ActualHeight - ColumnHeadersGrid.ActualHeight) / RowHeight)) + 1
        'Else
        '    maxNumberRows = CInt(Math.Floor((HorizontalScrollViewer.ViewportHeight - ColumnHeadersGrid.ActualHeight) / RowHeight)) + 1
        'End If
        Dim parentWindow = Window.GetWindow(Me)
        If parentWindow IsNot Nothing AndAlso parentWindow.WindowState = WindowState.Maximized OrElse HorizontalScrollViewer.ActualHeight < HorizontalScrollViewer.ViewportHeight Then
            maxNumberRows = CInt(Math.Floor((HorizontalScrollViewer.ActualHeight - ColumnHeadersGrid.ActualHeight) / RowHeight)) + 1
        Else
            maxNumberRows = CInt(Math.Floor((HorizontalScrollViewer.ViewportHeight - ColumnHeadersGrid.ActualHeight) / RowHeight)) + 1
        End If
        '
        ' Leave room for last row.
        If VerticalScrollbar.Value = VerticalScrollbar.Maximum Then maxNumberRows -= 1
        'Can't be negative
        If maxNumberRows < 0 Then Return 0 Else Return maxNumberRows
    End Function
    Private Sub NumberOfColumnsChanged()
        RemoveHandler ColumnHeadersGrid.SizeChanged, AddressOf ColumnsGridSizeChanged
        '
        GridPanel.Children.Clear()
        RowHeadersGrid.Children.Clear()
        RowColorGrid.Children.Clear()
        ColumnHeadersGrid.Children.Clear()
        GridPanel.RowDefinitions.Clear()
        RowHeadersGrid.RowDefinitions.Clear()
        RowColorGrid.RowDefinitions.Clear()
        ColumnHeadersGrid.ColumnDefinitions.Clear()

        'Create Columns
        GridPanel.ColumnDefinitions.Clear()
        GridLinesCanvas.Children.Clear()
        CreateColumns()
        '
        For i As Int32 = 0 To _columnsSortedOrder.Count - 1
            If _columnsSortedOrder(i) = SortOrder.Ascending Then
                CType(ColumnHeadersGrid.Children(i * 2), ColumnHeader).AddSorter(True)
            ElseIf _columnsSortedOrder(i) = SortOrder.Descending Then
                CType(ColumnHeadersGrid.Children(i * 2), ColumnHeader).AddSorter(False)
            End If
        Next
        '
        'Create first row line
        Dim lengthBinding As New Binding("ActualWidth") With {.ElementName = "GridPanel"}
        Dim rowDistanceFromTop As Double = RowHeight * (GridPanel.RowDefinitions.Count) - (RowLineThickness / 2)
        Dim rowLine As New Line
        With rowLine
            .SnapsToDevicePixels = True
            .X1 = 0
            .Y1 = rowDistanceFromTop
            BindingOperations.SetBinding(rowLine, Line.X2Property, lengthBinding)
            .Y2 = rowDistanceFromTop
            .StrokeThickness = RowLineThickness
            .Stroke = RowLineColor
        End With
        GridLinesCanvas.Children.Add(rowLine)
        LoadRows()
        SetSelectedCells()
        'SetEditedCells()
        'SetActiveCell()
        RefreshColumnWidths()
        UpdateRowHeaders()
    End Sub

#End Region
    Private Sub Refresh()
        '
        If DataView Is Nothing Then Exit Sub
        If DataView.ParentDatabase.DataBaseOpen = False Then DataView.ParentDatabase.Open()
        '
        _selectedCellIndices = New SortedDictionary(Of Int32, SortedSet(Of Integer)) 'List(Of Integer))
        _selectedColumnIndices = New List(Of Int32)
        _selectedDataRowIndices = New List(Of Int32)
        _activeCellDataColumnIndex = 0
        _activeCellVirtualRowIndex = 0
        _selectedRowsOnly = False

        ReDim _rowId(DataView.NumberOfRows - 1)
        ReDim _rowOffset(_rowId.Count - 1)
        '
        For i = 0 To _rowId.Count - 1
            _rowId(i) = i
        Next
        _rowId.CopyTo(_rowOffset, 0)
        '
        RemoveHandler ColumnHeadersGrid.SizeChanged, AddressOf ColumnsGridSizeChanged
        '
        GridPanel.Children.Clear()
        RowHeadersGrid.Children.Clear()
        RowColorGrid.Children.Clear()
        ColumnHeadersGrid.Children.Clear()
        GridPanel.RowDefinitions.Clear()
        RowHeadersGrid.RowDefinitions.Clear()
        RowColorGrid.RowDefinitions.Clear()
        ColumnHeadersGrid.ColumnDefinitions.Clear()
        'Set the vertical scrollbar attributes
        _visibleRowCount = GetMaxRows(False)
        If _visibleRowCount < 0 Then _visibleRowCount = 0
        If _visibleRowCount > DataView.NumberOfRows Then _visibleRowCount = DataView.NumberOfRows
        RemoveHandler VerticalScrollbar.ValueChanged, AddressOf VerticalScrollBar_ValueChanged
        VerticalScrollbar.Maximum = DataView.NumberOfRows - _visibleRowCount
        'If CInt(Math.Floor(VerticalScrollbar.Value)) >= CInt(VerticalScrollbar.Maximum) Then VerticalScrollbar.Value -= 1
        AddHandler VerticalScrollbar.ValueChanged, AddressOf VerticalScrollBar_ValueChanged
        '
        VerticalScrollbar.ViewportSize = _visibleRowCount
        '
        If _columnsSortedOrder.Count <> DataView.ColumnNames.Count Then ReDim Preserve _columnsSortedOrder(DataView.ColumnNames.Count - 1)
        '
        'Create Columns
        GridPanel.ColumnDefinitions.Clear()
        GridLinesCanvas.Children.Clear()
        CreateColumns()
        '
        For i As Int32 = 0 To _columnsSortedOrder.Count - 1
            If _columnsSortedOrder(i) = SortOrder.Ascending Then
                CType(ColumnHeadersGrid.Children(i * 2), ColumnHeader).AddSorter(True)
            ElseIf _columnsSortedOrder(i) = SortOrder.Descending Then
                CType(ColumnHeadersGrid.Children(i * 2), ColumnHeader).AddSorter(False)
            End If
        Next
        '
        'Create first row line
        Dim lengthBinding As New Binding("ActualWidth") With {.ElementName = "GridPanel"}
        Dim rowDistanceFromTop As Double = RowHeight * (GridPanel.RowDefinitions.Count) - (RowLineThickness / 2)
        Dim rowLine As New Line With {.SnapsToDevicePixels = True, .X1 = 0, .Y1 = rowDistanceFromTop, .Y2 = rowDistanceFromTop, .StrokeThickness = RowLineThickness, .Stroke = RowLineColor}
        BindingOperations.SetBinding(rowLine, Line.X2Property, lengthBinding)

        GridLinesCanvas.Children.Add(rowLine)
        LoadRows()

        Me.UpdateLayout()

        DeSelectAllCells()
        SetSelectedCells()
        'SetEditedCells()
        SetActiveCell()
        RefreshColumnWidths()
        UpdateRowHeaders()

    End Sub

    Private Sub LoadRows()
        If GridLinesCanvas.Children.Count > DataView.ColumnNames.Count + 2 Then
            GridLinesCanvas.Children.RemoveRange(DataView.ColumnNames.Count + 2, GridLinesCanvas.Children.Count - 2 - DataView.ColumnNames.Count)
        End If
        GridPanel.Children.Clear()
        GridPanel.RowDefinitions.Clear()
        RowHeadersGrid.Children.Clear()
        RowHeadersGrid.RowDefinitions.Clear()
        RowColorGrid.Children.Clear()
        RowColorGrid.RowDefinitions.Clear()
        'Create Rows
        For i As Int32 = 0 To _visibleRowCount - 1
            AddRow()
        Next
    End Sub
    Private Sub CreateColumns()
        AddHandler ColumnHeadersGrid.SizeChanged, AddressOf ColumnsGridSizeChanged
        Dim columnWidth As Int32
        Dim columnResizers As GridSplitter
        Dim columnDefinitions As ColumnDefinition
        Dim header As ColumnHeader
        ReDim _columnWidths(DataView.ColumnNames.Count - 1)
        Dim pixelsPerDip = VisualTreeHelper.GetDpi(Me).PixelsPerDip
        For i As Int32 = 0 To DataView.ColumnNames.Count - 1
            ' Add Column Definition to the header row
            If AutoFitColumns = True Then
                columnDefinitions = New ColumnDefinition 'With {.Width = New GridLength(1, GridUnitType.Star)}
                _columnWidths(i) = New GridLength(1, GridUnitType.Star)
            Else
                columnWidth = CInt(New FormattedText(DataView.ColumnNames(i), Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, New Typeface(New FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 12, Brushes.Black, pixelsPerDip).Width) + 12
                columnDefinitions = New ColumnDefinition With {.Width = New GridLength(columnWidth)}
                _columnWidths(i) = New GridLength(columnWidth)
            End If
            ColumnHeadersGrid.ColumnDefinitions.Add(columnDefinitions)

            ' Add Column Header
            header = New ColumnHeader(DataView.ColumnNames(i), DataView.ColumnTypes(i)) With {.HeaderTextblockStyle = ColumnHeaderTextblockStyle, .HeaderBorderStyle = ColumnHeaderBorderStyle}
            AddHandler header.MouseRightButtonUp, AddressOf CreateColumnContextMenu
            Grid.SetColumn(header, i)
            ColumnHeadersGrid.Children.Add(header)

            ' Add Column Resizer
            If i < DataView.ColumnNames.Count - 1 Then
                columnResizers = New GridSplitter With {.Background = Brushes.Transparent, .Width = 10, .Margin = New Thickness(0, 0, -5, 0), .VerticalAlignment = VerticalAlignment.Stretch, .HorizontalAlignment = HorizontalAlignment.Right}
                AddHandler columnResizers.MouseDoubleClick, AddressOf ResizeColumnSplitterDoubleClick
                AddHandler columnResizers.DragDelta, AddressOf ResizeColumnSplitterDragDelta
                AddHandler columnResizers.DragCompleted, AddressOf ResizeColumnSplitterDragComplete
                Grid.SetZIndex(columnResizers, 1)
                Grid.SetColumn(columnResizers, i)
                ColumnHeadersGrid.Children.Add(columnResizers)
            End If

            ' Add a column definition to the main grid panel (for the data).
            columnDefinitions = New ColumnDefinition
            GridPanel.ColumnDefinitions.Add(columnDefinitions)
        Next
        '
        If DataView.ColumnNames.Count = 0 Then Exit Sub
        'Create Columns Lines
        Dim lengthBinding As New Binding(NameOf(Grid.ActualHeight)) With {.Source = GridPanel}
        Dim gridLine As Line
        For i As Int32 = 0 To DataView.ColumnNames.Count
            gridLine = New Line With {.SnapsToDevicePixels = True, .X1 = 0, .Y1 = 0, .X2 = 0, .StrokeThickness = ColumnLineThickness, .Stroke = ColumnLineColor}
            BindingOperations.SetBinding(gridLine, Line.Y2Property, lengthBinding)
            GridLinesCanvas.Children.Add(gridLine)
        Next
        'Set column line positions
        RefreshColumnWidths(False)

    End Sub

    Public Sub SetEditingCell()
        If _cellEditTextBox.IsFocused = False Then Exit Sub
        '_cellEditTextBox.Text = GetCellText(Grid.GetRow(_cellEditTextBox), Grid.GetColumn(_cellEditTextBox)) 'This would revert the cell back to the original value.
        GridPanel.Focus()
    End Sub
#End Region

#Region "Resize Logic"

    Private Sub ResizeColumnSplitterDragComplete(sender As Object, e As DragCompletedEventArgs)
        'Dim splitter As GridSplitter = CType(sender, GridSplitter)
        'Dim columnIndex As Int32 = Grid.GetColumn(splitter)
        'Dim newColumnWidth As Double = ColumnHeadersGrid.ColumnDefinitions(columnIndex).ActualWidth + e.HorizontalChange
        'ResizeColumnWidth(columnIndex, newColumnWidth)
    End Sub

    Private Sub ResizeColumnSplitterDragDelta(sender As Object, e As DragDeltaEventArgs)
        Dim splitter As GridSplitter = CType(sender, GridSplitter)
        Dim columnIndex As Int32 = Grid.GetColumn(splitter)
        Dim newColumnWidth As Double = ColumnHeadersGrid.ColumnDefinitions(columnIndex).ActualWidth + e.HorizontalChange
        ResizeColumnWidth(columnIndex, newColumnWidth)
    End Sub
    Private Sub ResizeColumnSplitterDoubleClick(ByVal sender As Object, e As MouseButtonEventArgs)
        Try
            Mouse.OverrideCursor = Cursors.Wait
            Dim splitter As GridSplitter = CType(sender, GridSplitter)
            Dim columnIndex As Int32 = Grid.GetColumn(splitter)
            ResizeColumnWidth(columnIndex)
            Mouse.OverrideCursor = Nothing
        Catch ex As Exception
            Mouse.OverrideCursor = Nothing
        End Try
    End Sub

    ''' <summary>
    ''' Resize the width of a column to the specified value. if specified width is under the minimum width then it will recalculate to auto fit.
    ''' </summary>
    ''' <param name="columnIndex">Index of column to be resized.</param>
    ''' <param name="columnWidth">Optional width to be specified for the column. If under minimum width then column is resized to auto fit.</param>
    Public Sub ResizeColumnWidth(columnIndex As Int32, Optional columnWidth As Int32 = -1)
        If columnWidth > 5 Then
            'ColumnHeadersGrid.ColumnDefinitions(columnIndex).Width = New GridLength(columnWidth)
            _columnWidths(columnIndex) = New GridLength(columnWidth)
        Else
            Dim data As Object() = DataView.GetColumn(DataView.ColumnNames(columnIndex))
            '
            Dim pixelsPerDip = VisualTreeHelper.GetDpi(Me).PixelsPerDip
            Dim max As Int32 = CInt(New FormattedText(DataView.ColumnNames(columnIndex), Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, New Typeface(New FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 12, Brushes.Black, pixelsPerDip).Width) + 12
            Dim formattedWidth As Int32
            Dim stringData As String
            Dim stringLength As Int32 = 0
            For i As Int32 = 0 To data.Count - 1
                stringData = data(i).ToString
                If stringLength <= stringData.Length Then
                    stringLength = stringData.Length
                    formattedWidth = New FormattedText(stringData, Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, New Typeface(New FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 12, Brushes.Black, pixelsPerDip).Width + 6
                    If formattedWidth > max Then max = formattedWidth
                End If
            Next
            If max < 6 Then max = 6
            _columnWidths(columnIndex) = New GridLength(max)
            'ColumnHeadersGrid.ColumnDefinitions(columnIndex).Width = New GridLength(max)
        End If
        RefreshColumnWidths()

    End Sub
    Private Sub ColumnsGridSizeChanged(sender As Object, e As SizeChangedEventArgs)
        RefreshColumnWidths()
    End Sub
    ''' <summary>
    ''' Method to Redraw the column lines and optionally set the active cell to true.
    ''' </summary>
    ''' <param name="setActive">Set the active cell after updating column line lengths.</param>
    Public Sub RefreshColumnWidths(Optional ByVal setActive As Boolean = True)
        Dim runningWidth As Double = 0

        'First define the absolute and auto widths
        Dim sumStar = 0
        Dim columnWidths(DataView.ColumnNames.Count - 1) As Double
        For i As Int32 = 0 To DataView.ColumnNames.Count - 1
            If _columnWidths(i).IsAuto Then
                columnWidths(i) = _columnWidths(i).Value
                runningWidth += _columnWidths(i).Value
            ElseIf _columnWidths(i).IsAbsolute Then
                columnWidths(i) = _columnWidths(i).Value
                runningWidth += _columnWidths(i).Value
            ElseIf _columnWidths(i).IsStar Then
                sumStar += _columnWidths(i).Value
            End If
            'Debug.Print($"{CType(GridLinesCanvas.Children(i + 1), Line).X1} : {ColumnHeadersGrid.ColumnDefinitions(i).Width} : {GridPanel.ColumnDefinitions(i).Width} : {ColumnHeadersGrid.ColumnDefinitions(i).ActualWidth} : {GridPanel.ColumnDefinitions(i).ActualWidth}")
        Next
        'Now define for the remaining space and remaining stars.
        Dim remainingSpace As Double = (HorizontalScrollViewer.ActualWidth - (ColumnLineThickness)) - runningWidth 'HorizontalScrollViewer.ViewportWidth - runningWidth
        Dim starredWidth As Double = 0
        For i As Int32 = 0 To DataView.ColumnNames.Count - 1
            If _columnWidths(i).IsStar Then
                If sumStar = 0 Then sumStar = _columnWidths(i).Value
                starredWidth = remainingSpace * _columnWidths(i).Value / sumStar
                If starredWidth < 10 Then starredWidth = 10
                columnWidths(i) = starredWidth
            End If
        Next

        'Finally update the line positions
        runningWidth = 0
        For i As Int32 = 0 To DataView.ColumnNames.Count - 1
            ColumnHeadersGrid.ColumnDefinitions(i).Width = New GridLength(columnWidths(i))
            GridPanel.ColumnDefinitions(i).Width = New GridLength(columnWidths(i))
            runningWidth += columnWidths(i) 'If(GridPanel.ColumnDefinitions(i).ActualWidth = 0, GridPanel.ColumnDefinitions(i).Width.Value, GridPanel.ColumnDefinitions(i).ActualWidth)
            CType(GridLinesCanvas.Children(i + 1), Line).X1 = runningWidth
            CType(GridLinesCanvas.Children(i + 1), Line).X2 = runningWidth
        Next
        '
        If setActive = True Then SetActiveCell()
    End Sub

    Private Sub HorizontalScrollViewer_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        RefreshView()
        'If _isLoaded = False Then Exit Sub
        'If IsNothing(DataView) Then Exit Sub
        ''
        'Dim newNumberRows As Int32 = GetMaxRows()
        'Dim nDataRows As Int32 = DataView.NumberOfRows
        'If _selectedRowsOnly = True Then nDataRows = _selectedDataRowIndices.Count
        'If newNumberRows > nDataRows Then newNumberRows = nDataRows
        'If newNumberRows <> _visibleRowCount AndAlso GridPanel.Children.Contains(_cellEditTextBox) Then GridPanel.Focus()
        'Dim offset As Int32 = newNumberRows - _visibleRowCount
        'If offset > (VerticalScrollbar.Maximum - Math.Floor(VerticalScrollbar.Value)) Then VerticalScrollbar.Value = VerticalScrollbar.Maximum
        'If newNumberRows < _visibleRowCount Then
        '    Do
        '        'Remove Row
        '        For i As Int32 = 0 To DataView.ColumnNames.Count - 1
        '            GridPanel.Children.RemoveAt(GridPanel.Children.Count - 1)
        '        Next
        '        GridPanel.RowDefinitions.RemoveAt(GridPanel.RowDefinitions.Count - 1)
        '        RowColorGrid.Children.RemoveAt(RowColorGrid.Children.Count - 1)
        '        RowColorGrid.RowDefinitions.RemoveAt(RowColorGrid.RowDefinitions.Count - 1)
        '        RowHeadersGrid.Children.RemoveAt(RowHeadersGrid.Children.Count - 1)
        '        RowHeadersGrid.RowDefinitions.RemoveAt(RowHeadersGrid.RowDefinitions.Count - 1)
        '        GridLinesCanvas.Children.RemoveAt(GridLinesCanvas.Children.Count - 1)
        '        _visibleRowCount -= 1
        '    Loop Until _visibleRowCount = newNumberRows
        '    If CInt(Math.Floor(VerticalScrollbar.Value)) = CInt(VerticalScrollbar.Maximum) Then
        '        VerticalScrollbar.Maximum = nDataRows - _visibleRowCount
        '        VerticalScrollbar.Value = VerticalScrollbar.Maximum
        '    Else
        '        VerticalScrollbar.Maximum = nDataRows - _visibleRowCount
        '    End If
        'ElseIf newNumberRows > _visibleRowCount Then
        '    Do
        '        AddRow()
        '        If _selectedRowsOnly = True Then
        '            For i As Int32 = 0 To DataView.ColumnNames.Count - 1
        '                SelectCell(i, _visibleRowCount)
        '            Next
        '        End If
        '        _visibleRowCount += 1
        '    Loop Until _visibleRowCount = newNumberRows
        '    VerticalScrollbar.Maximum = nDataRows - newNumberRows
        '    UpdateRowHeaders()
        'End If
        ''
        'RefreshColumnWidths(False)
        ''
        'If _selectedRowsOnly = False Then SetSelectedCells()
        'VerticalScrollbar.ViewportSize = _visibleRowCount
    End Sub
    Public Sub RefreshView()
        If _isLoaded = False Then Exit Sub
        If IsNothing(DataView) Then Exit Sub
        '
        Dim newNumberRows As Int32 = GetMaxRows()
        Dim nDataRows As Int32 = DataView.NumberOfRows
        If _selectedRowsOnly = True Then nDataRows = _selectedDataRowIndices.Count
        If newNumberRows > nDataRows Then newNumberRows = nDataRows
        If newNumberRows <> _visibleRowCount AndAlso GridPanel.Children.Contains(_cellEditTextBox) Then GridPanel.Focus()
        Dim offset As Int32 = newNumberRows - _visibleRowCount
        If offset > (VerticalScrollbar.Maximum - Math.Floor(VerticalScrollbar.Value)) Then VerticalScrollbar.Value = VerticalScrollbar.Maximum
        If newNumberRows < _visibleRowCount Then
            Do
                'Remove Row
                For i As Int32 = 0 To DataView.ColumnNames.Count - 1
                    GridPanel.Children.RemoveAt(GridPanel.Children.Count - 1)
                Next
                GridPanel.RowDefinitions.RemoveAt(GridPanel.RowDefinitions.Count - 1)
                RowColorGrid.Children.RemoveAt(RowColorGrid.Children.Count - 1)
                RowColorGrid.RowDefinitions.RemoveAt(RowColorGrid.RowDefinitions.Count - 1)
                RowHeadersGrid.Children.RemoveAt(RowHeadersGrid.Children.Count - 1)
                RowHeadersGrid.RowDefinitions.RemoveAt(RowHeadersGrid.RowDefinitions.Count - 1)
                GridLinesCanvas.Children.RemoveAt(GridLinesCanvas.Children.Count - 1)
                _visibleRowCount -= 1
            Loop Until _visibleRowCount = newNumberRows
            If CInt(Math.Floor(VerticalScrollbar.Value)) = CInt(VerticalScrollbar.Maximum) Then
                VerticalScrollbar.Maximum = nDataRows - _visibleRowCount
                VerticalScrollbar.Value = VerticalScrollbar.Maximum
            Else
                VerticalScrollbar.Maximum = nDataRows - _visibleRowCount
            End If
        ElseIf newNumberRows > _visibleRowCount Then
            Do
                AddRow()
                If _selectedRowsOnly = True Then
                    For i As Int32 = 0 To DataView.ColumnNames.Count - 1
                        SelectCell(i, _visibleRowCount)
                    Next
                End If
                _visibleRowCount += 1
            Loop Until _visibleRowCount = newNumberRows
            VerticalScrollbar.Maximum = nDataRows - newNumberRows
            UpdateRowHeaders()
        End If
        '
        RefreshColumnWidths(False)
        '
        If _selectedRowsOnly = False Then SetSelectedCells()
        VerticalScrollbar.ViewportSize = _visibleRowCount
    End Sub

    'Private Sub Table_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged
    '    If _isLoaded = False Then Exit Sub
    '    If IsNothing(DataView) Then Exit Sub
    '    '
    '    Dim newNumberRows As Int32 = GetMaxRows()
    '    Dim nDataRows As Int32 = DataView.NumberOfRows
    '    If _selectedRowsOnly = True Then nDataRows = _selectedDataRowIndices.Count
    '    If newNumberRows > nDataRows Then newNumberRows = nDataRows
    '    If newNumberRows <> _visibleRowCount AndAlso GridPanel.Children.Contains(_cellEditTextBox) Then GridPanel.Focus()
    '    Dim offset As Int32 = newNumberRows - _visibleRowCount
    '    If offset > (VerticalScrollbar.Maximum - Math.Floor(VerticalScrollbar.Value)) Then VerticalScrollbar.Value = VerticalScrollbar.Maximum
    '    If newNumberRows < _visibleRowCount Then
    '        Do
    '            'Remove Row
    '            For i As Int32 = 0 To DataView.ColumnNames.Count - 1
    '                GridPanel.Children.RemoveAt(GridPanel.Children.Count - 1)
    '            Next
    '            GridPanel.RowDefinitions.RemoveAt(GridPanel.RowDefinitions.Count - 1)
    '            RowColorGrid.Children.RemoveAt(RowColorGrid.Children.Count - 1)
    '            RowColorGrid.RowDefinitions.RemoveAt(RowColorGrid.RowDefinitions.Count - 1)
    '            RowHeadersGrid.Children.RemoveAt(RowHeadersGrid.Children.Count - 1)
    '            RowHeadersGrid.RowDefinitions.RemoveAt(RowHeadersGrid.RowDefinitions.Count - 1)
    '            GridLinesCanvas.Children.RemoveAt(GridLinesCanvas.Children.Count - 1)
    '            _visibleRowCount -= 1
    '        Loop Until _visibleRowCount = newNumberRows
    '        If CInt(Math.Floor(VerticalScrollbar.Value)) = CInt(VerticalScrollbar.Maximum) Then
    '            VerticalScrollbar.Maximum = nDataRows - _visibleRowCount
    '            VerticalScrollbar.Value = VerticalScrollbar.Maximum
    '        Else
    '            VerticalScrollbar.Maximum = nDataRows - _visibleRowCount
    '        End If
    '    ElseIf newNumberRows > _visibleRowCount Then
    '        Do
    '            AddRow()
    '            If _selectedRowsOnly = True Then
    '                For i As Int32 = 0 To DataView.ColumnNames.Count - 1
    '                    SelectCell(i, _visibleRowCount)
    '                Next
    '            End If
    '            _visibleRowCount += 1
    '        Loop Until _visibleRowCount = newNumberRows
    '        VerticalScrollbar.Maximum = nDataRows - newNumberRows
    '        UpdateRowHeaders()
    '    End If
    '    '
    '    RefreshColumnLinePositions(False)
    '    '
    '    If _selectedRowsOnly = False Then SetSelectedCells()
    '    VerticalScrollbar.ViewportSize = _visibleRowCount
    'End Sub
#End Region

#Region "Adding Rows and Defining Cell Text"
    Private Sub AddRow()
        If GridPanel Is Nothing OrElse DataView Is Nothing OrElse VerticalScrollbar Is Nothing Then Exit Sub
        'Create new row definition
        GridPanel.RowDefinitions.Add(New RowDefinition With {.Height = New GridLength(RowHeight)})
        'Create new row Cells
        Dim newCell As Cell
        For j As Int32 = 0 To DataView.ColumnTypes.Count - 1
            newCell = New Cell() With {.CellStyle = CellTextblockStyle}
            Grid.SetRow(newCell, GridPanel.RowDefinitions.Count - 1)
            Grid.SetColumn(newCell, j)
            GridPanel.Children.Add(newCell)
        Next
        'Fill new row with data
        Dim scrollBarValue As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        'If CInt(VerticalScrollbar.Maximum) >= 0 AndAlso scrollBarValue <= CInt(VerticalScrollbar.Maximum) Then 'OrElse CInt(VerticalScrollbar.Maximum) <> scrollBarValue

        If (scrollBarValue + (GridPanel.RowDefinitions.Count - 1)) < _rowId.Count Then
            If _selectedRowsOnly = False Then
                FillRow(GridPanel.RowDefinitions.Count - 1, DataView.GetRow(_rowId(scrollBarValue + GridPanel.RowDefinitions.Count - 1)))
            Else
                If _columnSortOrder = SortOrder.None Then
                    If _selectedDataRowIndices.Count >= scrollBarValue + GridPanel.RowDefinitions.Count Then FillRow(GridPanel.RowDefinitions.Count - 1, DataView.GetRow(_rowId(_selectedDataRowIndices(scrollBarValue + GridPanel.RowDefinitions.Count - 1))))
                Else
                    If _sortedSelectedRowOffsets.Length >= scrollBarValue + GridPanel.RowDefinitions.Count Then FillRow(GridPanel.RowDefinitions.Count - 1, DataView.GetRow(_rowId(_sortedSelectedRowOffsets(scrollBarValue + GridPanel.RowDefinitions.Count - 1))))
                End If
            End If
        End If

        'End If
        'Create row line
        Dim lengthBinding As New Binding(NameOf(Grid.ActualWidth)) With {.Source = GridPanel}
        Dim rowDistanceFromTop As Double = RowHeight * (GridPanel.RowDefinitions.Count) - (RowLineThickness / 2)
        Dim rowLine As New Line With {.SnapsToDevicePixels = True, .X1 = 0, .Y1 = rowDistanceFromTop, .Y2 = rowDistanceFromTop, .StrokeThickness = RowLineThickness, .Stroke = RowLineColor}
        BindingOperations.SetBinding(rowLine, Line.X2Property, lengthBinding)
        GridLinesCanvas.Children.Add(rowLine)
        'Create Row selector
        RowHeadersGrid.RowDefinitions.Add(New RowDefinition With {.Height = New GridLength(RowHeight), .MaxHeight = RowHeight})
        Dim newRowSelector As New RowHeader With {.HeaderTextblockStyle = RowHeaderTextblockStyle, .HeaderBorderStyle = RowHeaderBorderStyle, .Height = RowHeight}
        Grid.SetRow(newRowSelector, RowHeadersGrid.RowDefinitions.Count - 1)
        RowHeadersGrid.Children.Add(newRowSelector)
        'Create row color
        RowColorGrid.RowDefinitions.Add(New RowDefinition With {.Height = New GridLength(RowHeight)})
        Dim fillColor As SolidColorBrush = RowColor
        If _rowId(RowColorGrid.RowDefinitions.Count() - 1) Mod 2 <> 0 Then fillColor = AlternateRowColor
        Dim rect As New Rectangle With {.Stroke = New SolidColorBrush(Colors.Transparent), .StrokeThickness = 0, .Fill = fillColor}
        Grid.SetRow(rect, RowColorGrid.RowDefinitions.Count - 1)
        RowColorGrid.Children.Add(rect)
    End Sub
    Private Sub FillRow(rowIndex As Int32, row() As Object)
        If row Is Nothing Then
            For i As Int32 = 0 To DataView.ColumnNames.Count - 1 '_visibleColumnIndices.Count - 1
                SetCellText(rowIndex, i, "")
            Next
        Else
            For i As Int32 = 0 To DataView.ColumnNames.Count - 1 '_visibleColumnIndices.Count - 1
                'SetCellText(rowIndex, _visibleColumnIndices(i), row(_visibleColumnIndices(i)).ToString)
                If row(i) Is Nothing Then SetCellText(rowIndex, i, "") Else SetCellText(rowIndex, i, row(i).ToString)
            Next
        End If

    End Sub
    Public Sub UpdateVisibleRows()
        Dim dataRowIndices As List(Of Int32) = GetDataRowIndexes(0, _visibleRowCount - 1)
        For i As Int32 = 0 To dataRowIndices.Count - 1
            'FillRow(i, DataView.GetRow(dataRowIndices(i), _visibleColumnIndices))
            FillRow(i, DataView.GetRow(dataRowIndices(i))) ', startrow)
        Next
    End Sub
    Private Sub SetCellText(rowIndex As Int32, columnIndex As Int32, newText As String)
        CType(GridPanel.Children(rowIndex * DataView.ColumnNames.Count + columnIndex), Cell).Text = newText
    End Sub
    Private Function GetCellText(rowIndex As Int32, columnIndex As Int32) As String
        Return CType(GridPanel.Children(rowIndex * DataView.ColumnNames.Count + columnIndex), Cell).Text
    End Function
#End Region

#Region "Vertical ScrollBar Logic"
    Private Sub TestGridPanel_MouseWheel(sender As Object, e As MouseWheelEventArgs)
        VerticalScrollbar.Value -= e.Delta / 10 'if e.Delta = 30 then table will go up 3 rows
    End Sub
    Private Sub RowsGrid_MouseWheel(sender As Object, e As MouseWheelEventArgs)
        VerticalScrollbar.Value -= e.Delta / 10 'if e.Delta = 30 then table will go up 3 rows
    End Sub
    Private Sub VerticalScrollBar_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        'Convert old and new values to integers to match row indices.
        Dim oldValue As Int32 = CInt(Math.Floor(e.OldValue))
        Dim newValue As Int32 = CInt(Math.Floor(e.NewValue))
        If oldValue = newValue Then Exit Sub
        '
        Dim counter As Int32 = 0
        If newValue >= VerticalScrollbar.Maximum AndAlso VerticalScrollbar.Maximum <> 0 Then 'The last row has been reached
            'remove the last row and refresh all cells.
            NumberOfRowsChanged()
            If _visibleRowCount > 0 Then FillRow(_visibleRowCount - 1, DataView.GetRow(GetDataRowIndex(_visibleRowCount - 1)))
        ElseIf oldValue >= VerticalScrollbar.Maximum AndAlso VerticalScrollbar.Maximum <> 0 Then 'The last row has been vacated
            'Add a row and refresh all cells.
            _visibleRowCount += 1
            'VerticalScrollbar.Maximum = DataView.NumberOfRows - _visibleRowCount
            VerticalScrollbar.ViewportSize = _visibleRowCount
            AddRow()
            UpdateVisibleRows()
        ElseIf oldValue > newValue Then 'Going Up
            Dim offset As Int32 = oldValue - newValue
            For i As Int32 = _visibleRowCount - 1 To offset Step -1
                For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                    SetCellText(i, j, GetCellText(i - offset, j))
                Next
            Next
            '
            If offset >= _visibleRowCount Then offset = _visibleRowCount
            For Each dataRowIndex As Int32 In GetDataRowIndexes(0, offset - 1)
                FillRow(counter, DataView.GetRow(dataRowIndex))
                counter += 1
            Next
        ElseIf oldValue < newValue Then 'Going Down
            Dim offset As Int32 = newValue - oldValue
            For i As Int32 = 0 To _visibleRowCount - offset - 1
                For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                    SetCellText(i, j, GetCellText(i + offset, j))
                Next
            Next

            If _visibleRowCount - offset - 1 < 0 Then
                offset = 0
            Else
                offset = _visibleRowCount - offset
            End If
            counter = offset
            For Each dataRowIndex As Int32 In GetDataRowIndexes(offset, _visibleRowCount - 1)
                FillRow(counter, DataView.GetRow(dataRowIndex))
                counter += 1
            Next
        End If
        'Turn off cell editing
        If GridPanel.Children.Contains(_cellEditTextBox) Then GridPanel.Focus()
        '
        If _selectedRowsOnly = False Then
            DeSelectAllCells()
            SetSelectedCells()
        Else
            SetSelectedCells()
        End If
        'SetEditedCells()
        UpdateRowHeaders()
    End Sub
    Private Sub UpdateRowHeaders()
        '
        If _visibleRowCount = 0 Then Exit Sub
        Dim firstRowVirtualIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        If VerticalScrollbar.Value = VerticalScrollbar.Maximum AndAlso VerticalScrollbar.Value <> 0 Then
            firstRowVirtualIndex = VerticalScrollbar.Maximum - _visibleRowCount
            If firstRowVirtualIndex < 0 Then firstRowVirtualIndex = 0
        End If
        '
        Dim alternate As Boolean = _rowOffset(_rowId(firstRowVirtualIndex)) Mod 2 <> 0
        For i As Int32 = 0 To _visibleRowCount - 1
            CType(RowColorGrid.Children(i), Rectangle).Fill = If(alternate = True, AlternateRowColor, RowColor)
            CType(RowHeadersGrid.Children(i), RowHeader).Text = GetDataRowIndex(i).ToString
            alternate = Not alternate
        Next
    End Sub

    Private Sub VerticalScrollbar_IsEnabledChanged(sender As Object, e As DependencyPropertyChangedEventArgs)
        If VerticalScrollbar.IsEnabled = False Then VerticalScrollbar.Visibility = Visibility.Collapsed Else VerticalScrollbar.Visibility = Visibility.Visible
    End Sub
#End Region

#Region "Selection Logic"

#Region "Indexing and rendering selected cells"
    ''' <summary>
    ''' Gets the row indices of all selected rows sorted by the table row position regardless of sorting. key=table row position, value=database row position
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSelectedRowVirtualRowIndices() As SortedDictionary(Of Int32, Int32)
        Dim sortedRowsIndices As New SortedDictionary(Of Int32, Int32)
        For i As Int32 = 0 To _selectedDataRowIndices.Count - 1
            sortedRowsIndices.Add(_rowOffset(_selectedDataRowIndices(i)), _selectedDataRowIndices(i))
        Next
        If _columnSortOrder = SortOrder.Descending Then sortedRowsIndices.Reverse()
        Return sortedRowsIndices
    End Function
    ''' <summary>
    ''' Gets the row indices of all selected cells sorted by the table row position regardless of sorting. key=table row position, value=database row position
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetSelectedCellVirtualRowIndices() As SortedDictionary(Of Int32, Int32)
        Dim sortedRowsIndices As New SortedDictionary(Of Int32, Int32)
        For Each selectedCell As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
            sortedRowsIndices.Add(_rowOffset(selectedCell.Key), selectedCell.Key)
        Next
        '
        If _columnSortOrder = SortOrder.Descending Then sortedRowsIndices.Reverse()
        Return sortedRowsIndices
    End Function

    Private Function GetDataRowIndexes(viewRowStartIndex As Int32, viewRowEndIndex As Int32) As List(Of Int32)
        If viewRowEndIndex = -1 Then Return New List(Of Int32)
        Dim dataRowIndexes As New List(Of Int32)(viewRowEndIndex - viewRowStartIndex)
        Dim firstRowVirtualIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        If _selectedRowsOnly = True Then
            If _columnSortOrder = SortOrder.None Then
                For i As Int32 = viewRowStartIndex To viewRowEndIndex
                    dataRowIndexes.Add(_rowId(_selectedDataRowIndices(firstRowVirtualIndex + i)))
                Next
            Else
                For i As Int32 = viewRowStartIndex To viewRowEndIndex
                    dataRowIndexes.Add(_rowId(_sortedSelectedRowOffsets(firstRowVirtualIndex + i)))
                Next
            End If
        Else
            For i As Int32 = viewRowStartIndex To viewRowEndIndex
                dataRowIndexes.Add(_rowId(firstRowVirtualIndex + i))
            Next
        End If
        '
        Return dataRowIndexes
    End Function

    Private Function GetDataRowIndex(viewRowIndex As Int32) As Int32
        Dim firstRowVirtualIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        If _selectedRowsOnly = True Then
            If _columnSortOrder = SortOrder.None Then Return _rowId(_selectedDataRowIndices(firstRowVirtualIndex + viewRowIndex))
            Return _rowId(_sortedSelectedRowOffsets(firstRowVirtualIndex + viewRowIndex))
        Else
            Return _rowId(firstRowVirtualIndex + viewRowIndex)
        End If
    End Function
    Private Function GetTableColumnIndex(gridPosition As Point) As Int32
        Dim runningSum As Double = 0
        For i As Int32 = 0 To GridPanel.ColumnDefinitions.Count - 1
            runningSum += GridPanel.ColumnDefinitions(i).ActualWidth
            If runningSum >= gridPosition.X Then Return i
        Next
        Return GridPanel.ColumnDefinitions.Count - 1
    End Function
    Private Function GetTableRowIndex(gridPosition As Point) As Int32
        Dim runningSum As Double = 0
        For i As Int32 = 0 To GridPanel.RowDefinitions.Count - 1
            runningSum += GridPanel.RowDefinitions(i).ActualHeight
            If runningSum >= gridPosition.Y Then Return i
        Next
        Return GridPanel.RowDefinitions.Count - 1
    End Function
    Private Sub SetSelectedCells()
        'All selected
        If AllCellsSelected = True Or _selectedRowsOnly = True Then
            For i As Int32 = 0 To _visibleRowCount - 1
                For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                    SelectCell(j, i)
                Next
            Next
            SetActiveCell()
            Exit Sub
        End If
        'Columns
        If _selectedColumnIndices.Count > 0 Then
            For Each columnIndex As Int32 In _selectedColumnIndices
                For i As Int32 = 0 To _visibleRowCount - 1
                    SelectCell(columnIndex, i)
                Next
            Next
        End If
        'Rows
        If _selectedDataRowIndices.Count > 0 Then
            Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
            Dim lastRowDataIndex As Int32 = firstRowDataIndex + _visibleRowCount - 1
            If _columnSortOrder = SortOrder.Ascending OrElse _columnSortOrder = SortOrder.Descending Then
                'Now set the proper row selections
                Dim counter As Int32 = 0
                For i As Int32 = firstRowDataIndex To lastRowDataIndex
                    If _selectedDataRowIndices.BinarySearch(_rowId(i)) >= 0 Then
                        For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                            SelectCell(j, counter)
                        Next
                    End If
                    counter += 1
                Next
            Else
                Dim searchIndex As Int32 = _selectedDataRowIndices.BinarySearch(firstRowDataIndex)
                If searchIndex < 0 Then searchIndex = (Not searchIndex) 'bitwise complement
                For i As Int32 = searchIndex To _selectedDataRowIndices.Count - 1
                    If _selectedDataRowIndices(i) > lastRowDataIndex Then Exit For
                    For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                        SelectCell(j, _selectedDataRowIndices(i) - firstRowDataIndex)
                    Next
                Next
            End If
        End If
        'Cells
        If _selectedCellIndices.Count > 0 Then
            Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
            Dim tableRowIndex As Int32
            For i As Int32 = firstRowDataIndex To (firstRowDataIndex + _visibleRowCount - 1)
                If _selectedCellIndices.ContainsKey(_rowId(i)) Then
                    tableRowIndex = _rowOffset(_rowId(i)) - firstRowDataIndex
                    For Each columnIndex As Int32 In _selectedCellIndices(_rowId(i))
                        SelectCell(columnIndex, tableRowIndex)
                    Next
                End If
            Next
        End If
        SetActiveCell()
    End Sub
    Private Sub SetActiveCell()
        If GridPanel.Children.Count = 0 Then Return
        Dim firstRowTableIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        Dim lastRowTableIndex As Int32 = firstRowTableIndex + _visibleRowCount - 1
        If _activeCellVirtualRowIndex >= firstRowTableIndex AndAlso _activeCellVirtualRowIndex <= lastRowTableIndex Then
            Dim rowIndex As Int32 = _activeCellVirtualRowIndex - firstRowTableIndex
            CType(GridPanel.Children(rowIndex * DataView.ColumnNames.Count + _activeCellDataColumnIndex), Cell).Background = ActiveCellBackground
            CType(GridPanel.Children(rowIndex * DataView.ColumnNames.Count + _activeCellDataColumnIndex), Cell).Foreground = ActiveCellForeground
        End If
    End Sub
    Public Sub SetActiveCell(newDataRowIndex As Int32, newDataColumnIndex As Int32, Optional scrollToRow As Boolean = False)
        '
        _activeCellDataColumnIndex = newDataColumnIndex
        _activeCellVirtualRowIndex = _rowOffset(newDataRowIndex)
        If scrollToRow Then VerticalScrollbar.Value = _activeCellVirtualRowIndex
        DeSelectAllCells()
        SetSelectedCells()
        RaiseEvent ActiveCellLocationChanged()
    End Sub
    Public Sub UpdateSelectedRowIndices(newSelectedRowIndices() As Int32)
        If IsNothing(newSelectedRowIndices) Then
            _selectedDataRowIndices.Clear()
        Else
            _selectedDataRowIndices = newSelectedRowIndices.ToList
            _selectedDataRowIndices.Sort()
        End If
        '
        DeSelectAllCells()
        If _selectedRowsOnly = False Then
            SetSelectedCells()
        Else
            ShowAll_Checked(Nothing, Nothing)
            SetSelectedCells()
            ShowSelected_Checked(Nothing, Nothing)
        End If
        If _selectedDataRowIndices.Count > 0 AndAlso _selectedRowsOnly = False Then
            ShowSelected.IsEnabled = True
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ShowSelectedIcon_22x22.png"))
            DeSelectAll.IsEnabled = True
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIcon_22x22.png"))
        Else
            ShowSelected.IsEnabled = False
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            DeSelectAll.IsEnabled = False
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
        End If

    End Sub
    Private Sub DeSelectAllCells()
        For i As Int32 = 0 To DataView.ColumnNames.Count - 1
            For j As Int32 = 0 To GridPanel.RowDefinitions.Count - 1
                DeSelectCell(i, j)
            Next
        Next
    End Sub
    Private Sub SelectCell(columnIndex As Int32, rowIndex As Int32)
        CType(GridPanel.Children(rowIndex * DataView.ColumnNames.Count + columnIndex), Cell).Background = SelectedColor
        CType(GridPanel.Children(rowIndex * DataView.ColumnNames.Count + columnIndex), Cell).Foreground = SelectedForegroundColor
    End Sub
    Private Sub DeSelectCell(columnIndex As Int32, rowIndex As Int32)
        CType(GridPanel.Children(rowIndex * DataView.ColumnNames.Count + columnIndex), Cell).Background = DeSelectedColor
        CType(GridPanel.Children(rowIndex * DataView.ColumnNames.Count + columnIndex), Cell).Foreground = DeSelectedForegroundColor
    End Sub
#End Region

#Region "Cell Selection Logic"
    Private Sub Grid_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        Dim lastRowDataIndex As Int32 = firstRowDataIndex + _visibleRowCount - 1
        Dim rowIndex As Int32 = _activeCellVirtualRowIndex - firstRowDataIndex
        If e.Key = Key.PageDown Then
            If lastRowDataIndex = DataView.NumberOfRows - 1 Then Exit Sub
            If _cellEditTextBox.IsFocused Then GridPanel.Focus()
            VerticalScrollbar.Value += _visibleRowCount
            Dim maxRows As Int32 = DataView.NumberOfRows
            If _selectedRowsOnly = True Then maxRows = _selectedDataRowIndices.Count
            If (_activeCellVirtualRowIndex + _visibleRowCount) < maxRows - _visibleRowCount Then
                SetActiveCell(_activeCellVirtualRowIndex + _visibleRowCount, _activeCellDataColumnIndex)
            Else
                SetActiveCell(CInt(Math.Floor(VerticalScrollbar.Value)) + rowIndex, _activeCellDataColumnIndex)
            End If
        End If
        If e.Key = Key.PageUp Then
            If firstRowDataIndex = 0 Then Exit Sub
            If _cellEditTextBox.IsFocused Then GridPanel.Focus()
            VerticalScrollbar.Value -= _visibleRowCount
            If (firstRowDataIndex - _visibleRowCount) >= 0 Then
                SetActiveCell(_activeCellVirtualRowIndex - _visibleRowCount, _activeCellDataColumnIndex)
            Else
                SetActiveCell(CInt(Math.Floor(VerticalScrollbar.Value)) + rowIndex, _activeCellDataColumnIndex)
            End If
        End If
        If _cellEditTextBox.IsFocused Then Exit Sub
        If e.Key = Key.Left Then
            If _activeCellDataColumnIndex > 0 Then SetActiveCell(_activeCellVirtualRowIndex, _activeCellDataColumnIndex - 1)
            e.Handled = True
            Exit Sub
        End If
        If e.Key = Key.Down Then
            If _selectedRowsOnly = True Then
                If _activeCellVirtualRowIndex < _selectedDataRowIndices.Count - 1 Then SetActiveCell(_activeCellVirtualRowIndex + 1, _activeCellDataColumnIndex)
            Else
                If _activeCellVirtualRowIndex < DataView.NumberOfRows - 1 Then SetActiveCell(_activeCellVirtualRowIndex + 1, _activeCellDataColumnIndex)
            End If
            If _activeCellVirtualRowIndex < firstRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex
            If _activeCellVirtualRowIndex > lastRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1
            e.Handled = True
            Exit Sub
        End If
        If e.Key = Key.Right Then
            If _activeCellDataColumnIndex < DataView.ColumnNames.Count - 1 Then SetActiveCell(_activeCellVirtualRowIndex, _activeCellDataColumnIndex + 1)
            e.Handled = True
            Exit Sub
        End If
        If e.Key = Key.Up Then
            If _activeCellVirtualRowIndex > 0 Then SetActiveCell(_activeCellVirtualRowIndex - 1, _activeCellDataColumnIndex)
            If _activeCellVirtualRowIndex < firstRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex
            If _activeCellVirtualRowIndex > lastRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1
            e.Handled = True
            Exit Sub
        End If
    End Sub

    Private Sub Grid_KeyDown(sender As Object, e As KeyEventArgs)
        If _cellEditTextBox.IsFocused = True Then Exit Sub
        If e.Key = Key.LeftCtrl Or e.Key = Key.RightCtrl Then Exit Sub
        If e.Key = Key.LeftShift Or e.Key = Key.RightShift Then Exit Sub
        Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        Dim lastRowDataIndex As Int32 = firstRowDataIndex + _visibleRowCount - 1
        If e.Key = Key.Tab Then
            If _activeCellDataColumnIndex < DataView.ColumnNames.Count - 1 Then SetActiveCell(_activeCellVirtualRowIndex, _activeCellDataColumnIndex + 1)
            If _activeCellVirtualRowIndex < firstRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex
            If _activeCellVirtualRowIndex > lastRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1
            e.Handled = True
            Exit Sub
        End If
        If e.Key = Key.Enter Then
            If _selectedRowsOnly = True Then
                If _activeCellVirtualRowIndex < _selectedDataRowIndices.Count - 1 Then SetActiveCell(_activeCellVirtualRowIndex + 1, _activeCellDataColumnIndex)
            Else
                If _activeCellVirtualRowIndex < DataView.NumberOfRows - 1 Then SetActiveCell(_activeCellVirtualRowIndex + 1, _activeCellDataColumnIndex)
            End If
            If _activeCellVirtualRowIndex < firstRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex
            If _activeCellVirtualRowIndex > lastRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1
            e.Handled = True
            Exit Sub
        End If
        '
        If ((e.Key < Key.NumPad0) OrElse (e.Key > Key.Divide)) AndAlso ((e.Key < Key.D0) OrElse (e.Key > Key.D9)) AndAlso ((e.Key < Key.A) OrElse (e.Key > Key.Z)) AndAlso ((e.Key < Key.Oem1) OrElse (e.Key > Key.Oem3)) AndAlso ((e.Key < Key.OemOpenBrackets) OrElse (e.Key > Key.OemQuotes)) Then
            If e.Key <> Key.Space Then
                e.Handled = True
                Exit Sub
            End If
        End If
        If e.Key = Key.Escape Then
            Clipboard.Clear()
            Exit Sub
        End If
        'Copy Paste Logic
        If Keyboard.IsKeyDown(Key.LeftCtrl) Or Keyboard.IsKeyDown(Key.RightCtrl) Then
            If e.Key = Key.V And Editable = True Then
                If Clipboard.ContainsText AndAlso _selectedRowsOnly = False Then
                    If _activeCellVirtualRowIndex < firstRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex
                    If _activeCellVirtualRowIndex > lastRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1
                    Paste()
                End If
            ElseIf e.Key = Key.C Then
                Copy()
            ElseIf e.Key = Key.A Then
                AllCellsSelected = True
                _selectedDataRowIndices.Clear()
                _selectedCellIndices.Clear()
                _selectedColumnIndices.Clear()
                DeSelectAllCells()
                SetSelectedCells()
            ElseIf e.Key = Key.Z Then
                'I don't like this solution. The problem is that the toolbars are connected to the viewer. A better solution would be to have the toolbars be seperate controls and throw events here to undo or redo.
                If Undo.IsEnabled = True And Undo.Visibility = Visibility.Visible Then UndoLastEdit()
            ElseIf e.Key = Key.Y Then
                If Redo.IsEnabled = True And Redo.Visibility = Visibility.Visible Then RedoLastEdit()
            End If
            Exit Sub
        End If
        'Enter cell edit logic
        If Editable = True And _readOnlyColumns.Contains(_activeCellDataColumnIndex) = False Then
            If _activeCellVirtualRowIndex < firstRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex : firstRowDataIndex = CInt(Math.Floor(VerticalScrollbar.Value))
            If _activeCellVirtualRowIndex > lastRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1 : firstRowDataIndex = CInt(Math.Floor(VerticalScrollbar.Value))
            If _activeCellVirtualRowIndex < 0 Then Exit Sub
            Dim rowIndex As Int32 = _activeCellVirtualRowIndex - firstRowDataIndex
            _mouseSelectionMode = SelectionMode.EditSelect
            Dim initialText As String = GetCellText(rowIndex, _activeCellDataColumnIndex)
            _cellEditTextBox.Tag = New Tuple(Of String, Point)(initialText, New Point(_activeCellDataColumnIndex, GetDataRowIndex(rowIndex)))
            Grid.SetColumn(_cellEditTextBox, _activeCellDataColumnIndex)
            Grid.SetRow(_cellEditTextBox, rowIndex)
            If GridPanel.Children.Contains(_cellEditTextBox) Then GridPanel.Children.Remove(_cellEditTextBox)
            GridPanel.Children.Add(_cellEditTextBox)
            _cellEditTextBox.SelectAll()
            PreviewEditText(Nothing, e)
            _cellEditTextBox.Focus()
            'RaiseEvent SelectionModeChanged(_MouseSelectionMode)
        End If
    End Sub

    Private Sub GridPanel_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles GridPanel.MouseLeftButtonDown
        Dim gridPosition As Point = e.GetPosition(GridPanel)
        '
        If _selectedRowsOnly = True Then
            'Re-select the previously active cell
            Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
            Dim lastRowDataIndex As Int32 = firstRowDataIndex + _visibleRowCount - 1
            If _activeCellVirtualRowIndex >= firstRowDataIndex AndAlso _activeCellVirtualRowIndex <= lastRowDataIndex Then
                SelectCell(_activeCellDataColumnIndex, _activeCellVirtualRowIndex - firstRowDataIndex)
            End If
            'set and select the new active cell
            _mouseSelectionMode = SelectionMode.None
            _mouseDownColumnIndex = GetTableColumnIndex(gridPosition)
            _mouseDownVirtualRowIndex = CInt(Math.Floor(VerticalScrollbar.Value)) + GetTableRowIndex(gridPosition)
            _activeCellDataColumnIndex = _mouseDownColumnIndex
            _activeCellVirtualRowIndex = _mouseDownVirtualRowIndex
            RaiseEvent ActiveCellLocationChanged()
            SetActiveCell()
            'convert the cell to edit mode if double clicked.
            If e.ClickCount = 2 And Editable = True And _readOnlyColumns.Contains(_mouseDownColumnIndex) = False Then
                _mouseSelectionMode = SelectionMode.EditSelect
                Dim rowIndex As Int32 = GetTableRowIndex(gridPosition)
                Dim initialText As String = GetCellText(rowIndex, _mouseDownColumnIndex)
                _cellEditTextBox.Tag = New Tuple(Of String, Point)(initialText, New Point(_mouseDownColumnIndex, GetDataRowIndex(rowIndex)))
                Grid.SetColumn(_cellEditTextBox, _mouseDownColumnIndex)
                Grid.SetRow(_cellEditTextBox, rowIndex)
                _cellEditTextBox.Text = initialText
                If GridPanel.Children.Contains(_cellEditTextBox) Then GridPanel.Children.Remove(_cellEditTextBox)
                GridPanel.Children.Add(_cellEditTextBox)
                _cellEditTextBox.SelectAll()
                Exit Sub
            End If
            CType(sender, UIElement).CaptureMouse()
            Exit Sub
        End If
        AllCellsSelected = False
        If (Keyboard.IsKeyDown(Key.LeftCtrl) Or Keyboard.IsKeyDown(Key.RightCtrl)) Or (Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.RightShift)) Then
        Else
            If _selectedDataRowIndices.Count > 0 Then
                _selectedDataRowIndices.Clear()
                RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
            End If
            _selectedCellIndices.Clear()
            _selectedColumnIndices.Clear()
            DeSelectAllCells()
        End If
        '
        If Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.RightShift) Then
        Else
            _mouseDownVirtualRowIndex = CInt(Math.Floor(VerticalScrollbar.Value)) + GetTableRowIndex(gridPosition)
            _mouseDownColumnIndex = GetTableColumnIndex(gridPosition)
            _activeCellDataColumnIndex = _mouseDownColumnIndex
            _activeCellVirtualRowIndex = _mouseDownVirtualRowIndex
            RaiseEvent ActiveCellLocationChanged()
        End If
        If CellSelectable = True Then
            _mouseSelectionMode = SelectionMode.CellSelect
        ElseIf RowSelectable = True Then
            _mouseSelectionMode = SelectionMode.RowSelect
        ElseIf ColumnSelectable = True Then
            _mouseSelectionMode = SelectionMode.ColumnSelect
        Else
            _mouseSelectionMode = SelectionMode.None
        End If
        If e.ClickCount = 2 And Editable = True And _readOnlyColumns.Contains(_mouseDownColumnIndex) = False Then 'this is not optimal. (Actually I don't think it is bad. 08/25/2014 WLF)
            _mouseSelectionMode = SelectionMode.EditSelect
            Dim rowIndex As Int32 = GetTableRowIndex(gridPosition)
            _mouseDownVirtualRowIndex = CInt(Math.Floor(VerticalScrollbar.Value)) + GetTableRowIndex(gridPosition)
            Dim initialText As String = GetCellText(rowIndex, _mouseDownColumnIndex)
            _cellEditTextBox.Tag = New Tuple(Of String, Point)(initialText, New Point(_mouseDownColumnIndex, GetDataRowIndex(rowIndex)))
            Grid.SetColumn(_cellEditTextBox, _mouseDownColumnIndex)
            Grid.SetRow(_cellEditTextBox, rowIndex)
            _cellEditTextBox.Text = initialText
            If GridPanel.Children.Contains(_cellEditTextBox) Then GridPanel.Children.Remove(_cellEditTextBox)
            GridPanel.Children.Add(_cellEditTextBox)
            _cellEditTextBox.SelectAll()
            'RaiseEvent SelectionModeChanged(_MouseSelectionMode)
            Exit Sub
        End If
        CType(sender, UIElement).CaptureMouse()
        'RaiseEvent SelectionModeChanged(_MouseSelectionMode)
    End Sub
    Private Sub GridPanel_MouseMove(sender As Object, e As MouseEventArgs) Handles GridPanel.MouseMove
        If e.LeftButton = MouseButtonState.Pressed Then
            If _mouseSelectionMode = SelectionMode.None OrElse _mouseSelectionMode = SelectionMode.EditSelect Then
            Else
                AllCellsSelected = False
                If (Keyboard.IsKeyDown(Key.LeftCtrl) Or Keyboard.IsKeyDown(Key.RightCtrl)) Or (Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.RightShift)) Then
                    DeSelectAllCells()
                    SetSelectedCells()
                Else
                    DeSelectAllCells()
                End If
            End If
            Dim verticalScrollBarValue As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
            Dim gridPosition As Point = e.GetPosition(GridPanel)
            If _mouseSelectionMode = SelectionMode.CellSelect Then
                Dim tableRowIndex As Int32 = GetTableRowIndex(gridPosition)
                Dim mouseMoveDataRowIndex As Int32 = tableRowIndex + verticalScrollBarValue
                Dim mouseMoveColumnIndex As Int32 = GetTableColumnIndex(gridPosition)
                Dim rowStep As Int32 = -1
                Dim columnStep As Int32 = -1
                If mouseMoveDataRowIndex >= _activeCellVirtualRowIndex Then rowStep = 1
                If mouseMoveColumnIndex >= _activeCellDataColumnIndex Then columnStep = 1
                Dim startValue As Int32 = _activeCellVirtualRowIndex - verticalScrollBarValue
                If startValue < 0 Then startValue = 0
                If startValue > (_visibleRowCount - 1) Then startValue = _visibleRowCount - 1
                '
                For i As Int32 = _activeCellDataColumnIndex To mouseMoveColumnIndex Step columnStep
                    For j As Int32 = startValue To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        SelectCell(i, j)
                    Next
                Next
            ElseIf _mouseSelectionMode = SelectionMode.RowSelect Then
                Dim mouseMoveDataRowIndex As Int32 = GetTableRowIndex(gridPosition) + verticalScrollBarValue
                Dim rowStep As Int32 = 1
                If mouseMoveDataRowIndex < _mouseDownVirtualRowIndex Then rowStep = -1
                If _mouseDownVirtualRowIndex < verticalScrollBarValue Then
                    For i As Int32 = 0 To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                            SelectCell(j, i)
                        Next
                    Next
                ElseIf _mouseDownVirtualRowIndex > (verticalScrollBarValue + _visibleRowCount - 1) Then
                    For i As Int32 = _visibleRowCount - 1 To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                            SelectCell(j, i)
                        Next
                    Next
                Else
                    For i As Int32 = (_mouseDownVirtualRowIndex - verticalScrollBarValue) To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                            SelectCell(j, i)
                        Next
                    Next
                End If
            ElseIf _mouseSelectionMode = SelectionMode.ColumnSelect Then
                Dim columnIndex As Int32 = GetTableColumnIndex(gridPosition)
                Dim columnStep As Int32 = 1
                If columnIndex < _mouseDownColumnIndex Then columnStep = -1
                For i As Int32 = _mouseDownColumnIndex To columnIndex Step columnStep
                    For j As Int32 = 0 To _visibleRowCount - 1
                        SelectCell(i, j)
                    Next
                Next
            End If
            SetActiveCell()
        End If
    End Sub
    Private Sub GridPanel_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles GridPanel.MouseLeftButtonUp
        AllCellsSelected = False
        Dim gridPosition As Point = e.GetPosition(GridPanel)
        Dim verticalScrollBarValue As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        Dim mouseUpDataRowIndex As Int32 = GetTableRowIndex(gridPosition) + verticalScrollBarValue
        Dim mouseUpColumnIndex As Int32 = GetTableColumnIndex(gridPosition)
        If _mouseSelectionMode = SelectionMode.CellSelect Then
            If _activeCellVirtualRowIndex = mouseUpDataRowIndex AndAlso _activeCellDataColumnIndex = mouseUpColumnIndex Then
                If _selectedCellIndices.ContainsKey(_rowId(mouseUpDataRowIndex)) Then
                    _selectedCellIndices(_rowId(mouseUpDataRowIndex)).Add(mouseUpColumnIndex)
                Else
                    _selectedCellIndices.Add(_rowId(mouseUpDataRowIndex), New SortedSet(Of Int32)({mouseUpColumnIndex}))
                End If
            Else
                Dim rowStep As Int32 = -1
                Dim columnStep As Int32 = -1
                If mouseUpDataRowIndex >= _activeCellVirtualRowIndex Then rowStep = 1
                If mouseUpColumnIndex >= _activeCellDataColumnIndex Then columnStep = 1
                '
                For i As Int32 = _activeCellVirtualRowIndex To mouseUpDataRowIndex Step rowStep
                    If _selectedCellIndices.ContainsKey(_rowId(i)) = False Then _selectedCellIndices.Add(_rowId(i), New SortedSet(Of Int32))
                    For j As Int32 = _activeCellDataColumnIndex To mouseUpColumnIndex Step columnStep
                        _selectedCellIndices(_rowId(i)).Add(j)
                    Next
                Next
            End If
            SetSelectedCells()
        ElseIf _mouseSelectionMode = SelectionMode.RowSelect Then
            Dim rowStep As Int32 = 1
            If mouseUpDataRowIndex < _mouseDownVirtualRowIndex Then rowStep = -1
            For i As Int32 = _mouseDownVirtualRowIndex To mouseUpDataRowIndex Step rowStep
                _selectedDataRowIndices.Add(_rowId(i))
            Next
            _selectedDataRowIndices = _selectedDataRowIndices.Distinct.ToList
            _selectedDataRowIndices.Sort()
            _mouseDownColumnIndex = _activeCellDataColumnIndex
            _mouseDownVirtualRowIndex = _activeCellVirtualRowIndex
            SetSelectedCells()
            RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
        ElseIf _mouseSelectionMode = SelectionMode.ColumnSelect Then
            Dim columnStep As Int32 = 1
            If mouseUpColumnIndex < _mouseDownColumnIndex Then columnStep = -1
            For i As Int32 = _mouseDownColumnIndex To mouseUpColumnIndex Step columnStep
                _selectedColumnIndices.Add(i)
            Next
            _selectedColumnIndices = _selectedColumnIndices.Distinct.ToList
            _selectedColumnIndices.Sort()
            SetSelectedCells()
        ElseIf _mouseSelectionMode = SelectionMode.EditSelect Then
            _cellEditTextBox.Focus()
        End If
        '
        If _mouseSelectionMode <> SelectionMode.EditSelect Then
            If _selectedDataRowIndices.Count > 0 And _selectedRowsOnly = False Then
                ShowSelected.IsEnabled = True
                CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ShowSelectedIcon_22x22.png"))
                DeSelectAll.IsEnabled = True
                CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIcon_22x22.png"))
            ElseIf _selectedDataRowIndices.Count <= 0 And _selectedRowsOnly = True Then
                ShowAll_Checked(Nothing, Nothing)
            ElseIf _selectedRowsOnly = True Then
                'Do nothing
            Else
                ShowSelected.IsEnabled = False
                CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
                DeSelectAll.IsEnabled = False
                CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            End If
        End If
        '
        _mouseSelectionMode = SelectionMode.None
        Dim el As UIElement = CType(sender, UIElement)
        el.ReleaseMouseCapture()
        'RaiseEvent SelectionModeChanged(_MouseSelectionMode)
    End Sub
#End Region

#Region "Row Selection Logic"
    Private Sub RowsGrid_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        If _selectedRowsOnly = True Then
            _mouseSelectionMode = SelectionMode.None
            Exit Sub
        End If
        AllCellsSelected = False
        If (Keyboard.IsKeyDown(Key.LeftCtrl) Or Keyboard.IsKeyDown(Key.RightCtrl)) Or (Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.RightShift)) Then
        Else
            If _selectedDataRowIndices.Count > 0 Then
                _selectedDataRowIndices.Clear()
                RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
            End If
            _selectedCellIndices.Clear()
            _selectedColumnIndices.Clear()
            DeSelectAllCells()
        End If
        '
        Dim gridPosition As Point = e.GetPosition(GridPanel)
        If Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.RightShift) Then
        Else
            _mouseDownVirtualRowIndex = GetTableRowIndex(gridPosition) + CInt(Math.Floor(VerticalScrollbar.Value))
            _activeCellVirtualRowIndex = _mouseDownVirtualRowIndex
            Dim widthSums As Double
            Dim counter As Int32 = 0
            Do Until widthSums > HorizontalScrollViewer.HorizontalOffset
                widthSums += ColumnHeadersGrid.ColumnDefinitions(counter).ActualWidth
                counter += 1
            Loop
            _activeCellDataColumnIndex = counter - 1
        End If
        If RowSelectable = True Then
            _mouseSelectionMode = SelectionMode.RowSelect
        Else
            _mouseSelectionMode = SelectionMode.None
        End If
        Dim el As UIElement = CType(sender, UIElement)
        el.CaptureMouse()
    End Sub
    Private Sub RowsGrid_MouseMove(sender As Object, e As MouseEventArgs)
        Dim gridPosition As Point = e.GetPosition(GridPanel)
        If e.LeftButton = MouseButtonState.Pressed Then
            If _mouseSelectionMode = SelectionMode.None OrElse _mouseSelectionMode = SelectionMode.EditSelect Then
            Else
                AllCellsSelected = False
                If (Keyboard.IsKeyDown(Key.LeftCtrl) Or Keyboard.IsKeyDown(Key.RightCtrl)) Or (Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.RightShift)) Then
                    DeSelectAllCells()
                    SetSelectedCells()
                Else
                    DeSelectAllCells()
                End If
            End If
            Dim verticalScrollBarValue As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
            Dim mouseMoveDataRowIndex As Int32 = GetTableRowIndex(gridPosition) + verticalScrollBarValue
            If _mouseSelectionMode = SelectionMode.RowSelect Then
                Dim rowStep As Int32 = 1
                If mouseMoveDataRowIndex < _mouseDownVirtualRowIndex Then rowStep = -1
                If _mouseDownVirtualRowIndex < verticalScrollBarValue Then
                    For i As Int32 = 0 To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                            SelectCell(j, i)
                        Next
                    Next
                ElseIf _mouseDownVirtualRowIndex > (verticalScrollBarValue + _visibleRowCount - 1) Then
                    For i As Int32 = _visibleRowCount - 1 To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                            SelectCell(j, i)
                        Next
                    Next
                Else
                    For i As Int32 = (_mouseDownVirtualRowIndex - verticalScrollBarValue) To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                            SelectCell(j, i)
                        Next
                    Next
                End If
            ElseIf _mouseSelectionMode = SelectionMode.CellSelect Then
                Dim mouseMoveColumnIndex As Int32 = GetTableColumnIndex(gridPosition)
                Dim rowStep As Int32 = -1
                Dim columnStep As Int32 = -1
                If mouseMoveDataRowIndex >= _mouseDownVirtualRowIndex Then rowStep = 1
                If mouseMoveColumnIndex >= _mouseDownColumnIndex Then columnStep = 1
                '
                For i As Int32 = _mouseDownColumnIndex To mouseMoveColumnIndex Step columnStep
                    For j As Int32 = (_mouseDownVirtualRowIndex - verticalScrollBarValue) To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        SelectCell(i, j)
                    Next
                Next
            End If
            SetActiveCell()
        End If
    End Sub
    Private Sub RowsGrid_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        AllCellsSelected = False
        Dim verticalScrollBarValue As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        Dim gridPosition As Point = e.GetPosition(GridPanel)
        Dim mouseUpVirtualRowIndex As Int32 = GetTableRowIndex(gridPosition) + verticalScrollBarValue
        If _mouseSelectionMode = SelectionMode.RowSelect Then
            Dim rowStep As Int32 = 1
            If mouseUpVirtualRowIndex < _mouseDownVirtualRowIndex Then rowStep = -1
            For i As Int32 = _mouseDownVirtualRowIndex To mouseUpVirtualRowIndex Step rowStep
                _selectedDataRowIndices.Add(_rowId(i))
            Next
            _selectedDataRowIndices = _selectedDataRowIndices.Distinct.ToList
            _selectedDataRowIndices.Sort()
            SetSelectedCells()
            RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
        ElseIf _mouseSelectionMode = SelectionMode.CellSelect Then
            If _mouseDownVirtualRowIndex = mouseUpVirtualRowIndex AndAlso _mouseDownColumnIndex = 0 Then
                If _selectedCellIndices.ContainsKey(_rowId(mouseUpVirtualRowIndex)) = False Then _selectedCellIndices.Add(_rowId(mouseUpVirtualRowIndex), New SortedSet(Of Int32))
                _selectedCellIndices(_rowId(mouseUpVirtualRowIndex)).Add(0)
                SelectCell(0, mouseUpVirtualRowIndex - verticalScrollBarValue)
            Else
                Dim rowStep As Int32 = -1
                Dim columnStep As Int32 = -1
                If mouseUpVirtualRowIndex >= _mouseDownVirtualRowIndex Then rowStep = 1
                If 0 >= _mouseDownColumnIndex Then columnStep = 1
                '
                For i As Int32 = _mouseDownVirtualRowIndex To mouseUpVirtualRowIndex Step rowStep
                    If _selectedCellIndices.ContainsKey(_rowId(i)) = False Then _selectedCellIndices.Add(_rowId(i), New SortedSet(Of Int32))
                    For j As Int32 = _mouseDownColumnIndex To 0 Step columnStep
                        _selectedCellIndices(_rowId(i)).Add(j)
                    Next
                Next
                SetSelectedCells()
            End If
        End If
        '
        If _selectedDataRowIndices.Count > 0 And _selectedRowsOnly = False Then
            ShowSelected.IsEnabled = True
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ShowSelectedIcon_22x22.png"))
            DeSelectAll.IsEnabled = True
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIcon_22x22.png"))
        ElseIf _selectedDataRowIndices.Count <= 0 And _selectedRowsOnly = True Then
            ShowAll_Checked(Nothing, Nothing)
        ElseIf _selectedRowsOnly = True Then
            'Do nothing
        Else
            ShowSelected.IsEnabled = False
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            DeSelectAll.IsEnabled = False
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            ShowAll_Checked(Nothing, Nothing)
        End If
        _mouseSelectionMode = SelectionMode.None
        Dim el As UIElement = CType(sender, UIElement)
        el.ReleaseMouseCapture()
    End Sub
    Private Sub RowsGrid_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)
        Dim dataRowIndex As Int32 = GetDataRowIndex(GetTableRowIndex(e.GetPosition(GridPanel)))
        '
        If _selectedDataRowIndices.BinarySearch(dataRowIndex) < 0 Then
            _selectedDataRowIndices.Clear()
            _selectedDataRowIndices.Add(dataRowIndex)
            DeSelectAllCells()
            SetSelectedCells()
            RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
        End If
        '
        Dim rowMenu As New ContextMenu
        If Editable = True Then
            Dim headerText As String = "Delete Rows"
            If _selectedDataRowIndices.Count = 1 Then headerText = "Delete Row"
            Dim rowMenuItem As New MenuItem With {.Header = headerText, .IsEnabled = Editable, .Icon = New Image() With {.Source = New BitmapImage(New Uri("pack://application:,,/GenericControls;component/Resources/delete_row.png"))}}
            AddHandler rowMenuItem.Click, AddressOf DeleteRows
            rowMenu.Items.Add(rowMenuItem)
        End If
        '
        rowMenu.IsOpen = True
        RaiseEvent RowRightButtonUp(rowMenu, dataRowIndex)
    End Sub
    Private Sub DeleteRows(sender As Object, e As RoutedEventArgs)
        DataView.DeleteRows(_selectedDataRowIndices.ToArray())
    End Sub
#End Region

#Region "Column Select Logic"
    Private Sub ColumnsGrid_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        AllCellsSelected = False
        If (Keyboard.IsKeyDown(Key.LeftCtrl) Or Keyboard.IsKeyDown(Key.RightCtrl)) Or (Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.RightShift)) Or _selectedRowsOnly = True Then
        Else
            If _selectedDataRowIndices.Count > 0 Then
                _selectedDataRowIndices.Clear()
                RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
            End If
            _selectedCellIndices.Clear()
            _selectedColumnIndices.Clear()
            DeSelectAllCells()
        End If
        '
        Dim gridPosition As Point = e.GetPosition(GridPanel)
        If Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.RightShift) Then
        Else
            _mouseDownColumnIndex = GetTableColumnIndex(gridPosition)
            _activeCellVirtualRowIndex = CInt(Math.Floor(VerticalScrollbar.Value))
            _activeCellDataColumnIndex = _mouseDownColumnIndex
        End If
        If _selectedRowsOnly = True Then
            _mouseSelectionMode = SelectionMode.None
        ElseIf ColumnSelectable = True Then
            _mouseSelectionMode = SelectionMode.ColumnSelect
        Else
            _mouseSelectionMode = SelectionMode.None
        End If
        '
        If e.ClickCount = 2 Then
            _mouseSelectionMode = SelectionMode.None
            _mouseDownColumnIndex = GetTableColumnIndex(gridPosition)
            If _selectedRowsOnly = False Then _selectedColumnIndices.Add(_mouseDownColumnIndex)
            Select Case _columnSortOrder
                Case SortOrder.Ascending
                    SortColumnDescending()
                Case SortOrder.Descending
                    SortColumnAscending()
                Case SortOrder.None
                    SortColumnDescending()
            End Select
            'RaiseEvent SelectionModeChanged(_MouseSelectionMode)
            Exit Sub
        End If
        Dim el As UIElement = CType(sender, UIElement)
        el.CaptureMouse()
    End Sub
    Private Sub ColumnsGrid_MouseMove(sender As Object, e As MouseEventArgs)
        Dim gridPosition As Point = e.GetPosition(GridPanel)
        If e.LeftButton = MouseButtonState.Pressed Then
            If _mouseSelectionMode = SelectionMode.None Then Exit Sub
            AllCellsSelected = False
            If (Keyboard.IsKeyDown(Key.LeftCtrl) Or Keyboard.IsKeyDown(Key.RightCtrl)) Or (Keyboard.IsKeyDown(Key.LeftShift) Or Keyboard.IsKeyDown(Key.RightShift)) Then
                DeSelectAllCells()
                SetSelectedCells()
            Else
                DeSelectAllCells()
            End If
            If _mouseSelectionMode = SelectionMode.ColumnSelect Then
                Dim columnIndex As Int32 = GetTableColumnIndex(gridPosition)
                Dim columnStep As Int32 = 1
                If columnIndex < _mouseDownColumnIndex Then columnStep = -1
                For i As Int32 = _mouseDownColumnIndex To columnIndex Step columnStep
                    For j As Int32 = 0 To _visibleRowCount - 1
                        SelectCell(i, j)
                    Next
                Next
            ElseIf _mouseSelectionMode = SelectionMode.CellSelect Then
                Dim verticalScrollBarValue As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
                Dim mouseMoveDataRowIndex As Int32 = verticalScrollBarValue
                Dim mouseMoveColumnIndex As Int32 = GetTableColumnIndex(gridPosition)
                Dim rowStep As Int32 = -1
                Dim columnStep As Int32 = -1
                If mouseMoveDataRowIndex >= _mouseDownVirtualRowIndex Then rowStep = 1
                If mouseMoveColumnIndex >= _mouseDownColumnIndex Then columnStep = 1
                '
                For i As Int32 = _mouseDownColumnIndex To mouseMoveColumnIndex Step columnStep
                    For j As Int32 = (_mouseDownVirtualRowIndex - verticalScrollBarValue) To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        SelectCell(i, j)
                    Next
                Next
            End If
            SetActiveCell()
        End If
    End Sub
    Private Sub ColumnsGrid_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        AllCellsSelected = False
        Dim gridPosition As Point = e.GetPosition(GridPanel)
        Dim mouseUpColumnIndex As Int32 = GetTableColumnIndex(gridPosition)
        If _mouseSelectionMode = SelectionMode.ColumnSelect Then
            Dim columnStep As Int32 = 1
            If mouseUpColumnIndex < _mouseDownColumnIndex Then columnStep = -1
            For i As Int32 = _mouseDownColumnIndex To mouseUpColumnIndex Step columnStep
                _selectedColumnIndices.Add(i)
            Next
            _selectedColumnIndices = _selectedColumnIndices.Distinct.ToList
            _selectedColumnIndices.Sort()
            SetSelectedCells()
        ElseIf _mouseSelectionMode = SelectionMode.CellSelect Then
            Dim verticalScrollBarValue As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
            Dim mouseUpDataRowIndex As Int32 = verticalScrollBarValue
            If _mouseDownVirtualRowIndex = mouseUpDataRowIndex AndAlso _mouseDownColumnIndex = mouseUpColumnIndex Then
                If _selectedCellIndices.ContainsKey(_rowId(mouseUpDataRowIndex)) = False Then _selectedCellIndices.Add(_rowId(mouseUpDataRowIndex), New SortedSet(Of Int32))

                _selectedCellIndices(_rowId(mouseUpDataRowIndex)).Add(mouseUpColumnIndex)
                SelectCell(mouseUpColumnIndex, mouseUpDataRowIndex - verticalScrollBarValue)
            Else
                Dim rowStep As Int32 = -1
                Dim columnStep As Int32 = -1
                If mouseUpDataRowIndex >= _mouseDownVirtualRowIndex Then rowStep = 1
                If mouseUpColumnIndex >= _mouseDownColumnIndex Then columnStep = 1
                '
                For i As Int32 = _mouseDownVirtualRowIndex To mouseUpDataRowIndex Step rowStep
                    If _selectedCellIndices.ContainsKey(_rowId(i)) = False Then _selectedCellIndices.Add(_rowId(i), New SortedSet(Of Int32))
                    For j As Int32 = _mouseDownColumnIndex To mouseUpColumnIndex Step columnStep
                        _selectedCellIndices(_rowId(i)).Add(j)
                    Next
                Next
                SetSelectedCells()
            End If
        End If
        '
        If _selectedDataRowIndices.Count > 0 And _selectedRowsOnly = False Then
            ShowSelected.IsEnabled = True
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ShowSelectedIcon_22x22.png"))
            DeSelectAll.IsEnabled = True
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIcon_22x22.png"))
        ElseIf _selectedDataRowIndices.Count <= 0 And _selectedRowsOnly = True Then
            ShowAll_Checked(Nothing, Nothing)
        ElseIf _selectedRowsOnly = True Then
            'Do nothing
        Else
            ShowSelected.IsEnabled = False
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            DeSelectAll.IsEnabled = False
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
        End If
        _mouseSelectionMode = SelectionMode.None
        Dim el As UIElement = CType(sender, UIElement)
        el.ReleaseMouseCapture()
    End Sub
#End Region

#Region "Select All Logic"
    Private Sub SelectAllLeftMouseDown(sender As Object, e As MouseButtonEventArgs)
        _mouseSelectionMode = SelectionMode.All
    End Sub
    Private Sub SelectAllLeftMouseUp(sender As Object, e As MouseButtonEventArgs)
        If _mouseSelectionMode = SelectionMode.All Then
            AllCellsSelected = Not AllCellsSelected
            _selectedDataRowIndices.Clear()
            _selectedCellIndices.Clear()
            _selectedColumnIndices.Clear()
        ElseIf _mouseSelectionMode = SelectionMode.CellSelect Then
            AllCellsSelected = False
            Dim verticalScrollBarValue As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
            Dim mouseUpDataRowIndex As Int32 = verticalScrollBarValue
            Dim mouseUpColumnIndex As Int32 = 0
            If _mouseDownVirtualRowIndex = mouseUpDataRowIndex AndAlso _mouseDownColumnIndex = mouseUpColumnIndex Then
                If _selectedCellIndices.ContainsKey(_rowId(mouseUpDataRowIndex)) = False Then _selectedCellIndices.Add(_rowId(mouseUpDataRowIndex), New SortedSet(Of Int32))
                _selectedCellIndices(_rowId(mouseUpDataRowIndex)).Add(mouseUpColumnIndex)
                SelectCell(mouseUpColumnIndex, mouseUpDataRowIndex - verticalScrollBarValue)
            Else
                Dim rowStep As Int32 = -1
                Dim columnStep As Int32 = -1
                If mouseUpDataRowIndex >= _mouseDownVirtualRowIndex Then rowStep = 1
                If mouseUpColumnIndex >= _mouseDownColumnIndex Then columnStep = 1
                '
                For i As Int32 = _mouseDownVirtualRowIndex To mouseUpDataRowIndex Step rowStep
                    If _selectedCellIndices.ContainsKey(_rowId(i)) = False Then _selectedCellIndices.Add(_rowId(i), New SortedSet(Of Int32))
                    For j As Int32 = _mouseDownColumnIndex To mouseUpColumnIndex Step columnStep
                        _selectedCellIndices(_rowId(i)).Add(j)
                    Next
                Next
            End If
        Else
            AllCellsSelected = False
        End If
        DeSelectAllCells()
        SetSelectedCells()
        _mouseSelectionMode = SelectionMode.None
    End Sub
#End Region

#End Region

#Region "Copy and Paste Logic"
    'Private Sub TableControlGrid_PreviewKeyDown(sender As Object, e As Input.KeyEventArgs) Handles TableControlGrid.PreviewKeyDown
    '    If e.Key = Key.Escape Then
    '        Clipboard.Clear()
    '        Exit Sub
    '    End If
    '    If Keyboard.IsKeyDown(Key.LeftCtrl) Or Keyboard.IsKeyDown(Key.RightCtrl) Then
    '        If e.Key = Key.V And Editable = True Then
    '            If Clipboard.ContainsText AndAlso _SelectedRowsOnly = False Then Paste()
    '        ElseIf e.Key = Key.C Then
    '            Copy()
    '        ElseIf e.Key = Key.Z Then
    '            If Undo.IsEnabled = True Then _EditSession.Undo()
    '        End If
    '    End If
    'End Sub
    Private Sub GridPanel_RightMouseUp(sender As Object, e As MouseButtonEventArgs) Handles GridPanel.MouseRightButtonUp
        Dim enableCopy As Boolean = False
        If AllCellsSelected = True Then
            enableCopy = True
        ElseIf _selectedCellIndices.Count > 0 Then
            enableCopy = True
        ElseIf _selectedColumnIndices.Count > 0 Then
            enableCopy = True
        ElseIf _selectedDataRowIndices.Count > 0 Then
            enableCopy = True
        End If
        '
        Dim gridMenu As New ContextMenu
        Dim gridMenuItem As New MenuItem With {.IsEnabled = enableCopy, .Header = "Copy", .Icon = New Image() With {.Source = New BitmapImage(New Uri("pack://application:,,/GenericControls;component/Resources/copy.png"))}}
        AddHandler gridMenuItem.Click, AddressOf Copy
        gridMenu.Items.Add(gridMenuItem)
        '
        gridMenuItem = New MenuItem With {.IsEnabled = enableCopy, .Header = "Copy with Headers", .Icon = New Image() With {.Source = New BitmapImage(New Uri("pack://application:,,/GenericControls;component/Resources/copy_w_headers.png"))}}
        AddHandler gridMenuItem.Click, AddressOf CopyWithHeaders
        gridMenu.Items.Add(gridMenuItem)
        '
        If Editable = True Then
            gridMenuItem = New MenuItem With {.Header = "Paste", .Icon = New Image() With {.Source = New BitmapImage(New Uri("pack://application:,,/GenericControls;component/Resources/paste.png"))}}
            AddHandler gridMenuItem.Click, AddressOf Paste
            If Clipboard.ContainsText = False OrElse _selectedRowsOnly = True Then gridMenuItem.IsEnabled = False
            gridMenu.Items.Add(gridMenuItem)
        End If
        gridMenu.IsOpen = True
    End Sub
    Private Sub Paste()
        Try
            PasteClipboard()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Function GetClipboardData() As String()()
        'Based on the following linq nightmare: Clipboard.GetText().Split(ControlChars.Lf).[Select](Function(row) row.Split(ControlChars.Tab).[Select](Function(cell) If(cell.Length > 0 AndAlso cell(cell.Length - 1) = ControlChars.Cr, cell.Substring(0, cell.Length - 1), cell)).ToArray()).Where(Function(a) a.Any(Function(b) b.Length > 0)).ToArray()
        Dim clipText = Clipboard.GetText()
        Dim clipTextLineSplit = clipText.Split(ControlChars.Lf)
        Dim result(clipTextLineSplit.Count - 1)() As String
        Dim clipboardRows = clipTextLineSplit.Select(Function(r) r.Split(ControlChars.Tab))
        Dim counter As Int32 = 0
        For Each row In clipboardRows
            Dim rowData(row.Count - 1) As String
            For i As Int32 = 0 To row.Count - 1
                If row(i).Length > 0 AndAlso row(row.Length - 1) = ControlChars.Cr Then
                    rowData(i) = row(i).Substring(0, row(i).Length - 1)
                Else
                    rowData(i) = row(i)
                End If
            Next
            '
            'optionally trim empty cells (this caused errors when copying with empty cells present).
            'Where(Function(a) a.Any(Function(b) b.Length > 0)
            '
            result(counter) = rowData
            counter += 1
        Next
        '
        Return result
    End Function
    Private Sub PasteClipboard()
        If IsSelectionContinuous() = False Then Throw New Exception("Selection must be continuous to paste.")
        '

        Dim clipboardData As String()() = GetClipboardData()
        Dim dataBaseRowIndex, columnIndex As Int32
        Dim rowIndices As New List(Of Int32)
        Dim columnIndices As New List(Of Int32)
        Dim editValues As New List(Of Object)
        If AllCellsSelected = True Then
            For i As Int32 = 0 To DataView.NumberOfRows - 1
                If i = clipboardData.Count Then Exit For
                For j As Int32 = 0 To DataView.ColumnNames.Count - 1
                    If j = clipboardData(i).Count Then Exit For
                    If _readOnlyColumns.Contains(j) Then Continue For
                    rowIndices.Add(_rowId(i))
                    columnIndices.Add(j)
                    editValues.Add(clipboardData(i)(j))
                Next
            Next
        ElseIf _selectedColumnIndices.Count > 0 Then
            For i As Int32 = 0 To DataView.NumberOfRows - 1
                If i = clipboardData.Count Then Exit For
                For j As Int32 = 0 To _selectedColumnIndices.Count - 1
                    If j = clipboardData(i).Count Then Exit For
                    If _readOnlyColumns.Contains(_selectedColumnIndices(j)) Then Continue For
                    rowIndices.Add(_rowId(i))
                    columnIndices.Add(_selectedColumnIndices(j))
                    editValues.Add(clipboardData(i)(j))
                Next
            Next
        ElseIf _selectedDataRowIndices.Count > 0 Then
            Dim rowIndexCounter As Int32 = 0
            For Each r As KeyValuePair(Of Int32, Int32) In GetSelectedRowVirtualRowIndices()
                If rowIndexCounter = clipboardData.Count Then Exit For
                For i As Int32 = 0 To DataView.ColumnNames.Count - 1
                    If i = clipboardData(rowIndexCounter).Count Then Exit For
                    If _readOnlyColumns.Contains(i) Then Continue For
                    rowIndices.Add(r.Value)
                    columnIndices.Add(i)
                    editValues.Add(clipboardData(rowIndexCounter)(i))
                Next
                rowIndexCounter += 1
            Next
        ElseIf _selectedCellIndices.Count > 0 Then
            dataBaseRowIndex = _selectedCellIndices.First.Key
            columnIndex = _selectedCellIndices.First.Value.First
            If _selectedCellIndices.Count = 1 AndAlso _selectedCellIndices.First.Value.Count = 1 Then
                For i As Int32 = 0 To clipboardData.Count - 1
                    If i = clipboardData.Count Then Exit For
                    If _rowOffset(dataBaseRowIndex) + i >= DataView.NumberOfRows Then Exit For
                    For j As Int32 = 0 To clipboardData(i).Count - 1
                        If j = clipboardData(i).Count Then Exit For
                        If columnIndex + j >= DataView.ColumnNames.Count Then Exit For
                        If _readOnlyColumns.Contains(CInt(columnIndex + j)) Then Continue For
                        rowIndices.Add(_rowId(_rowOffset(dataBaseRowIndex) + i))
                        columnIndices.Add(columnIndex + j)
                        editValues.Add(clipboardData(i)(j))
                    Next
                Next
            Else
                '
                Dim rowIndexCounter As Int32 = 0
                Dim columnIndexCounter As Int32
                For Each r As KeyValuePair(Of Int32, Int32) In GetSelectedCellVirtualRowIndices()
                    If rowIndexCounter = clipboardData.Count Then Exit For
                    columnIndexCounter = 0
                    For Each column As Int32 In _selectedCellIndices(r.Value)
                        If columnIndexCounter = clipboardData(rowIndexCounter).Count Then Exit For
                        If _readOnlyColumns.Contains(column) Then Continue For
                        rowIndices.Add(r.Value)
                        columnIndices.Add(column)
                        editValues.Add(clipboardData(rowIndexCounter)(columnIndexCounter))
                        columnIndexCounter += 1
                    Next
                    rowIndexCounter += 1
                Next
            End If
        Else
            Throw New Exception("No Cells are currently selected to paste into.")
        End If
        '
        DataView.EditCells(rowIndices.ToArray(), columnIndices.ToArray(), editValues.ToArray())
        UpdateVisibleRows()
        UpdateUndoRedoButtons()
    End Sub
    Private Sub ExportTableButton_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataView) Then Exit Sub
        Dim filters As String = "comma delimited(*.csv) |*.csv|database(*.dbf) |*.dbf|Excel(*.xlsx) |*.xlsx|Sqlite(*.sqlite) |*.sqlite"
        Try
            Dim saveFileBrowser As New SaveFileDialog With {.Filter = filters, .FilterIndex = 3}
            If saveFileBrowser.ShowDialog = True Then
                Select Case System.IO.Path.GetExtension(saveFileBrowser.FileName.ToString)
                    Case ".csv"
                        DataView.ExportToCsv(saveFileBrowser.FileName.ToString)
                    Case ".dbf"
                        DataView.ExportToDbf(saveFileBrowser.FileName.ToString)
                    Case ".xlsx"
                        DataView.ExportToXlsx(saveFileBrowser.FileName.ToString)
                    Case ".sqlite"
                        DataView.ExportToSqlite(saveFileBrowser.FileName.ToString, DataView.TableName)
                    Case Else
                        Throw New Exception("selected file format extension '" & IO.Path.GetExtension(saveFileBrowser.FileName.ToString) & "' is not supported for export.")
                End Select
            Else
                'FileSaveDialog = ""
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub CopyButton_Click(sender As Object, e As RoutedEventArgs)
        Copy()
    End Sub

    Private Sub CopyWithHeadersButton_Click(sender As Object, e As RoutedEventArgs)
        CopyWithHeaders()
    End Sub
    Private Sub Copy()
        Try
            CaptureSelectionToClipBoard(False)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub CopyWithHeaders()
        Try
            CaptureSelectionToClipBoard(True)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub PasteButton_Click(sender As Object, e As RoutedEventArgs)
        Paste()
    End Sub

    Private Sub SelectAllButton_Click(sender As Object, e As RoutedEventArgs)
        AllCellsSelected = Not AllCellsSelected
        _selectedDataRowIndices.Clear()
        _selectedCellIndices.Clear()
        _selectedColumnIndices.Clear()
        DeSelectAllCells()
        SetSelectedCells()
    End Sub




    Private Sub CaptureSelectionToClipBoard(includeHeaders As Boolean)
        If IsSelectionUniform() = False Then Throw New Exception("Selection must be uniform to copy.")
        Clipboard.Clear()
        '
        If AllCellsSelected = True Then
            CopyAllToClipboard(includeHeaders)
        ElseIf _selectedDataRowIndices.Count > 0 Then
            CopySelectedRowsToClipboard(includeHeaders)
        ElseIf _selectedColumnIndices.Count > 0 Then
            CopySelectedColumnsToClipboard(includeHeaders)
        ElseIf _selectedCellIndices.Count > 0 Then
            CopySelectedCellsToClipboard(includeHeaders)
        End If
    End Sub
    Private Sub CopyAllToClipboard(includeHeaders As Boolean)
        If _selectedColumnIndices.Count * DataView.NumberOfRows > 50000 Then
            Dim msgString As String = "Operation will copy " & DataView.NumberOfRows * DataView.ColumnNames.Count & " cell values to the clipboard.  Are you sure you want to copy that much data to the clipboard?"
            If MsgBox(msgString, MsgBoxStyle.YesNo, "Large Amount of Data To Clipboard") <> MsgBoxResult.Yes Then Exit Sub
        End If
        '
        Dim boardText As New Text.StringBuilder
        '
        Try
            Mouse.OverrideCursor = Cursors.Wait
            '
            If includeHeaders = True Then
                boardText.Append(DataView.ColumnNames(0))
                For i As Int32 = 1 To DataView.ColumnNames.Count - 1
                    boardText.Append(ChrW(9) & DataView.ColumnNames(i))
                Next
                boardText.Append(ChrW(10))
            End If
            '
            Dim readerRow() As Object
            For i As Int32 = 0 To DataView.NumberOfRows - 1
                readerRow = DataView.GetRow(_rowId(i))
                boardText.Append(readerRow(0).ToString)
                For j As Int32 = 1 To DataView.ColumnNames.Count - 1
                    boardText.Append(ChrW(9) & readerRow(j).ToString)
                Next
                If i < (DataView.NumberOfRows - 1) Then boardText.Append(ChrW(10))
            Next
            Mouse.OverrideCursor = Nothing
        Catch ex As Exception
            Mouse.OverrideCursor = Nothing
            Throw New Exception("Error copying data to clipboard.")
        End Try
        '
        Clipboard.SetText(boardText.ToString)
    End Sub
    Private Sub CopySelectedColumnsToClipboard(includeHeaders As Boolean)
        If _selectedColumnIndices.Count * DataView.NumberOfRows > 50000 Then
            Dim msgString As String = "Operation will copy " & _selectedColumnIndices.Count * DataView.NumberOfRows & " cell values to the clipboard.  Are you sure you want to copy that much data to the clipboard?"
            If MsgBox(msgString, MsgBoxStyle.YesNo, "Large Amount of Data To Clipboard") <> MsgBoxResult.Yes Then Exit Sub
        End If
        '
        Dim boardText As New Text.StringBuilder
        '
        Try
            Dim columnData As New List(Of Object())
            Mouse.OverrideCursor = Cursors.Wait
            '
            For i As Int32 = 0 To _selectedColumnIndices.Count - 1
                If includeHeaders Then boardText.Append(DataView.ColumnNames(_selectedColumnIndices(i)) & ChrW(9))
                columnData.Add(DataView.GetColumn(DataView.ColumnNames(_selectedColumnIndices(i))))
            Next
            If includeHeaders Then boardText(boardText.Length - 1) = ChrW(10)
            '
            For i As Int32 = 0 To DataView.NumberOfRows - 1
                boardText.Append(columnData(0)(_rowId(i)).ToString)
                For j As Int32 = 1 To columnData.Count - 1
                    boardText.Append(ChrW(9) & columnData(j)(_rowId(i)).ToString)
                Next
                boardText.Append(ChrW(10))
            Next
            boardText.Remove(boardText.Length - 1, 1)
            Mouse.OverrideCursor = Nothing
        Catch ex As Exception
            Mouse.OverrideCursor = Nothing
            Throw New Exception("Error copying data to clipboard.")
        End Try
        '
        Clipboard.SetText(boardText.ToString)
    End Sub
    Private Sub CopySelectedRowsToClipboard(includeHeaders As Boolean)
        If _selectedDataRowIndices.Count * DataView.ColumnNames.Count > 50000 Then
            Dim msgString As String = "Operation will copy " & _selectedDataRowIndices.Count * DataView.ColumnNames.Count & " cell values to the clipboard.  Are you sure you want to copy that much data to the clipboard?"
            If MsgBox(msgString, MsgBoxStyle.YesNo, "Large Amount of Data To Clipboard") <> MsgBoxResult.Yes Then Exit Sub
        End If
        '
        Dim boardText As New Text.StringBuilder
        Dim readerRow() As Object
        Try
            Mouse.OverrideCursor = Cursors.Wait
            '
            If includeHeaders = True Then
                boardText.Append(DataView.ColumnNames(0))
                For i As Int32 = 1 To DataView.ColumnNames.Count - 1
                    boardText.Append(ChrW(9) & DataView.ColumnNames(i))
                Next
                boardText.Append(ChrW(10))
            End If
            '
            If _columnSortOrder <> SortOrder.None Then
                '
                For Each r As KeyValuePair(Of Int32, Int32) In GetSelectedRowVirtualRowIndices()
                    readerRow = DataView.GetRow(r.Value)
                    boardText.Append(readerRow(0).ToString)
                    For j As Int32 = 1 To DataView.ColumnNames.Count - 1
                        boardText.Append(ChrW(9) & readerRow(j).ToString)
                    Next
                    boardText.Append(ChrW(10))
                Next
                boardText.Remove(boardText.Length - 1, 1)
            Else
                For i As Int32 = 0 To _selectedDataRowIndices.Count - 1
                    readerRow = DataView.GetRow(_selectedDataRowIndices(i))
                    boardText.Append(readerRow(0).ToString)
                    For j As Int32 = 1 To DataView.ColumnNames.Count - 1
                        boardText.Append(ChrW(9) & readerRow(j).ToString)
                    Next
                    boardText.Append(ChrW(10))
                Next
                boardText.Remove(boardText.Length - 1, 1)
            End If
            Mouse.OverrideCursor = Nothing
        Catch ex As Exception
            Mouse.OverrideCursor = Nothing
            Throw New Exception("Error copying data to clipboard.")
        End Try
        '
        Clipboard.SetText(boardText.ToString)
    End Sub
    Private Sub CopySelectedCellsToClipboard(includeHeaders As Boolean)
        If _selectedCellIndices.Count > 50000 Then
            Dim msgString As String = "Operation will copy " & _selectedCellIndices.Count & " cell values to the clipboard.  It can take a long time to copy this much data, are you sure you want to copy that much data to the clipboard?"
            If MsgBox(msgString, MsgBoxStyle.YesNo, "Large Amount of Data To Clipboard") <> MsgBoxResult.Yes Then Exit Sub
        End If
        '
        Dim boardText As New Text.StringBuilder
        '
        Try
            Mouse.OverrideCursor = Cursors.Wait
            Dim keysInVisualOrder As New SortedDictionary(Of Int32, Int32)
            For Each dataRowIndex As Int32 In _selectedCellIndices.Keys
                keysInVisualOrder.Add(_rowOffset(dataRowIndex), dataRowIndex)
            Next
            '
            If includeHeaders = True Then
                For Each column As Int32 In _selectedCellIndices(_selectedCellIndices.Keys.First)
                    boardText.Append(DataView.ColumnNames(column) & ChrW(9))
                Next
                boardText(boardText.Length - 1) = ChrW(10)
            End If
            '
            Dim row() As Object
            Dim rowCellEdits As SortedSet(Of Int32)
            For Each dataRowIndex As Int32 In keysInVisualOrder.Values
                rowCellEdits = _selectedCellIndices(dataRowIndex)
                row = DataView.GetRow(dataRowIndex)
                '
                For Each column As Int32 In rowCellEdits
                    boardText.Append(row(column).ToString & ChrW(9))
                Next
                boardText.Chars(boardText.Length - 1) = ChrW(10)
            Next
            boardText.Remove(boardText.Length - 1, 1)
            Mouse.OverrideCursor = Nothing
        Catch ex As Exception
            Mouse.OverrideCursor = Nothing
            Throw New Exception("Error copying data to clipboard.")
        End Try
        '
        Clipboard.SetDataObject(boardText.ToString)
    End Sub
    Private Function IsSelectionUniform() As Boolean
        If _selectedColumnIndices.Count > 0 AndAlso _selectedDataRowIndices.Count > 0 Then Return False
        'All Cells Selected
        If AllCellsSelected = True Then
            Return True
        ElseIf _selectedDataRowIndices.Count > 0 Then
            'Check for non-uniform cell selection
            If _selectedCellIndices.Count > 0 Then
                For Each rowSelectedCells As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
                    If _selectedDataRowIndices.BinarySearch(rowSelectedCells.Key) < 0 Then Return False
                Next
            End If
        ElseIf _selectedColumnIndices.Count > 0 Then
            'check for non-uniform cell selection
            If _selectedCellIndices.Count > 0 Then
                For Each rowSelectedCells As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
                    For Each column As Int32 In rowSelectedCells.Value
                        If _selectedColumnIndices.BinarySearch(column) < 0 Then Return False
                    Next
                Next
            End If
        ElseIf _selectedCellIndices.Count > 0 Then
            Dim columnCount As Int32 = _selectedCellIndices.First.Value.Count
            Dim columnIndices() As Int32 = _selectedCellIndices.First.Value.ToArray
            For Each rowSelectedCells As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
                If columnCount <> rowSelectedCells.Value.Count Then Return False
                For i As Int32 = 0 To columnIndices.Count - 1
                    If rowSelectedCells.Value.Contains(columnIndices(i)) = False Then Return False
                Next
            Next
        End If
        '
        Return True

    End Function
    Private Function IsSelectionContinuous() As Boolean
        If _selectedColumnIndices.Count > 0 AndAlso _selectedDataRowIndices.Count > 0 Then Return False
        If AllCellsSelected = True Then
            Return True
        ElseIf _selectedColumnIndices.Count > 0 Then
            For i As Int32 = 1 To _selectedColumnIndices.Count - 1
                If _selectedColumnIndices(i) - _selectedColumnIndices(i - 1) <> 1 Then Return False
            Next
            If _selectedCellIndices.Count > 0 Then
                If _selectedCellIndices.Count > 0 Then
                    For Each selectedCellsByRow As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
                        For Each columnIndex As Int32 In selectedCellsByRow.Value
                            If _selectedColumnIndices.BinarySearch(columnIndex) < 0 Then Return False
                        Next
                    Next
                End If
            End If
        ElseIf _selectedDataRowIndices.Count > 0 Then
            'Test the Rows
            Dim rowIndex As Int32
            Dim virtualSelectedRowIndices As SortedDictionary(Of Int32, Int32) = GetSelectedRowVirtualRowIndices()
            rowIndex = virtualSelectedRowIndices.First.Key
            For Each r As KeyValuePair(Of Int32, Int32) In virtualSelectedRowIndices
                If r.Key - rowIndex > 1 Then Return False
                rowIndex = r.Key
            Next
            '
            If _selectedCellIndices.Count > 0 Then
                For Each selectedCellsByRow As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
                    If _selectedDataRowIndices.BinarySearch(selectedCellsByRow.Key) < 0 Then Return False
                Next
            End If
        ElseIf _selectedCellIndices.Count > 0 Then
            If _selectedCellIndices.Count = 1 Then Return True
            'Test the Columns
            Dim columnIndices() As Int32 = _selectedCellIndices.First.Value.ToArray
            Dim tempRowIndex As Int32 = _selectedCellIndices.First.Key
            Dim virtualRowsSorted As New List(Of Int32)(_selectedCellIndices.Count)
            For Each selectedCellsByRow As KeyValuePair(Of Int32, SortedSet(Of Int32)) In _selectedCellIndices
                virtualRowsSorted.Add(_rowOffset(selectedCellsByRow.Key))
                If selectedCellsByRow.Key = tempRowIndex Then Continue For
                For i As Int32 = 0 To columnIndices.Count - 1
                    If selectedCellsByRow.Value.Contains(columnIndices(i)) = False Then Return False
                Next
                tempRowIndex = selectedCellsByRow.Key
            Next
            virtualRowsSorted.Sort()
            For i As Int32 = 1 To virtualRowsSorted.Count - 1
                If virtualRowsSorted(i) - virtualRowsSorted(i - 1) <> 1 Then Return False
            Next
        End If
        Return True
    End Function
#End Region

#Region "Cell Editing Logic"
    Private Sub PreviewEditText(sender As Object, e As KeyEventArgs)
        Dim editBox As TextBox = CType(sender, TextBox)
        Select Case e.Key
            Case Key.Enter
                GridPanel.Focus()
            Case Key.Tab
                GridPanel.Focus()
            Case Key.Escape
                editBox.Text = GetCellText(Grid.GetRow(editBox), Grid.GetColumn(editBox))
                GridPanel.Focus()
        End Select
    End Sub
    Private Sub EditTextLostFocus(sender As Object, e As RoutedEventArgs)
        Dim editBox As TextBox = CType(sender, TextBox)
        Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        Dim lastRowDataIndex As Int32 = firstRowDataIndex + _visibleRowCount - 1
        If _activeCellVirtualRowIndex < firstRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex
        If _activeCellVirtualRowIndex > lastRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1
        Dim columnIndex, rowIndex As Int32
        If editBox.Tag.GetType <> GetType(Tuple(Of String, Point)) Then
            columnIndex = Grid.GetColumn(editBox)
            rowIndex = _activeCellVirtualRowIndex - firstRowDataIndex
            rowIndex = GetDataRowIndex(rowIndex)
        Else
            columnIndex = CInt(CType(editBox.Tag, Tuple(Of String, Point)).Item2.X)
            rowIndex = CInt(CType(editBox.Tag, Tuple(Of String, Point)).Item2.Y)
        End If

        GridPanel.Children.Remove(editBox)
        Dim newText As String = editBox.Text
        If CType(editBox.Tag, Tuple(Of String, Point)).Item1 <> newText Then
            Try
                DataView.EditCell(rowIndex, columnIndex, newText)
                If _selectedRowsOnly = True Then
                    Dim selectedRowOffset As Int32 = -1
                    If _columnSortOrder = SortOrder.None Then
                        selectedRowOffset = _rowOffset(_selectedDataRowIndices.IndexOf(rowIndex))
                    Else
                        selectedRowOffset = Array.IndexOf(_sortedSelectedRowOffsets, _rowOffset(rowIndex))
                    End If
                    SetCellText(selectedRowOffset - firstRowDataIndex, columnIndex, newText)
                Else
                    SetCellText(_rowOffset(rowIndex) - firstRowDataIndex, columnIndex, newText)
                End If
                UpdateUndoRedoButtons()
                SetActiveCell()
                _mouseSelectionMode = SelectionMode.None
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End If
    End Sub
#End Region

#Region "Column Context Menu"
    Private Sub CreateColumnContextMenu(sender As Object, e As MouseButtonEventArgs)
        If DataView.NumberOfRows = 0 Then Exit Sub

        Dim gridPosition As Point = e.GetPosition(GridPanel)
        _mouseDownColumnIndex = GetTableColumnIndex(gridPosition)
        '
        If _selectedColumnIndices.BinarySearch(_mouseDownColumnIndex) < 0 Then
            'I want to select the column but I don't want to de-select the rows because I don't want to lose
            'the ability to apply to selected records only in the field calculator.
            '
            'If _selectedDataRowIndices.Count > 0 Then
            '    _selectedDataRowIndices.Clear()
            '    RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
            'End If
            _selectedCellIndices.Clear()
            _selectedColumnIndices.Clear()
            _selectedColumnIndices.Add(_mouseDownColumnIndex)
            DeSelectAllCells()
            SetSelectedCells()
        End If
        '
        Dim columnMenu As New ContextMenu
        Dim columnMenuItem As New MenuItem With {.Header = "Sort Ascending", .Icon = New Image() With {.Source = New BitmapImage(New Uri("pack://application:,,/GenericControls;component/Resources/SortASCFilter.png"))}}
        AddHandler columnMenuItem.Click, AddressOf SortColumnAscending
        If _columnsSortedOrder(_mouseDownColumnIndex) = SortOrder.Ascending Then columnMenuItem.IsEnabled = False
        columnMenu.Items.Add(columnMenuItem)
        '
        columnMenuItem = New MenuItem With {.Header = "Sort Descending", .Icon = New Image() With {.Source = New BitmapImage(New Uri("pack://application:,,/GenericControls;component/Resources/SortDSCFilter.png"))}}
        AddHandler columnMenuItem.Click, AddressOf SortColumnDescending
        If _columnsSortedOrder(_mouseDownColumnIndex) = SortOrder.Descending Then columnMenuItem.IsEnabled = False
        columnMenu.Items.Add(columnMenuItem)
        '
        columnMenuItem = New MenuItem With {.Header = "Remove Sort", .Icon = New Image() With {.Source = New BitmapImage(New Uri("pack://application:,,/GenericControls;component/Resources/ClearFilter.png"))}}
        AddHandler columnMenuItem.Click, AddressOf RemoveSort
        If _columnsSortedOrder(_mouseDownColumnIndex) = SortOrder.None Then columnMenuItem.IsEnabled = False
        columnMenu.Items.Add(columnMenuItem)
        '
        columnMenuItem = New MenuItem With {.Header = "Summary Statistics...", .Icon = New Image() With {.Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/SummaryStatistics_16x.png"))}}
        AddHandler columnMenuItem.Click, AddressOf CalcColumnStatistics
        Select Case DataView.ColumnTypes(_mouseDownColumnIndex)
            'Case GetType(Boolean)
                'columnMenuItem.IsEnabled = False
            Case GetType(Object)
                columnMenuItem.IsEnabled = False
        End Select
        columnMenu.Items.Add(columnMenuItem)
        '
        columnMenuItem = New MenuItem With {.Header = "Find...", .Icon = New Image() With {.Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/QuickFind_16x.png"))}}
        AddHandler columnMenuItem.Click, AddressOf SearchText
        If _selectedRowsOnly = True Then columnMenuItem.IsEnabled = False
        If DataView.NumberOfRows = 0 Then columnMenuItem.IsEnabled = False
        columnMenu.Items.Add(columnMenuItem)

        If Editable = True And _readOnlyColumns.Contains(_mouseDownColumnIndex) = False Then
            columnMenuItem = New MenuItem With {.Header = "Field Calculator...", .Icon = New Image() With {.Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/calculator_16x.png"))}}
            AddHandler columnMenuItem.Click, AddressOf OpenFcForSpecificColumn
            columnMenu.Items.Add(columnMenuItem)
        End If

        If Editable = True And _readOnlyColumns.Contains(_mouseDownColumnIndex) = False Then
            columnMenuItem = New MenuItem With {.Header = "Delete Columns", .Icon = New Image() With {.Source = New BitmapImage(New Uri("pack://application:,,/GenericControls;component/Resources/delete_column.png"))}}
            If _selectedColumnIndices.Count = 1 Then columnMenuItem.Header = "Delete Column"
            AddHandler columnMenuItem.Click, AddressOf DeleteColumn
            columnMenu.Items.Add(columnMenuItem)
        End If

        columnMenu.IsOpen = True

    End Sub

    Private Sub DeleteColumn(sender As Object, e As RoutedEventArgs)
        Dim columnIndices As List(Of Int32) = _selectedColumnIndices.ToList
        For i As Int32 = columnIndices.Count - 1 To 0 Step -1
            If _readOnlyColumns.Contains(columnIndices(i)) Then columnIndices.RemoveAt(i)
        Next
        '
        DataView.DeleteColumns(columnIndices.ToArray)
    End Sub
    Private Sub CalcColumnStatistics(sender As Object, e As RoutedEventArgs)
        Dim columnstats As New ColumnStatsWindow(Me, _mouseDownColumnIndex) With {.Owner = Window.GetWindow(Me)}
        columnstats.Show()
    End Sub
    Private Sub SearchText()
        If DataView.NumberOfRows > 0 Then
            Dim findWindow As New FindAndReplace(Me, _mouseDownColumnIndex, _activeCellVirtualRowIndex)
            findWindow.ShowDialog()
        End If
    End Sub
    Private Sub OpenFcForSpecificColumn(sender As Object, e As RoutedEventArgs)
        Dim f As New FieldCalculator(DataView, _selectedDataRowIndices, _readOnlyColumns, DataView.ColumnNames(_mouseDownColumnIndex))
        If f.ShowDialog = True Then
            UpdateVisibleRows()
            UpdateUndoRedoButtons()
        End If
    End Sub
#End Region

#Region "Sorting Logic"
    Private Sub RemoveSort()
        _columnSortOrder = SortOrder.None
        For i As Int32 = 0 To DataView.ColumnNames.Count - 1
            _columnsSortedOrder(i) = SortOrder.None
        Next
        For i = 0 To _rowId.Count - 1
            _rowId(i) = i
        Next
        _rowId.CopyTo(_rowOffset, 0)
        UpdateVisibleRows()
        If _selectedRowsOnly = False Then
            DeSelectAllCells()
            SetSelectedCells()
        End If

        CType(ColumnHeadersGrid.Children(_mouseDownColumnIndex * 2), ColumnHeader).RemoveSorter()
        UpdateRowHeaders()
    End Sub
    Private Sub SortColumnDescending()
        Try
            Mouse.OverrideCursor = Cursors.Wait
            SortColumn(_mouseDownColumnIndex, False)
            _columnSortOrder = SortOrder.Descending
            For i As Int32 = 0 To DataView.ColumnNames.Count - 1
                _columnsSortedOrder(i) = SortOrder.None
            Next
            _columnsSortedOrder(_mouseDownColumnIndex) = SortOrder.Descending
            If _selectedRowsOnly = False Then
                DeSelectAllCells()
                SetSelectedCells()
            Else
                ReDim _sortedSelectedRowOffsets(_selectedDataRowIndices.Count - 1)
                For i As Int32 = 0 To _selectedDataRowIndices.Count - 1
                    _sortedSelectedRowOffsets(i) = _rowOffset(_selectedDataRowIndices(i))
                Next
                Array.Sort(_sortedSelectedRowOffsets)
                _sortedSelectedRowOffsets.Reverse()
            End If
            UpdateVisibleRows()
            '
            For i As Int32 = 0 To DataView.ColumnNames.Count - 1
                CType(ColumnHeadersGrid.Children(i * 2), ColumnHeader).RemoveSorter()
            Next
            CType(ColumnHeadersGrid.Children(_mouseDownColumnIndex * 2), ColumnHeader).AddSorter(False)
            UpdateRowHeaders()
            Mouse.OverrideCursor = Nothing
        Catch ex As Exception
            Mouse.OverrideCursor = Nothing
        End Try
    End Sub
    Private Sub SortColumnAscending()
        Try
            Mouse.OverrideCursor = Cursors.Wait
            SortColumn(_mouseDownColumnIndex, True)
            _columnSortOrder = SortOrder.Ascending
            For i As Int32 = 0 To DataView.ColumnNames.Count - 1
                _columnsSortedOrder(i) = SortOrder.None
            Next
            _columnsSortedOrder(_mouseDownColumnIndex) = SortOrder.Ascending
            If _selectedRowsOnly = False Then
                DeSelectAllCells()
                SetSelectedCells()
            Else
                ReDim _sortedSelectedRowOffsets(_selectedDataRowIndices.Count - 1)
                For i As Int32 = 0 To _selectedDataRowIndices.Count - 1
                    _sortedSelectedRowOffsets(i) = _rowOffset(_selectedDataRowIndices(i))
                Next
                Array.Sort(_sortedSelectedRowOffsets)
            End If
            UpdateVisibleRows()
            '
            For i As Int32 = 0 To DataView.ColumnNames.Count - 1
                CType(ColumnHeadersGrid.Children(i * 2), ColumnHeader).RemoveSorter()
            Next
            CType(ColumnHeadersGrid.Children(_mouseDownColumnIndex * 2), ColumnHeader).AddSorter(True)
            UpdateRowHeaders()
            Mouse.OverrideCursor = Nothing
        Catch ex As Exception
            Mouse.OverrideCursor = Nothing
        End Try
    End Sub
    Private Sub SortColumn(columnIndex As Int32, ByVal ascending As Boolean)
        Dim columnData() As Object = DataView.GetColumn(columnIndex)
        '
        Dim idx As List(Of Integer)
        Select Case DataView.ColumnTypes(columnIndex)
            Case GetType(Byte)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Byte, Int32)(If(IsDBNull(x), Byte.MinValue, CByte(x)), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Int16)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Short, Int32)(If(IsDBNull(x), Short.MinValue, CShort(x)), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(UInt16)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of UShort, Int32)(If(IsDBNull(x), UShort.MinValue, CUShort(x)), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Int32)
                'Dim sorted As List(Of KeyValuePair(Of Int32, Int32))
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Int32, Int32)(If(IsDBNull(x), Integer.MinValue, CInt(x)), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(UInt32)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of UInt32, Int32)(If(IsDBNull(x), UInteger.MinValue, CUInt(x)), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Int64)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Long, Int32)(If(IsDBNull(x), Long.MinValue, CLng(x)), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(UInt64)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of ULong, Int32)(If(IsDBNull(x), ULong.MinValue, CULng(x)), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Single)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Single, Int32)(If(IsDBNull(x), Single.NaN, CSng(x)), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Double)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Double, Int32)(If(IsDBNull(x), Double.NaN, CDbl(x)), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(String)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of String, Int32)(x.ToString, i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Boolean)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Boolean, Int32)(If(IsDBNull(x), False, CBool(x)), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case Else
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of String, Int32)(x.ToString, i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
        End Select
        '
        If ascending = False Then idx.Reverse()
        For i = 0 To idx.Count - 1
            _rowOffset(idx(i)) = i
        Next
        idx.CopyTo(_rowId)
    End Sub
#End Region

#Region "Toolbar Buttons"

#Region "Selection Toolbar"
    Private Sub ShowAll_Checked(sender As Object, e As RoutedEventArgs)

        _selectedRowsOnly = False
        VerticalScrollbar.Maximum = DataView.NumberOfRows - CInt(Math.Floor(RowsAr.ActualHeight / RowHeight))
        If _selectedDataRowIndices.Count > 0 Then
            If _selectedDataRowIndices(0) > VerticalScrollbar.Maximum Then
                VerticalScrollbar.Value = VerticalScrollbar.Maximum - 1
            Else
                VerticalScrollbar.Value = _selectedDataRowIndices(0)
            End If
        End If
        _visibleRowCount = CInt(Math.Floor(RowsAr.ActualHeight / RowHeight))
        If _visibleRowCount > DataView.NumberOfRows Then _visibleRowCount = DataView.NumberOfRows
        LoadRows()
        SetSelectedCells()
        UpdateRowHeaders()
        CType(ShowAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
        ShowAll.IsEnabled = False
        GridPanel.Focus()
    End Sub
    Public Sub ShowSelected_Checked(sender As Object, e As RoutedEventArgs)
        If _selectedDataRowIndices.Count > 0 Then
            If _columnSortOrder <> SortOrder.None Then
                ReDim _sortedSelectedRowOffsets(_selectedDataRowIndices.Count - 1)
                For i As Int32 = 0 To _selectedDataRowIndices.Count - 1
                    _sortedSelectedRowOffsets(i) = _rowOffset(_selectedDataRowIndices(i))
                Next
                Array.Sort(_sortedSelectedRowOffsets)
                If _columnSortOrder = SortOrder.Descending Then _sortedSelectedRowOffsets.Reverse()
            End If
            '
            _selectedRowsOnly = True
            CType(ShowAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ShowAllIcon_22x22.png"))
            ShowAll.IsEnabled = True
            If _visibleRowCount > _selectedDataRowIndices.Count Then _visibleRowCount = _selectedDataRowIndices.Count
            VerticalScrollbar.Maximum = _selectedDataRowIndices.Count - _visibleRowCount
            VerticalScrollbar.Value = 0
            LoadRows()
            _activeCellDataColumnIndex = 0
            _activeCellVirtualRowIndex = 0
            SetSelectedCells()
            UpdateRowHeaders()
        End If
        GridPanel.Focus()
    End Sub
    Private Sub DeSelectAll_Click(sender As Object, e As RoutedEventArgs)
        If _selectedDataRowIndices.Count > 0 Then
            _selectedDataRowIndices.Clear()
            RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
        End If
        DeSelectAllCells()
        SetSelectedCells()
        ShowSelected.IsEnabled = False
        CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
        DeSelectAll.IsEnabled = False
        CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
        If _selectedRowsOnly = True Then
            _selectedRowsOnly = False
            _visibleRowCount = CInt(Math.Floor(RowsAr.ActualHeight / RowHeight))
            If _visibleRowCount > DataView.NumberOfRows Then _visibleRowCount = DataView.NumberOfRows
            VerticalScrollbar.Maximum = DataView.NumberOfRows - _visibleRowCount
            LoadRows()
            SetSelectedCells()
            CType(ShowAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            ShowAll.IsEnabled = False
        End If
        GridPanel.Focus()
    End Sub
    Private Sub SelectByAttribute_Click(sender As Object, e As RoutedEventArgs)
        Dim attributeSelector As New FieldCalculator(DataView, _selectedDataRowIndices, Nothing, Nothing, True)
        AddHandler attributeSelector.ContentRendered, AddressOf SelectorRendered

        If attributeSelector.ShowDialog = True Then
            AllCellsSelected = False
            _selectedDataRowIndices.Clear()
            _selectedCellIndices.Clear()
            _selectedColumnIndices.Clear()
            DeSelectAllCells()
            If _selectedRowsOnly = False Then
                _selectedDataRowIndices = attributeSelector.GetSelectedRows
            Else
                ShowAll_Checked(Nothing, Nothing)
                _selectedDataRowIndices = attributeSelector.GetSelectedRows
                ShowSelected_Checked(Nothing, Nothing)
            End If
            SetSelectedCells()
            If _selectedDataRowIndices.Count > 0 And _selectedRowsOnly = False Then
                ShowSelected.IsEnabled = True
                CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ShowSelectedIcon_22x22.png"))
                DeSelectAll.IsEnabled = True
                CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIcon_22x22.png"))
            Else
                ShowSelected.IsEnabled = False
                CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
                DeSelectAll.IsEnabled = False
                CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            End If
            RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
            _attributeSelectorString = attributeSelector.ExpressionCalculator.GetExpressionText()
            RemoveHandler attributeSelector.ContentRendered, AddressOf SelectorRendered
        End If
        GridPanel.Focus()
    End Sub

    Private Sub SelectorRendered(sender As Object, e As EventArgs)
        DirectCast(sender, FieldCalculator).ExpressionCalculator.SetExpressionText(_attributeSelectorString)
    End Sub
#End Region

#Region "Editing Toolbar"
    Private Sub Redo_Click(sender As Object, e As RoutedEventArgs) Handles Redo.Click
        RedoLastEdit()
    End Sub
    Private Sub RedoLastEdit()
        DataView.RedoEdit()
        UpdateVisibleRows()
        UpdateUndoRedoButtons()
    End Sub
    Private Sub Undo_Click(sender As Object, e As RoutedEventArgs) Handles Undo.Click
        UndoLastEdit()
    End Sub
    Private Sub UndoLastEdit()
        DataView.UndoEdit()
        UpdateVisibleRows()
        UpdateUndoRedoButtons()
    End Sub
    Private Sub UpdateUndoRedoButtons()
        If DataView.CanUndo() Then
            Undo.IsEnabled = True
            SaveButton.IsEnabled = True
            CType(Undo.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/Undo.png"))
            CType(SaveButton.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/Save.ico"))
        Else
            'Disable undo button
            Undo.IsEnabled = False
            SaveButton.IsEnabled = False
            CType(Undo.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/Undodisabled.png"))
            CType(SaveButton.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/Savedisabled.ico"))
        End If
        'Disable Redo if no more edits to redo
        If DataView.CanRedo = False Then
            Redo.IsEnabled = False
            CType(Redo.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/RedoDisabled.png"))
        Else
            'Enable Redo
            Redo.IsEnabled = True
            CType(Redo.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTableViewControl;component/Resources/Redo.png"))
        End If
    End Sub
    Private Sub Save_Click(sender As Object, e As RoutedEventArgs) Handles SaveButton.Click
        If MsgBox("Are you sure you want to save edits?", MsgBoxStyle.OkCancel, "Apply Edits") = MsgBoxResult.Ok Then
            Mouse.OverrideCursor = Cursors.Wait
            Try
                DataView.ApplyEdits()
                UpdateUndoRedoButtons()
                'RaiseEvent EditsSaved()
                Mouse.OverrideCursor = Nothing
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error Saving Edits")
                Mouse.OverrideCursor = Nothing
            End Try
        End If
    End Sub
    Private Sub OpenFC_Click(sender As Object, e As RoutedEventArgs) Handles OpenFC.Click
        Dim f As New FieldCalculator(DataView, _selectedDataRowIndices, _readOnlyColumns)
        AddHandler f.ContentRendered, AddressOf CalculatorRendered

        If f.ShowDialog = True Then
            _fieldCalculatorString = f.ExpressionCalculator.GetExpressionText
            UpdateVisibleRows()
            UpdateUndoRedoButtons()
            RemoveHandler f.ContentRendered, AddressOf CalculatorRendered
        End If
    End Sub

    Private Sub CalculatorRendered(sender As Object, e As EventArgs)
        DirectCast(sender, FieldCalculator).ExpressionCalculator.SetExpressionText(_fieldCalculatorString)
    End Sub
#End Region

#End Region

    Private Sub TableViewer_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
        If Not IsNothing(DataView) Then
            RemoveHandler DataView.RowsAdded, AddressOf TableViewRowsAdded
            RemoveHandler DataView.RowsDeleted, AddressOf TableViewRowsDeleted
            RemoveHandler DataView.ColumnsAdded, AddressOf TableViewColumnsAdded
            RemoveHandler DataView.ColumnsDeleted, AddressOf TableViewColumnsDeleted
        End If
    End Sub

    Private Sub TableViewer_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim wasFalse As Boolean = _isLoaded
        _isLoaded = True
        If wasFalse = False Then RefreshView()

    End Sub



    Private Class ColumnHeader
        Inherits Border

        Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(ColumnHeader), New PropertyMetadata(""))
        Public Property Text As String
            Get
                Return CType(GetValue(TextProperty), String)
            End Get
            Set(value As String)
                SetValue(TextProperty, value)
            End Set
        End Property
        Public Shared ReadOnly HeaderTextblockStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(HeaderTextblockStyle), GetType(Style), GetType(ColumnHeader), New PropertyMetadata(GetDefaultColumnHeaderTextblockStyle()))
        Public Property HeaderTextblockStyle As Style
            Get
                Return CType(GetValue(HeaderTextblockStyleProperty), Style)
            End Get
            Set(value As Style)
                SetValue(HeaderTextblockStyleProperty, value)
            End Set
        End Property
        Public Shared ReadOnly HeaderBorderStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(HeaderBorderStyle), GetType(Style), GetType(ColumnHeader), New PropertyMetadata(GetDefaultColumnHeaderBorderStyle()))
        Public Property HeaderBorderStyle As Style
            Get
                Return CType(GetValue(HeaderBorderStyleProperty), Style)
            End Get
            Set(value As Style)
                SetValue(HeaderBorderStyleProperty, value)
            End Set
        End Property

        Private _sortDownViewBox As New Viewbox() With {.Width = 9, .Visibility = Visibility.Collapsed, .Margin = New Thickness(2, 0, 2, 0)}
        Private _sortUpViewBox As New Viewbox() With {.Width = 9, .Visibility = Visibility.Collapsed, .Margin = New Thickness(2, 0, 2, 0)}
        'Private _downArrowCanvas As Canvas
        'Private _upArrowCanvas As Canvas
        Public Sub New(columnName As String, columnType As Type)
            'Set up the Column Header area
            Dim g As New Grid()
            g.ColumnDefinitions.Add(New ColumnDefinition() With {.Width = GridLength.Auto})
            g.ColumnDefinitions.Add(New ColumnDefinition() With {.Width = New GridLength(1, GridUnitType.Star)})
            'Define sort glyphs
            Dim myResourceDictionary As New ResourceDictionary() With {.Source = New Uri("/DataTableViewControl;component/Resources/TableViewerResources.xaml", UriKind.RelativeOrAbsolute)}
            Dim downArrowCanvas = CType(myResourceDictionary("SortArrow"), Canvas)
            _sortDownViewBox.Child = downArrowCanvas
            g.Children.Add(_sortDownViewBox)
            '
            Dim upArrowCanvas = CType(myResourceDictionary("SortArrow"), Canvas)
            upArrowCanvas.LayoutTransform = New RotateTransform(180)
            _sortUpViewBox.Child = upArrowCanvas
            g.Children.Add(_sortUpViewBox)
            '
            'Define the header textblock and textblock style
            Dim tBlock As New TextBlock With {.HorizontalAlignment = HorizontalAlignment.Center, .VerticalAlignment = VerticalAlignment.Center, .Background = Brushes.Transparent, .FontWeight = FontWeights.Bold, .TextTrimming = TextTrimming.CharacterEllipsis}
            Dim b As New Binding() With {.Path = New PropertyPath(NameOf(Text)), .Source = Me}
            tBlock.SetBinding(TextBlock.TextProperty, b)
            '
            Dim styleBinding As New Binding() With {.Path = New PropertyPath(NameOf(HeaderTextblockStyle)), .Source = Me}
            tBlock.SetBinding(TextBlock.StyleProperty, styleBinding)
            Grid.SetColumn(tBlock, 1)
            '
            'Add elements to the grid
            'g.Children.Add(_downArrowCanvas)
            'g.Children.Add(_upArrowCanvas)
            g.Children.Add(tBlock)
            'Define the column header style elements
            Dim borderStyleBinding As New Binding() With {.Path = New PropertyPath(NameOf(HeaderBorderStyle)), .Source = Me}
            SetBinding(Border.StyleProperty, borderStyleBinding)
            Child = g
            '
            Text = columnName
            Select Case columnType
                Case GetType(Byte)
                    ToolTip = columnName & vbCrLf & "Type: Byte"
                Case GetType(Int16), GetType(UInt16)
                    ToolTip = columnName & vbCrLf & "Type: Short Integer"
                Case GetType(Int32), GetType(UInt32)
                    ToolTip = columnName & vbCrLf & "Type: Integer"
                Case GetType(Int64), GetType(UInt64)
                    ToolTip = columnName & vbCrLf & "Type: Long Integer"
                Case GetType(Single)
                    ToolTip = columnName & vbCrLf & "Type: Single"
                Case GetType(Double)
                    ToolTip = columnName & vbCrLf & "Type: Double"
                Case GetType(String)
                    ToolTip = columnName & vbCrLf & "Type: String"
                Case GetType(Boolean)
                    ToolTip = columnName & vbCrLf & "Type: Boolean"
                Case Else
                    ToolTip = columnName & vbCrLf & "Type: Unknown"
            End Select

        End Sub
        Public Sub RemoveSorter()
            _sortDownViewBox.Visibility = Visibility.Collapsed
            _sortUpViewBox.Visibility = Visibility.Collapsed
        End Sub
        Public Sub AddSorter(ascending As Boolean)
            If ascending = False Then
                _sortDownViewBox.Visibility = Visibility.Visible
                _sortUpViewBox.Visibility = Visibility.Collapsed
            Else
                _sortDownViewBox.Visibility = Visibility.Collapsed
                _sortUpViewBox.Visibility = Visibility.Visible
            End If
        End Sub
    End Class
    Private Class Cell
        Inherits Border

        Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(Cell), New PropertyMetadata(""))
        Public Property Text As String
            Get
                Return CType(GetValue(TextProperty), String)
            End Get
            Set(value As String)
                SetValue(TextProperty, value)
            End Set
        End Property
        Public Shared ReadOnly CellStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(CellStyle), GetType(Style), GetType(Cell), New PropertyMetadata(GetDefaultCellTextblockStyle()))
        Public Property CellStyle As Style
            Get
                Return CType(GetValue(CellStyleProperty), Style)
            End Get
            Set(value As Style)
                SetValue(CellStyleProperty, value)
            End Set
        End Property

        Public Shared ReadOnly ForegroundProperty As DependencyProperty = DependencyProperty.Register(NameOf(Foreground), GetType(Brush), GetType(Cell), New PropertyMetadata(New SolidColorBrush(Colors.Black)))
        Public Property Foreground As Brush
            Get
                Return CType(GetValue(ForegroundProperty), Brush)
            End Get
            Set(value As Brush)
                SetValue(ForegroundProperty, value)
            End Set
        End Property

        Public Sub New()
            HorizontalAlignment = HorizontalAlignment.Stretch
            VerticalAlignment = VerticalAlignment.Stretch
            'Margin = New Thickness(1, 1, 0, 0)
            Background = Brushes.Transparent
            IsHitTestVisible = False
            '
            Dim tBlock = New TextBlock
            'Style
            Dim styleBinding As New Binding() With {.Path = New PropertyPath(NameOf(CellStyle)), .Source = Me}
            tBlock.SetBinding(TextBlock.StyleProperty, styleBinding)
            'Text
            Dim textBinding As New Binding() With {.Path = New PropertyPath(NameOf(Text)), .Source = Me}
            tBlock.SetBinding(TextBlock.TextProperty, textBinding)
            'Foreground
            Dim foregroundBinding As New Binding() With {.Path = New PropertyPath(NameOf(Foreground)), .Source = Me}
            tBlock.SetBinding(TextBlock.ForegroundProperty, foregroundBinding)
            Child = tBlock
        End Sub
    End Class
    Private Class RowHeader
        Inherits Border

        Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(RowHeader), New PropertyMetadata(""))
        Public Property Text As String
            Get
                Return CType(GetValue(TextProperty), String)
            End Get
            Set(value As String)
                SetValue(TextProperty, value)
            End Set
        End Property
        Public Shared ReadOnly HeaderTextblockStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(HeaderTextblockStyle), GetType(Style), GetType(RowHeader), New PropertyMetadata(GetDefaultRowHeaderTextblockStyle()))
        Public Property HeaderTextblockStyle As Style
            Get
                Return CType(GetValue(HeaderTextblockStyleProperty), Style)
            End Get
            Set(value As Style)
                SetValue(HeaderTextblockStyleProperty, value)
            End Set
        End Property
        Public Shared ReadOnly HeaderBorderStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(HeaderBorderStyle), GetType(Style), GetType(RowHeader), New PropertyMetadata(GetDefaultRowHeaderBorderStyle()))
        Public Property HeaderBorderStyle As Style
            Get
                Return CType(GetValue(HeaderBorderStyleProperty), Style)
            End Get
            Set(value As Style)
                SetValue(HeaderBorderStyleProperty, value)
            End Set
        End Property
        Public Sub New()
            '
            'Define the header textblock and textblock style
            Dim tBlock As New TextBlock With {.HorizontalAlignment = HorizontalAlignment.Left, .VerticalAlignment = VerticalAlignment.Center, .Margin = New Thickness(2, 0, 0, 0), .Background = Brushes.Transparent}
            Dim b As New Binding() With {.Path = New PropertyPath(NameOf(Text)), .Source = Me}
            tBlock.SetBinding(TextBlock.TextProperty, b)
            '
            Dim styleBinding As New Binding() With {.Path = New PropertyPath(NameOf(HeaderTextblockStyle)), .Source = Me}
            tBlock.SetBinding(TextBlock.StyleProperty, styleBinding)

            'Define the column header style elements
            Dim borderStyleBinding As New Binding() With {.Path = New PropertyPath(NameOf(HeaderBorderStyle)), .Source = Me}
            SetBinding(Border.StyleProperty, borderStyleBinding)
            Child = tBlock

        End Sub
    End Class

    Private Shared Function GetDefaultCellTextblockStyle() As Style
        Dim s As New Style(GetType(TextBlock))
        s.Setters.Add(New Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Stretch))
        s.Setters.Add(New Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center))
        s.Setters.Add(New Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left))
        s.Setters.Add(New Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis))
        s.Setters.Add(New Setter(TextBlock.BackgroundProperty, Brushes.Transparent))
        s.Setters.Add(New Setter(TextBlock.ForegroundProperty, Brushes.Black))
        s.Setters.Add(New Setter(TextBlock.IsHitTestVisibleProperty, False))
        s.Setters.Add(New Setter(TextBlock.MarginProperty, New Thickness(2, 0, 2, 0)))
        Return s
    End Function
    Private Shared Function GetDefaultColumnHeaderTextblockStyle() As Style
        Dim s As New Style(GetType(TextBlock))
        s.Setters.Add(New Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center))
        s.Setters.Add(New Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center))
        s.Setters.Add(New Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center))
        s.Setters.Add(New Setter(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis))
        s.Setters.Add(New Setter(TextBlock.BackgroundProperty, Brushes.Transparent))
        s.Setters.Add(New Setter(TextBlock.ForegroundProperty, Brushes.Black))
        s.Setters.Add(New Setter(TextBlock.FontWeightProperty, FontWeights.Bold))
        s.Setters.Add(New Setter(TextBlock.TextWrappingProperty, TextWrapping.WrapWithOverflow))
        s.Setters.Add(New Setter(TextBlock.IsHitTestVisibleProperty, False))
        Return s
    End Function
    Private Shared Function GetDefaultColumnHeaderBorderStyle() As Style
        Dim columnHeaderBackground As New LinearGradientBrush(New GradientStopCollection({New GradientStop(SystemColors.ControlLightLightColor, 0.25), New GradientStop(Color.FromArgb(255, 237, 238, 243), 1)}), New Point(0, 0), New Point(0, 1))
        Dim s As New Style(GetType(Border))
        s.Setters.Add(New Setter(Border.HorizontalAlignmentProperty, HorizontalAlignment.Stretch))
        s.Setters.Add(New Setter(Border.VerticalAlignmentProperty, VerticalAlignment.Stretch))
        s.Setters.Add(New Setter(Border.BackgroundProperty, columnHeaderBackground))
        s.Setters.Add(New Setter(Border.CornerRadiusProperty, New CornerRadius(0.5)))
        s.Setters.Add(New Setter(Border.BorderBrushProperty, New SolidColorBrush(Color.FromRgb(204, 206, 219))))
        s.Setters.Add(New Setter(Border.BorderThicknessProperty, New Thickness(1)))
        s.Setters.Add(New Setter(Border.MarginProperty, New Thickness(-0.5)))
        s.Setters.Add(New Setter(Border.MinHeightProperty, CDbl(23)))
        s.Setters.Add(New Setter(Border.SnapsToDevicePixelsProperty, True))
        Dim mouseOverTrigger As New Trigger() With {.Property = IsMouseOverProperty, .Value = True}
        mouseOverTrigger.Setters.Add(New Setter(Border.BackgroundProperty, New SolidColorBrush(Color.FromRgb(201, 222, 245))))
        s.Triggers.Add(mouseOverTrigger)
        Dim keyFocusWithinTrigger As New Trigger() With {.Property = IsKeyboardFocusWithinProperty, .Value = True}
        keyFocusWithinTrigger.Setters.Add(New Setter(Border.BackgroundProperty, New SolidColorBrush(Color.FromRgb(201, 222, 245))))
        s.Triggers.Add(keyFocusWithinTrigger)
        Return s
    End Function

    Private Shared Function GetDefaultRowHeaderTextblockStyle() As Style
        Dim s As New Style(GetType(TextBlock))
        s.Setters.Add(New Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Left))
        s.Setters.Add(New Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center))
        s.Setters.Add(New Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center))
        s.Setters.Add(New Setter(TextBlock.BackgroundProperty, Brushes.Transparent))
        s.Setters.Add(New Setter(TextBlock.ForegroundProperty, Brushes.Black))
        s.Setters.Add(New Setter(TextBlock.MarginProperty, New Thickness(2, 0, 0, 0)))
        s.Setters.Add(New Setter(TextBlock.IsHitTestVisibleProperty, False))
        Return s
    End Function
    Private Shared Function GetDefaultRowHeaderBorderStyle() As Style
        Dim rowHeaderBackground As New LinearGradientBrush(New GradientStopCollection({New GradientStop(SystemColors.ControlLightLightColor, 0.25), New GradientStop(Color.FromArgb(255, 237, 238, 243), 1)}), New Point(0, 0), New Point(1, 0))

        Dim s As New Style(GetType(Border))
        s.Setters.Add(New Setter(Border.HorizontalAlignmentProperty, HorizontalAlignment.Stretch))
        s.Setters.Add(New Setter(Border.VerticalAlignmentProperty, VerticalAlignment.Stretch))
        s.Setters.Add(New Setter(Border.BackgroundProperty, rowHeaderBackground))
        s.Setters.Add(New Setter(Border.CornerRadiusProperty, New CornerRadius(0)))
        s.Setters.Add(New Setter(Border.BorderBrushProperty, New SolidColorBrush(Color.FromRgb(204, 206, 219))))
        s.Setters.Add(New Setter(Border.BorderThicknessProperty, New Thickness(1)))
        s.Setters.Add(New Setter(Border.MarginProperty, New Thickness(0, -0.5, 0, -0.5)))
        s.Setters.Add(New Setter(Border.SnapsToDevicePixelsProperty, True))
        Return s
    End Function
End Class