Imports System.ComponentModel

Public Class NumericTextBox2
    Implements INotifyPropertyChanged


    ''' <summary>
    ''' Dependency property for the number. 
    ''' </summary>
    Public Shared ValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(Value), GetType(Double), GetType(NumericTextBox2), New UIPropertyMetadata(CDbl(0), AddressOf ValueChangedCallback))

    ''' <summary>
    ''' Property Changed Callback for the Number property.
    ''' </summary>
    Private Shared Sub ValueChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(NumericTextBox2) Then Exit Sub
        Dim thisControl = DirectCast(d, NumericTextBox2)
        '
        If IsNothing(e.NewValue) Then Exit Sub
        Dim newNumber As Double
        If Double.TryParse(e.NewValue.ToString, newNumber) = False Then
            If e.NewValue.GetType <> GetType(Double) Then
                thisControl.ValueIsValid = False
                thisControl.ToolTip = $"Number is not valid."
                Exit Sub
            End If
            newNumber = DirectCast(e.NewValue, Double)
        End If
        '
        If (newNumber < thisControl.MinValue) OrElse (newNumber > thisControl.MaxValue) Then
            thisControl.ValueIsValid = False
            thisControl.ToolTip = $"Number must be within range '{thisControl.MinValue}' to '{thisControl.MaxValue}'."
            Exit Sub
        End If
        '
        thisControl.ValueIsValid = True
        thisControl.ToolTip = Nothing '$"Number must be within range '{thisControl.MinValue}' to '{thisControl.MaxValue}'."

    End Sub
    ''' <summary>
    ''' Gets and sets the number. 
    ''' </summary>
    Public Property Value As Double
        Get
            Return DirectCast(GetValue(ValueProperty), Double)
        End Get
        Set(value As Double)
            SetValue(ValueProperty, value)
        End Set
    End Property


    Public Shared IsReadOnlyProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsReadOnly), GetType(Boolean), GetType(NumericTextBox2), New UIPropertyMetadata(False))
    Public Property IsReadOnly As Boolean
        Get
            Return DirectCast(GetValue(IsReadOnlyProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsReadOnlyProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the can have negative property. 
    ''' </summary>
    Public Shared CanHaveNegativeProperty As DependencyProperty = DependencyProperty.Register(NameOf(CanHaveNegative), GetType(Boolean), GetType(NumericTextBox2), New PropertyMetadata(True))

    ''' <summary>
    ''' Determines if the number can be negative. 
    ''' </summary>
    Public Property CanHaveNegative As Boolean
        Get
            Return DirectCast(GetValue(CanHaveNegativeProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(CanHaveNegativeProperty, value)
        End Set
    End Property

    Private _valueIsValid As Boolean = True

    ''' <summary>
    ''' Determines if the value is valid.
    ''' </summary>
    Public Property ValueIsValid As Boolean
        Get
            Return _valueIsValid
        End Get
        Private Set(value As Boolean)
            If _valueIsValid <> value Then
                _valueIsValid = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ValueIsValid)))
            End If
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the is whole number property. 
    ''' </summary>
    Public Shared IsWholeNumberProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsWholeNumber), GetType(Boolean), GetType(NumericTextBox2), New PropertyMetadata(False))

    ''' <summary>
    ''' Gets and sets whether the number must be a whole number. 
    ''' </summary>
    Public Property IsWholeNumber As Boolean
        Get
            Return DirectCast(GetValue(IsWholeNumberProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsWholeNumberProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the max value property. 
    ''' </summary>
    Public Shared MaxValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxValue), GetType(Double), GetType(NumericTextBox2), New FrameworkPropertyMetadata(Double.MaxValue))

    ''' <summary>
    ''' Gets and sets the maximum value allowed. 
    ''' </summary>
    Public Property MaxValue As Double
        Get
            Return DirectCast(GetValue(MaxValueProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxValueProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the min value property. 
    ''' </summary>
    Public Shared MinValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinValue), GetType(Double), GetType(NumericTextBox2), New FrameworkPropertyMetadata(Double.MinValue))

    ''' <summary>
    ''' Gets and sets the minimum value allowed. 
    ''' </summary>
    Public Property MinValue As Double
        Get
            Return DirectCast(GetValue(MinValueProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinValueProperty, value)
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    ''' Textbox preview text input. 
    ''' </summary>
    Private Sub TextBox_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim tBox As TextBox = NumericTBox
        If Not Char.IsDigit(CChar(e.Text)) Then e.Handled = True 'numeric only
        If e.Text = Chr(8) Then e.Handled = False 'allow Backspace
        If e.Text = " " Then e.Handled = True 'don't allow spaces (note, this doesn't fire for the previewtextinput event so doesn't have an effect here).
        If CanHaveNegative = True Then If e.Text = "-" And tBox.SelectionStart = 0 And tBox.Text.IndexOf("-", StringComparison.Ordinal) = -1 Then e.Handled = False 'allow negative
        If IsWholeNumber = False Then
            If e.Text = "." Then 'allow one decimal
                If tBox.Text.IndexOf(".", StringComparison.Ordinal) = -1 Then e.Handled = False
                If tBox.SelectedText.IndexOf(".", StringComparison.Ordinal) > -1 Then e.Handled = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' Textbox preview key down. 
    ''' </summary>
    Private Sub TextBox_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Space Then e.Handled = True
    End Sub

    ''' <summary>
    ''' Textbox preview key up. 
    ''' </summary>
    Private Sub TextBox_PreviewKeyUp(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Enter Then
            Dim tBox As TextBox = NumericTBox
            Dim binding As BindingExpression = BindingOperations.GetBindingExpression(tBox, TextBox.TextProperty)
            If binding IsNot Nothing Then binding.UpdateSource()
        End If
    End Sub

    Public Sub SelectAll()
        If Not NumericTBox.IsKeyboardFocusWithin Then NumericTBox.Focus()
        NumericTBox.SelectAll()
    End Sub

End Class
