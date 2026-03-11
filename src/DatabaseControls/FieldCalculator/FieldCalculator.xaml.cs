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
using System.Windows.Input;
using DatabaseManager;
using ExpressionParser;
using ExpressionParser.Parser;
using ExpressionParserControls;

namespace DatabaseControls
{
    /// <summary>
    /// A window that provides field calculation functionality for data tables.
    /// Allows users to create new columns or update existing columns using mathematical expressions.
    /// Can also be used for selecting records by attribute conditions.
    /// </summary>
    public partial class FieldCalculator : GenericControls.MetroWindow
    {
        /// <summary>
        /// Reference to the data table view being operated on.
        /// </summary>
        private readonly DataTableView _dbView;

        /// <summary>
        /// Indicates whether an existing field is being updated (true) or a new field is being created (false).
        /// </summary>
        private bool _existingField;

        /// <summary>
        /// The name of the field being created or updated.
        /// </summary>
        private string? _fieldName;

        /// <summary>
        /// List of row indices that are currently selected in the data table.
        /// </summary>
        private readonly List<int>? _selectedRows;

        /// <summary>
        /// Indicates whether this calculator is being used for "Select By Attribute" functionality.
        /// </summary>
        private readonly bool _isSelectByAttribute;

        /// <summary>
        /// List of row indices that match the selection criteria when using "Select By Attribute" mode.
        /// </summary>
        private List<int> _rowsToSelect = new List<int>();

