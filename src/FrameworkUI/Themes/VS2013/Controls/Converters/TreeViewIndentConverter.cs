using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FrameworkUI
{
    public class TreeViewIndentConverter : IValueConverter
    {
        public double Indent { get; set; }

        public int GetItemDepth(TreeViewItem item)
        {
            var parent = default(TreeViewItem);
            while (GetParent(item) != null)
                return GetItemDepth(parent) + 1;
            return 0;
        }

        private TreeViewItem GetParent(TreeViewItem item)
        {
            var parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
                parent = VisualTreeHelper.GetParent(parent);
            return parent as TreeViewItem;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TreeViewItem item = value as TreeViewItem;
            if (item == null) return new Thickness(0d);
            return new Thickness(Indent * GetItemDepth(item), 0d, 0d, 0d);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
