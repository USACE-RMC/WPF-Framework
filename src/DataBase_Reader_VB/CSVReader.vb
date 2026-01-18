Imports System.Data
Imports System.IO
Public Class CsvReader
    Inherits DatabaseManager
    Private ReadOnly _table As InMemoryReader

    Public Sub New(filePath As String, hasHeaders As Boolean, dataLineStartIndex As Int32, fieldsEnclosedInQuotes As Boolean)
        If Path.GetExtension(filePath).ToLower <> ".csv" Then Throw New Exception("This is not a .csv file")
        _dataBasePath = filePath
        Dim table As New DataTable(Path.GetFileNameWithoutExtension(filePath))
        Using csvParser As New FileIO.TextFieldParser(filePath) With {.TextFieldType = FileIO.FieldType.Delimited}
            csvParser.SetDelimiters(New String() {","})
            csvParser.HasFieldsEnclosedInQuotes = fieldsEnclosedInQuotes
            Dim lineArray() As String
            lineArray = csvParser.ReadFields()
            If hasHeaders = True Then
                For Each header As String In lineArray
                    If table.Columns.Contains(header) Then
                        Dim counter As Int32 = 1
                        Do Until table.Columns.Contains(header & "_" & counter) = False
                            counter += 1
                        Loop
                        table.Columns.Add(header & "_" & counter, GetType(String))
                    Else
                        table.Columns.Add(header, GetType(String))
                    End If
                Next
            Else
                For i As Int32 = 0 To lineArray.Count - 1
                    table.Columns.Add("Column_" & i + 1, GetType(String))
                Next
            End If
            '
            For i As Integer = 0 To dataLineStartIndex - 1
                lineArray = csvParser.ReadFields()
            Next
            'Dim lineCounter As Int32 = 0
            While Not csvParser.EndOfData
                If lineArray.Count <> table.Columns.Count Then
                    Try
                        lineArray = csvParser.ReadFields()
                    Catch ex As FileIO.MalformedLineException
                        'If the fields are enclosed in quotes then likely failed due to quotes within the quoted text
                        If fieldsEnclosedInQuotes Then lineArray = csvParser.ErrorLine.Substring(1, csvParser.ErrorLine.Length - 2).Split({Chr(34) & "," & Chr(34)}, StringSplitOptions.None)
                    End Try
                    Continue While
                End If
                table.Rows.Add(lineArray)
                'lineCounter += 1
                Try
                    lineArray = csvParser.ReadFields()
                Catch ex As FileIO.MalformedLineException
                    'If the fields are enclosed in quotes then likely failed due to quotes within the quoted text
                    If fieldsEnclosedInQuotes Then lineArray = csvParser.ErrorLine.Substring(1, csvParser.ErrorLine.Length - 2).Split({Chr(34) & "," & Chr(34)}, StringSplitOptions.None)
                End Try
            End While
            'Get the last row if it is valid.
            If lineArray.Count = table.Columns.Count Then table.Rows.Add(lineArray)
        End Using
        _table = New InMemoryReader(table)
        _tableNames = GetTableNames()
    End Sub

    Public Overrides Sub Open()
        '
    End Sub
    Public Overrides Sub Close()
        '
    End Sub
    Public Overrides Function GetTableNames() As String()
        Return _table.GetTableNames
    End Function
    Public Overrides Function GetTableManager(tableName As String) As DataTableView
        Return New CsvTableReader(Me, _table, tableName)
    End Function

    Public Overrides Function GetStoredNumberOfRows(tableName As String) As Long
        Return _table.GetStoredNumberOfRows(tableName)
    End Function
    Public Overrides Function GetStoredNumberOfColumns(tableName As String) As Int32
        Return _table.GetStoredNumberOfColumns(tableName)
    End Function

    Private Class CsvTableReader
        Inherits DataTableView
        Private ReadOnly _table As DataTable

        Public Sub New(inMemManager As CsvReader, inMemReader As InMemoryReader, dataTableName As String)
            _parentDatabase = inMemManager
            _table = inMemReader.Table
            _tableName = dataTableName
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            _storedNumberOfRows = GetStoredRowCount()
            'view stuff
            InitializeView()
            'AddHandler _parentDatabase.RowAdded, Sub(changedTableName As String, rowData() As Object) RowDataChanged(changedTableName)
            'AddHandler _parentDatabase.RowsAdded, Sub(changedTableName As String, rowData As List(Of Object())) RowDataChanged(changedTableName)
            'AddHandler _parentDatabase.RowDeleted, Sub(changedTableName As String, rowIndex As Int32) RowDataChanged(changedTableName)
            'AddHandler _parentDatabase.RowsDeleted, Sub(changedTableName As String, rowIndices() As Int32) RowDataChanged(changedTableName)
            'AddHandler _parentDatabase.ColumnAdded, Sub(changedTableName As String, columnName As String, columnType As Type) ColumnDataChanged(changedTableName)
            'AddHandler _parentDatabase.ColumnDeleted, Sub(changedTableName As String, columnName As String) ColumnDataChanged(changedTableName)
            'AddHandler _parentDatabase.ColumnsDeleted, Sub(changedTableName As String, columnsDeleted() As String) ColumnDataChanged(changedTableName)
        End Sub
        'Private Sub RowDataChanged(changedTableName As String)
        '    'If ChangedTableName <> _TableName Then Exit Sub
        '    '_nRows = GetStoredRowCount()
        'End Sub
        'Private Sub ColumnDataChanged(changedTableName As String)
        '    'If ChangedTableName <> _TableName Then Exit Sub
        '    _storedColumnNames = GetStoredColumnNames()
        '    _storedColumnTypes = GetStoredColumnTypes()
        '    '_nColumns = _storedColumnNames.Count
        'End Sub

        Protected Overrides Function GetStoredColumnNames() As String()
            Dim names(_table.Columns.Count - 1) As String
            For i As Int32 = 0 To names.Count - 1
                names(i) = _table.Columns(i).ColumnName
            Next
            Return names
        End Function
        Protected Overrides Function GetStoredColumnTypes() As Type()
            Dim types(_table.Columns.Count - 1) As Type
            For i As Int32 = 0 To types.Count - 1
                types(i) = _table.Columns(i).DataType
            Next
            Return types
        End Function
        Protected Overrides Function GetStoredRowCount() As ULong
            Return CULng(_table.Rows.Count)
        End Function


