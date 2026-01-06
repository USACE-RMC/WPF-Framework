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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace GenericControls
{
    /// <summary>
    /// A user control that allows the selection of predefined line styles (dash patterns).
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
    public partial class LineStyleSelectorControl:UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LineStyleSelectorControl"/> class.
        /// </summary>
        public LineStyleSelectorControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets a list of predefined dash patterns used as line styles.
        /// </summary>
        public static List<DoubleCollection> LineStyleOptions { get; private set; } = new List<DoubleCollection>(new[] { new DoubleCollection(new[] { 0d }), new DoubleCollection(), new DoubleCollection(new[] { 4d, 1d }), new DoubleCollection(new[] { 1d, 1d }), new DoubleCollection(new[] { 4d, 1d, 1d, 1d }), new DoubleCollection(new[] { 4d, 1d, 4d, 1d, 1d, 1d }), new DoubleCollection(new[] { 4d, 1d, 1d, 1d, 1d, 1d }), new DoubleCollection(new[] { 4d, 1d, 4d, 1d, 1d, 1d, 1d, 1d }), new DoubleCollection(new[] { 10d, 1d }), new DoubleCollection(new[] { 10d, 1d, 1d, 1d }), new DoubleCollection(new[] { 10d, 1d, 1d, 1d, 1d, 1d }) });

        /// <summary>
        /// Identifies the <see cref="SelectedDashArray"/> dependency property.
        /// </summary>
        public static DependencyProperty SelectedDashArrayProperty = DependencyProperty.Register(nameof(SelectedDashArray), typeof(DoubleCollection), typeof(LineStyleSelectorControl), new UIPropertyMetadata(new DoubleCollection()));
        public DoubleCollection SelectedDashArray
        {
            get
            {
                return (DoubleCollection)this.GetValue(SelectedDashArrayProperty);
            }
            set
            {
                this.SetValue(SelectedDashArrayProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="DashArrayOptions"/> dependency property.
        /// </summary>
        public static DependencyProperty DashArrayOptionsProperty = DependencyProperty.Register(nameof(DashArrayOptions), typeof(IList<DoubleCollection>), typeof(LineStyleSelectorControl), new FrameworkPropertyMetadata(LineStyleOptions));
        /// <summary>
        /// gets/sets the list of available dash array options.
        /// </summary>
        public IList<DoubleCollection> DashArrayOptions
        {
            get
            {
                return (IList<DoubleCollection>)this.GetValue(DashArrayOptionsProperty);
            }
            set
            {
                this.SetValue(DashArrayOptionsProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property.
        /// </summary>
        public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(LineStyleSelectorControl), new UIPropertyMetadata("Title"));
        /// <summary>
        /// gets/sets the display title of the control.
        /// </summary>
        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }
            set
            {
                this.SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="MaxPropertyWidth"/> dependency property.
        /// </summary>
        public static DependencyProperty MaxPropertyWidthProperty = DependencyProperty.Register(nameof(MaxPropertyWidth), typeof(double), typeof(LineStyleSelectorControl), new UIPropertyMetadata(PropertyDefaults.DefaultMaxPropertyWidth));
        /// <summary>
        /// gets/sets the maximum width of the control's label section.
        /// </summary>
        public double MaxPropertyWidth
        {
            get
            {
                return (double)this.GetValue(MaxPropertyWidthProperty);
            }
            set
            {
                this.SetValue(MaxPropertyWidthProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="MinPropertyWidth"/> dependency property.
        /// </summary>
        public static DependencyProperty MinPropertyWidthProperty = DependencyProperty.Register(nameof(MinPropertyWidth), typeof(double), typeof(LineStyleSelectorControl), new UIPropertyMetadata(PropertyDefaults.DefaultMinPropertyWidth));
        /// <summary>
        /// gets/sets the minimum width of the control's label section.
        /// </summary>
        public double MinPropertyWidth
        {
            get
            {
                return (double)this.GetValue(MinPropertyWidthProperty);
            }
            set
            {
                this.SetValue(MinPropertyWidthProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="PropertyWidth"/> dependency property.
        /// </summary>
        public static DependencyProperty PropertyWidthProperty = DependencyProperty.Register(nameof(PropertyWidth), typeof(GridLength), typeof(LineStyleSelectorControl), new UIPropertyMetadata(PropertyDefaults.DefaultPropertyWidth));
        /// <summary>
        /// gets/sets the width of the control's property column.
        /// </summary>
        public GridLength PropertyWidth
        {
            get
            {
                return (GridLength)this.GetValue(PropertyWidthProperty);
            }
            set
            {
                this.SetValue(PropertyWidthProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="ShowLeaderLine"/> dependency property.
        /// </summary>
        public static DependencyProperty ShowLeaderLineProperty = DependencyProperty.Register(nameof(ShowLeaderLine), typeof(bool), typeof(LineStyleSelectorControl), new UIPropertyMetadata(true));
        /// <summary>
        /// gets/sets a value indicating whether a leader line is shown next to the label.
        /// </summary>
        public bool ShowLeaderLine
        {
            get
            {
                return (bool)this.GetValue(ShowLeaderLineProperty);
            }
            set
            {
                this.SetValue(ShowLeaderLineProperty, value);
            }
        }

    }

    /// <summary>
    /// Converts between <see cref="DoubleCollection"/> objects for dash array selection.
    /// Used to ensure consistent object references when binding to <see cref="LineStyleSelectorControl.LineStyleOptions"/>
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
    public class DoubleCollectionConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="DoubleCollection"/> to a known reference in the predefined <see cref="LineStyleSelectorControl.LineStyleOptions'"/> list.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="targetType">The target type of the conversion.</param>
        /// <param name="parameter">Optional parameter.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The matching reference or blank; indicating no match.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var blank = LineStyleSelectorControl.LineStyleOptions[0];
            if (value == null)
                return blank;
            if (value.GetType() != typeof(DoubleCollection))
                return blank;
            // 
            DoubleCollection selectedStyle = (DoubleCollection)value;
            if (selectedStyle.Count == 0)
                return LineStyleSelectorControl.LineStyleOptions[1];
            foreach (var lineStyle in LineStyleSelectorControl.LineStyleOptions)
            {
                if (lineStyle is null)
                    continue;
                if (lineStyle.SequenceEqual(selectedStyle))
                    return lineStyle;
            }
            // 
            return blank;
        }

        /// <summary>
        /// Converts back from the selection to a new <see cref="DoubleCollection"/> object.
        /// </summary>
        /// <param name="value">The selected dash array.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Optional parameter.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A cloned <see cref="DoubleCollection"/> from the input, or a new one if invalid.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return new DoubleCollection();
            if (value.GetType() != typeof(DoubleCollection))
                return new DoubleCollection();
            return ((DoubleCollection)value).Clone();
        }
    }
}