Imports System.ComponentModel
Imports System.Windows.Controls.Primitives
Imports System.Data
Imports System.Collections.ObjectModel
Imports System.Collections.Specialized

Public Class CopyPasteDataGrid
    Inherits DataGrid
    Implements INotifyPropertyChanged

    'TODO: Show row numbers

#Region "Construction"

    ''' <summary>
    ''' Add context menu on new. 
    ''' </summary>
    Public Sub New()

        CanUserResizeColumns = False
        SelectionMode = DataGridSelectionMode.Extended
        SelectionUnit = DataGridSelectionUnit.CellOrRowHeader
        CanUserDeleteRows = False
        CanUserAddRows = False
        CanUserResizeRows = False
        CanUserSortColumns = False
        CanUserReorderColumns = False
        CanUserAddInsertDeleteRows = True
        RowHeight = 24
        VerticalContentAlignment = VerticalAlignment.Center



        ' Add context menu handler
        AddHandler Me.ContextMenuOpening, AddressOf ContextMenu_Opening

        ' Add Copy/Paste Context Menu
        ' Add Row(s)
        _addRowsCMI = New MenuItem With {.Name = "AddRows", .Header = "Add Row(s)"}
        _addRowsCMI.Icon = New Image() With {.Source = Bitmap2BitmapSource(My.Resources.add_row)}
        AddHandler _addRowsCMI.Click, Sub(sender As Object, e As RoutedEventArgs)
                                          Dim uniqueRows = GetRowsWithSelectedCells()
                                          AddRows(Math.Max(uniqueRows.Count, 1))
                                      End Sub
        _CopyPasteContextMenu.Items.Add(_addRowsCMI)
        ' Insert Row(s)
        _insertRowsCMI = New MenuItem With {.Name = "InsertRows", .Header = "Insert Row(s)"}
        _insertRowsCMI.Icon = New Image() With {.Source = Bitmap2BitmapSource(My.Resources.insert_row)}
        AddHandler _insertRowsCMI.Click, Sub(sender As Object, e As RoutedEventArgs) InsertRows()
        _CopyPasteContextMenu.Items.Add(_insertRowsCMI)
        ' Delete Row(s)
        _deleteRowsCMI = New MenuItem With {.Name = "DeleteRows", .Header = "Delete Row(s)"}
        _deleteRowsCMI.Icon = New Image() With {.Source = Bitmap2BitmapSource(My.Resources.delete_row)}
        AddHandler _deleteRowsCMI.Click, Sub(sender As Object, e As RoutedEventArgs) DeleteRows()
        _CopyPasteContextMenu.Items.Add(_deleteRowsCMI)

        ' Set up custom menu items
        ' Separator
        _customSeparatorCM = New Separator With {.Name = "CustomItemsSeparator", .Visibility = Visibility.Collapsed}
        _CopyPasteContextMenu.Items.Add(_customSeparatorCM)

        AddHandler _customContextItems.CollectionChanged, Sub(sender As Object, e As NotifyCollectionChangedEventArgs)
                                                              If Not e.OldItems Is Nothing Then
                                                                  For Each item As MenuItem In e.OldItems
                                                                      _CopyPasteContextMenu.Items.Remove(item)
                                                                  Next
                                                              End If
                                                              If Not e.NewItems Is Nothing Then
                                                                  For Each item As MenuItem In e.NewItems
                                                                      _CopyPasteContextMenu.Items.Insert(3 + _customContextItems.Count, item)
                                                                  Next
                                                              End If
                                                              '
                                                              If _customContextItems.Count = 0 Then
                                                                  _customSeparatorCM.Visibility = Visibility.Collapsed
                                                              Else
                                                                  _customSeparatorCM.Visibility = Visibility.Visible
                                                              End If
                                                          End Sub


        ' Separator
        _seperatorCM = New Separator With {.Name = "Separator"}
        _CopyPasteContextMenu.Items.Add(_seperatorCM)
        ' Select All
        _selectAllCMI = New MenuItem With {.Name = "SelectAll", .Header = "Select All"}
        _selectAllCMI.Icon = New Image() With {.Source = Bitmap2BitmapSource(My.Resources.select_all)}
        AddHandler _selectAllCMI.Click, AddressOf SelectAll_Click
        _CopyPasteContextMenu.Items.Add(_selectAllCMI)
        ' Copy
        _copyCMI = New MenuItem With {.Name = "Copy", .Header = "Copy"}
        _copyCMI.Icon = New Image() With {.Source = Bitmap2BitmapSource(My.Resources.copy)}
        AddHandler _copyCMI.Click, AddressOf Copy_Click
        _CopyPasteContextMenu.Items.Add(_copyCMI)
        ' Copy w / Headers
        _copyWHeadersCMI = New MenuItem With {.Name = "CopyWHeaders", .Header = "Copy w/ Headers"}
        _copyWHeadersCMI.Icon = New Image() With {.Source = Bitmap2BitmapSource(My.Resources.copy_w_headers)}
        AddHandler _copyWHeadersCMI.Click, AddressOf CopyWithHeaders_Click
        _CopyPasteContextMenu.Items.Add(_copyWHeadersCMI)
        ' Paste
        _pasteCMI = New MenuItem With {.Name = "Paste", .Header = "Paste"}
        _pasteCMI.Icon = New Image() With {.Source = Bitmap2BitmapSource(My.Resources.paste)}
        AddHandler _pasteCMI.Click, AddressOf Paste_Click
        _CopyPasteContextMenu.Items.Add(_pasteCMI)

        ' Add sort context menu
        ' Sort ASC
        _sortASCCMI = New MenuItem With {.Name = "SortASC", .Header = New TextBlock With {.Text = "Sort Ascending", .TextAlignment = TextAlignment.Left}}
        _sortASCCMI.HorizontalContentAlignment = HorizontalAlignment.Left
        _sortASCCMI.Icon = New Image() With {.Source = Bitmap2BitmapSource(My.Resources.SortASCFilter)}
        AddHandler _sortASCCMI.Click, AddressOf SortAscending
        _sortContextMenu.Items.Add(_sortASCCMI)
        ' Sort DSC
        _sortDSCCMI = New MenuItem With {.Name = "SortDSC", .Header = New TextBlock With {.Text = "Sort Descending", .TextAlignment = TextAlignment.Left}}
        _sortDSCCMI.Icon = New Image() With {.Source = Bitmap2BitmapSource(My.Resources.SortDSCFilter)}
        AddHandler _sortDSCCMI.Click, AddressOf SortDescending
        _sortContextMenu.Items.Add(_sortDSCCMI)
        ' Clear Sort
        _clearSortCMI = New MenuItem With {.Name = "ClearSort", .Header = New TextBlock With {.Text = "Clear Sort", .TextAlignment = TextAlignment.Left}}
        _clearSortCMI.Icon = New Image() With {.Source = Bitmap2BitmapSource(My.Resources.ClearFilter)}
        AddHandler _clearSortCMI.Click, Sub(sender As Object, e As RoutedEventArgs) ClearSort()
        _sortContextMenu.Items.Add(_clearSortCMI)


        ' These should be false. 
        CanUserDeleteRows = False
        CanUserAddRows = False

        ' This live updates the data grid with changes to the source.
        Me.Items.IsLiveSorting = True

    End Sub

#End Region

#Region "Members"

#Region "Context Menu Items"

    ' Add/Insert/Delete & Copy/Paste Context Menu
    Private _addRowsCMI As MenuItem
    Private _insertRowsCMI As MenuItem
    Private _deleteRowsCMI As MenuItem
    Private _seperatorCM As Separator
    Private _selectAllCMI As MenuItem
    Private _copyCMI As MenuItem
    Private _copyWHeadersCMI As MenuItem
    Private _pasteCMI As MenuItem
    Private _CopyPasteContextMenu As New ContextMenu With {.Focusable = False}

    ' Sort Context Menu
    Private _sortASCCMI As MenuItem
    Private _sortDSCCMI As MenuItem
    Private _clearSortCMI As MenuItem
    Private _sortContextMenu As New ContextMenu With {.Focusable = False}
    Private _rightClickedColumn As DataGridColumn

    ' Custom menu items
    Private _customSeparatorCM As Separator
    Private _customContextItems As New ObservableCollection(Of MenuItem)

#End Region

#Region "Properties"

    Private _canUserAddInsertDeleteRows As Boolean = True

    ''' <summary>
    ''' Determines whether the user can add, insert, or delete rows. 
    ''' </summary>
    <Category("Rows"), Description("Determines whether the user can add, insert, or delete rows.")>
    <Browsable(True)> <DefaultValue(True)>
    Public Property CanUserAddInsertDeleteRows As Boolean
        Get
            Return _canUserAddInsertDeleteRows
        End Get
        Set(value As Boolean)
            If _canUserAddInsertDeleteRows <> value Then
                _canUserAddInsertDeleteRows = value
                If _canUserAddInsertDeleteRows = False Then
                    If _seperatorCM IsNot Nothing Then _seperatorCM.Visibility = Visibility.Collapsed
                    If _addRowsCMI IsNot Nothing Then _addRowsCMI.Visibility = Visibility.Collapsed
                    If _insertRowsCMI IsNot Nothing Then _insertRowsCMI.Visibility = Visibility.Collapsed
                    If _deleteRowsCMI IsNot Nothing Then _deleteRowsCMI.Visibility = Visibility.Collapsed
                Else
                    If _seperatorCM IsNot Nothing Then _seperatorCM.Visibility = Visibility.Visible
                    If _addRowsCMI IsNot Nothing Then _addRowsCMI.Visibility = Visibility.Visible
                    If _insertRowsCMI IsNot Nothing Then _insertRowsCMI.Visibility = Visibility.Visible
                    If _deleteRowsCMI IsNot Nothing Then _deleteRowsCMI.Visibility = Visibility.Visible
                End If
                '
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CanUserAddInsertDeleteRows)))
            End If
        End Set
    End Property

    ''' <summary>
    ''' Determines whether rows are automatically added when pasting.
    ''' </summary>
    <Category("Rows"), Description("Determines whether rows are automatically added when pasting.")>
    <Browsable(True)> <DefaultValue(True)>
    Public Property PasteAddsRows As Boolean = True

    ''' <summary>
    ''' Gets and sets the row type for the data grid.
    ''' </summary>
    Public Property RowType As Type = Nothing

    ''' <summary>
    ''' Determines whether to show the sort context menu.
    ''' </summary>
    Public Property ShowSortContextMenu As Boolean = True

    Public ReadOnly Property CustomMenuItems As ObservableCollection(Of MenuItem)
        Get
            Return _customContextItems
        End Get
    End Property

#End Region

#Region "Events"

    Public Event PreviewPasteData(clipboardData As String()(), ByRef cancelPaste As Boolean)
    Public Event DataPasted()
    Public Event PreviewAddRows(startRowIndex As Integer, nRows As Integer, ByRef cancelAddRows As Boolean)
    Public Event RowsAdded(startRowIndex As Integer, nRows As Integer)
    Public Event PreviewDeleteRows(ByVal rowindices As List(Of Int32), ByRef cancel As Boolean)
    Public Event RowsDeleted(ByVal rowindices As List(Of Int32))
    Public Event Sorted As EventHandler(Of ValueEventArgs(Of DataGridColumn))
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

#End Region

#End Region

#Region "Methods"

    ''' <summary>
    ''' Determines what functionality is available When the datagrid is loaded. 
    ''' </summary>
    Private Sub Me_Loaded(sender As Object, e As RoutedEventArgs) Handles MyBase.Loaded

        ' If the datagrid.IsReadOnly is true then CanUserAddInsertDeleteRows is false.
        ' The paste button is disabled as well. 
        If Me.IsReadOnly = True Then
            ' Disallow editing
            CanUserAddInsertDeleteRows = False
            ' Disable the paste button
            _pasteCMI.IsEnabled = False
            _pasteCMI.Visibility = Visibility.Collapsed
        End If

        If Me.CanSelectMultipleItems = False Then
            _selectAllCMI.IsEnabled = False
            _selectAllCMI.Visibility = Visibility.Collapsed
        End If

        ' Collapse menu items if user cannot edit the data grid.
        'If CanUserAddInsertDeleteRows = False Then
        '    _seperatorCM.Visibility = Visibility.Collapsed
        '    _addRowsCMI.Visibility = Visibility.Collapsed
        '    _insertRowsCMI.Visibility = Visibility.Collapsed
        '    _deleteRowsCMI.Visibility = Visibility.Collapsed
        'End If

    End Sub

