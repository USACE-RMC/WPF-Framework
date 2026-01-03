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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace GenericControls
{
    public abstract class DataGridRowItem : INotifyPropertyChanged
    {

        #region Construction

        /// <summary>
    /// Contructs a new data grid row item.
    /// </summary>
    /// <param name="list">The observable collection of all data grid row items.</param>
    /// <param name="parentDataGrid">Optional. The parent validation data grid. Default = nothing.</param>
        public DataGridRowItem(System.Collections.ObjectModel.ObservableCollection<object> list, ValidationDataGrid parentDataGrid = null)
        {
            _parentList = list;
            _parentDataGrid = parentDataGrid;
            AddValidationRules();
            // this is required to ensure that each property has a rule so that the binding will not throw a key not found error.  this could be fixed by adding an attribute [validates] to each property, and only binding error state display to those columns.
            System.Reflection.PropertyInfo[] pinfo = GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo p in pinfo)
            {
                if (p.Name == "RuleMap")
                    continue;
                if (_ruleMap.ContainsKey(p.Name) == false)
                    AddRule(p.Name, () => false, "");
            }
        }

        #endregion

        #region Members

        protected Dictionary<string, PropertyRule> _ruleMap = new Dictionary<string, PropertyRule>();
        protected System.Collections.ObjectModel.ObservableCollection<object> _parentList;
        protected Dictionary<string, HashSet<string>> _associatedProperties = new Dictionary<string, HashSet<string>>();
        private ValidationDataGrid _parentDataGrid;
        private bool _recurse = true;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
    /// The list that this row item is an item of. This is used for complex validation rules that require knowledege 
    /// of neighbors or all other items in the list.
    /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<object> ParentList
        {
            set
            {
                _parentList = value;
            }
        }

        /// <summary>
    /// The map of all property rules for this row item.
    /// </summary>
        public Dictionary<string, PropertyRule> RuleMap
        {
            get
            {
                return _ruleMap;
            }
        }

        #endregion

        #region Methods

        /// <summary>
    /// The required magic for defining when a property is in error. Use the "AddRule" call to add a specific rule.
    /// </summary>
        public abstract void AddValidationRules();

        /// <summary>
    /// Allows specification of prettier property names. Default return should be the property name, will appear as column header (unless modified), and as series name if curvedatagridrowitem is used.
    /// </summary>
    /// <param name="propertyName">The property that needs to be transformed into a better displayable name.</param>
        public abstract string PropertyDisplayName(string propertyName);

        /// <summary>
    /// Allows specification of properties to not be displayed in a datagrid
    /// </summary>
    /// <param name="propertyName">Property name.</param>
        public abstract bool IsGridDisplayable(string propertyName);

        /// <summary>
    /// Raise property changed event.
    /// </summary>
    /// <param name="propertyName">Optional. Name of the property that changed.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            ValidateProperty(propertyName);
            // this simplifies the number of calls in the setter.. and sets up the default behavior.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
    /// Allows each row item to have all properties update their error state. This is helpful when the grid is first displayed and when rows are added.
    /// </summary>
        public void ForceValidation()
        {
            foreach (string propertyName in _ruleMap.Keys)
                ValidateProperty(propertyName);
        }

        /// <summary>
    /// Add a rule for a specific property. If the error condition is met then the cell of the defined property will turn red and the error message will show as a tooltip.
    /// </summary>
    /// <param name="propertyName">Property name that the rule will be applied to.</param>
    /// <param name="errorCondition">The function that dictates an error has occurred or not. If the errorCondition returns true then the property for the given row will be assumed to have an error.</param>
    /// <param name="errorMessage">The error message that will be shown in the tooltip when the errorCondition returns true.</param>
    /// <param name="associatedProperties">Any properties that are associated with the target property. This guarantees proper updating when related properties are changed.</param>
        protected void AddRule(string propertyName, Func<bool> errorCondition, string errorMessage, string[] associatedProperties = null)
        {
            if (_ruleMap.ContainsKey(propertyName))
            {
                _ruleMap[propertyName].AddRule(errorCondition, errorMessage);
            }
            else
            {
                _ruleMap.Add(propertyName, new PropertyRule(errorCondition, errorMessage));
            }
            if (associatedProperties == null)
                return;
            // Update associated properties. The dictionary key is the associated property so that when validate gets called on an associated property it will update the target property.
            if (_associatedProperties.ContainsKey(propertyName) == false)
                _associatedProperties.Add(propertyName, new HashSet<string>());
            foreach (string assProp in associatedProperties)
            {
                if (_associatedProperties.ContainsKey(assProp) == false)
                    _associatedProperties.Add(assProp, new HashSet<string>());
                _associatedProperties[assProp].Add(propertyName);
            }
        }

        /// <summary>
    /// Allows selective validation by property. This can be called in a setter with an empty argument to validate the property. 
    /// By default, this happens in the inotifyproperty changed event. You can define the property name, if you wish for another 
    /// property other than the one just being set to be validated as well.
    /// </summary>
    /// <param name="propertyName">The name of the property to validate.</param>
        public void ValidateProperty(string propertyName)
        {
            if (_parentDataGrid is not null && _parentDataGrid.SuppressValidation == true)
                return;
            if (_ruleMap.ContainsKey(propertyName))
                _ruleMap[propertyName].ExecuteRules();
            // 
            if (_associatedProperties.ContainsKey(propertyName))
            {
                foreach (string _property in _associatedProperties[propertyName])
                    _ruleMap[_property].ExecuteRules(); // the property is guaranteed to be in the rule map since it was added in the addrule sub.
            }
        }

        /// <summary>
        /// Checks if a property value is unique within the parent list of DataGridRowItems.
        /// If a duplicate is found, sets error messages on the duplicate rows accordingly.
        /// </summary>
        /// <param name="propertyName">The name of the property to check for uniqueness.</param>
        /// <param name="errorMessage">The error message to use if a duplicate is found.</param>
        /// <returns>True if the current row has a duplicate value; otherwise, false.</returns>
        protected bool UniqueRule(string propertyName, string errorMessage)
        {
            if (_parentDataGrid is not null && _parentDataGrid.SuppressValidation == true)
                return default;
            if (_parentDataGrid is not null && _parentDataGrid.PerformingBulkValidation == true)
                return default;
            bool @bool = false;
            if (_parentList.Count >= 1)
            {
                for (int i = 0, loopTo = _parentList.Count - 1; i <= loopTo; i++)
                {
                    DataGridRowItem iRowItem = (DataGridRowItem)_parentList[i];
                    string iValue = GetType().GetProperty(propertyName).GetValue(iRowItem, null).ToString();
                    bool hasDuplicate = false;
                    // 
                    // Check for duplicates
                    for (int j = 0, loopTo1 = _parentList.Count - 1; j <= loopTo1; j++)
                    {
                        DataGridRowItem jRowItem = (DataGridRowItem)_parentList[j];
                        string jValue = GetType().GetProperty(propertyName).GetValue(jRowItem, null).ToString();
                        if (jRowItem.Equals(iRowItem))
                            continue;
                        // 
                        if ((iValue ?? "") == (jValue ?? ""))
                        {
                            hasDuplicate = true;
                        }
                    }
                    // 
                    if (iRowItem.Equals(this))
                    {
                        @bool = hasDuplicate;
                        continue;
                    }
                    // 
                    // If not ME, then update row item error boolean and error message.
                    bool hasError = iRowItem.RuleMap[propertyName].HasError;
                    var hasErrorMessage = iRowItem.RuleMap[propertyName].ErrorMessage.Split(Environment.NewLine.ToCharArray()).ToList();
                    for (int j = hasErrorMessage.Count - 1; j >= 0; j -= 1)
                    {
                        if ((hasErrorMessage[j] ?? "") == (Environment.NewLine ?? "") || string.IsNullOrEmpty(hasErrorMessage[j]) || (hasErrorMessage[j] ?? "") == (errorMessage ?? ""))
                            hasErrorMessage.RemoveAt(j);
                    }
                    if (hasDuplicate == true)
                    {
                        if (hasError == false)
                            iRowItem.RuleMap[propertyName].HasError = hasDuplicate;
                        hasErrorMessage.Add(errorMessage);
                    }
                    // Need to check if there was previously just 1 error, and if that error was this one.
                    else if (hasError == true && hasErrorMessage.Count == 0)
                    {
                        iRowItem.RuleMap[propertyName].HasError = hasDuplicate;
                    }
                    iRowItem.RuleMap[propertyName].ErrorMessage = "";
                    for (int j = 0, loopTo2 = hasErrorMessage.Count - 1; j <= loopTo2; j++)
                        iRowItem.RuleMap[propertyName].ErrorMessage += j == 0 ? hasErrorMessage[j] : Environment.NewLine + hasErrorMessage[j];
                }
            }

            return @bool;
        }

        /// <summary>
        /// Validates that the current row's value (from a callback function) follows the correct order
        /// relative to its neighbors in the parent list (ascending or descending).
        /// </summary>
        /// <typeparam name="T">Comparable value type (unused but reserved for future).</typeparam>
        /// <typeparam name="DT">Row type derived from DataGridRowItem.</typeparam>
        /// <param name="callBack">A function that retrieves the double value to compare.</param>
        /// <param name="propertyName">The property namee used to revalidate neighboring rows if needed.</param>
        /// <param name="ascending">Whether the order should be ascending (true) or descending (false)</param>
        /// <param name="canBeEqual">Whether equal values are allowed (true) or not (false)</param>
        /// <returns>True if the ordering rule is violated; otherwise, false</returns>
        protected bool OrderRule<T, DT>(Func<DT, double> callBack, string propertyName, bool @ascending = true, bool canBeEqual = true)
                where T : IComparable
                where DT : DataGridRowItem
        {
            if (_parentDataGrid is not null && _parentDataGrid.SuppressValidation == true)
                return default;
            if (_parentList == null)
                return false;
            int currentIndex = _parentList.IndexOf(this);
            if (currentIndex == -1)
                return false;
            double currentValue = callBack((DT)this); // _parentList(currentIndex).GetType().GetProperty(propertyName).GetValue(_parentList(currentIndex))
            int previousIndex = currentIndex - 1;
            int nextIndex = currentIndex + 1;
            // 
            // value has changed so the next and previous row needs to check to make sure it is still ordered.
            if (_recurse == true)
            {
                _recurse = false;
                if (previousIndex > 0)
                {
                    ((DataGridRowItem)_parentList[previousIndex])._recurse = false;
                    ((DataGridRowItem)_parentList[previousIndex]).RuleMap[propertyName].ExecuteRules();
                    ((DataGridRowItem)_parentList[previousIndex])._recurse = true;
                }
                if (nextIndex < _parentList.Count)
                {
                    ((DataGridRowItem)_parentList[nextIndex])._recurse = false;
                    ((DataGridRowItem)_parentList[nextIndex]).RuleMap[propertyName].ExecuteRules();
                    ((DataGridRowItem)_parentList[nextIndex])._recurse = true;
                }
                _recurse = true;
            }
            // Verify that the changed value is not smaller than the previous row value.
            if (previousIndex >= 0)
            {
                double previousValue = callBack((DT)_parentList[previousIndex]);
                // 
                if (currentValue == previousValue)
                {
                    if (canBeEqual == false)
                        return true;
                }
                else if (ascending == true)
                {
                    if (currentValue < previousValue)
                        return true;
                }
                else if (currentValue > previousValue)
                    return true;
            }
            // 
            return false;
        }

        #endregion
    }
}