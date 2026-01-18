Public Class ColumnEdit
    Inherits TableEdit

    Private _columnIndex As Int32
    Private ReadOnly _columnData As List(Of Object)
    Private ReadOnly _removedValues As New List(Of Object)

    Public ReadOnly Property ColumnIndex As Int32
        Get
            Return _columnIndex
        End Get
    End Property
    Public ReadOnly Property ColumnData() As List(Of Object)
        Get
            Return _columnData
        End Get
    End Property

    Public Sub New(indexOfColumn As Int32, columnData() As Object)
        _columnIndex = indexOfColumn
        _columnData = columnData.ToList()
    End Sub
    Public Overrides Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
        If indexOfColumn <> _columnIndex Then Return New List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        For i As Int32 = 0 To _columnData.Count - 1
            result.Add(New CellEdit(i, _columnIndex, _columnData(i)))
        Next
        Return result
    End Function

    Public Overrides Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
        If _columnIndex >= 0 Then
            Return New List(Of CellEdit)({New CellEdit(indexOfRow, _columnIndex, _columnData(indexOfRow))})
        Else
            Return New List(Of CellEdit)
        End If
    End Function
    Public Overrides Function ContainsRow(indexOfRow As Integer) As Boolean
        Return True
    End Function

    Public Overrides Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional ByVal dataOfColumn() As T = Nothing) ', columnData() As Object)
        If _columnIndex >= indexOfColumn Then _columnIndex += 1

        If _columnIndex < 0 Then
            If _columnIndex * -1 > indexOfColumn Then
                _columnIndex += 1
            ElseIf _columnIndex * -1 = indexOfColumn Then
                _columnIndex *= -1
            End If
        End If
    End Sub

    Public Overrides Sub ColumnDeleted(indexOfColumn As Integer)
        If _columnIndex < 0 Then If _columnIndex * -1 >= indexOfColumn Then _columnIndex -= 1

        If _columnIndex = indexOfColumn Then _columnIndex *= -1
        If _columnIndex > indexOfColumn Then _columnIndex -= 1
    End Sub

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional ByVal rowData() As Object = Nothing)
        If IsNothing(rowData) Then
            _columnData.Insert(indexOfRow, _removedValues.Last)
            _removedValues.RemoveAt(_removedValues.Count - 1)
        Else
            _columnData.Insert(indexOfRow, rowData(_columnIndex))
        End If
    End Sub

    Public Overrides Sub RowDeleted(indexOfRow As Integer)
        _removedValues.Add(_columnData(indexOfRow))
        _columnData.RemoveAt(indexOfRow)
    End Sub

    Public Overrides Function ContainsCell(indexOfColumn As Integer, indexOfRow As Integer, ByRef returnValue As Object) As Boolean
        If _columnIndex <> indexOfColumn Then Return False
        returnValue = _columnData(indexOfRow)
        Return True
    End Function

    Public Overrides Function ContainsColumn(indexOfColumn As Integer) As Boolean
        Return _columnIndex = indexOfColumn
    End Function
End Class
