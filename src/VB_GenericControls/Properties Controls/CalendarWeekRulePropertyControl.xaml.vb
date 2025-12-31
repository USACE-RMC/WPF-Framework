Imports System.ComponentModel
Imports System.Globalization
Public Class CalendarWeekRulePropertyControl
    Implements INotifyPropertyChanged

    Public Shared ReadOnly Property CalendarRuleOptions As New List(Of CalendarWeekRule)(DirectCast([Enum].GetValues(GetType(CalendarWeekRule)), CalendarWeekRule()))

    Public Shared SelectedCalendarWeekRuleProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedCalendarWeekRule), GetType(CalendarWeekRule), GetType(CalendarWeekRulePropertyControl), New UIPropertyMetadata(CalendarWeekRule.FirstDay))

    Public Property SelectedCalendarWeekRule As CalendarWeekRule
        Get
            Return DirectCast(GetValue(SelectedCalendarWeekRuleProperty), CalendarWeekRule)
        End Get
        Set(ByVal value As CalendarWeekRule)
            SetValue(SelectedCalendarWeekRuleProperty, value)
        End Set
    End Property
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(CalendarWeekRulePropertyControl), New UIPropertyMetadata("Title"))
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property
    '
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(CalendarWeekRulePropertyControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))
    Public Property MaxPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxPropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(CalendarWeekRulePropertyControl), New UIPropertyMetadata(DefaultMinPropertyWidth))
    Public Property MinPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinPropertyWidthProperty, value)
        End Set
    End Property
    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(CalendarWeekRulePropertyControl), New UIPropertyMetadata(DefaultPropertyWidth))
    Public Property PropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(PropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(PropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(CalendarWeekRulePropertyControl), New UIPropertyMetadata(True))
    Public Property ShowLeaderLine As Boolean
        Get
            Return DirectCast(GetValue(ShowLeaderLineProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowLeaderLineProperty, value)
        End Set
    End Property
    Private _actualWidth As Double = 0
    Public Property ActualPropertyWidth As Double
        Get
            Return _actualWidth
        End Get
        Private Set(value As Double)
            If _actualWidth <> value Then
                _actualWidth = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ActualPropertyWidth)))
            End If
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub ControlSizeChanged(sender As Object, e As SizeChangedEventArgs)
        Dim el = TryCast(sender, FrameworkElement)
        ActualPropertyWidth = el.ActualWidth
    End Sub
End Class
