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
using System.Windows;
using ExpressionParser;

namespace ExpressionParserControls.Demo
{

    /// <summary>
    /// Represents the main window of the application, which contains a calculator control
    /// </summary>
    public partial class MainWindow:Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the expression changed event from a CalculatorControl instance.
        /// Parses the expression and displays the result or error messages in the associated UI text block.
        /// </summary>
        private void CalculatorControl_ExpressionChanged()
        {
            IParserNode result;
            try
            {
                result = this.testCalc.GetParseTree();
            }
            catch (Exception ex)
            {
                // GetParseTree can throw on pathological input (stack overflow on deeply
                // nested expressions, unexpected token patterns). Surface the message.
                this.ResultTextBlock.Text = "Parse failed: " + ex.Message;
                return;
            }
            this.ResultTextBlock.Text = "";
            // check for parse errors
            if (result == null)
            {
                this.ResultTextBlock.Text = "Error expression returns nothing";
                return;
            }
            List<ParseError> errorList = result.GetErrors;
            // check that output type is correct
            if (errorList.Count > 0)
            {
                this.ResultTextBlock.Text = "Error in expression";
            }
            else
            {
                if (result.ContainsVariable())
                {
                    this.ResultTextBlock.Text = "Error defining variables";
                }
                else
                {
                    try
                    {
                        this.ResultTextBlock.Text = "Example: result equal to '" + result.Evaluate().Result?.ToString() + "'";
                    }
                    catch (Exception ex)
                    {
                        // Evaluate can raise InvalidCastException / FormatException for
                        // operations that mix incompatible types at runtime.
                        this.ResultTextBlock.Text = "Evaluation failed: " + ex.Message;
                    }
                }
            }
        }
    }
}