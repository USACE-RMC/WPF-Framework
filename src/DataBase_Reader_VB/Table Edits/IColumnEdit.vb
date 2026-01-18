Public Interface IColumnEdit
    ReadOnly Property ColumnIndex() As Int32
    ReadOnly Property ColumnName() As String
    ReadOnly Property ColumnDataType() As Type
    ReadOnly Property IsColumnAdd() As Boolean
End Interface
