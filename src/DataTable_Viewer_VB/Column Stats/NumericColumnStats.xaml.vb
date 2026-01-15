Imports System.Collections.ObjectModel
Imports System.Data
Imports DatabaseManager
Imports Numerics.Data.Statistics.Statistics
Imports OxyPlot

Public Class NumericColumnStats

    Public Shared ReadOnly DataProperty As DependencyProperty = DependencyProperty.Register(NameOf(Data), GetType(Double()), GetType(NumericColumnStats), New UIPropertyMetadata(New Double() {}, AddressOf DataProperty_Callback))

    Private Shared Sub DataProperty_Callback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim thisControl = CType(d, NumericColumnStats)
        '
        Dim newValue() As Double = TryCast(e.NewValue, Double())
        If newValue Is Nothing Then
            thisControl._sortedData = New Double() {}
        Else
            thisControl._sortedData = newValue.Where(Function(o) Double.IsNaN(o) = False AndAlso Double.IsInfinity(o) = False).ToArray()
            Array.Sort(thisControl._sortedData)
        End If
        '
        thisControl.DefaultR = Math.Pow(thisControl._sortedData(thisControl._sortedData.Length - 1) / thisControl._sortedData.Average(), (1.0 / (3 - 1)))
        Dim defaultInterval As Double = (thisControl._sortedData(thisControl._sortedData.Length - 1) - thisControl._sortedData(0)) / 5 '5 equally sized bins
        If thisControl.DefaultIntervalSize = thisControl.IntervalSize Then thisControl.IntervalSize = defaultInterval
        thisControl.DefaultIntervalSize = defaultInterval
        '
        thisControl.UpdateDataView()

    End Sub

    Public Property Data As Double()
        Get
            Return CType(GetValue(DataProperty), Double())
        End Get
        Set(value As Double())
            SetValue(DataProperty, value)
        End Set
    End Property

    Public Shared ReadOnly DataNameProperty As DependencyProperty = DependencyProperty.Register(NameOf(DataName), GetType(String), GetType(NumericColumnStats), New UIPropertyMetadata(""))

    Public Property DataName As String
        Get
            Return CType(GetValue(DataNameProperty), String)
        End Get
        Set(value As String)
            SetValue(DataNameProperty, value)
        End Set
    End Property


    Public Shared ReadOnly ClassCountProperty As DependencyProperty = DependencyProperty.Register(NameOf(ClassCount), GetType(Integer), GetType(NumericColumnStats), New UIPropertyMetadata(CInt(4), AddressOf ClassCountProperty_Callback))
    Private Shared Sub ClassCountProperty_Callback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        CType(d, NumericColumnStats).Plot()
    End Sub

    Public Property ClassCount As Integer
        Get
            Return CType(GetValue(ClassCountProperty), Integer)
        End Get
        Set(value As Integer)
            SetValue(ClassCountProperty, value)
        End Set
    End Property

    Public Shared ReadOnly ThresholdProperty As DependencyProperty = DependencyProperty.Register(NameOf(Threshold), GetType(Double), GetType(NumericColumnStats), New UIPropertyMetadata(CDbl(0.4), AddressOf ClassCountProperty_Callback))

    Public Property Threshold As Double
        Get
            Return CType(GetValue(ThresholdProperty), Double)
        End Get
        Set(value As Double)
            SetValue(ThresholdProperty, value)
        End Set
    End Property

    Public Shared ReadOnly DeviationsProperty As DependencyProperty = DependencyProperty.Register(NameOf(Deviations), GetType(Double), GetType(NumericColumnStats), New UIPropertyMetadata(CDbl(2), AddressOf ClassCountProperty_Callback))

    Public Property Deviations As Double
        Get
            Return CType(GetValue(DeviationsProperty), Double)
        End Get
        Set(value As Double)
            SetValue(DeviationsProperty, value)
        End Set
    End Property

    Public Shared ReadOnly IntervalSizeProperty As DependencyProperty = DependencyProperty.Register(NameOf(IntervalSize), GetType(Double), GetType(NumericColumnStats), New UIPropertyMetadata(CDbl(10), AddressOf ClassCountProperty_Callback))

    Public Property IntervalSize As Double
        Get
            Return CType(GetValue(IntervalSizeProperty), Double)
        End Get
        Set(value As Double)
            SetValue(IntervalSizeProperty, value)
        End Set
    End Property

    Public Shared ReadOnly DefaultIntervalSizeProperty As DependencyProperty = DependencyProperty.Register(NameOf(DefaultIntervalSize), GetType(Double), GetType(NumericColumnStats), New UIPropertyMetadata(CDbl(10)))

    Public Property DefaultIntervalSize As Double
        Get
            Return CType(GetValue(DefaultIntervalSizeProperty), Double)
        End Get
        Set(value As Double)
            SetValue(DefaultIntervalSizeProperty, value)
        End Set
    End Property

    Public Shared ReadOnly DefaultRProperty As DependencyProperty = DependencyProperty.Register(NameOf(DefaultR), GetType(Double), GetType(NumericColumnStats), New UIPropertyMetadata(CDbl(1.08)))

    Public Property DefaultR As Double
        Get
            Return CType(GetValue(DefaultRProperty), Double)
        End Get
        Set(value As Double)
            SetValue(DefaultRProperty, value)
        End Set
    End Property

    Public Shared ReadOnly GeometricRProperty As DependencyProperty = DependencyProperty.Register(NameOf(GeometricR), GetType(Double), GetType(NumericColumnStats), New UIPropertyMetadata(CDbl(1.08), AddressOf ClassCountProperty_Callback))

    Public Property GeometricR As Double
        Get
            Return CType(GetValue(GeometricRProperty), Double)
        End Get
        Set(value As Double)
            SetValue(GeometricRProperty, value)
        End Set
    End Property

    Public Shared ReadOnly GeometricMinProperty As DependencyProperty = DependencyProperty.Register(NameOf(GeometricMin), GetType(Double), GetType(NumericColumnStats), New UIPropertyMetadata(CDbl(1), AddressOf ClassCountProperty_Callback))

    Public Property GeometricMin As Double
        Get
            Return CType(GetValue(GeometricMinProperty), Double)
        End Get
        Set(value As Double)
            SetValue(GeometricMinProperty, value)
        End Set
    End Property


    Public Shared ReadOnly GeometricMirrorProperty As DependencyProperty = DependencyProperty.Register(NameOf(GeometricMirror), GetType(Boolean), GetType(NumericColumnStats), New UIPropertyMetadata(False, AddressOf ClassCountProperty_Callback))

    Public Property GeometricMirror As Boolean
        Get
            Return CType(GetValue(GeometricMirrorProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(GeometricMirrorProperty, value)
        End Set
    End Property
    'Public Shared ReadOnly GeometricMaxProperty As DependencyProperty = DependencyProperty.Register(NameOf(GeometricMax), GetType(Double), GetType(NumericColumnStats), New UIPropertyMetadata(CDbl(100), AddressOf ClassCountProperty_Callback))

    'Public Property GeometricMax As Double
    '    Get
    '        Return CType(GetValue(GeometricMaxProperty), Double)
    '    End Get
    '    Set(value As Double)
    '        SetValue(GeometricMaxProperty, value)
    '    End Set
    'End Property

    'Public ReadOnly Property BackgroundHistogramData As New ObservableCollection(Of OxyPlot.Series.HistogramItem)
    Public ReadOnly Property HistogramData As New ObservableCollection(Of OxyPlot.Series.HistogramItem)

    Private _sortedData() As Double = New Double() {}
    Private _stringFormat As String = "{0:0}"

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        '
        'Set the summary statistics table
        Dim statsDataTable As New DataTable("StatsDataTable")
        statsDataTable.Columns.Add(New DataColumn("Statistic", GetType(String)))
        statsDataTable.Columns.Add(New DataColumn("Value", GetType(Double)))
        statsDataTable.Rows.Add({"Count", Double.NaN})
        statsDataTable.Rows.Add({"Minimum", Double.NaN})
        statsDataTable.Rows.Add({"Maximum", Double.NaN})
        statsDataTable.Rows.Add({"Sum", Double.NaN})
        statsDataTable.Rows.Add({"Mean", Double.NaN})
        statsDataTable.Rows.Add({"Std. Deviation", Double.NaN})
        statsDataTable.Rows.Add({"5th %-ile", Double.NaN})
        statsDataTable.Rows.Add({"25th %-ile", Double.NaN})
        statsDataTable.Rows.Add({"50th %-ile", Double.NaN})
        statsDataTable.Rows.Add({"75th %-ile", Double.NaN})
        statsDataTable.Rows.Add({"95th %-ile", Double.NaN})

        Dim statsDataView As New InMemoryReader(statsDataTable)
        StatsTable.DataView = statsDataView.GetTableManager(statsDataTable.TableName)
        StatsTable.TableToolbarTray.Visibility = Visibility.Collapsed
        '
        'Set the breaks table
        Dim breaksDataTable As New DataTable("BreaksTable")
        breaksDataTable.Columns.Add(New DataColumn("Less Than", GetType(Double)))
        breaksDataTable.Columns.Add(New DataColumn("Count", GetType(Integer)))
        Dim breaksDataView As New InMemoryReader(breaksDataTable)
        Dim viewer = breaksDataView.GetTableManager(breaksDataTable.TableName)
        AddHandler viewer.EditAdded, AddressOf BreaksEditAdded
        BreaksTable.DataView = viewer
        BreaksTable.SetColumnsAsReadOnly({"Count"})
        '
        BreaksTable.TableToolbarTray.Visibility = Visibility.Collapsed

        HistogramSeries.FillColor = Color.FromArgb(75, 220, 20, 60)
        HistogramSeries.StrokeColor = Color.FromArgb(255, 255, 0, 0)


    End Sub

    Private Sub BreaksEditAdded(edit As TableEdit)
        'Throw New NotImplementedException()
        If edit.GetType = GetType(CellEdit) Then
            Dim breaks = Array.ConvertAll(BreaksTable.DataView.GetColumn(0), Function(o) If(IsDBNull(o), 0, CDbl(o)))

            Dim update As Boolean = False
            For i As Int32 = DirectCast(edit, CellEdit).RowIndex To 1 Step -1
                If breaks(i) < breaks(i - 1) Then
                    breaks(i - 1) = breaks(i)
                    update = True
                End If
            Next
            For i As Int32 = DirectCast(edit, CellEdit).RowIndex To breaks.Count - 2
                If breaks(i + 1) < breaks(i) Then
                    breaks(i + 1) = breaks(i)
                    update = True
                End If
            Next

            If update = True Then
                BreaksTable.DataView.EditColumn(0, breaks)
            End If

            If ManualIntervalItem.IsSelected Then
                Plot()
            Else
                If breaks.Count <> ClassCount Then ClassCount = breaks.Count
                ManualIntervalItem.IsSelected = True
            End If
        End If
        'Debug.Print(edit.GetType().ToString)
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

    Private Sub GroupingComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        ClassCountControl.Visibility = Visibility.Collapsed
        HeadTailThresholdControl.Visibility = Visibility.Collapsed
        DeviationsControl.Visibility = Visibility.Collapsed
        IntervalSizeControl.Visibility = Visibility.Collapsed
        GeometricRControl.Visibility = Visibility.Collapsed
        GeometricMirrorControl.Visibility = Visibility.Collapsed

        If JenksBreaksItem.IsSelected OrElse QuantilesItem.IsSelected OrElse EqualIntervalItem.IsSelected OrElse ManualIntervalItem.IsSelected Then
            ClassCountControl.Visibility = Visibility.Visible
            'ElseIf GeometricItem.IsSelected Then
            'GeometricRControl.Visibility = Visibility.Visible
            'GeometricMirrorControl.Visibility = Visibility.Visible
        ElseIf HeadTailsItem.IsSelected Then
            HeadTailThresholdControl.Visibility = Visibility.Visible
        ElseIf StandardDeviationItem.IsSelected Then
            DeviationsControl.Visibility = Visibility.Visible
        ElseIf CustomIntervalItem.IsSelected Then
            IntervalSizeControl.Visibility = Visibility.Visible
        Else


        End If
        Plot()
    End Sub

    Private Sub UpdateDataView()
        If StatsTable Is Nothing OrElse StatsTable.DataView Is Nothing Then Exit Sub
        Dim columnData() As Double
        If _sortedData Is Nothing OrElse _sortedData.Count = 0 Then
            ReDim columnData(StatsTable.DataView.NumberOfRows - 1)
        Else
            Dim prodMoments = ProductMoments(_sortedData)
            columnData = New Double() {
            _sortedData.Count,
            _sortedData(0),
            _sortedData.Last,
            _sortedData.Sum,
            prodMoments(0),
            prodMoments(1),
            Percentile(_sortedData, 0.05, True),
            Percentile(_sortedData, 0.25, True),
            Percentile(_sortedData, 0.5, True),
            Percentile(_sortedData, 0.75, True),
            Percentile(_sortedData, 0.95, True)
        }
            '
            If _sortedData.Count <> Data.Count Then
                InvalidDataDetectedWarningTextBlock.Visibility = Visibility.Visible
            Else
                InvalidDataDetectedWarningTextBlock.Visibility = Visibility.Collapsed
            End If
        End If

        StatsTable.DataView.EditColumn(1, columnData)
        'StatsTable.ResizeColumnWidth(0)
        StatsTable.UpdateVisibleRows()

        'Update background histogram
        'Dim breaks(-1) As Double
        'Dim rangeCounts(-1) As Int32
        'breaks = Classification.EqualInterval(_sortedData, 50, True)
        'If breaks.Count = 0 Then
        '    'BackgroundHistogramData.Clear()
        '    Exit Sub
        'End If
        'ReDim rangeCounts(breaks.Count - 1)
        'Dim binIdx As Int32 = 0
        'For i As Int32 = 0 To _sortedData.Count - 1
        '    If _sortedData(i) <= breaks(binIdx) Then
        '        rangeCounts(binIdx) += 1
        '    Else
        '        i -= 1 'rangeCounts(binIdx) += 1
        '        binIdx += 1
        '    End If
        'Next

        'Dim ranges As New List(Of Tuple(Of Double, Double)) '= jenksBreaks.GetRanges()
        'ranges.Add(New Tuple(Of Double, Double)(_sortedData(0), breaks(0)))
        'For i As Int32 = 1 To breaks.Count - 1
        '    ranges.Add(New Tuple(Of Double, Double)(breaks(i - 1), breaks(i)))
        'Next

        'BackgroundHistogramData.Clear()
        'For i As Int32 = 0 To breaks.Count - 1
        '    Dim hItem = New OxyPlot.Series.HistogramItem(ranges(i).Item1, ranges(i).Item2, rangeCounts(i) * (ranges(i).Item2 - ranges(i).Item1))
        '    BackgroundHistogramData.Add(hItem)
        'Next

        'update histogram numeric output
        _stringFormat = "{0:0}"
        'Dim divisor As Int32 = 1
        Select Case (_sortedData.Last - _sortedData(0))
            Case Is < 0.1
                _stringFormat = "{0:0.####}"
            Case Is < 1
                _stringFormat = "{0:0.###}"
            Case Is < 10
                _stringFormat = "{0:0.##}"
            Case Is < 100
                _stringFormat = "{0:0.#}"
            Case Is >= 1000000
                'divisor = 1000
        End Select
        'If divisor > 1 Then xAxis.Unit = String.Format("{0:n0}", divisor) & "'s"
        '
        HistogramSeries.LabelFormatString = _stringFormat
        Plot()
    End Sub
    Private Sub Plot()
        If GroupingComboBox.SelectedIndex = -1 Then GroupingComboBox.SelectedIndex = 0
        If _sortedData Is Nothing OrElse _sortedData.Count = 0 Then Exit Sub
        Mouse.OverrideCursor = Cursors.Wait

        Dim breaks(-1) As Double
        '
        Dim rangeCounts(breaks.Count - 1) As Int32
        If JenksBreaksItem.IsSelected Then
            'Dim nBreaks As Int32 = Math.Min(convertedData.Distinct().Count, 6)
            'ReDim rangeCounts(nBreaks - 1)
            breaks = Classification.JenksNaturalBreaks(_sortedData, ClassCount, True, rangeCounts)
        ElseIf RiceRuleItem.IsSelected Then
            If _sortedData(0) = _sortedData(_sortedData.Count - 1) Then
                ReDim breaks(0)
                breaks(0) = _sortedData.Count
            Else
                Dim hist = New Numerics.Data.Statistics.Histogram(_sortedData)
                ReDim breaks(hist.NumberOfBins - 1)
                For i As Int32 = 0 To hist.NumberOfBins - 1
                    breaks(i) = hist.Item(i).UpperBound
                Next
            End If
        ElseIf QuantilesItem.IsSelected Then
            breaks = Classification.Quantiles(_sortedData, ClassCount, True)
            'ElseIf GeometricItem.IsSelected Then
            'breaks = Numerics.Classification.GeometricInterval(_sortedData, GeometricR, True, GeometricMirror)
            'breaks = Numerics.Classification.GeometricInterval(GeometricR, GeometricMin, GeometricMax)
        ElseIf HeadTailsItem.IsSelected Then
            breaks = Classification.HeadTailInterval(_sortedData, True, Threshold)
        ElseIf StandardDeviationItem.IsSelected Then
            breaks = Classification.StandardDeviationInterval(_sortedData, Deviations, True)
        ElseIf EqualIntervalItem.IsSelected Then
            breaks = Classification.EqualInterval(_sortedData, ClassCount, True)
        ElseIf CustomIntervalItem.IsSelected Then
            breaks = Classification.DefinedInterval(_sortedData, IntervalSize, True)
        ElseIf ManualIntervalItem.IsSelected Then
            breaks = Array.ConvertAll(BreaksTable.DataView.GetColumn(0), Function(o) If(IsDBNull(o), 0, CDbl(o)))
            If breaks.Count < ClassCount Then
                Dim oldCount As Int32 = breaks.Count
                ReDim Preserve breaks(ClassCount - 1)
                For i As Int32 = oldCount To ClassCount - 1
                    breaks(i) = breaks(i - 1)
                Next
            End If
        End If
        '
        'If there are too many breaks don't show the label.
        If breaks.Count >= 30 Then
            HistogramSeries.LabelFormatString = Nothing
        ElseIf HistogramSeries.LabelFormatString Is Nothing Then
            HistogramSeries.LabelFormatString = _stringFormat
        End If
        'Clear breaks table
        'If BreaksTable.DataView.NumberOfRows > 0 Then BreaksTable.DataView.DeleteRows(0, BreaksTable.DataView.NumberOfRows - 1)
        '
        If breaks.Count = 0 Then
            HistogramData.Clear()
            Exit Sub
        End If
        '
        ReDim rangeCounts(breaks.Count - 1)
        Dim binIdx As Int32 = 0
        For i As Int32 = 0 To _sortedData.Count - 1
            If _sortedData(i) <= breaks(binIdx) Then
                rangeCounts(binIdx) += 1
            Else
                i -= 1 'rangeCounts(binIdx) += 1
                binIdx += 1
                If binIdx >= breaks.Length Then Exit For
            End If
        Next

        Dim ranges As New List(Of Tuple(Of Double, Double)) '= jenksBreaks.GetRanges()
        ranges.Add(New Tuple(Of Double, Double)(_sortedData(0), breaks(0)))
        For i As Int32 = 1 To breaks.Count - 1
            'If rangeCounts(i) = 0 Then
            '    ranges(i - 1) = New Tuple(Of Double, Double)(ranges(i - 1).Item1, breaks(i))
            'Else
            ranges.Add(New Tuple(Of Double, Double)(breaks(i - 1), breaks(i)))
            'End If
        Next

        HistogramData.Clear()

        Dim breaksDataTable As New DataTable("BreaksTable")
        breaksDataTable.Columns.Add(New DataColumn("Less Than", GetType(Double)))
        breaksDataTable.Columns.Add(New DataColumn("Count", GetType(Integer)))

        For i As Int32 = 0 To ranges.Count - 1
            'Set breaks table
            breaksDataTable.Rows.Add({ranges(i).Item2, rangeCounts(i)})
            'BreaksTable.DataView.AddRow({ranges(i).Item2, rangeCounts(i)})
            'CType(plotSeries, Wpf.ColumnSeries).Items.Add(New Series.ColumnItem(rangeCounts(i), i))

            If ranges(i).Item2 = ranges(i).Item1 Then
                Dim offset = 0.000001 '.1 / (convertedData(convertedData.Count - 1) - convertedData(0))
                Dim hItem = New OxyPlot.Series.HistogramItem(ranges(i).Item1 - offset, ranges(i).Item2 + offset, rangeCounts(i) * (2 * offset))
                HistogramData.Add(hItem)
            ElseIf rangeCounts(i) = 0 Then
                Dim hItem = New OxyPlot.Series.HistogramItem(ranges(i).Item1, ranges(i).Item2, 0)
                HistogramData.Add(hItem)
            Else
                Dim hItem = New OxyPlot.Series.HistogramItem(ranges(i).Item1, ranges(i).Item2, rangeCounts(i) * (ranges(i).Item2 - ranges(i).Item1))
                HistogramData.Add(hItem)
            End If
        Next

        'Set up the breaks table
        Dim breaksDataView As New InMemoryReader(breaksDataTable)
        RemoveHandler BreaksTable.DataView.EditAdded, AddressOf BreaksEditAdded
        BreaksTable.DataView = breaksDataView.GetTableManager(breaksDataTable.TableName)
        AddHandler BreaksTable.DataView.EditAdded, AddressOf BreaksEditAdded
        BreaksTable.SetColumnsAsReadOnly({"Count"})
        '
        StatsPlot.InvalidatePlot(True)
        Mouse.OverrideCursor = Nothing
    End Sub
End Class
