Public Class DragAdorner
    Inherits Adorner

    Private _vBrush As Brush
    Private _location As Point
    'Private _offset As Double
    Public Sub New(adornedElement As UIElement, offset As Point)

        MyBase.New(adornedElement)
        IsHitTestVisible = False
        Focusable = False
        _vBrush = New VisualBrush(MyBase.AdornedElement) With {.Stretch = Stretch.None, .AlignmentX = AlignmentX.Left}
        _vBrush.Opacity = 0.8
    End Sub

    Public Sub UpdatePosition(location As Point)
        _location = New Point(location.X, location.Y - 17.5)
        InvalidateVisual()
    End Sub

    Protected Overrides Sub OnRender(dc As DrawingContext)
        dc.PushOpacityMask(New LinearGradientBrush(Colors.White, Colors.Transparent, 45))
        dc.DrawRectangle(_vBrush, Nothing, New Rect(_location.X, _location.Y, Math.Min(RenderSize.Width, 500), Math.Min(RenderSize.Height, 400)))
    End Sub
    'Private Sub ForceUpdate(element As FrameworkElement)
    '    Dim s As New Size(AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height)
    '    element.Measure(s)
    '    element.Arrange(New Rect(0, 0, AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height))
    '    element.UpdateLayout()
    'End Sub
End Class
