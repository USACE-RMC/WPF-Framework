Imports System.ComponentModel

Public Class NumericSliderPropertyControl
    Implements INotifyPropertyChanged

#Region "Members"

    ''' <summary>
    ''' Dependency property for the number. 
    ''' </summary>
    Public Shared NumberProperty As DependencyProperty = DependencyProperty.Register(NameOf(Number), GetType(Double), GetType(NumericSliderPropertyControl), New UIPropertyMetadata(CDbl(0), AddressOf NumberChangedCallback))

    ''' <summary>
    ''' Gets and sets the number. 
    ''' </summary>
    Public Property Number As Double
        Get
            Return DirectCast(GetValue(NumberProperty), Double)
        End Get
        Set(value As Double)
            SetValue(NumberProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Property Changed Callback for the Number property.
    ''' </summary>
    Private Shared Sub NumberChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(NumericSliderPropertyControl) Then Exit Sub
        Dim thisControl = DirectCast(d, NumericSliderPropertyControl)
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
        'If Double.IsNaN(newNumber) OrElse Double.IsNaN(thisControl.DefaultNumber) Then
        '    thisControl.NumberIsDefault = (Double.IsNaN(newNumber) AndAlso Double.IsNaN(thisControl.DefaultNumber))
        'Else
        '    thisControl.NumberIsDefault = (newNumber = thisControl.DefaultNumber)
        'End If
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
    ''' Dependency property for the can have negative property. 
    ''' </summary>
    Public Shared CanHaveNegativeProperty As DependencyProperty = DependencyProperty.Register(NameOf(CanHaveNegative), GetType(Boolean), GetType(NumericSliderPropertyControl), New PropertyMetadata(True))

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

    ''' <summary>
    ''' Dependency property for the is text box enabled property. 
    ''' </summary>
    Public Shared IsTextBoxEnabledProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsTextBoxEnabled), GetType(Boolean), GetType(NumericSliderPropertyControl), New UIPropertyMetadata(True))

    ''' <summary>
    ''' Gets and sets whether the text box is enabled. 
    ''' </summary>
    Public Property IsTextBoxEnabled As Boolean
        Get
            Return DirectCast(GetValue(IsTextBoxEnabledProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsTextBoxEnabledProperty, value)
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
                RaisePropertyChanged(NameOf(ValueIsValid))
            End If
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the is whole number property. 
    ''' </summary>
    Public Shared IsWholeNumberProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsWholeNumber), GetType(Boolean), GetType(NumericSliderPropertyControl), New PropertyMetadata(False))

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
    Public Shared MaxValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxValue), GetType(Double), GetType(NumericSliderPropertyControl), New FrameworkPropertyMetadata(Double.MaxValue))

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
    Public Shared MinValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinValue), GetType(Double), GetType(NumericSliderPropertyControl), New FrameworkPropertyMetadata(Double.MinValue))

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

    ''' <summary>
    ''' Dependency property for the min value property. 
    ''' </summary>
    Public Shared IncrementProperty As DependencyProperty = DependencyProperty.Register(NameOf(Increment), GetType(Double), GetType(NumericSliderPropertyControl), New FrameworkPropertyMetadata(CDbl(1)))

    ''' <summary>
    ''' Gets and sets the minimum value allowed. 
    ''' </summary>
    Public Property Increment As Double
        Get
            Return DirectCast(GetValue(IncrementProperty), Double)
        End Get
        Set(value As Double)
            SetValue(IncrementProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the title property. 
    ''' </summary>
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(NumericSliderPropertyControl), New UIPropertyMetadata("Title"))

    ''' <summary>
    ''' Gets and sets the title of the property. 
    ''' </summary>
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property

#Region "Control Width"

    ''' <summary>
    ''' Dependency property for the max property width. 
    ''' </summary>
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(NumericSliderPropertyControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))

    ''' <summary>
    ''' Gets and sets the maximum property width. 
    ''' </summary>
    Public Property MaxPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxPropertyWidthProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the min property width. 
    ''' </summary>
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(NumericSliderPropertyControl), New UIPropertyMetadata(DefaultMinPropertyWidth))

    ''' <summary>
    ''' Gets and sets the minimum property width. 
    ''' </summary>
    Public Property MinPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinPropertyWidthProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the property width. 
    ''' </summary>
    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(NumericSliderPropertyControl), New UIPropertyMetadata(DefaultPropertyWidth))

    ''' <summary>
    ''' Gets and sets the property width. 
    ''' </summary>
    Public Property PropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(PropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(PropertyWidthProperty, value)
        End Set
    End Property

    Public Shared TextPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(TextPropertyWidth), GetType(GridLength), GetType(NumericSliderPropertyControl), New UIPropertyMetadata(DefaultPropertyWidth))

    ''' <summary>
    ''' Gets and sets the property width. 
    ''' </summary>
    Public Property TextPropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(TextPropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(TextPropertyWidthProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the show leader line property. 
    ''' </summary>
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(NumericSliderPropertyControl), New UIPropertyMetadata(True))

    ''' <summary>
    ''' Gets and sets whether to show the leader line. 
    ''' </summary>
    Public Property ShowLeaderLine As Boolean
        Get
            Return DirectCast(GetValue(ShowLeaderLineProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowLeaderLineProperty, value)
        End Set
    End Property

    Private _actualWidth As Double = 0

    ''' <summary>
    ''' Gets and sets the actual property width. 
    ''' </summary>
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

#End Region

    ''' <summary>
    ''' The property changed event. 
    ''' </summary>
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

#End Region

#Region "Methods"

    ''' <summary>
    ''' Raise the property changed event. 
    ''' </summary>
    ''' <param name="propertyName">The name of the property to change. </param>
    Private Sub RaisePropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    ''' <summary>
    ''' Update the actual propert width with the control size changes. 
    ''' </summary>
    Private Sub ControlSizeChanged(sender As Object, e As SizeChangedEventArgs)
        Dim el = TryCast(sender, FrameworkElement)
        ActualPropertyWidth = el.ActualWidth
    End Sub

    ''' <summary>
    ''' Textbox preview text input. 
    ''' </summary>
    Private Sub TextBox_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim tBox As TextBox = DirectCast(sender, TextBox)
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
            Dim tBox As TextBox = DirectCast(sender, TextBox)
            Dim binding As BindingExpression = BindingOperations.GetBindingExpression(tBox, TextBox.TextProperty)
            If binding IsNot Nothing Then binding.UpdateSource()
        End If
    End Sub

#End Region
End Class
