Imports System.Data.OleDb
Imports ADOX
Imports System.Data
Public Class MdbReader
    'Inherits DataTableView
    Inherits DatabaseManager
    Private ReadOnly _dbConnection As OleDbConnection
    'Private _TableNames() As String
    'Private _CurrentTableName As String
    'Private _CurrentAutoIncrementColumn As String
    'Private _AutoIncrementSeed As Long
    'Private _AutoIncrementStep As Long
    'Public ReadOnly Property CurrentTableName As String
    '    Get
    '        Return _CurrentTableName
    '    End Get
    'End Property
    'Public ReadOnly Property DBConnection As OleDbConnection
    '    Get
    '        Return _DBConnection
    '    End Get
    'End Property
    Public Shared Sub CreateMdbFile(ByVal filePath As String, Optional ByVal overwrite As Boolean = True)
        If IO.File.Exists(filePath) Then
            If overwrite = True Then
                IO.File.Delete(filePath)
            Else
                Throw New Exception("File named '" & IO.Path.GetFileName(filePath) & "' already exists in the specified directory.")
            End If
        End If
        Dim cat As New Catalog
        Try
            cat.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & filePath)
            cat = Nothing
        Catch ex As Exception
        Finally
            cat = Nothing
        End Try
    End Sub
    Public Sub New(ByVal dataBaseFile As String)
        If IO.Path.GetExtension(dataBaseFile) <> ".mdb" Then Throw New Exception("Provided database file, " & IO.Path.GetFileName(dataBaseFile) & " is not an access .mdb file.")
        '
        _dataBasePath = dataBaseFile
        _dbConnection = New OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;Data Source=" & dataBaseFile)
        _tableNames = GetTableNames()
    End Sub
    Public Overrides Function GetTableNames() As String()
        Dim wasOpen As Boolean = _dataBaseOpen
        If _dataBaseOpen = False Then Open()
        'We only want user tables, not system tables
        Dim restrictions(3) As String
        restrictions(3) = "Table"
        Dim tables As DataTable = _dbConnection.GetSchema("Tables", restrictions)
        Dim result(tables.Rows.Count - 1) As String
        For i As Int32 = 0 To result.Count - 1
            result(i) = tables.Rows(i)(2).ToString
        Next
        If wasOpen = False Then Close()
        Return result
    End Function
    Public Overrides Sub Open()
        _dbConnection.Open()
        _dataBaseOpen = True
    End Sub
    Public Overrides Sub Close()
        _dbConnection.Close()
        _dataBaseOpen = False
    End Sub
    Public Overrides Function GetTableManager(tableName As String) As DataTableView
        Return New MdbTableReader(Me, tableName, _dbConnection)
    End Function
    Public Sub DeleteTable(tableName As String)
        If _tableNames.Contains(tableName) = False Then Exit Sub
        Dim wasOpen As Boolean = _dataBaseOpen
        If _dataBaseOpen = False Then Open()
        '
        Using cmd As OleDbCommand = _dbConnection.CreateCommand
            cmd.CommandText = "DROP TABLE [" & tableName & "]"
            cmd.ExecuteNonQuery()
        End Using
        _tableNames = GetTableNames()
        If wasOpen = False Then Close()
    End Sub
    Public Overrides Function GetStoredNumberOfRows(tableName As String) As Long
        Dim wasOpen As Boolean = _dataBaseOpen
        If _dataBaseOpen = False Then Open()
        Dim createcmd As New OleDbCommand("SELECT COUNT(*) From " & tableName, _dbConnection)
        Dim nRows As Long = CLng(createcmd.ExecuteScalar)
        If wasOpen = False Then Close()
        Return nRows
    End Function
    Public Overrides Function GetStoredNumberOfColumns(tableName As String) As Int32
        Dim wasOpen As Boolean = _dataBaseOpen
        If _dataBaseOpen = False Then Open()
        '
        Dim columnCount As Int32
        Using theOleDbcommand As New OleDbCommand("SELECT * FROM " & tableName, _dbConnection)
            Using theAdapter As New OleDbDataAdapter(theOleDbcommand)
                Using theSchemaTable As New DataTable
                    theAdapter.FillSchema(theSchemaTable, SchemaType.Source)
                    columnCount = theSchemaTable.Columns.Count
                End Using
            End Using
        End Using
        '
        If wasOpen = False Then Close()
        Return columnCount
    End Function

    Private Class MdbTableReader
        Inherits DataTableView

        Private _rowIdArray() As Int32
        Private ReadOnly _dbConnection As OleDbConnection
        Private ReadOnly _autoIncrementColumn As String
        Public Sub New(reader As MdbReader, dataTableName As String, dbConnection As OleDbConnection)
            _tableName = dataTableName
            _dbConnection = dbConnection
            _parentDatabase = reader
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            _storedNumberOfRows = GetStoredRowCount()
            InitializeView()
            '_columnNames = GetStoredColumnNames()
            '_columnTypes = GetStoredColumnTypes()
            ''_nColumns = _storedColumnNames.Count
            '_nRows = _storedNumberOfRows
            'mdb specific
            _rowIdArray = GetRowIdArray()
            _autoIncrementColumn = GetAutoIncrementColumn()
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

