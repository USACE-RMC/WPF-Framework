Imports System.Globalization
Imports System.Windows.Markup


#Region "Boolean Converters"

Public Class ReverseBooleanConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return value
        If value.GetType <> GetType(Boolean) Then Return Nothing
        Return Not CBool(value)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return value
        If value.GetType <> GetType(Boolean) Then Return Nothing
        Return Not CBool(value)
    End Function
End Class

Public Class BooleanToColorConverter
    Implements IValueConverter

    Public Property TrueValue() As Color
    Public Property FalseValue() As Color

    Public Sub New()
        ' set defaults
        TrueValue = Colors.Black
        FalseValue = Colors.Transparent
    End Sub

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return Nothing
        If value.GetType <> GetType(Boolean) Then Return Nothing
        Return If(CBool(value), TrueValue, FalseValue)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return Nothing
        If value.GetType = GetType(Color) Then
            If Color.AreClose(DirectCast(value, Color), TrueValue) Then Return True
            If Color.AreClose(DirectCast(value, Color), FalseValue) Then Return False
        End If
        '
        Return Nothing
    End Function
End Class

Public Class BooleanToBrushConverter
    Implements IValueConverter

    Public Property TrueValue() As Brush
    Public Property FalseValue() As Brush

    Public Sub New()
        ' set defaults
        TrueValue = Brushes.Black
        FalseValue = Brushes.Transparent
    End Sub

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return Nothing
        If value.GetType <> GetType(Boolean) Then Return Nothing
        Return If(CBool(value), TrueValue, FalseValue)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return Nothing
        If value.GetType = GetType(Brush) Then
            If value.Equals(TrueValue) Then Return True
            If value.Equals(FalseValue) Then Return False
        End If
        '
        Return Nothing
    End Function
End Class

Public Class BooleanToTextConverter
    Implements IValueConverter

    Public Property TrueValue() As String
    Public Property FalseValue() As String

    Public Sub New()
        ' set defaults
        TrueValue = ""
        FalseValue = ""
    End Sub

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return Nothing
        If value.GetType <> GetType(Boolean) Then Return Nothing
        Return If(CBool(value), TrueValue, FalseValue)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return Nothing
        If value.GetType = GetType(String) Then
            If DirectCast(value, String) = TrueValue Then Return True
            If DirectCast(value, String) = FalseValue Then Return False
        Else
            Dim castValue As String = value.ToString()
            If castValue = TrueValue Then Return True
            If castValue = FalseValue Then Return False
        End If
        '
        Return Nothing
    End Function
End Class

Public Class BooleanToDoubleConverter
    Implements IValueConverter

    Public Property TrueValue() As Double
    Public Property FalseValue() As Double

    Public Sub New()
        ' set defaults
        TrueValue = 0
        FalseValue = 0
    End Sub

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return Nothing
        If value.GetType <> GetType(Boolean) Then Return Nothing
        Return If(CBool(value), TrueValue, FalseValue)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return Nothing
        If value.GetType = GetType(Double) Then
            If DirectCast(value, Double) = TrueValue Then Return True
            If DirectCast(value, Double) = FalseValue Then Return False
        Else
            Dim castValue As Double
            If Double.TryParse(value.ToString(), castValue) Then
                If castValue = TrueValue Then Return True
                If castValue = FalseValue Then Return False
            End If
        End If
        '
        Return Nothing
    End Function
End Class

Public Class BooleanToVisibilityConverter
    Implements IValueConverter

    Public Property TrueValue() As Visibility
    Public Property FalseValue() As Visibility

    Public Sub New()
        ' set defaults
        TrueValue = Visibility.Visible
        FalseValue = Visibility.Collapsed
    End Sub

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return Nothing
        If value.GetType <> GetType(Boolean) Then Return Nothing
        Return If(CBool(value), TrueValue, FalseValue)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If Equals(value, TrueValue) Then
            Return True
        End If
        If Equals(value, FalseValue) Then
            Return False
        End If
        Return Nothing
    End Function
End Class

Public Class VisibilityToBooleanConverter
    Implements IValueConverter

    Public Property CollapsedValue() As Boolean
    Public Property HiddenValue() As Boolean
    Public Property VisibleValue() As Boolean

    Public Sub New()
        ' set defaults
        CollapsedValue = False
        HiddenValue = False
        VisibleValue = True
    End Sub

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value.GetType <> GetType(Visibility) Then Return Nothing
        Select Case DirectCast(value, Visibility)
            Case Visibility.Collapsed
                Return CollapsedValue
            Case Visibility.Hidden
                Return HiddenValue
            Case Visibility.Visible
                Return VisibleValue
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        'This simple logic won't work with the three state system. For my cases currently it doesn't matter. In the future this might have to be fixed probably by using some private variables to keep track of the converted state.
        If Equals(value, CollapsedValue) Then Return Visibility.Collapsed
        If Equals(value, HiddenValue) Then Return Visibility.Hidden
        If Equals(value, VisibleValue) Then Return Visibility.Visible
        '
        Return Nothing
    End Function
End Class

#End Region

