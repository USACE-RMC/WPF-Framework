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
    /// Represents a constant integer value in the parser tree.
    /// This node always evaluates to the same <c>int</c> and contains no variables.
    /// </summary>
    public class IntegerNode : IParserNode
    {

        private readonly int _value;
        private List<ParseError> _errorMessages = new List<ParseError>();

        /// <summary>
        /// Gets whether the node is case-sensitive.
        /// Always <c>false</c> for integer literals.
        /// </summary>
        public bool IsCaseSensitive { get; private set; } = false;

        /// <summary>
        /// Gets the result type produced by this node.
        /// Always <see cref="ResultType.Integer"/>.
        /// </summary>
        public ResultType OutputType { get; private set; } = ResultType.Integer;

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
        /// Initializes a new instance of the <see cref="IntegerNode"/> class with a constant integer value.
        /// </summary>
        /// <param name="value">The constant integer value represented by this node.</param>
        public IntegerNode(int value)
        {
            _value = value;
        }

        /// <summary>
        /// Simplifies the integer node.
        /// Since it is already a constant literal, this returns the current instance. 
        /// </summary>
        /// <returns>This <see cref="IntegerNode"/> instance.</returns>
        public IParserNode Simplify()
        {
            return this;
        }

        /// <summary>
        /// Indicates whether the node contains any variable references.
        /// Always returns <c>false</c> for constant integer nodes.
        /// </summary>
        /// <returns><c>false</c>.</returns>
        public bool ContainsVariable()
        {
            return false;
        }

        /// <summary>
        /// Evaluates the node and returns its constant integer value.
        /// </summary>
        /// <returns>A <see cref="ParseNodeResult"/> containing the integer value, or an error if present.</returns>
        public ParseNodeResult Evaluate()
        {
            if (ContainsErrors)
                return new ParseNodeResult(null, ResultType.Error);
            return new ParseNodeResult(_value, ResultType.Integer);
        }

        /// <summary>
        /// Gets the list of variable nodes used within this expression.
        /// Always returns an empty list for constants.
        /// </summary>
        /// <returns>An empty list of <see cref="VariableNode"/>.</returns>
        public List<VariableNode> GetVariableNodes()
        {
            return new List<VariableNode>();
        }

    }
}