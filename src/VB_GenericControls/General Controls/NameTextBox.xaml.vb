Public Class NameTextBox
    Public Shared TextProperty As DependencyProperty = DependencyProperty.Register(NameOf(Text), GetType(String), GetType(NameTextBox), New FrameworkPropertyMetadata(""))
    ''' <summary>
    ''' The name string.
    ''' </summary>
    ''' <returns></returns>
    Public Property Text As String
        Get
            Return CType(GetValue(TextProperty), String)
        End Get
        Set(value As String)
            SetValue(TextProperty, value)
        End Set
    End Property

    Public Shared CharacterLimitProperty As DependencyProperty = DependencyProperty.Register(NameOf(CharacterLimit), GetType(Int32), GetType(NameTextBox), New FrameworkPropertyMetadata(CInt(64), AddressOf ValidationProperty_Callback))
    ''' <summary>
    ''' Maximum number of characters that the name string can contain. Default is 64 characters.
    ''' </summary>
    ''' <returns></returns>
    Public Property CharacterLimit As Int32
        Get
            Return CType(GetValue(CharacterLimitProperty), Int32)
        End Get
        Set(value As Int32)
            SetValue(CharacterLimitProperty, value)
        End Set
    End Property

    Public Shared CanBeBlankProperty As DependencyProperty = DependencyProperty.Register(NameOf(CanBeBlank), GetType(Boolean), GetType(NameTextBox), New FrameworkPropertyMetadata(False, AddressOf ValidationProperty_Callback))

    ''' <summary>
    ''' Value indicating if the name string can be blank/empty or not. Default is no.
    ''' </summary>
    ''' <returns></returns>
    Public Property CanBeBlank As Boolean
        Get
            Return CType(GetValue(CanBeBlankProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(CanBeBlankProperty, value)
        End Set
    End Property

    Public Shared InvalidCharactersProperty As DependencyProperty = DependencyProperty.Register(NameOf(InvalidCharacters), GetType(Char()), GetType(NameTextBox), New FrameworkPropertyMetadata(GetDefaultInvalidCharacters(), AddressOf ValidationProperty_Callback))

    ''' <summary>
    ''' Array of characters that are invalid. Default is invalid filename characters with the addition of apostrophe, left bracket, and right bracket.
    ''' </summary>
    ''' <returns></returns>
    Public Property InvalidCharacters As Char()
        Get
            Return CType(GetValue(InvalidCharactersProperty), Char())
        End Get
        Set(value As Char())
            SetValue(InvalidCharactersProperty, value)
        End Set
    End Property

    Public Shared InvalidStringsProperty As DependencyProperty = DependencyProperty.Register(NameOf(InvalidStrings), GetType(String()), GetType(NameTextBox), New FrameworkPropertyMetadata(New String() {}, AddressOf ValidationProperty_Callback))
    ''' <summary>
    ''' Array of strings that are invalid. Default is no invalid strings.
    ''' </summary>
    ''' <returns></returns>
    Public Property InvalidStrings As String()
        Get
            Return CType(GetValue(InvalidStringsProperty), String())
        End Get
        Set(value As String())
            SetValue(InvalidStringsProperty, value)
        End Set
    End Property

    Public Shared IsValidProperty As DependencyProperty = DependencyProperty.Register(NameOf(IsValid), GetType(Boolean), GetType(NameTextBox), New FrameworkPropertyMetadata(True))
    ''' <summary>
    ''' Value indicating if the name string can be blank/empty or not. Default is no.
    ''' </summary>
    ''' <returns></returns>
    Public Property IsValid As Boolean
        Get
            Return CType(GetValue(IsValidProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(IsValidProperty, value)
        End Set
    End Property

    Private Shared Sub ValidationProperty_Callback(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        If IsNothing(d) Then Exit Sub
        If d.GetType <> GetType(NameTextBox) Then Exit Sub
        Dim thisControl = DirectCast(d, NameTextBox)
        thisControl.ValidateText()
    End Sub

    ''' <summary>
    ''' Shared function to get the default invalid characters for the name textbox. invalid characters includes invalid file name characters, apostraphe, left bracket, and right bracket.
    ''' </summary>
    ''' <returns>array of default invalid name characters</returns>
    Public Shared Function GetDefaultInvalidCharacters() As Char()
        Dim invalidCharsList As New List(Of Char)(IO.Path.GetInvalidFileNameChars)
        invalidCharsList.Add("'"c)
        invalidCharsList.Add("["c)
        invalidCharsList.Add("]"c)
        Return invalidCharsList.ToArray()
    End Function

    Public Event TextChanged(sender As Object, e As TextChangedEventArgs)

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ValidateText()
    End Sub

    Private Sub NameTBox_TextChanged(sender As Object, e As TextChangedEventArgs)

        RaiseEvent TextChanged(sender, e)

        ValidateText()
    End Sub
    Public Sub ValidateText()
        NameTBox.ToolTip = Nothing
        '
        If NameTBox.Text.Length > CharacterLimit And CharacterLimit > -1 Then
            NameTBox.ToolTip = "The name entered is longer (" & NameTBox.Text.Length & " characters) than the maximum name length (" & CharacterLimit & " characters)."
            IsValid = False
            Exit Sub
        End If
        '
        If NameTBox.Text = "" And CanBeBlank = False Then
            NameTBox.ToolTip = "The name entered cannot be blank. The name entered must not be blank and must be less than " & CharacterLimit & " characters."
            IsValid = False
            Exit Sub
        End If
        '
        If InvalidStrings.Contains(NameTBox.Text) Then
            NameTBox.ToolTip = "Name entered already exists and must be unique."
            IsValid = False
            Exit Sub
        End If
        '
        For Each badChar As Char In InvalidCharacters
            If NameTBox.Text.Contains(badChar) Then
                NameTBox.ToolTip = "Invalid character in name: '" & badChar & "'"
                IsValid = False
                Exit Sub
            End If
        Next
        '
        IsValid = True
    End Sub
    ''' <summary>
    ''' Get all error messages associated with the text in the name textbox.
    ''' </summary>
    ''' <returns>A list of error messages.</returns>
    Public Function GetErrorMessages() As List(Of String)
        Dim errorList As New List(Of String)
        If IsValid = True Then Return errorList
        '
        If NameTBox.Text.Length > CharacterLimit And CharacterLimit > -1 Then
            errorList.Add("The name entered is longer (" & NameTBox.Text.Length & " characters) than the maximum name length (" & CharacterLimit & " characters).")
        End If
        '
        If NameTBox.Text = "" And CanBeBlank = False Then
            errorList.Add("The name entered cannot be blank. The name entered must not be blank and must be less than " & CharacterLimit & " characters.")
        End If
        '
        If InvalidStrings.Contains(NameTBox.Text) Then
            errorList.Add("Name entered already exists and must be unique.")
        End If
        '
        For Each badChar As Char In InvalidCharacters
            If NameTBox.Text.Contains(badChar) Then
                errorList.Add("Invalid character in name: '" & badChar & "'")
            End If
        Next
        '
        Return errorList
    End Function

    Public Sub SelectAll()
        NameTBox.SelectAll()
    End Sub

    Private Sub TextBox_PreviewKeyUp(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Enter Then
            Dim tBox As TextBox = DirectCast(sender, TextBox)
            Dim binding As BindingExpression = BindingOperations.GetBindingExpression(tBox, TextBox.TextProperty)
            If binding IsNot Nothing Then binding.UpdateSource()
        End If
    End Sub

End Class
