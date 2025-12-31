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

using System;
using System.Collections.ObjectModel;
using System.Linq;
using GenericControls;
using Numerics.Data;
using Numerics.Distributions;

namespace NumericControls
{
    public class DistributionRowItem : DataGridRowItem
    {

        private UnivariateDistributionBase _distribution;
        private double _maxXValue;
        private double _minXValue;
        private double _maxYValue;
        private double _minYValue;
        private bool _isStrictX;
        private bool _isStrictY;
        private SortOrder _xOrder;
        private SortOrder _yOrder;
        private string[] _propertyNames;
        private string[] _propertyDisplayNames;
        public static double RelativeDoubleTolerance = double.Epsilon;

        /// <summary>
        /// The probability distribution.
        /// </summary>
        public UnivariateDistributionBase Distribution
        {
            get { return _distribution; }
        }

        public double MaxXValue
        {
            get { return _maxXValue; }
            set
            {
                if (_maxXValue != value)
                {
                    _maxXValue = value;
                    NotifyPropertyChanged(nameof(MaxXValue));
                }
            }
        }

        public double MinXValue
        {
            get { return _minXValue; }
            set
            {
                if (_minXValue != value)
                {
                    _minXValue = value;
                    NotifyPropertyChanged(nameof(MinXValue));
                }
            }
        }

        public double MaxYValue
        {
            get { return _maxYValue; }
            set
            {
                if (_maxYValue != value)
                {
                    _maxYValue = value;
                    NotifyPropertyChanged(nameof(MaxYValue));
                }
            }
        }

        public double MinYValue
        {
            get { return _minYValue; }
            set
            {
                if (_minYValue != value)
                {
                    _minYValue = value;
                    NotifyPropertyChanged(nameof(MinYValue));
                }
            }
        }

        public bool IsStrictX
        {
            get { return _isStrictX; }
            set
            {
                if (_isStrictX != value)
                {
                    _isStrictX = value;
                    NotifyPropertyChanged(nameof(IsStrictX));
                }
            }
        }

        public bool IsStrictY
        {
            get { return _isStrictY; }
            set
            {
                if (_isStrictY != value)
                {
                    _isStrictY = value;
                    NotifyPropertyChanged(nameof(IsStrictY));
                }
            }
        }

        public SortOrder XOrder
        {
            get { return _xOrder; }
            set
            {
                if (_xOrder != value)
                {
                    _xOrder = value;
                    NotifyPropertyChanged(nameof(XOrder));
                }
            }
        }

        public SortOrder YOrder
        {
            get { return _yOrder; }
            set
            {
                if (_yOrder != value)
                {
                    _yOrder = value;
                    NotifyPropertyChanged(nameof(YOrder));
                }
            }
        }

        private double _x;
        private double _p1;
        private double _p2;
        private double _p3;
        private double _p4;

        public double X
        {
            get { return _x; }
            set
            {
                if (Math.Abs(_x - value) > RelativeDoubleTolerance || double.IsNaN(_x) || double.IsNaN(value))
                {
                    _x = value;
                    NotifyPropertyChanged(nameof(X));
                }
            }
        }

        /// <summary>
        /// Parameter 1
        /// </summary>
        /// <returns>First Parameter in the Distribution.</returns>
        public double P1
        {
            get { return _p1; }
            set
            {
                if (Math.Abs(_p1 - value) > RelativeDoubleTolerance || double.IsNaN(_p1) || double.IsNaN(value))
                {
                    _p1 = value;
                    NotifyPropertyChanged(nameof(P1));
                }
            }
        }

        /// <summary>
        /// Parameter 2
        /// </summary>
        /// <returns>Second Parameter in the Distribution.</returns>
        public double P2
        {
            get { return _p2; }
            set
            {
                if (Math.Abs(_p2 - value) > RelativeDoubleTolerance || double.IsNaN(_p2) || double.IsNaN(value))
                {
                    _p2 = value;
                    NotifyPropertyChanged(nameof(P2));
                }
            }
        }

