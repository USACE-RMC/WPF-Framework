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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ExpressionParser;

namespace ExpressionParserControls
{
    /// <summary>
    /// A WPF UserControl that provides an interactive expression editor with buttons for arithmetic operations
    /// and integration with a parser for generating a parse tree.
    /// </summary>
    public partial class CalculatorControl:UserControl
    {
        /// <summary>
        /// Dictionary containing variable names and their associated result types for expression parsing.
        /// </summary>
        private Dictionary<string, ResultType> _variables;

        /// <summary>
        /// Raised when the expression in the LexTextBox has changed.
        /// </summary>
        public event ExpressionChangedEventHandler ExpressionChanged;

        /// <summary>
        /// Delegate for expression change notifications.
        /// </summary>
        public delegate void ExpressionChangedEventHandler();

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorControl"/> class.
        /// </summary>
        public CalculatorControl()
        {

            // This call is required by the designer.
            this.InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
        }

        /// <summary>
        /// Sets the expression text from the LexTextBox.
        /// </summary>
        /// <param name="expressionText">The expression to display.</param>
        public void SetExpressionText(string expressionText)
        {
            this.LexTextBox.Text = expressionText;
        }

        /// <summary>
        /// Gets the current expression text from the LexTextBox.
        /// </summary>
        /// <returns>The current expression text.</returns>
        public string GetExpressionText()
        {
            return this.LexTextBox.Text;
        }

        /// <summary>
        /// Sets the available variables used in parsing the expression.
        /// </summary>
        /// <param name="availableVariables">A dictionary of variable names and types.</param>
        public void SetVariables(Dictionary<string, ResultType> availableVariables)
        {
            _variables = availableVariables;
        }

        /// <summary>
        /// Inserts the plus operator into the LexTextBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("+");
        }

        /// <summary>
        /// Inserts the minus operator into the LexTextBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void SubtractButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("-");
        }

        /// <summary>
        /// Inserts the multiplication operator into the LexTextBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void MultiplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("*");
        }

        /// <summary>
        /// Inserts the division operator into the LextTextBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void DivideButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("/");
        }

        /// <summary>
        /// Inserts the exponent operator into the LextTextBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void ExponentButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("^");
        }

        /// <summary>
        /// Inserts an equals sign into the LexTextBox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("=");
        }

        /// <summary>
        /// Inserts a custom string of text into the LextTextBox.
        /// </summary>
        /// <param name="textToInsert">The text to insert.</param>
        public void InsertText(string textToInsert)
        {
            this.LexTextBox.InsertText(textToInsert);
        }

        /// <summary>
        /// Returns the parsed tree generated from the current expression.
        /// </summary>
        /// <returns>The parsed expression tree as an IParserNode.</returns>
        public IParserNode GetParseTree()
        {
            return ExpressionParser.Parser.Parser.Parse(this.LexTextBox.GetTokenList, !(this.IsCaseSensitiveCheckbox.IsChecked ?? false), _variables);
        }

        /// <summary>
        /// Called when the expression in the LexTextBox changes.
        /// </summary>
        /// <param name="tokenList">The updated token list.</param>
        private void LexTextBox_ExpressionChanged(List<Token> tokenList)
        {
            ExpressionChanged?.Invoke();
        }

        /// <summary>
        /// Handles navigation to a help document from within the LexTextBox.
        /// Opens or focuses the AvailableFunctions window and selects the appropriate help topic.
        /// </summary>
        /// <param name="helpDocumentPath">The path to the help document.</param>
        private void LexTextBox_HelpDocumentCalled(string helpDocumentPath)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w.Name == "AvailableFunctionsWindow")
                {
                    w.Activate();
                    // Need to select the appropriate item from the list of available functions.
                    foreach (TreeViewItem item in ((AvailableFunctions)w.Content).AvailableFunctionsProp.Items)
                    {
                        if ((new Uri(item.Tag?.ToString() ?? "").AbsolutePath ?? "") == (helpDocumentPath ?? ""))
                            item.IsSelected = true;
                    }
                    return;
                }
            }
            //
            var availFunctionsWindow = new Window() { Name = "AvailableFunctionsWindow", Title = "Available Functions", Content = new AvailableFunctions() { ExpressionText = LexTextBox, Margin = new Thickness(5d) }, ResizeMode = ResizeMode.CanResize, Width = 600d, Height = 400d };
            foreach (TreeViewItem item in ((AvailableFunctions)availFunctionsWindow.Content).AvailableFunctionsProp.Items)
            {
                if ((new Uri(item.Tag?.ToString() ?? "").AbsolutePath ?? "") == (helpDocumentPath ?? ""))
                    item.IsSelected = true;
            }
            availFunctionsWindow.Show();
        }

        /// <summary>
        /// Re-parses the expression when the case-sensitivity setting is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void IsCaseSensitiveCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            this.LexTextBox_ExpressionChanged(this.LexTextBox.GetTokenList);
        }

        /// <summary>
        /// Displays the Available Functions window or activates it if already open.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void FunctionsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w.Name == "AvailableFunctionsWindow")
                {
                    w.Activate();
                    return;
                }
            }
            //
            var availFunctionsWindow = new Window() { Name = "AvailableFunctionsWindow", Title = "Available Functions", Content = new AvailableFunctions() { ExpressionText = LexTextBox, Margin = new Thickness(5d) }, ResizeMode = ResizeMode.CanResize, Width = 600d, Height = 400d };
            ((TreeViewItem)((AvailableFunctions)availFunctionsWindow.Content).AvailableFunctionsProp.Items[0]).IsSelected = true;
            availFunctionsWindow.Show();
        }
    }

    /// <summary>
    /// Converts a boolean value to a Visibility enumeration. True becomes Visible, false becomes collapsed.
    /// </summary>
    /// <remarks>
    /// This converter is maintained for backwards compatibility. For new code, consider using
    /// <see cref="GenericControls.BooleanToVisibilityConverter"/> which provides additional configuration options.
    /// </remarks>
    [Obsolete("Use GenericControls.BooleanToVisibilityConverter for new code. This converter is maintained for backwards compatibility.")]
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to Visibility
        /// </summary>
        /// <param name="value">The source boolean value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Optional parameter (unused).</param>
        /// <param name="culture">Culture information.</param>
        /// <returns>Visibility.Visible if the value is true, otherwise Visibility.Collapsed.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if (value is bool b && b)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        /// <summary>
        /// Not implemented. Converts back from Visibility to boolean
        /// </summary>
        /// <param name="value">The source value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">Optional parameter.</param>
        /// <param name="culture">Culture information.</param>
        /// <returns>Not applicable - this method always throws NotImplementedException.</returns>
        /// <exception cref="NotImplementedException">This method is not implemented.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}