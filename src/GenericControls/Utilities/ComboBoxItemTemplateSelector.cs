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
using System.Windows.Media;

namespace GenericControls
{
    /// <summary>
    /// A custom <see cref="DataTemplateSelector"/> that chooses between two templates based on 
    /// whether the ComboBox item is being rendered in the dropdown list or in the selected item display.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class ComboBoxItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// gets/sets the <see cref="DataTemplate"/> used to display items in the dropdown portion of the ComboBox.
        /// </summary>
        public DataTemplate DropDownTemplate { get; set; }

        /// <summary>
        /// gets/sets the <see cref="DataTemplate"/> used to display the selecte item in the ComboBox.
        /// </summary>
        public DataTemplate SelectedTemplate { get; set; }

        /// <summary>
        /// Returns a <see cref="DataTemplate"/> based on the container context-dropdown list or selected item.
        /// </summary>
        /// <param name="item">The data object.</param>
        /// <param name="container">The element in the visual tree that will contain the data template.</param>
        /// <returns></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var comboBoxItem = GetVisualParent<ComboBoxItem>(container);

            if (comboBoxItem is not null)
            {
                return DropDownTemplate;
            }

            return SelectedTemplate;
        }

        /// <summary>
        /// Traverses the visual tree upward to find the first ancestor of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of ancestor to find.</typeparam>
        /// <param name="childObject">The starting child object.</param>
        /// <returns></returns>
        public static T GetVisualParent<T>(object childObject) where T : Visual
        {
            DependencyObject child = childObject as DependencyObject;

            while (child is not null && !(child is T))
                child = VisualTreeHelper.GetParent(child);

            return child as T;
        }
    }
}