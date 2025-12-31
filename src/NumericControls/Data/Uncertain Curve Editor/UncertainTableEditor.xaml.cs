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
using Numerics.Distributions;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NumericControls
{
    /// <summary>
    /// Interaction logic for UncertainTableEditor.xaml
    /// </summary>
    public partial class UncertainTableEditor : UserControl
    {
        public static DependencyProperty AddRemoveRowsProperty = DependencyProperty.Register(nameof(AddRemoveRows), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(true));

        public bool AddRemoveRows
        {
            get { return (bool)GetValue(AddRemoveRowsProperty); }
            set { SetValue(AddRemoveRowsProperty, value); }
        }

        public static DependencyProperty XColumnHeaderProperty = DependencyProperty.Register(nameof(XColumnHeader), typeof(string), typeof(UncertainTableEditor), new FrameworkPropertyMetadata("X Data"));

        public string XColumnHeader
        {
            get { return (string)GetValue(XColumnHeaderProperty); }
            set { SetValue(XColumnHeaderProperty, value); }
        }

        public static DependencyProperty YColumnHeaderProperty = DependencyProperty.Register(nameof(YColumnHeader), typeof(string), typeof(UncertainTableEditor), new FrameworkPropertyMetadata("Y Data"));

        public string YColumnHeader
        {
            get { return (string)GetValue(YColumnHeaderProperty); }
            set { SetValue(YColumnHeaderProperty, value); }
        }

        public static DependencyProperty IsStrictXProperty = DependencyProperty.Register(nameof(IsStrictX), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(false));

        public bool IsStrictX
        {
            get { return (bool)GetValue(IsStrictXProperty); }
            set { SetValue(IsStrictXProperty, value); }
        }

        public static DependencyProperty IsStrictYProperty = DependencyProperty.Register(nameof(IsStrictY), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(false));

        public bool IsStrictY
        {
            get { return (bool)GetValue(IsStrictYProperty); }
            set { SetValue(IsStrictYProperty, value); }
        }

        public static DependencyProperty OrderXProperty = DependencyProperty.Register(nameof(OrderX), typeof(SortOrder), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(SortOrder.Ascending));

        public SortOrder OrderX
        {
            get { return (SortOrder)GetValue(OrderXProperty); }
            set { SetValue(OrderXProperty, value); }
        }

        public static DependencyProperty OrderYProperty = DependencyProperty.Register(nameof(OrderY), typeof(SortOrder), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(SortOrder.Ascending));

        public SortOrder OrderY
        {
            get { return (SortOrder)GetValue(OrderYProperty); }
            set { SetValue(OrderYProperty, value); }
        }

        public static DependencyProperty MaximumXProperty = DependencyProperty.Register(nameof(MaximumX), typeof(double), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(double.MaxValue));

        public double MaximumX
        {
            get { return (double)GetValue(MaximumXProperty); }
            set { SetValue(MaximumXProperty, value); }
        }

        public static DependencyProperty MinimumXProperty = DependencyProperty.Register(nameof(MinimumX), typeof(double), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(double.MinValue));

        public double MinimumX
        {
            get { return (double)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        public static DependencyProperty MaximumYProperty = DependencyProperty.Register(nameof(MaximumY), typeof(double), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(double.MaxValue));

        public double MaximumY
        {
            get { return (double)GetValue(MaximumYProperty); }
            set { SetValue(MaximumYProperty, value); }
        }

        public static DependencyProperty MinimumYProperty = DependencyProperty.Register(nameof(MinimumY), typeof(double), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(double.MinValue));

        public double MinimumY
        {
            get { return (double)GetValue(MinimumYProperty); }
            set { SetValue(MinimumYProperty, value); }
        }

        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(false));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static DependencyProperty ShowToolBarProperty = DependencyProperty.Register(nameof(ShowToolBar), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(false));

        public bool ShowToolBar
        {
            get { return (bool)GetValue(ShowToolBarProperty); }
            set { SetValue(ShowToolBarProperty, value); }
        }

        public static DependencyProperty XValuesSharedProperty = DependencyProperty.Register(nameof(XValuesShared), typeof(bool), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(false));

        public bool XValuesShared
        {
            get { return (bool)GetValue(XValuesSharedProperty); }
            set { SetValue(XValuesSharedProperty, value); }
        }

        public static DependencyProperty ColumnHeaderStyleProperty = DependencyProperty.Register(nameof(ColumnHeaderStyle), typeof(Style), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(null));

        public Style ColumnHeaderStyle
        {
            get { return (Style)GetValue(ColumnHeaderStyleProperty); }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }

        public static DependencyProperty CellStyleProperty = DependencyProperty.Register(nameof(CellStyle), typeof(Style), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(null));

        public Style CellStyle
        {
            get { return (Style)GetValue(CellStyleProperty); }
            set { SetValue(CellStyleProperty, value); }
        }

        public static DependencyProperty SelectedUncertainOrderedDataProperty = DependencyProperty.Register(nameof(SelectedUncertainOrderedData), typeof(UncertainOrderedPairedData), typeof(UncertainTableEditor), new PropertyMetadata(null));

        /// <summary>
        /// Get and set the selected probability distribution.
        /// </summary>
        public UncertainOrderedPairedData SelectedUncertainOrderedData
        {
            get { return (UncertainOrderedPairedData)GetValue(SelectedUncertainOrderedDataProperty); }
            set { SetValue(SelectedUncertainOrderedDataProperty, value); }
        }

        public static DependencyProperty DistributionsProperty = DependencyProperty.Register(nameof(Distributions), typeof(ObservableCollection<UncertainOrderedPairedData>), typeof(UncertainTableEditor), new FrameworkPropertyMetadata(null));

        public ObservableCollection<UncertainOrderedPairedData> Distributions
        {
            get { return (ObservableCollection<UncertainOrderedPairedData>)GetValue(DistributionsProperty); }
            set { SetValue(DistributionsProperty, value); }
        }

        public event ColumnsAutoGeneratedEventHandler ColumnsAutoGenerated;

        public delegate void ColumnsAutoGeneratedEventHandler(object sender, ObservableCollection<DataGridColumn> e);

        public UncertainTableEditor()
        {

            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            ColumnHeaderStyle = (Style)FindResource("WrappedColumnHeaderStyle");

            // 
            MyUncertainTableEditor.ValidationGrid.AutoGeneratedColumns += (object sender, EventArgs e) => ColumnsAutoGenerated?.Invoke(sender, MyUncertainTableEditor.ValidationGrid.Columns);
        }

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
