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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace GenericControls
{
    /// <summary>
    /// A user control for selecting a directory path with a browse button.
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
    public partial class DirectorySelectorControl : UserControl,INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectorySelectorControl"/> class.
        /// </summary>
        public DirectorySelectorControl()
        {
            InitializeComponent();
        }

        #region Members

        /// <summary>
        /// Dependency property for the text property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(DirectorySelectorControl), new FrameworkPropertyMetadata(""));

        /// <summary>
        /// Gets or sets the directory path text.
        /// </summary>
        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }
            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(DirectorySelectorControl), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the control is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return (bool)this.GetValue(IsReadOnlyProperty);
            }
            set
            {
                this.SetValue(IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the title property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(DirectorySelectorControl), new FrameworkPropertyMetadata("Title"));

        /// <summary>
        /// Gets or sets the title text displayed for the control.
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
        /// Dependency property for the property width.
        /// </summary>
        public static readonly DependencyProperty PropertyWidthProperty = DependencyProperty.Register(nameof(PropertyWidth), typeof(GridLength), typeof(DirectorySelectorControl), new FrameworkPropertyMetadata(PropertyDefaults.DefaultPropertyWidth));

        /// <summary>
        /// Gets or sets the width of the property column in the layout.
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

        /// <summary>
        /// Dependency property for the maximum property width.
        /// </summary>
        public static readonly DependencyProperty MaxPropertyWidthProperty = DependencyProperty.Register(nameof(MaxPropertyWidth), typeof(double), typeof(DirectorySelectorControl), new FrameworkPropertyMetadata(PropertyDefaults.DefaultMaxPropertyWidth));

        /// <summary>
        /// Gets or sets the maximum width of the property area.
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
        /// Dependency property for the minimum property width.
        /// </summary>
        public static readonly DependencyProperty MinPropertyWidthProperty = DependencyProperty.Register(nameof(MinPropertyWidth), typeof(double), typeof(DirectorySelectorControl), new FrameworkPropertyMetadata(PropertyDefaults.DefaultMinPropertyWidth));

        /// <summary>
        /// Gets or sets the minimum width of the property area.
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
        /// Dependency property for the show leader line property.
        /// </summary>
        public static readonly DependencyProperty ShowLeaderLineProperty = DependencyProperty.Register(nameof(ShowLeaderLine), typeof(bool), typeof(DirectorySelectorControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets a value indicating whether the leader line should be visible.
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

        #endregion

        private double _actualWidth = 0d;

        /// <summary>
        /// Gets the actual rendered width of the control.
        /// Updates when the control's size changes.
        /// </summary>
        public double ActualPropertyWidth
        {
            get
            {
                return _actualWidth;
            }
            private set
            {
                if (_actualWidth != value)
                {
                    _actualWidth = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActualPropertyWidth)));
                }
            }
        }

        /// <summary>
        /// Occurs when a property value changes, primarily for <see cref="ActualPropertyWidth"/>.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the text is changed in the associated text box.
        /// </summary>
        public event TextChangedEventHandler TextChanged;

        /// <summary>
        /// Delegate for the <see cref="TextChanged"/> event.
        /// </summary>
        /// <param name="sender">The control where the event originated.</param>
        /// <param name="e">Event arguments containing change details.</param>
        public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);

        /// <summary>
        /// Handles the control's <see cref="FrameworkElement.SizeChanged"/> event to update the <see cref="ActualPropertyWidth"/>.
        /// </summary>
        /// <param name="sender">The control that triggered the event.</param>
        /// <param name="e">Size changed event arguments.</param>
        private void ControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            ActualPropertyWidth = el.ActualWidth;
        }

        /// <summary>
        /// Opens a folder browser dialog and updates the <see cref="Text"/> property with the selected path.
        /// </summary>
        /// <param name="sender">The button triggering the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void FolderPathButton_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = GeneralMethods.FolderBrowserDialog(Window.GetWindow(this));
            if (!(folderPath == null) && !string.IsNullOrEmpty(folderPath))
                Text = folderPath;
        }

        /// <summary>
        /// Raises the <see cref="TextChanged"/> event when the folder path text box changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Text changed event arguments.</param>
        private void FolderPathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }
    }
}