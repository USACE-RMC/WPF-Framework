Public Class DeleteColumnsEdit
    Inherits TableEdit

    Private ReadOnly _columnsDeleted() As DeleteColumnEdit
    Public ReadOnly Property ColumnsDeleted() As DeleteColumnEdit()
        Get
            Return _columnsDeleted
        End Get
    End Property
    Public Sub New(deleteColumnEdits() As DeleteColumnEdit)
        _columnsDeleted = deleteColumnEdits
    End Sub

    Public Overrides Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional columnData() As T = Nothing)
        For Each columnDeleteEdit As DeleteColumnEdit In _columnsDeleted
            columnDeleteEdit.ColumnAdded(indexOfColumn, columnData)
        Next
    End Sub

    Public Overrides Sub ColumnDeleted(indexOfColumn As Integer)
        For Each columnDeleteEdit As DeleteColumnEdit In _columnsDeleted
            columnDeleteEdit.ColumnDeleted(indexOfColumn)
        Next
    End Sub

    Public Overrides Function ContainsCell(indexOfColumn As Integer, indexOfRow As Integer, ByRef returnValue As Object) As Boolean
        For Each columnDeleteEdit As DeleteColumnEdit In _columnsDeleted
            If columnDeleteEdit.ContainsCell(indexOfColumn, indexOfRow, returnValue) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function ContainsColumn(indexOfColumn As Integer) As Boolean
        For Each columnDeleteEdit As DeleteColumnEdit In _columnsDeleted
            If columnDeleteEdit.ContainsColumn(indexOfColumn) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function ContainsRow(indexOfRow As Integer) As Boolean
        For Each columnDeleteEdit As DeleteColumnEdit In _columnsDeleted
            If columnDeleteEdit.ContainsRow(indexOfRow) Then Return True
        Next
        Return False
    End Function

    Public Overrides Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        For Each columnDeleteEdit As DeleteColumnEdit In _columnsDeleted
            result.AddRange(columnDeleteEdit.GetEditedCellsInColumn(indexOfColumn))
        Next
        Return result
    End Function

    Public Overrides Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        For Each columnDeleteEdit As DeleteColumnEdit In _columnsDeleted
            result.AddRange(columnDeleteEdit.GetEditedCellsInRow(indexOfRow))
        Next
        Return result
    End Function

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional rowData() As Object = Nothing)
        For Each columnDeleteEdit As DeleteColumnEdit In _columnsDeleted
            columnDeleteEdit.RowAdded(indexOfRow, rowData)
        Next
    End Sub

    Public Overrides Sub RowDeleted(indexOfRow As Integer)
        For Each columnDeleteEdit As DeleteColumnEdit In _columnsDeleted
            columnDeleteEdit.RowDeleted(indexOfRow)
        Next
    End Sub
End Class
