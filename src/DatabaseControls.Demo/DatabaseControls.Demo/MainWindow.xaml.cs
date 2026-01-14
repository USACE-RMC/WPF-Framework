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

using System.IO;
using System.Windows;
using System.Windows.Controls;
using DatabaseManager;
using Microsoft.Win32;

namespace DatabaseControls.Demo
{
    /// <summary>
    /// Main window for the DatabaseControls demo application.
    /// Provides functionality to open, view, and export database files.
    /// </summary>
    /// <remarks>
    /// This window demonstrates the usage of the <see cref="DatabaseControls.TableViewer"/> control
    /// for viewing and editing database tables from various file formats including:
    /// <list type="bullet">
    /// <item><description>DBF (dBASE) files</description></item>
    /// <item><description>MDB (Microsoft Access) files</description></item>
    /// <item><description>SQLite database files</description></item>
    /// <item><description>CSV (Comma-Separated Values) files</description></item>
    /// </list>
    /// </remarks>
    public partial class MainWindow : Window
    {
        #region Fields

        /// <summary>
        /// The database manager instance used to read and manage database connections.
        /// </summary>
        private DatabaseManager.DatabaseManager? _databaseReader;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the Loaded event of the main window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Window initialization complete
        }

        /// <summary>
        /// Handles the Click event of the Select Database button.
        /// Opens a file dialog allowing the user to select a database file to open.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Supported file formats:
        /// <list type="bullet">
        /// <item><description>.dbf - dBASE database files (single table)</description></item>
        /// <item><description>.mdb - Microsoft Access database files (multiple tables)</description></item>
        /// <item><description>.sqlite - SQLite database files (multiple tables)</description></item>
        /// <item><description>.csv - Comma-separated values files (single table)</description></item>
        /// </list>
        /// For multi-table formats (MDB, SQLite), a table selection combo box is enabled.
        /// For single-table formats (DBF, CSV), the table is loaded directly.
        /// </remarks>
        private void SelectDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            string inputFile = ShowFileOpenDialog("Database file (*.dbf, *.mdb, *.sqlite, *.csv)|*.dbf;*.mdb;*.sqlite;*.csv");

            if (string.IsNullOrEmpty(inputFile))
            {
                return;
            }

            string extension = Path.GetExtension(inputFile).ToLower();

            switch (extension)
            {
                case ".mdb":
                    LoadMdbFile(inputFile);
                    break;

                case ".dbf":
                    LoadDbfFile(inputFile);
                    break;

                case ".sqlite":
                    LoadSqliteFile(inputFile);
                    break;

                case ".csv":
                    LoadCsvFile(inputFile);
                    break;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the table combo box.
        /// Loads the selected table into the TableViewer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void TableComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            if (TableComboBox.SelectedIndex == -1)
            {
                return;
            }

            if (_databaseReader == null)
            {
                return;
            }

            string selectedTable = (string)TableComboBox.SelectedValue;
            DataTableView tableView = _databaseReader.GetTableManager(selectedTable);
            TestViewer.DataView = tableView;
        }

        /// <summary>
        /// Handles the Click event of the Export menu item.
        /// Opens a save file dialog to export the current table data to various formats.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Supported export formats:
        /// <list type="bullet">
        /// <item><description>.csv - Comma-separated values</description></item>
        /// <item><description>.dbf - dBASE database file</description></item>
        /// <item><description>.xlsx - Microsoft Excel workbook</description></item>
        /// <item><description>.sqlite - SQLite database file</description></item>
        /// </list>
        /// </remarks>
        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TestViewer.DataView == null)
            {
                return;
            }

            string filters = "Comma delimited (*.csv)|*.csv|Database (*.dbf)|*.dbf|Excel (*.xlsx)|*.xlsx|SQLite (*.sqlite)|*.sqlite";
            var saveFileDialog = new SaveFileDialog { Filter = filters };

            if (saveFileDialog.ShowDialog() == true)
            {
                string outputPath = saveFileDialog.FileName;
                string extension = Path.GetExtension(outputPath).ToLower();

                switch (extension)
                {
                    case ".csv":
                        TestViewer.DataView.ExportToCsv(outputPath);
                        break;

                    case ".dbf":
                        TestViewer.DataView.ExportToDbf(outputPath);
                        break;

                    case ".xls":
                    case ".xlsx":
                        TestViewer.DataView.ExportToXlsx(outputPath);
                        break;

                    case ".sqlite":
                        TestViewer.DataView.ExportToSqlite(outputPath, TestViewer.DataView.TableName);
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads a Microsoft Access database file (.mdb) and populates the table combo box.
        /// </summary>
        /// <param name="filePath">The path to the MDB file.</param>
        private void LoadMdbFile(string filePath)
        {
            var tableReader = new MdbReader(filePath);

            TableComboBox.IsEnabled = true;
            TableComboBox.Items.Clear();

            string[] tableNames = tableReader.GetTableNames();
            foreach (string tableName in tableNames)
            {
                TableComboBox.Items.Add(tableName);
            }

            _databaseReader = tableReader;
        }

        /// <summary>
        /// Loads a dBASE database file (.dbf) directly into the viewer.
        /// </summary>
        /// <param name="filePath">The path to the DBF file.</param>
        private void LoadDbfFile(string filePath)
        {
            _databaseReader = new DbfReader(filePath);

            TableComboBox.Items.Clear();
            TableComboBox.Items.Add(Path.GetFileNameWithoutExtension(filePath));
            TableComboBox.IsEnabled = false;
            TableComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Loads a SQLite database file and populates the table combo box.
        /// </summary>
        /// <param name="filePath">The path to the SQLite file.</param>
        private void LoadSqliteFile(string filePath)
        {
            var tableReader = new SQLiteManager(filePath);

            TableComboBox.IsEnabled = true;
            TableComboBox.Items.Clear();

            string[] tableNames = tableReader.GetTableNames();
            foreach (string tableName in tableNames)
            {
                TableComboBox.Items.Add(tableName);
            }

            _databaseReader = tableReader;
        }

        /// <summary>
        /// Loads a CSV file directly into the viewer.
        /// </summary>
        /// <param name="filePath">The path to the CSV file.</param>
        private void LoadCsvFile(string filePath)
        {
            _databaseReader = new CsvReader(filePath, hasHeader: true, skipRows: 1, isReadOnly: false);

            TableComboBox.Items.Clear();
            TableComboBox.Items.Add(Path.GetFileNameWithoutExtension(filePath));
            TableComboBox.IsEnabled = false;
            TableComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Displays an open file dialog with the specified filter.
        /// </summary>
        /// <param name="filters">The file filter string for the dialog.</param>
        /// <returns>The selected file path, or an empty string if cancelled.</returns>
        private static string ShowFileOpenDialog(string filters)
        {
            var openFileDialog = new OpenFileDialog { Filter = filters };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return string.Empty;
        }

        #endregion
    }
}
