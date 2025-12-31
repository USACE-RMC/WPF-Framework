using System.Windows.Media;
using GenericControls;
using FrameworkInterfaces;

namespace FrameworkUI
{

    /// <summary>
    /// A class for unsaved elements.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class UnsavedElement
    {

        /// <summary>
        /// Construct a new Unsaved Element.
        /// </summary>
        /// <param name="element">The unsaved element.</param>
        public UnsavedElement(IElement element)
        {
            Element = element;
        }

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
