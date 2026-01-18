'Imports System.Data.OleDb
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text
Imports ClosedXML

Public MustInherit Class DataTableView

#Region "Variables"
    'stored stuff
    Protected _parentDatabase As DatabaseManager
    Protected _tableName As String
    Protected _storedColumnNames() As String
    Protected _storedColumnTypes() As Type
    Protected _storedNumberOfRows As Integer
    'View stuff
    Private _nRows As Integer
    Private _columnNames() As String
    Private _columnTypes() As Type
    Private ReadOnly _edits As New List(Of TableEdit)
    Private _editIndex As Int32 = -1
    Private _viewToStoredRowIndex() As Int32 'array index is the view row index, array value is the stored row index
    Private _viewToStoredColumnIndex() As Int32 'array index is the view column index, array value is the stored column index
    Private _hiddenColumns As New HashSet(Of String)
#End Region

#Region "Properties"
    Public ReadOnly Property ParentDatabase As DatabaseManager
        Get
            Return _parentDatabase
        End Get
    End Property

    Public ReadOnly Property TableName As String
        Get
            Return _tableName
        End Get
    End Property

    Public ReadOnly Property ColumnNames As String()
        Get
            Return _columnNames
        End Get
    End Property

    Public ReadOnly Property ColumnTypes As Type()
        Get
            Return _columnTypes
        End Get
    End Property

    Public ReadOnly Property NumberOfRows As Int32
        Get
            Return _nRows
        End Get
    End Property
#End Region

#Region "Events"
    Public Event RowsDeleted(rowindices() As Int32)
    Public Event RowsAdded(rowindices() As Int32)
    Public Event ColumnsDeleted(columnIndices() As Int32)
    Public Event ColumnsAdded(columnIndices() As Int32)
    Public Event EditAdded(edit As TableEdit)
#End Region

    Protected Sub InitializeView()
        _nRows = CInt(GetStoredRowCount()) '_storedNumberOfRows
        _columnNames = GetStoredColumnNames()
        _columnTypes = GetStoredColumnTypes()
        ReDim _viewToStoredRowIndex(_nRows - 1)
        For i As Int32 = 0 To _viewToStoredRowIndex.Count - 1
            _viewToStoredRowIndex(i) = i
        Next
        ReDim _viewToStoredColumnIndex(_columnNames.Count - 1)
        For i As Int32 = 0 To _viewToStoredColumnIndex.Count - 1
            _viewToStoredColumnIndex(i) = i
        Next
        '
        'Haven't decided yet if this should be taken care of by the user of the table view or the view itself. I don't want to throw a message box here for sure. 
        'Maybe both here and if the user wants to watch for it then there as well?
        'AddHandler _parentDatabase.EditsSaved, Sub(savedEditTableName As String)
        '                                           if _tableName = savedEditTableName Then CancelEdits()
        '                                       End Sub
    End Sub
