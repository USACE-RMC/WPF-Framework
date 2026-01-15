Imports DatabaseManager
Imports ExpressionParser
Public Class FieldCalculator
    Private _selectedText As String
    Private ReadOnly _dbView As DataTableView
    Private _existingField As Boolean
    Private _fieldName As String
    Private ReadOnly _selectedRows As List(Of Int32)
    Private ReadOnly _isSelectByAttribute As Boolean
    Private _rowsToSelect As New List(Of Int32)
    Public ReadOnly Property GetSelectedRows As List(Of Int32)
        Get
            Return _rowsToSelect
        End Get
    End Property

    Sub New(dataView As DataTableView, selectedRows As List(Of Int32), readOnlyColumns As HashSet(Of Int32), Optional ByVal header As String = Nothing, Optional ByVal IsSelectByAttribute As Boolean = False)

        ' This call is required by the designer.
        InitializeComponent()
        '
        _isSelectByAttribute = IsSelectByAttribute
        If _isSelectByAttribute = True Then Me.Title = "Select By Attribute"
        _dbView = dataView
        _selectedRows = selectedRows
        'Set variables
        VariablesListBox.Items.Clear()
        Dim vars As New Dictionary(Of String, ResultType)
        For i As Int32 = 0 To _dbView.ColumnNames.Count - 1
            If vars.ContainsKey(_dbView.ColumnNames(i)) Then Continue For
            vars.Add(_dbView.ColumnNames(i), Parser.TypeToResultType(_dbView.ColumnTypes(i)))
            VariablesListBox.Items.Add(New ListBoxItem() With {.Content = _dbView.ColumnNames(i)})
        Next
        ExpressionCalculator.SetVariables(vars)
        '
        If _isSelectByAttribute Then
            FieldSelectionGrid.Visibility = Visibility.Collapsed
            FieldSelectionGrid.IsEnabled = False
            If IsNothing(_selectedRows) OrElse _selectedRows.Count = 0 Then
                UseSelectedRange.IsEnabled = False
            Else
                UseSelectedRange.IsEnabled = True
                UseSelectedRange.IsChecked = True
            End If
            'UseSelectedRange.IsEnabled = False
            'UseSelectedRange.Visibility = Visibility.Collapsed
            'resultTextBlock.Visibility = Visibility.Collapsed
        Else
            If IsNothing(_selectedRows) OrElse _selectedRows.Count = 0 Then
                UseSelectedRange.IsEnabled = False
            Else
                UseSelectedRange.IsEnabled = True
                UseSelectedRange.IsChecked = True
            End If
            '
            If IsNothing(header) Then
                UpdateExistingRadioButton.IsChecked = False
                CreateNewFieldRadioButton.IsChecked = True
                _existingField = UpdateExistingRadioButton.IsChecked
                For i = 0 To _dbView.ColumnNames.Count - 1
                    If readOnlyColumns.Contains(i) Then Continue For
                    ExistingFieldsCombobox.Items.Add(_dbView.ColumnNames(i))
                Next
                ExistingFieldsCombobox.SelectedItem = If(_dbView.ColumnNames.Length > 0, _dbView.ColumnNames(0), Nothing)
            Else
                ExistingFieldsCombobox.Items.Add(header)
                ExistingFieldsCombobox.SelectedIndex = 0
                FieldSelectionGrid.Visibility = Windows.Visibility.Collapsed
                FieldSelectionGrid.IsEnabled = False
                _existingField = True
                _fieldName = header
            End If
        End If

    End Sub

    Private Sub ExecuteButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles ExecuteButton.Click
        If _dbView.NumberOfRows = 0 Then
            MsgBox("No rows exist. Cannot execute field calculator without data rows.", MsgBoxStyle.Critical, "No Rows Exist")
            Exit Sub
        End If
        Dim tree As IParserNode = ExpressionCalculator.GetParseTree
        If Not tree Is Nothing Then
            EvaluateColumn(tree)
        End If
    End Sub

    Private Function GetExpressionData(t As IParserNode, useSelectedRows As Boolean) As Object()
        Dim columnData() As Object

        If useSelectedRows = True Then ReDim columnData(_selectedRows.Count - 1) Else ReDim columnData(_dbView.NumberOfRows - 1)
        If t.ContainsVariable = False Then
            For i As Int32 = 0 To columnData.Count - 1
                columnData(i) = t.Evaluate().Result
            Next
        Else
            'set up the variables for updating the values for each row
            Dim varNodes As List(Of VariableNode) = t.GetVariableNodes()
            Dim nodeToColumnIndices(varNodes.Count - 1) As Int32
            Dim nodeColumnData(varNodes.Count - 1)() As Object
            For i As Int32 = 0 To varNodes.Count - 1
                nodeToColumnIndices(i) = Array.IndexOf(_dbView.ColumnNames, varNodes(i).VariableName)
                nodeColumnData(i) = _dbView.GetColumn(varNodes(i).VariableName)
            Next
            'grab the appropriate row data and update the column information to be edited.
            If useSelectedRows = True Then
                For i As Int32 = 0 To columnData.Count - 1
                    For j As Int32 = 0 To nodeToColumnIndices.Count - 1
                        varNodes(j).SetValue(nodeColumnData(j)(_selectedRows(i)))
                    Next
                    columnData(i) = t.Evaluate().Result
                Next
            Else
                For i As Int32 = 0 To columnData.Count - 1
                    For j As Int32 = 0 To nodeToColumnIndices.Count - 1
                        varNodes(j).SetValue(nodeColumnData(j)(i))
                    Next
                    columnData(i) = t.Evaluate().Result
                Next
            End If
        End If
        '
        Return columnData
    End Function
    Private Sub EvaluateColumn(t As IParserNode)
        Mouse.OverrideCursor = Cursors.Wait
        'Calculate the data
        Dim useSelectedRows As Boolean = UseSelectedRange.IsChecked
        If UseSelectedRange.IsEnabled = False Then useSelectedRows = False 'use selected rows is not enabled so ignore.
        If IsNothing(_selectedRows) OrElse _selectedRows.Count = 0 Then useSelectedRows = False 'there are no rows selected.
        If _existingField = False Then useSelectedRows = False 'ignore use selected range since we need data for every row to create a new column.
        Dim columnData() As Object = GetExpressionData(t, useSelectedRows)
        '
        If _isSelectByAttribute Then
            _rowsToSelect = New List(Of Int32)
            If useSelectedRows Then
                For i As Int32 = 0 To _selectedRows.Count - 1
                    If If(IsDBNull(columnData(i)), False, CBool(columnData(i))) = True Then
                        _rowsToSelect.Add(_selectedRows(i))
                    End If
                Next
            Else
                For i As Int32 = 0 To columnData.Count - 1
                    If If(IsDBNull(columnData(i)), False, CBool(columnData(i))) = True Then
                        _rowsToSelect.Add(i)
                    End If
                Next
            End If

            Mouse.OverrideCursor = Nothing
            DialogResult = True
            Exit Sub
        End If
        '
        'Add the data to the database
        If _existingField = False Then
            If NewFieldName.Text = "" Then
                MsgBox("You have not named your new data field.")
                Mouse.OverrideCursor = Nothing
                Exit Sub
            ElseIf _dbView.ColumnNames.Contains(NewFieldName.Text) Then
                MsgBox("The field name, " & NewFieldName.Text & ", already exists. Please define a new field name, or choose the option " & Chr(34) & "Update Existing Field" & Chr(34) & ".")
                Mouse.OverrideCursor = Nothing
                Exit Sub
            End If
            '
            _dbView.AddColumn(NewFieldName.Text, columnData, ParseNodeResult.ParserResultTypeToType(t.OutputType))
        Else
            Dim columnIndex As Integer = Array.IndexOf(_dbView.ColumnNames, _fieldName)
            Dim fcType As ResultType = ParseNodeResult.TypeToParserResultType(_dbView.ColumnTypes(columnIndex))
            If fcType = ResultType.UnDeclared Then
                MsgBox("the type of the selected column is not supported for output")
                Mouse.OverrideCursor = Nothing
                Exit Sub
            End If
            '
            If useSelectedRows Then
                'loop through selected cells
                Dim rowIndices(_selectedRows.Count - 1) As Int32
                Dim columnIndices(rowIndices.Count - 1) As Int32
                For i = 0 To _selectedRows.Count - 1
                    rowIndices(i) = _selectedRows(i)
                    columnIndices(i) = columnIndex
                Next
                Try
                    _dbView.EditCells(rowIndices, columnIndices, columnData)
                Catch ex As Exception
                    MsgBox("Error editing the table:  " & ex.Message)
                    Mouse.OverrideCursor = Nothing
                    Exit Sub
                End Try
            Else
                Try
                    _dbView.EditColumn(columnIndex, columnData)
                Catch ex As Exception
                    MsgBox("Error editing the table:  " & ex.Message)
                    Mouse.OverrideCursor = Nothing
                    Exit Sub
                End Try
            End If
        End If
        '
        Mouse.OverrideCursor = Nothing
        DialogResult = True
    End Sub

    Private Sub ExistingFieldsCombobox_SelectionChanged(sender As System.Object, e As System.Windows.Controls.SelectionChangedEventArgs) Handles ExistingFieldsCombobox.SelectionChanged
        If UpdateExistingRadioButton.IsChecked Then
            _fieldName = ExistingFieldsCombobox.SelectedItem
            'UpdateHeader(ExistingFieldsCombobox.SelectedItem)
            'ExpressionWindow.SetOutputType = GetTreeType()
            'ExpressionWindow.Parse()
        End If
    End Sub
    Private Sub CreateNewFieldRadioButton_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles CreateNewFieldRadioButton.Checked
        'If IsLoaded Then
        'UpdateExistingRadioButton.IsChecked = False
        'ExistingFieldsCombobox.IsEnabled = False
        'NewFieldName.IsEnabled = True
        'If NewFieldName.Text <> "" Then
        '    UpdateHeader(NewFieldName.Text)
        'Else
        '    UpdateHeader("No Field name has been provided")
        'End If
        'UseSelectedRange.IsEnabled = False
        '_existingField = UpdateExistingRadioButton.IsChecked
        'ExpressionWindow.SetOutputType = Nothing
        'ExpressionWindow.Parse()
        'End If
    End Sub
    Private Sub NewFieldName_TextChanged(sender As System.Object, e As System.Windows.Controls.TextChangedEventArgs) Handles NewFieldName.TextChanged
        ' If CreateNewFieldRadioButton.IsChecked Then UpdateHeader(NewFieldName.Text)
    End Sub
    Private Sub UpdateExistingRadioButton_Checked(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles UpdateExistingRadioButton.Checked
        If IsLoaded Then
            'ExistingFieldsCombobox.IsEnabled = True
            'CreateNewFieldRadioButton.IsChecked = False
            'NewFieldName.IsEnabled = False
            'If ExistingFieldsCombobox.Items.Count <> 0 Then
            '    If ExistingFieldsCombobox.SelectedItem <> "" Then
            '        UpdateHeader(ExistingFieldsCombobox.SelectedItem)
            '    Else
            '        ExistingFieldsCombobox.SelectedItem = ExistingFieldsCombobox.Items(0)
            '        UpdateHeader(ExistingFieldsCombobox.SelectedItem)
            '    End If
            'End If
            'If _selectedRows.Count > 0 Then UseSelectedRange.IsEnabled = True
            'ExpressionWindow.SetOutputType = GetTreeType()
            'ExpressionWindow.Parse()
        End If
        _existingField = UpdateExistingRadioButton.IsChecked

    End Sub

    Private Sub ErrorLogButton_Click(sender As Object, e As RoutedEventArgs) Handles ErrorLogButton.Click
        If ExpressionCalculator.GetParseTree.ContainsErrors = False Then Exit Sub
        Dim errors = ExpressionCalculator.GetParseTree.GetErrors()
        Dim errorwindow As New ErrorWindow(errors, "Errors Encountered in Expression")
        errorwindow.Show()
        'End If
    End Sub

    Private Sub ExpressionCalculator_ExpressionChanged()
        resultTextBlock.Text = ""
        ErrorLogButton.IsEnabled = False
        'check for parse errors
        Dim parseNode As IParserNode = ExpressionCalculator.GetParseTree()
        If IsNothing(parseNode) Then Exit Sub
        Dim errorList As List(Of ParseError) = parseNode.GetErrors()
        'check that output type is correct
        ErrorLogButton.IsEnabled = errorList.Count > 0
        ExecuteButton.IsEnabled = errorList.Count = 0
        If errorList.Count > 0 Then
            resultTextBlock.Text = "Errors found in expression"
        Else
            If _isSelectByAttribute And parseNode.OutputType <> ResultType.Boolean Then
                resultTextBlock.Text = "When selecting by attributes the expression result must be in true/false logical format. The current expression returns a result of type '" & parseNode.OutputType.ToString & "'."
                ExecuteButton.IsEnabled = False
            Else
                If _dbView.NumberOfRows = 0 Then Exit Sub
                If parseNode.ContainsVariable Then
                    Dim variables = parseNode.GetVariableNodes()
                    Dim firstrow() As Object = _dbView.GetRow(0)
                    For Each var In variables
                        Dim index As Int32 = Array.IndexOf(_dbView.ColumnNames, var.VariableName)
                        If index <> -1 Then var.SetValue(firstrow(index))
                    Next
                End If
                If _isSelectByAttribute Then
                    If CBool(parseNode.Evaluate().Result) = True Then
                        resultTextBlock.Text = "First record (row 0) WILL get selected."
                    Else
                        resultTextBlock.Text = "First record (row 0) will NOT get selected."
                    End If
                    'ExpressionWindow.GetResult & "'"
                Else
                    Dim result = parseNode.Evaluate()
                    If result.Type = ResultType.Error Then
                        resultTextBlock.Text = $"Error attempting to evaluate the first data record."
                    Else
                        resultTextBlock.Text = $"First record (row 0) = '{result.Result}'"
                    End If
                End If
            End If
        End If
    End Sub

    'Private Sub ExpressionCalculator_HelpCalled(helpDocumentPath As String)
    '    HelpExpander.IsExpanded = True
    '    If IO.File.Exists(helpDocumentPath) = False Then HelpBrowser.Navigate("about:blank")
    '    Try
    '        HelpBrowser.Navigate(New Uri(helpDocumentPath))
    '    Catch ex As Exception
    '        HelpBrowser.Navigate("about:blank")
    '    End Try
    'End Sub

    'Private Sub HelpExpander_Expanded(sender As Object, e As RoutedEventArgs)
    '    Debug.Print("Is Expanded = " & HelpExpander.IsExpanded)
    'End Sub

    Private Sub InsertVariableButton_Click(sender As Object, e As RoutedEventArgs)
        If VariablesListBox.SelectedIndex = -1 Then Exit Sub
        If IsNothing(VariablesListBox.SelectedItem) Then Exit Sub
        ExpressionCalculator.InsertText("[" & DirectCast(VariablesListBox.SelectedItem, ListBoxItem).Content & "]")
    End Sub

    Private Sub VariablesListBox_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        If VariablesListBox.SelectedIndex = -1 Then Exit Sub
        If IsNothing(VariablesListBox.SelectedItem) Then Exit Sub
        ExpressionCalculator.InsertText("[" & DirectCast(VariablesListBox.SelectedItem, ListBoxItem).Content & "]")
    End Sub

    Private Sub VariablesListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        InsertVariableButton.IsEnabled = Not IsNothing(VariablesListBox.SelectedItem)
    End Sub
End Class
