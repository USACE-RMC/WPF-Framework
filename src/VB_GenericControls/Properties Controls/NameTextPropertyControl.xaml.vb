Imports System.ComponentModel

Public Class NameTextPropertyControl
    Implements INotifyPropertyChanged


#Region "Construction"

#End Region

#Region "Members"

    ''' <summary>
    ''' Dependency property for the character limit property.
    ''' </summary>
    Public Shared CharacterLimitProperty As DependencyProperty = DependencyProperty.Register(NameOf(CharacterLimit), GetType(Int32), GetType(NameTextPropertyControl), New FrameworkPropertyMetadata(64))

    ''' <summary>
    ''' Maximum number of characters that the name string can contain. Default is 64 characters.
    ''' </summary>
    Public Property CharacterLimit As Int32
        Get
            Return CType(GetValue(CharacterLimitProperty), Int32)
        End Get
        Set(value As Int32)
            SetValue(CharacterLimitProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the existing names property.
    ''' </summary>
    Public Shared ExistingNamesProperty As DependencyProperty = DependencyProperty.Register(NameOf(ExistingNames), GetType(String()), GetType(NameTextPropertyControl), New FrameworkPropertyMetadata(New String() {}))

    ''' <summary>
    ''' Array of strings that are invalid. Default is no invalid strings.
    ''' </summary>
    Public Property ExistingNames As String()
        Get
            Return CType(GetValue(ExistingNamesProperty), String())
        End Get
        Set(value As String())
            SetValue(ExistingNamesProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the invalid characters property. 
    ''' </summary>
    Public Shared InvalidCharactersProperty As DependencyProperty = DependencyProperty.Register(NameOf(InvalidCharacters), GetType(Char()), GetType(NameTextPropertyControl), New FrameworkPropertyMetadata(NameTextBox.GetDefaultInvalidCharacters()))

    ''' <summary>
    ''' Array of characters that are invalid. Default is invalid filename characters with the addition of apostrophe, left bracket, and right bracket.
    ''' </summary>
    Public Property InvalidCharacters As Char()
        Get
            Return CType(GetValue(InvalidCharactersProperty), Char())
        End Get
        Set(value As Char())
            SetValue(InvalidCharactersProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Dependency property for the text property.
    ''' </summary>
    Public Shared TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(NameTextPropertyControl), New UIPropertyMetadata(""))

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

    ''' <summary>
    ''' Dependency property for the title property.
    ''' </summary>
    Public Shared TitleProperty As DependencyProperty = DependencyProperty.Register(NameOf(Title), GetType(String), GetType(NameTextPropertyControl), New UIPropertyMetadata("Title"))

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
    Public Shared PropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(PropertyWidth), GetType(GridLength), GetType(NameTextPropertyControl), New UIPropertyMetadata(DefaultPropertyWidth))

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
    Public Shared MaxPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MaxPropertyWidth), GetType(Double), GetType(NameTextPropertyControl), New UIPropertyMetadata(DefaultMaxPropertyWidth))

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
    Public Shared MinPropertyWidthProperty As DependencyProperty = DependencyProperty.Register(NameOf(MinPropertyWidth), GetType(Double), GetType(NameTextPropertyControl), New UIPropertyMetadata(DefaultMinPropertyWidth))

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
    Public Shared ShowLeaderLineProperty As DependencyProperty = DependencyProperty.Register(NameOf(ShowLeaderLine), GetType(Boolean), GetType(NameTextPropertyControl), New UIPropertyMetadata(True))

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

    Private Sub ControlSizeChanged(sender As Object, e As SizeChangedEventArgs)
        Dim el = TryCast(sender, FrameworkElement)
        ActualPropertyWidth = el.ActualWidth
    End Sub

    ''' <summary>
    ''' Place focus on the text box and set the caret position.
    ''' </summary>
    ''' <param name="caretIndex"></param>
    Public Sub TextBoxFocus(caretIndex As Int32)
        Keyboard.Focus(NameTextBox.NameTBox)
        NameTextBox.NameTBox.CaretIndex = caretIndex
        NameTextBox.NameTBox.Focus()
    End Sub

    Private Sub NameTextBox_PreviewKeyUp(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Enter Then
            'Dim tBox As NameTextBox = DirectCast(sender, NameTextBox)
            'Dim binding As BindingExpression = BindingOperations.GetBindingExpression(tBox, NameTextBox.TextProperty)
            'If binding IsNot Nothing Then binding.UpdateSource()
            DirectCast(e.OriginalSource, UIElement).MoveFocus(New TraversalRequest(FocusNavigationDirection.Next))
        End If
    End Sub
End Class