#Region "Editing stuff"
    Public Sub ApplyEdits()
        Dim cancelSave As Boolean = False
        _parentDatabase.OnPreviewEditsSaved(_tableName, cancelSave)
        If cancelSave = True Then Exit Sub
        '
        If _editIndex < 0 Then CancelEdits() : Exit Sub
        'first apply all of the row and column edits since the cell edits indices have been updated to the current view.
        Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
        If wasOpen = False Then _parentDatabase.Open()
        '
        For i As Int32 = 0 To _editIndex
            Select Case _edits(i).GetType
                Case GetType(AddRowEdit)
                    AddRowToDatabase(DirectCast(_edits(i), AddRowEdit).OriginalRowEdit)
                Case GetType(AddRowsEdit)
                    AddRowsToDatabase(DirectCast(_edits(i), AddRowsEdit).GetOriginalRowEdits())
                Case GetType(AddColumnEdit(Of Byte()))
                    AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Byte())).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Byte())).OriginalColumnEdit)
                Case GetType(AddColumnEdit(Of Double))
                    AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Double)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Double)).OriginalColumnEdit)
                Case GetType(AddColumnEdit(Of Single))
                    AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Single)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Single)).OriginalColumnEdit)
                Case GetType(AddColumnEdit(Of Long))
                    AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Long)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Long)).OriginalColumnEdit)
                Case GetType(AddColumnEdit(Of Integer))
                    AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Integer)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Integer)).OriginalColumnEdit)
                Case GetType(AddColumnEdit(Of Short))
                    AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Short)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Short)).OriginalColumnEdit)
                Case GetType(AddColumnEdit(Of Byte))
                    AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Byte)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Byte)).OriginalColumnEdit)
                Case GetType(AddColumnEdit(Of Boolean))
                    AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Boolean)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Boolean)).OriginalColumnEdit)
                Case GetType(AddColumnEdit(Of String))
                    AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of String)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of String)).OriginalColumnEdit)
                'Case GetType(AddColumnEdit)
                '    ApplyAddColumnEdit(DirectCast(_edits(i), AddColumnEdit))
                Case GetType(AddColumnsEdit)
                    For Each columnToAdd As IColumnEdit In DirectCast(_edits(i), AddColumnsEdit).AddColumnEdits
                        Select Case columnToAdd.GetType
                            Case GetType(AddColumnEdit(Of Byte()))
                                AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Byte())).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Byte())).OriginalColumnEdit)
                            Case GetType(AddColumnEdit(Of Double))
                                AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Double)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Double)).OriginalColumnEdit)
                            Case GetType(AddColumnEdit(Of Single))
                                AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Single)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Single)).OriginalColumnEdit)
                            Case GetType(AddColumnEdit(Of Long))
                                AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Long)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Long)).OriginalColumnEdit)
                            Case GetType(AddColumnEdit(Of Integer))
                                AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Integer)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Integer)).OriginalColumnEdit)
                            Case GetType(AddColumnEdit(Of Short))
                                AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Short)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Short)).OriginalColumnEdit)
                            Case GetType(AddColumnEdit(Of Byte))
                                AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Byte)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Byte)).OriginalColumnEdit)
                            Case GetType(AddColumnEdit(Of Boolean))
                                AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of Boolean)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of Boolean)).OriginalColumnEdit)
                            Case GetType(AddColumnEdit(Of String))
                                AddColumnToDatabase(DirectCast(_edits(i), AddColumnEdit(Of String)).ColumnName, DirectCast(_edits(i), AddColumnEdit(Of String)).OriginalColumnEdit)
                        End Select
                        'ApplyAddColumnEdit(columnToAdd)
                    Next
                Case GetType(DeleteRowEdit)
                    DeleteRowFromDatabase(DirectCast(_edits(i), DeleteRowEdit).RowIndex)
                Case GetType(DeleteRowsEdit)
                    DeleteRowsFromDatabase(DirectCast(_edits(i), DeleteRowsEdit).GetDeletedRowIndices())
                Case GetType(DeleteColumnEdit)
                    DeleteColumnFromDatabase(_storedColumnNames(DirectCast(_edits(i), DeleteColumnEdit).ColumnIndex))
                Case GetType(DeleteColumnsEdit)
                    With DirectCast(_edits(i), DeleteColumnsEdit)
                        Dim columnsToDelete(.ColumnsDeleted.Count - 1) As String
                        For j As Int32 = 0 To .ColumnsDeleted.Count - 1
                            columnsToDelete(j) = _storedColumnNames(.ColumnsDeleted(j).ColumnIndex)
                        Next
                        DeleteColumnsFromDatabase(columnsToDelete)
                    End With
            End Select
        Next

        'next apply all of the database edits (the indices should match up with the database since all the row/column deletes and additions have been made.
        For i As Int32 = 0 To _editIndex
            Select Case _edits(i).GetType
                Case GetType(CellEdit)
                    ApplyCellEdit(DirectCast(_edits(i), CellEdit))
                Case GetType(ColumnEdit)
                    ApplyEditColumnEdit(DirectCast(_edits(i), ColumnEdit))
                Case GetType(MultiCellEdit)
                    With DirectCast(_edits(i), MultiCellEdit)
                        Dim columnIndices(.CellEdits.Count - 1) As Int32, rowIndices(.CellEdits.Count - 1) As Int32, cellvalues(.CellEdits.Count - 1) As Object
                        For j As Int32 = 0 To columnIndices.Count - 1
                            columnIndices(j) = .CellEdits(j).ColumnIndex
                            rowIndices(j) = .CellEdits(j).RowIndex
                            cellvalues(j) = .CellEdits(j).Value
                        Next
                        EditDatabaseCells(columnIndices, rowIndices, cellvalues)
                    End With
                Case GetType(RowEdit)
                    'for now I am just applying the edits as a multi-cell edit. This works alright since the database calls put it in a transaction. 
                    With DirectCast(_edits(i), RowEdit).GetRowData()
                        Dim columnIndices(.CellEdits.Count - 1) As Int32, rowIndices(.CellEdits.Count - 1) As Int32, cellvalues(.CellEdits.Count - 1) As Object
                        For j As Int32 = 0 To columnIndices.Count - 1
                            columnIndices(j) = .CellEdits(j).ColumnIndex
                            rowIndices(j) = .CellEdits(j).RowIndex
                            cellvalues(j) = .CellEdits(j).Value
                        Next
                        EditDatabaseCells(columnIndices, rowIndices, cellvalues)
                    End With
                    'row edits have not been implemented. No real purpose for them yet.
                    'Throw New NotImplementedException()
            End Select
        Next
        '
        If wasOpen = False Then _parentDatabase.Close()
        'Raise edits saved event
        Dim editsSaved As New List(Of TableEdit)
        For i As Int32 = 0 To _editIndex
            editsSaved.Add(_edits(i))
        Next
        'Clear edits
        CancelEdits()

        _parentDatabase.OnEditsSaved(_tableName, editsSaved)


    End Sub
    Private Sub ApplyCellEdit(cellToEdit As CellEdit)
        With cellToEdit
            Select Case .Value.GetType
                Case GetType(Double)
                    EditDatabaseCell(.ColumnIndex, .RowIndex, DirectCast(.Value, Double))
                Case GetType(Single)
                    EditDatabaseCell(.ColumnIndex, .RowIndex, DirectCast(.Value, Single))
                Case GetType(Long), GetType(ULong)
                    EditDatabaseCell(.ColumnIndex, .RowIndex, DirectCast(.Value, Long))
                Case GetType(Integer), GetType(UInteger)
                    EditDatabaseCell(.ColumnIndex, .RowIndex, DirectCast(.Value, Integer))
                Case GetType(Short), GetType(UShort)
                    EditDatabaseCell(.ColumnIndex, .RowIndex, DirectCast(.Value, Short))
                Case GetType(Byte)
                    EditDatabaseCell(.ColumnIndex, .RowIndex, DirectCast(.Value, Byte))
                Case GetType(Boolean)
                    EditDatabaseCell(.ColumnIndex, .RowIndex, DirectCast(.Value, Boolean))
                Case GetType(String)
                    EditDatabaseCell(.ColumnIndex, .RowIndex, DirectCast(.Value, String))
                Case GetType(Byte())
                    EditDatabaseCell(.ColumnIndex, .RowIndex, DirectCast(.Value, Byte()))
            End Select
        End With
    End Sub
    Private Sub ApplyEditColumnEdit(columnEditToAdd As ColumnEdit)
        With columnEditToAdd
            Select Case _storedColumnTypes(.ColumnIndex)
                Case GetType(Double)
                    EditDatabaseColumn(_storedColumnNames(.ColumnIndex), Array.ConvertAll(Of Object, Double)(.ColumnData.ToArray, Function(o) DirectCast(o, Double)))
                Case GetType(Single)
                    EditDatabaseColumn(_storedColumnNames(.ColumnIndex), Array.ConvertAll(Of Object, Single)(.ColumnData.ToArray, Function(o) DirectCast(o, Single)))
                Case GetType(Long), GetType(ULong)
                    EditDatabaseColumn(_storedColumnNames(.ColumnIndex), Array.ConvertAll(Of Object, Long)(.ColumnData.ToArray, Function(o) DirectCast(o, Long)))
                Case GetType(Integer), GetType(UInteger)
                    EditDatabaseColumn(_storedColumnNames(.ColumnIndex), Array.ConvertAll(Of Object, Int32)(.ColumnData.ToArray, Function(o) DirectCast(o, Int32)))
                Case GetType(Short), GetType(UShort)
                    EditDatabaseColumn(_storedColumnNames(.ColumnIndex), Array.ConvertAll(Of Object, Short)(.ColumnData.ToArray, Function(o) DirectCast(o, Short)))
                Case GetType(Byte)
                    EditDatabaseColumn(_storedColumnNames(.ColumnIndex), Array.ConvertAll(Of Object, Byte)(.ColumnData.ToArray, Function(o) DirectCast(o, Byte)))
                Case GetType(Boolean)
                    EditDatabaseColumn(_storedColumnNames(.ColumnIndex), Array.ConvertAll(Of Object, Boolean)(.ColumnData.ToArray, Function(o) DirectCast(o, Boolean)))
                Case GetType(Byte())
                    EditDatabaseColumn(_storedColumnNames(.ColumnIndex), Array.ConvertAll(Of Object, Byte())(.ColumnData.ToArray, Function(o) DirectCast(o, Byte())))
                Case GetType(String)
                    EditDatabaseColumn(_storedColumnNames(.ColumnIndex), Array.ConvertAll(Of Object, String)(.ColumnData.ToArray, Function(o) DirectCast(o, String)))
            End Select
        End With
    End Sub
    'Private Sub ApplyAddColumnEdit(Of T)(columnEdits As AddColumnEdit(Of T))
    '    With columnEdits
    '        Select Case .ColumnDataType
    '            Case GetType(Double)
    '                AddColumnToDatabase(.ColumnName, Array.ConvertAll(Of Object, Double)(.OriginalColumnEdit, Function(o) DirectCast(o, Double)))
    '            Case GetType(Single)
    '                AddColumnToDatabase(.ColumnName, Array.ConvertAll(Of Object, Single)(.OriginalColumnEdit, Function(o) DirectCast(o, Single)))
    '            Case GetType(Long), GetType(ULong)
    '                AddColumnToDatabase(.ColumnName, Array.ConvertAll(Of Object, Long)(.OriginalColumnEdit, Function(o) DirectCast(o, Long)))
    '            Case GetType(Integer), GetType(UInteger)
    '                AddColumnToDatabase(.ColumnName, Array.ConvertAll(Of Object, Int32)(.OriginalColumnEdit, Function(o) DirectCast(o, Int32)))
    '            Case GetType(Short), GetType(UShort)
    '                AddColumnToDatabase(.ColumnName, Array.ConvertAll(Of Object, Short)(.OriginalColumnEdit, Function(o) DirectCast(o, Short)))
    '            Case GetType(Byte)
    '                AddColumnToDatabase(.ColumnName, Array.ConvertAll(Of Object, Byte)(.OriginalColumnEdit, Function(o) DirectCast(o, Byte)))
    '            Case GetType(Boolean)
    '                AddColumnToDatabase(.ColumnName, Array.ConvertAll(Of Object, Boolean)(.OriginalColumnEdit, Function(o) DirectCast(o, Boolean)))
    '            Case GetType(String)
    '                AddColumnToDatabase(.ColumnName, Array.ConvertAll(Of Object, String)(.OriginalColumnEdit, Function(o) DirectCast(o, String)))
    '        End Select
    '    End With
    'End Sub

    Public Function CanUndo() As Boolean
        Return _editIndex >= 0
    End Function
    Public Function CanRedo() As Boolean
        Return _editIndex < _edits.Count - 1
    End Function
    Public Sub EditCell(rowIndex As Int32, columnIndex As Int32, cellEdit As Object)
        If ConvertToColumnType(_columnTypes(columnIndex), cellEdit) = False Then Throw New Exception("Cell edit of type '" & cellEdit.GetType.ToString & "' is not a valid type for column '" & _columnNames(columnIndex) & "' which is of type '" & _columnTypes(columnIndex).ToString & "'.")
        AddEdit(New CellEdit(rowIndex, columnIndex, cellEdit))
    End Sub
    Public Sub EditCell(rowIndex As Int32, columnName As String, cellEdit As Object)
        Dim columnIndex = Array.IndexOf(_columnNames, columnName)
        EditCell(rowIndex, columnIndex, cellEdit)
    End Sub
    Public Sub EditCells(rowIndices() As Int32, columnIndices() As Int32, cellEdits() As Object)
        If rowIndices.Count <> columnIndices.Count Then Exit Sub
        If columnIndices.Count <> cellEdits.Count Then Exit Sub
        Dim cellEditSet(rowIndices.Count - 1) As CellEdit
        For i As Int32 = 0 To rowIndices.Count - 1
            If ConvertToColumnType(_columnTypes(columnIndices(i)), cellEdits(i)) = False Then Throw New Exception("Cell edit of type '" & cellEdits(i).GetType.ToString & "' is not a valid type for column '" & _columnNames(columnIndices(i)) & "' which is of type '" & _columnTypes(columnIndices(i)).ToString & "'. Error occurred at row index " & rowIndices(i) & ".")
            cellEditSet(i) = New CellEdit(rowIndices(i), columnIndices(i), cellEdits(i))
        Next
        '
        AddEdit(New MultiCellEdit(cellEditSet))
    End Sub
    Public Sub EditRow(rowIndex As Int32, rowData() As Object)
        If rowData.Count <> _columnNames.Count Then Throw New Exception("Number of columns in row edit do not match the number of columns in the current view.")
        For i As Int32 = 0 To rowData.Count - 1
            If ConvertToColumnType(_columnTypes(i), rowData(i)) = False Then Throw New Exception("Row edit of type '" & rowData(i).GetType.ToString & "' is not a valid type for column '" & _columnNames(i) & "' which is of type '" & _columnTypes(i).ToString & "'.")
        Next
        '
        AddEdit(New RowEdit(rowData, rowIndex))
    End Sub
    Public Sub EditColumn(columnIndex As Int32, columnData() As Object)
        If columnData.Count = 0 Then Exit Sub
        If columnData.Count <> _nRows Then Throw New Exception("Number of records in the column to edit do not match the number of records in the current view.")
        For i As Int32 = 0 To columnData.Count - 1
            'this is a hack to make it work with byte arrays. not the best solution by any means.
            Dim b = columnData(i)
            If ConvertToColumnType(_columnTypes(columnIndex), b) = False Then Throw New Exception("Attempting to edit column '" & _columnNames(columnIndex) & "' which is of type '" & _columnTypes(columnIndex).Name & "' with invalid data of type '" & columnData(i).GetType.Name & "'. Error occurred at row index " & i & ".")
            columnData(i) = b
        Next
        '
        AddEdit(New ColumnEdit(columnIndex, columnData))
    End Sub
    Public Sub EditColumn(Of T)(columnIndex As Int32, columnData() As T)
        If columnData.Count = 0 Then Exit Sub
        If columnData.Count <> _nRows Then Throw New Exception("Number of records in the column to edit do not match the number of records in the current view.")
        'If issupportedcolumntype = False Then Throw New Exception("The following column type '" & GetType(T).ToString & "' is not currently supported for this database.")
        Dim editedColumn(columnData.Count - 1) As Object
        For i As Int32 = 0 To columnData.Count - 1
            'this is a hack to make it work with byte arrays. not the best solution by any means.
            Dim b As Object = columnData(i)
            If ConvertToColumnType(_columnTypes(columnIndex), b) = False Then Throw New Exception("Attempting to edit column '" & _columnNames(columnIndex) & "' which is of type '" & _columnTypes(columnIndex).Name & "' with invalid data of type '" & columnData(i).GetType.Name & "'. Error occurred at row index " & i & ".")
            editedColumn(i) = b
        Next
        '
        AddEdit(New ColumnEdit(columnIndex, editedColumn))
    End Sub
    Public Sub DeleteRow(rowIndex As Int32)

        If rowIndex < 0 Then Throw New Exception("Attempting to delete a row that is not in the current view.")
        If rowIndex >= _nRows Then Throw New Exception("Attempting to delete a row that is not in the current view.")

        AddEdit(New DeleteRowEdit(rowIndex))
        RowDeleteMade(rowIndex)
    End Sub
    Public Sub DeleteRows(startIndex As Int32, endIndex As Int32)
        If startIndex < 0 OrElse endIndex < 0 Then Throw New Exception("Attempting to delete a row that is not in the current view.")
        If startIndex >= _nRows OrElse endIndex >= _nRows Then Throw New Exception("Attempting to delete a row that is not in the current view.")
        If endIndex < startIndex Then Exit Sub 'Throw New Exception("Attempting to delete a row that is not in the current view.")
        '
        Dim rowsToDelete((endIndex - startIndex)) As DeleteRowEdit
        Dim rowIndices((endIndex - startIndex)) As Int32
        Dim counter As Int32 = 0
        For i As Int32 = startIndex To endIndex
            rowIndices(counter) = i
            rowsToDelete(counter) = New DeleteRowEdit(i)
            counter += 1
        Next
        AddEdit(New DeleteRowsEdit(rowsToDelete))
        RowDeletesMade(rowIndices)
    End Sub
    Public Sub DeleteRows(rowIndices() As Int32)
        If IsNothing(rowIndices) Then Exit Sub
        If rowIndices.Count = 0 Then
            Exit Sub
        ElseIf rowIndices.Count = 1 Then
            DeleteRow(rowIndices(0))
        Else
            Array.Sort(rowIndices)
            If rowIndices(0) < 0 Then Throw New Exception("Attempting to delete a row that is not in the current view.")
            If rowIndices(rowIndices.Count - 1) >= _nRows Then Throw New Exception("Attempting to delete a row that is not in the current view.")
            Dim rowsToDelete(rowIndices.Count - 1) As DeleteRowEdit
            For i As Int32 = 0 To rowIndices.Count - 1
                rowsToDelete(i) = New DeleteRowEdit(rowIndices(i))
            Next

            AddEdit(New DeleteRowsEdit(rowsToDelete))
            RowDeletesMade(rowIndices)
        End If
    End Sub
    Public Sub AddRow()
        Dim dummyRow(_columnNames.Count - 1) As Object
        For i As Int32 = 0 To _columnNames.Count - 1
            'If _columnTypes(i) = GetType(Byte()) Then 'this is a hack. need to handle integer primary keys better. {CByte(0)}
            dummyRow(i) = DBNull.Value
            'ElseIf _columnTypes(i) = GetType(Integer) Then
            'dummyRow(i) = DBNull.Value 'this is a hack. need to handle integer primary keys better.
            'Else
            'dummyRow(i) = DBNull.Value 'this is a hack. need to handle integer primary keys better."0"
            'End If
        Next
        AddRow(dummyRow)
    End Sub
    Public Sub AddRow(rowData() As Object)
        If IsNothing(rowData) Then Exit Sub
        If rowData.Count = 0 Then Exit Sub
        If rowData.Count <> _columnNames.Count Then Throw New Exception("Number of columns in a row to be added do not match the number of columns in the current view.")
        For i As Int32 = 0 To rowData.Count - 1
            If ConvertToColumnType(_columnTypes(i), rowData(i)) = False Then Throw New Exception("Row edit of type '" & rowData(i).GetType.ToString & "' is not a valid type for column '" & _columnNames(i) & "' which is of type '" & _columnTypes(i).ToString & "'.")
        Next
        'Add it to the edit stack
        AddEdit(New AddRowEdit(rowData, _nRows))
        RowAddMade(_nRows, True, rowData)
    End Sub
    Public Sub AddRows(rowData As List(Of Object()))
        If IsNothing(rowData) Then Exit Sub
        If rowData.Count = 0 Then Exit Sub
        For i As Int32 = 0 To rowData.Count - 1
            If rowData(i).Count <> _columnNames.Count Then Throw New Exception("Number of columns in a row to be added do not match the number of columns in the current view.")
            For j As Int32 = 0 To rowData(i).Count - 1
                If ConvertToColumnType(_columnTypes(j), rowData(i)(j)) = False Then Throw New Exception("Row edit of type '" & rowData(i)(j).GetType.ToString & "' is not a valid type for column '" & _columnNames(j) & "' which is of type '" & _columnTypes(j).ToString & "'.")
            Next
        Next
        'Add it to the edit stack
        Dim rowsToAdd(rowData.Count - 1) As AddRowEdit
        Dim rowIndices(rowData.Count - 1) As Int32
        For i As Int32 = 0 To rowData.Count - 1
            rowIndices(i) = _nRows + i
            rowsToAdd(i) = New AddRowEdit(rowData(i), rowIndices(i))
        Next

        AddEdit(New AddRowsEdit(rowsToAdd))
        RowAddsMade(rowIndices, rowData)
    End Sub
    Public Sub AddRows(newRowData As DataTable)
        If IsNothing(newRowData) Then Exit Sub
        If newRowData.Rows.Count = 0 Then Exit Sub
        'For i As Int32 = 0 To newRowData.Rows.Count - 1
        If newRowData.Columns.Count <> _columnNames.Count Then Throw New Exception("Number of columns in a row to be added do not match the number of columns in the current view.")
        For j As Int32 = 0 To newRowData.Columns.Count - 1
            If ConvertToColumnType(_columnTypes(j), newRowData.Rows(0)(j)) = False Then Throw New Exception("Row edit of type '" & newRowData.Columns(j).GetType.ToString & "' is not a valid type for column '" & _columnNames(j) & "' which is of type '" & _columnTypes(j).ToString & "'.")
        Next
        'Next
        'Add it to the edit stack
        Dim rowsToAdd(newRowData.Rows.Count - 1) As AddRowEdit
        Dim rowIndices(newRowData.Rows.Count - 1) As Int32
        Dim rowData As New List(Of Object())
        For i As Int32 = 0 To newRowData.Rows.Count - 1
            rowIndices(i) = _nRows + i
            rowData.Add(newRowData.Rows(i).ItemArray)
            rowsToAdd(i) = New AddRowEdit(newRowData.Rows(i).ItemArray, rowIndices(i))
        Next

        AddEdit(New AddRowsEdit(rowsToAdd))
        RowAddsMade(rowIndices, rowData)
    End Sub
    Public Sub DeleteColumn(columnIndex As Int32)
        If columnIndex < 0 Then Exit Sub
        If columnIndex >= _columnNames.Count Then Throw New Exception("Attempting to delete a column that is not in the current view.")
        '
        AddEdit(New DeleteColumnEdit(columnIndex))
        ColumnDeleteMade(columnIndex)
    End Sub
    ''' <summary>
    ''' This method is not fully tested. Use at own risk.
    ''' </summary>
    ''' <param name="columnIndex">Index of the column to hide.</param>
    Public Sub HideColumn(columnIndex As Int32)
        If columnIndex < 0 Then Exit Sub
        If columnIndex >= _columnNames.Count Then Throw New Exception("Attempting to hide a column that is not in the current view.")
        '
        _hiddenColumns.Add(_columnNames(columnIndex))
        UpdateColumnInfo()
        'AddEdit(New DeleteColumnEdit(columnIndex))
        'ColumnDeleteMade(columnIndex)
    End Sub
    ''' <summary>
    ''' This method is not fully tested. Use at own risk.
    ''' </summary>
    ''' <param name="columnName">name of the column to hide.</param>
    Public Sub HideColumn(columnName As String)
        If _columnNames.Contains(columnName) = False Then Throw New Exception("Attempting to hide a column that is not in the current view.")
        '
        _hiddenColumns.Add(columnName)
        UpdateColumnInfo()
        'AddEdit(New DeleteColumnEdit(columnIndex))
        'ColumnDeleteMade(columnIndex)
    End Sub
    Public Sub DeleteColumns(columnIndices() As Int32)
        If IsNothing(columnIndices) Then Exit Sub
        If columnIndices.Count = 0 Then Exit Sub
        Array.Sort(columnIndices)
        If columnIndices(0) < 0 Then Throw New Exception("Attempting to delete a column that is not in the current view.")
        If columnIndices(columnIndices.Count - 1) >= _columnNames.Count Then Throw New Exception("Attempting to delete a column that is not in the current view.")
        '
        Dim columnsToDelete(columnIndices.Count - 1) As DeleteColumnEdit
        For i As Int32 = 0 To columnIndices.Count - 1
            columnsToDelete(i) = New DeleteColumnEdit(columnIndices(i))
        Next
        AddEdit(New DeleteColumnsEdit(columnsToDelete))
        ColumnDeletesMade(columnIndices)
    End Sub

    ''' <summary>
    ''' Adds a blank column to the table.
    ''' </summary>
    ''' <param name="columnName">Name of the new column to add.</param>
    ''' <param name="columnType">Type of the new column to add.</param>
    Public Sub AddColumn(columnName As String, columnType As Type)
        Dim defaultValue = GetDefaultFromType(columnType)
        Dim columnData = Enumerable.Repeat(defaultValue, _nRows).ToArray()
        AddColumn(columnName, columnData, columnType)
    End Sub

    Public Sub AddColumn(Of T)(columnName As String, columnData() As T)
        If columnData.Count <> _nRows Then Throw New Exception("Number of records in the column to edit do not match the number of records in the current view.")
        'If issupportedcolumntype = False Then Throw New Exception("The following column type '" & GetType(T).ToString & "' is not currently supported for this database.")
        '
        AddEdit(New AddColumnEdit(Of T)(columnData, _columnNames.Count, columnName))
        'update column information and existing edits
        ColumnAddMade(_columnNames.Count, True, columnData)
    End Sub
    Public Sub AddColumn(columnName As String, columnData() As Object, columnDataType As Type)
        If columnData.Count <> _nRows Then Throw New Exception("Number of records in the column to edit do not match the number of records in the current view.")
        If columnData.Count > 0 Then
            For i As Int32 = 0 To columnData.Count - 1
                'If columnData(i).GetType.IsArray Then
                'If columnData(i).GetType <> GetType(Byte()) Then Throw New Exception("Column edit of type '" & columnData(i).GetType.ToString & "' is not a valid type for column '" & _storedColumnNames(columnIndex) & "' which is of type '" & _storedColumnTypes(columnIndex).ToString & "'. Error occurred at row index " & i & ".")
                'this is a hack to make it work with byte arrays. not the best solution by any means.
                Dim b = columnData(i)
                If ConvertToColumnType(columnDataType, b) = False Then Throw New Exception("column data that is added must all be of the same type (e.g. integer). Column at row '" & i + 1 & "' is of type '" & columnData(i).GetType.ToString() & "' and does not match the expected data type of type '" & columnDataType.ToString() & "'.")
                columnData(i) = b
                'Else
                'If ConvertToColumnType(_columnTypes(columnIndex), columnData(i)) = False Then Throw New Exception("Column edit of type '" & columnData(i).GetType.ToString & "' is not a valid type for column '" & _storedColumnNames(columnIndex) & "' which is of type '" & _storedColumnTypes(columnIndex).ToString & "'. Error occurred at row index " & i & ".")
                'End If
                'If columnData(i).GetType <> columnDataType Then Throw New Exception("column data that is added must all be of the same type (e.g. integer). Column at row '" & i + 1 & "' is of type '" & columnData(i).GetType.ToString() & "' and does not match the expected data type of type '" & columnDataType.ToString() & "'.")
            Next
        End If
        '
        Select Case columnDataType
            Case GetType(Double)
                AddColumn(columnName, Array.ConvertAll(columnData.ToArray, Function(o) If(IsDBNull(o), CDbl(0), CDbl(o))))
            Case GetType(Single)
                AddColumn(columnName, Array.ConvertAll(columnData.ToArray, Function(o) If(IsDBNull(o), CSng(0), CSng(o))))
            Case GetType(Long), GetType(ULong)
                AddColumn(columnName, Array.ConvertAll(columnData.ToArray, Function(o) If(IsDBNull(o), CLng(0), CLng(o))))
            Case GetType(Integer), GetType(UInteger)
                AddColumn(columnName, Array.ConvertAll(columnData.ToArray, Function(o) If(IsDBNull(o), CInt(0), CInt(o))))
            Case GetType(Short), GetType(UShort)
                AddColumn(columnName, Array.ConvertAll(columnData.ToArray, Function(o) If(IsDBNull(o), CShort(0), CShort(o))))
            Case GetType(Byte)
                AddColumn(columnName, Array.ConvertAll(columnData.ToArray, Function(o) If(IsDBNull(o), CByte(0), CByte(o))))
            Case GetType(Boolean)
                AddColumn(columnName, Array.ConvertAll(columnData.ToArray, Function(o) If(IsDBNull(o), False, CBool(o))))
            Case GetType(Byte())
                AddColumn(columnName, Array.ConvertAll(columnData.ToArray, Function(o) If(IsDBNull(o), New Byte() {}, CType(o, Byte()))))
            Case GetType(String)
                AddColumn(columnName, Array.ConvertAll(columnData.ToArray, Function(o) If(IsDBNull(o), "", CStr(o))))
        End Select

        'update column information and existing edits
        'ColumnAddMade(_columnNames.Count, True, columnData)
    End Sub
    Public Sub AddColumns(namesOfColumns() As String, columnData As List(Of Object()), columnDataTypes() As Type)
        If IsNothing(columnData) Then Exit Sub
        If IsNothing(_columnNames) Then Exit Sub
        If IsNothing(ColumnTypes) Then Exit Sub
        If columnData.Count = 0 Then Exit Sub
        If _columnNames.Count <> columnData.Count Then Throw New Exception("Number of column names and column datasets do not match for the columns to add.")
        If _columnNames.Count <> columnDataTypes.Count Then Throw New Exception("Number of column names and column types do not match for the columns to add.")
        For i As Int32 = 0 To columnData.Count - 1
            If columnData(i).Count <> _nRows Then Throw New Exception("Number of records in the column to edit do not match the number of records in the current view.")
            If columnData(i).Count > 0 Then
                For j As Int32 = 0 To columnData(i).Count - 1
                    If columnData(i)(j).GetType <> columnDataTypes(i) Then Throw New Exception("column data that is added must all be of the same type (e.g. integer). Column at row '" & j + 1 & "' is of type '" & columnData(i)(j).GetType.ToString() & "' and does not match the expected data type of type '" & columnDataTypes(i).ToString() & "'.")
                Next
            End If
        Next
        '
        Dim columnsToAdd(namesOfColumns.Count - 1) As IColumnEdit
        Dim addedIndices(namesOfColumns.Count - 1) As Int32
        For i As Int32 = 0 To namesOfColumns.Count - 1
            addedIndices(i) = _columnNames.Count + i
            Select Case columnDataTypes(i)
                Case GetType(Double)
                    columnsToAdd(i) = New AddColumnEdit(Of Double)(Array.ConvertAll(Of Object, Double)(columnData(i).ToArray, Function(o) CDbl(o)), addedIndices(i), namesOfColumns(i))
                Case GetType(Single)
                    columnsToAdd(i) = New AddColumnEdit(Of Single)(Array.ConvertAll(Of Object, Single)(columnData(i).ToArray, Function(o) CSng(o)), addedIndices(i), namesOfColumns(i))
                Case GetType(Long), GetType(ULong)
                    columnsToAdd(i) = New AddColumnEdit(Of Long)(Array.ConvertAll(Of Object, Long)(columnData(i).ToArray, Function(o) CLng(o)), addedIndices(i), namesOfColumns(i))
                Case GetType(Integer), GetType(UInteger)
                    columnsToAdd(i) = New AddColumnEdit(Of Integer)(Array.ConvertAll(Of Object, Integer)(columnData(i).ToArray, Function(o) CInt(o)), addedIndices(i), namesOfColumns(i))
                Case GetType(Short), GetType(UShort)
                    columnsToAdd(i) = New AddColumnEdit(Of Short)(Array.ConvertAll(Of Object, Short)(columnData(i).ToArray, Function(o) CShort(o)), addedIndices(i), namesOfColumns(i))
                Case GetType(Byte)
                    columnsToAdd(i) = New AddColumnEdit(Of Byte)(Array.ConvertAll(Of Object, Byte)(columnData(i).ToArray, Function(o) CByte(o)), addedIndices(i), namesOfColumns(i))
                Case GetType(Boolean)
                    columnsToAdd(i) = New AddColumnEdit(Of Boolean)(Array.ConvertAll(Of Object, Boolean)(columnData(i).ToArray, Function(o) CBool(o)), addedIndices(i), namesOfColumns(i))
                Case GetType(Byte())
                    columnsToAdd(i) = New AddColumnEdit(Of Byte())(Array.ConvertAll(Of Object, Byte())(columnData(i).ToArray, Function(o) CType(o, Byte())), addedIndices(i), namesOfColumns(i))
                Case GetType(String)
                    columnsToAdd(i) = New AddColumnEdit(Of String)(Array.ConvertAll(Of Object, String)(columnData(i).ToArray, Function(o) o.ToString), addedIndices(i), namesOfColumns(i))
            End Select
            'columnsToAdd(i) = New AddColumnEdit(columnData(i), addedIndices(i), namesOfColumns(i), columnDataTypes(i))
        Next
        '
        AddEdit(New AddColumnsEdit(columnsToAdd))
        UpdateColumnInfo()
        '
        For i As Int32 = 0 To addedIndices.Count - 1
            For j As Int32 = 0 To _editIndex - 1
                _edits(j).ColumnAdded(addedIndices(i), columnData(i))
            Next
        Next
        '
        RaiseEvent ColumnsAdded(addedIndices)
    End Sub
    Public Sub CancelEdits()
        _edits.Clear()
        _editIndex = -1
        InitializeView()
    End Sub
    Public Sub UndoEdit()
        Dim editToUndo As TableEdit = _edits(_editIndex)
        If _editIndex > -1 Then _editIndex -= 1
        Select Case editToUndo.GetType()
            Case GetType(DeleteRowEdit)
                RowAddMade(DirectCast(editToUndo, DeleteRowEdit).RowIndex, False)
            Case GetType(DeleteRowsEdit)
                Dim rowIndices() As Int32 = DirectCast(editToUndo, DeleteRowsEdit).GetDeletedRowIndices()
                Dim rowData As New List(Of Object())
                For i As Int32 = 0 To rowIndices.Count - 1
                    rowData.Add(Nothing)
                Next
                RowAddsMade(rowIndices.ToArray, rowData)
            Case GetType(AddRowEdit)
                RowDeleteMade(DirectCast(editToUndo, AddRowEdit).RowIndex)
            Case GetType(AddRowsEdit)
                Dim rowIndices As New List(Of Int32)
                For Each rowAdd As AddRowEdit In DirectCast(editToUndo, AddRowsEdit).AddRowEdits
                    rowIndices.Add(rowAdd.RowIndex)
                Next
                RowDeletesMade(rowIndices.ToArray)
            Case GetType(DeleteColumnEdit)
                ColumnAddMade(Of Boolean)(DirectCast(editToUndo, DeleteColumnEdit).ColumnIndex, True)
            Case GetType(DeleteColumnsEdit)
                Dim columnIndices As New List(Of Int32)
                Dim rowData As New List(Of Object())
                For Each columnDelete As DeleteColumnEdit In DirectCast(editToUndo, DeleteColumnsEdit).ColumnsDeleted
                    columnIndices.Add(columnDelete.ColumnIndex)
                    rowData.Add(Nothing)
                Next
                ColumnAddsMade(columnIndices.ToArray, rowData)
            Case GetType(AddColumnsEdit)
                Dim columnIndices As New List(Of Int32)
                For Each columnAdd As IColumnEdit In DirectCast(editToUndo, AddColumnsEdit).AddColumnEdits
                    columnIndices.Add(columnAdd.ColumnIndex)
                Next
                ColumnDeletesMade(columnIndices.ToArray)
        End Select

        If TypeOf editToUndo Is IColumnEdit Then
            If CType(editToUndo, IColumnEdit).IsColumnAdd = True Then ColumnDeleteMade(CType(editToUndo, IColumnEdit).ColumnIndex)
        End If


    End Sub

    Public Sub RedoEdit()
        If _editIndex < _edits.Count - 1 Then _editIndex += 1
        Dim editToRedo As TableEdit = _edits(_editIndex)
        Select Case editToRedo.GetType()
            Case GetType(DeleteRowEdit)
                RowDeleteMade(DirectCast(editToRedo, DeleteRowEdit).RowIndex)
            Case GetType(DeleteRowsEdit)
                Dim rowIndices() As Int32 = DirectCast(editToRedo, DeleteRowsEdit).GetDeletedRowIndices()
                RowDeletesMade(rowIndices.ToArray())
            Case GetType(AddRowEdit)
                RowAddMade(DirectCast(editToRedo, AddRowEdit).RowIndex, True)
            Case GetType(AddRowsEdit)
                Dim rowData As New List(Of Object())
                Dim rowIndices As New List(Of Int32)
                For Each rowAdd As AddRowEdit In DirectCast(editToRedo, AddRowsEdit).AddRowEdits
                    rowIndices.Add(rowAdd.RowIndex)
                    rowData.Add(Nothing)
                Next
                RowAddsMade(rowIndices.ToArray, rowData)
            Case GetType(DeleteColumnEdit)
                ColumnDeleteMade(DirectCast(editToRedo, DeleteColumnEdit).ColumnIndex)
            Case GetType(DeleteColumnsEdit)
                Dim columnIndices As New List(Of Int32)
                For Each columnDelete As DeleteColumnEdit In DirectCast(editToRedo, DeleteColumnsEdit).ColumnsDeleted
                    columnIndices.Add(columnDelete.ColumnIndex)
                Next
                ColumnDeletesMade(columnIndices.ToArray)
            'Case GetType(AddColumnEdit)
            'ColumnAddMade(DirectCast(editToRedo, AddColumnEdit).ColumnIndex, True)
            Case GetType(AddColumnsEdit)
                Dim rowData As New List(Of Object())
                Dim columnIndices As New List(Of Int32)
                For Each columnAdd As IColumnEdit In DirectCast(editToRedo, AddColumnsEdit).AddColumnEdits
                    columnIndices.Add(columnAdd.ColumnIndex)
                    rowData.Add(Nothing)
                Next
                ColumnAddsMade(columnIndices.ToArray, rowData)
        End Select

        If TypeOf editToRedo Is IColumnEdit Then
            If CType(editToRedo, IColumnEdit).IsColumnAdd = True Then ColumnAddMade(Of Boolean)(CType(editToRedo, IColumnEdit).ColumnIndex, True)
        End If
    End Sub
    Private Sub AddEdit(newEdit As TableEdit)
        _editIndex += 1
        If _editIndex < _edits.Count Then
            _edits.RemoveRange(_editIndex, _edits.Count - _editIndex)
            _edits.Add(newEdit)
        Else
            _edits.Add(newEdit)
        End If
        '
        RaiseEvent EditAdded(newEdit)
    End Sub
    Private Sub RowDeleteMade(rowIndex As Int32)
        _nRows -= 1
        '
        UpdateRowInfo()
        '
        For i As Int32 = 0 To _editIndex
            _edits(i).RowDeleted(rowIndex)
        Next
        '
        RaiseEvent RowsDeleted({rowIndex})
    End Sub
    Private Sub RowDeletesMade(rowIndices() As Int32)
        _nRows -= rowIndices.Count
        '
        UpdateRowInfo()
        '
        For i As Int32 = 0 To _editIndex
            'For some reason I am not updating the row indices for the delete/add row edits. I need to test if not updating row indices is ok.
            'This can really get slow when there are a lot of rows getting deleted (opportunity to improve).
            If _edits(i).GetType = GetType(DeleteRowEdit) Or _edits(i).GetType = GetType(DeleteRowsEdit) Then Continue For
            For j As Int32 = rowIndices.Count - 1 To 0 Step -1
                _edits(i).RowDeleted(rowIndices(j))
            Next
        Next
        '
        RaiseEvent RowsDeleted(rowIndices)
    End Sub
    Private Sub RowAddMade(rowIndex As Int32, ignoreLastEdit As Boolean, Optional ByVal rowData() As Object = Nothing)
        _nRows += 1
        '
        UpdateRowInfo()
        '
        If ignoreLastEdit Then
            For i As Int32 = 0 To _editIndex - 1
                _edits(i).RowAdded(rowIndex, rowData)
            Next
        Else
            For i As Int32 = 0 To _editIndex
                _edits(i).RowAdded(rowIndex, rowData)
            Next
        End If
        '
        RaiseEvent RowsAdded({rowIndex})
    End Sub
    Private Sub RowAddsMade(rowIndices() As Int32, rowData As List(Of Object()))
        _nRows += rowIndices.Count
        '
        UpdateRowInfo()
        '
        For i As Int32 = 0 To _editIndex
            'For some reason I am not updating the row indices for the delete/add row edits. I need to test if not updating row indices is ok. 
            'This can really get slow when there are a lot of rows getting added (opportunity to improve).
            If _edits(i).GetType = GetType(DeleteRowEdit) Or _edits(i).GetType = GetType(DeleteRowsEdit) Then Continue For
            For j As Int32 = 0 To rowIndices.Count - 1
                _edits(i).RowAdded(rowIndices(j), rowData(j))
            Next
        Next
        '
        RaiseEvent RowsAdded(rowIndices)
    End Sub
    Private Sub ColumnDeleteMade(columnIndex As Int32)
        UpdateColumnInfo()
        '
        For i As Int32 = 0 To _editIndex
            _edits(i).ColumnDeleted(columnIndex)
        Next
        '
        RaiseEvent ColumnsDeleted({columnIndex})
    End Sub
    Private Sub ColumnDeletesMade(columnIndices() As Int32)
        UpdateColumnInfo()
        '
        For i As Int32 = 0 To columnIndices.Count - 1
            For j As Int32 = 0 To _editIndex
                _edits(j).ColumnDeleted(columnIndices(i))
            Next
        Next
        '
        RaiseEvent ColumnsDeleted(columnIndices)
    End Sub
    Private Sub ColumnAddMade(Of T)(columnIndex As Int32, ignoreLastEdit As Boolean, Optional ByVal columnData() As T = Nothing)
        UpdateColumnInfo()
        '
        If ignoreLastEdit = True Then
            For i As Int32 = 0 To _editIndex - 1
                _edits(i).ColumnAdded(columnIndex, columnData)
            Next
        Else
            For i As Int32 = 0 To _editIndex
                _edits(i).ColumnAdded(columnIndex, columnData)
            Next
        End If
        '
        RaiseEvent ColumnsAdded({columnIndex})
    End Sub
    Private Sub ColumnAddsMade(columnIndices() As Int32, columnData As List(Of Object()))
        UpdateColumnInfo()
        '
        For i As Int32 = 0 To columnIndices.Count - 1
            For j As Int32 = 0 To _editIndex
                _edits(j).ColumnAdded(columnIndices(i), columnData(i))
            Next
        Next
        '
        RaiseEvent ColumnsAdded(columnIndices)
    End Sub
    Private Sub UpdateRowInfo()
        Dim edit As TableEdit
        Dim newStored As New List(Of Int32)
        For i As Int32 = 0 To CInt(_storedNumberOfRows) - 1
            newStored.Add(i)
        Next
        For i As Int32 = 0 To _editIndex
            edit = _edits(i)
            Select Case edit.GetType()
                Case GetType(AddRowEdit)
                    newStored.Add(-1)
                Case GetType(AddRowsEdit)
                    For j As Int32 = 0 To DirectCast(edit, AddRowsEdit).AddRowEdits.Count - 1
                        newStored.Add(-1)
                    Next
                Case GetType(DeleteRowEdit)
                    newStored.RemoveAt(DirectCast(edit, DeleteRowEdit).RowIndex)
                Case GetType(DeleteRowsEdit)
                    With DirectCast(edit, DeleteRowsEdit)
                        Dim rowsdeleted(.DeleteRowEdits.Count - 1) As Int32
                        For j As Int32 = 0 To .DeleteRowEdits.Count - 1
                            rowsdeleted(j) = .DeleteRowEdits(j).RowIndex
                        Next
                        Array.Sort(rowsdeleted)
                        For j As Int32 = rowsdeleted.Count - 1 To 0 Step -1
                            newStored.RemoveAt(rowsdeleted(j))
                        Next
                    End With
            End Select
        Next
        '
        _viewToStoredRowIndex = newStored.ToArray()
    End Sub
    Private Sub UpdateColumnInfo()
        Dim newColumnNames As List(Of String) = GetStoredColumnNames().ToList()
        Dim newColumnTypes As List(Of Type) = GetStoredColumnTypes().ToList()
        Dim edit As TableEdit
        Dim newStored As New List(Of Int32)
        For i As Int32 = 0 To newColumnNames.Count - 1
            newStored.Add(i)
        Next
        'hidden columns
        For i As Int32 = newColumnNames.Count - 1 To 0 Step -1
            If _hiddenColumns.Contains(newColumnNames(i)) Then
                newColumnNames.RemoveAt(i)
                newColumnTypes.RemoveAt(i)
                newStored.RemoveAt(i)
            End If
        Next
        'edits
        For i As Int32 = 0 To _editIndex
            edit = _edits(i)
            If TypeOf edit Is IColumnEdit Then
                If CType(edit, IColumnEdit).IsColumnAdd = True Then
                    newColumnNames.Add(CType(edit, IColumnEdit).ColumnName)
                    newColumnTypes.Add(CType(edit, IColumnEdit).ColumnDataType)
                    newStored.Add(-1)
                End If
            End If
            '
            Select Case edit.GetType()
                'Case GetType(AddColumnEdit)
                '    newColumnNames.Add(DirectCast(edit, AddColumnEdit).ColumnName)
                '    newColumnTypes.Add(DirectCast(edit, AddColumnEdit).ColumnDataType)
                '    newStored.Add(-1)
                Case GetType(AddColumnsEdit)
                    For Each editColumn As IColumnEdit In DirectCast(edit, AddColumnsEdit).AddColumnEdits
                        newColumnNames.Add(editColumn.ColumnName)
                        newColumnTypes.Add(editColumn.ColumnDataType)
                        newStored.Add(-1)
                    Next
                Case GetType(DeleteColumnEdit)
                    newColumnNames.RemoveAt(DirectCast(edit, DeleteColumnEdit).ColumnIndex)
                    newColumnTypes.RemoveAt(DirectCast(edit, DeleteColumnEdit).ColumnIndex)
                    newStored.RemoveAt(DirectCast(edit, DeleteColumnEdit).ColumnIndex)
                Case GetType(DeleteColumnsEdit)
                    With DirectCast(edit, DeleteColumnsEdit)
                        For j As Int32 = .ColumnsDeleted.Count - 1 To 0 Step -1
                            newColumnNames.RemoveAt(.ColumnsDeleted(j).ColumnIndex)
                            newColumnTypes.RemoveAt(.ColumnsDeleted(j).ColumnIndex)
                            newStored.RemoveAt(.ColumnsDeleted(j).ColumnIndex)
                        Next
                    End With
            End Select
        Next
        '
        '
        _viewToStoredColumnIndex = newStored.ToArray()
        _columnNames = newColumnNames.ToArray()
        _columnTypes = newColumnTypes.ToArray()
    End Sub
#End Region

#Region "Get view data"
    Public Function GetCell(columnIndex As Int32, rowIndex As Int32) As Object
        If _editIndex < 0 Then Return GetStoredCell(columnIndex, rowIndex)
        Dim result As Object = Nothing
        For i As Int32 = _editIndex To 0 Step -1
            If _edits(i).ContainsCell(columnIndex, rowIndex, result) Then Return result
        Next
        '
        Return GetStoredCell(_viewToStoredColumnIndex(columnIndex), _viewToStoredRowIndex(rowIndex))
    End Function
    Public Function GetCell(columnName As String, rowIndex As Int32) As Object
        If _editIndex < 0 Then Return GetStoredCell(columnName, rowIndex)
        Return GetCell(Array.IndexOf(_columnNames, columnName), rowIndex)
    End Function
    Public Function GetCells(columnIndices() As Int32, rowIndices() As Int32) As Object()
        If _editIndex < 0 Then Return GetStoredCells(columnIndices, rowIndices)
        Dim result(columnIndices.Count - 1) As Object
        For i As Int32 = 0 To columnIndices.Count - 1
            result(i) = GetCell(columnIndices(i), rowIndices(i))
        Next
        Return result
    End Function
    Public Function GetRow(rowIndex As Int32) As Object()
        If _editIndex < 0 Then Return GetStoredRow(rowIndex)
        'Get the stored data
        Dim result(_columnNames.Count - 1) As Object
        If _viewToStoredRowIndex(rowIndex) < _storedNumberOfRows And _viewToStoredRowIndex(rowIndex) >= 0 Then
            Dim storedrow() As Object = GetStoredRow(_viewToStoredRowIndex(rowIndex))
            For i As Int32 = 0 To _viewToStoredColumnIndex.Count - 1
                If _viewToStoredColumnIndex(i) < _storedColumnNames.Count And _viewToStoredColumnIndex(i) >= 0 Then
                    result(i) = storedrow(_viewToStoredColumnIndex(i))
                End If
            Next
        End If
        'update with edits made
        For i As Int32 = 0 To _editIndex
            For Each edit As CellEdit In _edits(i).GetEditedCellsInRow(rowIndex)
                result(edit.ColumnIndex) = edit.Value
            Next
        Next
        '
        Return result
    End Function
    Public Function GetRow(rowIndex As Int32, columnIndices() As Int32) As Object()
        If _editIndex < 0 Then Return GetStoredRow(rowIndex, columnIndices)
        '
        Dim allColumnsResult() As Object = GetRow(rowIndex)
        Dim result(columnIndices.Count - 1) As Object
        For i As Int32 = 0 To columnIndices.Count - 1
            result(i) = allColumnsResult(columnIndices(i))
        Next
        Return result
    End Function
    Public Function GetRow(rowIndex As Int32, columns() As String) As Object()
        If _editIndex < 0 Then Return GetStoredRow(rowIndex, columns)
        '
        Dim columnIndices(columns.Count - 1) As Int32
        For i As Int32 = 0 To columns.Count - 1
            columnIndices(i) = Array.IndexOf(_columnNames, columns(i))
        Next
        Return GetRow(rowIndex, columnIndices)
    End Function
    Public Function GetRows(startRowIndex As Int32, endRowIndex As Int32) As List(Of Object())
        If _editIndex < 0 Then Return GetStoredRows(startRowIndex, endRowIndex)
        '
        Dim result As New List(Of Object())
        For i As Int32 = startRowIndex To endRowIndex
            result.Add(GetRow(i))
        Next
        Return result
    End Function
    ''' <summary>
    '''     returns a specified field of data from the data table as an array of the column type.
    ''' </summary>
    ''' <param name="columnIndex">Column index of the desired data.</param>
    ''' <returns>A one-dimensional array of the column type.</returns>
    ''' <remarks></remarks>
    Public Function GetColumn(columnIndex As Int32) As Object()
        If _editIndex < 0 Then Return GetStoredColumn(columnIndex)
        Dim result(_nRows - 1) As Object
        If _viewToStoredColumnIndex(columnIndex) >= 0 And _viewToStoredColumnIndex(columnIndex) < _storedColumnNames.Count Then
            Dim storedcolumn() As Object = GetStoredColumn(_viewToStoredColumnIndex(columnIndex))
            For i As Int32 = 0 To _viewToStoredRowIndex.Count - 1
                If _viewToStoredRowIndex(i) >= 0 And _viewToStoredRowIndex(i) < _storedNumberOfRows Then
                    result(i) = storedcolumn(_viewToStoredRowIndex(i))
                End If
            Next
        End If
        '
        For i As Int32 = 0 To _editIndex
            For Each edit As CellEdit In _edits(i).GetEditedCellsInColumn(columnIndex)
                result(edit.RowIndex) = edit.Value
            Next
        Next
        '
        Return result
    End Function

    ''' <summary>
    '''     returns a specified field of data from the data table as an array of the column type.
    ''' </summary>
    ''' <param name="columnName">Name of the desired column of data.</param>
    ''' <returns>A one-dimensional array of the column type.</returns>
    ''' <remarks></remarks>
    Public Function GetColumn(columnName As String) As Object()
        If _editIndex < 0 Then Return GetStoredColumn(columnName)
        Return GetColumn(Array.IndexOf(_columnNames, columnName))
    End Function

#End Region

#Region "Get data from the stored database"

    Protected MustOverride Function GetStoredRowCount() As ULong
    Protected MustOverride Function GetStoredColumnNames() As String()
    Protected MustOverride Function GetStoredColumnTypes() As Type()

    Protected MustOverride Function GetStoredCell(storedColumnIndex As Int32, storedRowIndex As Int32) As Object
    Protected MustOverride Function GetStoredCell(storedColumnName As String, storedRowIndex As Int32) As Object
    Protected MustOverride Function GetStoredCells(storedColumnIndices() As Int32, storedRowIndices() As Int32) As Object()
    Protected MustOverride Function GetStoredRow(storedRowIndex As Int32) As Object()
    Protected MustOverride Function GetStoredRow(storedRowIndex As Int32, storedColumnIndices() As Int32) As Object()
    Protected MustOverride Function GetStoredRow(storedRowIndex As Int32, storedColumns() As String) As Object()
    Protected MustOverride Function GetStoredRows(startStoredRowIndex As Int32, endStoredRowIndex As Int32) As List(Of Object())
    ''' <summary>
    '''     returns a specified field of data from the data table as an array of the column type.
    ''' </summary>
    ''' <param name="storedColumnIndex">Column index of the desired data.</param>
    ''' <returns>A one-dimensional array of the column type.</returns>
    ''' <remarks>Written 9/8/2012 by Woodrow Lee Fields.</remarks>
    Protected MustOverride Function GetStoredColumn(storedColumnIndex As Int32) As Object()

    ''' <summary>
    '''     returns a specified field of data from the data table as an array of the column type.
    ''' </summary>
    ''' <param name="storedColumnName">Name of the desired column of data.</param>
    ''' <returns>A one-dimensional array of the column type.</returns>
    ''' <remarks>Written 9/8/2012 by Woodrow Lee Fields.</remarks>
    Protected MustOverride Function GetStoredColumn(storedColumnName As String) As Object()
#End Region

#Region "Add data to stored database"
    'Protected MustOverride Sub AddColumnToDatabase(columnName as String, columnData() As Object)
    Protected MustOverride Sub AddColumnToDatabase(columnName As String, columnData()() As Byte)
    Protected MustOverride Sub AddColumnToDatabase(columnName As String, columnData() As Byte)
    Protected MustOverride Sub AddColumnToDatabase(columnName As String, columnData() As Int16)
    Protected MustOverride Sub AddColumnToDatabase(columnName As String, columnData() As Int32)
    Protected MustOverride Sub AddColumnToDatabase(columnName As String, columnData() As Int64)
    Protected MustOverride Sub AddColumnToDatabase(columnName As String, columnData() As Single)
    Protected MustOverride Sub AddColumnToDatabase(columnName As String, columnData() As Double)
    Protected MustOverride Sub AddColumnToDatabase(columnName As String, columnData() As String)
    Protected MustOverride Sub AddColumnToDatabase(columnName As String, columnData() As Boolean)
    Protected MustOverride Sub AddRowToDatabase(row() As Object)
    Protected MustOverride Sub AddRowToDatabase()
    Protected MustOverride Sub AddRowsToDatabase(newRowData As List(Of Object()))
    'Protected MustOverride Sub AddRowsToDatabase(newRowData()() As Object)
    Protected Sub AddRowsToDatabase(newRowData As DataTable)
        Dim rowsAdded As New List(Of Object())(newRowData.Rows.Count)
        For i = 0 To newRowData.Rows.Count - 1
            rowsAdded.Add(newRowData.Rows(i).ItemArray)
        Next
        AddRowsToDatabase(rowsAdded)
    End Sub
#End Region

#Region "Delete data from stored database"
    Protected MustOverride Sub DeleteColumnFromDatabase(columnName As String)
    Protected MustOverride Sub DeleteColumnsFromDatabase(columnsToDelete() As String)
    '
    Protected MustOverride Sub DeleteRowFromDatabase(rowIndex As Int32)
    Protected MustOverride Sub DeleteRowsFromDatabase(rowIndices() As Int32)
#End Region

#Region "Edit data in stored database"
    'Protected MustOverride Sub EditDatabaseColumn(columnName As String, columnData() As Object)
    Protected MustOverride Sub EditDatabaseColumn(columnName As String, columnData() As Byte)
    Protected MustOverride Sub EditDatabaseColumn(columnName As String, columnData() As Int16)
    Protected MustOverride Sub EditDatabaseColumn(columnName As String, columnData() As Int32)
    Protected MustOverride Sub EditDatabaseColumn(columnName As String, columnData() As Int64)
    Protected MustOverride Sub EditDatabaseColumn(columnName As String, columnData() As Single)
    Protected MustOverride Sub EditDatabaseColumn(columnName As String, columnData() As Double)
    Protected MustOverride Sub EditDatabaseColumn(columnName As String, columnData() As String)
    Protected MustOverride Sub EditDatabaseColumn(columnName As String, columnData() As Boolean)
    Protected MustOverride Sub EditDatabaseColumn(columnName As String, columnData()() As Byte)

    '
    'Protected MustOverride Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Object)
    Protected MustOverride Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Byte)
    Protected MustOverride Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Int16)
    Protected MustOverride Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Int32)
    Protected MustOverride Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Int64)
    Protected MustOverride Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Single)
    Protected MustOverride Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Double)
    Protected MustOverride Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As String)
    Protected MustOverride Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Boolean)
    Protected MustOverride Sub EditDatabaseCell(columnindex As Int32, rowindex As Int32, cellvalue As Byte())
    Protected MustOverride Sub EditDatabaseCells(columnIndices() As Int32, rowIndices() As Int32, cellValues() As Object)
    Protected MustOverride Sub EditDatabaseCells(columnNamesToEdit() As String, rowIndices() As Int32, cellValues() As Object)
#End Region

    Public Function SearchColumn(startIndex As Int32, endIndex As Int32, columnName As String, searchValue As String, matchCase As Boolean, wholeWord As Boolean) As Integer
        If _columnNames.Contains(columnName) = False Then Return -1
        Dim loopStep = 1, cellValue As String
        If matchCase = False Then searchValue = searchValue.ToLower
        If startIndex > endIndex Then loopStep = -1
        Dim columnArray() As Object = GetColumn(columnName)
        If columnArray.Count = 0 Then Return -1
        For i As Int32 = startIndex To endIndex Step loopStep
            cellValue = columnArray(i).ToString()
            If matchCase = False Then cellValue = cellValue.ToLower
            If wholeWord = False Then
                If cellValue.Contains(searchValue) Then Return i
            Else
                If searchValue = cellValue Then Return i
            End If
        Next
        Return -1
    End Function

    Public Function SearchColumn(startIndex As Int32, endIndex As Int32, columnIndex As Int32, searchValue As String, matchCase As Boolean, wholeWord As Boolean) As Integer
        If columnIndex < 0 Or columnIndex >= _columnNames.Count Then Return -1
        Return SearchColumn(startIndex, endIndex, _columnNames(columnIndex), searchValue, matchCase, wholeWord)
    End Function

    Public Function GetNumericColumns() As List(Of String)
        Dim numericColumns As New List(Of String)
        For i = 0 To _columnNames.Count - 1
            If DatabaseManager.IsNumericType(_columnTypes(i)) Then
                numericColumns.Add(_columnNames(i))
            End If
        Next
        Return numericColumns
    End Function

    Public Sub ExportToSqlite(filePath As String, tableName As String, Optional rowIndicesToExport() As Int32 = Nothing, Optional ByVal columnIndicesToExport() As Int32 = Nothing)
        If File.Exists(filePath) = False Then SQLiteManager.CreateSqLiteFile(filePath)
        Dim sqliteDatabase As New SQLiteManager(filePath)
        If sqliteDatabase.DataBaseOpen = False Then sqliteDatabase.Open()
        Dim dt As DataTable = ExportToDataTable(rowIndicesToExport, columnIndicesToExport)
        dt.TableName = tableName
        sqliteDatabase.SaveDataTable(dt)
        sqliteDatabase.Close()
    End Sub

    ''' <summary>
    '''     Exports a data table to a comma delimited text file.
    ''' </summary>
    ''' <param name="filePath">Output file path</param>
    ''' <param name="rowIndicesToExport">Which rows to export as row indices</param>
    ''' <param name="columnIndicesToExport">Which columns to export as column indices</param>
    ''' <remarks></remarks>
    Public Sub ExportToCsv(filePath As String, Optional ByVal rowIndicesToExport() As Int32 = Nothing, Optional ByVal columnIndicesToExport() As Int32 = Nothing)
        If Path.GetExtension(filePath) <> ".csv" Then _
            Throw New Exception("Supplied file path is not a comma delimited file (.csv).")
        '
        If IsNothing(columnIndicesToExport) Then
            ReDim columnIndicesToExport(_columnNames.Count - 1)
            For i = 0 To _columnNames.Count - 1
                columnIndicesToExport(i) = i
            Next
        End If
        If IsNothing(rowIndicesToExport) Then
            ReDim rowIndicesToExport(_nRows - 1)
            For i = 0 To _nRows - 1
                rowIndicesToExport(i) = i
            Next
        End If
        Dim csvWriter As New StreamWriter(filePath)
        csvWriter.Write(_columnNames(columnIndicesToExport(0)))
        For i = 1 To columnIndicesToExport.Count - 1
            csvWriter.Write("," & _columnNames(columnIndicesToExport(i)))
        Next
        csvWriter.WriteLine()
        Dim row() As Object
        For i = 0 To rowIndicesToExport.Count - 1
            row = GetRow(rowIndicesToExport(i), columnIndicesToExport)
            csvWriter.Write(row(0).ToString)
            For j = 1 To row.Count - 1
                csvWriter.Write("," & row(j).ToString)
            Next
            csvWriter.WriteLine()
        Next
        csvWriter.Close()
        csvWriter.Dispose()
        '
    End Sub

    ''' <summary>
    '''     Exports the contents of a DataTable to an excel spreadsheet.
    '''     FilePath must have an .xlsx or .xls extension.
    '''     The name of the excel worksheet will be the name of the supplied DataTable
    ''' </summary>
    Public Sub ExportToXlsx(filePath As String, Optional ByVal rowIndicesToExport() As Int32 = Nothing, Optional ByVal columnIndicesToExport() As Int32 = Nothing)
        'This sub  will create a new excel worksheet and write the contents of the datatable to it.
        'If the FilePath given does not point to an already existing file, a new excel file will be created.
        'The name of the worksheet will be the "Name" attribute of the provided datatable.

        If Path.GetExtension(filePath) <> ".xlsx" Then Throw New Exception("Supplied file path is not an excel spreadsheet file (.xlsx).")

        'if the file exists, check to see if the spreadsheet is open
        If File.Exists(filePath) Then
            Using wb As New Excel.XLWorkbook(filePath)
                'make sure table name is unique.
                Dim nameOfTable As String = String.Copy(_tableName)
                Dim counter As Int32 = 1
                Do
                    If wb.Worksheets.Any(Function(o) o.Name = nameOfTable) Then
                        nameOfTable = nameOfTable & "_" & counter
                        counter += 1
                    Else
                        Exit Do
                    End If
                Loop
                '
                Dim ws = wb.Worksheets.Add(nameOfTable)
                Dim headerRange = ws.Cell(1, 1).InsertData(_columnNames, True)
                Dim dataRange = ws.Cell(2, 1).InsertData(ExportToDataTable(rowIndicesToExport, columnIndicesToExport))
                headerRange.Style.Font.Bold = True
                headerRange.Style.Font.FontColor = Excel.XLColor.Black
                headerRange.Style.Fill.BackgroundColor = Excel.XLColor.FromArgb(225, 240, 250)
                headerRange.Style.Alignment.Horizontal = Excel.XLAlignmentHorizontalValues.Left
                headerRange.Style.Border.BottomBorder = Excel.XLBorderStyleValues.Medium
                headerRange.Style.Border.BottomBorderColor = Excel.XLColor.FromArgb(125, 140, 150)
                headerRange.SetAutoFilter(True)
                ws.Columns(1, _columnNames.Count).AdjustToContents()
                '
                wb.SaveAs(filePath)
            End Using
        Else
            Using wb As New Excel.XLWorkbook()
                Dim ws = wb.Worksheets.Add(_tableName)
                Dim headerRange = ws.Cell(1, 1).InsertData(_columnNames, True)
                Dim dataRange = ws.Cell(2, 1).InsertData(ExportToDataTable(rowIndicesToExport, columnIndicesToExport))
                headerRange.Style.Font.Bold = True
                headerRange.Style.Font.FontColor = Excel.XLColor.Black
                headerRange.Style.Fill.BackgroundColor = Excel.XLColor.FromArgb(225, 240, 250)
                headerRange.Style.Alignment.Horizontal = Excel.XLAlignmentHorizontalValues.Left
                headerRange.Style.Border.BottomBorder = Excel.XLBorderStyleValues.Medium
                headerRange.Style.Border.BottomBorderColor = Excel.XLColor.FromArgb(125, 140, 150)
                headerRange.SetAutoFilter(True)
                ws.Columns(1, _columnNames.Count).AdjustToContents()
                '
                wb.SaveAs(filePath)
            End Using
        End If
    End Sub

    Public Function ExportToDataTable(Optional ByVal rowIndicesToExport() As Int32 = Nothing, Optional ByVal columnIndicesToExport() As Int32 = Nothing) As DataTable
        If IsNothing(columnIndicesToExport) Then
            ReDim columnIndicesToExport(_columnNames.Count - 1)
            For i = 0 To _columnNames.Count - 1
                columnIndicesToExport(i) = i
            Next
        End If
        If IsNothing(rowIndicesToExport) Then
            ReDim rowIndicesToExport(_nRows - 1)
            For i = 0 To _nRows - 1
                rowIndicesToExport(i) = i
            Next
        End If
        '
        Dim dt As New DataTable
        For i = 0 To columnIndicesToExport.Count - 1
            If dt.Columns.Contains(_columnNames(columnIndicesToExport(i))) Then
                Dim sameNameCounter = 1
                Dim newColumnName As String = _columnNames(columnIndicesToExport(i)) & sameNameCounter
                Do Until dt.Columns.Contains(newColumnName) = False
                    sameNameCounter += 1
                    newColumnName = _columnNames(columnIndicesToExport(i)) & sameNameCounter
                Loop
                dt.Columns.Add(newColumnName, _columnTypes(columnIndicesToExport(i)))
            Else
                dt.Columns.Add(_columnNames(columnIndicesToExport(i)), _columnTypes(columnIndicesToExport(i)))
            End If
        Next

        Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
        If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
        For i = 0 To rowIndicesToExport.Count - 1
            dt.Rows.Add(GetRow(rowIndicesToExport(i), columnIndicesToExport))
        Next
        If wasOpen = False Then _parentDatabase.Close()
        '
        Return dt
    End Function

    Public Sub ExportToDbf(filePath As String, Optional ByVal rowIndicesToExport() As Int32 = Nothing, Optional ByVal columnIndicesToExport() As Int32 = Nothing)
        If Path.GetExtension(filePath) <> ".dbf" Then _
            Throw New Exception("Supplied file path is not a database file (.dbf).")
        If IsNothing(columnIndicesToExport) Then
            ReDim columnIndicesToExport(_columnNames.Count - 1)
            For i = 0 To _columnNames.Count - 1
                columnIndicesToExport(i) = i
            Next
        End If
        If IsNothing(rowIndicesToExport) Then
            ReDim rowIndicesToExport(_nRows - 1)
            For i = 0 To _nRows - 1
                rowIndicesToExport(i) = i
            Next
        End If
        'only include DBF fields that are supported
        Dim filteredColumnsToExport As New List(Of Int32)
        For i = 0 To columnIndicesToExport.Count - 1
            Select Case _columnTypes(i)
                Case GetType(Double), GetType(Single), GetType(Int32), GetType(UInt32), GetType(Short), GetType(UShort),
                    GetType(Byte), GetType(Boolean), GetType(String)
                    filteredColumnsToExport.Add(columnIndicesToExport(i))
            End Select
        Next
        'Gotta Start the DBF
        Dim dt As New DataTable
        For i = 0 To filteredColumnsToExport.Count - 1
            If dt.Columns.Contains(_columnNames(filteredColumnsToExport(i))) Then
                Dim sameNameCounter = 1
                Dim newColumnName As String = _columnNames(filteredColumnsToExport(i)) & sameNameCounter
                Do Until dt.Columns.Contains(newColumnName) = False
                    sameNameCounter += 1
                    newColumnName = _columnNames(filteredColumnsToExport(i)) & sameNameCounter
                Loop
                dt.Columns.Add(newColumnName, _columnTypes(filteredColumnsToExport(i)))
            Else
                dt.Columns.Add(_columnNames(filteredColumnsToExport(i)), _columnTypes(filteredColumnsToExport(i)))
            End If
        Next
        dt.Rows.Add(GetRow(rowIndicesToExport(0), filteredColumnsToExport.ToArray))
        If File.Exists(filePath) Then Kill(filePath)
        Try
            DbfReader.CreateDbf(filePath, dt)
            Dim outputDbf As New DbfReader(filePath)
            Dim outputDbft As DataTableView = outputDbf.GetTableManager(Path.GetFileNameWithoutExtension(filePath))
            For i = 1 To rowIndicesToExport.Count - 1
                outputDbft.AddRow(GetRow(rowIndicesToExport(i), filteredColumnsToExport.ToArray))
            Next
            outputDbft.ApplyEdits()
        Catch ex As Exception
            Throw New Exception("An error occured during export." & vbCrLf & ex.Message)
            'If File.Exists(filePath) Then Kill(filePath)
        End Try
    End Sub

    ''' <summary>
    ''' Converts the input value to the specified type.
    ''' </summary>
    ''' <param name="columnType"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    Public Shared Function ConvertToColumnType(columnType As Type, ByRef value As Object) As Boolean
        If IsDBNull(value) Then Return True
        If value Is Nothing Then
            value = DBNull.Value
            Return True
        End If
        Select Case columnType
            Case GetType(Double)
                If value.GetType <> GetType(Double) Then
                    Dim test As Double
                    If Double.TryParse(value.ToString(), test) = False Then Return False
                    value = test
                End If
            Case GetType(Single)
                If value.GetType <> GetType(Single) Then
                    Dim test As Single
                    If Single.TryParse(value.ToString(), test) = False Then Return False
                    value = test
                End If
            Case GetType(Long)
                Dim test As Double, test2 As Long
                If Double.TryParse(value.ToString(), test) = False Then Return False
                Try
                    test2 = Convert.ToInt64(test)
                Catch ex As Exception
                    Return False
                End Try
                value = test2
            Case GetType(ULong)
                Dim test As Double, test2 As ULong
                If Double.TryParse(value.ToString(), test) = False Then Return False
                Try
                    test2 = Convert.ToUInt64(test)
                Catch ex As Exception
                    Return False
                End Try
                value = test2
            Case GetType(Integer)
                Dim test As Double, test2 As Integer
                If Double.TryParse(value.ToString(), test) = False Then Return False
                Try
                    test2 = Convert.ToInt32(test)
                Catch ex As Exception
                    Return False
                End Try
                value = test2
            Case GetType(UInteger)
                Dim test As Double, test2 As UInteger
                If Double.TryParse(value.ToString(), test) = False Then Return False
                Try
                    test2 = Convert.ToUInt32(test)
                Catch ex As Exception
                    Return False
                End Try
                value = test2
            Case GetType(Short)
                Dim test As Double, test2 As Short
                If Double.TryParse(value.ToString(), test) = False Then Return False
                Try
                    test2 = Convert.ToInt16(test)
                Catch ex As Exception
                    Return False
                End Try
                value = test2
            Case GetType(UShort)
                Dim test As Double, test2 As UShort
                If Double.TryParse(value.ToString(), test) = False Then Return False
                Try
                    test2 = Convert.ToUInt16(test)
                Catch ex As Exception
                    Return False
                End Try
                value = test2
            Case GetType(Byte)
                Dim test As Double, test2 As Byte
                If Double.TryParse(value.ToString(), test) = False Then Return False
                Try
                    test2 = Convert.ToByte(test)
                Catch ex As Exception
                    Return False
                End Try
                value = test2
            Case GetType(Boolean)
                Dim test As Boolean
                If Boolean.TryParse(value.ToString(), test) = False Then test = (value.ToString() = "1")
                value = test
            Case GetType(Byte())
                If value.GetType <> GetType(Byte()) Then
                    Dim bf = New BinaryFormatter()
                    Using ms As New MemoryStream()
                        bf.Serialize(ms, value)
                        value = ms.ToArray()
                    End Using
                End If
                Return True
            Case GetType(String)
                If IsNothing(value) Then
                    value = ""
                Else
                    value = value.ToString
                End If
                'If String.IsNullOrEmpty(value) = True Then Return True
                'If value.GetType <> GetType(String) Then Return False
                ' value = value.ToString
                '.tostring is sufficient for the majority of cases.

        End Select
        Return True
    End Function

    ''' <summary>
    ''' Creates a value from a specified type. For string it is "" and for values it is zero.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetDefaultFromType(columnType As Type) As Object
        Select Case columnType
            Case GetType(Double)
                Return CDbl(0)
            Case GetType(Single)
                Return CSng(0)
            Case GetType(Long), GetType(ULong)
                Return CLng(0)
            Case GetType(Integer), GetType(UInteger)
                Return CInt(0)
            Case GetType(Short), GetType(UShort)
                Return CShort(0)
            Case GetType(Byte)
                Return CByte(0)
            Case GetType(Boolean)
                Return False
            Case GetType(Byte())
                Return New Byte() {}
            Case GetType(String)
                Return ""
            Case Else
                Return New NotImplementedException()
        End Select
    End Function
End Class































