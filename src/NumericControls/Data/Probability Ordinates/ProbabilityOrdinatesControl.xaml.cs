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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// A user control that provides an interactive interface for managing probability ordinates in distribution fitting.
    /// This control displays a data grid where users can enter and modify exceedance probabilities for analysis.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class ProbabilityOrdinatesControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProbabilityOrdinatesControl"/> class.
        /// Sets up the control and subscribes to collection change events for probability row items.
        /// </summary>
        public ProbabilityOrdinatesControl()
        {
            InitializeComponent();
            _probabilityRowItems.CollectionChanged += ProbabilityRowItems_CollectionChanged;
        }

        /// <summary>
        /// Identifies the <see cref="ProbabilityOrdinates"/> dependency property.
        /// This property represents the probability ordinates to be displayed and edited.
        /// </summary>
        public static DependencyProperty ProbabilityOrdinatesProperty = DependencyProperty.Register(nameof(ProbabilityOrdinates), typeof(ProbabilityOrdinates), typeof(ProbabilityOrdinatesControl), new PropertyMetadata(null, ElementCallback));

        /// <summary>
        /// Gets or sets the probability ordinates for this control.
        /// </summary>
        /// <value>An instance of <see cref="Numerics.Data.ProbabilityOrdinates"/>.</value>
        public ProbabilityOrdinates ProbabilityOrdinates
        {
            get { return (ProbabilityOrdinates)GetValue(ProbabilityOrdinatesProperty); }
            set { SetValue(ProbabilityOrdinatesProperty, value); }
        }

        /// <summary>
        /// Callback method invoked when the <see cref="ProbabilityOrdinates"/> property changes.
        /// Handles cleanup of the old ordinates and initialization of the new ordinates, including event subscriptions and data grid population.
        /// </summary>
        /// <param name="d">The dependency object whose property changed.</param>
        /// <param name="e">Event arguments containing the old and new property values.</param>
        private static void ElementCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d as ProbabilityOrdinatesControl == null) return;
            var thisControl = (ProbabilityOrdinatesControl)d;

            // Remove old stuff
            // Clear old probability row items
            for (int i = thisControl._probabilityRowItems.Count - 1; i >= 0; i--)
            {
                thisControl._probabilityRowItems.RemoveAt(i);
            }
            // Clear data grid
            thisControl.DataGrid.ItemsSource = null;
            // Remove handlers
            var oldOrdinates = (ProbabilityOrdinates)e.OldValue;
            if (oldOrdinates != null)
                oldOrdinates.CollectionChanged -= thisControl.ProbabilityOrdinates_CollectionChanged;

            // Set up new stuff
            if (e.NewValue == null) return;
            var newOrdinates = e.NewValue as ProbabilityOrdinates;
            if (newOrdinates != null)
            {
                // Add row items
                foreach (double ordinate in newOrdinates)
                {
                    thisControl._probabilityRowItems.Add(new ProbabilityOrdinateRowItem(thisControl._probabilityRowItems, ordinate));
                }
                // Set data grid
                thisControl.DataGrid.ItemsSource = thisControl._probabilityRowItems;
                // Add handlers
                newOrdinates.CollectionChanged += thisControl.ProbabilityOrdinates_CollectionChanged;
            }

        }

        /// <summary>
        /// Flag to suppress UI updates when model changes are being propagated to prevent circular update loops.
        /// </summary>
        private bool _suppressUIUpdate = false;

        /// <summary>
        /// Flag to suppress model updates when UI changes are being propagated to prevent circular update loops.
        /// </summary>
        private bool _suppressModelUpdate = false;

        /// <summary>
        /// Observable collection of probability ordinate row items displayed in the data grid.
        /// </summary>
        private ObservableCollection<object> _probabilityRowItems = new ObservableCollection<object>();

        /// <summary>
        /// Handles the automatic column generation event for the data grid.
        /// Configures column styling, width, and tooltips for the probability column.
        /// </summary>
        /// <param name="sender">The data grid generating the column.</param>
        /// <param name="e">Event arguments containing the column being generated.</param>
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == nameof(ProbabilityOrdinateRowItem.Probability))
            {
                e.Column.MinWidth = 20;
                e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                ((DataGridTextColumn)e.Column).CellStyle = (Style)FindResource("Right_CellStyle");
                ((DataGridTextColumn)e.Column).HeaderStyle = (Style)FindResource("Center_ColumnHeaderStyle");

                // Add tool tip to column header style
                var headerStyle = new Style(typeof(DataGridColumnHeader));
                var currentStyle = ((DataGridTextColumn)e.Column).HeaderStyle;

                if (currentStyle != null)
                {
                    foreach (var setter in currentStyle.Setters)
                        headerStyle.Setters.Add(setter);
                }
                headerStyle.Setters.Add(new Setter(ToolTipProperty, new TextBlock()
                {
                    Text = "Enter the desired probabilities.",
                    FontWeight = FontWeights.Normal,
                    TextAlignment = TextAlignment.Left,
                    TextWrapping = TextWrapping.Wrap
                }));
                ((DataGridTextColumn)e.Column).HeaderStyle = headerStyle;
            }
        }

        /// <summary>
        /// Handles the addition of new rows to the data grid.
        /// Synchronizes the addition of new probability ordinates to the underlying model.
        /// </summary>
        /// <param name="startRowIndex">The index where the first new row was added.</param>
        /// <param name="nRows">The number of rows that were added.</param>
        private void DataGrid_RowsAdded(int startRowIndex, int nRows)
        {
            _suppressUIUpdate = true;
            for (int i = 0; i < nRows; i++)
            {
                ProbabilityOrdinates.Insert(startRowIndex + i, ((ProbabilityOrdinateRowItem)_probabilityRowItems[startRowIndex + i]).Probability);
            }
            _suppressUIUpdate = false;
        }

        /// <summary>
        /// Handles the deletion of rows from the data grid.
        /// Synchronizes the removal of probability ordinates from the underlying model.
        /// </summary>
        /// <param name="rowindices">The list of row indices that were deleted.</param>
        private void DataGrid_RowsDeleted(List<int> rowindices)
        {
            _suppressUIUpdate = true;
            for (int i = rowindices.Count - 1; i >= 0; i--)
            {
                ProbabilityOrdinates.RemoveAt(rowindices[i]);
            }
            _suppressUIUpdate = false;
        }

        /// <summary>
        /// Handles collection change events for the probability row items.
        /// Subscribes or unsubscribes property change handlers for items as they are added or removed.
        /// </summary>
        /// <param name="sender">The observable collection that changed.</param>
        /// <param name="e">Event arguments containing information about the collection change.</param>
        private void ProbabilityRowItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ProbabilityOrdinateRowItem item in e.NewItems)
                {
                    item.PropertyChanged += ProbabilityOrdinateRowItem_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ProbabilityOrdinateRowItem item in e.OldItems)
                {
                    item.PropertyChanged -= ProbabilityOrdinateRowItem_PropertyChanged;
                }
            }
        }

        /// <summary>
        /// Handles property change events for individual probability ordinate row items.
        /// Updates the underlying model when a probability value changes in the UI.
        /// </summary>
        /// <param name="sender">The probability ordinate row item that changed.</param>
        /// <param name="e">Event arguments containing the name of the property that changed.</param>
        private void ProbabilityOrdinateRowItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ProbabilityOrdinateRowItem.Probability)) return;
            if (_suppressModelUpdate) return;

            int rowIndex = _probabilityRowItems.IndexOf((ProbabilityOrdinateRowItem)sender);
            if (rowIndex >= 0 && rowIndex < ProbabilityOrdinates.Count)
            {
                _suppressUIUpdate = true;
                ProbabilityOrdinates[rowIndex] = ((ProbabilityOrdinateRowItem)sender).Probability;
                _suppressUIUpdate = false;
            }
        }

        /// <summary>
        /// Handles collection change events for the probability ordinates in the model.
        /// Synchronizes changes from the model to the UI by updating the row items collection.
        /// </summary>
        /// <param name="sender">The probability ordinates collection that changed.</param>
        /// <param name="e">Event arguments containing information about the collection change.</param>
        private void ProbabilityOrdinates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_suppressUIUpdate == false)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    for (int i = e.NewStartingIndex; i < e.NewItems.Count; i++)
                    {
                        _probabilityRowItems.Insert(i, new ProbabilityOrdinateRowItem(_probabilityRowItems, (double)e.NewItems[i]));
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    for (int i = e.OldItems.Count - 1; i >= e.OldStartingIndex; i--)
                    {
                        _probabilityRowItems.RemoveAt(i);
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    _suppressModelUpdate = true;
                    for (int i = e.NewStartingIndex; i < e.NewItems.Count; i++)
                    {
                        ((ProbabilityOrdinateRowItem)_probabilityRowItems[i]).Probability = (double)e.NewItems[i];
                    }
                    _suppressModelUpdate = false;
                }
            }
        }
    }
}
