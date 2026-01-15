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

using System.Data;

namespace ExpressionParser
{
    /// <summary>
    /// Represents a logical AND or OR operation node in a parser tree.
    /// This node evaluates to a Boolean result based on multiple child nodes.
    /// </summary>
    public class AndOrNode : IParserNode
    {

        /// <summary>
        /// Indicates whether this is an AND operation (true) or an OR operation (false).
        /// </summary>
        private readonly bool _isAnd;

        /// <summary>
        /// The collection of parser nodes to test for the logical operation.
        /// </summary>
        private IEnumerable<IParserNode> _nodesToTest;

        /// <summary>
        /// The list of parse errors encountered during construction or evaluation.
        /// </summary>
        private List<ParseError> _errorMessages = new List<ParseError>();

        /// <summary>
        /// Gets a value indicating the node is case-sensitive.
        /// </summary>
        public bool IsCaseSensitive { get; private set; } = false;

        /// <summary>
        /// Gets the type of value produced by this node, always <see cref="ResultType.Boolean"/>.
        /// </summary>
        public ResultType OutputType { get; private set; } = ResultType.Boolean;

        /// <summary>
        /// Gets a value indicating whether this node or any of its children contain parsing errors.
        /// </summary>
        public bool ContainsErrors
        {
            get
            {
                return _errorMessages.Count > 0;
            }
        }

        /// <summary>
        /// Gets a list of parsing errors associated with this node.
        /// </summary>
        public List<ParseError> GetErrors => _errorMessages;

        /// <summary>
        /// Initializes a new instance of <see cref="AndOrNode"/> class with no inputs.
        /// </summary>
        public AndOrNode(IEnumerable<IParserNode> nodesToTest, List<string> parameterErrors, Token token)
        {
            _nodesToTest = nodesToTest;
            //
            if (_nodesToTest == null)
            {
                _errorMessages.Add(new ParseError(token, "Operator does not contain anything to test. The " + token.TokenString + " must have at least one logical test to work."));
                _nodesToTest = new IParserNode[] { }; // set as empty array to reduce null exception potential.
            }
            else
            {
                for (int i = 0; i < _nodesToTest.Count(); i++)
                {
                    var node = _nodesToTest.ElementAtOrDefault(i);
                    if (node == null)
                    {
                        _errorMessages.Add(new ParseError(token, "Contains an empty value (entry number " + (i + 1) + ") each entry must be a logical true/false statement."));
                        continue;
                    }
                    // Propagate errors from child nodes
                    _errorMessages.AddRange(node.GetErrors);
                    //
                    if (node.OutputType == ResultType.Boolean)
                        continue;
                    _errorMessages.Add(new ParseError(token, "Contains a value (entry number " + (i + 1) + ") that is not a logical true/false statement. Current return type at entry number " + (i + 1) + " is " + ((int)node.OutputType).ToString()));
                }
            }
            //
            _isAnd = token.Type == TokenType.And;
            //
            foreach (string errorString in parameterErrors)
                _errorMessages.Add(new ParseError(token, errorString + " for the " + token.Type.ToString() + " function"));
            //
            if (_errorMessages.Count > 0)
                OutputType = ResultType.Error;
        }

        /// <summary>
        /// Simplifies the node. If it contains no variables, evaluates to a constant Boolean result.
        /// </summary>
        /// <returns>A simplified <see cref="IParserNode"/> that is either this node or a <see cref="BooleanNode"/></returns>
        public IParserNode Simplify()
        {
            if (ContainsVariable())
            {
                foreach (var testNode in _nodesToTest)
                {
                    if (testNode == null)
                        continue;
                    testNode.Simplify();
                }
                return this;
            }
            // 
            return new BooleanNode(Convert.ToBoolean(Evaluate().Result));
        }

        /// <summary>
        /// Determines whether any child node contains a variable reference.
        /// </summary>
        /// <returns><c>true</c> if any node contains a variable; otherwise <c>false</c>.</returns>
        public bool ContainsVariable()
        {
            bool hasVariable = false;
            foreach (var testNode in _nodesToTest)
            {
                if (!(testNode == null))
                    hasVariable = testNode.ContainsVariable() ? true : hasVariable;
            }
            // 
            return hasVariable;
        }

        /// <summary>
        /// Evaluates the logical AND or OR operation based on the child nodes.
        /// </summary>
        /// <returns>A <see cref="ParseNodeResult"/> containing a Boolean result or an error.</returns>
        public ParseNodeResult Evaluate()
        {
            if (ContainsErrors)
                return new ParseNodeResult(null, ResultType.Error);
            // 
            bool result;
            if (_isAnd)
            {
                result = _nodesToTest.All(o => Convert.ToBoolean(o.Evaluate().Result) == true);
            }
            else
            {
                result = _nodesToTest.Any(o => Convert.ToBoolean(o.Evaluate().Result) == true);
            }
            // 
            return new ParseNodeResult(result, ResultType.Boolean);
        }

        /// <summary>
        /// Returns all variable nodes found in the subtree of this node.
        /// </summary>
        /// <returns>A list of <see cref="VariableNode"/> instances used within this node.</returns>
        public List<VariableNode> GetVariableNodes()
        {
            var result = new List<VariableNode>();
            if (ContainsVariable())
            {
                foreach (var testNode in _nodesToTest)
                {
                    if (!(testNode == null))
                        result.AddRange(testNode.GetVariableNodes());
                }
            }
            return result;
        }
    }
}