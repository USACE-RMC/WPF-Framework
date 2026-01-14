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
using System.ComponentModel;
using System.Windows;
using DatabaseManager;

namespace DatabaseControls
{
    /// <summary>
    /// A window that displays summary statistics for a selected column in the TableViewer.
    /// Automatically determines whether to show numeric or alphabetic statistics based on the column data type.
    /// Supports filtering statistics to selected rows only.
    /// </summary>
    public partial class ColumnStatsWindow : Window
    {
        #region Private Fields

        /// <summary>
        /// Reference to the parent TableViewer control containing the data.
        /// </summary>
        private readonly TableViewer _viewer;

        /// <summary>
        /// The zero-based index of the column to display statistics for.
        /// </summary>
        private readonly int _columnIndex;

        /// <summary>
        /// Indicates whether the column contains numeric data.
        /// </summary>
        private bool _isNumeric;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnStatsWindow"/> class with the specified table viewer and column index.
        /// </summary>
        /// <param name="theViewer">The TableViewer control containing the data to analyze.</param>
        /// <param name="columnIndex">The zero-based index of the column to display statistics for.</param>
        public ColumnStatsWindow(TableViewer theViewer, int columnIndex)
        {
            InitializeComponent();

            _columnIndex = columnIndex;
            _viewer = theViewer;

            if (_viewer == null)
                return;

            string fieldName = _viewer.DataView.ColumnNames[_columnIndex];
            Title = fieldName + " Summary Statistics";

            _isNumeric = DatabaseManager.DatabaseManager.IsNumericType(_viewer.DataView.ColumnTypes[_columnIndex]);

            if (_isNumeric)
            {
                NumericColumnViewer.Visibility = Visibility.Visible;
                NumericColumnViewer.DataName = fieldName;
                AlphabeticColumnViewer.Visibility = Visibility.Collapsed;
            }
            else
            {
                NumericColumnViewer.Visibility = Visibility.Collapsed;
                AlphabeticColumnViewer.Visibility = Visibility.Visible;
                AlphabeticColumnViewer.DataName = fieldName;
            }

            _viewer.SelectedRowIndicesChanged += ViewerSelectionChanged;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles changes to the selected row indices in the parent TableViewer.
        /// Enables or disables the "Selected Rows Only" checkbox based on whether rows are selected.
        /// </summary>
        /// <param name="selectedRowIndices">The list of currently selected row indices.</param>
        private void ViewerSelectionChanged(List<int> selectedRowIndices)
        {
            if (selectedRowIndices == null || selectedRowIndices.Count == 0)
            {
                SelectedOnlyCheckbox.IsEnabled = false;
                SelectedOnlyCheckbox.IsChecked = false;
            }
            else
            {
                SelectedOnlyCheckbox.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handles the ContentRendered event of the ColumnStatsWindow.
        /// Loads and displays the initial column statistics after the window has been rendered.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ColumnStatsWindow_ContentRendered(object sender, EventArgs e)
        {
            if (_viewer == null)
                return;

            if (_viewer.GetSelectedRows().Count > 0)
                SelectedOnlyCheckbox.IsEnabled = true;

            object[] fieldData = _viewer.DataView.GetColumn(_viewer.DataView.ColumnNames[_columnIndex]);

            if (_isNumeric)
            {
                NumericColumnViewer.Data = Array.ConvertAll(fieldData, o => Convert.IsDBNull(o) ? 0.0 : Convert.ToDouble(o));
            }
            else
            {
                AlphabeticColumnViewer.Data = fieldData;
            }
        }

        /// <summary>
        /// Handles the Closing event of the ColumnStatsWindow.
        /// Unsubscribes from the TableViewer's SelectedRowIndicesChanged event to prevent memory leaks.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void ColumnStatsWindow_Closing(object sender, CancelEventArgs e)
        {
            _viewer.SelectedRowIndicesChanged -= ViewerSelectionChanged;
        }

        /// <summary>
        /// Handles the Checked event of the SelectedOnlyCheckbox control.
        /// Filters the statistics to show only data from the selected rows.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SelectedOnlyCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            object[] fieldData;

            object[] tempData = _viewer.DataView.GetColumn(_viewer.DataView.ColumnNames[_columnIndex]);
            List<int> selectedRows = _viewer.GetSelectedRows();

            if (selectedRows.Count > 0)
            {
                List<object> realData = new List<object>();
                for (int i = 0; i < selectedRows.Count; i++)
                {
                    realData.Add(tempData[selectedRows[i]]);
                }
                fieldData = realData.ToArray();
            }
            else
            {
                fieldData = tempData;
            }

            if (_isNumeric)
            {
                NumericColumnViewer.Data = Array.ConvertAll(fieldData, o => Convert.IsDBNull(o) ? 0.0 : Convert.ToDouble(o));
            }
            else
            {
                AlphabeticColumnViewer.Data = fieldData;
            }
        }

        /// <summary>
        /// Handles the Unchecked event of the SelectedOnlyCheckbox control.
        /// Restores the statistics to show data from all rows in the column.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SelectedOnlyCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            object[] fieldData = _viewer.DataView.GetColumn(_viewer.DataView.ColumnNames[_columnIndex]);

            if (_isNumeric)
            {
                NumericColumnViewer.Data = Array.ConvertAll(fieldData, o => Convert.IsDBNull(o) ? 0.0 : Convert.ToDouble(o));
            }
            else
            {
                AlphabeticColumnViewer.Data = fieldData;
            }
        }

        #endregion
    }
}
