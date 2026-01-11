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

using GenericControls;
using Numerics.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NumericControls
{
    /// <summary>
    /// A user control for displaying and editing time series data in a tabular format.
    /// Provides functionality for data validation, mathematical operations, and data manipulation.
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
    public partial class TimeSeriesTable : UserControl
    {
        /// <summary>
        /// Identifies the <see cref="Series"/> dependency property.
        /// </summary>
        public static DependencyProperty SeriesProperty = DependencyProperty.Register(nameof(Series), typeof(TimeSeries), typeof(TimeSeriesTable), new PropertyMetadata(new TimeSeries(), SetData));

        /// <summary>
        /// Handles changes to the Series property and configures the grid for the time interval type.
        /// </summary>
        /// <param name="d">The dependency object that changed.</param>
        /// <param name="e">The property changed event arguments.</param>
        private static void SetData(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(TimeSeriesTable)) return;
            TimeSeriesTable thisControl = (TimeSeriesTable)d;

            thisControl.DateTimeColumn.IsReadOnly = true;
            thisControl.DateTimeSelectorColumn.Visibility = Visibility.Collapsed;
            thisControl.DateTimeColumn.CellStyle = (Style)thisControl.TryFindResource("Right_CellStyleDisabled");
            if (e.NewValue == null) { return; }
            TimeSeries newSeries = e.NewValue as TimeSeries;
            if (newSeries == null)
            {
                thisControl.TimeSeriesDataGrid.IsEnabled = false;
                return;
            }


            if (newSeries.TimeInterval == TimeInterval.Irregular)
            {
                thisControl.DateTimeColumn.CellStyle = (Style)thisControl.TryFindResource("Right_CellStyle");
                thisControl.DateTimeSelectorColumn.Visibility = Visibility.Visible;
                thisControl.DateTimeColumn.IsReadOnly = false;
            }
        }

        /// <summary>
        /// Gets or sets the time series data displayed in the table.
        /// </summary>
        public TimeSeries Series
        {
            get { return (TimeSeries)GetValue(SeriesProperty); }
            set { SetValue(SeriesProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MaximumX"/> dependency property.
        /// </summary>
        public static DependencyProperty MaximumXProperty = DependencyProperty.Register(nameof(MaximumX), typeof(DateTime), typeof(TimeSeriesTable), new FrameworkPropertyMetadata(DateTime.MaxValue));

        /// <summary>
        /// Gets or sets the maximum allowed date/time value for validation.
        /// </summary>
        public DateTime MaximumX
        {
            get { return (DateTime)GetValue(MaximumXProperty); }
            set { SetValue(MaximumXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinimumX"/> dependency property.
        /// </summary>
        public static DependencyProperty MinimumXProperty = DependencyProperty.Register(nameof(MinimumX), typeof(DateTime), typeof(TimeSeriesTable), new FrameworkPropertyMetadata(DateTime.MinValue));

        /// <summary>
        /// Gets or sets the minimum allowed date/time value for validation.
        /// </summary>
        public DateTime MinimumX
        {
            get { return (DateTime)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MaximumY"/> dependency property.
        /// </summary>
        public static DependencyProperty MaximumYProperty = DependencyProperty.Register(nameof(MaximumY), typeof(double), typeof(TimeSeriesTable), new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or sets the maximum allowed Y value for validation.
        /// </summary>
        public double MaximumY
        {
            get { return (double)GetValue(MaximumYProperty); }
            set { SetValue(MaximumYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinimumY"/> dependency property.
        /// </summary>
        public static DependencyProperty MinimumYProperty = DependencyProperty.Register(nameof(MinimumY), typeof(double), typeof(TimeSeriesTable), new FrameworkPropertyMetadata(double.MinValue));

        /// <summary>
        /// Gets or sets the minimum allowed Y value for validation.
        /// </summary>
        public double MinimumY
        {
            get { return (double)GetValue(MinimumYProperty); }
            set { SetValue(MinimumYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(TimeSeriesTable), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the control is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="XColumnHeader"/> dependency property.
        /// </summary>
        public static DependencyProperty XColumnHeaderProperty = DependencyProperty.Register(nameof(XColumnHeader), typeof(string), typeof(TimeSeriesTable), new FrameworkPropertyMetadata("X Data"));

        /// <summary>
        /// Gets or sets the column header text for the date/time (X) column.
        /// </summary>
        public string XColumnHeader
        {
            get { return (string)GetValue(XColumnHeaderProperty); }
            set { SetValue(XColumnHeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="YColumnHeader"/> dependency property.
        /// </summary>
        public static DependencyProperty YColumnHeaderProperty = DependencyProperty.Register(nameof(YColumnHeader), typeof(string), typeof(TimeSeriesTable), new FrameworkPropertyMetadata("Y Data"));

        /// <summary>
        /// Gets or sets the column header text for the value (Y) column.
        /// </summary>
        public string YColumnHeader
        {
            get { return (string)GetValue(YColumnHeaderProperty); }
            set { SetValue(YColumnHeaderProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSeriesTable"/> class.
        /// </summary>
        public TimeSeriesTable()
        {
            InitializeComponent();
            TimeSeriesDataGrid.RowType = typeof(SeriesOrdinate<DateTime, double>);
            TimeSeriesDataGrid.PasteAddsRows = true;

            var b = new Binding(nameof(XColumnHeader)) { Source = this, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, FallbackValue = "Date Times", TargetNullValue = "Date Times" };
            BindingOperations.SetBinding((DateTimeColumn), DataGridTemplateColumn.HeaderProperty, b);

            b = new Binding(nameof(YColumnHeader)) { Source = this, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, FallbackValue = "Date Values", TargetNullValue = "Date Values" };
            BindingOperations.SetBinding((ValueColumn), DataGridTemplateColumn.HeaderProperty, b);


            // context menu
            List<MathFunctionType> operands = new List<MathFunctionType>();
            List<MathFunctionType> nonOperands = new List<MathFunctionType>();
            foreach (MathFunctionType fnc in (MathFunctionType[])Enum.GetValues(typeof(MathFunctionType)))
            {
                if (HasOperand(fnc)) { operands.Add(fnc); }
                else { nonOperands.Add(fnc); }
            }

            var x = new MenuItem() { Header = "Calculator", Icon = new Rectangle { Width = 16, Height = 16, Fill = Application.Current.TryFindResource("CalculatorIconBrush") as Brush } };
            foreach (MathFunctionType fnc in operands)
            {
                x.Items.Add(new MenuItem() { Header = MathFunctionTypeToNameConverter.GetName(fnc), Icon = FunctionToImage(fnc), ToolTip = MathFunctionTypeToTooltipConverter.GetTooltip(fnc), Tag = fnc });
                MenuItem h = (MenuItem)x.Items[x.Items.Count - 1];
                h.Click += CalculatorButton_Click;
            }
            foreach (MathFunctionType fnc in nonOperands)
            {
                x.Items.Add(new MenuItem() { Header = MathFunctionTypeToNameConverter.GetName(fnc), Icon = FunctionToImage(fnc), ToolTip = MathFunctionTypeToTooltipConverter.GetTooltip(fnc), Tag = fnc });
                ((MenuItem)x.Items[x.Items.Count - 1]).Click += CalculatorButton_Click;
            }

            TimeSeriesDataGrid.CustomMenuItems.Add(x);
        }

        /// <summary>
        /// Determines whether a mathematical function type requires an operand value.
        /// </summary>
        /// <param name="function">The mathematical function type to check.</param>
        /// <returns><c>true</c> if the function requires an operand; otherwise, <c>false</c>.</returns>
        public static bool HasOperand(MathFunctionType function)
        {
            if (function == MathFunctionType.Add || function == MathFunctionType.Subtract ||
                function == MathFunctionType.Multiply || function == MathFunctionType.Divide ||
                function == MathFunctionType.Logarithm || function == MathFunctionType.Exponentiate ||
                function == MathFunctionType.Replace)
            { return true; }
            else
            { return false; }
        }

        /// <summary>
        /// Converts a math function type to an icon element.
        /// </summary>
        /// <param name="fnc">The math function type.</param>
        /// <returns>A ContentControl containing the function's icon.</returns>
        private ContentControl FunctionToImage(MathFunctionType fnc)
        {
            return new ContentControl { Width = 16, Height = 16, Content = MathFunctionTypeToIconConverter.GetIcon(fnc) };
        }

        /// <summary>
        /// Handles calculator button clicks and applies mathematical operations to selected cells.
        /// </summary>
        /// <param name="sender">The menu item that was clicked.</param>
        /// <param name="e">The routed event arguments.</param>
        private void CalculatorButton_Click(object sender, RoutedEventArgs e)
        {
            var x = sender as MenuItem;
            if (x == null) { return; }
            if (x.Tag == null) { return; }

            var f = (MathFunctionType)x.Tag;

            List<int> ints = new List<int>();
            var selectedCells = TimeSeriesDataGrid.SelectedCells.ToArray();
            foreach (DataGridCellInfo cellInfo in selectedCells)
            {
                if (cellInfo.Column.DisplayIndex <= 1) { continue; }
                ints.Add(Series.IndexOf(cellInfo.Item));
            }

            if (HasOperand(f))
            {
                var w = new NumericEntry() { Owner = Window.GetWindow(this), WindowStartupLocation = WindowStartupLocation.CenterOwner };
                var c = new MathFunctionTypeToNameConverter();
                var cToolTip = new MathFunctionTypeToTooltipConverter();
                w.Title = (string)c.Convert(f, f.GetType(), null, null);
                w.ToolTip = (string)cToolTip.Convert(f, f.GetType(), null, null);
                // Label
                w.ValueTextBox.IsWholeNumber = false;
                w.ValueTextBox.CanHaveNegative = true;
                switch (f)
                {
                    case MathFunctionType.Add:
                        w.NameLabel.Text = "x + "; break;                
                    case MathFunctionType.Subtract:
                        w.NameLabel.Text = "x - "; break;
                    case MathFunctionType.Multiply:
                        w.NameLabel.Text = "x * "; break;
                    case MathFunctionType.Divide:
                        w.NameLabel.Text = "x / ";
                        w.ValueTextBox.Value = 1;
                        break;
                    case MathFunctionType.Logarithm:
                        w.NameLabel.Text = $"log(x)";
                        w.ValueTextBox.Value = 10;
                        w.ValueTextBox.ToolTip = "Base value for the log function.";
                        w.ValueTextBox.IsWholeNumber = true;
                        w.ValueTextBox.CanHaveNegative = false;
                        break;
                    case MathFunctionType.Exponentiate:
                        w.NameLabel.Text = "x^"; break;
                    case MathFunctionType.Replace:
                        w.NameLabel.Text = "x = "; break;
                    default:
                        w.NameLabel.Text = f.ToString(); break;
                }

                if (w.ShowDialog() == true) { MathEditorControl.ApplyFunctionToSeries(Series, f, w.ValueTextBox.Value, ints); }
            }
            else
            {
                MathEditorControl.ApplyFunctionToSeries(Series, f, 0, ints);
            }

            // Reselect cells
            TimeSeriesDataGrid.SelectedCells.Clear();
            foreach (DataGridCellInfo cellInfo in selectedCells)
            {
                TimeSeriesDataGrid.SelectedCells.Add(cellInfo);
            }
        }

        /// <summary>
        /// Handles column header clicks to select all cells in the clicked column.
        /// </summary>
        /// <param name="sender">The column header that was clicked.</param>
        /// <param name="e">The routed event arguments.</param>
        private void DataGridColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            DataGridColumnHeader columnHeader = sender as DataGridColumnHeader;
            if (columnHeader == null) return;
            TimeSeriesDataGrid.SelectedCells.Clear();
            foreach (var item in TimeSeriesDataGrid.Items)
            {
                TimeSeriesDataGrid.SelectedCells.Add(new DataGridCellInfo(item, columnHeader.Column));
            }
        }

        /// <summary>
        /// Handles preview add rows event to create new time series ordinates before they are added to the grid.
        /// </summary>
        /// <param name="startRowIndex">The starting row index for the new rows.</param>
        /// <param name="nRows">The number of rows to add.</param>
        /// <param name="cancelAddRows">Whether to cancel the add rows operation.</param>
        private void TimeSeriesDataGrid_PreviewAddRows(int startRowIndex, int nRows, ref bool cancelAddRows)
        {
            cancelAddRows = true;
            bool _wasSuppressed = Series.SuppressCollectionChanged;
            Series.SuppressCollectionChanged = true;

            // Start Time
            DateTime startTime = (Series == null || Series.Count == 0) ? new DateTime(2020, 1, 1, 0, 0, 0) : Series[0].Index;
            if (startRowIndex == 0)
            {
                for (int i = 0; i < nRows; i++) { startTime = TimeSeries.SubtractTimeInterval(startTime, Series.TimeInterval); }
            }

            for (int i = startRowIndex; i < startRowIndex + nRows; i++)
            {
                Series.Insert(i, new SeriesOrdinate<DateTime, double>(startTime, double.NaN));
            }

            if (Series.TimeInterval != TimeInterval.Irregular) { Series.ShiftAllDates(startTime); }

            Series.SuppressCollectionChanged = _wasSuppressed;
            if (Series.SuppressCollectionChanged == false) { TimeSeriesDataGrid.Items.Refresh(); }
        }

        /// <summary>
        /// Handles the math popup opened event and initializes the math editor control.
        /// </summary>
        /// <param name="sender">The popup that was opened.</param>
        /// <param name="e">The event arguments.</param>
        private void MathPopup_Opened(object sender, EventArgs e)
        {
            if (sender == null || sender.GetType() != typeof(Popup)) { return; }
            Popup mathPopup = (Popup)sender;
            if (mathPopup.DataContext == null) { return; }
            if (mathPopup.DataContext.GetType() != typeof(TimeSeriesTable)) { return; }
            // 
            Border border = mathPopup.Child as Border;
            if (border == null) { return; }
            MathEditorControl picker = border.Child as MathEditorControl;
            if (picker == null) { return; }
            // 
            picker.Series = Series;
            picker.Source = TimeSeriesDataGrid;

            if (!picker.ValueTextBox.IsKeyboardFocusWithin) { picker.ValueTextBox.Focus(); }
            picker.ValueTextBox.SelectAll();
        }

        /// <summary>
        /// Handles preview key down events to handle the Delete key for clearing cell values.
        /// </summary>
        /// <param name="sender">The data grid.</param>
        /// <param name="e">The key event arguments.</param>
        private void TimeSeriesDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var grid = (CopyPasteDataGrid)sender;
            if (Key.Delete == e.Key)
            {
                foreach (var cell in grid.SelectedCells)
                {
                    if (cell.Column.IsReadOnly == false) 
                    {
                        var ord = cell.Item as SeriesOrdinate<DateTime, double>;
                        if(ord != null) 
                        { 
                            if(cell.Column.DisplayIndex==1)
                            {
                                ord.Index = new DateTime(0001, 1, 1, 0, 0, 0);
                            }
                            else if(cell.Column.DisplayIndex==2)
                            {
                                ord.Value = double.NaN;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles rows added to the grid and re-enables collection changed notifications.
        /// </summary>
        /// <param name="startRowIndex">The starting row index of added rows.</param>
        /// <param name="nRows">The number of rows added.</param>
        private void TimeSeriesDataGrid_RowsAdded(int startRowIndex, int nRows)
        {
            Series.SuppressCollectionChanged = false;
            Series.RaiseCollectionChangedReset();
        }

        /// <summary>
        /// Handles preview delete rows event to suppress collection changed notifications during deletion.
        /// </summary>
        /// <param name="rowindices">The indices of rows to be deleted.</param>
        /// <param name="cancel">Whether to cancel the delete operation.</param>
        private void TimeSeriesDataGrid_PreviewDeleteRows(List<int> rowindices, ref bool cancel)
        {
            Series.SuppressCollectionChanged = true;
        }

        /// <summary>
        /// Handles rows deleted from the grid and re-enables collection changed notifications.
        /// </summary>
        /// <param name="rowindices">The indices of deleted rows.</param>
        private void TimeSeriesDataGrid_RowsDeleted(List<int> rowindices)
        {
            Series.SuppressCollectionChanged = false;
            Series.RaiseCollectionChangedReset();
        }

        /// <summary>
        /// Handles preview paste data event to suppress collection changed notifications during paste.
        /// </summary>
        /// <param name="clipboardData">The clipboard data being pasted.</param>
        /// <param name="cancelPaste">Whether to cancel the paste operation.</param>
        private void TimeSeriesDataGrid_PreviewPasteData(string[][] clipboardData, ref bool cancelPaste)
        {
            Series.SuppressCollectionChanged = true;
        }

        /// <summary>
        /// Handles data pasted into the grid and re-enables collection changed notifications.
        /// </summary>
        private void TimeSeriesDataGrid_DataPasted()
        {
            Series.SuppressCollectionChanged = false;
            Series.RaiseCollectionChangedReset();
        }

    }

    /// <summary>
    /// Converts a double value to a font style, displaying NaN and Infinity values in italic.
    /// </summary>
    public class DoubleToFontFamilyConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.IsNaN((double)value) || double.IsInfinity((double)value)) { return FontStyles.Italic; }
            return FontStyles.Normal;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts between <see cref="DateTime"/> values and their string representations using the current culture's date/time format.
    /// </summary>
    public class DateToStringConverter : IValueConverter
    {
        private readonly string _pattern = $"{Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern} {Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortTimePattern}";
        private readonly CultureInfo _fp = Thread.CurrentThread.CurrentCulture;

        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString(_pattern, _fp);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime newDate;
            if (DateTime.TryParseExact((string)value, _pattern, _fp, DateTimeStyles.None, out newDate) == false) { DateTime.TryParse((string)value, out newDate); }

            return newDate;
        }
    }
}
