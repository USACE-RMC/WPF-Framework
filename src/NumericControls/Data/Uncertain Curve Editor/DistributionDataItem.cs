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

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Numerics.Data;
using Numerics.Distributions;

namespace NumericControls
{
    /// <summary>
    /// Represents a data item that manages distribution data for uncertain ordered paired data.
    /// Handles the collection of distribution row items and synchronizes changes between the data model and UI.
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
    public class DistributionDataItem
    {

        private UncertainOrderedPairedData _data;
        private bool _updatingData = false;
        private double _maxXValue;
        private double _minXValue;
        private double _maxYValue;
        private double _minYValue;
        private bool _isStrictX;
        private bool _isStrictY;
        private SortOrder _xOrder;
        private SortOrder _yOrder;
        public static double RelativeDoubleTolerance = double.Epsilon;

        public double MaxXValue
        {
            get { return _maxXValue; }
            set
            {
                if (_maxXValue != value)
                {
                    _maxXValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxXValue)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinXValue)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxYValue)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinYValue)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsStrictX)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsStrictY)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(XOrder)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(YOrder)));
                }
            }
        }

        public UncertainOrderedPairedData Data
        {
            get { return _data; }
            set
            {
                if (_data == null && value == null) return;
                if (_data == null || value == null)
                {
                    if (_data != null) _data.CollectionChanged -= DataCollectionChanged;
                    _data = value;
                    Refresh();
                    if (_data != null) _data.CollectionChanged += DataCollectionChanged;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
                }
                if (_data != value)
                {
                    _data.CollectionChanged -= DataCollectionChanged;
                    _data = value;
                    Refresh();
                    _data.CollectionChanged += DataCollectionChanged;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
                }
            }
        }

        public UnivariateDistributionType Distribution { get; private set; }
        public string DistributionName { get; private set; }
        public ObservableCollection<object> DistributionRows { get; private set; } = new ObservableCollection<object>();

        public event DataChangedEventHandler DataChanged;

        public delegate void DataChangedEventHandler(int dataChangedIndex);

        public event PropertyChangedEventHandler PropertyChanged;

        public DistributionDataItem(UncertainOrderedPairedData d, UnivariateDistributionType dType, double minX, double maxX, double minY, double maxY, bool strictX, bool strictY, SortOrder orderX, SortOrder orderY)
        {
            _data = d;
            if (_data != null) _data.CollectionChanged += DataCollectionChanged;
            Distribution = dType;
            _minXValue = minX;
            _maxXValue = maxX;
            _minYValue = minY;
            _maxYValue = maxY;
            _isStrictX = strictX;
            _isStrictY = strictY;
            _xOrder = orderX;
            _yOrder = orderY;
            DistributionName = UnivariateDistributionFactory.CreateDistribution(Distribution).DisplayName;
            Refresh();
        }

        /// <summary>
        /// Handles collection changed events from the underlying data source and synchronizes with the UI.
        /// </summary>
        /// <param name="sender">The data collection that changed.</param>
        /// <param name="e">The collection changed event arguments.</param>
        private void DataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_updatingData == true) return;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        Refresh();
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        int startIndex = e.OldStartingIndex;
                        if (startIndex == -1)
                        {
                            Refresh();
                        }
                        else
                        {
                            for (int i = 1; i <= e.OldItems.Count; i++)
                                DistributionRows.RemoveAt(startIndex);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        int index = _data.IndexOf((UncertainOrdinate)e.NewItems[0]);
                        Refresh(index);
                        break;
                    }
                default:
                    {
                        Refresh();
                        break;
                    }
            }
        }

        public void Refresh()
        {
            foreach (DistributionRowItem row in DistributionRows)
                row.PropertyChanged -= RowItem_PropertyChanged;
            DistributionRows.Clear();
            if (Data == null) return;
            DistributionRowItem rowItem;
            foreach (var o in Data)
            {
                rowItem = new DistributionRowItem(o.X, o.Y, DistributionRows, MinXValue, MaxXValue, MinYValue, MaxYValue, IsStrictX, IsStrictY, XOrder, YOrder);
                rowItem.PropertyChanged += RowItem_PropertyChanged;
                DistributionRows.Add(rowItem);
            }
        }

        public void Refresh(int rowIndex)
        {
            if (_data == null) return;
            if ((rowIndex >= _data.Count) || (rowIndex < 0)) return;
            ((DistributionRowItem)DistributionRows[rowIndex]).PropertyChanged -= RowItem_PropertyChanged;
            var rowItem = new DistributionRowItem(Data[rowIndex].X, Data[rowIndex].Y, DistributionRows, MinXValue, MaxXValue, MinYValue, MaxYValue, IsStrictX, IsStrictY, XOrder, YOrder);
            rowItem.PropertyChanged += RowItem_PropertyChanged;
            DistributionRows[rowIndex] = rowItem;
        }

        /// <summary>
        /// Handles property changed events for row items and updates the data source.
        /// </summary>
        /// <param name="sender">The row item that changed.</param>
        /// <param name="e">The property changed event arguments.</param>
        private void RowItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _updatingData = true;
            DistributionRowItem rItem = (DistributionRowItem)sender;
            int dataIndex = DistributionRows.IndexOf(rItem);
            _data[dataIndex] = new UncertainOrdinate(rItem.X, rItem.Distribution.Clone());
            _updatingData = false;
            DataChanged?.Invoke(dataIndex);
        }

    }
}
