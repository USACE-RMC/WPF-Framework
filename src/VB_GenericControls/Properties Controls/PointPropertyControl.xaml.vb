Public Class PointPropertyControl

    Public Shared DecimalsProperty As DependencyProperty = DependencyProperty.Register(NameOf(Decimals), GetType(Int32), GetType(PointPropertyControl), New UIPropertyMetadata(CInt(5), AddressOf InitializeControl))
    Public Property Decimals As Int32
        Get
            Return DirectCast(GetValue(DecimalsProperty), Int32)
        End Get
        Set(ByVal value As Int32)
            SetValue(DecimalsProperty, value)
        End Set
    End Property

    Public Shared DataPointProperty As DependencyProperty = DependencyProperty.Register(NameOf(DataPoint), GetType(Point), GetType(PointPropertyControl), New UIPropertyMetadata(New Point(0, 0), AddressOf InitializeControl))

    Public Property DataPoint As Point
        Get
            Return DirectCast(GetValue(DataPointProperty), Point)
        End Get
        Set(ByVal value As Point)
            SetValue(DataPointProperty, value)
        End Set
    End Property

    Private Shared Sub InitializeControl(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(PointPropertyControl) Then Exit Sub
        Dim thisControl = DirectCast(d, PointPropertyControl)
        '
        If IsNothing(e.NewValue) Then Exit Sub
        If e.NewValue.GetType <> GetType(Point) Then Exit Sub
        Dim newDataPoint As Point = DirectCast(e.NewValue, Point)
        'Update the textboxes with the new values
        'remove the handlers so the property doesn't get triggered for update.
        RemoveHandler thisControl.DataPointX.TextChanged, AddressOf thisControl.DataPointX_TextChanged
        RemoveHandler thisControl.DataPointY.TextChanged, AddressOf thisControl.DataPointY_TextChanged
        'update the values in the textboxes
        thisControl.DataPointX.Text = Math.Round(newDataPoint.X, thisControl.Decimals).ToString
        thisControl.DataPointY.Text = Math.Round(newDataPoint.Y, thisControl.Decimals).ToString
        'add the handlers back for updating back to source.
        AddHandler thisControl.DataPointX.TextChanged, AddressOf thisControl.DataPointX_TextChanged
        AddHandler thisControl.DataPointY.TextChanged, AddressOf thisControl.DataPointY_TextChanged
    End Sub

    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(PointPropertyControl), New UIPropertyMetadata("Title"))
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property
    '
    Public Shared IsReadOnlyProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsReadOnly), GetType(Boolean), GetType(PointPropertyControl), New UIPropertyMetadata(False))
    Public Property IsReadOnly As Boolean
        Get
            Return DirectCast(GetValue(IsReadOnlyProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsReadOnlyProperty, value)
        End Set
    End Property
    '
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(PointPropertyControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))
    Public Property MaxPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxPropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(PointPropertyControl), New UIPropertyMetadata(DefaultMinPropertyWidth))
    Public Property MinPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinPropertyWidthProperty, value)
        End Set
    End Property

    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(PointPropertyControl), New UIPropertyMetadata(DefaultPropertyWidth))
    Public Property PropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(PropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(PropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(PointPropertyControl), New UIPropertyMetadata(True))
    Public Property ShowLeaderLine As Boolean
        Get
            Return DirectCast(GetValue(ShowLeaderLineProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowLeaderLineProperty, value)
        End Set
    End Property



    Private Sub DataPointX_TextChanged(sender As Object, e As TextChangedEventArgs)
        DataPointChanged()
    End Sub

    Private Sub DataPointY_TextChanged(sender As Object, e As TextChangedEventArgs)
        DataPointChanged()
    End Sub
    Private Sub DataPointChanged()
        If DataPointX.IsValidDouble() = False OrElse DataPointY.IsValidDouble() = False Then Exit Sub 'Point = Point.Undefined
        '
        Dim xValue As Double = DataPointX.GetValueAsDouble()
        Dim yValue As Double = DataPointY.GetValueAsDouble()
        '
        DataPoint = New Point(xValue, yValue)
    End Sub
End Class