        /// <summary>
        /// Parameter 3
        /// </summary>
        /// <returns>Third Parameter in the Distribution.</returns>
        public double P3
        {
            get { return _p3; }
            set
            {
                if (Math.Abs(_p3 - value) > RelativeDoubleTolerance || double.IsNaN(_p3) || double.IsNaN(value))
                {
                    _p3 = value;
                    NotifyPropertyChanged(nameof(P3));
                }
            }
        }

        /// <summary>
        /// Parameter 4
        /// </summary>
        /// <returns>Fourth Parameter in the Distribution.</returns>
        public double P4
        {
            get { return _p4; }
            set
            {
                if (Math.Abs(_p4 - value) > RelativeDoubleTolerance || double.IsNaN(_p4) || double.IsNaN(value))
                {
                    _p4 = value;
                    NotifyPropertyChanged(nameof(P4));
                }
            }
        }

        public double Minimum => _distribution.ParametersValid ? _distribution.InverseCDF(_minProbability) : double.NaN;

        public double Maximum => _distribution.ParametersValid ? _distribution.InverseCDF(_maxProbability) : double.NaN;

        public double Mean => _distribution.ParametersValid ? _distribution.Mean : double.NaN; 


        private readonly double _minProbability = 1E-5d; // 1E-5 the max number of Monte Carlo samples will be 10,000 1E-4.
        private readonly double _maxProbability = 1d - 1E-5d; // 1 - 1E-5

        public DistributionRowItem(double xVal, UnivariateDistributionBase dist, ObservableCollection<object> list, double minX, double maxX, double minY, double maxY, bool strictX, bool strictY, Numerics.Data.SortOrder orderX, Numerics.Data.SortOrder orderY) : base(list)
        {
            MinXValue = minX;
            MinYValue = minY;
            MaxXValue = maxX;
            MaxYValue = maxY;
            _x = xVal;
            _distribution = dist.Clone();

            if (_distribution.Type == UnivariateDistributionType.PertPercentile ||
                _distribution.Type == UnivariateDistributionType.PertPercentileZ)
            {
                _minProbability = 0.05;
                _maxProbability = 0.95;
            }

            double[] parameters = _distribution.GetParameters;
            if (parameters.Count() > 4)
            {
                throw new Exception("Uncertain Ordered Paired Data editor can only work with distributions with 4 parameters or less.");
            }
            // 
            _p1 = parameters[0];
            if (parameters.Count() >= 2) { _p2 = parameters[1]; }
            if (parameters.Count() >= 3) { _p3 = parameters[2]; }
            if (parameters.Count() >= 4) { _p4 = parameters[3]; }
            _propertyNames = _distribution.GetParameterPropertyNames;
            _propertyDisplayNames = new string[(_propertyNames.Count())];
            string[,] paramString = _distribution.ParametersToString;
            // 
            for (int i = 0; i < paramString.GetLength(0); i++)
            {
                _propertyDisplayNames[i] = paramString[i, 0];
            }
            IsStrictX = strictX;
            IsStrictY = strictY;
            XOrder = orderX;
            YOrder = orderY;
            AddDataRules();
        }

        public void RaisePropertyChanged()
        {
            NotifyPropertyChanged();
        }

        private bool SetDistribution()
        {
            var newParams = new double[_distribution.NumberOfParameters];
            newParams[0] = P1;
            if (_propertyNames.Count() >= 2) { newParams[1] = P2; }
            if (_propertyNames.Count() >= 3) { newParams[2] = P3; }
            if (_propertyNames.Count() >= 4) { newParams[3] = P4; }
            // 
            _distribution.SetParameters(newParams);

            if (_distribution.ParametersValid == false)
            {
                var argError = _distribution.ValidateParameters(newParams, false);
                // 
                for (int i = 0; i < _propertyNames.Count(); i++)
                    RuleMap["P" + (i + 1)].ErrorMessage = argError.Message;
                return true;
            }

            return false;
        }

        public override void AddValidationRules()
        {
        }

