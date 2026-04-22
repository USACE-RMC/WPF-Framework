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

using GenericControls;
using Numerics.Data;
using System.Collections.ObjectModel;

namespace NumericControls
{
    /// <summary>
    /// Represents a data grid row item for time series ordinates, providing a view model wrapper
    /// that uses the clone-and-replace pattern for undo/redo support.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When a cell edit occurs, the setter clones the underlying SeriesOrdinate,
    /// modifies the clone, and replaces the original in the <see cref="TimeSeries"/> collection via the indexer.
    /// This fires a <see cref="System.Collections.Specialized.NotifyCollectionChangedAction.Replace"/> event
    /// which the undo bridge records.
    /// </para>
    /// <para>
    /// During undo replay, <see cref="SetOrdinate"/> updates the internal reference without triggering
    /// clone-and-replace, preventing feedback loops.
    /// </para>
    /// </remarks>
    public class TimeSeriesRowItem : DataGridRowItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSeriesRowItem"/> class with default values.
        /// </summary>
        public TimeSeriesRowItem() : base(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSeriesRowItem"/> class with the specified collection and ordinate.
        /// </summary>
        /// <param name="observableCollection">The observable collection that contains this row item.</param>
        /// <param name="ordinate">The time series ordinate to be wrapped by this row item.</param>
        /// <param name="series">The time series collection that contains the ordinate, used for clone-and-replace on edits.</param>
        /// <param name="index">The positional index of this ordinate within the series, used for O(1) replacement.</param>
        public TimeSeriesRowItem(ObservableCollection<object> observableCollection, SeriesOrdinate<DateTime, double> ordinate, TimeSeries series, int index) : base(observableCollection)
        {
            _ordinate = ordinate;
            _series = series;
            _index = index;
        }

        /// <summary>
        /// The underlying time series ordinate model object.
        /// </summary>
        private SeriesOrdinate<DateTime, double> _ordinate;

        /// <summary>
        /// The time series collection containing the ordinate, used for the clone-and-replace pattern.
        /// </summary>
        private TimeSeries _series;

        /// <summary>
        /// The positional index of this ordinate within the series.
        /// Used for O(1) replacement instead of IndexOf which uses value equality
        /// and can return the wrong index when duplicate entries exist.
        /// Stable because RowItems are rebuilt from scratch on every Reset event.
        /// </summary>
        private int _index;

        /// <summary>
        /// When true, suppresses <see cref="DataGridRowItem.NotifyPropertyChanged"/> calls in property setters.
        /// Set during bulk paste operations where per-cell visual updates are wasteful because
        /// <see cref="TimeSeriesTable.RebuildRowItems"/> will refresh everything at the end.
        /// </summary>
        internal bool SuppressNotify { get; set; }

        /// <summary>
        /// Gets or sets the date and time of this time series observation.
        /// When set, creates a clone of the underlying ordinate, modifies the clone, and replaces
        /// it in the time series to fire a CollectionChanged Replace event for undo support.
        /// </summary>
        public DateTime DateTime
        {
            get => _ordinate.Index;
            set
            {
                if (_ordinate.Index != value)
                {
                    var clone = _ordinate.Clone();
                    clone.Index = value;
                    ReplaceOrdinate(clone);
                    if (!SuppressNotify) NotifyPropertyChanged(nameof(DateTime));
                }
            }
        }

        /// <summary>
        /// Gets or sets the measured value of this time series observation.
        /// When set, creates a clone of the underlying ordinate, modifies the clone, and replaces
        /// it in the time series to fire a CollectionChanged Replace event for undo support.
        /// </summary>
        public double Value
        {
            get => _ordinate.Value;
            set
            {
                if (_ordinate.Value != value)
                {
                    var clone = _ordinate.Clone();
                    clone.Value = value;
                    ReplaceOrdinate(clone);
                    if (!SuppressNotify) NotifyPropertyChanged(nameof(Value));
                }
            }
        }

        /// <summary>
        /// Updates the underlying ordinate reference and notifies the UI of all property changes.
        /// Called by the CollectionChanged(Replace) handler during undo/redo to sync the RowItem
        /// with the restored model data without triggering a clone-and-replace back to the series.
        /// </summary>
        /// <param name="newOrdinate">The restored ordinate from the model.</param>
        public void SetOrdinate(SeriesOrdinate<DateTime, double> newOrdinate)
        {
            _ordinate = newOrdinate;
            NotifyPropertyChanged(nameof(DateTime));
            NotifyPropertyChanged(nameof(Value));
        }

        /// <summary>
        /// Replaces the current ordinate in the time series with a new clone using the stored positional index.
        /// This fires a CollectionChanged Replace event which the UndoableCollectionBridge records.
        /// Uses <see cref="_index"/> instead of IndexOf to avoid O(n) lookup
        /// and value-equality ambiguity when duplicate entries exist.
        /// </summary>
        /// <param name="newOrdinate">The cloned and modified ordinate to replace the current one.</param>
        private void ReplaceOrdinate(SeriesOrdinate<DateTime, double> newOrdinate)
        {
            if (_series == null) return;
            if (_index >= 0 && _index < _series.Count)
            {
                _series[_index] = newOrdinate;
                _ordinate = newOrdinate;
            }
        }

        /// <summary>
        /// Adds validation rules for the time series row item properties.
        /// Validates that the value is a valid number.
        /// </summary>
        public override void AddValidationRules()
        {
            AddRule(nameof(Value), () => double.IsInfinity(Value), "The value must be a finite number.", new[] { nameof(Value) });
        }

        /// <summary>
        /// Determines whether the specified property should be displayed in the data grid.
        /// </summary>
        /// <param name="propertyName">The name of the property to check.</param>
        /// <returns>True if the property should be displayed; otherwise, false.</returns>
        public override bool IsGridDisplayable(string propertyName)
        {
            if (propertyName == nameof(DateTime) || propertyName == nameof(Value))
                return true;
            return false;
        }

        /// <summary>
        /// Gets the display name for the specified property to be shown in the data grid header.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The display name for the property, or null if not recognized.</returns>
        public override string PropertyDisplayName(string propertyName)
        {
            if (propertyName == nameof(DateTime))
                return "Date Time";
            else if (propertyName == nameof(Value))
                return "Value";
            else
                return null;
        }
    }
}
