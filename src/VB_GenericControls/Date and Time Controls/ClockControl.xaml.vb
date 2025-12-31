
Public Class ClockControl

    Public Shared TimeProperty As DependencyProperty = DependencyProperty.Register(NameOf(Time), GetType(DateTime), GetType(ClockControl), New UIPropertyMetadata(New DateTime(1980, 7, 30, 12, 0, 0), AddressOf TimePropertyCallback))

    Private Shared Sub TimePropertyCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(ClockControl) Then Exit Sub
        Dim thisControl = DirectCast(d, ClockControl)
        '
        If IsNothing(e.NewValue) Then Exit Sub
        If e.NewValue.GetType <> GetType(DateTime) Then Exit Sub
        thisControl.ClearSelected()
        thisControl.SelectCurrentTime(DirectCast(e.NewValue, DateTime))
    End Sub

    Public Property Time As DateTime
        Get
            Return DirectCast(GetValue(TimeProperty), DateTime)
        End Get
        Set(value As DateTime)
            SetValue(TimeProperty, value)
        End Set
    End Property

    Public Shared Is24HourProperty As DependencyProperty = DependencyProperty.Register(NameOf(Is24Hour), GetType(Boolean), GetType(ClockControl), New FrameworkPropertyMetadata(False))
    Public Property Is24Hour As Boolean
        Get
            Return DirectCast(GetValue(Is24HourProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(Is24HourProperty, value)
        End Set
    End Property

    Public Shared IsHoursProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsHours), GetType(Boolean), GetType(ClockControl), New UIPropertyMetadata(True))
    Public Property IsHours As Boolean
        Get
            Return DirectCast(GetValue(IsHoursProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsHoursProperty, value)
            ClearSelected()
            SelectCurrentTime(Time)
        End Set
    End Property
    Public Shared IsMinutesProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsMinutes), GetType(Boolean), GetType(ClockControl), New UIPropertyMetadata(False))
    Public Property IsMinutes As Boolean
        Get
            Return DirectCast(GetValue(IsMinutesProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsMinutesProperty, value)
            ClearSelected()
            SelectCurrentTime(Time)
        End Set
    End Property
    Public Shared IsSecondsProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsSeconds), GetType(Boolean), GetType(ClockControl), New UIPropertyMetadata(False))
    Public Property IsSeconds As Boolean
        Get
            Return DirectCast(GetValue(IsSecondsProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsSecondsProperty, value)
            ClearSelected()
            SelectCurrentTime(Time)
        End Set
    End Property
    Public Shared HasMinutesProperty As DependencyProperty = DependencyProperty.Register(NameOf(HasMinutes), GetType(Boolean), GetType(ClockControl), New UIPropertyMetadata(True))
    Public Property HasMinutes As Boolean
        Get
            Return DirectCast(GetValue(HasMinutesProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(HasMinutesProperty, value)
        End Set
    End Property
    Public Shared HasSecondsProperty As DependencyProperty = DependencyProperty.Register(NameOf(HasSeconds), GetType(Boolean), GetType(ClockControl), New UIPropertyMetadata(False))
    Public Property HasSeconds As Boolean
        Get
            Return DirectCast(GetValue(HasSecondsProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(HasSecondsProperty, value)
        End Set
    End Property
    Public Shared SelectedTimeColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedTimeColor), GetType(SolidColorBrush), GetType(ClockControl), New UIPropertyMetadata(Brushes.Black))
    Public Property SelectedTimeColor As SolidColorBrush
        Get
            Return DirectCast(GetValue(SelectedTimeColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(SelectedTimeColorProperty, value)
        End Set
    End Property
    Public Shared TimeColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(TimeColor), GetType(SolidColorBrush), GetType(ClockControl), New UIPropertyMetadata(Brushes.DarkGray))
    Public Property TimeColor As SolidColorBrush
        Get
            Return DirectCast(GetValue(TimeColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(TimeColorProperty, value)
        End Set
    End Property
    Public Shared SelectedTimeFontSizeProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedTimeFontSize), GetType(Double), GetType(ClockControl), New UIPropertyMetadata(CDbl(24)))
    Public Property SelectedTimeFontSize As Double
        Get
            Return DirectCast(GetValue(SelectedTimeFontSizeProperty), Double)
        End Get
        Set(value As Double)
            SetValue(SelectedTimeFontSizeProperty, value)
        End Set
    End Property
    Public Shared TimeFontSizeProperty As DependencyProperty = DependencyProperty.Register(NameOf(TimeFontSize), GetType(Double), GetType(ClockControl), New UIPropertyMetadata(CDbl(24)))
    Public Property TimeFontSize As Double
        Get
            Return DirectCast(GetValue(TimeFontSizeProperty), Double)
        End Get
        Set(value As Double)
            SetValue(TimeFontSizeProperty, value)
        End Set
    End Property

    Public Shared SelectedColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedColor), GetType(SolidColorBrush), GetType(ClockControl), New UIPropertyMetadata(New SolidColorBrush(Color.FromArgb(255, 24, 24, 25))))
    Public Property SelectedColor As SolidColorBrush
        Get
            Return DirectCast(GetValue(SelectedColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(SelectedColorProperty, value)
        End Set
    End Property
    Public Shared HighlightColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(HighlightColor), GetType(SolidColorBrush), GetType(ClockControl), New UIPropertyMetadata(Brushes.LightGray))
    Public Property HighlightColor As SolidColorBrush
        Get
            Return DirectCast(GetValue(HighlightColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(HighlightColorProperty, value)
        End Set
    End Property
    Public Shared FaceColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(FaceColor), GetType(SolidColorBrush), GetType(ClockControl), New UIPropertyMetadata(New SolidColorBrush(Color.FromArgb(255, 240, 240, 245))))
    Public Property FaceColor As SolidColorBrush
        Get
            Return DirectCast(GetValue(FaceColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(FaceColorProperty, value)
        End Set
    End Property
    Public Shared HandColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(HandColor), GetType(SolidColorBrush), GetType(ClockControl), New UIPropertyMetadata(Brushes.DarkGray))
    Public Property HandColor As SolidColorBrush
        Get
            Return DirectCast(GetValue(HandColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(HandColorProperty, value)
        End Set
    End Property
    Public Shared PreviewHandColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(PreviewHandColor), GetType(SolidColorBrush), GetType(ClockControl), New UIPropertyMetadata(Brushes.LightGray))
    Public Property PreviewHandColor As SolidColorBrush
        Get
            Return DirectCast(GetValue(PreviewHandColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(PreviewHandColorProperty, value)
        End Set
    End Property

    Public Shared FontColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(FontColor), GetType(SolidColorBrush), GetType(ClockControl), New UIPropertyMetadata(Brushes.Black))
    Public Property FontColor As SolidColorBrush
        Get
            Return DirectCast(GetValue(FontColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(FontColorProperty, value)
        End Set
    End Property
    Public Shared SelectedFontColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedFontColor), GetType(SolidColorBrush), GetType(ClockControl), New UIPropertyMetadata(Brushes.White))
    Public Property SelectedFontColor As SolidColorBrush
        Get
            Return DirectCast(GetValue(SelectedFontColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(SelectedFontColorProperty, value)
        End Set
    End Property

    Private _clockMajorNumberStyle As Style
    Private _clockMinorNumberStyle As Style
    Private _hourBorders(-1) As ClockToggle
    Private _minuteBorders(59) As ClockToggle
    Private _secondBorders(59) As ClockToggle

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        '
        ' Add any initialization after the InitializeComponent() call.
        _clockMajorNumberStyle = CType(Me.FindResource("ClockMajorValueStyle"), Style)
        _clockMinorNumberStyle = CType(Me.FindResource("ClockMinorValueStyle"), Style)
        'Set up Hours
        InitializeClock()
        '
        'Minute and second borders only need to be set once on initialization
        Dim transformRadius As Double = 42 'transform radius
        'Get and set the minute and second tick circles
        For i As Int32 = 1 To 59
            If (i Mod 5) <> 0 Then
                _minuteBorders(i) = GetBorder(i, 180 - i * 6, transformRadius, ClockToggle.State.MinuteMinor)
                MinutesGrid.Children.Add(_minuteBorders(i))
                _secondBorders(i) = GetBorder(i, 180 - i * 6, transformRadius, ClockToggle.State.MinuteMinor)
                SecondsGrid.Children.Add(_secondBorders(i))
            End If
        Next

        'larger toggles (5, 10, 15, etc.) need to be rendered after to make sure they show up on top of the minor minute toggles.
        'Adding to the grid after the minors have been added set the render order.
        For i As Int32 = 0 To 55 Step 5
            _minuteBorders(i) = GetBorder(i, 180 - i * 6, transformRadius, ClockToggle.State.MinuteMajor)
            MinutesGrid.Children.Add(_minuteBorders(i))
            _secondBorders(i) = GetBorder(i, 180 - i * 6, transformRadius, ClockToggle.State.MinuteMajor)
            SecondsGrid.Children.Add(_secondBorders(i))
        Next
    End Sub
    Private Sub ClockControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        InitializeClock()
        SelectCurrentTime(Time)
    End Sub
    Private Sub InitializeClock()
        Dim transformRadius As Double = 42 'transform radius
        '
        'Get and set the hour tick circles
        ReDim _hourBorders(If(Is24Hour = True, 23, 11))
        For i As Int32 = HoursGrid.Children.Count - 1 To 0 Step -1
            If HoursGrid.Children(i).GetType = GetType(ClockToggle) Then HoursGrid.Children.RemoveAt(i)
        Next
        'Add hour tick circles
        Dim angle As Double = 360 / _hourBorders.Count
        If Is24Hour Then
            For i As Int32 = 1 To _hourBorders.Count - 1 Step 2
                _hourBorders(i) = GetBorder(i, 180 - i * angle, transformRadius, ClockToggle.State.HourMinor)
                HoursGrid.Children.Add(_hourBorders(i))
            Next
            '
            _hourBorders(0) = GetBorder(_hourBorders.Count, 180, transformRadius, ClockToggle.State.HourMajor)
            HoursGrid.Children.Add(_hourBorders(0))
            For i As Int32 = 2 To _hourBorders.Count - 1 Step 2
                _hourBorders(i) = GetBorder(i, 180 - i * angle, transformRadius, ClockToggle.State.HourMajor)
                HoursGrid.Children.Add(_hourBorders(i))
            Next
        Else
            _hourBorders(0) = GetBorder(_hourBorders.Count, 180, transformRadius, ClockToggle.State.HourMajor)
            HoursGrid.Children.Add(_hourBorders(0))
            For i As Int32 = 1 To _hourBorders.Count - 1
                _hourBorders(i) = GetBorder(i, 180 - i * angle, transformRadius, ClockToggle.State.HourMajor)
                HoursGrid.Children.Add(_hourBorders(i))
            Next
        End If
    End Sub

    Private Function GetBorder(t As Integer, angleDegree As Double, transformRadius As Double, borderState As ClockToggle.State) As ClockToggle
        Dim b As New ClockToggle() With {.TimeValue = t, .TimeState = borderState}
        If borderState = ClockToggle.State.MinuteMinor Or borderState = ClockToggle.State.SecondMinor Or borderState = ClockToggle.State.HourMinor Then
            b.Style = _clockMinorNumberStyle
        Else
            b.Style = _clockMajorNumberStyle
        End If
        '
        b.RenderTransform = New TranslateTransform(transformRadius * Math.Sin(angleDegree * Math.PI / 180), transformRadius * Math.Cos(angleDegree * Math.PI / 180))
        Return b
    End Function

    Private Sub HoursMouseUp(sender As Object, e As MouseButtonEventArgs)
        IsMinutes = False
        IsSeconds = False
        IsHours = True
    End Sub
    Private Sub MinutesMouseUp(sender As Object, e As MouseButtonEventArgs)
        If HasMinutes = False Then Exit Sub
        IsMinutes = True
        IsSeconds = False
        IsHours = False
    End Sub
    Private Sub SecondsMouseUp(sender As Object, e As MouseButtonEventArgs)
        IsMinutes = False
        IsSeconds = True
        IsHours = False
    End Sub
    Private Sub AmPmMouseUp(sender As Object, e As MouseButtonEventArgs)
        Dim hour As Int32 = Time.Hour
        If hour >= 12 Then hour = hour - 12 Else hour = hour + 12
        Time = New DateTime(Time.Year, Time.Month, Time.Day, hour, Time.Minute, Time.Second)
    End Sub
    Private Sub ClockFace_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.ChangedButton = MouseButton.Left Then
            If IsHours Then
                If HasMinutes Then
                    IsMinutes = True
                    IsSeconds = False
                    IsHours = False
                End If
            ElseIf IsMinutes Then
                    IsHours = False
                    If HasSeconds Then
                        IsMinutes = False
                        IsSeconds = True
                    End If
                ElseIf IsSeconds Then
                    IsHours = False
                If HasSeconds Then
                    IsMinutes = False
                    IsSeconds = True
                End If
            End If
        End If
    End Sub
    Private Sub ClockFace_MouseDown(sender As Object, e As MouseButtonEventArgs)
        Dim timeValue As Int32 = GetTimeValue(e.GetPosition(ClockFace))
        ClearHighlighted()
        If e.LeftButton = MouseButtonState.Pressed Then
            SetTimeValue(timeValue)
        Else
            SetHighlighted(timeValue)
        End If
    End Sub
    Private Sub ClockFace_MouseMove(sender As Object, e As MouseEventArgs)
        Dim timeValue As Int32 = GetTimeValue(e.GetPosition(ClockFace))
        ClearHighlighted()
        If e.LeftButton = MouseButtonState.Pressed Then
            SetTimeValue(timeValue)
        Else
            SetHighlighted(timeValue)
        End If
        'UpdateForTimeValue(timeValue, e.LeftButton)
    End Sub
    Private Function GetTimeValue(p As Point) As Int32
        'Dim radiusFromCenter As Double = LineMagnitude(p.X, p.Y, 50, 50)
        'Dim p0 As New Point(50, 50)
        'Dim slope As Double = (p.Y - p0.Y) / (p.X - p0.X)
        'Dim angle As Double = Math.Atan(slope)

        Dim quadrantSize As Double
        If IsHours Then
            quadrantSize = 1 / _hourBorders.Count 'If(Is24Hour = True, 24, 12)
        Else 'Must be minutes or seconds
            quadrantSize = 1 / 60
        End If
        '
        Dim angleRadians As Double = Math.Atan2(-50 + p.X, 50 - p.Y)
        Dim arc0To1 As Double = If(angleRadians > 0, (angleRadians / Math.PI) / 2, (2 + angleRadians / Math.PI) / 2)
        Dim timeValue As Int32 = CInt(arc0To1 / quadrantSize)
        If IsHours Then
            If timeValue = _hourBorders.Count Then timeValue = 0
        Else 'Must be minutes or seconds
            If timeValue = 60 Then timeValue = 0
        End If
        Return timeValue
    End Function
    Private Sub SetTimeValue(timeValue As Int32)
        If IsHours Then
            'Convert from AM/PM to 24 hour value
            If Is24Hour = False Then
                Dim hour As Int32 = Time.Hour
                If timeValue >= 12 Then timeValue = timeValue - 12
                If hour >= 12 Then timeValue = timeValue + 12
            End If
            Time = New DateTime(Time.Year, Time.Month, Time.Day, timeValue, Time.Minute, Time.Second)
        ElseIf IsMinutes Then
            Time = New DateTime(Time.Year, Time.Month, Time.Day, Time.Hour, timeValue, Time.Second)
        ElseIf IsSeconds Then
            Time = New DateTime(Time.Year, Time.Month, Time.Day, Time.Hour, Time.Minute, timeValue)
        End If
    End Sub
    Private Sub ClearHighlighted()
        If IsHours Then
            For i As Int32 = 0 To _hourBorders.Count - 1
                _hourBorders(i).IsHighlighted = False
            Next
            PreviewHourHandLine.Visibility = Visibility.Hidden
        ElseIf IsMinutes Then
            For i As Int32 = 0 To _minuteBorders.Count - 1
                _minuteBorders(i).IsHighlighted = False
            Next
            PreviewMinuteHandLine.Visibility = Visibility.Hidden
        ElseIf IsSeconds Then
            For i As Int32 = 0 To _secondBorders.Count - 1
                _secondBorders(i).IsHighlighted = False
            Next
            PreviewSecondHandLine.Visibility = Visibility.Hidden
        End If
    End Sub
    Private Sub SetHighlighted(timeValue As Int32)
        If IsHours Then
            If _hourBorders(timeValue).IsSelected = False Then _hourBorders(timeValue).IsHighlighted = True
            PreviewHourHandLine.Visibility = Visibility.Visible
            PreviewHourHandLine.X2 = CType(_hourBorders(timeValue).RenderTransform, TranslateTransform).X
            PreviewHourHandLine.Y2 = CType(_hourBorders(timeValue).RenderTransform, TranslateTransform).Y
        ElseIf IsMinutes Then
            If _minuteBorders(timeValue).IsSelected = False Then _minuteBorders(timeValue).IsHighlighted = True
            PreviewMinuteHandLine.Visibility = Visibility.Visible
            PreviewMinuteHandLine.X2 = CType(_minuteBorders(timeValue).RenderTransform, TranslateTransform).X
            PreviewMinuteHandLine.Y2 = CType(_minuteBorders(timeValue).RenderTransform, TranslateTransform).Y
        ElseIf IsSeconds Then
            If _secondBorders(timeValue).IsSelected = False Then _secondBorders(timeValue).IsHighlighted = True
            PreviewSecondHandLine.Visibility = Visibility.Visible
            PreviewSecondHandLine.X2 = CType(_secondBorders(timeValue).RenderTransform, TranslateTransform).X
            PreviewSecondHandLine.Y2 = CType(_secondBorders(timeValue).RenderTransform, TranslateTransform).Y
        End If
    End Sub
    Private Sub ClearSelected()
        If IsHours Then
            For i As Int32 = 0 To _hourBorders.Count - 1
                _hourBorders(i).IsSelected = False
            Next
        ElseIf IsMinutes Then
            For i As Int32 = 0 To _minuteBorders.Count - 1
                _minuteBorders(i).IsSelected = False
            Next
        ElseIf IsSeconds Then
            For i As Int32 = 0 To _secondBorders.Count - 1
                _secondBorders(i).IsSelected = False
            Next
        End If
    End Sub
    Public Sub SelectTime(timeValue As DateTime)
        SelectCurrentTime(timeValue)
    End Sub
    Private Sub SelectCurrentTime(timeValue As DateTime)
        ClearHighlighted()
        If IsHours Then
            Dim hour As Int32 = timeValue.Hour
            If _hourBorders.Count < 13 Then
                If hour >= 12 Then hour = hour - 12 'Else hour = hour + 12
            End If
            _hourBorders(hour).IsSelected = True
            HourHandLine.X2 = CType(_hourBorders(hour).RenderTransform, TranslateTransform).X
            HourHandLine.Y2 = CType(_hourBorders(hour).RenderTransform, TranslateTransform).Y
        ElseIf IsMinutes Then
            _minuteBorders(timeValue.Minute).IsSelected = True
            MinuteHandLine.X2 = CType(_minuteBorders(timeValue.Minute).RenderTransform, TranslateTransform).X
            MinuteHandLine.Y2 = CType(_minuteBorders(timeValue.Minute).RenderTransform, TranslateTransform).Y
        ElseIf IsSeconds Then
            _secondBorders(timeValue.Second).IsSelected = True
            SecondHandLine.X2 = CType(_secondBorders(timeValue.Second).RenderTransform, TranslateTransform).X
            SecondHandLine.Y2 = CType(_secondBorders(timeValue.Second).RenderTransform, TranslateTransform).Y
        End If
        '
        If Is24Hour = False Then
            If timeValue.Hour < 12 Then
                AMFace.Background = SelectedColor
                AMText.Foreground = SelectedFontColor
                PMFace.Background = FaceColor
                PMText.Foreground = FontColor
            Else
                PMFace.Background = SelectedColor
                PMText.Foreground = SelectedFontColor
                AMFace.Background = FaceColor
                AMText.Foreground = FontColor
            End If
        End If
    End Sub

    Public Shared Function LineMagnitude(x1 As Double, y1 As Double, x2 As Double, y2 As Double) As Double
        'There exists methods to approximate the square root that are much faster than math.sqrt(). 
        'May want to consider implementing. http: //blog.wouldbetheologian.com/2011/11/fast-approximate-sqrt-method-in-c.html
        Return Math.Sqrt((x2 - x1) ^ 2 + (y2 - y1) ^ 2)
    End Function

    Private Sub ClockFace_MouseEnter(sender As Object, e As MouseEventArgs)
        ClockFace.Focus()
    End Sub

    Private Sub ClockFace_MouseLeave(sender As Object, e As MouseEventArgs)
        ClearHighlighted()
    End Sub

    Private Sub AMPMBorder_MouseEnter(sender As Object, e As MouseEventArgs)
        Dim selectedBorder As Border = CType(sender, Border)
        '
        Dim hour As Int32 = Time.Hour
        If hour >= 12 And selectedBorder.Equals(AMFace) Then
            AMFace.Background = HighlightColor
        ElseIf hour < 12 And selectedBorder.Equals(PMFace) Then
            PMFace.Background = HighlightColor
        End If

    End Sub

    Private Sub AMPMBorder_MouseLeave(sender As Object, e As MouseEventArgs)
        Dim selectedBorder As Border = CType(sender, Border)
        '
        Dim hour As Int32 = Time.Hour
        If hour >= 12 And selectedBorder.Equals(AMFace) Then
            AMFace.Background = FaceColor
        ElseIf hour < 12 And selectedBorder.Equals(PMFace) Then
            PMFace.Background = FaceColor
        End If
    End Sub

    Private Sub AMPMBorder_MouseUp(sender As Object, e As MouseButtonEventArgs)
        '
        Dim selectedBorder As Border = CType(sender, Border)

        Dim hour As Int32 = Time.Hour
        If selectedBorder.Equals(AMFace) And hour >= 12 Then
            hour = hour - 12
            Time = New DateTime(Time.Year, Time.Month, Time.Day, hour, Time.Minute, Time.Second)
        ElseIf selectedBorder.Equals(PMFace) And hour < 12 Then
            hour = hour + 12
            Time = New DateTime(Time.Year, Time.Month, Time.Day, hour, Time.Minute, Time.Second)
        End If
    End Sub

End Class
Public Class ClockToggle
    Inherits Border

    Public Enum State
        HourMajor
        MinuteMajor
        SecondMajor
        HourMinor
        MinuteMinor
        SecondMinor
    End Enum

    Public Shared IsSelectedProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsSelected), GetType(Boolean), GetType(ClockToggle), New UIPropertyMetadata(False))

    Public Property IsSelected As Boolean
        Get
            Return DirectCast(GetValue(IsSelectedProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsSelectedProperty, value)
        End Set
    End Property
    Public Shared IsHighlightedProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsHighlighted), GetType(Boolean), GetType(ClockToggle), New FrameworkPropertyMetadata(False))
    Public Property IsHighlighted As Boolean
        Get
            Return DirectCast(GetValue(IsHighlightedProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsHighlightedProperty, value)
        End Set
    End Property

    Public Shared FontColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(FontColor), GetType(SolidColorBrush), GetType(ClockToggle), New UIPropertyMetadata(Brushes.Black))
    Public Property FontColor As SolidColorBrush
        Get
            Return DirectCast(GetValue(FontColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(FontColorProperty, value)
        End Set
    End Property
    Public Shared TimeValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(TimeValue), GetType(Integer), GetType(ClockToggle), New UIPropertyMetadata(1))
    Public Property TimeValue As Integer
        Get
            Return DirectCast(GetValue(TimeValueProperty), Integer)
        End Get
        Set(value As Integer)
            SetValue(TimeValueProperty, value)
        End Set
    End Property
    Public Shared TimeStateProp As DependencyProperty = DependencyProperty.Register(NameOf(TimeState), GetType(State), GetType(ClockToggle), New UIPropertyMetadata(State.HourMajor, AddressOf StateChangedCallback))

    Private Shared Sub StateChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(ClockToggle) Then Exit Sub
        Dim thisControl = DirectCast(d, ClockToggle)
        '
        If IsNothing(e.NewValue) Then Exit Sub
        If e.NewValue.GetType <> GetType(State) Then Exit Sub
        Dim newState As State = DirectCast(e.NewValue, State)
        If newState = State.MinuteMinor Or newState = State.SecondMinor Or newState = State.HourMinor Then
            thisControl.Child = Nothing
        End If


    End Sub

    Public Property TimeState As State
        Get
            Return DirectCast(GetValue(TimeStateProp), State)
        End Get
        Set(value As State)
            SetValue(TimeStateProp, value)
        End Set
    End Property

    Public Sub New()

        'Make it a circle
        IsHitTestVisible = False
        SetBinding(Border.CornerRadiusProperty, New Binding() With {.Path = New PropertyPath("ActualHeight"), .Source = Me})
        SetBinding(Border.WidthProperty, New Binding() With {.Path = New PropertyPath("ActualHeight"), .Source = Me})
        'Create inner text
        Dim tBox As New TextBlock With {.FontSize = 7, .VerticalAlignment = VerticalAlignment.Center, .HorizontalAlignment = HorizontalAlignment.Stretch, .TextAlignment = TextAlignment.Center}
        tBox.SetBinding(TextBlock.TextProperty, New Binding() With {.Path = New PropertyPath(NameOf(TimeValue)), .Source = Me})
        tBox.SetBinding(TextBlock.ForegroundProperty, New Binding() With {.Path = New PropertyPath("FontColor"), .Source = Me})
        'add text to circle
        Child = tBox
    End Sub

End Class

