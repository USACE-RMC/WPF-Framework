Public Class DateAndTimePickerControl
    Public Shared DateAndTimeProperty As DependencyProperty = DependencyProperty.Register(NameOf(DateAndTime), GetType(DateTime), GetType(DateAndTimePickerControl), New UIPropertyMetadata(New DateTime(1980, 7, 30, 12, 0, 0), AddressOf TimePropertyCallback))

    Private Shared Sub TimePropertyCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(DateAndTimePickerControl) Then Exit Sub
        Dim thisControl = DirectCast(d, DateAndTimePickerControl)
        '
        If IsNothing(e.NewValue) Then Exit Sub
        If e.NewValue.GetType <> GetType(DateTime) Then Exit Sub
        If thisControl.CalendarSelector.DisplayDate <> DirectCast(e.NewValue, DateTime) Then thisControl.CalendarSelector.DisplayDate = DirectCast(e.NewValue, DateTime)
        'For Each child In thisControl.HoursGrid.Children
        '    If child.GetType <> GetType(ClockToggle) Then Continue For
        '    DirectCast(child, ClockToggle).IsSelected = CInt(DirectCast(child, ClockToggle).Text) = DirectCast(e.NewValue, DateTime).Hour
        'Next
    End Sub
    Public Shared Is24HourProperty As DependencyProperty = DependencyProperty.Register(NameOf(Is24Hour), GetType(Boolean), GetType(DateAndTimePickerControl), New FrameworkPropertyMetadata(False))
    Public Property Is24Hour As Boolean
        Get
            Return DirectCast(GetValue(Is24HourProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(Is24HourProperty, value)
        End Set
    End Property
    Public Property DateAndTime As DateTime
        Get
            Return DirectCast(GetValue(DateAndTimeProperty), DateTime)
        End Get
        Set(value As DateTime)
            SetValue(DateAndTimeProperty, value)
        End Set
    End Property
    Public Shared HasSecondsProperty As DependencyProperty = DependencyProperty.Register(NameOf(HasSeconds), GetType(Boolean), GetType(DateAndTimePickerControl), New UIPropertyMetadata(False))
    Public Property HasSeconds As Boolean
        Get
            Return DirectCast(GetValue(HasSecondsProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(HasSecondsProperty, value)
        End Set
    End Property
    Public Shared SetToHoursOnLoadProperty As DependencyProperty = DependencyProperty.Register(NameOf(SetToHoursOnLoad), GetType(Boolean), GetType(DateAndTimePickerControl), New UIPropertyMetadata(False))
    Public Property SetToHoursOnLoad As Boolean
        Get
            Return DirectCast(GetValue(SetToHoursOnLoadProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(SetToHoursOnLoadProperty, value)
        End Set
    End Property

    Private Sub CalendarSelector_DisplayDateChanged(sender As Object, e As CalendarDateChangedEventArgs)
        DateAndTime = New DateTime(e.AddedDate.Value.Year, e.AddedDate.Value.Month, DateAndTime.Day, DateAndTime.Hour, DateAndTime.Minute, DateAndTime.Second)
    End Sub

    Private Sub CalendarSelector_SelectedDatesChanged(sender As Object, e As SelectionChangedEventArgs)
        If e.AddedItems.Count = 0 Then Exit Sub
        If e.AddedItems(0).GetType <> GetType(DateTime) Then Exit Sub
        Dim newDate As DateTime = CDate(e.AddedItems(0))
        DateAndTime = New DateTime(DateAndTime.Year, DateAndTime.Month, newDate.Day, DateAndTime.Hour, DateAndTime.Minute, DateAndTime.Second)
    End Sub

    ''' <summary>
    ''' The following solution is to release the mouse capture from the calendar to allow other controls to get focus. 
    ''' Solution was found here: https://stackoverflow.com/questions/25352961/have-to-click-away-twice-from-calendar-in-wpf/50536606#50536606
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CalendarSelector_GotMouseCapture(sender As Object, e As MouseEventArgs)
        Dim originalElement As UIElement = CType(e.OriginalSource, UIElement)
        If originalElement.GetType = GetType(Primitives.CalendarDayButton) OrElse originalElement.GetType = GetType(Primitives.CalendarItem) Then originalElement.ReleaseMouseCapture()
    End Sub

    Private Sub DateAndTimePickerControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If SetToHoursOnLoad = True Then
            Clock.IsHours = True
            Clock.IsMinutes = False
            Clock.IsSeconds = False
        End If
    End Sub
End Class
