'Public Class FCTreeItem
'    Inherits TreeViewItem
'    Private _ParseTreeNode As FieldCalculationParser.IDisplayToTreeNode
'    Public Event TextToAdd(ByVal TextToInsert As String)
'    Sub New(ByVal ParsetreeNode As FieldCalculationParser.IDisplayToTreeNode)
'        ' This call is required by the designer.
'        'InitializeComponent()

'        ' Add any initialization after the InitializeComponent() call.
'        _ParseTreeNode = ParsetreeNode
'        MyBase.Header = _ParseTreeNode.DisplayName
'        Dim t As New Windows.Controls.ToolTip
'        t.Content = _ParseTreeNode.FunctionSyntax
'        MyBase.ToolTip = t
'        Dim c As New ContextMenu
'        Dim helpmenuitem As New MenuItem
'        helpmenuitem.Header = "Help for " & _ParseTreeNode.DisplayName
'        AddHandler helpmenuitem.Click, AddressOf LaunchHelp
'        c.Items.Add(helpmenuitem)
'        MyBase.ContextMenu = c
'    End Sub
'    Private Sub LaunchHelp(ByVal sender As Object, ByVal e As RoutedEventArgs)
'        AddHandler QuickHelp.HelpDialog.HelpNotFound, AddressOf closeHelp
'        Dim h As New QuickHelp.HelpDialog(_ParseTreeNode.HelpFile, _ParseTreeNode.DisplayName, _ParseTreeNode.GetType.Assembly.FullName, _ParseTreeNode.GetType.Namespace)
'        Try
'            h.Show()
'        Catch ex As Exception

'        End Try


'    End Sub
'    Private Sub CloseHelp(ByRef hd As QuickHelp.HelpDialog)
'        hd.Close()
'    End Sub

'    Private Sub FCTreeItem_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles Me.MouseDoubleClick
'        If e.LeftButton = MouseButtonState.Pressed Then
'            RaiseEvent TextToAdd(_ParseTreeNode.FunctionSyntax)
'        End If
'    End Sub
'End Class
