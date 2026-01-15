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
    /// Represents a node that performs a rounding operation (ROUND, ROUNDUP, or ROUNDDOWN)
    /// on a numeric value with optional precision.
    /// </summary>
    public class RoundNode : IParserNode
    {

        /// <summary>
        /// The parser node representing the number to be rounded.
        /// </summary>
        private IParserNode _number = null;

        /// <summary>
        /// The parser node representing the number of digits for rounding precision.
        /// </summary>
        private IParserNode _digits = null;

        /// <summary>
        /// The list of parse errors encountered during construction or evaluation.
        /// </summary>
        private List<ParseError> _errorMessages = new List<ParseError>();

        /// <summary>
        /// The rounding operation function (ROUND, ROUNDUP, or ROUNDDOWN).
        /// </summary>
        private readonly Func<double, double, double> _operation;

        /// <summary>
        /// Gets whether the node is case-sensitive.
        /// Always <c>false</c> for rounding operations.
        /// </summary>
        public bool IsCaseSensitive { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether this node or its children contain any parsing errors.
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
        /// Gets the output type of this node.
        /// Always <see cref="ResultType.Double"/> by default.
        /// </summary>
        public ResultType OutputType { get; private set; } = ResultType.Double;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoundNode"/> class with specified parameters and token type.
        /// </summary>
        /// <param name="parameters">The list of parser nodes: the value to round and optional digit precision.</param>
        /// <param name="parameterErrors">The list of parameter parsing error messages.</param>
        /// <param name="token">The token that determines the type of rounding (ROUND, ROUNDUP, or ROUNDDOWN).</param>
        public RoundNode(List<IParserNode> parameters, List<string> parameterErrors, Token token)
        {
            if (parameters.Count >= 1)
                _number = parameters[0];
            if (parameters.Count >= 2)
                _digits = parameters[1];
            if (parameters.Count > 2)
            {
                _errorMessages.Add(new ParseError(token, "Too many parameters defined for rounding function which requires the number to be rounded and an optional number of digits (e.g. ROUND(2.15,1) = 2.2)"));
            }
            // 
            foreach (string errorString in parameterErrors)
                _errorMessages.Add(new ParseError(token, errorString + " for the rounding function"));
            // 
            if (_digits == null)
                _digits = new IntegerNode(0);
            // 
            if (_number == null)
            {
                _errorMessages.Add(new ParseError(token, "No number found in the rounding function to round."));
            }
            else
            {
                if (Parser.Parser.IsNumericType(_number) == false)
                    _errorMessages.Add(new ParseError(token, "Value to be rounded is not a numeric value, unable to perform mathematic operations on non-numeric values."));
                _errorMessages.AddRange(_number.GetErrors);
            }
            // 
            if (Parser.Parser.IsNumericType(_digits) == false)
                _errorMessages.Add(new ParseError(token, "Number of digits specified for rounding is not a valid number, unable to perform mathematic operations on non-numeric values."));
            _errorMessages.AddRange(_digits.GetErrors);
            // 
            if (_errorMessages.Count > 0)
                OutputType = ResultType.Error;
            // 
            switch (token.Type)
            {
                case TokenType.Round:
                    {
                        _operation = new Func<double, double, double>((a, b) =>
                                {
                                    if (b >= 0d && b < 15d)
                                        return Math.Round(a, (int)Math.Round(b));
                                    // Allows for negative precision (e.g. ROUND(234.2,-1) = 230)
                                    return Math.Round(a * Math.Pow(10d, b), 0) / Math.Pow(10d, b);
                                });
                        break;
                    }
                case TokenType.RoundUp:
                    {
                        _operation = new Func<double, double, double>((a, b) =>
                                {
                                    if (b == 0d)
                                        return Math.Ceiling(a);
                                    // Allows for negative precision (e.g. ROUNDUP(234.2,-1) = 240)
                                    return Math.Ceiling(a * Math.Pow(10d, b)) / Math.Pow(10d, b);
                                });
                        break;
                    }
                case TokenType.RoundDown:
                    {
                        _operation = new Func<double, double, double>((a, b) =>
                                {
                                    if (b == 0d)
                                        return Math.Floor(a);
                                    // Allows for negative precision (e.g. ROUNDDOWN(234.2,-1) = 230)
                                    return Math.Floor(a * Math.Pow(10d, b)) / Math.Pow(10d, b);
                                });
                        break;
                    }
            }
        }

        /// <summary>
        /// Simplifies the node. If it contains no variables, evaluates and returns a constant numeric node.
        /// </summary>
        /// <returns>A simplified <see cref="IParserNode"/>, either this instance or a constant value node.</returns>
        public IParserNode Simplify()
        {
            if (ContainsVariable())
            {
                _number = _number.Simplify();
                _digits = _digits.Simplify();
                return this;
            }
            //
            if (Convert.ToDouble(_digits.Evaluate().Result) <= 0d)
                return new IntegerNode(Convert.ToInt32(Evaluate().Result));
            return new DecimalNode(Convert.ToDouble(Evaluate().Result));
        }

        /// <summary>
        /// Determines whether this node or any of its parameters contain variables.
        /// </summary>
        /// <returns><c>true</c> if the number or digits parameter contains a variable; otherwise <c>false</c>.</returns>
        public bool ContainsVariable()
        {
            return _number.ContainsVariable() || _digits.ContainsVariable();
        }

        /// <summary>
        /// Evaluates the node and performs the rounding operation on the input value.
        /// </summary>
        /// <returns>A <see cref="ParseNodeResult"/> containing the rounded value or an error.</returns>
        public ParseNodeResult Evaluate()
        {
            if (ContainsErrors)
                return new ParseNodeResult(null, ResultType.Error);
            //
            double digit = Convert.ToDouble(_digits.Evaluate().Result);
            double result = _operation(Convert.ToDouble(_number.Evaluate().Result), digit);
            if (digit <= 0d)
            {
                return new ParseNodeResult((int)Math.Round(result), ResultType.Integer);
            }
            else
            {
                return new ParseNodeResult(result, ResultType.Double);
            }
        }

        /// <summary>
        /// Gets a list of all variable nodes used in the number or digits expressions.
        /// </summary>
        /// <returns>A list of <see cref="VariableNode"/> instances.</returns>
        public List<VariableNode> GetVariableNodes()
        {
            var result = new List<VariableNode>();
            if (ContainsVariable())
            {
                if (!(_number == null))
                    result.AddRange(_number.GetVariableNodes());
                if (!(_digits == null))
                    result.AddRange(_digits.GetVariableNodes());
            }
            return result;
        }
    }
}