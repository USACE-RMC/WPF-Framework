Public Class ThicknessControl
    Public Shared ThicknessProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedThickness), GetType(Thickness), GetType(ThicknessControl), New UIPropertyMetadata(New Thickness(1)))
    Public Property SelectedThickness As Thickness
        Get
            Return DirectCast(GetValue(ThicknessProperty), Thickness)
        End Get
        Set(value As Thickness)
            SetValue(ThicknessProperty, value)
        End Set
    End Property

    Private Sub TextBox_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim tBox As TextBox = DirectCast(sender, TextBox)
        If Not Char.IsDigit(CChar(e.Text)) Then e.Handled = True 'numeric only
        If e.Text = Chr(8) Then e.Handled = False 'allow Backspace
        If e.Text = " " Then e.Handled = True 'don't allow spaces (note, this doesn't fire for the previewtextinput event so doesn't have an effect here).
        'If CanBeNegative = True Then If e.Text = "-" And SelectionStart = 0 And Text.IndexOf("-", StringComparison.Ordinal) = -1 Then e.Handled = False 'allow negative
        If e.Text = "." Then 'allow one decimal
            If tBox.Text.IndexOf(".", StringComparison.Ordinal) = -1 Then e.Handled = False
            If tBox.SelectedText.IndexOf(".", StringComparison.Ordinal) > -1 Then e.Handled = False
        End If
    End Sub

    Private Sub TextBox_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Space Then e.Handled = True
    End Sub

    Private Sub TextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        Dim tBox As TextBox = DirectCast(sender, TextBox)
        Dim dblValue As Double
        If Double.TryParse(tBox.Text, dblValue) Then
            Select Case tBox.Name
                Case LeftThicknessTextBox.Name
                    SelectedThickness = New Thickness(dblValue, SelectedThickness.Top, SelectedThickness.Right, SelectedThickness.Bottom)
                Case TopThicknessTextBox.Name
                    SelectedThickness = New Thickness(SelectedThickness.Left, dblValue, SelectedThickness.Right, SelectedThickness.Bottom)
                Case RightThicknessTextBox.Name
                    SelectedThickness = New Thickness(SelectedThickness.Left, SelectedThickness.Top, dblValue, SelectedThickness.Bottom)
                Case BottomThicknessTextBox.Name
                    SelectedThickness = New Thickness(SelectedThickness.Left, SelectedThickness.Top, SelectedThickness.Right, dblValue)
            End Select
        End If
    End Sub
End Class
