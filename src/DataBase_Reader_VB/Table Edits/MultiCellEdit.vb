Public Class MultiCellEdit
    Inherits TableEdit

    Private ReadOnly _cellEdits() As CellEdit
    Private _minColumnIndex As Int32
    Private _maxColumnIndex As Int32
    Private _minRowIndex As Int32
    Private _maxRowIndex As Int32
    Public ReadOnly Property CellEdits() As CellEdit()
        Get
            Return _cellEdits
        End Get
    End Property
    Public Sub New(cellEdits() As CellEdit)
        _cellEdits = cellEdits
        _minColumnIndex = cellEdits(0).ColumnIndex
        _maxColumnIndex = _minColumnIndex
        _minRowIndex = cellEdits(0).RowIndex
        _maxRowIndex = _minRowIndex
        '
        For Each edit As CellEdit In cellEdits
            If _minColumnIndex > edit.ColumnIndex Then _minColumnIndex = edit.ColumnIndex
            If _maxColumnIndex < edit.ColumnIndex Then _maxColumnIndex = edit.ColumnIndex
            If _minRowIndex > edit.RowIndex Then _minRowIndex = edit.RowIndex
            If _maxRowIndex < edit.RowIndex Then _maxRowIndex = edit.RowIndex
        Next
    End Sub

    Public Overrides Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        If indexOfColumn < _minColumnIndex Then Return result
        If indexOfColumn > _maxColumnIndex Then Return result
        For i As Int32 = 0 To _cellEdits.Count - 1
            If _cellEdits(i).ColumnIndex = indexOfColumn And _cellEdits(i).RowIndex >= 0 Then result.Add(_cellEdits(i))
        Next
        Return result
    End Function

    Public Overrides Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        If indexOfRow < _minRowIndex Then Return result
        If indexOfRow > _maxRowIndex Then Return result
        For i As Int32 = 0 To _cellEdits.Count - 1
            If _cellEdits(i).RowIndex = indexOfRow And _cellEdits(i).ColumnIndex >= 0 Then result.Add(_cellEdits(i))
        Next
        Return result
    End Function

    Public Overrides Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional ByVal columnData() As T = Nothing)
        If indexOfColumn > _maxColumnIndex Then Exit Sub
        For i As Int32 = 0 To _cellEdits.Count - 1
            _cellEdits(i).ColumnAdded(indexOfColumn, columnData)
        Next
        If indexOfColumn <= _minColumnIndex Then _minColumnIndex += 1
        If indexOfColumn <= _maxColumnIndex Then _maxColumnIndex += 1
    End Sub

    Public Overrides Sub ColumnDeleted(indexOfColumn As Integer)
        If indexOfColumn > _maxColumnIndex Then Exit Sub
        For i As Int32 = 0 To _cellEdits.Count - 1
            _cellEdits(i).ColumnDeleted(indexOfColumn)
        Next
        If indexOfColumn < _minColumnIndex Then _minColumnIndex -= 1
        If indexOfColumn < _maxColumnIndex Then _maxColumnIndex -= 1
    End Sub

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional ByVal rowData() As Object = Nothing)
        If indexOfRow > _maxRowIndex Then Exit Sub
        For i As Int32 = 0 To _cellEdits.Count - 1
            _cellEdits(i).RowAdded(indexOfRow)
        Next
        If indexOfRow <= _minRowIndex Then _minRowIndex += 1
        If indexOfRow <= _maxRowIndex Then _maxRowIndex += 1
    End Sub

    Public Overrides Sub RowDeleted(indexOfRow As Integer)
        If indexOfRow > _maxRowIndex Then Exit Sub
        For i As Int32 = 0 To _cellEdits.Count - 1
            _cellEdits(i).RowDeleted(indexOfRow)
        Next
        If indexOfRow < _minRowIndex Then _minRowIndex -= 1
        If indexOfRow < _maxRowIndex Then _maxRowIndex -= 1
    End Sub
    Public Overrides Function ContainsCell(indexOfColumn As Integer, indexOfRow As Integer, ByRef returnValue As Object) As Boolean
        If indexOfRow < _minRowIndex Then Return False
        If indexOfRow > _maxRowIndex Then Return False
        If indexOfColumn < _minColumnIndex Then Return False
        If indexOfColumn > _maxColumnIndex Then Return False
        For i As Int32 = 0 To _cellEdits.Count - 1
            If _cellEdits(i).ContainsCell(indexOfColumn, indexOfRow, returnValue) Then Return True
        Next
        Return False
    End Function
    Public Overrides Function ContainsColumn(indexOfColumn As Integer) As Boolean
        If indexOfColumn < _minColumnIndex Then Return False
        If indexOfColumn > _maxColumnIndex Then Return False
        For i As Int32 = 0 To _cellEdits.Count - 1
            If _cellEdits(i).ColumnIndex = indexOfColumn Then Return True
        Next
        Return False
    End Function
    Public Overrides Function ContainsRow(indexOfRow As Integer) As Boolean
        If indexOfRow < _minRowIndex Then Return False
        If indexOfRow > _maxRowIndex Then Return False
        For i As Int32 = 0 To _cellEdits.Count - 1
            If _cellEdits(i).RowIndex = indexOfRow Then Return True
        Next
        Return False
    End Function
End Class
