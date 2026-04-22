/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
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

using System.Windows;
using System.Windows.Controls;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for FileManagementOptions.xaml providing file management settings configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class FileManagementOptions : UserControl
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FileManagementOptions"/> class.
        /// </summary>
        public FileManagementOptions()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
        }

        /// <summary>
        /// Dependency property for the compress project file on close boolean.
        /// </summary>
        public static DependencyProperty CompressProjectFileOnCloseProperty = DependencyProperty.Register(nameof(CompressProjectFileOnClose), typeof(bool), typeof(FileManagementOptions), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether to compress the project file on close.
        /// </summary>
        public bool CompressProjectFileOnClose
        {
            get { return (bool)GetValue(CompressProjectFileOnCloseProperty); }
            set { SetValue(CompressProjectFileOnCloseProperty, value); }
        }

        /// <summary>
        /// Dependency property for the create auto-recover backup boolean.
        /// </summary>
        public static DependencyProperty CreateAutoRecoverBackupProperty = DependencyProperty.Register(nameof(CreateAutoRecoverBackup), typeof(bool), typeof(FileManagementOptions), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether to create an AutoRecover backup file.
        /// </summary>
        public bool CreateAutoRecoverBackup
        {
            get { return (bool)GetValue(CreateAutoRecoverBackupProperty); }
            set { SetValue(CreateAutoRecoverBackupProperty, value); }
        }

        /// <summary>
        /// Dependency property for the AutoRecover interval.
        /// </summary>
        public static DependencyProperty AutoRecoverIntervalProperty = DependencyProperty.Register(nameof(AutoRecoverInterval), typeof(int), typeof(FileManagementOptions), new UIPropertyMetadata(10));

        /// <summary>
        /// Gets or sets the AutoRecover interval in minutes.
        /// </summary>
        public int AutoRecoverInterval
        {
            get { return (int)GetValue(AutoRecoverIntervalProperty); }
            set { SetValue(AutoRecoverIntervalProperty, value); }
        }

        /// <summary>
        /// Dependency property for the keep last backup version on close boolean.
        /// </summary>
        public static DependencyProperty KeepLastBackupVersionProperty = DependencyProperty.Register(nameof(KeepLastBackupVersion), typeof(bool), typeof(FileManagementOptions), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether to keep the last backup version if the file were to unexpectedly close.
        /// </summary>
        public bool KeepLastBackupVersion
        {
            get { return (bool)GetValue(KeepLastBackupVersionProperty); }
            set { SetValue(KeepLastBackupVersionProperty, value); }
        }

    }
}
