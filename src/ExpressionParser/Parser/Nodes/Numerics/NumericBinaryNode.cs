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

namespace ExpressionParser
{
    /// <summary>
    /// Represents a binary numeric operation node in a parsed expression tree.
    /// Performs a numeric operation (e.g., addition, subtraction, multiplication, division, exponentiation) on two operands.
    /// </summary>
    public class NumericBinaryNode : IParserNode
    {

        private IParserNode _leftNode;
        private IParserNode _rightNode;
        private readonly Func<double, double, double> _operation;
        private List<ParseError> _errorMessages = new List<ParseError>();

        /// <summary>
        /// Gets a value indicating whether the node is case-sensitive.
        /// Always <c>false</c> for numeric operations.
        /// </summary>
        public bool IsCaseSensitive { get; private set; } = false;

        /// <summary>
        /// Gets the result type produced by this node.
        /// The type is Double if either operand is Double, or if the operation is division or exponentiation; otherwise Integer.
        /// </summary>
        public ResultType OutputType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this node contains any parsing or evaluation errors.
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
        /// Initializes a new instance of the <see cref="NumericBinaryNode"/> class with specified left and right nodes, operation, and token.
        /// </summary>
        /// <param name="leftNode">The left operand node.</param>
        /// <param name="rightNode">The right operand node.</param>
        /// <param name="operation">The numeric operation to apply (takes two doubles and returns a double).</param>
        /// <param name="token">The token that triggered this binary operation (used for error context).</param>
        public NumericBinaryNode(IParserNode leftNode, IParserNode rightNode, Func<double, double, double> operation, Token token)
        {
            _leftNode = leftNode;
            _rightNode = rightNode;
            _operation = operation;
            // 
            if (_leftNode == null)
            {
                _errorMessages.Add(new ParseError(token, "Nothing found on the left side of mathematic operator (" + token.TokenString + ")."));
            }
            else
            {
                if (Parser.Parser.IsNumericType(_leftNode) == false)
                    _errorMessages.Add(new ParseError(token, "left hand side of operator (" + token.TokenString + ") is not a numeric value, unable to perform mathematic operations on non-numeric values."));
                _errorMessages.AddRange(_leftNode.GetErrors);
            }
            if (_rightNode == null)
            {
                _errorMessages.Add(new ParseError(token, "Nothing found on the right side of mathematic operator (" + token.TokenString + ")."));
            }
            else
            {
                if (Parser.Parser.IsNumericType(_rightNode) == false)
                    _errorMessages.Add(new ParseError(token, "right hand side of operator (" + token.TokenString + ") is not a numeric value, unable to perform mathematic operations on non-numeric values."));
                _errorMessages.AddRange(_rightNode.GetErrors);
            }
            if (!(_leftNode == null) && !(_rightNode == null))
            {
                OutputType = _leftNode.OutputType == ResultType.Double || _rightNode.OutputType == ResultType.Double ? ResultType.Double : ResultType.Integer;
            }
            if (token.Type == TokenType.Division || token.Type == TokenType.Exponent)
                OutputType = ResultType.Double;
            // 
            if (_errorMessages.Count > 0)
                OutputType = ResultType.Error;
        }

        /// <summary>
        /// Simplifies the numeric binary node by simplifying its operands and evaluating constant expressions.
        /// If the node contains no variables, it is evaluated and replaced with a constant node.
        /// </summary>
        /// <returns>A simplified node (either this node with simplified children, or a constant DecimalNode or IntegerNode).</returns>
        public IParserNode Simplify()
        {
            if (ContainsVariable())
            {
                if (!(_leftNode == null))
                    _leftNode = _leftNode.Simplify();
                if (!(_rightNode == null))
                    _rightNode = _rightNode.Simplify();
                return this;
            }
            //
            if (OutputType == ResultType.Double)
                return new DecimalNode(Convert.ToDouble(Evaluate().Result));
            return new IntegerNode(Convert.ToInt32(Evaluate().Result));
        }

        /// <summary>
        /// Determines whether this node or any of its child nodes contain variable references.
        /// </summary>
        /// <returns>true if the node or its children contain variables; otherwise, false.</returns>
        public bool ContainsVariable()
        {
            bool hasVariable = false;
            if (!(_leftNode == null))
                hasVariable = _leftNode.ContainsVariable() ? true : hasVariable;
            if (!(_rightNode == null))
                hasVariable = _rightNode.ContainsVariable() ? true : hasVariable;
            // 
            return hasVariable;
        }

        /// <summary>
        /// Evaluates the numeric binary operation by evaluating both operands and applying the operation function.
        /// </summary>
        /// <returns>A <see cref="ParseNodeResult"/> containing the computed result, or an error if the node contains errors.</returns>
        public ParseNodeResult Evaluate()
        {
            if (ContainsErrors)
                return new ParseNodeResult(null, ResultType.Error);
            double result = _operation(Convert.ToDouble(_leftNode.Evaluate().Result), Convert.ToDouble(_rightNode.Evaluate().Result));
            if (OutputType == ResultType.Integer)
            {
                return new ParseNodeResult((int)Math.Round(result), OutputType);
            }
            else
            {
                return new ParseNodeResult(result, OutputType);
            }
        }

        /// <summary>
        /// Gets all variable nodes contained within this node's operands.
        /// </summary>
        /// <returns>A list of all <see cref="VariableNode"/> instances found in the left and right operands.</returns>
        public List<VariableNode> GetVariableNodes()
        {
            var result = new List<VariableNode>();
            if (ContainsVariable())
            {
                if (!(_leftNode == null))
                    result.AddRange(_leftNode.GetVariableNodes());
                if (!(_rightNode == null))
                    result.AddRange(_rightNode.GetVariableNodes());
            }
            return result;
        }
    }
}