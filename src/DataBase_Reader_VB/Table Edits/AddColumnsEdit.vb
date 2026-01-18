Public Class AddColumnsEdit
    Inherits TableEdit

    Private ReadOnly _addColumnEdits() As IColumnEdit

    Public ReadOnly Property AddColumnEdits() As IColumnEdit()
        Get
            Return _addColumnEdits
        End Get
    End Property

    Public Sub New(columnsToAdd() As IColumnEdit)
        _addColumnEdits = columnsToAdd
    End Sub

    Public Overrides Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional columnData() As T = Nothing)
        For Each columnAdded As TableEdit In _addColumnEdits
            columnAdded.ColumnAdded(indexOfColumn, columnData)
        Next
    End Sub

    Public Overrides Sub ColumnDeleted(indexOfColumn As Integer)
        For Each columnAdded As TableEdit In _addColumnEdits
            columnAdded.ColumnDeleted(indexOfColumn)
        Next
    End Sub

    Public Overrides Function ContainsCell(indexOfColumn As Integer, indexOfRow As Integer, ByRef returnValue As Object) As Boolean
        For Each columnAdded As TableEdit In _addColumnEdits
            If columnAdded.ContainsCell(indexOfColumn, indexOfRow, returnValue) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function ContainsColumn(indexOfColumn As Integer) As Boolean
        For Each columnAdded As TableEdit In _addColumnEdits
            If columnAdded.ContainsColumn(indexOfColumn) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function ContainsRow(indexOfRow As Integer) As Boolean
        For Each columnAdded As TableEdit In _addColumnEdits
            If columnAdded.ContainsRow(indexOfRow) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        For Each columnAdded As TableEdit In _addColumnEdits
            result.AddRange(columnAdded.GetEditedCellsInColumn(indexOfColumn))
        Next
        Return result
    End Function

    Public Overrides Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        For Each columnAdded As TableEdit In _addColumnEdits
            result.AddRange(columnAdded.GetEditedCellsInRow(indexOfRow))
        Next
        Return result
    End Function

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional rowData() As Object = Nothing)
        For Each columnAdded As TableEdit In _addColumnEdits
            columnAdded.RowAdded(indexOfRow, rowData)
        Next
    End Sub

    Public Overrides Sub RowDeleted(indexOfRow As Integer)
        For Each columnAdded As TableEdit In _addColumnEdits
            columnAdded.RowDeleted(indexOfRow)
        Next
    End Sub
End Class
