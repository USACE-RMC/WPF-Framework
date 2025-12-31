Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.ComponentModel

Public Class DataGridToolbar

#Region "Construction"

    ''' <summary>
    ''' Construct new data grid toolbar.
    ''' </summary>
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Dim defaultButtonStyle = DefaultStackPanelButtonStyle()
        AddRowsButton.Style = defaultButtonStyle
        InsertRowsButton.Style = defaultButtonStyle
        DeleteRowsButton.Style = defaultButtonStyle
        SelectAllButton.Style = defaultButtonStyle
        CopyButton.Style = defaultButtonStyle
        CopyWithHeadersButton.Style = defaultButtonStyle
        PasteButton.Style = defaultButtonStyle
        '
        Dim defaultSeparatorStyle = DefaultStackPanelSeparatorStyle()
        EditSelectSeparator.Style = defaultSeparatorStyle
        CustomOptionsSeparator.Style = defaultSeparatorStyle
        '
        InsertRowsButton.IsEnabled = False
        DeleteRowsButton.IsEnabled = False
        CopyButton.IsEnabled = False
        CopyWithHeadersButton.IsEnabled = False
        PasteButton.IsEnabled = False
        '
        AddHandler _customButtons.CollectionChanged, Sub(sender As Object, e As NotifyCollectionChangedEventArgs)
                                                         If Not e.OldItems Is Nothing Then
                                                             For Each item As Button In e.OldItems
                                                                 ToolbarStackPanel.Children.Remove(item)
                                                             Next
                                                         End If
                                                         If Not e.NewItems Is Nothing Then
                                                             For Each item As Button In e.NewItems
                                                                 item.Style = DefaultStackPanelButtonStyle()
                                                                 ToolbarStackPanel.Children.Insert(3 + _customButtons.Count, item)
                                                             Next
                                                         End If
                                                         '
                                                         If _customButtons.Count = 0 Then
                                                             CustomOptionsSeparator.Visibility = Visibility.Collapsed
                                                         Else
                                                             CustomOptionsSeparator.Visibility = Visibility.Visible
                                                         End If
                                                     End Sub
    End Sub



#End Region

#Region "Members"

    ''' <summary>
    ''' Dependency property for the data grid. 
    ''' </summary>
    Public Shared DataGridProperty As DependencyProperty = DependencyProperty.Register(NameOf(DataGrid), GetType(CopyPasteDataGrid),
                                                                                         GetType(DataGridToolbar), New FrameworkPropertyMetadata(Nothing, AddressOf DataGridChangedCallback))

    ''' <summary>
    ''' Data Grid property callback. 
    ''' </summary>
    Private Shared Sub DataGridChangedCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(DataGridToolbar) Then Exit Sub
        Dim thisControl = DirectCast(d, DataGridToolbar)
        '
        If e.OldValue IsNot Nothing Then
            Dim oldGrid = TryCast(e.OldValue, CopyPasteDataGrid)
            If oldGrid IsNot Nothing Then RemoveHandler oldGrid.PropertyChanged, AddressOf thisControl.DataGridPropertyChanged
        End If

        If IsNothing(e.NewValue) Then Exit Sub
        Dim newGrid = TryCast(e.NewValue, CopyPasteDataGrid)
        If IsNothing(newGrid) Then Exit Sub
        'Can probably set the visibility of these buttons by binding to the datagrid property and using the booleantovisibility converter. Need to check.
        AddHandler newGrid.PropertyChanged, AddressOf thisControl.DataGridPropertyChanged

        If newGrid.CanUserAddInsertDeleteRows Then
            thisControl.AddRowsButton.Visibility = Visibility.Visible
            thisControl.InsertRowsButton.Visibility = Visibility.Visible
            thisControl.DeleteRowsButton.Visibility = Visibility.Visible
        Else
            thisControl.AddRowsButton.Visibility = Visibility.Collapsed
            thisControl.InsertRowsButton.Visibility = Visibility.Collapsed
            thisControl.DeleteRowsButton.Visibility = Visibility.Collapsed
        End If
        '
        thisControl.PasteButton.IsEnabled = Not newGrid.IsReadOnly

        AddHandler newGrid.SelectedCellsChanged, Sub(sender As Object, et As SelectedCellsChangedEventArgs)
                                                     If newGrid.SelectedCells.Count <= 0 Then
                                                         thisControl.InsertRowsButton.IsEnabled = False
                                                         thisControl.DeleteRowsButton.IsEnabled = False
                                                         thisControl.CopyButton.IsEnabled = False
                                                         thisControl.CopyWithHeadersButton.IsEnabled = False
                                                         thisControl.PasteButton.IsEnabled = False
                                                     Else
                                                         thisControl.InsertRowsButton.IsEnabled = True
                                                         thisControl.DeleteRowsButton.IsEnabled = True
                                                         thisControl.CopyButton.IsEnabled = True
                                                         thisControl.CopyWithHeadersButton.IsEnabled = True
                                                         '
                                                         If newGrid.IsReadOnly = True Then Exit Sub
                                                         ' If there is nothing on the clipboard, then disable the paste button.
                                                         Try
                                                             'Dim clipboardData As String()() = DirectCast(Clipboard.GetText(), String).Split(ControlChars.Lf).[Select](Function(row) row.Split(ControlChars.Tab).[Select](Function(Clipboardcell) If(Clipboardcell.Length > 0 AndAlso Clipboardcell(Clipboardcell.Length - 1) = ControlChars.Cr, Clipboardcell.Substring(0, Clipboardcell.Length - 1), Clipboardcell)).ToArray()).Where(Function(a) a.Any(Function(b) b.Length > 0)).ToArray()
                                                             'If clipboardData.Length = 0 Then
                                                             If thisControl.IsClipboardEmpty() Then
                                                                 thisControl.PasteButton.IsEnabled = False
                                                             Else
                                                                 thisControl.PasteButton.IsEnabled = True
                                                             End If
                                                         Catch ex As Exception
                                                             thisControl.PasteButton.IsEnabled = False
                                                         End Try

                                                     End If
                                                 End Sub


    End Sub

    Public Declare Function CountClipboardFormats Lib "user32" () As Long

    Function IsClipboardEmpty() As Boolean
        Return CountClipboardFormats() = 0
    End Function

    Private Sub DataGridPropertyChanged(sender As Object, e As PropertyChangedEventArgs)
        If e.PropertyName = NameOf(CopyPasteDataGrid.CanUserAddInsertDeleteRows) Then
            If DataGrid.CanUserAddInsertDeleteRows Then
                AddRowsButton.Visibility = Visibility.Visible
                InsertRowsButton.Visibility = Visibility.Visible
                DeleteRowsButton.Visibility = Visibility.Visible
            Else
                AddRowsButton.Visibility = Visibility.Collapsed
                InsertRowsButton.Visibility = Visibility.Collapsed
                DeleteRowsButton.Visibility = Visibility.Collapsed
            End If
        End If
    End Sub

    ''' <summary>
    ''' Get and set the data grid.
    ''' </summary>
    <Category("Miscellaneous"), Description("Get and set the data grid for the control.")>
    <Browsable(True)>
    Public Property DataGrid As CopyPasteDataGrid
        Get
            Return CType(GetValue(DataGridProperty), CopyPasteDataGrid)
        End Get
        Set(value As CopyPasteDataGrid)
            SetValue(DataGridProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the data grid. 
    ''' </summary>
    Public Shared ToolOrientationProperty As DependencyProperty = DependencyProperty.Register(NameOf(ToolOrientation), GetType(Orientation),
                                                                                         GetType(DataGridToolbar), New FrameworkPropertyMetadata(Orientation.Horizontal))

    ''' <summary>
    ''' Get and set the data grid.
    ''' </summary>
    <Category("Miscellaneous"), Description("Get and set the tool buttons orientation.")>
    <Browsable(True)>
    Public Property ToolOrientation As Orientation
        Get
            Return CType(GetValue(ToolOrientationProperty), Orientation)
        End Get
        Set(value As Orientation)
            SetValue(ToolOrientationProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the control background color. 
    ''' </summary>
    Public Shared BackgroundColorProperty As DependencyProperty = DependencyProperty.Register(NameOf(BackgroundColor), GetType(SolidColorBrush),
                                                                                              GetType(DataGridToolbar), New FrameworkPropertyMetadata(New SolidColorBrush(Colors.Transparent)))

    ''' <summary>
    ''' Get and set the control background color. 
    ''' </summary>
    <Category("Brush"), Description("Gets and sets the background color brush of the control.")>
    <Browsable(True)>
    Public Property BackgroundColor As SolidColorBrush
        Get
            Return CType(GetValue(BackgroundColorProperty), SolidColorBrush)
        End Get
        Set(value As SolidColorBrush)
            SetValue(BackgroundColorProperty, value)
        End Set
    End Property


    ''' <summary>
    ''' Dependency property for the stack panel button style.
    ''' </summary>
    Public Shared StackPanelButtonStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(StackPanelButtonStyle), GetType(Style),
                                                                                              GetType(DataGridToolbar), New FrameworkPropertyMetadata(DefaultStackPanelButtonStyle(), AddressOf StackPanelButtonStylePropertyCallback))

    Private Shared Sub StackPanelButtonStylePropertyCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        Dim thisControl = DirectCast(d, DataGridToolbar)
        '
        Dim newStyle = TryCast(e.NewValue, Style)

        If Not thisControl.AddRowsButton Is Nothing Then thisControl.AddRowsButton.Style = newStyle
        If Not thisControl.InsertRowsButton Is Nothing Then thisControl.InsertRowsButton.Style = newStyle
        If Not thisControl.DeleteRowsButton Is Nothing Then thisControl.DeleteRowsButton.Style = newStyle
        If Not thisControl.SelectAllButton Is Nothing Then thisControl.SelectAllButton.Style = newStyle
        If Not thisControl.CopyButton Is Nothing Then thisControl.CopyButton.Style = newStyle
        If Not thisControl.CopyWithHeadersButton Is Nothing Then thisControl.CopyWithHeadersButton.Style = newStyle
        If Not thisControl.PasteButton Is Nothing Then thisControl.PasteButton.Style = newStyle
    End Sub


    Private Shared Function DefaultStackPanelButtonStyle() As Style
        Dim s = New Style(GetType(Button), CType(Application.Current.FindResource(ToolBar.ButtonStyleKey), Style)) 'CType(Application.Current.FindResource(ToolBar.ButtonStyleKey), Style)
        '
        s.Setters.Add(New Setter(Button.HeightProperty, CDbl(24)))
        s.Setters.Add(New Setter(Button.WidthProperty, CDbl(24)))
        s.Setters.Add(New Setter(Button.CursorProperty, Cursors.Hand))
        Dim disabledShadeTrigger As New Trigger() With {.[Property] = Button.IsEnabledProperty, .Value = False}
        disabledShadeTrigger.Setters.Add(New Setter(Button.OpacityProperty, 0.5))

        s.Triggers.Add(disabledShadeTrigger)

        Return s
    End Function

    ''' <summary>
    ''' Gets and sets the stack panel button style. 
    ''' </summary>
    Public Property StackPanelButtonStyle As Style
        Get
            Return CType(GetValue(StackPanelButtonStyleProperty), Style)
        End Get
        Set(value As Style)
            SetValue(StackPanelButtonStyleProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the stack panel separator style.
    ''' </summary>
    Public Shared StackPanelSeperatorStyleProperty As DependencyProperty = DependencyProperty.Register(NameOf(StackPanelSeperatorStyle), GetType(Style),
                                                                                              GetType(DataGridToolbar), New FrameworkPropertyMetadata(DefaultStackPanelSeparatorStyle(), AddressOf StackPanelSeperatorStylePropertyCallback))
    Private Shared Sub StackPanelSeperatorStylePropertyCallback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        Dim thisControl = DirectCast(d, DataGridToolbar)
        '
        Dim newStyle = TryCast(e.NewValue, Style)
        '
        If Not thisControl.EditSelectSeparator Is Nothing Then thisControl.EditSelectSeparator.Style = newStyle
        If Not thisControl.CustomOptionsSeparator Is Nothing Then thisControl.CustomOptionsSeparator.Style = newStyle
    End Sub

    Private Shared Function DefaultStackPanelSeparatorStyle() As Style
        Dim s As New Style(GetType(Separator))
        s.Setters.Add(New Setter(Separator.MarginProperty, New Thickness(2, 1, 2, 1)))
        ' Vertical text
        Dim verticalTransform As New RotateTransform(90)
        s.Setters.Add(New Setter(Separator.LayoutTransformProperty, verticalTransform))

        Return s
    End Function

    ''' <summary>
    ''' Gets and sets the stack panel separator style. 
    ''' </summary>
    Public Property StackPanelSeperatorStyle As Style
        Get
            Return CType(GetValue(StackPanelSeperatorStyleProperty), Style)
        End Get
        Set(value As Style)
            SetValue(StackPanelSeperatorStyleProperty, value)
        End Set
    End Property

    Private _customButtons As New ObservableCollection(Of Button)
    Public Property CustomButtons As ObservableCollection(Of Button)
        Get
            Return _customButtons
        End Get
        Private Set(value As ObservableCollection(Of Button))
            'do nothing
        End Set
    End Property
#End Region

#Region "Methods"

    ''' <summary>
    ''' On click, add rows. 
    ''' </summary>
    Private Sub AddRowsButton_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataGrid) Then Exit Sub
        If DataGrid.CanUserAddInsertDeleteRows Then
            Dim uniqueRows = DataGrid.GetRowsWithSelectedCells()
            DataGrid.AddRows(Math.Max(uniqueRows.Count, 1))
            DataGrid.Focus()
        End If
    End Sub

    ''' <summary>
    ''' On click, insert rows. 
    ''' </summary>
    Private Sub InsertRowsButton_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataGrid) Then Exit Sub
        If DataGrid.CanUserAddInsertDeleteRows Then DataGrid.InsertRows()
        DataGrid.Focus()
    End Sub

    ''' <summary>
    ''' On click, delete rows.
    ''' </summary>
    Private Sub DeleteRowsButton_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataGrid) Then Exit Sub
        If DataGrid.CanUserAddInsertDeleteRows Then DataGrid.DeleteRows()
        DataGrid.Focus()
    End Sub


    ''' <summary>
    ''' On click, select all.
    ''' </summary>
    Private Sub SelectAllButton_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataGrid) Then Exit Sub
        DataGrid.SelectAllCells()
        DataGrid.Focus()
    End Sub

    ''' <summary>
    ''' On click, copy selection.
    ''' </summary>
    Private Sub CopyButton_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataGrid) Then Exit Sub
        ApplicationCommands.Copy.Execute(Nothing, DataGrid)
        DataGrid.Focus()
    End Sub

    ''' <summary>
    ''' On click, copy selection with table headers.
    ''' </summary>
    Private Sub CopyWithHeadersButton_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataGrid) Then Exit Sub
        DataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader
        ApplicationCommands.Copy.Execute(Nothing, DataGrid)
        DataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader
        DataGrid.Focus()
    End Sub

    ''' <summary>
    ''' On click, paste.
    ''' </summary>
    Private Sub PasteButton_Click(sender As Object, e As RoutedEventArgs)
        If IsNothing(DataGrid) Then Exit Sub
        ' Only paste if there is data in the clipboard.
        Try
            Dim clipboardData As String()() = DirectCast(Clipboard.GetText(), String).Split(ControlChars.Lf).[Select](Function(row) row.Split(ControlChars.Tab).[Select](Function(Clipboardcell) If(Clipboardcell.Length > 0 AndAlso Clipboardcell(Clipboardcell.Length - 1) = ControlChars.Cr, Clipboardcell.Substring(0, Clipboardcell.Length - 1), Clipboardcell)).ToArray()).Where(Function(a) a.Any(Function(b) b.Length > 0)).ToArray()
            If clipboardData.Length > 0 Then DataGrid.PasteClipboard()
        Catch ex As Exception
        End Try
        DataGrid.Focus()
    End Sub

#End Region

End Class
