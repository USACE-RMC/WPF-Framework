
Public Class AddColumnEdit(Of T)
    Inherits TableEdit
    Implements IColumnEdit
    Private ReadOnly _originalColumnEdit() As T
    Private ReadOnly _columnData As List(Of T)
    Private ReadOnly _removedValues As New List(Of T)
    Private _columnIndex As Int32
    Private ReadOnly _columnName As String
    'Private ReadOnly _columnDataType As Type
    Public ReadOnly Property ColumnIndex() As Int32 Implements IColumnEdit.ColumnIndex
        Get
            Return _columnIndex
        End Get
    End Property
    Public ReadOnly Property ColumnName() As String Implements IColumnEdit.ColumnName
        Get
            Return _columnName
        End Get
    End Property
    Public ReadOnly Property ColumnDataType() As Type Implements IColumnEdit.ColumnDataType
        Get
            Return GetType(T) '_columnDataType
        End Get
    End Property
    Public ReadOnly Property ColumnData() As List(Of T)
        Get
            Return _columnData
        End Get
    End Property
    Public ReadOnly Property OriginalColumnEdit() As T()
        Get
            Return _originalColumnEdit
        End Get
    End Property
    Public ReadOnly Property IsColumnAdd As Boolean Implements IColumnEdit.IsColumnAdd
        Get
            Return True
        End Get
    End Property

    Public Sub New(data() As T, indexOfColumn As Int32, nameOfColumn As String) ', dataTypeOfColumn As Type)
        _columnIndex = indexOfColumn
        _originalColumnEdit = data
        _columnData = data.ToList()
        _columnName = nameOfColumn
        '_columnDataType = dataTypeOfColumn
    End Sub

    Public Overrides Sub ColumnAdded(Of U)(indexOfColumn As Integer, Optional ByVal dataOfColumn() As U = Nothing) ', columnData() As Object)
        If _columnIndex >= indexOfColumn Then
            _columnIndex += 1
        End If
        If _columnIndex < 0 Then
            If _columnIndex * -1 > indexOfColumn Then
                _columnIndex += 1
            ElseIf _columnIndex * -1 = indexOfColumn Then
                _columnIndex *= -1
            End If
        End If
    End Sub

    Public Overrides Sub ColumnDeleted(indexOfColumn As Integer)
        If _columnIndex < 0 Then
            If _columnIndex * -1 >= indexOfColumn Then _columnIndex -= 1
        End If
        If _columnIndex = indexOfColumn Then
            _columnIndex *= -1
        End If
        If _columnIndex > indexOfColumn Then
            _columnIndex -= 1
        End If
    End Sub

    Public Overrides Sub RowAdded(indexOfRow As Integer, Optional ByVal rowData() As Object = Nothing)
        If IsNothing(rowData) Then
            _columnData.Insert(indexOfRow, _removedValues.Last)
            _removedValues.RemoveAt(_removedValues.Count - 1)
        Else
            _columnData.Insert(indexOfRow, rowData(_columnIndex))
        End If
    End Sub

    Public Overrides Sub RowDeleted(indexOfRow As Integer)
        _removedValues.Add(_columnData(indexOfRow))
        _columnData.RemoveAt(indexOfRow)
    End Sub

    Public Overrides Function ContainsRow(indexOfRow As Integer) As Boolean
        Return True
    End Function

    Public Overrides Function GetEditedCellsInColumn(indexOfColumn As Integer) As List(Of CellEdit)
        If indexOfColumn <> _columnIndex Then Return New List(Of CellEdit)
        Dim result As New List(Of CellEdit)
        For i As Int32 = 0 To _columnData.Count - 1
            result.Add(New CellEdit(i, _columnIndex, _columnData(i)))
        Next
        Return result
    End Function

    Public Overrides Function GetEditedCellsInRow(indexOfRow As Integer) As List(Of CellEdit)
        If _columnIndex < 0 Then Return New List(Of CellEdit)
        Return New List(Of CellEdit)({New CellEdit(indexOfRow, _columnIndex, _columnData(indexOfRow))})
        '
    End Function

    Public Overrides Function ContainsCell(indexOfColumn As Integer, indexOfRow As Integer, ByRef returnValue As Object) As Boolean
        If _columnIndex <> indexOfColumn Then Return False
        returnValue = _columnData(indexOfRow)
        Return True
    End Function

    Public Overrides Function ContainsColumn(indexOfColumn As Integer) As Boolean
        Return _columnIndex = indexOfColumn
    End Function
End Class
