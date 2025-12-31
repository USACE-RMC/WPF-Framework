Imports System.Globalization


Public Class RangeWrapper
    Inherits DependencyObject

    Public Shared ReadOnly MaximumProperty As DependencyProperty = DependencyProperty.Register(NameOf(Maximum), GetType(Double), GetType(RangeWrapper), New FrameworkPropertyMetadata(Double.MaxValue))

    Public Property Maximum As Double
        Get
            Return CDbl(GetValue(MaximumProperty))
        End Get
        Set(ByVal value As Double)
            SetValue(MaximumProperty, value)
        End Set
    End Property
    Public Shared ReadOnly MinimumProperty As DependencyProperty = DependencyProperty.Register(NameOf(Minimum), GetType(Double), GetType(RangeWrapper), New FrameworkPropertyMetadata(Double.MinValue))

    Public Property Minimum As Double
        Get
            Return CDbl(GetValue(MinimumProperty))
        End Get
        Set(ByVal value As Double)
            SetValue(MinimumProperty, value)
        End Set
    End Property

End Class


Public Class RangeValidationRule
    Inherits ValidationRule

    Public Property Wrapper As RangeWrapper

    Public Sub New()
        'ValidationStep = ValidationStep.UpdatedValue
    End Sub

    Public Overrides Function Validate(value As Object, cultureInfo As CultureInfo) As ValidationResult
        Dim numericValue As Double = 0

        Try
            If (CStr(value)).Length > 0 Then numericValue = Double.Parse(CType(value, String))
        Catch e As Exception
            Return New ValidationResult(False, $"Illegal characters or {e.Message}")
        End Try

        If (numericValue < Wrapper.Minimum) OrElse (numericValue > Wrapper.Maximum) Then
            Return New ValidationResult(False, $"Number must be within range '{Wrapper.Minimum}' to '{Wrapper.Maximum}'.")
        End If

        Return ValidationResult.ValidResult
    End Function
End Class
Public Class BindingProxy
    Inherits Freezable

    Protected Overrides Function CreateInstanceCore() As Freezable
        Return New BindingProxy()
    End Function

    Public Property Data As Object
        Get
            Return CObj(GetValue(DataProperty))
        End Get
        Set(ByVal value As Object)
            SetValue(DataProperty, value)
        End Set
    End Property

    Public Shared ReadOnly DataProperty As DependencyProperty = DependencyProperty.Register(NameOf(Data), GetType(Object), GetType(BindingProxy), New PropertyMetadata(Nothing))
End Class
