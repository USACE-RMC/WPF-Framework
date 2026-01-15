Imports ExpressionParser
Public Class SelectByAttribute
    'Private _firstparts As String
    'Private _secondparts As String
    'Private _caretIndex As Integer
    'Private _selectedtext As String
    Private ReadOnly _dbView As DataBase_Reader.DataTableView
    'Private ReadOnly _wfgrid As WpfCustomViewer
    Private ReadOnly _selection As List(Of Int32)
    'Private _parseerrors As List(Of String)
    Sub New(view As DataBase_Reader.DataTableView)
        ' This call is required by the designer.
        InitializeComponent()
        '_wfgrid = wpfviewer
        'AddHandler ExpressionWindow.ParseSuccess, AddressOf DisplayResult
        'AddHandler ExpressionWindow.ErrorsFound, AddressOf ErrorsFound
        'AddHandler AvailableFunctions.TextToAdd, AddressOf UpdateTextbox
        'Grid.SetRow(ExpressionWindow.GetTextBlock, 2)
        'ExpressionGrid.Children.Add(ExpressionWindow.GetTextBlock)

        _dbView = view
        _selection = New List(Of Int32)
        'SelectedField.Text = "Select from " & System.IO.Path.GetFileNameWithoutExtension(_dbView.ParentDatabase.DataBasePath) & " where:"
        'ExpressionWindow.SetHeaders = _dbView.ColumnNames.ToList()
        'ExpressionWindow.SetHeaderTypes = _dbView.ColumnTypes.ToList()
        'ExpressionWindow.SetOutputType = GetType(Boolean)
        'If _dbView.NumberOfRows > 0 Then ExpressionWindow.SetDataForFirstRow = _dbView.GetRow(0)
        'For i = 0 To _dbView.ColumnNames.Count - 1
        '    AvailableFields.Items.Add(_dbView.ColumnNames(i))
        'Next
        Dim vars As New Dictionary(Of String, ResultType)
        For i As Int32 = 0 To _dbView.ColumnNames.Count - 1
            If vars.ContainsKey(_dbView.ColumnNames(i)) Then Continue For
            vars.Add(_dbView.ColumnNames(i), Parser.TypeToResultType(_dbView.ColumnTypes(i)))
        Next
        ExpressionCalculator.SetVariables(vars)
    End Sub
    Public ReadOnly Property GetSelectedRows As List(Of Int32)
        Get
            Return _selection
        End Get
    End Property

    'Private Sub DisplayResult()
    '    'resulttextblock.Text = "Example: First data record (row id = 0) would be equal to '" & ExpressionWindow.GetResult & "'"
    '    ErrorLogButton.IsEnabled = False
    '    If _dbView.NumberOfRows > 0 Then ExecuteButton.IsEnabled = True
    'End Sub
    'Private Sub ErrorsFound(treeIsNothing As Boolean)
    'If Not treeIsNothing Then
    '    _parseErrors = ExpressionWindow.GetTree.GetParseErrors
    '    _parseErrors.AddRange(ExpressionWindow.GetTree.GetErrorMessages)
    '    ErrorLogButton.IsEnabled = True
    'End If
    'ExecuteButton.IsEnabled = False
    'resultTextBlock.Text = ExpressionWindow.GetResult
    'End Sub
    'Sub UpdateTextbox(texttoinsert As String)
    '    If IsNothing(_selectedtext) Then
    '    Else
    '        ExpressionWindow.Text = ExpressionWindow.Text.Remove(_caretIndex, _selectedtext.Count)
    '    End If
    '    _firstparts = Strings.Left(ExpressionWindow.Text, _caretIndex)
    '    _secondparts = Strings.Right(ExpressionWindow.Text, ExpressionWindow.Text.Length - _caretIndex)
    '    inserttext(texttoinsert)
    'End Sub
    'Sub InsertText(texttoinsert As String)
    '    ExpressionWindow.Text = _firstparts & texttoinsert & _secondparts
    '    ExpressionWindow.Focus()
    '    ExpressionWindow.CaretIndex = _firstparts.Length + texttoinsert.Length
    '    ExpressionWindow.Parse()
    'End Sub
    Private Sub ExecuteButton_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles ExecuteButton.Click
        'Mouse.OverrideCursor = Cursors.Wait
        'Dim tree As FieldCalculationParser.ParseTreeNode = ExpressionWindow.GetTree
        'If Not tree Is Nothing Then
        '    If tree.containsVariable Then
        '        EVALUATECOLUMN(tree)
        '    Else
        '        MsgBox("Your Expression does not contain a variable, the resulting selection will be all rows or no rows...")
        '        EVALUATECOLUMN(tree)
        '    End If
        'End If
    End Sub
    Private Sub EvaluateColumn(t As IParserNode)
        '    Dim headers As List(Of String) = t.GetHeaderNames
        '    Dim uniques As New List(Of String)
        '    For i As Int32 = 0 To headers.Count - 1
        '        If Not uniques.Contains(headers(i)) Then
        '            uniques.Add(headers(i))
        '        End If
        '    Next
        '    t.SetColNums(uniques)
        '    Dim types As New List(Of Type)
        '    Dim data As New List(Of Object())
        '    For i = 0 To uniques.Count - 1
        '        types.Add(_dbView.ColumnTypes(array.IndexOf(_dbView.ColumnNames, uniques(i)))) '_variablenames.IndexOf(uniques(i))))
        '        data.Add(_dbView.GetColumn(array.IndexOf(_dbView.ColumnNames, uniques(i))))
        '    Next
        '    Dim r(uniques.Count - 1) As Object
        '    Select Case t.Type
        '        Case FieldCalculationParser.TypeEnum.Bool
        '            For i = 0 To _dbView.NumberOfRows - 1
        '                For j = 0 To uniques.Count - 1
        '                    r(j) = data(j)(i)
        '                Next
        '                FieldCalculationParser.ParseTreeNode.RowOrCellNum = i
        '                t.Update(r)
        '                If t.Evaluate.GetResult Then
        '                    _selection.Add(i)
        '                End If
        '            Next
        '        Case FieldCalculationParser.TypeEnum.Str
        '            MsgBox("Cannot select by string, change the expression to yeild a boolean result")
        '            Mouse.OverrideCursor = Nothing
        '            Exit Sub

        '        Case FieldCalculationParser.TypeEnum.Num
        '            MsgBox("Cannot select by numerical expression, change the expression to yeild a boolean result")
        '            Mouse.OverrideCursor = Nothing
        '            Exit Sub
        '    End Select
        '    Mouse.OverrideCursor = Nothing
        '    DialogResult = True

    End Sub
    'Private Sub TextBox1_LostFocus(sender As Object, e As System.Windows.RoutedEventArgs) Handles ExpressionWindow.LostFocus
    '    _caretIndex = ExpressionWindow.CaretIndex
    '    _selectedtext = ExpressionWindow.SelectedText
    'End Sub
    'Private Sub EQ_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles EQButton.Click
    '    UpdateTextbox("=")
    'End Sub
    'Private Sub Exponent_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles ExponentButton.Click
    '    UpdateTextbox("^")
    'End Sub

    'Private Sub Divide_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles DivideButton.Click
    '    UpdateTextbox("/")
    'End Sub

    'Private Sub Multiply_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles MultiplyButton.Click
    '    UpdateTextbox("*")
    'End Sub

    'Private Sub Add_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles AddButton.Click
    '    UpdateTextbox("+")
    'End Sub

    'Private Sub Subtract_Click(sender As System.Object, e As System.Windows.RoutedEventArgs) Handles SubtractButton.Click
    '    UpdateTextbox("-")
    'End Sub
    'Private Sub AvailableFields_MouseDoubleClick(sender As Object, e As System.Windows.Input.MouseButtonEventArgs) Handles AvailableFields.MouseDoubleClick
    '    UpdateTextbox("[" & AvailableFields.SelectedItem & "]")
    'End Sub

    Private Sub ErrorLogButton_Click(sender As Object, e As RoutedEventArgs) Handles ErrorLogButton.Click
        'If _parseerrors.Count = 0 Then
        'Else
        '    Dim errorwindow As New ErrorWindow(_parseerrors, "Errors Encountered in Expression")
        '    errorwindow.Show()
        'End If
    End Sub

    Private Sub SelectByAttribute_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
        'If ExpressionWindow.Text <> "" Then ExpressionWindow.Parse()
    End Sub

    'Private Sub IsCaseSensitive_Checked(sender As Object, e As RoutedEventArgs) Handles IsCaseSensitive.Checked
    '    ExpressionWindow.IsCaseSensitive = IsCaseSensitive.IsChecked
    'End Sub
    'Private Sub IsCaseSensitive_UnChecked(sender As Object, e As RoutedEventArgs) Handles IsCaseSensitive.Unchecked
    '    ExpressionWindow.IsCaseSensitive = IsCaseSensitive.IsChecked
    'End Sub
End Class