#Region "Define Table for function calls"
        'Public Sub SetTableReader(ByVal TableName As String)
        '    _CurrentTableName = TableName
        '    _NRows = GetStoredRowCount(_CurrentTableName)
        '    SetColumnInfo(_CurrentTableName)
        '    _NColumns = _ColumnNames.Count
        '    If _CurrentAutoIncrementColumn = "" Then
        '        AddAutoIncrementColumn(_CurrentTableName)
        '    End If
        '    '
        '    Close()
        'End Sub
        Protected Overrides Function GetStoredRowCount() As ULong
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Dim createcmd As New OleDbCommand("SELECT COUNT(*) From " & _tableName, _dbConnection)
            Return CULng(createcmd.ExecuteScalar)
        End Function
        Protected Overrides Function GetStoredColumnNames() As String()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim result() As String
            Using theOleDbcommand As New OleDbCommand("SELECT * FROM " & _tableName, _dbConnection)
                Using theAdapter As New OleDbDataAdapter(theOleDbcommand)
                    Using theSchemaTable As New DataTable
                        theAdapter.FillSchema(theSchemaTable, SchemaType.Source)
                        ReDim result(theSchemaTable.Columns.Count - 1)
                        Dim counter As Int32 = 0
                        For Each col As DataColumn In theSchemaTable.Columns
                            result(counter) = col.ColumnName
                            counter += 1
                        Next
                    End Using
                    '
                End Using
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
            Return result
        End Function
        Protected Overrides Function GetStoredColumnTypes() As Type()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim result() As Type
            Using theOleDbcommand As New OleDbCommand("SELECT * FROM " & _tableName, _dbConnection)
                Using theAdapter As New OleDbDataAdapter(theOleDbcommand)
                    Using theSchemaTable As New DataTable
                        theAdapter.FillSchema(theSchemaTable, SchemaType.Source)
                        ReDim result(theSchemaTable.Columns.Count - 1)
                        Dim counter As Int32 = 0
                        For Each col As DataColumn In theSchemaTable.Columns
                            result(counter) = col.DataType
                            counter += 1
                        Next
                    End Using
                    '
                End Using
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
            Return result
        End Function
        Private Function GetRowIdArray() As Integer()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Dim autoIncrColumn As String = GetAutoIncrementColumn()
            If autoIncrColumn = "" Then
                AddAutoIncrementColumn()
            Else
                'Create Indexes if they don't already exist
                Dim restrictions(4) As String
                Dim createAutoIndex As Boolean = True
                Try
                    'Using theOleDbcommand As New OleDbCommand("SELECT * FROM " & _tableName, _dbConnection)
                    'Using theAdapter As New OleDbDataAdapter(theOleDbcommand)
                    restrictions(4) = _tableName
                    Using theSchemaTable As DataTable = _dbConnection.GetSchema("INDEXES", restrictions)
                        If theSchemaTable.Rows.Count > 0 Then
                            For Each row As DataRow In theSchemaTable.Rows
                                If row("COLUMN_NAME").ToString() = autoIncrColumn Then
                                    createAutoIndex = False
                                    Exit For
                                End If
                            Next
                        End If
                        If createAutoIndex = True Then
                            Using addIndexcommand As New OleDbCommand("CREATE INDEX IDIndex ON " & _tableName & " (" & autoIncrColumn & ")", _dbConnection)
                                addIndexcommand.ExecuteNonQuery()
                            End Using
                        End If
                    End Using
                    'End Using
                    'End Using
                Catch
                    'MsgBox("Error with indexes")
                End Try
            End If
            '
            Dim column(GetStoredRowCount() - 1) As Int32
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, autoIncrColumn)
            Using cmd As New OleDbCommand("SELECT [" & autoIncrColumn & "] FROM [" & _tableName & "]", _dbConnection)
                Using reader As OleDbDataReader = cmd.ExecuteReader
                    If reader.HasRows Then
                        Dim counter As Int32 = 0
                        While reader.Read
                            column(counter) = reader.GetInt32(columnIndex)
                            counter += 1
                        End While
                    End If
                End Using
            End Using

            If wasOpen = False Then _parentDatabase.Close()
            Return column

        End Function
        Private Function GetAutoIncrementColumn() As String
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim autoIncrementColumn As String = ""
            Using theOleDbcommand As New OleDbCommand("SELECT * FROM " & _tableName, _dbConnection)
                Using theAdapter As New OleDbDataAdapter(theOleDbcommand)
                    Using theSchemaTable As New DataTable
                        theAdapter.FillSchema(theSchemaTable, SchemaType.Source)
                        For Each col As DataColumn In theSchemaTable.Columns
                            If col.AutoIncrement = True Then autoIncrementColumn = col.ColumnName
                        Next
                    End Using
                    '
                End Using
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
            Return autoIncrementColumn
        End Function
        'Private Sub SetColumnInfo(ByVal TableName As String)
        '    If _ParentDatabase.DataBaseOpen = False Then _ParentDatabase.Open()
        '    Dim theOleDBcommand As New OleDbCommand("SELECT * FROM " & TableName, _DBConnection)
        '    Dim theAdapter As New OleDbDataAdapter(theOleDBCommand)

        '    Dim theSchemaTable As New DataTable
        '    theAdapter.FillSchema(theSchemaTable, SchemaType.Source)
        '    Dim Found As Int32 = 0
        '    ReDim _ColumnNames(theSchemaTable.Columns.Count - 1)
        '    ReDim _ColumnTypes(_ColumnNames.Count - 1)
        '    Dim counter As Int32 = 0
        '    For Each col As DataColumn In theSchemaTable.Columns
        '        _ColumnNames(Counter) = col.ColumnName
        '        _ColumnTypes(Counter) = col.DataType
        '        If col.AutoIncrement Then
        '            Found += 1
        '            _CurrentAutoIncrementColumn = col.ColumnName
        '            _AutoIncrementSeed = col.AutoIncrementSeed
        '            _AutoIncrementStep = col.AutoIncrementStep
        '        End If
        '        Counter += 1
        '    Next
        '    '
        '    If Found <> 1 Then _CurrentAutoIncrementColumn = ""
        '    If Found = 1 Then
        '        'Create Indexes if they don't already exist
        '        Dim restrictions(4) As String
        '        theSchemaTable = New DataTable
        '        theOleDBCommand = New OleDbCommand
        '        Dim CreateAutoIndex As Boolean = True
        '        Try
        '            restrictions(4) = TableName
        '            theSchemaTable = _DBConnection.GetSchema("INDEXES", restrictions)
        '            If theSchemaTable.Rows.Count > 0 Then
        '                For Each row As DataRow In theSchemaTable.Rows
        '                    If row("COLUMN_NAME").ToString() = _CurrentAutoIncrementColumn Then
        '                        CreateAutoIndex = False
        '                        Exit For
        '                    End If
        '                Next
        '            End If
        '            If CreateAutoIndex = True Then
        '                theOleDBCommand = New OleDbCommand("CREATE INDEX IDIndex ON " & TableName & " (" & _CurrentAutoIncrementColumn & ")", _DBConnection)
        '                theOleDBCommand.ExecuteNonQuery()
        '            End If
        '        Catch
        '            'MsgBox("Error with indexes")
        '        End Try

        '        Dim columnNamesWithoutAuto As New List(Of String)
        '        Dim columnTypesWithoutAuto As New List(Of Type)
        '        For i As Int32 = 0 To _ColumnNames.Count - 1
        '            If _ColumnNames(i) = _CurrentAutoIncrementColumn Then
        '                '
        '            Else
        '                ColumnNamesWithoutAuto.Add(_ColumnNames(i))
        '                ColumnTypesWithoutAuto.Add(_ColumnTypes(i))
        '            End If
        '        Next
        '        _ColumnNames = ColumnNamesWithoutAuto.ToArray
        '        _ColumnTypes = ColumnTypesWithoutAuto.ToArray
        '    End If
        '    '
        '    theAdapter.Dispose()
        '    theOleDBCommand.Dispose()
        '    theSchemaTable.Dispose()

        '    'Dim Restrictions As String() = New String() {Nothing, Nothing, TableName, Nothing}
        '    'Dim columnNames As New List(Of String)
        '    'Dim columnTypes As New List(Of Type)
        '    'Dim DT As DataTable = _DBConnection.GetSchema("Columns", Restrictions)
        '    'For Each DR As DataRow In DT.Rows
        '    '    StoredColumnNames.Add(DR("Column_Name").ToString)
        '    '    StoredColumnTypes.Add(oleDbToNetTypeConverter(DR("Data_Type")))
        '    'Next
        '    '_ColumnNames = StoredColumnNames.ToArray
        '    '_ColumnTypes = StoredColumnTypes.ToArray
        '    'DT.Dispose()
        'End Sub
        Private Sub AddAutoIncrementColumn()
            Dim possibleAutoNames As String() = New String() {"ID", "OID", "OBJECTID", "COUNTER", "ROWID", "AUTOID", "ROWINDEX", "TAG", "ID_1", "ID_2", "ID_3", "OID_1", "OID_2", "OID_3"}
            Dim chosenAutoName As String = ""
            For i As Int32 = 0 To possibleAutoNames.Count - 1
                If _storedColumnNames.Contains(possibleAutoNames(i)) = False Then
                    chosenAutoName = possibleAutoNames(i)
                    Exit For
                End If
            Next

            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Try
                Dim commandString As String = "ALTER TABLE " & _tableName & " ADD COLUMN " & chosenAutoName & " IDENTITY" 'INT NOT NULL AUTO_INCREMENT, ADD PRIMARY KEY (" & ChosenAutoName & ")"
                Using command As New OleDbCommand(commandString, _dbConnection)
                    command.ExecuteNonQuery()
                    '_CurrentAutoIncrementColumn = chosenAutoName
                    '
                    command.CommandText = "CREATE INDEX IDIndex ON " & _tableName & " (" & chosenAutoName & ")"
                    command.ExecuteNonQuery()
                End Using
                If wasOpen = False Then _parentDatabase.Close()
            Catch ex As Exception
                If ex.Message.Contains("Increase MaxLocksPerFile") Then
                    Try
                        Dim adodbConn As New ADODB.Connection
                        adodbConn.Open("Provider=Microsoft.Jet.OleDb.4.0;Data Source=" & _parentDatabase.DataBasePath)
                        adodbConn.Properties("Jet OLEDB:Max Locks Per File").Value = 64000
                        adodbConn.Execute("ALTER TABLE " & _tableName & " ADD COLUMN " & chosenAutoName & " IDENTITY")
                        adodbConn.Close()
                        '
                        Using command As New OleDbCommand("CREATE INDEX IDIndex ON " & _tableName & " (" & chosenAutoName & ")", _dbConnection)
                            command.ExecuteNonQuery()
                        End Using
                        If wasOpen = False Then _parentDatabase.Close()
                    Catch exLocks As Exception
                        If wasOpen = False Then _parentDatabase.Close()
                        Throw exLocks
                    End Try
                Else
                    If wasOpen = False Then _parentDatabase.Close()
                    Throw ex
                End If

            End Try

        End Sub
