Imports System.IO
Imports System.Reflection
Imports Microsoft.Win32

''' <summary>
''' The following class is based off the solution from https://www.codeproject.com/Articles/23731/RecentFileList-a-WPF-MRU.
''' It has been converted to VB.NET and modified to only support Most Recently Used (MRU) through registry.
''' </summary>
Public Class RecentFileList
    Inherits Separator
    Public Interface IPersist
        Function RecentFiles(max As Integer) As List(Of String)
        Sub InsertFile(filepath As String, max As Integer)
        Sub RemoveFile(filepath As String, max As Integer)
    End Interface

    Public Property Persister() As IPersist

    Public Sub UseRegistryPersister()
        Persister = New RegistryPersister()
    End Sub
    Public Sub UseRegistryPersister(key As String)
        Persister = New RegistryPersister(key)
    End Sub

    Public Property MaxNumberOfFiles() As Integer
    Public Property MaxPathLength() As Integer
    Public Property FileMenu() As MenuItem

    ''' <summary>
    ''' Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
    ''' Default = "_{0}:  {2}"
    ''' </summary>
    Public Property MenuItemFormatOneToNine() As String

    ''' <summary>
    ''' Used in: String.Format( MenuItemFormat, index, filepath, displayPath );
    ''' Default = "{0}:  {2}"
    ''' </summary>
    Public Property MenuItemFormatTenPlus() As String

    Public Delegate Function GetMenuItemTextDelegate(index As Integer, filepath As String) As String
    Public Property GetMenuItemTextHandler() As GetMenuItemTextDelegate

    Public Event MenuClick As EventHandler(Of MenuClickEventArgs)

    Private _Separator As Separator = Nothing
    Private _RecentFiles As List(Of RecentFile) = Nothing

    Public Sub New()
        Persister = New RegistryPersister()

        MaxNumberOfFiles = 9
        MaxPathLength = 50
        MenuItemFormatOneToNine = "_{0}:  {2}"
        MenuItemFormatTenPlus = "{0}:  {2}"

        AddHandler Me.Loaded, AddressOf HookFileMenu
    End Sub

    Private Sub HookFileMenu(s As Object, e As RoutedEventArgs)
        Dim parentItem As MenuItem = TryCast(Parent, MenuItem)
        If IsNothing(parentItem) Then Throw New ApplicationException("Parent must be a MenuItem")
        '
        If Not IsNothing(FileMenu) AndAlso FileMenu.Equals(parentItem) Then Return
        '
        If FileMenu IsNot Nothing Then
            RemoveHandler FileMenu.SubmenuOpened, AddressOf _FileMenu_SubmenuOpened
        End If
        '
        FileMenu = parentItem
        AddHandler FileMenu.SubmenuOpened, AddressOf _FileMenu_SubmenuOpened
    End Sub

    Public ReadOnly Property RecentFiles() As List(Of String)
        Get
            Return Persister.RecentFiles(MaxNumberOfFiles)
        End Get
    End Property
    Public Sub RemoveFile(filepath As String)
        Persister.RemoveFile(filepath, MaxNumberOfFiles)
    End Sub
    Public Sub InsertFile(filepath As String)
        Persister.InsertFile(filepath, MaxNumberOfFiles)
    End Sub

    Private Sub _FileMenu_SubmenuOpened(sender As Object, e As RoutedEventArgs)
        SetMenuItems()
    End Sub

    Private Sub SetMenuItems()
        RemoveMenuItems()

        LoadRecentFiles()

        InsertMenuItems()
    End Sub

    Private Sub RemoveMenuItems()
        If _Separator IsNot Nothing Then
            FileMenu.Items.Remove(_Separator)
        End If

        If _RecentFiles IsNot Nothing Then
            For Each r As RecentFile In _RecentFiles
                If r.MenuItem IsNot Nothing Then
                    FileMenu.Items.Remove(r.MenuItem)
                End If
            Next
        End If

        _Separator = Nothing
        _RecentFiles = Nothing
    End Sub

    Private Sub InsertMenuItems()
        If _RecentFiles Is Nothing Then
            Return
        End If
        If _RecentFiles.Count = 0 Then
            Return
        End If

        Dim iMenuItem As Integer = FileMenu.Items.IndexOf(Me)
        For Each r As RecentFile In _RecentFiles
            Dim header As String = GetMenuItemText(r.Number, r.Filepath, r.DisplayPath)

            r.MenuItem = New MenuItem() With {.Header = header}
            AddHandler r.MenuItem.Click, AddressOf MenuItem_Click

            FileMenu.Items.Insert(System.Threading.Interlocked.Increment(iMenuItem), r.MenuItem)
        Next

        _Separator = New Separator()
        FileMenu.Items.Insert(System.Threading.Interlocked.Increment(iMenuItem), _Separator)
    End Sub

    Private Function GetMenuItemText(index As Integer, filepath As String, displaypath As String) As String
        Dim delegateGetMenuItemText As GetMenuItemTextDelegate = GetMenuItemTextHandler
        If delegateGetMenuItemText IsNot Nothing Then
            Return delegateGetMenuItemText(index, filepath)
        End If

        Dim format As String = (If(index < 10, MenuItemFormatOneToNine, MenuItemFormatTenPlus))

        Dim shortPath As String = ShortenPathname(displaypath, MaxPathLength)

        Return [String].Format(format, index, filepath, shortPath)
    End Function

    ' This method is taken from Joe Woodbury's article at: http://www.codeproject.com/KB/cs/mrutoolstripmenu.aspx

    ''' <summary>
    ''' Shortens a pathname for display purposes.
    ''' </summary>
    ''' <param name="pathname">The pathname to shorten.</param>
    ''' <param name="maxLength">The maximum number of characters to be displayed.</param>
    ''' <remarks>Shortens a pathname by either removing consecutive components of a path
    ''' and/or by removing characters from the end of the filename and replacing
    ''' then with three elipses (...)
    ''' <para>In all cases, the root of the passed path will be preserved in it's entirety.</para>
    ''' <para>If a UNC path is used or the pathname and maxLength are particularly short,
    ''' the resulting path may be longer than maxLength.</para>
    ''' <para>This method expects fully resolved pathnames to be passed to it.
    ''' (Use Path.GetFullPath() to obtain this.)</para>
    ''' </remarks>
    ''' <returns></returns>
    Public Shared Function ShortenPathname(pathname As String, maxLength As Integer) As String
        If pathname.Length <= maxLength Then
            Return pathname
        End If

        Dim root As String = Path.GetPathRoot(pathname)
        If root.Length > 3 Then
            root += Path.DirectorySeparatorChar
        End If

        Dim elements As String() = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)

        Dim filenameIndex As Integer = elements.GetLength(0) - 1

        If elements.GetLength(0) = 1 Then
            ' pathname is just a root and filename
            If elements(0).Length > 5 Then
                ' long enough to shorten
                ' if path is a UNC path, root may be rather long
                If root.Length + 6 >= maxLength Then
                    Return (root & elements(0).Substring(0, 3)) + "..."
                Else
                    Return pathname.Substring(0, maxLength - 3) + "..."
                End If
            End If
        ElseIf (root.Length + 4 + elements(filenameIndex).Length) > maxLength Then
            ' pathname is just a root and filename
            root += "...\"

            Dim len As Integer = elements(filenameIndex).Length
            If len < 6 Then
                Return root + elements(filenameIndex)
            End If

            If (root.Length + 6) >= maxLength Then
                len = 3
            Else
                len = maxLength - root.Length - 3
            End If
            Return (root & elements(filenameIndex).Substring(0, len)) + "..."
        ElseIf elements.GetLength(0) = 2 Then
            Return (root & Convert.ToString("...\")) + elements(1)
        Else
            Dim len As Integer = 0
            Dim begin As Integer = 0

            For i As Integer = 0 To filenameIndex - 1
                If elements(i).Length > len Then
                    begin = i
                    len = elements(i).Length
                End If
            Next

            Dim totalLength As Integer = pathname.Length - len + 3
            Dim [end] As Integer = begin + 1

            While totalLength > maxLength
                If begin > 0 Then
                    totalLength -= elements(System.Threading.Interlocked.Decrement(begin)).Length - 1
                End If

                If totalLength <= maxLength Then
                    Exit While
                End If

                If [end] < filenameIndex Then
                    totalLength -= elements(System.Threading.Interlocked.Increment([end])).Length - 1
                End If

                If begin = 0 AndAlso [end] = filenameIndex Then
                    Exit While
                End If
            End While

            ' assemble final string

            For i As Integer = 0 To begin - 1
                root += elements(i) + "\"c
            Next

            root += "...\"

            For i As Integer = [end] To filenameIndex - 1
                root += elements(i) + "\"c
            Next

            Return root + elements(filenameIndex)
        End If
        Return pathname
    End Function

    Private Sub LoadRecentFiles()
        _RecentFiles = LoadRecentFilesCore()
    End Sub

    Private Function LoadRecentFilesCore() As List(Of RecentFile)
        Dim list As List(Of String) = RecentFiles

        Dim files As New List(Of RecentFile)(list.Count)

        Dim i As Integer = 0
        For Each filepath As String In list
            files.Add(New RecentFile(System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1), filepath))
        Next

        Return files
    End Function

    Private Class RecentFile
        Public Number As Integer = 0
        Public Filepath As String = ""
        Public MenuItem As MenuItem = Nothing

        Public ReadOnly Property DisplayPath() As String
            Get
                Return Path.Combine(Path.GetDirectoryName(Filepath), Path.GetFileNameWithoutExtension(Filepath))
            End Get
        End Property

        Public Sub New(number As Integer, filepath As String)
            Me.Number = number
            Me.Filepath = filepath
        End Sub
    End Class

    Public Class MenuClickEventArgs
        Inherits EventArgs
        Public Property Filepath() As String
            Get
                Return m_Filepath
            End Get
            Private Set(value As String)
                m_Filepath = value
            End Set
        End Property
        Private m_Filepath As String

        Public Sub New(filepath As String)
            Me.Filepath = filepath
        End Sub
    End Class

    Private Sub MenuItem_Click(sender As Object, e As EventArgs)
        Dim menuItem As MenuItem = TryCast(sender, MenuItem)

        OnMenuClick(menuItem)
    End Sub

    Protected Overridable Sub OnMenuClick(menuItem As MenuItem)
        Dim filepath As String = GetFilepath(menuItem)

        If [String].IsNullOrEmpty(filepath) Then
            Return
        End If

        RaiseEvent MenuClick(menuItem, New MenuClickEventArgs(filepath))
        'Dim dMenuClick As EventHandler(Of MenuClickEventArgs) = MenuClick
        'RaiseEvent dMenuClick(menuItem, New MenuClickEventArgs(filepath))
    End Sub

    Private Function GetFilepath(menuItem As MenuItem) As String
        For Each r As RecentFile In _RecentFiles
            If r.MenuItem.Equals(menuItem) Then
                Return r.Filepath
            End If
        Next

        Return [String].Empty
    End Function

    '-----------------------------------------------------------------------------------------

    Private NotInheritable Class ApplicationAttributes
        Private Sub New()
        End Sub
        Shared ReadOnly _Assembly As Assembly = Nothing

        Shared ReadOnly _Title As AssemblyTitleAttribute = Nothing
        Shared ReadOnly _Company As AssemblyCompanyAttribute = Nothing
        Shared ReadOnly _Copyright As AssemblyCopyrightAttribute = Nothing
        Shared ReadOnly _Product As AssemblyProductAttribute = Nothing

        Public Shared Property Title() As String
            Get
                Return m_Title
            End Get
            Private Set(value As String)
                m_Title = value
            End Set
        End Property
        Private Shared m_Title As String
        Public Shared Property CompanyName() As String
            Get
                Return m_CompanyName
            End Get
            Private Set(value As String)
                m_CompanyName = value
            End Set
        End Property
        Private Shared m_CompanyName As String
        Public Shared Property Copyright() As String
            Get
                Return m_Copyright
            End Get
            Private Set(value As String)
                m_Copyright = value
            End Set
        End Property
        Private Shared m_Copyright As String
        Public Shared Property ProductName() As String
            Get
                Return m_ProductName
            End Get
            Private Set(value As String)
                m_ProductName = value
            End Set
        End Property
        Private Shared m_ProductName As String

        Shared _Version As Version = Nothing
        Public Shared Property Version() As String
            Get
                Return m_Version
            End Get
            Private Set(value As String)
                m_Version = value
            End Set
        End Property
        Private Shared m_Version As String

        Shared Sub New()
            Try
                Title = [String].Empty
                CompanyName = [String].Empty
                Copyright = [String].Empty
                ProductName = [String].Empty
                Version = [String].Empty

                _Assembly = Assembly.GetEntryAssembly()

                If _Assembly IsNot Nothing Then
                    Dim attributes As Object() = _Assembly.GetCustomAttributes(False)

                    For Each attribute As Object In attributes
                        Dim type As Type = attribute.[GetType]()

                        If type = GetType(AssemblyTitleAttribute) Then
                            _Title = DirectCast(attribute, AssemblyTitleAttribute)
                        End If
                        If type = GetType(AssemblyCompanyAttribute) Then
                            _Company = DirectCast(attribute, AssemblyCompanyAttribute)
                        End If
                        If type = GetType(AssemblyCopyrightAttribute) Then
                            _Copyright = DirectCast(attribute, AssemblyCopyrightAttribute)
                        End If
                        If type = GetType(AssemblyProductAttribute) Then
                            _Product = DirectCast(attribute, AssemblyProductAttribute)
                        End If
                    Next

                    _Version = _Assembly.GetName().Version
                End If

                If _Title IsNot Nothing Then
                    Title = _Title.Title
                End If
                If _Company IsNot Nothing Then
                    CompanyName = _Company.Company
                End If
                If _Copyright IsNot Nothing Then
                    Copyright = _Copyright.Copyright
                End If
                If _Product IsNot Nothing Then
                    ProductName = _Product.Product
                End If
                If _Version IsNot Nothing Then
                    Version = _Version.ToString()
                End If
            Catch
            End Try
        End Sub
    End Class

    '-----------------------------------------------------------------------------------------

    Private Class RegistryPersister
        Implements IPersist
        Public Property RegistryKey() As String

        Public Sub New()
            'RegistryKey = (Convert.ToString((Convert.ToString("Software\") & ApplicationAttributes.CompanyName) + "\") & ApplicationAttributes.ProductName) + "\" + "RecentFileList"
            RegistryKey = "Software\HEC\" & ApplicationAttributes.ProductName & "\RecentFileList"
        End Sub

        Public Sub New(key As String)
            RegistryKey = key
        End Sub

        Private Function Key(i As Integer) As String
            Return i.ToString("00")
        End Function

        Public Function RecentFiles(max As Integer) As List(Of String) Implements IPersist.RecentFiles
            Dim k As RegistryKey = Registry.CurrentUser.OpenSubKey(RegistryKey)
            If k Is Nothing Then k = Registry.CurrentUser.CreateSubKey(RegistryKey)
            '
            Dim list As New List(Of String)(max)
            '
            For i As Integer = 0 To max - 1
                Dim filename As String = DirectCast(k.GetValue(Key(i)), String)
                If String.IsNullOrEmpty(filename) Then Exit For

                list.Add(filename)
            Next
            '
            Return list
        End Function

        Public Sub InsertFile(filepath As String, max As Integer) Implements IPersist.InsertFile
            Dim k As RegistryKey = Registry.CurrentUser.OpenSubKey(RegistryKey)
            If k Is Nothing Then Registry.CurrentUser.CreateSubKey(RegistryKey)
            '
            k = Registry.CurrentUser.OpenSubKey(RegistryKey, True)
            '
            RemoveFile(filepath, max)
            '
            For i As Integer = max - 2 To 0 Step -1
                Dim sThis As String = Key(i)
                Dim sNext As String = Key(i + 1)
                '
                Dim oThis As Object = k.GetValue(sThis)
                If oThis Is Nothing Then Continue For
                '
                k.SetValue(sNext, oThis)
            Next
            '
            k.SetValue(Key(0), filepath)
        End Sub

        Public Sub RemoveFile(filepath As String, max As Integer) Implements IPersist.RemoveFile
            Dim k As RegistryKey = Registry.CurrentUser.OpenSubKey(RegistryKey)
            If k Is Nothing Then
                Return
            End If

            For i As Integer = 0 To max - 1
again:
                Dim s As String = DirectCast(k.GetValue(Key(i)), String)
                If s IsNot Nothing AndAlso s.Equals(filepath, StringComparison.CurrentCultureIgnoreCase) Then
                    RemoveFile(i, max)
                    GoTo again
                End If
            Next
        End Sub

        Private Sub RemoveFile(index As Integer, max As Integer)
            Dim k As RegistryKey = Registry.CurrentUser.OpenSubKey(RegistryKey, True)
            If k Is Nothing Then Return

            k.DeleteValue(Key(index), False)

            For i As Integer = index To max - 2
                Dim sThis As String = Key(i)
                Dim sNext As String = Key(i + 1)

                Dim oNext As Object = k.GetValue(sNext)
                If oNext Is Nothing Then
                    Exit For
                End If

                k.SetValue(sThis, oNext)
                k.DeleteValue(sNext)
            Next
        End Sub
    End Class

    '-----------------------------------------------------------------------------------------
    'Keeping the following code for potential future implementations for an XML MRU (currently using registry)
    '
    'Private Class XmlPersister
    '    Implements IPersist
    '    Public Property Filepath() As String
    '        Get
    '            Return m_Filepath
    '        End Get
    '        Set(value As String)
    '            m_Filepath = value
    '        End Set
    '    End Property
    '    Private m_Filepath As String
    '    Public Property Stream() As Stream
    '        Get
    '            Return m_Stream
    '        End Get
    '        Set(value As Stream)
    '            m_Stream = value
    '        End Set
    '    End Property
    '    Private m_Stream As Stream

    '    Public Sub New()
    '        Filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), (Convert.ToString("HEC" & Convert.ToString("\")) & ApplicationAttributes.ProductName) + "\" + "RecentFileList.xml")
    '        'Filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), (Convert.ToString(ApplicationAttributes.CompanyName & Convert.ToString("\")) & ApplicationAttributes.ProductName) + "\" + "RecentFileList.xml")
    '    End Sub

    '    Public Sub New(filepath__1 As String)
    '        Filepath = filepath__1
    '    End Sub

    '    Public Sub New(stream__1 As Stream)
    '        Stream = stream__1
    '    End Sub

    '    Public Function RecentFiles(max As Integer) As List(Of String) Implements IPersist.RecentFiles
    '        Return Load(max)
    '    End Function

    '    Public Sub InsertFile(filepath As String, max As Integer) Implements IPersist.InsertFile
    '        Update(filepath, True, max)
    '    End Sub

    '    Public Sub RemoveFile(filepath As String, max As Integer) Implements IPersist.RemoveFile
    '        Update(filepath, False, max)
    '    End Sub

    '    Private Sub Update(filepath As String, insert As Boolean, max As Integer)
    '        Dim old As List(Of String) = Load(max)

    '        Dim list As New List(Of String)(old.Count + 1)

    '        If insert Then
    '            list.Add(filepath)
    '        End If

    '        CopyExcluding(old, filepath, list, max)

    '        Save(list, max)
    '    End Sub

    '    Private Sub CopyExcluding(source As List(Of String), exclude As String, target As List(Of String), max As Integer)
    '        For Each s As String In source
    '            If Not [String].IsNullOrEmpty(s) Then
    '                If Not s.Equals(exclude, StringComparison.OrdinalIgnoreCase) Then
    '                    If target.Count < max Then
    '                        target.Add(s)
    '                    End If
    '                End If
    '            End If
    '        Next
    '    End Sub

    '    Private Class SmartStream
    '        Implements IDisposable
    '        Private _IsStreamOwned As Boolean = True
    '        Private _Stream As Stream = Nothing

    '        Public ReadOnly Property Stream() As Stream
    '            Get
    '                Return _Stream
    '            End Get
    '        End Property

    '        Public Shared Widening Operator CType([me] As SmartStream) As Stream
    '            Return [me].Stream
    '        End Operator

    '        Public Sub New(filepath As String, mode As FileMode)
    '            _IsStreamOwned = True

    '            Directory.CreateDirectory(Path.GetDirectoryName(filepath))

    '            _Stream = File.Open(filepath, mode)
    '        End Sub

    '        Public Sub New(stream As Stream)
    '            _IsStreamOwned = False
    '            _Stream = stream
    '        End Sub

    '        Public Sub Dispose() Implements IDisposable.Dispose
    '            If _IsStreamOwned AndAlso _Stream IsNot Nothing Then
    '                _Stream.Dispose()
    '            End If

    '            _Stream = Nothing
    '        End Sub
    '    End Class

    '    Private Function OpenStream(mode As FileMode) As SmartStream
    '        If Not [String].IsNullOrEmpty(Filepath) Then
    '            Return New SmartStream(Filepath, mode)
    '        Else
    '            Return New SmartStream(Stream)
    '        End If
    '    End Function

    '    Private Function Load(max As Integer) As List(Of String)
    '        Dim list As New List(Of String)(max)

    '        Using ms As New MemoryStream()
    '            Using ss As SmartStream = OpenStream(FileMode.OpenOrCreate)
    '                If ss.Stream.Length = 0 Then
    '                    Return list
    '                End If

    '                ss.Stream.Position = 0

    '                Dim buffer As Byte() = New Byte((1 << 20) - 1) {}
    '                While True
    '                    Dim bytes As Integer = ss.Stream.Read(buffer, 0, buffer.Length)
    '                    If bytes = 0 Then
    '                        Exit While
    '                    End If
    '                    ms.Write(buffer, 0, bytes)
    '                End While

    '                ms.Position = 0
    '            End Using

    '            Dim x As XmlTextReader = Nothing

    '            Try
    '                x = New XmlTextReader(ms)

    '                While x.Read()
    '                    Select Case x.NodeType
    '                        Case XmlNodeType.XmlDeclaration, XmlNodeType.Whitespace
    '                            Exit Select

    '                        Case XmlNodeType.Element
    '                            Select Case x.Name
    '                                Case "RecentFiles"
    '                                    Exit Select

    '                                Case "RecentFile"
    '                                    If list.Count < max Then
    '                                        list.Add(x.GetAttribute(0))
    '                                    End If
    '                                    Exit Select
    '                                Case Else

    '                                    Debug.Assert(False)
    '                                    Exit Select
    '                            End Select
    '                            Exit Select

    '                        Case XmlNodeType.EndElement
    '                            Select Case x.Name
    '                                Case "RecentFiles"
    '                                    Return list
    '                                Case Else
    '                                    Debug.Assert(False)
    '                                    Exit Select
    '                            End Select
    '                            Exit Select
    '                        Case Else

    '                            Debug.Assert(False)
    '                            Exit Select
    '                    End Select
    '                End While
    '            Finally
    '                If x IsNot Nothing Then
    '                    x.Close()
    '                End If
    '            End Try
    '        End Using
    '        Return list
    '    End Function

    '    Private Sub Save(list As List(Of String), max As Integer)
    '        Using ms As New MemoryStream()
    '            Dim x As XmlTextWriter = Nothing

    '            Try
    '                x = New XmlTextWriter(ms, Encoding.UTF8)
    '                If x Is Nothing Then
    '                    Debug.Assert(False)
    '                    Return
    '                End If

    '                x.Formatting = Formatting.Indented

    '                x.WriteStartDocument()

    '                x.WriteStartElement("RecentFiles")

    '                For Each filepath As String In list
    '                    x.WriteStartElement("RecentFile")
    '                    x.WriteAttributeString("Filepath", filepath)
    '                    x.WriteEndElement()
    '                Next

    '                x.WriteEndElement()

    '                x.WriteEndDocument()

    '                x.Flush()

    '                Using ss As SmartStream = OpenStream(FileMode.Create)
    '                    ss.Stream.SetLength(0)

    '                    ms.Position = 0

    '                    Dim buffer As Byte() = New Byte((1 << 20) - 1) {}
    '                    While True
    '                        Dim bytes As Integer = ms.Read(buffer, 0, buffer.Length)
    '                        If bytes = 0 Then
    '                            Exit While
    '                        End If
    '                        ss.Stream.Write(buffer, 0, bytes)
    '                    End While
    '                End Using
    '            Finally
    '                If x IsNot Nothing Then
    '                    x.Close()
    '                End If
    '            End Try
    '        End Using
    '    End Sub
    'End Class

End Class
