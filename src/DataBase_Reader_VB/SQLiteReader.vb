Imports System.Data.SQLite
'Imports System.IO
Public Class SqLiteReader
    Inherits DatabaseReader
    Private ReadOnly _dbConnection As SQLiteConnection
    Public ReadOnly Property DbConnection As SQLiteConnection
        Get
            Return _dbConnection
        End Get
    End Property

    Public Sub New(dataBaseFile As String)
        _dataBasePath = dataBaseFile
        _dbConnection = New SQLiteConnection("Data Source=" & _dataBasePath & ";Version=3;")
        _tableNames = GetTableNames()
    End Sub
    Public Sub New(dataBaseFile As String, databasePassword As String)
        _dataBasePath = dataBaseFile
        _dbConnection = New SQLiteConnection("Data Source=" & _dataBasePath & ";Version=3;Password=" & databasePassword & ";")
        _tableNames = GetTableNames()
    End Sub
    Public Shared Sub CreateSqLiteFile(databaseFile As String)
        SQLiteConnection.CreateFile(databaseFile)
    End Sub
    Public Shared Sub CreateSqLiteFile(databaseFile As String, databasePassword As String)
        SQLiteConnection.CreateFile(databaseFile)

        Using sqlConn As New SQLiteConnection("DataSource=" & databaseFile & ";Version=3;")
            'sqlConn.ConnectionString = "DataSource=" & databaseFile & ";Version=3;New=False;Compress=True;"
            sqlConn.Open()
            sqlConn.ChangePassword(databasePassword)
            'sqlConn.Close()
        End Using

        'Using conn As New SQLiteConnection("Data Source=" & databaseFile & ";Version=3;")
        '    conn.SetPassword(databasePassword)
        'End Using
    End Sub
    'Public Sub SetPassword(databasePassword)
    '    Dim wasOpen As Boolean = _dataBaseOpen
    '    If _dataBaseOpen = True Then Close()
    '    _dbConnection.SetPassword(databasePassword)
    '    If wasOpen = True Then Open()
    'End Sub
    Public Overrides Sub Open()
        _dbConnection.Open()
        _dataBaseOpen = True
    End Sub
    Public Overrides Sub Close()
        _dbConnection.Close()
        _dataBaseOpen = False
    End Sub
    Public Sub Vacuum()
        Dim wasOpen As Boolean = _dataBaseOpen
        If _dataBaseOpen = False Then Open()
        '
        Using command As New SQLiteCommand("vacuum", _dbConnection)
            command.ExecuteNonQuery()
        End Using
    End Sub
    Public Sub CopyTable(existingTableName As String, newTableName As String)
        Dim wasOpen As Boolean = _dataBaseOpen
        If _dataBaseOpen = False Then Open()
        If _tableNames.Contains(existingTableName) = False Then Throw New Exception("Table '" & existingTableName & "' does not exist in the database.")
        'Create the table copy create statement from existing table.
        Dim existingCreateStatement As String = ""
        Using cmd As New SQLiteCommand("SELECT sql FROM sqlite_master WHERE type='table' AND name='" & existingTableName & "'", _dbConnection)
            Using reader As System.Data.SQLite.SQLiteDataReader = cmd.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read
                        existingCreateStatement = CStr(reader.Item(0))
                    End While
                End If
            End Using
        End Using
        If existingCreateStatement = "" Then Throw New Exception("Table '" & existingTableName & "' does not have a create statement and currently cannot be copied.")
        'rename existing table to new table and create fresh copy of existing table.
        RenameTable(existingTableName, newTableName)
        Using command As New SQLiteCommand(existingCreateStatement, _dbConnection)
            command.ExecuteNonQuery()
        End Using
        'Copy from new table into existing table since existing table was simply renamed to new table to simplify create statement.
        Using command As New SQLiteCommand("INSERT INTO [" & existingTableName & "] SELECT * FROM [" & newTableName & "]", _dbConnection)
            command.ExecuteNonQuery()
        End Using
        '
        If wasOpen = False Then Close()
        _tableNames = GetTableNames()
    End Sub
    Public Sub RenameTable(oldTableName As String, newTableName As String)
        Dim wasOpen As Boolean = _dataBaseOpen
        If _dataBaseOpen = False Then Open()
        If _tableNames.Contains(oldTableName) = False Then Throw New Exception("Table '" & oldTableName & "' does not exist in the database.")
        '
        Using command As New SQLiteCommand("ALTER TABLE [" & oldTableName & "] RENAME TO [" & newTableName & "]", _dbConnection)
            command.ExecuteNonQuery()
        End Using
        '
        If wasOpen = False Then Close()
        _tableNames = GetTableNames()
    End Sub
    Public Overrides Function GetTableNames() As String()
        Dim wasOpen As Boolean = _dataBaseOpen
        If _dataBaseOpen = False Then Open()
        '
        Dim result As New List(Of String)
        Using command As New SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table'", _dbConnection)
            Using reader As SQLiteDataReader = command.ExecuteReader()
                If reader.HasRows Then
                    While reader.Read
                        result.Add(reader.Item(0))
                    End While
                End If
            End Using
        End Using
        '
        If wasOpen = False Then Close()
        _tableNames = result.ToArray
        Return result.ToArray
    End Function
    Public Sub SaveDataTable(dt As DataTable)
        Try
            Dim wasOpen As Boolean = _dataBaseOpen
            If _dataBaseOpen = False Then Open()
            '
            Dim parameters As New List(Of SQLiteParameter)
            Dim sb As New Text.StringBuilder(500)
            sb.Append("Create Table [").Append(dt.TableName).Append("] (")
            For Each c As DataColumn In dt.Columns
                Dim tc = Type.GetTypeCode(c.DataType)
                sb.Append("[").Append(c.ColumnName).Append("] ")
                Select Case tc
                    Case TypeCode.String
                        sb.Append("TEXT,")
                        'If c.MaxLength > 255 OrElse c.MaxLength < 0 Then Sb.Append("TEXT")
                        'Else Sb.AppendFormat("Text)", c.MaxLength)
                    Case TypeCode.DateTime : sb.Append("DATETIME,")
                    Case TypeCode.Byte, TypeCode.SByte : sb.Append("INT1,")
                    Case TypeCode.Int16, TypeCode.UInt16 : sb.Append("INT2,")
                    Case TypeCode.Int32, TypeCode.UInt32 : sb.Append("INT4,")
                    Case TypeCode.Int64, TypeCode.UInt64 : sb.Append("INT8,")
                    Case TypeCode.Single : sb.Append("FLOAT,")
                    Case TypeCode.Double : sb.Append("DOUBLE,")
                    Case TypeCode.Decimal : sb.Append("NUMBER,")
                    Case TypeCode.Char : sb.Append("CHAR,")
                    Case TypeCode.Boolean : sb.Append("BOOLEAN,")
                    Case TypeCode.Object : sb.Append("BLOB,")
                    Case Else
                        If c.DataType Is GetType(Guid) Then
                            sb.Append("Text,")
                        Else
                            Throw New Exception(tc.ToString & " Not implemented, Column: " & c.ColumnName)
                        End If
                End Select
                parameters.Add(New SQLiteParameter(c.ColumnName))
            Next
            If dt.Columns.Count > 0 Then
                sb.Chars(sb.Length - 1) = ")"c
            Else
                sb.Remove(sb.Length - 1, 1)
            End If
            'Create the table
            Using tr As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = tr
                    'Create Table
                    cmd.CommandText = sb.ToString
                    cmd.ExecuteNonQuery()
                End Using
                tr.Commit()
            End Using
            'add the rows
            Using tr As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = tr
                    'Create Insert string
                    Dim insertText As String = "INSERT INTO [" & dt.TableName & "] VALUES ("
                    For i As Int32 = 0 To parameters.Count - 1
                        insertText = insertText & "?" & "," 'InsertText & "@" & parameters(i).ParameterName & "," 'using @ does not allow spaces in column names.
                        cmd.Parameters.Add(parameters(i))
                    Next
                    insertText = insertText.Substring(0, insertText.Length - 1) & ")"
                    cmd.CommandText = insertText
                    'Insert Rows
                    For Each r As DataRow In dt.Rows
                        For i As Int32 = 0 To parameters.Count - 1
                            parameters(i).Value = r.Item(i)
                        Next
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
            _tableNames = GetTableNames()
            If wasOpen = False Then Close()

        Catch ex As Exception
            Throw New Exception("Error writing SQLite DataTable: " & ex.Message)
        End Try
    End Sub
    Public Sub CreateTable(ByVal newTableName As String, ByVal tableColumnNames() As String, ByVal newColumnTypes() As Type)
        Dim wasOpen As Boolean = _dataBaseOpen
        If _dataBaseOpen = False Then Open()
        Dim sb As New Text.StringBuilder(500)
        sb.Append("Create Table [").Append(newTableName).Append("] (")
        For i As Int32 = 0 To tableColumnNames.Count - 1
            sb.Append("[").Append(tableColumnNames(i)).Append("] ")
            Select Case newColumnTypes(i)
                Case GetType(String) : sb.Append("TEXT,")
                Case GetType(DateTime) : sb.Append("DATETIME,")
                Case GetType(Byte), GetType(SByte) : sb.Append("INT1,")
                Case GetType(Int16), GetType(UInt16) : sb.Append("INT2,")
                Case GetType(Int32), GetType(UInt32) : sb.Append("INT4,")
                Case GetType(Int64), GetType(UInt64) : sb.Append("INT8,")
                Case GetType(Single) : sb.Append("FLOAT,")
                Case GetType(Double) : sb.Append("DOUBLE,")
                Case GetType(Decimal) : sb.Append("NUMBER,")
                Case GetType(Char) : sb.Append("CHAR,")
                Case GetType(Boolean) : sb.Append("BOOLEAN,")
                Case GetType(Object), GetType(Byte()) : sb.Append("BLOB,")
                Case Else
                    Throw New Exception(newColumnTypes(i).ToString & " Not implemented, Column: " & tableColumnNames(i))
            End Select
        Next
        If tableColumnNames.Count > 0 Then
            sb.Chars(sb.Length - 1) = ")"
        Else
            sb.Remove(sb.Length - 1, 1)
        End If
        Using cmd As SQLiteCommand = _dbConnection.CreateCommand
            cmd.CommandText = sb.ToString
            cmd.ExecuteNonQuery()
        End Using
        _tableNames = GetTableNames()
        If wasOpen = False Then Close()
    End Sub
    Public Sub DeleteTable(tableName As String)
        If _tableNames.Contains(tableName) = False Then Exit Sub
        Dim wasOpen As Boolean = _dataBaseOpen
        If _dataBaseOpen = False Then Open()
        '
        Using cmd As SQLiteCommand = _dbConnection.CreateCommand
            cmd.CommandText = "DROP TABLE [" & tableName & "]"
            cmd.ExecuteNonQuery()
        End Using
        _tableNames = GetTableNames()
        Close()
        If wasOpen = True Then Open()
    End Sub
    Public Overrides Function GetTableManager(tableName As String) As DataTableView
        Return New SqLiteTableReader(Me, tableName, _dbConnection)
    End Function

    Public Overrides Function GetStoredNumberOfRows(tableName As String) As Long
        Dim wasOpen As Boolean = _dataBaseOpen
        If _dataBaseOpen = False Then Open()
        Dim rowCount As Long
        Using command As New SQLiteCommand("SELECT Count(*) FROM [" & tableName & "]", _dbConnection)
            rowCount = CLng(command.ExecuteScalar)
        End Using
        If wasOpen = False Then Close()
        Return rowCount
    End Function

    Private Class SqLiteTableReader
        Inherits DataTableView
        Private _rowIdArray() As Int32
        Private _columnIsPrimaryKey() As Boolean
        Private ReadOnly _dbConnection As SQLiteConnection
        Public Sub New(reader As SqLiteReader, dataTableName As String, dbConnection As SQLiteConnection)
            _tableName = dataTableName
            _parentDatabase = reader
            _dbConnection = dbConnection
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            _storedNumberOfRows = GetStoredRowCount()
            InitializeView()
            '_columnNames = GetStoredColumnNames()
            '_columnTypes = GetStoredColumnTypes()
            ''_nColumns = _storedColumnNames.Count
            '_nRows = _storedNumberOfRows
            'sqlite specific
            _rowIdArray = GetRowIdArray()
            '
            'AddHandler _parentDatabase.RowAdded, Sub(changedTableName As String, rowData() As Object) RowDataChanged(changedTableName)
            'AddHandler _parentDatabase.RowsAdded, Sub(changedTableName As String, rowData As List(Of Object())) RowDataChanged(changedTableName)
            'AddHandler _parentDatabase.RowDeleted, Sub(changedTableName As String, rowIndex As Int32) RowDataChanged(changedTableName)
            'AddHandler _parentDatabase.RowsDeleted, Sub(changedTableName As String, rowIndices() As Int32) RowDataChanged(changedTableName)
            'AddHandler _parentDatabase.ColumnAdded, Sub(changedTableName As String, columnName As String, columnType As Type) ColumnDataChanged(changedTableName)
            'AddHandler _parentDatabase.ColumnDeleted, Sub(changedTableName As String, columnName As String) ColumnDataChanged(changedTableName)
            'AddHandler _parentDatabase.ColumnsDeleted, Sub(changedTableName As String, columnsDeleted() As String) ColumnDataChanged(changedTableName)
        End Sub

        'Private Sub RowDataChanged(ByVal changedTableName As String)
        '    If changedTableName <> _tableName Then Exit Sub
        '    _rowIdArray = GetRowIdArray()
        '    '_nRows = GetStoredRowCount()
        'End Sub
        'Private Sub ColumnDataChanged(ByVal changedTableName As String)
        '    If changedTableName <> _tableName Then Exit Sub
        '    _storedColumnNames = GetStoredColumnNames()
        '    _storedColumnTypes = GetStoredColumnTypes()
        '    '_nColumns = _storedColumnNames.Count
        'End Sub

        Protected Overrides Function GetStoredRowCount() As ULong
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Dim rowCount As ULong
            Using command As New SQLiteCommand("SELECT Count(*) FROM [" & _tableName & "]", _dbConnection)
                rowCount = CLng(command.ExecuteScalar)
            End Using
            If wasOpen = False Then _parentDatabase.Close()
            Return rowCount
        End Function
        Private Function GetRowIdArray() As Integer()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim rowIDs As New List(Of Int32)
            Using cmd As New SQLiteCommand("SELECT rowid FROM [" & _tableName & "]", _dbConnection)
                Using reader As SQLiteDataReader = cmd.ExecuteReader
                    If reader.HasRows Then
                        While reader.Read
                            rowIDs.Add(CUInt(reader.Item(0)))
                        End While
                    End If
                End Using
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
            Return rowIDs.ToArray
        End Function
        Protected Overrides Function GetStoredColumnNames() As String()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim result() As String
            Using command As New SQLiteCommand("PRAGMA table_info([" & _tableName & "])", _dbConnection)
                Dim adap As New SQLiteDataAdapter(command)
                Dim tab As New DataTable
                adap.Fill(tab)
                ReDim result(tab.Rows.Count - 1)
                ReDim _columnIsPrimaryKey(tab.Rows.Count - 1)

                For i As Int32 = 0 To tab.Rows.Count - 1
                    result(i) = CStr(tab.Rows(i)(1))
                    If CInt(tab.Rows(i)(5)) > 0 Then _columnIsPrimaryKey(i) = True
                Next
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
            Return result
        End Function
        Protected Overrides Function GetStoredColumnTypes() As Type()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim existingColumnTypes() As Type
            Using command As New SQLiteCommand("PRAGMA table_info([" & _tableName & "])", _dbConnection)
                Dim adap As New SQLiteDataAdapter(command)
                Dim tab As New DataTable
                adap.Fill(tab)
                ReDim existingColumnTypes(tab.Rows.Count - 1)

                Dim typeString As String
                For i As Int32 = 0 To tab.Rows.Count - 1
                    typeString = CStr(tab.Rows(i)(2))
                    If typeString.Contains("INT") Then
                        Select Case typeString
                            Case "INT1", "TINYINT"
                                existingColumnTypes(i) = GetType(Byte)
                            Case "INT2", "SMALLINT"
                                existingColumnTypes(i) = GetType(Int16)
                            Case "INT4", "INTEGER", "MEDIUMINT"
                                existingColumnTypes(i) = GetType(Int32)
                            Case "INT8"
                                existingColumnTypes(i) = GetType(Int64)
                            Case "POINT", "MULTIPOINT" 'This is to handle GeoPackage and their geometry type codes...which fail the sqlite typing rules.
                                existingColumnTypes(i) = GetType(Byte())
                            Case Else 'Unknown, this will probably break
                                existingColumnTypes(i) = GetType(Byte())
                        End Select
                    ElseIf typeString.Contains("CHAR") Or typeString.Contains("CLOB") Or typeString.Contains("TEXT") Then
                        existingColumnTypes(i) = GetType(String)
                    ElseIf typeString.Contains("FLOA") Then 'This doesn't guarantee single precision per the sqlite spec. It is still stored internally as 8 byte decimal.
                        existingColumnTypes(i) = GetType(Single)
                    ElseIf typeString.Contains("REAL") Or typeString.Contains("DOUB") Then
                        existingColumnTypes(i) = GetType(Double)
                    ElseIf typeString.Contains("BOOL") Then
                        existingColumnTypes(i) = GetType(Boolean)
                    ElseIf typeString.Contains("BLOB") Then
                        existingColumnTypes(i) = GetType(Byte())
                    Else
                        existingColumnTypes(i) = GetType(Byte())
                    End If
                Next
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
            Return existingColumnTypes
        End Function

        Protected Overrides Sub AddRowToDatabase(row() As Object)
            Dim parameters As New List(Of SQLiteParameter)
            If row.Count <> _storedColumnNames.Count Then Throw New Exception("Number of columns that you are trying to add do not match the number of columns in the database.")
            For i As Int32 = 0 To row.Count - 1
                If ConvertToColumnType(_storedColumnTypes(i), row(i)) = False Then Throw New Exception("Column type of '" & row(i).GetType.ToString & "' is invalid for column '" & _storedColumnNames(i) & "'.")
                parameters.Add(New SQLiteParameter(_storedColumnNames(i)))
            Next
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                'Create Insert string
                Dim insertText As String = "INSERT INTO [" & _tableName & "] VALUES ("
                For i As Int32 = 0 To parameters.Count - 1
                    insertText = insertText & "?" & "," 'InsertText & "@" & Parameters(i).ParameterName & ","
                    cmd.Parameters.Add(parameters(i))
                Next
                insertText = insertText.Substring(0, insertText.Length - 1) & ")"
                cmd.CommandText = insertText
                'Insert Rows
                For i As Int32 = 0 To row.Count - 1
                    If _columnIsPrimaryKey(i) = True Then
                        parameters(i).Value = DBNull.Value 'should let SQLite do its thing with primary key columns.
                    Else
                        parameters(i).Value = row(i)
                    End If
                Next
                cmd.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnRowsAdded(_tablename, New List(Of Object())({row}))
            _storedNumberOfRows = GetStoredRowCount()
            _rowIdArray = GetRowIdArray()
            '_parentDatabase.OnRowAdded(_tableName, row)
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddRowToDatabase()
            Dim row(_storedColumnNames.Count - 1) As Object
            For i As Int32 = 0 To _storedColumnNames.Count - 1
                row(i) = DBNull.Value
            Next
            AddRowToDatabase(row)
        End Sub
        Protected Overrides Sub AddRowsToDatabase(newRowData As List(Of Object()))
            For i As Int32 = 0 To newRowData.Count - 1
                If newRowData(i).Count <> _storedColumnNames.Count Then Throw New Exception("Number of columns does not match for row " & i.ToString & ".")
                For j As Int32 = 0 To newRowData(i).Count - 1
                    If ConvertToColumnType(_storedColumnTypes(j), newRowData(i)(j)) = False Then Throw New Exception("Column type '" & newRowData(i)(j).GetType.ToString & "' does not match for column '" & _storedColumnNames(j) & "' in row " & i.ToString & ".")
                Next
            Next
            Dim parameters As New List(Of SQLiteParameter)
            For i As Int32 = 0 To _storedColumnNames.Count - 1
                parameters.Add(New SQLiteParameter(_storedColumnNames(i)))
            Next
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using tr As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = tr
                    'Create Insert string
                    Dim insertText As String = "INSERT INTO [" & _tableName & "] VALUES ("
                    For i As Int32 = 0 To parameters.Count - 1
                        insertText = insertText & "?" & "," 'InsertText & "@" & Parameters(i).ParameterName & ","
                        cmd.Parameters.Add(parameters(i))
                    Next
                    insertText = insertText.Substring(0, insertText.Length - 1) & ")"
                    cmd.CommandText = insertText
                    'Insert Rows
                    For i As Int32 = 0 To newRowData.Count - 1
                        For j As Int32 = 0 To newRowData(i).Count - 1
                            If _columnIsPrimaryKey(j) = True Then
                                parameters(j).Value = DBNull.Value 'should let SQLite do its thing with primary key columns.
                            Else
                                parameters(j).Value = newRowData(i)(j)
                            End If
                        Next
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnRowsAdded(_tableName, newRowData)
            _storedNumberOfRows = GetStoredRowCount()
            _rowIdArray = GetRowIdArray()
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub DeleteRowFromDatabase(rowIndex As Integer)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using cmd As New SQLiteCommand("DELETE FROM [" & _tableName & "] WHERE rowid=" & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnRowsDeleted(_tableName, {rowIndex})
            _storedNumberOfRows = GetStoredRowCount()
            _rowIdArray = GetRowIdArray()
            '_parentDatabase.OnRowDeleted(_tableName, rowIndex)
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub DeleteRowsFromDatabase(ByVal rowIndices() As Int32)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    For i As Int32 = 0 To rowIndices.Count - 1
                        cmd.CommandText = "DELETE FROM [" & _tableName & "] WHERE rowid=" & _rowIdArray(rowIndices(i))
                        cmd.ExecuteNonQuery()
                    Next

                End Using
                trans.Commit()
            End Using
            '
            '_parentDatabase.OnRowsDeleted(_tableName, rowIndices)
            _storedNumberOfRows = GetStoredRowCount()
            _rowIdArray = GetRowIdArray()
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
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
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Byte)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Using cmd As New SQLiteCommand("ALTER TABLE [" & _tableName & "] ADD COLUMN [" & columnName & "] INT1", _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Byte))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            '
            EditDatabaseColumn(columnName, columnData)
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Double)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Using cmd As New SQLiteCommand("ALTER TABLE [" & _tableName & "] ADD COLUMN [" & columnName & "] DOUBLE", _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Double))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            '
            EditDatabaseColumn(columnName, columnData)
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Integer)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Using cmd As New SQLiteCommand("ALTER TABLE [" & _tableName & "] ADD COLUMN [" & columnName & "] INT4", _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
            '

            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Integer))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            '
            EditDatabaseColumn(columnName, columnData)
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Int64)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Using cmd As New SQLiteCommand("ALTER TABLE [" & _tableName & "] ADD COLUMN [" & columnName & "] INT8", _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Int64))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            '
            EditDatabaseColumn(columnName, columnData)
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Short)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Using cmd As New SQLiteCommand("ALTER TABLE [" & _tableName & "] ADD COLUMN [" & columnName & "] INT2", _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Short))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            '
            EditDatabaseColumn(columnName, columnData)
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Single)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Using cmd As New SQLiteCommand("ALTER TABLE [" & _tableName & "] ADD COLUMN [" & columnName & "] FLOAT", _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Single))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            '
            EditDatabaseColumn(columnName, columnData)
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Boolean)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Using cmd As New SQLiteCommand("ALTER TABLE [" & _tableName & "] ADD COLUMN [" & columnName & "] BOOLEAN", _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Boolean))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            '
            EditDatabaseColumn(columnName, columnData)
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As String)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Using cmd As New SQLiteCommand("ALTER TABLE [" & _tableName & "] ADD COLUMN [" & columnName & "] TEXT", _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(String))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            '
            EditDatabaseColumn(columnName, columnData)
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub

        Protected Overrides Sub DeleteColumnsFromDatabase(columnsToDelete() As String)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim newColumnNames As New List(Of String)
            Dim newColumnTypes As New List(Of Type)
            Dim sb As New Text.StringBuilder
            For i As Int32 = 0 To _storedColumnNames.Count - 1
                If columnsToDelete.Contains(_storedColumnNames(i)) Then Continue For
                newColumnNames.Add(_storedColumnNames(i))
                newColumnTypes.Add(_storedColumnTypes(i))
                sb.Append("[").Append(_storedColumnNames(i)).Append("] ")
                Select Case _storedColumnTypes(i)
                    Case GetType(String) : sb.Append("TEXT,")
                    Case GetType(DateTime) : sb.Append("DATETIME,")
                    Case GetType(Byte), GetType(SByte) : sb.Append("INT1,")
                    Case GetType(Int16), GetType(UInt16) : sb.Append("INT2,")
                    Case GetType(Int32), GetType(UInt32) : sb.Append("INT4,")
                    Case GetType(Int64), GetType(UInt64) : sb.Append("INT8,")
                    Case GetType(Single) : sb.Append("FLOAT,")
                    Case GetType(Double) : sb.Append("DOUBLE,")
                    Case GetType(Decimal) : sb.Append("NUMBER,")
                    Case GetType(Char) : sb.Append("CHAR,")
                    Case GetType(Boolean) : sb.Append("BOOLEAN,")
                    Case GetType(Object), GetType(Byte()) : sb.Append("BLOB,")
                    Case Else
                        Throw New Exception(_storedColumnTypes(i).ToString & " Not implemented, Column: " & _storedColumnNames(i))
                End Select
            Next
            sb.Remove(sb.Length - 1, 1)

            Using tr As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = tr
                    cmd.CommandText = "CREATE TEMPORARY TABLE [" & _tableName & "_Old] (" & sb.ToString & ")"
                    cmd.ExecuteNonQuery()
                    '
                    cmd.CommandText = "INSERT INTO [" & _tableName & "_Old] SELECT " & sb.ToString & " FROM [" & _tableName & "]"
                    cmd.ExecuteNonQuery()
                    '
                    cmd.CommandText = "DROP TABLE [" & _tableName & "]"
                    cmd.ExecuteNonQuery()
                    '
                    cmd.CommandText = "CREATE TABLE [" & _tableName & "] (" & sb.ToString & ")"
                    cmd.ExecuteNonQuery()
                    '
                    cmd.CommandText = "INSERT INTO [" & _tableName & "] SELECT " & sb.ToString & " FROM [" & _tableName & "_Old]"
                    cmd.ExecuteNonQuery()
                    '
                    cmd.CommandText = "DROP TABLE [" & _tableName & "_Old]"
                    cmd.ExecuteNonQuery()
                End Using
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnColumnsDeleted(_tableName, columnsToDelete)
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub DeleteColumnFromDatabase(columnName As String)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim newColumnNames As New List(Of String)
            Dim newColumnTypes As New List(Of Type)
            Dim sb As New Text.StringBuilder
            For i As Int32 = 0 To _storedColumnNames.Count - 1
                If _storedColumnNames(i) = columnName Then Continue For
                newColumnNames.Add(_storedColumnNames(i))
                newColumnTypes.Add(_storedColumnTypes(i))
                sb.Append("[").Append(_storedColumnNames(i)).Append("] ")
                Select Case _storedColumnTypes(i)
                    Case GetType(String) : sb.Append("TEXT,")
                    Case GetType(DateTime) : sb.Append("DATETIME,")
                    Case GetType(Byte), GetType(SByte) : sb.Append("INT1,")
                    Case GetType(Int16), GetType(UInt16) : sb.Append("INT2,")
                    Case GetType(Int32), GetType(UInt32) : sb.Append("INT4,")
                    Case GetType(Int64), GetType(UInt64) : sb.Append("INT8,")
                    Case GetType(Single) : sb.Append("FLOAT,")
                    Case GetType(Double) : sb.Append("DOUBLE,")
                    Case GetType(Decimal) : sb.Append("NUMBER,")
                    Case GetType(Char) : sb.Append("CHAR,")
                    Case GetType(Boolean) : sb.Append("BOOLEAN,")
                    Case GetType(Object), GetType(Byte()) : sb.Append("BLOB,")
                    Case Else
                        Throw New Exception(_storedColumnTypes(i).ToString & " Not implemented, Column: " & _storedColumnNames(i))
                End Select
            Next
            sb.Remove(sb.Length - 1, 1)

            Using tr As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = tr
                    cmd.CommandText = "CREATE TEMPORARY TABLE [" & _tableName & "_Old] (" & sb.ToString & ")"
                    cmd.ExecuteNonQuery()
                    '
                    cmd.CommandText = "INSERT INTO [" & _tableName & "_Old] SELECT " & sb.ToString & " FROM [" & _tableName & "]"
                    cmd.ExecuteNonQuery()
                    '
                    cmd.CommandText = "DROP TABLE [" & _tableName & "]"
                    cmd.ExecuteNonQuery()
                    '
                    cmd.CommandText = "CREATE TABLE [" & _tableName & "] (" & sb.ToString & ")"
                    cmd.ExecuteNonQuery()
                    '
                    cmd.CommandText = "INSERT INTO [" & _tableName & "] SELECT " & sb.ToString & " FROM [" & _tableName & "_Old]"
                    cmd.ExecuteNonQuery()
                    '
                    cmd.CommandText = "DROP TABLE [" & _tableName & "_Old]"
                    cmd.ExecuteNonQuery()
                End Using
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnColumnsDeleted(_tableName, {columnName})
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            '_parentDatabase.OnColumnDeleted(_tableName, columnName)
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub

        Protected Overrides Sub EditDatabaseCells(ByVal columnIndices() As Int32, ByVal rowIndices() As Int32, ByVal cellValues() As Object)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using tr As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = tr
                    For i As Int32 = 0 To columnIndices.Count - 1
                        If cmd.Parameters.Contains(_storedColumnNames(columnIndices(i))) = False Then cmd.Parameters.Add(New SQLiteParameter(_storedColumnNames(columnIndices(i))))
                        cmd.Parameters(_storedColumnNames(columnIndices(i))).Value = cellValues(i)
                        'cmd.CommandText = "UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndices(i)) & "]='" & cellValues(i).ToString & "' WHERE rowid=" & _rowIdArray(rowIndices(i))
                        cmd.CommandText = "UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndices(i)) & "]= @" & _storedColumnNames(columnIndices(i)) & " WHERE rowid=" & _rowIdArray(rowIndices(i))
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseCells(ByVal columnNamesToEdit() As String, ByVal rowIndices() As Int32, ByVal cellValues() As Object)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using tr As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = tr
                    For i As Int32 = 0 To columnNamesToEdit.Count - 1
                        If cmd.Parameters.Contains(columnNamesToEdit(i)) = False Then cmd.Parameters.Add(New SQLiteParameter(columnNamesToEdit(i)))
                        cmd.Parameters(columnNamesToEdit(i)).Value = cellValues(i)
                        cmd.CommandText = "UPDATE [" & _tableName & "] SET [" & columnNamesToEdit(i) & "]= @" & columnNamesToEdit(i) & " WHERE rowid=" & _rowIdArray(rowIndices(i)) '"UPDATE [" & _tableName & "] SET [" & columnNamesToEdit(i) & "]='" & cellValues(i).ToString & "' WHERE rowid=" & _rowIdArray(rowIndices(i))
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
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
        Protected Overrides Sub EditDatabaseCell(ByVal columnIndex As Int32, ByVal rowIndex As Int32, ByVal cellValue As Boolean)
            Using cmd As New SQLiteCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue.ToString & "' WHERE rowid=" & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(ByVal columnIndex As Int32, ByVal rowIndex As Int32, ByVal cellValue As Byte)
            Using cmd As New SQLiteCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue.ToString & "' WHERE rowid=" & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(ByVal columnIndex As Int32, ByVal rowIndex As Int32, ByVal cellValue As Short)
            Using cmd As New SQLiteCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue.ToString & "' WHERE rowid=" & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(ByVal columnIndex As Int32, ByVal rowIndex As Int32, ByVal cellValue As Integer)
            Using cmd As New SQLiteCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue.ToString & "' WHERE rowid=" & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(ByVal columnIndex As Int32, ByVal rowIndex As Int32, ByVal cellValue As Long)
            Using cmd As New SQLiteCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue.ToString & "' WHERE rowid=" & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(ByVal columnIndex As Int32, ByVal rowIndex As Int32, ByVal cellValue As Single)
            Using cmd As New SQLiteCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue.ToString & "' WHERE rowid=" & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(ByVal columnIndex As Int32, ByVal rowIndex As Int32, ByVal cellValue As Double)
            Using cmd As New SQLiteCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue.ToString & "' WHERE rowid=" & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(ByVal columnIndex As Int32, ByVal rowIndex As Int32, ByVal cellValue As String)
            'Using cmd As New SQLiteCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue & "' WHERE rowid=" & _rowIdArray(rowIndex), _dbConnection)
            Using cmd As New SQLiteCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]= @" & _storedColumnNames(columnIndex) & " WHERE rowid=" & _rowIdArray(rowIndex), _dbConnection)
                cmd.Parameters.Add(New SQLiteParameter(_storedColumnNames(columnIndex), cellValue))
                cmd.ExecuteNonQuery()
            End Using
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
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: '" & columnName & "' is of type '" & _storedColumnTypes(columnIndex).ToString & "' not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim commandText As String = "UPDATE [" & _tableName & "] SET [" & columnName & "]='"
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    For i As Int32 = 0 To columnData.Count - 1
                        cmd.CommandText = commandText & columnData(i) & "' WHERE rowid=" & _rowIdArray(i)
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                trans.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(ByVal columnName As String, ByVal columnData() As Byte)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: '" & columnName & "' is of type '" & _storedColumnTypes(columnIndex).ToString & "' not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim commandText As String = "UPDATE [" & _tableName & "] SET [" & columnName & "]='"
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    For i As Int32 = 0 To columnData.Count - 1
                        cmd.CommandText = commandText & columnData(i) & "' WHERE rowid=" & _rowIdArray(i)
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                trans.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData()() As Byte)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: '" & columnName & "' is of type '" & _storedColumnTypes(columnIndex).ToString & "' not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim commandText As String = "UPDATE [" & _tableName & "] SET [" & columnName & "]=? WHERE rowid="
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    cmd.Parameters.Add("columnName", DbType.Binary)
                    For i As Int32 = 0 To columnData.Count - 1
                        cmd.CommandText = commandText & _rowIdArray(i)
                        cmd.Parameters(0).Value = columnData(i)
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                trans.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(ByVal columnName As String, ByVal columnData() As Double)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: '" & columnName & "' is of type '" & _storedColumnTypes(columnIndex).ToString & "' not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim commandText As String = "UPDATE [" & _tableName & "] SET [" & columnName & "]='"
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    For i As Int32 = 0 To columnData.Count - 1
                        cmd.CommandText = commandText & columnData(i) & "' WHERE rowid=" & _rowIdArray(i)
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                trans.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(ByVal columnName As String, ByVal columnData() As Int32)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: '" & columnName & "' is of type '" & _storedColumnTypes(columnIndex).ToString & "' not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim commandText As String = "UPDATE [" & _tableName & "] SET [" & columnName & "]='"
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    For i As Int32 = 0 To columnData.Count - 1
                        cmd.CommandText = commandText & columnData(i) & "' WHERE rowid=" & _rowIdArray(i)
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                trans.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(ByVal columnName As String, ByVal columnData() As Int16)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: '" & columnName & "' is of type '" & _storedColumnTypes(columnIndex).ToString & "' not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim commandText As String = "UPDATE [" & _tableName & "] SET [" & columnName & "]='"
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    For i As Int32 = 0 To columnData.Count - 1
                        cmd.CommandText = commandText & columnData(i) & "' WHERE rowid=" & _rowIdArray(i)
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                trans.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(ByVal columnName As String, ByVal columnData() As Int64)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: '" & columnName & "' is of type '" & _storedColumnTypes(columnIndex).ToString & "' not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim commandText As String = "UPDATE [" & _tableName & "] SET [" & columnName & "]='"
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    For i As Int32 = 0 To columnData.Count - 1
                        cmd.CommandText = commandText & columnData(i) & "' WHERE rowid=" & _rowIdArray(i)
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                trans.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(ByVal columnName As String, ByVal columnData() As Single)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: '" & columnName & "' is of type '" & _storedColumnTypes(columnIndex).ToString & "' not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim commandText As String = "UPDATE [" & _tableName & "] SET [" & columnName & "]='"
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    For i As Int32 = 0 To columnData.Count - 1
                        cmd.CommandText = commandText & columnData(i) & "' WHERE rowid=" & _rowIdArray(i)
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                trans.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(ByVal columnName As String, ByVal columnData() As String)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: '" & columnName & "' is of type '" & _storedColumnTypes(columnIndex).ToString & "' not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim commandText As String = "UPDATE [" & _tableName & "] SET [" & columnName & "]=@" & columnName
            'Dim commandText As String = "UPDATE [" & _tableName & "] SET [" & columnName & "]='"
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Parameters.Add(New SQLiteParameter(columnName))
                    cmd.Transaction = trans
                    For i As Int32 = 0 To columnData.Count - 1
                        cmd.Parameters(0).Value = columnData(i)
                        'cmd.CommandText = commandText & columnData(i) & "' WHERE rowid=" & _rowIdArray(i)
                        cmd.CommandText = commandText & " WHERE rowid=" & _rowIdArray(i)
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                trans.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub

        Protected Overloads Overrides Function GetStoredCell(ByVal storedColumnIndex As Int32, ByVal storedRowIndex As Int32) As Object
            Return GetStoredCell(_storedColumnNames(storedColumnIndex), storedRowIndex)
        End Function

        Protected Overloads Overrides Function GetStoredCell(storedColumnName As String, storedRowIndex As Integer) As Object
            Using command As New SQLiteCommand("SELECT [" & storedColumnName & "] FROM [" & _tableName & "] WHERE rowid=" & _rowIdArray(storedRowIndex), _dbConnection)
                Return command.ExecuteScalar()
            End Using
        End Function

        Protected Overrides Function GetStoredCells(storedColumnIndices() As Int32, storedRowIndices() As Int32) As Object()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If wasOpen = False Then _parentDatabase.Open()
            '
            Dim result(storedColumnIndices.Count - 1) As Object
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    '
                    For i As Int32 = 0 To storedColumnIndices.Count - 1
                        cmd.CommandText = "SELECT [" & _storedColumnNames(storedColumnIndices(i)) & "] FROM [" & _tableName & "] WHERE rowid=" & _rowIdArray(storedRowIndices(i))
                        result(i) = cmd.ExecuteScalar
                    Next
                End Using
                trans.Commit()
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
            Return result
        End Function

        Protected Overloads Overrides Function GetStoredColumn(storedColumnIndex As Integer) As Object()
            Return GetStoredColumn(_storedColumnNames(storedColumnIndex))
        End Function

        Protected Overloads Overrides Function GetStoredColumn(storedColumnName As String) As Object()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If wasOpen = False Then _parentDatabase.Open()
            '
            Dim column(_rowIdArray.Count - 1) As Object
            Using cmd As New SQLiteCommand("SELECT [" & storedColumnName & "] FROM [" & _tableName & "]", _dbConnection)
                Using reader As SQLiteDataReader = cmd.ExecuteReader
                    If reader.HasRows Then
                        Dim counter As Int32 = 0
                        While reader.Read
                            column(counter) = reader.Item(storedColumnName)
                            counter += 1
                        End While
                    End If
                End Using
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
            Return column
        End Function

        Protected Overloads Overrides Function GetStoredRow(ByVal storedRowIndex As Int32) As Object()
            Dim row(0) As Object
            Using cmd As New SQLiteCommand("SELECT * FROM [" & _tableName & "] WHERE rowid=" & _rowIdArray(storedRowIndex), _dbConnection)
                Using reader As SQLiteDataReader = cmd.ExecuteReader
                    If reader.HasRows Then
                        While reader.Read
                            ReDim row(reader.FieldCount - 1)
                            'reader.GetValues(row)
                            For i As Int32 = 0 To reader.FieldCount - 1
                                If reader.IsDBNull(i) Then
                                    row(i) = reader.Item(i)
                                Else
                                    row(i) = Convert.ChangeType(reader.Item(i), _storedColumnTypes(i))
                                End If
                            Next
                        End While
                    End If
                End Using
            End Using
            '
            Return row
        End Function

        Protected Overloads Overrides Function GetStoredRow(storedRowIndex As Integer, storedColumnNames() As String) As Object()
            Dim commandString As String = "SELECT [" & storedColumnNames(0) & "]"
            For i As Int32 = 1 To storedColumnNames.Count - 1
                commandString = commandString & ",[" & storedColumnNames(i) & "]"
            Next
            commandString = commandString & " FROM [" & _tableName & "] WHERE rowid=" & _rowIdArray(storedRowIndex)
            '
            Dim row(_storedColumnNames.Count - 1) As Object
            Using cmd As New SQLiteCommand(commandString, _dbConnection)
                Using reader As SQLiteDataReader = cmd.ExecuteReader
                    If reader.HasRows Then
                        While reader.Read
                            reader.GetValues(row) 'not changing type here, decided that the time hit to find the column index for each stored column name isn't worth it.
                        End While
                    End If
                End Using
            End Using
            '
            Return row
        End Function
        Protected Overloads Overrides Function GetStoredRow(storedRowIndex As Integer, storedColumnIndices() As Int32) As Object()
            Dim commandString As String = "SELECT [" & _storedColumnNames(storedColumnIndices(0)) & "]"
            For i As Int32 = 1 To storedColumnIndices.Count - 1
                commandString = commandString & ",[" & _storedColumnNames(storedColumnIndices(i)) & "]"
            Next
            commandString = commandString & " FROM [" & _tableName & "] WHERE rowid=" & _rowIdArray(storedRowIndex)
            '
            Dim row(_storedColumnNames.Count - 1) As Object
            Using cmd As New SQLiteCommand(commandString, _dbConnection)
                Using reader As SQLiteDataReader = cmd.ExecuteReader
                    If reader.HasRows Then
                        While reader.Read
                            'reader.GetValues(row)
                            For i As Int32 = 0 To reader.FieldCount - 1
                                If reader.IsDBNull(i) Then
                                    row(i) = reader.Item(i)
                                Else
                                    row(i) = Convert.ChangeType(reader.Item(i), _storedColumnTypes(storedColumnIndices(i)))
                                End If
                            Next
                        End While
                    End If
                End Using
            End Using
            '
            Return row
        End Function

        Protected Overrides Function GetStoredRows(ByVal startStoredRowIndex As Int32, ByVal endStoredRowIndex As Int32) As List(Of Object())
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim rows As New List(Of Object())
            Dim row(_storedColumnNames.Count() - 1) As Object
            Using trans As SQLiteTransaction = _dbConnection.BeginTransaction
                Using cmd As SQLiteCommand = _dbConnection.CreateCommand '(CommandString, _DBConnection)
                    cmd.Transaction = trans
                    For i As Int32 = startStoredRowIndex To endStoredRowIndex
                        cmd.CommandText = "SELECT * FROM [" & _tableName & "] WHERE rowid=" & _rowIdArray(i)
                        Using reader As SQLiteDataReader = cmd.ExecuteReader
                            If reader.HasRows Then
                                While reader.Read
                                    ReDim row(reader.FieldCount - 1)
                                    'reader.GetValues(row)
                                    For j As Int32 = 0 To reader.FieldCount - 1
                                        If reader.IsDBNull(j) Then
                                            row(j) = reader.Item(j)
                                        Else
                                            row(j) = Convert.ChangeType(reader.Item(j), _storedColumnTypes(j))
                                        End If
                                    Next
                                    rows.Add(row)
                                End While
                            End If
                        End Using
                    Next
                End Using
                trans.Commit()
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
            Return rows
        End Function


    End Class

End Class
