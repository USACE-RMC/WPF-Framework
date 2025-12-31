Public Class NumericTextBox
    Public Property CanBeBlank As Boolean
    Public Property CanBeNegative As Boolean
    Public Property IsWholeNumber As Boolean
    Public Property MaxValue As Double = Double.MaxValue
    Public Property MinValue As Double = Double.MinValue


    Public Shared IsReadOnlyProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsReadOnly), GetType(Boolean), GetType(NumericTextBox), New UIPropertyMetadata(False))
    Public Property IsReadOnly As Boolean
        Get
            Return DirectCast(GetValue(IsReadOnlyProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsReadOnlyProperty, value)
        End Set
    End Property

    Public Shared ReadOnly AboveMaxValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(AboveMaxValue), GetType(Boolean), GetType(NumericTextBox), New FrameworkPropertyMetadata(False))
    Public Property AboveMaxValue As Boolean
        Get
            Return CBool(GetValue(AboveMaxValueProperty))
        End Get
        Private Set(value As Boolean)
            SetValue(AboveMaxValueProperty, value)
        End Set
    End Property
    Public Shared ReadOnly BelowMinValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(BelowMinValue), GetType(Boolean), GetType(NumericTextBox), New FrameworkPropertyMetadata(False))

    Public Property BelowMinValue As Boolean
        Get
            Return CBool(GetValue(BelowMinValueProperty))
        End Get
        Private Set(value As Boolean)
            SetValue(BelowMinValueProperty, value)
        End Set
    End Property
    Public Shared ReadOnly ValueIsValidProperty As DependencyProperty = DependencyProperty.Register(NameOf(ValueIsValid), GetType(Boolean), GetType(NumericTextBox), New FrameworkPropertyMetadata(True))

    Public Property ValueIsValid As Boolean
        Get
            Return CBool(GetValue(ValueIsValidProperty))
        End Get
        Private Set(value As Boolean)
            SetValue(ValueIsValidProperty, value)
        End Set
    End Property
    Public Shared ReadOnly InvalidTextProperty As DependencyProperty = DependencyProperty.Register(NameOf(InvalidText), GetType(Boolean), GetType(NumericTextBox), New FrameworkPropertyMetadata(False))

    Public Property InvalidText As Boolean
        Get
            Return CBool(GetValue(InvalidTextProperty))
        End Get
        Private Set(value As Boolean)
            SetValue(InvalidTextProperty, value)
        End Set
    End Property
    Public Shared TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(NumericTextBox), New UIPropertyMetadata(""))
    Public Property Text As String
        Get
            Return CType(GetValue(TextProperty), String)
        End Get
        Set(value As String)
            SetValue(TextProperty, value)
        End Set
    End Property

    Public Event TextChanged(sender As Object, e As TextChangedEventArgs)

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'AddHandler Me.GotFocus, Sub()
        '                            NumericTBox.Focus()
        '                        End Sub
    End Sub
    Public Sub SetFocus()
        NumericTBox.Focus()
    End Sub
    Private Sub TextBox_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Space Then e.Handled = True
    End Sub

    Private Sub TextBox_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        If Not Char.IsDigit(CChar(e.Text)) Then e.Handled = True 'numeric only
        If e.Text = Chr(8) Then e.Handled = False 'allow Backspace
        If e.Text = " " Then e.Handled = True 'don't allow spaces (note, this doesn't fire for the previewtextinput event so doesn't have an effect here).
        If CanBeNegative = True Then If e.Text = "-" And NumericTBox.SelectionStart = 0 And NumericTBox.Text.IndexOf("-", StringComparison.Ordinal) = -1 Then e.Handled = False 'allow negative
        If IsWholeNumber = False And e.Text = "." Then 'allow one decimal
            If NumericTBox.Text.IndexOf(".", StringComparison.Ordinal) = -1 Then e.Handled = False
            If NumericTBox.SelectedText.IndexOf(".", StringComparison.Ordinal) > -1 Then e.Handled = False
        End If
        'If e.KeyChar = Chr(13) Then OK.Focus() 'enter to move to next
    End Sub

    Private Sub TextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        Dim doubleValue As Double
        If Double.TryParse(NumericTBox.Text, doubleValue) = False Then
            If NumericTBox.Text = "" And CanBeBlank = True Then
                ValueIsValid = True
                InvalidText = False
            Else
                InvalidText = True
                ValueIsValid = False
            End If
        Else
            InvalidText = False
            AboveMaxValue = doubleValue > MaxValue
            BelowMinValue = doubleValue < MinValue
            ValueIsValid = Not (AboveMaxValue = True Or BelowMinValue = True)
        End If
        RaiseEvent TextChanged(sender, e)
    End Sub
    Public Sub SelectAll()
        NumericTBox.SelectAll()
    End Sub

    Public Function IsValidDouble() As Boolean
        Dim doubleValue As Double
        Return Double.TryParse(NumericTBox.Text, doubleValue)
    End Function
    Public Function IsValidSingle() As Boolean
        Dim singleValue As Single
        Return Single.TryParse(NumericTBox.Text, singleValue)
    End Function
    Public Function IsValidInteger() As Boolean
        If IsValidDouble() = False Then Return False
        Dim dblValue As Double = GetValueAsDouble()

        If dblValue > Integer.MaxValue Or dblValue < Integer.MinValue Then Return False
        '
        Return True
    End Function
    Public Function GetValueAsDouble() As Double
        Dim doubleValue As Double
        Double.TryParse(NumericTBox.Text, doubleValue)
        '
        Return doubleValue
    End Function
    Public Function GetValueAsSingle() As Single
        Dim singleValue As Single
        Single.TryParse(NumericTBox.Text, singleValue)
        '
        Return singleValue
    End Function
    Public Function GetValueAsInteger() As Int32
        Dim dblValue As Double = GetValueAsDouble()

        If dblValue > Integer.MaxValue Then Return Integer.MaxValue
        If dblValue < Integer.MinValue Then Return Integer.MinValue
        '
        Return CInt(dblValue)
    End Function

    Private Sub NumericTBox_KeyUp(sender As Object, e As KeyEventArgs) Handles NumericTBox.KeyUp
        If e.Key = Key.Enter Then
            Dim tBox As TextBox = CType(sender, TextBox)
            Dim prop As DependencyProperty = TextBox.TextProperty
            Dim binding As BindingExpression = BindingOperations.GetBindingExpression(tBox, prop)
            If binding IsNot Nothing Then
                binding.UpdateSource()
            End If
        End If
    End Sub

End Class
