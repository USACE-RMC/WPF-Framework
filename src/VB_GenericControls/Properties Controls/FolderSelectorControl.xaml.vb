Imports System.ComponentModel

Public Class FolderSelectorControl
    Implements INotifyPropertyChanged

#Region "Members"

    ''' <summary>
    ''' Dependency property for the text property.
    ''' </summary>
    Public Shared TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(FolderSelectorControl), New FrameworkPropertyMetadata(""))

    ''' <summary>
    ''' Gets and sets the text.
    ''' </summary>
    Public Property Text As String
        Get
            Return DirectCast(GetValue(TextProperty), String)
        End Get
        Set(value As String)
            SetValue(TextProperty, value)
        End Set
    End Property

    Public Shared IsReadOnlyProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsReadOnly), GetType(Boolean), GetType(FolderSelectorControl), New FrameworkPropertyMetadata(False))
    Public Property IsReadOnly As Boolean
        Get
            Return DirectCast(GetValue(IsReadOnlyProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsReadOnlyProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the title property.
    ''' </summary>
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(FolderSelectorControl), New FrameworkPropertyMetadata("Title"))

    ''' <summary>
    ''' Gets and sets the title.
    ''' </summary>
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property


    ''' <summary>
    ''' Dependency property for the max width property.
    ''' </summary>
    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(FolderSelectorControl), New FrameworkPropertyMetadata(DefaultPropertyWidth))

    ''' <summary>
    ''' Gets and sets the max width of the control.
    ''' </summary>
    Public Property PropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(PropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(PropertyWidthProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the max width property.
    ''' </summary>
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(FolderSelectorControl), New FrameworkPropertyMetadata(DefaultMaxPropertyWidth))

    ''' <summary>
    ''' Gets and sets the max width of the control.
    ''' </summary>
    Public Property MaxPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxPropertyWidthProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the min width property.
    ''' </summary>
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(FolderSelectorControl), New FrameworkPropertyMetadata(DefaultMinPropertyWidth))

    ''' <summary>
    ''' Gets and sets the min width of the control.
    ''' </summary>
    Public Property MinPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinPropertyWidthProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the show leader line property.
    ''' </summary>
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(FolderSelectorControl), New FrameworkPropertyMetadata(False, FrameworkPropertyMetadataOptions.AffectsRender))

    ''' <summary>
    ''' Determines of the leader line should be visible. 
    ''' </summary>
    Public Property ShowLeaderLine As Boolean
        Get
            Return DirectCast(GetValue(ShowLeaderLineProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowLeaderLineProperty, value)
        End Set
    End Property

#End Region
    Private _actualWidth As Double = 0
    Public Property ActualPropertyWidth As Double
        Get
            Return _actualWidth
        End Get
        Private Set(value As Double)
            If _actualWidth <> value Then
                _actualWidth = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ActualPropertyWidth)))
            End If
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Public Event TextChanged(sender As Object, e As TextChangedEventArgs)

    Private Sub ControlSizeChanged(sender As Object, e As SizeChangedEventArgs)
        Dim el = TryCast(sender, FrameworkElement)
        ActualPropertyWidth = el.ActualWidth
    End Sub

    Private Sub FolderPathButton_Click(sender As Object, e As RoutedEventArgs)
        Dim folder As String = FolderBrowserDialog(Window.GetWindow(Me))
        If Not IsNothing(folder) AndAlso folder <> "" Then Text = folder
    End Sub

    Private Sub FolderPathTextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
        RaiseEvent TextChanged(sender, e)
    End Sub
End Class
