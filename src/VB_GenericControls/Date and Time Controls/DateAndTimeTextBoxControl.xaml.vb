Imports System.ComponentModel

Public Class DateAndTimeTextBoxControl
    Implements INotifyPropertyChanged

    Public Shared SelectedDateTimeProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedDateTime), GetType(DateTime), GetType(DateAndTimeTextBoxControl), New UIPropertyMetadata(New DateTime(2017, 8, 7, 20, 35, 23), AddressOf DateChangedCallback))

    Private Shared Sub DateChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(DateAndTimeTextBoxControl) Then Exit Sub
        Dim thisControl = DirectCast(d, DateAndTimeTextBoxControl)
        '
        thisControl.OnDateTimeChanged()
    End Sub

    Public Property SelectedDateTime As DateTime
        Get
            Return CType(GetValue(SelectedDateTimeProperty), DateTime)
        End Get
        Set(value As DateTime)
            SetValue(SelectedDateTimeProperty, value)
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Sub OnDateTimeChanged()
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SelectedDateTime)))
    End Sub

    Public Shared Is24HourProperty As DependencyProperty = DependencyProperty.Register(NameOf(Is24Hour), GetType(Boolean), GetType(DateAndTimeTextBoxControl), New FrameworkPropertyMetadata(False))

    Public Property Is24Hour As Boolean
        Get
            Return DirectCast(GetValue(Is24HourProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(Is24HourProperty, value)
        End Set
    End Property

    Private Sub Textbox_KeyUp(sender As Object, e As KeyEventArgs) Handles DateTimeTextBox.KeyUp
        If e.Key = Key.Enter Then
            Dim tBox As TextBox = CType(sender, TextBox)
            Dim prop As DependencyProperty = TextBox.TextProperty
            Dim binding As BindingExpression = BindingOperations.GetBindingExpression(tBox, prop)
            If binding IsNot Nothing Then binding.UpdateSource()
        End If
    End Sub
End Class