        private void AddDataRules()
        {
            if (XOrder != SortOrder.None)
            {
                AddRule(nameof(X), () => OrderRule<double, DistributionRowItem>(o => o.X, nameof(X), XOrder == SortOrder.Ascending, !IsStrictX), "X values must be in " + XOrder.ToString() + " order.");
            }
            AddRule(nameof(X), () => X < MinXValue, "X values must be greater than or equal to " + MinXValue + ".");
            AddRule(nameof(X), () => X > MaxXValue, "X values must be less than or equal to " + MaxXValue + ".");

            // These need to be before the min, max, and mean validation rules. I think so that the distribution parameters can be set so that the mean, min, and max can be updated.
            AddRule(nameof(P1), SetDistribution, null);
            if (_propertyNames.Count() >= 2) { AddRule(nameof(P2), SetDistribution, null); }
            if (_propertyNames.Count() >= 3) { AddRule(nameof(P3), SetDistribution, null); }
            if (_propertyNames.Count() >= 4) { AddRule(nameof(P4), SetDistribution, null); }

            // Order Rules
            if (YOrder != SortOrder.None)
            {
                // Means
                AddRule(nameof(P1), () => OrderRule<double, DistributionRowItem>(o => o.Mean, nameof(P1), YOrder == SortOrder.Ascending, !IsStrictY), "Mean values must be in " + YOrder.ToString() + " order.");
                if (_propertyNames.Count() >= 2)
                    AddRule(nameof(P2), () => OrderRule<double, DistributionRowItem>(o => o.Mean, nameof(P2), YOrder == SortOrder.Ascending, !IsStrictY), "Mean values must be in " + YOrder.ToString() + " order.");
                if (_propertyNames.Count() >= 3)
                    AddRule(nameof(P3), () => OrderRule<double, DistributionRowItem>(o => o.Mean, nameof(P3), YOrder == SortOrder.Ascending, !IsStrictY), "Mean values must be in " + YOrder.ToString() + " order.");
                if (_propertyNames.Count() >= 4)
                    AddRule(nameof(P4), () => OrderRule<double, DistributionRowItem>(o => o.Mean, nameof(P4), YOrder == SortOrder.Ascending, !IsStrictY), "Mean values must be in " + YOrder.ToString() + " order.");

                // Minimums
                AddRule(nameof(P1), () => OrderRule<double, DistributionRowItem>(o => o.Minimum, nameof(Minimum), YOrder == SortOrder.Ascending, !IsStrictY), "Minimum values must be in " + YOrder.ToString() + " order.");
                if (_propertyNames.Count() >= 2)
                    AddRule(nameof(P2), () => OrderRule<double, DistributionRowItem>(o => o.Minimum, nameof(Minimum), YOrder == SortOrder.Ascending, !IsStrictY), "Minimum values must be in " + YOrder.ToString() + " order.");
                if (_propertyNames.Count() >= 3)
                    AddRule(nameof(P3), () => OrderRule<double, DistributionRowItem>(o => o.Minimum, nameof(Minimum), YOrder == SortOrder.Ascending, !IsStrictY), "Minimum values must be in " + YOrder.ToString() + " order.");
                if (_propertyNames.Count() >= 4)
                    AddRule(nameof(P4), () => OrderRule<double, DistributionRowItem>(o => o.Minimum, nameof(Minimum), YOrder == SortOrder.Ascending, !IsStrictY), "Minimum values must be in " + YOrder.ToString() + " order.");

                // Maximums
                AddRule(nameof(P1), () => OrderRule<double, DistributionRowItem>(o => o.Maximum, nameof(Maximum), YOrder == SortOrder.Ascending, !IsStrictY), "Maximum values must be in " + YOrder.ToString() + " order.");
                if (_propertyNames.Count() >= 2)
                    AddRule(nameof(P2), () => OrderRule<double, DistributionRowItem>(o => o.Maximum, nameof(Maximum), YOrder == SortOrder.Ascending, !IsStrictY), "Maximum values must be in " + YOrder.ToString() + " order.");
                if (_propertyNames.Count() >= 3)
                    AddRule(nameof(P3), () => OrderRule<double, DistributionRowItem>(o => o.Maximum, nameof(Maximum), YOrder == SortOrder.Ascending, !IsStrictY), "Maximum values must be in " + YOrder.ToString() + " order.");
                if (_propertyNames.Count() >= 4)
                    AddRule(nameof(P4), () => OrderRule<double, DistributionRowItem>(o => o.Maximum, nameof(Maximum), YOrder == SortOrder.Ascending, !IsStrictY), "Maximum values must be in " + YOrder.ToString() + " order.");
            }
            // Minimum and Maximum values
            AddRule(nameof(P1), () => Minimum < MinYValue || double.IsNaN(Minimum), "The min values must be greater than " + MinYValue + ".", new[] { "P2", "P3", "P4" });
            AddRule(nameof(P1), () => Maximum > MaxYValue || double.IsNaN(Maximum), "The max values must be less than " + MaxYValue + ".");
            if (_propertyNames.Count() >= 2)
            {
                AddRule(nameof(P2), () => Minimum < MinYValue || double.IsNaN(Minimum), "The min values must be greater than " + MinYValue + ".", new[] { "P1", "P3", "P4" });
                AddRule(nameof(P2), () => Maximum > MaxYValue || double.IsNaN(Maximum), "The max values must be less than " + MaxYValue + ".");
            }

            if (_propertyNames.Count() >= 3)
            {
                AddRule(nameof(P3), () => Minimum < MinYValue || double.IsNaN(Minimum), "The min values must be greater than " + MinYValue + ".", new[] { "P1", "P2", "P4" });
                AddRule(nameof(P3), () => Maximum > MaxYValue || double.IsNaN(Maximum), "The max values must be less than " + MaxYValue + ".");
            }

            if (_propertyNames.Count() >= 4)
            {
                AddRule(nameof(P4), () => Minimum < MinYValue || double.IsNaN(Minimum), "The min values must be greater than " + MinYValue + ".", new[] { "P1", "P2", "P3" });
                AddRule(nameof(P4), () => Maximum > MaxYValue || double.IsNaN(Maximum), "The max values must be less than " + MaxYValue + ".");
            }
            if (Distribution as Pert != null || Distribution as PertPercentile != null)
            {
                AddRule(nameof(P1), () => P1 < MinYValue, "The parameter values must be greater than " + MinYValue + ".", new[] { "P2", "P3", "P4" });
                AddRule(nameof(P1), () => P1 > MaxYValue, "The parameter values must be less than " + MaxYValue + ".");
                if (_propertyNames.Count() >= 2)
                {
                    AddRule(nameof(P2), () => P2 < MinYValue, "The parameter values must be greater than " + MinYValue + ".", new[] { "P1", "P3", "P4" });
                    AddRule(nameof(P2), () => P2 > MaxYValue, "The parameter values must be less than " + MaxYValue + ".");
                }

                if (_propertyNames.Count() >= 3)
                {
                    AddRule(nameof(P3), () => P3 < MinYValue, "The parameter values must be greater than " + MinYValue + ".", new[] { "P1", "P2", "P4" });
                    AddRule(nameof(P3), () => P3 > MaxYValue, "The parameter values must be less than " + MaxYValue + ".");
                }
            }
        }

        public override string PropertyDisplayName(string propertyName)
        {
            if (propertyName == nameof(P1)) { return _propertyDisplayNames[0]; }
            if (propertyName == nameof(P2) && _propertyDisplayNames.Count() > 1) { return _propertyDisplayNames[1]; }
            if (propertyName == nameof(P3) && _propertyDisplayNames.Count() > 2) { return _propertyDisplayNames[2]; }
            if (propertyName == nameof(P4) && _propertyDisplayNames.Count() > 3) { return _propertyDisplayNames[3]; }
            // 
            return propertyName;
        }

        public override bool IsGridDisplayable(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(X): return true;
                case nameof(P1): return true;
                case nameof(P2): return _propertyDisplayNames.Count() > 1;
                case nameof(P3): return _propertyDisplayNames.Count() > 2;
                case nameof(P4): return _propertyDisplayNames.Count() > 3;
                default: return false;
            }
        }
    }
}
