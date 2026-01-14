Imports DatabaseManager.DatabaseManager

Public Class ColumnStatsWindow
    Private ReadOnly _viewer As TableViewer
    Private ReadOnly _columnIndex As Int32
    Private _isNumeric As Boolean

    Public Sub New(theViewer As TableViewer, columnIndex As Int32)
        InitializeComponent()
        '
        _columnIndex = columnIndex
        _viewer = theViewer
        If IsNothing(_viewer) Then Exit Sub
        '
        Dim fieldName As String = _viewer.DataView.ColumnNames(_columnIndex)
        Title = fieldName & " Summary Statistics"
        '
        _isNumeric = IsNumericType(_viewer.DataView.ColumnTypes(_columnIndex))
        '
        If _isNumeric Then
            NumericColumnViewer.Visibility = Visibility.Visible
            NumericColumnViewer.DataName = fieldName
            AlphabeticColumnViewer.Visibility = Visibility.Collapsed
        Else
            NumericColumnViewer.Visibility = Visibility.Collapsed
            AlphabeticColumnViewer.Visibility = Visibility.Visible
            AlphabeticColumnViewer.DataName = fieldName
        End If

        AddHandler _viewer.SelectedRowIndicesChanged, AddressOf ViewerSelectionChanged
        '
    End Sub

    Private Sub ViewerSelectionChanged(selectedRowIndices As List(Of Integer))
        If (selectedRowIndices Is Nothing OrElse selectedRowIndices.Count = 0) Then
            SelectedOnlyCheckbox.IsEnabled = False
            SelectedOnlyCheckbox.IsChecked = False
        Else
            SelectedOnlyCheckbox.IsEnabled = True
        End If
    End Sub


    Private Sub ColumnStatsWindow_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
        If _viewer Is Nothing Then Exit Sub
        If _viewer.GetSelectedRows.Count > 0 Then SelectedOnlyCheckbox.IsEnabled = True

        Dim fieldData = _viewer.DataView.GetColumn(_viewer.DataView.ColumnNames(_columnIndex))
        If _isNumeric Then
            NumericColumnViewer.Data = Array.ConvertAll(fieldData, Function(o) If(IsDBNull(o), 0, CDbl(o)))
        Else
            AlphabeticColumnViewer.Data = fieldData
        End If
    End Sub

    Private Sub ColumnStatsWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        RemoveHandler _viewer.SelectedRowIndicesChanged, AddressOf ViewerSelectionChanged
    End Sub

    Private Sub SelectedOnlyCheckbox_Checked(sender As Object, e As RoutedEventArgs)
        Dim fieldData() As Object
        '
        Dim tempData() As Object = _viewer.DataView.GetColumn(_viewer.DataView.ColumnNames(_columnIndex))
        Dim selectedRows As List(Of Int32) = _viewer.GetSelectedRows
        If selectedRows.Count > 0 Then
            Dim realData As New List(Of Object)
            For i As Int32 = 0 To selectedRows.Count - 1
                realData.Add(tempData(selectedRows(i)))
            Next
            fieldData = realData.ToArray
        Else
            fieldData = tempData
        End If
        '
        If _isNumeric Then
            NumericColumnViewer.Data = Array.ConvertAll(fieldData, Function(o) If(IsDBNull(o), 0, CDbl(o)))
        Else
            AlphabeticColumnViewer.Data = fieldData
        End If
    End Sub

    Private Sub SelectedOnlyCheckbox_Unchecked(sender As Object, e As RoutedEventArgs)
        Dim fieldData() As Object = _viewer.DataView.GetColumn(_viewer.DataView.ColumnNames(_columnIndex))
        '
        If _isNumeric Then
            NumericColumnViewer.Data = Array.ConvertAll(fieldData, Function(o) If(IsDBNull(o), 0, CDbl(o)))
        Else
            AlphabeticColumnViewer.Data = fieldData
        End If
    End Sub

End Class
