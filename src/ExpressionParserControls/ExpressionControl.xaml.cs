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
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ExpressionParser;

namespace ExpressionParserControls
{
    /// <summary>
    /// Represents a control for editing and displaying mathematical or logical expressions.
    /// Provides real-time syntax highlighting, token parsing, and support for hyperlinks to help documentation.
    /// </summary>
    public partial class ExpressionControl
    {
        /// <summary>
        /// Default parenthesis colors used when theme resources are not available.
        /// </summary>
        private static readonly SolidColorBrush[] DefaultParenthesisColors = new[] { Brushes.Black, Brushes.Green, Brushes.Purple, Brushes.OrangeRed, Brushes.CornflowerBlue, Brushes.DarkGoldenrod, Brushes.Red };

        /// <summary>
        /// Predefined colors for highlighting parentheses in the expression.
        /// Resolved from theme DynamicResources at load time, with hardcoded fallbacks.
        /// </summary>
        private SolidColorBrush[] _parenthesisColors = DefaultParenthesisColors;

        /// <summary>
        /// Flag indicating whether the expression is currently being formatted.
        /// </summary>
        private bool _formattingExpression = false;

        /// <summary>
        /// Dependency property backing the <see cref="Text"/> property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ExpressionControl), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextPropertyChanged));

    private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ExpressionControl)d;
        if (control._formattingExpression || !control.IsLoaded)
            return;

        string newText = (string)e.NewValue ?? "";
        string currentText = new System.Windows.Documents.TextRange(
            control.ExpressionTextBox.Document.ContentStart,
            control.ExpressionTextBox.Document.ContentEnd).Text.Trim(Environment.NewLine.ToCharArray());

