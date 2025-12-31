Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Windows.Interop
Imports Microsoft.Win32

''' <summary>
''' Folder browser dialog.
''' </summary>
''' <remarks>
''' <para>
'''     Authors:
'''     Josip Medved
'''     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil 
''' </para>
''' <para>
''' Versions:
'''     <list type="bullet">
'''         <item><description>
'''         Originally created in 2-02-2012 by Josip Medved <see href = "https://www.medo64.com/2011/12/openfolderdialog/"/>.
'''         </description></item>
'''         <item><description>
'''         July 2019 - Haden Smith made changes to this to make it compatible with WPF. Changed the name to "FolderBrowserDialog" and added Title property./>.
'''         </description></item>
'''         <item>
'''         <description>
'''         June 2020 - Haden added the ability to use Window rather than IWin32Window. 
'''         If we need a better WPF solution we can use <see href = "https://github.com/McNeight/WpfFolderBrowser"/>.
'''         </description>
'''         </item>
'''     </list>
''' </para>
''' </remarks>
Public Class FolderBrowser

#Region "Members"

    ''' <summary>
    ''' Gets/sets title of dialog. 
    ''' </summary>
    Public Property Title() As String

    ''' <summary>
    ''' Gets/sets folder in which dialog will be open.
    ''' </summary>
    Public Property InitialDirectory() As String

    ''' <summary>
    ''' Gets/sets directory in which dialog will be open if there is no recent directory available.
    ''' </summary>
    Public Property DefaultDirectory() As String

    ''' <summary>
    ''' Gets selected folder.
    ''' </summary>
    Public Property SelectedFolder() As String


#Region "WPF32Window Wrapper Class"

    ''' <summary>
    ''' Helper wrapper class to convert a WPF window to a IWin32Window.
    ''' </summary>
    Private Class Wpf32Window
        Implements IWin32Window
        Public ReadOnly Property Handle As IntPtr Implements IWin32Window.Handle
        Public Sub New(ByVal wpfWindow As Window)
            If wpfWindow Is Nothing Then wpfWindow = GetDefaultOwnerWindow()
            Handle = New WindowInteropHelper(wpfWindow).Handle
        End Sub
        Private Function GetDefaultOwnerWindow() As Window
            Dim defaultWindow As Window = Nothing
            If Application.Current IsNot Nothing AndAlso Application.Current.MainWindow IsNot Nothing Then
                defaultWindow = Application.Current.MainWindow
            End If
            Return defaultWindow
        End Function
    End Class

#End Region

#End Region

    ''' <summary>
    ''' Show folder browser dialog.
    ''' </summary>
    ''' <param name="owner">The owner of the dialog box.</param>
    Public Function ShowDialog(Optional owner As Window = Nothing) As Boolean
        Dim w32owner As New Wpf32Window(owner)
        If Environment.OSVersion.Version.Major >= 6 Then
            Return ShowVistaDialog(w32owner)
        Else
            Return ShowLegacyDialog(w32owner)
        End If
    End Function

    ''' <summary>
    ''' If OS is newer than Vista, then show dialog. 
    ''' </summary>
    ''' <param name="owner">The owner that opened the dialog as a IWin32Window.</param>
    Private Function ShowVistaDialog(owner As IWin32Window) As Boolean
        Dim frm = DirectCast(New NativeMethods.FileOpenDialogRCW(), NativeMethods.IFileDialog)
        Dim options As UInteger
        frm.GetOptions(options)
        options = options Or NativeMethods.FOS_PICKFOLDERS Or NativeMethods.FOS_FORCEFILESYSTEM Or NativeMethods.FOS_NOVALIDATE Or NativeMethods.FOS_NOTESTFILECREATE Or NativeMethods.FOS_DONTADDTORECENT
        frm.SetOptions(options)
        If Me.Title IsNot Nothing Then
            frm.SetTitle(Title)
        End If
        If Me.InitialDirectory IsNot Nothing Then
            Dim directoryShellItem As NativeMethods.IShellItem
            Dim riid = New Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")
            'IShellItem
            If NativeMethods.SHCreateItemFromParsingName(Me.InitialDirectory, IntPtr.Zero, riid, directoryShellItem) = NativeMethods.S_OK Then
                frm.SetFolder(directoryShellItem)
            End If
        End If
        If Me.DefaultDirectory IsNot Nothing Then
            Dim directoryShellItem As NativeMethods.IShellItem
            Dim riid = New Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")
            'IShellItem
            If NativeMethods.SHCreateItemFromParsingName(Me.DefaultDirectory, IntPtr.Zero, riid, directoryShellItem) = NativeMethods.S_OK Then
                frm.SetDefaultFolder(directoryShellItem)
            End If
        End If

        If frm.Show(owner.Handle) = NativeMethods.S_OK Then
            Dim shellItem As NativeMethods.IShellItem
            If frm.GetResult(shellItem) = NativeMethods.S_OK Then
                Dim pszString As IntPtr
                If shellItem.GetDisplayName(NativeMethods.SIGDN_FILESYSPATH, pszString) = NativeMethods.S_OK Then
                    If pszString <> IntPtr.Zero Then
                        Try
                            Me.SelectedFolder = Marshal.PtrToStringAuto(pszString)
                            Return True
                        Finally
                            Marshal.FreeCoTaskMem(pszString)
                        End Try
                    End If
                End If
            End If
        End If
        Return False
    End Function

    ''' <summary>
    ''' If the user is calling this method from a version of Windows older than Vista, then show legacy dialog. 
    ''' </summary>
    ''' <param name="owner">The owner that opened the dialog as a IWin32Window.</param>
    Private Function ShowLegacyDialog(owner As IWin32Window) As Boolean
        Dim frm = New SaveFileDialog()
        frm.CheckFileExists = False
        frm.CheckPathExists = True
        frm.CreatePrompt = False
        frm.Filter = "|" + Guid.Empty.ToString()
        frm.FileName = "any"
        If Me.InitialDirectory IsNot Nothing Then
            frm.InitialDirectory = Me.InitialDirectory
        End If
        frm.OverwritePrompt = False
        frm.Title = "Select Folder"
        frm.ValidateNames = False
        If frm.ShowDialog(CType(owner, Window)) = True Then
            Me.SelectedFolder = Path.GetDirectoryName(frm.FileName)
            Return True
        Else
            Return False
        End If
    End Function

