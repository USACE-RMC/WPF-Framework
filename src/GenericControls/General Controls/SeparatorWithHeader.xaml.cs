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

namespace GenericControls
{
    /// <summary>
    /// A user control that displays a horizontal separator with a centered header text and 
    /// adjustable widths for the left and right separator lines.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class SeparatorWithHeader:UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SeparatorWithHeader"/> class.
        /// </summary>
        public SeparatorWithHeader()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dependency property for the header text.
        /// </summary>
        public static DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(string), typeof(SeparatorWithHeader), new UIPropertyMetadata(""));
        /// <summary>
        /// gets/sets the header text displayed between between the left and right separators.
        /// </summary>
        public string Header
        {
            get
            {
                return (this.GetValue(HeaderProperty)?.ToString());
            }
            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the width of the left separator 
        /// </summary>
        public static DependencyProperty LeftSeparatorWidthProperty = DependencyProperty.Register(nameof(LeftSeparatorWidth), typeof(GridLength), typeof(SeparatorWithHeader), new UIPropertyMetadata(new GridLength(0.5d, GridUnitType.Star)));
        /// <summary>
        /// gets/sets the width of the left separator line.
        /// </summary>
        public GridLength LeftSeparatorWidth
        {
            get
            {
                return (GridLength)this.GetValue(LeftSeparatorWidthProperty);
            }
            set
            {
                this.SetValue(LeftSeparatorWidthProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the width of the right separator
        /// </summary>
        public static DependencyProperty RightSeparatorWidthProperty = DependencyProperty.Register(nameof(RightSeparatorWidth), typeof(GridLength), typeof(SeparatorWithHeader), new UIPropertyMetadata(new GridLength(0.5d, GridUnitType.Star)));
        /// <summary>
        /// gets/sets the width of the right separator line.
        /// </summary>
        public GridLength RightSeparatorWidth
        {
            get
            {
                return (GridLength)this.GetValue(RightSeparatorWidthProperty);
            }
            set
            {
                this.SetValue(RightSeparatorWidthProperty, value);
            }
        }

    }
}