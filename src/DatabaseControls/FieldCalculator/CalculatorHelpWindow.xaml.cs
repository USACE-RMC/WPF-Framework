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
using System.IO;
using System.Reflection;

namespace DatabaseControls
{
    /// <summary>
    /// A window that displays help documentation for the calculator functionality.
    /// Loads embedded HTML help resources for FieldCalculator or SelectByAttribute.
    /// </summary>
    public partial class CalculatorHelpWindow : GenericControls.MetroWindow
    {
        private readonly string _helpResourcePrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorHelpWindow"/> class.
        /// </summary>
        /// <param name="helpType">The type of help to display: "FieldCalculator" or "SelectByAttribute".</param>
        public CalculatorHelpWindow(string helpType = "FieldCalculator")
        {
            InitializeComponent();
            _helpResourcePrefix = helpType == "SelectByAttribute"
                ? "DatabaseControls.Resources.Help.SelectByAttribute."
                : "DatabaseControls.Resources.Help.FieldCalculator.";
            Title = helpType == "SelectByAttribute" ? "Select By Attribute Help" : "Field Calculator Help";
            ContentRendered += CalculatorHelpWindow_ContentRendered;
        }

        private void CalculatorHelpWindow_ContentRendered(object? sender, EventArgs e)
        {
            LoadHelpContent();
        }

        private void LoadHelpContent()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string htmlResourceName = _helpResourcePrefix +
                (_helpResourcePrefix.Contains("SelectByAttribute") ? "SelectByAttribute.html" : "FieldCalculator.html");

            using var stream = assembly.GetManifestResourceStream(htmlResourceName);
            if (stream == null)
            {
                HelpBrowser.NavigateToString("<html><body><p>Help content not available.</p></body></html>");
                return;
            }

            // Extract HTML and any referenced images to a temp directory
            string tempDir = Path.Combine(Path.GetTempPath(), "DatabaseControls_Help");
            Directory.CreateDirectory(tempDir);

            // Extract all resources with this prefix
            foreach (string name in assembly.GetManifestResourceNames())
            {
                if (!name.StartsWith(_helpResourcePrefix))
                    continue;

                string fileName = name.Substring(_helpResourcePrefix.Length);
                string filePath = Path.Combine(tempDir, fileName);

                using var resourceStream = assembly.GetManifestResourceStream(name);
                if (resourceStream != null)
                {
                    using var fileStream = File.Create(filePath);
                    resourceStream.CopyTo(fileStream);
                }
            }

            string htmlPath = Path.Combine(tempDir, htmlResourceName.Substring(_helpResourcePrefix.Length));
            if (File.Exists(htmlPath))
            {
                HelpBrowser.Navigate(new Uri(htmlPath));
            }
            else
            {
                HelpBrowser.NavigateToString("<html><body><p>Help content not available.</p></body></html>");
            }
        }
    }
}
