
Public Class AddRowEdit
    Inherits TableEdit
    Private ReadOnly _originalRowEdit() As Object
    Private ReadOnly _rowData As List(Of Object)
    Private _rowIndex As Int32
    Public ReadOnly Property RowIndex() As Int32
        Get
            Return _rowIndex
        End Get
    End Property
    Public ReadOnly Property RowData() As List(Of Object)
        Get
            Return _rowData
        End Get
    End Property
    Public ReadOnly Property OriginalRowEdit() As Object()
        Get
            Return _originalRowEdit
        End Get
    End Property
    Public Sub New(newRowData() As Object, indexOfRow As Int32)
        _originalRowEdit = newRowData
        _rowData = newRowData.ToList()
        _rowIndex = indexOfRow
    End Sub

    Public Overrides Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional ByVal columnData() As T = Nothing)
        _rowData.Insert(indexOfColumn, 0)
    End Sub

    Public Overrides Sub ColumnDeleted(indexOfColumn As Integer)
        _rowData.RemoveAt(indexOfColumn)
    End Sub

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional ByVal dataOfRow() As Object = Nothing)
        If _rowIndex >= indexOfRow Then _rowIndex += 1
        If _rowIndex < 0 Then
            If _rowIndex * -1 > indexOfRow Then
                _rowIndex += 1
            ElseIf _rowIndex * -1 = indexOfRow Then
                _rowIndex *= -1
            End If
        End If
    End Sub

    Public Overrides Sub RowDeleted(indexOfRow As Integer)
        If _rowIndex < 0 Then
            If _rowIndex * -1 >= indexOfRow Then _rowIndex -= 1
        End If
        If _rowIndex = indexOfRow Then _rowIndex *= -1
        If _rowIndex > indexOfRow Then _rowIndex -= 1
    End Sub

    Public Overrides Function ContainsCell(indexOfColumn As Integer, indexOfRow As Integer, ByRef returnValue As Object) As Boolean
        If _rowIndex <> indexOfRow Then Return False
        If indexOfColumn < 0 Then Return False
        If indexOfColumn >= _rowData.Count Then Return False
        returnValue = _rowData(indexOfColumn)
        Return True
    End Function

    Public Overrides Function ContainsColumn(indexOfColumn As Integer) As Boolean
        If indexOfColumn < 0 Then Return False
        If indexOfColumn >= _rowData.Count Then Return False
        Return True
    End Function

    Public Overrides Function ContainsRow(indexOfRow As Integer) As Boolean
        Return _rowIndex = indexOfRow
    End Function

    Public Overrides Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
        If _rowIndex < 0 Then Return New List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        If indexOfColumn < 0 Then Return result
        If indexOfColumn >= _rowData.Count Then Return result
        result.Add(New CellEdit(_rowIndex, indexOfColumn, _rowData(indexOfColumn)))
        Return result
    End Function

    Public Overrides Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        If _rowIndex <> indexOfRow Then Return result
        For i As Int32 = 0 To _rowData.Count - 1
            result.Add(New CellEdit(_rowIndex, i, _rowData(i)))
        Next
        Return result
    End Function
End Class
