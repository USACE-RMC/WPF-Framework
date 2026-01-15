'Public Class AvailableFunctionsTreeView
'    Inherits Windows.Controls.TreeView
'    Public Event TextToAdd(ByVal text As String)
'    Sub New()
'        Dim dll As System.Reflection.Assembly = System.Reflection.Assembly.Load("FieldCalculationParser")
'        Dim s As Type() = dll.GetTypes
'        Dim maintree As New List(Of Windows.Controls.TreeViewItem)
'        For Each en As String In System.Enum.GetNames(GetType(FieldCalculationParser.DisplayTypes))
'            Dim treeitem As New Windows.Controls.TreeViewItem
'            treeitem.Header = en
'            maintree.Add(treeitem)
'        Next
'        FieldCalculationParser.ParseTreeNode.Initialize()
'        Dim displayitem As FieldCalculationParser.IDisplayToTreeNode
'        For i As Integer = 0 To s.Count - 1
'            Dim t As Type = s(i).GetInterface("FieldCalculationParser.IDisplayToTreeNode")
'            If IsNothing(t) Then
'            Else
'                Dim oh As System.Runtime.Remoting.ObjectHandle = System.Activator.CreateInstance("FieldCalculationParser", s(i).FullName) 'requires empty constructor
'                displayitem = oh.Unwrap

'                Dim fc As New FCTreeItem(displayitem)
'                AddHandler fc.TextToAdd, AddressOf UpdateTextbox
'                For j = 0 To maintree.Count - 1
'                    If maintree(j).Header = displayitem.DisplayType.ToString Then
'                        maintree(j).Items.Add(fc) : Exit For
'                    End If
'                Next
'            End If

'        Next
'        For i = 0 To maintree.Count - 1
'            MyBase.Items.Add(maintree(i))
'        Next
'    End Sub
'    Private Sub UpdateTextBox(ByVal text As String)
'        RaiseEvent TextToAdd(text)
'    End Sub
'End Class
