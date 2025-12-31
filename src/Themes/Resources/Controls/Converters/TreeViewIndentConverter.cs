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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Themes
{
    /// <summary>
    /// Converts a TreeViewItem to a Thickness representing the indentation
    /// based on the item's depth in the tree hierarchy.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This converter is used in TreeView control templates to calculate
    /// the left margin for tree items based on their nesting level.
    /// </para>
    /// <para>
    ///     <b>Authors:</b>
    ///     <list type="bullet">
    ///         <item>Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil</item>
    ///     </list>
    /// </para>
    /// </remarks>
    public class TreeViewIndentConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the indentation amount per level in pixels.
        /// </summary>
        /// <value>The indentation amount. Default is 0.</value>
        public double Indent { get; set; }

        /// <summary>
        /// Gets the depth of a TreeViewItem in the tree hierarchy.
        /// </summary>
        /// <param name="item">The TreeViewItem to get the depth for.</param>
        /// <returns>The zero-based depth of the item.</returns>
        public int GetItemDepth(TreeViewItem item)
        {
            var parent = GetParent(item);
            if (parent != null)
                return GetItemDepth(parent) + 1;
            return 0;
        }

        /// <summary>
        /// Gets the parent TreeViewItem of the specified item.
        /// </summary>
        /// <param name="item">The TreeViewItem to get the parent for.</param>
        /// <returns>The parent TreeViewItem, or null if the item is at the root level.</returns>
        private TreeViewItem GetParent(TreeViewItem item)
        {
            var parent = VisualTreeHelper.GetParent(item);
            while (parent != null && !(parent is TreeViewItem || parent is TreeView))
                parent = VisualTreeHelper.GetParent(parent);
            return parent as TreeViewItem;
        }

        /// <summary>
        /// Converts a TreeViewItem to a Thickness representing left indentation.
        /// </summary>
        /// <param name="value">The TreeViewItem to convert.</param>
        /// <param name="targetType">The target type (expected to be Thickness).</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A Thickness with left margin based on item depth, or zero thickness if value is null.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TreeViewItem item = value as TreeViewItem;
            if (item == null) return new Thickness(0d);
            return new Thickness(Indent * GetItemDepth(item), 0d, 0d, 0d);
        }

        /// <summary>
        /// Not implemented. This converter only supports one-way binding.
        /// </summary>
        /// <exception cref="NotImplementedException">Always thrown.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
