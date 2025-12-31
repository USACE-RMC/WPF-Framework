Imports System.ComponentModel

Public Class LineWidthSelectorControl
    Implements INotifyPropertyChanged

    Public Shared SelectedWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedWidth), GetType(Double), GetType(LineWidthSelectorControl), New UIPropertyMetadata(CDbl(0)))
    Public Property SelectedWidth As Double
        Get
            Return DirectCast(GetValue(SelectedWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(SelectedWidthProperty, value)
        End Set
    End Property
    '
    Public Shared WidthOptionsProperty As DependencyProperty = DependencyProperty.Register(NameOf(WidthOptions), GetType(IList(Of Double)), GetType(LineWidthSelectorControl), New PropertyMetadata(New List(Of Double)({0, 1, 2, 3, 4, 5})))
    Public Property WidthOptions As IList(Of Double)
        Get
            Return DirectCast(GetValue(WidthOptionsProperty), IList(Of Double))
        End Get
        Set(value As IList(Of Double))
            SetValue(WidthOptionsProperty, value)
        End Set
    End Property
    '
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(LineWidthSelectorControl), New UIPropertyMetadata("Title"))
    Public Property Title As String
        Get
            Return DirectCast(GetValue(TitleProperty), String)
        End Get
        Set(value As String)
            SetValue(TitleProperty, value)
        End Set
    End Property
    '
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(LineWidthSelectorControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))
    Public Property MaxPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MaxPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MaxPropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(LineWidthSelectorControl), New UIPropertyMetadata(DefaultMinPropertyWidth))
    Public Property MinPropertyWidth As Double
        Get
            Return DirectCast(GetValue(MinPropertyWidthProperty), Double)
        End Get
        Set(value As Double)
            SetValue(MinPropertyWidthProperty, value)
        End Set
    End Property
    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(LineWidthSelectorControl), New UIPropertyMetadata(DefaultPropertyWidth))
    Public Property PropertyWidth As GridLength
        Get
            Return DirectCast(GetValue(PropertyWidthProperty), GridLength)
        End Get
        Set(value As GridLength)
            SetValue(PropertyWidthProperty, value)
        End Set
    End Property
    '
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(LineWidthSelectorControl), New UIPropertyMetadata(True))
    Public Property ShowLeaderLine As Boolean
        Get
            Return DirectCast(GetValue(ShowLeaderLineProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowLeaderLineProperty, value)
        End Set
    End Property

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

    Private Sub ControlSizeChanged(sender As Object, e As SizeChangedEventArgs)
        Dim el = TryCast(sender, FrameworkElement)
        ActualPropertyWidth = el.ActualWidth
    End Sub

    Private Sub ComboBox_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim tBox As ComboBox = DirectCast(sender, ComboBox)
        If Not Char.IsDigit(CChar(e.Text)) Then e.Handled = True 'numeric only
        If e.Text = Chr(8) Then e.Handled = False 'allow Backspace
        If e.Text = " " Then e.Handled = True 'don't allow spaces (note, this doesn't fire for the previewtextinput event so doesn't have an effect here).
        'If CanHaveNegative = True Then If e.Text = "-" And tBox.Text.IndexOf("-", StringComparison.Ordinal) = -1 Then e.Handled = False 'allow negative
        If e.Text = "." Then 'allow one decimal
            If tBox.Text.IndexOf(".", StringComparison.Ordinal) = -1 Then e.Handled = False
            'If tBox.SelectedText.IndexOf(".", StringComparison.Ordinal) > -1 Then e.Handled = False
        End If
    End Sub

    Private Sub ComboBox_PreviewKeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Space Then e.Handled = True
    End Sub
End Class
