using Numerics.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NumericControls.Distributions.Univariate
{
    /// <summary>
    /// Interaction logic for DistributionWithSelectorControl.xaml
    /// </summary>
    public partial class DistributionWithSelectorControl : UserControl
    {
        public DistributionWithSelectorControl()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Dependency property for the selected distribution.
        /// </summary>
        public static DependencyProperty SelectedDistributionProperty = DependencyProperty.Register(nameof(SelectedDistribution), typeof(UnivariateDistributionBase), typeof(DistributionWithSelectorControl), new PropertyMetadata(new Normal(), SetDistribution));
        /// <summary>
        /// Get and set the selected probability distribution.
        /// </summary>
        public UnivariateDistributionBase SelectedDistribution
        {
            get { return (UnivariateDistributionBase)GetValue(SelectedDistributionProperty); }
            set { SetValue(SelectedDistributionProperty, value); }
        }

        /// <summary>
        /// Dependency property for showing the distribution pdf plot.
        /// </summary>
        public static DependencyProperty ShowPlotProperty = DependencyProperty.Register(nameof(ShowPlot), typeof(bool), typeof(DistributionWithSelectorControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets and sets the distribution options.
        /// </summary>
        public bool ShowPlot
        {
            get { return (bool)GetValue(ShowPlotProperty); }
            set { SetValue(ShowPlotProperty, value); }
        }

        /// <summary>
        /// Dependency property for showing the distribution summary statistics.
        /// </summary>
        public static DependencyProperty ShowStatisticsProperty = DependencyProperty.Register(nameof(ShowStatistics), typeof(bool), typeof(DistributionWithSelectorControl), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets and sets the distribution options.
        /// </summary>
        public bool ShowStatistics
        {
            get { return (bool)GetValue(ShowStatisticsProperty); }
            set { SetValue(ShowStatisticsProperty, value); }
        }

        /// <summary>
        /// Dependency property for the control distribution options.
        /// </summary>
        public static DependencyProperty DistributionsProperty = DependencyProperty.Register(nameof(Distributions), typeof(IList<UnivariateDistributionBase>), typeof(DistributionWithSelectorControl), new PropertyMetadata(DistributionSelectorControl.DefaultDistributions));

        /// <summary>
        /// Gets and sets the distribution options.
        /// </summary>
        public List<UnivariateDistributionBase> Distributions
        {
            get { return (List<UnivariateDistributionBase>)GetValue(DistributionsProperty); }
            set { SetValue(DistributionsProperty, value); }
        }

        /// <summary>
        /// When the dependency property changes, this sets the distribution.
        /// </summary>
        private static void SetDistribution(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(DistributionWithSelectorControl)) return;
            DistributionWithSelectorControl thisControl = (DistributionWithSelectorControl)d;
            // 
            if (e.NewValue == null)
            {
                thisControl.DistributionTextBlock.Text = "No Distribution Selected"; // .DistributionCombobox.SelectedIndex = -1
            }
            else
            {
                UnivariateDistributionBase newDistribution = e.NewValue as UnivariateDistributionBase;
                if ((newDistribution == null) || (thisControl.Distributions.FirstOrDefault(o => o.Type == newDistribution.Type) == null))
                {
                    // If this is reached that means the distribution doesn't exist in the distribution options
                    thisControl.DistributionTextBlock.Text = "No Distribution Selected";
                }
                else
                {
                    // Set the distribution textblock text.
                    var sb = new StringBuilder(newDistribution.DisplayName + "(");
                    var shortNames = newDistribution.ParameterNamesShortForm;
                    var paramVals = newDistribution.GetParameters;
                    for (int i = 0; i < shortNames.Count(); i++)
                    {
                        sb.Append(shortNames[i]);
                        sb.Append("=");
                        sb.Append(string.Format("{0:0.#####}", paramVals[i]));
                        if (i != shortNames.Count() - 1)
                            sb.Append(", ");
                    }
                    // 
                    sb.Append(")");
                    thisControl.DistributionTextBlock.Text = sb.ToString();
                }
            }
        }

        /// <summary>
        /// When the mouse enters the datagrid set the popup to staysopen=true to allow the datagrid to keep capture of the mouse and give the datagrid the focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            DistributionPopup.StaysOpen = true;
            SelectorControl.Focus();
        }

        /// <summary>
        /// When the mouse leaves the control set the popup to staysopen=false and give it focus so that clicking outside the popup will close it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            DistributionPopup.StaysOpen = false;
            DistributionPopup.Focus();
        }

        /// <summary>
        /// When a context menu closes the focus gets all out of whack in the popup and needs to be reset. Maybe because it is a popup on a popup? WPF Inception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            DistributionPopup.IsOpen = false;
            DistributionPopup.IsOpen = true;
        }
    }
}
