using System.Collections;
using System.Reflection;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Wpf;
using Xunit;

namespace OxyPlotControls.Tests.Rendering;

/// <summary>
/// Tests for the Phase 1 Item 1.7 <see cref="FormattedText"/> cache in
/// <see cref="DrawingVisualRenderContext.DrawText"/>.
/// </summary>
/// <remarks>
/// The cache mirrors the existing MeasureText cache. Axis tick labels, axis titles, and
/// legend entries typically repeat across frames; caching the FormattedText eliminates
/// ~100 allocations per render on a typical plot. These tests verify:
/// 1. Identical inputs produce a cache hit (same reference returned).
/// 2. Varying any cache-key component (text / family / size / weight / color) produces a miss.
/// 3. Changing DpiScale clears the cache (dimensions would be wrong).
/// </remarks>
public class DrawTextCacheTests
{
    /// <summary>
    /// Accessor for the private <c>drawTextCache</c> field so tests can inspect its contents
    /// without exposing implementation details on the public API.
    /// </summary>
    private static IDictionary GetDrawTextCache(DrawingVisualRenderContext rc)
    {
        var field = typeof(DrawingVisualRenderContext).GetField(
            "drawTextCache",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        return (IDictionary)field.GetValue(rc)!;
    }

    /// <summary>
    /// Two DrawText calls with identical inputs should result in a single cache entry
    /// reused across both calls — the second call must NOT allocate a new FormattedText.
    /// </summary>
    [Fact]
    public void DrawText_IdenticalInputs_ReusesCachedFormattedText()
    {
        var rc = new DrawingVisualRenderContext();
        var visual = new DrawingVisual();
        rc.OpenDrawing(visual);

        rc.DrawText(
            new ScreenPoint(0, 0),
            "Hello",
            OxyColors.Black,
            "Segoe UI",
            12,
            400,
            0,
            HorizontalAlignment.Left,
            VerticalAlignment.Top,
            null);

        rc.DrawText(
            new ScreenPoint(10, 10),
            "Hello",
            OxyColors.Black,
            "Segoe UI",
            12,
            400,
            0,
            HorizontalAlignment.Left,
            VerticalAlignment.Top,
            null);

        rc.CloseDrawing();

        var cache = GetDrawTextCache(rc);
        Assert.Single(cache);
    }

    /// <summary>
    /// Varying any component of the cache key (here: fontSize) should produce a second
    /// cache entry — the cache must not accidentally collapse distinct renders.
    /// </summary>
    [Fact]
    public void DrawText_DifferentFontSize_AllocatesSeparateCacheEntry()
    {
        var rc = new DrawingVisualRenderContext();
        var visual = new DrawingVisual();
        rc.OpenDrawing(visual);

        rc.DrawText(new ScreenPoint(0, 0), "Label", OxyColors.Black, "Segoe UI", 12, 400, 0,
            HorizontalAlignment.Left, VerticalAlignment.Top, null);
        rc.DrawText(new ScreenPoint(0, 0), "Label", OxyColors.Black, "Segoe UI", 14, 400, 0,
            HorizontalAlignment.Left, VerticalAlignment.Top, null);

        rc.CloseDrawing();

        var cache = GetDrawTextCache(rc);
        Assert.Equal(2, cache.Count);
    }

    /// <summary>
    /// Different colors must NOT share a FormattedText because its foreground brush is
    /// embedded in the object.
    /// </summary>
    [Fact]
    public void DrawText_DifferentColor_AllocatesSeparateCacheEntry()
    {
        var rc = new DrawingVisualRenderContext();
        var visual = new DrawingVisual();
        rc.OpenDrawing(visual);

        rc.DrawText(new ScreenPoint(0, 0), "Same", OxyColors.Red, "Segoe UI", 12, 400, 0,
            HorizontalAlignment.Left, VerticalAlignment.Top, null);
        rc.DrawText(new ScreenPoint(0, 0), "Same", OxyColors.Blue, "Segoe UI", 12, 400, 0,
            HorizontalAlignment.Left, VerticalAlignment.Top, null);

        rc.CloseDrawing();

        var cache = GetDrawTextCache(rc);
        Assert.Equal(2, cache.Count);
    }

    /// <summary>
    /// Setting <see cref="DrawingVisualRenderContext.DpiScale"/> to a new value must clear
    /// the FormattedText cache; cached dimensions are DPI-dependent and reusing them at
    /// a different DPI would produce incorrect layout.
    /// </summary>
    [Fact]
    public void DpiScaleChange_ClearsDrawTextCache()
    {
        var rc = new DrawingVisualRenderContext();
        var visual = new DrawingVisual();
        rc.OpenDrawing(visual);

        rc.DrawText(new ScreenPoint(0, 0), "Text", OxyColors.Black, "Segoe UI", 12, 400, 0,
            HorizontalAlignment.Left, VerticalAlignment.Top, null);

        rc.CloseDrawing();

        var cache = GetDrawTextCache(rc);
        Assert.Single(cache);

        rc.DpiScale = 1.5;

        Assert.Empty(cache);
    }
}
