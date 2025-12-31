/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this library.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;

namespace GenericControls
{

    /// <summary>
/// Folder browser dialog.
/// </summary>
/// <remarks>
/// <para>
///     Authors:
///     Josip Medved
///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil 
/// </para>
/// <para>
/// Versions:
///     <list type="bullet">
///         <item><description>
///         Originally created in 2-02-2012 by Josip Medved <see href = "https://www.medo64.com/2011/12/openfolderdialog/"/>.
///         </description></item>
///         <item><description>
///         July 2019 - Haden Smith made changes to this to make it compatible with WPF. Changed the name to "FolderBrowserDialog" and added Title property./>.
///         </description></item>
///         <item>
///         <description>
///         June 2020 - Haden added the ability to use Window rather than IWin32Window. 
///         If we need a better WPF solution we can use <see href = "https://github.com/McNeight/WpfFolderBrowser"/>.
///         </description>
///         </item>
///     </list>
/// </para>
/// </remarks>
    public class FolderBrowser
    {

        #region Members

        /// <summary>
    /// Gets/sets title of dialog. 
    /// </summary>
        public string Title { get; set; }

        /// <summary>
    /// Gets/sets folder in which dialog will be open.
    /// </summary>
        public string InitialDirectory { get; set; }

        /// <summary>
    /// Gets/sets directory in which dialog will be open if there is no recent directory available.
    /// </summary>
        public string DefaultDirectory { get; set; }

        /// <summary>
    /// Gets selected folder.
    /// </summary>
        public string SelectedFolder { get; set; }


        #region WPF32Window Wrapper Class

        /// <summary>
    /// Helper wrapper class to convert a WPF window to a IWin32Window.
    /// </summary>
        private class Wpf32Window : IWin32Window
        {
            public IntPtr Handle { get; private set; }
            public Wpf32Window(Window wpfWindow)
            {
                if (wpfWindow is null)
                    wpfWindow = GetDefaultOwnerWindow();
                Handle = new WindowInteropHelper(wpfWindow).Handle;
            }
            private Window GetDefaultOwnerWindow()
            {
                Window defaultWindow = null;
                if (Application.Current is not null && Application.Current.MainWindow is not null)
                {
                    defaultWindow = Application.Current.MainWindow;
                }
                return defaultWindow;
            }
        }

        #endregion

        #endregion

        /// <summary>
    /// Show folder browser dialog.
    /// </summary>
    /// <param name="owner">The owner of the dialog box.</param>
        public bool ShowDialog(Window owner = null)
        {
            var w32owner = new Wpf32Window(owner);
            if (Environment.OSVersion.Version.Major >= 6)
            {
                return ShowVistaDialog(w32owner);
            }
            else
            {
                return ShowLegacyDialog(w32owner);
            }
        }

