Public Class CellEdit
    Inherits TableEdit
    Private _rowIndex As Int32
    Private _columnIndex As Int32
    Private ReadOnly _editValue As Object
    Public ReadOnly Property RowIndex As Int32
        Get
            Return _rowIndex
        End Get
    End Property
    Public ReadOnly Property ColumnIndex As Int32
        Get
            Return _columnIndex
        End Get
    End Property
    Public ReadOnly Property Value As Object
        Get
            Return _editValue
        End Get
    End Property
    Public Sub New(indexOfRow As Int32, indexOfColumn As Int32, editValue As Object)
        _rowIndex = indexOfRow
        _columnIndex = indexOfColumn
        _editValue = editValue
    End Sub

    Public Overrides Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
        If indexOfColumn = _columnIndex Then Return New List(Of CellEdit)({Me})
        Return New List(Of CellEdit)
    End Function

    Public Overrides Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
        If indexOfRow = _rowIndex Then Return New List(Of CellEdit)({Me})
        Return New List(Of CellEdit)
    End Function

    Public Overrides Function ContainsRow(indexOfRow As Integer) As Boolean
        Return _rowIndex = indexOfRow
    End Function

    Public Overrides Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional ByVal columnData() As T = Nothing)
        If _columnIndex >= indexOfColumn Then _columnIndex += 1
        '
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
        '
        If _columnIndex = indexOfColumn Then _columnIndex *= -1
        If _columnIndex > indexOfColumn Then _columnIndex -= 1
    End Sub

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional ByVal rowData() As Object = Nothing) 
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
        If _rowIndex < 0 Then If _rowIndex * -1 >= indexOfRow Then _rowIndex -= 1

        If _rowIndex = indexOfRow Then _rowIndex *= -1
        If _rowIndex > indexOfRow Then _rowIndex -= 1
    End Sub

    Public Overrides Function ContainsCell(indexOfColumn As Integer, indexOfRow As Integer, ByRef returnValue As Object) As Boolean
        If indexOfColumn <> _columnIndex Then Return False
        If indexOfRow <> _rowIndex Then Return False
        returnValue = _editValue
        Return True
    End Function

    Public Overrides Function ContainsColumn(indexOfColumn As Integer) As Boolean
        Return indexOfColumn = _columnIndex
    End Function
End Class
