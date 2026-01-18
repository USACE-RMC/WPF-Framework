Imports System.IO
Public Class DbfReader
    Inherits DatabaseManager
    Private _dbfStream As FileStream
    Private _dbfReader As BinaryReader
    Public ReadOnly Property DbReader As BinaryReader
        Get
            Return _dbfReader
        End Get
    End Property
    Private Structure DbfField
        'Public Name As String
        Public TypeId As String
        'Public Type As Type
        Public Length As Integer
        Public NumDecimal As Integer
        Public WriteField As Boolean
    End Structure

    Public Sub New(filePath As String)
        If Path.GetExtension(filePath).ToLower <> ".dbf" Then Throw New Exception("This is not a .dbf file")
        _dataBasePath = filePath
        _tableNames = GetTableNames()
    End Sub

    Public Overrides Sub Open()
        _dbfStream = New FileStream(_dataBasePath, FileMode.Open, FileAccess.Read, FileShare.Read)
        _dbfReader = New BinaryReader(_dbfStream)
        _dataBaseOpen = True
    End Sub
    Public Overrides Sub Close()
        _dataBaseOpen = False
        _dbfReader.Close() : _dbfReader.Dispose()
        _dbfStream.Close() : _dbfStream.Dispose()
    End Sub
    Public Overrides Function GetTableManager(tableName As String) As DataTableView
        Return New DbfTableReader(Me, tableName)
    End Function
    Public Overrides Function GetTableNames() As String()
        Return New String() {Path.GetFileNameWithoutExtension(_dataBasePath)}
    End Function
    Public Overrides Function GetStoredNumberOfRows(tableName As String) As Long
        Dim wasOpen As Boolean = DataBaseOpen
        If DataBaseOpen = False Then Open()
        _dbfReader.BaseStream.Position = 0
        '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
        Dim bytes() As Byte = _dbfReader.ReadBytes(32)
        Dim numberOfRows As Int32 = BitConverter.ToInt32(bytes, 4) 'number of rows
        '
        If wasOpen = False Then Close()
        Return numberOfRows
    End Function
    Public Overrides Function GetStoredNumberOfColumns(tableName As String) As Int32
        Dim wasOpen As Boolean = DataBaseOpen
        If DataBaseOpen = False Then Open()
        _dbfReader.BaseStream.Position = 0
        '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
        Dim bytes() As Byte = _dbfReader.ReadBytes(32)
        Dim firstDataRecordIndex = BitConverter.ToInt16(bytes, 8) 'always 32 + 32*number of fields + 1
        Dim totalColumns As Int32 = (firstDataRecordIndex - 32 - 1) \ 32
        '
        If wasOpen = False Then Close()
        Return totalColumns
    End Function
    Private Class DbfTableReader
        Inherits DataTableView
        Private _firstDataRecordIndex As Int16
        Private _recordStartPositions As New List(Of Int64)
        Private ReadOnly _parentDbfReader As DbfReader
        Private _recordLength As Int16
        Private _lengths() As Byte
        Private _positions() As Int16
        Public Sub New(ByVal parentManager As DbfReader, ByVal dataTableName As String)
            _parentDatabase = parentManager
            _parentDbfReader = parentManager
            _tableName = dataTableName
            LoadAttributeInfo()
            InitializeView()
            '
            'AddHandler _parentDatabase.RowAdded, Sub(changedTableName As String, rowData() As Object)
            '                                         'If ChangedTableName <> TableName Then Exit Sub
            '                                         _recordStartPositions.Add(_firstDataRecordIndex + 1 + ((_recordStartPositions.Count - 1) * _recordLength))
            '                                     End Sub

            'AddHandler _parentDatabase.RowsAdded, Sub(changedTableName As String, rowData As List(Of Object()))
            '                                          For i As Int32 = 0 To rowData.Count - 1
            '                                              _recordStartPositions.Add(_firstDataRecordIndex + 1 + ((_recordStartPositions.Count - 1) * _recordLength))
            '                                          Next
            '                                      End Sub
            'AddHandler _parentDatabase.EditsSaved, Sub(editedTableName as String)
            '                                           LoadAttributeInfo()
            '                                       End Sub
            'AddHandler _parentDatabase.RowDeleted, Sub(changedTableName As String, rowIndex As Int32) LoadAttributeInfo()
            'AddHandler _parentDatabase.RowsDeleted, Sub(changedTableName As String, rowIndices() As Int32) LoadAttributeInfo()
            'AddHandler _parentDatabase.ColumnAdded, Sub(changedTableName As String, columnName As String, columnType As Type) LoadAttributeInfo()
            'AddHandler _parentDatabase.ColumnDeleted, Sub(changedTableName As String, columnName As String) LoadAttributeInfo()
            'AddHandler _parentDatabase.ColumnsDeleted, Sub(changedTableName As String, columnsDeleted() As String) LoadAttributeInfo()
        End Sub
        Protected Overrides Function GetStoredColumnNames() As String()
            Return _storedColumnNames
        End Function
        Protected Overrides Function GetStoredColumnTypes() As Type()
            Return _storedColumnTypes
        End Function
        Protected Overrides Function GetStoredRowCount() As ULong
            Return _recordStartPositions.Count
        End Function

#Region "Database Row Stuff"
        Protected Overrides Sub AddRowToDatabase(ByVal row() As Object)
            'Error Checking
            If row.Count <> _storedColumnNames.Count Then Throw New Exception("Number of columns that you are trying to add do not match the number of columns in the database.")
            For i As Int32 = 0 To row.Count - 1
                If ConvertToColumnType(_storedColumnTypes(i), row(i)) = False Then Throw New Exception("Column data for " & _storedColumnNames(i) & " was of an incorrect data type.")
            Next
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Insert Row Data
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                'Update the number of rows
                dbfReaderFs.Position = 4
                dbfReaderFs.Write(BitConverter.GetBytes(CInt(_recordStartPositions.Count + 1)), 0, 4)
                dbfReaderFs.Position = dbfReaderFs.Length - 1
                'Write the Row
                Dim nDecimals As Byte
                Dim value As String, asciiBytes As Byte(), formatString As String
                dbfReaderFs.WriteByte(32)
                For i As Int32 = 0 To row.Count - 1
                    Select Case _storedColumnTypes(i)
                        Case GetType(Single), GetType(Double)
                            dbfReaderFs.Position = 32 + i * 32 + 17
                            nDecimals = dbfReaderFs.ReadByte
                            If _lengths(i) - nDecimals < 7 Then
                                formatString = "G"
                            Else
                                formatString = "0."
                                formatString = formatString.PadRight(formatString.Length + nDecimals, "0"c)
                                formatString = formatString & "e+000"
                            End If

                            'edit the appropriate part of the dbf file
                            dbfReaderFs.Position = dbfReaderFs.Length
                            If row(i).Equals(DBNull.Value) Then
                                value = ("").PadRight(_lengths(i), Chr(32))
                            Else
                                value = CType(row(i), Double).ToString(formatString).PadRight(_lengths(i), Chr(32))
                            End If
                            asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                            dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                        Case GetType(Integer), GetType(Short), GetType(Byte)
                            If row(i).Equals(DBNull.Value) Then
                                value = ("").PadLeft(_lengths(i), Chr(32))
                            Else
                                value = CType(row(i), Integer).ToString().PadLeft(_lengths(i), Chr(32))
                            End If
                            asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                            dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                        Case GetType(Boolean)
                            If row(i).Equals(DBNull.Value) Then
                                value = "0"
                            ElseIf CType(row(i), Boolean) = True Then
                                value = "1" 'can be 1,T,t,Y,and y
                            Else
                                value = "0" 'can be 0,F,f,N, and n
                            End If
                            asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                            dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                        Case GetType(String)
                            If row(i).Equals(DBNull.Value) Then
                                value = ("").PadRight(_lengths(i), Chr(32))
                            Else
                                If CStr(row(i)).Length > _lengths(i) Then
                                    value = CStr(row(i)).Substring(0, _lengths(i))
                                Else
                                    value = CStr(row(i)).PadRight(_lengths(i), Chr(32))
                                End If
                            End If
                            asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                            dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                    End Select
                Next

                ' eof dbf file marker
                dbfReaderFs.WriteByte(26)
                '
            End Using
            '
            LoadAttributeInfo()
            '_parentdatabase.OnRowsAdded(_tablename, new List(Of Object())({row}))
            '
            If wasOpen = True Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub AddRowToDatabase()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Insert Row Data
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Dim row(_storedColumnNames.Count - 1) As Object
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                'Update the number of rows
                dbfReaderFs.Position = 4
                dbfReaderFs.Write(BitConverter.GetBytes(CInt(_recordStartPositions.Count + 1)), 0, 4)
                dbfReaderFs.Position = dbfReaderFs.Length - 1
                'Write the Row
                'Dim nDecimals As Byte
                Dim value As String, asciiBytes As Byte() ', formatString As String
                dbfReaderFs.WriteByte(32)
                For i As Int32 = 0 To _storedColumnNames.Count - 1
                    Select Case _storedColumnTypes(i)
                        Case GetType(Single), GetType(Double)
                            'dbfReaderFs.Position = 32 + i * 32 + 17
                            'nDecimals = dbfReaderFs.ReadByte
                            'If _Lengths(i) - NDecimals < 7 Then
                            '    FormatString = "G"
                            'Else
                            '    FormatString = "0."
                            '    For j As Int32 = 1 To NDecimals
                            '        FormatString = FormatString & "0"
                            '    Next
                            '    FormatString = FormatString & "e+000"
                            'End If

                            'edit the appropriate part of the dbf file
                            'row(i) = DBNull.Value
                            dbfReaderFs.Position = dbfReaderFs.Length
                            value = ("").PadRight(_lengths(i), Chr(32))
                            asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                            dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                        Case GetType(Integer), GetType(Short), GetType(Byte)
                            'row(i) = DBNull.Value
                            value = ("").PadLeft(_lengths(i), Chr(32))
                            asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                            dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                        Case GetType(Boolean)
                            'If CType(Row(i), Boolean) = True Then
                            '    value = "1" 'can be 1,T,t,Y,and y
                            'Else
                            '    value = "0" 'can be 0,F,f,N, and n
                            'End If
                            'row(i) = False
                            asciiBytes = Text.Encoding.ASCII.GetBytes(("0"))
                            dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                        Case GetType(String)
                            'If CStr(Row(i)).Length > _Lengths(i) Then
                            '    value = CStr(Row(i)).Substring(0, _Lengths(i))
                            'Else
                            'row(i) = ""
                            value = ("").PadRight(_lengths(i), Chr(32))
                            'End If
                            asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                            dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                    End Select
                Next

                ' eof dbf file marker
                dbfReaderFs.WriteByte(26)
                '
            End Using
            '
            LoadAttributeInfo()
            '_parentDatabase.OnRowsAdded(_tablename, new List(Of Object())({row}))
            '
            If wasOpen = True Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub AddRowsToDatabase(newRowData As List(Of Object()))
            'Error Checking
            For Each row As Object() In newRowData
                If row.Count <> _storedColumnNames.Count Then Throw New Exception("Number of columns that you are trying to add do not match the number of columns in the database.")
            Next
            For Each row As Object() In newRowData
                For i As Int32 = 0 To row.Count - 1
                    If ConvertToColumnType(_storedColumnTypes(i), row(i)) = False Then Throw New Exception("Column data for " & _storedColumnNames(i) & " was of an incorrect data type.")
                Next
            Next
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Insert Row Data
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                'Update the number of rows
                dbfReaderFs.Position = 4
                dbfReaderFs.Write(BitConverter.GetBytes(CInt(_recordStartPositions.Count + newRowData.Count)), 0, 4)
                dbfReaderFs.Position = dbfReaderFs.Length - 1
                'Write the Row
                Dim nDecimals As Byte
                Dim value As String, asciiBytes As Byte(), formatString As String
                For Each row As Object() In newRowData
                    dbfReaderFs.WriteByte(32)
                    For i As Int32 = 0 To row.Count - 1
                        Select Case _storedColumnTypes(i)
                            Case GetType(Single), GetType(Double)
                                dbfReaderFs.Position = 32 + i * 32 + 17
                                nDecimals = dbfReaderFs.ReadByte
                                If _lengths(i) - nDecimals < 7 Then
                                    formatString = "G"
                                Else
                                    formatString = "0."
                                    formatString = formatString.PadRight(nDecimals + formatString.Length, "0"c)
                                    formatString = formatString & "e+000"
                                End If

                                'edit the appropriate part of the dbf file
                                dbfReaderFs.Position = dbfReaderFs.Length
                                value = CType(row(i), Double).ToString(formatString).PadRight(_lengths(i), Chr(32))
                                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                            Case GetType(Integer), GetType(Short), GetType(Byte)
                                value = CType(row(i), Integer).ToString().PadLeft(_lengths(i), Chr(32))
                                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                            Case GetType(Boolean)
                                If CType(row(i), Boolean) = True Then
                                    value = "1" 'can be 1,T,t,Y,and y
                                Else
                                    value = "0" 'can be 0,F,f,N, and n
                                End If
                                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                            Case GetType(String)
                                If CStr(row(i)).Length > _lengths(i) Then
                                    value = CStr(row(i)).Substring(0, _lengths(i))
                                Else
                                    value = CStr(row(i)).PadRight(_lengths(i), Chr(32))
                                End If
                                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                        End Select
                    Next
                Next
                ' eof dbf file marker
                dbfReaderFs.WriteByte(26)
                '
            End Using
            '
            LoadAttributeInfo()
            '_parentDatabase.OnRowsAdded(_tableName, newRowData)
            '
            If wasOpen = True Then _parentDatabase.Open()
        End Sub
        'Public Overrides Sub AddRowsToDatabase(TableName As String, NewRowData As DataTable)
        '    Dim RowsAdded As New List(Of Object())(NewRowData.Rows.Count)
        '    For i As Int32 = 0 To NewRowData.Rows.Count - 1
        '        RowsAdded.Add(NewRowData.Rows(i).ItemArray)
        '    Next
        '    AddRowsToDatabase(TableName, RowsAdded)
        'End Sub

        Protected Overrides Sub DeleteRowFromDatabase(rowIndex As Int32)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadByte)
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    writeBwDbf.Write(CInt(_recordStartPositions.Count - 1))
                    _parentDbfReader.DbReader.BaseStream.Position += 4
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(24))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    For i As Int32 = 0 To _storedColumnNames.Count - 1
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    For i As Int32 = 0 To _recordStartPositions.Count - 1
                        If i = rowIndex Then
                            _parentDbfReader.DbReader.BaseStream.Position += _recordLength
                        Else
                            writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(_recordLength))
                        End If
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                Threading.Thread.Sleep(1000)
                File.Delete(_parentDatabase.DataBasePath)
            End Try
            '
            Try
                File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            Catch ex As Exception
                Threading.Thread.Sleep(1000)
                File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            End Try
            '
            '_parentDatabase.OnRowsDeleted(_Tablename,{rowIndex})
            '   
            File.Delete(tmpdbf)
            LoadAttributeInfo()
            If wasOpen = True Then
                _parentDatabase.Open()
            Else
                _parentDatabase.Close()
            End If
        End Sub
        Protected Overrides Sub DeleteRowsFromDatabase(ByVal rowIndices() As Int32)
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            '
            Array.Sort(rowIndices)
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadByte)
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    writeBwDbf.Write(CInt(_recordStartPositions.Count - rowIndices.Count))
                    _parentDbfReader.DbReader.BaseStream.Position += 4
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(24))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    For i As Int32 = 0 To _storedColumnNames.Count - 1
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    Dim rowToDeleteIndex As Int32 = 0
                    For i As Int32 = 0 To _recordStartPositions.Count - 1
                        If i = rowIndices(rowToDeleteIndex) Then
                            _parentDbfReader.DbReader.BaseStream.Position += _recordLength
                            If rowToDeleteIndex < rowIndices.Count - 1 Then rowToDeleteIndex += 1
                        Else
                            writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(_recordLength))
                        End If
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                Threading.Thread.Sleep(1000)
                File.Delete(_parentDatabase.DataBasePath)
            End Try
            '
            Try
                File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            Catch ex As Exception
                Threading.Thread.Sleep(1000)
                File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            End Try
            '
            '_parentDatabase.OnRowsDeleted(_tableName, rowIndices)
            '   
            File.Delete(tmpdbf)
            LoadAttributeInfo()
            If wasOpen = True Then
                _parentDatabase.Open()
            Else
                _parentDatabase.Close()
            End If
        End Sub

        Private Function ParseRowBytes(ByVal rowBytes() As Byte) As Object()
            Dim row(_storedColumnNames.Count - 1) As Object
            Dim cellValue As String
            For i = 0 To _storedColumnNames.Count - 1
                cellValue = Text.Encoding.UTF8.GetString(rowBytes, _positions(i), _lengths(i)).Trim(Chr(0))
                row(i) = ConvertCellValueToProperType(cellValue, _storedColumnTypes(i))
            Next
            Return row
        End Function
        Protected Overrides Function GetStoredRow(storedRowIndex As Int32) As Object()
            '_dbfreader.BaseStream.Seek(_firstDataRecordIndex + 1 + ((RowIndex) * _recordLength), SeekOrigin.Begin)
            _parentDbfReader.DbReader.BaseStream.Position = _recordStartPositions(storedRowIndex)
            Return ParseRowBytes(_parentDbfReader.DbReader.ReadBytes(_recordLength))
        End Function
        Protected Overrides Function GetStoredRow(storedRowIndex As Int32, ByVal storedColumnNames() As String) As Object()
            Dim columnIndices(storedColumnNames.Count - 1) As Int32
            For i As Int32 = 0 To storedColumnNames.Count - 1
                columnIndices(i) = Array.IndexOf(_storedColumnNames, storedColumnNames(i))
            Next
            Return GetStoredRow(storedRowIndex, columnIndices)
        End Function
        Protected Overrides Function GetStoredRow(storedRowIndex As Int32, storedColumnIndices() As Integer) As Object()
            _parentDbfReader.DbReader.BaseStream.Position = _recordStartPositions(storedRowIndex)
            Dim rowBytes() As Byte = _parentDbfReader.DbReader.ReadBytes(_recordLength)
            Dim cellValue As String
            Dim returnValues(storedColumnIndices.Count - 1) As Object
            For i = 0 To storedColumnIndices.Count - 1
                cellValue = Text.Encoding.UTF8.GetString(rowBytes, _positions(storedColumnIndices(i)), _lengths(storedColumnIndices(i))).Trim(Chr(0))
                returnValues(i) = ConvertCellValueToProperType(cellValue, _storedColumnTypes(storedColumnIndices(i)))
            Next
            Return returnValues
        End Function
        Protected Overrides Function GetStoredRows(startStoredRowIndex As Int32, endStoredRowIndex As Int32) As List(Of Object())
            'Need to compare this function with a function that reads in a block of bytes.  In other words, is it faster to use the readers
            '.readbytes() function for each cell or is it faster to just read in all of the requested bytes up front and parse the cells from
            'the byte array?
            If endStoredRowIndex > _recordStartPositions.Count - 1 Then endStoredRowIndex = _recordStartPositions.Count - 1
            Dim row(_storedColumnNames.Count - 1) As Object
            Dim rows As New List(Of Object())(endStoredRowIndex - startStoredRowIndex + 1)
            Dim rowBytes() As Byte
            Dim cellValue As String
            '_dbfreader.BaseStream.Seek(_firstDataRecordIndex + 1 + ((StartRowIndex) * _recordLength), SeekOrigin.Begin)
            For i As Int32 = startStoredRowIndex To endStoredRowIndex
                _parentDbfReader.DbReader.BaseStream.Seek(_recordStartPositions(i), SeekOrigin.Begin)
                rowBytes = _parentDbfReader.DbReader.ReadBytes(_recordLength)
                For j = 0 To _storedColumnNames.Count - 1
                    cellValue = Text.Encoding.UTF8.GetString(rowBytes, _positions(j), _lengths(j)).Trim(Chr(0))
                    row(j) = ConvertCellValueToProperType(cellValue, _storedColumnTypes(j))
                Next
                'Rows.Add(ParseRowBytes(_dbfreader.ReadBytes(_recordLength)))
                rows.Add(row.Clone)
            Next
            Return rows
        End Function
