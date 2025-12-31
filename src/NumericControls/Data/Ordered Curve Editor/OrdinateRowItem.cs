using System;
using System.Collections.ObjectModel;
using GenericControls;
using Numerics.Data;

namespace NumericControls
{
    public class OrdinateRowItem : DataGridRowItem
    {
        private double _maxXValue;
        private double _minXValue;
        private double _maxYValue;
        private double _minYValue;
        private bool _isStrictX;
        private bool _isStrictY;
        private SortOrder _xOrder;
        private SortOrder _yOrder;
        private double _x;
        private double _y;


        // Private _propertyNames() As String
        // Private _propertyDisplayNames() As String
        public static double RelativeDoubleTolerance = 0.00000000000001d; // 1E-14

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

        public double X
        {
            get { return _x; }
            set
            {
                if (!_x.Equals(value))
                {
                    _x = value;
                    NotifyPropertyChanged(nameof(X));
                }
            }
        }

        /// <summary>
        /// Y value
        /// </summary>
        /// <returns>Y value.</returns>
        public double Y
        {
            get { return _y; }
            set
            {
                if (Math.Abs(_y - value) > RelativeDoubleTolerance)
                {
                    _y = value;
                    NotifyPropertyChanged(nameof(Y));
                }
            }
        }

        private string _xColumnHeader;
        private string _yColumnHeader;

        public OrdinateRowItem(double xVal, double yVal, string xColumnHeader, string yColumnHeader, ObservableCollection<object> list, double minX, double maxX, double minY, double maxY, bool strictX, bool strictY, SortOrder orderX, SortOrder orderY) : base(list)
        {
            MinXValue = minX;
            MinYValue = minY;
            MaxXValue = maxX;
            MaxYValue = maxY;
            _x = xVal;
            _y = yVal;
            IsStrictX = strictX;
            IsStrictY = strictY;
            XOrder = orderX;
            YOrder = orderY;
            _xColumnHeader = xColumnHeader;
            _yColumnHeader = yColumnHeader;
            AddDataRules();
            // AddHandler Me.PropertyChanged, AddressOf UpdateDistribution
        }

        public Ordinate GetOrdinate()
        {
            return new Ordinate(X, Y);
        }

        public void RaisePropertyChanged()
        {
            NotifyPropertyChanged();
        }

        public override void AddValidationRules()
        {
        }

        private void AddDataRules()
        {
            if (XOrder != SortOrder.None)
                AddRule(nameof(X), () => OrderRule<double, OrdinateRowItem>(o => o.X, nameof(X), XOrder == SortOrder.Ascending, !IsStrictX), "X values must be in " + XOrder.ToString() + " order.");
            AddRule(nameof(X), () => X < MinXValue, "X values must be greater than or equal to " + MinXValue + ".");
            AddRule(nameof(X), () => X > MaxXValue, "X values must be less than or equal to " + MaxXValue + ".");
            if (YOrder != SortOrder.None)
                AddRule(nameof(Y), () => OrderRule<double, OrdinateRowItem>(o => o.Y, nameof(Y), YOrder == SortOrder.Ascending, !IsStrictY), "Y values must be in " + YOrder.ToString() + " order.");
            AddRule(nameof(Y), () => Y < MinYValue, "Y values must be greater than or equal to " + MinYValue + ".");
            AddRule(nameof(Y), () => Y > MaxYValue, "Y values must be less than or equal to " + MaxYValue + ".");
        }

        public override string PropertyDisplayName(string propertyName)
        {
            if (propertyName == nameof(X)) return _xColumnHeader;
            if (propertyName == nameof(Y)) return _yColumnHeader;
            return propertyName;
        }

        public override bool IsGridDisplayable(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(X): return true;
                case nameof(Y): return true;
                default: return false;
            }
        }

    }
}
