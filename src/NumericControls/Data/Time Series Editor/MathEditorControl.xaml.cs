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
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Numerics.Data.Statistics.Histogram;

namespace NumericControls
{
    /// <summary>
    /// A user control for applying mathematical operations to time series data.
    /// Provides a UI for selecting mathematical functions and applying them to selected cells or entire time series.
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
    public partial class MathEditorControl : UserControl
    {
        private CopyPasteDataGrid _source = null;
        private TimeSeries _series = null;
        private DataGridCellInfo[] _selectedCells = null;
        private List<int> _selectedValueRowIndices = new List<int>();
        private bool _selectionConsecutive = false;

        public MathEditorControl()
        {
            InitializeComponent();

            List<MathFunctionType> operands = new List<MathFunctionType>();
            List<MathFunctionType> nonOperands = new List<MathFunctionType>();
            foreach (MathFunctionType fnc in (MathFunctionType[])Enum.GetValues(typeof(MathFunctionType)))
            {
                if (fnc == MathFunctionType.Add || fnc == MathFunctionType.Subtract ||
                    fnc == MathFunctionType.Multiply || fnc == MathFunctionType.Divide ||
                    fnc == MathFunctionType.Logarithm || fnc == MathFunctionType.Exponentiate)
                { operands.Add(fnc); }
                else
                { nonOperands.Add(fnc); }
            }

            OperandItemsControl.ItemsSource = operands;
            NoOperandItemsControl.ItemsSource = nonOperands;
        }

        public TimeSeries Series { get => _series; set => _series = value; }


        public CopyPasteDataGrid Source
        {
            get => _source;
            set
            {
                _source = value;
                _selectedValueRowIndices.Clear();
                _selectedCells = _source.SelectedCells.ToArray();
                foreach (DataGridCellInfo cellInfo in _selectedCells)
                {
                    if (cellInfo.Column.DisplayIndex <= 1) { continue; }
                    _selectedValueRowIndices.Add(_series.IndexOf(cellInfo.Item));
                }
                _selectionConsecutive = false;
                if (_selectedValueRowIndices.Count > 0 && _selectedValueRowIndices.Count < _series.Count)
                {
                    _selectedValueRowIndices.Sort();
                    _selectionConsecutive = true;
                    for (int i = 0; i < _selectedValueRowIndices.Count; i++)
                    {
                        if (_selectedValueRowIndices[i] - i != _selectedValueRowIndices[0]) { _selectionConsecutive = false; break; }
                    }

                    if (_selectionConsecutive)
                    {
                        NotificationText.Text = $"Applies to rows {_selectedValueRowIndices[0] + 1} - {_selectedValueRowIndices[_selectedValueRowIndices.Count - 1] + 1}.";// +
                                                                                                                                                                           //$"{Environment.NewLine}" +
                                                                                                                                                                           //$"Selection is {(_selectionConsecutive == true ? "continuous" : "discontinuous")}." +
                                                                                                                                                                           //$"{Environment.NewLine}" +
                                                                                                                                                                           //$"(Rows {_selectedValueRowIndices[0] + 1} - {_selectedValueRowIndices[_selectedValueRowIndices.Count - 1] + 1})";
                    }
                    else
                    {
                        string rowsString = string.Empty;
                        if (_selectedValueRowIndices.Count > 8)
                        {
                            rowsString = $"{_selectedValueRowIndices[0] + 1}, {_selectedValueRowIndices[1] + 1},...,{_selectedValueRowIndices[_selectedValueRowIndices.Count - 1] + 1}";
                        }
                        else
                        {
                            rowsString = string.Join(", ", _selectedValueRowIndices.Select(item => item + 1));
                        }
                        NotificationText.Text = $"Applies to rows {rowsString}.";// +
                                                                                 //$"{Environment.NewLine}" +
                                                                                 //$"Selection is {(_selectionConsecutive == true ? "continuous" : "discontinuous")}." +
                                                                                 //$"{Environment.NewLine}" +
                                                                                 //$"(Rows {rowsString})";
                    }
                }
                else
                {
                    NotificationText.Text = "Applies to all.";
                }
            }
        }

        /// <summary>
        /// Handles the click event for a math function button and applies the selected mathematical operation.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">The routed event arguments.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) { return; }
            var cntxt = btn.DataContext;
            if (cntxt == null || cntxt.GetType() != typeof(MathFunctionType)) { return; }
            //
            if (_source == null || _series == null) { return; }
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                MathFunctionType functionType = (MathFunctionType)cntxt;

                if (ValueTextBox.ValueIsValid == false)
                {
                    //update text notification
                    NotificationText.Text = "*Operand is not valid for this operation.";
                }

                double value = ValueTextBox.Value;

                ApplyFunctionToSeries(_series, functionType, value, _selectedValueRowIndices);