Public Class InRangeConverter
    Implements IValueConverter

    Public Property LowerBound() As Double
    Public Property UpperBound() As Double
    Public Sub New()
        ' set defaults
        LowerBound = Double.MinValue
        UpperBound = Double.MaxValue
    End Sub

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return False
        Dim data As Double = Double.Parse(value.ToString())
        '
        If data < LowerBound OrElse data > UpperBound Then Return False
        Return True
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class AlwaysVisibleConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.Convert
        Return Visibility.Visible
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class TimeTextConverter
    Inherits DependencyObject
    Implements IValueConverter

    Public Property TimeType As TimeTarget = TimeTarget.Hour

    Public Shared Is24HourProperty As DependencyProperty = DependencyProperty.Register(NameOf(Is24Hour), GetType(Boolean), GetType(TimeTextConverter), New UIPropertyMetadata(False))

    Public Property Is24Hour As Boolean
        Get
            Return DirectCast(GetValue(Is24HourProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(Is24HourProperty, value)
        End Set
    End Property
    Public Enum TimeTarget
        Hour
        Minute
        Second
        Meridian
    End Enum
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value.GetType <> GetType(DateTime) Then Return ""
        Select Case TimeType
            Case TimeTarget.Hour
                If Is24Hour Then Return DirectCast(value, DateTime).ToString("HH", CultureInfo.InvariantCulture)
                Return DirectCast(value, DateTime).ToString("hh", CultureInfo.InvariantCulture)
            Case TimeTarget.Minute
                Return DirectCast(value, DateTime).ToString("mm", CultureInfo.InvariantCulture)
            Case TimeTarget.Second
                Return DirectCast(value, DateTime).ToString("ss", CultureInfo.InvariantCulture)
            Case TimeTarget.Meridian
                If Is24Hour Then Return ""
                Return DirectCast(value, DateTime).ToString("tt", CultureInfo.InvariantCulture)
            Case Else
                Return ""
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class DataGridWidthConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim width As Double = Double.Parse(value.ToString())
        'If width < 12 Then Return 0
        Return width - SystemParameters.VerticalScrollBarWidth
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotSupportedException()
    End Function
End Class

Public Class DoubleToThicknessConverter
    Implements IValueConverter

    Public Property Left As Double = 0
    Public Property Top As Double = 0
    Public Property Right As Double = 0
    Public Property Bottom As Double = 0
    '
    Public Property IsLeft As Boolean = True
    Public Property IsRight As Boolean = True
    Public Property IsTop As Boolean = True
    Public Property IsBottom As Boolean = True

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim dblValue As Double
        If Double.TryParse(value.ToString, dblValue) = False Then Return Nothing
        '
        If IsLeft Then Left = dblValue
        If IsTop Then Top = dblValue
        If IsRight Then Right = dblValue
        If IsBottom Then Bottom = dblValue
        '
        Return New Thickness(Left, Top, Right, Bottom)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return value
        Dim t As Thickness = DirectCast(value, Thickness)
        If IsLeft Then Return t.Left
        If IsRight Then Return t.Right
        If IsTop Then Return t.Top
        If IsBottom Then Return t.Bottom
        'if all else fails average them.
        Return (t.Left + t.Right + t.Top + t.Bottom) / 4
    End Function
End Class

Public Class ThicknessToDoubleConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return value
        Dim t As Thickness = DirectCast(value, Thickness)
        Return CDbl((t.Left + t.Right + t.Top + t.Bottom) / 4)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Dim dblValue As Double
        If Double.TryParse(value.ToString, dblValue) = False Then Return Nothing
        Return New Thickness(dblValue)
    End Function
End Class

Public Class ColorToSolidBrushConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return Nothing
        Return New SolidColorBrush(DirectCast(value, Color))
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return Nothing
        Return DirectCast(value, SolidColorBrush).Color
    End Function
End Class

Public Class DrawingColorToSolidColorBrushConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return Nothing
        If value.GetType <> GetType(System.Drawing.Color) Then Return Nothing
        Dim c = DirectCast(value, System.Drawing.Color)
        Return New SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B))
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return Nothing
        Dim scb = TryCast(value, SolidColorBrush)
        If IsNothing(scb) Then Return Nothing
        Dim c = scb.Color
        Return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B)
    End Function
End Class

Public Class FontToFontFamilyConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return value
        Return New FontFamily(value.ToString)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return value
        Return DirectCast(value, FontFamily).Source
    End Function
End Class

Public Class FontFamilyToFontStringConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return value
        Return DirectCast(value, FontFamily).Source
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return value
        Return New FontFamily(value.ToString)
    End Function
End Class

Public Class IntToDoubleConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return Nothing
        Return CDbl(value)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return Nothing
        Return CInt(value)
    End Function

End Class

Public Class DoubleToStringConverter
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return Nothing
        Dim stringDouble As Double
        Double.TryParse(value.ToString, NumberStyles.Any, CultureInfo.InvariantCulture, stringDouble)
        Return stringDouble
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return Nothing
        Return value.ToString
    End Function
End Class

