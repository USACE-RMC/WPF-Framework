Public MustInherit Class TableEdit
    Public MustOverride Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
    Public MustOverride Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
    ' ''' <summary>
    ' ''' Checks if the cell in question is in this edit
    ' ''' </summary>
    ' ''' <param name="cell">a row column pair</param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    Public MustOverride Function ContainsCell(indexOfColumn As Int32, indexOfRow As Int32, ByRef returnValue As Object) As Boolean
    Public MustOverride Function ContainsColumn(indexOfColumn As Integer) As Boolean
    Public MustOverride Function ContainsRow(indexOfRow As Integer) As Boolean
    Public MustOverride Sub ColumnDeleted(indexOfColumn As Integer)
    Public MustOverride Sub ColumnAdded(Of T)(indexOfColumn As Integer, Optional ByVal columnData() As T = Nothing)
    Public MustOverride Sub RowDeleted(indexOfRow As Int32)
    Public MustOverride Sub RowAdded(indexOfRow As Int32, Optional ByVal rowData() As Object = Nothing)
End Class
