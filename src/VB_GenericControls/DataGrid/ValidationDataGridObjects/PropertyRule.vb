Imports System.ComponentModel

Public Class PropertyRule
    Implements INotifyPropertyChanged

#Region "Construction"

    ''' <summary>
    ''' Construct new property rule.
    ''' </summary>
    ''' <param name="rule">Rule as a function that returns a boolean.</param>
    ''' <param name="message">The error message to display if function returns True.</param>
    Public Sub New(rule As Func(Of Boolean), message As String)
        _rules.Add(New Rule(rule, message))
    End Sub

#End Region

#Region "Members"

    ' This has to implement notify property changed to alert the UI to change color state.
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private ReadOnly _rules As New List(Of Rule)
    Private _hasError As Boolean = False
    Private _errorMessage As String = String.Empty

    ''' <summary>
    ''' Determines whether the property has an erro. 
    ''' </summary>
    Public Property HasError() As Boolean
        Get
            Return _hasError
        End Get
        Set
            If _hasError <> Value Then
                _hasError = Value
                NotifyPropertyChanged(NameOf(HasError))
            End If
        End Set
    End Property

    ''' <summary>
    ''' Returns the error message as string.
    ''' </summary>
    Public Property ErrorMessage() As String
        Get
            Return _errorMessage
        End Get
        Set
            If _errorMessage <> Value Then
                _errorMessage = Value
                NotifyPropertyChanged(NameOf(ErrorMessage))
            End If
        End Set
    End Property

    Public ReadOnly Property Rules As List(Of Rule)
        Get
            Return _rules
        End Get
    End Property

    ''' <summary>
    ''' Class for the property rule. Each rule has a function and an error message.
    ''' </summary>
    Public Class Rule
        Public ReadOnly Expression As Func(Of Boolean)
        Public ReadOnly Message As String
        Public HasError As Boolean
        Friend Sub New(expression As Func(Of Boolean), message As String)
            Me.Expression = expression
            Me.Message = message
        End Sub
    End Class

#End Region

#Region "Methods"

    ''' <summary>
    ''' Raise property changed event.
    ''' </summary>
    ''' <param name="propertyName">Name of the property than changed.</param>
    Protected Sub NotifyPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    ''' <summary>
    ''' Add rule to the proprty.
    ''' </summary>
    ''' <param name="rule">Rule as a function that returns a boolean.</param>
    ''' <param name="message">The error message to display if function returns True.</param>
    Friend Sub AddRule(rule As Func(Of Boolean), message As String)
        _rules.Add(New Rule(rule, message))
    End Sub



    ''' <summary>
    ''' Execute the property rules.
    ''' </summary>
    Friend Sub ExecuteRules()
        ErrorMessage = ""
        HasError = False
        Try
            For i As Int32 = 0 To _rules.Count - 1
                If _rules(i).Expression() = True Then
                    '_rules(i).HasError = True
                    HasError = True
                    ErrorMessage += If(i = 0, _rules(i).Message, Environment.NewLine + _rules(i).Message)
                End If
            Next

            'For Each r As Rule In _rules
            '    If r.Expression() = True Then
            '        r.HasError = True
            '        ErrorMessage = ErrorMessage & r.Message & vbLf
            '        HasError = True
            '    End If
            'Next
            'If HasError Then ErrorMessage = ErrorMessage.TrimEnd(New [Char]() {ControlChars.Lf})
        Catch e As Exception
            _errorMessage = e.Message
            HasError = True
        End Try
    End Sub

#End Region

End Class
