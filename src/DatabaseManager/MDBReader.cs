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
using System.Data.OleDb;
using System.Linq;
using ADOX;

namespace DatabaseManager
{
    /// <summary>
    /// Provides read and write access to Microsoft Access MDB database files using OLE DB.
    /// This class enables table management, data manipulation, and schema operations for MDB files.
    /// </summary>
    /// <remarks>
    /// This implementation uses the Microsoft Jet OLE DB 4.0 provider which requires
    /// the Microsoft Access Database Engine to be installed on the system.
    /// </remarks>
    public class MdbReader : DatabaseManager
    {
        private readonly OleDbConnection _dbConnection;
        // Private _TableNames() As String
        // Private _CurrentTableName As String
        // Private _CurrentAutoIncrementColumn As String
        // Private _AutoIncrementSeed As Long
        // Private _AutoIncrementStep As Long
        // Public ReadOnly Property CurrentTableName As String
        // Get
        // Return _CurrentTableName
        // End Get
        // End Property
        // Public ReadOnly Property DBConnection As OleDbConnection
        // Get
        // Return _DBConnection
        // End Get
        // End Property

        /// <summary>
        /// Creates a new Microsoft Access MDB database file at the specified path.
        /// </summary>
        /// <param name="filePath">The full path where the new MDB file should be created.</param>
        /// <param name="overwrite">If true, overwrites an existing file at the path; if false, throws an exception when file exists.</param>
        /// <exception cref="Exception">Thrown when the file exists and overwrite is false.</exception>
        public static void CreateMdbFile(string filePath, bool overwrite = true)
        {
            if (System.IO.File.Exists(filePath))
            {
                if (overwrite == true)
                {
                    System.IO.File.Delete(filePath);
                }
                else
                {
                    throw new Exception("File named '" + System.IO.Path.GetFileName(filePath) + "' already exists in the specified directory.");
                }
            }
            var cat = new Catalog();
            try
            {
                cat.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath);
                cat = null;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cat = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MdbReader"/> class for the specified MDB file.
        /// </summary>
        /// <param name="dataBaseFile">The path to the MDB database file to open.</param>
        /// <exception cref="Exception">Thrown when the file does not have a .mdb extension.</exception>
        public MdbReader(string dataBaseFile)
        {
            if (System.IO.Path.GetExtension(dataBaseFile) != ".mdb")
                throw new Exception("Provided database file, " + System.IO.Path.GetFileName(dataBaseFile) + " is not an access .mdb file.");
            // 
            _dataBasePath = dataBaseFile;
            _dbConnection = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + dataBaseFile);
            _tableNames = GetTableNames();
        }

        /// <summary>
        /// Retrieves the names of all user tables in the MDB database.
        /// </summary>
        /// <returns>An array of table names, excluding system tables.</returns>
        public override string[] GetTableNames()
        {
            bool wasOpen = _dataBaseOpen;
            if (_dataBaseOpen == false)
                Open();
            // We only want user tables, not system tables
            var restrictions = new string[4];
            restrictions[3] = "Table";
            var tables = _dbConnection.GetSchema("Tables", restrictions);
            var result = new string[tables.Rows.Count];
            for (int i = 0, loopTo = result.Count() - 1; i <= loopTo; i++)
                result[i] = tables.Rows[i][2].ToString();
            if (wasOpen == false)
                Close();
            return result;
        }

        /// <summary>
        /// Opens the connection to the MDB database.
        /// </summary>
        public override void Open()
        {
            _dbConnection.Open();
            _dataBaseOpen = true;
        }

        /// <summary>
        /// Closes the connection to the MDB database.
        /// </summary>
        public override void Close()
        {
            _dbConnection.Close();
            _dataBaseOpen = false;
        }

        /// <summary>
        /// Gets a table manager for the specified table in the MDB database.
        /// </summary>
        /// <param name="tableName">The name of the table to manage.</param>
        /// <returns>A <see cref="DataTableView"/> instance for interacting with the specified table.</returns>
        public override DataTableView GetTableManager(string tableName)
        {
            return new MdbTableReader(this, tableName, _dbConnection);
        }

        /// <summary>
        /// Deletes a table from the MDB database.
        /// </summary>
        /// <param name="tableName">The name of the table to delete.</param>
        /// <remarks>If the table does not exist, this method returns without error.</remarks>
        public void DeleteTable(string tableName)
        {
            if (_tableNames.Contains(tableName) == false)
                return;
            bool wasOpen = _dataBaseOpen;
            if (_dataBaseOpen == false)
                Open();
            // 
            using (var cmd = _dbConnection.CreateCommand())
            {
                cmd.CommandText = "DROP TABLE [" + tableName + "]";
                cmd.ExecuteNonQuery();
            }
            _tableNames = GetTableNames();
            if (wasOpen == false)
                Close();
        }

        /// <summary>
        /// Gets the number of rows stored in the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table to query.</param>
        /// <returns>The total number of rows in the table.</returns>
        public override long GetStoredNumberOfRows(string tableName)
        {
            bool wasOpen = _dataBaseOpen;
            if (_dataBaseOpen == false)
                Open();
            var createcmd = new OleDbCommand("SELECT COUNT(*) From " + tableName, _dbConnection);
            long nRows = Convert.ToInt64(createcmd.ExecuteScalar());
            if (wasOpen == false)
                Close();
            return nRows;
        }

        /// <summary>
        /// Gets the number of columns in the specified table.
        /// </summary>
        /// <param name="tableName">The name of the table to query.</param>
        /// <returns>The total number of columns in the table.</returns>
        public override int GetStoredNumberOfColumns(string tableName)
        {
            bool wasOpen = _dataBaseOpen;
            if (_dataBaseOpen == false)
                Open();
            // 
            int columnCount;
            using (var theOleDbcommand = new OleDbCommand("SELECT * FROM " + tableName, _dbConnection))
            {
                using (var theAdapter = new OleDbDataAdapter(theOleDbcommand))
                {
                    using (var theSchemaTable = new DataTable())
                    {
                        theAdapter.FillSchema(theSchemaTable, SchemaType.Source);
                        columnCount = theSchemaTable.Columns.Count;
                    }
                }
            }
            // 
            if (wasOpen == false)
                Close();
            return columnCount;
        }

        /// <summary>
        /// Provides table-level data access and editing capabilities for MDB database tables.
        /// </summary>
        private class MdbTableReader : DataTableView
        {
            private int[] _rowIdArray;
            private readonly OleDbConnection _dbConnection;
            private readonly string _autoIncrementColumn;

            /// <summary>
            /// Initializes a new instance of the <see cref="MdbTableReader"/> class.
            /// </summary>
            /// <param name="reader">The parent <see cref="MdbReader"/> instance.</param>
            /// <param name="dataTableName">The name of the table to manage.</param>
            /// <param name="dbConnection">The OLE DB connection to use for database operations.</param>
            public MdbTableReader(MdbReader reader, string dataTableName, OleDbConnection dbConnection)
            {
                _tableName = dataTableName;
                _dbConnection = dbConnection;
                _parentDatabase = reader;
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                _storedNumberOfRows = (int)GetStoredRowCount();
                InitializeView();
                // _columnNames = GetStoredColumnNames()
                // _columnTypes = GetStoredColumnTypes()
                // '_nColumns = _storedColumnNames.Count
                // _nRows = _storedNumberOfRows
                // mdb specific
                _rowIdArray = GetRowIdArray();
                _autoIncrementColumn = GetAutoIncrementColumn();
                // 
                // AddHandler _parentDatabase.RowAdded, Sub(changedTableName As String, rowData() As Object) RowDataChanged(changedTableName)
                // AddHandler _parentDatabase.RowsAdded, Sub(changedTableName As String, rowData As List(Of Object())) RowDataChanged(changedTableName)
                // AddHandler _parentDatabase.RowDeleted, Sub(changedTableName As String, rowIndex As Int32) RowDataChanged(changedTableName)
                // AddHandler _parentDatabase.RowsDeleted, Sub(changedTableName As String, rowIndices() As Int32) RowDataChanged(changedTableName)
                // AddHandler _parentDatabase.ColumnAdded, Sub(changedTableName As String, columnName As String, columnType As Type) ColumnDataChanged(changedTableName)
                // AddHandler _parentDatabase.ColumnDeleted, Sub(changedTableName As String, columnName As String) ColumnDataChanged(changedTableName)
                // AddHandler _parentDatabase.ColumnsDeleted, Sub(changedTableName As String, columnsDeleted() As String) ColumnDataChanged(changedTableName)
            }

