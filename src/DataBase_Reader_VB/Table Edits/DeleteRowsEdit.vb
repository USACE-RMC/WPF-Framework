Public Class DeleteRowsEdit
    Inherits TableEdit

    Private ReadOnly _deleteRowEdits() As DeleteRowEdit
    Public ReadOnly Property DeleteRowEdits() As DeleteRowEdit()
        Get
            Return _deleteRowEdits
        End Get
    End Property
    Public Sub New(rowDeleteEdits() As DeleteRowEdit)
        _deleteRowEdits = rowDeleteEdits
    End Sub

    Public Function GetDeletedRowIndices() As Int32()
        Dim rowsDeleted(_deleteRowEdits.Count - 1) As Int32
        For i As Int32 = 0 To _deleteRowEdits.Count - 1
            rowsDeleted(i) = _deleteRowEdits(i).RowIndex
        Next
        Return rowsDeleted
    End Function

    Public Overrides Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional columnData() As T = Nothing)
        For Each rowDeleteEdit As DeleteRowEdit In _deleteRowEdits
            rowDeleteEdit.ColumnAdded(indexOfColumn, columnData)
        Next
    End Sub

    Public Overrides Sub ColumnDeleted(indexOfColumn As Integer)
        For Each rowDeleteEdit As DeleteRowEdit In _deleteRowEdits
            rowDeleteEdit.ColumnDeleted(indexOfColumn)
        Next
    End Sub

    Public Overrides Function ContainsCell(indexOfColumn As Integer, indexOfRow As Integer, ByRef returnValue As Object) As Boolean
        For Each rowDeleteEdit As DeleteRowEdit In _deleteRowEdits
            If rowDeleteEdit.ContainsCell(indexOfColumn, indexOfRow, returnValue) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function ContainsColumn(indexOfColumn As Integer) As Boolean
        For Each rowDeleteEdit As DeleteRowEdit In _deleteRowEdits
            If rowDeleteEdit.ContainsColumn(indexOfColumn) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function ContainsRow(indexOfRow As Integer) As Boolean
        For Each rowDeleteEdit As DeleteRowEdit In _deleteRowEdits
            If rowDeleteEdit.ContainsRow(indexOfRow) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        For Each rowDeleteEdit As DeleteRowEdit In _deleteRowEdits
            result.AddRange(rowDeleteEdit.GetEditedCellsInColumn(indexOfColumn))
        Next
        Return result
    End Function

    Public Overrides Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        For Each rowDeleteEdit As DeleteRowEdit In _deleteRowEdits
            result.AddRange(rowDeleteEdit.GetEditedCellsInRow(indexOfRow))
        Next
        Return result
    End Function

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional rowData() As Object = Nothing)
        For Each rowDeleteEdit As DeleteRowEdit In _deleteRowEdits
            rowDeleteEdit.RowAdded(indexOfRow, rowData)
        Next
    End Sub

    Public Overrides Sub RowDeleted(indexOfRow As Integer)
        For Each rowDeleteEdit As DeleteRowEdit In _deleteRowEdits
            rowDeleteEdit.RowDeleted(indexOfRow)
        Next
    End Sub
End Class