#End Region

#Region "Column Stuff"
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
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using command As New OleDbCommand("ALTER TABLE " & _tableName & " ADD COLUMN [" & columnName & "] LONGBINARY", _dbConnection)
                command.ExecuteNonQuery()
            End Using
            '
            EditDatabaseColumn(columnName, columnData)
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Boolean))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Boolean)
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using command As New OleDbCommand("ALTER TABLE " & _tableName & " ADD COLUMN [" & columnName & "] LOGICAL", _dbConnection)
                command.ExecuteNonQuery()
            End Using
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As New OleDbCommand()
                    command.Transaction = tr
                    command.Connection = _dbConnection
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]=" & columnData(i) & " WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                '
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Boolean))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Byte)
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using command As New OleDbCommand("ALTER TABLE " & _tableName & " ADD COLUMN [" & columnName & "] BYTE", _dbConnection)
                command.ExecuteNonQuery()
            End Using
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As New OleDbCommand()
                    command.Transaction = tr
                    command.Connection = _dbConnection
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]=" & columnData(i) & " WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                '
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Byte))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Double)
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using command As New OleDbCommand("ALTER TABLE " & _tableName & " ADD COLUMN [" & columnName & "] DOUBLE", _dbConnection)
                command.ExecuteNonQuery()
            End Using
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As New OleDbCommand()
                    command.Transaction = tr
                    command.Connection = _dbConnection
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]=" & columnData(i) & " WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                '
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Double))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Integer)
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using command As New OleDbCommand("ALTER TABLE " & _tableName & " ADD COLUMN [" & columnName & "] INT", _dbConnection)
                command.ExecuteNonQuery()
            End Using
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As New OleDbCommand()
                    command.Transaction = tr
                    command.Connection = _dbConnection
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]=" & columnData(i) & " WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                '
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Integer))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Long)
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using command As New OleDbCommand("ALTER TABLE " & _tableName & " ADD COLUMN [" & columnName & "] INTEGER8", _dbConnection)
                command.ExecuteNonQuery()
            End Using
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As New OleDbCommand()
                    command.Transaction = tr
                    command.Connection = _dbConnection
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]=" & columnData(i) & " WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                '
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Long))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Short)
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using command As New OleDbCommand("ALTER TABLE " & _tableName & " ADD COLUMN [" & columnName & "] SHORT", _dbConnection)
                command.ExecuteNonQuery()
            End Using
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As New OleDbCommand()
                    command.Transaction = tr
                    command.Connection = _dbConnection
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]=" & columnData(i) & " WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                '
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Short))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Single)
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using command As New OleDbCommand("ALTER TABLE " & _tableName & " ADD COLUMN [" & columnName & "] SINGLE", _dbConnection)
                command.ExecuteNonQuery()
            End Using
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As New OleDbCommand()
                    command.Transaction = tr
                    command.Connection = _dbConnection
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]=" & columnData(i) & " WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                '
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Single))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As String)
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Column Name " & columnName & " already exists, choose a different name.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("Number of records do not match the number of records for the new column.")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using command As New OleDbCommand("ALTER TABLE " & _tableName & " ADD COLUMN [" & columnName & "] Text(50)", _dbConnection)
                command.ExecuteNonQuery()
            End Using
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As New OleDbCommand()
                    command.Transaction = tr
                    command.Connection = _dbConnection
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]='" & columnData(i) & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                '
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(String))
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub

        Protected Overrides Sub DeleteColumnFromDatabase(columnName As String)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using command As New OleDbCommand("ALTER TABLE " & _tableName & " DROP COLUMN [" & columnName & "]", _dbConnection)
                command.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnColumnsDeleted(_tableName, {columnName})
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub DeleteColumnsFromDatabase(columnsToDelete() As String)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As OleDbCommand = _dbConnection.CreateCommand()
                    command.Transaction = tr
                    For i As Int32 = 0 To columnsToDelete.Count - 1
                        command.CommandText = "ALTER TABLE " & _tableName & " DROP COLUMN [" & columnsToDelete(i) & "]"
                        command.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnColumnsDeleted(_tableName, columnsToDelete)
            _storedColumnNames = GetStoredColumnNames()
            _storedColumnTypes = GetStoredColumnTypes()
            If wasOpen = False Then _parentDatabase.Close()
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
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex < 0 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & "not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As OleDbCommand = _dbConnection.CreateCommand
                    command.Transaction = tr
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]='" & columnData(i) & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Byte)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex < 0 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & "not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As OleDbCommand = _dbConnection.CreateCommand
                    command.Transaction = tr
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]='" & columnData(i) & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
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
            Using trans As OleDbTransaction = _dbConnection.BeginTransaction
                Using cmd As OleDbCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    cmd.Parameters.Add("columnName", OleDbType.Binary)
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
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Short)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex < 0 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & "not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As OleDbCommand = _dbConnection.CreateCommand
                    command.Transaction = tr
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]='" & columnData(i) & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Int32)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex < 0 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & "not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As OleDbCommand = _dbConnection.CreateCommand
                    command.Transaction = tr
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]='" & columnData(i) & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Long)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex < 0 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & "not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As OleDbCommand = _dbConnection.CreateCommand
                    command.Transaction = tr
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]='" & columnData(i) & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Double)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex < 0 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & "not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As OleDbCommand = _dbConnection.CreateCommand
                    command.Transaction = tr
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]='" & columnData(i) & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Single)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex < 0 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & "not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As OleDbCommand = _dbConnection.CreateCommand
                    command.Transaction = tr
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]='" & columnData(i) & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As String)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex < 0 Then Throw New Exception("The desired Field Name: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Field: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & "not of type '" & columnData(0).GetType.ToString & "'.")
            If _rowIdArray.Count <> columnData.Count Then Throw New Exception("The table '" & _tableName & "' does not have: " & columnData.Count & " records")
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As OleDbCommand = _dbConnection.CreateCommand
                    command.Transaction = tr
                    For i As Int32 = 0 To columnData.Count - 1
                        command.CommandText = "UPDATE " & _tableName & " SET [" & columnName & "]='" & columnData(i) & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        command.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub

        Protected Overrides Function GetStoredColumn(storedColumnName As String) As Object()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim column(_rowIdArray.Count - 1) As Object
            Using cmd As New OleDbCommand("SELECT [" & storedColumnName & "] FROM [" & _tableName & "]", _dbConnection)
                Using reader As OleDbDataReader = cmd.ExecuteReader
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
        Protected Overrides Function GetStoredColumn(storedColumnIndex As Int32) As Object()
            Return GetStoredColumn(_storedColumnNames(storedColumnIndex))
        End Function
