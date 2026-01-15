using FrameworkInterfaces;
using Numerics.Distributions;
using NumericControls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace FrameworkUI.Demo.UI
{
    /// <summary>
    /// A WPF user control for editing parametric hazard function properties.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This control provides a property editor interface for configuring parametric hazard functions including:
    /// </para>
    /// <list type="bullet">
    ///     <item><description>Meta data (name, description, dates)</description></item>
    ///     <item><description>Hazard type and units selection</description></item>
    ///     <item><description>Parent distribution selection and parameter configuration</description></item>
    ///     <item><description>Uncertainty settings (record length, confidence intervals, realizations)</description></item>
    ///     <item><description>Probability ordinates table for frequency curve plotting</description></item>
    /// </list>
    /// </remarks>
    public partial class HazardPropertiesControl : UserControl
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="HazardPropertiesControl"/> class.
        /// </summary>
        public HazardPropertiesControl()
        {
            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            PropertyAttributes.SetDefaultAttributes("Parametric Hazard Function Properties", _defaultDescription);

            FrequencyDataGrid.RowType = typeof(ProbabilityOrdinateRowItem);
            _combobox.ItemsSource = DistributionOptions;
            _combobox.DisplayMemberPath = "DisplayName";
            _combobox.HorizontalAlignment = HorizontalAlignment.Stretch;
            _combobox.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            Distribution.InnerContent = _combobox;

            _probabilityRowItems.CollectionChanged += (sender, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        ((ProbabilityOrdinateRowItem)item).PropertyChanged += ProbabilityOrdinateRowItem_PropertyChanged;
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.OldItems)
                    {
                        ((ProbabilityOrdinateRowItem)item).PropertyChanged -= ProbabilityOrdinateRowItem_PropertyChanged;
                    }
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    // No action needed for reset
                }
            };
        }

        #endregion

        #region Members

        private bool _isLoaded = false;
        private static readonly string _defaultDescription = "A parametric hazard function can be created using user-defined parameters, with or without uncertainty. When uncertainty is included, the parametric bootstrap method is used to quantify it.";
        private ComboBox _combobox = new ComboBox();
        private string _previousName;
        private bool _supressUIUpdate = false;
        private bool _supressModelUpdate = false;
        private ObservableCollection<object> _probabilityRowItems = new ObservableCollection<object>();
        private ObservableCollection<Parameter> _parameterList = new ObservableCollection<Parameter>();

        /// <summary>
        /// Identifies the <see cref="Element"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            nameof(Element),
            typeof(HazardElement),
            typeof(HazardPropertiesControl),
            new PropertyMetadata(null, ElementPropertyChanged));

        /// <summary>
        /// Handles changes to the <see cref="Element"/> dependency property.
        /// </summary>
        /// <param name="d">The dependency object on which the property changed.</param>
        /// <param name="e">Event arguments containing the old and new values.</param>
        private static void ElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(HazardPropertiesControl)) return;

            var thisControl = (HazardPropertiesControl)d;

            // Clear old probability row items
            for (int i = thisControl._probabilityRowItems.Count - 1; i >= 0; i--)
            {
                thisControl._probabilityRowItems.RemoveAt(i);
            }
            thisControl.FrequencyDataGrid.ItemsSource = null;

            // Remove any handlers from the old element
            var oldElement = e.OldValue as HazardElement;
            if (oldElement != null)
            {
                oldElement.ProbabilityOrdinates.CollectionChanged -= thisControl.ProbabilityOrdinates_CollectionChanged;
                oldElement.PropertyChanged -= thisControl.Element_PropertyChanged;
                thisControl._combobox.SelectionChanged -= thisControl.DistributionCombobox_SelectionChanged;
            }

            var newElement = e.NewValue as HazardElement;
            if (newElement != null)
            {
                newElement.ProbabilityOrdinates.CollectionChanged += thisControl.ProbabilityOrdinates_CollectionChanged;

                foreach (var prob in newElement.ProbabilityOrdinates)
                {
                    thisControl._probabilityRowItems.Add(new ProbabilityOrdinateRowItem(thisControl._probabilityRowItems, prob));
                }
                thisControl.FrequencyDataGrid.ItemsSource = thisControl._probabilityRowItems;

                for (int i = 0; i < thisControl.DistributionOptions.Length; i++)
                {
                    if (thisControl.DistributionOptions[i].Type == newElement.ParentDistribution.Type)
                    {
                        thisControl._combobox.SelectedIndex = i;
                        break;
                    }
                }

                newElement.PropertyChanged += thisControl.Element_PropertyChanged;
                thisControl._combobox.SelectionChanged += thisControl.DistributionCombobox_SelectionChanged;
                thisControl.UpdateParameterDataGrid();
                thisControl.SetDataGridStyle();
                thisControl.ParametersTable.ItemsSource = thisControl._parameterList;
            }
        }

        /// <summary>
        /// Gets or sets the parametric hazard element bound to this control.
        /// </summary>
        /// <value>
        /// The <see cref="HazardElement"/> instance being edited.
        /// </value>
        public HazardElement Element
        {
            get => (HazardElement)GetValue(ElementProperty);
            set => SetValue(ElementProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ExistingNames"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExistingNamesProperty = DependencyProperty.Register(
            nameof(ExistingNames),
            typeof(string[]),
            typeof(HazardPropertiesControl),
            new FrameworkPropertyMetadata(new string[] { }));

        /// <summary>
        /// Gets or sets the array of existing element names for validation purposes.
        /// </summary>
        /// <value>
        /// An array of strings representing names already in use by other elements.
        /// </value>
        public string[] ExistingNames
        {
            get => (string[])GetValue(ExistingNamesProperty);
            private set => SetValue(ExistingNamesProperty, value);
        }

        /// <summary>
        /// Gets the available parent distribution options.
        /// </summary>
        /// <value>
        /// An array of <see cref="UnivariateDistributionBase"/> instances representing
        /// the supported probability distributions for parametric hazard functions.
        /// </value>
        public UnivariateDistributionBase[] DistributionOptions => new[] {
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.Exponential),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.GammaDistribution),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.GeneralizedExtremeValue),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.GeneralizedLogistic),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.GeneralizedNormal),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.GeneralizedPareto),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.Gumbel),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.KappaFour),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.LnNormal),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.LogNormal),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.LogPearsonTypeIII),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.Normal),
            UnivariateDistributionFactory.CreateDistribution(UnivariateDistributionType.PearsonTypeIII)
        };

        /// <summary>
        /// Gets the list of available confidence interval width options.
        /// </summary>
        /// <value>
        /// An observable collection of <see cref="ConfidenceIntervalItem"/> objects
        /// representing common confidence interval widths (90%, 95%, 98%, 99%).
        /// </value>
        public ObservableCollection<ConfidenceIntervalItem> ConfidenceIntervalWidthList => new ObservableCollection<ConfidenceIntervalItem>(new[]
        {
            new ConfidenceIntervalItem("90%", 0.9),
            new ConfidenceIntervalItem("95%", 0.95),
            new ConfidenceIntervalItem("98%", 0.98),
            new ConfidenceIntervalItem("99%", 0.99)
        });

        /// <summary>
        /// Gets the list of available parameter estimation methods.
        /// </summary>
        /// <value>
        /// An observable collection of <see cref="EstimationMethod"/> objects
        /// representing the supported estimation techniques (Product Moments, Linear Moments, Maximum Likelihood).
        /// </value>
        public ObservableCollection<EstimationMethod> EstimationMethodList => new ObservableCollection<EstimationMethod>(new[]
        {
            new EstimationMethod("Product Moments", ParameterEstimationMethod.MethodOfMoments),
            new EstimationMethod("Linear Moments", ParameterEstimationMethod.MethodOfLinearMoments),
            new EstimationMethod("Maximum Likelihood", ParameterEstimationMethod.MaximumLikelihood)
        });

        /// <summary>
        /// Gets the list of Monte Carlo realization count options.
        /// </summary>
        /// <value>
        /// A list of doubles representing common realization counts for bootstrap analysis.
        /// </value>
        public List<double> RealizationList => new List<double> { 100, 500, 1000, 5000, 10000 };

        #endregion

        #region Property Attributes

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the Name control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Name_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Element.Name), Element);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the Description control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Description_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Element.Description), Element);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the CreationDate control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void CreationDate_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Element.CreationDate), Element);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the LastModified control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void LastModified_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Element.LastModified), Element);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the Distribution control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Distribution_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Element.ParentDistribution), Element);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the ParameterDataGrid control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ParameterDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.SetDefaultAttributes("Distribution Parameters", "The parameters of the parent probability distribution.");
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the IsUncertain control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void IsUncertain_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Element.IsUncertain), Element);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the EffectiveRecordLength control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void EffectiveRecordLength_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Element.EffectiveRecordLength), Element);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the EstimationMethod control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void EstimationMethod_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Element.EstimationMethod), Element);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the ConfidenceInterval control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ConfidenceInterval_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Element.ConfidenceIntervalWidth), Element);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the Realizations control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Realizations_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Element.Realizations), Element);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the PRNGSeed control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void PRNGSeed_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.GetPropertyAttributes(nameof(Element.PRNGSeed), Element);
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the DataGrid control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void DataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.SetDefaultAttributes("Probability Ordinates", "The exceedance probabilities used for plotting the probability distribution.");
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event for the TabItem control to display property attributes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void TabItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PropertyAttributes.SetDefaultAttributes("Parametric Hazard Function Properties", _defaultDescription);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the GotFocus event of the Name control to store the previous name for validation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Name_GotFocus(object sender, RoutedEventArgs e)
        {
            _previousName = Element.Name;
            ExistingNames = Element.ParentCollection.GetElementNames(Element).ToArray();
        }

        /// <summary>
        /// Handles the LostFocus event of the Name control to revert invalid names.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// If the name is not unique or otherwise invalid, the name is reverted to its previous value.
        /// </remarks>
        private void Name_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Name.NameTextBox.IsValid) return;
            if (Element != null) Element.Name = _previousName;
        }

        #endregion

        #region Distribution Parameters

        /// <summary>
        /// Handles property changed events from the bound element to update the parameter grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the property name.</param>
        private void Element_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Element.ParentDistribution))
            {
                UpdateParameterDataGrid();
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the distribution combobox to update the parent distribution.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void DistributionCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_combobox.SelectedIndex != -1)
            {
                Element.ParentDistribution = (UnivariateDistributionBase)_combobox.SelectedItem;
            }
        }

        /// <summary>
        /// Updates the parameter data grid with the current distribution's parameters.
        /// </summary>
        private void UpdateParameterDataGrid()
        {
            _parameterList.Clear();
            var names = Element.ParentDistribution.GetParameterPropertyNames;
            var display = Element.ParentDistribution.ParametersToString;
            var parms = Element.ParentDistribution.GetParameters;

            for (int i = 0; i < parms.Length; i++)
            {
                var parm = new Parameter(names[i], display[i, 0], parms[i]);
                parm.PropertyChanged += ParameterChanged;
                _parameterList.Add(parm);
            }
        }

        /// <summary>
        /// Handles parameter value changes to update the distribution and validate parameters.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the property name.</param>
        private void ParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Parameter.Value))
            {
                for (int i = 0; i < _parameterList.Count; i++)
                {
                    _parameterList[i].IsValid = true;
                    _parameterList[i].ErrorMessage = null;
                }

                Element.SetDistributionParameters(_parameterList.Select(x => x.Value).ToArray());

                if (!Element.ParentDistribution.ParametersValid)
                {
                    var ex = Element.ParentDistribution.ValidateParameters(_parameterList.Select(x => x.Value).ToArray(), false);
                    SetInvalidParameter(ex.ParamName, ex.Message);
                }

                SetDataGridStyle();
            }
        }

        /// <summary>
        /// Sets a parameter as invalid with the specified error message.
        /// </summary>
        /// <param name="parameterName">The name of the invalid parameter.</param>
        /// <param name="errorMessage">The error message describing the validation failure.</param>
        private void SetInvalidParameter(string parameterName, string errorMessage)
        {
            var param = _parameterList.FirstOrDefault(x => string.Equals(x.Name, parameterName, StringComparison.OrdinalIgnoreCase));
            if (param == null) return;
            param.IsValid = false;
            param.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Sets the data grid cell styles including validation error highlighting.
        /// </summary>
        private void SetDataGridStyle()
        {
            NameColumn.CellStyle = (Style)FindResource("Left_CellStyle");
            ValueColumn.CellStyle = (Style)FindResource("Right_CellStyle");

            var style = new Style();
            if (style != null)
            {
                foreach (Setter setter in ValueColumn.CellStyle.Setters)
                {
                    style.Setters.Add(setter);
                }

                var dt1 = new DataTrigger
                {
                    Binding = new Binding("IsValid"),
                    Value = false
                };
                dt1.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF7B6AF"))));
                dt1.Setters.Add(new Setter(DataGridCell.BorderBrushProperty, new SolidColorBrush(Colors.Red)));
                dt1.Setters.Add(new Setter(DataGridCell.ToolTipProperty, new Binding("ErrorMessage")));
                style.Triggers.Add(dt1);
            }

            ValueColumn.CellStyle = style;
        }

        #endregion

        #region Probability Ordinates

        /// <summary>
        /// Handles property changes in probability ordinate row items to synchronize with the model.
        /// </summary>
        /// <param name="s">The source of the event.</param>
        /// <param name="ea">The <see cref="PropertyChangedEventArgs"/> instance containing the property name.</param>
        private void ProbabilityOrdinateRowItem_PropertyChanged(object s, PropertyChangedEventArgs ea)
        {
            if (ea.PropertyName != nameof(ProbabilityOrdinateRowItem.Ordinate)) return;
            if (_supressModelUpdate) return;

            int rowIndex = _probabilityRowItems.IndexOf((ProbabilityOrdinateRowItem)s);
            if (rowIndex >= 0 && rowIndex < Element.ProbabilityOrdinates.Count)
            {
                _supressUIUpdate = true;
                Element.ProbabilityOrdinates[rowIndex] = ((ProbabilityOrdinateRowItem)s).Ordinate;
                _supressUIUpdate = false;
            }
        }

        /// <summary>
        /// Handles collection changes in the probability ordinates to synchronize the UI.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the change details.</param>
        private void ProbabilityOrdinates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_supressUIUpdate)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        int addCounter = e.NewStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            _probabilityRowItems.Insert(addCounter, new ProbabilityOrdinateRowItem(_probabilityRowItems, (double)item));
                            addCounter++;
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldItems.Count == 1)
                        {
                            _probabilityRowItems.RemoveAt(e.OldStartingIndex);
                        }
                        else
                        {
                            int removeCounter = e.OldStartingIndex + e.OldItems.Count;
                            foreach (var item in e.OldItems)
                            {
                                _probabilityRowItems.RemoveAt(removeCounter);
                                removeCounter--;
                            }
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        int replaceCounter = e.NewStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            _supressModelUpdate = true;
                            ((ProbabilityOrdinateRowItem)_probabilityRowItems[replaceCounter]).Ordinate = (double)item;
                            replaceCounter++;
                            _supressModelUpdate = false;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the AutoGeneratingColumn event of the DataGrid to configure column properties.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="DataGridAutoGeneratingColumnEventArgs"/> instance containing the column data.</param>
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ProbabilityOrdinateRowItem.Ordinate):
                    e.Column.MinWidth = 20;
                    e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    ((DataGridTextColumn)e.Column).CellStyle = (Style)FindResource("Right_CellStyle");
                    ((DataGridTextColumn)e.Column).HeaderStyle = (Style)FindResource("Center_ColumnHeaderStyle");

                    // Add tooltip to column header style
                    var headerStyle = new Style(typeof(DataGridColumnHeader));
                    var currentStyle = ((DataGridTextColumn)e.Column).HeaderStyle;

                    if (currentStyle != null)
                    {
                        foreach (Setter setter in currentStyle.Setters)
                        {
                            headerStyle.Setters.Add(setter);
                        }
                    }

                    headerStyle.Setters.Add(new Setter(ToolTipProperty, new TextBlock
                    {
                        Text = "Enter the desired values as exceedance probabilities.",
                        FontWeight = FontWeights.Normal,
                        TextAlignment = TextAlignment.Left,
                        TextWrapping = TextWrapping.Wrap
                    }));

                    ((DataGridTextColumn)e.Column).HeaderStyle = headerStyle;
                    break;
            }
        }

        /// <summary>
        /// Handles rows being added to the data grid to synchronize with the model.
        /// </summary>
        /// <param name="startRowIndex">The starting index of the added rows.</param>
        /// <param name="nRows">The number of rows added.</param>
        private void DataGrid_RowsAdded(int startRowIndex, int nRows)
        {
            _supressUIUpdate = true;
            for (int i = 0; i < nRows; i++)
            {
                Element.ProbabilityOrdinates.Insert(startRowIndex + i, ((ProbabilityOrdinateRowItem)_probabilityRowItems[startRowIndex + i]).Ordinate);
            }
            _supressUIUpdate = false;
        }

        /// <summary>
        /// Handles rows being deleted from the data grid to synchronize with the model.
        /// </summary>
        /// <param name="rowIndices">The list of row indices that were deleted.</param>
        private void DataGrid_RowsDeleted(List<int> rowIndices)
        {
            _supressUIUpdate = true;
            for (int i = rowIndices.Count - 1; i >= 0; i--)
            {
                Element.ProbabilityOrdinates.RemoveAt(rowIndices[i]);
            }
            _supressUIUpdate = false;
        }

        #endregion

        /// <summary>
        /// Handles the Click event of the SimulateButton to run the bootstrap estimation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SimulateButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            Element.Estimate();
            Mouse.OverrideCursor = null;
        }
    }
}