#Region "Context Menu"

    ''' <summary>
    ''' Determine the column that has been right-clicked.
    ''' </summary>
    Private Sub CopyPasteDataGrid_PreviewMouseRightButtonUp(sender As Object, e As MouseButtonEventArgs) Handles Me.PreviewMouseRightButtonUp
        Dim DepObject As DependencyObject = CType(e.OriginalSource, DependencyObject)
        While DepObject IsNot Nothing AndAlso Not (TypeOf DepObject Is DataGridColumnHeader) AndAlso Not (TypeOf DepObject Is DataGridRow)
            DepObject = VisualTreeHelper.GetParent(DepObject)
        End While
        If DepObject Is Nothing Then
            Return
        End If
        If TypeOf DepObject Is DataGridColumnHeader Then
            _rightClickedColumn = CType(DepObject, DataGridColumnHeader).Column
            Me.ContextMenu = _sortContextMenu
            _CopyPasteContextMenu.Visibility = Visibility.Collapsed
            _sortContextMenu.Visibility = Visibility.Visible
        Else
            Me.ContextMenu = _CopyPasteContextMenu
            _CopyPasteContextMenu.Visibility = Visibility.Visible
            _sortContextMenu.Visibility = Visibility.Collapsed
        End If
    End Sub

    ''' <summary>
    ''' When context menu is opening, handle the behavior of the menus.
    ''' </summary>
    Private Sub ContextMenu_Opening(sender As Object, e As ContextMenuEventArgs)
        If _CopyPasteContextMenu.Visibility = Visibility.Visible Then
            If Me.SelectedCells.Count <= 0 Then
                ' If no cells are selected, 
                ' then disable the buttons and exit sub.
                _insertRowsCMI.IsEnabled = False
                _deleteRowsCMI.IsEnabled = False
                _copyCMI.IsEnabled = False
                _copyWHeadersCMI.IsEnabled = False
                _pasteCMI.IsEnabled = False
                Exit Sub
            Else
                _addRowsCMI.IsEnabled = CanUserAddInsertDeleteRows
                _insertRowsCMI.IsEnabled = CanUserAddInsertDeleteRows
                _deleteRowsCMI.IsEnabled = CanUserAddInsertDeleteRows
                _copyCMI.IsEnabled = True
                _copyWHeadersCMI.IsEnabled = True
                _pasteCMI.IsEnabled = True
            End If

            ' If there is nothing on the clipboard, then disable the paste button.
            Try
                Dim clipboardData As String()() = DirectCast(Clipboard.GetText(), String).Split(ControlChars.Lf).[Select](Function(row) row.Split(ControlChars.Tab).[Select](Function(Clipboardcell) If(Clipboardcell.Length > 0 AndAlso Clipboardcell(Clipboardcell.Length - 1) = ControlChars.Cr, Clipboardcell.Substring(0, Clipboardcell.Length - 1), Clipboardcell)).ToArray()).Where(Function(a) a.Any(Function(b) b.Length > 0)).ToArray()
                If clipboardData.Length = 0 Then
                    _pasteCMI.IsEnabled = False
                Else
                    _pasteCMI.IsEnabled = True
                End If
            Catch ex As Exception
                _pasteCMI.IsEnabled = False
            End Try

        Else
            If ShowSortContextMenu = False Then e.Handled = True
            If _rightClickedColumn Is Nothing Then e.Handled = True
        End If
    End Sub

