using System.Windows.Controls;

namespace FrameworkUI
{
    /// <summary>
    /// A class for recent file items.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Authors:
    ///     Woody Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class RecentFileItem
    {

        public string Name
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(FilePath); }
        }

        public string DisplayLocation
        {
            get { return UtilityFunctions.ShortenPathname(System.IO.Path.GetDirectoryName(FilePath), 50); }
        }

        /// <summary>
        /// Gets and sets the recent file path.
        /// </summary>
        public string FilePath { get; set; } = "";

        /// <summary>
        /// Gets and sets the file menu item.
        /// </summary>
        public MenuItem MenuItem { get; set; } = null;

        /// <summary>
        /// Gets the recent file display path.
        /// </summary>
        public string DisplayPath
        {
            get { return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FilePath), System.IO.Path.GetFileNameWithoutExtension(FilePath)); }
        }

        /// <summary>
        /// Construct a new recent file item.
        /// </summary>
        /// <param name="filePath">The full file path.</param>
        public RecentFileItem(string filePath)
        {
            FilePath = filePath;
        }

    }
}
