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

using Numerics.Data;
using Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NumericControls
{
    /// <summary>
    /// A user control for editing uncertain ordered paired data in a table format with distribution selection.
    /// Combines a distribution selector combobox with a table editor for managing uncertainty in paired data.
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
    public partial class UncertainOrderedDataTableEditor : UserControl
    {
        /// <summary>
        /// Identifies the <see cref="AddRemoveRows"/> dependency property.
        /// </summary>
        public static DependencyProperty AddRemoveRowsProperty = DependencyProperty.Register(nameof(AddRemoveRows), typeof(bool), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether users can add or remove rows.
        /// </summary>
        public bool AddRemoveRows
        {
            get { return (bool)GetValue(AddRemoveRowsProperty); }
            set { SetValue(AddRemoveRowsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="XColumnHeader"/> dependency property.
        /// </summary>
        public static DependencyProperty XColumnHeaderProperty = DependencyProperty.Register(nameof(XColumnHeader), typeof(string), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata("X Data"));

        /// <summary>
        /// Gets or sets the column header text for the X (independent variable) column.
        /// </summary>
        public string XColumnHeader
        {
            get { return (string)GetValue(XColumnHeaderProperty); }
            set { SetValue(XColumnHeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="YColumnHeader"/> dependency property.
        /// </summary>
        public static DependencyProperty YColumnHeaderProperty = DependencyProperty.Register(nameof(YColumnHeader), typeof(string), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata("Y Data"));

        /// <summary>
        /// Gets or sets the column header text for the Y (distribution parameters) columns.
        /// </summary>
        public string YColumnHeader
        {
            get { return (string)GetValue(YColumnHeaderProperty); }
            set { SetValue(YColumnHeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsStrictX"/> dependency property.
        /// </summary>
        public static DependencyProperty IsStrictXProperty = DependencyProperty.Register(nameof(IsStrictX), typeof(bool), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(false, PropertyChanged_Callback));

        /// <summary>
        /// Gets or sets a value indicating whether X values must be strictly ordered (no duplicates allowed).
        /// </summary>
        public bool IsStrictX
        {
            get { return (bool)GetValue(IsStrictXProperty); }
            set { SetValue(IsStrictXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsStrictY"/> dependency property.
        /// </summary>
        public static DependencyProperty IsStrictYProperty = DependencyProperty.Register(nameof(IsStrictY), typeof(bool), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(false, PropertyChanged_Callback));

        /// <summary>
        /// Gets or sets a value indicating whether Y values must be strictly ordered (no duplicates allowed).
        /// </summary>
        public bool IsStrictY
        {
            get { return (bool)GetValue(IsStrictYProperty); }
            set { SetValue(IsStrictYProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OrderX"/> dependency property.
        /// </summary>
        public static DependencyProperty OrderXProperty = DependencyProperty.Register(nameof(OrderX), typeof(SortOrder), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(SortOrder.Ascending, PropertyChanged_Callback));

        /// <summary>
        /// Gets or sets the required sort order for X values.
        /// </summary>
        public SortOrder OrderX
        {
            get { return (SortOrder)GetValue(OrderXProperty); }
            set { SetValue(OrderXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="OrderY"/> dependency property.
        /// </summary>
        public static DependencyProperty OrderYProperty = DependencyProperty.Register(nameof(OrderY), typeof(SortOrder), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(SortOrder.Ascending, PropertyChanged_Callback));

        /// <summary>
        /// Gets or sets the required sort order for Y values.
        /// </summary>
        public SortOrder OrderY
        {
            get { return (SortOrder)GetValue(OrderYProperty); }
            set { SetValue(OrderYProperty, value); }
        }

        private static void PropertyChanged_Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UncertainOrderedDataTableEditor thisControl = (UncertainOrderedDataTableEditor)d;
            // 
            foreach (var dist in thisControl._distributions)
            {
                dist.OrderX = thisControl.OrderX;
                dist.OrderY = thisControl.OrderY;
                dist.StrictX = thisControl.IsStrictX;
                dist.StrictY = thisControl.IsStrictY;
            }
        }

        /// <summary>
        /// Identifies the <see cref="MaximumX"/> dependency property.
        /// </summary>
        public static DependencyProperty MaximumXProperty = DependencyProperty.Register(nameof(MaximumX), typeof(double), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or sets the maximum allowed X value for validation.
        /// </summary>
        public double MaximumX
        {
            get { return (double)GetValue(MaximumXProperty); }
            set { SetValue(MaximumXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MinimumX"/> dependency property.
        /// </summary>
        public static DependencyProperty MinimumXProperty = DependencyProperty.Register(nameof(MinimumX), typeof(double), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(double.MinValue));

        /// <summary>
        /// Gets or sets the minimum allowed X value for validation.
        /// </summary>
        public double MinimumX
        {
            get { return (double)GetValue(MinimumXProperty); }
            set { SetValue(MinimumXProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="MaximumY"/> dependency property.
        /// </summary>
        public static DependencyProperty MaximumYProperty = DependencyProperty.Register(nameof(MaximumY), typeof(double), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(double.MaxValue));

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
        public static DependencyProperty MinimumYProperty = DependencyProperty.Register(nameof(MinimumY), typeof(double), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(double.MinValue));

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
        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the control is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShowToolBar"/> dependency property.
        /// </summary>
        public static DependencyProperty ShowToolBarProperty = DependencyProperty.Register(nameof(ShowToolBar), typeof(bool), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the toolbar is visible.
        /// </summary>
        public bool ShowToolBar
        {
            get { return (bool)GetValue(ShowToolBarProperty); }
            set { SetValue(ShowToolBarProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ColumnHeaderStyle"/> dependency property.
        /// </summary>
        public static DependencyProperty ColumnHeaderStyleProperty = DependencyProperty.Register(nameof(ColumnHeaderStyle), typeof(Style), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style for column headers in the data grid.
        /// </summary>
        public Style ColumnHeaderStyle
        {
            get { return (Style)GetValue(ColumnHeaderStyleProperty); }
            set { SetValue(ColumnHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="CellStyle"/> dependency property.
        /// </summary>
        public static DependencyProperty CellStyleProperty = DependencyProperty.Register(nameof(CellStyle), typeof(Style), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the style for cells in the data grid.
        /// </summary>
        public Style CellStyle
        {
            get { return (Style)GetValue(CellStyleProperty); }
            set { SetValue(CellStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DistributionSelectorMaxWidth"/> dependency property.
        /// </summary>
        public static DependencyProperty DistributionSelectorMaxWidthProperty = DependencyProperty.Register(nameof(DistributionSelectorMaxWidth), typeof(double), typeof(UncertainOrderedDataTableEditor), new FrameworkPropertyMetadata(double.MaxValue));

        /// <summary>
        /// Gets or sets the maximum width of the distribution selector control.
        /// </summary>
        public double DistributionSelectorMaxWidth
        {
            get { return (double)GetValue(DistributionSelectorMaxWidthProperty); }
            set { SetValue(DistributionSelectorMaxWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedUncertainOrderedData"/> dependency property.
        /// </summary>
        public static DependencyProperty SelectedUncertainOrderedDataProperty = DependencyProperty.Register(nameof(SelectedUncertainOrderedData), typeof(UncertainOrderedPairedData), typeof(UncertainOrderedDataTableEditor), new PropertyMetadata(null, SelectedUncertainData_Callback));
        private bool _settingSelected = false;

        private static void SelectedUncertainData_Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(UncertainOrderedDataTableEditor)) return;
            UncertainOrderedDataTableEditor thisControl = (UncertainOrderedDataTableEditor)d;
            if (thisControl._updatingSelected == true) return;
            // Using collection changed to update the other distribution options caused issues with event handlers not being let go when the control was closed.

            UncertainOrderedPairedData newData = e.NewValue as UncertainOrderedPairedData;
            if (newData == null) return;

            // Get distribution index
            int index = -1;
            for (int i = 0; i < thisControl._distributions.Count; i++)
            {
                if (thisControl._distributions[i].Distribution == newData.Distribution)
                {
                    index = i;
                    break;
                }
            }
            // Check if current distribution is supported
            if (index == -1)
            {
                thisControl.CurveUncertaintyComboBox.SelectedIndex = -1;
                return;
            }
            // 
            // If thisControl._distributions(index).Equals(newData) = False Then
            thisControl._settingSelected = true;
            thisControl.CurveUncertaintyComboBox.SelectedIndex = -1;
            thisControl._distributions[index] = newData;
            thisControl.CurveUncertaintyComboBox.SelectedIndex = index;
            for (int i = 0; i < thisControl._distributions.Count; i++)
            {
                if (i == index)
                    continue;
                thisControl._distributions[i] = thisControl._distributions[i].Clone();
            }

            thisControl._settingSelected = false;
        }

        private bool _updatingSelected = false;

        private void CurveUncertaintyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_settingSelected == true) return;
            _updatingSelected = true;
            // 
            if (CurveUncertaintyComboBox.SelectedIndex >= 0)
            {
                SelectedUncertainOrderedData = _distributions[CurveUncertaintyComboBox.SelectedIndex];
            }
            else
            {
                SelectedUncertainOrderedData = null;
            }

            _updatingSelected = false;
        }

        /// <summary>
        /// Get and set the selected probability distribution.
        /// </summary>
        public UncertainOrderedPairedData SelectedUncertainOrderedData
        {
            get { return (UncertainOrderedPairedData)GetValue(SelectedUncertainOrderedDataProperty); }
            set { SetValue(SelectedUncertainOrderedDataProperty, value); }
        }

        private ObservableCollection<UncertainOrderedPairedData> _distributions = new ObservableCollection<UncertainOrderedPairedData>();

        /// <summary>
        /// Dependency property for the control distribution options.
        /// </summary>
        public static DependencyProperty DistributionOptionsProperty = DependencyProperty.Register(nameof(DistributionOptions), typeof(IList<UnivariateDistributionType>), typeof(UncertainOrderedDataTableEditor), new PropertyMetadata(null, DistributionOptionsCallback));

        /// <summary>
        /// Gets and sets the distribution options.
        /// </summary>
        public IList<UnivariateDistributionType> DistributionOptions
        {
            get { return (IList<UnivariateDistributionType>)GetValue(DistributionOptionsProperty); }
            set { SetValue(DistributionOptionsProperty, value); }
        }

        /// <summary>
        /// When the dependency property changes, this sets the distribution options.
        /// </summary>
        private static void DistributionOptionsCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UncertainOrderedDataTableEditor thisControl = (UncertainOrderedDataTableEditor)d;
            UnivariateDistributionBase distribution;
            IList<UnivariateDistributionType> distOptions = e.NewValue as IList<UnivariateDistributionType>;
            if (e.NewValue == null || distOptions == null)
            {
                thisControl.DistributionOptions = DefaultDistributionOptions;
            }
            else
            {
                // clear any distributions that should go
                var toDelete = new List<UncertainOrderedPairedData>();
                foreach (var dist in thisControl._distributions)
                {
                    if (distOptions.Contains(dist.Distribution) == false)
                        toDelete.Add(dist);
                }

                foreach (var item in toDelete)
                    thisControl._distributions.Remove(item);
                // add any new ones.
                foreach (var dist in distOptions)
                {
                    if (thisControl._distributions.Any(o => o.Distribution == dist)) continue;
                    // 
                    distribution = UnivariateDistributionFactory.CreateDistribution(dist);
                    if (distribution == null) continue;
                    // 
                    var ordinates = new List<UncertainOrdinate>();
                    for (int i = 0; i <= 1; i++)
                        ordinates.Add(new UncertainOrdinate(i, UnivariateDistributionFactory.CreateDistribution(dist)));
                    // 
                    var uncertainData = new UncertainOrderedPairedData(ordinates, thisControl.IsStrictX, thisControl.OrderX, thisControl.IsStrictY, thisControl.OrderY, dist);
                    thisControl._distributions.Add(uncertainData);
                }
            }
            // 
        }

        /// <summary>
        /// Currently does not support bivariate, empirical, or kernel density.
        /// </summary>
        /// <returns></returns>
        public static List<UnivariateDistributionType> DefaultDistributionOptions
        {
            get
            {
                return ((UnivariateDistributionType[])Enum.GetValues(typeof(UnivariateDistributionType))).Where(o => (o != UnivariateDistributionType.Bernoulli) &
                                                                                                                     (o != UnivariateDistributionType.Beta) &
                                                                                                                     (o != UnivariateDistributionType.Binomial) &
                                                                                                                     (o != UnivariateDistributionType.Cauchy) &
                                                                                                                     (o != UnivariateDistributionType.ChiSquared) &
                                                                                                                     (o != UnivariateDistributionType.CompetingRisks) &
                                                                                                                     (o != UnivariateDistributionType.Empirical) &
                                                                                                                     (o != UnivariateDistributionType.Geometric) &
                                                                                                                     (o != UnivariateDistributionType.InverseChiSquared) &
                                                                                                                     (o != UnivariateDistributionType.InverseGamma) &
                                                                                                                     (o != UnivariateDistributionType.KappaFour) &
                                                                                                                     (o != UnivariateDistributionType.KernelDensity) &
                                                                                                                     (o != UnivariateDistributionType.Mixture) &
                                                                                                                     (o != UnivariateDistributionType.NoncentralT) &
                                                                                                                     (o != UnivariateDistributionType.UniformDiscrete) &
                                                                                                                     (o != UnivariateDistributionType.Poisson)).ToList();
            }
        }

        public event ColumnsAutoGeneratedEventHandler ColumnsAutoGenerated;

        public delegate void ColumnsAutoGeneratedEventHandler(object sender, ObservableCollection<DataGridColumn> e);

        public UncertainOrderedDataTableEditor()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
            ColumnHeaderStyle = (Style)FindResource("WrappedColumnHeaderStyle");
            DistributionOptions = DefaultDistributionOptions;
            CurveUncertaintyComboBox.ItemsSource = _distributions;
            UncertainTableEditor.ValidationGrid.AutoGeneratedColumns += (object sender, EventArgs e) => ColumnsAutoGenerated?.Invoke(sender, UncertainTableEditor.ValidationGrid.Columns);
        }

        public void ForceGridValidation()
        {
            UncertainTableEditor.ForceGridValidation(); 
        }

        public void Refresh()
        {
            UncertainTableEditor.Refresh();
        }

    }

    /// <summary>
    /// Converts UncertainOrderedPairedData objects to their distribution display names for UI binding.
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
    public class DistributionNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            UncertainOrderedPairedData val = value as UncertainOrderedPairedData;
            if (val == null) return "";
            if (val.Count > 0) return val[0].Y.DisplayName;
            return UnivariateDistributionFactory.CreateDistribution(val.Distribution).DisplayName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

}
