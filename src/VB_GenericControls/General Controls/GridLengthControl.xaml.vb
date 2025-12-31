Public Class GridLengthControl
    Public Shared ReadOnly Property GridLengthUnitOptions As New List(Of GridUnitType)(DirectCast([Enum].GetValues(GetType(GridUnitType)), GridUnitType()))


    Public Shared GridLengthProperty As DependencyProperty = DependencyProperty.Register(NameOf(GridLength), GetType(GridLength), GetType(GridLengthControl), New FrameworkPropertyMetadata(GridLength.Auto, AddressOf GridLengthPropertyCallback))
    Private Shared Sub GridLengthPropertyCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(GridLengthControl) Then Exit Sub
        Dim thisControl = DirectCast(d, GridLengthControl)
        '
        If IsNothing(e.NewValue) Then Exit Sub
        If e.NewValue.GetType <> GetType(GridLength) Then Exit Sub
        Dim newGridLength As GridLength = DirectCast(e.NewValue, GridLength)
        If IsNothing(newGridLength.Value) OrElse IsNothing(newGridLength.GridUnitType) Then
            thisControl.GridLengthValue = 100
            thisControl.GridLengthUnit = GridUnitType.Auto
            Exit Sub
        End If
        'If IsNothing(thisControl.GridLength) Then
        If IsNothing(newGridLength.Value) OrElse IsNothing(newGridLength.GridUnitType) Then
                thisControl.GridLengthValue = 100
                thisControl.GridLengthUnit = GridUnitType.Auto
            Else
                If thisControl.GridLengthValue <> newGridLength.Value Then thisControl.GridLengthValue = newGridLength.Value
                If thisControl.GridLengthUnit <> newGridLength.GridUnitType Then thisControl.GridLengthUnit = newGridLength.GridUnitType
            End If
        'End If
    End Sub
    Public Property GridLength As GridLength
        Get
            Return CType(GetValue(GridLengthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(GridLengthProperty, value)
        End Set
    End Property

    Public Shared GridLengthValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(GridLengthValue), GetType(Double), GetType(GridLengthControl), New FrameworkPropertyMetadata(CDbl(100), AddressOf GridLengthValuePropertyCallback))
    Private Shared Sub GridLengthValuePropertyCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(GridLengthControl) Then Exit Sub
        Dim thisControl = DirectCast(d, GridLengthControl)
        '
        thisControl.UpdateGridLengthProperty()
    End Sub
    Public Property GridLengthValue As Double
        Get
            Return CType(GetValue(GridLengthValueProperty), Double)
        End Get
        Set(value As Double)
            SetValue(GridLengthValueProperty, value)
        End Set
    End Property

    Public Shared GridLengthUnitProperty As DependencyProperty = DependencyProperty.Register(NameOf(GridLengthUnit), GetType(GridUnitType), GetType(GridLengthControl), New FrameworkPropertyMetadata(GridUnitType.Auto, AddressOf GridLengthUnitPropertyCallback))
    Private Shared Sub GridLengthUnitPropertyCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(GridLengthControl) Then Exit Sub
        Dim thisControl = DirectCast(d, GridLengthControl)
        '
        thisControl.UpdateGridLengthProperty()
    End Sub
    Public Property GridLengthUnit As GridUnitType
        Get
            Return CType(GetValue(GridLengthUnitProperty), GridUnitType)
        End Get
        Set(value As GridUnitType)
            SetValue(GridLengthUnitProperty, value)
        End Set
    End Property

    Private Sub UpdateGridLengthProperty()
        If IsNothing(GridLengthValue) OrElse IsNothing(GridLengthUnit) Then Exit Sub
        'Refresh the gridlength
        GridLength = New GridLength(GridLengthValue, GridLengthUnit)
    End Sub

    Private Sub TextBox_LostFocus(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
