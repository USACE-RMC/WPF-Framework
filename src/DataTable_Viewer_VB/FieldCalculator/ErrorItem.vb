Public Class ErrorItem
    Implements System.ComponentModel.INotifyPropertyChanged
    Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged
    Private _message As String
    Sub New(ByVal str As String)
        _message = str
    End Sub
    Public Property Message As String
        Get
            Return _message
        End Get
        Set(value As String)
            _message = value
            NotifyPropertyChanged("Message")
        End Set
    End Property
    Private Sub NotifyPropertyChanged(ByVal info As String)
        RaiseEvent PropertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(info))
    End Sub
End Class
