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

namespace ExpressionParser
{
    /// <summary>
    /// Represents a conditional IF expression in the parser tree.
    /// Evaluates the logical test, and returns the result of the if_true or if_false node accordingly.
    /// </summary>
    public class IfNode : IParserNode
    {

        /// <summary>
        /// The parser node representing the logical test condition.
        /// </summary>
        private IParserNode _testNode = null;

        /// <summary>
        /// The parser node representing the value to return when the test is true.
        /// </summary>
        private IParserNode _ifTrue = null;

        /// <summary>
        /// The parser node representing the value to return when the test is false.
        /// </summary>
        private IParserNode _ifFalse = null;

        /// <summary>
        /// The list of parse errors encountered during construction or evaluation.
        /// </summary>
        private List<ParseError> _errorMessages = new List<ParseError>();

        /// <summary>
        /// Gets whether the node is case-sensitive.
        /// Always <c>false</c> for IF expressions.
        /// </summary>
        public bool IsCaseSensitive { get; private set; } = false;

        /// <summary>
        /// Gets the output type of the IF expression.
        /// This is inferred from the if_true and if_false branches or set to Error if invalid.
        /// </summary>
        public ResultType OutputType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the node has any construction or evaluation errors.
        /// </summary>
        public bool ContainsErrors
        {
            get
            {
                return _errorMessages.Count > 0;
            }
        }

        /// <summary>
        /// Gets the list of parse errors associated with this node.
        /// </summary>
        public List<ParseError> GetErrors => _errorMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfNode"/> class using provided parameters and the token context.
        /// </summary>
        /// <param name="parameters">The list of parser nodes.</param>
        /// <param name="parameterErrors">Additional parameter-level error messages.</param>
        /// <param name="token">The token representing the IF keyword (used for error reporting).</param>
        public IfNode(List<IParserNode> parameters, List<string> parameterErrors, Token token)
        {
            if (parameters.Count >= 1)
                _testNode = parameters[0];
            if (parameters.Count >= 2)
                _ifTrue = parameters[1];
            if (parameters.Count >= 3)
                _ifFalse = parameters[2];
            if (parameters.Count > 3)
            {
                _errorMessages.Add(new ParseError(token, "Too many parameters defined for the If function which needs to have a logical_test, an if_true value, and an if_false value ( e.g. IF(logical_test, if_true, if_false) )."));
            }
            // 
            if (_testNode == null)
            {
                _errorMessages.Add(new ParseError(token, "Nothing found for the logical test in the IF(logical_test, if_true, if_false) function."));
            }
            else
            {
                if (_testNode.OutputType != ResultType.Boolean)
                    _errorMessages.Add(new ParseError(token, "The IF statement must have a logical test that returns true or false (not " + ((int)_testNode.OutputType).ToString() + ") such as: If 5 is greater than 3 then fish otherwise car 'IF(5>3,fish,car' ."));
                _errorMessages.AddRange(_testNode.GetErrors);
            }
            // 
            if (_ifTrue == null)
            {
                _errorMessages.Add(new ParseError(token, "Nothing found for the if_true outcome in the IF(logical_test, if_true, if_false) function."));
            }
            else
            {
                _errorMessages.AddRange(_ifTrue.GetErrors);
            }
            // 
            if (_ifFalse == null)
            {
                _errorMessages.Add(new ParseError(token, "Nothing found for the if_false outcome in the IF(logical_test, if_true, if_false) function."));
            }
            else
            {
                _errorMessages.AddRange(_ifFalse.GetErrors);
            }
            //
            if (!(_ifTrue == null) && !(_ifFalse == null))
            {
                if (_ifTrue.OutputType != _ifFalse.OutputType)
                {
                    if (Parser.Parser.IsNumericType(_ifTrue) && Parser.Parser.IsNumericType(_ifFalse))
                    {
                        OutputType = _ifTrue.OutputType == ResultType.Double || _ifFalse.OutputType == ResultType.Double ? ResultType.Double : ResultType.Integer;
                    }
                    else
                    {
                        _errorMessages.Add(new ParseError(token, "The if_true output type (" + ((int)_ifTrue.OutputType).ToString() + ") and if_false output type (" + ((int)_ifFalse.OutputType).ToString() + ") must be of the same type."));
                    }
                }
                else
                {
                    OutputType = _ifTrue.OutputType;
                }
            }
            // 
            foreach (string errorString in parameterErrors)
                _errorMessages.Add(new ParseError(token, errorString + " for the IF function"));
            // 
            if (_errorMessages.Count > 0)
                OutputType = ResultType.Error;
        }

        /// <summary>
        /// Simplifies the IF node. If no variables exist, evaluates and returns a constant node.
        /// </summary>
        /// <returns>A simplified version of this node, or a constant node.</returns>
        public IParserNode Simplify()
        {
            if (ContainsVariable())
            {
                if (!(_testNode == null))
                    _testNode = _testNode.Simplify();
                if (!(_ifTrue == null))
                    _ifTrue = _ifTrue.Simplify();
                if (!(_ifFalse == null))
                    _ifFalse = _ifFalse.Simplify();
                return this;
            }
            //
            var result = Evaluate().Result;
            switch (OutputType)
            {
                case ResultType.Integer:
                    return new IntegerNode(Convert.ToInt32(result));
                case ResultType.Double:
                    return new DecimalNode(Convert.ToDouble(result));
                case ResultType.Boolean:
                    return new BooleanNode(Convert.ToBoolean(result));
                case ResultType.String:
                    return new StringNode(Convert.ToString(result));
                default:
                    return new DecimalNode(Convert.ToDouble(result));
            }
        }

        /// <summary>
        /// Determines whether this node contains any variables.
        /// </summary>
        /// <returns><c>true</c> if any branch contains a variable; otherwise <c>false</c>.</returns>
        public bool ContainsVariable()
        {
            bool hasVariable = false;
            if (!(_testNode == null))
                hasVariable |= _testNode.ContainsVariable();
            if (!(_ifTrue == null))
                hasVariable |= _ifTrue.ContainsVariable();
            if (!(_ifFalse == null))
                hasVariable |= _ifFalse.ContainsVariable();
            // 
            return hasVariable;
        }

        /// <summary>
        /// Evaluates the IF expression by testing the condition and returning the result of the appropriate branch.
        /// </summary>
        /// <returns>A <see cref="ParseNodeResult"/> containing the output value or error.</returns>
        public ParseNodeResult Evaluate()
        {
            if (ContainsErrors)
                return new ParseNodeResult(null, ResultType.Error);
            //
            var testResult = _testNode.Evaluate();
            if (testResult.Result == null)
                return new ParseNodeResult(null, ResultType.Error);
            if (Convert.ToBoolean(testResult.Result))
            {
                return new ParseNodeResult(_ifTrue.Evaluate().Result, OutputType);
            }
            else
            {
                return new ParseNodeResult(_ifFalse.Evaluate().Result, OutputType);
            }
        }

        /// <summary>
        /// Gets all variable nodes used within the IF expression.
        /// </summary>
        /// <returns>A list of <see cref="VariableNode"/> objects found in any part of the IF statement.</returns>
        public List<VariableNode> GetVariableNodes()
        {
            var result = new List<VariableNode>();
            if (ContainsVariable())
            {
                if (!(_testNode == null))
                    result.AddRange(_testNode.GetVariableNodes());
                if (!(_ifTrue == null))
                    result.AddRange(_ifTrue.GetVariableNodes());
                if (!(_ifFalse == null))
                    result.AddRange(_ifFalse.GetVariableNodes());
            }
            return result;
        }
    }
}