#End Region

#Region "Database Column Stuff"
        'Public Function GetColumnRaw(columnName As String) As String()
        '    If _storedColumnNames.Contains(columnName) = False Then
        '        Throw New Exception("Field does not exist")
        '    End If
        '    Dim colindex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
        '    Return GetColumnRaw(colindex)
        'End Function
        'Public Function GetColumnRaw(columnIndex As Int32) As String()
        '    If _storedColumnNames.Count < columnIndex Then
        '        Throw New Exception("Column Index entered does not exist")
        '    End If
        '    Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
        '    If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
        '    Dim dbfbytes() As Byte
        '    Dim result As New List(Of String)(_recordStartPositions.Count) '(_NRows - 1) As String
        '    _parentDbfReader.DbReader.BaseStream.Position = _firstDataRecordIndex
        '    For i As Int32 = 0 To _recordStartPositions.Count - 1
        '        dbfbytes = _parentDbfReader.DbReader.ReadBytes(_recordLength)
        '        If dbfbytes(0) = 32 Then
        '            result.Add(Text.Encoding.UTF8.GetString(dbfbytes, _positions(columnIndex) + 1, _lengths(columnIndex)).Trim(Chr(0)))
        '        End If
        '    Next
        '    If wasOpen = False Then _parentDatabase.Close()
        '    Return result.ToArray
        'End Function
        Protected Overrides Function GetStoredColumn(storedColumnName As String) As Object()
            If _storedColumnNames.Contains(storedColumnName) = False Then Throw New Exception("Field '" & storedColumnName & "' does not exist")
            Dim colindex As Int32 = Array.IndexOf(_storedColumnNames, storedColumnName)
            Return GetStoredColumn(colindex)
        End Function
        Protected Overrides Function GetStoredColumn(storedColumnIndex As Integer) As Object()
            If _storedColumnNames.Count <= storedColumnIndex Then Throw New Exception("Requrested index does not exist")
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            Dim dbfbytes() As Byte
            Dim result As New List(Of Object)(_recordStartPositions.Count) '(_NRows - 1) As Object
            'Dim colindex As Int32 = Array.IndexOf(_ColumnNames, ColumnName)
            _parentDbfReader.DbReader.BaseStream.Position = _firstDataRecordIndex
            Dim cellValue As String
            For i As Int32 = 0 To _recordStartPositions.Count - 1
                dbfbytes = _parentDbfReader.DbReader.ReadBytes(_recordLength)
                'If dbfbytes(0) = 32 Then
                cellValue = Text.Encoding.UTF8.GetString(dbfbytes, _positions(storedColumnIndex) + 1, _lengths(storedColumnIndex)).Trim(Chr(0))
                result.Add(ConvertCellValueToProperType(cellValue, _storedColumnTypes(storedColumnIndex)))
                'End If
            Next
            If wasOpen = False Then _parentDatabase.Close()
            Return result.ToArray
        End Function
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
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Double)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired column named: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Column: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & ", not of type Double.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("The dbf file does not have: " & columnData.Count & " records")
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the number of decimals from the Main File Header
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            _parentDbfReader.DbReader.BaseStream.Position = 32 + (columnIndex - 1) * 32 + 17
            Dim nDecimals As Byte
            nDecimals = _parentDbfReader.DbReader.ReadByte
            _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the position of the value in the row
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim lengthBegin As Int32 = 0
            For i As Int32 = 0 To columnIndex - 1
                lengthBegin += _lengths(i)
            Next
            lengthBegin += 1
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Data as rows
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)
                '
                Dim value As String, asciiBytes As Byte(), startPosition As Int32, formatString As String
                If _lengths(columnIndex) - nDecimals < 7 Then
                    formatString = "G"
                Else
                    formatString = "0."
                    formatString = formatString.PadRight(formatString.Length + nDecimals, "0"c)
                    formatString = formatString & "e+000"
                End If

                For i As Int32 = 0 To _recordStartPositions.Count - 1
                    startPosition = _recordStartPositions(i) - 1 '_firstDataRecordIndex + i * _recordLength
                    dbfReaderFs.Position = startPosition + lengthBegin
                    value = columnData(i).ToString(formatString).PadRight(_lengths(columnIndex), Chr(32))
                    asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                    dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                Next
            End Using
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Single)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired column named: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Column: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & ", not of type Double.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("The dbf file does not have: " & columnData.Count & " records")
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the number of decimals from the Main File Header
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            _parentDbfReader.DbReader.BaseStream.Position = 32 + (columnIndex - 1) * 32 + 17
            Dim nDecimals As Byte
            nDecimals = _parentDbfReader.DbReader.ReadByte
            _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the position of the value in the row
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim lengthBegin As Int32 = 0
            For i As Int32 = 0 To columnIndex - 1
                lengthBegin += _lengths(i)
            Next
            lengthBegin += 1
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Data as rows
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)
                '
                Dim value As String, asciiBytes As Byte(), startPosition As Int32, formatString As String
                If _lengths(columnIndex) - nDecimals < 7 Then
                    formatString = "G"
                Else
                    formatString = "0."
                    formatString = formatString.PadRight(formatString.Length + nDecimals, "0"c)
                    formatString = formatString & "e+000"
                End If

                For i As Int32 = 0 To _recordStartPositions.Count - 1
                    startPosition = _recordStartPositions(i) - 1 '_firstDataRecordIndex + i * _recordLength
                    dbfReaderFs.Position = startPosition + lengthBegin
                    value = columnData(i).ToString(formatString).PadRight(_lengths(columnIndex), Chr(32))
                    asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                    dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                Next
            End Using
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData()() As Byte)
            Throw New NotImplementedException("dbf file format does not support storing binary array data.")
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As String)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired column named: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Column: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & ", not of type Double.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("The dbf file does not have: " & columnData.Count & " records")
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Verify the DBF is closed
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the position of the value in the row
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim lengthBegin As Int32 = 0
            For i As Int32 = 0 To columnIndex - 1
                lengthBegin += _lengths(i)
            Next
            lengthBegin += 1
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Data as rows
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                '
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)
                '
                Dim value As String, asciiBytes As Byte(), startPosition As Int32
                For i As Int32 = 0 To _recordStartPositions.Count - 1
                    startPosition = _recordStartPositions(i) - 1 '_firstDataRecordIndex + i * _recordLength
                    dbfReaderFs.Position = startPosition + lengthBegin
                    If columnData(i).Length > _lengths(columnIndex) Then
                        value = columnData(i).Substring(0, _lengths(columnIndex))
                    Else
                        value = columnData(i).PadRight(_lengths(columnIndex), Chr(32))
                    End If
                    asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                    dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                Next
            End Using
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Boolean)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired column named: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Column: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & ", not of type Double.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("The dbf file does not have: " & columnData.Count & " records")
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Verify the DBF is closed
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the position of the value in the row
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim lengthBegin As Int32 = 0
            For i As Int32 = 0 To columnIndex - 1
                lengthBegin += _lengths(i)
            Next
            lengthBegin += 1
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Data as rows
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                '
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)
                '
                Dim value As String, asciiBytes As Byte(), startPosition As Int32
                For i As Int32 = 0 To _recordStartPositions.Count - 1
                    startPosition = _recordStartPositions(i) - 1 '_firstDataRecordIndex + i * _recordLength
                    dbfReaderFs.Position = startPosition + lengthBegin
                    If columnData(i) Then
                        value = "1" 'can be 1,T,t,Y,and y
                    Else
                        value = "0" 'can be 0,F,f,N, and n
                    End If
                    asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                    dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                Next
            End Using
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Int64)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired column named: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Column: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & ", not of type Double.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("The dbf file does not have: " & columnData.Count & " records")
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Verify the DBF is closed
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the position of the value in the row
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim lengthBegin As Int32 = 0
            For i As Int32 = 0 To columnIndex - 1
                lengthBegin += _lengths(i)
            Next
            lengthBegin += 1
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Data as rows
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                '
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)
                '
                Dim value As String, asciiBytes As Byte(), startPosition As Int32
                For i As Int32 = 0 To _recordStartPositions.Count - 1
                    startPosition = _recordStartPositions(i) - 1 '_firstDataRecordIndex + i * _recordLength
                    dbfReaderFs.Position = startPosition + lengthBegin
                    value = columnData(i).ToString().PadLeft(_lengths(columnIndex), Chr(32)) 'probably needs space padding.value = DoubleData(i).ToString(FormatString).PadLeft(_Lengths(fieldindex), Chr(32))
                    asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                    dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                Next
            End Using
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Int32)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired column named: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Column: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & ", not of type Double.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("The dbf file does not have: " & columnData.Count & " records")
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Verify the DBF is closed
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the position of the value in the row
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim lengthBegin As Int32 = 0
            For i As Int32 = 0 To columnIndex - 1
                lengthBegin += _lengths(i)
            Next
            lengthBegin += 1
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Data as rows
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                '
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)
                '
                Dim value As String, asciiBytes As Byte(), startPosition As Int32
                For i As Int32 = 0 To _recordStartPositions.Count - 1
                    startPosition = _recordStartPositions(i) - 1 '_firstDataRecordIndex + i * _recordLength
                    dbfReaderFs.Position = startPosition + lengthBegin
                    value = columnData(i).ToString().PadLeft(_lengths(columnIndex), Chr(32)) 'probably needs space padding.value = DoubleData(i).ToString(FormatString).PadLeft(_Lengths(fieldindex), Chr(32))
                    asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                    dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                Next
            End Using
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Int16)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired column named: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Column: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & ", not of type Double.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("The dbf file does not have: " & columnData.Count & " records")
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Verify the DBF is closed
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the position of the value in the row
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim lengthBegin As Int32 = 0
            For i As Int32 = 0 To columnIndex - 1
                lengthBegin += _lengths(i)
            Next
            lengthBegin += 1
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Data as rows
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                '
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)
                '
                Dim value As String, asciiBytes As Byte(), startPosition As Int32
                For i As Int32 = 0 To _recordStartPositions.Count - 1
                    startPosition = _recordStartPositions(i) - 1 '_firstDataRecordIndex + i * _recordLength
                    dbfReaderFs.Position = startPosition + lengthBegin
                    value = columnData(i).ToString().PadLeft(_lengths(columnIndex), Chr(32)) 'probably needs space padding.value = DoubleData(i).ToString(FormatString).PadLeft(_Lengths(fieldindex), Chr(32))
                    asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                    dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                Next
            End Using
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Byte)
            If columnData.Count = 0 Then Exit Sub
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex = -1 Then Throw New Exception("The desired column named: " & columnName & " does not exist.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), columnData(0)) = False Then Throw New Exception("The desired Column: " & columnName & " is of type " & _storedColumnTypes(columnIndex).ToString & ", not of type Double.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("The dbf file does not have: " & columnData.Count & " records")
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Verify the DBF is closed
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Get the position of the value in the row
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim lengthBegin As Int32 = 0
            For i As Int32 = 0 To columnIndex - 1
                lengthBegin += _lengths(i)
            Next
            lengthBegin += 1
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Data as rows
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                '
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)
                '
                Dim value As String, asciiBytes As Byte(), startPosition As Int32
                For i As Int32 = 0 To _recordStartPositions.Count - 1
                    startPosition = _recordStartPositions(i) - 1 '_firstDataRecordIndex + i * _recordLength
                    dbfReaderFs.Position = startPosition + lengthBegin
                    value = columnData(i).ToString().PadLeft(_lengths(columnIndex), Chr(32)) 'probably needs space padding.value = DoubleData(i).ToString(FormatString).PadLeft(_Lengths(fieldindex), Chr(32))
                    asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                    dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                Next
            End Using
            If wasOpen Then _parentDatabase.Open()
        End Sub

        Protected Overrides Sub DeleteColumnFromDatabase(columnName As String)
            'If _ColumnNames.Contains(ColumnName) = False Then Throw New Exception("Field name " & ColumnName & " does not exist in the dbf file.")
            '
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, columnName)
            If columnIndex < 0 Then Exit Sub
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    Dim nRecords As Int32 = _parentDbfReader.DbReader.ReadInt32 'number of rows
                    writeBwDbf.Write(nRecords)
                    Dim firstDataRecordIndex As Int16 = _parentDbfReader.DbReader.ReadInt16 'always 32 + 32*number of fields + 1
                    writeBwDbf.Write(CShort(firstDataRecordIndex - 32))
                    Dim recordLen As Int16 = _parentDbfReader.DbReader.ReadInt16
                    writeBwDbf.Write(CShort(recordLen - _lengths(columnIndex)))
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(20))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    For i As Int32 = 0 To _storedColumnNames.Count - 1
                        If i = columnIndex Then
                            _parentDbfReader.DbReader.BaseStream.Position += 32
                            Continue For
                        End If
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Seek(firstDataRecordIndex, SeekOrigin.Begin)
                    Dim preFieldrecordLength As Int32 = 0
                    For j As Int32 = 0 To columnIndex - 1
                        preFieldrecordLength += _lengths(j)
                    Next
                    Dim postFieldrecordLength As Int32 = 0
                    For j As Int32 = columnIndex + 1 To _lengths.Count - 1
                        postFieldrecordLength += _lengths(j)
                    Next
                    For i As Int32 = 0 To nRecords - 1
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(preFieldrecordLength + 1)) 'The plus 1 is for the preceding byte that indicates deletion or not deleted.
                        _parentDbfReader.DbReader.BaseStream.Position += _lengths(columnIndex)
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(postFieldrecordLength))
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                Threading.Thread.Sleep(1000)
                File.Delete(_parentDatabase.DataBasePath)
            End Try
            '
            Try
                File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            Catch ex As Exception
                Threading.Thread.Sleep(1000)
                File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            End Try
            '
            LoadAttributeInfo()
            File.Delete(tmpdbf)
            '
            '_parentDatabase.OnColumnsDeleted(_tablename, {columnName})
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub DeleteColumnsFromDatabase(columnsToDelete() As String)
            'If _ColumnNames.Contains(ColumnName) = False Then Throw New Exception("Field name " & ColumnName & " does not exist in the dbf file.")
            '
            Dim saveColumn(_storedColumnNames.Count - 1) As Boolean
            Dim numberToDelete As Int32 = 0
            For i As Int32 = 0 To _storedColumnNames.Count - 1
                If columnsToDelete.Contains(_storedColumnNames(i)) Then
                    saveColumn(i) = False
                    numberToDelete += 1
                Else
                    saveColumn(i) = True
                End If
            Next
            '
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    Dim nRecords As Int32 = _parentDbfReader.DbReader.ReadInt32 'number of rows
                    writeBwDbf.Write(nRecords)
                    Dim firstDataRecordIndex As Int16 = _parentDbfReader.DbReader.ReadInt16 'always 32 + 32*number of fields + 1
                    writeBwDbf.Write(CShort(firstDataRecordIndex - 32 * numberToDelete))
                    Dim recordLen As Int16 = _parentDbfReader.DbReader.ReadInt16
                    Dim lengthToRemove As Short = 0
                    For i As Int32 = 0 To saveColumn.Count - 1
                        If saveColumn(i) = False Then lengthToRemove += _lengths(i)
                    Next
                    writeBwDbf.Write(CShort(recordLen - lengthToRemove))
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(20))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    For i As Int32 = 0 To _storedColumnNames.Count - 1
                        If saveColumn(i) = True Then
                            writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                        Else
                            _parentDbfReader.DbReader.BaseStream.Position += 32
                        End If
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Seek(firstDataRecordIndex, SeekOrigin.Begin)
                    For i As Int32 = 0 To nRecords - 1
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadByte) 'preceding byte that indicates deletion or not deleted.
                        For j As Int32 = 0 To saveColumn.Count - 1
                            If saveColumn(j) = True Then
                                writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(_lengths(j)))
                            Else
                                _parentDbfReader.DbReader.BaseStream.Position += _lengths(j)
                            End If
                        Next
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                Threading.Thread.Sleep(1000)
                File.Delete(_parentDatabase.DataBasePath)
            End Try
            '
            Try
                File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            Catch ex As Exception
                Threading.Thread.Sleep(1000)
                File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            End Try
            '
            LoadAttributeInfo()
            File.Delete(tmpdbf)
            '
            '_parentDatabase.OnColumnsDeleted(_tableName, columnsToDelete)
            If wasOpen Then _parentDatabase.Open()
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
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData As Byte()())
            Throw New NotImplementedException("DBF does not support storage of byte arrays.")
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Double)
            Dim fieldLength As Byte = 19
            If columnName.Length > 10 Then Throw New Exception("DBF name is too long, column name cannot be longer than 10 characters.")
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Field name " & columnName & " already exists in the dbf file, choose a different name.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("Number of records in dbf file do not match the number of records in Values() array.")
            'open stream to read the dbf
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    Dim nRecords As Int32 = _parentDbfReader.DbReader.ReadInt32 'number of rows
                    writeBwDbf.Write(nRecords)
                    Dim firstDataRecordIndex As Int16 = _parentDbfReader.DbReader.ReadInt16 'always 32 + 32*number of fields + 1
                    writeBwDbf.Write(CShort(firstDataRecordIndex + 32))
                    Dim recordLen As Int16 = _parentDbfReader.DbReader.ReadInt16
                    writeBwDbf.Write(CShort(recordLen + fieldLength))
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(20))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    Do Until _parentDbfReader.DbReader.PeekChar() = 13
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                    Loop
                    columnName = columnName.PadRight(10, Chr(0))
                    Dim asciiBytes As Byte() = Text.Encoding.ASCII.GetBytes(columnName)
                    writeBwDbf.Write(asciiBytes) 'FieldName
                    writeBwDbf.Write("F") 'FieldType
                    writeBwDbf.Write(CInt(0)) 'FieldDataAddress
                    writeBwDbf.Write(fieldLength) 'FieldLength
                    writeBwDbf.Write(CByte(11)) 'NDecimalPlaces
                    writeBwDbf.Write(CByte(0)) 'FieldFlag
                    writeBwDbf.Write(CInt(0)) 'AutoIncrementValue
                    writeBwDbf.Write(CByte(0)) 'AutoIncrementStep
                    For i As Int32 = 1 To 8
                        writeBwDbf.Write(CByte(0)) 'reserved
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Seek(firstDataRecordIndex, SeekOrigin.Begin)
                    Dim recordBytes() As Byte
                    For i As Int32 = 0 To nRecords - 1
                        recordBytes = _parentDbfReader.DbReader.ReadBytes(recordLen)
                        'If RecordBytes(0) = 32 Then
                        asciiBytes = Text.Encoding.ASCII.GetBytes(columnData(i).ToString("0.00000000000e+000").PadRight(fieldLength, Chr(32)))
                        'Else
                        'asciiBytes = Text.Encoding.ASCII.GetBytes(("").PadRight(FieldLength, Chr(32)))
                        'End If
                        writeBwDbf.Write(recordBytes)
                        writeBwDbf.Write(asciiBytes)
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                MsgBox("Unable to save changes: " & ex.Message) : File.Delete(tmpdbf) : Exit Sub
            End Try
            '
            File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Double))
            LoadAttributeInfo()
            File.Delete(tmpdbf)
            '
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Single)
            Dim fieldLength As Byte = 19
            If columnName.Length > 10 Then Throw New Exception("DBF name is too long, column name cannot be longer than 10 characters.")
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Field name " & columnName & " already exists in the dbf file, choose a different name.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("Number of records in dbf file do not match the number of records in Values() array.")
            'open stream to read the dbf
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    Dim nRecords As Int32 = _parentDbfReader.DbReader.ReadInt32 'number of rows
                    writeBwDbf.Write(nRecords)
                    Dim firstDataRecordIndex As Int16 = _parentDbfReader.DbReader.ReadInt16 'always 32 + 32*number of fields + 1
                    writeBwDbf.Write(CShort(firstDataRecordIndex + 32))
                    Dim recordLen As Int16 = _parentDbfReader.DbReader.ReadInt16
                    writeBwDbf.Write(CShort(recordLen + fieldLength))
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(20))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    Do Until _parentDbfReader.DbReader.PeekChar() = 13
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                    Loop
                    columnName = columnName.PadRight(10, Chr(0))
                    Dim asciiBytes As Byte() = Text.Encoding.ASCII.GetBytes(columnName)
                    writeBwDbf.Write(asciiBytes) 'FieldName
                    writeBwDbf.Write("F") 'FieldType
                    writeBwDbf.Write(CInt(0)) 'FieldDataAddress
                    writeBwDbf.Write(fieldLength) 'FieldLength
                    writeBwDbf.Write(CByte(11)) 'NDecimalPlaces
                    writeBwDbf.Write(CByte(0)) 'FieldFlag
                    writeBwDbf.Write(CInt(0)) 'AutoIncrementValue
                    writeBwDbf.Write(CByte(0)) 'AutoIncrementStep
                    For i As Int32 = 1 To 8
                        writeBwDbf.Write(CByte(0)) 'reserved
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Seek(firstDataRecordIndex, SeekOrigin.Begin)
                    Dim recordBytes() As Byte
                    For i As Int32 = 0 To nRecords - 1
                        recordBytes = _parentDbfReader.DbReader.ReadBytes(recordLen)
                        'If RecordBytes(0) = 32 Then
                        asciiBytes = Text.Encoding.ASCII.GetBytes(columnData(i).ToString("0.00000000000e+000").PadRight(fieldLength, Chr(32)))
                        'Else
                        'asciiBytes = Text.Encoding.ASCII.GetBytes(("").PadRight(FieldLength, Chr(32)))
                        'End If
                        writeBwDbf.Write(recordBytes)
                        writeBwDbf.Write(asciiBytes)
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                MsgBox("Unable to save changes: " & ex.Message) : File.Delete(tmpdbf) : Exit Sub
            End Try
            '
            File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Single))
            LoadAttributeInfo()
            File.Delete(tmpdbf)
            '
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Int64)
            Dim fieldLength As Byte = Long.MinValue.ToString.Length
            If columnName.Length > 10 Then Throw New Exception("DBF name is too long, column name cannot be longer than 10 characters.")
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Field name " & columnName & " already exists in the dbf file, choose a different name.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("Number of records in dbf file do not match the number of records in Values() array.")
            'open stream to read the dbf
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    Dim nRecords As Int32 = _parentDbfReader.DbReader.ReadInt32 'number of rows
                    writeBwDbf.Write(nRecords)
                    Dim firstDataRecordIndex As Int16 = _parentDbfReader.DbReader.ReadInt16 'always 32 + 32*number of fields + 1
                    writeBwDbf.Write(CShort(firstDataRecordIndex + 32))
                    Dim recordLen As Int16 = _parentDbfReader.DbReader.ReadInt16
                    writeBwDbf.Write(CShort(recordLen + fieldLength))
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(20))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    Do Until _parentDbfReader.DbReader.PeekChar() = 13
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                    Loop
                    columnName = columnName.PadRight(10, Chr(0))
                    Dim asciiBytes As Byte() = Text.Encoding.ASCII.GetBytes(columnName)
                    writeBwDbf.Write(asciiBytes) 'FieldName
                    writeBwDbf.Write("N") 'FieldType
                    writeBwDbf.Write(CInt(0)) 'FieldDataAddress
                    writeBwDbf.Write(fieldLength) 'FieldLength
                    writeBwDbf.Write(CByte(0)) 'NDecimalPlaces
                    writeBwDbf.Write(CByte(0)) 'FieldFlag
                    writeBwDbf.Write(CInt(0)) 'AutoIncrementValue
                    writeBwDbf.Write(CByte(0)) 'AutoIncrementStep
                    For i As Int32 = 1 To 8
                        writeBwDbf.Write(CByte(0)) 'reserved
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Seek(firstDataRecordIndex, SeekOrigin.Begin)
                    Dim recordBytes() As Byte
                    For i As Int32 = 0 To nRecords - 1
                        recordBytes = _parentDbfReader.DbReader.ReadBytes(recordLen)
                        'If RecordBytes(0) = 32 Then
                        asciiBytes = Text.Encoding.ASCII.GetBytes(columnData(i).ToString.PadRight(fieldLength, Chr(32)))
                        'Else
                        'asciiBytes = Text.Encoding.ASCII.GetBytes(("").PadRight(FieldLength, Chr(32)))
                        'End If
                        writeBwDbf.Write(recordBytes)
                        writeBwDbf.Write(asciiBytes)
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                MsgBox("Unable to save changes: " & ex.Message) : File.Delete(tmpdbf) : Exit Sub
            End Try
            '
            File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Int64))
            LoadAttributeInfo()
            File.Delete(tmpdbf)
            '
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Int32)
            Dim fieldLength As Byte = Integer.MinValue.ToString.Length
            If columnName.Length > 10 Then Throw New Exception("DBF name is too long, column name cannot be longer than 10 characters.")
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Field name " & columnName & " already exists in the dbf file, choose a different name.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("Number of records in dbf file do not match the number of records in Values() array.")
            'open stream to read the dbf
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    Dim nRecords As Int32 = _parentDbfReader.DbReader.ReadInt32 'number of rows
                    writeBwDbf.Write(nRecords)
                    Dim firstDataRecordIndex As Int16 = _parentDbfReader.DbReader.ReadInt16 'always 32 + 32*number of fields + 1
                    writeBwDbf.Write(CShort(firstDataRecordIndex + 32))
                    Dim recordLen As Int16 = _parentDbfReader.DbReader.ReadInt16
                    writeBwDbf.Write(CShort(recordLen + fieldLength))
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(20))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    Do Until _parentDbfReader.DbReader.PeekChar() = 13
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                    Loop
                    columnName = columnName.PadRight(10, Chr(0))
                    Dim asciiBytes As Byte() = Text.Encoding.ASCII.GetBytes(columnName)
                    writeBwDbf.Write(asciiBytes) 'FieldName
                    writeBwDbf.Write("N") 'FieldType
                    writeBwDbf.Write(CInt(0)) 'FieldDataAddress
                    writeBwDbf.Write(fieldLength) 'FieldLength
                    writeBwDbf.Write(CByte(0)) 'NDecimalPlaces
                    writeBwDbf.Write(CByte(0)) 'FieldFlag
                    writeBwDbf.Write(CInt(0)) 'AutoIncrementValue
                    writeBwDbf.Write(CByte(0)) 'AutoIncrementStep
                    For i As Int32 = 1 To 8
                        writeBwDbf.Write(CByte(0)) 'reserved
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Seek(firstDataRecordIndex, SeekOrigin.Begin)
                    Dim recordBytes() As Byte
                    For i As Int32 = 0 To nRecords - 1
                        recordBytes = _parentDbfReader.DbReader.ReadBytes(recordLen)
                        'If RecordBytes(0) = 32 Then
                        asciiBytes = Text.Encoding.ASCII.GetBytes(columnData(i).ToString.PadRight(fieldLength, Chr(32)))
                        'Else
                        'asciiBytes = Text.Encoding.ASCII.GetBytes(("").PadRight(FieldLength, Chr(32)))
                        'End If
                        writeBwDbf.Write(recordBytes)
                        writeBwDbf.Write(asciiBytes)
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                MsgBox("Unable to save changes: " & ex.Message) : File.Delete(tmpdbf) : Exit Sub
            End Try
            '
            File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Int32))
            LoadAttributeInfo()
            File.Delete(tmpdbf)
            '
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Int16)
            Dim fieldLength As Byte = Short.MinValue.ToString.Length
            If columnName.Length > 10 Then Throw New Exception("DBF name is too long, column name cannot be longer than 10 characters.")
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Field name " & columnName & " already exists in the dbf file, choose a different name.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("Number of records in dbf file do not match the number of records in Values() array.")
            'open stream to read the dbf
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    Dim nRecords As Int32 = _parentDbfReader.DbReader.ReadInt32 'number of rows
                    writeBwDbf.Write(nRecords)
                    Dim firstDataRecordIndex As Int16 = _parentDbfReader.DbReader.ReadInt16 'always 32 + 32*number of fields + 1
                    writeBwDbf.Write(CShort(firstDataRecordIndex + 32))
                    Dim recordLen As Int16 = _parentDbfReader.DbReader.ReadInt16
                    writeBwDbf.Write(CShort(recordLen + fieldLength))
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(20))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    Do Until _parentDbfReader.DbReader.PeekChar() = 13
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                    Loop
                    columnName = columnName.PadRight(10, Chr(0))
                    Dim asciiBytes As Byte() = Text.Encoding.ASCII.GetBytes(columnName)
                    writeBwDbf.Write(asciiBytes) 'FieldName
                    writeBwDbf.Write("N") 'FieldType
                    writeBwDbf.Write(CInt(0)) 'FieldDataAddress
                    writeBwDbf.Write(fieldLength) 'FieldLength
                    writeBwDbf.Write(CByte(0)) 'NDecimalPlaces
                    writeBwDbf.Write(CByte(0)) 'FieldFlag
                    writeBwDbf.Write(CInt(0)) 'AutoIncrementValue
                    writeBwDbf.Write(CByte(0)) 'AutoIncrementStep
                    For i As Int32 = 1 To 8
                        writeBwDbf.Write(CByte(0)) 'reserved
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Seek(firstDataRecordIndex, SeekOrigin.Begin)
                    Dim recordBytes() As Byte
                    For i As Int32 = 0 To nRecords - 1
                        recordBytes = _parentDbfReader.DbReader.ReadBytes(recordLen)
                        'If RecordBytes(0) = 32 Then
                        asciiBytes = Text.Encoding.ASCII.GetBytes(columnData(i).ToString.PadRight(fieldLength, Chr(32)))
                        'Else
                        'asciiBytes = Text.Encoding.ASCII.GetBytes(("").PadRight(FieldLength, Chr(32)))
                        'End If
                        writeBwDbf.Write(recordBytes)
                        writeBwDbf.Write(asciiBytes)
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                MsgBox("Unable to save changes: " & ex.Message) : File.Delete(tmpdbf) : Exit Sub
            End Try
            '
            File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Short))
            LoadAttributeInfo()
            File.Delete(tmpdbf)
            '
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Byte)
            Dim fieldLength As Byte = 3
            If columnName.Length > 10 Then Throw New Exception("DBF name is too long, column name cannot be longer than 10 characters.")
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Field name " & columnName & " already exists in the dbf file, choose a different name.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("Number of records in dbf file do not match the number of records in Values() array.")
            'open stream to read the dbf
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    Dim nRecords As Int32 = _parentDbfReader.DbReader.ReadInt32 'number of rows
                    writeBwDbf.Write(nRecords)
                    Dim firstDataRecordIndex As Int16 = _parentDbfReader.DbReader.ReadInt16 'always 32 + 32*number of fields + 1
                    writeBwDbf.Write(CShort(firstDataRecordIndex + 32))
                    Dim recordLen As Int16 = _parentDbfReader.DbReader.ReadInt16
                    writeBwDbf.Write(CShort(recordLen + fieldLength))
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(20))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    Do Until _parentDbfReader.DbReader.PeekChar() = 13
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                    Loop
                    columnName = columnName.PadRight(10, Chr(0))
                    Dim asciiBytes As Byte() = Text.Encoding.ASCII.GetBytes(columnName)
                    writeBwDbf.Write(asciiBytes) 'FieldName
                    writeBwDbf.Write("N") 'FieldType
                    writeBwDbf.Write(CInt(0)) 'FieldDataAddress
                    writeBwDbf.Write(fieldLength) 'FieldLength
                    writeBwDbf.Write(CByte(0)) 'NDecimalPlaces
                    writeBwDbf.Write(CByte(0)) 'FieldFlag
                    writeBwDbf.Write(CInt(0)) 'AutoIncrementValue
                    writeBwDbf.Write(CByte(0)) 'AutoIncrementStep
                    For i As Int32 = 1 To 8
                        writeBwDbf.Write(CByte(0)) 'reserved
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Seek(firstDataRecordIndex, SeekOrigin.Begin)
                    Dim recordBytes() As Byte
                    For i As Int32 = 0 To nRecords - 1
                        recordBytes = _parentDbfReader.DbReader.ReadBytes(recordLen)
                        'If RecordBytes(0) = 32 Then
                        asciiBytes = Text.Encoding.ASCII.GetBytes(columnData(i).ToString.PadRight(fieldLength, Chr(32)))
                        'Else
                        'asciiBytes = Text.Encoding.ASCII.GetBytes(("").PadRight(FieldLength, Chr(32)))
                        'End If
                        writeBwDbf.Write(recordBytes)
                        writeBwDbf.Write(asciiBytes)
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                MsgBox("Unable to save changes: " & ex.Message) : File.Delete(tmpdbf) : Exit Sub
            End Try
            '
            File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Byte))
            LoadAttributeInfo()
            File.Delete(tmpdbf)
            '
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Boolean)
            Dim fieldLength As Byte = 1
            If columnName.Length > 10 Then Throw New Exception("DBF name is too long, column name cannot be longer than 10 characters.")
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Field name " & columnName & " already exists in the dbf file, choose a different name.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("Number of records in dbf file do not match the number of records in Values() array.")
            'open stream to read the dbf
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    Dim nRecords As Int32 = _parentDbfReader.DbReader.ReadInt32 'number of rows
                    writeBwDbf.Write(nRecords)
                    Dim firstDataRecordIndex As Int16 = _parentDbfReader.DbReader.ReadInt16 'always 32 + 32*number of fields + 1
                    writeBwDbf.Write(CShort(firstDataRecordIndex + 32))
                    Dim recordLen As Int16 = _parentDbfReader.DbReader.ReadInt16
                    writeBwDbf.Write(CShort(recordLen + fieldLength))
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(20))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    Do Until _parentDbfReader.DbReader.PeekChar() = 13
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                    Loop
                    columnName = columnName.PadRight(10, Chr(0))
                    Dim asciiBytes As Byte() = Text.Encoding.ASCII.GetBytes(columnName)
                    writeBwDbf.Write(asciiBytes) 'FieldName
                    writeBwDbf.Write("L") 'FieldType
                    writeBwDbf.Write(CInt(0)) 'FieldDataAddress
                    writeBwDbf.Write(fieldLength) 'FieldLength
                    writeBwDbf.Write(CByte(0)) 'NDecimalPlaces
                    writeBwDbf.Write(CByte(0)) 'FieldFlag
                    writeBwDbf.Write(CInt(0)) 'AutoIncrementValue
                    writeBwDbf.Write(CByte(0)) 'AutoIncrementStep
                    For i As Int32 = 1 To 8
                        writeBwDbf.Write(CByte(0)) 'reserved
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Seek(firstDataRecordIndex, SeekOrigin.Begin)
                    Dim recordBytes() As Byte
                    For i As Int32 = 0 To nRecords - 1
                        recordBytes = _parentDbfReader.DbReader.ReadBytes(recordLen)
                        'If RecordBytes(0) = 32 Then
                        If columnData(i) = True Then
                            asciiBytes = Text.Encoding.ASCII.GetBytes("1".PadRight(fieldLength, Chr(32)))
                        Else
                            asciiBytes = Text.Encoding.ASCII.GetBytes("0".PadRight(fieldLength, Chr(32)))
                        End If
                        'Else
                        'asciiBytes = Text.Encoding.ASCII.GetBytes(("0").PadRight(FieldLength, Chr(32)))
                        'End If
                        writeBwDbf.Write(recordBytes)
                        writeBwDbf.Write(asciiBytes)
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                MsgBox("Unable to save changes: " & ex.Message) : File.Delete(tmpdbf) : Exit Sub
            End Try
            '
            File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Boolean))
            LoadAttributeInfo()
            File.Delete(tmpdbf)
            '
            If wasOpen Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As String)
            If columnName.Length > 10 Then Throw New Exception("DBF name is too long, column name cannot be longer than 10 characters.")
            If _storedColumnNames.Contains(columnName) = True Then Throw New Exception("Field name " & columnName & " already exists in the dbf file, choose a different name.")
            If _recordStartPositions.Count <> columnData.Count Then Throw New Exception("Number of records in dbf file do not match the number of records in Values() array.")
            '
            Dim fieldLength As Byte = 0
            Dim valueBytesLength As Int32
            For i As Int32 = 0 To columnData.Count - 1
                valueBytesLength = Text.Encoding.UTF8.GetByteCount(columnData(i))
                If valueBytesLength > 256 Then Throw New Exception("String value at index: " & i.ToString & " contains too many characters for writing to the .dbf file format.")
                If valueBytesLength > fieldLength Then fieldLength = CByte(valueBytesLength)
            Next
            'open stream to read the dbf
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'temporary file to write new dbf
            Dim tmpdbf As String = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            If File.Exists(tmpdbf) Then tmpdbf = Path.GetTempPath() & Guid.NewGuid().ToString() & ".dbf"
            Using writeFsDbf As New FileStream(tmpdbf, FileMode.Create)
                Using writeBwDbf As New BinaryWriter(writeFsDbf)
                    _parentDbfReader.DbReader.BaseStream.Position = 0
                    '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    writeBwDbf.Write(CByte(Now.Year - 1900)) 'YY    
                    writeBwDbf.Write(CByte(Now.Month))       'MM 
                    writeBwDbf.Write(CByte(Now.Day))         'DD 
                    _parentDbfReader.DbReader.BaseStream.Position += 3
                    '
                    Dim nRecords As Int32 = _parentDbfReader.DbReader.ReadInt32 'number of rows
                    writeBwDbf.Write(nRecords)
                    Dim firstDataRecordIndex As Int16 = _parentDbfReader.DbReader.ReadInt16 'always 32 + 32*number of fields + 1
                    writeBwDbf.Write(CShort(firstDataRecordIndex + 32))
                    Dim recordLen As Int16 = _parentDbfReader.DbReader.ReadInt16
                    writeBwDbf.Write(CShort(recordLen + fieldLength))
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(20))
                    '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
                    Do Until _parentDbfReader.DbReader.PeekChar() = 13
                        writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(32))
                    Loop
                    columnName = columnName.PadRight(10, Chr(0))
                    Dim asciiBytes As Byte() = Text.Encoding.ASCII.GetBytes(columnName)
                    writeBwDbf.Write(asciiBytes) 'FieldName
                    writeBwDbf.Write("C") 'FieldType
                    writeBwDbf.Write(CInt(0)) 'FieldDataAddress
                    writeBwDbf.Write(fieldLength) 'FieldLength
                    writeBwDbf.Write(CByte(0)) 'NDecimalPlaces
                    writeBwDbf.Write(CByte(0)) 'FieldFlag
                    writeBwDbf.Write(CInt(0)) 'AutoIncrementValue
                    writeBwDbf.Write(CByte(0)) 'AutoIncrementStep
                    For i As Int32 = 1 To 8
                        writeBwDbf.Write(CByte(0)) 'reserved
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '''''''''''''''''''''''''''''''''''''''Data as rows'''''''''''''''''''''''''''
                    _parentDbfReader.DbReader.BaseStream.Seek(firstDataRecordIndex, SeekOrigin.Begin)
                    Dim recordBytes() As Byte
                    For i As Int32 = 0 To nRecords - 1
                        recordBytes = _parentDbfReader.DbReader.ReadBytes(recordLen)
                        'If RecordBytes(0) = 32 Then
                        asciiBytes = Text.Encoding.ASCII.GetBytes(columnData(i).ToString.PadRight(fieldLength, Chr(32)))
                        'Else
                        'asciiBytes = Text.Encoding.ASCII.GetBytes(("").PadRight(FieldLength, Chr(32)))
                        'End If
                        writeBwDbf.Write(recordBytes)
                        writeBwDbf.Write(asciiBytes)
                    Next
                    writeBwDbf.Write(_parentDbfReader.DbReader.ReadBytes(1))
                    '
                    _parentDatabase.Close()
                End Using
            End Using
            '
            'update original
            Try
                File.Delete(_parentDatabase.DataBasePath)
            Catch ex As Exception
                MsgBox("Unable to save changes: " & ex.Message) : File.Delete(tmpdbf) : Exit Sub
            End Try
            '
            File.Copy(tmpdbf, _parentDatabase.DataBasePath)
            '
            '_parentDatabase.OnColumnAdded(_tableName, columnName, GetType(String))
            LoadAttributeInfo()
            File.Delete(tmpdbf)
            '
            If wasOpen Then _parentDatabase.Open()
        End Sub

