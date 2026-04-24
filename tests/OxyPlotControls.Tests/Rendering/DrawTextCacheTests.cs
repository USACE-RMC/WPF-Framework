/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* - Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* - Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* - The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

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
