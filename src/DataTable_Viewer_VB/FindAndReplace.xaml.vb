Public Class FindAndReplace
    Private ReadOnly _tableViewer As TableViewer
    Private _currentRow As Int32
    Private ReadOnly _columnIndex As Int32
    Private _foundInstance As Boolean = False
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Sub New(tableViewer As TableViewer, columnIndex As Int32, startRow As Int32)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _tableViewer = tableViewer
        _columnIndex = columnIndex
        Title = "Find In: " & tableViewer.DataView.ColumnNames(columnIndex)
        _currentRow = startRow
    End Sub

    Private Sub FindNextButton_Click(sender As System.Object, e As RoutedEventArgs) Handles FindNextButton.Click
        If FindText.Text = "" Then Exit Sub
        _currentRow += 1
        If _currentRow = _tableViewer.DataView.NumberOfRows Then _currentRow = 0
        Dim searchIndex As Int32 = _tableViewer.DataView.SearchColumn(_currentRow, _tableViewer.DataView.NumberOfRows - 1, _columnIndex, FindText.Text, CBool(MatchCaseCheckbox.IsChecked), CBool(MatchWordCheckbox.IsChecked)) 'SearchColumn(_CurrentRow, _TableViewer.DataView.NumberOfRows - 1, FindText.Text, MatchCaseCheckbox.IsChecked, MatchWordCheckbox.IsChecked)
        If SearchIndex >= 0 Then
            _foundInstance = True
            _tableViewer.SetActiveCell(searchIndex, _columnIndex, True)
            _currentRow = searchIndex
        Else
            _currentRow = 0
            SearchIndex = _tableViewer.DataView.SearchColumn(_currentRow, _tableViewer.DataView.NumberOfRows - 1, _columnIndex, FindText.Text, CBool(MatchCaseCheckbox.IsChecked), CBool(MatchWordCheckbox.IsChecked)) 'SearchColumn(_CurrentRow, _TableViewer.DataView.NumberOfRows - 1, FindText.Text, MatchCaseCheckbox.IsChecked, MatchWordCheckbox.IsChecked)
            If SearchIndex >= 0 Then
                _foundInstance = True
                _tableViewer.SetActiveCell(searchIndex, _columnIndex, True)
                _currentRow = searchIndex ' + 1
            End If
        End If
        '
        If _foundInstance = False Then MsgBox("Search for: '" & FindText.Text & "' was not found.")
    End Sub

    Private Sub FindPreviousButton_Click(sender As System.Object, e As RoutedEventArgs) Handles FindPreviousButton.Click
        If FindText.Text = "" Then Exit Sub
        _currentRow -= 1
        If _currentRow = -1 Then _currentRow = _tableViewer.DataView.NumberOfRows - 1
        Dim searchIndex As Int32 = _tableViewer.DataView.SearchColumn(_currentRow, 0, _columnIndex, FindText.Text, CBool(MatchCaseCheckbox.IsChecked), CBool(MatchWordCheckbox.IsChecked)) 'SearchColumn(_CurrentRow, 0, FindText.Text, MatchCaseCheckbox.IsChecked, MatchWordCheckbox.IsChecked)
        If searchIndex >= 0 Then
            _foundInstance = True
            _tableViewer.SetActiveCell(searchIndex, _columnIndex, True)
            _currentRow = searchIndex ' - 1
        Else
            _currentRow = _tableViewer.DataView.NumberOfRows - 1
            searchIndex = _tableViewer.DataView.SearchColumn(_currentRow, 0, _columnIndex, FindText.Text, CBool(MatchCaseCheckbox.IsChecked), CBool(MatchWordCheckbox.IsChecked)) 'SearchColumn(_CurrentRow, 0, FindText.Text, MatchCaseCheckbox.IsChecked, MatchWordCheckbox.IsChecked)
            If searchIndex >= 0 Then
                _foundInstance = True
                _tableViewer.SetActiveCell(searchIndex, _columnIndex, True)
                _currentRow = searchIndex ' - 1
            End If
        End If
        '
        If _foundInstance = False Then MsgBox("Search for: '" & FindText.Text & "' was not found.")
    End Sub
    Private Sub FindText_TextChanged(sender As System.Object, e As TextChangedEventArgs) Handles FindText.TextChanged
        _foundInstance = False
    End Sub
End Class
