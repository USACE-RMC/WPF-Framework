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

using Numerics.Distributions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NumericControls.Distributions.Univariate
{
    /// <summary>
    /// A popup control for selecting and configuring univariate probability distributions.
    /// Provides a compact interface that expands to show distribution parameters, plot, and statistics.
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
    public partial class DistributionSelectorPopup : UserControl
    {
        public DistributionSelectorPopup()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Dependency property for the selected distribution.
        /// </summary>
        public static DependencyProperty SelectedDistributionProperty = DependencyProperty.Register(nameof(SelectedDistribution), typeof(UnivariateDistributionBase), typeof(DistributionSelectorPopup), new PropertyMetadata(new Normal(), SetDistribution));
        /// <summary>
        /// Get and set the selected probability distribution.
        /// </summary>
        public UnivariateDistributionBase SelectedDistribution
        {
            get { return (UnivariateDistributionBase)GetValue(SelectedDistributionProperty); }
            set { SetValue(SelectedDistributionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShowPlot"/> dependency property.
        /// </summary>
        public static DependencyProperty ShowPlotProperty = DependencyProperty.Register(nameof(ShowPlot), typeof(bool), typeof(DistributionSelectorPopup), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the PDF plot is visible.
        /// </summary>
        /// <value><c>true</c> to show the PDF plot; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool ShowPlot
        {
            get { return (bool)GetValue(ShowPlotProperty); }
            set { SetValue(ShowPlotProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ShowStatistics"/> dependency property.
        /// </summary>
        public static DependencyProperty ShowStatisticsProperty = DependencyProperty.Register(nameof(ShowStatistics), typeof(bool), typeof(DistributionSelectorPopup), new FrameworkPropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether the summary statistics are visible.
        /// </summary>
        /// <value><c>true</c> to show summary statistics; otherwise, <c>false</c>. Default is <c>false</c>.</value>
        public bool ShowStatistics
        {
            get { return (bool)GetValue(ShowStatisticsProperty); }
            set { SetValue(ShowStatisticsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Distributions"/> dependency property.
        /// </summary>
        public static DependencyProperty DistributionsProperty = DependencyProperty.Register(nameof(Distributions), typeof(List<UnivariateDistributionBase>), typeof(DistributionSelectorPopup), new PropertyMetadata(DistributionSelectorControl.DefaultDistributions));

        /// <summary>
        /// Gets or sets the list of available distribution types for selection.
        /// </summary>
        public List<UnivariateDistributionBase> Distributions
        {
            get { return (List<UnivariateDistributionBase>)GetValue(DistributionsProperty); }
            set { SetValue(DistributionsProperty, value); }
        }

        /// <summary>
        /// Occurs when the selected distribution changes.
        /// </summary>
        public event DistributionChangedEventHandler DistributionChanged;

        /// <summary>
        /// Represents the method that will handle the <see cref="DistributionChanged"/> event.
        /// </summary>
        public delegate void DistributionChangedEventHandler();

        private void DistributionChangedRaiser()
        {
            DistributionChanged?.Invoke();
        }

        /// <summary>
        /// When the dependency property changes, this sets the distribution.
        /// </summary>
        private static void SetDistribution(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(DistributionSelectorPopup)) return;
            DistributionSelectorPopup thisControl = (DistributionSelectorPopup)d;
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
                    var sb = new StringBuilder(newDistribution.DisplayName + " (");
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

            thisControl.DistributionChangedRaiser();
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
