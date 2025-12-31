Public Class ResizableTextBoxControl
    Public Shared TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(ResizableTextBoxControl), New UIPropertyMetadata(""))
    Public Property Text As String
        Get
            Return CType(GetValue(TextProperty), String)
        End Get
        Set(value As String)
            SetValue(TextProperty, value)
        End Set
    End Property
    '
    Public Shared IsReadOnlyProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsReadOnly), GetType(Boolean), GetType(ResizableTextBoxControl), New UIPropertyMetadata(False))
    Public Property IsReadOnly As Boolean
        Get
            Return DirectCast(GetValue(IsReadOnlyProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsReadOnlyProperty, value)
        End Set
    End Property

    'Private _isResizing As Boolean = False
    'Private _startPosition As Point

    'Private Sub ResizeGripper_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
    '    If Mouse.Capture(ResizeGripper) Then
    '        _isResizing = True
    '        _startPosition = e.GetPosition(Me)
    '    End If
    'End Sub

    'Private Sub ResizeGripper_MouseMove(sender As Object, e As MouseEventArgs)
    '    If _isResizing Then
    '        Dim currentPosition As Point = e.GetPosition(Me)
    '        Dim diffY As Double = currentPosition.Y - _startPosition.Y
    '        Height += diffY
    '        _startPosition = currentPosition
    '    End If
    'End Sub

    'Private Sub ResizeGripper_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
    '    If _isResizing = True Then
    '        _isResizing = False
    '    End If
    'End Sub

    Private Sub ResizeThumb_DragDelta(sender As Object, e As Primitives.DragDeltaEventArgs)
        Dim newHeight As Double = ActualHeight + e.VerticalChange
        If newHeight < 18 Then newHeight = 18
        If newHeight > MaxHeight Then newHeight = MaxHeight
        Height = newHeight
    End Sub

    Private Sub TextBox_PreviewKeyUp(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Enter Then
            Dim tBox As TextBox = DirectCast(sender, TextBox)
            Dim binding As BindingExpression = BindingOperations.GetBindingExpression(tBox, TextBox.TextProperty)
            If binding IsNot Nothing Then binding.UpdateSource()
        End If
    End Sub
End Class
