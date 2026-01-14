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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using ExpressionParser;

namespace DatabaseControls
{
    /// <summary>
    /// A window that displays a list of errors encountered during expression parsing or evaluation.
    /// Implements <see cref="INotifyPropertyChanged"/> for data binding support.
    /// </summary>
    public partial class ErrorWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The backing field for the Errors property.
        /// </summary>
        private ObservableCollection<ErrorItem> _errors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWindow"/> class with a list of string error messages.
        /// </summary>
        /// <param name="errorList">The list of error messages to display.</param>
        /// <param name="title">The title to display in the window title bar.</param>
        public ErrorWindow(List<string> errorList, string title)
        {
            // This call is required by the designer.
            InitializeComponent();
            this.Title = title;

            // Add any initialization after the InitializeComponent() call.
            var tmp = new ObservableCollection<ErrorItem>();
            for (int i = 0; i < errorList.Count; i++)
            {
                tmp.Add(new ErrorItem(errorList[i]));
            }
            Errors = tmp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorWindow"/> class with a list of parse errors.
        /// </summary>
        /// <param name="errorList">The list of parse errors to display.</param>
        /// <param name="title">The title to display in the window title bar.</param>
        public ErrorWindow(IList<ParseError> errorList, string title)
        {
            // This call is required by the designer.
            InitializeComponent();
            this.Title = title;

            // Add any initialization after the InitializeComponent() call.
            var tmp = new ObservableCollection<ErrorItem>();
            for (int i = 0; i < errorList.Count; i++)
            {
                tmp.Add(new ErrorItem(errorList[i].Description));
            }
            Errors = tmp;
        }

        /// <summary>
        /// Gets or sets the collection of error items to display in the data grid.
        /// </summary>
        public ObservableCollection<ErrorItem> Errors
        {
            get { return _errors; }
            set
            {
                _errors = value;
                NotifyPropertyChanged("Errors");
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// </summary>
        /// <param name="info">The name of the property that changed.</param>
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        /// <summary>
        /// Handles the click event for the Cancel/Close button.
        /// Closes the error window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
