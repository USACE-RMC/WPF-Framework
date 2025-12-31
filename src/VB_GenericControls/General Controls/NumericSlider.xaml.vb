''' <summary>
''' A numeric slider control.
''' </summary>
''' <remarks>
''' <para>
'''     Authors:
'''     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil 
''' </para>
''' <para>
''' Versions:
'''     <list type="bullet">
'''         <item><description>
'''         Created in February 2020 by Haden Smith.
'''         </description></item>
'''     </list>
''' </para>
''' </remarks>
Public Class NumericSlider

#Region "Construction"

    ''' <summary>
    ''' Construct new numeric up-down control.
    ''' </summary>
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.
        ' set the default values
        Minimum = CDbl(0)
        Maximum = CDbl(100)
        Value = CDbl(0.0)
        Increment = 1
        DecimalPlaces = 0
    End Sub

#End Region

#Region "Members"

    Private _value As Double
    Private _max As Double
    Private _min As Double
    Private _decimalPlaces As Integer
    Private _increment As Double = 1
    Private _thousandsSeperator As Boolean
    Private FormatString As String

    ''' <summary>
    ''' Dependency property for setting the numeric text box value.
    ''' </summary>
    Public Shared ValueProperty As DependencyProperty = DependencyProperty.Register(NameOf(Value), GetType(Double), GetType(NumericSlider), New PropertyMetadata(CDbl(0.0), AddressOf SetText))

    ''' <summary>
    ''' Gets and sets the current value of the numeric slider control.
    ''' </summary>
    Public Property Value As Double
        Get
            Return CType(GetValue(ValueProperty), Double)
        End Get
        Set(newValue As Double)
            SetValue(ValueProperty, newValue)
        End Set
    End Property

    ''' <summary>
    ''' Set the text after the value has changed. 
    ''' </summary>
    Private Shared Sub SetText(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        Dim thisControl = DirectCast(d, NumericSlider)
        If thisControl.ThousandsSeperator = True Then
            thisControl.FormatString = "N" & thisControl.DecimalPlaces.ToString
        Else
            thisControl.FormatString = "F" & thisControl.DecimalPlaces.ToString
        End If
        If CType(e.NewValue, Double) > thisControl.Maximum Then
            thisControl.NumericTextBox.Text = thisControl.Maximum.ToString(thisControl.FormatString)
        ElseIf CType(e.NewValue, Double) < thisControl.Minimum Then
            thisControl.NumericTextBox.Text = thisControl.Minimum.ToString(thisControl.FormatString)
        Else
            thisControl.NumericTextBox.Text = CType(e.NewValue, Double).ToString(thisControl.FormatString)
        End If
    End Sub

    ''' <summary>
    ''' Dependency property for setting the Maximum value.
    ''' </summary>
    Public Shared MaximumProperty As DependencyProperty = DependencyProperty.Register(NameOf(Maximum), GetType(Double), GetType(NumericSlider), New PropertyMetadata(CDbl(100)))


    ''' <summary>
    ''' Gets and sets the maximum value for the numeric slider control.
    ''' </summary>
    Public Property Maximum As Double
        Get
            Return CType(GetValue(MaximumProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaximumProperty, value)
            NumericTextBox.MaxValue = value
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for setting the Minimum value.
    ''' </summary>
    Public Shared MinimumProperty As DependencyProperty = DependencyProperty.Register(NameOf(Minimum), GetType(Double), GetType(NumericSlider), New PropertyMetadata(CDbl(0)))


    ''' <summary>
    ''' Get and set the minimum value for the numeric slider control.
    ''' </summary>
    Public Property Minimum As Double
        Get
            Return CType(GetValue(MinimumProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinimumProperty, value)
            NumericTextBox.MinValue = value
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for setting the Increment value.
    ''' </summary>
    Public Shared IncrementProperty As DependencyProperty = DependencyProperty.Register(NameOf(Increment), GetType(Double), GetType(NumericSlider), New PropertyMetadata(CDbl(1)))

    ''' <summary>
    ''' Gets and sets the amount to increment and decrement on each button click.
    ''' </summary>
    Public Property Increment As Double
        Get
            Return CType(GetValue(IncrementProperty), Double)
        End Get
        Set(value As Double)
            SetValue(IncrementProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Gets and sets the number of decimal places to display.
    ''' </summary>
    Public Property DecimalPlaces As Integer
        Get
            Return _decimalPlaces
        End Get
        Set(value As Integer)
            If value < 0 OrElse value > 99 Then
                Throw New ArgumentOutOfRangeException("DecimalPlaces", "Value of '" & value.ToString & "' is not valid. 'DecimalPlaces' should be between 0 and 99.")
            End If
            _decimalPlaces = value
        End Set
    End Property

    ''' <summary>
    ''' Gets and sets whether the thousands seperator will be displayed.
    ''' </summary>
    Public Property ThousandsSeperator As Boolean
        Get
            Return _thousandsSeperator
        End Get
        Set(value As Boolean)
            _thousandsSeperator = value
        End Set
    End Property

    Public Event ValueChanged(value As Double)

#End Region

#Region "Methods"

    ''' <summary>
    ''' On click, increment up.
    ''' </summary>
    Private Sub cmdUp_Click(sender As Object, e As RoutedEventArgs)
        If Value < Maximum Then
            Value += Increment
        End If
    End Sub

    ''' <summary>
    ''' On click, increment down.
    ''' </summary>
    Private Sub cmdDown_Click(sender As Object, e As RoutedEventArgs)
        If Value > Minimum Then
            Value -= Increment
        End If
    End Sub

    Private Sub Slider_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles Slider.ValueChanged
        RaiseEvent ValueChanged(Value)
    End Sub

#End Region

End Class
