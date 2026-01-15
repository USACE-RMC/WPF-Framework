Imports System.Data
Imports DatabaseManager
Imports OxyPlot

Public Class AlphabeticColumnStats
    Public Shared ReadOnly DataProperty As DependencyProperty = DependencyProperty.Register(NameOf(Data), GetType(Object()), GetType(AlphabeticColumnStats), New UIPropertyMetadata(New Object() {}, AddressOf DataProperty_Callback))

    Private Shared Sub DataProperty_Callback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim thisControl = CType(d, AlphabeticColumnStats)
        '
        thisControl.Plot()
    End Sub

    Public Property Data As Object()
        Get
            Return CType(GetValue(DataProperty), Object())
        End Get
        Set(value As Object())
            SetValue(DataProperty, value)
        End Set
    End Property

    Public Shared ReadOnly DataNameProperty As DependencyProperty = DependencyProperty.Register(NameOf(DataName), GetType(String), GetType(AlphabeticColumnStats), New UIPropertyMetadata(""))

    Public Property DataName As String
        Get
            Return CType(GetValue(DataNameProperty), String)
        End Get
        Set(value As String)
            SetValue(DataNameProperty, value)
        End Set
    End Property

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub AlphabeticColumnStats_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded

    End Sub
    Private Function DefaultDataTable() As DataTable
        Dim statsDataTable As New DataTable("StatsDataTable")
        statsDataTable.Columns.Add(New DataColumn("Unique Value", GetType(String)))
        statsDataTable.Columns.Add(New DataColumn("Count", GetType(Integer)))
        statsDataTable.Rows.Add({"< Total Count >", CInt(0)})
        Return statsDataTable
    End Function
    Private Sub Plot()
        If Data Is Nothing OrElse Data.Count = 0 Then
            'StatsTable.DataView.DeleteRows(1, StatsTable.DataView.NumberOfRows - 1)
        End If

        Dim dict As New Dictionary(Of String, Integer)
        Dim emptyCount As Integer = 0
        Dim tempString As String
        For i As Int32 = 0 To Data.Count - 1 'this could take a really long time.  not sure if there is a better way
            If IsDBNull(Data(i)) OrElse String.IsNullOrEmpty(Data(i)) Then
                emptyCount += 1
            Else
                tempString = CStr(Data(i))
                If dict.ContainsKey(tempString) Then
                    dict(tempString) += 1
                Else
                    dict.Add(tempString, 1)
                End If
            End If
        Next

        'Sorty (leaving it) the dictionary by values
        Dim sortedList As List(Of KeyValuePair(Of String, Integer)) = dict.ToList
        If emptyCount > 0 Then sortedList.Add(New KeyValuePair(Of String, Integer)("", emptyCount))
        sortedList.Sort(Function(pair1, pair2) pair2.Value.CompareTo(pair1.Value))
        '
        'create a pie chart?
        CType(PieSeries.InternalSeries, Series.PieSeries).Slices.Clear()
        If sortedList.Count < 4 Then
            For Each uniquevalue As KeyValuePair(Of String, Int32) In sortedList
                CType(PieSeries.InternalSeries, Series.PieSeries).Slices.Add(New Series.PieSlice(uniquevalue.Key, uniquevalue.Value / Data.Count))
            Next
        Else
            Dim sumSoFar As Double
            For i As Int32 = 0 To 3
                CType(PieSeries.InternalSeries, Series.PieSeries).Slices.Add(New OxyPlot.Series.PieSlice(sortedList(i).Key, sortedList(i).Value / Data.Count))
                sumSoFar += sortedList(i).Value / Data.Count
            Next
            CType(PieSeries.InternalSeries, Series.PieSeries).Slices.Add(New OxyPlot.Series.PieSlice("All Other", (1 - sumSoFar)))
        End If
        '
        Dim statsDataTable As New DataTable("StatsDataTable")
        statsDataTable.Columns.Add(New DataColumn("Unique Value", GetType(String)))
        statsDataTable.Columns.Add(New DataColumn("Count", GetType(Integer)))
        statsDataTable.Rows.Add({"< Total Count >", Data.Count})
        'Dim rowsToAdd As New List(Of Object())
        For Each uniquevalue As KeyValuePair(Of String, Int32) In sortedList
            'rowsToAdd.Add({uniquevalue.Key, uniquevalue.Value})
            statsDataTable.Rows.Add({uniquevalue.Key, uniquevalue.Value})
        Next
        '
        Dim statsDataView As New InMemoryReader(statsDataTable)
        StatsTable.DataView = statsDataView.GetTableManager(statsDataTable.TableName)
        StatsTable.ResizeColumnWidth(0)
        'StatsTable.ResizeColumnWidth(1)
        StatsTable.UpdateVisibleRows()
        'StatsTable.DataView.AddRows(rowsToAdd)
        '
        StatsPlot.InvalidatePlot(True)
    End Sub

    Private Sub PlotToolbar_PropertiesCalled(targetPlot As Wpf.Plot, openProperties As Boolean, propertyExpander As OxyplotControls.OxyplotPropertiesControl.PropertyEXP, selectedObject As Object)
        If openProperties = False Then
            If PropertiesControl.Visibility = Visibility.Visible Then PropertiesControl.ExpandProperty(propertyExpander, selectedObject)
            Exit Sub
        End If
        '
        If selectedObject Is Nothing Then
            If PropertiesControl.Visibility = Visibility.Collapsed Then
                PropertiesControl.Visibility = Visibility.Visible
                PropertiesControl.ExpandProperty(propertyExpander)
            Else
                PropertiesControl.Visibility = Visibility.Collapsed
            End If
        Else
            PropertiesControl.Visibility = Visibility.Visible
            PropertiesControl.ExpandProperty(propertyExpander, selectedObject)
        End If
    End Sub

    Private Sub PropertiesControl_ClosePropertiesCalled(propertiesControl As OxyplotControls.OxyplotPropertiesControl)
        propertiesControl.Visibility = Visibility.Collapsed
    End Sub

End Class