        /// <summary>
        /// Gets the list of row indices that were selected by the attribute expression.
        /// Only populated when the calculator is used in "Select By Attribute" mode.
        /// </summary>
        public List<int> GetSelectedRows
        {
            get { return _rowsToSelect; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldCalculator"/> class.
        /// </summary>
        /// <param name="dataView">The data table view to perform calculations on.</param>
        /// <param name="selectedRows">List of currently selected row indices, or null if no selection.</param>
        /// <param name="readOnlyColumns">Set of column indices that should not be editable.</param>
        /// <param name="header">Optional header/field name. If provided, the field selection UI is hidden and this field is used.</param>
        /// <param name="isSelectByAttribute">If true, the calculator operates in "Select By Attribute" mode for filtering records.</param>
        public FieldCalculator(DataTableView dataView, List<int>? selectedRows, HashSet<int>? readOnlyColumns, string? header = null, bool isSelectByAttribute = false)
        {
            // This call is required by the designer.
            InitializeComponent();

            // Prevent content flicker on open — render invisible until layout completes
            this.Opacity = 0;
            this.ContentRendered += (s, e) => this.Opacity = 1;

            _isSelectByAttribute = isSelectByAttribute;
            if (_isSelectByAttribute)
            {
                this.Title = "Select By Attribute";
            }

            _dbView = dataView;
            _selectedRows = selectedRows;

            // Wire up the CalculatorControl events for functions panel and help navigation
            ExpressionCalculator.FunctionsToggleChanged += OnFunctionsToggleChanged;
            ExpressionCalculator.HelpDocumentRequested += OnHelpDocumentRequested;

            // Wire the functions panel to insert into the expression control
            FunctionsPanel.ExpressionText = ExpressionCalculator.ExpressionEditor;
            FunctionsPanel.SelectedFunctionChanged += OnFunctionsPanelSelectedFunctionChanged;

            // Set variables
            VariablesListBox.Items.Clear();
            var vars = new Dictionary<string, ResultType>();
            for (int i = 0; i < _dbView.ColumnNames.Length; i++)
            {
                if (vars.ContainsKey(_dbView.ColumnNames[i]))
                {
                    continue;
                }
                vars.Add(_dbView.ColumnNames[i], Parser.TypeToResultType(_dbView.ColumnTypes[i]));
                VariablesListBox.Items.Add(new ListBoxItem() { Content = _dbView.ColumnNames[i] });
            }
            ExpressionCalculator.SetVariables(vars);

            if (_isSelectByAttribute)
            {
                FieldSelectionGrid.Visibility = Visibility.Collapsed;
                FieldSelectionGrid.IsEnabled = false;
                if (_selectedRows == null || _selectedRows.Count == 0)
                {
                    UseSelectedRange.IsEnabled = false;
                }
                else
                {
                    UseSelectedRange.IsEnabled = true;
                    UseSelectedRange.IsChecked = true;
                }
            }
            else
            {
                if (_selectedRows == null || _selectedRows.Count == 0)
                {
                    UseSelectedRange.IsEnabled = false;
                }
                else
                {
                    UseSelectedRange.IsEnabled = true;
                    UseSelectedRange.IsChecked = true;
                }

                if (header == null)
                {
                    UpdateExistingRadioButton.IsChecked = false;
                    CreateNewFieldRadioButton.IsChecked = true;
                    _existingField = UpdateExistingRadioButton.IsChecked == true;
                    for (int i = 0; i < _dbView.ColumnNames.Length; i++)
                    {
                        if (readOnlyColumns != null && readOnlyColumns.Contains(i))
                        {
                            continue;
                        }
                        ExistingFieldsCombobox.Items.Add(_dbView.ColumnNames[i]);
                    }
                    ExistingFieldsCombobox.SelectedItem = _dbView.ColumnNames.Length > 0 ? _dbView.ColumnNames[0] : null;
                }
                else
                {
                    ExistingFieldsCombobox.Items.Add(header);
                    ExistingFieldsCombobox.SelectedIndex = 0;
                    FieldSelectionGrid.Visibility = Visibility.Collapsed;
                    FieldSelectionGrid.IsEnabled = false;
                    _existingField = true;
                    _fieldName = header;
                }
            }
        }

        /// <summary>
        /// Handles the Execute button click event. Validates the expression and applies the calculation to the data.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_dbView.NumberOfRows == 0)
            {
                GenericControls.MessageBox.Show("No rows exist. Cannot execute field calculator without data rows.", "No Rows Exist", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IParserNode tree = ExpressionCalculator.GetParseTree();
            if (tree != null)
            {
                EvaluateColumn(tree);
            }
        }

        /// <summary>
        /// Evaluates the expression for each applicable row and returns the calculated data.
        /// </summary>
        /// <param name="t">The parsed expression tree to evaluate.</param>
        /// <param name="useSelectedRows">If true, only evaluates for selected rows; otherwise evaluates for all rows.</param>
        /// <returns>An array of calculated values, one for each applicable row.</returns>
        private object[] GetExpressionData(IParserNode t, bool useSelectedRows)
        {
            object[] columnData;

            if (useSelectedRows && _selectedRows != null)
            {
                columnData = new object[_selectedRows.Count];
            }
            else
            {
                columnData = new object[_dbView.NumberOfRows];
            }

            if (!t.ContainsVariable())
            {
                for (int i = 0; i < columnData.Length; i++)
                {
                    columnData[i] = t.Evaluate().Result;
                }
            }
            else
            {
                // Set up the variables for updating the values for each row
                List<VariableNode> varNodes = t.GetVariableNodes();
                int[] nodeToColumnIndices = new int[varNodes.Count];
                object[][] nodeColumnData = new object[varNodes.Count][];

                for (int i = 0; i < varNodes.Count; i++)
                {
                    nodeToColumnIndices[i] = Array.IndexOf(_dbView.ColumnNames, varNodes[i].VariableName);
                    nodeColumnData[i] = _dbView.GetColumn(varNodes[i].VariableName);
                }

                // Grab the appropriate row data and update the column information to be edited.
                if (useSelectedRows && _selectedRows != null)
                {
                    for (int i = 0; i < columnData.Length; i++)
                    {
                        for (int j = 0; j < nodeToColumnIndices.Length; j++)
                        {
                            varNodes[j].SetValue(nodeColumnData[j][_selectedRows[i]]);
                        }
                        columnData[i] = t.Evaluate().Result;
                    }
                }
                else
                {
                    for (int i = 0; i < columnData.Length; i++)
                    {
                        for (int j = 0; j < nodeToColumnIndices.Length; j++)
                        {
                            varNodes[j].SetValue(nodeColumnData[j][i]);
                        }
                        columnData[i] = t.Evaluate().Result;
                    }
                }
            }

            return columnData;
        }

        /// <summary>
        /// Evaluates the expression and applies the results to the data table.
        /// Handles both field calculation and "Select By Attribute" modes.
        /// </summary>
        /// <param name="t">The parsed expression tree to evaluate.</param>
        private void EvaluateColumn(IParserNode t)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            // Calculate the data
            bool useSelectedRows = UseSelectedRange.IsChecked == true;
            if (!UseSelectedRange.IsEnabled)
            {
                useSelectedRows = false; // use selected rows is not enabled so ignore.
            }
            if (_selectedRows == null || _selectedRows.Count == 0)
            {
                useSelectedRows = false; // there are no rows selected.
            }
            if (!_existingField)
            {
                useSelectedRows = false; // ignore use selected range since we need data for every row to create a new column.
            }

            object[] columnData = GetExpressionData(t, useSelectedRows);

            if (_isSelectByAttribute)
            {
                _rowsToSelect = new List<int>();
                if (useSelectedRows && _selectedRows != null)
                {
                    for (int i = 0; i < _selectedRows.Count; i++)
                    {
                        bool value = columnData[i] is DBNull ? false : Convert.ToBoolean(columnData[i]);
                        if (value)
                        {
                            _rowsToSelect.Add(_selectedRows[i]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < columnData.Length; i++)
                    {
                        bool value = columnData[i] is DBNull ? false : Convert.ToBoolean(columnData[i]);
                        if (value)
                        {
                            _rowsToSelect.Add(i);
                        }
                    }
                }

                Mouse.OverrideCursor = null;
                DialogResult = true;
                return;
            }

            // Add the data to the database
            if (!_existingField)
            {
                if (string.IsNullOrEmpty(NewFieldName.Text))
                {
                    GenericControls.MessageBox.Show("You have not named your new data field.");
                    Mouse.OverrideCursor = null;
                    return;
                }
                else if (Array.IndexOf(_dbView.ColumnNames, NewFieldName.Text) >= 0)
                {
                    GenericControls.MessageBox.Show("The field name, " + NewFieldName.Text + ", already exists. Please define a new field name, or choose the option \"Update Existing Field\".");
                    Mouse.OverrideCursor = null;
                    return;
                }

                _dbView.AddColumn(NewFieldName.Text, columnData, ParseNodeResult.ParserResultTypeToType(t.OutputType));
            }
            else
            {
                int columnIndex = Array.IndexOf(_dbView.ColumnNames, _fieldName);
                ResultType fcType = ParseNodeResult.TypeToParserResultType(_dbView.ColumnTypes[columnIndex]);
                if (fcType == ResultType.UnDeclared)
                {
                    GenericControls.MessageBox.Show("the type of the selected column is not supported for output");
                    Mouse.OverrideCursor = null;
                    return;
                }

                if (useSelectedRows && _selectedRows != null)
                {
                    // Loop through selected cells
                    int[] rowIndices = new int[_selectedRows.Count];
                    int[] columnIndices = new int[rowIndices.Length];
                    for (int i = 0; i < _selectedRows.Count; i++)
                    {
                        rowIndices[i] = _selectedRows[i];
                        columnIndices[i] = columnIndex;
                    }
                    try
                    {
                        _dbView.EditCells(rowIndices, columnIndices, columnData);
                    }
                    catch (Exception ex)
                    {
                        GenericControls.MessageBox.Show("Error editing the table:  " + ex.Message);
                        Mouse.OverrideCursor = null;
                        return;
                    }
                }
                else
                {
                    try
                    {
                        _dbView.EditColumn(columnIndex, columnData);
                    }
                    catch (Exception ex)
                    {
                        GenericControls.MessageBox.Show("Error editing the table:  " + ex.Message);
                        Mouse.OverrideCursor = null;
                        return;
                    }
                }
            }

            Mouse.OverrideCursor = null;
            DialogResult = true;
        }

        /// <summary>
        /// Handles the selection changed event for the existing fields combo box.
        /// Updates the target field name when a different field is selected.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ExistingFieldsCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UpdateExistingRadioButton.IsChecked == true)
            {
                _fieldName = ExistingFieldsCombobox.SelectedItem as string;
            }
        }

        /// <summary>
        /// Handles the checked event for the "Create New Field" radio button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CreateNewFieldRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Placeholder for future functionality
        }

        /// <summary>
        /// Handles the text changed event for the new field name text box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void NewFieldName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Placeholder for future functionality
        }

        /// <summary>
        /// Handles the checked event for the "Update Existing Field" radio button.
        /// Updates the existing field flag when the radio button is checked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void UpdateExistingRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _existingField = UpdateExistingRadioButton.IsChecked == true;
        }