#End Region


        ' ''' <summary>
        ' ''' A light version
        ' ''' </summary>
        ' ''' <returns></returns>
        ' ''' <remarks></remarks>
        'Public Function Get_Field_Data() As List(Of Array)
        '    Dim Result As New List(Of Array)
        '    For i As Int32 = 0 To _ColumnTypes.Count - 1
        '        Select Case _ColumnTypes(i)
        '            Case GetType(Int32)
        '                Dim TempInt(_NRows - 1) As Int32
        '                Result.Add(TempInt)
        '            Case GetType(Double)
        '                Dim TempDbl(_NRows - 1) As Double
        '                Result.Add(TempDbl)
        '            Case GetType(Boolean)
        '                Dim TempBool(_NRows - 1) As Boolean
        '                Result.Add(TempBool)
        '            Case GetType(String)
        '                Dim TempStr(_NRows - 1) As String
        '                Result.Add(TempStr)
        '            Case Else
        '                Dim TempStr(_NRows - 1) As String
        '                Result.Add(TempStr)
        '        End Select
        '    Next
        '    'Import the Attributes
        '    Try
        '        '
        '        ' read in the file
        '        Dim dbfBytes() As Byte
        '        If _ParentDatabase.DataBaseOpen = False Then _ParentDatabase.Open()
        '        '
        '        ' read in attribues for each row(shape)  
        '        Dim cellValue As String
        '        Dim StringData As New List(Of String)
        '        _dbfreader.BaseStream.Position = _firstDataRecordIndex + 1
        '        For i As Int32 = 0 To _NRows - 1
        '            dbfBytes = _dbfreader.ReadBytes(_recordLength)
        '            For j As Int32 = 0 To _Lengths.Count - 1
        '                CellValue = Text.Encoding.UTF8.GetString(dbfBytes, _Positions(j), _Lengths(j)).Trim(Chr(0))
        '                Select Case _ColumnTypes(j)
        '                    Case GetType(Int32)
        '                        If CellValue = "" Then CellValue = "0"
        '                        CType(Result(j), Int32())(i) = Integer.Parse(CellValue)
        '                    Case GetType(Double)
        '                        If CellValue = "" Then CellValue = "0"
        '                        CType(Result(j), Double())(i) = Double.Parse(CellValue) 'Convert.ToDouble(CellValue) 'CDbl(CellValue)
        '                    Case GetType(String)
        '                        'Result(j)(i) = CellValue.Trim(" ")
        '                        If StringData.Contains(CellValue.Trim(CChar(" "))) Then
        '                            CType(Result(j), String())(i) = StringData.Item(StringData.IndexOf(CellValue.Trim(CChar(" "))))
        '                        Else
        '                            StringData.Add(CellValue.Trim(CChar(" ")))
        '                            CType(Result(j), String())(i) = StringData.Last
        '                        End If
        '                    Case GetType(Boolean)
        '                        CType(Result(j), Boolean())(i) = Convert.ToBoolean(CellValue) 'CBool(CellValue)
        '                End Select
        '            Next
        '        Next
        '        ' all done
        '        Return Result
        '    Catch ex As Exception
        '        '
        '        ' an error occured
        '        MsgBox("An error reading in the shapefile attributes file: " &  _ParentDatabase.DatabasePath & vbCrLf & ex.Message, MsgBoxStyle.Exclamation)
        '        Return Nothing
        '    End Try

        'End Function