#End Region

#Region "Select All, Copy, Paste"

    ''' <summary>
    ''' Select all the cells in the data grid.
    ''' </summary>
    Private Sub SelectAll_Click(sender As Object, e As RoutedEventArgs)
        If Me.CanSelectMultipleItems = False Then Return
        Me.SelectAllCells()
    End Sub

    ''' <summary>
    ''' Copy the selected cells.
    ''' </summary>
    Private Sub Copy_Click(sender As Object, e As RoutedEventArgs)
        ApplicationCommands.Copy.Execute(Nothing, Me)
    End Sub

    ''' <summary>
    ''' Copy the selected cells with data grid headers.
    ''' </summary>
    Private Sub CopyWithHeaders_Click(sender As Object, e As RoutedEventArgs)
        Me.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader
        ApplicationCommands.Copy.Execute(Nothing, Me)
        Me.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader
    End Sub

    ''' <summary>
    ''' When the user presses control+v, paste clipboard.
    ''' </summary>
    Private Sub Me_KeyDown(sender As System.Object, e As KeyEventArgs) Handles Me.KeyDown
        If Keyboard.IsKeyDown(Key.LeftCtrl) Or Keyboard.IsKeyDown(Key.RightCtrl) Then
            If e.Key = Key.V Then PasteClipboard()
        End If
    End Sub

    ''' <summary>
    ''' Paste from the clipboard.
    ''' </summary>
    Private Sub Paste_Click(sender As Object, e As RoutedEventArgs)
        PasteClipboard()
    End Sub

    ''' <summary>
    ''' Paste from the clipboard.
    ''' </summary>
    Public Sub PasteClipboard()
        Try
            If Me.SelectedCells.Count <= 0 Then Exit Sub
            Dim clipboardData As String()() = DirectCast(Clipboard.GetText(), String).Split(ControlChars.Lf).[Select](Function(row) row.Split(ControlChars.Tab).[Select](Function(cell) If(cell.Length > 0 AndAlso cell(cell.Length - 1) = ControlChars.Cr, cell.Substring(0, cell.Length - 1), cell)).ToArray()).Where(Function(a) a.Any(Function(b) b.Length > 0)).ToArray()
            Dim cancelPaste As Boolean = False
            RaiseEvent PreviewPasteData(clipboardData, cancelPaste)
            If cancelPaste = True Then Exit Sub
            Dim rowIndex, columnIndex As Int32

            If Me.SelectedCells.Count = 1 Then

                'fill beyond selected cell
                Dim cellinfo As DataGridCellInfo = Me.SelectedCells(0)
                rowIndex = Me.Items.IndexOf(cellinfo.Item)
                columnIndex = cellinfo.Column.DisplayIndex

                For i As Int32 = 0 To clipboardData.Count - 1
                    If (rowIndex + i) > Me.Items.Count - 1 Then
                        If PasteAddsRows = False Then Exit For
                        ' Clear sorting
                        ClearSort()
                        ' Add rows
                        AddRows(clipboardData.Count - i)
                    End If
                    For j As Int32 = 0 To clipboardData(i).Count - 1
                        If (columnIndex + j) > Me.Columns.Count - 1 Then Continue For
                        If Me.Columns(columnIndex + j).IsReadOnly Then Continue For
                        Dim rowType As Type = Me.Items(rowIndex + i).GetType
                        If rowType = GetType(DataRowView) Then
                            'source is likely a datatable
                            DirectCast(Me.Items(rowIndex + i), DataRowView).Row.Item(columnIndex + j) = clipboardData(i)(j)
                        Else
                            Dim binding As Binding = TryCast(TryCast(Me.Columns(columnIndex + j), DataGridBoundColumn).Binding, Binding)
                            Dim y As System.Reflection.PropertyInfo = rowType.GetProperty(binding.Path.Path)
                            Try
                                If IsDoubleType(y.PropertyType) Then
                                    If String.IsNullOrEmpty(clipboardData(i)(j)) OrElse
                                        clipboardData(i)(j) = "" OrElse
                                        (clipboardData(i)(j).Length > 0 AndAlso clipboardData(i)(j).Substring(0, 1) = " ") Then
                                        y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(Double.NaN, y.PropertyType), Nothing)
                                    ElseIf clipboardData(i)(j) = "-inf" OrElse
                                            clipboardData(i)(j) = "-Inf" OrElse
                                            clipboardData(i)(j) = "-infinity" OrElse
                                            clipboardData(i)(j) = "-Infinity" Then
                                        y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(Double.NegativeInfinity, y.PropertyType), Nothing)
                                    ElseIf clipboardData(i)(j) = "+inf" OrElse
                                            clipboardData(i)(j) = "+Inf" OrElse
                                            clipboardData(i)(j) = "+infinity" OrElse
                                            clipboardData(i)(j) = "+Infinity" Then
                                        y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(Double.PositiveInfinity, y.PropertyType), Nothing)
                                    Else
                                        y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(clipboardData(i)(j), y.PropertyType), Nothing)
                                    End If
                                Else
                                    y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(clipboardData(i)(j), y.PropertyType), Nothing)
                                End If
                            Catch ex As Exception
                                If IsNumericType(y.PropertyType) Then y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(If(IsDoubleType(y.PropertyType), Double.NaN, 0), y.PropertyType), Nothing)
                            End Try
                        End If
                    Next
                Next
            ElseIf Me.SelectedCells.Count > 1 Then
                'Test for continuous selection
                Dim Rows, Columns As New List(Of Int32)
                For Each cellInfo As DataGridCellInfo In Me.SelectedCells
                    rowIndex = Me.Items.IndexOf(cellInfo.Item)
                    columnIndex = cellInfo.Column.DisplayIndex
                    Rows.Add(rowIndex)
                    Columns.Add(columnIndex)
                Next
                '
                Rows.Sort()
                Dim RowMax As Int32 = Rows(Rows.Count - 1)
                rowIndex = Rows(0)
                Columns.Sort()
                columnIndex = Columns(0)
                Dim ColumnMax As Int32 = Columns(Columns.Count - 1)
                Dim CellCheck As DataGridCell
                For i As Int32 = rowIndex To RowMax
                    For j As Int32 = columnIndex To ColumnMax
                        CellCheck = GetCell(i, j)
                        If CellCheck.IsSelected = False Then
                            Mouse.OverrideCursor = Nothing
                            MsgBox("Invalid selection, selected cells must be continuous.", MsgBoxStyle.Information, "Invalid Selection")
                            Exit Sub
                        End If
                    Next
                Next
                'set the clipboard data
                For i As Int32 = 0 To clipboardData.Count - 1
                    If (rowIndex + i) > RowMax Then Exit For
                    '
                    For j As Int32 = 0 To clipboardData(i).Count - 1
                        If (columnIndex + j) > ColumnMax Then Continue For
                        If Me.Columns(columnIndex + j).IsReadOnly Then Continue For
                        Dim rowType As Type = Me.Items(rowIndex + i).GetType
                        If rowType = GetType(System.Data.DataRowView) Then
                            'source is likely a datatable
                            DirectCast(Me.Items(rowIndex + i), System.Data.DataRowView).Row.Item(columnIndex + j) = clipboardData(i)(j)
                        Else
                            Dim binding As Binding = TryCast(TryCast(Me.Columns(columnIndex + j), DataGridBoundColumn).Binding, Binding)
                            Dim y As System.Reflection.PropertyInfo = rowType.GetProperty(binding.Path.Path)
                            Try
                                If IsDoubleType(y.PropertyType) Then
                                    If String.IsNullOrEmpty(clipboardData(i)(j)) OrElse
                                        clipboardData(i)(j) = "" OrElse
                                        (clipboardData(i)(j).Length > 0 AndAlso clipboardData(i)(j).Substring(0, 1) = " ") Then
                                        y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(Double.NaN, y.PropertyType), Nothing)
                                    ElseIf clipboardData(i)(j) = "-inf" OrElse
                                            clipboardData(i)(j) = "-Inf" OrElse
                                            clipboardData(i)(j) = "-infinity" OrElse
                                            clipboardData(i)(j) = "-Infinity" Then
                                        y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(Double.NegativeInfinity, y.PropertyType), Nothing)
                                    ElseIf clipboardData(i)(j) = "+inf" OrElse
                                            clipboardData(i)(j) = "+Inf" OrElse
                                            clipboardData(i)(j) = "+infinity" OrElse
                                            clipboardData(i)(j) = "+Infinity" Then
                                        y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(Double.PositiveInfinity, y.PropertyType), Nothing)
                                    Else
                                        y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(clipboardData(i)(j), y.PropertyType), Nothing)
                                    End If
                                Else
                                    y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(clipboardData(i)(j), y.PropertyType), Nothing)
                                End If
                            Catch ex As Exception
                                If IsNumericType(y.PropertyType) Then y.SetValue(Me.Items(rowIndex + i), Convert.ChangeType(If(IsDoubleType(y.PropertyType), Double.NaN, 0), y.PropertyType), Nothing)
                            End Try
                        End If
                    Next
                Next
            End If
            '
            Me.Items.Refresh()
            RaiseEvent DataPasted()
            '
        Catch ex As Exception
            Mouse.OverrideCursor = Nothing
            MsgBox("Error pasting data from clipboard.", MsgBoxStyle.Information, "Error in paste from clipboard")
        End Try

    End Sub