        /// <summary>
        /// Handles the expression changed event from the calculator control.
        /// Validates the expression and updates the result preview.
        /// Error display is handled internally by the CalculatorControl.
        /// </summary>
        private void ExpressionCalculator_ExpressionChanged()
        {
            resultTextBlock.Text = "";

            // Check for parse errors
            IParserNode parseNode = ExpressionCalculator.GetParseTree();
            if (parseNode == null)
            {
                return;
            }

            // Use the CalculatorControl's HasErrors property
            ExecuteButton.IsEnabled = !ExpressionCalculator.HasErrors;

            if (ExpressionCalculator.HasErrors)
            {
                // Errors are displayed in the errors expander — no need for result text
            }
            else
            {
                if (_isSelectByAttribute && parseNode.OutputType != ResultType.Boolean)
                {
                    resultTextBlock.Text = "When selecting by attributes the expression result must be in true/false logical format. The current expression returns a result of type '" + parseNode.OutputType.ToString() + "'.";
                    ExecuteButton.IsEnabled = false;
                }
                else
                {
                    if (_dbView.NumberOfRows == 0)
                    {
                        return;
                    }

                    if (parseNode.ContainsVariable())
                    {
                        var variables = parseNode.GetVariableNodes();
                        object[] firstRow = _dbView.GetRow(0);
                        foreach (var var in variables)
                        {
                            int index = Array.IndexOf(_dbView.ColumnNames, var.VariableName);
                            if (index != -1)
                            {
                                var.SetValue(firstRow[index]);
                            }
                        }
                    }

                    if (_isSelectByAttribute)
                    {
                        if (Convert.ToBoolean(parseNode.Evaluate().Result))
                        {
                            resultTextBlock.Text = "First record (row 0) WILL get selected.";
                        }
                        else
                        {
                            resultTextBlock.Text = "First record (row 0) will NOT get selected.";
                        }
                    }
                    else
                    {
                        var result = parseNode.Evaluate();
                        if (result.Type == ResultType.Error)
                        {
                            resultTextBlock.Text = "Error attempting to evaluate the first data record.";
                        }
                        else
                        {
                            resultTextBlock.Text = $"First record (row 0) = '{result.Result}'";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the click event for the Insert Variable button.
        /// Inserts the selected variable into the expression.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void InsertVariableButton_Click(object sender, RoutedEventArgs e)
        {
            if (VariablesListBox.SelectedIndex == -1)
            {
                return;
            }
            if (VariablesListBox.SelectedItem == null)
            {
                return;
            }
            ExpressionCalculator.InsertText("[" + ((ListBoxItem)VariablesListBox.SelectedItem).Content + "]");
        }

        /// <summary>
        /// Handles the mouse double-click event for the variables list box.
        /// Inserts the double-clicked variable into the expression.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void VariablesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (VariablesListBox.SelectedIndex == -1)
            {
                return;
            }
            if (VariablesListBox.SelectedItem == null)
            {
                return;
            }
            ExpressionCalculator.InsertText("[" + ((ListBoxItem)VariablesListBox.SelectedItem).Content + "]");
        }

        /// <summary>
        /// Handles the selection changed event for the variables list box.
        /// Enables or disables the Insert Variable button based on selection state.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void VariablesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InsertVariableButton.IsEnabled = VariablesListBox.SelectedItem != null;
        }

        #region Functions Panel

        /// <summary>
        /// Handles the f(x) toggle from the CalculatorControl toolbar.
        /// Shows or hides the functions column in the 3-column layout.
        /// </summary>
        /// <param name="show">True to show, false to hide the functions column.</param>
        private void OnFunctionsToggleChanged(bool show)
        {
            if (show)
            {
                FunctionsColumn.Width = new GridLength(170);
                FunctionsColumn.MinWidth = 100;
                FunctionsLabel.Visibility = Visibility.Visible;
                FunctionsPanel.Visibility = Visibility.Visible;
                InsertFunctionButton.Visibility = Visibility.Visible;
            }
            else
            {
                FunctionsColumn.Width = new GridLength(0);
                FunctionsColumn.MinWidth = 0;
                FunctionsLabel.Visibility = Visibility.Collapsed;
                FunctionsPanel.Visibility = Visibility.Collapsed;
                InsertFunctionButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles help document navigation from the expression editor.
        /// Opens the functions panel and selects the matching function.
        /// </summary>
        /// <param name="helpDocumentPath">The help document path to navigate to.</param>
        private void OnHelpDocumentRequested(string helpDocumentPath)
        {
            // Ensure the functions panel is visible
            ExpressionCalculator.SetFunctionsToggle(true);

            // Select the function in the tree
            FunctionsPanel.SelectFunctionByHelpPath(helpDocumentPath);
        }

        /// <summary>
        /// Handles function selection changes from the functions panel.
        /// Updates the help expander in the CalculatorControl and enables the Insert button.
        /// </summary>
        /// <param name="func">The selected function descriptor, or null if deselected.</param>
        private void OnFunctionsPanelSelectedFunctionChanged(FunctionDescriptor? func)
        {
            if (func != null)
            {
                ExpressionCalculator.ShowFunctionHelp(func);
                InsertFunctionButton.IsEnabled = true;
            }
            else
            {
                InsertFunctionButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Handles the Insert button click for the functions column.
        /// Inserts the selected function's text into the expression.
        /// </summary>
        private void InsertFunctionButton_Click(object sender, RoutedEventArgs e)
        {
            FunctionsPanel.InsertSelectedFunction();
        }

        #endregion
    }
}
