Imports System.IO

Public MustInherit Class DatabaseManager
    Protected _dataBaseOpen As Boolean = False
    Protected _dataBasePath As String
    Protected _tableNames() As String
    Public ReadOnly Property DataBasePath As String
        Get
            Return _dataBasePath
        End Get
    End Property
    Public ReadOnly Property DataBaseOpen As Boolean
        Get
            Return _dataBaseOpen
        End Get
    End Property
    Public ReadOnly Property TableNames() As String()
        Get
            Return _tableNames
        End Get
    End Property

#Region "Events"
    Public Event EditsSaved(tableName As String, editsSaved As List(Of TableEdit))
    Public Sub OnEditsSaved(tableName As String, editsSaved As List(Of TableEdit))
        RaiseEvent editsSaved(tableName, editsSaved)
    End Sub
    Public Event PreviewEditsSaved(tableName As String, ByRef cancel As Boolean)
    Public Sub OnPreviewEditsSaved(tableName As String, ByRef cancel As Boolean)
        RaiseEvent PreviewEditsSaved(tableName, cancel)
    End Sub
    'Public Event RowsAdded(ByVal tableName As String, ByVal rowData As List(Of Object()))
    'Public Sub OnRowsAdded(ByVal tableName As String, ByVal rowData As List(Of Object()))
    '    RaiseEvent RowsAdded(tableName, rowData)
    'End Sub
    'Public Event RowsDeleted(ByVal tableName As String, ByVal rowIndices() As Int32)
    'Public Sub OnRowsDeleted(ByVal tableName As String, ByVal rowIndices() As Int32)
    '    RaiseEvent RowsDeleted(tableName, rowIndices)
    'End Sub
    'Public Event ColumnAdded(ByVal tableName As String, ByVal columnName As String, ByVal columnType As Type)
    'Public Sub OnColumnAdded(ByVal tableName As String, ByVal columnName As String, ByVal columnType As Type)
    '    RaiseEvent ColumnAdded(tableName, columnName, columnType)
    'End Sub
    'Public Event ColumnsDeleted(ByVal tableName As String, ByVal columnNames() As String)
    'Public Sub OnColumnsDeleted(ByVal tableName As String, ByVal columnNames() As String)
    '    RaiseEvent ColumnsDeleted(tableName, columnNames)
    'End Sub
#End Region



    Public MustOverride Sub Open()
    Public MustOverride Sub Close()
    Public MustOverride Function GetTableManager(ByVal tableName As String) As DataTableView
    Public MustOverride Function GetTableNames() As String()

    ''' <summary>
    ''' Get stored number of rows of a table.
    ''' </summary>
    ''' <param name="tableName">Table name.</param>
    ''' <returns></returns>
    Public MustOverride Function GetStoredNumberOfRows(tableName As String) As Long

    ''' <summary>
    ''' Get stored number of columns of a table.
    ''' </summary>
    ''' <param name="tableName">Table name.</param>
    ''' <returns></returns>
    Public MustOverride Function GetStoredNumberOfColumns(tableName As String) As Int32

    ''' <summary>
    ''' Determines if a type is numeric.  Nullable numeric types are considered numeric.
    ''' </summary>
    ''' <remarks>
    ''' Boolean is not considered numeric.
    ''' http://stackoverflow.com/questions/124411/using-net-how-can-i-determine-if-a-type-is-a-numeric-valuetype
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

    Public Shared Sub ConvertCsvToSqLite(inputFile As String, outputFile As String, outputtableName As String, hasHeaders As Boolean, dataLineStartIndex As Int32, fieldsEnclosedInQuotes As Boolean)
        'If Path.GetExtension(OutputFile).ToLower <> ".sqlite" Then Throw New Exception("Provided database file, " & System.IO.Path.GetFileName(OutputFile) & " is not a SQLite (.sqlite) file.")
        If Path.GetExtension(inputFile).ToLower <> ".csv" Then Throw New Exception("Provided database file, " & Path.GetFileName(outputFile) & " is not a comma separated (.csv) file.")
        If File.Exists(inputFile) = False Then Throw New Exception("Input database file, " & Path.GetFileName(outputFile) & " does not exist.")
        '
        Dim sqLiteEdit As New SQLiteManager(outputFile)
        sqLiteEdit.Open()
        Dim tableNames() As String = sqLiteEdit.GetTableNames
        For i As Int32 = 0 To tableNames.Count - 1
            If outputtableName = tableNames(i) Then Throw New Exception("Output table name already exists in the SQLite database file.")
        Next
        '
        Dim tempDataTable As New DataTable
        Dim columnHeaders As New List(Of String)
        Dim columnTypes As New List(Of Type)
        Using csvParser As New FileIO.TextFieldParser(inputFile) With {.TextFieldType = FileIO.FieldType.Delimited}
            csvParser.HasFieldsEnclosedInQuotes = fieldsEnclosedInQuotes
            csvParser.SetDelimiters(New String() {","})
            '
            Dim lineArray() As String = csvParser.ReadFields() 'csvRead.ReadLine().Split(",")
            If hasHeaders = True Then
                Dim headerCounter As Int32
                For Each header As String In lineArray
                    headerCounter = 1
                    Do Until tempDataTable.Columns.Contains(header) = False
                        header = header & headerCounter
                        headerCounter += 1
                    Loop
                    tempDataTable.Columns.Add(header, GetType(String))
                    columnHeaders.Add(header)
                    columnTypes.Add(GetType(String))
                Next
            Else
                For i As Int32 = 0 To lineArray.Count - 1
                    tempDataTable.Columns.Add("Column_" & i + 1, GetType(String))
                    columnHeaders.Add("Column_" & i + 1)
                    columnTypes.Add(GetType(String))
                Next
            End If
            Try
                sqLiteEdit.CreateTable(outputtableName, columnHeaders.ToArray, columnTypes.ToArray)
                Dim sqLiteTable As DataTableView = sqLiteEdit.GetTableManager(outputtableName)
                '
                For i As Integer = 0 To dataLineStartIndex - 1
                    lineArray = csvParser.ReadFields() 'csvRead.ReadLine()
                Next
                Dim lineCounter As Int32 = 0
                While Not csvParser.EndOfData
                    If lineArray.Count <> columnHeaders.Count Then Continue While
                    tempDataTable.Rows.Add(lineArray)
                    '
                    lineCounter += 1
                    If lineCounter > 500000 Then
                        sqLiteTable.AddRows(tempDataTable)
                        tempDataTable.Rows.Clear()
                        lineCounter = 0
                    End If
                    '
                    Try
                        lineArray = csvParser.ReadFields()
                    Catch ex As FileIO.MalformedLineException
                        'If the fields are enclosed in quotes then likely failed due to quotes within the quoted text
                        If fieldsEnclosedInQuotes Then lineArray = csvParser.ErrorLine.Substring(1, csvParser.ErrorLine.Length - 2).Split({Chr(34) & "," & Chr(34)}, StringSplitOptions.None)
                    End Try
                End While
                sqLiteTable.AddRows(tempDataTable)
                sqLiteTable.ApplyEdits()
                sqLiteEdit.Close()
            Catch ex As Exception
                Throw New Exception("Error occurred while writing sqlite table: " & vbCrLf & ex.Message)
            End Try
        End Using
    End Sub
End Class
