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

using System.ComponentModel;
using System.Windows.Controls;

namespace GenericControls
{
    /// <summary>
    /// A user control for displaying property attributes such as display name and description.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
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
    public partial class PropertyAttributes:UserControl
    {

        #region Construction

        /// <summary>
        /// Initialize an empty control.
        /// </summary>
        public PropertyAttributes()
        {
            // This call is required by the designer.
            this.InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the default attribute description to display.
        /// </summary>
        /// <param name="displayName">The property display name.</param>
        /// <param name="description">The property description.</param>
        public void SetDefaultAttributes(string displayName, string description)
        {
            this.NameTextBlock.Text = displayName;
            this.Description.Text = description;
        }

        /// <summary>
        /// Gets the attributes for the specified class object.
        /// </summary>
        /// <param name="classObject">The class object.</param>
        public void GetClassAttributes(object classObject)
        {
            // Get the attributes for the class object.
            var attributes = TypeDescriptor.GetAttributes(classObject.GetType());
            // Update the name and description text boxes.
            this.NameTextBlock.Text = ((DisplayNameAttribute)attributes[typeof(DisplayNameAttribute)]).DisplayName.ToString();
            this.Description.Text = ((DescriptionAttribute)attributes[typeof(DescriptionAttribute)]).Description.ToString();
        }

        /// <summary>
        /// Gets the attributes for the property from a specified class object.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="classObject">The class object.</param>
        public void GetPropertyAttributes(string propertyName, object classObject)
        {
            // Get property descriptors for the class.
            var propertyDescriptors = TypeDescriptor.GetProperties(classObject.GetType());
            // Get the attributes for property.
            var attributes = propertyDescriptors[propertyName].Attributes;
            // Update the name and description text boxes.
            this.NameTextBlock.Text = ((DisplayNameAttribute)attributes[typeof(DisplayNameAttribute)]).DisplayName.ToString();
            this.Description.Text = ((DescriptionAttribute)attributes[typeof(DescriptionAttribute)]).Description.ToString();
        }

        #endregion

    }
}