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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using ExpressionParser;

namespace ExpressionParserControls
{
    /// <summary>
    /// Represents a control that displays a list of available functions for the expression parser.
    /// Provides function selection, insertion, and navigation to help documentation.
    /// </summary>
    public partial class AvailableFunctions
    {

        /// <summary>
        /// Dependency property backing the <see cref="ExpressionText"/> property.
        /// </summary>
        public static DependencyProperty ExpressionTextProperty = DependencyProperty.Register(nameof(ExpressionText), typeof(ExpressionControl), typeof(AvailableFunctions), new FrameworkPropertyMetadata(null)); // , AddressOf RichTextChanged))

        /// <summary>
        /// Gets/sets the bound <see cref="ExpressionControl"/> instance associated with this control.
        /// </summary>
        public ExpressionControl ExpressionText
        {
            get
            {
                return (ExpressionControl)this.GetValue(ExpressionTextProperty);
            }
            set
            {
                this.SetValue(ExpressionTextProperty, value);
            }
        }

        /// <summary>
        /// Event raised when a function is selected an inserted into the expression.
        /// </summary>
        public event InsertCalledEventHandler InsertCalled;

        /// <summary>
        /// Delegate for the <see cref="InsertCalled"/> event, sending the inserted function text.
        /// </summary>
        /// <param name="stringToInsert">The function text to insert into the expression.</param>
        public delegate void InsertCalledEventHandler(string stringToInsert);

        /// <summary>
        /// Initializes a new instance of the <see cref="AvailableFunctions"/> class and populates available functions.
        /// </summary>
        public AvailableFunctions()
        {

            // This call is required by the designer.
            this.InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            TreeViewItem availableFunction;
            var helpDocs = new HashSet<string>();
            foreach (var functionKey in Lexer.Keywords)
            {
                if (functionKey.Value.Item4 != TokenClass.Function)
                    continue;
                if (helpDocs.Contains(functionKey.Value.Item1))
                    continue;
                string helpDocumentPath = Environment.CurrentDirectory + "/" + functionKey.Value.Item1;
                var headerText = new TextBlock() { Text = functionKey.Key, FontWeight = FontWeights.Bold, Margin = new Thickness(0d, 1d, 0d, 1d), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch };
                availableFunction = new TreeViewItem() { Header = headerText, Tag = helpDocumentPath, VerticalContentAlignment = VerticalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                availableFunction.Selected += (sender,e) =>
                    {
                        if (System.IO.File.Exists(helpDocumentPath) == false)
                            this.HelpBrowser.Navigate("about:blank");
                        try
                        {
                            this.HelpBrowser.Navigate(new Uri(helpDocumentPath));
                        }
                        catch (Exception)
                        {
                            this.HelpBrowser.Navigate("about:blank");
                        }
                    };
                availableFunction.MouseDoubleClick += (sender,e) =>
                    {
                        if (!(ExpressionText == null))
                            ExpressionText.InsertText(functionKey.Key + "(");
                        InsertCalled?.Invoke(functionKey.Key + "(");
                    };

                this.AvailableFunctionsProp.Items.Add(availableFunction);
                helpDocs.Add(functionKey.Value.Item1);
            }
        }

        /// <summary>
        /// Enables or disables the insert button depending on whether a function is selected.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void AvailableFunctions_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.InsertFunctionButton.IsEnabled = !(this.AvailableFunctionsProp.SelectedItem == null);
        }

        /// <summary>
        /// Inserts the selected function into the expression text when the insert button is clicked.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.AvailableFunctionsProp.SelectedItem == null)
                return;
            if (this.AvailableFunctionsProp.SelectedItem.GetType() != typeof(TreeViewItem))
                return;
            string functionText = ((TextBlock)((TreeViewItem)this.AvailableFunctionsProp.SelectedItem).Header).Text;
            if (!(ExpressionText == null))
                ExpressionText.InsertText(functionText + "(");
            InsertCalled?.Invoke(functionText + "(");
        }

    }
}