#Region "Database Cell Stuff"
        Private Function ConvertCellValueToProperType(ByVal cellValue As String, ByVal columnType As Type) As Object
            Select Case columnType
                Case GetType(Int32)
                    Dim i As Int32
                    If Integer.TryParse(cellValue, i) = False Then Return DBNull.Value
                    Return i
                Case GetType(Double)
                    Dim i As Double
                    If Double.TryParse(cellValue, i) = False Then Return DBNull.Value
                    Return i
                Case GetType(String)
                    Return cellValue.Trim(CChar(" "))
                Case GetType(Boolean)
                    Select Case cellValue
                        Case "0", "F", "f", "N", "n"
                            Return False
                        Case "1", "T", "t", "Y", "y"
                            Return True
                        Case Else
                            Return False
                    End Select
                Case Else
                    Throw New Exception("Column Type not supported")
            End Select
        End Function

        Protected Overrides Function GetStoredCell(ByVal storedColumnName As String, ByVal storedRowIndex As Int32) As Object
            Dim columnIndex As Int32 = Array.IndexOf(_storedColumnNames, storedColumnName)
            If columnIndex = -1 Then Throw New Exception("Column Name " & storedColumnName & " does not exist.")
            Return ConvertCellValueToProperType(ReadRawCellSafe(columnIndex, storedRowIndex), _storedColumnTypes(columnIndex))
        End Function
        Protected Overrides Function GetStoredCell(storedColumnIndex As Integer, storedRowIndex As Int32) As Object
            Return ConvertCellValueToProperType(ReadRawCellSafe(storedColumnIndex, storedRowIndex), _storedColumnTypes(storedColumnIndex))
        End Function
        Protected Overrides Function GetStoredCells(storedColumnIndices() As Int32, storedRowIndices() As Int32) As Object()
            Dim cells(storedColumnIndices.Count - 1) As Object
            For i As Int32 = 0 To storedColumnIndices.Count - 1
                cells(i) = ConvertCellValueToProperType(ReadRawCellSafe(storedColumnIndices(i), storedRowIndices(i)), _storedColumnTypes(storedColumnIndices(i)))
            Next
            Return cells
        End Function
        Private Function ReadRawCellSafe(ByVal col As Integer, ByVal row As Integer) As String
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            'much slower for some reason _dbfreader.BaseStream.Position = _firstDataRecordIndex + 1 + (row * _recordLength) + _Positions(col)
            '_dbfreader.BaseStream.Seek(_firstDataRecordIndex + 1 + (row * _recordLength) + _Positions(col), SeekOrigin.Begin)
            _parentDbfReader.DbReader.BaseStream.Position = _recordStartPositions(row) + _positions(col)
            Dim result As String = Text.Encoding.UTF8.GetString(_parentDbfReader.DbReader.ReadBytes(_lengths(col)), 0, _lengths(col)).Trim(Chr(0))
            If wasOpen = False Then _parentDatabase.Close()
            Return result
        End Function
        Public Function ReadRawCellUnsafe(ByVal columnIndex As Int32, ByVal rowIndex As Int32) As String
            'much slower for some reason _dbfreader.BaseStream.Position = _firstDataRecordIndex + 1 + (Row * _recordLength) + _Positions(Column)
            '_dbfreader.BaseStream.Seek(_firstDataRecordIndex + 1 + (Row * _recordLength) + _Positions(Column), SeekOrigin.Begin)
            _parentDbfReader.DbReader.BaseStream.Position = _recordStartPositions(rowIndex) + _positions(columnIndex)
            Return Text.Encoding.UTF8.GetString(_parentDbfReader.DbReader.ReadBytes(_lengths(columnIndex)), 0, _lengths(columnIndex)).Trim(Chr(0))
        End Function

        Protected Overrides Sub EditDatabaseCells(ByVal columnEditNames() As String, ByVal rowIndices() As Int32, ByVal cellValues() As Object)
            Dim columnIndices(columnEditNames.Count - 1) As Int32
            For i As Int32 = 0 To columnEditNames.Count - 1
                columnIndices(i) = Array.IndexOf(_storedColumnNames, columnEditNames(i))
                If columnIndices(i) = -1 Then Throw New Exception("The column: " & columnEditNames(i) & " does not exist in the database.")
            Next
            EditDatabaseCells(columnIndices, rowIndices, cellValues)
        End Sub
        Protected Overrides Sub EditDatabaseCells(columnIndices() As Integer, rowIndices() As Integer, cellValues() As Object)
            '
            For i As Int32 = 0 To columnIndices.Count - 1
                If ConvertToColumnType(_storedColumnTypes(columnIndices(i)), cellValues(i)) = False Then Throw New Exception("The desired Column: " & _storedColumnNames(i) & " is of type " & _storedColumnTypes(columnIndices(i)).ToString & " not of type " & cellValues(i).GetType.ToString & ".")
            Next
            '
            Dim sorted As List(Of KeyValuePair(Of Int32, Int32)) = columnIndices.ToList.[Select](Function(x, i) New KeyValuePair(Of Int32, Int32)(CInt(x), i)).OrderBy(Function(x) x.Key).ToList()
            Dim idx As List(Of Integer) = sorted.[Select](Function(x) x.Value).ToList
            '''''''''''''''''''''''''''''''''''''''Get the position of the value in the row'''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = True Then _parentDatabase.Close()
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                'update yymmdd to indicate date of edit 
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)
                '
                Dim value As String, asciiBytes As Byte(), formatString As String
                Dim nDecimals As Byte
                For i As Int32 = 0 To idx.Count - 1
                    Select Case _storedColumnTypes(columnIndices(idx(i)))
                        Case GetType(Byte), GetType(Int16), GetType(UInt16), GetType(Int32), GetType(UInt32), GetType(Int64), GetType(UInt64)
                            Do
                                dbfReaderFs.Position = _recordStartPositions(rowIndices(idx(i))) + _positions(columnIndices(idx(i)))
                                value = cellValues(idx(i)).ToString().PadLeft(_lengths(columnIndices(idx(i))), Chr(32)) 'probably needs space padding.value = DoubleData.ToString(FormatString).PadLeft(_Lengths(fieldindex), Chr(32))
                                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                                i += 1
                                If i > idx.Count - 1 Then Exit Do
                            Loop Until columnIndices(idx(i)) <> columnIndices(idx(i - 1))
                            i -= 1
                        Case GetType(Single), GetType(Double)
                            dbfReaderFs.Position = 32 + (columnIndices(idx(i))) * 32 + 17
                            nDecimals = dbfReaderFs.ReadByte
                            If _lengths(columnIndices(idx(i))) - nDecimals < 7 Then
                                formatString = "G"
                            Else
                                formatString = "0."
                                formatString = formatString.PadRight(formatString.Length + nDecimals, "0"c)
                                formatString = formatString & "e+000"
                            End If
                            Do
                                dbfReaderFs.Position = _recordStartPositions(rowIndices(idx(i))) + _positions(columnIndices(idx(i)))
                                value = CDbl(cellValues(idx(i))).ToString(formatString).PadRight(_lengths(columnIndices(idx(i))), Chr(32))
                                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                                i += 1
                                If i > idx.Count - 1 Then Exit Do
                            Loop Until columnIndices(idx(i)) <> columnIndices(idx(i - 1))
                            i -= 1
                        Case GetType(String)
                            Do
                                dbfReaderFs.Position = _recordStartPositions(rowIndices(idx(i))) + _positions(columnIndices(idx(i)))
                                If cellValues(idx(i)).Length > _lengths(columnIndices(idx(i))) Then
                                    value = cellValues(idx(i)).Substring(0, _lengths(columnIndices(idx(i))))
                                Else
                                    value = cellValues(idx(i)).PadRight(_lengths(columnIndices(idx(i))), Chr(32))
                                End If
                                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                                i += 1
                                If i > idx.Count - 1 Then Exit Do
                            Loop Until columnIndices(idx(i)) <> columnIndices(idx(i - 1))
                            i -= 1
                        Case GetType(Boolean)
                            Do
                                dbfReaderFs.Position = _recordStartPositions(rowIndices(idx(i))) + _positions(columnIndices(idx(i)))
                                If CBool(cellValues(idx(i))) Then
                                    value = "1" 'can be 1,T,t,Y,and y
                                Else
                                    value = "0" 'can be 0,F,f,N, and n
                                End If
                                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
                                i += 1
                                If i > idx.Count - 1 Then Exit Do
                            Loop Until columnIndices(idx(i)) <> columnIndices(idx(i - 1))
                            i -= 1
                            'Case Else

                    End Select
                Next

            End Using
            If wasOpen = True Then _parentDatabase.Open()
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

        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Double)
            If columnIndex >= _storedColumnNames.Count Then Throw New Exception("The column index does not exist in the database.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), cellValue) = False Then Throw New Exception("The desired Column: " & _storedColumnNames(columnIndex) & " is of type " & _storedColumnTypes(columnIndex).ToString & " not of type Double.")
            '''''''''''''''''''''''''''''''''''''''''Get the number of decimals from the Main File Header''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            _parentDbfReader.DbReader.BaseStream.Position = 32 + (columnIndex) * 32 + 17
            Dim nDecimals As Byte
            nDecimals = _parentDbfReader.DbReader.ReadByte
            _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''Get the position of the value in the row'''''''''''''''''''''''''''
            'update yymmdd to indicate date of edit 
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)
                'find format of field
                Dim value As String, asciiBytes As Byte(), formatString As String
                If _lengths(columnIndex) - nDecimals < 7 Then
                    formatString = "G"
                Else
                    formatString = "0."
                    formatString = formatString.PadRight(formatString.Length + nDecimals, "0"c)
                    formatString = formatString & "e+000"
                End If

                'edit the appropriate part of the dbf file
                dbfReaderFs.Position = _recordStartPositions(rowIndex) + _positions(columnIndex)
                value = cellValue.ToString(formatString).PadRight(_lengths(columnIndex), Chr(32))
                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
            End Using
            If wasOpen = True Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Single)
            If columnIndex >= _storedColumnNames.Count Then Throw New Exception("The column index does not exist in the database.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), cellValue) = False Then Throw New Exception("The desired Column: " & _storedColumnNames(columnIndex) & " is of type " & _storedColumnTypes(columnIndex).ToString & " not of type Double.")
            '''''''''''''''''''''''''''''''''''''''''Get the number of decimals from the Main File Header''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            _parentDbfReader.DbReader.BaseStream.Position = 32 + (columnIndex) * 32 + 17
            Dim nDecimals As Byte
            nDecimals = _parentDbfReader.DbReader.ReadByte
            _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''Get the position of the value in the row'''''''''''''''''''''''''''
            'update yymmdd to indicate date of edit 
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)
                'find format of field
                Dim value As String, asciiBytes As Byte(), formatString As String
                If _lengths(columnIndex) - nDecimals < 7 Then
                    formatString = "G"
                Else
                    formatString = "0."
                    formatString = formatString.PadRight(formatString.Length + nDecimals, "0"c)
                    formatString = formatString & "e+000"
                End If

                'edit the appropriate part of the dbf file
                dbfReaderFs.Position = _recordStartPositions(rowIndex) + _positions(columnIndex)
                value = cellValue.ToString(formatString).PadRight(_lengths(columnIndex), Chr(32))
                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
            End Using
            If wasOpen = True Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As String)
            If columnIndex >= _storedColumnNames.Count Then Throw New Exception("The column index does not exist in the database.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), cellValue) = False Then Throw New Exception("The desired Column: " & _storedColumnNames(columnIndex) & " is of type " & _storedColumnTypes(columnIndex).ToString & " not of type Double.")
            '''''''''''''''''''''''''''''''''''''''''Get the number of decimals from the Main File Header''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = True Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''Get the position of the value in the row'''''''''''''''''''''''''''
            'update yymmdd to indicate date of edit
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)

                Dim value As String, asciiBytes As Byte()

                'edit the appropriate part of the dbf file
                dbfReaderFs.Position = _recordStartPositions(rowIndex) + _positions(columnIndex)
                If cellValue.Length > _lengths(columnIndex) Then
                    value = cellValue.Substring(0, _lengths(columnIndex))
                Else
                    value = cellValue.PadRight(_lengths(columnIndex), Chr(32))
                End If
                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
            End Using
            If wasOpen = True Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Int32)
            If columnIndex >= _storedColumnNames.Count Then Throw New Exception("The column index does not exist in the database.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), cellValue) = False Then Throw New Exception("The desired Column: " & _storedColumnNames(columnIndex) & " is of type " & _storedColumnTypes(columnIndex).ToString & " not of type Double.")
            '''''''''''''''''''''''''''''''''''''''''Get the number of decimals from the Main File Header''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = True Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''Get the position of the value in the row'''''''''''''''''''''''''''
            'update yymmdd to indicate date of edit
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)

                Dim value As String, asciiBytes As Byte()
                'edit the appropriate part of the dbf file
                dbfReaderFs.Position = _recordStartPositions(rowIndex) + _positions(columnIndex)
                value = cellValue.ToString().PadLeft(_lengths(columnIndex), Chr(32)) 'probably needs space padding.value = DoubleData.ToString(FormatString).PadLeft(_Lengths(fieldindex), Chr(32))
                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
            End Using
            If wasOpen = True Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Int64)
            If columnIndex >= _storedColumnNames.Count Then Throw New Exception("The column index does not exist in the database.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), cellValue) = False Then Throw New Exception("The desired Column: " & _storedColumnNames(columnIndex) & " is of type " & _storedColumnTypes(columnIndex).ToString & " not of type Double.")
            '''''''''''''''''''''''''''''''''''''''''Get the number of decimals from the Main File Header''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = True Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''Get the position of the value in the row'''''''''''''''''''''''''''
            'update yymmdd to indicate date of edit
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)

                Dim value As String, asciiBytes As Byte()
                'edit the appropriate part of the dbf file
                dbfReaderFs.Position = _recordStartPositions(rowIndex) + _positions(columnIndex)
                value = cellValue.ToString().PadLeft(_lengths(columnIndex), Chr(32)) 'probably needs space padding.value = DoubleData.ToString(FormatString).PadLeft(_Lengths(fieldindex), Chr(32))
                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
            End Using
            If wasOpen = True Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Byte)
            If columnIndex >= _storedColumnNames.Count Then Throw New Exception("The column index does not exist in the database.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), cellValue) = False Then Throw New Exception("The desired Column: " & _storedColumnNames(columnIndex) & " is of type " & _storedColumnTypes(columnIndex).ToString & " not of type Double.")
            '''''''''''''''''''''''''''''''''''''''''Get the number of decimals from the Main File Header''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = True Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''Get the position of the value in the row'''''''''''''''''''''''''''
            'update yymmdd to indicate date of edit
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)

                Dim value As String, asciiBytes As Byte()
                'edit the appropriate part of the dbf file
                dbfReaderFs.Position = _recordStartPositions(rowIndex) + _positions(columnIndex)
                value = cellValue.ToString().PadLeft(_lengths(columnIndex), Chr(32)) 'probably needs space padding.value = DoubleData.ToString(FormatString).PadLeft(_Lengths(fieldindex), Chr(32))
                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
            End Using
            If wasOpen = True Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Short)
            If columnIndex >= _storedColumnNames.Count Then Throw New Exception("The column index does not exist in the database.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), cellValue) = False Then Throw New Exception("The desired Column: " & _storedColumnNames(columnIndex) & " is of type " & _storedColumnTypes(columnIndex).ToString & " not of type Double.")
            '''''''''''''''''''''''''''''''''''''''''Get the number of decimals from the Main File Header''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = True Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''Get the position of the value in the row'''''''''''''''''''''''''''
            'update yymmdd to indicate date of edit
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)

                Dim value As String, asciiBytes As Byte()

                'edit the appropriate part of the dbf file
                dbfReaderFs.Position = _recordStartPositions(rowIndex) + _positions(columnIndex)
                value = cellValue.ToString().PadLeft(_lengths(columnIndex), Chr(32)) 'probably needs space padding.value = DoubleData.ToString(FormatString).PadLeft(_Lengths(fieldindex), Chr(32))
                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
            End Using
            If wasOpen = True Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnIndex As Int32, rowIndex As Int32, cellValue As Boolean)
            If columnIndex >= _storedColumnNames.Count Then Throw New Exception("The column index does not exist in the database.")
            If ConvertToColumnType(_storedColumnTypes(columnIndex), cellValue) = False Then Throw New Exception("The desired Column: " & _storedColumnNames(columnIndex) & " is of type " & _storedColumnTypes(columnIndex).ToString & " not of type Double.")
            '''''''''''''''''''''''''''''''''''''''''Get the number of decimals from the Main File Header''''''''''''''''''''''''''''''''''''''''''''''''''
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = True Then _parentDatabase.Close()
            '''''''''''''''''''''''''''''''''''''''Get the position of the value in the row'''''''''''''''''''''''''''
            'update yymmdd to indicate date of edit 
            Using dbfReaderFs As New FileStream(_parentDatabase.DataBasePath, FileMode.Open, FileAccess.ReadWrite)
                Dim yymmdd(2) As Byte
                yymmdd(0) = CByte(Now.Year - 1900) 'YY 
                yymmdd(1) = CByte(Now.Month)       'MM 
                yymmdd(2) = CByte(Now.Day)         'DD 
                dbfReaderFs.Position = 1
                dbfReaderFs.Write(yymmdd, 0, 3)

                Dim value As String, asciiBytes As Byte()

                'edit the appropriate part of the dbf file
                dbfReaderFs.Position = _recordStartPositions(rowIndex) + _positions(columnIndex)
                If cellValue Then
                    value = "1" 'can be 1,T,t,Y,and y
                Else
                    value = "0" 'can be 0,F,f,N, and n
                End If
                asciiBytes = Text.Encoding.ASCII.GetBytes(value)
                dbfReaderFs.Write(asciiBytes, 0, asciiBytes.Count)
            End Using
            If wasOpen = True Then _parentDatabase.Open()
        End Sub
        Protected Overrides Sub EditDatabaseCell(columnindex As Integer, rowindex As Integer, cellvalue As Byte())
            Throw New NotImplementedException("DBF does not support storage of byte arrays.")
        End Sub
#End Region

        'Public Function readblockstrings(ByVal StartRow As Int32, ByVal EndRow As Int32) As String()
        '    _dbfreader.BaseStream.Seek(_firstDataRecordIndex + 1 + (StartRow * _recordLength), SeekOrigin.Begin)
        '    Dim StringValues((EndRow - StartRow + 1) * _NColumns - 1) As String

        '    Dim Bytes() As Byte = _dbfreader.ReadBytes(_recordLength * (EndRow - StartRow + 1))
        '    Dim Counter As Int32 = 0
        '    Dim ByteLocation As Int32 = 0
        '    Do Until StartRow = EndRow + 1
        '        For i As Int32 = 0 To _Lengths.Count - 1
        '            StringValues(Counter) = Text.Encoding.UTF8.GetString(Bytes, ByteLocation, _Lengths(i)).Trim(Chr(0))
        '            Counter += 1
        '            ByteLocation += _Lengths(i)
        '        Next
        '        StartRow += 1
        '    Loop

        '    Return StringValues
        'End Function
        'Public Function readblockobjects(ByVal StartRow As Int32, ByVal EndRow As Int32) As List(Of Object())
        '    _dbfreader.BaseStream.Seek(_firstDataRecordIndex + 1 + (StartRow * _recordLength), SeekOrigin.Begin)
        '    Dim Rows As New List(Of Object())(EndRow - StartRow + 1)

        '    Dim Bytes() As Byte = _dbfreader.ReadBytes(_recordLength * (EndRow - StartRow + 1))
        '    Dim RowBytes(_recordLength - 1) As Byte
        '    Dim Counter As Int32 = 0
        '    Dim ByteLocation As Int32 = 0
        '    Dim Row(_ColumnNames.Count - 1) As Object
        '    Dim cellValue As String
        '    Do Until StartRow = EndRow + 1
        '        Array.Copy(Bytes, ByteLocation, RowBytes, 0, _recordLength)
        '        For j = 0 To _ColumnNames.Count - 1
        '            CellValue = Text.Encoding.UTF8.GetString(RowBytes, _Positions(j), _Lengths(j)).Trim(Chr(0))
        '            Row(j) = ConvertCellValueToProperType(CellValue, _ColumnTypes(j))
        '        Next
        '        Rows.Add(Row.Clone)

        '        ByteLocation += _recordLength
        '        StartRow += 1
        '    Loop

        '    Return Rows
        'End Function

        Private Sub LoadAttributeInfo()
            Dim wasOpen As Boolean = _parentDatabase.DataBaseOpen
            If _parentDatabase.DataBaseOpen = False Then _parentDatabase.Open()
            _parentDbfReader.DbReader.BaseStream.Position = 0
            '''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
            Dim bytes() As Byte = _parentDbfReader.DbReader.ReadBytes(32)
            _storedNumberOfRows = BitConverter.ToInt32(bytes, 4) 'number of rows
            _firstDataRecordIndex = BitConverter.ToInt16(bytes, 8) 'always 32 + 32*number of fields + 1
            _recordLength = BitConverter.ToInt16(bytes, 10)
            Dim totalColumns As Int32 = (_firstDataRecordIndex - 32 - 1) \ 32
            '
            _recordStartPositions = New List(Of Int64)(_storedNumberOfRows)
            For i As Int64 = 0 To _storedNumberOfRows - 1
                _parentDbfReader.DbReader.BaseStream.Position = _firstDataRecordIndex + ((i) * _recordLength)
                'If _dbfreader.ReadByte = 32 Then
                _recordStartPositions.Add(_firstDataRecordIndex + 1 + (i * _recordLength))
                'End If
            Next
            '_NRows = _RecordStartPositions.Count
            '''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
            _parentDbfReader.DbReader.BaseStream.Position = 32
            ' create the table columns
            ReDim _lengths(totalColumns - 1)
            ReDim _positions(totalColumns - 1)
            ReDim _storedColumnNames(totalColumns - 1)
            ReDim _storedColumnTypes(totalColumns - 1)

            For col As Integer = 1 To totalColumns
                bytes = _parentDbfReader.DbReader.ReadBytes(32)
                _storedColumnNames(col - 1) = Text.Encoding.ASCII.GetString(bytes, 0, 10).Trim(CChar(""))
                ' field type
                Select Case Text.Encoding.UTF8.GetString(bytes, 11, 1)
                    Case "I"
                        _storedColumnTypes(col - 1) = GetType(Int32)
                    Case "N"
                        If bytes(17) > 0 Then 'number of decimals
                            _storedColumnTypes(col - 1) = GetType(Double)
                        Else
                            _storedColumnTypes(col - 1) = GetType(Int32)
                        End If
                    Case "F", "B"
                        _storedColumnTypes(col - 1) = GetType(Double)
                    Case "L"
                        _storedColumnTypes(col - 1) = GetType(Boolean)
                    Case "C"
                        _storedColumnTypes(col - 1) = GetType(String)
                    Case Else
                        _storedColumnTypes(col - 1) = GetType(String)
                End Select

                ' displacement of field
                'Dim Displacement As Integer = BitConverter.ToInt32(bytes, 12) 'unused beyond this.
                '
                ' field length
                _lengths(col - 1) = bytes(16)
                If col = 1 Then
                    _positions(col - 1) = 0
                Else
                    _positions(col - 1) = _positions(col - 2) + _lengths(col - 2)
                End If
                '
                ' number of decimal places
                '.NumDecimal = dbfBytes(pos + 17)
                '
                ' field flags 
                'Dim fieldFlags As Byte = dbfBytes(pos + 18)
                'Dim AutoIncrNext As Integer = BitConverter.ToInt32(dbfBytes, pos + 19)
                'Dim AutoIncrStep As Integer = BitConverter.ToInt32(dbfBytes, pos + 23)
            Next

            If wasOpen = False Then _parentDatabase.Close()
        End Sub

    End Class


    Public Shared Sub CreateDbf(ByVal outputPath As String, ByVal columnHeaders As List(Of String), ByVal recordData As List(Of Array), Optional ByVal overwrite As Boolean = True)
        If Path.GetExtension(outputPath).ToLower <> ".dbf" Then Throw New Exception("DBF path does not contain the proper extension (.dbf).")
        If File.Exists(outputPath) = True Then
            If overwrite = True Then
                File.Delete(outputPath)
            Else
                Throw New Exception("DBF file: " & Path.GetFileName(outputPath) & " aready exists in the chosen location.")
            End If
        End If
        '
        Try
            Dim fs As New FileStream(outputPath, FileMode.Create)
            ' dbf header (write now to fill space, then re-write again later)
            Dim header(31) As Byte
            ' dbf file type
            header(0) = 3
            ' last updated      
            header(1) = CByte(Now.Year - 1900) 'YY 
            header(2) = CByte(Now.Month)       'MM 
            header(3) = CByte(Now.Day)         'DD 
            ' code page mark(NFI)
            header(29) = 87
            header(29) = 57
            header(29) = 0
            fs.Write(header, 0, 32)
            ' fields          
            Dim dbfFields(recordData.Count - 1) As DbfField
            Dim recordLength As Short = 1
            For column As Integer = 0 To recordData.Count - 1
                Dim fieldbytes(31) As Byte
                ' column name
                Dim columnName As String = columnHeaders(column)
                If columnName.Length > 10 Then columnName = Left(columnName, 10)
                Array.Copy(Text.Encoding.UTF8.GetBytes(columnName), 0, fieldbytes, 0, Text.Encoding.UTF8.GetBytes(columnName).Length)
                ' column type        
                'dbfFields(Column).Type = RecordData(Column)(0).GetType
                Select Case recordData(column).GetType 'dbfFields(Column).Type
                    Case GetType(Int32()), GetType(UInt32())
                        dbfFields(column).TypeId = "N"
                        dbfFields(column).Length = Integer.MinValue.ToString.Length
                        dbfFields(column).NumDecimal = 0
                        dbfFields(column).WriteField = True
                        'dbfFields(Column).Type = GetType(Int32)
                    Case GetType(Short()), GetType(UShort())
                        dbfFields(column).TypeId = "N"
                        dbfFields(column).Length = Short.MinValue.ToString.Length
                        dbfFields(column).NumDecimal = 0
                        dbfFields(column).WriteField = True
                        'dbfFields(Column).Type = GetType(Short)
                    Case GetType(Byte)
                        dbfFields(column).TypeId = "N"
                        dbfFields(column).Length = 3
                        dbfFields(column).NumDecimal = 0
                        dbfFields(column).WriteField = True
                    Case GetType(Double()), GetType(Single())
                        dbfFields(column).TypeId = "F"
                        dbfFields(column).Length = 19
                        dbfFields(column).NumDecimal = 11
                        dbfFields(column).WriteField = True
                        'dbfFields(Column).Type = GetType(Double)
                    Case GetType(Boolean)
                        dbfFields(column).TypeId = "L"
                        dbfFields(column).Length = 1
                        dbfFields(column).NumDecimal = 0
                        dbfFields(column).WriteField = True
                    Case GetType(String())
                        Dim col As String() = CType(recordData(column), String())
                        'dbfFields(Column).Type = GetType(String)
                        dbfFields(column).TypeId = "C"
                        ' compute the maximum character length used in the table
                        dbfFields(column).Length = 0
                        For i As Int32 = 0 To col.Length - 1
                            If col(i) = "" Then 'DBNull.Value.Equals(col(i)) Then
                                ' null data
                            Else
                                ' character data
                                Dim cellText As String = col(i)
                                Dim cellBytesLength As Integer = Text.Encoding.UTF8.GetByteCount(cellText)
                                If cellBytesLength > dbfFields(column).Length Then dbfFields(column).Length = cellBytesLength
                            End If
                        Next
                        dbfFields(column).NumDecimal = 0
                        ' only write fields with length less or equal to 256 characters
                        dbfFields(column).WriteField = dbfFields(column).Length <= 256
                    Case Else
                        ' only write primative fields (skip the feature column)
                        dbfFields(column).WriteField = False
                End Select
                '
                If dbfFields(column).WriteField Then
                    fieldbytes(11) = Text.Encoding.UTF8.GetBytes(dbfFields(column).TypeId)(0)
                    fieldbytes(16) = CByte(dbfFields(column).Length)
                    fieldbytes(17) = CByte(dbfFields(column).NumDecimal)
                    recordLength = CShort(recordLength + dbfFields(column).Length)
                    '
                    ' write the field information
                    fs.Write(fieldbytes, 0, 32)
                End If
            Next
            ' blank space at the end of the header (spec say use 0, but xl says use 13)     
            fs.WriteByte(13)
            ' note the data starting byte position 
            Dim firstRecordPosition As Short = CShort(fs.Position)
            ' write out the feature datatable
            Dim recordCount As Integer = 0
            For i As Int32 = 0 To recordData(0).Length - 1
                ' deleted record indicator  (32 = space (' ') for not deleted, 42 = asterisk ('*') for deleted)     
                ' http://www.dbase.com/Knowledgebase/INT/db7_file_fmt.htm   
                fs.WriteByte(32)
                For column As Integer = 0 To recordData.Count - 1
                    If dbfFields(column).WriteField Then
                        Dim cellText As String
                        Select Case recordData(column).GetType
                            Case GetType(Int32()), GetType(UInt32())
                                cellText = CType(recordData(column), Int32())(i).ToString
                            Case GetType(Short()), GetType(UShort())
                                cellText = CType(recordData(column), Short())(i).ToString
                            Case GetType(Byte())
                                cellText = CType(recordData(column), Byte())(i).ToString
                            Case GetType(Single())
                                cellText = Format(CType(recordData(column), Single())(i), "0.00000000000e+000")
                            Case GetType(Double())
                                cellText = Format(CType(recordData(column), Double())(i), "0.00000000000e+000")
                            Case GetType(Boolean())
                                If CType(recordData(column), Boolean())(i) = True Then
                                    cellText = "1"
                                Else
                                    cellText = "0"
                                End If
                            Case Else
                                cellText = CType(recordData(column), String())(i)
                        End Select
                        ' get the bytes for the cell characters - use the UTF8 instead of ascii to handle extended characters (above 127)
                        Dim cellbytes() As Byte = Text.Encoding.UTF8.GetBytes(cellText)
                        ' write them to the filestream
                        fs.Write(cellbytes, 0, cellbytes.Count)
                        ' write blanks at the end to make up to the column length
                        For n As Integer = cellbytes.Length + 1 To dbfFields(column).Length
                            fs.WriteByte(32)
                        Next
                    End If
                Next
                recordCount += 1
            Next
            ' eof dbf file marker
            fs.WriteByte(26)

            fs.Position = 4
            fs.Write(BitConverter.GetBytes(recordCount), 0, 4)
            fs.Write(BitConverter.GetBytes(firstRecordPosition), 0, 2)
            fs.Write(BitConverter.GetBytes(recordLength), 0, 2)
            ' all done
            fs.Close()
        Catch ex As Exception
            ' an error occured
            Throw New Exception("An error writing shapefile attributes file: " & outputPath)
        End Try
    End Sub
    Public Shared Sub CreateDbf(ByVal outputPath As String, ByVal dt As DataTable, Optional ByVal overwrite As Boolean = True)
        If Path.GetExtension(outputPath).ToLower <> ".dbf" Then Throw New Exception("DBF path does not contain the proper extension (.dbf).")
        If File.Exists(outputPath) = True Then
            If overwrite = True Then
                File.Delete(outputPath)
            Else
                Throw New Exception("DBF file: " & Path.GetFileName(outputPath) & " aready exists in the chosen location.")
            End If
        End If
        '
        Try
            Dim fs As New FileStream(outputPath, FileMode.Create)
            ' dbf header (write now to fill space, then re-write again later)
            Dim header(31) As Byte
            ' dbf file type
            header(0) = 3
            ' last updated      
            header(1) = CByte(Now.Year - 1900) 'YY 
            header(2) = CByte(Now.Month)       'MM 
            header(3) = CByte(Now.Day)         'DD 
            ' code page mark(NFI)
            header(29) = 87
            header(29) = 57
            header(29) = 0
            fs.Write(header, 0, 32)
            ' fields          
            Dim dbfFields(dt.Columns.Count - 1) As DbfField
            Dim recordLength As Short = 1
            For column As Integer = 0 To dt.Columns.Count - 1
                Dim col As DataColumn = dt.Columns(column)
                Dim fieldbytes(31) As Byte
                ' column name
                Dim colname As String = col.ColumnName
                If colname.Length > 10 Then colname = Left(colname, 10)
                Array.Copy(Text.Encoding.UTF8.GetBytes(colname), 0, fieldbytes, 0, Text.Encoding.UTF8.GetBytes(colname).Length)
                ' column type        
                'dbfFields(Column).Type = col.DataType
                If col.DataType Is GetType(Integer) OrElse col.DataType Is GetType(UInteger) Then
                    dbfFields(column).TypeId = "N"
                    dbfFields(column).Length = Integer.MinValue.ToString.Length
                    dbfFields(column).NumDecimal = 0
                    dbfFields(column).WriteField = True
                ElseIf col.datatype Is GetType(Long) OrElse col.datatype Is GetType(ULong) Then
                    dbfFields(column).TypeId = "N"
                    dbfFields(column).Length = Long.MinValue.ToString.Length
                    dbfFields(column).NumDecimal = 0
                    dbfFields(column).WriteField = True
                ElseIf col.DataType Is GetType(Short) OrElse col.DataType Is GetType(UShort) Then
                    dbfFields(column).TypeId = "N"
                    dbfFields(column).Length = Short.MinValue.ToString.Length
                    dbfFields(column).NumDecimal = 0
                    dbfFields(column).WriteField = True
                ElseIf col.DataType Is GetType(Byte) Then
                    dbfFields(column).TypeId = "N"
                    dbfFields(column).Length = 3
                    dbfFields(column).NumDecimal = 0
                    dbfFields(column).WriteField = True
                ElseIf col.DataType Is GetType(Boolean) Then
                    dbfFields(column).TypeId = "L"
                    dbfFields(column).Length = 1
                    dbfFields(column).NumDecimal = 0
                    dbfFields(column).WriteField = True
                ElseIf col.DataType Is GetType(Double) OrElse col.DataType Is GetType(Single) Then
                    dbfFields(column).TypeId = "F"
                    dbfFields(column).Length = 19
                    dbfFields(column).NumDecimal = 11
                    dbfFields(column).WriteField = True
                ElseIf col.DataType Is GetType(String) Then
                    dbfFields(column).TypeId = "C"
                    ' compute the maximum character length used in the table
                    dbfFields(column).Length = 0
                    For Each row As DataRow In dt.Rows
                        If DBNull.Value.Equals(row(column)) Then
                            ' null data
                        Else
                            ' character data
                            Dim cellText As String = CStr(row(column))
                            Dim cellBytesLength As Integer = Text.Encoding.UTF8.GetByteCount(cellText)
                            If cellBytesLength > dbfFields(column).Length Then dbfFields(column).Length = cellBytesLength
                        End If
                    Next
                    dbfFields(column).NumDecimal = 0
                    ' only write fields with length less or equal to 256 characters
                    dbfFields(column).WriteField = dbfFields(column).Length <= 256
                Else
                    ' only write primative fields (skip the feature column)
                    dbfFields(column).WriteField = False
                End If
                '
                If dbfFields(column).WriteField Then
                    fieldbytes(11) = Text.Encoding.UTF8.GetBytes(dbfFields(column).TypeId)(0)
                    fieldbytes(16) = CByte(dbfFields(column).Length)
                    fieldbytes(17) = CByte(dbfFields(column).NumDecimal)
                    recordLength = CShort(recordLength + dbfFields(column).Length)
                    '
                    ' write the field information
                    fs.Write(fieldbytes, 0, 32)
                End If
            Next
            ' blank space at the end of the header (spec say use 0, but xl says use 13)     
            fs.WriteByte(13)
            ' note the data starting byte position 
            Dim firstRecordPosition As Short = CShort(fs.Position)
            ' write out the feature datatable
            Dim recordCount As Integer = 0
            For Each row As DataRow In dt.Rows
                ' deleted record indicator  (32 = space (' ') for not deleted, 42 = asterisk ('*') for deleted)     
                ' http://www.dbase.com/Knowledgebase/INT/db7_file_fmt.htm     
                fs.WriteByte(32)
                For c As Integer = 0 To dt.Columns.Count - 1
                    If dbfFields(c).WriteField Then
                        Dim cellText As String
                        If TypeOf (row(c)) Is DBNull Then
                            ' cell is blank
                            cellText = ""
                        ElseIf TypeOf (row(c)) Is Double OrElse TypeOf (row(c)) Is Single Then
                            ' col is floating point (single or double)
                            cellText = Format(row(c), "0.00000000000e+000")
                        ElseIf TypeOf (row(c)) Is Integer Then
                            ' col is integer
                            cellText = row(c).ToString
                        ElseIf TypeOf (row(c)) Is Boolean Then
                            If CType(row(c), Boolean) = True Then
                                cellText = "1"
                            Else
                                cellText = "0"
                            End If
                        Else
                            ' col is string
                            cellText = CStr(row(c))
                        End If
                        ' get the bytes for the cell characters - use the UTF8 instead of ascii to handle extended characters (above 127)
                        Dim cellbytes() As Byte = Text.Encoding.UTF8.GetBytes(cellText)
                        ' write them to the filestream
                        fs.Write(cellbytes, 0, cellbytes.Count)
                        ' write blanks at the end to make up to the column length
                        For n As Integer = cellbytes.Length + 1 To dbfFields(c).Length
                            fs.WriteByte(32)
                        Next
                    End If
                Next
                recordCount += 1
            Next
            ' eof dbf file marker
            fs.WriteByte(26)
            ' go back and update the header
            fs.Position = 4
            fs.Write(BitConverter.GetBytes(recordCount), 0, 4)
            fs.Write(BitConverter.GetBytes(firstRecordPosition), 0, 2)
            fs.Write(BitConverter.GetBytes(recordLength), 0, 2)
            ' all done
            fs.Close()
        Catch ex As Exception
            ' an error occured
            Throw New Exception("An error occurred when writing DBF file: " & outputPath & vbNewLine & ex.Message)
        End Try
    End Sub
End Class