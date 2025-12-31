Public Class ComboBoxItemTemplateSelector
    Inherits DataTemplateSelector

    Public Property DropDownTemplate As DataTemplate
    Public Property SelectedTemplate As DataTemplate

    Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) As DataTemplate
        Dim comboBoxItem As ComboBoxItem = GetVisualParent(Of ComboBoxItem)(container)

        If comboBoxItem IsNot Nothing Then
            Return DropDownTemplate
        End If

        Return SelectedTemplate
    End Function

    Public Shared Function GetVisualParent(Of T As Visual)(ByVal childObject As Object) As T
        Dim child As DependencyObject = TryCast(childObject, DependencyObject)

        While (child IsNot Nothing) AndAlso Not (TypeOf child Is T)
            child = VisualTreeHelper.GetParent(child)
        End While

        Return TryCast(child, T)
    End Function
End Class
