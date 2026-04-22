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
    /// Represents a composite table edit that deletes multiple rows.
    /// Each row deletion is encapsulated by a <see cref="DeleteRowEdit"/> instance.
    /// </summary>
    public class DeleteRowsEdit : TableEdit
    {

        private readonly DeleteRowEdit[] _deleteRowEdits;

        /// <summary>
        /// Gets the collection of individual row deletion edits.
        /// </summary>
        public DeleteRowEdit[] DeleteRowEdits
        {
            get
            {
                return _deleteRowEdits;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteRowEdit"/> class.
        /// </summary>
        /// <param name="rowDeleteEdits"></param>
        public DeleteRowsEdit(DeleteRowEdit[] rowDeleteEdits)
        {
            _deleteRowEdits = rowDeleteEdits;
        }
        
        /// <summary>
        /// Gets the row indices that are marked for deletion 
        /// </summary>
        /// <returns>An array of deleted row indices.</returns>
        public int[] GetDeletedRowIndices()
        {
            var rowsDeleted = new int[_deleteRowEdits.Length];
            for (int i = 0; i < _deleteRowEdits.Length; i++)
                rowsDeleted[i] = _deleteRowEdits[i].RowIndex;
            return rowsDeleted;
        }

        /// <summary>
        /// Propagates column addition to all contained <see cref="DeleteRowEdit"/> instances.
        /// </summary>
        /// <typeparam name="T">The type of the data in the added column.</typeparam>
        /// <param name="indexOfColumn">The index where the column was added.</param>
        /// <param name="columnData">Optional column data.</param>
        public override void ColumnAdded<T>(int indexOfColumn, T[] columnData = null)
        {
            foreach (DeleteRowEdit rowDeleteEdit in _deleteRowEdits)
                rowDeleteEdit.ColumnAdded(indexOfColumn, columnData);
        }

        /// <summary>
        /// Propagates column deletion to all contained <see cref="DeleteRowEdit"/> instances.
        /// </summary>
        /// <param name="indexOfColumn">The index of the deleted column.</param>
        public override void ColumnDeleted(int indexOfColumn)
        {
            foreach (DeleteRowEdit rowDeleteEdit in _deleteRowEdits)
                rowDeleteEdit.ColumnDeleted(indexOfColumn);
        }

        /// <summary>
        /// Checks whether any contained edit affects the specified cell.
        /// </summary>
        /// <param name="indexOfColumn">The column index of the cell.</param>
        /// <param name="indexOfRow">The row index of the cell.</param>
        /// <param name="returnValue">Output parameter for the cell's value if found.</param>
        /// <returns><c>true</c> if any contained edit affects the specified cell; otherwise, <c>false</c>.</returns>
        public override bool ContainsCell(int indexOfColumn, int indexOfRow, ref object returnValue)
        {
            foreach (DeleteRowEdit rowDeleteEdit in _deleteRowEdits)
            {
                if (rowDeleteEdit.ContainsCell(indexOfColumn, indexOfRow, ref returnValue))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether any contained edit affects the specified column.
        /// </summary>
        /// <param name="indexOfColumn">The column index to check.</param>
        /// <returns><c>true</c> if any contained edit affects the specified column; otherwise, <c>false</c>.</returns>
        public override bool ContainsColumn(int indexOfColumn)
        {
            foreach (DeleteRowEdit rowDeleteEdit in _deleteRowEdits)
            {
                if (rowDeleteEdit.ContainsColumn(indexOfColumn))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether any contained edit affects the specified row.
        /// </summary>
        /// <param name="indexOfRow">The row index to check.</param>
        /// <returns><c>true</c> if any contained edit affects the specified row; otherwise, <c>false</c>.</returns>
        public override bool ContainsRow(int indexOfRow)
        {
            foreach (DeleteRowEdit rowDeleteEdit in _deleteRowEdits)
            {
                if (rowDeleteEdit.ContainsRow(indexOfRow))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the list of cell edits in the specified column across all contained edits.
        /// </summary>
        /// <param name="indexOfColumn">The column index to retrieve edits from.</param>
        /// <returns>A list of cell edits from all contained edits that affect the specified column.</returns>
        public override List<CellEdit> GetEditedCellsInColumn(int indexOfColumn)
        {
            var result = new List<CellEdit>();
            foreach (DeleteRowEdit rowDeleteEdit in _deleteRowEdits)
                result.AddRange(rowDeleteEdit.GetEditedCellsInColumn(indexOfColumn));
            return result;
        }

        /// <summary>
        /// Gets the list of cell edits in the specified row across all contained edits.
        /// </summary>
        /// <param name="indexOfRow">The row index to retrieve edits from.</param>
        /// <returns>A list of cell edits from all contained edits that affect the specified row.</returns>
        public override List<CellEdit> GetEditedCellsInRow(int indexOfRow)
        {
            var result = new List<CellEdit>();
            foreach (DeleteRowEdit rowDeleteEdit in _deleteRowEdits)
                result.AddRange(rowDeleteEdit.GetEditedCellsInRow(indexOfRow));
            return result;
        }

        /// <summary>
        /// Propagates row addition to all contained <see cref="DeleteRowEdit"/> instances.
        /// </summary>
        /// <param name="indexOfRow">The index where the row was added.</param>
        /// <param name="rowData">Optional row data.</param>
        public override void RowAdded(int indexOfRow, object[] rowData = null)
        {
            foreach (DeleteRowEdit rowDeleteEdit in _deleteRowEdits)
                rowDeleteEdit.RowAdded(indexOfRow, rowData);
        }

        /// <summary>
        /// Propagates row deletion to all contained <see cref="DeleteRowEdit"/> instances.
        /// </summary>
        /// <param name="indexOfRow">The index of the deleted row.</param>
        public override void RowDeleted(int indexOfRow)
        {
            foreach (DeleteRowEdit rowDeleteEdit in _deleteRowEdits)
                rowDeleteEdit.RowDeleted(indexOfRow);
        }
    }
}