Public Class StringToDoubleConverter
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return Nothing
        Return value.ToString
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return Nothing
        Dim stringDouble As Double
        Double.TryParse(value.ToString, NumberStyles.Any, CultureInfo.InvariantCulture, stringDouble)
        Return stringDouble
    End Function
End Class

Public Class DoubleToDataGridLengthConverter
    Implements IValueConverter

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        If targetType = GetType(DataGridLength) Then
            If value Is Nothing Then Return DataGridLength.Auto
            If TypeOf value Is Double Then Return New DataGridLength(CDbl(value))
            Return DataGridLength.Auto
        End If

        If targetType = GetType(Double) Then
            If TypeOf value Is DataGridLength Then Return (CType(value, DataGridLength)).Value
            Return Double.NaN
        End If

        Return Nothing
    End Function

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Return ConvertBack(value, targetType, parameter, culture)
    End Function
End Class

Public Class DoubleToGridLengthConverter
    Implements IValueConverter

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        If targetType = GetType(GridLength) Then
            If value Is Nothing Then Return GridLength.Auto
            If TypeOf value Is Double Then Return New GridLength(CDbl(value))
            Return GridLength.Auto
        End If

        If targetType = GetType(Double) Then
            If TypeOf value Is GridLength Then Return (CType(value, GridLength)).Value
            Return Double.NaN
        End If

        Return Nothing
    End Function

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Return ConvertBack(value, targetType, parameter, culture)
    End Function
End Class

Public Class DoubleToCornerRadiusConverter
    Implements IValueConverter

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        If targetType = GetType(CornerRadius) Then
            If value Is Nothing Then Return New CornerRadius()
            If TypeOf value Is Double Then Return New CornerRadius(CDbl(value))
            Return New CornerRadius()
        End If

        If targetType = GetType(Double) Then
            If TypeOf value Is CornerRadius Then Return (CType(value, CornerRadius)).BottomLeft
            Return Double.NaN
        End If

        Return Nothing
    End Function

    Public Function Convert(ByVal value As Object, ByVal targetType As Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Return ConvertBack(value, targetType, parameter, culture)
    End Function
End Class

Public Class VectorToPointConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If IsNothing(value) Then Return value
        If value.GetType <> GetType(Vector) Then Return Nothing
        Dim sv As Vector = DirectCast(value, Vector)
        '
        Return New Point(sv.X, sv.Y)
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If IsNothing(value) Then Return value
        If value.GetType <> GetType(Point) Then Return Nothing
        Dim p As Point = DirectCast(value, Point)
        '
        Return New Vector(p.X, p.Y)
    End Function
End Class

Public Class TabSizeConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim tabControl As TabControl = CType(values(0), TabControl)
        Dim width As Double = (tabControl.ActualWidth / tabControl.Items.Count)
        If width < 12 Then Return 0
        Return width - (tabControl.Items.Count + 1)
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class DoubleToNAConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing Then Return "N/A"
        If value.GetType <> GetType(Double) Then Return "N/A"
        Dim val = DirectCast(value, Double)
        '
        If Double.IsNaN(val) Then Return "N/A"
        If Double.IsPositiveInfinity(val) Then Return "+∞"
        If Double.IsNegativeInfinity(val) Then Return "-∞"
        If Double.IsInfinity(val) Then Return "∞"
        '
        Return val
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If value Is Nothing Then Return Double.NaN
        If value.GetType = GetType(String) Then
            Dim val = DirectCast(value, String)
            '
            If val = "NA" Then Return Double.NaN
            If val = "N/A" Then Return Double.NaN
            If val = "inf" OrElse val = "Inf" OrElse val = "infinity" OrElse val = "Infinity" OrElse val = "∞" Then Return Double.PositiveInfinity
            If val = "+inf" OrElse val = "+Inf" OrElse val = "+infinity" OrElse val = "+Infinity" OrElse val = "+∞" Then Return Double.PositiveInfinity
            If val = "-inf" OrElse val = "-Inf" OrElse val = "-infinity" OrElse val = "-Infinity" OrElse val = "-∞" Then Return Double.NegativeInfinity

            ' If value is not a number, return NaN
            Dim dblVal As Double
            If Double.TryParse(val, dblVal) = False Then
                Return Double.NaN
            End If
        End If
        '
        Return value
    End Function
End Class




Public Class StringToNAConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing Then Return "N/A"
        If value.GetType <> GetType(String) Then Return "N/A"
        Dim val = DirectCast(value, String)
        '
        If Double.IsNaN(CDbl(val)) Then Return "N/A"
        If Double.IsInfinity(CDbl(val)) Then Return "N/A"
        '
        Return val
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

Public Class GridlineColorLightConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing OrElse value.GetType <> GetType(SolidColorBrush) Then
            Return New SolidColorBrush(Color.FromArgb(51, 0, 0, 0))
        End If

        Dim c = DirectCast(value, SolidColorBrush)
        If c.Color.A < 51 Then
            Return New SolidColorBrush(Color.FromArgb(CByte(c.Color.A * 0.2), c.Color.R, c.Color.G, c.Color.B))
        Else
            Return New SolidColorBrush(Color.FromArgb(51, c.Color.R, c.Color.G, c.Color.B))
        End If

    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function
End Class

