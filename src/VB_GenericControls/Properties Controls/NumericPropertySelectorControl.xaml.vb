
Imports System.ComponentModel
Imports System.Globalization

Public Class NumericPropertySelectorControl
    Implements INotifyPropertyChanged

    Public Shared SelectedNumberProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedNumber), GetType(Double), GetType(NumericPropertySelectorControl), New UIPropertyMetadata(CDbl(0)))
    Public Property SelectedNumber As Double
        Get
            Return DirectCast(GetValue(SelectedNumberProperty), Double)
        End Get
        Set(value As Double)
            SetValue(SelectedNumberProperty, value)
        End Set
    End Property
    '
    Public Shared NumericOptionsProperty As DependencyProperty = DependencyProperty.Register(NameOf(NumericOptions), GetType(IList(Of Double)), GetType(NumericPropertySelectorControl), New PropertyMetadata(New List(Of Double)({0, 1, 2, 3, 4, 5})))
    Public Property NumericOptions As IList(Of Double)
        Get
            Return DirectCast(GetValue(NumericOptionsProperty), IList(Of Double))
        End Get
        Set(value As IList(Of Double))
            SetValue(NumericOptionsProperty, value)
        End Set
    End Property

    Public Shared IsEditableProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsEditable), GetType(Boolean), GetType(NumericPropertySelectorControl), New PropertyMetadata(True))
    Public Property IsEditable As Boolean
        Get
            Return DirectCast(GetValue(IsEditableProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsEditableProperty, value)
        End Set
    End Property
    '
    Public Shared CanHaveNegativeProperty As DependencyProperty = DependencyProperty.Register(NameOf(CanHaveNegative), GetType(Boolean), GetType(NumericPropertySelectorControl), New PropertyMetadata(True))
    Public Property CanHaveNegative As Boolean
        Get
            Return DirectCast(GetValue(CanHaveNegativeProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(CanHaveNegativeProperty, value)
        End Set
    End Property
    '
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(NumericPropertySelectorControl), New UIPropertyMetadata("Title"))
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property
    'Public Shared ComboBoxWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(ComboBoxWidth), GetType(Double), GetType(NumericPropertySelectorControl), New UIPropertyMetadata(CDbl(100)))
    'Public Property ComboBoxWidth As Double
    '    Get
    '        Return DirectCast(GetValue(ComboBoxWidthProperty), Double)
    '    End Get
    '    Set(value As Double)
    '        SetValue(ComboBoxWidthProperty, value)
    '    End Set
    'End Property
    '
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(NumericPropertySelectorControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))
    Public Property MaxPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxPropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(NumericPropertySelectorControl), New UIPropertyMetadata(DefaultMinPropertyWidth))
    Public Property MinPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinPropertyWidthProperty, value)
        End Set
    End Property

    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(NumericPropertySelectorControl), New UIPropertyMetadata(DefaultPropertyWidth))
    Public Property PropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(PropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(PropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(NumericPropertySelectorControl), New UIPropertyMetadata(True))
    Public Property ShowLeaderLine As Boolean
        Get
            Return DirectCast(GetValue(ShowLeaderLineProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowLeaderLineProperty, value)
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

    Private Sub ComboBox_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim tBox As ComboBox = DirectCast(sender, ComboBox)
        If Not Char.IsDigit(CChar(e.Text)) Then e.Handled = True 'numeric only
        If e.Text = Chr(8) Then e.Handled = False 'allow Backspace
        If e.Text = " " Then e.Handled = True 'don't allow spaces (note, this doesn't fire for the previewtextinput event so doesn't have an effect here).
        If CanHaveNegative = True Then If e.Text = "-" And tBox.Text.IndexOf("-", StringComparison.Ordinal) = -1 Then e.Handled = False 'allow negative
        If e.Text = "." Then 'allow one decimal
            If tBox.Text.IndexOf(".", StringComparison.Ordinal) = -1 Then e.Handled = False
            'If tBox.SelectedText.IndexOf(".", StringComparison.Ordinal) > -1 Then e.Handled = False
        End If
    End Sub

    Private Sub ComboBox_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Space Then e.Handled = True
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''' <summary>
    ''' The following is a hack job to allow the user to single click into a cell to edit the combobox text value. 
    ''' In some cases (e.g. inside a datagrid cell through templatecolumn or in the avalon dock framework) it seems that
    ''' the mouseup event gets handled elsewhere and never makes it to the textbox control when a combobox is editable.
    ''' </summary>
    Private _previewUp As Boolean = False

    Private Sub NumericPropertySelector_PreviewMouseUp(sender As Object, e As MouseButtonEventArgs)
        If IsEditable = False Then Exit Sub
        _previewUp = True
    End Sub

    Private Sub NumericPropertySelector_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If IsEditable = False Then Exit Sub
        If _previewUp = True Then
            Dim tb As TextBox = DirectCast(NumericComboBox.Template.FindName("PART_EditableTextBox", NumericComboBox), TextBox)
            tb.Focus()
        End If
        _previewUp = False
    End Sub

    'End Hack
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

End Class

Public Class DoubleConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Return value
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Dim d As Double
        If Double.TryParse(value.ToString(), d) = False Then Return Binding.DoNothing
        Return d
    End Function
End Class
