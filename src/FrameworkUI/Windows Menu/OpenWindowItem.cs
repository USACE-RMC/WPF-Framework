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

using System.Windows.Controls;
using System.Windows.Media;
using GenericControls;
using FrameworkInterfaces;
using Xceed.Wpf.AvalonDock.Layout;

namespace FrameworkUI
{

    /// <summary>
    /// A class for open window items.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class OpenWindowItem
    {
        /// <summary>
        /// Construct a new Window Item.
        /// </summary>
        /// <param name="document">The AvalonDock layout document that is displayed in the window.</param>
        /// <param name="element">The element that is bound to the document.</param>
        public OpenWindowItem(LayoutDocument document, IElement element)
        {
            Document = document;
            Element = element;
        }

        /// <summary>
        /// Gets the name of the window item.
        /// </summary>
        public string Name
        {
            get { return Document.Title; }
        }

        /// <summary>
        /// Gets or sets the window menu item.
        /// </summary>
        public MenuItem MenuItem { get; set; } = null;

        /// <summary>
        /// Gets AvalonDock layout document that is displayed in the window.
        /// </summary>
        public LayoutDocument Document { get; private set; } = null;

        /// <summary>
        /// Gets Element that is bound to the document.
        /// </summary>
        public IElement Element { get; private set; } = null;

        /// <summary>
        /// Gets the element image source.
        /// </summary>
        public ImageSource ImageSource
        {
            get { return GeneralMethods.Bitmap2BitmapSource(Element.ElementImage); }
        }


    }
}
