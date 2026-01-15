Public Class ExpressionTextBox
    Inherits TextBox
    Private _parenColors As Stack(Of SolidColorBrush)
    Private _colorList As List(Of SolidColorBrush)
    Private _textBlock As TextBlock
    Private _bw As System.ComponentModel.BackgroundWorker
    Private _numLines As Integer
    Private _s As Stopwatch
    Private _tree As FieldCalculationParser.ParseTreeNode
    Private _result As String
    Private _isCaseSensitive As Boolean
    Private _headers As List(Of String)
    Private _headertypes As List(Of Type)
    Private _firstRowOfData As Object()
    Private _outputType As FieldCalculationParser.TypeEnum

    Private Event NumLinesChanged()
    Public Event ErrorsFound(ByVal TreeIsNothing As Boolean)
    Public Event ParseSuccess()
#Region "Properties"
    Public Property IsCaseSensitive As Boolean
        Get
            Return _isCaseSensitive
        End Get
        Set(value As Boolean)
            _isCaseSensitive = value
            Parse()
        End Set
    End Property
    Public ReadOnly Property GetTree As FieldCalculationParser.ParseTreeNode
        Get
            Return _tree
        End Get
    End Property
    Public ReadOnly Property GetResult As String
        Get
            Return _Result
        End Get
    End Property

    Public WriteOnly Property SetHeaders As List(Of String)
        Set(value As List(Of String))
            _headers = value
        End Set
    End Property
    Public WriteOnly Property SetHeaderTypes As List(Of Type)
        Set(value As List(Of Type))
            _headertypes = value
        End Set
    End Property
    Public ReadOnly Property GetTextBlock As TextBlock
        Get
            Return _TextBlock
        End Get
    End Property
    Public WriteOnly Property SetOutputType As Type
        Set(value As Type)
            Select Case value
                Case GetType(Double)
                    _outputType = FieldCalculationParser.TypeEnum.Dub
                Case GetType(Boolean)
                    _outputType = FieldCalculationParser.TypeEnum.Bool
                Case GetType(String)
                    _outputType = FieldCalculationParser.TypeEnum.Str
                Case GetType(Int32)
                    _outputType = FieldCalculationParser.TypeEnum.Int
                Case GetType(Single)
                    _outputType = FieldCalculationParser.TypeEnum.Dub
                Case GetType(Byte)
                    _outputType = FieldCalculationParser.TypeEnum.byt
                Case GetType(Short)
                    _outputType = FieldCalculationParser.TypeEnum.Shrt
                Case Nothing
                    _outputType = FieldCalculationParser.TypeEnum.UnDeclared
                Case Else
                    MsgBox("the type of the selected header is not supported for output")
            End Select
        End Set
    End Property
    Public WriteOnly Property SetDataForFirstRow As Object()
        Set(value As Object())
            _firstRowOfData = value
        End Set
    End Property
#End Region
    Sub New()
        _outputType = FieldCalculationParser.TypeEnum.UnDeclared

        _s = New Stopwatch
        _bw = New System.ComponentModel.BackgroundWorker
        AddHandler _bw.DoWork, AddressOf swstart
        AddHandler _bw.RunWorkerCompleted, AddressOf RemoveTextBlock
        TextWrapping = TextWrapping.Wrap
        _bw.WorkerSupportsCancellation = True
        _textBlock = New TextBlock With {.TextWrapping = TextWrapping.Wrap, .Margin = New Thickness(5, 16, 5, 0), .Background = New SolidColorBrush(Color.FromArgb(230, 240, 240, 240)), .Effect = New Effects.DropShadowEffect(), .Visibility = Visibility.Hidden, .HorizontalAlignment = HorizontalAlignment.Left, .VerticalAlignment = VerticalAlignment.Top}
        _NumLines = 1
        AddHandler _TextBlock.MouseEnter, AddressOf CancelBW
        AddHandler _TextBlock.MouseLeave, AddressOf StartBW
        AddHandler NumLinesChanged, AddressOf Shiftdownward
        Grid.SetRow(_textBlock, 1)
        _colorList = New List(Of SolidColorBrush)
        _colorList.Add(New SolidColorBrush(Color.FromArgb(255, 255, 0, 0)))
        _colorList.Add(New SolidColorBrush(Color.FromArgb(255, 0, 255, 0)))
        _colorList.Add(New SolidColorBrush(Color.FromArgb(255, 255, 0, 255)))
        _colorList.Add(New SolidColorBrush(Color.FromArgb(255, 0, 204, 204)))
        _colorList.Add(New SolidColorBrush(Color.FromArgb(255, 255, 130, 0)))
        _colorList.Add(New SolidColorBrush(Color.FromArgb(255, 0, 255, 128)))
        _colorList.Add(New SolidColorBrush(Color.FromArgb(255, 157, 0, 255)))
        _colorList.Add(New SolidColorBrush(Color.FromArgb(255, 102, 204, 0)))
    End Sub
#Region "TextBlockEvents"
    Private Sub Shiftdownward()
        _textBlock.Margin = New Thickness(_textBlock.Margin.Left, 5 + (16 * MyBase.LineCount), _textBlock.Margin.Right, _textBlock.Margin.Bottom)
        _numLines = MyBase.LineCount
    End Sub
    Private Sub RemoveTextBlock()
        _textBlock.Visibility = Windows.Visibility.Hidden
    End Sub
    Private Sub StartBW()
        _s.Restart()
        If Not _bw.IsBusy Then _bw.RunWorkerAsync()
    End Sub
    Private Sub CancelBW()
        _s.Stop()
    End Sub
    Private Sub swstart()
        While _s.ElapsedMilliseconds < 2000
        End While
    End Sub