#End Region

#Region "Row Stuff"
        Protected Overrides Sub AddRowToDatabase(row() As Object)
            'Error Checking
            If row.Count <> _storedColumnNames.Count Then Throw New Exception("Number of columns that you are trying to add do not match the number of columns in the database.")
            For i As Int32 = 0 To row.Count - 1
                If ConvertToColumnType(_storedColumnTypes(i), row(i)) = False Then Throw New Exception("Column type '" & row(i).GetType.ToString & "' does not match for column '" & _storedColumnNames(i) & "'.")
            Next
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()

            Dim insertString As New Text.StringBuilder
            insertString.Append("INSERT INTO [" & _tableName & "] (")
            insertString.Append(_storedColumnNames(0))
            For i As Int32 = 1 To _storedColumnNames.Count - 1
                insertString.Append("," & _storedColumnNames(i))
            Next
            insertString.Append(") VALUES (")
            insertString.Append("?")
            For i As Int32 = 1 To _storedColumnNames.Count - 1
                insertString.Append(",?")
            Next
            insertString.Append(")")
            '
            Using command As New OleDbCommand(insertString.ToString, _dbConnection)
                For i As Int32 = 0 To _storedColumnNames.Count - 1
                    command.Parameters.AddWithValue(_storedColumnNames(i), row(i))
                Next
                '
                command.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnRowsAdded(_tablename, New List(Of Object())({row}))
            _rowIdArray = GetRowIdArray()
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
                    If ConvertToColumnType(_storedColumnTypes(j), newRowData(i)(j)) = False Then Throw New Exception("Column type '" & newRowData(i)(j).GetType.ToString & "' does not match for column '" & _storedColumnNames(i) & "' in row " & i.ToString & ".")
                Next
            Next
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim insertString As New Text.StringBuilder
            insertString.Append("INSERT INTO [" & _tableName & "] (")
            insertString.Append(_storedColumnNames(0))
            For i As Int32 = 1 To _storedColumnNames.Count - 1
                insertString.Append("," & _storedColumnNames(i))
            Next
            insertString.Append(") VALUES (")
            insertString.Append("?")
            For i As Int32 = 1 To _storedColumnNames.Count - 1
                insertString.Append(",?")
            Next
            insertString.Append(")")
            '
            Using trans As OleDbTransaction = _dbConnection.BeginTransaction
                Using command As New OleDbCommand(insertString.ToString, _dbConnection, trans)
                    For i As Int32 = 0 To _storedColumnNames.Count - 1
                        command.Parameters.Add(New OleDbParameter(_storedColumnNames(i), _storedColumnTypes(i)))
                    Next
                    For i As Int32 = 0 To newRowData.Count - 1
                        For j As Int32 = 0 To newRowData(i).Count - 1
                            command.Parameters(j).Value = newRowData(i)(j)
                        Next
                        command.ExecuteNonQuery()
                    Next
                End Using
                '
                trans.Commit()
            End Using
            '
            '_parentDatabase.OnRowsAdded(_tableName, newRowData)
            _rowIdArray = GetRowIdArray()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub

        Protected Overrides Sub DeleteRowFromDatabase(rowIndex As Integer)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using command As New OleDbCommand("DELETE FROM [" & _tableName & "] WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndex), _dbConnection)
                command.ExecuteNonQuery()
            End Using
            '
            '_parentDatabase.OnRowsDeleted(_tableName, {rowIndex})
            _rowIdArray = GetRowIdArray()
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub DeleteRowsFromDatabase(rowIndices() As Integer)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Using tr As OleDbTransaction = _dbConnection.BeginTransaction
                Using cmd As OleDbCommand = _dbConnection.CreateCommand
                    cmd.Transaction = tr
                    For i As Int32 = 0 To rowIndices.Count - 1
                        cmd.CommandText = "DELETE FROM [" & _tableName & "] WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndices(i))
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                tr.Commit()
            End Using
            '
            '_parentDatabase.OnRowsDeleted(_tableName, rowIndices)
            _rowIdArray = GetRowIdArray()
            '
            If wasOpen = False Then _parentDatabase.Close()
        End Sub

        Protected Overrides Function GetStoredRow(storedRowIndex As Integer) As Object()
            Dim row(_storedColumnNames.Count - 1) As Object
            Using cmd As New OleDbCommand("SELECT * FROM [" & _tableName & "] WHERE " & _autoIncrementColumn & " = " & _rowIdArray(storedRowIndex), _dbConnection)
                Using reader As OleDbDataReader = cmd.ExecuteReader
                    If reader.HasRows Then
                        While reader.Read
                            reader.GetValues(row)
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
            commandString = commandString & " FROM [" & _tableName & "] WHERE " & _autoIncrementColumn & " = " & _rowIdArray(storedRowIndex)
            '
            Dim row(storedColumnNames.Count - 1) As Object
            Using cmd As New OleDbCommand(commandString, _dbConnection)
                Using reader As OleDbDataReader = cmd.ExecuteReader
                    If reader.HasRows Then
                        While reader.Read
                            reader.GetValues(row)
                        End While
                    End If
                End Using
            End Using
            '
            Return row
        End Function
        Protected Overloads Overrides Function GetStoredRow(storedRowIndex As Integer, storedColumnIndices() As Int32) As Object()
            Dim columns(storedColumnIndices.Count - 1) As String
            For i As Int32 = 0 To storedColumnIndices.Count - 1
                columns(i) = _storedColumnNames(storedColumnIndices(i))
            Next
            Return GetStoredRow(storedRowIndex, columns)
        End Function

        Protected Overrides Function GetStoredRows(ByVal startStoredRowIndex As Int32, ByVal endStoredRowIndex As Int32) As List(Of Object())
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim rows As New List(Of Object())
            Dim row(0) As Object
            Using trans As OleDbTransaction = _dbConnection.BeginTransaction
                Using cmd As OleDbCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    For i As Int32 = startStoredRowIndex To endStoredRowIndex
                        cmd.CommandText = "SELECT * FROM [" & _tableName & "] WHERE " & _autoIncrementColumn & " = " & _rowIdArray(i)
                        Using reader As OleDbDataReader = cmd.ExecuteReader
                            If reader.HasRows Then
                                While reader.Read
                                    ReDim row(reader.FieldCount - 1)
                                    reader.GetValues(row)
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
#End Region

#Region "Cell Stuff"
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
            Using cmd As New OleDbCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Byte)
            Using cmd As New OleDbCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Short)
            Using cmd As New OleDbCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Int32)
            Using cmd As New OleDbCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Int64)
            Using cmd As New OleDbCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Double)
            Using cmd As New OleDbCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Single)
            Using cmd As New OleDbCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As String)
            Using cmd As New OleDbCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]='" & cellValue & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndex), _dbConnection)
                cmd.ExecuteNonQuery()
            End Using
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Integer, cellValue As Byte())
            Using cmd As New OleDbCommand("UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndex) & "]=? WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndex), _dbConnection)
                cmd.Parameters(0).Value = cellValue
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Protected Overrides Sub EditDatabaseCells(columnNamesToEdit() As String, rowIndices() As Integer, cellValues() As Object)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using trans As OleDbTransaction = _dbConnection.BeginTransaction
                Using cmd As OleDbCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    For i As Int32 = 0 To columnNamesToEdit.Count - 1
                        cmd.CommandText = "UPDATE [" & _tableName & "] SET [" & columnNamesToEdit(i) & "]='" & cellValues(i).ToString & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndices(i))
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                trans.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub
        Protected Overrides Sub EditDatabaseCells(columnIndices() As Int32, rowIndices() As Integer, cellValues() As Object)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Using trans As OleDbTransaction = _dbConnection.BeginTransaction
                Using cmd As OleDbCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    For i As Int32 = 0 To columnIndices.Count - 1
                        cmd.CommandText = "UPDATE [" & _tableName & "] SET [" & _storedColumnNames(columnIndices(i)) & "]='" & cellValues(i).ToString & "' WHERE " & _autoIncrementColumn & " = " & _rowIdArray(rowIndices(i))
                        cmd.ExecuteNonQuery()
                    Next
                End Using
                trans.Commit()
            End Using
            If wasOpen = False Then _parentDatabase.Close()
        End Sub

        Protected Overrides Function GetStoredCell(storedColumnName As String, storedRowIndex As Integer) As Object
            Using command As New OleDbCommand("SELECT " & storedColumnName & " FROM " & _tableName & " WHERE " & _autoIncrementColumn & " = " & _rowIdArray(storedRowIndex), _dbConnection)
                Return command.ExecuteScalar()
            End Using
        End Function
        Protected Overrides Function GetStoredCell(storedColumnIndex As Int32, storedRowIndex As Integer) As Object
            Using command As New OleDbCommand("SELECT " & _storedColumnNames(storedColumnIndex) & " FROM " & _tableName & " WHERE " & _autoIncrementColumn & " = " & _rowIdArray(storedRowIndex), _dbConnection)
                Return command.ExecuteScalar()
            End Using
        End Function
        Protected Overrides Function GetStoredCells(storedColumnIndices() As Int32, storedRowIndices() As Int32) As Object()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Dim result(storedColumnIndices.Count - 1) As Object
            Using trans As OleDbTransaction = _dbConnection.BeginTransaction
                Using cmd As OleDbCommand = _dbConnection.CreateCommand
                    cmd.Transaction = trans
                    '
                    For i As Int32 = 0 To storedColumnIndices.Count - 1
                        cmd.CommandText = "SELECT [" & _storedColumnNames(storedColumnIndices(i)) & "] FROM [" & _tableName & "] WHERE " & _autoIncrementColumn & " = " & _rowIdArray(storedRowIndices(i))
                        result(i) = cmd.ExecuteScalar
                    Next
                End Using
                trans.Commit()
            End Using
            '
            If wasOpen = False Then _parentDatabase.Close()
            Return result
        End Function
