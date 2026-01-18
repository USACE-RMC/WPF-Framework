Public Class AddRowsEdit
    Inherits TableEdit

    Private ReadOnly _addRowEdits() As AddRowEdit
    Public ReadOnly Property AddRowEdits() As AddRowEdit()
        Get
            Return _addRowEdits
        End Get
    End Property
    Public Sub New(rowEditsToAdd() As AddRowEdit)
        _addRowEdits = rowEditsToAdd
    End Sub
    Public Function GetOriginalRowEdits() As List(Of Object())
        Dim edits As New List(Of Object())(_addRowEdits.Count)
        For Each rowEdit As AddRowEdit In _addRowEdits
            edits.Add(rowEdit.OriginalRowEdit)
        Next
        '
        Return edits
    End Function
    Public Overrides Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional columnData() As T = Nothing)
        For Each rowEditToAdd As AddRowEdit In _addRowEdits
            rowEditToAdd.ColumnAdded(indexOfColumn, columnData)
        Next
    End Sub

    Public Overrides Sub ColumnDeleted(indexOfColumn As Integer)
        For Each rowEditToAdd As AddRowEdit In _addRowEdits
            rowEditToAdd.ColumnDeleted(indexOfColumn)
        Next
    End Sub

    Public Overrides Function ContainsCell(indexOfColumn As Integer, indexOfRow As Integer, ByRef returnValue As Object) As Boolean
        For Each rowEditToAdd As AddRowEdit In _addRowEdits
            If rowEditToAdd.ContainsCell(indexOfColumn, indexOfRow, returnValue) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function ContainsColumn(indexOfColumn As Integer) As Boolean
        For Each rowEditToAdd As AddRowEdit In _addRowEdits
            If rowEditToAdd.ContainsColumn(indexOfColumn) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function ContainsRow(indexOfRow As Integer) As Boolean
        For Each rowEditToAdd As AddRowEdit In _addRowEdits
            If rowEditToAdd.ContainsRow(indexOfRow) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        For Each rowEditToAdd As AddRowEdit In _addRowEdits
            result.AddRange(rowEditToAdd.GetEditedCellsInColumn(indexOfColumn))
        Next
        Return result
    End Function

    Public Overrides Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        For Each rowEditToAdd As AddRowEdit In _addRowEdits
            result.AddRange(rowEditToAdd.GetEditedCellsInRow(indexOfRow))
        Next
        Return result
    End Function

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional rowData() As Object = Nothing)
        For Each rowEditToAdd As AddRowEdit In _addRowEdits
            rowEditToAdd.RowAdded(indexOfRow, rowData)
        Next
    End Sub

    Public Overrides Sub RowDeleted(indexOfRow As Integer)
        For Each rowEditToAdd As AddRowEdit In _addRowEdits
            rowEditToAdd.RowDeleted(indexOfRow)
        Next
    End Sub
End Class
