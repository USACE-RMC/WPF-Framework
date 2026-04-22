// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DrawingVisualHost.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   A FrameworkElement that hosts a single DrawingVisual for immediate-mode rendering.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OxyPlot.Wpf
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// A <see cref="FrameworkElement"/> that hosts a single <see cref="DrawingVisual"/>
    /// for immediate-mode rendering via <see cref="DrawingVisualRenderContext"/>.
    /// </summary>
    /// <remarks>
    /// This is the standard WPF pattern for hosting a DrawingVisual inside the element tree.
    /// One element, one visual, zero layout overhead per rendered shape.
    /// </remarks>
    public class DrawingVisualHost : FrameworkElement
    {
        /// <summary>
        /// The hosted drawing visual.
        /// </summary>
        private readonly DrawingVisual visual = new DrawingVisual();

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingVisualHost"/> class.
        /// </summary>
        public DrawingVisualHost()
        {
            // Match CanvasRenderContext's text quality: use Display mode for pixel-snapped,
            // crisp text rendering. Without this, FormattedText drawn inside the DrawingVisual
            // uses Ideal mode, which produces blurrier text than the Canvas backend.
            // Set on both the FrameworkElement (this) and the DrawingVisual — WPF reads
            // TextFormattingMode from the nearest UIElement ancestor during DrawingContext.DrawText().
            TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
            TextOptions.SetTextFormattingMode(this.visual, TextFormattingMode.Display);

            // Enable ClearType sub-pixel rendering even when transforms (e.g., RotateTransform
            // for Y-axis titles) are applied. Without this hint, WPF falls back to grayscale
            // anti-aliasing for transformed text, producing noticeably blurrier results.
            // The Canvas backend sets ClearTypeHint.Enabled per TextBlock (CanvasRenderContext line 392);
            // setting it on the host element and visual achieves the same effect for DrawingVisual.
            RenderOptions.SetClearTypeHint(this, ClearTypeHint.Enabled);
            RenderOptions.SetClearTypeHint(this.visual, ClearTypeHint.Enabled);

            this.UseLayoutRounding = true;
            this.SnapsToDevicePixels = true;
            this.AddVisualChild(this.visual);
            this.AddLogicalChild(this.visual);
        }

        /// <summary>
        /// Gets the hosted <see cref="DrawingVisual"/>.
        /// </summary>
        public DrawingVisual Visual => this.visual;

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        protected override int VisualChildrenCount => 1;

        /// <summary>
        /// Returns the specified visual child.
        /// </summary>
        /// <param name="index">The index of the visual child.</param>
        /// <returns>The visual child at the specified index.</returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.visual;
        }
    }
}
