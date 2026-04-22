// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WpfPdfRenderContext.cs" company="OxyPlot">
//   Copyright (c) 2020 OxyPlot contributors
// </copyright>
// <summary>
//   Provides a PDF render context that uses WPF text geometry for Unicode support.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Wpf
{
    using System;

    /// <summary>
    /// Provides a PDF render context that converts text to geometry paths using WPF's
    /// <see cref="System.Windows.Media.FormattedText"/>, enabling correct rendering of
    /// all Unicode characters including Greek letters and mathematical symbols.
    /// </summary>
    public class WpfPdfRenderContext : PdfRenderContext
    {
        /// <summary>
        /// DIP to points conversion factor. OxyPlot fontSize is in DIP (1/96 inch),
        /// PDF coordinates are in points (1/72 inch).
        /// </summary>
        private const double DipToPoints = 72.0 / 96.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfPdfRenderContext"/> class.
        /// </summary>
        /// <param name="width">The page width in points (1/72 inch).</param>
        /// <param name="height">The page height in points (1/72 inch).</param>
        /// <param name="background">The background color.</param>
        public WpfPdfRenderContext(double width, double height, OxyColor background)
            : base(width, height, background)
        {
        }

        /// <summary>
        /// Draws text at the specified position by converting it to geometry paths.
        /// </summary>
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
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            // fontSize is in DIP (1/96 inch). Measure in DIP then convert to points.
            var textSizeDip = TextToGeometryHelper.MeasureText(text, fontFamily, fontSize, fontWeight);
            double width = textSizeDip.Width * DipToPoints;
            double height = textSizeDip.Height * DipToPoints;

            if (maxSize != null)
            {
                if (width > maxSize.Value.Width)
                {
                    width = Math.Max(maxSize.Value.Width, 0);
                }

                if (height > maxSize.Value.Height)
                {
                    height = Math.Max(maxSize.Value.Height, 0);
                }
            }

            // Compute alignment offsets in points (same logic as base PdfRenderContext)
            double dx = 0;
            if (halign == HorizontalAlignment.Center)
            {
                dx = -width / 2;
            }

            if (halign == HorizontalAlignment.Right)
            {
                dx = -width;
            }

            double dy = 0;
            if (valign == VerticalAlignment.Middle)
            {
                dy = -height / 2;
            }

            if (valign == VerticalAlignment.Top)
            {
                dy = -height;
            }

            // Build text geometry in WPF coordinates (Y-down, units = DIP)
            var pathGeometry = TextToGeometryHelper.BuildGeometry(text, fontFamily, fontSize, fontWeight);

            this.doc.SaveState();
            this.doc.SetFillColor(fill);

            // p.X, p.Y are in points (model renders to a points-sized OxyRect).
            // Flip Y for PDF coordinate system (Y-up).
            double pdfY = this.doc.PageHeight - p.Y;
            this.doc.Translate(p.X, pdfY);

            if (Math.Abs(rotate) > 1e-6)
            {
                this.doc.Rotate(-rotate);
            }

            // Apply alignment offset (in points)
            this.doc.Translate(dx, dy);

            // The geometry is in DIP with Y-down. We need to:
            // 1. Scale DIP → points (multiply by DipToPoints)
            // 2. Flip Y (negate Y scale)
            // 3. Shift up by height so flipped text extends upward from baseline
            // Combined: Transform(scale, 0, 0, -scale, 0, height)
            // This maps WPF (0,0) to PDF (0, height) and WPF (0, h_dip) to PDF (0, 0)
            this.doc.Transform(DipToPoints, 0, 0, -DipToPoints, 0, height);

            TextToGeometryHelper.WriteGeometryToPdf(this.doc, pathGeometry);
            this.doc.Fill();

            this.doc.RestoreState();
        }

        /// <summary>
        /// Measures text using WPF font metrics. Returns size in points for consistency
        /// with the PDF coordinate system used by the model render rectangle.
        /// </summary>
        public override OxySize MeasureText(string text, string fontFamily, double fontSize, double fontWeight)
        {
            var sizeDip = TextToGeometryHelper.MeasureText(text, fontFamily, fontSize, fontWeight);
            return new OxySize(sizeDip.Width * DipToPoints, sizeDip.Height * DipToPoints);
        }
    }
}
