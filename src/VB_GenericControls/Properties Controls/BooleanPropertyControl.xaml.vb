Public Class BooleanPropertyControl
    Public Shared IsSelectedProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsSelected), GetType(Boolean), GetType(BooleanPropertyControl), New UIPropertyMetadata(True))
    Public Property IsSelected As Boolean
        Get
            Return DirectCast(GetValue(IsSelectedProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsSelectedProperty, value)
        End Set
    End Property
    '
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(BooleanPropertyControl), New UIPropertyMetadata("Title"))
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property
    ''
    'Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(BooleanPropertyControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))
    'Public Property MaxPropertyWidth As Double
    '    Get
    '        Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
    '    End Get
    '    Set(value As Double)
    '        SetValue(MaxPropertyWidthProperty, value)
    '    End Set
    'End Property
    ''
    'Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(TextPropertyControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))
    'Public Property MinPropertyWidth As Double
    '    Get
    '        Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
    '    End Get
    '    Set(value As Double)
    '        SetValue(MinPropertyWidthProperty, value)
    '    End Set
    'End Property
    '
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(BooleanPropertyControl), New UIPropertyMetadata(True))
    Public Property ShowLeaderLine As Boolean
        Get
            Return DirectCast(GetValue(ShowLeaderLineProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowLeaderLineProperty, value)
        End Set
    End Property

    Public Event Checked(sender As Object, e As RoutedEventArgs)
    Public Event Unchecked(sender As Object, e As RoutedEventArgs)

    Private Sub CheckBox_Checked(sender As Object, e As RoutedEventArgs)
        RaiseEvent Checked(Me, e)
    End Sub

    Private Sub CheckBox_Unchecked(sender As Object, e As RoutedEventArgs)
        RaiseEvent Unchecked(Me, e)
    End Sub
End Class
