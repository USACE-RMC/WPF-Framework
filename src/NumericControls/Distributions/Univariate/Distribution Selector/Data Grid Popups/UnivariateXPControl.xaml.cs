using Numerics.Distributions;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace NumericControls
{
    /// <summary>
    /// Interaction logic for UnivariateXPControl.xaml
    /// </summary>
    public partial class UnivariateXPControl : UserControl
    {
        private ObservableCollection<object> _univariateRowData = new ObservableCollection<object>();

        /// <summary>
        /// Dependency property for the selected distribution.
        /// </summary>
        public static DependencyProperty DistributionProperty = DependencyProperty.Register(nameof(Distribution), typeof(EmpiricalDistribution), typeof(UnivariateXPControl), new PropertyMetadata(new EmpiricalDistribution(), SetDistribution));

        private static void SetDistribution(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(UnivariateXPControl)) return;
            UnivariateXPControl thisControl = (UnivariateXPControl)d;
            thisControl._univariateRowData.Clear();
            // 
            if (e.NewValue == null) return;
            EmpiricalDistribution newDistribution = e.NewValue as EmpiricalDistribution;
            if (newDistribution == null) return;
            // 
            var xVals = newDistribution.XValues;
            var yVals = newDistribution.ProbabilityValues;
            // 
            thisControl._univariateRowData.Clear();
            for (int i = 0; i < xVals.Count; i++)
                thisControl._univariateRowData.Add(new UnivariateDistributionValidatingRow(xVals[i], yVals[i], newDistribution.Minimum, newDistribution.Maximum, thisControl._univariateRowData));
        }

        /// <summary>
        /// Get and set the selected probability distribution.
        /// </summary>
        public EmpiricalDistribution Distribution
        {
            get { return (EmpiricalDistribution)GetValue(DistributionProperty); }
            set { SetValue(DistributionProperty, value); }
        }

        public UnivariateXPControl()
        {

            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            UnivariateGrid.ItemsSource = _univariateRowData;
        }

        private void UnivariateGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.HeaderStyle = (Style)Resources["WrappedColumnHeaderStyle"];
        }

        private void ColumnHeaderClick(object sender, RoutedEventArgs e)
        {
            DataGridColumnHeader columnHeader = sender as DataGridColumnHeader;
            if (columnHeader == null) return;
            // 
            UnivariateGrid.SelectedCells.Clear();
            foreach (var item in UnivariateGrid.Items)
                UnivariateGrid.SelectedCells.Add(new DataGridCellInfo(item, columnHeader.Column));
        }
    }
}
