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
using System.Windows.Controls;

namespace NumericControls
{
    /// <summary>
    /// A user control for editing XY paired data in a data grid format.
    /// Provides functionality for entering and managing two-dimensional data points.
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
    public partial class XYDataGridControl : UserControl
    {
        public XYDataGridControl()
        {

            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            // XYDataGridPlus.DataGrid = New DataGrid
            // XYDataGridPlus.CanUserAddInsertDeleteRows = True
            // XYDataGridPlus.PasteAddsRows = True

            XYDataList.Add(new XYDataPoint(1d, 4d));
            XYDataList.Add(new XYDataPoint(2d, 5d));
            XYDataList.Add(new XYDataPoint(3d, 6d));
            XYDataTable.ItemsSource = XYDataList;

            // With XYDataGridPlus.DataGrid

            // .ItemsSource = XYDataList

            // .HeadersVisibility = DataGridHeadersVisibility.All
            // .CanUserResizeColumns = False
            // .HorizontalGridLinesBrush = New SolidColorBrush(CType(New BrushConverter().ConvertFrom("#FF353B7A"), Color))
            // .VerticalGridLinesBrush = New SolidColorBrush(CType(New BrushConverter().ConvertFrom("#FF353B7A"), Color))
            // .AutoGenerateColumns = False
            // .SelectionMode = DataGridSelectionMode.Extended
            // .SelectionUnit = DataGridSelectionUnit.CellOrRowHeader
            // .Margin = New Thickness(2, 4, 2, 2)
            // .CanUserDeleteRows = True
            // .CanUserAddRows = True
            // .CanUserResizeRows = False
            // .CanUserSortColumns = False
            // .CanUserReorderColumns = False
            // .Background = New SolidColorBrush(Colors.Transparent)
            // .VerticalContentAlignment = VerticalAlignment.Center

            // Dim XValCol As New DataGridTextColumn
            // With XValCol
            // .Header = "X Values"
            // .IsReadOnly = False
            // .Width = 100
            // .Binding = New Binding("X")
            // End With

            // Dim YValCol As New DataGridTextColumn
            // With YValCol
            // .Header = "Y Values"
            // .IsReadOnly = False
            // .Width = 100
            // .Binding = New Binding("Y")
            // End With

            // .Columns.Add(XValCol)
            // .Columns.Add(YValCol)

            // End With



        }

        public ObservableCollection<object> XYDataList = new ObservableCollection<object>();
    }

    /// <summary>
    /// Represents a single XY data point with property change notification.
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
    public class XYDataPoint : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the XYDataPoint class with default values.
        /// </summary>
        public XYDataPoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the XYDataPoint class with specified X and Y values.
        /// </summary>
        /// <param name="xValue">The X coordinate value.</param>
        /// <param name="yValue">The Y coordinate value.</param>
        public XYDataPoint(double xValue, double yValue)
        {
            X = xValue;
            Y = yValue;
        }

        private double _x;
        private double _y;

        /// <summary>
        /// Gets or sets the X coordinate value.
        /// </summary>
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(value)));
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate value.
        /// </summary>
        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(value)));
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
