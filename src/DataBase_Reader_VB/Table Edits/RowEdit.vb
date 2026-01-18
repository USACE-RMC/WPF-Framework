
Public Class RowEdit
    Inherits TableEdit

    Private ReadOnly _rowData As List(Of Object)
    Private ReadOnly _removedValues As New List(Of Object)
    Private _rowIndex As Int32
    Public ReadOnly Property RowIndex As Int32
        Get
            Return _rowIndex
        End Get
    End Property
    'Private _isValidEdit As Boolean
    Public Sub New(newRowData() As Object, indexOfRow As Int32)
        _rowData = newRowData.ToList()
        _rowIndex = indexOfRow
    End Sub

    Public Overrides Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional ByVal columnData() As T = Nothing)
        If IsNothing(columnData) Then
            _rowData.Insert(indexOfColumn, _removedValues.Last)
            _removedValues.RemoveAt(_removedValues.Count - 1)
        Else
            _rowData.Insert(indexOfColumn, columnData(_rowIndex))
        End If
    End Sub

    Public Overrides Sub ColumnDeleted(indexOfColumn As Integer)
        _removedValues.Add(_rowData(indexOfColumn))
        _rowData.RemoveAt(indexOfColumn)
    End Sub

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional ByVal rowData() As Object = Nothing)
        If _rowIndex > indexOfRow Then _rowIndex += 1
    End Sub

    Public Overrides Sub RowDeleted(indexOfRow As Integer)
        If _rowIndex > indexOfRow Then _rowIndex -= 1
    End Sub

    Public Overrides Function ContainsRow(indexOfRow As Integer) As Boolean
        Return _rowIndex = indexOfRow
    End Function

    Public Overrides Function ContainsCell(indexOfColumn As Integer, indexOfRow As Integer, ByRef returnvalue As Object) As Boolean
        If _rowIndex = indexOfRow Then
            returnvalue = _rowData(indexOfColumn)
            Return True
        Else
            Return False
        End If
    End Function

    Public Overrides Function ContainsColumn(indexOfColumn As Integer) As Boolean
        Return True
    End Function

    Public Overrides Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
        If _rowIndex < 0 Then Return New List(Of CellEdit)
        Return New List(Of CellEdit)({New CellEdit(_rowIndex, indexOfColumn, _rowData(indexOfColumn))})
    End Function

    Public Overrides Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        If indexOfRow <> _rowIndex Then Return result
        For i As Int32 = 0 To _rowData.Count - 1
            result.Add(New CellEdit(_rowIndex, i, _rowData(i)))
        Next
        Return result
    End Function
    Public Function GetRowData() As MultiCellEdit
        Dim cellEdits(_rowData.Count - 1) As CellEdit
        For i As Int32 = 0 To _rowData.Count - 1
            cellEdits(i) = New CellEdit(_rowIndex, i, _rowData(i))
        Next
        '
        Return New MultiCellEdit(cellEdits)
    End Function
End Class