            // Private Sub RowDataChanged(ByVal changedTableName As String)
            // If changedTableName <> _tableName Then Exit Sub
            // _rowIdArray = GetRowIdArray()
            // '_nRows = GetStoredRowCount()
            // End Sub
            // Private Sub ColumnDataChanged(ByVal changedTableName As String)
            // If changedTableName <> _tableName Then Exit Sub
            // _storedColumnNames = GetStoredColumnNames()
            // _storedColumnTypes = GetStoredColumnTypes()
            // '_nColumns = _storedColumnNames.Count
            // End Sub

            #region Define Table for function calls
            // Public Sub SetTableReader(ByVal TableName As String)
            // _CurrentTableName = TableName
            // _NRows = GetStoredRowCount(_CurrentTableName)
            // SetColumnInfo(_CurrentTableName)
            // _NColumns = _ColumnNames.Count
            // If _CurrentAutoIncrementColumn = "" Then
            // AddAutoIncrementColumn(_CurrentTableName)
            // End If
            // '
            // Close()
            // End Sub
            protected override ulong GetStoredRowCount()
            {
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                var createcmd = new OleDbCommand("SELECT COUNT(*) From " + _tableName, _dbConnection);
                return Convert.ToUInt64(createcmd.ExecuteScalar());
            }
            protected override string[] GetStoredColumnNames()
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                string[] result;
                using (var theOleDbcommand = new OleDbCommand("SELECT * FROM " + _tableName, _dbConnection))
                {
                    using (var theAdapter = new OleDbDataAdapter(theOleDbcommand))
                    {
                        using (var theSchemaTable = new DataTable())
                        {
                            theAdapter.FillSchema(theSchemaTable, SchemaType.Source);
                            result = new string[theSchemaTable.Columns.Count];
                            int counter = 0;
                            foreach (DataColumn col in theSchemaTable.Columns)
                            {
                                result[counter] = col.ColumnName;
                                counter += 1;
                            }
                        }
                        // 
                    }
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
                return result;
            }
            protected override Type[] GetStoredColumnTypes()
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                Type[] result;
                using (var theOleDbcommand = new OleDbCommand("SELECT * FROM " + _tableName, _dbConnection))
                {
                    using (var theAdapter = new OleDbDataAdapter(theOleDbcommand))
                    {
                        using (var theSchemaTable = new DataTable())
                        {
                            theAdapter.FillSchema(theSchemaTable, SchemaType.Source);
                            result = new Type[theSchemaTable.Columns.Count];
                            int counter = 0;
                            foreach (DataColumn col in theSchemaTable.Columns)
                            {
                                result[counter] = col.DataType;
                                counter += 1;
                            }
                        }
                        // 
                    }
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
                return result;
            }
            private int[] GetRowIdArray()
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                string autoIncrColumn = GetAutoIncrementColumn();
                if (string.IsNullOrEmpty(autoIncrColumn))
                {
                    AddAutoIncrementColumn();
                }
                else
                {
                    // Create Indexes if they don't already exist
                    var restrictions = new string[5];
                    bool createAutoIndex = true;
                    try
                    {
                        // Using theOleDbcommand As New OleDbCommand("SELECT * FROM " & _tableName, _dbConnection)
                        // Using theAdapter As New OleDbDataAdapter(theOleDbcommand)
                        restrictions[4] = _tableName;
                        using (var theSchemaTable = _dbConnection.GetSchema("INDEXES", restrictions))
                        {
                            if (theSchemaTable.Rows.Count > 0)
                            {
                                foreach (DataRow row in theSchemaTable.Rows)
                                {
                                    if ((row["COLUMN_NAME"].ToString() ?? "") == (autoIncrColumn ?? ""))
                                    {
                                        createAutoIndex = false;
                                        break;
                                    }
                                }
                            }
                            if (createAutoIndex == true)
                            {
                                using (var addIndexcommand = new OleDbCommand("CREATE INDEX IDIndex ON " + _tableName + " (" + autoIncrColumn + ")", _dbConnection))
                                {
                                    addIndexcommand.ExecuteNonQuery();
                                }
                            }
                            // End Using
                            // End Using
                        }
                    }
                    catch
                    {
                        // MsgBox("Error with indexes")
                    }
                }
                // 
                var column = new int[(int)Math.Round(GetStoredRowCount() - 1m + 1)];
                int columnIndex = Array.IndexOf(_storedColumnNames, autoIncrColumn);
                using (var cmd = new OleDbCommand("SELECT [" + autoIncrColumn + "] FROM [" + _tableName + "]", _dbConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            int counter = 0;
                            while (reader.Read())
                            {
                                column[counter] = reader.GetInt32(columnIndex);
                                counter += 1;
                            }
                        }
                    }
                }

                if (wasOpen == false)
                    _parentDatabase.Close();
                return column;

            }
            private string GetAutoIncrementColumn()
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                string autoIncrementColumn = "";
                using (var theOleDbcommand = new OleDbCommand("SELECT * FROM " + _tableName, _dbConnection))
                {
                    using (var theAdapter = new OleDbDataAdapter(theOleDbcommand))
                    {
                        using (var theSchemaTable = new DataTable())
                        {
                            theAdapter.FillSchema(theSchemaTable, SchemaType.Source);
                            foreach (DataColumn col in theSchemaTable.Columns)
                            {
                                if (col.AutoIncrement == true)
                                    autoIncrementColumn = col.ColumnName;
                            }
                        }
                        // 
                    }
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
                return autoIncrementColumn;
            }
            // Private Sub SetColumnInfo(ByVal TableName As String)
            // If _ParentDatabase.DataBaseOpen = False Then _ParentDatabase.Open()
            // Dim theOleDBcommand As New OleDbCommand("SELECT * FROM " & TableName, _DBConnection)
            // Dim theAdapter As New OleDbDataAdapter(theOleDBCommand)

            // Dim theSchemaTable As New DataTable
            // theAdapter.FillSchema(theSchemaTable, SchemaType.Source)
            // Dim Found As Int32 = 0
            // ReDim _ColumnNames(theSchemaTable.Columns.Count - 1)
            // ReDim _ColumnTypes(_ColumnNames.Count - 1)
            // Dim counter As Int32 = 0
            // For Each col As DataColumn In theSchemaTable.Columns
            // _ColumnNames(Counter) = col.ColumnName
            // _ColumnTypes(Counter) = col.DataType
            // If col.AutoIncrement Then
            // Found += 1
            // _CurrentAutoIncrementColumn = col.ColumnName
            // _AutoIncrementSeed = col.AutoIncrementSeed
            // _AutoIncrementStep = col.AutoIncrementStep
            // End If
            // Counter += 1
            // Next
            // '
            // If Found <> 1 Then _CurrentAutoIncrementColumn = ""
            // If Found = 1 Then
            // 'Create Indexes if they don't already exist
            // Dim restrictions(4) As String
            // theSchemaTable = New DataTable
            // theOleDBCommand = New OleDbCommand
            // Dim CreateAutoIndex As Boolean = True
            // Try
            // restrictions(4) = TableName
            // theSchemaTable = _DBConnection.GetSchema("INDEXES", restrictions)
            // If theSchemaTable.Rows.Count > 0 Then
            // For Each row As DataRow In theSchemaTable.Rows
            // If row("COLUMN_NAME").ToString() = _CurrentAutoIncrementColumn Then
            // CreateAutoIndex = False
            // Exit For
            // End If
            // Next
            // End If
            // If CreateAutoIndex = True Then
            // theOleDBCommand = New OleDbCommand("CREATE INDEX IDIndex ON " & TableName & " (" & _CurrentAutoIncrementColumn & ")", _DBConnection)
            // theOleDBCommand.ExecuteNonQuery()
            // End If
            // Catch
            // 'MsgBox("Error with indexes")
            // End Try