End Class

#Region "Native Methods for Creating Dialog Window"

Friend NotInheritable Class NativeMethods
    Private Sub New()
    End Sub

#Region "Constants"

    Public Const FOS_PICKFOLDERS As UInteger = &H20
    Public Const FOS_FORCEFILESYSTEM As UInteger = &H40
    Public Const FOS_NOVALIDATE As UInteger = &H100
    Public Const FOS_NOTESTFILECREATE As UInteger = &H10000
    Public Const FOS_DONTADDTORECENT As UInteger = &H2000000
    Public Const S_OK As UInteger = &H0
    Public Const SIGDN_FILESYSPATH As UInteger = &H80058000UI

#End Region

#Region "COM"

    <ComImport, ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate), Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")>
    Friend Class FileOpenDialogRCW
    End Class

    <ComImport, Guid("42F85136-DB7E-439C-85F1-E4075D135FC8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IFileDialog
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        <PreserveSig>
        Function Show(<[In], [Optional]> hwndOwner As IntPtr) As UInteger
        'IModalWindow 

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function SetFileTypes(<[In]> cFileTypes As UInteger, <[In], MarshalAs(UnmanagedType.LPArray)> rgFilterSpec As IntPtr) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function SetFileTypeIndex(<[In]> iFileType As UInteger) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function GetFileTypeIndex(ByRef piFileType As UInteger) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function Advise(<[In], MarshalAs(UnmanagedType.[Interface])> pfde As IntPtr, ByRef pdwCookie As UInteger) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function Unadvise(<[In]> dwCookie As UInteger) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function SetOptions(<[In]> fos As UInteger) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function GetOptions(ByRef fos As UInteger) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Sub SetDefaultFolder(<[In], MarshalAs(UnmanagedType.[Interface])> psi As IShellItem)

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function SetFolder(<[In], MarshalAs(UnmanagedType.[Interface])> psi As IShellItem) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function GetFolder(<MarshalAs(UnmanagedType.[Interface])> ByRef ppsi As IShellItem) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function GetCurrentSelection(<MarshalAs(UnmanagedType.[Interface])> ByRef ppsi As IShellItem) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function SetFileName(<[In], MarshalAs(UnmanagedType.LPWStr)> pszName As String) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function GetFileName(<MarshalAs(UnmanagedType.LPWStr)> ByRef pszName As String) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function SetTitle(<[In], MarshalAs(UnmanagedType.LPWStr)> pszTitle As String) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function SetOkButtonLabel(<[In], MarshalAs(UnmanagedType.LPWStr)> pszText As String) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function SetFileNameLabel(<[In], MarshalAs(UnmanagedType.LPWStr)> pszLabel As String) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function GetResult(<MarshalAs(UnmanagedType.[Interface])> ByRef ppsi As IShellItem) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function AddPlace(<[In], MarshalAs(UnmanagedType.[Interface])> psi As IShellItem, fdap As UInteger) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function SetDefaultExtension(<[In], MarshalAs(UnmanagedType.LPWStr)> pszDefaultExtension As String) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function Close(<MarshalAs(UnmanagedType.[Error])> hr As UInteger) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function SetClientGuid(<[In]> ByRef guid As Guid) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function ClearClientData() As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function SetFilter(<MarshalAs(UnmanagedType.[Interface])> pFilter As IntPtr) As UInteger
    End Interface

    <ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
    Friend Interface IShellItem
        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function BindToHandler(<[In]> pbc As IntPtr, <[In]> ByRef rbhid As Guid, <[In]> ByRef riid As Guid, <Out, MarshalAs(UnmanagedType.[Interface])> ByRef ppvOut As IntPtr) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function GetParent(<MarshalAs(UnmanagedType.[Interface])> ByRef ppsi As IShellItem) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function GetDisplayName(<[In]> sigdnName As UInteger, ByRef ppszName As IntPtr) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function GetAttributes(<[In]> sfgaoMask As UInteger, ByRef psfgaoAttribs As UInteger) As UInteger

        <MethodImpl(MethodImplOptions.InternalCall, MethodCodeType:=MethodCodeType.Runtime)>
        Function Compare(<[In], MarshalAs(UnmanagedType.[Interface])> psi As IShellItem, <[In]> hint As UInteger, ByRef piOrder As Integer) As UInteger
    End Interface

#End Region

    <DllImport("shell32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
    Friend Shared Function SHCreateItemFromParsingName(<MarshalAs(UnmanagedType.LPWStr)> pszPath As String, pbc As IntPtr, ByRef riid As Guid, <MarshalAs(UnmanagedType.[Interface])> ByRef ppv As IShellItem) As Integer
    End Function

End Class

#End Region