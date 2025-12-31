Imports System.ComponentModel

Public Class ContentPropertyControl
    Implements INotifyPropertyChanged

    '
    Public Shared InnerContentProperty As DependencyProperty = DependencyProperty.Register(NameOf(InnerContent), GetType(Object), GetType(ContentPropertyControl), New UIPropertyMetadata(Nothing))
    Public Property InnerContent As Object
        Get
            Return DirectCast(GetValue(InnerContentProperty), Object)
        End Get
        Set(value As Object)
            SetValue(InnerContentProperty, value)
        End Set
    End Property

    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(ContentPropertyControl), New UIPropertyMetadata("Title"))
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property
    '
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(ContentPropertyControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))
    Public Property MaxPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxPropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(ContentPropertyControl), New UIPropertyMetadata(DefaultMinPropertyWidth))
    Public Property MinPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinPropertyWidthProperty, value)
        End Set
    End Property

    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(ContentPropertyControl), New UIPropertyMetadata(DefaultPropertyWidth))
    Public Property PropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(PropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(PropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(ContentPropertyControl), New UIPropertyMetadata(True))
    Public Property ShowLeaderLine As Boolean
        Get
            Return DirectCast(GetValue(ShowLeaderLineProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowLeaderLineProperty, value)
        End Set
    End Property
    Public Shared PropertyHeightProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyHeight), GetType(Double), GetType(ContentPropertyControl), New UIPropertyMetadata(DefaultPropertyHeight))
    Public Property PropertyHeight As Double
        Get
            Return DirectCast(GetValue(PropertyHeightProperty), Double)
        End Get
        Set(value As Double)
            SetValue(PropertyHeightProperty, value)
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
