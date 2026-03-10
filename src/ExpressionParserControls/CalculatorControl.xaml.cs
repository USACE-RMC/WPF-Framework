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
    /// A WPF UserControl that provides an interactive expression editor with operator buttons,
    /// an inline functions panel, a function help expander, and an auto-opening errors expander.
    /// </summary>
    public partial class CalculatorControl : UserControl
    {
        /// <summary>
        /// Dictionary containing variable names and their associated result types for expression parsing.
        /// </summary>
        private Dictionary<string, ResultType> _variables;

        /// <summary>
        /// Tracks the currently displayed function in the help expander.
        /// </summary>
        private FunctionDescriptor? _currentHelpFunction;

        /// <summary>
        /// Raised when the expression in the LexTextBox has changed.
        /// </summary>
        public event ExpressionChangedEventHandler ExpressionChanged;

        /// <summary>
        /// Delegate for expression change notifications.
        /// </summary>
        public delegate void ExpressionChangedEventHandler();

        #region Dependency Properties

        /// <summary>
        /// Identifies the HasErrors dependency property.
        /// True when the current expression contains parse errors.
        /// </summary>
        public static readonly DependencyProperty HasErrorsProperty = DependencyProperty.Register(
            nameof(HasErrors), typeof(bool), typeof(CalculatorControl),
            new PropertyMetadata(false));

        /// <summary>
        /// Gets whether the current expression contains parse errors.
        /// </summary>
        public bool HasErrors
        {
            get => (bool)GetValue(HasErrorsProperty);
            private set => SetValue(HasErrorsProperty, value);
        }

        /// <summary>
        /// Identifies the ErrorCount dependency property.
        /// The number of parse errors in the current expression.
        /// </summary>
        public static readonly DependencyProperty ErrorCountProperty = DependencyProperty.Register(
            nameof(ErrorCount), typeof(int), typeof(CalculatorControl),
            new PropertyMetadata(0));

        /// <summary>
        /// Gets the number of parse errors in the current expression.
        /// </summary>
        public int ErrorCount
        {
            get => (int)GetValue(ErrorCountProperty);
            private set => SetValue(ErrorCountProperty, value);
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatorControl"/> class.
        /// </summary>
        public CalculatorControl()
        {
            this.InitializeComponent();

            // Wire up the functions panel to the expression control
            FunctionsPanel.ExpressionText = LexTextBox;
            FunctionsPanel.SelectedFunctionChanged += OnFunctionsPanelSelectedFunctionChanged;
        }

        #region Public API

        /// <summary>
        /// Sets the expression text in the LexTextBox.
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
        /// Inserts a custom string of text into the LexTextBox.
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

        #endregion

        #region Operator Button Handlers

        /// <summary>
        /// Inserts the plus operator into the LexTextBox.
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("+");
        }

        /// <summary>
        /// Inserts the minus operator into the LexTextBox.
        /// </summary>
        private void SubtractButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("-");
        }

        /// <summary>
        /// Inserts the multiplication operator into the LexTextBox.
        /// </summary>
        private void MultiplyButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("*");
        }

        /// <summary>
        /// Inserts the division operator into the LexTextBox.
        /// </summary>
        private void DivideButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("/");
        }

        /// <summary>
        /// Inserts the exponent operator into the LexTextBox.
        /// </summary>
        private void ExponentButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("^");
        }

        /// <summary>
        /// Inserts the equals sign into the LexTextBox.
        /// </summary>
        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("=");
        }

        /// <summary>
        /// Inserts the less-than operator into the LexTextBox.
        /// </summary>
        private void LessThanButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText("<");
        }

        /// <summary>
        /// Inserts the greater-than operator into the LexTextBox.
        /// </summary>
        private void GreaterThanButton_Click(object sender, RoutedEventArgs e)
        {
            this.LexTextBox.InsertText(">");
        }

        #endregion

        #region Expression Changed & Error Display

        /// <summary>
        /// Called when the expression in the LexTextBox changes.
        /// Updates the error display and fires the ExpressionChanged event.
        /// </summary>
        private void LexTextBox_ExpressionChanged(List<Token> tokenList)
        {
            UpdateErrorDisplay();
            ExpressionChanged?.Invoke();
        }

        /// <summary>
        /// Re-parses the expression when the case-sensitivity setting is changed.
        /// </summary>
        private void IsCaseSensitiveCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            this.LexTextBox_ExpressionChanged(this.LexTextBox.GetTokenList);
        }

        /// <summary>
        /// Parses the current expression, updates the errors expander with any parse errors,
        /// and sets HasErrors/ErrorCount properties.
        /// </summary>
        private void UpdateErrorDisplay()
        {
            IParserNode parseNode = null;
            try
            {
                parseNode = GetParseTree();
            }
            catch
            {
                // Parse failed entirely
            }

            if (parseNode == null)
            {
                HasErrors = false;
                ErrorCount = 0;
                ErrorsExpander.Visibility = Visibility.Collapsed;
                ErrorsExpander.IsExpanded = false;
                return;
            }

            var errors = parseNode.GetErrors;
            HasErrors = errors.Count > 0;
            ErrorCount = errors.Count;

            if (errors.Count > 0)
            {
                ErrorsList.ItemsSource = errors;
                ErrorsExpanderHeader.Text = $"Expression Errors ({errors.Count})";
                ErrorsExpander.Visibility = Visibility.Visible;
                ErrorsExpander.IsExpanded = true;
            }
            else
            {
                ErrorsExpander.Visibility = Visibility.Collapsed;
                ErrorsExpander.IsExpanded = false;
                ErrorsList.ItemsSource = null;
            }
        }

        #endregion

        #region Functions Panel

        /// <summary>
        /// Shows the functions panel and sets the column width when the toggle is checked.
        /// </summary>
        private void FunctionsToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            FunctionsPanelColumn.Width = new GridLength(200);
            FunctionsPanelColumn.MinWidth = 140;
        }

        /// <summary>
        /// Hides the functions panel and collapses the column when the toggle is unchecked.
        /// </summary>
        private void FunctionsToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            FunctionsPanelColumn.Width = new GridLength(0);
            FunctionsPanelColumn.MinWidth = 0;
        }

        /// <summary>
        /// Handles function selection changes from the inline functions panel.
        /// Updates the function help expander with the selected function's details.
        /// </summary>
        private void OnFunctionsPanelSelectedFunctionChanged(FunctionDescriptor? func)
        {
            if (func != null)
            {
                ShowFunctionHelp(func);
            }
        }

        #endregion

        #region Function Help Expander

        /// <summary>
        /// Displays the function help expander with details for the given function.
        /// </summary>
        /// <param name="func">The function descriptor to display.</param>
        private void ShowFunctionHelp(FunctionDescriptor func)
        {
            _currentHelpFunction = func;

            HelpFunctionName.Text = func.Name;
            HelpSyntax.Text = func.Syntax;
            HelpReturns.Text = func.Returns;
            HelpDescription.Text = func.Description;
            HelpExample.Text = func.Example;
            FunctionHelpHeader.Text = $"Function: {func.Name}";
            HelpInsertButton.IsEnabled = true;

            FunctionHelpExpander.Visibility = Visibility.Visible;
            FunctionHelpExpander.IsExpanded = true;
        }

        /// <summary>
        /// Inserts the current help function's text into the expression.
        /// </summary>
        private void HelpInsertButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentHelpFunction != null)
            {
                this.LexTextBox.InsertText(_currentHelpFunction.InsertText);
            }
        }

        /// <summary>
        /// Handles navigation to a help document from within the LexTextBox.
        /// Opens the functions panel inline and selects the appropriate function.
        /// </summary>
        /// <param name="helpDocumentPath">The path to the help document.</param>
        private void LexTextBox_HelpDocumentCalled(string helpDocumentPath)
        {
            // Open the functions panel if not already open
            FunctionsToggleButton.IsChecked = true;

            // Select the function in the tree
            FunctionsPanel.SelectFunctionByHelpPath(helpDocumentPath);
        }

        #endregion
    }

    /// <summary>
    /// Converts a boolean value to a Visibility enumeration. True becomes Visible, false becomes Collapsed.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to Visibility.
        /// </summary>
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
        /// Not implemented.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
