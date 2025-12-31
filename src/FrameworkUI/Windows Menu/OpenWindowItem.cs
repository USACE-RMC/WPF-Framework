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
        /// Gets and sets the window menu item.
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
