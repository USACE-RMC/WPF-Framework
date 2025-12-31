Imports System.ComponentModel

Public Class ColorItem
    Implements INotifyPropertyChanged

    Private _name As String = ""
    Private _colorTest As SolidColorBrush = New SolidColorBrush(Colors.White)

    Public Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Name)))
        End Set
    End Property
    Public Property ColorTest As SolidColorBrush
        Get
            Return _colorTest
        End Get
        Set(value As SolidColorBrush)
            _colorTest = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ColorTest)))
        End Set
    End Property


    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
End Class
