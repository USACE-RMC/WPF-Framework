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
using GenericControls;
using Numerics.Data;

namespace NumericControls
{
    /// <summary>
    /// Represents a row item for displaying and validating ordinate (X, Y) data in a data grid.
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

        /// <summary>
        /// Relative tolerance for comparing double values.
        /// </summary>
        public static double RelativeDoubleTolerance = 0.00000000000001d; // 1E-14

        /// <summary>
        /// Gets or sets the maximum allowed X value.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the minimum allowed X value.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the maximum allowed Y value.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the minimum allowed Y value.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether X values must be strictly ordered (no duplicates allowed).
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether Y values must be strictly ordered (no duplicates allowed).
        /// </summary>
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

        /// <summary>
        /// Gets or sets the sort order for X values.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the sort order for Y values.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the X value.
        /// </summary>
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
        /// Gets or sets the Y value.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the OrdinateRowItem class.
        /// </summary>
        /// <param name="xVal">The initial X value.</param>
        /// <param name="yVal">The initial Y value.</param>
        /// <param name="xColumnHeader">The column header for X values.</param>
        /// <param name="yColumnHeader">The column header for Y values.</param>
        /// <param name="list">The observable collection this item belongs to.</param>
        /// <param name="minX">The minimum allowed X value.</param>
        /// <param name="maxX">The maximum allowed X value.</param>
        /// <param name="minY">The minimum allowed Y value.</param>
        /// <param name="maxY">The maximum allowed Y value.</param>
        /// <param name="strictX">Whether X values must be strictly ordered.</param>
        /// <param name="strictY">Whether Y values must be strictly ordered.</param>
        /// <param name="orderX">The sort order for X values.</param>
        /// <param name="orderY">The sort order for Y values.</param>
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
        }

        /// <summary>
        /// Gets an Ordinate object representing this row's X and Y values.
        /// </summary>
        /// <returns>An Ordinate object.</returns>
        public Ordinate GetOrdinate()
        {
            return new Ordinate(X, Y);
        }

        /// <summary>
        /// Raises the PropertyChanged event for all properties.
        /// </summary>
        public void RaisePropertyChanged()
        {
            NotifyPropertyChanged();
        }

        /// <inheritdoc/>
        public override void AddValidationRules()
        {
        }

        /// <summary>
        /// Adds data validation rules including range checks and ordering constraints.
        /// </summary>
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

        /// <inheritdoc/>
        public override string PropertyDisplayName(string propertyName)
        {
            if (propertyName == nameof(X)) return _xColumnHeader;
            if (propertyName == nameof(Y)) return _yColumnHeader;
            return propertyName;
        }

        /// <inheritdoc/>
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
