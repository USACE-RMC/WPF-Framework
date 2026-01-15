Imports DatabaseManager
Imports Microsoft.Win32
Imports Numerics.Mathematics.SpecialFunctions

'Imports System.Data.SQLite
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Class MainWindow
    Private _TheDataTableReader As DatabaseManager.DatabaseManager
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'Dim Err = 0
        'Dim Dub = 1 << 0
        'Dim Sng = 1 << 1
        'Dim Deciml = Dub + Sng
        'Dim Shrt = 1 << 3
        'Dim Int = 1 << 4
        'Dim byt = 1 << 5
        'Dim integral = Shrt + Int + byt
        'Dim Num = Dub + Sng + Shrt + Int + byt
        'Dim Str = 1 << 6
        'Dim Bool = 1 << 7
        'Dim UnDeclared = 1 << 8
        'Dim Valid = Dub + Sng + Shrt + Int + byt + Str + Bool + UnDeclared
        'Debug.Print(Valid)


        'Dim cellValue As String = "*******************"
        'Dim test As Double
        'If Double.TryParse(cellValue, test) = False Then
        '    Debug.Print("Good")
        'Else
        '    Debug.Print("Why!!!!")
        'End If



    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Inputfile As String = FileOpenDialog("Database file (*.dbf, *.mdb, *.sqlite, *.csv)|*.dbf;*.mdb;*.sqlite;*.csv")
        If Inputfile = "" Then Exit Sub
        If Path.GetExtension(Inputfile).ToLower = ".mdb" Then
            Dim TableReader As New MdbReader(Inputfile)
            'Pick a table to view
            ComboBox1.IsEnabled = True
            ComboBox1.Items.Clear()
            Dim TableNames() As String = TableReader.GetTableNames
            For i As Int32 = 0 To TableNames.Count - 1
                ComboBox1.Items.Add(TableNames(i))
            Next
            _TheDataTableReader = TableReader
        ElseIf Path.GetExtension(Inputfile).ToLower = ".dbf" Then
            _TheDataTableReader = New DbfReader(Inputfile)
            ComboBox1.Items.Clear()
            ComboBox1.Items.Add(Path.GetFileNameWithoutExtension(Inputfile))
            ComboBox1.IsEnabled = False
            ComboBox1.SelectedIndex = 0
        ElseIf Path.GetExtension(Inputfile).ToLower = ".sqlite" Then
            Dim TableReader As New SQLiteManager(Inputfile)
            'Pick a table to view
            ComboBox1.IsEnabled = True
            ComboBox1.Items.Clear()
            Dim TableNames() As String = TableReader.GetTableNames
            For i As Int32 = 0 To TableNames.Count - 1
                ComboBox1.Items.Add(TableNames(i))
            Next
            _TheDataTableReader = TableReader
        ElseIf Path.GetExtension(Inputfile).ToLower = ".csv" Then
            _TheDataTableReader = New CsvReader(Inputfile, True, 1, False)
            ComboBox1.Items.Clear()
            ComboBox1.Items.Add(Path.GetFileNameWithoutExtension(Inputfile))
            ComboBox1.IsEnabled = False
            ComboBox1.SelectedIndex = 0
        End If

    End Sub

    Private Sub ComboBox1_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs)
        If IsLoaded = True Then
            If ComboBox1.SelectedIndex = -1 Then Exit Sub
            Dim theView As DataTableView = _TheDataTableReader.GetTableManager(CStr(ComboBox1.SelectedValue))
            TestViewer.DataView = theView
        End If
    End Sub
    Public Function FileOpenDialog(ByVal filters As String) As String
        Dim OpenfileDialog As New OpenFileDialog
        OpenfileDialog.Filter = filters
        If OpenfileDialog.ShowDialog() = True Then
            FileOpenDialog = OpenfileDialog.FileName.ToString
        Else
            FileOpenDialog = ""
        End If
    End Function
    Private Sub MainWindow_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        'Dim Timer As New Stopwatch
        'Timer.Start()
        'Try
        '    DataTableView.ConvertCSVToSQLite("C:\Temp\delete\CSVToSQLiteConverter\Faf3_4in.csv", "C:\Temp\delete\CSVToSQLiteConverter\Test.sqlite", "Faf3_4in", True, 1)
        'Catch ex As Exception
        '    Debug.Print(ex.Message)
        'End Try
        'Timer.Stop()
        'Debug.Print(Timer.Elapsed.TotalMilliseconds.ToString)

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ''Get dbf reader.
        'Dim dbtFile As String = "C:\Projects\TotalRisk\FDA\Muncie\1.4.3\Muncie\SdmgData.dbt"
        'Dim dbr As New DbfReader("C:\Projects\TotalRisk\FDA\Muncie\1.4.3\Muncie\SdmgData.dbf")
        'Dim view As DataTableView = dbr.GetTableManager(dbr.TableNames(0))
        'Dim ids() As String = Array.ConvertAll(view.GetColumn("ID_STGDAMG"), Function(o) o.ToString)
        'Dim names() As String = Array.ConvertAll(view.GetColumn("NM_STGDAMG"), Function(o) o.ToString) 'name
        'Dim pointers() As String = Array.ConvertAll(view.GetColumn("FU_STRUC"), Function(o) o.ToString)

        'Dim doubleResults = New Dictionary(Of Integer, Double())
        ''Using fs As New FileStream(dbtFile, FileMode.Open, FileAccess.Read)
        ''    Using dbtBR As New BinaryReader(fs)
        ''        '
        ''        'Dim occTypeNames() As String = Array.ConvertAll(view.GetColumn("NM_OCCTYPE"), Function(o) o.ToString) 'name
        ''        Dim descriptions() As String = GetMemoData(dbtBR, pointers)
        ''        For Each d In descriptions

        ''        Next
        ''    End Using
        ''End Using

        'Dim results = ReadDBT(dbtFile, doubleResults)
        'For Each r As KeyValuePair(Of Integer, String) In results
        '    Dim idx = Array.IndexOf(pointers, r.Key.ToString())
        '    If idx = -1 Then Continue For
        '    Debug.Print(names(idx))
        '    Dim strL = r.Value.Trim().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        '    For Each l In strL
        '        Debug.Print(String.Join("\t", l.Split("\t".ToCharArray())))
        '    Next
        'Next





        'Dim sourceFile As String = "C:\Temp\delete\LST_Example_Project\Range Wash Fan\LifeSim\Results\Sensitivity.gpkg"
        'Dim scenarios As String() = New String() {"LST-1.0_MidN_MidBV"} '{"1.0", "0.5", "0.75", "1.0", "1", "1_NB", "2", "2_NB", "3", "3_NB"} '


        'Dim sqReader As New SQLiteManager(sourceFile)
        'sqReader.Open()
        ''
        'Dim simName As String = Path.GetFileNameWithoutExtension(sourceFile)
        ''Dim llU65() As Double
        ''Dim llO65() As Double
        ''Dim llCaught() As Double
        ''Dim llTotal() As Double
        'Dim stats() As Double
        'Dim sDamage(), cDamage(), vDamage() As Double
        'Dim totalDamage() As Double
        ''
        'Dim tablePath As String
        'Debug.Print("Scenario, Min, 25th, 50th, 75th, Max")
        'For Each tod In {"2", "14"}
        '    For i As Int32 = 0 To scenarios.Count - 1

        '        tablePath = $"{simName}>Results_By_Iteration>{scenarios(i)}>{tod}>Leveed Area>Leveed Area"
        '        'tablePath = $"West>EPZ_Samples_By_Iteration>{scenarios(i)}>{tod}>Leveed Area"
        '        Dim reader = sqReader.GetTableManager(tablePath)
        '        'llTotal = Array.ConvertAll(reader.GetColumn("Relative_Warning_Issuance"), Function(o) If(IsDBNull(o), 0, CDbl(o)))
        '        'llTotal = Array.ConvertAll(reader.GetColumn("PAR_Warned"), Function(o) If(IsDBNull(o), 0, CDbl(o)))
        '        'llTotal = Array.ConvertAll(reader.GetColumn("PAR_Mobilized"), Function(o) If(IsDBNull(o), 0, CDbl(o)))

        '        'llU65 = Array.ConvertAll(reader.GetColumn("Population_At_RiskU65"), Function(o) If(IsDBNull(o), 0, CDbl(o)))
        '        'llO65 = Array.ConvertAll(reader.GetColumn("Population_At_RiskO65"), Function(o) If(IsDBNull(o), 0, CDbl(o)))
        '        'llU65 = Array.ConvertAll(reader.GetColumn("Threatened_PopulationU65"), Function(o) If(IsDBNull(o), 0, CDbl(o)))
        '        'llO65 = Array.ConvertAll(reader.GetColumn("Threatened_PopulationO65"), Function(o) If(IsDBNull(o), 0, CDbl(o)))
        '        'llU65 = Array.ConvertAll(reader.GetColumn("LL_In_StructuresU65"), Function(o) If(IsDBNull(o), 0, CDbl(o)))
        '        'llO65 = Array.ConvertAll(reader.GetColumn("LL_In_StructuresO65"), Function(o) If(IsDBNull(o), 0, CDbl(o)))
        '        '
        '        'ReDim llTotal(llU65.Count - 1)
        '        'For j As Int32 = 0 To llU65.Count - 1
        '        '    llTotal(j) = llU65(j) + llO65(j)
        '        'Next

        '        'Damages
        '        sDamage = Array.ConvertAll(reader.GetColumn("Structure_Damage"), Function(o) If(IsDBNull(o), 0, CDbl(o)))
        '        cDamage = Array.ConvertAll(reader.GetColumn("Content_Damage"), Function(o) If(IsDBNull(o), 0, CDbl(o)))
        '        vDamage = Array.ConvertAll(reader.GetColumn("Vehicle_Damage"), Function(o) If(IsDBNull(o), 0, CDbl(o)))

        '        '
        '        ReDim totalDamage(sDamage.Count - 1)
        '        For j As Int32 = 0 To sDamage.Count - 1
        '            totalDamage(j) = sDamage(j) + cDamage(j) + vDamage(j)
        '        Next

        '        stats = Numerics.Data.Statistics.Statistics.FiveNumberSummary(totalDamage)

        '        Debug.Print($"{scenarios(i)}, {String.Join(", ", stats)}")
        '    Next
        '    Debug.Print("............................................................................................")
        'Next

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'Dim x As MDBReader
        'Dim Results As New List(Of Double())
        'Dim Data1 As List(Of Double)
        'Dim Data2 As List(Of Double)
        'Dim Data3 As List(Of Double)
        'Dim Data4 As List(Of Double)
        'For i As Int32 = 0 To scenarios.Count - 1
        'If i = scenarios.Count - 3 Then Continue For

        'mdbPath = "C:\Temp\LifeSim\WND\LifeSim Output Results\Current_Conditions\" & scenarios(i) & " Simulation\" & scenarios(i) & " - Alternative\2\Results_By_EPZ.mdb"
        'x = New MdbReader(mdbPath)
        'x.DBConnection.Open()
        'Using cmd As New System.Data.OleDb.OleDbCommand("SELECT Population_At_Risk, Threatened_Population, LOL_In_Structures, LOL_Caught FROM LA_North_I5_Results", x.DBConnection)
        '    Using reader As System.Data.OleDb.OleDbDataReader = cmd.ExecuteReader
        '        If reader.HasRows Then
        '            Data1 = New List(Of Double)
        '            Data2 = New List(Of Double)
        '            Data3 = New List(Of Double)
        '            Data4 = New List(Of Double)
        '            While reader.Read
        '                Data1.Add(CDbl(reader.Item(0))) 'CDbl(reader.Item(0)) + CDbl(reader.Item(0)) * 0.0899) 'CDbl(reader.Item(0)))
        '                Data2.Add(CDbl(reader.Item(1)))
        '                Data3.Add(CDbl(reader.Item(2)))
        '                Data4.Add(CDbl(reader.Item(3)))
        '            End While
        '            Dim bps1 As New Statistics.ProductMomentsStats(Data1.ToArray)
        '            Dim bps2 As New Statistics.ProductMomentsStats(Data2.ToArray)
        '            Dim bps3 As New Statistics.ProductMomentsStats(Data3.ToArray)
        '            Dim bps4 As New Statistics.ProductMomentsStats(Data4.ToArray)
        '            'Dim em As New HEC_Statistics.Emperical(Data1)
        '            Results.Add(New Double() {bps1.GetMean, bps2.GetMean, bps3.GetMean, bps4.GetMean}) 'em.GetPercentile(0.05), em.GetPercentile(0.95)})
        '        End If
        '    End Using
        'End Using
        '    '
        '    Using cmd As New System.Data.OleDb.OleDbCommand("SELECT Population_At_Risk, Threatened_Population, LOL_In_Structures, LOL_Caught FROM LA_South_I5_Results", x.DBConnection)
        '        Using reader As System.Data.OleDb.OleDbDataReader = cmd.ExecuteReader
        '            If reader.HasRows Then
        '                Data1 = New List(Of Double)
        '                Data2 = New List(Of Double)
        '                Data3 = New List(Of Double)
        '                Data4 = New List(Of Double)
        '                While reader.Read
        '                    Data1.Add(CDbl(reader.Item(0))) 'CDbl(reader.Item(0)) + CDbl(reader.Item(0)) * 0.0899) 'CDbl(reader.Item(0)))
        '                    Data2.Add(CDbl(reader.Item(1)))
        '                    Data3.Add(CDbl(reader.Item(2)))
        '                    Data4.Add(CDbl(reader.Item(3)))
        '                End While
        '                Dim bps1 As New Statistics.ProductMomentsStats(Data1.ToArray)
        '                Dim bps2 As New Statistics.ProductMomentsStats(Data2.ToArray)
        '                Dim bps3 As New Statistics.ProductMomentsStats(Data3.ToArray)
        '                Dim bps4 As New Statistics.ProductMomentsStats(Data4.ToArray)
        '                'Dim em As New HEC_Statistics.Emperical(Data1)
        '                Results.Add(New Double() {bps1.GetMean, bps2.GetMean, bps3.GetMean, bps4.GetMean}) 'em.GetPercentile(0.05), em.GetPercentile(0.95)})
        '            End If
        '        End Using
        '    End Using
        '    '
        '    Using cmd As New System.Data.OleDb.OleDbCommand("SELECT Population_At_Risk, Threatened_Population, LOL_In_Structures, LOL_Caught FROM Orange_CA_Results", x.DBConnection)
        '        Using reader As System.Data.OleDb.OleDbDataReader = cmd.ExecuteReader
        '            If reader.HasRows Then
        '                Data1 = New List(Of Double)
        '                Data2 = New List(Of Double)
        '                Data3 = New List(Of Double)
        '                Data4 = New List(Of Double)
        '                While reader.Read
        '                    Data1.Add(CDbl(reader.Item(0))) 'CDbl(reader.Item(0)) + CDbl(reader.Item(0)) * 0.0899) 'CDbl(reader.Item(0)))
        '                    Data2.Add(CDbl(reader.Item(1)))
        '                    Data3.Add(CDbl(reader.Item(2)))
        '                    Data4.Add(CDbl(reader.Item(3)))
        '                End While
        '                Dim bps1 As New Statistics.ProductMomentsStats(Data1.ToArray)
        '                Dim bps2 As New Statistics.ProductMomentsStats(Data2.ToArray)
        '                Dim bps3 As New Statistics.ProductMomentsStats(Data3.ToArray)
        '                Dim bps4 As New Statistics.ProductMomentsStats(Data4.ToArray)
        '                'Dim em As New HEC_Statistics.Emperical(Data1)
        '                Results.Add(New Double() {bps1.GetMean, bps2.GetMean, bps3.GetMean, bps4.GetMean}) 'em.GetPercentile(0.05), em.GetPercentile(0.95)})
        '            End If
        '        End Using
        '    End Using
        '    x.DBConnection.Close()
        'Next

        'For i As Int32 = 0 To Results.Count - 1 Step 3
        '        Debug.Print(Results(i)(0) & "," & Results(i)(1) & "," & Results(i)(2) & "," & Results(i)(3) & "," & Results(i + 1)(0) & "," & Results(i + 1)(1) & "," & Results(i + 1)(2) & "," & Results(i + 1)(3) & "," & Results(i + 2)(0) & "," & Results(i + 2)(1) & "," & Results(i + 2)(2) & "," & Results(i + 2)(3))
        '    Next



        'Dim types(3) As Type
        'types(0) = GetType(Int32)
        'types(1) = GetType(Double)
        'types(2) = GetType(String)
        'types(3) = GetType(Object)

        ''Dim IntType As Integer = 521
        ''Dim DoubleType As Double = 5313.54654
        'Dim StrType As String = "AHAHAH"
        'Dim BLOB As Object = New Byte() {1, 2, 3, 4, 1, 12, 2, 5, 6, 2, 3, 4, 3, 5, 2, 2, 52, 12, 6, 46, 74, 12, 121, 16}

        'If File.Exists("C:\Temp\delete\Test.sqlite") Then Kill("C:\Temp\delete\Test.sqlite")
        'Dim x As New SQLiteReader("C:\Temp\delete\Test.sqlite")
        'x.DBConnection.Open()
        'x.CreateTable("TestTable", New String() {"A", "B", "C", "D"}, types)

        'Dim Parameters As New List(Of System.Data.SQLite.SQLiteParameter)
        'Parameters.Add(New System.Data.SQLite.SQLiteParameter("A"))
        'Parameters.Add(New System.Data.SQLite.SQLiteParameter("B"))
        'Parameters.Add(New System.Data.SQLite.SQLiteParameter("C"))
        'Parameters.Add(New System.Data.SQLite.SQLiteParameter("D"))

        'Dim timer As New Stopwatch
        'timer.Start()

        'Using tr As System.Data.SQLite.SQLiteTransaction = x.DBConnection.BeginTransaction
        '    Using cmd As System.Data.SQLite.SQLiteCommand = x.DBConnection.CreateCommand
        '        cmd.Transaction = tr
        '        'Create Insert string
        '        Dim InsertText As String = "INSERT INTO TestTable (A,B,C,D) VALUES (" '?,?,?,?)" '"INSERT INTO " & DT.TableName & " VALUES ("
        '        For i As Int32 = 0 To Parameters.Count - 1
        '            InsertText = InsertText & "@" & Parameters(i).ParameterName & ","
        '            cmd.Parameters.Add(Parameters(i))
        '        Next
        '        InsertText = InsertText.Substring(0, InsertText.Length - 1) & ")"
        '        cmd.CommandText = InsertText

        '        For i As Int32 = 0 To 100000
        '            Parameters(0).Value = i
        '            Parameters(1).Value = i * 1.54637 * i
        '            Parameters(2).Value = StrType & "_" & i
        '            Parameters(3).Value = BLOB
        '            cmd.ExecuteNonQuery()
        '        Next

        '    End Using
        '    tr.Commit()
        'End Using
        'x.DBConnection.Close()

        'timer.Stop()
        'Debug.Print(timer.Elapsed.TotalMilliseconds)


        ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ''mdb testing
        ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        'If File.Exists("C:\Temp\delete\Test.mdb") Then Kill("C:\Temp\delete\Test.mdb")
        'Dim x As New MDBReader("C:\Temp\delete\Test.mdb")

        'x.CreateTable("TestTable", New String() {"A", "B", "C", "D"}, types)

        'If x.DataBaseOpen = False Then x.Open()

        'Dim Parameters As New List(Of System.Data.OleDb.OleDbParameter)
        'Parameters.Add(New System.Data.OleDb.OleDbParameter("A", System.Data.OleDb.OleDbType.Integer))
        'Parameters.Add(New System.Data.OleDb.OleDbParameter("B", System.Data.OleDb.OleDbType.Double))
        'Parameters.Add(New System.Data.OleDb.OleDbParameter("C", System.Data.OleDb.OleDbType.Char))
        'Parameters.Add(New System.Data.OleDb.OleDbParameter("D", System.Data.OleDb.OleDbType.Binary))

        'Dim timer As New Stopwatch
        'timer.Start()

        'Using tr As System.Data.OleDb.OleDbTransaction = x.DBConnection.BeginTransaction
        '    Using cmd As System.Data.OleDb.OleDbCommand = x.DBConnection.CreateCommand
        '        cmd.Transaction = tr

        '        cmd.Parameters.Add(Parameters(0))
        '        cmd.Parameters.Add(Parameters(1))
        '        cmd.Parameters.Add(Parameters(2))
        '        cmd.Parameters.Add(Parameters(3))
        '        cmd.CommandText = "INSERT INTO TestTable (A,B,C,D) Values (?,?,?,?)"

        '        For i As Int32 = 0 To 100000
        '            Parameters(0).Value = IntType
        '            Parameters(1).Value = DoubleType
        '            Parameters(2).Value = StrType
        '            Parameters(3).Value = BLOB
        '            cmd.ExecuteNonQuery()
        '        Next

        '    End Using
        '    tr.Commit()
        'End Using

        'x.Close()

        'Timer.Stop()
        'Debug.Print(Timer.Elapsed.TotalMilliseconds)





        '    Dim x As New dbfView("C:\Temp\LifeSim\WND\LifeSim Analysis\Structure_Inventory\WND_NSI.dbf") '"C:\Temp\LifeSim\WND\Input Data\NSI_NAD83_Albers.dbf")
        '    Dim RowStart As Int32 = 10
        '    Dim RowEnd As Int32 = 1000
        '    Dim CellValue As Object
        '    Dim timer As New Stopwatch
        '    timer.Start()
        '    For i As Int32 = RowStart To RowEnd
        '        For j As Int32 = 0 To x.NumberOfColumns - 1
        '            CellValue = x.GetStoredCell(j, i)
        '        Next
        '    Next
        '    timer.Stop()
        '    Debug.Print("Get Cells Safe:" & timer.Elapsed.TotalMilliseconds)

        '    timer = New Stopwatch
        '    timer.Start()
        '    For i As Int32 = RowStart To RowEnd
        '        For j As Int32 = 0 To x.NumberOfColumns - 1
        '            CellValue = x.ReadRawCellUnsafe(j, i)
        '        Next
        '    Next
        '    timer.Stop()
        '    Debug.Print("Get Raw Cells Unsafe:" & timer.Elapsed.TotalMilliseconds)

        '    Dim Rows As New List(Of Object())(RowEnd - RowStart + 1)
        '    timer = New Stopwatch
        '    timer.Start()
        '    For i As Int32 = RowStart To RowEnd
        '        Rows.Add(x.GetStoredRow(i))
        '    Next
        '    timer.Stop()
        '    Debug.Print("Get Rows One at a time:" & timer.Elapsed.TotalMilliseconds)

        '    Rows = New List(Of Object())(RowEnd - RowStart + 1)
        '    timer = New Stopwatch
        '    timer.Start()
        '    Rows = x.GetStoredRows(RowStart, RowEnd)
        '    timer.Stop()
        '    Debug.Print("Get Rows as Group:" & timer.Elapsed.TotalMilliseconds)

        '    'Dim block() As String
        '    'timer = New Stopwatch
        '    'timer.Start()
        '    'block = x.readblockstrings(RowStart, RowEnd)
        '    'timer.Stop()
        '    'Debug.Print("readblockstrings:" & timer.Elapsed.TotalMilliseconds)
        '    ''
        '    'Rows = New List(Of Object())(RowEnd - RowStart + 1)
        '    'timer = New Stopwatch
        '    'timer.Start()
        '    'Rows = x.readblockobjects(RowStart, RowEnd)
        '    'timer.Stop()
        '    'Debug.Print("readblockobjects:" & timer.Elapsed.TotalMilliseconds)

        '    Me.Close()
    End Sub

    Private Sub ExportMenuItem_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Filters As String = "comma delimited(*.csv) |*.csv|database(*.dbf) |*.dbf|Excel(*.xlsx) |*.xlsx|Sqlite(*.sqlite) |*.sqlite"
        Dim SaveFileBrowser As New SaveFileDialog With {.Filter = Filters}
        If SaveFileBrowser.ShowDialog = True Then
            'FileSaveDialog = SaveFileBrowser.FileName.ToString
            Select Case System.IO.Path.GetExtension(SaveFileBrowser.FileName.ToString)
                Case ".csv"
                    TestViewer.DataView.ExportToCsv(SaveFileBrowser.FileName.ToString)
                Case ".dbf"
                    TestViewer.DataView.ExportToDbf(SaveFileBrowser.FileName.ToString)
                Case ".xls", ".xlsx"
                    TestViewer.DataView.ExportToXlsx(SaveFileBrowser.FileName.ToString)
                Case ".sqlite"
                    TestViewer.DataView.ExportToSqlite(SaveFileBrowser.FileName.ToString, TestViewer.DataView.TableName)
            End Select
        Else
            'FileSaveDialog = ""
        End If

    End Sub

    Private Sub TestItemForDebug_Click(sender As Object, e As RoutedEventArgs)
        'TestViewer.DataView.AddColumn("GDSF", GetType(Double))
        'Dim sw As New Stopwatch
        'sw.Start()

        'For i As Int32 = 0 To TestViewer.DataView.NumberOfRows - 1
        '    TestViewer.DataView.GetRow(i)
        'Next
        'sw.Stop()
        'Debug.Print(sw.Elapsed.TotalMilliseconds)
    End Sub

End Class