        if (currentText != newText)
        {
            control._formattingExpression = true;
            control.ExpressionTextBox.Document.Blocks.Clear();
            var p = new System.Windows.Documents.Paragraph();
            p.Inlines.Add(new System.Windows.Documents.Run(newText));
            control.ExpressionTextBox.Document.Blocks.Add(p);
            control._formattingExpression = false;
            // Trigger a TextChanged to reformat with syntax highlighting
            control.ExpressionTextBox_TextChanged(control.ExpressionTextBox, new TextChangedEventArgs(System.Windows.Controls.RichTextBox.TextChangedEvent, UndoAction.None));
        }
    }

        /// <summary>
        /// Gets or sets the text content of the expression editor.
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
        /// Gets the list of tokens parsed from the current expression.
        /// </summary>
        public List<Token> GetTokenList { get; private set; } = new List<Token>();

        /// <summary>
        /// Raised when the expression changes and tokens are re-evaluated.
        /// </summary>
        public event ExpressionChangedEventHandler ExpressionChanged;

        /// <summary>
        /// Delegate for handling expression changes.
        /// </summary>
        /// <param name="tokenList">List of tokens parsed from the expression.</param>
        public delegate void ExpressionChangedEventHandler(List<Token> tokenList);

        /// <summary>
        /// Raised when a help document is triggered by clicking a hyperlink in the expression editor.
        /// </summary>
        public event HelpDocumentCalledEventHandler HelpDocumentCalled;

        /// <summary>
        /// Delegate for the HelpDocumentCalled event.
        /// </summary>
        /// <param name="helpDocumentPath">The path of the help document.</param>
        public delegate void HelpDocumentCalledEventHandler(string helpDocumentPath);

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionControl"/> class.
        /// </summary>
        /// <summary>
        /// Overflow color used when parenthesis nesting exceeds the color array length.
        /// </summary>
        private SolidColorBrush _overflowColor = Brushes.Gray;

        public ExpressionControl()
        {
            // This call is required by the designer.
            this.InitializeComponent();

            // Resolve theme-aware parenthesis colors after the control is loaded
            // so that DynamicResource keys from the application's merged dictionaries are available.
            this.Loaded += (s, e) => ResolveThemeColors();
        }

        /// <summary>
        /// Resolves parenthesis and overflow colors from theme DynamicResources, falling back to defaults.
        /// </summary>
        private void ResolveThemeColors()
        {
            var colors = new SolidColorBrush[7];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = TryFindResource($"ExpressionControl.Parenthesis.Color{i}") as SolidColorBrush ?? DefaultParenthesisColors[i];
            }
            _parenthesisColors = colors;
            _overflowColor = TryFindResource("ExpressionControl.Parenthesis.Overflow") as SolidColorBrush ?? Brushes.Gray;
        }

        /// <summary>
        /// Colors associated with token classes. 
        /// </summary>
        private List<SolidColorBrush> _colors = new List<SolidColorBrush>();

        /// <summary>
        /// Inserts text into current expression at the caret position.
        /// </summary>
        /// <param name="textToInsert">The text to insert.</param>
        /// <param name="focusRichTextBox">Whether to refocus the editor after insertion.</param>
        public void InsertText(string textToInsert, bool focusRichTextBox = true)
        {
            this.ExpressionTextBox.Selection.Text = textToInsert;
            if (focusRichTextBox)
            {
                this.ExpressionTextBox.Focus(); // Set Logical Focus
                Keyboard.Focus(this.ExpressionTextBox);
            }
        }

        /// <summary>
        /// Handles changes to the expression text box, performing syntax highlighting and token analysis 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs"/> instance containing the event data.</param>
        private void ExpressionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded == false)
                return;
            if (_formattingExpression == true)
                return;
            _formattingExpression = true;
            // Get caret position within the unformatted text string.
            string allRichText = new TextRange(this.ExpressionTextBox.Document.ContentStart, this.ExpressionTextBox.Document.ContentEnd).Text.Trim(Environment.NewLine.ToCharArray());
            string fromCaretText = new TextRange(this.ExpressionTextBox.CaretPosition, this.ExpressionTextBox.Document.ContentEnd).Text.Trim(Environment.NewLine.ToCharArray());
            // Handle the situation where data was potentially pasted.
            if (e.Changes.Count > 0 && e.Changes.Any(o => o.AddedLength > 1))
            {
                allRichText = allRichText.Replace(Environment.NewLine, " ").Replace("\n", " ");
                fromCaretText = fromCaretText.Replace(Environment.NewLine, " ").Replace("\n", " ");
            }
            int caretTextPosition = allRichText.Length - fromCaretText.Length;
            // Get the token list.
            GetTokenList = Lexer.TokenizeStringToList(allRichText);
            // Clear the expression box of previous entries.
            this.ExpressionTextBox.Document.Blocks.Clear();
            var p = new Paragraph();
            this.ExpressionTextBox.Document.Blocks.Add(p);
            // 
            int parenthesisColorPosition = 0;
            TextPointer newCaret = null;
            Token t;
            Run r = null;
            for (int i = 0; i < GetTokenList.Count; i++)
            {
                t = GetTokenList[i];
                r = new Run(t.TokenString) { FontSize = FontSize, FontWeight = FontWeight, FontStyle = FontStyle, FontFamily = FontFamily, FontStretch = FontStretch };
                if (t.Type == TokenType.LeftParenthesis)
                {
                    r.Foreground = parenthesisColorPosition > _parenthesisColors.Length - 1 ? _overflowColor : _parenthesisColors[parenthesisColorPosition];
                    p.Inlines.Add(r);
                    parenthesisColorPosition += 1;
                }
                else if (t.Type == TokenType.RightParenthesis)
                {
                    parenthesisColorPosition -= 1;
                    if (parenthesisColorPosition < 0)
                        parenthesisColorPosition = 0;
                    r.Foreground = parenthesisColorPosition > _parenthesisColors.Length - 1 ? _overflowColor : _parenthesisColors[parenthesisColorPosition];
                    p.Inlines.Add(r);
                }
                else if (t.TokenGroup == TokenClass.Function)
                {
                    var h = new Hyperlink(r) { NavigateUri = new Uri(Path.Combine(Environment.CurrentDirectory, t.HelpDocPath)), IsEnabled = true };
                    p.Inlines.Add(h);
                }
                // If the operator is defined here as hyperlink then the text can get too messy. The operators are self explanatory enough to not require help documentation.
                // ElseIf t.TokenGroup = TokenClass.Operator Then
                // Dim h As New Hyperlink(r) With {.NavigateUri = New Uri(Environment.CurrentDirectory & "/" & t.HelpDocPath), .IsEnabled = True}
                // p.Inlines.Add(h)
                else
                {
                    p.Inlines.Add(r);
                }
                //
                if (i < GetTokenList.Count - 1)
                {
                    if (t.StartPosition <= caretTextPosition && GetTokenList[i + 1].StartPosition > caretTextPosition)
                    {
                        newCaret = r.ContentStart.GetPositionAtOffset(caretTextPosition - t.StartPosition);
                    }
                }
                // last token if the new caret position was never defined
                else if (newCaret == null && r != null)
                    newCaret = r.ContentStart.GetPositionAtOffset(caretTextPosition - GetTokenList.Last().StartPosition);
            }
            //
            if (newCaret != null)
                this.ExpressionTextBox.CaretPosition = newCaret;
            Text = allRichText;
            _formattingExpression = false;
            // 
            ExpressionChanged?.Invoke(GetTokenList);
        }

        /// <summary>
        /// Handles the mouse left-button down event on a hyperlink, triggering help documentation.
        /// </summary>
        /// <param name="sender">The hyperlink sender.</param>
        /// <param name="e">Mouse button event arguments.</param>
        private void HyperlinkMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender.GetType() != typeof(Hyperlink))
                return;
            HelpDocumentCalled?.Invoke(((Hyperlink)sender).NavigateUri.AbsolutePath);
            e.Handled = true;
        }

        /// <summary>
        /// Placeholder for selection logic in the expression textbox.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Routed event arguments.</param>
        private void ExpressionTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // might want to allow some work here such as bolding the matching parenthesis if there is one.
        }
    }
}