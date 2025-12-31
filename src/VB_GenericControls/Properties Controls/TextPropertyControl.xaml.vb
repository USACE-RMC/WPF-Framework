Imports System.ComponentModel

Public Class TextPropertyControl
    Implements INotifyPropertyChanged

    Public Shared TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(TextPropertyControl), New UIPropertyMetadata(""))
    Public Property Text As String
        Get
            Return DirectCast(GetValue(TextProperty), String)
        End Get
        Set(value As String)
            SetValue(TextProperty, value)
        End Set
    End Property
    '
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(TextPropertyControl), New UIPropertyMetadata("Title"))
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property

    Public Shared TextWrappingProperty As DependencyProperty = DependencyProperty.Register(NameOf(TextWrapping), GetType(TextWrapping), GetType(TextPropertyControl), New UIPropertyMetadata(TextWrapping.WrapWithOverflow))
    Public Property TextWrapping As TextWrapping
        Get
            Return DirectCast(GetValue(TextWrappingProperty), TextWrapping)
        End Get
        Set(value As TextWrapping)
            SetValue(TextWrappingProperty, value)
        End Set
    End Property

    '
    Public Shared MinTitleWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinTitleWidth), GetType(Double), GetType(TextPropertyControl), New UIPropertyMetadata(CDbl(100)))
    Public Property MinTitleWidth As Double
        Get
            Return DirectCast(GetValue(MinTitleWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinTitleWidthProperty, value)
        End Set
    End Property
    '
    Public Shared IsReadOnlyProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsReadOnly), GetType(Boolean), GetType(TextPropertyControl), New UIPropertyMetadata(False))
    Public Property IsReadOnly As Boolean
        Get
            Return DirectCast(GetValue(IsReadOnlyProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsReadOnlyProperty, value)
        End Set
    End Property
    '
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(TextPropertyControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))
    Public Property MaxPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxPropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(TextPropertyControl), New UIPropertyMetadata(DefaultMinPropertyWidth))
    Public Property MinPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinPropertyWidthProperty, value)
        End Set
    End Property

    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(TextPropertyControl), New UIPropertyMetadata(DefaultPropertyWidth))
    Public Property PropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(PropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(PropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(TextPropertyControl), New UIPropertyMetadata(True))
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
