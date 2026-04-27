using FrameworkInterfaces;
using FrameworkInterfaces.Messaging;
using FrameworkInterfaces.Undo;
using Numerics.Distributions;
using Numerics.Data;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FrameworkUI.Demo
{
    /// <summary>
    /// Represents a hazard element in the demo project. This is an example project element for testing purposes.
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
    public class HazardElement : ElementBase, IUndoableElement
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="HazardElement"/> class.
        /// </summary>
        /// <param name="name">The name of the hazard element.</param>
        /// <param name="parentCollection">The parent collection that contains this element.</param>
        /// <param name="openFromFile">Optional parameter to open the function from disk upon construction.</param>
        public HazardElement(string name, IElementCollection parentCollection, bool openFromFile = false) : base(name, parentCollection)
        {
            Name = name;
            CreationDate = DateTime.Now;
            LastModified = DateTime.Now;

            ProbabilityOrdinates = new ProbabilityOrdinates();
        

            // Create the undo bridge for collection changes
            // Note: Use UndoManager property (not _undoManager field) to ensure lazy initialization
            _ordinatesBridge = new UndoableCollectionBridge<double>(
                    ProbabilityOrdinates,
                () => IsUndoEnabled ? UndoManager : null,
                "probability ordinates",
                this
            );


            InitializeMessages();

            if (openFromFile)
            {
                Open();
                _nameValid = ValidateName(DemoProject.InvalidNameCharacters, 50, "PHF");
            }
            else
            {
                _nameValid = ValidateName(DemoProject.InvalidNameCharacters, 50, "PHF");
                _messenger.Add(_descriptionMsg);
                _messenger.Add(_estimatedMsg);
            }

            SetElementValidation();
            SetIsDirty(false);
        }

        /// <summary>
        /// Initializes the message items for validation and warnings.
        /// </summary>
        private void InitializeMessages()
        {
            _descriptionMsg = new BasicMessageItem(MessageType.Message, "The parametric hazard function does not have a description.",
                this, ParentCollection.Name, Name, nameof(Description), "PHF-MSG-001");

            _lowRealizationsWarning = new BasicMessageItem(MessageType.Warning, "The number of realizations is less than 1,000. The accuracy of the confidence intervals and mean curve will be diminished.",
                this, ParentCollection.Name, Name, nameof(Realizations), "PHF-WNG-001");

            _parentDistMsg = new BasicMessageItem(MessageType.Error, "The parent distribution parameters are invalid.",
                this, ParentCollection.Name, Name, nameof(ParentDistribution), "PHF-ERR-007");

            _recordLengthRangeMsg = new BasicMessageItem(MessageType.Error, "The effective record length must be between 10 and 10,000.",
                this, ParentCollection.Name, Name, nameof(EffectiveRecordLength), "PHF-ERR-008");

            _noConfidenceIntervalMsg = new BasicMessageItem(MessageType.Error, "The confidence interval width cannot be nothing.",
                this, ParentCollection.Name, Name, nameof(ConfidenceIntervalWidth), "PHF-ERR-009");

            _badConfidenceIntervalMsg = new BasicMessageItem(MessageType.Error, "The confidence interval width must be between 0 and 1.",
                this, ParentCollection.Name, Name, nameof(ConfidenceIntervalWidth), "PHF-ERR-010");

            _badSeedMsg = new BasicMessageItem(MessageType.Error, "The PRNG seed must be greater than 0.",
                this, ParentCollection.Name, Name, nameof(PRNGSeed), "PHF-ERR-011");

            _badRealizationsMsg = new BasicMessageItem(MessageType.Error, "The number of realizations must be between 100 and 100,000.",
                this, ParentCollection.Name, Name, nameof(Realizations), "PHF-ERR-012");

            _noOrdinatesMsg = new BasicMessageItem(MessageType.Error, "There must be at least one probability ordinate.",
                this, ParentCollection.Name, Name, nameof(ProbabilityOrdinates), "PHF-ERR-013");

            _badOrdinatesMsg = new BasicMessageItem(MessageType.Error, "All probability values must be between 0 and 1. Please resolve all of the errors in the probability ordinate table.",
                this, ParentCollection.Name, Name, nameof(ProbabilityOrdinates), "PHF-ERR-014");

            _unorderedOrdinatesMsg = new BasicMessageItem(MessageType.Error, "The probability values must be in ascending order. Please resolve all of the errors in the probability ordinate table.",
                this, ParentCollection.Name, Name, nameof(ProbabilityOrdinates), "PHF-ERR-015");

            _badSimulationMsg = new BasicMessageItem(MessageType.Error, "There were errors during the bootstrap simulation. Try adjusting the input parameters or changing the parameter estimation method.",
                this, ParentCollection.Name, Name, nameof(Estimate), "PHF-ERR-016");

            _estimatedMsg = new BasicMessageItem(MessageType.Error, "The parametric hazard function has not been estimated.",
                this, ParentCollection.Name, Name, nameof(IsEstimated), "PHF-ERR-017");

            _pmomMsg = new BasicMessageItem(MessageType.Error, "The selected distribution cannot be estimated with product moments.",
                this, ParentCollection.Name, Name, nameof(EstimationMethod), "PHF-ERR-018");

            _lmomMsg = new BasicMessageItem(MessageType.Error, "The selected distribution cannot be estimated with linear moments.",
                this, ParentCollection.Name, Name, nameof(EstimationMethod), "PHF-ERR-019");

            // Add all messages to the local list for easy access.
            _messages.AddRange(new[] {
                _descriptionMsg,
                _estimatedMsg,
                _lowRealizationsWarning,
                _parentDistMsg,
                _recordLengthRangeMsg,
                _noConfidenceIntervalMsg,
                _badConfidenceIntervalMsg,
                _badSeedMsg,
                _badRealizationsMsg,
                _noOrdinatesMsg,
                _badOrdinatesMsg,
                _unorderedOrdinatesMsg,
                _badSimulationMsg,
                _pmomMsg,
                _lmomMsg
            });
        }

        #endregion

        #region Members

        #region IMetaData Properties

        /// <inheritdoc/>
        [Category("Meta Data"), DisplayName("Name"), Description("The name of this parametric hazard function."), Browsable(true)]
        public override string Name
        {
            get => NameField;
            set
            {
                if (NameField != value)
                {
                    var oldValue = NameField;
                    NameField = value;

                    // Reset messages with new name
                    foreach (var item in _messages)
                    {
                        item.SourceName = value;
                    }

                    _nameValid = ValidateName(DemoProject.InvalidNameCharacters, 50, "PHF");
                    SetElementValidation();
                    RecordPropertyChange(nameof(Name), oldValue, value);
                }
            }
        }

        /// <inheritdoc/>
        [Category("Meta Data"), DisplayName("Description"), Description("The description of this hazard function."), Browsable(true)]
        public override string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    var oldValue = _description;
                    _description = value;
                    if (string.IsNullOrEmpty(_description))
                    {
                        _messenger.Add(_descriptionMsg);
                    }
                    else
                    {
                        _messenger.Remove(_descriptionMsg);
                    }
                    RecordPropertyChange(nameof(Description), oldValue, value);
                }
            }
        }

        /// <inheritdoc/>
        [Category("Meta Data"), DisplayName("Creation Date"), Description("The date and time this hazard function was first created."), Browsable(true)]
        public override DateTime CreationDate { get; }

        /// <inheritdoc/>
        [Category("Meta Data"), DisplayName("Last Edited"), Description("The date and time this hazard function was last modified."), Browsable(true)]
        public override DateTime LastModified { get; }

        #endregion

        #region IElement Properties

        /// <inheritdoc/>
        private static readonly Lazy<ImageSource> s_icon = new(() => { var img = new BitmapImage(new Uri("pack://application:,,,/FrameworkUI.Demo;component/Resources/Hazard_Icon.png")); img.Freeze(); return img; });
        public override ImageSource ElementImage => s_icon.Value;

        /// <inheritdoc/>
        public override bool CanCopyFromExternal => true;

        /// <inheritdoc/>
        public override string NameOnDisk { get; }

        /// <inheritdoc/>
        public override bool IsValid => _isValid;

        #endregion

        #region Properties and Events

        private UnivariateDistributionBase _parentDistribution = new LogPearsonTypeIII();
        private int _effectiveRecordLength = 100;
        private double _confidenceIntervalWidth = 0.9;
        private int _realizations = 10000;
        private int _prngSeed = 12345;
        private ParameterEstimationMethod _estimationMethod = ParameterEstimationMethod.MethodOfMoments;
        private ProbabilityOrdinates _probabilityOrdinates = new ProbabilityOrdinates();
        private UndoableCollectionBridge<double> _ordinatesBridge;
        private string _plotSettings;
        private bool _isEstimated = false;
        private bool _minmaxComputed = false;
        private double[] _minmax = new double[2];
        private bool _isUncertain = true;

        private bool _nameValid = false;
        private bool _distributionValid = true;
        private bool _recordLengthValid = true;
        private bool _confidenceIntervalValid = true;
        private bool _realizationsValid = true;
        private bool _ordinatesValid = true;
        private bool _estimationMethodValid = true;

        private List<BasicMessageItem> _messages = new List<BasicMessageItem>();
        private Messenger _messenger = Messenger.GetInstance();

        private BasicMessageItem _descriptionMsg;
        private BasicMessageItem _lowRealizationsWarning;
        private BasicMessageItem _parentDistMsg;
        private BasicMessageItem _recordLengthRangeMsg;
        private BasicMessageItem _noConfidenceIntervalMsg;
        private BasicMessageItem _badConfidenceIntervalMsg;
        private BasicMessageItem _badSeedMsg;
        private BasicMessageItem _badRealizationsMsg;
        private BasicMessageItem _noOrdinatesMsg;
        private BasicMessageItem _badOrdinatesMsg;
        private BasicMessageItem _unorderedOrdinatesMsg;
        private BasicMessageItem _badSimulationMsg;
        private BasicMessageItem _estimatedMsg;
        private BasicMessageItem _pmomMsg;
        private BasicMessageItem _lmomMsg;

        /// <summary>
        /// Univariate continuous probability distribution used to model the population.
        /// </summary>
        [Category("Inputs"), DisplayName("Parent Distribution"), Description("The continuous distribution describing the population."), Browsable(true)]
        public UnivariateDistributionBase ParentDistribution
        {
            get => _parentDistribution;
            set
            {
                if (_parentDistribution != value)
                {
                    var oldValue = _parentDistribution;
                    _parentDistribution = value;

                    // Validate parent distribution parameters
                    _distributionValid = true;
                    _messenger.Remove(_parentDistMsg);
                    if (!_parentDistribution.ParametersValid)
                    {
                        _distributionValid = false;
                        _messenger.Add(_parentDistMsg);
                    }

                    IsEstimated = false;
                    ClearResults();
                    ValidateEstimationMethod();
                    RecordPropertyChange(nameof(ParentDistribution), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Determines if the distribution has uncertainty.
        /// </summary>
        [Category("Parameter"), DisplayName("Uncertainty On"), Description("Determines if the parametric hazard function includes uncertainty defined by the effective record length (ERL)."), Browsable(true)]
        public bool IsUncertain
        {
            get => _isUncertain;
            set
            {
                if (_isUncertain != value)
                {
                    var oldValue = _isUncertain;
                    _isUncertain = value;
                    IsEstimated = false;
                    ClearResults();
                    ValidateEstimationMethod();
                    RecordPropertyChange(nameof(IsUncertain), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets and sets the effective record length.
        /// </summary>
        [Category("Inputs"), DisplayName("Effective Record Length"), Description("The Effective Record Length (ERL) is used to estimate the uncertainty in the parent distribution. A higher ERL indicates more data coverage, resulting in reduced uncertainty and narrower confidence intervals."), Browsable(true)]
        public int EffectiveRecordLength
        {
            get => _effectiveRecordLength;
            set
            {
                if (_effectiveRecordLength != value)
                {
                    var oldValue = _effectiveRecordLength;
                    _effectiveRecordLength = value;

                    if (_effectiveRecordLength < 10 || _effectiveRecordLength > 10000)
                    {
                        _recordLengthValid = false;
                        _messenger.Add(_recordLengthRangeMsg);
                    }
                    else
                    {
                        _recordLengthValid = true;
                        _messenger.Remove(_recordLengthRangeMsg);
                    }
                    IsEstimated = false;
                    ClearResults();
                    SetElementValidation();
                    RecordPropertyChange(nameof(EffectiveRecordLength), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets and sets the width of the confidence intervals.
        /// </summary>
        [Category("Output Options"), DisplayName("Confidence Interval Width"), Description("Sets the width of the confidence interval. For example, a 90% confidence interval means the value of interest falls within this range with a 90% probability. To ensure accuracy, it is recommended to compute at least 10,000 realizations for a 90% confidence interval."), Browsable(true)]
        public double ConfidenceIntervalWidth
        {
            get => _confidenceIntervalWidth;
            set
            {
                if (_confidenceIntervalWidth != value)
                {   
                    var oldValue = _confidenceIntervalWidth;
                    _confidenceIntervalWidth = value;

                    _confidenceIntervalValid = true;
                    _messenger.Remove(_noConfidenceIntervalMsg);
                    _messenger.Remove(_badConfidenceIntervalMsg);
                    if (double.IsNaN(_confidenceIntervalWidth))
                    {
                        _confidenceIntervalValid = false;
                        _messenger.Add(_noConfidenceIntervalMsg);
                    }
                    if (_confidenceIntervalWidth <= 0 || _confidenceIntervalWidth >= 1)
                    {
                        _confidenceIntervalValid = false;
                        _messenger.Add(_badConfidenceIntervalMsg);
                    }

                    IsEstimated = false;
                    ClearResults();
                    SetElementValidation();
                    RecordPropertyChange(nameof(ConfidenceIntervalWidth), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets and sets the number of bootstrap realizations.
        /// </summary>
        [Category("Simulation Options"), DisplayName("Realizations"), Description("The number of bootstrap realizations used in the analysis. For accurate results, it is recommended to compute at least 10,000 realizations to ensure a reliable 90% confidence interval."), Browsable(true)]
        public int Realizations
        {
            get => _realizations;
            set
            {
                if (_realizations != value)
                {   
                    var oldValue = _realizations;
                    _realizations = value;

                    if (_realizations < 100 || _realizations > 100000)
                    {
                        _realizationsValid = false;
                        _messenger.Add(_badRealizationsMsg);
                    }
                    else
                    {
                        _realizationsValid = true;
                        _messenger.Remove(_badRealizationsMsg);
                    }
                    if (_realizations < 1000)
                    {
                        _messenger.Add(_lowRealizationsWarning);
                    }
                    else
                    {
                        _messenger.Remove(_lowRealizationsWarning);
                    }

                    IsEstimated = false;
                    ClearResults();
                    SetElementValidation();
                    RecordPropertyChange(nameof(Realizations), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets and sets the pseudo random number generator (PRNG) seed.
        /// </summary>
        [Category("Simulation Options"), DisplayName("PRNG Seed"), Description("The seed for the pseudo-random number generator (PRNG) used in the bootstrap simulation. Setting a seed ensures repeatability of results."), Browsable(true), DefaultValue(12345)]
        public int PRNGSeed
        {
            get => _prngSeed;
            set
            {
                if (_prngSeed != value)
                {
                    var oldValue = _prngSeed;
                    _prngSeed = value;

                    if (_prngSeed <= 0)
                    {
                        _messenger.Add(_badSeedMsg);
                    }
                    else
                    {
                        _messenger.Remove(_badSeedMsg);
                    }

                    IsEstimated = false;
                    ClearResults();
                    SetElementValidation();
                    RecordPropertyChange(nameof(PRNGSeed), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets and sets the parameter estimation method.
        /// </summary>
        [Category("Simulation Options"), DisplayName("Estimation Method"), Description("The parameter estimation technique used in the bootstrap uncertainty analysis."), Browsable(true)]
        public ParameterEstimationMethod EstimationMethod
        {
            get => _estimationMethod;
            set
            {
                if (_estimationMethod != value)
                {
                    var oldValue = _estimationMethod;
                    _estimationMethod = value;
                    IsEstimated = false;
                    ClearResults();
                    ValidateEstimationMethod();
                    RecordPropertyChange(nameof(EstimationMethod), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets and sets the exceedance probability values used for plotting the distribution.
        /// </summary>
        public ProbabilityOrdinates ProbabilityOrdinates
        {
            get => _probabilityOrdinates;
            set
            {
                if (_probabilityOrdinates != null)
                    _probabilityOrdinates.CollectionChanged -= ProbabilityOrdinates_CollectionChanged;

                _probabilityOrdinates = value ?? new ProbabilityOrdinates();

                _probabilityOrdinates.CollectionChanged += ProbabilityOrdinates_CollectionChanged;

                RaisePropertyChange(nameof(ProbabilityOrdinates));
            }
        }

        /// <summary>
        /// Gets and sets the plot settings.
        /// </summary>
        public string PlotSettings
        {
            get => _plotSettings;
            set
            {
                if (_plotSettings != value)
                {
                    var oldValue = _plotSettings;
                    _plotSettings = value;
                    RecordPropertyChange(nameof(PlotSettings), oldValue, value);
                }
            }
        }

        /// <summary>
        /// Determines whether the distribution has been bootstrapped.
        /// </summary>
        public bool IsEstimated
        {
            get => _isEstimated;
            private set
            {
                if (_isEstimated != value)
                {
                    _isEstimated = value;

                    _messenger.Remove(_estimatedMsg);
                    if (!_isEstimated)
                    {
                        _messenger.Add(_estimatedMsg);
                    }

                    SetElementValidation();
                    RaisePropertyChange(nameof(IsEstimated));
                }
            }
        }

        /// <summary>
        /// The parametric bootstrap results. 
        /// </summary>
        public UncertaintyAnalysisResults Results { get; private set; }

        /// <summary>
        /// Determines if the function is deterministic or if it has uncertainty.
        /// </summary>
        public bool IsDeterministic => !IsUncertain;

        #endregion

        #endregion

        #region Methods

        #region IElement Methods

        /// <inheritdoc/>
        public override IElement Copy(string newName = null)
        {
            var element = new HazardElement(newName ?? Name, ParentCollection)
            {
                Description = Description,
                ParentDistribution = ParentDistribution.Clone(),
                IsUncertain = IsUncertain,
                EffectiveRecordLength = EffectiveRecordLength,
                ConfidenceIntervalWidth = ConfidenceIntervalWidth,
                Realizations = Realizations,
                PRNGSeed = PRNGSeed,
                EstimationMethod = EstimationMethod
            };

            // Disable undo recording while copying collection data
            element.IsUndoEnabled = false;
            element._probabilityOrdinates.Clear();
            foreach (var p in ProbabilityOrdinates)
            {
                element._probabilityOrdinates.Add(p);
            }
            element._plotSettings = _plotSettings;
            element._isEstimated = false;
            element.IsUndoEnabled = true;
            element.ClearUndoHistory();

            return element;
        }

        /// <inheritdoc/>
        public override IElement CopyFromExternal(string itemName, string fullFileName)
        {
            // Copy from external file
            return null;
        }

        /// <inheritdoc/>
        public override void Delete()
        {
            _messenger.Clear(this);
            _undoManager.Clear();
            _ordinatesBridge?.Dispose();
            SetIsDirty(false);
            //
            // Delete from disk or database
            //
            RaiseDeleted(this);
        }

        /// <inheritdoc/>
        public override void Open()
        {
            _messenger.Clear(this);
            IsUndoEnabled =false;

            //
            // Load from disk or database
            //

            SetElementValidation();
            IsUndoEnabled = true;
            ClearUndoHistory();
            SetIsDirty(false);
        }

        /// <inheritdoc/>
        public override void Save()
        {
            // Preview save
            bool cancel = false;
            RaisePreviewObjectSaved(this, ref cancel);
            if (cancel) return;

            // Update last edited
            _lastModified = DateTime.Now;

            //
            // Save to disk or database
            //

            _nameOnDisk = Name;
            MarkUndoSavePoint();
            SetIsDirty(false);
            RaiseObjectSaved(this);
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Determines if the critical inputs are valid.
        /// </summary>
        private bool AreInputsValid()
        {
            if (!_nameValid) return false;
            if (_parentDistribution == null || !_distributionValid) return false;
            if (!_recordLengthValid) return false;
            if (!_confidenceIntervalValid) return false;
            if (_prngSeed <= 0) return false;
            if (!_realizationsValid) return false;
            if (!_estimationMethodValid) return false;

            _ordinatesValid = true;
            _messenger.Remove(_noOrdinatesMsg);
            _messenger.Remove(_badOrdinatesMsg);
            _messenger.Remove(_unorderedOrdinatesMsg);

            if (ProbabilityOrdinates.Count == 0)
            {
                _ordinatesValid = false;
                _messenger.Add(_noOrdinatesMsg);
            }

            for (int i = 0; i < ProbabilityOrdinates.Count; i++)
            {
                if (ProbabilityOrdinates[i] < 0 || ProbabilityOrdinates[i] > 1)
                {
                    _ordinatesValid = false;
                    _messenger.Add(_badOrdinatesMsg);
                    break;
                }
                if (i > 0 && ProbabilityOrdinates[i] <= ProbabilityOrdinates[i - 1])
                {
                    _ordinatesValid = false;
                    _messenger.Add(_unorderedOrdinatesMsg);
                    break;
                }
            }

            if (!_ordinatesValid) return false;

            return true;
        }

        /// <summary>
        /// Validates the selected estimation method.
        /// </summary>
        private void ValidateEstimationMethod()
        {
            _estimationMethodValid = true;
            _messenger.Remove(_pmomMsg);
            _messenger.Remove(_lmomMsg);

            if (IsUncertain && EstimationMethod == ParameterEstimationMethod.MethodOfMoments)
            {
                if (ParentDistribution.Type == UnivariateDistributionType.GeneralizedNormal ||
                    ParentDistribution.Type == UnivariateDistributionType.KappaFour ||
                    ParentDistribution.Type == UnivariateDistributionType.Weibull)
                {
                    _messenger.Add(_pmomMsg);
                    _estimationMethodValid = false;
                }
            }
            else if (IsUncertain && EstimationMethod == ParameterEstimationMethod.MethodOfLinearMoments)
            {
                if (ParentDistribution.Type == UnivariateDistributionType.Weibull)
                {
                    _messenger.Add(_lmomMsg);
                    _estimationMethodValid = false;
                }
            }

            SetElementValidation();
        }

        /// <summary>
        /// Sets the IsValid property for the element, this needs to be called whenever a property is changed.
        /// </summary>
        private void SetElementValidation()
        {
            bool valid = AreInputsValid();
            if (!_isEstimated) valid = false;

            if (valid != _isValid)
            {
                _isValid = valid;
                RaisePropertyChange(nameof(IsValid));
            }
        }

        #endregion

        #region Element Specific Methods

        /// <summary>
        /// Raise property changed when the collection changed. 
        /// </summary>
        private void ProbabilityOrdinates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsEstimated = false;
            ClearResults();
            SetElementValidation();
            RaisePropertyChange(nameof(ProbabilityOrdinates));
        }

        /// <summary>
        /// Clear the results.
        /// </summary>
        private void ClearResults()
        {
            Results = new UncertaintyAnalysisResults();
            _minmaxComputed = false;
        }

        /// <summary>
        /// Set the parent distribution parameters.
        /// </summary>
        /// <param name="parameters">The list of parameters.</param>
        public void SetDistributionParameters(IList<double> parameters)
        {
            if (parameters.Count != ParentDistribution.NumberOfParameters) return;
            var parms = ParentDistribution.GetParameters;

            for (int i = 0; i < ParentDistribution.NumberOfParameters; i++)
            {
                if (parms[i] != parameters[i])
                {
                    IsEstimated = false;
                    ClearResults();
                    RaisePropertyChange("Distribution Parameters");
                    break;
                }
            }

            ParentDistribution.SetParameters(parameters);

            _distributionValid = true;
            _messenger.Remove(_parentDistMsg);
            if (!ParentDistribution.ParametersValid)
            {
                _distributionValid = false;
                _messenger.Add(_parentDistMsg);
            }
        }

        /// <summary>
        /// Perform the parametric bootstrap.
        /// </summary>
        public void Estimate()
        {
            if (!AreInputsValid()) return;

            _messenger.Remove(_badSimulationMsg);
            _messenger.Add(new BasicMessageItem(MessageType.Event, $"The parametric bootstrap analysis for '{Name}' has started.",
                this, ParentCollection.Name, Name, nameof(HazardElement)));

            IsEstimated = false;
            ClearResults();

            // Get the computed curve
            Results.ParentDistribution = ParentDistribution;
            var probs = new double[ProbabilityOrdinates.Count];
            Results.ModeCurve = new double[ProbabilityOrdinates.Count];

            for (int i = 0; i < ProbabilityOrdinates.Count; i++)
            {
                probs[i] = 1 - ProbabilityOrdinates[i];
                Results.ModeCurve[i] = ParentDistribution.InverseCDF(probs[i]);
            }

            if (IsUncertain)
            {
                try
                {
                    // Create seed from hash code
                    int seed = PRNGSeed;
                    // Perform the bootstrap
                    var bootstrap = new BootstrapAnalysis(ParentDistribution, EstimationMethod, EffectiveRecordLength, Realizations, seed);
                    Results = bootstrap.Estimate(probs, 1 - ConfidenceIntervalWidth);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Bootstrap estimation failed: {ex.Message}");
                    _messenger.Add(_badSimulationMsg);
                    IsEstimated = false;
                    return;
                }
            }

            _messenger.Add(new BasicMessageItem(MessageType.Event, $"The parametric bootstrap analysis for '{Name}' is complete.",
                this, ParentCollection.Name, Name, nameof(HazardElement)));

            IsEstimated = true;
        }

        /// <summary>
        /// Returns the mean hazard function
        /// </summary>
        public IUnivariateDistribution SampleFunction()
        {
            if (!IsEstimated) return null;

            if (IsUncertain)
            {
                var xValues = Results.MeanCurve.ToArray();
                var pValues = new double[ProbabilityOrdinates.Count];

                for (int i = 0; i < ProbabilityOrdinates.Count; i++)
                {
                    pValues[i] = 1 - ProbabilityOrdinates[i];
                }

                Array.Reverse(xValues);
                Array.Reverse(pValues);

                return new EmpiricalDistribution(xValues, pValues) { ProbabilityTransform = Numerics.Data.Transform.NormalZ };
            }
            else
            {
                return ParentDistribution.Clone();
            }
        }

        /// <summary>
        /// Returns the hazard function associated with the specified percentile. 
        /// </summary>
        /// <param name="percentile">The percentile of the uncertainty distribution.</param>
        public IUnivariateDistribution SampleFunction(double percentile)
        {
            if (!IsEstimated) return null;

            if (IsUncertain)
            {
                var distribution = ParentDistribution.Clone();
                // Get parameter set
                int i = Math.Min((int)Math.Floor(percentile * Realizations), Realizations - 1);
                if (Results?.ParameterSets?[i].Values != null)
                {
                    distribution.SetParameters(Results.ParameterSets[i].Values);
                }
                return distribution;
            }
            else
            {
                return ParentDistribution.Clone();
            }
        }

        /// <summary>
        /// Returns the hazard function associated with the specified array index. 
        /// </summary>
        /// <param name="index">Zero-based index.</param>
        public IUnivariateDistribution SampleFunction(int index)
        {
            if (!IsEstimated) return null;
            if (index < 0 || index > Realizations - 1) return null;

            if (IsUncertain)
            {
                var distribution = ParentDistribution.Clone();
                // Get parameter set
                if (Results?.ParameterSets?[index].Values != null)
                {
                    distribution.SetParameters(Results.ParameterSets[index].Values);
                }
                return distribution;
            }
            else
            {
                return ParentDistribution.Clone();
            }
        }

        /// <summary>
        /// Returns the minimum hazard level of the function.
        /// </summary>
        /// <param name="meanOnly">Determines whether to return the mean-only value, or value from full uncertainty.</param>
        public double MinHazard(bool meanOnly)
        {
            if (_minmaxComputed) return _minmax[0];
            ComputeMinMax(meanOnly);
            return _minmax[0];
        }

        /// <summary>
        /// Returns the maximum hazard level of the function. 
        /// </summary>
        /// <param name="meanOnly">Determines whether to return the mean-only value, or value from full uncertainty.</param>
        public double MaxHazard(bool meanOnly)
        {
            if (_minmaxComputed) return _minmax[1];
            ComputeMinMax(meanOnly);
            return _minmax[1];
        }

        /// <summary>
        /// Compute min max of function. 
        /// </summary>
        private void ComputeMinMax(bool meanOnly)
        {
            double minP = Numerics.Tools.Min(ProbabilityOrdinates);
            double maxP = Numerics.Tools.Max(ProbabilityOrdinates);

            if (meanOnly && IsUncertain)
            {
                var dist = SampleFunction();
                _minmax[0] = dist == null ? double.NaN : dist.InverseCDF(1 - maxP);
                _minmax[1] = dist == null ? double.NaN : dist.InverseCDF(1 - minP);
            }
            else if (!meanOnly && IsUncertain && Results != null)
            {
                _minmax[0] = double.MaxValue;
                _minmax[1] = double.MinValue;

                double localMin = double.MaxValue;
                double localMax = double.MinValue;
                object lockObj = new object();

                Parallel.For(0, Realizations, idx =>
                {
                    var dist = ParentDistribution.Clone();
                    if (Results?.ParameterSets?[idx].Values != null)
                    {
                        dist.SetParameters(Results.ParameterSets[idx].Values);
                        double minX = dist.InverseCDF(1 - maxP);
                        double maxX = dist.InverseCDF(1 - minP);
                        lock (lockObj)
                        {
                            if (minX < localMin) localMin = minX;
                            if (maxX > localMax) localMax = maxX;
                        }
                    }
                });

                _minmax[0] = localMin;
                _minmax[1] = localMax;
            }
            else
            {
                _minmax[0] = ParentDistribution.InverseCDF(1 - maxP);
                _minmax[1] = ParentDistribution.InverseCDF(1 - minP);
            }

            _minmaxComputed = true;
        }

        #endregion

        #endregion

    }
}