Public Class StringListPropertyControl
    Public Shared StringListProperty As DependencyProperty = DependencyProperty.Register(NameOf(StringList), GetType(IList(Of String)), GetType(StringListPropertyControl), New PropertyMetadata(New List(Of String), AddressOf StringListPropertyChanged_Callback))
    Public Property StringList As IList(Of String)
        Get
            Return DirectCast(GetValue(StringListProperty), IList(Of String))
        End Get
        Set(value As IList(Of String))
            SetValue(StringListProperty, value)
        End Set
    End Property
    Private _internalList As IList(Of Object)
    '
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(StringListPropertyControl), New UIPropertyMetadata("Title"))
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property
    Private Shared Sub StringListPropertyChanged_Callback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(StringListPropertyControl) Then Exit Sub
        Dim thisControl = DirectCast(d, StringListPropertyControl)
        '
        thisControl.StringListDataGrid.ItemsSource = Nothing
        If IsNothing(e.NewValue) Then thisControl.StringList = New List(Of String) 'Exit Sub
        thisControl._internalList = New List(Of Object)(thisControl.StringList.Count)
        For i As Int32 = 0 To thisControl.StringList.Count - 1
            thisControl._internalList.Add(New StringContainer(thisControl.StringList(i)))
        Next
        thisControl.StringListDataGrid.ItemsSource = thisControl._internalList
    End Sub

    Public Shared AddRemoveEnabledProperty As DependencyProperty = DependencyProperty.Register(NameOf(AddRemoveEnabled), GetType(Boolean), GetType(StringListPropertyControl), New UIPropertyMetadata(True, AddressOf AddRemoveEnabledChanged_Callback))
    Public Property AddRemoveEnabled As Boolean
        Get
            Return DirectCast(GetValue(AddRemoveEnabledProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(AddRemoveEnabledProperty, value)
        End Set
    End Property

    Private Shared Sub AddRemoveEnabledChanged_Callback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(StringListPropertyControl) Then Exit Sub
        Dim thisControl = DirectCast(d, StringListPropertyControl)
        thisControl.StringListDataGrid.CanUserAddInsertDeleteRows = DirectCast(e.NewValue, Boolean)
    End Sub

    Private Sub TextBlock_MouseUp(sender As Object, e As MouseButtonEventArgs)
        StringListDataGrid.SelectedCells.Clear()
        For Each item In StringListDataGrid.Items
            StringListDataGrid.SelectedCells.Add(New DataGridCellInfo(item, StringColumn))
        Next
    End Sub
    Private Function GetStringList() As IList(Of String)
        Dim result As New List(Of String)
        For i As Int32 = 0 To StringListDataGrid.Items.Count - 1
            Dim c = StringListDataGrid.GetCell(i, 0)
            If IsNothing(c) Then
                result.Add("")
            ElseIf c.Content.GetType = GetType(TextBox) Then
                result.Add(DirectCast(c.Content, TextBox).Text)
            ElseIf c.Content.GetType = GetType(TextBlock) Then
                result.Add(DirectCast(c.Content, TextBlock).Text)
            Else
                result.Add(c.Content.ToString)
            End If
        Next
        Return result
    End Function
    'Private Sub StringListDataGrid_SizeChanged(sender As Object, e As SizeChangedEventArgs)
    'StringColumn.Width = New DataGridLength(StringListDataGrid.ActualWidth - 4, DataGridLengthUnitType.Star, StringListDataGrid.ActualWidth, StringListDataGrid.ActualWidth - 2)
    'StringListDataGrid.UpdateLayout()
    'StringColumn.Width = New DataGridLength(StringListDataGrid.ActualWidth - 3)

    ' End Sub

    Private Sub StringListDataGrid_CellEditEnding(sender As Object, e As DataGridCellEditEndingEventArgs)
        StringList = GetStringList()
    End Sub

    Private Sub StringListDataGrid_RowsAdded(startrow As Integer, numrows As Integer)
        StringList = GetStringList()
    End Sub

    Private Sub StringListDataGrid_DataPasted()
        StringList = GetStringList()
    End Sub


    'Private Sub Border_SizeChanged(sender As Object, e As SizeChangedEventArgs)
    'StringColumn.Width = New DataGridLength(StringListDataGrid.ActualWidth - 3)
    'StringListDataGrid.Width = 0
    'UpdateLayout()
    'StringListDataGrid.Width = border.ActualWidth - 5
    'Debug.Print(border.ActualWidth.ToString)
    'End Sub
    Private Class StringContainer
        Public Property TheString As String
        Public Sub New()
            TheString = ""
        End Sub
        Public Sub New(newString As String)
            TheString = newString
        End Sub
    End Class
End Class
