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

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Xml;

namespace FrameworkUI
{
    /// <summary>
    /// A class for displaying recent file items in a menu.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Woody Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class RecentFiles : Separator
    {
        /// <summary>
        /// Construct the RecentFiles class
        /// </summary>
        public RecentFiles()
        {
            Loaded += ConnectToMenu;
            MaxFilePathWidth = 50;
            MenuItemFormatOneToNine = "_{0}:  {1}";
            MenuItemFormatTenPlus = "{0}:  {1}";
        }

        /// <summary>
        /// Get the observable collection of recent files.
        /// </summary>
        public ObservableCollection<RecentFileItem> Collection { get; private set; } = new ObservableCollection<RecentFileItem>();

        /// <summary>
        /// Gets or sets the parent file menu.
        /// </summary>
        public MenuItem FileMenu { get; set; }

        /// <summary>
        /// Event raised when a recent file menu item is clicked.
        /// </summary>
        /// <param name="filePath">The full file path of the recent file item.</param>
        public event MenuClickEventHandler MenuClick;

        public delegate void MenuClickEventHandler(string filePath);

        /// <summary>
        /// Gets or sets the number of recent files to show in the menu.
        /// </summary>
        public int NumberOfFilesToDisplay { get; set; }

        /// <summary>
        /// Get or Set the width of the recent file item's text in the menu.
        /// Default: 35
        /// </summary>
        public int MaxFilePathWidth { get; set; }

        /// <summary>
        /// Gets or sets the XML file path for storing the recent file list.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Used in: String.Format( MenuItemFormat, index, file path, displayPath );
        /// Default = "_{0}:  {2}"
        /// </summary>
        public string MenuItemFormatOneToNine { get; set; }

        /// <summary>
        /// Used in: String.Format( MenuItemFormat, index, file path, displayPath );
        /// Default = "{0}:  {2}"
        /// </summary>
        public string MenuItemFormatTenPlus { get; set; }

        /// <summary>
        /// Separator used at bottom of recent file list.
        /// </summary>
        private Separator MenuSeparator = null;

        /// <summary>
        /// A menu item to display more recent files.
        /// </summary>
        private MenuItem MoreFilesMenuItem = null;

        /// <summary>
        /// Gets the index of the recent file item containing the specified file path.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        public int IndexOf(string filePath)
        {
            for (int i = 0; i < Collection.Count; i++)
                if (Collection[i].FilePath == filePath) return i;
            return -1;
        }

        /// <summary>
        /// Open recent file item.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        public void OpenItem(string filePath)
        {
            MenuClick?.Invoke(filePath);
        }

        /// <summary>
        /// Add a new recent file item.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        public void AddItem(string filePath)
        {
            int index = IndexOf(filePath);
            if (index >= 0) RemoveItem(index);
            var item = new RecentFileItem(filePath);
            Collection.Insert(0, item);
            AddToJumpList(filePath);
            SaveToXML();
        }

        /// <summary>
        /// Remove recent file item.
        /// </summary>
        /// <param name="index">Zero-based index of the file to remove.</param>
        public void RemoveItem(int index)
        {
            if (index >= 0)
            {
                if (FileMenu != null)
                {
                    FileMenu.Items.Remove(Collection[index].MenuItem);
                }
                Collection.RemoveAt(index);
                RemoveFromJumpList(index);
            }
        }

        /// <summary>
        /// Clear all recent file items.
        /// </summary>
        public void ClearAll()
        {
            for (int i = Collection.Count - 1; i >= 0; i -= 1)
                RemoveItem(i);
        }

        /// <summary>
        /// Add recent file to Microsoft Windows Jump List.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        public void AddToJumpList(string filePath)
        {
            var task = new JumpTask
            {
                ApplicationPath = Assembly.GetEntryAssembly().Location,
                Title = Path.GetFileNameWithoutExtension(filePath),
                Arguments = filePath,
                Description = filePath,
                CustomCategory = "Recent Files"
            };

            var jumpList = JumpList.GetJumpList(Application.Current);
            jumpList.JumpItems.Insert(0, task);
            if (jumpList.JumpItems.Count > NumberOfFilesToDisplay)
            {
                jumpList.JumpItems.RemoveAt(NumberOfFilesToDisplay);
            }

            jumpList.ShowRecentCategory = false;
            jumpList.Apply();
            JumpList.SetJumpList(Application.Current, jumpList);
        }

        /// <summary>
        /// Remove recent file from Microsoft Windows Jump List.
        /// </summary>
        /// <param name="index">Zero-based index of the file to remove.</param>
        public void RemoveFromJumpList(int index)
        {
            var jumpList = JumpList.GetJumpList(Application.Current);
            if (index > jumpList.JumpItems.Count - 1) return;
            jumpList.JumpItems.RemoveAt(index);
            jumpList.ShowRecentCategory = false;
            jumpList.Apply();
            JumpList.SetJumpList(Application.Current, jumpList);
        }

        /// <summary>
        /// Load the Microsoft Windows Jump List.
        /// </summary>
        public void LoadJumpList()
        {
            var jumpList = JumpList.GetJumpList(Application.Current);
            jumpList.JumpItems.Clear();
            jumpList.ShowRecentCategory = false;
            for (int i = 0; i < Collection.Count; i++)
            {
                if (i <= NumberOfFilesToDisplay - 1)
                {
                    var task = new JumpTask
                    {
                        ApplicationPath = Assembly.GetEntryAssembly().Location,
                        Title = Path.GetFileNameWithoutExtension(Collection[i].FilePath),
                        Arguments = Collection[i].FilePath,
                        Description = Collection[i].FilePath,
                        CustomCategory = "Recent Files"
                    };
                    jumpList.JumpItems.Add(task);
                }
            }

            jumpList.Apply();
            JumpList.SetJumpList(Application.Current, jumpList);
        }

        /// <summary>
        /// Load the recent files from XML.
        /// </summary>
        public void LoadFromXML()
        {
            // Check if the recent file list exists. 
            if (File.Exists(FilePath) == true)
            {
                // Clear collection
                ClearAll();
                // Load recent files from disk
                var xml = new XmlDocument();
                xml.Load(FilePath);
                var recentFilesNode = xml.GetElementsByTagName("RecentFiles").Item(0);
                if (recentFilesNode == null) return;

                foreach (XmlNode node in recentFilesNode.ChildNodes)
                {
                    // Check to see if the file still exists
                    var pathAttribute = node.Attributes?["Path"];
                    if (pathAttribute == null) continue;

                    string filePath = pathAttribute.Value;
                    if (File.Exists(filePath))
                    {
                        var item = new RecentFileItem(filePath);
                        Collection.Add(item);
                    }
                }
            }
            LoadJumpList();
        }

        /// <summary>
        /// Save the recent files to XML.
        /// </summary>
        public void SaveToXML()
        {
            // Create the recent files directory if it doesn't already exist.
            if (Directory.Exists(Path.GetDirectoryName(FilePath)) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            }
            // 
            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", null, null));
            var root = xml.CreateElement("RecentFiles");
            foreach (var fileItem in Collection)
            {
                var item = xml.CreateElement("Item");
                var attribute = xml.CreateAttribute("Path");
                attribute.Value = fileItem.FilePath;
                item.Attributes.Append(attribute);
                root.AppendChild(item);
            }
            xml.AppendChild(root);
            xml.Save(FilePath);
        }

        /// <summary>
        /// Connect to the parent file menu item.
        /// </summary>
        private void ConnectToMenu(object sender, RoutedEventArgs e)
        {
            MenuItem parentItem = Parent as MenuItem;
            if (parentItem == null) throw new ApplicationException("Parent must be a MenuItem");
            if (FileMenu != null && FileMenu.Equals(parentItem)) return;
            if (FileMenu != null) FileMenu.SubmenuOpened -= FileMenu_SubMenuOpened;
            FileMenu = parentItem;
            FileMenu.SubmenuOpened += FileMenu_SubMenuOpened;
        }

        /// <summary>
        /// When the parent file menu item is opened, build the list of recent files.
        /// </summary>
        private void FileMenu_SubMenuOpened(object sender, RoutedEventArgs e)
        {
            RemoveMenuItems();
            LoadMenuItems();
        }

        /// <summary>
        /// Remove all recent file menu items.
        /// </summary>
        private void RemoveMenuItems()
        {
            if (MenuSeparator != null) FileMenu.Items.Remove(MenuSeparator);
            // clear the list of menu items
            for (int i = Collection.Count - 1; i >= 0; i -= 1)
            {
                if (FileMenu.Items.Contains(Collection[i].MenuItem) == true)
                {
                    FileMenu.Items.Remove(Collection[i].MenuItem);
                }
            }
            if (MoreFilesMenuItem != null) FileMenu.Items.Remove(MoreFilesMenuItem);
            MenuSeparator = null;
            MoreFilesMenuItem = null;
        }

        /// <summary>
        /// Load all recent file menu items.
        /// </summary>
        private void LoadMenuItems()
        {
            int iMenuItem = FileMenu.Items.IndexOf(this);
            if (Collection.Count == 0) return;
            
            for (int i = 0; i < Collection.Count; i++)
            {
                if (i <= NumberOfFilesToDisplay - 1)
                {
                    string header = GetMenuItemText(i + 1, Collection[i].DisplayPath);
                    Collection[i].MenuItem = new MenuItem() { Header = header, ToolTip = new TextBlock() { Text = Collection[i].FilePath, TextWrapping = TextWrapping.Wrap } };
                    Collection[i].MenuItem.Click += MenuItem_Click;
                    // add menu item
                    iMenuItem += 1;
                    FileMenu.Items.Insert(iMenuItem, Collection[i].MenuItem);
                }
                else
                {
                    MoreFilesMenuItem = new MenuItem() { Header = "More Files..." };
                    MoreFilesMenuItem.Click += MoreFilesMenuItem_Click;
                    // add menu item
                    iMenuItem += 1;
                    FileMenu.Items.Insert(iMenuItem, MoreFilesMenuItem);
                    break;
                }
            }
            MenuSeparator = new Separator();
            FileMenu.Items.Insert(iMenuItem + 1, MenuSeparator);
            LoadJumpList();
        }

        /// <summary>
        /// Get the menu item header text.
        /// </summary>
        /// <param name="index">The 1-based index of the recent file item.</param>
        /// <param name="displaypath">Display path to the file.</param>
        private string GetMenuItemText(int index, string displaypath)
        {
            string format = index < 10 ? MenuItemFormatOneToNine : MenuItemFormatTenPlus;
            string shortPath = UtilityFunctions.ShortenPathname(displaypath, MaxFilePathWidth);
            return string.Format(format, index, shortPath);
        }

        /// <summary>
        /// On click, open recent file.
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            int iMenuItem = FileMenu.Items.IndexOf(this);
            int iSender = FileMenu.Items.IndexOf((MenuItem)sender);
            int iItem = iSender - iMenuItem - 1;
            OpenItem(Collection[iItem].FilePath);
        }

        /// <summary>
        /// When clicked, open the recent files dialog.
        /// </summary>
        private void MoreFilesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ImageSource image = null;
            if (Collection.Count > 0)
            {
                using (var sysicon = Icon.ExtractAssociatedIcon(Collection[0].FilePath))
                {
                    image = Imaging.CreateBitmapSourceFromHIcon(sysicon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
            }
            var recentFilesDialog = new RecentFilesDialog() { Files = this, FileImage = image, Icon = image };
            recentFilesDialog.ShowDialog();
        }

    }
}
