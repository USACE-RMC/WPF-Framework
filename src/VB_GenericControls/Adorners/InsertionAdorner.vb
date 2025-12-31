''' <summary>
''' InsertionAdorner class implemented from code developed at Zag studios which was found (with tutorial) on the Zag log (http://www.zagstudio.com/blog/488#.Us185vRDvp4)
''' </summary>
''' <remarks>Implemented by Woodrow Lee Fields 01/08/2014</remarks>
Public Class InsertionAdorner
    Inherits Adorner
    Private ReadOnly isSeparatorHorizontal As Boolean
    Public Property IsInFirstHalf() As Boolean
        Get
            Return _isInFirstHalf
        End Get
        Set(value As Boolean)
            _isInFirstHalf = value
        End Set
    End Property
    Private _isInFirstHalf As Boolean
    Private ReadOnly _adornerLayer As AdornerLayer
    Private Shared ReadOnly _pen As Pen
    Private Shared ReadOnly _triangleGeometry As PathGeometry

    ' Create the pen and triangle in a static constructor and freeze them to improve performance.
    Shared Sub New()
        _pen = New Pen(Brushes.Gray, 2)
        _pen.Freeze()

        Dim firstLine As New LineSegment(New Point(0, -5), False)
        firstLine.Freeze()
        Dim secondLine As New LineSegment(New Point(0, 5), False)
        secondLine.Freeze()

        Dim figure As New PathFigure
        figure.StartPoint = New Point(5, 0)
        figure.Segments.Add(firstLine)
        figure.Segments.Add(secondLine)
        figure.Freeze()

        _triangleGeometry = New PathGeometry()
        _triangleGeometry.Figures.Add(figure)
        _triangleGeometry.Freeze()
    End Sub

    Public Sub New(isSeparatorHorizontal As Boolean, isInFirstHalf As Boolean, adornedElement As UIElement, adornerLayer As AdornerLayer)
        MyBase.New(adornedElement)
        Me.isSeparatorHorizontal = isSeparatorHorizontal
        Me.IsInFirstHalf = isInFirstHalf
        _adornerLayer = adornerLayer
        IsHitTestVisible = False

        _adornerLayer.Add(Me)
    End Sub

    ' This draws one line and two triangles at each end of the line.
    Protected Overrides Sub OnRender(drawingContext As DrawingContext)
        Dim startPoint As Point
        Dim endPoint As Point

        CalculateStartAndEndPoint(startPoint, endPoint)
        drawingContext.DrawLine(_pen, startPoint, endPoint)

        If isSeparatorHorizontal Then
            DrawTriangle(drawingContext, startPoint, 0)
            DrawTriangle(drawingContext, endPoint, 180)
        Else
            DrawTriangle(drawingContext, startPoint, 90)
            DrawTriangle(drawingContext, endPoint, -90)
        End If
    End Sub

    Private Sub DrawTriangle(drawingContext As DrawingContext, origin As Point, angle As Double)
        drawingContext.PushTransform(New TranslateTransform(origin.X, origin.Y))
        drawingContext.PushTransform(New RotateTransform(angle))

        drawingContext.DrawGeometry(_pen.Brush, Nothing, _triangleGeometry)

        drawingContext.Pop()
        drawingContext.Pop()
    End Sub

    Private Sub CalculateStartAndEndPoint(ByRef startPoint As Point, ByRef endPoint As Point)
        startPoint = New Point()
        endPoint = New Point()

        Dim eWidth As Double = AdornedElement.RenderSize.Width
        Dim eHeight As Double = AdornedElement.RenderSize.Height

        If isSeparatorHorizontal Then
            endPoint.X = eWidth
            If Not IsInFirstHalf Then
                startPoint.Y = eHeight
                endPoint.Y = eHeight
            End If
        Else
            endPoint.Y = eHeight
            If Not IsInFirstHalf Then
                startPoint.X = eWidth
                endPoint.X = eWidth
            End If
        End If
    End Sub

    Public Sub Detach()
        _adornerLayer.Remove(Me)
    End Sub

End Class