            // Dim columnNamesWithoutAuto As New List(Of String)
            // Dim columnTypesWithoutAuto As New List(Of Type)
            // For i As Int32 = 0 To _ColumnNames.Count - 1
            // If _ColumnNames(i) = _CurrentAutoIncrementColumn Then
            // '
            // Else
            // ColumnNamesWithoutAuto.Add(_ColumnNames(i))
            // ColumnTypesWithoutAuto.Add(_ColumnTypes(i))
            // End If
            // Next
            // _ColumnNames = ColumnNamesWithoutAuto.ToArray
            // _ColumnTypes = ColumnTypesWithoutAuto.ToArray
            // End If
            // '
            // theAdapter.Dispose()
            // theOleDBCommand.Dispose()
            // theSchemaTable.Dispose()

            // 'Dim Restrictions As String() = New String() {Nothing, Nothing, TableName, Nothing}
            // 'Dim columnNames As New List(Of String)
            // 'Dim columnTypes As New List(Of Type)
            // 'Dim DT As DataTable = _DBConnection.GetSchema("Columns", Restrictions)
            // 'For Each DR As DataRow In DT.Rows
            // '    StoredColumnNames.Add(DR("Column_Name").ToString)
            // '    StoredColumnTypes.Add(oleDbToNetTypeConverter(DR("Data_Type")))
            // 'Next
            // '_ColumnNames = StoredColumnNames.ToArray
            // '_ColumnTypes = StoredColumnTypes.ToArray
            // 'DT.Dispose()
            // End Sub
            private void AddAutoIncrementColumn()
            {
                string[] possibleAutoNames = new string[] { "ID", "OID", "OBJECTID", "COUNTER", "ROWID", "AUTOID", "ROWINDEX", "TAG", "ID_1", "ID_2", "ID_3", "OID_1", "OID_2", "OID_3" };
                string chosenAutoName = "";
                for (int i = 0, loopTo = possibleAutoNames.Count() - 1; i <= loopTo; i++)
                {
                    if (_storedColumnNames.Contains(possibleAutoNames[i]) == false)
                    {
                        chosenAutoName = possibleAutoNames[i];
                        break;
                    }
                }

                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                try
                {
                    string commandString = "ALTER TABLE " + _tableName + " ADD COLUMN " + chosenAutoName + " IDENTITY"; // INT NOT NULL AUTO_INCREMENT, ADD PRIMARY KEY (" & ChosenAutoName & ")"
                    using (var command = new OleDbCommand(commandString, _dbConnection))
                    {
                        command.ExecuteNonQuery();
                        // _CurrentAutoIncrementColumn = chosenAutoName
                        // 
                        command.CommandText = "CREATE INDEX IDIndex ON " + _tableName + " (" + chosenAutoName + ")";
                        command.ExecuteNonQuery();
                    }
                    if (wasOpen == false)
                        _parentDatabase.Close();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Increase MaxLocksPerFile"))
                    {
                        try
                        {
                            var adodbConn = new ADODB.Connection();
                            adodbConn.Open("Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + _parentDatabase.DataBasePath);
                            adodbConn.Properties["Jet OLEDB:Max Locks Per File"].Value = 64000;

                            object recordsAffected;
                            string commandString = "ALTER TABLE " + _tableName + " ADD COLUMN " + chosenAutoName + " IDENTITY";
                            adodbConn.Execute(commandString, out recordsAffected,1);
                            adodbConn.Close();
                            // 
                            using (var command = new OleDbCommand("CREATE INDEX IDIndex ON " + _tableName + " (" + chosenAutoName + ")", _dbConnection))
                            {
                                command.ExecuteNonQuery();
                            }
                            if (wasOpen == false)
                                _parentDatabase.Close();
                        }
                        catch (Exception exLocks)
                        {
                            if (wasOpen == false)
                                _parentDatabase.Close();
                            throw exLocks;
                        }
                    }
                    else
                    {
                        if (wasOpen == false)
                            _parentDatabase.Close();
                        throw ex;
                    }

                }

            }
            #endregion

            #region Column Stuff
            // Protected Overrides Sub AddColumnToDatabase(columnName As String, columnData() As Object)
            // Select Case columnData(0).GetType
            // Case GetType(Byte)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Byte)().ToArray())
            // Case GetType(Short)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Short)().ToArray())
            // Case GetType(Integer)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Integer)().ToArray())
            // Case GetType(Long)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Long)().ToArray())
            // Case GetType(Single)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Single)().ToArray())
            // Case GetType(Double)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Double)().ToArray())
            // Case GetType(Boolean)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Boolean)().ToArray())
            // Case GetType(String)
            // EditDatabaseColumn(columnName, columnData.Cast(Of String)().ToArray())
            // End Select
            // End Sub
            protected override void AddColumnToDatabase(string columnName, byte[][] columnData)
            {
                if (_storedColumnNames.Contains(columnName) == true)
                    throw new Exception("Column Name " + columnName + " already exists, choose a different name.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("Number of records do not match the number of records for the new column.");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var command = new OleDbCommand("ALTER TABLE " + _tableName + " ADD COLUMN [" + columnName + "] LONGBINARY", _dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                // 
                EditDatabaseColumn(columnName, columnData);
                // 
                // _parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Boolean))
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void AddColumnToDatabase(string columnName, bool[] columnData)
            {
                if (_storedColumnNames.Contains(columnName) == true)
                    throw new Exception("Column Name " + columnName + " already exists, choose a different name.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("Number of records do not match the number of records for the new column.");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var command = new OleDbCommand("ALTER TABLE " + _tableName + " ADD COLUMN [" + columnName + "] LOGICAL", _dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = new OleDbCommand())
                    {
                        command.Transaction = tr;
                        command.Connection = _dbConnection;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]=" + columnData[i] + " WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    // 
                    tr.Commit();
                }
                // 
                // _parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Boolean))
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void AddColumnToDatabase(string columnName, byte[] columnData)
            {
                if (_storedColumnNames.Contains(columnName) == true)
                    throw new Exception("Column Name " + columnName + " already exists, choose a different name.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("Number of records do not match the number of records for the new column.");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var command = new OleDbCommand("ALTER TABLE " + _tableName + " ADD COLUMN [" + columnName + "] BYTE", _dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = new OleDbCommand())
                    {
                        command.Transaction = tr;
                        command.Connection = _dbConnection;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]=" + columnData[i] + " WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    // 
                    tr.Commit();
                }
                // 
                // _parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Byte))
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void AddColumnToDatabase(string columnName, double[] columnData)
            {
                if (_storedColumnNames.Contains(columnName) == true)
                    throw new Exception("Column Name " + columnName + " already exists, choose a different name.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("Number of records do not match the number of records for the new column.");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var command = new OleDbCommand("ALTER TABLE " + _tableName + " ADD COLUMN [" + columnName + "] DOUBLE", _dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = new OleDbCommand())
                    {
                        command.Transaction = tr;
                        command.Connection = _dbConnection;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]=" + columnData[i] + " WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    // 
                    tr.Commit();
                }
                // 
                // _parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Double))
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void AddColumnToDatabase(string columnName, int[] columnData)
            {
                if (_storedColumnNames.Contains(columnName) == true)
                    throw new Exception("Column Name " + columnName + " already exists, choose a different name.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("Number of records do not match the number of records for the new column.");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var command = new OleDbCommand("ALTER TABLE " + _tableName + " ADD COLUMN [" + columnName + "] INT", _dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = new OleDbCommand())
                    {
                        command.Transaction = tr;
                        command.Connection = _dbConnection;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]=" + columnData[i] + " WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    // 
                    tr.Commit();
                }
                // 
                // _parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Integer))
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void AddColumnToDatabase(string columnName, long[] columnData)
            {
                if (_storedColumnNames.Contains(columnName) == true)
                    throw new Exception("Column Name " + columnName + " already exists, choose a different name.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("Number of records do not match the number of records for the new column.");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var command = new OleDbCommand("ALTER TABLE " + _tableName + " ADD COLUMN [" + columnName + "] INTEGER8", _dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = new OleDbCommand())
                    {
                        command.Transaction = tr;
                        command.Connection = _dbConnection;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]=" + columnData[i] + " WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    // 
                    tr.Commit();
                }
                // 
                // _parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Long))
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void AddColumnToDatabase(string columnName, short[] columnData)
            {
                if (_storedColumnNames.Contains(columnName) == true)
                    throw new Exception("Column Name " + columnName + " already exists, choose a different name.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("Number of records do not match the number of records for the new column.");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var command = new OleDbCommand("ALTER TABLE " + _tableName + " ADD COLUMN [" + columnName + "] SHORT", _dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = new OleDbCommand())
                    {
                        command.Transaction = tr;
                        command.Connection = _dbConnection;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]=" + columnData[i] + " WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    // 
                    tr.Commit();
                }
                // 
                // _parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Short))
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void AddColumnToDatabase(string columnName, float[] columnData)
            {
                if (_storedColumnNames.Contains(columnName) == true)
                    throw new Exception("Column Name " + columnName + " already exists, choose a different name.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("Number of records do not match the number of records for the new column.");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var command = new OleDbCommand("ALTER TABLE " + _tableName + " ADD COLUMN [" + columnName + "] SINGLE", _dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = new OleDbCommand())
                    {
                        command.Transaction = tr;
                        command.Connection = _dbConnection;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]=" + columnData[i] + " WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    // 
                    tr.Commit();
                }
                // 
                // _parentDatabase.OnColumnAdded(_tableName, columnName, GetType(Single))
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void AddColumnToDatabase(string columnName, string[] columnData)
            {
                if (_storedColumnNames.Contains(columnName) == true)
                    throw new Exception("Column Name " + columnName + " already exists, choose a different name.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("Number of records do not match the number of records for the new column.");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var command = new OleDbCommand("ALTER TABLE " + _tableName + " ADD COLUMN [" + columnName + "] Text(50)", _dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = new OleDbCommand())
                    {
                        command.Transaction = tr;
                        command.Connection = _dbConnection;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]='" + columnData[i] + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    // 
                    tr.Commit();
                }
                // 
                // _parentDatabase.OnColumnAdded(_tableName, columnName, GetType(String))
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }

            protected override void DeleteColumnFromDatabase(string columnName)
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var command = new OleDbCommand("ALTER TABLE " + _tableName + " DROP COLUMN [" + columnName + "]", _dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                // 
                // _parentDatabase.OnColumnsDeleted(_tableName, {columnName})
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void DeleteColumnsFromDatabase(string[] columnsToDelete)
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = _dbConnection.CreateCommand())
                    {
                        command.Transaction = tr;
                        for (int i = 0, loopTo = columnsToDelete.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "ALTER TABLE " + _tableName + " DROP COLUMN [" + columnsToDelete[i] + "]";
                            command.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                // 
                // _parentDatabase.OnColumnsDeleted(_tableName, columnsToDelete)
                _storedColumnNames = GetStoredColumnNames();
                _storedColumnTypes = GetStoredColumnTypes();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            // Protected Overrides Sub EditDatabaseColumn(columnName As String, columnData() As Object)
            // Select Case columnData(0).GetType
            // Case GetType(Byte)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Byte)().ToArray())
            // Case GetType(Short)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Short)().ToArray())
            // Case GetType(Integer)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Integer)().ToArray())
            // Case GetType(Long)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Long)().ToArray())
            // Case GetType(Single)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Single)().ToArray())
            // Case GetType(Double)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Double)().ToArray())
            // Case GetType(Boolean)
            // EditDatabaseColumn(columnName, columnData.Cast(Of Boolean)().ToArray())
            // Case GetType(String)
            // EditDatabaseColumn(columnName, columnData.Cast(Of String)().ToArray())
            // End Select
            // End Sub
            protected override void EditDatabaseColumn(string columnName, bool[] columnData)
            {
                if (columnData.Count() == 0)
                    return;
                int columnIndex = Array.IndexOf(_storedColumnNames, columnName);
                if (columnIndex < 0)
                    throw new Exception("The desired Field Name: " + columnName + " does not exist.");
                bool localConvertToColumnType() { var tmp = columnData; object argvalue = tmp[0]; var ret = ConvertToColumnType(_storedColumnTypes[columnIndex], ref argvalue); tmp[0] = Convert.ToBoolean(argvalue); return ret; }

                if (localConvertToColumnType() == false)
                    throw new Exception("The desired Field: " + columnName + " is of type " + _storedColumnTypes[columnIndex].ToString() + "not of type '" + columnData[0].GetType().ToString() + "'.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("The table '" + _tableName + "' does not have: " + columnData.Count() + " records");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = _dbConnection.CreateCommand())
                    {
                        command.Transaction = tr;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]='" + columnData[i] + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void EditDatabaseColumn(string columnName, byte[] columnData)
            {
                if (columnData.Count() == 0)
                    return;
                int columnIndex = Array.IndexOf(_storedColumnNames, columnName);
                if (columnIndex < 0)
                    throw new Exception("The desired Field Name: " + columnName + " does not exist.");
                bool localConvertToColumnType() { var tmp1 = columnData; object argvalue1 = tmp1[0]; var ret = ConvertToColumnType(_storedColumnTypes[columnIndex], ref argvalue1); tmp1[0] = Convert.ToByte(argvalue1); return ret; }

                if (localConvertToColumnType() == false)
                    throw new Exception("The desired Field: " + columnName + " is of type " + _storedColumnTypes[columnIndex].ToString() + "not of type '" + columnData[0].GetType().ToString() + "'.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("The table '" + _tableName + "' does not have: " + columnData.Count() + " records");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = _dbConnection.CreateCommand())
                    {
                        command.Transaction = tr;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]='" + columnData[i] + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void EditDatabaseColumn(string columnName, byte[][] columnData)
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                if (columnData.Count() == 0)
                    return;
                int columnIndex = Array.IndexOf(_storedColumnNames, columnName);
                if (columnIndex == -1)
                    throw new Exception("The desired Field Name: " + columnName + " does not exist.");
                bool localConvertToColumnType() { var tmp2 = columnData; object argvalue2 = tmp2[0]; var ret = ConvertToColumnType(_storedColumnTypes[columnIndex], ref argvalue2); tmp2[0] = (byte[])argvalue2; return ret; }

                if (localConvertToColumnType() == false)
                    throw new Exception("The desired Field: '" + columnName + "' is of type '" + _storedColumnTypes[columnIndex].ToString() + "' not of type '" + columnData[0].GetType().ToString() + "'.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("The table '" + _tableName + "' does not have: " + columnData.Count() + " records");
                // 
                string commandText = "UPDATE [" + _tableName + "] SET [" + columnName + "]=? WHERE rowid=";
                using (var trans = _dbConnection.BeginTransaction())
                {
                    using (var cmd = _dbConnection.CreateCommand())
                    {
                        cmd.Transaction = trans;
                        cmd.Parameters.Add("columnName", OleDbType.Binary);
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            cmd.CommandText = commandText + _rowIdArray[i];
                            cmd.Parameters[0].Value = columnData[i];
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                }
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void EditDatabaseColumn(string columnName, short[] columnData)
            {
                if (columnData.Count() == 0)
                    return;
                int columnIndex = Array.IndexOf(_storedColumnNames, columnName);
                if (columnIndex < 0)
                    throw new Exception("The desired Field Name: " + columnName + " does not exist.");
                bool localConvertToColumnType() { var tmp3 = columnData; object argvalue3 = tmp3[0]; var ret = ConvertToColumnType(_storedColumnTypes[columnIndex], ref argvalue3); tmp3[0] = Convert.ToInt16(argvalue3); return ret; }

                if (localConvertToColumnType() == false)
                    throw new Exception("The desired Field: " + columnName + " is of type " + _storedColumnTypes[columnIndex].ToString() + "not of type '" + columnData[0].GetType().ToString() + "'.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("The table '" + _tableName + "' does not have: " + columnData.Count() + " records");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = _dbConnection.CreateCommand())
                    {
                        command.Transaction = tr;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]='" + columnData[i] + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void EditDatabaseColumn(string columnName, int[] columnData)
            {
                if (columnData.Count() == 0)
                    return;
                int columnIndex = Array.IndexOf(_storedColumnNames, columnName);
                if (columnIndex < 0)
                    throw new Exception("The desired Field Name: " + columnName + " does not exist.");
                bool localConvertToColumnType() { var tmp4 = columnData; object argvalue4 = tmp4[0]; var ret = ConvertToColumnType(_storedColumnTypes[columnIndex], ref argvalue4); tmp4[0] = Convert.ToInt32(argvalue4); return ret; }

                if (localConvertToColumnType() == false)
                    throw new Exception("The desired Field: " + columnName + " is of type " + _storedColumnTypes[columnIndex].ToString() + "not of type '" + columnData[0].GetType().ToString() + "'.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("The table '" + _tableName + "' does not have: " + columnData.Count() + " records");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = _dbConnection.CreateCommand())
                    {
                        command.Transaction = tr;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]='" + columnData[i] + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void EditDatabaseColumn(string columnName, long[] columnData)
            {
                if (columnData.Count() == 0)
                    return;
                int columnIndex = Array.IndexOf(_storedColumnNames, columnName);
                if (columnIndex < 0)
                    throw new Exception("The desired Field Name: " + columnName + " does not exist.");
                bool localConvertToColumnType() { var tmp5 = columnData; object argvalue5 = tmp5[0]; var ret = ConvertToColumnType(_storedColumnTypes[columnIndex], ref argvalue5); tmp5[0] = Convert.ToInt64(argvalue5); return ret; }

                if (localConvertToColumnType() == false)
                    throw new Exception("The desired Field: " + columnName + " is of type " + _storedColumnTypes[columnIndex].ToString() + "not of type '" + columnData[0].GetType().ToString() + "'.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("The table '" + _tableName + "' does not have: " + columnData.Count() + " records");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = _dbConnection.CreateCommand())
                    {
                        command.Transaction = tr;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]='" + columnData[i] + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void EditDatabaseColumn(string columnName, double[] columnData)
            {
                if (columnData.Count() == 0)
                    return;
                int columnIndex = Array.IndexOf(_storedColumnNames, columnName);
                if (columnIndex < 0)
                    throw new Exception("The desired Field Name: " + columnName + " does not exist.");
                bool localConvertToColumnType() { var tmp6 = columnData; object argvalue6 = tmp6[0]; var ret = ConvertToColumnType(_storedColumnTypes[columnIndex], ref argvalue6); tmp6[0] = Convert.ToDouble(argvalue6); return ret; }

                if (localConvertToColumnType() == false)
                    throw new Exception("The desired Field: " + columnName + " is of type " + _storedColumnTypes[columnIndex].ToString() + "not of type '" + columnData[0].GetType().ToString() + "'.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("The table '" + _tableName + "' does not have: " + columnData.Count() + " records");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = _dbConnection.CreateCommand())
                    {
                        command.Transaction = tr;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]='" + columnData[i] + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void EditDatabaseColumn(string columnName, float[] columnData)
            {
                if (columnData.Count() == 0)
                    return;
                int columnIndex = Array.IndexOf(_storedColumnNames, columnName);
                if (columnIndex < 0)
                    throw new Exception("The desired Field Name: " + columnName + " does not exist.");
                bool localConvertToColumnType() { var tmp7 = columnData; object argvalue7 = tmp7[0]; var ret = ConvertToColumnType(_storedColumnTypes[columnIndex], ref argvalue7); tmp7[0] = Convert.ToSingle(argvalue7); return ret; }

                if (localConvertToColumnType() == false)
                    throw new Exception("The desired Field: " + columnName + " is of type " + _storedColumnTypes[columnIndex].ToString() + "not of type '" + columnData[0].GetType().ToString() + "'.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("The table '" + _tableName + "' does not have: " + columnData.Count() + " records");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = _dbConnection.CreateCommand())
                    {
                        command.Transaction = tr;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]='" + columnData[i] + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void EditDatabaseColumn(string columnName, string[] columnData)
            {
                if (columnData.Count() == 0)
                    return;
                int columnIndex = Array.IndexOf(_storedColumnNames, columnName);
                if (columnIndex < 0)
                    throw new Exception("The desired Field Name: " + columnName + " does not exist.");
                bool localConvertToColumnType() { var tmp8 = columnData; object argvalue8 = tmp8[0]; var ret = ConvertToColumnType(_storedColumnTypes[columnIndex], ref argvalue8); tmp8[0] = Convert.ToString(argvalue8); return ret; }

                if (localConvertToColumnType() == false)
                    throw new Exception("The desired Field: " + columnName + " is of type " + _storedColumnTypes[columnIndex].ToString() + "not of type '" + columnData[0].GetType().ToString() + "'.");
                if (_rowIdArray.Count() != columnData.Count())
                    throw new Exception("The table '" + _tableName + "' does not have: " + columnData.Count() + " records");
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var command = _dbConnection.CreateCommand())
                    {
                        command.Transaction = tr;
                        for (int i = 0, loopTo = columnData.Count() - 1; i <= loopTo; i++)
                        {
                            command.CommandText = "UPDATE " + _tableName + " SET [" + columnName + "]='" + columnData[i] + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            command.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
            }

            protected override object[] GetStoredColumn(string storedColumnName)
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                var column = new object[(_rowIdArray.Count())];
                using (var cmd = new OleDbCommand("SELECT [" + storedColumnName + "] FROM [" + _tableName + "]", _dbConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            int counter = 0;
                            while (reader.Read())
                            {
                                column[counter] = reader[storedColumnName];
                                counter += 1;
                            }
                        }
                    }
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
                return column;
            }
            protected override object[] GetStoredColumn(int storedColumnIndex)
            {
                return GetStoredColumn(_storedColumnNames[storedColumnIndex]);
            }
            #endregion

            #region Row Stuff
            protected override void AddRowToDatabase(object[] row)
            {
                // Error Checking
                if (row.Count() != _storedColumnNames.Count())
                    throw new Exception("Number of columns that you are trying to add do not match the number of columns in the database.");
                for (int i = 0, loopTo = row.Count() - 1; i <= loopTo; i++)
                {
                    if (ConvertToColumnType(_storedColumnTypes[i], ref row[i]) == false)
                        throw new Exception("Column type '" + row[i].GetType().ToString() + "' does not match for column '" + _storedColumnNames[i] + "'.");
                }
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();

                var insertString = new System.Text.StringBuilder();
                insertString.Append("INSERT INTO [" + _tableName + "] (");
                insertString.Append(_storedColumnNames[0]);
                for (int i = 1, loopTo1 = _storedColumnNames.Count() - 1; i <= loopTo1; i++)
                    insertString.Append("," + _storedColumnNames[i]);
                insertString.Append(") VALUES (");
                insertString.Append("?");
                for (int i = 1, loopTo2 = _storedColumnNames.Count() - 1; i <= loopTo2; i++)
                    insertString.Append(",?");
                insertString.Append(")");
                // 
                using (var command = new OleDbCommand(insertString.ToString(), _dbConnection))
                {
                    for (int i = 0, loopTo3 = _storedColumnNames.Count() - 1; i <= loopTo3; i++)
                        command.Parameters.AddWithValue(_storedColumnNames[i], row[i]);
                    // 
                    command.ExecuteNonQuery();
                }
                // 
                // _parentDatabase.OnRowsAdded(_tablename, New List(Of Object())({row}))
                _rowIdArray = GetRowIdArray();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void AddRowToDatabase()
            {
                var row = new object[(_storedColumnNames.Count())];
                for (int i = 0, loopTo = _storedColumnNames.Count() - 1; i <= loopTo; i++)
                    row[i] = DBNull.Value;
                AddRowToDatabase(row);
            }

            protected override void AddRowsToDatabase(List<object[]> newRowData)
            {
                for (int i = 0, loopTo = newRowData.Count - 1; i <= loopTo; i++)
                {
                    if (newRowData[i].Count() != _storedColumnNames.Count())
                        throw new Exception("Number of columns does not match for row " + i.ToString() + ".");
                    for (int j = 0, loopTo1 = newRowData[i].Count() - 1; j <= loopTo1; j++)
                    {
                        var tmp = newRowData[i];
                        var argvalue = tmp[j];
                        if (ConvertToColumnType(_storedColumnTypes[j], ref argvalue) == false)
                            throw new Exception("Column type '" + newRowData[i][j].GetType().ToString() + "' does not match for column '" + _storedColumnNames[i] + "' in row " + i.ToString() + ".");
                    }
                }
                // 
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                var insertString = new System.Text.StringBuilder();
                insertString.Append("INSERT INTO [" + _tableName + "] (");
                insertString.Append(_storedColumnNames[0]);
                for (int i = 1, loopTo2 = _storedColumnNames.Count() - 1; i <= loopTo2; i++)
                    insertString.Append("," + _storedColumnNames[i]);
                insertString.Append(") VALUES (");
                insertString.Append("?");
                for (int i = 1, loopTo3 = _storedColumnNames.Count() - 1; i <= loopTo3; i++)
                    insertString.Append(",?");
                insertString.Append(")");
                // 
                using (var trans = _dbConnection.BeginTransaction())
                {
                    using (var command = new OleDbCommand(insertString.ToString(), _dbConnection, trans))
                    {
                        for (int i = 0, loopTo4 = _storedColumnNames.Count() - 1; i <= loopTo4; i++)
                            command.Parameters.Add(new OleDbParameter(_storedColumnNames[i], _storedColumnTypes[i]));
                        for (int i = 0, loopTo5 = newRowData.Count - 1; i <= loopTo5; i++)
                        {
                            for (int j = 0, loopTo6 = newRowData[i].Count() - 1; j <= loopTo6; j++)
                                command.Parameters[j].Value = newRowData[i][j];
                            command.ExecuteNonQuery();
                        }
                    }
                    // 
                    trans.Commit();
                }
                // 
                // _parentDatabase.OnRowsAdded(_tableName, newRowData)
                _rowIdArray = GetRowIdArray();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }

            protected override void DeleteRowFromDatabase(int rowIndex)
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                using (var command = new OleDbCommand("DELETE FROM [" + _tableName + "] WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndex], _dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                // 
                // _parentDatabase.OnRowsDeleted(_tableName, {rowIndex})
                _rowIdArray = GetRowIdArray();
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void DeleteRowsFromDatabase(int[] rowIndices)
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                using (var tr = _dbConnection.BeginTransaction())
                {
                    using (var cmd = _dbConnection.CreateCommand())
                    {
                        cmd.Transaction = tr;
                        for (int i = 0, loopTo = rowIndices.Count() - 1; i <= loopTo; i++)
                        {
                            cmd.CommandText = "DELETE FROM [" + _tableName + "] WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndices[i]];
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                // 
                // _parentDatabase.OnRowsDeleted(_tableName, rowIndices)
                _rowIdArray = GetRowIdArray();
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
            }

            protected override object[] GetStoredRow(int storedRowIndex)
            {
                var row = new object[(_storedColumnNames.Count())];
                using (var cmd = new OleDbCommand("SELECT * FROM [" + _tableName + "] WHERE " + _autoIncrementColumn + " = " + _rowIdArray[storedRowIndex], _dbConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                                reader.GetValues(row);
                        }
                    }
                }
                // 
                return row;
            }
            protected override object[] GetStoredRow(int storedRowIndex, string[] storedColumnNames)
            {
                string commandString = "SELECT [" + storedColumnNames[0] + "]";
                for (int i = 1, loopTo = storedColumnNames.Count() - 1; i <= loopTo; i++)
                    commandString = commandString + ",[" + storedColumnNames[i] + "]";
                commandString = commandString + " FROM [" + _tableName + "] WHERE " + _autoIncrementColumn + " = " + _rowIdArray[storedRowIndex];
                // 
                var row = new object[(storedColumnNames.Count())];
                using (var cmd = new OleDbCommand(commandString, _dbConnection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                                reader.GetValues(row);
                        }
                    }
                }
                // 
                return row;
            }
            protected override object[] GetStoredRow(int storedRowIndex, int[] storedColumnIndices)
            {
                var columns = new string[(storedColumnIndices.Count())];
                for (int i = 0, loopTo = storedColumnIndices.Count() - 1; i <= loopTo; i++)
                    columns[i] = _storedColumnNames[storedColumnIndices[i]];
                return GetStoredRow(storedRowIndex, columns);
            }

            protected override List<object[]> GetStoredRows(int startStoredRowIndex, int endStoredRowIndex)
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                var rows = new List<object[]>();
                var row = new object[1];
                using (var trans = _dbConnection.BeginTransaction())
                {
                    using (var cmd = _dbConnection.CreateCommand())
                    {
                        cmd.Transaction = trans;
                        for (int i = startStoredRowIndex, loopTo = endStoredRowIndex; i <= loopTo; i++)
                        {
                            cmd.CommandText = "SELECT * FROM [" + _tableName + "] WHERE " + _autoIncrementColumn + " = " + _rowIdArray[i];
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        row = new object[reader.FieldCount];
                                        reader.GetValues(row);
                                        rows.Add(row);
                                    }
                                }
                            }
                        }
                    }
                    trans.Commit();
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
                return rows;
            }
            #endregion

            #region Cell Stuff
            // Protected Overrides Sub EditDatabaseCell(columnIndex As Integer, rowIndex As Integer, cellValue As Object)
            // Select Case cellValue.GetType
            // Case GetType(Byte)
            // EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Byte))
            // Case GetType(Short)
            // EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Short))
            // Case GetType(Integer)
            // EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Int32))
            // Case GetType(Long)
            // EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Int64))
            // Case GetType(Single)
            // EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Single))
            // Case GetType(Double)
            // EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Double))
            // Case GetType(Boolean)
            // EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, Boolean))
            // Case GetType(String)
            // EditDatabaseCell(columnIndex, rowIndex, DirectCast(cellValue, String))
            // End Select
            // End Sub
            protected override void EditDatabaseCell(int columnIndex, int rowIndex, bool cellValue)
            {
                using (var cmd = new OleDbCommand("UPDATE [" + _tableName + "] SET [" + _storedColumnNames[columnIndex] + "]='" + cellValue + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndex], _dbConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            protected override void EditDatabaseCell(int columnIndex, int rowIndex, byte cellValue)
            {
                using (var cmd = new OleDbCommand("UPDATE [" + _tableName + "] SET [" + _storedColumnNames[columnIndex] + "]='" + cellValue + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndex], _dbConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            protected override void EditDatabaseCell(int columnIndex, int rowIndex, short cellValue)
            {
                using (var cmd = new OleDbCommand("UPDATE [" + _tableName + "] SET [" + _storedColumnNames[columnIndex] + "]='" + cellValue + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndex], _dbConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            protected override void EditDatabaseCell(int columnIndex, int rowIndex, int cellValue)
            {
                using (var cmd = new OleDbCommand("UPDATE [" + _tableName + "] SET [" + _storedColumnNames[columnIndex] + "]='" + cellValue + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndex], _dbConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            protected override void EditDatabaseCell(int columnIndex, int rowIndex, long cellValue)
            {
                using (var cmd = new OleDbCommand("UPDATE [" + _tableName + "] SET [" + _storedColumnNames[columnIndex] + "]='" + cellValue + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndex], _dbConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            protected override void EditDatabaseCell(int columnIndex, int rowIndex, double cellValue)
            {
                using (var cmd = new OleDbCommand("UPDATE [" + _tableName + "] SET [" + _storedColumnNames[columnIndex] + "]='" + cellValue + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndex], _dbConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            protected override void EditDatabaseCell(int columnIndex, int rowIndex, float cellValue)
            {
                using (var cmd = new OleDbCommand("UPDATE [" + _tableName + "] SET [" + _storedColumnNames[columnIndex] + "]='" + cellValue + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndex], _dbConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            protected override void EditDatabaseCell(int columnIndex, int rowIndex, string cellValue)
            {
                using (var cmd = new OleDbCommand("UPDATE [" + _tableName + "] SET [" + _storedColumnNames[columnIndex] + "]='" + cellValue + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndex], _dbConnection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            protected override void EditDatabaseCell(int columnIndex, int rowIndex, byte[] cellValue)
            {
                using (var cmd = new OleDbCommand("UPDATE [" + _tableName + "] SET [" + _storedColumnNames[columnIndex] + "]=? WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndex], _dbConnection))
                {
                    cmd.Parameters[0].Value = cellValue;
                    cmd.ExecuteNonQuery();
                }
            }

            /// <summary>
            /// <inheritdoc/>
            /// </summary>
            /// <param name="columnIndex"></param>
            /// <param name="rowIndex"></param>
            /// <param name="cellValue"></param>
            protected override void EditDatabaseCell(int columnIndex, int rowIndex, DateTime cellValue)
            {
                using (var cmd = new OleDbCommand("UPDATE [" + _tableName + "] SET [" + _storedColumnNames[columnIndex] + "]=? WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndex], _dbConnection))
                {
                    cmd.Parameters.Add(new OleDbParameter(_storedColumnNames[columnIndex], OleDbType.Date));
                    cmd.Parameters[0].Value = cellValue;
                    cmd.ExecuteNonQuery();
                }
            }

            protected override void EditDatabaseCells(string[] columnNamesToEdit, int[] rowIndices, object[] cellValues)
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var trans = _dbConnection.BeginTransaction())
                {
                    using (var cmd = _dbConnection.CreateCommand())
                    {
                        cmd.Transaction = trans;
                        for (int i = 0, loopTo = columnNamesToEdit.Count() - 1; i <= loopTo; i++)
                        {
                            cmd.CommandText = "UPDATE [" + _tableName + "] SET [" + columnNamesToEdit[i] + "]='" + cellValues[i].ToString() + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndices[i]];
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                }
                if (wasOpen == false)
                    _parentDatabase.Close();
            }
            protected override void EditDatabaseCells(int[] columnIndices, int[] rowIndices, object[] cellValues)
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                using (var trans = _dbConnection.BeginTransaction())
                {
                    using (var cmd = _dbConnection.CreateCommand())
                    {
                        cmd.Transaction = trans;
                        for (int i = 0, loopTo = columnIndices.Count() - 1; i <= loopTo; i++)
                        {
                            cmd.CommandText = "UPDATE [" + _tableName + "] SET [" + _storedColumnNames[columnIndices[i]] + "]='" + cellValues[i].ToString() + "' WHERE " + _autoIncrementColumn + " = " + _rowIdArray[rowIndices[i]];
                            cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                }
                if (wasOpen == false)
                    _parentDatabase.Close();
            }

            protected override object GetStoredCell(string storedColumnName, int storedRowIndex)
            {
                using (var command = new OleDbCommand("SELECT " + storedColumnName + " FROM " + _tableName + " WHERE " + _autoIncrementColumn + " = " + _rowIdArray[storedRowIndex], _dbConnection))
                {
                    return command.ExecuteScalar();
                }
            }
            protected override object GetStoredCell(int storedColumnIndex, int storedRowIndex)
            {
                using (var command = new OleDbCommand("SELECT " + _storedColumnNames[storedColumnIndex] + " FROM " + _tableName + " WHERE " + _autoIncrementColumn + " = " + _rowIdArray[storedRowIndex], _dbConnection))
                {
                    return command.ExecuteScalar();
                }
            }
            protected override object[] GetStoredCells(int[] storedColumnIndices, int[] storedRowIndices)
            {
                bool wasOpen = _parentDatabase.DataBaseOpen;
                if (_parentDatabase.DataBaseOpen == false)
                    _parentDatabase.Open();
                // 
                var result = new object[(storedColumnIndices.Count())];
                using (var trans = _dbConnection.BeginTransaction())
                {
                    using (var cmd = _dbConnection.CreateCommand())
                    {
                        cmd.Transaction = trans;
                        // 
                        for (int i = 0, loopTo = storedColumnIndices.Count() - 1; i <= loopTo; i++)
                        {
                            cmd.CommandText = "SELECT [" + _storedColumnNames[storedColumnIndices[i]] + "] FROM [" + _tableName + "] WHERE " + _autoIncrementColumn + " = " + _rowIdArray[storedRowIndices[i]];
                            result[i] = cmd.ExecuteScalar();
                        }
                    }
                    trans.Commit();
                }
                // 
                if (wasOpen == false)
                    _parentDatabase.Close();
                return result;
            }
            #endregion

            // Public Sub ExecuteCommandNonQuery(ByVal command As OleDbCommand)
            // Command.ExecuteNonQuery()
            // End Sub


        }

        /// <summary>
        /// Creates a new table in the MDB database with the specified schema.
        /// </summary>
        /// <param name="name">The name for the new table.</param>
        /// <param name="tableColumnNames">An array of column names for the table.</param>
        /// <param name="tableColumnTypes">An array of .NET types corresponding to each column.</param>
        /// <param name="autoIncrementColumn">Optional name for an auto-increment identity column.</param>
        /// <exception cref="Exception">Thrown when the table cannot be created.</exception>
        public void CreateTable(string name, string[] tableColumnNames, Type[] tableColumnTypes, string autoIncrementColumn = null)
        {
            bool leaveOpen = DataBaseOpen;
            if (DataBaseOpen == false)
                Open();
            var sb = new System.Text.StringBuilder(500);
            sb.Append("Create Table [").Append(name).Append("] (");
            // Dim HasAutofield As Int16 = 0
            if (!(autoIncrementColumn == null))
            {
                sb.Append("[").Append(autoIncrementColumn).Append("]").Append(" ");
                sb.Append("IDENTITY");
                sb.Append(",");
            }
            for (int i = 0, loopTo = tableColumnNames.Count() - 1; i <= loopTo; i++)
            {
                sb.Append("[").Append(tableColumnNames[i]).Append("]").Append(" ");
                switch (tableColumnTypes[i])
                {
                    case var @case when @case == typeof(string):
                        {
                            sb.Append("Text(50)");
                            break;
                        }
                    case var case1 when case1 == typeof(DateTime):
                        {
                            sb.Append("DATETIME");
                            break;
                        }
                    case var case2 when case2 == typeof(byte):
                    case var case3 when case3 == typeof(sbyte):
                        {
                            sb.Append("BYTE");
                            break;
                        }
                    case var case4 when case4 == typeof(short):
                    case var case5 when case5 == typeof(ushort):
                        {
                            sb.Append("SHORT");
                            break;
                        }
                    case var case6 when case6 == typeof(int):
                    case var case7 when case7 == typeof(uint):
                        {
                            sb.Append("INT");
                            break;
                        }
                    case var case8 when case8 == typeof(float):
                        {
                            sb.Append("SINGLE");
                            break;
                        }
                    case var case9 when case9 == typeof(double):
                        {
                            sb.Append("DOUBLE");
                            break;
                        }
                    case var case10 when case10 == typeof(decimal):
                        {
                            sb.Append("NUMBER");
                            break;
                        }
                    case var case11 when case11 == typeof(char):
                        {
                            sb.Append("Text(1)");
                            break;
                        }
                    case var case12 when case12 == typeof(bool):
                        {
                            sb.Append("LOGICAL");
                            break;
                        }
                    case var case13 when case13 == typeof(object):
                        {
                            sb.Append("LONGBINARY");
                            break;
                        }
                    case var case14 when case14 == typeof(byte[]):
                        {
                            sb.Append("LONGBINARY");
                            break;
                        }

                    default:
                        {
                            throw new Exception(tableColumnTypes[i].ToString() + " Not implemented, Column: " + tableColumnNames[i]);
                        }
                }
                sb.Append(",");
            }
            sb[sb.Length - 1] = ')';
            // 
            try
            {
                var createcmd = new OleDbCommand(sb.ToString(), _dbConnection);
                createcmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (leaveOpen == false)
                Close();
            _tableNames = GetTableNames();
        }

        /// <summary>
        /// Saves a <see cref="DataTable"/> to the MDB database as a new table.
        /// </summary>
        /// <param name="name">The name for the new table in the database.</param>
        /// <param name="dt">The <see cref="DataTable"/> containing the data to save.</param>
        /// <remarks>
        /// This method creates a new table with the schema derived from the DataTable's columns
        /// and inserts all rows from the DataTable into the new table.
        /// </remarks>
        public void SaveDataTableToDataBase(string name, DataTable dt)
        {
            bool reOpen = DataBaseOpen;
            if (DataBaseOpen == false)
                Open();
            var sb = new System.Text.StringBuilder(500);
            sb.Append("Create Table [").Append(name).Append("] (");
            // Dim HasAutofield As Int16 = 0
            foreach (DataColumn c in dt.Columns)
            {
                // If c.ColumnName = "OBJECTID" Then
                // Sb.Append("[").Append(c.ColumnName).Append("]").Append(" "c)
                // Sb.Append("IDENTITY")
                // Sb.Append(","c)
                // HasAutofield = 1
                // Else
                var tc = Type.GetTypeCode(c.DataType);
                sb.Append("[").Append(c.ColumnName).Append("]").Append(' ');
                switch (tc)
                {
                    case TypeCode.String:
                        {
                            if (c.MaxLength > 255 || c.MaxLength < 0)
                                sb.Append("Text(50)");
                            else
                                sb.AppendFormat("Text({0})", c.MaxLength);
                            break;
                        }
                    case TypeCode.DateTime:
                        {
                            sb.Append("DATETIME");
                            break;
                        }
                    case TypeCode.Int16:
                        {
                            sb.Append("SHORT");
                            break;
                        }
                    case TypeCode.UInt16:
                        {
                            sb.Append("SHORT");
                            break;
                        }
                    case TypeCode.Int32:
                        {
                            sb.Append("INT");
                            break;
                        }
                    case TypeCode.UInt32:
                        {
                            sb.Append("INT");
                            break;
                        }
                    case TypeCode.Single:
                        {
                            sb.Append("SINGLE");
                            break;
                        }
                    case TypeCode.Double:
                        {
                            sb.Append("DOUBLE");
                            break;
                        }
                    case TypeCode.Decimal:
                        {
                            sb.Append("NUMBER");
                            break;
                        }
                    case TypeCode.Char:
                        {
                            sb.Append("Text(1)");
                            break;
                        }
                    case TypeCode.Boolean:
                        {
                            sb.Append("LOGICAL");
                            break;
                        }
                    case TypeCode.Byte:
                        {
                            sb.Append("BYTE");
                            break;
                        }
                    case TypeCode.Object:
                        {
                            sb.Append("LONGBINARY");
                            break;
                        }

                    default:
                        {
                            if (ReferenceEquals(c.DataType, typeof(Guid)))
                            {
                                sb.Append("Text(50)");
                            }
                            else
                            {
                                throw new Exception(tc.ToString() + " Not implemented, Column: " + c.ColumnName);
                            }

                            break;
                        }
                }
                sb.Append(',');
                // End If
            }
            sb[sb.Length - 1] = ')';
            // 
            try
            {
                var createcmd = new OleDbCommand(sb.ToString(), _dbConnection);
                createcmd.ExecuteNonQuery();
                var accDataAdapter = new OleDbDataAdapter("SELECT * FROM " + name, _dbConnection);
                var accCommandBuilder = new OleDbCommandBuilder(accDataAdapter);
                accDataAdapter.InsertCommand = accCommandBuilder.GetInsertCommand();
                accDataAdapter.Update(dt);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            if (reOpen == false)
                Close();
            _tableNames = GetTableNames();
        }

        /// <summary>
        /// Renames an existing table in the MDB database.
        /// </summary>
        /// <param name="oldName">The current name of the table to rename.</param>
        /// <param name="newName">The new name for the table.</param>
        /// <remarks>
        /// This method uses ADOX to rename the table, which requires closing and reopening the connection.
        /// </remarks>
        public void RenameTable(string oldName, string newName)
        {
            bool reopen = _dataBaseOpen;
            if (_dataBaseOpen == true)
                Close();
            var cn = new ADODB.Connection();
            var catalog = new Catalog();
            int i;

            cn.ConnectionString = "Provider=Microsoft.Jet.OleDb.4.0;Data Source=" + _dataBasePath;
            cn.Open();

            catalog.ActiveConnection = cn;

            var tables = catalog.Tables;
            var loopTo = tables.Count - 1;
            for (i = 0; i <= loopTo; i++)
            {
                if ((tables[i].Name ?? "") == (oldName ?? ""))
                {
                    tables[i].Name = newName;
                    break;
                }
            }

            cn.Close();

            cn = null;
            tables = null;
            if (reopen == true)
                Open();
            _tableNames = GetTableNames();
        }



        // Public Sub CompactRepair()
        // Dim reopen As Boolean = _dataBaseOpen
        // If _dataBaseOpen = True Then Close()
        // Dim strAccessDatabasePath As String = _dataBasePath
        // Dim tempAccessDatabasePath As String = IO.Path.GetDirectoryName(_dataBasePath) & "\tmp1.mdb"
        // Dim lockedDbFileInfo As New IO.FileInfo(strAccessDatabasePath.Replace(".mdb", ".ldb"))
        // If lockedDbFileInfo.Exists Then
        // Throw New Exception("Database " & IO.Path.GetFileName(_dataBasePath) & " is currently in use.")
        // End If

        // Rename(strAccessDatabasePath, tempAccessDatabasePath)

        // The JRO engine only works currently on 32-bit systems. The reference to JRO has been removed as it was only used to compact the database. This decision shouldn't affect future releases too much
        // since support for access has reduced considerably in lieu on sqlite.
        // Dim jro As New JRO.JetEngine

        // jro.CompactDatabase("Provider=Microsoft.Jet.OLEDB.4.0;" & "Data Source=" & tempAccessDatabasePath,
        // "Provider=Microsoft.Jet.OLEDB.4.0;" & "Data Source=" & strAccessDatabasePath & ";Jet OLEDB:Engine Type=5")
        // IO.File.Delete(tempAccessDatabasePath)
        // If reopen = True Then Open()
        // End Sub
        // Private Function oleDbToNetTypeConverter(oleDbTypeNumber As Integer) As Type
        // Select Case oleDbTypeNumber
        // Case 0
        // Return GetType(Nullable)
        // Case 2
        // Return GetType(Int16)
        // Case 3
        // Return GetType(Int32)
        // Case 4
        // Return GetType([Single])
        // Case 5
        // Return GetType([Double])
        // Case 6
        // Return GetType([Decimal])
        // Case 7
        // Return GetType(DateTime)
        // Case 8
        // Return GetType([String])
        // Case 9
        // Return GetType([Object])
        // Case 10
        // Return GetType(Exception)
        // Case 11
        // Return GetType([Boolean])
        // Case 12
        // Return GetType([Object])
        // Case 13
        // Return GetType([Object])
        // Case 14
        // Return GetType([Decimal])
        // Case 16
        // Return GetType([SByte])
        // Case 17
        // Return GetType([Byte])
        // Case 18
        // Return GetType(UInt16)
        // Case 19
        // Return GetType(UInt32)
        // Case 20
        // Return GetType(Int64)
        // Case 21
        // Return GetType(UInt64)
        // Case 64
        // Return GetType(DateTime)
        // Case 72
        // Return GetType(Guid)
        // Case 128
        // Return GetType([Byte]())
        // Case 129
        // Return GetType([String])
        // Case 130
        // Return GetType([String])
        // Case 131
        // Return GetType([Decimal])
        // Case 133
        // Return GetType(DateTime)
        // Case 134
        // Return GetType(TimeSpan)
        // Case 135
        // Return GetType(DateTime)
        // Case 138
        // Return GetType([Object])
        // Case 139
        // Return GetType([Decimal])
        // Case 200
        // Return GetType([String])
        // Case 201
        // Return GetType([String])
        // Case 202
        // Return GetType([String])
        // Case 203
        // Return GetType([String])
        // Case 204
        // Return GetType([Byte]())
        // Case 205
        // Return GetType([Byte]())
        // End Select
        // Throw (New Exception("DataType Not Supported"))
        // End Function
    }
}