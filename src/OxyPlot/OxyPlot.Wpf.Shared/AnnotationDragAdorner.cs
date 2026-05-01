// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationDragAdorner.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Lightweight overlay element for rendering annotation place / drag previews with minimal per-mouse-move cost.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Lightweight overlay used to render an in-progress annotation's preview during placement
    /// or drag. Mirrors the design of <see cref="ZoomRectangleAdorner"/> for the magnifier zoom:
    /// a hit-test-invisible <see cref="FrameworkElement"/> that draws via <see cref="OnRender"/>
    /// and is mutated per mouse-move via <c>Show*</c> methods that call
    /// <see cref="UIElement.InvalidateVisual"/> (no layout pass, no plot re-render).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Why this exists.</b> Annotation place/drag previously wrote a WPF DependencyProperty
    /// on the live annotation per mouse-move (e.g., <c>EndPoint</c>, <c>MaximumX</c>, <c>X</c>).
    /// Each DP write fires <c>Annotation.AppearanceChanged</c> which calls
    /// <c>IPlotView.InvalidatePlot(false)</c> unconditionally, re-rendering every series at
    /// 60–125 Hz during a drag. On a dense multi-LineSeries plot this produced visible
    /// multi-frame lag — exactly the problem <see cref="ZoomRectangleAdorner"/> solved for the
    /// magnifier zoom.
    /// </para>
    /// <para>
    /// <b>What this does instead.</b> A toolbar mouse handler captures the annotation's
    /// initial state at <c>MouseDown</c>, then on each <c>MouseMove</c> calls one of the
    /// <c>Show*</c> methods to update the adorner's screen-space preview. The annotation's
    /// own DPs are written exactly once at <c>MouseUp</c>. The adorner draws a faithful
    /// red-outlined preview matching the existing "drag highlight" UX (which used to set
    /// <c>Color = Colors.Red</c> / <c>Fill = Colors.Red</c> on the live annotation).
    /// </para>
    /// <para>
    /// All preview drawing uses frozen, statically cached <see cref="Pen"/> and <see cref="Brush"/>
    /// instances. <see cref="EdgeMode.Aliased"/> is applied to skip stroke-tessellation
    /// anti-aliasing (the preview is a transient drag affordance — slight pixel jaggedness
    /// is acceptable). The adorner is hit-test invisible so it never intercepts the drag's
    /// mouse events.
    /// </para>
    /// </remarks>
    public sealed class AnnotationDragAdorner : FrameworkElement
    {
        /// <summary>
        /// Frozen, shared red stroke pen for outlined previews (1.5 px solid red).
        /// </summary>
        private static readonly Pen RedStrokePen;

        /// <summary>
        /// Frozen, shared semi-transparent red fill brush for filled previews
        /// (rectangle / ellipse / polygon when fill is appropriate).
        /// </summary>
        private static readonly Brush RedFillBrush;

        /// <summary>
        /// Frozen, shared solid red brush for arrowheads, point markers, and text.
        /// </summary>
        private static readonly Brush RedSolidBrush;

        /// <summary>
        /// Typeface used for the text-annotation preview's placeholder text.
        /// </summary>
        private static readonly Typeface PreviewTypeface;

        /// <summary>
        /// The active preview shape. <see cref="Shape.None"/> means no preview is drawn.
        /// </summary>
        private Shape activeShape;

        /// <summary>
        /// Bounding rectangle for rect / ellipse / text preview.
        /// </summary>
        private Rect rect;

        /// <summary>
        /// First endpoint for line / arrow / axis-line previews. Also used as anchor for
        /// point and text previews.
        /// </summary>
        private Point pt1;

        /// <summary>
        /// Second endpoint for line / arrow / axis-line previews.
        /// </summary>
        private Point pt2;

        /// <summary>
        /// Radius for the point preview circle.
        /// </summary>
        private double pointRadius;

        /// <summary>
        /// Arrowhead length for the arrow preview, in pixels along the arrow direction.
        /// </summary>
        private double arrowHeadLength;

        /// <summary>
        /// Arrowhead width for the arrow preview, in pixels perpendicular to the arrow direction.
        /// </summary>
        private double arrowHeadWidth;

        /// <summary>
        /// Vertices for polygon / polyline previews. Held as a simple array because we only
        /// iterate; no need for mutation after assignment.
        /// </summary>
        private Point[] polyPoints;

        /// <summary>
        /// Text content for the text preview ("Text Annotation" placeholder).
        /// </summary>
        private string previewText;

        /// <summary>
        /// Font size for the text preview.
        /// </summary>
        private double textFontSize;

        static AnnotationDragAdorner()
        {
            RedStrokePen = new Pen(new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00)), 1.5);
            RedStrokePen.Freeze();

            RedFillBrush = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0x00, 0x00));
            RedFillBrush.Freeze();

            RedSolidBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
            RedSolidBrush.Freeze();

            // Match the default OxyPlot annotation typeface (Segoe UI). Frozen by construction.
            PreviewTypeface = new Typeface("Segoe UI");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnnotationDragAdorner"/> class.
        /// </summary>
        public AnnotationDragAdorner()
        {
            // Aliased edges skip WPF stroke-tessellation AA. The preview is a transient drag
            // affordance — slight pixel jaggedness is acceptable and worth the major perf win.
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            // Drag preview never intercepts pointer events.
            this.IsHitTestVisible = false;

            // The adorner draws to its own coordinate space; no layout participation needed.
            this.SnapsToDevicePixels = true;
        }

        /// <summary>
        /// Identifies which preview shape (if any) is currently active.
        /// </summary>
        private enum Shape
        {
            /// <summary>No preview is drawn.</summary>
            None,

            /// <summary>Rectangle outline preview.</summary>
            Rect,

            /// <summary>Ellipse outline preview.</summary>
            Ellipse,

            /// <summary>Line with an arrowhead at <see cref="pt2"/>.</summary>
            Arrow,

            /// <summary>Line spanning the plot area (no arrowhead) — used by line annotations.</summary>
            AxisLine,

            /// <summary>Filled point marker.</summary>
            Point,

            /// <summary>Open polyline through <see cref="polyPoints"/>.</summary>
            PolyOpen,

            /// <summary>Closed polygon through <see cref="polyPoints"/>.</summary>
            PolyClosed,

            /// <summary>Text label preview with bounding rectangle.</summary>
            Text,
        }

        /// <summary>
        /// Shows a rectangle outline preview at the specified screen-space rect.
        /// </summary>
        /// <param name="r">The rectangle in adorner-local coordinates.</param>
        public void ShowRect(Rect r)
        {
            this.activeShape = Shape.Rect;
            this.rect = r;
            this.InvalidateVisual();
        }

        /// <summary>
        /// Shows an ellipse outline preview at the specified screen-space rect (the ellipse
        /// is inscribed in the rect).
        /// </summary>
        /// <param name="r">The bounding rectangle in adorner-local coordinates.</param>
        public void ShowEllipse(Rect r)
        {
            this.activeShape = Shape.Ellipse;
            this.rect = r;
            this.InvalidateVisual();
        }

        /// <summary>
        /// Shows an arrow preview from <paramref name="start"/> to <paramref name="end"/>
        /// with an arrowhead at <paramref name="end"/>.
        /// </summary>
        /// <param name="start">The arrow's tail in adorner-local coordinates.</param>
        /// <param name="end">The arrow's tip in adorner-local coordinates.</param>
        /// <param name="headLength">Arrowhead length along the arrow direction (pixels).</param>
        /// <param name="headWidth">Arrowhead width perpendicular to the arrow direction (pixels).</param>
        public void ShowArrow(Point start, Point end, double headLength, double headWidth)
        {
            this.activeShape = Shape.Arrow;
            this.pt1 = start;
            this.pt2 = end;
            this.arrowHeadLength = headLength;
            this.arrowHeadWidth = headWidth;
            this.InvalidateVisual();
        }

        /// <summary>
        /// Shows a line preview between two points without an arrowhead. Used for vertical /
        /// horizontal / sloped line annotations spanning the plot area.
        /// </summary>
        /// <param name="p1">First endpoint in adorner-local coordinates.</param>
        /// <param name="p2">Second endpoint in adorner-local coordinates.</param>
        public void ShowAxisLine(Point p1, Point p2)
        {
            this.activeShape = Shape.AxisLine;
            this.pt1 = p1;
            this.pt2 = p2;
            this.InvalidateVisual();
        }

        /// <summary>
        /// Shows a filled point marker preview at the specified center with the specified radius.
        /// </summary>
        /// <param name="center">Center of the point in adorner-local coordinates.</param>
        /// <param name="radius">Radius of the point marker in pixels.</param>
        public void ShowPoint(Point center, double radius)
        {
            this.activeShape = Shape.Point;
            this.pt1 = center;
            this.pointRadius = radius;
            this.InvalidateVisual();
        }

        /// <summary>
        /// Shows a polyshape preview connecting the given points. If <paramref name="closed"/>
        /// is true the shape is closed (polygon); otherwise it's open (polyline).
        /// </summary>
        /// <param name="pts">Vertices in adorner-local coordinates. Copied internally.</param>
        /// <param name="closed">True for a closed polygon; false for an open polyline.</param>
        public void ShowPolyshape(IReadOnlyList<Point> pts, bool closed)
        {
            if (pts == null) throw new ArgumentNullException(nameof(pts));
            this.activeShape = closed ? Shape.PolyClosed : Shape.PolyOpen;
            // Copy into a private array so subsequent caller-side mutations don't affect us.
            var copy = new Point[pts.Count];
            for (int i = 0; i < pts.Count; i++)
            {
                copy[i] = pts[i];
            }
            this.polyPoints = copy;
            this.InvalidateVisual();
        }

        /// <summary>
        /// Shows a text-annotation preview: a red-bordered bounding rectangle plus the
        /// placeholder text rendered in red at the specified anchor point.
        /// </summary>
        /// <param name="anchor">Top-left corner of the text in adorner-local coordinates.</param>
        /// <param name="text">The placeholder text content.</param>
        /// <param name="fontSize">Font size in pixels.</param>
        /// <param name="bounds">Approximate bounding rectangle (for the dashed outline).</param>
        public void ShowText(Point anchor, string text, double fontSize, Rect bounds)
        {
            this.activeShape = Shape.Text;
            this.pt1 = anchor;
            this.previewText = text ?? string.Empty;
            this.textFontSize = fontSize > 0 ? fontSize : 12;
            this.rect = bounds;
            this.InvalidateVisual();
        }

        /// <summary>
        /// Clears the preview so the adorner draws nothing on the next render frame.
        /// </summary>
        public void Clear()
        {
            if (this.activeShape == Shape.None) return;
            this.activeShape = Shape.None;
            this.polyPoints = null;
            this.previewText = null;
            this.InvalidateVisual();
        }

        /// <inheritdoc/>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            switch (this.activeShape)
            {
                case Shape.None:
                    return;

                case Shape.Rect:
                    if (this.rect.Width > 0 && this.rect.Height > 0)
                    {
                        drawingContext.DrawRectangle(null, RedStrokePen, this.rect);
                    }
                    break;

                case Shape.Ellipse:
                    if (this.rect.Width > 0 && this.rect.Height > 0)
                    {
                        var center = new Point(
                            this.rect.X + this.rect.Width / 2,
                            this.rect.Y + this.rect.Height / 2);
                        drawingContext.DrawEllipse(null, RedStrokePen, center,
                            this.rect.Width / 2, this.rect.Height / 2);
                    }
                    break;

                case Shape.Arrow:
                    DrawArrow(drawingContext, this.pt1, this.pt2,
                        this.arrowHeadLength, this.arrowHeadWidth);
                    break;

                case Shape.AxisLine:
                    drawingContext.DrawLine(RedStrokePen, this.pt1, this.pt2);
                    break;

                case Shape.Point:
                    if (this.pointRadius > 0)
                    {
                        drawingContext.DrawEllipse(RedSolidBrush, RedStrokePen,
                            this.pt1, this.pointRadius, this.pointRadius);
                    }
                    break;

                case Shape.PolyOpen:
                case Shape.PolyClosed:
                    DrawPolyshape(drawingContext, this.polyPoints,
                        closed: this.activeShape == Shape.PolyClosed);
                    break;

                case Shape.Text:
                    DrawTextPreview(drawingContext, this.pt1, this.previewText,
                        this.textFontSize, this.rect);
                    break;
            }
        }

        /// <summary>
        /// Draws an arrow as a line plus a filled triangular arrowhead at the tip. The
        /// arrowhead is computed from the arrow direction so the preview reads as a true
        /// arrow, matching the default OxyPlot ArrowAnnotation appearance.
        /// </summary>
        private static void DrawArrow(DrawingContext dc, Point start, Point end,
            double headLength, double headWidth)
        {
            var dx = end.X - start.X;
            var dy = end.Y - start.Y;
            var length = Math.Sqrt(dx * dx + dy * dy);
            if (length < double.Epsilon)
            {
                // Degenerate arrow — just a point. Don't try to compute a direction.
                return;
            }

            // Normalised direction along the arrow.
            var dirX = dx / length;
            var dirY = dy / length;

            // Perpendicular unit vector (rotated +90°).
            var perpX = -dirY;
            var perpY = dirX;

            // Base of the arrowhead, headLength back from the tip along the arrow direction.
            var actualHead = Math.Min(headLength, length);
            var baseX = end.X - dirX * actualHead;
            var baseY = end.Y - dirY * actualHead;

            // Half-width offset perpendicular to the arrow.
            var halfWidth = headWidth / 2;
            var leftX = baseX + perpX * halfWidth;
            var leftY = baseY + perpY * halfWidth;
            var rightX = baseX - perpX * halfWidth;
            var rightY = baseY - perpY * halfWidth;

            // Shaft from start to the base of the arrowhead so the line and the head don't
            // visually overlap the tip stroke.
            dc.DrawLine(RedStrokePen, start, new Point(baseX, baseY));

            // Arrowhead triangle, filled with solid red and outlined with the same stroke pen.
            var triangle = new StreamGeometry();
            using (var ctx = triangle.Open())
            {
                ctx.BeginFigure(end, isFilled: true, isClosed: true);
                ctx.LineTo(new Point(leftX, leftY), isStroked: true, isSmoothJoin: false);
                ctx.LineTo(new Point(rightX, rightY), isStroked: true, isSmoothJoin: false);
            }
            triangle.Freeze();
            dc.DrawGeometry(RedSolidBrush, RedStrokePen, triangle);
        }

        /// <summary>
        /// Draws a polyshape (open polyline or closed polygon) with semi-transparent fill on
        /// closed shapes and a red stroke on both.
        /// </summary>
        private static void DrawPolyshape(DrawingContext dc, Point[] pts, bool closed)
        {
            if (pts == null || pts.Length < 2) return;

            var geometry = new StreamGeometry();
            using (var ctx = geometry.Open())
            {
                ctx.BeginFigure(pts[0], isFilled: closed, isClosed: closed);
                for (int i = 1; i < pts.Length; i++)
                {
                    ctx.LineTo(pts[i], isStroked: true, isSmoothJoin: false);
                }
            }
            geometry.Freeze();
            dc.DrawGeometry(closed ? RedFillBrush : null, RedStrokePen, geometry);
        }

        /// <summary>
        /// Draws the text-annotation preview: a dashed bounding rectangle plus the placeholder
        /// text rendered in red. The bounding rect is computed by the caller via
        /// <see cref="FormattedText"/> measurement so the preview matches the placed-annotation
        /// extent closely.
        /// </summary>
        private static void DrawTextPreview(DrawingContext dc, Point anchor, string text,
            double fontSize, Rect bounds)
        {
            if (bounds.Width > 0 && bounds.Height > 0)
            {
                dc.DrawRectangle(null, RedStrokePen, bounds);
            }

            if (!string.IsNullOrEmpty(text))
            {
                var ft = new FormattedText(
                    text,
                    CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    PreviewTypeface,
                    fontSize,
                    RedSolidBrush,
                    pixelsPerDip: 1.0);
                dc.DrawText(ft, anchor);
            }
        }
    }
}