#End Region

#Region "Add, Insert, Delete"

    ''' <summary>
    ''' Add row to the end of the data grid.
    ''' </summary>
    Public Sub AddRow(rowData As Dictionary(Of String, Object))
        'Count of rows to be added and insert point, if nRowsToAdd is zero or less then just add one row
        'Dim rowCount As Int32 = Math.Max(nRowsToAdd, 1)
        Dim insertAtRow As Integer = Me.Items.Count
        'raise preview event and cancel add if requested.
        'Dim cancelAdd As Boolean = False
        'RaiseEvent PreviewAddRows(insertAtRow, rowCount, cancelAdd)
        'If cancelAdd = True Then Exit Sub
        'exit if there are no items to create an instance from.
        If RowType = Nothing Then
            If Me.Items.Count = 0 Then Exit Sub
            RowType = Me.Items(0).GetType
        End If
        '
        Try
            If RowType = GetType(DataRowView) Then
                'source is likely a datatable
                If Me.Items.Count = 0 Then Exit Sub
                Dim table = DirectCast(Me.Items(0), DataRowView).DataView.Table
                Dim newRow = table.NewRow() 'DirectCast(Me.Items(0), DataRowView).DataView.AddNew()
                For Each v In rowData
                    newRow.Item(v.Key) = v.Value
                Next
                newRow.EndEdit()
            Else
                'get item source (must implement ilist)
                Dim cv As CollectionView = TryCast(Me.ItemsSource, CollectionView)
                Dim itemList As IList
                If Not IsNothing(cv) Then
                    itemList = CType(cv.SourceCollection, IList)
                Else
                    itemList = CType(Me.ItemsSource, IList)
                End If
                '
                Dim obj = Activator.CreateInstance(RowType)
                For Each v In rowData
                    Dim y As System.Reflection.PropertyInfo = RowType.GetProperty(v.Key)
                    Try
                        y.SetValue(obj, Convert.ChangeType(v.Value, y.PropertyType), Nothing)
                    Catch ex As Exception
                        If IsNumericType(y.PropertyType) Then y.SetValue(obj, Convert.ChangeType("0", y.PropertyType), Nothing)
                    End Try
                Next
                'Add rows equal to the number of rows to add (minimum of 1).
                'For i As Int32 = 1 To rowCount
                itemList.Add(obj) 'Activator.CreateInstance(RowType))
                'Next
            End If
            'Raise the rows added event
            RaiseEvent RowsAdded(insertAtRow, 1)
            '
            'Items.Refresh()
            '
        Catch ex As Exception
            MsgBox("Hey developer this shouldn't happen you did something wrong:" & vbNewLine & ex.Message, MsgBoxStyle.Information, "Insert Error")
            Exit Sub
        End Try
    End Sub


    ''' <summary>
    ''' Add rows to the end of the data grid.
    ''' </summary>
    Public Sub AddRows(nRowsToAdd As Integer)
        'Count of rows to be added and insert point, if nRowsToAdd is zero or less then just add one row
        Dim rowCount As Int32 = Math.Max(nRowsToAdd, 1)
        Dim insertAtRow As Integer = Me.Items.Count
        'raise preview event and cancel add if requested.
        Dim cancelAdd As Boolean = False
        RaiseEvent PreviewAddRows(insertAtRow, rowCount, cancelAdd)
        If cancelAdd = True Then Exit Sub
        'exit if there are no items to create an instance from.
        If RowType = Nothing Then
            If Me.Items.Count = 0 Then Exit Sub
            RowType = Me.Items(0).GetType
        End If
        Try
            If RowType = GetType(DataRowView) Then
                'source is likely a datatable
                If Me.Items.Count = 0 Then Exit Sub
                Dim table = DirectCast(Me.Items(0), DataRowView).DataView.Table
                table.Rows.Add(table.NewRow())
                'DirectCast(Me.Items(0), DataRowView).DataView.AddNew()
            Else
                'get item source (must implement ilist)
                Dim cv As CollectionView = TryCast(Me.ItemsSource, CollectionView)
                Dim itemList As IList
                If Not IsNothing(cv) Then
                    itemList = CType(cv.SourceCollection, IList)
                Else
                    itemList = CType(Me.ItemsSource, IList)
                End If
                'Add rows equal to the number of rows to add (minimum of 1).
                For i As Int32 = 1 To rowCount
                    itemList.Add(Activator.CreateInstance(RowType))
                Next
            End If
            '
            RaiseEvent RowsAdded(insertAtRow, rowCount)
            'Items.Refresh()
        Catch ex As Exception
            MsgBox("Hey developer this shouldn't happen you did something wrong:" & vbNewLine & ex.Message, MsgBoxStyle.Information, "Insert Error")
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Insert rows into data grid.
    ''' </summary>
    Public Sub InsertRows()
        If Me.SelectedCells.Count <= 0 Then Exit Sub
        Dim uniqueRows = GetRowsWithSelectedCells()
        'Count of rows to be added and insert point
        Dim rowCount As Int32 = Math.Max(uniqueRows.Count, 1)
        Dim insertAtRow As Integer = uniqueRows.Min
        '
        InsertRows(insertAtRow, rowCount)
    End Sub

    Public Sub InsertRows(startRowIndex As Int32, rowCount As Int32)
        ' Get list of sorted rows
        'get item source (must implement ilist)
        Dim cv As CollectionView = TryCast(Me.ItemsSource, CollectionView)
        Dim itemList As IList
        If Not IsNothing(cv) Then
            itemList = CType(cv.SourceCollection, IList)
        Else
            itemList = CType(Me.ItemsSource, IList)
        End If
        '
        Dim rowList As New List(Of Object)
        Dim sortedList = CollectionViewSource.GetDefaultView(itemList).GetEnumerator()
        For i As Int32 = 0 To itemList.Count - 1
            sortedList.MoveNext()
            rowList.Add(sortedList.Current)
        Next
        '
        If startRowIndex > itemList.Count - 1 Then startRowIndex = itemList.Count - 1
        Dim insertAtRowSorted As Integer = itemList.IndexOf(rowList.Item(startRowIndex))
        '
        'raise preview event and cancel add if requested.
        Dim cancelAdd As Boolean = False
        RaiseEvent PreviewAddRows(insertAtRowSorted, rowCount, cancelAdd)
        If cancelAdd = True Then Exit Sub
        If RowType = Nothing Then
            If Me.Items.Count = 0 Then Exit Sub
            RowType = Me.Items(0).GetType
        End If
        Try
            If RowType = GetType(DataRowView) Then
                'source is likely a datatable
                If Me.Items.Count = 0 Then Exit Sub
                'Dim rowView = DirectCast(Me.Items(0), DataRowView)
                Dim table = DirectCast(Me.Items(0), DataRowView).DataView.Table
                For i As Int32 = 1 To rowCount
                    table.Rows.InsertAt(table.NewRow(), insertAtRowSorted)
                Next
            Else
                For i As Int32 = 1 To rowCount
                    itemList.Insert(insertAtRowSorted, Activator.CreateInstance(RowType))
                Next
            End If
            '
            RaiseEvent RowsAdded(insertAtRowSorted, rowCount)
            'Items.Refresh()
        Catch ex As Exception
            MsgBox("Hey developer this shouldn't happen you did something wrong:" & vbNewLine & ex.Message, MsgBoxStyle.Information, "Insert Error")
            Exit Sub
        End Try
    End Sub

    ''' <summary>
    ''' Delete rows from the table.
    ''' </summary>
    Public Sub DeleteRows()
        If Me.SelectedCells.Count <= 0 Then Exit Sub
        Dim UniqueRows As List(Of Int32) = GetRowsWithSelectedCells().ToList()
        Dim UniqueSortedRows As New List(Of Int32)
        ' If the rows are not sorted, then the unique rows 
        ' and unique sorted row list is the same.
        '
        Dim cancel As Boolean
        RaiseEvent PreviewDeleteRows(UniqueRows, cancel)
        If cancel = True Then Exit Sub
        '
        ' Get list of sorted rows
        'get item source (must implement ilist)
        Dim cv As CollectionView = TryCast(Me.ItemsSource, CollectionView)
        Dim itemList As IList
        If Not IsNothing(cv) Then
            itemList = CType(cv.SourceCollection, IList)
        Else
            itemList = CType(Me.ItemsSource, IList)
        End If
        Dim rowList As New List(Of Object)
        Dim sortedList = CollectionViewSource.GetDefaultView(itemList).GetEnumerator()
        For i As Int32 = 0 To itemList.Count - 1
            sortedList.MoveNext()
            rowList.Add(sortedList.Current)
        Next
        '
        ' Get list of sorted row indeces
        UniqueRows.Sort()
        For i As Int32 = 0 To UniqueRows.Count - 1
            Dim index As Integer = itemList.IndexOf(rowList.Item(UniqueRows(i)))
            UniqueSortedRows.Add(index)
        Next
        '
        ' Perform delete
        UniqueSortedRows.Sort()
        For i As Int32 = UniqueSortedRows.Count - 1 To 0 Step -1
            itemList.RemoveAt(UniqueSortedRows(i))
        Next

        'Dim editableItems As IEditableCollectionView = Me.Items
        'For i As Int32 = 0 To UniqueRows.Count - 1
        '    If editableItems.CanRemove Then editableItems.RemoveAt(UniqueRows(i) - i)
        'Next
        '
        RaiseEvent RowsDeleted(UniqueSortedRows)
        'Items.Refresh()
    End Sub

    ''' <summary>
    ''' Get unique row indexes that have cells selected.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetRowsWithSelectedCells() As HashSet(Of Int32)
        Dim uniqueRows As New HashSet(Of Int32), rowIndex As Int32
        For Each cellInfo As DataGridCellInfo In Me.SelectedCells
            rowIndex = Me.Items.IndexOf(cellInfo.Item)
            If uniqueRows.Contains(rowIndex) = False Then uniqueRows.Add(rowIndex)
        Next
        Return uniqueRows
    End Function

    ''' <summary>
    ''' Get unique column indexes that have cells selected.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetColumnsWithSelectedCells() As HashSet(Of Int32)
        Dim uniqueColumns As New HashSet(Of Int32)
        For Each cellInfo As DataGridCellInfo In Me.SelectedCells
            If uniqueColumns.Contains(cellInfo.Column.DisplayIndex) = False Then uniqueColumns.Add(cellInfo.Column.DisplayIndex)
        Next
        Return uniqueColumns
    End Function

