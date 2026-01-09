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

using System.Windows;
using System.Windows.Controls;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for GeneralOptions.xaml providing general application settings configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class GeneralOptions : UserControl
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralOptions"/> class.
        /// </summary>
        public GeneralOptions()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
        }

        /// <summary>
        /// Theme list for combobox.
        /// </summary>
        public List<string> ThemeList { get; set; } = new List<string>(new[] { "Blue", "Light" }); 

        /// <summary>
        /// Dependency property for the color theme.
        /// </summary>
        public static DependencyProperty ColorThemeProperty = DependencyProperty.Register(nameof(ColorTheme), typeof(string), typeof(GeneralOptions), new PropertyMetadata("Light"));

        /// <summary>
        /// Gets or sets the color theme.
        /// </summary>
        public string ColorTheme
        {
            get { return GetValue(ColorThemeProperty).ToString(); }
            set { SetValue(ColorThemeProperty, value); }
        }

        /// <summary>
        /// Dependency property for whether to save window layout.
        /// </summary>
        public static DependencyProperty SaveWindowLayoutProperty = DependencyProperty.Register(nameof(SaveWindowLayout), typeof(bool), typeof(GeneralOptions), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether to save window layout.
        /// </summary>
        public bool SaveWindowLayout
        {
            get { return (bool)GetValue(SaveWindowLayoutProperty); }
            set { SetValue(SaveWindowLayoutProperty, value); }
        }

        /// <summary>
        /// Dependency property for the number of window menu items to show.
        /// </summary>
        public static DependencyProperty MaxWindowMenuItemsProperty = DependencyProperty.Register(nameof(MaxWindowMenuItems), typeof(int), typeof(GeneralOptions), new UIPropertyMetadata(10));

        /// <summary>
        /// Gets or sets the number of window menu items to show.
        /// </summary>
        public int MaxWindowMenuItems
        {
            get { return (int)GetValue(MaxWindowMenuItemsProperty); }
            set { SetValue(MaxWindowMenuItemsProperty, value); }
        }

        /// <summary>
        /// Dependency property for the number of window menu items to show.
        /// </summary>
        public static DependencyProperty MaxRecentFileItemsProperty = DependencyProperty.Register(nameof(MaxRecentFileItems), typeof(int), typeof(GeneralOptions), new UIPropertyMetadata(10));

        /// <summary>
        /// Gets or sets the number of recent project items to show.
        /// </summary>
        public int MaxRecentFileItems
        {
            get { return (int)GetValue(MaxRecentFileItemsProperty); }
            set { SetValue(MaxRecentFileItemsProperty, value); }
        }

    }
}
