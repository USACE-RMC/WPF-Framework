using System.Collections.ObjectModel;
using GenericControls;

namespace NumericControls
{
    public class UnivariateDistributionValidatingRow : DataGridRowItem
    {

        private double _xMax;
        private double _xMin;
        private double _x;
        private double _p;

        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                NotifyPropertyChanged(nameof(X));
            }
        }

        public double P
        {
            get { return _p; }

            set
            {
                _p = value;
                NotifyPropertyChanged(nameof(P));
            }
        }

        public double MinX
        {
            get { return _xMin; }
            set
            {
                if (_xMin != value)
                {
                    _xMin = value;
                    NotifyPropertyChanged(nameof(MinX));
                }
            }
        }

        public double MaxX
        {
            get { return _xMax; }
            set
            {
                if (_xMax != value)
                {
                    _xMax = value;
                    NotifyPropertyChanged(nameof(MaxX));
                }
            }
        }

        public UnivariateDistributionValidatingRow() : base(null)
        {
            // 
            _x = 0d;
            _p = 0d;
            _xMin = double.MinValue;
            _xMax = double.MaxValue;
        }

        public UnivariateDistributionValidatingRow(double xValue, double pValue, double xMin, double xMax, ObservableCollection<object> theList) : base(theList)
        {
            // 
            _x = xValue;
            _p = pValue;
            _xMin = xMin;
            _xMax = xMax;
        }

        public override void AddValidationRules()
        {
            AddRule(nameof(P), () => P > 1d, "Probability must be less than or equal to 1");
            AddRule(nameof(P), () => P < 0d, "Probability must be greater than or equal to 0");
            AddRule(nameof(X), () => X > MaxX, "X must be less than or equal to " + MaxX);
            AddRule(nameof(X), () => X < MinX, "X must be greater than or equal to " + MinX);
            AddRule(nameof(P), () => OrderRule<double, UnivariateDistributionValidatingRow>(o => o.P, nameof(P)), "Probability must be in ascending order");
            AddRule(nameof(X), () => OrderRule<double, UnivariateDistributionValidatingRow>(o => o.X, nameof(X)), "X values must be in ascending order");
        }

        public override string PropertyDisplayName(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(X): return "X";
                case nameof(P): return "Probability";
                default: return propertyName;
            }
        }

        public override bool IsGridDisplayable(string propertyName)
        {
            if ((propertyName == nameof(X)) || (propertyName == nameof(P))) return true;
            return false;
        }

    }
}
