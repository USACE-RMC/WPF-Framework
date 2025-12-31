Imports System.Globalization

Public Class Color_Picker

    Private ReadOnly _markerTransform As New TranslateTransform()
    Private _colorPosition As Point
    Private _updateMarker As Boolean = True
    Private _updateSlider As Boolean = True
    Private _isLoaded As Boolean = False

    Public Shared ColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(Color), GetType(SolidColorBrush), GetType(Color_Picker), New FrameworkPropertyMetadata(Brushes.White, AddressOf ColorCallback))

    Private Shared Sub ColorCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(Color_Picker) Then Exit Sub
        Dim thisControl = DirectCast(d, Color_Picker)
        '
        If IsNothing(e.NewValue) Then Exit Sub
        Dim newColor = TryCast(e.NewValue, SolidColorBrush)
        If IsNothing(newColor) Then Exit Sub
        If thisControl._updateMarker Then thisControl.UpdateMarkerPosition(newColor.Color)
        If thisControl._updateSlider Then thisControl.UpdateSliderPosition(newColor.Color)
        '
        thisControl.PreviewDrawingBrush.Opacity = newColor.Color.A / 255
        '
        If thisControl.MaxAStop IsNot Nothing Then thisControl.MaxAStop.Color = Media.Color.FromRgb(newColor.Color.R, newColor.Color.G, newColor.Color.B)

        If thisControl.MinRStop IsNot Nothing Then thisControl.MinRStop.Color = Media.Color.FromRgb(0, newColor.Color.G, newColor.Color.B)
        If thisControl.MaxRStop IsNot Nothing Then thisControl.MaxRStop.Color = Media.Color.FromRgb(255, newColor.Color.G, newColor.Color.B)

        If thisControl.MinGStop IsNot Nothing Then thisControl.MinGStop.Color = Media.Color.FromRgb(newColor.Color.R, 0, newColor.Color.B)
        If thisControl.MaxGStop IsNot Nothing Then thisControl.MaxGStop.Color = Media.Color.FromRgb(newColor.Color.R, 255, newColor.Color.B)

        If thisControl.MinBStop IsNot Nothing Then thisControl.MinBStop.Color = Media.Color.FromRgb(newColor.Color.R, newColor.Color.G, 0)
        If thisControl.MaxBStop IsNot Nothing Then thisControl.MaxBStop.Color = Media.Color.FromRgb(newColor.Color.R, newColor.Color.G, 255)
    End Sub

    ''' <summary>
    ''' Color Property
    ''' </summary>
    ''' <returns></returns>
    Public Property Color As SolidColorBrush
        Get
            Return DirectCast(GetValue(ColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(ColorProperty, value)
        End Set
    End Property

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DataContext = Me
    End Sub
    Public Overrides Sub OnApplyTemplate()

        MyBase.OnApplyTemplate()
        Dim pickerBrush As New LinearGradientBrush() With {.StartPoint = New Point(0.5, 0), .EndPoint = New Point(0.5, 1), .ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation}

        Dim colorsList As List(Of Color) = GenerateHsvSpectrum()
        Dim stopIncrement As Double = 1 / colorsList.Count

        Dim i As Integer
        For i = 0 To colorsList.Count - 1
            pickerBrush.GradientStops.Add(New GradientStop(colorsList(i), i * stopIncrement))
        Next

        pickerBrush.GradientStops(i - 1).Offset = 1.0
        PART_ColorSlider.Background = pickerBrush
        'PART_SpectrumDisplay.Fill = pickerBrush
        '
        Dim SliderColor As Color = ConvertHsvToRgb(360 - PART_ColorSlider.Value, 1, 1)
        GradBrush1.Color = SliderColor
        GradStop2.Color = SliderColor
        '
    End Sub

    Private Sub Color_Picker_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If _isLoaded Then Exit Sub
        If IsNothing(Color) Then Exit Sub
        If _updateMarker Then UpdateMarkerPosition(Color.Color)
        If _updateSlider Then UpdateSliderPosition(Color.Color)
        '
        _isLoaded = True
    End Sub

    Private Sub PART_ColorSlider_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double))
        Dim SliderColor = ConvertHsvToRgb(360 - e.NewValue, 1, 1)
        GradBrush1.Color = SliderColor
        GradStop2.Color = SliderColor
        _updateSlider = False
        Color = DetermineColorFromPreview(_colorPosition)
        _updateSlider = True
        'UpdateControls(, False)
    End Sub

    Private Sub PreviewBorder_MouseUp(sender As Object, e As MouseButtonEventArgs)
        PreviewBorder.Cursor = Cursors.Cross
    End Sub
    Private Sub PreviewBorder_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        PreviewBorder.Cursor = Cursors.ScrollAll
        Dim p As Point = e.GetPosition(PreviewBorder)
        UpdateMarkerPosition(p)
        _updateMarker = False
        _updateSlider = False
        Color = DetermineColorFromPreview(_colorPosition)
        _updateMarker = True
        _updateSlider = True
        'UpdateControls(False, False)
    End Sub

    Private Sub PreviewBorder_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            PreviewBorder.Cursor = Cursors.ScrollAll
            Dim p As Point = e.GetPosition(PreviewBorder)
            UpdateMarkerPosition(p)
            _updateMarker = False
            _updateSlider = False
            Color = DetermineColorFromPreview(_colorPosition)
            _updateMarker = True
            _updateSlider = True
            Mouse.Synchronize()
        End If
    End Sub

    Private Sub PreviewBorder_MouseLeave(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            PreviewBorder.Cursor = Cursors.Cross
            Dim p As Point = e.GetPosition(PreviewBorder)
            UpdateMarkerPosition(p)
            _updateMarker = False
            _updateSlider = False
            Color = DetermineColorFromPreview(_colorPosition)
            _updateMarker = True
            _updateSlider = True
        End If
    End Sub
    Private Sub UpdateMarkerPosition(p As Point)
        If p.X > PreviewBorder.ActualWidth Then p.X = PreviewBorder.ActualWidth
        If p.X < 0 Then p.X = 0
        If p.Y > PreviewBorder.ActualHeight Then p.Y = PreviewBorder.ActualHeight
        If p.Y < 0 Then p.Y = 0
        _markerTransform.X = p.X
        _markerTransform.Y = p.Y
        If PreviewBorder.ActualWidth > 0 Then p.X /= PreviewBorder.ActualWidth
        If PreviewBorder.ActualHeight > 0 Then p.Y /= PreviewBorder.ActualHeight
        _colorPosition = p
        '
        PART_ColorMarker1.RenderTransform = _markerTransform
        PART_ColorMarker2.RenderTransform = _markerTransform
    End Sub
    Private Sub UpdateMarkerPosition(theColor As Color)
        _colorPosition = Nothing
        Dim hsv As HsvColor = ConvertRgbToHsv(theColor.R, theColor.G, theColor.B)
        Dim p As New Point(hsv.S, 1 - hsv.V)
        If p.X > 1 Then p.X = 1
        If p.X < 0 Then p.X = 0
        If p.Y > 1 Then p.Y = 1
        If p.Y < 0 Then p.Y = 0
        '
        p.X *= PreviewBorder.ActualWidth
        p.Y *= PreviewBorder.ActualHeight
        UpdateMarkerPosition(p)
    End Sub

    Private Sub UpdateSliderPosition(newColor As Color)
        RemoveHandler PART_ColorSlider.ValueChanged, AddressOf PART_ColorSlider_ValueChanged
        Dim hsv As HsvColor = ConvertRgbToHsv(newColor.R, newColor.B, newColor.G)
        PART_ColorSlider.Value = hsv.H
        Dim SliderColor = ConvertHsvToRgb(360 - PART_ColorSlider.Value, 1, 1)
        GradBrush1.Color = SliderColor
        GradStop2.Color = SliderColor
        AddHandler PART_ColorSlider.ValueChanged, AddressOf PART_ColorSlider_ValueChanged
    End Sub

    Private Function DetermineColorFromPreview(p As Point) As SolidColorBrush
        Dim hsv As New HsvColor(360 - PART_ColorSlider.Value, p.X, 1 - p.Y)
        Dim baseColor As Color = ConvertHsvToRgb(hsv.H, hsv.S, hsv.V)
        baseColor.A = If(Color Is Nothing, CByte(255), Color.Color.A)
        Return New SolidColorBrush(baseColor)
    End Function

    Private Sub Rectangle_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        Dim r As Rectangle = CType(sender, Rectangle)
        Dim c As Color = DirectCast(r.Fill, SolidColorBrush).Color
        c.A = If(Color Is Nothing, CByte(255), Color.Color.A)
        Color = New SolidColorBrush(c)
    End Sub

    Private Sub PreviewBorder_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If Color IsNot Nothing Then UpdateMarkerPosition(Color.Color)
    End Sub


#Region "HSV Stuff"

    ' Generates a list of colors with hues ranging from 0 360
    ' and a saturation and value of 1. 
    Public Shared Function GenerateHsvSpectrum() As List(Of Color)
        Dim colorsList As New List(Of Color)(8)
        '
        For i As Integer = 0 To 28
            colorsList.Add(ConvertHsvToRgb(i * 12, 1, 1))
        Next
        colorsList.Add(ConvertHsvToRgb(0, 1, 1))
        '
        Return colorsList
    End Function

    ' Converts an HSV color to an RGB color.
    Public Shared Function ConvertHsvToRgb(h As Double, s As Double, v As Double) As Color

        Dim r As Double, g As Double, b As Double

        If Math.Abs(s) < 0.00000001 Then
            r = v
            g = v
            b = v
        Else
            Dim i As Integer
            Dim f As Double, p As Double, q As Double, t As Double

            h = If(Math.Abs(h - 360) < 0.00000001, 0, h / 60)

            i = CInt(Math.Truncate(h))
            f = h - i
            '
            p = v * (1.0 - s)
            q = v * (1.0 - (s * f))
            t = v * (1.0 - (s * (1.0 - f)))
            '
            Select Case i
                Case 0
                    r = v
                    g = t
                    b = p
                Case 1
                    r = q
                    g = v
                    b = p
                Case 2
                    r = p
                    g = v
                    b = t
                Case 3
                    r = p
                    g = q
                    b = v
                Case 4
                    r = t
                    g = p
                    b = v
                Case Else
                    r = v
                    g = p
                    b = q
            End Select
        End If
        '
        Return Media.Color.FromArgb(255, CByte(r * 255), CByte(g * 255), CByte(b * 255))

    End Function

    ' Converts an RGB color to an HSV color.
    Public Shared Function ConvertRgbToHsv(r As Integer, g As Integer, b As Integer) As HsvColor

        Dim delta As Double, min As Double
        Dim h As Double = 0, s As Double, v As Double

        min = Math.Min(Math.Min(r, g), b)
        v = Math.Max(Math.Max(r, g), b)
        delta = v - min

        s = If(Math.Abs(v) < 0.00000001, 0, delta / v)

        If Math.Abs(s - 0) < 0.00000001 Then
            h = 0.0
        Else
            If Math.Abs(r - v) < 0.00000001 Then
                h = (g - b) / delta
            ElseIf Math.Abs(g - v) < 0.00000001 Then
                h = 2 + ((b - r) / delta)
            ElseIf Math.Abs(b - v) < 0.00000001 Then
                h = 4 + ((r - g) / delta)
            End If
            '
            h *= 60
            If h < 0.0 Then h += 360
        End If
        '
        Return New HsvColor With {.H = h, .S = s, .V = v / 255}

    End Function

    Public Structure HsvColor
        Public H As Double
        Public S As Double
        Public V As Double

        Public Sub New(h As Double, s As Double, v As Double)
            Me.H = h
            Me.S = s
            Me.V = v
        End Sub
    End Structure

#End Region

End Class
Public Enum ColorComponent
    A
    R
    G
    B
End Enum
Public Class ColorToByteConverter
    Implements IValueConverter

    Public Property Component As ColorComponent = ColorComponent.R
    Private _color As SolidColorBrush
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing Then
            _color = Nothing
            Return Nothing
        End If
        _color = TryCast(value, SolidColorBrush)
        If _color Is Nothing Then Return Nothing
        '
        Select Case Component
            Case ColorComponent.A
                Return _color.Color.A
            Case ColorComponent.R
                Return _color.Color.R
            Case ColorComponent.G
                Return _color.Color.G
            Case ColorComponent.B
                Return _color.Color.B
            Case Else
                Return CByte(0)
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        If value Is Nothing Then Return Nothing
        If _color Is Nothing Then Return New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
        '
        Dim doubleValue As Double = 0
        Dim byteValue As Byte = 0
        Dim unused = Double.TryParse(value.ToString(), doubleValue) '= False Then byteValue = 0 'Return New SolidColorBrush(Color.FromArgb(0, 0, 0, 0))
        '
        If doubleValue <= 255 AndAlso doubleValue >= 0 Then
            byteValue = CByte(doubleValue)
        End If

        Select Case Component
            Case ColorComponent.A
                Return New SolidColorBrush(Color.FromArgb(byteValue, _color.Color.R, _color.Color.G, _color.Color.B))
            Case ColorComponent.R
                Return New SolidColorBrush(Color.FromArgb(_color.Color.A, byteValue, _color.Color.G, _color.Color.B))
            Case ColorComponent.G
                Return New SolidColorBrush(Color.FromArgb(_color.Color.A, _color.Color.R, byteValue, _color.Color.B))
            Case ColorComponent.B
                Return New SolidColorBrush(Color.FromArgb(_color.Color.A, _color.Color.R, _color.Color.G, byteValue))
            Case Else
                Return New SolidColorBrush(Color.FromArgb(byteValue, byteValue, byteValue, byteValue))
        End Select
    End Function
End Class