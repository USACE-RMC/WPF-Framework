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

using System.Collections.Generic;
using System.Linq;

namespace DatabaseManager
{
    /// <summary>
    /// Represents an edit to a single row in a table, storing the modified values and 
    /// supporting undo-like operations for column changes.
    /// </summary>
    public class RowEdit : TableEdit
    {

        private readonly List<object> _rowData;
        private readonly List<object> _removedValues = new List<object>();
        private int _rowIndex;

        /// <summary>
        /// Gets the index of the row being edited.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return _rowIndex;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowEdit"/> class with specified row data and row index.
        /// </summary>
        /// <param name="newRowData">An array of objects representing the edited values for the row.</param>
        /// <param name="indexOfRow">The index of the row being edited.</param>
        public RowEdit(object[] newRowData, int indexOfRow)
        {
            _rowData = newRowData.ToList();
            _rowIndex = indexOfRow;
        }

        /// <summary>
        /// Handles a column insertion by inserting a value into the row at the specified index.
        /// </summary>
        /// <typeparam name="T">The type of the inserted column data.</typeparam>
        /// <param name="indexOfColumn">The index at which the column was added.</param>
        /// <param name="columnData">Optional data for the added column. If null, previously removed data is used.</param>
        public override void ColumnAdded<T>(int indexOfColumn, T[] columnData = null)
        {
            if (columnData == null)
            {
                _rowData.Insert(indexOfColumn, _removedValues.Last());
                _removedValues.RemoveAt(_removedValues.Count - 1);
            }
            else
            {
                _rowData.Insert(indexOfColumn, columnData[_rowIndex]);
            }
        }

        /// <summary>
        /// Handles a column deletion by removing the value at the specified column index and
        /// storing it for possible restoration
        /// </summary>
        /// <param name="indexOfColumn">The index of the column being deleted.</param>
        public override void ColumnDeleted(int indexOfColumn)
        {
            _removedValues.Add(_rowData[indexOfColumn]);
            _rowData.RemoveAt(indexOfColumn);
        }

        /// <summary>
        /// Adjusts the row index when a new row is inserted above the current row.
        /// </summary>
        /// <param name="indexOfRow">The index at which the new row was added.</param>
        /// <param name="rowData">Optional data for the new row.</param>
        public override void RowAdded(int indexOfRow, object[] rowData = null)
        {
            if (_rowIndex > indexOfRow)
                _rowIndex += 1;
        }

        /// <summary>
        /// Adjusts the row index when a row is deleted above the current row.
        /// </summary>
        /// <param name="indexOfRow">The index of the row that was deleted.</param>
        public override void RowDeleted(int indexOfRow)
        {
            if (_rowIndex > indexOfRow)
                _rowIndex -= 1;
        }

        /// <summary>
        /// Determines whether this row edit affects the specified row.
        /// </summary>
        /// <param name="indexOfRow">The row index to check.</param>
        /// <returns><c>true</c> if this edit affects the specified row; otherwise, <c>false</c>.</returns>
        public override bool ContainsRow(int indexOfRow)
        {
            return _rowIndex == indexOfRow;
        }

        /// <summary>
        /// Determines whether the specified cell is included in this row edit and
        /// retrieves its value.
        /// </summary>
        /// <param name="indexOfColumn">The column index of the cell.</param>
        /// <param name="indexOfRow">The row index of the cell.</param>
        /// <param name="returnValue">The value of the cell if it exists.</param>
        /// <returns><c>true</c> if the cell is part of this row edit; otherwise, <c>false</c>.</returns>
        public override bool ContainsCell(int indexOfColumn, int indexOfRow, ref object returnValue)
        {
            if (_rowIndex == indexOfRow)
            {
                returnValue = _rowData[indexOfColumn];
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Indicates that this row edit conceptually affects all columns.
        /// </summary>
        /// <param name="indexOfColumn">The column index to check.</param>
        /// <returns><c>true</c> since row edits affect all columns.</returns>
        public override bool ContainsColumn(int indexOfColumn)
        {
            return true;
        }

        /// <summary>
        /// Returns the cell edit corresponding to a specific column in this row.
        /// </summary>
        /// <param name="indexOfColumn">The column index to retrieve.</param>
        /// <returns>A list containing the cell edit for this column, or empty if row is deleted.</returns>
        public override List<CellEdit> GetEditedCellsInColumn(int indexOfColumn)
        {
            if (_rowIndex < 0)
                return new List<CellEdit>();
            return new List<CellEdit>(new[] { new CellEdit(_rowIndex, indexOfColumn, _rowData[indexOfColumn]) });
        }

        /// <summary>
        /// Returns the list of all <see cref="CellEdit"/> objects in this row.
        /// </summary>
        /// <param name="indexOfRow">The row index to retrieve.</param>
        /// <returns>A list of all cell edits in this row if the index matches; otherwise, an empty list.</returns>
        public override List<CellEdit> GetEditedCellsInRow(int indexOfRow)
        {
            var result = new List<CellEdit>();
            if (indexOfRow != _rowIndex)
                return result;
            for (int i = 0; i < _rowData.Count; i++)
                result.Add(new CellEdit(_rowIndex, i, _rowData[i]));
            return result;
        }

        /// <summary>
        /// Returns a <see cref="MultiCellEdit"/> representing all edited cells in this row.
        /// </summary>
        /// <returns>A <see cref="MultiCellEdit"/> constructed from all cell values in this row.</returns>
        public MultiCellEdit GetRowData()
        {
            var cellEdits = new CellEdit[_rowData.Count];
            for (int i = 0; i < _rowData.Count; i++)
                cellEdits[i] = new CellEdit(_rowIndex, i, _rowData[i]);
            // 
            return new MultiCellEdit(cellEdits);
        }
    }
}