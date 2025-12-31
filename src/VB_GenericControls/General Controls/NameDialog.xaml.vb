Public Class NameDialog
    'Private _CharacterLimit As Int32 = -1
    'Private _canBeBlank As Boolean = True
    'Private _canBeDuplicate As Boolean = True
    'Private _existingNames() As String = {}
    'Private _canHaveInvalidCharacters As Boolean = True

    Public Shared TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(NameDialog), New FrameworkPropertyMetadata(""))
    Public Property Text As String
        Get
            Return CType(GetValue(TextProperty), String)
        End Get
        Set(value As String)
            SetValue(TextProperty, value)
        End Set
    End Property

    Public Shared InnerContentProperty As DependencyProperty = DependencyProperty.Register(NameOf(InnerContent), GetType(Object), GetType(NameDialog), New FrameworkPropertyMetadata(Nothing))
    Public Property InnerContent As Object
        Get
            Return DirectCast(GetValue(InnerContentProperty), Object)
        End Get
        Set(value As Object)
            SetValue(InnerContentProperty, value)
        End Set
    End Property

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Sub New(ByVal CharLimit As Int32)
        InitializeComponent()
        NameTBox.CharacterLimit = CharLimit
    End Sub
    Public Sub New(charLimit As Int32, initialText As String, canBeBlank As Boolean, existingNames() As String, Optional ByVal invalidCharacters As Char() = Nothing)
        InitializeComponent()

        NameTBox.CharacterLimit = charLimit
        NameTBox.CanBeBlank = canBeBlank
        NameTBox.InvalidStrings = existingNames
        If Not IsNothing(invalidCharacters) Then NameTBox.InvalidCharacters = invalidCharacters
        Text = initialText
    End Sub
    Private Sub OKButton_Click(sender As Object, e As RoutedEventArgs)
        If NameTBox.IsValid = False Then
            MsgBox("Invalid name for the following reasons:" & vbNewLine & vbTab & "- " & String.Join(vbTab & "- ", NameTBox.GetErrorMessages), MsgBoxStyle.Critical, "Invalid Name")
            Exit Sub
        End If
        '
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub CancelButton_Click(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = False
        Me.Close()
    End Sub

    Private Sub NameDialog_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
        NameTBox.ValidateText()
        NameTBox.NameTBox.Focus()
        NameTBox.NameTBox.CaretIndex = NameTBox.Text.Length
    End Sub
End Class
