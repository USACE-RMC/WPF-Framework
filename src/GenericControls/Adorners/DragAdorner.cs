/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
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
using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace GenericControls
{
    /// <summary>
    /// An adorner that displays a semi-transparent visual representation of a UIElement during drag operations.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class DragAdorner : Adorner
    {

        private Brush _vBrush;
        private Point _location;
        // Private _offset As Double

        /// <summary>
        /// Initializes a new instance of the <see cref="DragAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The UIElement to display as a ghost image.</param>
        /// <param name="offset">The offset from the mouse pointer.</param>
        public DragAdorner(UIElement adornedElement, Point offset) : base(adornedElement)
        {
            IsHitTestVisible = false;
            Focusable = false;
            _vBrush = new VisualBrush(AdornedElement) { Stretch = Stretch.None, AlignmentX = AlignmentX.Left };
            _vBrush.Opacity = 0.8d;
        }

        /// <summary>
        /// Updates the position of the adorner on the screen.
        /// </summary>
        /// <param name="location">The new top-left location of the ghost element.</param>
        public void UpdatePosition(Point location)
        {
            _location = new Point(location.X, location.Y - 17.5d);
            InvalidateVisual();
        }

        /// <summary>
        /// Draws the visual representation of the drag adorner.
        /// </summary>
        /// <param name="dc">The drawing context to render into.</param>
        protected override void OnRender(DrawingContext dc)
        {
            dc.PushOpacityMask(new LinearGradientBrush(Colors.White, Colors.Transparent, 45d));
            dc.DrawRectangle(_vBrush, null, new Rect(_location.X, _location.Y, Math.Min(RenderSize.Width, 500d), Math.Min(RenderSize.Height, 400d)));
        }
    }
}