        /// <summary>
    /// If OS is newer than Vista, then show dialog. 
    /// </summary>
    /// <param name="owner">The owner that opened the dialog as a IWin32Window.</param>
        private bool ShowVistaDialog(IWin32Window owner)
        {
            NativeMethods.IFileDialog frm = (NativeMethods.IFileDialog)new NativeMethods.FileOpenDialogRCW();
            var options = default(uint);
            frm.GetOptions(ref options);
            options = options | NativeMethods.FOS_PICKFOLDERS | NativeMethods.FOS_FORCEFILESYSTEM | NativeMethods.FOS_NOVALIDATE | NativeMethods.FOS_NOTESTFILECREATE | NativeMethods.FOS_DONTADDTORECENT;
            frm.SetOptions(options);
            if (Title is not null)
            {
                frm.SetTitle(Title);
            }
            if (InitialDirectory is not null)
            {
                var directoryShellItem = default(NativeMethods.IShellItem);
                var riid = new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE");
                // IShellItem
                if (NativeMethods.SHCreateItemFromParsingName(InitialDirectory, IntPtr.Zero, ref riid, ref directoryShellItem) == NativeMethods.S_OK)
                {
                    frm.SetFolder(directoryShellItem);
                }
            }
            if (DefaultDirectory is not null)
            {
                var directoryShellItem = default(NativeMethods.IShellItem);
                var riid = new Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE");
                // IShellItem
                if (NativeMethods.SHCreateItemFromParsingName(DefaultDirectory, IntPtr.Zero, ref riid, ref directoryShellItem) == NativeMethods.S_OK)
                {
                    frm.SetDefaultFolder(directoryShellItem);
                }
            }

            if (frm.Show(owner.Handle) == NativeMethods.S_OK)
            {
                var shellItem = default(NativeMethods.IShellItem);
                if (frm.GetResult(ref shellItem) == NativeMethods.S_OK)
                {
                    IntPtr pszString = IntPtr.Zero;
                    if (shellItem.GetDisplayName(NativeMethods.SIGDN_FILESYSPATH, ref pszString) == NativeMethods.S_OK)
                    {
                        if (pszString != IntPtr.Zero)
                        {
                            try
                            {
                                SelectedFolder = Marshal.PtrToStringAuto(pszString);
                                return true;
                            }
                            finally
                            {
                                Marshal.FreeCoTaskMem(pszString);
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
    /// If the user is calling this method from a version of Windows older than Vista, then show legacy dialog. 
    /// </summary>
    /// <param name="owner">The owner that opened the dialog as a IWin32Window.</param>
        private bool ShowLegacyDialog(IWin32Window owner)
        {
            var frm = new SaveFileDialog();
            frm.CheckFileExists = false;
            frm.CheckPathExists = true;
            frm.CreatePrompt = false;
            frm.Filter = "|" + Guid.Empty.ToString();
            frm.FileName = "any";
            if (InitialDirectory is not null)
            {
                frm.InitialDirectory = InitialDirectory;
            }
            frm.OverwritePrompt = false;
            frm.Title = "Select Folder";
            frm.ValidateNames = false;
            if (frm.ShowDialog((Window)owner) is { } arg1 && arg1 == true)
            {
                SelectedFolder = System.IO.Path.GetDirectoryName(frm.FileName);
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    #region Native Methods for Creating Dialog Window

    internal sealed class NativeMethods
    {
        private NativeMethods()
        {
        }

        #region Constants

        public const uint FOS_PICKFOLDERS = 32U;
        public const uint FOS_FORCEFILESYSTEM = 64U;
        public const uint FOS_NOVALIDATE = 256U;
        public const uint FOS_NOTESTFILECREATE = 65536U;
        public const uint FOS_DONTADDTORECENT = 33554432U;
        public const uint S_OK = 0U;
        public const uint SIGDN_FILESYSPATH = 0x80058000U;

        #endregion

        #region COM

        [ComImport]
        [ClassInterface(ClassInterfaceType.None)]
        [TypeLibType(TypeLibTypeFlags.FCanCreate)]
        [Guid("DC1C5A9C-E88A-4DDE-A5A1-60F82A20AEF7")]
        internal class FileOpenDialogRCW
        {
        }

        [ComImport]
        [Guid("42F85136-DB7E-439C-85F1-E4075D135FC8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IFileDialog
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            [PreserveSig]
            uint Show([In][Optional] IntPtr hwndOwner);
            // IModalWindow 

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileTypes([In] uint cFileTypes, [In][MarshalAs(UnmanagedType.LPArray)] IntPtr rgFilterSpec);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileTypeIndex([In] uint iFileType);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetFileTypeIndex(ref uint piFileType);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Advise([In][MarshalAs(UnmanagedType.Interface)] IntPtr pfde, ref uint pdwCookie);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Unadvise([In] uint dwCookie);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetOptions([In] uint fos);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetOptions(ref uint fos);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            void SetDefaultFolder([In][MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFolder([In][MarshalAs(UnmanagedType.Interface)] IShellItem psi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetFolder([MarshalAs(UnmanagedType.Interface)] ref IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] ref IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileName([In][MarshalAs(UnmanagedType.LPWStr)] string pszName);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetFileName([MarshalAs(UnmanagedType.LPWStr)] ref string pszName);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetTitle([In][MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetOkButtonLabel([In][MarshalAs(UnmanagedType.LPWStr)] string pszText);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFileNameLabel([In][MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetResult([MarshalAs(UnmanagedType.Interface)] ref IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint AddPlace([In][MarshalAs(UnmanagedType.Interface)] IShellItem psi, uint fdap);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetDefaultExtension([In][MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Close([MarshalAs(UnmanagedType.Error)] uint hr);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetClientGuid([In] ref Guid guid);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint ClearClientData();

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
        }

        [ComImport]
        [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IShellItem
        {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint BindToHandler([In] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IntPtr ppvOut);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetParent([MarshalAs(UnmanagedType.Interface)] ref IShellItem ppsi);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetDisplayName([In] uint sigdnName, ref IntPtr ppszName);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint GetAttributes([In] uint sfgaoMask, ref uint psfgaoAttribs);

            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            uint Compare([In][MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, ref int piOrder);
        }

        #endregion

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] ref IShellItem ppv);

    }
}

#endregion
