Imports DataBase_Reader
Public Class WpfCustomViewer

#Region "Variables"
    'Basic Information
    Private _rowHeight As Int32 = 23
    Private _visibleRowCount As Int32
    Private _dataView As DataTableView
    'Selection Information
    Private _activeCellDataColumnIndex As Int32 = 0
    Private _activeCellVirtualRowIndex As Int32 = 0
    Private _rowSelectable As Boolean
    Private _columnSelectable As Boolean
    Private _cellSelectable As Boolean
    Private _mouseDownVirtualRowIndex As Int32
    Private _mouseDownColumnIndex As Int32
    Private _mouseSelectionMode As SelectionMode = SelectionMode.None
    Private _selectedDataRowIndices As List(Of Int32)
    Private _selectedColumnIndices As List(Of Int32)
    Private _selectedCellIndices As SortedDictionary(Of Int32, SortedSet(Of Int32)) 'key=row index in database, value=set of column indices for selected cells in the row key
    Private _allSelected As Boolean = False
    Private _selectedRowsOnly As Boolean = False
    Private _attributeSelectorString As String = ""
    'Sorting Information
    Private _rowOffset() As Int32 'for a given database row index, where it is in the table. if no sorting then all equal zero. 
    Private _rowId() As Int32 'for a given row index in the table (index in array), the row index in the database. If no sorting then 0=0,1=1,etc.
    Private _sortedSelectedRowOffsets() As Int32 'Used when selected rows only is being used.
    Private _columnSortOrder As SortOrder 'current sort order of the table (only one column can be sorted at a time).
    Private _columnsSortedOrder() As SortOrder 'sort order For Each column, only one column can be sorted at a time.
    'Editing Information
    Private _editable As Boolean
    Private _loadFieldCalculator As Boolean
    Private ReadOnly _cellEditTextBox As TextBox
    Private _fieldCalculatorString As String = ""

    Private _readOnlyColumns As HashSet(Of Int32)
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
    Public ReadOnly Property DataView As DataTableView
        Get
            Return _dataView
        End Get
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
    Public Property Editable As Boolean
        Get
            Return _editable 
        End Get
        Set()
            _editable = value 
        End Set
    End Property
    Public Property LoadFieldCalculator As Boolean
        Get
            Return _loadFieldCalculator
        End Get
        Set()
            _loadFieldCalculator = value
        End Set
    End Property
    'Public Shared ReadOnly EditableProperty As DependencyProperty = DependencyProperty.Register("Editable", GetType(Boolean), GetType(WPF_Custom_Viewer), New UIPropertyMetadata(False))
    Public Property RowSelectable As Boolean
        Get
            Return _rowSelectable 'CType(Me.GetValue(RowSelectableProperty), Boolean)
        End Get
        Set(value As Boolean)
            _rowSelectable = value 'Me.SetValue(RowSelectableProperty, value)
        End Set
    End Property
    'Public Shared ReadOnly RowSelectableProperty As DependencyProperty = DependencyProperty.Register("RowSelectable", GetType(Boolean), GetType(WPF_Custom_Viewer), New UIPropertyMetadata(False))

    Public Property ColumnSelectable As Boolean
        Get
            Return _columnSelectable
        End Get
        Set(value As Boolean)
            _columnSelectable = value
        End Set
    End Property
    Public Property CellSelectable As Boolean
        Get
            Return _cellSelectable
        End Get
        Set(value As Boolean)
            _cellSelectable = value
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
        _cellEditTextBox = New TextBox
        With _cellEditTextBox
            .Padding = New Thickness(0, 2, 0, 2)
            .Margin = New Thickness(1, 1, 0, 0)
        End With
        AddHandler _cellEditTextBox.PreviewKeyDown, AddressOf PreviewEditText
        AddHandler _cellEditTextBox.LostFocus, AddressOf EditTextLostFocus
        SelectionToolbar.IsEnabled = False
        EditorToolbar.IsEnabled = False
    End Sub
    Public Sub New(dataView As DataTableView)

        InitializeComponent()
        '
        _cellEditTextBox = New TextBox
        With _cellEditTextBox
            .Padding = New Thickness(0, 2, 0, 2)
            .Margin = New Thickness(1, 1, 0, 0)
        End With
        AddHandler _cellEditTextBox.PreviewKeyDown, AddressOf PreviewEditText
        AddHandler _cellEditTextBox.LostFocus, AddressOf EditTextLostFocus
        '
        LoadReader(dataView)
    End Sub
#End Region

