using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;


namespace FrameworkUI
{
    /// <summary>
    /// Utility functions. 
    /// </summary>
    public static class UtilityFunctions
    {

        public static byte[] UTF8StringToBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string UTF8BytesToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Function used to convert an integer to a color.
        /// </summary>
        /// <param name="ARGB">Alpha, red, green, blue as integer.</param>
        public static System.Drawing.Color IntegerToColor(ref int ARGB)
        {
            var Bytes = BitConverter.GetBytes(ARGB);
            byte Alpha = Bytes[3];
            byte Red = Bytes[2];
            byte Green = Bytes[1];
            byte Blue = Bytes[0];
            return System.Drawing.Color.FromArgb(Alpha, Red, Green, Blue);
        }

        /// <summary>
        /// Gets the free space available on the drive.
        /// </summary>
        /// <param name="driveName">Drive name.</param>
        public static long GetAvailableDriveSpace(string driveName)
        {
            var allDrives = DriveInfo.GetDrives();
            var freeSpace = default(long);
            foreach (var dInfo in allDrives)
            {
                if (dInfo.Name == driveName)
                {
                    freeSpace = dInfo.AvailableFreeSpace;
                    break;
                }
            }
            return freeSpace;
        }

        /// <summary>
        /// Shortens a pathname for display purposes.
        /// </summary>
        /// <param name="pathname">The pathname to shorten.</param>
        /// <param name="maxLength">The maximum number of characters to be displayed.</param>
        /// <remarks>
        /// Shortens a pathname by either removing consecutive components of a path
        /// and/or by removing characters from the end of the filename and replacing
        /// then with three ellipses (...)
        /// <para>
        /// In all cases, the root of the passed path will be preserved in it's entirety.
        /// </para>
        /// <para>
        /// If a UNC path is used or the pathname and maxLength are particularly short,
        /// the resulting path may be longer than maxLength.
        /// </para>
        /// <para>
        /// This method expects fully resolved pathnames to be passed to it.
        /// (Use Path.GetFullPath() to obtain this.)
        /// </para>
        /// <para>
        /// This method is taken from Joe Woodbury's article at: http://www.codeproject.com/KB/cs/mrutoolstripmenu.aspx
        /// </para>
        /// </remarks>
        /// <returns></returns>
        public static string ShortenPathname(string pathname, int maxLength)
        {
            if (pathname.Length <= maxLength)
            {
                return pathname;
            }

            string root = Path.GetPathRoot(pathname);
            if (root.Length > 3)
            {
                root += Path.DirectorySeparatorChar.ToString();
            }

            var elements = pathname.Substring(root.Length).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            int filenameIndex = elements.GetLength(0) - 1;
            if (elements.GetLength(0) == 1)
            {
                // pathname is just a root and filename
                if (elements[0].Length > 5)
                {
                    // long enough to shorten
                    // if path is a UNC path, root may be rather long
                    if (root.Length + 6 >= maxLength)
                    {
                        return root + elements[0].Substring(0, 3) + "...";
                    }
                    else
                    {
                        return pathname.Substring(0, maxLength - 3) + "...";
                    }
                }
            }
            else if (root.Length + 4 + elements[filenameIndex].Length > maxLength)
            {
                // pathname is just a root and filename
                root += @"...\";
                int len = elements[filenameIndex].Length;
                if (len < 6)
                {
                    return root + elements[filenameIndex];
                }

                if (root.Length + 6 >= maxLength)
                {
                    len = 3;
                }
                else
                {
                    len = maxLength - root.Length - 3;
                }

                return root + elements[filenameIndex].Substring(0, len) + "...";
            }
            else if (elements.GetLength(0) == 2)
            {
                return root + Convert.ToString(@"...\") + elements[1];
            }
            else
            {
                int len = 0;
                int begin = 0;
                for (int i = 0; i < filenameIndex; i++)
                {
                    if (elements[i].Length > len)
                    {
                        begin = i;
                        len = elements[i].Length;
                    }
                }

                int totalLength = pathname.Length - len + 3;
                int end = begin + 1;
                while (totalLength > maxLength)
                {
                    if (begin > 0)
                    {
                        totalLength -= elements[System.Threading.Interlocked.Decrement(ref begin)].Length - 1;
                    }

                    if (totalLength <= maxLength)
                    {
                        break;
                    }

                    if (end < filenameIndex)
                    {
                        totalLength -= elements[System.Threading.Interlocked.Increment(ref end)].Length - 1;
                    }

                    if (begin == 0 && end == filenameIndex)
                    {
                        break;
                    }
                }

                // assemble final string

                for (int i = 0; i < begin; i++)
                    root += elements[i] + '\\';
                root += @"...\";
                for (int i = end; i < filenameIndex; i++)
                    root += elements[i] + '\\';
                return root + elements[filenameIndex];
            }

            return pathname;
        }

        /// <summary>
        /// This contains a counter to help make names unique
        /// </summary>
        private static int menuNameCounter = 0;


        public static MenuItem Clone(this MenuItem sourceItem)
        {
            MenuItem copyItem = new MenuItem();

            var propInfoList = from p in typeof(MenuItem).GetProperties()
                               let attributes = p.GetCustomAttributes(true)
                               let notBrowseable = (from a in attributes where a.GetType() == typeof(BrowsableAttribute) select !(a as BrowsableAttribute).Browsable).FirstOrDefault()
                               where !notBrowseable && p.CanRead && p.CanWrite && p.Name != "Icon"
                               orderby p.Name
                               select p;

            // Copy over using reflections
            foreach (var propertyInfo in propInfoList)
            {
                object propertyInfoValue = propertyInfo.GetValue(sourceItem, null);
                propertyInfo.SetValue(copyItem, propertyInfoValue, null);
            }

            // Create a new menu name
            copyItem.Name = sourceItem.Name + "_" + menuNameCounter++;
            copyItem.IsHitTestVisible = true;

            // Recursively clone the sub items list
            foreach (var itm in sourceItem.Items)
            {
                MenuItem newItem;
                if (itm is MenuItem mItem)
                {
                    newItem = Clone(mItem);
                    copyItem.Items.Add(newItem);
                }
                else if (itm is Separator)
                {
                    copyItem.Items.Add(new Separator());
                }
                else
                {
                    throw new NotImplementedException("Menu item is not a MenuItem or a Separator");
                }
            }

            copyItem.CopyClick(sourceItem);

            // set the icon
            if (!(sourceItem.Icon is Image img)) { return copyItem; }
            if (img.Source is InteropBitmap ibmp) { copyItem.Icon = new Image() { Source = ibmp.Clone() }; }
            if (img.Source is BitmapImage bmp) { copyItem.Icon = new Image() { Source = bmp.Clone() }; }

            return copyItem;
        }

        /// <summary>
        /// Adds the handlers from the source component to the destination component
        /// </summary>
        /// <typeparam name="T">An IComponent type</typeparam>
        /// <param name="destinationComponent">The destination component.</param>
        /// <param name="sourceComponent">The source component.</param>
        public static void CopyClick(this MenuItem destinationComponent, MenuItem sourceComponent)
        {
            var events = sourceComponent.GetType().GetEvents().Where(x => x.Name == "Click");

            foreach (var theEvent in events)
            {
                var fieldInfo = sourceComponent.GetType().GetField($"{theEvent.Name}Event");
                RoutedEvent eventKind = (RoutedEvent)fieldInfo?.GetValue(sourceComponent);
                if (eventKind is null) { continue; }

                var reh = GetRoutedEventHandlers(sourceComponent, eventKind);

                foreach (Delegate re in reh)
                {
                    destinationComponent.Click += (RoutedEventHandler)re;
                }
            }
        }

        public static Delegate[] GetRoutedEventHandlers(UIElement element, RoutedEvent routedEvent)
        {
            if (element == null || routedEvent == null) { throw new ArgumentNullException(); }

            // Access the EventHandlersStore (internal class)
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty("EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            var eventHandlersStore = eventHandlersStoreProperty?.GetValue(element);

            if (eventHandlersStore == null) { return null; }

            // Get the GetRoutedEventHandlers method
            var getRoutedEventHandlersMethod = eventHandlersStore.GetType().GetMethod("GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public);
            var handlers = (RoutedEventHandlerInfo[])getRoutedEventHandlersMethod?.Invoke(eventHandlersStore, new object[] { routedEvent });

            if (handlers == null) { return null; }

            // Extract the delegates
            var delegates = new Delegate[handlers.Length];
            for (int i = 0; i < handlers.Length; i++)
            {
                delegates[i] = handlers[i].Handler;
            }

            return delegates;
        }


    }
}
