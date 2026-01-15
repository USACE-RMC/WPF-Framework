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

namespace DatabaseManager
{
    /// <summary>
    /// Represents an edit operation that deletes a single row from a table.
    /// This class is intended to record structural changes rather than specific cell modifications.
    /// </summary>
    public class DeleteRowEdit : TableEdit
    {
        private readonly int _rowIndex;

        /// <summary>
        /// Gets the index of the row that is marked for deletion.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return _rowIndex;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteRowEdit"/> class.
        /// </summary>
        /// <param name="viewRowIndex">The index of the row to be deleted.</param>
        public DeleteRowEdit(int viewRowIndex)
        {
            _rowIndex = viewRowIndex;
        }

        /// <summary>
        /// Handles the addition of a column.
        /// This operation is not applicable for a row delete and is currently unimplemented.
        /// </summary>
        /// <typeparam name="T">The type of data in the added column.</typeparam>
        /// <param name="indexOfColumn">The index of the added column.</param>
        /// <param name="columnData">Optional column data.</param>
        public override void ColumnAdded<T>(int indexOfColumn, T[] columnData = null)
        {
            // Throw New NotImplementedException()
        }

        /// <summary>
        /// Handles the deletion of a column. 
        /// This operation is not applicable for a column delete and is currently unimplemented.
        /// </summary>
        /// <param name="indexOfColumn">The index of the deleted column.</param>
        public override void ColumnDeleted(int indexOfColumn)
        {
            // Throw New NotImplementedException()
        }

        /// <summary>
        /// Handles the addition of a row. 
        /// Adjusting the internal row index could be implemented here if row tracking is required.
        /// </summary>
        /// <param name="indexOfRow">The index where the row was added.</param>
        /// <param name="rowData">Optional data for the added row.</param>
        public override void RowAdded(int indexOfRow, object[] rowData = null)
        {
            // If _rowIndex > indexOfRow Then _rowIndex += 1
        }

        /// <summary>
        /// Handles the deletion of a row.
        /// Adjusting the internal row index could be implemented here if row tracking is required.
        /// </summary>
        /// <param name="indexOfRow">The index of the deleted row.</param>
        public override void RowDeleted(int indexOfRow)
        {
            // If _rowIndex > indexOfRow Then _rowIndex -= 1
        }

        /// <summary>
        /// Indicates whether this edit affects a specific cell.
        /// </summary>
        /// <param name="indexOfColumn">The column index of the cell.</param>
        /// <param name="indexOfRow">The row index of the cell.</param>
        /// <param name="returnValue">The cell value, if found (unused).</param>
        /// <returns>Always returns <c>false</c> since row deletion does not track specific cell values.</returns>
        public override bool ContainsCell(int indexOfColumn, int indexOfRow, ref object returnValue)
        {
            return false;
        }

        /// <summary>
        /// Indicates whether this edit affects a specific column.
        /// </summary>
        /// <param name="indexOfColumn">The column index to check.</param>
        /// <returns>Always returns <c>false</c> since row deletion does not affect columns.</returns>
        public override bool ContainsColumn(int indexOfColumn)
        {
            return false;
        }

        /// <summary>
        /// Indicates whether this edit affects a specific row.
        /// </summary>
        /// <param name="indexOfRow">The row index to check.</param>
        /// <returns>Always returns <c>false</c> since this edit represents a structural deletion, not specific row data.</returns>
        public override bool ContainsRow(int indexOfRow)
        {
            return false;
        }

        /// <summary>
        /// Returns the list of cell edits in the specified column.
        /// </summary>
        /// <param name="indexOfColumn">The column index to check.</param>
        /// <returns>An empty list since row deletion does not modify specific cells.</returns>
        public override List<CellEdit> GetEditedCellsInColumn(int indexOfColumn)
        {
            return new List<CellEdit>();
        }

        /// <summary>
        /// Returns the list of cell edits in the specified row.
        /// </summary>
        /// <param name="indexOfRow">The row index to check.</param>
        /// <returns>An empty list since row deletion does not modify specific cells.</returns>
        public override List<CellEdit> GetEditedCellsInRow(int indexOfRow)
        {
            return new List<CellEdit>();
        }
    }
}