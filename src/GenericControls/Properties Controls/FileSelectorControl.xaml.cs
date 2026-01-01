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
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GenericControls
{

    public partial class FileSelectorControl : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSelectorControl"/> class.
        /// </summary>
        public FileSelectorControl()
        {
            InitializeComponent();
        }

        #region Members
        /// <summary>
    /// Dependency property for the text property.
    /// </summary>
        public static DependencyProperty FileFiltersProperty = DependencyProperty.Register(nameof(FileFilters), typeof(string), typeof(FileSelectorControl), new FrameworkPropertyMetadata("All files (*.*) |*.*"));

        /// <summary>
    /// Gets and sets the text.
    /// </summary>
        public string FileFilters
        {
            get
            {
                return (string)this.GetValue(FileFiltersProperty);
            }
            set
            {
                this.SetValue(FileFiltersProperty, value);
            }
        }


        /// <summary>
    /// Dependency property for the text property.
    /// </summary>
        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(FileSelectorControl), new FrameworkPropertyMetadata(""));

        /// <summary>
    /// Gets and sets the text.
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
        /// Identifies <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(FileSelectorControl), new FrameworkPropertyMetadata(false));
        /// <summary>
        /// gets/sets the value is read-only.
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
        public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(FileSelectorControl), new FrameworkPropertyMetadata("Title"));

        /// <summary>
    /// Gets and sets the title.
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
    /// Dependency property for the max width property.
    /// </summary>
        public static DependencyProperty PropertyWidthProperty = DependencyProperty.Register(nameof(PropertyWidth), typeof(GridLength), typeof(FileSelectorControl), new FrameworkPropertyMetadata(PropertyDefaults.DefaultPropertyWidth));

        /// <summary>
    /// Gets and sets the max width of the control.
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
    /// Dependency property for the max width property.
    /// </summary>
        public static DependencyProperty MaxPropertyWidthProperty = DependencyProperty.Register(nameof(MaxPropertyWidth), typeof(double), typeof(FileSelectorControl), new FrameworkPropertyMetadata(PropertyDefaults.DefaultMaxPropertyWidth));

        /// <summary>
    /// Gets and sets the max width of the control.
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
    /// Dependency property for the min width property.
    /// </summary>
        public static DependencyProperty MinPropertyWidthProperty = DependencyProperty.Register(nameof(MinPropertyWidth), typeof(double), typeof(FileSelectorControl), new FrameworkPropertyMetadata(PropertyDefaults.DefaultMinPropertyWidth));

        /// <summary>
    /// Gets and sets the min width of the control.
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
        public static DependencyProperty ShowLeaderLineProperty = DependencyProperty.Register(nameof(ShowLeaderLine), typeof(bool), typeof(FileSelectorControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
    /// Determines of the leader line should be visible. 
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
    /// Dependency property for the show leader line property.
    /// </summary>
        public static DependencyProperty ShowTitleProperty = DependencyProperty.Register(nameof(ShowTitle), typeof(bool), typeof(FileSelectorControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
    /// Determines of the leader line should be visible. 
    /// </summary>
        public bool ShowTitle
        {
            get
            {
                return (bool)this.GetValue(ShowTitleProperty);
            }
            set
            {
                this.SetValue(ShowTitleProperty, value);
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);

        /// <summary>
        /// Handles the control's <see cref="FrameworkElement.SizeChanged"/> event to update the <see cref="ActualPropertyWidth"/>.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            ActualPropertyWidth = el.ActualWidth;
        }

        /// <summary>
        /// Opens a file dialog and updates the <c>Text</c> property with the selected path.
        /// </summary>
        /// <param name="sender">The button triggering the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void FilePathButton_Click(object sender, RoutedEventArgs e)
        {
            string[] files = GeneralMethods.FileOpenDialog(FileFilters, false);
            if (files is null || files.Count() == 0 || !string.IsNullOrEmpty(files[0]))
                Text = files[0];
        }

        /// <summary>
        /// Raises the <see cref="TextChanged"/> event when the file text box changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Text changed event arguments.</param>
        private void FilePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }
    }
}