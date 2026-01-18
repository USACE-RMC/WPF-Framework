Public Class DeleteRowEdit
    Inherits TableEdit
    Private ReadOnly _rowIndex As Int32

    Public ReadOnly Property RowIndex As Int32
        Get
            Return _rowIndex
        End Get
    End Property
    Public Sub New(viewRowIndex As Int32)
        _rowIndex = viewRowIndex
    End Sub

    Public Overrides Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional ByVal columnData() As T = Nothing)
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Sub ColumnDeleted(indexOfColumn As Integer)
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional ByVal rowData() As Object = Nothing)
        'If _rowIndex > indexOfRow Then _rowIndex += 1
    End Sub

    Public Overrides Sub RowDeleted(indexOfRow As Integer)
        'If _rowIndex > indexOfRow Then _rowIndex -= 1
    End Sub

    Public Overrides Function ContainsCell(indexOfColumn As Integer, indexOfRow As Integer, ByRef returnValue As Object) As Boolean
        Return False
    End Function

    Public Overrides Function ContainsColumn(indexOfColumn As Integer) As Boolean
        Return False
    End Function

    Public Overrides Function ContainsRow(indexOfRow As Integer) As Boolean
        Return False
    End Function

    Public Overrides Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
        Return New List(Of CellEdit)
    End Function

    Public Overrides Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
        Return New List(Of CellEdit)
    End Function
End Class