#End Region

        'Public Sub ExecuteCommandNonQuery(ByVal command As OleDbCommand)
        '    Command.ExecuteNonQuery()
        'End Sub


    End Class

    Public Sub CreateTable(ByVal name As String, ByVal tableColumnNames() As String, ByVal tableColumnTypes() As Type, Optional ByVal autoIncrementColumn As String = Nothing)
        Dim leaveOpen As Boolean = DataBaseOpen
        If DataBaseOpen = False Then Open()
        Dim sb As New Text.StringBuilder(500)
        sb.Append("Create Table [").Append(name).Append("] (")
        'Dim HasAutofield As Int16 = 0
        If Not IsNothing(autoIncrementColumn) Then
            sb.Append("[").Append(autoIncrementColumn).Append("]").Append(" ")
            sb.Append("IDENTITY")
            sb.Append(",")
        End If
        For i As Int32 = 0 To tableColumnNames.Count - 1
            sb.Append("[").Append(tableColumnNames(i)).Append("]").Append(" ")
            Select Case tableColumnTypes(i)
                Case GetType(String) : sb.Append("Text(50)")
                Case GetType(DateTime) : sb.Append("DATETIME")
                Case GetType(Byte), GetType(SByte) : sb.Append("BYTE")
                Case GetType(Int16), GetType(UInt16) : sb.Append("SHORT")
                Case GetType(Int32), GetType(UInt32) : sb.Append("INT")
                Case GetType(Single) : sb.Append("SINGLE")
                Case GetType(Double) : sb.Append("DOUBLE")
                Case GetType(Decimal) : sb.Append("NUMBER")
                Case GetType(Char) : sb.Append("Text(1)")
                Case GetType(Boolean) : sb.Append("LOGICAL")
                Case GetType(Object) : sb.Append("LONGBINARY")
                Case GetType(Byte()) : sb.Append("LONGBINARY")
                Case Else
                    Throw New Exception(tableColumnTypes(i).ToString & " Not implemented, Column: " & tableColumnNames(i))
            End Select
            sb.Append(",")
        Next
        sb.Chars(sb.Length - 1) = ")"
        '
        Try
            Dim createcmd As New OleDbCommand(sb.ToString, _dbConnection)
            createcmd.ExecuteNonQuery()
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        If leaveOpen = False Then Close()
        _tableNames = GetTableNames()
    End Sub
    Public Sub SaveDataTableToDataBase(ByVal name As String, ByVal dt As DataTable)
        Dim reOpen As Boolean = DataBaseOpen
        If DataBaseOpen = False Then Open()
        Dim sb As New Text.StringBuilder(500)
        sb.Append("Create Table [").Append(name).Append("] (")
        'Dim HasAutofield As Int16 = 0
        For Each c As DataColumn In dt.Columns
            'If c.ColumnName = "OBJECTID" Then
            '    Sb.Append("[").Append(c.ColumnName).Append("]").Append(" "c)
            '    Sb.Append("IDENTITY")
            '    Sb.Append(","c)
            '    HasAutofield = 1
            'Else
            Dim tc = Type.GetTypeCode(c.DataType)
            sb.Append("[").Append(c.ColumnName).Append("]").Append(" "c)
            Select Case tc
                Case TypeCode.String
                    If c.MaxLength > 255 OrElse c.MaxLength < 0 _
                           Then sb.Append("Text(50)") _
                           Else sb.AppendFormat("Text({0})", c.MaxLength)
                Case TypeCode.DateTime : sb.Append("DATETIME")
                Case TypeCode.Int16 : sb.Append("SHORT")
                Case TypeCode.UInt16 : sb.Append("SHORT")
                Case TypeCode.Int32 : sb.Append("INT")
                Case TypeCode.UInt32 : sb.Append("INT")
                Case TypeCode.Single : sb.Append("SINGLE")
                Case TypeCode.Double : sb.Append("DOUBLE")
                Case TypeCode.Decimal : sb.Append("NUMBER")
                Case TypeCode.Char : sb.Append("Text(1)")
                Case TypeCode.Boolean : sb.Append("LOGICAL")
                Case TypeCode.Byte : sb.Append("BYTE")
                Case TypeCode.Object : sb.Append("LONGBINARY")
                Case Else
                    If c.DataType Is GetType(Guid) Then
                        sb.Append("Text(50)")
                    Else
                        Throw New Exception(tc.ToString & " Not implemented, Column: " & c.ColumnName)
                    End If
            End Select
            sb.Append(","c)
            'End If
        Next
        sb.Chars(sb.Length - 1) = ")"c
        '
        Try
            Dim createcmd As New OleDbCommand(sb.ToString, _dbConnection)
            createcmd.ExecuteNonQuery()
            Dim accDataAdapter As New OleDbDataAdapter("SELECT * FROM " & name, _dbConnection)
            Dim accCommandBuilder As New OleDbCommandBuilder(accDataAdapter)
            accDataAdapter.InsertCommand = accCommandBuilder.GetInsertCommand()
            accDataAdapter.Update(dt)
        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
        If reOpen = False Then Close()
        _tableNames = GetTableNames()
    End Sub
    Public Sub RenameTable(ByVal oldName As String, ByVal newName As String)
        Dim reopen As Boolean = _dataBaseOpen
        If _dataBaseOpen = True Then Close()
        Dim cn As New ADODB.Connection
        Dim catalog As New Catalog
        Dim i As Integer

        cn.ConnectionString = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" & _dataBasePath
        cn.Open()

        catalog.ActiveConnection = cn

        For i = 0 To catalog.Tables.Count() - 1
            If catalog.Tables(i).Name = oldName Then
                catalog.Tables(i).Name = newName
                Exit For
            End If
        Next

        cn.Close()

        cn = Nothing
        catalog = Nothing
        If reopen = True Then Open()
        _tableNames = GetTableNames()
    End Sub



    'Public Sub CompactRepair()
    '    Dim reopen As Boolean = _dataBaseOpen
    '    If _dataBaseOpen = True Then Close()
    '    Dim strAccessDatabasePath As String = _dataBasePath
    '    Dim tempAccessDatabasePath As String = IO.Path.GetDirectoryName(_dataBasePath) & "\tmp1.mdb"
    '    Dim lockedDbFileInfo As New IO.FileInfo(strAccessDatabasePath.Replace(".mdb", ".ldb"))
    '    If lockedDbFileInfo.Exists Then
    '        Throw New Exception("Database " & IO.Path.GetFileName(_dataBasePath) & " is currently in use.")
    '    End If

    '    Rename(strAccessDatabasePath, tempAccessDatabasePath)

    'The JRO engine only works currently on 32-bit systems. The reference to JRO has been removed as it was only used to compact the database. This decision shouldn't affect future releases too much
    'since support for access has reduced considerably in lieu on sqlite.
    '    Dim jro As New JRO.JetEngine

    '    jro.CompactDatabase("Provider=Microsoft.Jet.OLEDB.4.0;" & "Data Source=" & tempAccessDatabasePath,
    '                "Provider=Microsoft.Jet.OLEDB.4.0;" & "Data Source=" & strAccessDatabasePath & ";Jet OLEDB:Engine Type=5")
    '    IO.File.Delete(tempAccessDatabasePath)
    '    If reopen = True Then Open()
    'End Sub
    'Private Function oleDbToNetTypeConverter(oleDbTypeNumber As Integer) As Type
    '    Select Case oleDbTypeNumber
    '        Case 0
    '            Return GetType(Nullable)
    '        Case 2
    '            Return GetType(Int16)
    '        Case 3
    '            Return GetType(Int32)
    '        Case 4
    '            Return GetType([Single])
    '        Case 5
    '            Return GetType([Double])
    '        Case 6
    '            Return GetType([Decimal])
    '        Case 7
    '            Return GetType(DateTime)
    '        Case 8
    '            Return GetType([String])
    '        Case 9
    '            Return GetType([Object])
    '        Case 10
    '            Return GetType(Exception)
    '        Case 11
    '            Return GetType([Boolean])
    '        Case 12
    '            Return GetType([Object])
    '        Case 13
    '            Return GetType([Object])
    '        Case 14
    '            Return GetType([Decimal])
    '        Case 16
    '            Return GetType([SByte])
    '        Case 17
    '            Return GetType([Byte])
    '        Case 18
    '            Return GetType(UInt16)
    '        Case 19
    '            Return GetType(UInt32)
    '        Case 20
    '            Return GetType(Int64)
    '        Case 21
    '            Return GetType(UInt64)
    '        Case 64
    '            Return GetType(DateTime)
    '        Case 72
    '            Return GetType(Guid)
    '        Case 128
    '            Return GetType([Byte]())
    '        Case 129
    '            Return GetType([String])
    '        Case 130
    '            Return GetType([String])
    '        Case 131
    '            Return GetType([Decimal])
    '        Case 133
    '            Return GetType(DateTime)
    '        Case 134
    '            Return GetType(TimeSpan)
    '        Case 135
    '            Return GetType(DateTime)
    '        Case 138
    '            Return GetType([Object])
    '        Case 139
    '            Return GetType([Decimal])
    '        Case 200
    '            Return GetType([String])
    '        Case 201
    '            Return GetType([String])
    '        Case 202
    '            Return GetType([String])
    '        Case 203
    '            Return GetType([String])
    '        Case 204
    '            Return GetType([Byte]())
    '        Case 205
    '            Return GetType([Byte]())
    '    End Select
    '    Throw (New Exception("DataType Not Supported"))
    'End Function
End Class



