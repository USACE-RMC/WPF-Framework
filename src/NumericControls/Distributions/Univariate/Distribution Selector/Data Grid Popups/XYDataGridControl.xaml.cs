using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace NumericControls
{
    /// <summary>
    /// Interaction logic for XYDataGridControl.xaml
    /// </summary>
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

    public class XYDataPoint : INotifyPropertyChanged
    {
        public XYDataPoint()
        {
        }

        public XYDataPoint(double xValue, double yValue)
        {
            X = xValue;
            Y = yValue;
        }

        private double _x;
        private double _y;

        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(value)));
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(value)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
