
Public Class DeleteColumnEdit
    Inherits TableEdit
    Private ReadOnly _columnIndex As Int32

    Public ReadOnly Property ColumnIndex As Int32
        Get
            Return _columnIndex
        End Get
    End Property
    Public Sub New(indexOfColumn As Int32)
        _columnIndex = indexOfColumn
    End Sub

    Public Overrides Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional ByVal columnData() As T = Nothing)
        'If _columnIndex >= indexOfColumn Then _columnIndex += 1
    End Sub

    Public Overrides Sub ColumnDeleted(indexOfColumn As Integer)
        'If _columnIndex > indexOfColumn Then _columnIndex -= 1
    End Sub

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional ByVal rowData() As Object = Nothing) 
        'Throw New NotImplementedException()
    End Sub

    Public Overrides Sub RowDeleted(indexOfRow As Integer)
        'Throw New NotImplementedException()
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