                // Reselect cells
                foreach (DataGridCellInfo cellInfo in _selectedCells)
                {
                    _source.SelectedCells.Add(cellInfo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

        }

        public static void ApplyFunctionToSeries(TimeSeries series, MathFunctionType functionType, double value, List<int> indices)
        {
            //
            if (series == null) { return; }

            try
            {
                if (indices == null || indices.Count == 0 || indices.Count == series.Count)
                {
                    if (functionType == MathFunctionType.Add)
                    {
                        series.Add(value);
                    }
                    else if (functionType == MathFunctionType.Subtract)
                    {
                        series.Subtract(value);
                    }
                    else if (functionType == MathFunctionType.Multiply)
                    {
                        series.Multiply(value);
                    }
                    else if (functionType == MathFunctionType.Divide)
                    {
                        series.Divide(value);
                    }
                    else if (functionType == MathFunctionType.Exponentiate)
                    {
                        series.Exponentiate(value);
                    }
                    else if (functionType == MathFunctionType.Logarithm)
                    {
                        series.LogTransform(value);
                    }
                    else if (functionType == MathFunctionType.Inverse)
                    {
                        series.Inverse();
                    }
                    else if (functionType == MathFunctionType.Replace)
                    {
                        series.ReplaceMissingData(value);
                    }
                    else if (functionType == MathFunctionType.Interpolate)
                    {
                        series.InterpolateMissingData(series.Count);
                    }
                }
                else
                {
                    if (functionType == MathFunctionType.Add)
                    {
                        series.Add(value, indices);
                    }
                    else if (functionType == MathFunctionType.Subtract)
                    {
                        series.Subtract(value, indices);
                    }
                    else if (functionType == MathFunctionType.Multiply)
                    {
                        series.Multiply(value, indices);
                    }
                    else if (functionType == MathFunctionType.Divide)
                    {
                        series.Divide(value, indices);
                    }
                    else if (functionType == MathFunctionType.Exponentiate)
                    {
                        series.Exponentiate(value, indices);
                    }
                    else if (functionType == MathFunctionType.Logarithm)
                    {
                        series.LogTransform(indices, value);
                    }
                    else if (functionType == MathFunctionType.Inverse)
                    {
                        series.Inverse(indices);
                    }
                    else if (functionType == MathFunctionType.Replace)
                    {
                        series.ReplaceMissingData(indices, value);
                    }
                    else if (functionType == MathFunctionType.Interpolate)
                    {
                        series.InterpolateMissingData(indices.Count, indices);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

        }
    }

    public class MathFunctionTypeToNameConverter : IValueConverter
    {
        public static string GetName(MathFunctionType fnc)
        {
            switch (fnc)
            {
                case MathFunctionType.Logarithm: return "Logarithmic Transform";
                default: return fnc.ToString();
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(MathFunctionType)) { return null; }

            return GetName((MathFunctionType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MathFunctionTypeToTooltipConverter : IValueConverter
    {
        public static string GetTooltip(MathFunctionType fnc)
        {
            switch (fnc)
            {
                case MathFunctionType.Add: return "Add a constant to values. Missing values are kept as missing.";
                case MathFunctionType.Subtract: return "Subtract a constant from values. Missing values are kept as missing.";
                case MathFunctionType.Multiply: return "Multiply values by a constant. Missing values are kept as missing.";
                case MathFunctionType.Divide: return "Divide values by a constant. Missing values are kept as missing.";
                case MathFunctionType.Exponentiate: return "Raise values to a constant power. Missing values are kept as missing.";
                case MathFunctionType.Logarithm: return "Log transform values in a specified base. Missing values are kept as missing.";
                case MathFunctionType.Inverse: return "Replace values by its inverse (1/x). Missing values are kept as missing. Zero values are set to missing.";
                case MathFunctionType.Replace: return "Replace missing data (Double.NaN) with a constant.";
                case MathFunctionType.Interpolate: return "Interpolate missing data.";
                default: return fnc.ToString();
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(MathFunctionType)) { return null; }

            return GetTooltip((MathFunctionType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MathFunctionTypeToIconConverter : IValueConverter
    {
        public static BitmapImage GetIcon(MathFunctionType fnc)
        {
            switch (fnc)
            {
                case MathFunctionType.Add: return new BitmapImage(new Uri("pack://application:,,,/NumericControls;component/Resources/CalculatorPlus_16x.png"));
                case MathFunctionType.Subtract: return new BitmapImage(new Uri("pack://application:,,,/NumericControls;component/Resources/CalculatorMinus_16x.png"));
                case MathFunctionType.Multiply: return new BitmapImage(new Uri("pack://application:,,,/NumericControls;component/Resources/CalculatorMultiply_16x.png"));
                case MathFunctionType.Divide: return new BitmapImage(new Uri("pack://application:,,,/NumericControls;component/Resources/CalculatorDivide_16x.png"));
                case MathFunctionType.Exponentiate: return new BitmapImage(new Uri("pack://application:,,,/NumericControls;component/Resources/CalculatorExp_16x.png"));
                case MathFunctionType.Logarithm: return new BitmapImage(new Uri("pack://application:,,,/NumericControls;component/Resources/CalculatorLog_16x.png"));
                case MathFunctionType.Inverse: return new BitmapImage(new Uri("pack://application:,,,/NumericControls;component/Resources/CalculatorInvert_16x.png"));
                case MathFunctionType.Replace: return new BitmapImage(new Uri("pack://application:,,,/NumericControls;component/Resources/CalculatorReplace_16x.png"));
                case MathFunctionType.Interpolate: return new BitmapImage(new Uri("pack://application:,,,/NumericControls;component/Resources/CalculatorInterpolate_16x.png"));
                default: return new BitmapImage(new Uri("pack://application:,,,/NumericControls;component/Resources/Calculator_16x.png"));
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(MathFunctionType)) { return null; }

            return GetIcon((MathFunctionType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
