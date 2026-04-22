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
    /// Represents a batch edit operation that adds multiple columns to a table simultaneously.
    /// Each added column is represented by an <see cref="IColumnEdit"/> implementation.
    /// </summary>
    public class AddColumnsEdit : TableEdit
    {

        private readonly IColumnEdit[] _addColumnEdits;

        /// <summary>
        /// Gets the array of column edits representing the columns that were added.
        /// </summary>
        public IColumnEdit[] AddColumnEdits
        {
            get
            {
                return _addColumnEdits;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddColumnsEdit"/> class with a set of 
        /// columns to edit.
        /// </summary>
        /// <param name="columnsToAdd">The columns to add.</param>
        public AddColumnsEdit(IColumnEdit[] columnsToAdd)
        {
            _addColumnEdits = columnsToAdd;
        }

        /// <summary>
        /// Adjusts the column indices of the added columns when another column is added elsewhere in the table.
        /// </summary>
        /// <typeparam name="T">The type of the newly added column</typeparam>
        /// <param name="indexOfColumn">The index where the new column was added.</param>
        /// <param name="columnData">Optional data for the new column.</param>
        public override void ColumnAdded<T>(int indexOfColumn, T[] columnData = null)
        {
            foreach (TableEdit columnAdded in _addColumnEdits)
                columnAdded.ColumnAdded(indexOfColumn, columnData);
        }

        /// <summary>
        /// Adjusts the column indices of the added columns when a column is deleted from the table.
        /// </summary>
        /// <param name="indexOfColumn">The index of the deleted column.</param>
        public override void ColumnDeleted(int indexOfColumn)
        {
            foreach (TableEdit columnAdded in _addColumnEdits)
                columnAdded.ColumnDeleted(indexOfColumn);
        }

        /// <summary>
        /// Determines whether any of the added columns contain the specified cell.
        /// </summary>
        /// <param name="indexOfColumn">The column index of the cell.</param>
        /// <param name="indexOfRow">The row index of the cell.</param>
        /// <param name="returnValue">The value of the cell if it is found.</param>
        /// <returns><c>true</c> if any added column contains the specified cell; otherwise, <c>false</c>.</returns>
        public override bool ContainsCell(int indexOfColumn, int indexOfRow, ref object returnValue)
        {
            foreach (TableEdit columnAdded in _addColumnEdits)
            {
                if (columnAdded.ContainsCell(indexOfColumn, indexOfRow, ref returnValue))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether any of the added columns match the specified column index.
        /// </summary>
        /// <param name="indexOfColumn">The column index to check.</param>
        /// <returns><c>true</c> if any added column matches the specified index; otherwise, <c>false</c>.</returns>
        public override bool ContainsColumn(int indexOfColumn)
        {
            foreach (TableEdit columnAdded in _addColumnEdits)
            {
                if (columnAdded.ContainsColumn(indexOfColumn))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether any of the added columns contain the specified row.
        /// </summary>
        /// <param name="indexOfRow">The row index to check.</param>
        /// <returns><c>true</c> if any added column contains the specified row; otherwise, <c>false</c>.</returns>
        public override bool ContainsRow(int indexOfRow)
        {
            foreach (TableEdit columnAdded in _addColumnEdits)
            {
                if (columnAdded.ContainsRow(indexOfRow))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a list of all cell edits in the specified column across all added columns.
        /// </summary>
        /// <param name="indexOfColumn">The column index to query.</param>
        /// <returns>A list of cell edits from all added columns that affect the specified column.</returns>
        public override List<CellEdit> GetEditedCellsInColumn(int indexOfColumn)
        {
            var result = new List<CellEdit>();
            foreach (TableEdit columnAdded in _addColumnEdits)
                result.AddRange(columnAdded.GetEditedCellsInColumn(indexOfColumn));
            return result;
        }

        /// <summary>
        /// Gets a list of all cell edits in the specified row across all added columns.
        /// </summary>
        /// <param name="indexOfRow">The row index to query.</param>
        /// <returns>A list of cell edits from all added columns that affect the specified row.</returns>
        public override List<CellEdit> GetEditedCellsInRow(int indexOfRow)
        {
            var result = new List<CellEdit>();
            foreach (TableEdit columnAdded in _addColumnEdits)
                result.AddRange(columnAdded.GetEditedCellsInRow(indexOfRow));
            return result;
        }

        /// <summary>
        /// Inserts a new row into each added column.
        /// </summary>
        /// <param name="indexOfRow">The index where the row was added.</param>
        /// <param name="rowData">Optional data for the added row.</param>
        public override void RowAdded(int indexOfRow, object[] rowData = null)
        {
            foreach (TableEdit columnAdded in _addColumnEdits)
                columnAdded.RowAdded(indexOfRow, rowData);
        }

        /// <summary>
        /// Deletes the specified row from each added column.
        /// </summary>
        /// <param name="indexOfRow">The index of the deleted row.</param>
        public override void RowDeleted(int indexOfRow)
        {
            foreach (TableEdit columnAdded in _addColumnEdits)
                columnAdded.RowDeleted(indexOfRow);
        }
    }
}