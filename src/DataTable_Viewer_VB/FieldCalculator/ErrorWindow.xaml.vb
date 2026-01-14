Public Class ErrorWindow
    Implements System.ComponentModel.INotifyPropertyChanged
    Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
    Private _errors As System.Collections.ObjectModel.ObservableCollection(Of ErrorItem)
    Sub New(ByVal errorlist As List(Of String), ByVal title As String)

        ' This call is required by the designer.
        InitializeComponent()
        Me.Title = title
        ' Add any initialization after the InitializeComponent() call.
        Dim tmp As New System.Collections.ObjectModel.ObservableCollection(Of ErrorItem)
        For i = 0 To errorlist.Count - 1
            tmp.Add(New ErrorItem(errorlist(i)))
        Next
        Errors = tmp
    End Sub
    Sub New(ByVal errorlist As IList(Of ExpressionParser.ParseError), ByVal title As String)

        ' This call is required by the designer.
        InitializeComponent()
        Me.Title = title
        ' Add any initialization after the InitializeComponent() call.
        Dim tmp As New System.Collections.ObjectModel.ObservableCollection(Of ErrorItem)
        For i = 0 To errorlist.Count - 1
            tmp.Add(New ErrorItem(errorlist(i).Description))
        Next
        Errors = tmp
    End Sub
    Public Property Errors As System.Collections.ObjectModel.ObservableCollection(Of ErrorItem)
        Get
            Return _Errors
        End Get
        Set(value As System.Collections.ObjectModel.ObservableCollection(Of ErrorItem))
            _Errors = value
            NotifyPropertyChanged("Errors")
        End Set
    End Property
    Private Sub NotifyPropertyChanged(ByVal info As String)
        RaiseEvent PropertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(info))
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs) Handles CancelButton.Click
        Me.Close()
    End Sub
End Class
