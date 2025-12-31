Imports System.ComponentModel
Imports System.Globalization

Public Class NumericAutoPropertyControl
    Implements INotifyPropertyChanged

#Region "Members"

    ''' <summary>
    ''' Dependency property for the number. 
    ''' </summary>
    Public Shared NumberProperty As DependencyProperty = DependencyProperty.Register(NameOf(Number), GetType(Double), GetType(NumericAutoPropertyControl), New UIPropertyMetadata(CDbl(0), AddressOf NumberChangedCallback))

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
        If d.GetType <> GetType(NumericAutoPropertyControl) Then Exit Sub
        Dim thisControl = DirectCast(d, NumericAutoPropertyControl)
        '
        If IsNothing(e.NewValue) Then Exit Sub
        thisControl._cancelChange = False
        thisControl.RaisePreviewEvent(e.OldValue, e.NewValue, thisControl._cancelChange)
        If thisControl._cancelChange = True Then Exit Sub
        '
        If e.NewValue.GetType <> GetType(Double) Then
            thisControl.ValueIsValid = False
            thisControl.ToolTip = $"Number is not valid."
            Exit Sub
        End If
        Dim newNumber As Double = DirectCast(e.NewValue, Double)
        '
        If Double.IsNaN(newNumber) OrElse Double.IsNaN(thisControl.DefaultNumber) Then
            thisControl.NumberIsDefault = (Double.IsNaN(newNumber) AndAlso Double.IsNaN(thisControl.DefaultNumber))
        Else
            thisControl.NumberIsDefault = Math.Abs(newNumber - thisControl.DefaultNumber) < 2 ^ (-53)
        End If
        '
        If (newNumber < thisControl.MinValue) OrElse (newNumber > thisControl.MaxValue) Then
            thisControl.ValueIsValid = False
            thisControl.ToolTip = $"Number must be within range '{thisControl.MinValue}' to '{thisControl.MaxValue}'."
            Exit Sub
        End If


        thisControl.ValueIsValid = True
        thisControl.ToolTip = Nothing
        '
        thisControl.RaisePropertyChanged(NameOf(Number))
    End Sub

    Private Sub RaisePreviewEvent(oldValue As Object, newValue As Object, ByRef cancel As Boolean)
        RaiseEvent PreviewNumberChanged(oldValue, newValue, cancel)
    End Sub


    Private Shared ReadOnly NumberIsDefaultPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(NameOf(NumberIsDefault), GetType(Boolean), GetType(NumericAutoPropertyControl), New FrameworkPropertyMetadata(True))
    Public Shared ReadOnly NumberIsDefaultProperty As DependencyProperty = NumberIsDefaultPropertyKey.DependencyProperty

    ''' <summary>
    ''' Determines if the number is the default. 
    ''' </summary>
    Public Property NumberIsDefault As Boolean
        Get
            Return CBool(GetValue(NumberIsDefaultProperty))
        End Get
        Protected Set(ByVal value As Boolean)
            SetValue(NumberIsDefaultPropertyKey, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the default number. 
    ''' </summary>
    Public Shared DefaultNumberProperty As DependencyProperty = DependencyProperty.Register(NameOf(DefaultNumber), GetType(Double), GetType(NumericAutoPropertyControl), New UIPropertyMetadata(CDbl(0), AddressOf DefaultNumberChangedCallback))

    Private Shared Sub DefaultNumberChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(NumericAutoPropertyControl) Then Exit Sub
        Dim thisControl = DirectCast(d, NumericAutoPropertyControl)
        '
        If IsNothing(e.NewValue) Then Exit Sub
        If e.NewValue.GetType <> GetType(Double) Then Exit Sub
        Dim newNumber As Double = DirectCast(e.NewValue, Double)
        '
        If Double.IsNaN(newNumber) OrElse Double.IsNaN(thisControl.Number) Then
            thisControl.NumberIsDefault = (Double.IsNaN(newNumber) AndAlso Double.IsNaN(thisControl.Number))
        Else
            thisControl.NumberIsDefault = Math.Abs(newNumber - thisControl.Number) < 2 ^ (-53)
        End If
        '
        thisControl.RaisePropertyChanged(NameOf(DefaultNumber))
    End Sub



    ''' <summary>
    ''' Gets and sets the default number.
    ''' </summary>
    Public Property DefaultNumber As Double
        Get
            Return DirectCast(GetValue(DefaultNumberProperty), Double)
        End Get
        Set(value As Double)
            SetValue(DefaultNumberProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the can have negative property. 
    ''' </summary>
    Public Shared CanHaveNegativeProperty As DependencyProperty = DependencyProperty.Register(NameOf(CanHaveNegative), GetType(Boolean), GetType(NumericAutoPropertyControl), New PropertyMetadata(True))

    ''' <summary>
    ''' Dependency property for the max value property. 
    ''' </summary>
    Public Shared MaxValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxValue), GetType(Double), GetType(NumericAutoPropertyControl), New FrameworkPropertyMetadata(Double.MaxValue))

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
    Public Shared MinValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinValue), GetType(Double), GetType(NumericAutoPropertyControl), New FrameworkPropertyMetadata(Double.MinValue))

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

    Public Event PreviewNumberChanged(oldValue As Object, newValue As Object, ByRef cancel As Boolean)
    Private _cancelChange As Boolean = False

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
    ''' Dependency property for the allow text entry property.
    ''' </summary>
    Public Shared AllowTextEntryProperty As DependencyProperty = DependencyProperty.Register(NameOf(AllowTextEntry), GetType(Boolean), GetType(NumericAutoPropertyControl), New PropertyMetadata(True))

    ''' <summary>
    ''' Determines if text editing is allowed. 
    ''' </summary>
    Public Property AllowTextEntry As Boolean
        Get
            Return DirectCast(GetValue(AllowTextEntryProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(AllowTextEntryProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the title property. 
    ''' </summary>
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(NumericAutoPropertyControl), New UIPropertyMetadata("Title"))

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
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(NumericAutoPropertyControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))

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
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(NumericAutoPropertyControl), New UIPropertyMetadata(DefaultMinPropertyWidth))

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
    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(NumericAutoPropertyControl), New UIPropertyMetadata(DefaultPropertyWidth))

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

    ''' <summary>
    ''' Dependency property for the show leader line property. 
    ''' </summary>
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(NumericAutoPropertyControl), New UIPropertyMetadata(True))

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
    ''' Update the actual property width with the control size changes. 
    ''' </summary>
    Private Sub ControlSizeChanged(sender As Object, e As SizeChangedEventArgs)
        Dim el = TryCast(sender, FrameworkElement)
        ActualPropertyWidth = el.ActualWidth
    End Sub

    ''' <summary>
    ''' Text box preview text input. 
    ''' </summary>
    Private Sub TextBox_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        If AllowTextEntry = False Then
            Dim tBox As TextBox = DirectCast(sender, TextBox)
            If Not Char.IsDigit(CChar(e.Text)) Then e.Handled = True 'numeric only
            If e.Text = Chr(8) Then e.Handled = False 'allow Backspace
            If e.Text = " " Then e.Handled = True 'don't allow spaces (note, this doesn't fire for the preview text input event so doesn't have an effect here).
            If CanHaveNegative = True Then If e.Text = "-" And tBox.SelectionStart = 0 And tBox.Text.IndexOf("-", StringComparison.Ordinal) = -1 Then e.Handled = False 'allow negative
            If e.Text = "." Then 'allow one decimal
                If tBox.Text.IndexOf(".", StringComparison.Ordinal) = -1 Then e.Handled = False
                If tBox.SelectedText.IndexOf(".", StringComparison.Ordinal) > -1 Then e.Handled = False
            End If
        End If
    End Sub

    ''' <summary>
    ''' Text box preview key down. 
    ''' </summary>
    Private Sub TextBox_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Space Then e.Handled = True
    End Sub

    ''' <summary>
    ''' Text box preview key up. 
    ''' </summary>
    Private Sub TextBox_PreviewKeyUp(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Enter Then
            Dim elementWithFocus = CType(Keyboard.FocusedElement, UIElement)
            If Not IsNothing(elementWithFocus) Then
                elementWithFocus.MoveFocus(New TraversalRequest(FocusNavigationDirection.Right))
                elementWithFocus.Focus()
            End If
        End If
    End Sub

    ''' <summary>
    ''' When the text box loses focus, update the bound property. 
    ''' </summary>
    Private Sub TextBox_LostFocus(sender As Object, e As RoutedEventArgs)
        Dim txtBox As TextBox = DirectCast(sender, TextBox)
        Dim newValue As Double
        Dim be = BindingOperations.GetBindingExpressionBase(txtBox, TextBox.TextProperty)
        If Double.TryParse(txtBox.Text, newValue) Then
            be.UpdateSource()
        Else
            be.UpdateTarget()
        End If
    End Sub

    ''' <summary>
    ''' When the reset button is clicked, set the number to the default number. 
    ''' </summary>
    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Number = DefaultNumber
    End Sub

#End Region

End Class
