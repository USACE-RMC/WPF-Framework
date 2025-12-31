using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumericControls
{
    /// <summary>
    /// A class to define parameters.
    /// </summary>
    public class Parameter : INotifyPropertyChanged
    {
        public string DisplayName { get; private set; }
        public string Name { get; private set; }

        private double _value;
        private bool _isValid = true;
        private string _errorMessage = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (value != _isValid)
                {
                    _isValid = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsValid)));
                }
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (value != _errorMessage)
                {
                    _errorMessage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
                }
            }
        }

        public Parameter(string name, string displayName, double value)
        {
            Name = name;
            DisplayName = displayName;
            Value = value;
            IsValid = true;
        }
    }
}
