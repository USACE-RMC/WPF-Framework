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
    /// Encapsulates the result of evaluating a parse node, including both the computed value and its type.
    /// This class provides type safety and conversion utilities for working with expression evaluation results.
    /// </summary>
    public class ParseNodeResult
    {
        /// <summary>
        /// Gets the type of the result value.
        /// For example, ResultType.Double indicates that the Result object is of type double.
        /// </summary>
        public ResultType Type { get; private set; }

        /// <summary>
        /// Gets the result value of the parse node evaluation.
        /// The runtime type of this object corresponds to the Type property.
        /// </summary>
        public object Result { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ParseNodeResult class with the specified value and type.
        /// </summary>
        /// <param name="resultValue">The computed result value.</param>
        /// <param name="resultType">The type classification of the result.</param>
        public ParseNodeResult(object resultValue, ResultType resultType)
        {
            Result = resultValue;
            Type = resultType;
        }

        /// <summary>
        /// Converts a native CLR <see cref="Type"/> to the corresponding <see cref="ResultType"/> used in parsing.
        /// </summary>
        /// <param name="t">The CLR type to convert.</param>
        /// <returns>The corresponding ResultType, or ResultType.UnDeclared if the type is not recognized.</returns>
        public static ResultType TypeToParserResultType(Type t)
        {
            switch (t)
            {
                case var @case when @case == typeof(double):
                    {
                        return ResultType.Double;
                    }
                case var case1 when case1 == typeof(bool):
                    {
                        return ResultType.Boolean;
                    }
                case var case2 when case2 == typeof(string):
                    {
                        return ResultType.String;
                    }
                case var case3 when case3 == typeof(int):
                case var case4 when case4 == typeof(long):
                    {
                        return ResultType.Integer;
                    }
                case var case5 when case5 == typeof(float):
                    {
                        return ResultType.Single;
                    }
                case var case6 when case6 == typeof(short):
                    {
                        return ResultType.Short;
                    }
                case var case7 when case7 == typeof(byte):
                    {
                        return ResultType.Byte;
                    }

                default:
                    {
                        return ResultType.UnDeclared;
                    }
            }
        }

        /// <summary>
        /// Converts a <see cref="ResultType"/> to the corresponding CLR <see cref="Type"/>.
        /// </summary>
        /// <param name="t">The parser result type to convert.</param>
        /// <returns>The corresponding CLR Type, or typeof(object) if the ResultType is not recognized.</returns>
        public static Type ParserResultTypeToType(ResultType t)
        {
            switch (t)
            {
                case ResultType.Double:
                    {
                        return typeof(double);
                    }
                case ResultType.Boolean:
                    {
                        return typeof(bool);
                    }
                case ResultType.String:
                    {
                        return typeof(string);
                    }
                case ResultType.Integer:
                    {
                        return typeof(int);
                    }
                case ResultType.Single:
                    {
                        return typeof(float);
                    }
                case ResultType.Short:
                    {
                        return typeof(short);
                    }
                case ResultType.Byte:
                    {
                        return typeof(byte);
                    }

                default:
                    {
                        return typeof(object);
                    }
            }
        }
    }
}