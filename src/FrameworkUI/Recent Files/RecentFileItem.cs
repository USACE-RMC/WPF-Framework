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

        /// <summary>
        /// Gets the file name without extension.
        /// </summary>
        public string Name
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(FilePath); }
        }

        /// <summary>
        /// Gets the shortened display location of the file.
        /// </summary>
        public string DisplayLocation
        {
            get { return UtilityFunctions.ShortenPathname(System.IO.Path.GetDirectoryName(FilePath) ?? string.Empty, 50); }
        }

        /// <summary>
        /// Gets or sets the recent file path.
        /// </summary>
        public string FilePath { get; set; } = "";

        /// <summary>
        /// Gets or sets the file menu item.
        /// </summary>
        public MenuItem? MenuItem { get; set; }

        /// <summary>
        /// Gets the recent file display path.
        /// </summary>
        public string DisplayPath
        {
            get { return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FilePath) ?? string.Empty, System.IO.Path.GetFileNameWithoutExtension(FilePath)); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentFileItem"/> class.
        /// </summary>
        /// <param name="filePath">The full file path.</param>
        public RecentFileItem(string filePath)
        {
            FilePath = filePath;
        }

    }
}
