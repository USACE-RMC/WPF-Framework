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

namespace GenericControls
{

    public class PropertyRule : INotifyPropertyChanged
    {

        #region Construction

        /// <summary>
    /// Construct new property rule.
    /// </summary>
    /// <param name="rule">Rule as a function that returns a boolean.</param>
    /// <param name="message">The error message to display if function returns True.</param>
        public PropertyRule(Func<bool> rule, string message)
        {
            _rules.Add(new Rule(rule, message));
        }

        #endregion

        #region Members

        // This has to implement notify property changed to alert the UI to change color state.
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly List<Rule> _rules = new List<Rule>();
        private bool _hasError = false;
        private string _errorMessage = string.Empty;

        /// <summary>
    /// Determines whether the property has an erro. 
    /// </summary>
        public bool HasError
        {
            get
            {
                return _hasError;
            }
            set
            {
                if (_hasError != value)
                {
                    _hasError = value;
                    NotifyPropertyChanged(nameof(HasError));
                }
            }
        }

        /// <summary>
    /// Returns the error message as string.
    /// </summary>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                if ((_errorMessage ?? "") != (value ?? ""))
                {
                    _errorMessage = value;
                    NotifyPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        public List<Rule> Rules
        {
            get
            {
                return _rules;
            }
        }

        /// <summary>
    /// Class for the property rule. Each rule has a function and an error message.
    /// </summary>
        public class Rule
        {
            public readonly Func<bool> Expression;
            public readonly string Message;
            public bool HasError;
            internal Rule(Func<bool> expression, string message)
            {
                Expression = expression;
                Message = message;
            }
        }

        #endregion

        #region Methods

        /// <summary>
    /// Raise property changed event.
    /// </summary>
    /// <param name="propertyName">Name of the property than changed.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
    /// Add rule to the proprty.
    /// </summary>
    /// <param name="rule">Rule as a function that returns a boolean.</param>
    /// <param name="message">The error message to display if function returns True.</param>
        internal void AddRule(Func<bool> rule, string message)
        {
            _rules.Add(new Rule(rule, message));
        }



        /// <summary>
    /// Execute the property rules.
    /// </summary>
        internal void ExecuteRules()
        {
            ErrorMessage = "";
            HasError = false;
            try
            {
                for (int i = 0, loopTo = _rules.Count - 1; i <= loopTo; i++)
                {
                    if (_rules[i].Expression() == true)
                    {
                        HasError = true;
                        ErrorMessage += i == 0 ? _rules[i].Message : Environment.NewLine + _rules[i].Message;
                    }
                }
            }
            catch (Exception e)
            {
                _errorMessage = e.Message;
                HasError = true;
            }
        }

        #endregion

    }
}