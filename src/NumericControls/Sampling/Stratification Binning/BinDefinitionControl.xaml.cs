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

using Numerics.Sampling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NumericControls
{
    /// <summary>
    /// A control for selecting a collection of stratified x values primarily for binning purposes.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Woodrow Fields, USACE Risk Management Center, Woodrow.L.Fields@usace.army.mil
    /// </para>
    /// </remarks>
    public partial class BinDefinitionControl : UserControl
    {

        /// <summary>
        /// Dependency property for the array of stratification options.
        /// </summary>
        public static DependencyProperty StratificationOptionsCollectionProperty = DependencyProperty.Register(nameof(StratificationOptionsCollection), typeof(List<StratificationOptions>), typeof(BinDefinitionControl), new PropertyMetadata(null, SetStratificationOptionsCallback));

        /// <summary>
        /// Get and set the array of stratification options.
        /// </summary>
        public List<StratificationOptions> StratificationOptionsCollection
        {
            get { return (List<StratificationOptions>)GetValue(StratificationOptionsCollectionProperty); }
            set { SetValue(StratificationOptionsCollectionProperty, value); }
        }

        private static void SetStratificationOptionsCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(BinDefinitionControl)) return;
            BinDefinitionControl thisControl = (BinDefinitionControl)d;
            // 
            if (thisControl._settingSource) { return; }

            // Define the data
            foreach (var oldOptions in thisControl.StratificationOptionsRows)
            {
                ((StratificationOptionsRowItem)oldOptions).PropertyChanged -= thisControl.RItem_PropertyChanged;
            }
            thisControl.StratificationOptionsRows.Clear();

            //null checks
            if (e.NewValue == null) return;
            IList<StratificationOptions> newXValues = e.NewValue as IList<StratificationOptions>;
            if (newXValues == null) return;


            // Set data
            StratificationOptionsRowItem rowItem;
            foreach (StratificationOptions bin in newXValues)
            {
                rowItem = new StratificationOptionsRowItem(bin, thisControl.MaxBins, thisControl.StratificationOptionsRows);
                thisControl.StratificationOptionsRows.Add(rowItem);
            }
        }

        public static DependencyProperty ColumnHeaderStyleProperty = DependencyProperty.Register(nameof(ColumnHeaderStyle), typeof(Style), typeof(BinDefinitionControl), new FrameworkPropertyMetadata(null));

        public Style ColumnHeaderStyle
        {
            get { return (Style)GetValue(ColumnHeaderStyleProperty); }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }

        public static DependencyProperty CellStyleProperty = DependencyProperty.Register(nameof(CellStyle), typeof(Style), typeof(BinDefinitionControl), new FrameworkPropertyMetadata(null));

        public Style CellStyle
        {
            get { return (Style)GetValue(CellStyleProperty); }
            set { SetValue(CellStyleProperty, value); }
        }

        public static DependencyProperty MaxBinsProperty = DependencyProperty.Register(nameof(MaxBins), typeof(int), typeof(BinDefinitionControl), new FrameworkPropertyMetadata(1000));

        public int MaxBins
        {
            get { return (int)GetValue(MaxBinsProperty); }
            set { SetValue(MaxBinsProperty, value); }
        }

        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(BinDefinitionControl), new FrameworkPropertyMetadata(false));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static DependencyProperty StringFormatProperty = DependencyProperty.Register(nameof(StringFormat), typeof(string), typeof(BinDefinitionControl), new FrameworkPropertyMetadata("{0:#,##0.####}"));

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        public ObservableCollection<object> StratificationOptionsRows { get; private set; } = new ObservableCollection<object>();

        private bool _pastingData = false;
        private bool _settingSource = false;


        /// <summary>
        /// Dependency property for IsProbability.
        /// </summary>
        public static DependencyProperty IsProbabilityProperty = DependencyProperty.Register(nameof(IsProbability), typeof(bool), typeof(BinDefinitionControl), new PropertyMetadata(false, IsProbabilityChanged));

        /// <summary>
        /// When IsProbability is changed, update the row items in the validation data grid.
        /// </summary>
        private static void IsProbabilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(BinDefinitionControl)) return;
            BinDefinitionControl thisControl = (BinDefinitionControl)d;
            // 
            if (e.NewValue == null) return;
            bool newValue = (bool)e.NewValue;
            // Define the data
            foreach (StratificationOptionsRowItem r in thisControl.StratificationOptionsRows)
                r.IsProbability = newValue;
        }

        /// <summary>
        /// Determines whether or not the values in the bin definition control are probabilities.
        /// </summary>
        public bool IsProbability
        {
            get { return (bool)GetValue(IsProbabilityProperty); }
            set { SetValue(IsProbabilityProperty, value); }
        }

        /// <summary>
        /// Creates an empty bin definition control.
        /// </summary>
        public BinDefinitionControl()
        {

            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            StratificationOptionsRows.CollectionChanged += StratificationOptionsRows_CollectionChanged;

            ValidationGrid.ItemsSource = StratificationOptionsRows;
        }

        private void StratificationOptionsRows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var oldOptions in e.OldItems)
                {
                    ((StratificationOptionsRowItem)oldOptions).PropertyChanged -= RItem_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newOptions in e.NewItems)
                {
                    ((StratificationOptionsRowItem)newOptions).PropertyChanged += RItem_PropertyChanged;
                }
            }
        }

        private void DataGridColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Primitives.DataGridColumnHeader columnHeader = sender as System.Windows.Controls.Primitives.DataGridColumnHeader;
            if (columnHeader == null) return;
            // 
            ValidationGrid.SelectedCells.Clear();
            foreach (var item in ValidationGrid.Items)
            {
                ValidationGrid.SelectedCells.Add(new DataGridCellInfo(item, columnHeader.Column));
            }
        }

        private void ValidationGrid_RowsAdded(int startrow, int numrows)
        {
            UpdateSource();
        }

        private void ValidationGrid_RowsDeleted(List<int> rowindices)
        {
            UpdateSource();

        }

        private void ValidationGrid_PreviewPasteData(string[][] clipboardData, ref bool cancelPaste)
        {
            _pastingData = true;
        }

        private void ValidationGrid_DataPasted()
        {
            _pastingData = false;
            UpdateSource();
        }

        private void ValidationGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            foreach (var c in ValidationGrid.Columns)
            {
                if (c.HeaderStyle == null) c.HeaderStyle = (Style)FindResource("WrappedColumnHeaderStyle");
                ((DataGridTextColumn)c).Binding.StringFormat = StringFormat;
            }
        }

        /// <summary>
        /// Update the source dataset which triggers dependency property changed.
        /// </summary>
        private void UpdateSource()
        {
            if (_settingSource) { return; } else { _settingSource = true; }
            if (_pastingData) { return; }
            List<StratificationOptions> optionsToSet = new List<StratificationOptions>();

            foreach (object option in StratificationOptionsRows)
            {
                optionsToSet.Add(((StratificationOptionsRowItem)option).GetStratificationOptions());
            }

            StratificationOptionsCollection = optionsToSet;
            _settingSource = false;
        }

        private void RItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            StratificationOptionsRowItem rItem = (StratificationOptionsRowItem)sender;
            int dataIndex = StratificationOptionsRows.IndexOf(rItem);
            if (dataIndex < 0) { return; }

            UpdateSource();
        }

    }
}
