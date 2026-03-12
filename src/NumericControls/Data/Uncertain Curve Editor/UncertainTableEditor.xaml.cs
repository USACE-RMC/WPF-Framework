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

using Numerics.Data;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace NumericControls
{
    /// <summary>
    /// A user control for editing uncertain ordered paired data with support for multiple distributions.
    /// Provides a combobox for distribution selection and integrates with data table editing functionality.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class UncertainTableEditor : UserControl
    {
        /// <summary>
        /// Identifies the <see cref="AddRemoveRows"/> dependency property.
        /// </summary>
        public static DependencyProperty AddRemoveRowsProperty = DependencyProperty.Register(nameof(AddRemoveRows), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether users can add or remove rows.
        /// </summary>
        public bool AddRemoveRows
        {
            get { return (bool)GetValue(AddRemoveRowsProperty); }
            set { SetValue(AddRemoveRowsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="XColumnHeader"/> dependency property.
        /// </summary>
        public static DependencyProperty XColumnHeaderProperty = DependencyProperty.Register(nameof(XColumnHeader), typeof(string), typeof(UncertainTableEditor), new FrameworkPropertyMetadata("X Data"));

        /// <summary>
        /// Gets or sets the column header text for the X (independent variable) column.
        /// </summary>
        public string XColumnHeader
        {
            get { return (string)GetValue(XColumnHeaderProperty); }
            set { SetValue(XColumnHeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="YColumnHeader"/> dependency property.
        /// </summary>
        public static DependencyProperty YColumnHeaderProperty = DependencyProperty.Register(nameof(YColumnHeader), typeof(string), typeof(UncertainTableEditor), new FrameworkPropertyMetadata("Y Data"));

        /// <summary>
        /// Gets or sets the column header text for the Y (distribution parameters) columns.
        /// </summary>
        public string YColumnHeader
        {
            get { return (string)GetValue(YColumnHeaderProperty); }
            set { SetValue(YColumnHeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsStrictX"/> dependency property.
        /// </summary>
        public static DependencyProperty IsStrictXProperty = DependencyProperty.Register(nameof(IsStrictX), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether X values must be strictly ordered (no duplicates allowed).
        /// </summary>
        public bool IsStrictX
        {
            get { return (bool)GetValue(IsStrictXProperty); }
            set { SetValue(IsStrictXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsStrictY"/> dependency property.
        /// </summary>
        public static DependencyProperty IsStrictYProperty = DependencyProperty.Register(nameof(IsStrictY), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether Y values must be strictly ordered (no duplicates allowed).
        /// </summary>
        public bool IsStrictY
        {
            get { return (bool)GetValue(IsStrictYProperty); }
            set { SetValue(IsStrictYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OrderX"/> dependency property.
        /// </summary>
        public static DependencyProperty OrderXProperty = DependencyProperty.Register(nameof(OrderX), typeof(SortOrder), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(SortOrder.Ascending));

        /// <summary>
        /// Gets or sets the required sort order for X values.
        /// </summary>
        public SortOrder OrderX
        {
            get { return (SortOrder)GetValue(OrderXProperty); }
            set { SetValue(OrderXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OrderY"/> dependency property.
        /// </summary>
        public static DependencyProperty OrderYProperty = DependencyProperty.Register(nameof(OrderY), typeof(SortOrder), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(SortOrder.Ascending));

        /// <summary>
        /// Gets or sets the required sort order for Y values.
        /// </summary>
        public SortOrder OrderY
        {
            get { return (SortOrder)GetValue(OrderYProperty); }
            set { SetValue(OrderYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MaximumX"/> dependency property.
        /// </summary>
        public static DependencyProperty MaximumXProperty = DependencyProperty.Register(nameof(MaximumX), typeof(double), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or sets the maximum allowed X value for validation.
        /// </summary>
        public double MaximumX
        {
            get { return (double)GetValue(MaximumXProperty); }
            set { SetValue(MaximumXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinimumX"/> dependency property.
        /// </summary>
        public static DependencyProperty MinimumXProperty = DependencyProperty.Register(nameof(MinimumX), typeof(double), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(double.MinValue));

        /// <summary>
        /// Gets or sets the minimum allowed X value for validation.
        /// </summary>
        public double MinimumX
        {
            get { return (double)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MaximumY"/> dependency property.
        /// </summary>
        public static DependencyProperty MaximumYProperty = DependencyProperty.Register(nameof(MaximumY), typeof(double), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or sets the maximum allowed Y value for validation.
        /// </summary>
        public double MaximumY
        {
            get { return (double)GetValue(MaximumYProperty); }
            set { SetValue(MaximumYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinimumY"/> dependency property.
        /// </summary>
        public static DependencyProperty MinimumYProperty = DependencyProperty.Register(nameof(MinimumY), typeof(double), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(double.MinValue));

        /// <summary>
        /// Gets or sets the minimum allowed Y value for validation.
        /// </summary>
        public double MinimumY
        {
            get { return (double)GetValue(MinimumYProperty); }
            set { SetValue(MinimumYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the control is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShowToolBar"/> dependency property.
        /// </summary>
        public static DependencyProperty ShowToolBarProperty = DependencyProperty.Register(nameof(ShowToolBar), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the toolbar is visible.
        /// </summary>
        public bool ShowToolBar
        {
            get { return (bool)GetValue(ShowToolBarProperty); }
            set { SetValue(ShowToolBarProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="XValuesShared"/> dependency property.
        /// </summary>
        public static DependencyProperty XValuesSharedProperty = DependencyProperty.Register(nameof(XValuesShared), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether X values are shared across distributions.
        /// </summary>
        public bool XValuesShared
        {
            get { return (bool)GetValue(XValuesSharedProperty); }
            set { SetValue(XValuesSharedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ColumnHeaderStyle"/> dependency property.
        /// </summary>
        public static DependencyProperty ColumnHeaderStyleProperty = DependencyProperty.Register(nameof(ColumnHeaderStyle), typeof(Style), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style for column headers in the data grid.
        /// </summary>
        public Style ColumnHeaderStyle
        {
            get { return (Style)GetValue(ColumnHeaderStyleProperty); }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CellStyle"/> dependency property.
        /// </summary>
        public static DependencyProperty CellStyleProperty = DependencyProperty.Register(nameof(CellStyle), typeof(Style), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style for cells in the data grid.
        /// </summary>
        public Style CellStyle
        {
            get { return (Style)GetValue(CellStyleProperty); }
            set { SetValue(CellStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedUncertainOrderedData"/> dependency property.
        /// </summary>
        public static DependencyProperty SelectedUncertainOrderedDataProperty = DependencyProperty.Register(nameof(SelectedUncertainOrderedData), typeof(UncertainOrderedPairedData), typeof(UncertainTableEditor), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the currently selected uncertain ordered paired data.
        /// </summary>
        public UncertainOrderedPairedData SelectedUncertainOrderedData
        {
            get { return (UncertainOrderedPairedData)GetValue(SelectedUncertainOrderedDataProperty); }
            set { SetValue(SelectedUncertainOrderedDataProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Distributions"/> dependency property.
        /// </summary>
        public static DependencyProperty DistributionsProperty = DependencyProperty.Register(nameof(Distributions), typeof(ObservableCollection<UncertainOrderedPairedData>), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the collection of available distribution options.
        /// </summary>
        public ObservableCollection<UncertainOrderedPairedData> Distributions
        {
            get { return (ObservableCollection<UncertainOrderedPairedData>)GetValue(DistributionsProperty); }
            set { SetValue(DistributionsProperty, value); }
        }

        /// <summary>
        /// Occurs when the data grid columns have been auto-generated.
        /// </summary>
        public event ColumnsAutoGeneratedEventHandler ColumnsAutoGenerated;

        /// <summary>
        /// Represents a method that handles the columns auto-generated event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The collection of auto-generated columns.</param>
        public delegate void ColumnsAutoGeneratedEventHandler(object sender, ObservableCollection<DataGridColumn> e);

        /// <summary>
        /// Initializes a new instance of the <see cref="UncertainTableEditor"/> class.
        /// </summary>
        public UncertainTableEditor()
        {

            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            var wrappedHeaderStyle = TryFindResource("WrappedColumnHeaderStyle") as Style;
            if (wrappedHeaderStyle != null) ColumnHeaderStyle = wrappedHeaderStyle;
            MyUncertainTableEditor.ValidationGrid.AutoGeneratedColumns += (object sender, EventArgs e) => ColumnsAutoGenerated?.Invoke(sender, MyUncertainTableEditor.ValidationGrid.Columns);
        }

        /// <summary>
        /// Handles selection changes in the curve uncertainty combobox.
        /// </summary>
        /// <param name="sender">The combobox that changed.</param>
        /// <param name="e">The selection changed event arguments.</param>
        private void CurveUncertaintyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurveUncertaintyComboBox.SelectedIndex == -1 && Distributions != null)
            {
                // Sometimes the combobox gets set to nothing even though the selecteduncertaindata is not nothing and the itemssource contains the selected item.
                int expectedIndex = Distributions.IndexOf(SelectedUncertainOrderedData);
                if (expectedIndex >= 0) CurveUncertaintyComboBox.SelectedIndex = expectedIndex;
            }
        }
    }
}
