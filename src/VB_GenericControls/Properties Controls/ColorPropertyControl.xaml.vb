
Public Class ColorPropertyControl
    Public Shared SelectedColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedColor), GetType(SolidColorBrush), GetType(ColorPropertyControl), New UIPropertyMetadata(New SolidColorBrush(Colors.Black)))
    Public Property SelectedColor As SolidColorBrush
        Get
            Return DirectCast(GetValue(SelectedColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(SelectedColorProperty, value)
        End Set
    End Property
    '
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(ColorPropertyControl), New UIPropertyMetadata("Title"))
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property
    '
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(ColorPropertyControl), New UIPropertyMetadata(True))
    Public Property ShowLeaderLine As Boolean
        Get
            Return DirectCast(GetValue(ShowLeaderLineProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowLeaderLineProperty, value)
        End Set
    End Property
    Public Shared PropertyHeightProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyHeight), GetType(Double), GetType(ColorPropertyControl), New UIPropertyMetadata(DefaultPropertyHeight))
    Public Property PropertyHeight As Double
        Get
            Return DirectCast(GetValue(PropertyHeightProperty), Double)
        End Get
        Set(value As Double)
            SetValue(PropertyHeightProperty, value)
        End Set
    End Property
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(ColorPropertyControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))
    Public Property MaxPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxPropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(ColorPropertyControl), New UIPropertyMetadata(DefaultMinPropertyWidth))
    Public Property MinPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinPropertyWidthProperty, value)
        End Set
    End Property

    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(ColorPropertyControl), New UIPropertyMetadata(New GridLength(36)))
    Public Property PropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(PropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(PropertyWidthProperty, value)
        End Set
    End Property
End Class


