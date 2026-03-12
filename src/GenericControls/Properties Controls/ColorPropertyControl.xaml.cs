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
using System.Windows.Media;

namespace GenericControls
{
    /// <summary>
    /// A user control for displaying and selecting a color value, with configurable layout properties.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
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
    public partial class ColorPropertyControl:UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPropertyControl"/> class.
        /// </summary>
        public ColorPropertyControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dependency property for the <see cref="SelectedColor"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(nameof(SelectedColor), typeof(SolidColorBrush), typeof(ColorPropertyControl), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));
        /// <summary>
        /// Gets or sets the selected color represented as a <see cref="SolidColorBrush"/>.
        /// </summary>
        public SolidColorBrush SelectedColor
        {
            get
            {
                return (SolidColorBrush)this.GetValue(SelectedColorProperty);
            }
            set
            {
                this.SetValue(SelectedColorProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the <see cref="Title"/> property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(ColorPropertyControl), new UIPropertyMetadata("Title"));
        /// <summary>
        /// Gets or sets the title displayed for the color property.
        /// </summary>
        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }
            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the <see cref="ShowLeaderLine"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowLeaderLineProperty = DependencyProperty.Register(nameof(ShowLeaderLine), typeof(bool), typeof(ColorPropertyControl), new UIPropertyMetadata(true));
        /// <summary>
        /// Gets or sets a value indicating whether to show a visual leader line in the UI.
        /// </summary>
        public bool ShowLeaderLine
        {
            get
            {
                return (bool)this.GetValue(ShowLeaderLineProperty);
            }
            set
            {
                this.SetValue(ShowLeaderLineProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the <see cref="PropertyHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty PropertyHeightProperty = DependencyProperty.Register(nameof(PropertyHeight), typeof(double), typeof(ColorPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultPropertyHeight));
        /// <summary>
        /// Gets or sets the height of the property row in the layout.
        /// </summary>
        public double PropertyHeight
        {
            get
            {
                return (double)this.GetValue(PropertyHeightProperty);
            }
            set
            {
                this.SetValue(PropertyHeightProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the <see cref="MaxPropertyWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty MaxPropertyWidthProperty = DependencyProperty.Register(nameof(MaxPropertyWidth), typeof(double), typeof(ColorPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultMaxPropertyWidth));
        /// <summary>
        /// Gets or sets the maximum allowed width of the property.
        /// </summary>
        public double MaxPropertyWidth
        {
            get
            {
                return (double)this.GetValue(MaxPropertyWidthProperty);
            }
            set
            {
                this.SetValue(MaxPropertyWidthProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the <see cref="MinPropertyWidth"/> property.
        /// </summary> 
        public static readonly DependencyProperty MinPropertyWidthProperty = DependencyProperty.Register(nameof(MinPropertyWidth), typeof(double), typeof(ColorPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultMinPropertyWidth));
        /// <summary>
        /// Gets or sets the minimum allowed width of the property.
        /// </summary>
        public double MinPropertyWidth
        {
            get
            {
                return (double)this.GetValue(MinPropertyWidthProperty);
            }
            set
            {
                this.SetValue(MinPropertyWidthProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the <see cref="PropertyWidth"/> property.
        /// </summary>
        public static readonly DependencyProperty PropertyWidthProperty = DependencyProperty.Register(nameof(PropertyWidth), typeof(GridLength), typeof(ColorPropertyControl), new UIPropertyMetadata(new GridLength(36d)));
        /// <summary>
        /// Gets or sets the current width of the property layout column.
        /// </summary>
        public GridLength PropertyWidth
        {
            get
            {
                return (GridLength)this.GetValue(PropertyWidthProperty);
            }
            set
            {
                this.SetValue(PropertyWidthProperty, value);
            }
        }
    }
}