#Region "Column Stuff"
        Protected Overrides Function GetStoredColumn(storedColumnName As String) As Object()
            Dim result(_table.Rows.Count - 1) As Object
            For i As Int32 = 0 To _table.Rows.Count - 1
                result(i) = _table.Rows(i).Item(storedColumnName)
            Next
            Return result
        End Function
        Protected Overrides Function GetStoredColumn(storedColumnIndex As Int32) As Object()
            Dim result(_table.Rows.Count - 1) As Object
            For i As Int32 = 0 To _table.Rows.Count - 1
                result(i) = _table.Rows(i).Item(storedColumnIndex)
            Next
            Return result
        End Function
        'Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Object)
        '    Select Case columnData(0).GetType
        '        Case GetType(Byte)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Byte)().ToArray())
        '        Case GetType(Short)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Short)().ToArray())
        '        Case GetType(Integer)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Integer)().ToArray())
        '        Case GetType(Long)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Long)().ToArray())
        '        Case GetType(Single)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Single)().ToArray())
        '        Case GetType(Double)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Double)().ToArray())
        '        Case GetType(Boolean)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Boolean)().ToArray())
        '        Case GetType(String)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of String)().ToArray())
        '    End Select
        'End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData As Byte()())
            Throw New NotImplementedException("CSV does not support storage of byte arrays.")
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Byte)
            If _table.Columns.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            _table.Columns.Add(columnName, GetType(Byte))
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(_table.Columns.Count - 1) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Byte))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Double)
            If _table.Columns.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")

            _table.Columns.Add(columnName, GetType(Double))
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(_table.Columns.Count - 1) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Double))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Integer)
            If _table.Columns.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")

            _table.Columns.Add(columnName, GetType(Integer))
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(_table.Columns.Count - 1) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Int32))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Long)
            If _table.Columns.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")

            _table.Columns.Add(columnName, GetType(Long))
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(_table.Columns.Count - 1) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Long))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Short)
            If _table.Columns.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")

            _table.Columns.Add(columnName, GetType(Short))
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(_table.Columns.Count - 1) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Short))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Single)
            If _table.Columns.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")

            _table.Columns.Add(columnName, GetType(Single))
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(_table.Columns.Count - 1) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Single))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Boolean)
            If _table.Columns.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")

            _table.Columns.Add(columnName, GetType(Boolean))
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(_table.Columns.Count - 1) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Boolean))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As String)
            If _table.Columns.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")

            _table.Columns.Add(columnName, GetType(String))
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(_table.Columns.Count - 1) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(String))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
        End Sub

        Protected Overrides Sub DeleteColumnFromDatabase(columnName As String)
            _table.Columns.Remove(columnName)
            '_parentDatabase.OnColumnsDeleted(_tableName, {columnName})
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
        End Sub
        Protected Overrides Sub DeleteColumnsFromDatabase(columnsToDelete() As String)
            For i As Int32 = 0 To columnsToDelete.Count - 1
                _table.Columns.Remove(columnsToDelete(i))
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnColumnsDeleted(_tableName, columnsToDelete)
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
        End Sub
        'Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Object)
        '    Select Case columnData(0).GetType
        '        Case GetType(Byte)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Byte)().ToArray())
        '        Case GetType(Short)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Short)().ToArray())
        '        Case GetType(Integer)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Integer)().ToArray())
        '        Case GetType(Long)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Long)().ToArray())
        '        Case GetType(Single)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Single)().ToArray())
        '        Case GetType(Double)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Double)().ToArray())
        '        Case GetType(Boolean)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of Boolean)().ToArray())
        '        Case GetType(String)
        '            EditDatabaseColumn(columnName, columnData.Cast(Of String)().ToArray())
        '    End Select
        'End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Boolean)
            If _table.Columns.Contains(columnName) = False Then Throw New Exception("Column Name " & columnName & " does not exist, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(columnName) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Byte)
            If _table.Columns.Contains(columnName) = False Then Throw New Exception("Column Name " & columnName & " does not exist, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(columnName) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData()() As Byte)
            Throw New NotImplementedException("csv file format does not support storing binary array data.")
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Int16)
            If _table.Columns.Contains(columnName) = False Then Throw New Exception("Column Name " & columnName & " does not exist, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(columnName) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Int32)
            If _table.Columns.Contains(columnName) = False Then Throw New Exception("Column Name " & columnName & " does not exist, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(columnName) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Int64)
            If _table.Columns.Contains(columnName) = False Then Throw New Exception("Column Name " & columnName & " does not exist, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(columnName) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Single)
            If _table.Columns.Contains(columnName) = False Then Throw New Exception("Column Name " & columnName & " does not exist, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(columnName) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Double)
            If _table.Columns.Contains(columnName) = False Then Throw New Exception("Column Name " & columnName & " does not exist, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(columnName) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As String)
            If _table.Columns.Contains(columnName) = False Then Throw New Exception("Column Name " & columnName & " does not exist, choose a different name.")
            If _table.Rows.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            For i As Int32 = 0 To columnData.Count - 1
                _table.Rows(i).Item(columnName) = columnData(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
#End Region

#Region "Row Stuff"
        Protected Overrides Sub AddRowToDatabase(row() As Object)
            _table.Rows.Add(row)
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnRowsAdded(_tablename, New List(Of Object())({row}))
        End Sub
        Protected Overrides Sub AddRowToDatabase()
            _table.Rows.Add(_table.NewRow)
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnRowsAdded(_tablename, New List(Of Object())({_table.Rows.Item(_table.Rows.Count - 1).ItemArray}))
        End Sub
        Protected Overrides Sub AddRowsToDatabase(newRowData As List(Of Object()))
            For i As Int32 = 0 To newRowData.Count - 1
                AddRowToDatabase(newRowData(i))
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnRowsAdded(_tableName, newRowData)
        End Sub

        Protected Overrides Sub DeleteRowFromDatabase(rowIndex As Integer)
            _table.Rows(rowIndex).Delete()
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnRowsDeleted(_tableName, {rowIndex})
        End Sub
        Protected Overrides Sub DeleteRowsFromDatabase(rowIndices() As Integer)
            Array.Sort(rowIndices)
            For i As Int32 = rowIndices.Count - 1 To 0 Step -1
                _table.Rows(rowIndices(i)).Delete()
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
            '_parentDatabase.OnRowsDeleted(_tableName, rowIndices)
        End Sub

        Protected Overrides Function GetStoredRow(storedRowIndex As Integer) As Object()
            Return _table.Rows(storedRowIndex).ItemArray
        End Function
        Protected Overrides Function GetStoredRow(storedRowIndex As Integer, storedColumnNames() As String) As Object()
            Dim result(storedColumnNames.Count - 1) As Object
            For i As Int32 = 0 To storedColumnNames.Count - 1
                result(i) = _table.Rows(storedRowIndex).Item(storedColumnNames(i))
            Next
            Return result
        End Function
        Protected Overrides Function GetStoredRow(storedRowIndex As Integer, storedColumnIndices() As Int32) As Object()
            Dim result(storedColumnIndices.Count - 1) As Object
            For i As Int32 = 0 To storedColumnIndices.Count - 1
                result(i) = _table.Rows(storedRowIndex).Item(storedColumnIndices(i))
            Next
            Return result
        End Function
        Protected Overrides Function GetStoredRows(ByVal startStoredRowIndex As Int32, ByVal endStoredRowIndex As Int32) As List(Of Object())
            Dim result As New List(Of Object())
            For i As Int32 = startStoredRowIndex To endStoredRowIndex
                result.Add(_table.Rows(i).ItemArray)
            Next
            Return result
        End Function
#End Region

#Region "Cell Stuff"
        Protected Overrides Sub EditDatabaseCells(columnsToEdit() As String, rowIndices() As Integer, cellValues() As Object)
            For i As Int32 = 0 To columnsToEdit.Count - 1
                _table.Rows(rowIndices(i)).Item(columnsToEdit(i)) = cellValues(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseCells(columnIndices() As Int32, rowIndices() As Integer, cellValues() As Object)
            For i As Int32 = 0 To columnIndices.Count - 1
                _table.Rows(rowIndices(i)).Item(columnIndices(i)) = cellValues(i)
            Next
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        'Protected Overrides Sub EditDatabaseCell(columnIndex As Integer, rowIndex As Integer, cellValue As Object)
        '    Select Case cellValue.GetType
        '        Case GetType(Byte)
        '            EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Byte))
        '        Case GetType(Short)
        '            EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Short))
        '        Case GetType(Integer)
        '            EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Int32))
        '        Case GetType(Long)
        '            EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Int64))
        '        Case GetType(Single)
        '            EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Single))
        '        Case GetType(Double)
        '            EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Double))
        '        Case GetType(Boolean)
        '            EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Boolean))
        '        Case GetType(String)
        '            EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, String))
        '    End Select
        'End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Boolean)
            _table.Rows(rowIndex).Item(columnIndex) = cellValue
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Byte)
            _table.Rows(rowIndex).Item(columnIndex) = cellValue
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Double)
            _table.Rows(rowIndex).Item(columnIndex) = cellValue
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Single)
            _table.Rows(rowIndex).Item(columnIndex) = cellValue
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Int16)
            _table.Rows(rowIndex).Item(columnIndex) = cellValue
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Int32)
            _table.Rows(rowIndex).Item(columnIndex) = cellValue
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Int64)
            _table.Rows(rowIndex).Item(columnIndex) = cellValue
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As String)
            _table.Rows(rowIndex).Item(columnIndex) = cellValue
            ExportToCsv(_parentDatabase.DataBasePath)
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Byte())
            Throw New NotImplementedException("CSV does not support storage of byte arrays.")
        End Sub

        Protected Overrides Function GetStoredCell(storedColumnIndex As Int32, storedRowIndex As Integer) As Object
            Return _table.Rows(storedRowIndex).Item(storedColumnIndex)
        End Function
        Protected Overrides Function GetStoredCell(storedColumnName As String, storedRowIndex As Integer) As Object
            Return _table.Rows(storedRowIndex).Item(storedColumnName)
        End Function
        Protected Overrides Function GetStoredCells(storedColumnIndices() As Int32, storedRowIndices() As Int32) As Object()
            Dim result(storedColumnIndices.Count - 1) As Object
            For i As Int32 = 0 To storedColumnIndices.Count - 1
                result(i) = _table.Rows(storedRowIndices(i)).Item(storedColumnIndices(i))
            Next
            Return result.ToArray
        End Function
#End Region

    End Class


End Class