#End Region
#Region "TextBlockText"
    Private Sub SetRichText(ByVal pos As Integer, ByVal token As FieldCalculationParser.TokenEnum, ByVal text As String, ByVal helpdoc As String)
        _textBlock.Visibility = Visibility.Hidden
        _textBlock.MaxWidth = MyBase.ActualWidth - ((_TextBlock.Margin.Left - MyBase.Margin.Left) + (_TextBlock.Margin.Right - MyBase.Margin.Right))
        If helpdoc = "" Then
            If token = FieldCalculationParser.TokenEnum.LPAREN Then
                _ParenColors.Push(_colorList(_ParenColors.Count Mod _colorList.Count))
                _TextBlock.Inlines.Add(text)
                _TextBlock.Inlines.Last.Foreground = _ParenColors.Peek
            ElseIf token = FieldCalculationParser.TokenEnum.RPAREN Then
                If _ParenColors.Count > 0 Then
                    _TextBlock.Inlines.Add(text)
                    _TextBlock.Inlines.Last.Foreground = _ParenColors.Pop
                Else
                    _TextBlock.Inlines.Add(text)
                End If
            Else
                _TextBlock.Inlines.Add(text)
            End If
        Else
            Dim r As New Bold(New Run(text))
            Dim link As New Hyperlink(r)
            link.Tag = text
            link.NavigateUri = New Uri(helpdoc, UriKind.Relative)
            link.IsEnabled = True
            AddHandler link.Click, AddressOf link_RequestNavigate
            _TextBlock.Inlines.Add(link)
        End If
        _textBlock.Visibility = Visibility.Visible
    End Sub
    Private Sub link_RequestNavigate(sender As Object, e As System.Windows.RoutedEventArgs)
        Dim link As Hyperlink = DirectCast(sender, Hyperlink)
        Dim tmp As String() = Split(link.NavigateUri.ToString, ".")
        'Dim assemblyname As String = tmp(0)
        Dim filestring As String = tmp(1)
        For i = 2 To tmp.Count - 1
            filestring = filestring & "." & tmp(i)
        Next
        Dim hd As New QuickHelp.HelpDialog(filestring, link.Tag.ToString, _Tree.GetType.Assembly.FullName, _Tree.GetType.Namespace)
        hd.Show()
    End Sub
#End Region
    Public Sub Parse()
        If IsNothing(_firstRowOfData) Then
            _result = "No rows in database"
            RaiseEvent ParseSuccess()
            Exit Sub
        End If
        _parenColors = New Stack(Of System.Windows.Media.SolidColorBrush)
        _textBlock.Inlines.Clear()
        If _numLines <> MyBase.LineCount Then RaiseEvent NumLinesChanged()
        _s.Restart()
        If Not _bw.IsBusy Then _bw.RunWorkerAsync()
        Dim strbytes() As Byte = New System.Text.UTF8Encoding().GetBytes(MyBase.Text)
        Dim ms As New System.IO.MemoryStream(strbytes)
        Dim scanner As New FieldCalculationParser.Scanner(ms)
        Dim parser As New FieldCalculationParser.Parser(scanner)
        AddHandler parser.TokenFound, AddressOf SetRichText
        FieldCalculationParser.ParseTreeNode.IsCaseSensitive = _isCaseSensitive
        FieldCalculationParser.ParseTreeNode.RowOrCellNum = 0
        Try
            _tree = parser.Parse(_headers, _headertypes, _outputType)
            _textBlock.Inlines.Add(" ")
            If Not IsNothing(_tree) Then
                If _tree.GetParseErrors.Count > 0 Then
                    If _tree.ContainsError Then
                        _result = "Tree contains errors"
                        RaiseEvent ErrorsFound(False)
                    Else
                        _result = "Invalid Syntax, check Error Log"
                        RaiseEvent ErrorsFound(False)
                    End If
                Else
                    If _tree.containsVariable Then
                        Dim importiantheaders As List(Of String) = GetColumnsFromTree(_tree)
                        If importiantheaders.Count = 0 Then
                        Else
                            _tree.SetColNums(importiantheaders)
                            _tree.Update(GetDataFromDatabaseForUniqueColumns(importiantheaders))
                        End If
                    End If
                    _result = _tree.Evaluate.GetResult.ToString
                    RaiseEvent ParseSuccess()
                End If
            Else
                _result = ""
                RaiseEvent ErrorsFound(True)
            End If
        Catch ex As Exception
            _result = "Exception"
            RaiseEvent ErrorsFound(True)
        End Try
    End Sub
    Private Sub Expression_KeyUp(sender As Object, e As System.Windows.Input.KeyEventArgs) Handles MyBase.KeyUp
        Parse()
    End Sub
    Private Function GetColumnsFromTree(tree As FieldCalculationParser.ParseTreeNode) As List(Of String)
        Return tree.GetHeaderNames.Distinct.ToList()
    End Function
    Private Function GetDataFromDatabaseForUniqueColumns(ByVal uniqueheaders As List(Of String)) As Object()
        Dim data As New List(Of Object)
        If IsNothing(_firstRowOfData) Then Return data.ToArray
        For i = 0 To uniqueheaders.Count - 1
            data.Add(_firstRowOfData(_headers.IndexOf(uniqueheaders(i))))
        Next
        Return data.ToArray
    End Function
End Class
