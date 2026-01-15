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

using GenericControls;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for DefaultsOptions.xaml providing default project settings configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class DefaultsOptions : UserControl
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultsOptions"/> class.
        /// </summary>
        public DefaultsOptions()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
        }

        /// <summary>
        /// Dependency property for the default location text.
        /// </summary>
        public static DependencyProperty DefaultLocationProperty = DependencyProperty.Register(nameof(DefaultLocation), typeof(string), typeof(DefaultsOptions), new UIPropertyMetadata(""));

        /// <summary>
        /// Gets or sets the folder location text.
        /// </summary>
        public string DefaultLocation
        {
            get { return GetValue(DefaultLocationProperty).ToString(); }
            set { SetValue(DefaultLocationProperty, value); }
        }

        /// <summary>
        /// Dependency property for the default value digits.
        /// </summary>
        public static DependencyProperty DefaultValueDigitsProperty = DependencyProperty.Register(nameof(DefaultValueDigits), typeof(int), typeof(DefaultsOptions), new UIPropertyMetadata(2));

        /// <summary>
        /// Gets or sets the default output value decimal digits.
        /// </summary>
        public int DefaultValueDigits
        {
            get { return (int)GetValue(DefaultValueDigitsProperty); }
            set { SetValue(DefaultValueDigitsProperty, value); }
        }

        /// <summary>
        /// Dependency property for custom options.
        /// </summary>
        public static DependencyProperty CustomOptionsProperty = DependencyProperty.Register(nameof(CustomOptions), typeof(object), typeof(DefaultsOptions), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the custom options control or content.
        /// </summary>
        public object CustomOptions
        {
            get { return GetValue(CustomOptionsProperty); }
            set { SetValue(CustomOptionsProperty, value); }
        }

        /// <summary>
        /// On click, open folder browser to select folder.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedLocation = GeneralMethods.FolderBrowserDialog(Window.GetWindow(this), "Project Location", Directory.Exists(DefaultLocation) == true ? DefaultLocation : null);
            if (!string.IsNullOrEmpty(selectedLocation)) DefaultLocation = selectedLocation;
        }

    }
}
