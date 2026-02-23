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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GenericControls
{
    /// <summary>
    /// A toolbar control providing standard DataGrid operations such as add, insert, delete, copy, and paste.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class DataGridToolbar
    {

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridToolbar"/> class.
        /// </summary>
        public DataGridToolbar()
        {

            // This call is required by the designer.
            this.InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            var defaultButtonStyle = DefaultStackPanelButtonStyle();
            this.AddRowsButton.Style = defaultButtonStyle;
            this.InsertRowsButton.Style = defaultButtonStyle;
            this.DeleteRowsButton.Style = defaultButtonStyle;
            this.SelectAllButton.Style = defaultButtonStyle;
            this.CopyButton.Style = defaultButtonStyle;
            this.CopyWithHeadersButton.Style = defaultButtonStyle;
            this.PasteButton.Style = defaultButtonStyle;
            // 
            var defaultSeparatorStyle = DefaultStackPanelSeparatorStyle();
            this.EditSelectSeparator.Style = defaultSeparatorStyle;
            this.CustomOptionsSeparator.Style = defaultSeparatorStyle;
            // 
            this.InsertRowsButton.IsEnabled = false;
            this.DeleteRowsButton.IsEnabled = false;
            this.CopyButton.IsEnabled = false;
            this.CopyWithHeadersButton.IsEnabled = false;
            this.PasteButton.IsEnabled = false;
            // 
            _customButtons.CollectionChanged += (sender, e) =>
                {
                    if (e.OldItems is not null)
                    {
                        foreach (Button item in e.OldItems)
                            this.ToolbarStackPanel.Children.Remove(item);
                    }
                    if (e.NewItems is not null)
                    {
                        foreach (Button item in e.NewItems)
                        {
                            item.Style = DefaultStackPanelButtonStyle();
                            this.ToolbarStackPanel.Children.Insert(3 + _customButtons.Count, item);
                        }
                    }
                    // 
                    if (_customButtons.Count == 0)
                    {
                        this.CustomOptionsSeparator.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        this.CustomOptionsSeparator.Visibility = Visibility.Visible;
                    }
                };
        }



        #endregion

        #region Members

        /// <summary>
        /// Dependency property for the data grid.
        /// </summary>
        public static DependencyProperty DataGridProperty = DependencyProperty.Register(nameof(DataGrid), typeof(CopyPasteDataGrid), typeof(DataGridToolbar), new FrameworkPropertyMetadata(null, DataGridChangedCallback));

        /// <summary>
        /// Callback method invoked when the DataGrid property changes.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event data containing old and new values.</param>
        private static void DataGridChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            if (d.GetType() != typeof(DataGridToolbar))
                return;
            DataGridToolbar thisControl = (DataGridToolbar)d;
            // 
            if (e.OldValue is not null)
            {
                CopyPasteDataGrid oldGrid = e.OldValue as CopyPasteDataGrid;
                if (oldGrid is not null)
                {
                    oldGrid.PropertyChanged -= thisControl.DataGridPropertyChanged;
                    oldGrid.SelectedCellsChanged -= thisControl.DataGrid_SelectedCellsChanged;
                }
            }

            if (e.NewValue == null)
                return;
            CopyPasteDataGrid newGrid = e.NewValue as CopyPasteDataGrid;
            if (newGrid == null)
                return;
            // Can probably set the visibility of these buttons by binding to the datagrid property and using the booleantovisibility converter. Need to check.
            newGrid.PropertyChanged += thisControl.DataGridPropertyChanged;

            if (newGrid.CanUserAddInsertDeleteRows)
            {
                thisControl.AddRowsButton.Visibility = Visibility.Visible;
                thisControl.InsertRowsButton.Visibility = Visibility.Visible;
                thisControl.DeleteRowsButton.Visibility = Visibility.Visible;
            }
            else
            {
                thisControl.AddRowsButton.Visibility = Visibility.Collapsed;
                thisControl.InsertRowsButton.Visibility = Visibility.Collapsed;
                thisControl.DeleteRowsButton.Visibility = Visibility.Collapsed;
            }
            thisControl.PasteButton.IsEnabled = !newGrid.IsReadOnly;

            newGrid.SelectedCellsChanged += thisControl.DataGrid_SelectedCellsChanged;
        }

        /// <summary>
        /// Handles the SelectedCellsChanged event on the associated DataGrid,
        /// enabling or disabling toolbar buttons based on the current selection.
        /// </summary>
        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var grid = DataGrid;
            if (grid == null) return;

            if (grid.SelectedCells.Count <= 0)
            {
                InsertRowsButton.IsEnabled = false;
                DeleteRowsButton.IsEnabled = false;
                CopyButton.IsEnabled = false;
                CopyWithHeadersButton.IsEnabled = false;
                PasteButton.IsEnabled = false;
            }
            else
            {
                InsertRowsButton.IsEnabled = true;
                DeleteRowsButton.IsEnabled = true;
                CopyButton.IsEnabled = true;
                CopyWithHeadersButton.IsEnabled = true;
                PasteButton.IsEnabled = !grid.IsReadOnly;
            }
        }

        /// <summary>
        /// Handles changes to data grid properties and updates toolbar visibility accordingly.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data containing the property name that changed.</param>
        private void DataGridPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName ?? "") == nameof(CopyPasteDataGrid.CanUserAddInsertDeleteRows))
            {
                if (DataGrid.CanUserAddInsertDeleteRows)
                {
                    this.AddRowsButton.Visibility = Visibility.Visible;
                    this.InsertRowsButton.Visibility = Visibility.Visible;
                    this.DeleteRowsButton.Visibility = Visibility.Visible;
                }
                else
                {
                    this.AddRowsButton.Visibility = Visibility.Collapsed;
                    this.InsertRowsButton.Visibility = Visibility.Collapsed;
                    this.DeleteRowsButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Gets or sets the data grid associated with this toolbar.
        /// </summary>
        [Category("Miscellaneous")]
        [Description("Get and set the data grid for the control.")]
        [Browsable(true)]
        public CopyPasteDataGrid DataGrid
        {
            get
            {
                return (CopyPasteDataGrid)this.GetValue(DataGridProperty);
            }
            set
            {
                this.SetValue(DataGridProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the toolbar orientation.
        /// </summary>
        public static DependencyProperty ToolOrientationProperty = DependencyProperty.Register(nameof(ToolOrientation), typeof(Orientation), typeof(DataGridToolbar), new FrameworkPropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Gets or sets the orientation of the toolbar buttons.
        /// </summary>
        [Category("Miscellaneous")]
        [Description("Get and set the tool buttons orientation.")]
        [Browsable(true)]
        public Orientation ToolOrientation
        {
            get
            {
                return (Orientation)(int)this.GetValue(ToolOrientationProperty);
            }
            set
            {
                this.SetValue(ToolOrientationProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the control background color.
        /// </summary>
        public static DependencyProperty BackgroundColorProperty = DependencyProperty.Register(nameof(BackgroundColor), typeof(SolidColorBrush), typeof(DataGridToolbar), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Gets or sets the control background color.
        /// </summary>
        [Category("Brush")]
        [Description("Gets and sets the background color brush of the control.")]
        [Browsable(true)]
        public SolidColorBrush BackgroundColor
        {
            get
            {
                return (SolidColorBrush)this.GetValue(BackgroundColorProperty);
            }
            set
            {
                this.SetValue(BackgroundColorProperty, value);
            }
        }


        /// <summary>
        /// Dependency property for the stack panel button style.
        /// </summary>
        public static DependencyProperty StackPanelButtonStyleProperty = DependencyProperty.Register(nameof(StackPanelButtonStyle), typeof(Style), typeof(DataGridToolbar), new FrameworkPropertyMetadata(DefaultStackPanelButtonStyle(), StackPanelButtonStylePropertyCallback));

        /// <summary>
        /// Callback method invoked when the StackPanelButtonStyle property changes.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event data containing old and new values.</param>
        private static void StackPanelButtonStylePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            DataGridToolbar thisControl = (DataGridToolbar)d;
            // 
            Style newStyle = e.NewValue as Style;

            if (thisControl.AddRowsButton is not null)
                thisControl.AddRowsButton.Style = newStyle;
            if (thisControl.InsertRowsButton is not null)
                thisControl.InsertRowsButton.Style = newStyle;
            if (thisControl.DeleteRowsButton is not null)
                thisControl.DeleteRowsButton.Style = newStyle;
            if (thisControl.SelectAllButton is not null)
                thisControl.SelectAllButton.Style = newStyle;
            if (thisControl.CopyButton is not null)
                thisControl.CopyButton.Style = newStyle;
            if (thisControl.CopyWithHeadersButton is not null)
                thisControl.CopyWithHeadersButton.Style = newStyle;
            if (thisControl.PasteButton is not null)
                thisControl.PasteButton.Style = newStyle;
        }

        /// <summary>
        /// Returns the default style for toolbar buttons in a stack panel layout.
        /// Cached after first creation to avoid repeated resource lookups.
        /// </summary>
        /// <returns>The default <see cref="Style"/> for toolbar buttons.</returns>
        private static Style _cachedDefaultButtonStyle;
        private static Style DefaultStackPanelButtonStyle()
        {
            if (_cachedDefaultButtonStyle != null)
                return _cachedDefaultButtonStyle;

            var s = new Style(typeof(Button), (Style)Application.Current.FindResource(ToolBar.ButtonStyleKey));
            s.Setters.Add(new Setter(FrameworkElement.HeightProperty, 24d));
            s.Setters.Add(new Setter(FrameworkElement.WidthProperty, 24d));
            s.Setters.Add(new Setter(FrameworkElement.CursorProperty, Cursors.Hand));
            var disabledShadeTrigger = new Trigger() { Property = UIElement.IsEnabledProperty, Value = false };
            disabledShadeTrigger.Setters.Add(new Setter(UIElement.OpacityProperty, 0.5d));
            s.Triggers.Add(disabledShadeTrigger);

            _cachedDefaultButtonStyle = s;
            return s;
        }

        /// <summary>
        /// Gets or sets the stack panel button style.
        /// </summary>
        public Style StackPanelButtonStyle
        {
            get
            {
                return (Style)this.GetValue(StackPanelButtonStyleProperty);
            }
            set
            {
                this.SetValue(StackPanelButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the stack panel separator style.
        /// </summary>
        public static DependencyProperty StackPanelSeparatorStyleProperty = DependencyProperty.Register(nameof(StackPanelSeparatorStyle), typeof(Style), typeof(DataGridToolbar), new FrameworkPropertyMetadata(DefaultStackPanelSeparatorStyle(), StackPanelSeperatorStylePropertyCallback));

        /// <summary>
        /// Callback method invoked when the StackPanelSeperatorStyle property changes.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event data containing old and new values.</param>
        private static void StackPanelSeperatorStylePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            DataGridToolbar thisControl = (DataGridToolbar)d;
            // 
            Style newStyle = e.NewValue as Style;
            // 
            if (thisControl.EditSelectSeparator is not null)
                thisControl.EditSelectSeparator.Style = newStyle;
            if (thisControl.CustomOptionsSeparator is not null)
                thisControl.CustomOptionsSeparator.Style = newStyle;
        }

        /// <summary>
        /// Returns the default style for separators in the toolbar.
        /// </summary>
        /// <returns>The default <see cref="Style"/>.</returns>
        private static Style DefaultStackPanelSeparatorStyle()
        {
            var s = new Style(typeof(Separator));
            s.Setters.Add(new Setter(FrameworkElement.MarginProperty, new Thickness(2d, 1d, 2d, 1d)));
            // Vertical text
            var verticalTransform = new RotateTransform(90d);
            s.Setters.Add(new Setter(FrameworkElement.LayoutTransformProperty, verticalTransform));

            return s;
        }

        /// <summary>
        /// Gets or sets the stack panel separator style.
        /// </summary>
        public Style StackPanelSeparatorStyle
        {
            get
            {
                return (Style)this.GetValue(StackPanelSeparatorStyleProperty);
            }
            set
            {
                this.SetValue(StackPanelSeparatorStyleProperty, value);
            }
        }

        private ObservableCollection<Button> _customButtons = new ObservableCollection<Button>();

        /// <summary>
        /// Gets the collection of custom buttons added by the user.
        /// These are displayed in the toolbar between the Edit and Clipboard groups.
        /// </summary>
        public ObservableCollection<Button> CustomButtons
        {
            get
            {
                return _customButtons;
            }
            private set
            {
                // do nothing
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Handles the AddRowsButton click event and adds rows to the data grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void AddRowsButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid == null)
                return;
            if (DataGrid.CanUserAddInsertDeleteRows)
            {
                var uniqueRows = DataGrid.GetRowsWithSelectedCells();
                DataGrid.AddRows(Math.Max(uniqueRows.Count, 1));
                DataGrid.Focus();
            }
        }

        /// <summary>
        /// Handles the InsertRowsButton click event and inserts rows into the data grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void InsertRowsButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid == null)
                return;
            if (DataGrid.CanUserAddInsertDeleteRows)
                DataGrid.InsertRows();
            DataGrid.Focus();
        }

        /// <summary>
        /// Handles the DeleteRowsButton click event and deletes rows from the data grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void DeleteRowsButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid == null)
                return;
            if (DataGrid.CanUserAddInsertDeleteRows)
                DataGrid.DeleteRows();
            DataGrid.Focus();
        }


        /// <summary>
        /// Handles the SelectAllButton click event and selects all cells in the data grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid == null)
                return;
            DataGrid.SelectAllCells();
            DataGrid.Focus();
        }

        /// <summary>
        /// Handles the CopyButton click event and copies the selected cells to the clipboard.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid == null)
                return;
            ApplicationCommands.Copy.Execute(null, DataGrid);
            DataGrid.Focus();
        }

        /// <summary>
        /// Handles the CopyWithHeadersButton click event and copies the selected cells with headers to the clipboard.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void CopyWithHeadersButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid == null)
                return;
            DataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
            ApplicationCommands.Copy.Execute(null, DataGrid);
            DataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
            DataGrid.Focus();
        }

        /// <summary>
        /// Handles the PasteButton click event and pastes data from the clipboard into the data grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid == null)
                return;
            // Only paste if there is data in the clipboard.
            try
            {
                string[][] clipboardData = Clipboard.GetText().Split('\n').Select(row => row.Split('\t').Select(Clipboardcell => Clipboardcell.Length > 0 && Clipboardcell[Clipboardcell.Length - 1] == '\r' ? Clipboardcell.Substring(0, Clipboardcell.Length - 1) : Clipboardcell).ToArray()).Where(a => a.Any(b => b.Length > 0)).ToArray();
                if (clipboardData.Length > 0)
                    DataGrid.PasteClipboard();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            DataGrid.Focus();
        }

        #endregion

    }
}