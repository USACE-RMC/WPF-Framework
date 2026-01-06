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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Numerics.Data;

namespace NumericControls
{
    /// <summary>
    /// A user control for editing ordered paired data in a table format.
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
    public partial class OrderedDataTableEditor : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the OrderedDataTableEditor class.
        /// </summary>
        public OrderedDataTableEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dependency property for the OrderedData property.
        /// </summary>
        public static DependencyProperty OrderedDataProperty = DependencyProperty.Register(nameof(OrderedData), typeof(OrderedPairedData), typeof(OrderedDataTableEditor), new PropertyMetadata(new OrderedPairedData(false, SortOrder.Ascending, false, SortOrder.Ascending), SetData));

        private static void SetData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(OrderedDataTableEditor)) return;
            OrderedDataTableEditor thisControl = (OrderedDataTableEditor)d;
            if (e.NewValue == null)
            {
                thisControl.CurveRows.Clear();
                return;
            }
            //
            OrderedPairedData newCurve = e.NewValue as OrderedPairedData;
            if (newCurve == null)
            {
                thisControl.CurveRows.Clear();
                return;
            }
            // Define the data
            thisControl.CurveRows.Clear();
            //
            OrdinateRowItem rowItem;
            foreach (Ordinate o in newCurve)
            {
                rowItem = new OrdinateRowItem(o.X, o.Y, thisControl.XColumnHeader, thisControl.YColumnHeader, thisControl.CurveRows, thisControl.MinimumX, thisControl.MaximumX, thisControl.MinimumY, thisControl.MaximumY, newCurve.StrictX, newCurve.StrictY, newCurve.OrderX, newCurve.OrderY);
                rowItem.PropertyChanged += thisControl.RowItemPropertyChanged;
                thisControl.CurveRows.Add(rowItem);
                // .ValidationGrid.ItemsSource = .CurveRows
            }
        }

        private void RowItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OrdinateRowItem rItem = (OrdinateRowItem)sender;
            int dataIndex = CurveRows.IndexOf(rItem);
            OrderedData[dataIndex] = rItem.GetOrdinate();
        }

        /// <summary>
        /// Gets or sets the ordered paired data to be edited.
        /// </summary>
        public OrderedPairedData OrderedData
        {
            get { return (OrderedPairedData)GetValue(OrderedDataProperty); }
            set { SetValue(OrderedDataProperty, value); }
        }

        /// <summary>
        /// Dependency property for the MaximumX property.
        /// </summary>
        public static DependencyProperty MaximumXProperty = DependencyProperty.Register(nameof(MaximumX), typeof(double), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or sets the maximum allowed X value.
        /// </summary>
        public double MaximumX
        {
            get { return (double)GetValue(MaximumXProperty); }
            set { SetValue(MaximumXProperty, value); }
        }

        /// <summary>
        /// Dependency property for the MinimumX property.
        /// </summary>
        public static DependencyProperty MinimumXProperty = DependencyProperty.Register(nameof(MinimumX), typeof(double), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata(double.MinValue));

        /// <summary>
        /// Gets or sets the minimum allowed X value.
        /// </summary>
        public double MinimumX
        {
            get { return (double)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        /// <summary>
        /// Dependency property for the MaximumY property.
        /// </summary>
        public static DependencyProperty MaximumYProperty = DependencyProperty.Register(nameof(MaximumY), typeof(double), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or sets the maximum allowed Y value.
        /// </summary>
        public double MaximumY
        {
            get { return (double)GetValue(MaximumYProperty); }
            set { SetValue(MaximumYProperty, value); }
        }

        /// <summary>
        /// Dependency property for the MinimumY property.
        /// </summary>
        public static DependencyProperty MinimumYProperty = DependencyProperty.Register(nameof(MinimumY), typeof(double), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata(double.MinValue));

        /// <summary>
        /// Gets or sets the minimum allowed Y value.
        /// </summary>
        public double MinimumY
        {
            get { return (double)GetValue(MinimumYProperty); }
            set { SetValue(MinimumYProperty, value); }
        }

        /// <summary>
        /// Dependency property for the IsReadOnly property.
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the data grid is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Dependency property for the XColumnHeader property.
        /// </summary>
        public static DependencyProperty XColumnHeaderProperty = DependencyProperty.Register(nameof(XColumnHeader), typeof(string), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata("X Data"));

        /// <summary>
        /// Gets or sets the column header text for the X data column.
        /// </summary>
        public string XColumnHeader
        {
            get { return (string)GetValue(XColumnHeaderProperty); }
            set { SetValue(XColumnHeaderProperty, value); }
        }

        /// <summary>
        /// Dependency property for the YColumnHeader property.
        /// </summary>
        public static DependencyProperty YColumnHeaderProperty = DependencyProperty.Register(nameof(YColumnHeader), typeof(string), typeof(OrderedDataTableEditor), new FrameworkPropertyMetadata("Y Data"));

        /// <summary>
        /// Gets or sets the column header text for the Y data column.
        /// </summary>
        public string YColumnHeader
        {
            get { return (string)GetValue(YColumnHeaderProperty); }
            set { SetValue(YColumnHeaderProperty, value); }
        }

        /// <summary>
        /// Gets the collection of curve row items for the data grid.
        /// </summary>
        public ObservableCollection<object> CurveRows { get; private set; } = new ObservableCollection<object>();

        /// <summary>
        /// Updates the data grid by forcing validation on all rows.
        /// </summary>
        public void UpdateGrid()
        {
            foreach (var r in CurveRows)
                ((OrdinateRowItem)r).ForceValidation();
        }

        private void DataGridColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Primitives.DataGridColumnHeader columnHeader = sender as System.Windows.Controls.Primitives.DataGridColumnHeader;
            if (columnHeader == null) return;
            //
            ValidationGrid.SelectedCells.Clear();
            foreach (var item in ValidationGrid.Items)
                ValidationGrid.SelectedCells.Add(new DataGridCellInfo(item, columnHeader.Column));
        }

        private void ValidationGrid_RowsAdded(int startrow, int numrows)
        {
            for (int i = startrow; i < startrow + numrows; i++)
                OrderedData.Insert(i, ((OrdinateRowItem)CurveRows[i]).GetOrdinate());
            UpdateGrid();
        }

        private void ValidationGrid_RowsDeleted(List<int> rowindices)
        {
            // Refresh the source
            rowindices.Sort();
            for (int i = rowindices.Count - 1; i >= 0; i -= 1)
                OrderedData.RemoveAt(rowindices[i]);
            UpdateGrid();
        }

        private void ValidationGrid_DataPasted()
        {
            UpdateGrid();
        }

        private void ValidationGrid_PreviewAddRows(int startRowIndex, int nRows, ref bool cancelAddRows)
        {
            cancelAddRows = true;
            for (int i = startRowIndex; i < startRowIndex + nRows; i++)
            {
                var r = new OrdinateRowItem(0d, 0d, XColumnHeader, YColumnHeader, CurveRows, MinimumX, MaximumX, MinimumY, MaximumY, OrderedData.StrictX, OrderedData.StrictY, OrderedData.OrderX, OrderedData.OrderY);
                r.PropertyChanged += RowItemPropertyChanged;
                CurveRows.Insert(i, r);
            }
            ValidationGrid_RowsAdded(startRowIndex, nRows);
        }
    }
}