#End Region

#Region "Support"

    ''' <summary>
    ''' Get the data grid row at a specified index.
    ''' </summary>
    ''' <param name="index">The index of the data grid row to be returned.</param>
    Public Function GetRow(index As Integer) As DataGridRow
        Dim row As DataGridRow = DirectCast(Me.ItemContainerGenerator.ContainerFromIndex(index), DataGridRow)
        If row Is Nothing AndAlso index < Me.Items.Count Then
            ' May be virtualized, bring into view and try again.
            Me.UpdateLayout()
            Me.ScrollIntoView(Me.Items(index))
            row = DirectCast(Me.ItemContainerGenerator.ContainerFromIndex(index), DataGridRow)
        End If
        Return row
    End Function

    ''' <summary>
    ''' Get the data grid cell at specified row and column indeces.
    ''' </summary>
    ''' <param name="rowindex">The row index of the data grid cell to be returned.</param>
    ''' <param name="column">The column index of the data grid cell to be returned.</param>
    Public Function GetCell(rowindex As Integer, column As Integer) As DataGridCell
        Dim row As DataGridRow = GetRow(rowindex)
        If row IsNot Nothing Then
            Dim presenter As DataGridCellsPresenter = GetTheVisualChild(Of DataGridCellsPresenter)(row)
            If presenter Is Nothing Then
                Me.ScrollIntoView(row, Me.Columns(column))
                presenter = GetTheVisualChild(Of DataGridCellsPresenter)(row)
            End If
            If presenter Is Nothing Then Return Nothing
            Dim cell As DataGridCell = DirectCast(presenter.ItemContainerGenerator.ContainerFromIndex(column), DataGridCell)
            Return cell
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Get the visual child.
    ''' </summary>
    Public Function GetTheVisualChild(Of T As Visual)(parent As Visual) As T
        Dim child As T = Nothing
        Dim numVisuals As Integer = VisualTreeHelper.GetChildrenCount(parent)
        For i As Integer = 0 To numVisuals - 1
            Dim v As Visual = DirectCast(VisualTreeHelper.GetChild(parent, i), Visual)
            child = TryCast(v, T)
            If child Is Nothing Then
                child = GetTheVisualChild(Of T)(v)
            End If
            If child IsNot Nothing Then
                Exit For
            End If
        Next
        Return child
    End Function

    ''' <summary>
    ''' Determines if a type is numeric. Nullable numeric types are considered numeric.
    ''' </summary>
    ''' <remarks>
    ''' Boolean is not considered numeric.
    ''' <see href="http://stackoverflow.com/questions/124411/using-net-how-can-i-determine-if-a-type-is-a-numeric-valuetype"/>
    ''' </remarks>
    Public Shared Function IsNumericType(typeToTest As Type) As Boolean
        If typeToTest Is Nothing Then
            Return False
        End If
        Select Case Type.GetTypeCode(typeToTest)
            Case TypeCode.Byte, TypeCode.Decimal, TypeCode.Double, TypeCode.Int16, TypeCode.Int32, TypeCode.Int64,
             TypeCode.SByte, TypeCode.Single, TypeCode.UInt16, TypeCode.UInt32, TypeCode.UInt64
                Return True
            Case TypeCode.Object
                If typeToTest.IsGenericType AndAlso typeToTest.GetGenericTypeDefinition() = GetType(Nullable(Of )) Then
                    Return IsNumericType(Nullable.GetUnderlyingType(typeToTest))
                End If
                Return False
        End Select
        Return False
    End Function

    Public Shared Function IsDoubleType(typeToTest As Type) As Boolean
        If typeToTest Is Nothing Then
            Return False
        End If
        Select Case Type.GetTypeCode(typeToTest)
            Case TypeCode.Double
                Return True
        End Select
        Return False
    End Function



#End Region

#Region "Sort"

    ''' <summary>
    ''' Sort the column in ascending order.
    ''' </summary>
    Private Sub SortAscending(sender As Object, e As RoutedEventArgs)
        ' Clear current sort descriptions
        Items.SortDescriptions.Clear()
        ' Add the new sort description
        Items.SortDescriptions.Add(New SortDescription(_rightClickedColumn.SortMemberPath, ListSortDirection.Ascending))
        ' Apply sort
        For Each col In Columns
            col.SortDirection = Nothing
        Next
        _rightClickedColumn.SortDirection = ListSortDirection.Ascending
        ' Refresh items to display sort
        Items.Refresh()
    End Sub

    ''' <summary>
    ''' Sort the column in descending order.
    ''' </summary>
    Private Sub SortDescending(sender As Object, e As RoutedEventArgs)
        ' Clear current sort descriptions
        Items.SortDescriptions.Clear()
        ' Add the new sort description
        Items.SortDescriptions.Add(New SortDescription(_rightClickedColumn.SortMemberPath, ListSortDirection.Descending))
        ' Apply sort
        For Each col In Columns
            col.SortDirection = Nothing
        Next
        _rightClickedColumn.SortDirection = ListSortDirection.Descending
        ' Refresh items to display sort
        Items.Refresh()
    End Sub

    ''' <summary>
    ''' Clear the sort. 
    ''' </summary>
    Public Sub ClearSort()
        ' Clear current sort descriptions
        Items.SortDescriptions.Clear()
        ' Clear sort
        For Each col In Columns
            col.SortDirection = Nothing
        Next
        ' Refresh items to display sort
        Items.Refresh()
    End Sub

    ''' <summary>
    ''' When sorting is complete, raise sorted event.
    ''' </summary>
    Protected Overrides Sub OnSorting(ByVal eventArgs As DataGridSortingEventArgs)
        MyBase.OnSorting(eventArgs)
        Dim column = eventArgs.Column
        RaiseEvent Sorted(Me, New ValueEventArgs(Of DataGridColumn)(column))
    End Sub

    ''' <summary>
    ''' Helper used to get event args.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class ValueEventArgs(Of T)
        Inherits EventArgs
        Public Sub New(ByVal value As T)
            Me.Value = value
        End Sub
        Public Property Value As T
    End Class

#End Region

#End Region

End Class
