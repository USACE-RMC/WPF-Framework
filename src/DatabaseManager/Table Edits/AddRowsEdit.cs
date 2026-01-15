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
    /// Represents a batch edit operation that adds multiple rows to a table.
    /// Each row is managed by an individual <see cref="AddRowEdit"/> instance.
    /// </summary>
    public class AddRowsEdit : TableEdit
    {

        private readonly AddRowEdit[] _addRowEdits;
        /// <summary>
        /// Gets the array of row edits representing the rows that were added.
        /// </summary>
        public AddRowEdit[] AddRowEdits
        {
            get
            {
                return _addRowEdits;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AddRowsEdit"/> class with a collection of row edits.
        /// </summary>
        /// <param name="rowEditsToAdd">An array of <see cref="AddRowsEdit"/> objects representing rows to add.</param>
        public AddRowsEdit(AddRowEdit[] rowEditsToAdd)
        {
            _addRowEdits = rowEditsToAdd;
        }
        /// <summary>
        /// Gets the original row data for each added row.
        /// </summary>
        /// <returns></returns>
        public List<object[]> GetOriginalRowEdits()
        {
            var edits = new List<object[]>(_addRowEdits.Count());
            foreach (AddRowEdit rowEdit in _addRowEdits)
                edits.Add(rowEdit.OriginalRowEdit);
            // 
            return edits;
        }

        /// <summary>
        /// Propagates a column addition to all added rows.
        /// </summary>
        /// <typeparam name="T">The type of the added column.</typeparam>
        /// <param name="indexOfColumn">The index where the column was added.</param>
        /// <param name="columnData">Optional data for the new column.</param>
        public override void ColumnAdded<T>(int indexOfColumn, T[] columnData = null)
        {
            foreach (AddRowEdit rowEditToAdd in _addRowEdits)
                rowEditToAdd.ColumnAdded(indexOfColumn, columnData);
        }

        /// <summary>
        /// Propagates a column deletion to all added rows.
        /// </summary>
        /// <param name="indexOfColumn">The index of the column to delete.</param>
        public override void ColumnDeleted(int indexOfColumn)
        {
            foreach (AddRowEdit rowEditToAdd in _addRowEdits)
                rowEditToAdd.ColumnDeleted(indexOfColumn);
        }

        /// <summary>
        /// Determines whether any added row contains the specified cell.
        /// </summary>
        /// <param name="indexOfColumn">The column index of the cell.</param>
        /// <param name="indexOfRow">The row index of the cell.</param>
        /// <param name="returnValue">The value of the cell if found.</param>
        /// <returns></returns>
        public override bool ContainsCell(int indexOfColumn, int indexOfRow, ref object returnValue)
        {
            foreach (AddRowEdit rowEditToAdd in _addRowEdits)
            {
                if (rowEditToAdd.ContainsCell(indexOfColumn, indexOfRow, ref returnValue))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether any added row contains the specified column.
        /// </summary>
        /// <param name="indexOfColumn">The column index to check.</param>
        /// <returns></returns>
        public override bool ContainsColumn(int indexOfColumn)
        {
            foreach (AddRowEdit rowEditToAdd in _addRowEdits)
            {
                if (rowEditToAdd.ContainsColumn(indexOfColumn))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether any added row is located at the specified index.
        /// </summary>
        /// <param name="indexOfRow">The row index to check.</param>
        /// <returns></returns>
        public override bool ContainsRow(int indexOfRow)
        {
            foreach (AddRowEdit rowEditToAdd in _addRowEdits)
            {
                if (rowEditToAdd.ContainsRow(indexOfRow))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a list of all cell edits in the specified column across all added rows.
        /// </summary>
        /// <param name="indexOfColumn">The column index to retrieve edits from.</param>
        /// <returns></returns>
        public override List<CellEdit> GetEditedCellsInColumn(int indexOfColumn)
        {
            var result = new List<CellEdit>();
            foreach (AddRowEdit rowEditToAdd in _addRowEdits)
                result.AddRange(rowEditToAdd.GetEditedCellsInColumn(indexOfColumn));
            return result;
        }

        /// <summary>
        /// Gets a list of all cell edits in the specified row across all added rows.
        /// </summary>
        /// <param name="indexOfRow">The row index to retrieve edits from.</param>
        /// <returns></returns>
        public override List<CellEdit> GetEditedCellsInRow(int indexOfRow)
        {
            var result = new List<CellEdit>();
            foreach (AddRowEdit rowEditToAdd in _addRowEdits)
                result.AddRange(rowEditToAdd.GetEditedCellsInRow(indexOfRow));
            return result;
        }

        /// <summary>
        /// Updates the internal row index of all added rows when a new row is inserted in the table.
        /// </summary>
        /// <param name="indexOfRow">The index where the row was added.</param>
        /// <param name="rowData">Optional data for the new row.</param>
        public override void RowAdded(int indexOfRow, object[] rowData = null)
        {
            foreach (AddRowEdit rowEditToAdd in _addRowEdits)
                rowEditToAdd.RowAdded(indexOfRow, rowData);
        }

        /// <summary>
        /// Updates the internal row index of all added rows when a row is deleted from the table.
        /// </summary>
        /// <param name="indexOfRow">The index of the deleted row.</param>
        public override void RowDeleted(int indexOfRow)
        {
            foreach (AddRowEdit rowEditToAdd in _addRowEdits)
                rowEditToAdd.RowDeleted(indexOfRow);
        }
    }
}