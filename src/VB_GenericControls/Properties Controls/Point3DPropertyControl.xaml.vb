Imports System.Windows.Media.Media3D
Imports System.ComponentModel

Public Class Point3DPropertyControl
    Implements INotifyPropertyChanged

    Public Shared DecimalsProperty As DependencyProperty = DependencyProperty.Register(NameOf(Decimals), GetType(Int32), GetType(Point3DPropertyControl), New UIPropertyMetadata(CInt(5), AddressOf DecimalsChanged_Callback))
    Public Property Decimals As Int32
        Get
            Return DirectCast(GetValue(DecimalsProperty), Int32)
        End Get
        Set(ByVal value As Int32)
            SetValue(DecimalsProperty, value)
        End Set
    End Property

    Private Shared Sub DecimalsChanged_Callback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(Point3DPropertyControl) Then Exit Sub
        Dim thisControl = DirectCast(d, Point3DPropertyControl)
        '
        If IsNothing(e.NewValue) Then Exit Sub
        If e.NewValue.GetType <> GetType(Int32) Then Exit Sub
        Dim newValue As Int32 = DirectCast(e.NewValue, Int32)
        'Update the textboxes with the new values
        'remove the handlers so the property doesn't get triggered for update.
        RemoveHandler thisControl.DataPointX.TextChanged, AddressOf thisControl.DataPointX_TextChanged
        RemoveHandler thisControl.DataPointY.TextChanged, AddressOf thisControl.DataPointY_TextChanged
        RemoveHandler thisControl.DataPointZ.TextChanged, AddressOf thisControl.DataPointZ_TextChanged
        'update the values in the textboxes
        thisControl.DataPointX.Text = Math.Round(thisControl.DataPoint.X, newValue).ToString
        thisControl.DataPointY.Text = Math.Round(thisControl.DataPoint.Y, newValue).ToString
        thisControl.DataPointZ.Text = Math.Round(thisControl.DataPoint.Z, newValue).ToString
        'add the handlers back for updating back to source.
        AddHandler thisControl.DataPointX.TextChanged, AddressOf thisControl.DataPointX_TextChanged
        AddHandler thisControl.DataPointY.TextChanged, AddressOf thisControl.DataPointY_TextChanged
        AddHandler thisControl.DataPointZ.TextChanged, AddressOf thisControl.DataPointZ_TextChanged
    End Sub

    Public Shared DataPointProperty As DependencyProperty = DependencyProperty.Register(NameOf(DataPoint), GetType(Point3D), GetType(Point3DPropertyControl), New UIPropertyMetadata(New Point3D(Double.MinValue, Double.MinValue, Double.MinValue), AddressOf DataPointChanged_Callback))

    Public Property DataPoint As Point3D
        Get
            Return DirectCast(GetValue(DataPointProperty), Point3D)
        End Get
        Set(ByVal value As Point3D)
            SetValue(DataPointProperty, value)
        End Set
    End Property

    Private Shared Sub DataPointChanged_Callback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(Point3DPropertyControl) Then Exit Sub
        Dim thisControl = DirectCast(d, Point3DPropertyControl)
        '
        If IsNothing(e.NewValue) Then Exit Sub
        If e.NewValue.GetType <> GetType(Point3D) Then Exit Sub
        Dim newDataPoint As Point3D = DirectCast(e.NewValue, Point3D)
        'Update the textboxes with the new values
        'remove the handlers so the property doesn't get triggered for update.
        RemoveHandler thisControl.DataPointX.TextChanged, AddressOf thisControl.DataPointX_TextChanged
        RemoveHandler thisControl.DataPointY.TextChanged, AddressOf thisControl.DataPointY_TextChanged
        RemoveHandler thisControl.DataPointZ.TextChanged, AddressOf thisControl.DataPointZ_TextChanged
        'update the values in the textboxes
        thisControl.DataPointX.Text = Math.Round(newDataPoint.X, thisControl.Decimals).ToString
        thisControl.DataPointY.Text = Math.Round(newDataPoint.Y, thisControl.Decimals).ToString
        thisControl.DataPointZ.Text = Math.Round(newDataPoint.Z, thisControl.Decimals).ToString
        'add the handlers back for updating back to source.
        AddHandler thisControl.DataPointX.TextChanged, AddressOf thisControl.DataPointX_TextChanged
        AddHandler thisControl.DataPointY.TextChanged, AddressOf thisControl.DataPointY_TextChanged
        AddHandler thisControl.DataPointZ.TextChanged, AddressOf thisControl.DataPointZ_TextChanged
    End Sub

    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(Point3DPropertyControl), New UIPropertyMetadata("Title"))
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property
    '
    Public Shared IsReadOnlyProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsReadOnly), GetType(Boolean), GetType(Point3DPropertyControl), New UIPropertyMetadata(False))
    Public Property IsReadOnly As Boolean
        Get
            Return DirectCast(GetValue(IsReadOnlyProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsReadOnlyProperty, value)
        End Set
    End Property
    '
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(Point3DPropertyControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))
    Public Property MaxPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxPropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(Point3DPropertyControl), New UIPropertyMetadata(DefaultMinPropertyWidth))
    Public Property MinPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinPropertyWidthProperty, value)
        End Set
    End Property

    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(Point3DPropertyControl), New UIPropertyMetadata(DefaultPropertyWidth))
    Public Property PropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(PropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(PropertyWidthProperty, value)
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

    '
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(Point3DPropertyControl), New UIPropertyMetadata(True))
    Public Property ShowLeaderLine As Boolean
        Get
            Return DirectCast(GetValue(ShowLeaderLineProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowLeaderLineProperty, value)
        End Set
    End Property

    Public Shared ShowTitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowTitle), GetType(Boolean), GetType(Point3DPropertyControl), New UIPropertyMetadata(True))
    Public Property ShowTitle As Boolean
        Get
            Return DirectCast(GetValue(ShowTitleProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowTitleProperty, value)
        End Set
    End Property


    Private Sub DataPointX_TextChanged(sender As Object, e As TextChangedEventArgs)
        DataPointChanged()
    End Sub

    Private Sub DataPointY_TextChanged(sender As Object, e As TextChangedEventArgs)
        DataPointChanged()
    End Sub

    Private Sub DataPointZ_TextChanged(sender As Object, e As TextChangedEventArgs)
        DataPointChanged()
    End Sub

    Private Sub DataPointChanged()
        If DataPointX.IsValidDouble() = False OrElse DataPointY.IsValidDouble() = False Then Exit Sub
        '
        Dim xValue As Double = DataPointX.GetValueAsDouble()
        Dim yValue As Double = DataPointY.GetValueAsDouble()
        Dim zValue As Double = DataPointZ.GetValueAsDouble()
        '
        DataPoint = New Point3D(xValue, yValue, zValue)
    End Sub
End Class
