// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawingVisualRenderContext.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides an <see cref="IRenderContext"/> implementation that renders to a <see cref="DrawingVisual"/>
//   using <see cref="DrawingContext"/>, eliminating per-element WPF layout overhead.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using FontWeights = OxyPlot.FontWeights;
    using HorizontalAlignment = OxyPlot.HorizontalAlignment;
    using VerticalAlignment = OxyPlot.VerticalAlignment;

    /// <summary>
    /// Provides an <see cref="IRenderContext"/> implementation that renders to a <see cref="DrawingVisual"/>
    /// using <see cref="DrawingContext"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This renderer draws all plot elements (lines, polygons, text, images) via a single
    /// <see cref="DrawingVisual"/>, avoiding the per-element WPF Measure/Arrange/Render overhead
    /// that <see cref="CanvasRenderContext"/> incurs from creating and destroying
    /// Path/TextBlock/Image elements every frame.
    /// </para>
    /// <para>
    /// Text positions are recorded during rendering to support toolbar hit-testing via
    /// <see cref="HitTestText"/>. This provides backend-agnostic text click detection
    /// equivalent to Canvas.InputHitTest for the <see cref="CanvasRenderContext"/>.
    /// </para>
    /// </remarks>
    public class DrawingVisualRenderContext : ClippingRenderContext
    {
        /// <summary>
        /// The brush cache, keyed by OxyColor.
        /// </summary>
        private readonly Dictionary<OxyColor, Brush> brushCache = new Dictionary<OxyColor, Brush>();

        /// <summary>
        /// The font family cache, keyed by family name.
        /// </summary>
        private readonly Dictionary<string, FontFamily> fontFamilyCache = new Dictionary<string, FontFamily>();

        /// <summary>
        /// The pen cache, keyed by composite pen parameters.
        /// Avoids repeated Pen allocation and Freeze() calls for identical stroke settings.
        /// </summary>
        private readonly Dictionary<long, Pen> penCache = new Dictionary<long, Pen>();

        /// <summary>
        /// The image cache, keyed by OxyImage.
        /// </summary>
        private readonly Dictionary<OxyImage, BitmapSource> imageCache = new Dictionary<OxyImage, BitmapSource>();

        /// <summary>
        /// The set of images used in the current rendering cycle.
        /// </summary>
        private readonly HashSet<OxyImage> imagesInUse = new HashSet<OxyImage>();

        /// <summary>
        /// The list of rendered text elements for hit-testing.
        /// </summary>
        private readonly List<TextHitResult> renderedText = new List<TextHitResult>();

        /// <summary>
        /// The active drawing context, or null if not currently rendering.
        /// </summary>
        private DrawingContext dc;

        /// <summary>
        /// Whether a clipping region is currently pushed on the drawing context.
        /// </summary>
        private bool clipPushed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingVisualRenderContext"/> class.
        /// </summary>
        public DrawingVisualRenderContext()
        {
            this.RendersToScreen = true;
        }

        /// <summary>
        /// Gets or sets the DPI scale factor.
        /// </summary>
        public double DpiScale { get; set; } = 1;

        /// <summary>
        /// Gets or sets the text formatting mode.
        /// </summary>
        /// <value>The text formatting mode. The default value is <see cref="System.Windows.Media.TextFormattingMode.Display"/>.</value>
        public TextFormattingMode TextFormattingMode { get; set; } = TextFormattingMode.Display;

        /// <summary>
        /// Gets or sets the visual offset for pixel snapping calculations.
        /// </summary>
        public Point VisualOffset { get; set; }

        /// <summary>
        /// Opens a <see cref="DrawingContext"/> on the specified <see cref="DrawingVisual"/> for rendering.
        /// Must be paired with a call to <see cref="CloseDrawing"/>.
        /// </summary>
        /// <param name="visual">The drawing visual to render into.</param>
        public void OpenDrawing(DrawingVisual visual)
        {
            this.dc = visual.RenderOpen();
            this.clipPushed = false;
            this.renderedText.Clear();
        }

        /// <summary>
        /// Closes the current <see cref="DrawingContext"/>, finalizing the rendered content.
        /// </summary>
        public void CloseDrawing()
        {
            if (this.dc != null)
            {
                this.dc.Close();
                this.dc = null;
            }
        }

        /// <summary>
        /// Sets the clipping area to the specified rectangle.
        /// Called by <see cref="ClippingRenderContext"/> when the active clip region changes.
        /// </summary>
        /// <param name="clippingRect">The clipping rectangle.</param>
        protected override void SetClip(OxyRect clippingRect)
        {
            if (this.dc == null)
            {
                return;
            }

            this.dc.PushClip(new RectangleGeometry(ToRect(clippingRect)));
            this.clipPushed = true;
        }

        /// <summary>
        /// Resets the clipping area.
        /// Called by <see cref="ClippingRenderContext"/> when the active clip region is removed.
        /// </summary>
        protected override void ResetClip()
        {
            if (this.dc == null || !this.clipPushed)
            {
                return;
            }

            this.dc.Pop();
            this.clipPushed = false;
        }

        /// <inheritdoc/>
        public override void DrawLine(
            IList<ScreenPoint> points,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin)
        {
            if (this.dc == null || points.Count < 2)
            {
                return;
            }

            var pen = this.CreatePen(stroke, thickness, edgeRenderingMode, lineJoin, dashArray);
            if (pen == null)
            {
                return;
            }

            var actualThickness = this.GetActualStrokeThickness(thickness, edgeRenderingMode);
            bool snap = this.ShouldSnapPoints(edgeRenderingMode, points);

            var sg = new StreamGeometry();
            using (var sgc = sg.Open())
            {
                var p0 = this.ToPoint(points[0], actualThickness, snap);
                sgc.BeginFigure(p0, false, false);
                for (int i = 1; i < points.Count; i++)
                {
                    sgc.LineTo(this.ToPoint(points[i], actualThickness, snap), true, false);
                }
            }

            sg.Freeze();
            this.dc.DrawGeometry(null, pen, sg);
        }

        /// <inheritdoc/>
        public override void DrawLineSegments(
            IList<ScreenPoint> points,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin)
        {
            if (this.dc == null || points.Count < 2)
            {
                return;
            }

            var pen = this.CreatePen(stroke, thickness, edgeRenderingMode, lineJoin, dashArray);
            if (pen == null)
            {
                return;
            }

            var actualThickness = this.GetActualStrokeThickness(thickness, edgeRenderingMode);
            bool snap = this.ShouldSnapPoints(edgeRenderingMode, points);

            var sg = new StreamGeometry();
            using (var sgc = sg.Open())
            {
                var p0 = this.ToPoint(points[0], actualThickness, snap);
                sgc.BeginFigure(p0, false, false);
                for (int i = 1; i < points.Count; i++)
                {
                    // Alternate stroked/unstroked: odd indices (segment endpoints) are stroked,
                    // even indices (start of next segment) are unstroked moves.
                    sgc.LineTo(this.ToPoint(points[i], actualThickness, snap), (i & 1) != 0, true);
                }
            }

            sg.Freeze();
            this.dc.DrawGeometry(null, pen, sg);
        }

        /// <inheritdoc/>
        public override void DrawPolygon(
            IList<ScreenPoint> points,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin)
        {
            if (this.dc == null || points.Count < 2)
            {
                return;
            }

            var pen = this.CreatePen(stroke, thickness, edgeRenderingMode, lineJoin, dashArray);
            var brush = fill.IsUndefined() ? null : this.GetCachedBrush(fill);
            if (pen == null && brush == null)
            {
                return;
            }

            var actualThickness = pen != null ? pen.Thickness : 0;
            bool snap = this.ShouldSnapPoints(edgeRenderingMode, points);

            var sg = new StreamGeometry { FillRule = FillRule.Nonzero };
            using (var sgc = sg.Open())
            {
                var p0 = this.ToPoint(points[0], actualThickness, snap);
                sgc.BeginFigure(p0, brush != null, true);
                for (int i = 1; i < points.Count; i++)
                {
                    sgc.LineTo(this.ToPoint(points[i], actualThickness, snap), pen != null, false);
                }
            }

            sg.Freeze();
            this.dc.DrawGeometry(brush, pen, sg);
        }

        /// <inheritdoc/>
        public override void DrawPolygons(
            IList<IList<ScreenPoint>> polygons,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            double[] dashArray,
            LineJoin lineJoin)
        {
            if (this.dc == null || polygons.Count == 0)
            {
                return;
            }

            var pen = this.CreatePen(stroke, thickness, edgeRenderingMode, lineJoin, dashArray);
            var brush = fill.IsUndefined() ? null : this.GetCachedBrush(fill);
            if (pen == null && brush == null)
            {
                return;
            }

            var actualThickness = pen != null ? pen.Thickness : 0;

            var sg = new StreamGeometry { FillRule = FillRule.Nonzero };
            using (var sgc = sg.Open())
            {
                foreach (var polygon in polygons)
                {
                    if (polygon.Count == 0)
                    {
                        continue;
                    }

                    bool snap = this.ShouldSnapPoints(edgeRenderingMode, polygon);
                    var p0 = this.ToPoint(polygon[0], actualThickness, snap);
                    sgc.BeginFigure(p0, brush != null, true);
                    for (int i = 1; i < polygon.Count; i++)
                    {
                        sgc.LineTo(this.ToPoint(polygon[i], actualThickness, snap), pen != null, false);
                    }
                }
            }

            sg.Freeze();
            this.dc.DrawGeometry(brush, pen, sg);
        }

        /// <inheritdoc/>
        public override void DrawRectangle(
            OxyRect rect,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode)
        {
            if (this.dc == null)
            {
                return;
            }

            var brush = fill.IsUndefined() ? null : this.GetCachedBrush(fill);
            var pen = this.CreatePen(stroke, thickness, edgeRenderingMode);
            if (brush == null && pen == null)
            {
                return;
            }

            var actualRect = this.GetActualRect(rect, thickness, edgeRenderingMode);
            this.dc.DrawRectangle(brush, pen, actualRect);
        }

        /// <inheritdoc/>
        public override void DrawRectangles(
            IList<OxyRect> rectangles,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode)
        {
            if (this.dc == null || rectangles.Count == 0)
            {
                return;
            }

            var brush = fill.IsUndefined() ? null : this.GetCachedBrush(fill);
            var pen = this.CreatePen(stroke, thickness, edgeRenderingMode);
            if (brush == null && pen == null)
            {
                return;
            }

            var sg = new StreamGeometry { FillRule = FillRule.Nonzero };
            using (var sgc = sg.Open())
            {
                foreach (var rect in rectangles)
                {
                    var r = this.GetActualRect(rect, thickness, edgeRenderingMode);
                    sgc.BeginFigure(r.TopLeft, brush != null, true);
                    sgc.PolyLineTo(new[] { r.TopRight, r.BottomRight, r.BottomLeft }, pen != null, false);
                }
            }

            sg.Freeze();
            this.dc.DrawGeometry(brush, pen, sg);
        }

        /// <inheritdoc/>
        public override void DrawEllipse(
            OxyRect rect,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode)
        {
            if (this.dc == null)
            {
                return;
            }

            var brush = fill.IsUndefined() ? null : this.GetCachedBrush(fill);
            var pen = this.CreatePen(stroke, thickness, edgeRenderingMode);
            if (brush == null && pen == null)
            {
                return;
            }

            var center = new Point(rect.Center.X, rect.Center.Y);
            this.dc.DrawEllipse(brush, pen, center, rect.Width / 2, rect.Height / 2);
        }

        /// <inheritdoc/>
        public override void DrawEllipses(
            IList<OxyRect> rectangles,
            OxyColor fill,
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode)
        {
            if (this.dc == null || rectangles.Count == 0)
            {
                return;
            }

            var brush = fill.IsUndefined() ? null : this.GetCachedBrush(fill);
            var pen = this.CreatePen(stroke, thickness, edgeRenderingMode);
            if (brush == null && pen == null)
            {
                return;
            }

            var sg = new StreamGeometry { FillRule = FillRule.Nonzero };
            using (var sgc = sg.Open())
            {
                foreach (var rect in rectangles)
                {
                    var centerY = rect.Center.Y;
                    sgc.BeginFigure(new Point(rect.Right, centerY), brush != null, true);
                    var size = new Size(rect.Width / 2, rect.Height / 2);
                    sgc.ArcTo(new Point(rect.Left, centerY), size, 180, false, SweepDirection.Clockwise, pen != null, false);
                    sgc.ArcTo(new Point(rect.Right, centerY), size, 180, false, SweepDirection.Clockwise, pen != null, false);
                }
            }

            sg.Freeze();
            this.dc.DrawGeometry(brush, pen, sg);
        }

        /// <inheritdoc/>
        public override void DrawText(
            ScreenPoint p,
            string text,
            OxyColor fill,
            string fontFamily,
            double fontSize,
            double fontWeight,
            double rotate,
            HorizontalAlignment halign,
            VerticalAlignment valign,
            OxySize? maxSize)
        {
            if (this.dc == null || string.IsNullOrEmpty(text))
            {
                return;
            }

            var brush = this.GetCachedBrush(fill);
            if (brush == null)
            {
                return;
            }

            var typeface = this.CreateTypeface(fontFamily, fontWeight);
            var ft = new FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize > 0 ? fontSize : 12,
                brush,
                null,
                this.TextFormattingMode,
                this.DpiScale);

            double textWidth = ft.Width;
            double textHeight = ft.Height;

            if (maxSize != null)
            {
                if (textWidth > maxSize.Value.Width + 1e-3)
                {
                    textWidth = Math.Max(maxSize.Value.Width, 0);
                }

                if (textHeight > maxSize.Value.Height + 1e-3)
                {
                    textHeight = Math.Max(maxSize.Value.Height, 0);
                }
            }

            double dx = 0;
            double dy = 0;

            if (halign == HorizontalAlignment.Center)
            {
                dx = -textWidth / 2;
            }
            else if (halign == HorizontalAlignment.Right)
            {
                dx = -textWidth;
            }

            if (valign == VerticalAlignment.Middle)
            {
                dy = -textHeight / 2;
            }
            else if (valign == VerticalAlignment.Bottom)
            {
                dy = -textHeight;
            }

            bool hasRotation = Math.Abs(rotate) > double.Epsilon;

            if (hasRotation)
            {
                this.dc.PushTransform(new TranslateTransform(p.X, p.Y));
                this.dc.PushTransform(new RotateTransform(rotate));
                this.dc.DrawText(ft, new Point(dx, dy));
                this.dc.Pop();
                this.dc.Pop();
            }
            else
            {
                this.dc.DrawText(ft, new Point(p.X + dx, p.Y + dy));
            }

            // Record text position for hit testing
            var bounds = ComputeTextBounds(p.X, p.Y, dx, dy, textWidth, textHeight, rotate);
            this.renderedText.Add(new TextHitResult(text, bounds, fontSize));
        }

        /// <inheritdoc/>
        public override OxySize MeasureText(string text, string fontFamily, double fontSize, double fontWeight)
        {
            if (string.IsNullOrEmpty(text))
            {
                return OxySize.Empty;
            }

            var typeface = this.CreateTypeface(fontFamily, fontWeight);
            var ft = new FormattedText(
                text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                typeface,
                fontSize > 0 ? fontSize : 12,
                Brushes.Black,
                null,
                this.TextFormattingMode,
                this.DpiScale);

            return new OxySize(ft.Width, ft.Height);
        }

        /// <inheritdoc/>
        public override void DrawImage(
            OxyImage source,
            double srcX,
            double srcY,
            double srcWidth,
            double srcHeight,
            double destX,
            double destY,
            double destWidth,
            double destHeight,
            double opacity,
            bool interpolate)
        {
            if (this.dc == null || destWidth <= 0 || destHeight <= 0 || srcWidth <= 0 || srcHeight <= 0)
            {
                return;
            }

            var bitmapChain = this.GetImageSource(source);
            if (bitmapChain == null)
            {
                return;
            }

            // Crop the source region if it differs from the full image.
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (srcX != 0 || srcY != 0 || srcWidth != bitmapChain.PixelWidth || srcHeight != bitmapChain.PixelHeight)
            // ReSharper restore CompareOfFloatsByEqualityOperator
            {
                bitmapChain = new CroppedBitmap(bitmapChain, new Int32Rect((int)srcX, (int)srcY, (int)srcWidth, (int)srcHeight));
            }

            bool applyOpacity = Math.Abs(opacity - 1.0) > double.Epsilon;
            if (applyOpacity)
            {
                this.dc.PushOpacity(opacity);
            }

            this.dc.DrawImage(bitmapChain, new Rect(destX, destY, destWidth, destHeight));

            if (applyOpacity)
            {
                this.dc.Pop();
            }
        }

        /// <inheritdoc/>
        public override void SetToolTip(string text)
        {
            // No-op: tooltips are handled at the model level via TrackerManipulator.
            // The tracker popup is rendered on the overlays Canvas, which is independent of this render context.
        }

        /// <inheritdoc/>
        public override void CleanUp()
        {
            // Remove unreferenced images from the cache.
            var imagesToRelease = this.imageCache.Keys.Where(i => !this.imagesInUse.Contains(i)).ToList();
            foreach (var i in imagesToRelease)
            {
                this.imageCache.Remove(i);
            }

            this.imagesInUse.Clear();
        }

        /// <summary>
        /// Returns the topmost rendered text element at the given point, or null.
        /// Searches back-to-front (topmost first) for the text element at the specified point.
        /// </summary>
        /// <param name="point">The point to test, in plot coordinates.</param>
        /// <returns>A <see cref="TextHitResult"/> if a text element was hit; otherwise, null.</returns>
        /// <remarks>
        /// This provides backend-agnostic text hit-testing equivalent to
        /// <c>Canvas.InputHitTest()</c> returning a <c>TextBlock</c> in the Canvas backend.
        /// Used by the OxyPlotToolbar for text editing initiation.
        /// </remarks>
        public TextHitResult HitTestText(ScreenPoint point)
        {
            for (int i = this.renderedText.Count - 1; i >= 0; i--)
            {
                if (this.renderedText[i].Bounds.Contains(point.X, point.Y))
                {
                    return this.renderedText[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a cached frozen brush for the specified color.
        /// </summary>
        /// <param name="color">The OxyPlot color.</param>
        /// <returns>A frozen <see cref="SolidColorBrush"/>, or null if the color is fully transparent.</returns>
        private Brush GetCachedBrush(OxyColor color)
        {
            if (color.A == 0)
            {
                return null;
            }

            if (!this.brushCache.TryGetValue(color, out var brush))
            {
                brush = new SolidColorBrush(color.ToColor());
                brush.Freeze();
                this.brushCache.Add(color, brush);
            }

            return brush;
        }

        /// <summary>
        /// Gets or creates a cached frozen <see cref="Pen"/> for the specified stroke parameters.
        /// Pens with identical parameters are reused within and across frames, avoiding repeated
        /// allocation and Freeze() overhead for gridlines, axes, and series that share the same stroke.
        /// </summary>
        /// <param name="stroke">The stroke color.</param>
        /// <param name="thickness">The stroke thickness.</param>
        /// <param name="edgeRenderingMode">The edge rendering mode (affects thickness snapping).</param>
        /// <param name="lineJoin">The line join style.</param>
        /// <param name="dashArray">The dash pattern, or null for solid lines.</param>
        /// <returns>A frozen <see cref="Pen"/>, or null if the stroke is undefined or thickness is zero.</returns>
        private Pen CreatePen(
            OxyColor stroke,
            double thickness,
            EdgeRenderingMode edgeRenderingMode,
            LineJoin lineJoin = LineJoin.Miter,
            double[] dashArray = null)
        {
            if (stroke.IsUndefined() || thickness <= 0)
            {
                return null;
            }

            var actualThickness = this.GetActualStrokeThickness(thickness, edgeRenderingMode);

            // Compute cache key from pen parameters
            long key = ComputePenCacheKey(stroke, actualThickness, lineJoin, dashArray);
            if (this.penCache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var brush = this.GetCachedBrush(stroke);
            if (brush == null)
            {
                return null;
            }

            var pen = new Pen(brush, actualThickness);

            switch (lineJoin)
            {
                case LineJoin.Round:
                    pen.LineJoin = PenLineJoin.Round;
                    break;
                case LineJoin.Bevel:
                    pen.LineJoin = PenLineJoin.Bevel;
                    break;
            }

            if (dashArray != null)
            {
                pen.DashStyle = new DashStyle(dashArray, 0);

                // WPF Pen.DashCap defaults to PenLineCap.Square, which extends each dash
                // by half the stroke thickness at each end. OxyPlot's dash arrays use
                // 1-unit gaps (e.g., {4,1} for Dash), so square caps fill the gap entirely,
                // making all dash styles render as solid lines.
                // Shape.StrokeDashCap (used by CanvasRenderContext) defaults to Flat.
                // Match that behavior here for 1:1 parity.
                pen.DashCap = PenLineCap.Flat;
            }

            pen.Freeze();
            this.penCache[key] = pen;
            return pen;
        }

        /// <summary>
        /// Computes a cache key for pen parameters.
        /// Combines color, thickness, line join, and dash array hash into a single long value.
        /// </summary>
        private static long ComputePenCacheKey(OxyColor color, double thickness, LineJoin lineJoin, double[] dashArray)
        {
            unchecked
            {
                int colorHash = (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
                int dashHash = 0;
                if (dashArray != null)
                {
                    dashHash = dashArray.Length;
                    for (int i = 0; i < dashArray.Length; i++)
                    {
                        dashHash = dashHash * 31 + dashArray[i].GetHashCode();
                    }
                }

                // Use multiplicative hash combining instead of XOR to avoid collisions.
                // Each component is mixed with a prime multiplier before combining.
                long upper = (long)colorHash << 32;
                int lower = thickness.GetHashCode();
                lower = lower * 397 + (int)lineJoin;
                lower = lower * 397 + dashHash;
                return upper | (lower & 0xFFFFFFFFL);
            }
        }

        /// <summary>
        /// Creates a typeface for the specified font family and weight.
        /// </summary>
        /// <param name="fontFamily">The font family name, or null for the default font.</param>
        /// <param name="fontWeight">The font weight value.</param>
        /// <returns>A <see cref="Typeface"/> instance.</returns>
        private Typeface CreateTypeface(string fontFamily, double fontWeight)
        {
            var ff = fontFamily != null ? this.GetCachedFontFamily(fontFamily) : new FontFamily("Segoe UI");
            var fw = fontWeight > FontWeights.Normal
                ? System.Windows.FontWeights.Bold
                : System.Windows.FontWeights.Normal;
            return new Typeface(ff, FontStyles.Normal, fw, FontStretches.Normal);
        }

        /// <summary>
        /// Gets a cached font family for the specified name.
        /// </summary>
        /// <param name="familyName">The font family name.</param>
        /// <returns>A <see cref="FontFamily"/> instance.</returns>
        private FontFamily GetCachedFontFamily(string familyName)
        {
            if (familyName == null)
            {
                return null;
            }

            if (!this.fontFamilyCache.TryGetValue(familyName, out var ff))
            {
                ff = new FontFamily(familyName);
                this.fontFamilyCache.Add(familyName, ff);
            }

            return ff;
        }

        /// <summary>
        /// Gets a cached bitmap source for the specified image.
        /// </summary>
        /// <param name="image">The OxyPlot image.</param>
        /// <returns>A <see cref="BitmapSource"/>, or null if the image is null.</returns>
        private BitmapSource GetImageSource(OxyImage image)
        {
            if (image == null)
            {
                return null;
            }

            if (!this.imagesInUse.Contains(image))
            {
                this.imagesInUse.Add(image);
            }

            if (this.imageCache.TryGetValue(image, out var src))
            {
                return src;
            }

            using (var ms = new MemoryStream(image.GetData()))
            {
                var btm = new BitmapImage();
                btm.BeginInit();
                btm.StreamSource = ms;
                btm.CacheOption = BitmapCacheOption.OnLoad;
                btm.EndInit();
                btm.Freeze();
                this.imageCache.Add(image, btm);
                return btm;
            }
        }

        /// <summary>
        /// Gets the actual stroke thickness, snapped to device pixels for <see cref="EdgeRenderingMode.PreferSharpness"/>.
        /// </summary>
        /// <param name="thickness">The requested stroke thickness.</param>
        /// <param name="edgeRenderingMode">The edge rendering mode.</param>
        /// <returns>The actual stroke thickness.</returns>
        private double GetActualStrokeThickness(double thickness, EdgeRenderingMode edgeRenderingMode)
        {
            switch (edgeRenderingMode)
            {
                case EdgeRenderingMode.PreferSharpness:
                    return PixelLayout.SnapStrokeThickness(thickness, this.DpiScale);
                default:
                    return thickness;
            }
        }

        /// <summary>
        /// Gets the actual rectangle, snapped to device pixels based on edge rendering mode.
        /// </summary>
        /// <param name="rect">The requested rectangle.</param>
        /// <param name="thickness">The stroke thickness.</param>
        /// <param name="edgeRenderingMode">The edge rendering mode.</param>
        /// <returns>The actual rectangle.</returns>
        private Rect GetActualRect(OxyRect rect, double thickness, EdgeRenderingMode edgeRenderingMode)
        {
            switch (edgeRenderingMode)
            {
                case EdgeRenderingMode.PreferGeometricAccuracy:
                case EdgeRenderingMode.PreferSpeed:
                    return ToRect(rect);
                default:
                    return PixelLayout.Snap(ToRect(rect), thickness, this.VisualOffset, this.DpiScale);
            }
        }

        /// <summary>
        /// Determines whether points should be snapped to device pixels.
        /// </summary>
        /// <param name="edgeRenderingMode">The edge rendering mode.</param>
        /// <param name="points">The points to check.</param>
        /// <returns>True if points should be pixel-snapped.</returns>
        private bool ShouldSnapPoints(EdgeRenderingMode edgeRenderingMode, IList<ScreenPoint> points)
        {
            switch (edgeRenderingMode)
            {
                case EdgeRenderingMode.PreferSharpness:
                    return true;
                case EdgeRenderingMode.Adaptive:
                case EdgeRenderingMode.Automatic:
                    return IsStraightLine(points);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Converts a <see cref="ScreenPoint"/> to a <see cref="Point"/>, optionally snapping to device pixels.
        /// </summary>
        /// <param name="sp">The screen point.</param>
        /// <param name="strokeThickness">The stroke thickness (for pixel snapping offset calculation).</param>
        /// <param name="snap">Whether to snap to device pixels.</param>
        /// <returns>The WPF point.</returns>
        private Point ToPoint(ScreenPoint sp, double strokeThickness, bool snap)
        {
            if (snap)
            {
                return PixelLayout.Snap(sp.X, sp.Y, strokeThickness, this.VisualOffset, this.DpiScale);
            }

            return new Point(sp.X, sp.Y);
        }

        /// <summary>
        /// Computes the axis-aligned bounding box of a text element, accounting for rotation.
        /// </summary>
        /// <param name="px">The text position X.</param>
        /// <param name="py">The text position Y.</param>
        /// <param name="dx">The alignment offset X.</param>
        /// <param name="dy">The alignment offset Y.</param>
        /// <param name="width">The text width.</param>
        /// <param name="height">The text height.</param>
        /// <param name="rotate">The rotation angle in degrees.</param>
        /// <returns>The bounding rectangle of the text.</returns>
        private static OxyRect ComputeTextBounds(
            double px, double py, double dx, double dy, double width, double height, double rotate)
        {
            if (Math.Abs(rotate) < double.Epsilon)
            {
                return new OxyRect(px + dx, py + dy, width, height);
            }

            double rad = rotate * Math.PI / 180;
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            // The four corners of the text rect, relative to the rotation origin.
            double x0 = dx, y0 = dy;
            double x1 = dx + width, y1 = dy;
            double x2 = dx + width, y2 = dy + height;
            double x3 = dx, y3 = dy + height;

            // Rotate each corner and translate to position.
            double rx0 = x0 * cos - y0 * sin + px;
            double ry0 = x0 * sin + y0 * cos + py;
            double rx1 = x1 * cos - y1 * sin + px;
            double ry1 = x1 * sin + y1 * cos + py;
            double rx2 = x2 * cos - y2 * sin + px;
            double ry2 = x2 * sin + y2 * cos + py;
            double rx3 = x3 * cos - y3 * sin + px;
            double ry3 = x3 * sin + y3 * cos + py;

            double minX = Math.Min(Math.Min(rx0, rx1), Math.Min(rx2, rx3));
            double minY = Math.Min(Math.Min(ry0, ry1), Math.Min(ry2, ry3));
            double maxX = Math.Max(Math.Max(rx0, rx1), Math.Max(rx2, rx3));
            double maxY = Math.Max(Math.Max(ry0, ry1), Math.Max(ry2, ry3));

            return new OxyRect(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Converts an <see cref="OxyRect"/> to a WPF <see cref="Rect"/>.
        /// </summary>
        /// <param name="r">The OxyPlot rectangle.</param>
        /// <returns>The WPF rectangle.</returns>
        private static Rect ToRect(OxyRect r)
        {
            return new Rect(r.Left, r.Top, r.Width, r.Height);
        }

        /// <summary>
        /// Represents a rendered text element for hit-testing.
        /// </summary>
        public class TextHitResult
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TextHitResult"/> class.
            /// </summary>
            /// <param name="text">The text content.</param>
            /// <param name="bounds">The bounding rectangle of the text.</param>
            /// <param name="fontSize">The font size.</param>
            public TextHitResult(string text, OxyRect bounds, double fontSize)
            {
                this.Text = text;
                this.Bounds = bounds;
                this.FontSize = fontSize;
            }

            /// <summary>
            /// Gets the text content.
            /// </summary>
            public string Text { get; }

            /// <summary>
            /// Gets the axis-aligned bounding rectangle of the rendered text.
            /// </summary>
            public OxyRect Bounds { get; }

            /// <summary>
            /// Gets the font size used to render the text.
            /// </summary>
            public double FontSize { get; }
        }
    }
}