#Region "Loading and Unloading Process"
    Private Sub Table_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        '
        If Editable = False AndAlso EditorToolbar.Visibility = Visibility.Visible Then
            EditorToolbar.Visibility = Visibility.Collapsed
        ElseIf Editable = True AndAlso EditorToolbar.Visibility = Visibility.Collapsed Then
            EditorToolbar.Visibility = Visibility.Visible
        End If
        '
        If _loadFieldCalculator = False Then OpenFC.Visibility = Visibility.Collapsed ' EditorToolbar.Items.RemoveAt(0) '
        '
        If RowSelectable = False AndAlso SelectionToolbar.Visibility = Visibility.Visible Then
            SelectionToolbar.Visibility = Visibility.Collapsed
        ElseIf RowSelectable = True AndAlso SelectionToolbar.Visibility = Visibility.Collapsed Then
            SelectionToolbar.Visibility = Visibility.Visible
        End If
        If RowSelectable = False AndAlso Editable = False Then TableViewerGrid.RowDefinitions(0).Height = New GridLength(0)
        '
        If IsNothing(_dataView) Then Exit Sub
        SetActiveCell(0, 0)
    End Sub
    Public Sub LoadReader(newView As DataTableView, Optional ByVal readOnlyColumns() As String = Nothing)
        If Not IsNothing(_dataView) Then
            RemoveHandler _dataView.RowsAdded, AddressOf TableViewRowsAdded
            RemoveHandler _dataView.RowsDeleted, AddressOf TableViewRowsDeleted
            RemoveHandler _dataView.ColumnsAdded, AddressOf TableViewColumnsAdded
            RemoveHandler _dataView.ColumnsDeleted, AddressOf TableViewColumnsDeleted
        End If
        _dataView = newView
        _readOnlyColumns = New HashSet(Of Integer)
        If Not IsNothing(readOnlyColumns) Then
            For i As Int32 = 0 To readOnlyColumns.Count - 1
                _readOnlyColumns.Add(Array.IndexOf(_dataView.ColumnNames, readOnlyColumns(i)))
            Next
        End If
        '
        If Not IsNothing(newView) Then
            AddHandler _dataView.RowsAdded, AddressOf TableViewRowsAdded
            AddHandler _dataView.RowsDeleted, AddressOf TableViewRowsDeleted
            AddHandler _dataView.ColumnsAdded, AddressOf TableViewColumnsAdded
            AddHandler _dataView.ColumnsDeleted, AddressOf TableViewColumnsDeleted
            If _dataView.ParentDatabase.DataBaseOpen = False Then _dataView.ParentDatabase.Open()
            '
            _columnSortOrder = SortOrder.None
            ReDim _columnsSortedOrder(_dataView.ColumnNames.Count - 1)
            For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
                _columnsSortedOrder(i) = SortOrder.None
            Next
            '
            'RowsColumnArea.Width = New GridLength(CInt(New FormattedText(_dataView.NumberOfRows.ToString, Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, New Typeface(New FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 12, Brushes.Black).Width) + 3)
            Refresh()
            SelectionToolbar.IsEnabled = True
            EditorToolbar.IsEnabled = True
        Else
            _selectedCellIndices = New SortedDictionary(Of Int32, SortedSet(Of Integer)) 'List(Of Integer))
            _selectedColumnIndices = New List(Of Int32)
            _selectedDataRowIndices = New List(Of Int32)
            _activeCellDataColumnIndex = 0
            _activeCellVirtualRowIndex = 0
            _selectedRowsOnly = False
            '
            RemoveHandler ColumnsGrid.SizeChanged, AddressOf UpdateColumnLines
            '
            GridPanel.Children.Clear()
            RowsGrid.Children.Clear()
            ColumnsGrid.Children.Clear()
            GridPanel.RowDefinitions.Clear()
            RowsGrid.RowDefinitions.Clear()
            ColumnsGrid.ColumnDefinitions.Clear()
            '
            GridPanel.ColumnDefinitions.Clear()
            GridLinesCanvas.Children.Clear()
            '
            SelectionToolbar.IsEnabled = False
            EditorToolbar.IsEnabled = False
        End If
    End Sub
    Public Sub SetColumnsAsReadOnly(columnIndices() As Int32)
        For i As Int32 = 0 To columnIndices.Count - 1
            _readOnlyColumns.Add(columnIndices(i))
        Next
    End Sub
    Public Sub SetColumnsAsReadOnly(columnNames() As String)
        For i As Int32 = 0 To columnNames.Count - 1
            _readOnlyColumns.Add(Array.IndexOf(_dataView.ColumnNames, columnNames(i)))
        Next
    End Sub
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
            If _activeCellDataColumnIndex > _dataView.ColumnNames.Count - 1 Then _activeCellDataColumnIndex = _dataView.ColumnNames.Count - 1
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
            If _activeCellVirtualRowIndex > _dataView.NumberOfRows - 1 Then _activeCellVirtualRowIndex = _dataView.NumberOfRows - 1
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
        _visibleRowCount = CInt(Math.Floor(RowsArea.ActualHeight / _rowHeight))
        RemoveHandler VerticalScrollbar.ValueChanged, AddressOf VerticalScrollBar_ValueChanged
        if _selectedRowsOnly=True Then
            If _visibleRowCount > _selectedDataRowIndices.Count Then _visibleRowCount = _selectedDataRowIndices.Count
            VerticalScrollbar.Maximum = _selectedDataRowIndices.Count - _visibleRowCount
        Else 
            If _visibleRowCount > _dataView.NumberOfRows Then _visibleRowCount = _dataView.NumberOfRows
            VerticalScrollbar.Maximum = _dataView.NumberOfRows - _visibleRowCount
        End If
        If CInt(Math.Floor(VerticalScrollbar.Value)) >= CInt(VerticalScrollbar.Maximum) Then VerticalScrollbar.Value -= 1
        AddHandler VerticalScrollbar.ValueChanged, AddressOf VerticalScrollBar_ValueChanged
        '
        'update rowid and row offsets
        ReDim _rowId(_dataView.NumberOfRows - 1)
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
        UpdateRowHeaders()
        'UpdateVisibleRows()
        SetSelectedCells()
        SetActiveCell(_activeCellVirtualRowIndex, _activeCellDataColumnIndex)
    End Sub
    Private Sub NumberOfColumnsChanged()
        RemoveHandler ColumnsGrid.SizeChanged, AddressOf UpdateColumnLines
        '
        GridPanel.Children.Clear()
        RowsGrid.Children.Clear()
        ColumnsGrid.Children.Clear()
        GridPanel.RowDefinitions.Clear()
        RowsGrid.RowDefinitions.Clear()
        ColumnsGrid.ColumnDefinitions.Clear()

        'Create Columns
        GridPanel.ColumnDefinitions.Clear()
        GridLinesCanvas.Children.Clear()
        CreateColumns()
        '
        For i As Int32 = 0 To _columnsSortedOrder.Count - 1
            If _columnsSortedOrder(i) = SortOrder.Ascending Then
                CType(ColumnsGrid.Children(i * 2), ColumnHeader).AddSorter(True)
            ElseIf _columnsSortedOrder(i) = SortOrder.Descending Then
                CType(ColumnsGrid.Children(i * 2), ColumnHeader).AddSorter(False)
            End If
        Next
        '
        'Create first row line
        Dim lengthBinding As New Binding("ActualWidth") With {.ElementName = "GridPanel"}
        Dim rowDistanceFromTop As Int32 = _rowHeight * (GridPanel.RowDefinitions.Count)
        Dim rowLine As New Line
        With rowLine
            .SnapsToDevicePixels = True
            .X1 = 0
            .Y1 = rowDistanceFromTop
            BindingOperations.SetBinding(rowLine, Line.X2Property, lengthBinding)
            .Y2 = rowDistanceFromTop
            .StrokeThickness = 1
            .Stroke = Brushes.CornflowerBlue
        End With
        GridLinesCanvas.Children.Add(rowLine)
        LoadRows()
        SetSelectedCells()
        'SetEditedCells()
        SetActiveCell()
        UpdateColumnLines(Nothing, Nothing)
        UpdateRowHeaders()
    End Sub
    Private Sub Refresh()
        '
        If _dataView.ParentDatabase.DataBaseOpen = False Then _dataView.ParentDatabase.Open()
        '
        _selectedCellIndices = New SortedDictionary(Of Int32, SortedSet(Of Integer)) 'List(Of Integer))
        _selectedColumnIndices = New List(Of Int32)
        _selectedDataRowIndices = New List(Of Int32)
        _activeCellDataColumnIndex = 0
        _activeCellVirtualRowIndex = 0
        _selectedRowsOnly = False

        ReDim _rowId(_dataView.NumberOfRows - 1)
        ReDim _rowOffset(_rowId.Count - 1)
        '
        For i = 0 To _rowId.Count - 1
            _rowId(i) = i
        Next
        _rowId.CopyTo(_rowOffset, 0)
        '
        RemoveHandler ColumnsGrid.SizeChanged, AddressOf UpdateColumnLines
        '
        GridPanel.Children.Clear()
        RowsGrid.Children.Clear()
        ColumnsGrid.Children.Clear()
        GridPanel.RowDefinitions.Clear()
        RowsGrid.RowDefinitions.Clear()
        ColumnsGrid.ColumnDefinitions.Clear()
        'Set the vertical scrollbar attributes
        _visibleRowCount = CInt(Math.Floor(RowsArea.ActualHeight / _rowHeight))
        If _visibleRowCount > _dataView.NumberOfRows Then _visibleRowCount = _dataView.NumberOfRows
        RemoveHandler VerticalScrollbar.ValueChanged, AddressOf VerticalScrollBar_ValueChanged
        VerticalScrollbar.Maximum = _dataView.NumberOfRows - _visibleRowCount
        If CInt(Math.Floor(VerticalScrollbar.Value)) >= CInt(VerticalScrollbar.Maximum) Then VerticalScrollbar.Value -= 1
        AddHandler VerticalScrollbar.ValueChanged, AddressOf VerticalScrollBar_ValueChanged
        '
        VerticalScrollbar.ViewportSize = _visibleRowCount
        '
        If _columnsSortedOrder.Count <> _dataView.ColumnNames.Count Then ReDim Preserve _columnsSortedOrder(_dataView.ColumnNames.Count - 1)
        '
        'Create Select All 
        Dim selectAll As New RowHeader
        AddHandler selectAll.MouseLeftButtonDown, AddressOf SelectAllLeftMouseDown
        AddHandler selectAll.MouseLeftButtonUp, AddressOf SelectAllLeftMouseUp
        Grid.SetRow(selectAll, 0)
        Grid.SetColumn(selectAll, 0)
        TableControlGrid.Children.Add(selectAll)
        '
        'Create Columns
        GridPanel.ColumnDefinitions.Clear()
        GridLinesCanvas.Children.Clear()
        CreateColumns()
        '
        For i As Int32 = 0 To _columnsSortedOrder.Count - 1
            If _columnsSortedOrder(i) = SortOrder.Ascending Then
                CType(ColumnsGrid.Children(i * 2), ColumnHeader).AddSorter(True)
            ElseIf _columnsSortedOrder(i) = SortOrder.Descending Then
                CType(ColumnsGrid.Children(i * 2), ColumnHeader).AddSorter(False)
            End If
        Next
        '
        'Create first row line
        Dim lengthBinding As New Binding("ActualWidth") With {.ElementName = "GridPanel"}
        Dim rowDistanceFromTop As Int32 = _rowHeight * (GridPanel.RowDefinitions.Count)
        Dim rowLine As New Line
        With rowLine
            .SnapsToDevicePixels = True
            .X1 = 0
            .Y1 = rowDistanceFromTop
            BindingOperations.SetBinding(rowLine, Line.X2Property, lengthBinding)
            .Y2 = rowDistanceFromTop
            .StrokeThickness = 1
            .Stroke = Brushes.CornflowerBlue
        End With
        GridLinesCanvas.Children.Add(rowLine)
        LoadRows()
        SetSelectedCells()
        'SetEditedCells()
        SetActiveCell()
        UpdateColumnLines(Nothing, Nothing)
        UpdateRowHeaders()
    End Sub
    Private Sub LoadRows()
        If GridLinesCanvas.Children.Count > _dataView.ColumnNames.Count + 2 Then
            GridLinesCanvas.Children.RemoveRange(_dataView.ColumnNames.Count + 2, GridLinesCanvas.Children.Count - 2 - _dataView.ColumnNames.Count)
        End If
        GridPanel.Children.Clear()
        GridPanel.RowDefinitions.Clear()
        RowsGrid.Children.Clear()
        RowsGrid.RowDefinitions.Clear()
        'Create Rows
        For i As Int32 = 0 To _visibleRowCount - 1
            AddRow()
        Next
    End Sub
    Private Sub CreateColumns()
        AddHandler ColumnsGrid.SizeChanged, AddressOf UpdateColumnLines
        ColumnHeadersArea.Height = New GridLength(_rowHeight)
        ColumnHead.Height = New GridLength(_rowHeight)
        Dim columnWidth As Int32
        Dim columnResizers As GridSplitter
        Dim columnDefinitions As ColumnDefinition
        Dim header As ColumnHeader
        Dim columnwidthbinding As Binding
        For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
            columnWidth = CInt(New FormattedText(_dataView.ColumnNames(i), Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, New Typeface(New FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 12, Brushes.Black).Width) + 12
            '
            columnResizers = New GridSplitter
            columnResizers.Background = Brushes.Transparent
            columnResizers.Width = 10
            columnResizers.Margin = New Thickness(0, 0, -5, 0)
            AddHandler columnResizers.MouseDoubleClick, AddressOf ResizeColumnSplitterDoubleClick
            Grid.SetZIndex(columnResizers, 1)
            columnResizers.Height = _rowHeight
            Grid.SetColumn(columnResizers, i)
            '
            columnDefinitions = New ColumnDefinition
            columnDefinitions.Width = New GridLength(columnWidth)
            ColumnsGrid.ColumnDefinitions.Add(columnDefinitions)
            '
            header = New ColumnHeader(_dataView.ColumnNames(i))
            Dim toolTipType As String
            Select Case _dataView.ColumnTypes(i)
                Case GetType(Byte)
                    toolTipType = "Type: Byte"
                Case GetType(Int16), GetType(UInt16)
                    toolTipType = "Type: Short Integer"
                Case GetType(Int32), GetType(UInt32)
                    toolTipType = "Type: Integer"
                Case GetType(Int64), GetType(UInt64)
                    toolTipType = "Type: Long Integer"
                Case GetType(Single)
                    toolTipType = "Type: Single"
                Case GetType(Double)
                    toolTipType = "Type: Double"
                Case GetType(String)
                    toolTipType = "Type: String"
                Case GetType(Boolean)
                    toolTipType = "Type: Boolean"
                Case Else
                    toolTipType = "Type: Unknown"
            End Select
            header.ToolTip = _dataView.ColumnNames(i) & vbCrLf & toolTipType
            If i = 0 Then header.Margin = New Thickness(0, 0, 1, 0)
            If i = _dataView.ColumnNames.Count - 1 Then header.Margin = New Thickness(1, 0, -1, 0)
            AddHandler header.MouseRightButtonUp, AddressOf CreateColumnContextMenu
            Grid.SetColumn(header, i)
            ColumnsGrid.Children.Add(header)
            ColumnsGrid.Children.Add(columnResizers)
            '
            columnwidthbinding = New Binding("Width.Value") With {.Source = columnDefinitions}
            columnDefinitions = New ColumnDefinition
            columnDefinitions.SetBinding(ColumnDefinition.WidthProperty, columnwidthbinding)
            GridPanel.ColumnDefinitions.Add(columnDefinitions)
        Next
        'Empty Grid Column Header for gridsplitter to work on last column
        columnDefinitions = New ColumnDefinition
        columnDefinitions.Width = New GridLength(0)
        ColumnsGrid.ColumnDefinitions.Add(columnDefinitions)
        'Create Columns Lines
        Dim lengthBinding As New Binding("ActualHeight") With {.ElementName = "GridPanel"}
        Dim gridLine As Line
        Dim aggregator As Int32 = 0
        '
        If _dataView.ColumnNames.Count = 0 Then Exit Sub
        For i As Int32 = 0 To _dataView.ColumnNames.Count
            gridLine = New Line
            With gridLine
                .SnapsToDevicePixels = True
                .X1 = aggregator
                .Y1 = 0
                .X2 = aggregator
                BindingOperations.SetBinding(gridLine, Line.Y2Property, lengthBinding)
                .StrokeThickness = 1
                .Stroke = Brushes.CornflowerBlue
            End With
            If i = 0 Then
                aggregator += GridPanel.ColumnDefinitions(i).Width.Value
            Else
                aggregator += GridPanel.ColumnDefinitions(i - 1).Width.Value
            End If
            GridLinesCanvas.Children.Add(gridLine)
        Next

    End Sub
    Public Sub HideSave()
        SaveButton.IsEnabled = False
        SaveButton.Visibility = Visibility.Collapsed
    End Sub
    Public Sub HideUndoRedo()
        Redo.IsEnabled=False
        Redo.Visibility = Visibility.Collapsed
        Undo.IsEnabled = False
        Undo.Visibility = Visibility.Collapsed
    End Sub
    Public Sub DisableEditing()
        If _cellEditTextBox.IsFocused = True Then
            _cellEditTextBox.Text = GetCellText(Grid.GetRow(_cellEditTextBox), Grid.GetColumn(_cellEditTextBox))
            GridPanel.Focus()
        End If
        EditorToolbar.Visibility = Visibility.Collapsed
        EditorToolbar.IsEnabled = False
        Editable = False
        CType(OpenFC.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/calculatorDisabled.png"))
        CType(Redo.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/RedoDisabled.png"))
        CType(Undo.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/Undodisabled.png"))
        CType(SaveButton.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/Savedisabled.ico"))
    End Sub
    Public Sub SetEditingCell()
        If _cellEditTextBox.IsFocused = False Then Exit Sub
        '_cellEditTextBox.Text = GetCellText(Grid.GetRow(_cellEditTextBox), Grid.GetColumn(_cellEditTextBox)) 'This would revert the cell back to the original value.
        GridPanel.Focus()
    End Sub
    Public Sub EnableEditing()
        EditorToolbar.Visibility = Visibility.Visible
        EditorToolbar.IsEnabled = True
        Editable = True
        CType(OpenFC.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/calculator.png"))
    End Sub
#End Region

#Region "Resize Logic"
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
    Public Sub ResizeColumnWidth(columnIndex As Int32, Optional columnWidth As Int32 = -1)
        If columnWidth > 5 Then
            ColumnsGrid.ColumnDefinitions(columnIndex).Width = New GridLength(columnWidth)
        Else
            Dim data As Object() = _dataView.GetColumn(_dataView.ColumnNames(columnIndex))
            '
            Dim max As Int32 = CInt(New FormattedText(_dataView.ColumnNames(columnIndex), Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, New Typeface(New FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 12, Brushes.Black).Width) + 12
            Dim formattedWidth As Int32
            Dim stringData As String
            Dim stringLength As Int32 = 0
            For i As Int32 = 0 To data.Count - 1
                stringData = data(i).ToString
                If stringLength < stringData.Length Then
                    stringLength = stringData.Length
                    formattedWidth = New FormattedText(stringData, Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, New Typeface(New FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 12, Brushes.Black).Width + 6
                    If formattedWidth > max Then max = formattedWidth
                End If
            Next
            If max < 6 Then max = 6
            ColumnsGrid.ColumnDefinitions(columnIndex).Width = New GridLength(max)
        End If

    End Sub
    Private Sub UpdateColumnLines(sender As Object, e As SizeChangedEventArgs)
        Dim runningWidth As Int32 = 0
        For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
            runningWidth += GridPanel.ColumnDefinitions(i).Width.Value
            CType(GridLinesCanvas.Children(i + 1), Line).X1 = runningWidth
            CType(GridLinesCanvas.Children(i + 1), Line).X2 = runningWidth
        Next
        SetActiveCell()
    End Sub
    Private Sub Table_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged
        If IsLoaded = False Then Exit Sub
        If IsNothing(_dataView) Then Exit Sub
        '
        Dim newNumberRows As Int32 = CInt(Math.Floor(RowsArea.ActualHeight / _rowHeight))
        dim nDataRows as int32 = _dataView.NumberOfRows
        if _selectedRowsOnly = True then nDataRows = _selectedDataRowIndices.Count
        If newNumberRows > nDataRows Then newNumberRows = nDataRows
        If newNumberRows <> _visibleRowCount AndAlso GridPanel.Children.Contains(_cellEditTextBox) Then GridPanel.Focus()
        Dim offset As Int32 = newNumberRows - _visibleRowCount
        If offset > (VerticalScrollbar.Maximum - Math.Floor(VerticalScrollbar.Value)) Then VerticalScrollbar.Value = VerticalScrollbar.Maximum
        If newNumberRows < _visibleRowCount Then
            Do
                'Remove Row
                For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
                    GridPanel.Children.RemoveAt(GridPanel.Children.Count - 1)
                Next
                GridPanel.RowDefinitions.RemoveAt(GridPanel.RowDefinitions.Count - 1)
                RowsGrid.Children.RemoveAt(RowsGrid.Children.Count - 1)
                RowsGrid.RowDefinitions.RemoveAt(RowsGrid.RowDefinitions.Count - 1)
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
                if _selectedRowsOnly = True Then
                    For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
                        SelectCell(i, _visibleRowCount)
                    Next
                End If
                _visibleRowCount += 1
            Loop Until _visibleRowCount = newNumberRows
            VerticalScrollbar.Maximum = nDataRows - newNumberRows
            UpdateRowHeaders()
        End If
        '
        if _selectedRowsOnly = False then SetSelectedCells()
        VerticalScrollbar.ViewportSize = _visibleRowCount
    End Sub
#End Region

#Region "Adding Rows and Defining Cell Info"
    Private Sub AddRow()
        'Create new row definition
        GridPanel.RowDefinitions.Add(New RowDefinition With {.Height = New GridLength(_rowHeight)})
        'Create new row Cells
        Dim newCell As Cell
        For j As Int32 = 0 To _dataView.ColumnTypes.Count - 1
            newCell = New Cell()
            Grid.SetRow(newCell, GridPanel.RowDefinitions.Count - 1)
            Grid.SetColumn(newCell, j)
            GridPanel.Children.Add(newCell)
        Next
        'Fill new row with data
        Dim scrollBarValue As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        If CInt(VerticalScrollbar.Maximum) = 0 OrElse CInt(VerticalScrollbar.Maximum) <> scrollBarValue Then
            If _selectedRowsOnly = False Then
                FillRow(GridPanel.RowDefinitions.Count - 1, _dataView.GetRow(_rowId(scrollBarValue + GridPanel.RowDefinitions.Count - 1)))
            Else
                If _columnSortOrder = SortOrder.None Then
                    FillRow(GridPanel.RowDefinitions.Count - 1, _dataView.GetRow(_rowId(_selectedDataRowIndices(scrollBarValue + GridPanel.RowDefinitions.Count - 1))))
                Else
                    FillRow(GridPanel.RowDefinitions.Count - 1, _dataView.GetRow(_rowId(_sortedSelectedRowOffsets(scrollBarValue + GridPanel.RowDefinitions.Count - 1))))
                End If
            End If
        End If
        'Create row line
        Dim lengthBinding As New Binding("ActualWidth") With {.ElementName = "GridPanel"}
        Dim rowDistanceFromTop As Int32 = _rowHeight * (GridPanel.RowDefinitions.Count)
        Dim rowLine As New Line With {.SnapsToDevicePixels = True, .X1 = 0, .Y1 = rowDistanceFromTop, .Y2 = rowDistanceFromTop, .StrokeThickness = 1, .Stroke = Brushes.CornflowerBlue}
        BindingOperations.SetBinding(rowLine, Line.X2Property, lengthBinding)
        GridLinesCanvas.Children.Add(rowLine)
        'Create Row selector
        RowsGrid.RowDefinitions.Add(New RowDefinition With {.Height = New GridLength(_rowHeight)})
        Dim newRowSelector As New RowHeader
        newRowSelector.Child = New TextBlock With {.Text = "", .VerticalAlignment = VerticalAlignment.Center, .HorizontalAlignment = HorizontalAlignment.Left, .Margin = New Thickness(2, 0, 0, 0)}
        Grid.SetRow(newRowSelector, RowsGrid.RowDefinitions.Count - 1)
        RowsGrid.Children.Add(newRowSelector)
    End Sub
    Private Sub FillRow(rowIndex As Int32, row() As Object)
        For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
            SetCellText(rowIndex, j, row(j).ToString)
        Next
    End Sub
    Public Sub UpdateVisibleRows()
        Dim dataRowIndices As List(Of Int32) = GetDataRowIndexes(0, _visibleRowCount - 1)
        For i As Int32 = 0 To dataRowIndices.Count - 1
            FillRow(i, _dataView.GetRow(dataRowIndices(i))) ', startrow)
        Next
    End Sub
    Private Sub SetCellText(rowIndex As Int32, columnIndex As Int32, newText As String)
        CType(GridPanel.Children(rowIndex * _dataView.ColumnNames.Count + columnIndex), Cell).Text = newText
    End Sub
    Private Function GetCellText(rowIndex As Int32, columnIndex As Int32) As String
        Return CType(GridPanel.Children(rowIndex * _dataView.ColumnNames.Count + columnIndex), Cell).Text
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
        If oldValue > newValue Then 'Going Up
            Dim offset As Int32 = oldValue - newValue
            For i As Int32 = _visibleRowCount - 1 To offset Step -1
                For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
                    SetCellText(i, j, GetCellText(i - offset, j))
                Next
            Next
            '
            If offset >= _visibleRowCount Then offset = _visibleRowCount
            For Each dataRowIndex As Int32 In GetDataRowIndexes(0, offset - 1)
                FillRow(counter, _dataView.GetRow(dataRowIndex))
                counter += 1
            Next
        ElseIf oldValue < newValue Then 'Going Down
            Dim offset As Int32 = newValue - oldValue
            For i As Int32 = 0 To _visibleRowCount - offset - 1
                For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
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
                FillRow(counter, _dataView.GetRow(dataRowIndex))
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
        RowsColumnArea.Width = New GridLength(CInt(New FormattedText(_dataView.NumberOfRows.ToString, Globalization.CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, New Typeface(New FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal), 12, Brushes.Black).Width) + 3)
        '
        For i As Int32 = 0 To _visibleRowCount - 1
            CType(CType(RowsGrid.Children(i), RowHeader).Child, TextBlock).Text = GetDataRowIndex(i).ToString
        Next
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
        If _allSelected = True Or _selectedRowsOnly = True Then
            For i As Int32 = 0 To _visibleRowCount - 1
                For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
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
                        For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
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
                    For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
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
        Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        Dim lastRowDataIndex As Int32 = firstRowDataIndex + _visibleRowCount - 1
        If _activeCellVirtualRowIndex >= firstRowDataIndex AndAlso _activeCellVirtualRowIndex <= lastRowDataIndex Then
            Dim rowIndex As Int32 = _activeCellVirtualRowIndex - firstRowDataIndex
            Dim rowHeight As Integer = _rowHeight
            Dim rowWidth As Double = GridPanel.ColumnDefinitions(_activeCellDataColumnIndex).Width.Value
            Dim aDrawing As New GeometryDrawing(Brushes.LightCyan, New Pen(Brushes.DarkBlue, 1), New RectangleGeometry(New Rect(0, 0, rowWidth, rowHeight)))
            Dim activeBrush As New DrawingBrush(aDrawing)
            CType(GridPanel.Children(rowIndex * _dataView.ColumnNames.Count + _activeCellDataColumnIndex), Cell).Background = activeBrush
        End If
    End Sub
    Public Sub SetActiveCell(newDataRowIndex As Int32, newDataColumnIndex As Int32)
        '
        _activeCellDataColumnIndex = newDataColumnIndex
        _activeCellVirtualRowIndex = newDataRowIndex
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
        If _selectedDataRowIndices.Count > 0 And _selectedRowsOnly = False Then
            ShowSelected.IsEnabled = True
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ShowSelectedIcon_22x22.png"))
            DeSelectAll.IsEnabled = True
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIcon_22x22.png"))
        Else
            ShowSelected.IsEnabled = False
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            DeSelectAll.IsEnabled = False
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
        End If

    End Sub
    Private Sub DeSelectAllCells()
        For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
            For j As Int32 = 0 To GridPanel.RowDefinitions.Count - 1
                DeSelectCell(i, j)
            Next
        Next
    End Sub
    Private Sub SelectCell(columnIndex As Int32, rowIndex As Int32)
        CType(GridPanel.Children(rowIndex * _dataView.ColumnNames.Count + columnIndex), Cell).Background = Brushes.Turquoise
    End Sub
    Private Sub DeSelectCell(columnIndex As Int32, rowIndex As Int32)
        CType(GridPanel.Children(rowIndex * _dataView.ColumnNames.Count + columnIndex), Cell).Background = Brushes.White
    End Sub
#End Region

#Region "Cell Selection Logic"
    Private Sub GridPanel_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles TableViewerGrid.PreviewKeyDown
        Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        Dim lastRowDataIndex As Int32 = firstRowDataIndex + _visibleRowCount - 1
        Dim rowIndex As Int32 = _activeCellVirtualRowIndex - firstRowDataIndex
        If e.Key = Key.PageDown Then
            If lastRowDataIndex = _dataView.NumberOfRows - 1 Then Exit Sub
            If _cellEditTextBox.IsFocused Then GridPanel.Focus()
            VerticalScrollbar.Value += _visibleRowCount
            Dim maxRows As Int32 = _dataView.NumberOfRows
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
                If _activeCellVirtualRowIndex < _dataView.NumberOfRows - 1 Then SetActiveCell(_activeCellVirtualRowIndex + 1, _activeCellDataColumnIndex)
            End If
            If _activeCellVirtualRowIndex < firstRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex
            If _activeCellVirtualRowIndex > lastRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1
            e.Handled = True
            Exit Sub
        End If
        If e.Key = Key.Right Then
            If _activeCellDataColumnIndex < _dataView.ColumnNames.Count - 1 Then SetActiveCell(_activeCellVirtualRowIndex, _activeCellDataColumnIndex + 1)
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
    Private Sub GridPanel_KeyDown(sender As Object, e As KeyEventArgs) Handles TableViewerGrid.KeyDown
        If _cellEditTextBox.IsFocused = True Then Exit Sub
        If e.Key = Key.LeftCtrl Or e.Key = Key.RightCtrl Then Exit Sub
        If e.Key = Key.LeftShift Or e.Key = Key.RightShift Then Exit Sub
        Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        Dim lastRowDataIndex As Int32 = firstRowDataIndex + _visibleRowCount - 1
        If e.Key = Key.Tab Then
            If _activeCellDataColumnIndex < _dataView.ColumnNames.Count - 1 Then SetActiveCell(_activeCellVirtualRowIndex, _activeCellDataColumnIndex + 1)
            If _activeCellVirtualRowIndex < firstRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex
            If _activeCellVirtualRowIndex > lastRowDataIndex Then VerticalScrollbar.Value = _activeCellVirtualRowIndex - _visibleRowCount + 1
            e.Handled = True
            Exit Sub
        End If
        If e.Key = Key.Enter Then
            If _selectedRowsOnly = True Then
                If _activeCellVirtualRowIndex < _selectedDataRowIndices.Count - 1 Then SetActiveCell(_activeCellVirtualRowIndex + 1, _activeCellDataColumnIndex)
            Else
                If _activeCellVirtualRowIndex < _dataView.NumberOfRows - 1 Then SetActiveCell(_activeCellVirtualRowIndex + 1, _activeCellDataColumnIndex)
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
                _allSelected = True
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
        _allSelected = False
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
        If _cellSelectable = True Then
            _mouseSelectionMode = SelectionMode.CellSelect
        ElseIf RowSelectable = True Then
            _mouseSelectionMode = SelectionMode.RowSelect
        ElseIf _columnSelectable = True Then
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
                _allSelected = False
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
                        For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
                            SelectCell(j, i)
                        Next
                    Next
                ElseIf _mouseDownVirtualRowIndex > (verticalScrollBarValue + _visibleRowCount - 1) Then
                    For i As Int32 = _visibleRowCount - 1 To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
                            SelectCell(j, i)
                        Next
                    Next
                Else
                    For i As Int32 = (_mouseDownVirtualRowIndex - verticalScrollBarValue) To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
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
        _allSelected = False
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
                CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ShowSelectedIcon_22x22.png"))
                DeSelectAll.IsEnabled = True
                CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIcon_22x22.png"))
            ElseIf _selectedDataRowIndices.Count <= 0 And _selectedRowsOnly = True Then
                ShowAll_Checked(Nothing, Nothing)
            ElseIf _selectedRowsOnly = True Then
                'Do nothing
            Else
                ShowSelected.IsEnabled = False
                CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
                DeSelectAll.IsEnabled = False
                CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
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
        _allSelected = False
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
                widthSums += ColumnsGrid.ColumnDefinitions(counter).ActualWidth
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
                _allSelected = False
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
                        For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
                            SelectCell(j, i)
                        Next
                    Next
                ElseIf _mouseDownVirtualRowIndex > (verticalScrollBarValue + _visibleRowCount - 1) Then
                    For i As Int32 = _visibleRowCount - 1 To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
                            SelectCell(j, i)
                        Next
                    Next
                Else
                    For i As Int32 = (_mouseDownVirtualRowIndex - verticalScrollBarValue) To (mouseMoveDataRowIndex - verticalScrollBarValue) Step rowStep
                        For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
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
        _allSelected = False
        Dim verticalScrollBarValue As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        Dim gridPosition As Point = e.GetPosition(GridPanel)
        Dim mouseUpDataRowIndex As Int32 = GetTableRowIndex(gridPosition) + verticalScrollBarValue
        If _mouseSelectionMode = SelectionMode.RowSelect Then
            Dim rowStep As Int32 = 1
            If mouseUpDataRowIndex < _mouseDownVirtualRowIndex Then rowStep = -1
            For i As Int32 = _mouseDownVirtualRowIndex To mouseUpDataRowIndex Step rowStep
                _selectedDataRowIndices.Add(_rowId(i))
            Next
            _selectedDataRowIndices = _selectedDataRowIndices.Distinct.ToList
            _selectedDataRowIndices.Sort()
            SetSelectedCells()
            RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
        ElseIf _mouseSelectionMode = SelectionMode.CellSelect Then
            If _mouseDownVirtualRowIndex = mouseUpDataRowIndex AndAlso _mouseDownColumnIndex = 0 Then
                If _selectedCellIndices.ContainsKey(_rowId(mouseUpDataRowIndex)) = False Then _selectedCellIndices.Add(_rowId(mouseUpDataRowIndex), New SortedSet(Of Int32))
                _selectedCellIndices(_rowId(mouseUpDataRowIndex)).Add(0)
                SelectCell(0, mouseUpDataRowIndex - verticalScrollBarValue)
            Else
                Dim rowStep As Int32 = -1
                Dim columnStep As Int32 = -1
                If mouseUpDataRowIndex >= _mouseDownVirtualRowIndex Then rowStep = 1
                If 0 >= _mouseDownColumnIndex Then columnStep = 1
                '
                For i As Int32 = _mouseDownVirtualRowIndex To mouseUpDataRowIndex Step rowStep
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
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ShowSelectedIcon_22x22.png"))
            DeSelectAll.IsEnabled = True
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIcon_22x22.png"))
        ElseIf _selectedDataRowIndices.Count <= 0 And _selectedRowsOnly = True Then
            ShowAll_Checked(Nothing, Nothing)
        ElseIf _selectedRowsOnly = True Then
            'Do nothing
        Else 
            ShowSelected.IsEnabled = False
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            DeSelectAll.IsEnabled = False
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            ShowAll_Checked(Nothing, Nothing)
        End If
        _mouseSelectionMode = SelectionMode.None
        Dim el As UIElement = CType(sender, UIElement)
        el.ReleaseMouseCapture()
    End Sub
    Private Sub RowsGrid_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)
        Dim verticalScrollBarValue As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        Dim gridPosition As Point = e.GetPosition(GridPanel)
        Dim mouseUpTableRowIndex As Int32 = GetTableRowIndex(gridPosition) + verticalScrollBarValue
        '
        If _selectedDataRowIndices.BinarySearch(_rowId(mouseUpTableRowIndex)) < 0 Then
            _selectedDataRowIndices.Clear()
            _selectedDataRowIndices.Add(_rowId(mouseUpTableRowIndex))
            DeSelectAllCells()
            SetSelectedCells()
            RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
        End If
        '
        Dim rowMenu As New ContextMenu
        'If Editable = True Then
        Dim headerText As String = "Delete Rows"
        If _selectedDataRowIndices.Count = 1 Then headerText = "Delete Row"
        Dim rowMenuItem As New MenuItem With {.Header = headerText, .IsEnabled = Editable}
        AddHandler rowMenuItem.Click, AddressOf DeleteRows
        rowMenu.Items.Add(rowMenuItem)
        '
        rowMenu.IsOpen = True
        'End If
        RaiseEvent RowRightButtonUp(rowMenu, _rowId(mouseUpTableRowIndex))
    End Sub
    Private Sub DeleteRows(sender As Object, e As RoutedEventArgs)
        _dataView.DeleteRows(_selectedDataRowIndices.ToArray())
    End Sub
#End Region

#Region "Column Select Logic"
    Private Sub ColumnsGrid_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        _allSelected = False
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
        ElseIf _columnSelectable = True Then
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
            _allSelected = False
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
        _allSelected = False
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
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ShowSelectedIcon_22x22.png"))
            DeSelectAll.IsEnabled = True
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIcon_22x22.png"))
        ElseIf _selectedDataRowIndices.Count <= 0 And _selectedRowsOnly = True Then
            ShowAll_Checked(Nothing, Nothing)
        ElseIf _selectedRowsOnly = True Then
            'Do nothing
        Else 
            ShowSelected.IsEnabled = False
            CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            DeSelectAll.IsEnabled = False
            CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
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
            _allSelected = Not _allSelected
            _selectedDataRowIndices.Clear()
            _selectedCellIndices.Clear()
            _selectedColumnIndices.Clear()
        ElseIf _mouseSelectionMode = SelectionMode.CellSelect Then
            _allSelected = False
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
            _allSelected = False
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
        If _allSelected = True Then
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
        Dim gridMenuItem As New MenuItem With {.IsEnabled = enableCopy, .Header = "Copy"}
        AddHandler gridMenuItem.Click, AddressOf Copy
        gridMenu.Items.Add(gridMenuItem)
        '
        gridMenuItem = New MenuItem With {.IsEnabled = enableCopy, .Header = "Copy with Headers"}
        AddHandler gridMenuItem.Click, AddressOf CopyWithHeaders
        gridMenu.Items.Add(gridMenuItem)
        '
        If Editable = True Then
            gridMenuItem = New MenuItem With {.Header = "Paste"}
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
    'Private Function ConvertToType(value As Object, toType As Type) As Object
    '    Select Case toType
    '        Case GetType(Double)
    '            Return CType(value, Double)
    '        Case GetType(Single)
    '            Return CType(value, Single)
    '        Case GetType(Long)
    '            Return CType(value, Long)
    '        Case GetType(Integer)
    '            Return CType(value, Integer)
    '        Case GetType(Short)
    '            Return CType(value, Short)
    '        Case GetType(Byte)
    '            Return CType(value, Byte)
    '        Case GetType(String)
    '            Return CType(value, String)
    '        Case GetType(Boolean)
    '            Return CType(value, Boolean)
    '        Case Else
    '            Return value.ToString
    '    End Select
    'End Function
    Private Sub PasteClipboard()
        If IsSelectionContinuous() = False Then Throw New Exception("Selection must be continuous to paste.")
        '
        Dim clipboardData As String()() = Clipboard.GetText().Split(ControlChars.Lf).[Select](Function(row) row.Split(ControlChars.Tab).[Select](Function(cell) If(cell.Length > 0 AndAlso cell(cell.Length - 1) = ControlChars.Cr, cell.Substring(0, cell.Length - 1), cell)).ToArray()).Where(Function(a) a.Any(Function(b) b.Length > 0)).ToArray()
        Dim dataBaseRowIndex, columnIndex As Int32
        Dim rowIndices As New List(Of Int32)
        Dim columnIndices As New List(Of Int32)
        Dim editValues As New List(Of Object)
        If _allSelected = True Then
            For i As Int32 = 0 To _dataView.NumberOfRows - 1
                If i = clipboardData.Count Then Exit For
                For j As Int32 = 0 To _dataView.ColumnNames.Count - 1
                    If j = clipboardData(i).Count Then Exit For
                    If _readOnlyColumns.Contains(j) Then Continue For
                    rowIndices.Add(_rowId(i))
                    columnIndices.Add(j)
                    editValues.Add(clipboardData(i)(j)) 
                Next
            Next
        ElseIf _selectedColumnIndices.Count > 0 Then
            For i As Int32 = 0 To _dataView.NumberOfRows - 1
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
                For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
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
                    If _rowOffset(dataBaseRowIndex) + i >= _dataView.NumberOfRows Then Exit For
                    For j As Int32 = 0 To clipboardData(i).Count - 1
                        If j = clipboardData(i).Count Then Exit For
                        If columnIndex + j >= _dataView.ColumnNames.Count Then Exit For
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
        _dataView.EditCells(rowIndices.ToArray(), columnIndices.ToArray(), editValues.ToArray())
        UpdateVisibleRows()
        UpdateUndoRedoButtons()
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
    Private Sub CaptureSelectionToClipBoard(includeHeaders As Boolean)
        If IsSelectionUniform() = False Then Throw New Exception("Selection must be uniform to copy.")
        Clipboard.Clear()
        '
        If _allSelected = True Then
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
        If _selectedColumnIndices.Count * _dataView.NumberOfRows > 50000 Then
            Dim msgString As String = "Operation will copy " & _dataView.NumberOfRows * _dataView.ColumnNames.Count & " cell values to the clipboard.  Are you sure you want to copy that much data to the clipboard?"
            If MsgBox(msgString, MsgBoxStyle.YesNo, "Large Amount of Data To Clipboard") <> MsgBoxResult.Yes Then Exit Sub
        End If
        '
        Dim boardText As New Text.StringBuilder
        '
        Try
            Mouse.OverrideCursor = Cursors.Wait
            '
            If includeHeaders = True Then
                boardText.Append(_dataView.ColumnNames(0))
                For i As Int32 = 1 To _dataView.ColumnNames.Count - 1
                    boardText.Append(ChrW(9) & _dataView.ColumnNames(i))
                Next
                boardText.Append(ChrW(10))
            End If
            '
            Dim readerRow() As Object
            For i As Int32 = 0 To _dataView.NumberOfRows - 1
                readerRow = _dataView.GetRow(_rowId(i))
                boardText.Append(readerRow(0).ToString)
                For j As Int32 = 1 To _dataView.ColumnNames.Count - 1
                    boardText.Append(ChrW(9) & readerRow(j).ToString)
                Next
                If i < (_dataView.NumberOfRows - 1) Then boardText.Append(ChrW(10))
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
        If _selectedColumnIndices.Count * _dataView.NumberOfRows > 50000 Then
            Dim msgString As String = "Operation will copy " & _selectedColumnIndices.Count * _dataView.NumberOfRows & " cell values to the clipboard.  Are you sure you want to copy that much data to the clipboard?"
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
                If includeHeaders Then boardText.Append(_dataView.ColumnNames(_selectedColumnIndices(i)) & ChrW(9))
                columnData.Add(_dataView.GetColumn(_dataView.ColumnNames(_selectedColumnIndices(i))))
            Next
            If includeHeaders Then boardText(boardText.Length - 1) = ChrW(10)
            '
            For i As Int32 = 0 To _dataView.NumberOfRows - 1
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
        If _selectedDataRowIndices.Count * _dataView.ColumnNames.Count > 50000 Then
            Dim msgString As String = "Operation will copy " & _selectedDataRowIndices.Count * _dataView.ColumnNames.Count & " cell values to the clipboard.  Are you sure you want to copy that much data to the clipboard?"
            If MsgBox(msgString, MsgBoxStyle.YesNo, "Large Amount of Data To Clipboard") <> MsgBoxResult.Yes Then Exit Sub
        End If
        '
        Dim boardText As New Text.StringBuilder
        Dim readerRow() As Object
        Try
            Mouse.OverrideCursor = Cursors.Wait
            '
            If includeHeaders = True Then
                boardText.Append(_dataView.ColumnNames(0))
                For i As Int32 = 1 To _dataView.ColumnNames.Count - 1
                    boardText.Append(ChrW(9) & _dataView.ColumnNames(i))
                Next
                boardText.Append(ChrW(10))
            End If
            '
            If _columnSortOrder <> SortOrder.None Then
                '
                For Each r As KeyValuePair(Of Int32, Int32) In GetSelectedRowVirtualRowIndices()
                    readerRow = _dataView.GetRow(r.Value)
                    boardText.Append(readerRow(0).ToString)
                    For j As Int32 = 1 To _dataView.ColumnNames.Count - 1
                        boardText.Append(ChrW(9) & readerRow(j).ToString)
                    Next
                    boardText.Append(ChrW(10))
                Next
                boardText.Remove(boardText.Length - 1, 1)
            Else
                For i As Int32 = 0 To _selectedDataRowIndices.Count - 1
                    readerRow = _dataView.GetRow(_selectedDataRowIndices(i))
                    boardText.Append(readerRow(0).ToString)
                    For j As Int32 = 1 To _dataView.ColumnNames.Count - 1
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
                    boardText.Append(_dataView.ColumnNames(column) & ChrW(9))
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
        If _allSelected = True Then
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
        If _allSelected = True Then
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
                _dataView.EditCell(rowIndex, columnIndex, newText)
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
        If _dataView.NumberOfRows = 0 Then Exit Sub

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
        Dim columnMenuItem As New MenuItem
        columnMenuItem.Header = "Sort Ascending"
        AddHandler columnMenuItem.Click, AddressOf SortColumnAscending
        If _columnsSortedOrder(_mouseDownColumnIndex) = SortOrder.Ascending Then columnMenuItem.IsEnabled = False
        columnMenu.Items.Add(columnMenuItem)
        '
        columnMenuItem = New MenuItem
        columnMenuItem.Header = "Sort Descending"
        AddHandler columnMenuItem.Click, AddressOf SortColumnDescending
        If _columnsSortedOrder(_mouseDownColumnIndex) = SortOrder.Descending Then columnMenuItem.IsEnabled = False
        columnMenu.Items.Add(columnMenuItem)
        '
        columnMenuItem = New MenuItem
        columnMenuItem.Header = "Remove Sort"
        AddHandler columnMenuItem.Click, AddressOf RemoveSort
        If _columnsSortedOrder(_mouseDownColumnIndex) = SortOrder.None Then columnMenuItem.IsEnabled = False
        columnMenu.Items.Add(columnMenuItem)
        '
        columnMenuItem = New MenuItem
        columnMenuItem.Header = "Calc. Statistics..."
        AddHandler columnMenuItem.Click, AddressOf CalcColumnStatistics
        Select Case _dataView.ColumnTypes(_mouseDownColumnIndex)
            Case GetType(Boolean)
                columnMenuItem.IsEnabled = False
            Case GetType(Object)
                columnMenuItem.IsEnabled = False
        End Select
        columnMenu.Items.Add(columnMenuItem)
        '
        columnMenuItem = New MenuItem
        columnMenuItem.Header = "Find..."
        AddHandler columnMenuItem.Click, AddressOf SearchText
        If _selectedRowsOnly = True Then columnMenuItem.IsEnabled = False
        If _dataView.NumberOfRows = 0 Then columnMenuItem.IsEnabled = False
        columnMenu.Items.Add(columnMenuItem)

        If Editable = True And _readOnlyColumns.Contains(_mouseDownColumnIndex) = False Then
            columnMenuItem = New MenuItem
            columnMenuItem.Header = "Field Calculator..."
            AddHandler columnMenuItem.Click, AddressOf OpenFcForSpecificColumn
            columnMenu.Items.Add(columnMenuItem)
        End If

        If Editable = True And _readOnlyColumns.Contains(_mouseDownColumnIndex) = False Then
            columnMenuItem = New MenuItem
            If _selectedColumnIndices.Count = 1 Then
                columnMenuItem.Header = "Delete Column"
            Else
                columnMenuItem.Header = "Delete Columns"
            End If
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
        _dataView.DeleteColumns(columnIndices.ToArray)
    End Sub
    Private Sub CalcColumnStatistics(sender As Object, e As RoutedEventArgs)
        'Dim columnstats As New ColumnStatsWindow(Me, _mouseDownColumnIndex)
        'columnstats.Owner = Window.GetWindow(Me)
        'columnstats.Show()
    End Sub
    Private Sub SearchText()
        If _dataView.NumberOfRows > 0 Then
            Dim findWindow As New FindAndReplace(Me, _mouseDownColumnIndex, _activeCellVirtualRowIndex)
            findWindow.ShowDialog()
        End If
    End Sub
    Private Sub OpenFcForSpecificColumn(sender As Object, e As RoutedEventArgs)
        Dim f As New FieldCalculator(_dataView, _selectedDataRowIndices, _readOnlyColumns, _dataView.ColumnNames(_mouseDownColumnIndex))
        If f.ShowDialog = True Then
            UpdateVisibleRows() 
            UpdateUndoRedoButtons()
        End If
    End Sub
#End Region

#Region "Sorting Logic"
    Private Sub RemoveSort()
        _columnSortOrder = SortOrder.None
        For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
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
        'Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
        'If _selectedRowsOnly = False Then
        '    For i As Int32 = 0 To _visibleRowCount - 1
        '        FillRow(i, _dataView.GetRow(_rowId(firstRowDataIndex))) ', startrow)
        '        firstRowDataIndex += 1
        '    Next
        '    DeSelectAllCells()
        '    'SetEditedCells()
        '    SetSelectedCells()
        'Else
        '    For i As Int32 = 0 To _visibleRowCount - 1
        '        FillRow(i, _dataView.GetRow(_rowId(_selectedDataRowIndices(firstRowDataIndex)))) ', startrow)
        '        firstRowDataIndex += 1
        '    Next
        '    'SetEditedCells()
        'End If
        CType(ColumnsGrid.Children(_mouseDownColumnIndex * 2), ColumnHeader).RemoveSorter()
        UpdateRowHeaders()
    End Sub
    Private Sub SortColumnDescending()
        Try
            Mouse.OverrideCursor = Cursors.Wait
            SortColumn(_mouseDownColumnIndex, False)
            _columnSortOrder = SortOrder.Descending
            For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
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
            'Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
            'If _selectedRowsOnly = False Then
            '    For i As Int32 = 0 To _visibleRowCount - 1
            '        FillRow(i, _dataView.GetRow(_rowId(firstRowDataIndex))) ', startrow)
            '        firstRowDataIndex += 1
            '    Next
            '    DeSelectAllCells()
            '    'SetEditedCells()
            '    SetSelectedCells()
            'Else
            '    ReDim _sortedSelectedRowOffsets(_selectedDataRowIndices.Count - 1)
            '    For i As Int32 = 0 To _selectedDataRowIndices.Count - 1
            '        _sortedSelectedRowOffsets(i) = _rowOffset(_selectedDataRowIndices(i))
            '    Next
            '    Array.Sort(_sortedSelectedRowOffsets)
            '    _sortedSelectedRowOffsets.Reverse()
            '    For i As Int32 = 0 To _visibleRowCount - 1
            '        FillRow(i, _dataView.GetRow(_rowId(_sortedSelectedRowOffsets(firstRowDataIndex))))
            '        firstRowDataIndex += 1
            '    Next
            '    'SetEditedCells()
            'End If
            For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
                CType(ColumnsGrid.Children(i * 2), ColumnHeader).RemoveSorter()
            Next
            CType(ColumnsGrid.Children(_mouseDownColumnIndex * 2), ColumnHeader).AddSorter(False)
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
            For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
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
            'Dim firstRowDataIndex As Int32 = CInt(Math.Floor(VerticalScrollbar.Value))
            'If _selectedRowsOnly = False Then
            '    For i As Int32 = 0 To _visibleRowCount - 1
            '        FillRow(i, _dataView.GetRow(_rowId(firstRowDataIndex))) ', startrow)
            '        firstRowDataIndex += 1
            '    Next
            '    DeSelectAllCells()
            '    'SetEditedCells()
            '    SetSelectedCells()
            'Else
            '    ReDim _sortedSelectedRowOffsets(_selectedDataRowIndices.Count - 1)
            '    For i As Int32 = 0 To _selectedDataRowIndices.Count - 1
            '        _sortedSelectedRowOffsets(i) = _rowOffset(_selectedDataRowIndices(i))
            '    Next
            '    Array.Sort(_sortedSelectedRowOffsets)
            '    For i As Int32 = 0 To _visibleRowCount - 1
            '        FillRow(i, _dataView.GetRow(_rowId(_sortedSelectedRowOffsets(firstRowDataIndex))))
            '        firstRowDataIndex += 1
            '    Next
            '    'SetEditedCells()
            'End If
            For i As Int32 = 0 To _dataView.ColumnNames.Count - 1
                CType(ColumnsGrid.Children(i * 2), ColumnHeader).RemoveSorter()
            Next
            CType(ColumnsGrid.Children(_mouseDownColumnIndex * 2), ColumnHeader).AddSorter(True)
            UpdateRowHeaders()
            Mouse.OverrideCursor = Nothing
        Catch ex As Exception
            Mouse.OverrideCursor = Nothing
        End Try
    End Sub
    'Public Function GetEditedColumn(columnIndex As Int32) As Object()
    '    Dim wasOpen As Boolean = _dataView.ParentDatabase.DataBaseOpen
    '    If _dataView.ParentDatabase.DataBaseOpen = False Then _dataView.ParentDatabase.Open()
    '    Dim columnData() As Object = _dataView.GetColumn(_dataView.ColumnNames(columnIndex))
    '    Dim columnEdits As List(Of DBFCellEdit) = _editSession.GetEditsInColumn(columnIndex)
    '    For Each columnEditCell As DBFCellEdit In columnEdits
    '        columnData(columnEditCell.GetCell.GetRow) = columnEditCell.GetEditedValue(columnEditCell.GetCell)
    '    Next
    '    If wasOpen = False Then _dataView.ParentDatabase.Close()
    '    Return columnData
    'End Function
    Private Sub SortColumn(columnIndex As Int32, ByVal ascending As Boolean)
        Dim columnData() As Object = _dataView.GetColumn(columnIndex)
        'Dim columnEdits As List(Of DBFCellEdit) = _EditSession.GetEditsInColumn(ColumnIndex)
        'For Each columnEditCell As DBFCellEdit In ColumnEdits
        '    ColumnData(ColumnEditCell.GetStoredCell.GetRow) = ColumnEditCell.GetEditedValue(ColumnEditCell.GetStoredCell)
        'Next
        '
        Dim idx As List(Of Integer)
        Select Case _dataView.ColumnTypes(columnIndex)
            Case GetType(Byte)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Byte, Int32)(CByte(x), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Int16)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Short, Int32)(CShort(x), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(UInt16)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of UShort, Int32)(CUShort(x), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Int32)
                Dim sorted As List(Of KeyValuePair(Of Int32, Int32)) = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Int32, Int32)(CInt(x), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(UInt32)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of UInt32, Int32)(CUInt(x), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Int64)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Long, Int32)(CLng(x), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(UInt64)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of ULong, Int32)(CULng(x), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Single)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Single, Int32)(CSng(x), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Double)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Double, Int32)(CDbl(x), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(String)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of String, Int32)(CStr(x), i)).OrderBy(Function(x) x.Key).ToList()
                idx = sorted.[Select](Function(x) x.Value).ToList
            Case GetType(Boolean)
                Dim sorted = columnData.ToList.[Select](Function(x, i) New KeyValuePair(Of Boolean, Int32)(CBool(x), i)).OrderBy(Function(x) x.Key).ToList()
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
        VerticalScrollbar.Maximum = _dataView.NumberOfRows - CInt(Math.Floor(RowsArea.ActualHeight / _rowHeight))
        If _selectedDataRowIndices.Count > 0 Then
            If _selectedDataRowIndices(0) > VerticalScrollbar.Maximum Then
                VerticalScrollbar.Value = VerticalScrollbar.Maximum - 1
            Else
                VerticalScrollbar.Value = _selectedDataRowIndices(0)
            End If
        End If
        _visibleRowCount = CInt(Math.Floor(RowsArea.ActualHeight / _rowHeight))
        If _visibleRowCount > _dataView.NumberOfRows Then _visibleRowCount = _dataView.NumberOfRows
        LoadRows()
        SetSelectedCells()
        UpdateRowHeaders()
        CType(ShowAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
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
            CType(ShowAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ShowAllIcon_22x22.png"))
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
        CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
        DeSelectAll.IsEnabled = False
        CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
        If _selectedRowsOnly = True Then
            _selectedRowsOnly = False
            _visibleRowCount = CInt(Math.Floor(RowsArea.ActualHeight / _rowHeight))
            If _visibleRowCount > _dataView.NumberOfRows Then _visibleRowCount = _dataView.NumberOfRows
            VerticalScrollbar.Maximum = _dataView.NumberOfRows - _visibleRowCount
            LoadRows()
            SetSelectedCells()
            CType(ShowAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            ShowAll.IsEnabled = False
        End If
        GridPanel.Focus()
    End Sub
    Private Sub SelectByAttribute_Click(sender As Object, e As RoutedEventArgs)
        Dim attributeselector As New SelectByAttribute(DataView)
        attributeselector.ExpressionCalculator.SetExpressionText(_attributeSelectorString)
        If attributeselector.ShowDialog = True Then
            _allSelected = False
            _selectedDataRowIndices.Clear()
            _selectedCellIndices.Clear()
            _selectedColumnIndices.Clear()
            DeSelectAllCells()
            If _selectedRowsOnly = False Then
                _selectedDataRowIndices = attributeselector.GetSelectedRows
            Else
                ShowAll_Checked(Nothing, Nothing)
                _selectedDataRowIndices = attributeselector.GetSelectedRows
                ShowSelected_Checked(Nothing, Nothing)
            End If
            SetSelectedCells()
            If _selectedDataRowIndices.Count > 0 And _selectedRowsOnly = False Then
                ShowSelected.IsEnabled = True
                CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ShowSelectedIcon_22x22.png"))
                DeSelectAll.IsEnabled = True
                CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIcon_22x22.png"))
            Else
                ShowSelected.IsEnabled = False
                CType(ShowSelected.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
                DeSelectAll.IsEnabled = False
                CType(DeSelectAll.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/ClearSelectionIconDisabled_22x22.png"))
            End If
            RaiseEvent SelectedRowIndicesChanged(_selectedDataRowIndices)
            _attributeSelectorString = attributeselector.ExpressionCalculator.GetExpressionText
        End If
        GridPanel.Focus()
    End Sub
#End Region

#Region "Editing Toolbar"
    Private Sub Redo_Click(sender As Object, e As RoutedEventArgs) Handles Redo.Click
        RedoLastEdit()
    End Sub
    Private Sub RedoLastEdit()
        _dataView.RedoEdit()
        UpdateVisibleRows()
        UpdateUndoRedoButtons()
    End Sub
    Private Sub Undo_Click(sender As Object, e As RoutedEventArgs) Handles Undo.Click
        UndoLastEdit()
    End Sub
    Private Sub UndoLastEdit()
        _dataView.UndoEdit()
        UpdateVisibleRows()
        UpdateUndoRedoButtons()
    End Sub
    Private Sub UpdateUndoRedoButtons()
        If _dataView.CanUndo() Then
            Undo.IsEnabled = True
            SaveButton.IsEnabled = True
            CType(Undo.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/Undo.png"))
            CType(SaveButton.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/Save.ico"))
        Else
            'Disable undo button
            Undo.IsEnabled = False
            SaveButton.IsEnabled = False
            CType(Undo.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/Undodisabled.png"))
            CType(SaveButton.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/Savedisabled.ico"))
        End If
        'Disable Redo if no more edits to redo
        If _dataView.CanRedo = False Then
            Redo.IsEnabled = False
            CType(Redo.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/RedoDisabled.png"))
        Else
            'Enable Redo
            Redo.IsEnabled = True
            CType(Redo.Content, Image).Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/Redo.png"))
        End If
    End Sub
    Private Sub Save_Click(sender As Object, e As RoutedEventArgs) Handles SaveButton.Click
        If MsgBox("Are you sure you want to save edits?", MsgBoxStyle.OkCancel, "Apply Edits") = MsgBoxResult.Ok Then
            Mouse.OverrideCursor = Cursors.Wait
            Try
                _dataView.ApplyEdits()
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
        Dim f As New FieldCalculator(_dataView, _selectedDataRowIndices, _readOnlyColumns)
        f.ExpressionCalculator.SetExpressionText(_fieldCalculatorString)
        If f.ShowDialog = True Then
            _fieldCalculatorString = f.ExpressionCalculator.GetExpressionText
        End If
    End Sub
#End Region

#End Region
    
    Private Sub WpfCustomViewer_Unloaded(sender As Object, e As RoutedEventArgs) Handles Me.Unloaded
        If Not IsNothing(_dataView) Then
            RemoveHandler _dataView.RowsAdded, AddressOf TableViewRowsAdded
            RemoveHandler _dataView.RowsDeleted, AddressOf TableViewRowsDeleted
            RemoveHandler _dataView.ColumnsAdded, AddressOf TableViewColumnsAdded
            RemoveHandler _dataView.ColumnsDeleted, AddressOf TableViewColumnsDeleted
        End If
    End Sub

    Private Sub VerticalScrollbar_IsEnabledChanged(sender As Object, e As DependencyPropertyChangedEventArgs)
        If VerticalScrollbar.IsEnabled = False Then VerticalScrollbar.Visibility = Visibility.Collapsed Else VerticalScrollbar.Visibility = Visibility.Visible
    End Sub
    Private Class ColumnHeader
        Inherits Border
        Public Sub New(header As String)
            Dim tBlock As New TextBlock
            tBlock.Padding = New Thickness(3, 0, 3, 0)
            tBlock.HorizontalAlignment = HorizontalAlignment.Stretch
            tBlock.VerticalAlignment = VerticalAlignment.Center
            tBlock.TextAlignment = TextAlignment.Center
            Background = Brushes.Transparent
            tBlock.Inlines.Add(" ") 'Space after the sort glyph
            tBlock.Inlines.Add(New Bold(New Run(header)))

            HorizontalAlignment = HorizontalAlignment.Stretch
            VerticalAlignment = VerticalAlignment.Stretch
            Background = Brushes.LightSteelBlue
            CornerRadius = New CornerRadius(2)
            BorderBrush = Brushes.SteelBlue
            BorderThickness = New Thickness(1)
            Margin = New Thickness(1, 0, 1, 0)
            Child = tBlock
        End Sub
        Public Sub RemoveSorter()
            If CType(Child, TextBlock).Inlines.Count = 3 Then CType(Child, TextBlock).Inlines.Remove(CType(Child, TextBlock).Inlines(0))
        End Sub
        Public Sub AddSorter(ascending As Boolean)
            Dim img As New Image
            img.VerticalAlignment = VerticalAlignment.Center
            img.HorizontalAlignment = HorizontalAlignment.Center
            img.Stretch = Stretch.Fill
            img.Width = 11
            img.Height = 11
            RenderOptions.SetBitmapScalingMode(img, BitmapScalingMode.HighQuality)
            If ascending = False Then
                img.Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/DownArrow.png"))
            Else
                img.Source = New BitmapImage(New Uri("pack://application:,,,/DataTable_Viewer;component/Resources/UpArrow.png"))
            End If
            If CType(Child, TextBlock).Inlines.Count = 2 Then
                CType(Child, TextBlock).Inlines.InsertBefore(CType(Child, TextBlock).Inlines(0), New InlineUIContainer(img))
            Else
                CType(Child, TextBlock).Inlines.Remove(CType(Child, TextBlock).Inlines(0))
                CType(Child, TextBlock).Inlines.InsertBefore(CType(Child, TextBlock).Inlines(0), New InlineUIContainer(img))
            End If
        End Sub
    End Class
    Private Class Cell
        Inherits TextBlock
        Public Sub New()
            HorizontalAlignment = HorizontalAlignment.Stretch
            VerticalAlignment = VerticalAlignment.Stretch
            Padding = New Thickness(3)
            TextAlignment = TextAlignment.Left
            TextTrimming = TextTrimming.CharacterEllipsis
            Margin = New Thickness(1, 1, 0, 0)
            Background = Brushes.White
            IsHitTestVisible = False
        End Sub
    End Class
    Private Class RowHeader
        Inherits Border
        Public Sub New()
            HorizontalAlignment = HorizontalAlignment.Stretch
            VerticalAlignment = VerticalAlignment.Stretch
            Background = Brushes.LightSteelBlue
            CornerRadius = New CornerRadius(2)
            BorderBrush = Brushes.SteelBlue
            BorderThickness = New Thickness(1)
            Margin = New Thickness(-1, 0, -1, -1)
        End Sub
    End Class
End Class


