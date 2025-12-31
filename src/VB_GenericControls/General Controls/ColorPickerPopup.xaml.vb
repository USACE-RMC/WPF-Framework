Imports System.Windows.Controls.Primitives

Public Class ColorPickerPopup
    Public Shared ColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(Color), GetType(SolidColorBrush), GetType(ColorPickerPopup), New UIPropertyMetadata(Brushes.Black))
    Public Property Color As SolidColorBrush
        Get
            Return DirectCast(GetValue(ColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(ColorProperty, value)
        End Set
    End Property

    'Private Sub ColorRectangle_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
    '    PickerPopup.IsOpen = True
    'End Sub
End Class
