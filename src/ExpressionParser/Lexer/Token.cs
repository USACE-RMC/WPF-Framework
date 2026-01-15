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
    // Order of operations uses the following precedence levels:
    // 1  | ()   []   ->   .   ::                                  | Function call, scope, array/member access
    // 2  | !   ~   -   +   *   &   sizeof   type cast   ++   --   | (most) unary operators, sizeof and type casts (right to left)
    // 3  | *   /   % MOD                                          | Multiplication, division, modulo
    // 4  | +   -                                                  | Addition and subtraction
    // 5  | <<   >>                                                | Bitwise shift left and right
    // 6  | <   <=   >   >=                                        | Comparisons: less-than and greater-than
    // 7  | ==   !=                                                | Comparisons: equal and not equal
    // 8  | &                                                      | Bitwise AND
    // 9  | ^                                                      | Bitwise exclusive OR (XOR)
    // 10 | |                                                      | Bitwise inclusive (normal) OR
    // 11 | &&                                                     | Logical AND
    // 12 | ||                                                     | Logical OR
    // 13 | ? :                                                    | Conditional expression (ternary)
    // 14 | =   +=   -=   *=   /=   %=   &=   |=   ^=   <<=   >>=  | Assignment operators (right to left)
    // 15 | ,                                                      | Comma operator

    /// <summary>
    /// Represents a single token produced by the lexical analysis of an expression string.
    /// A token encapsulates the type, value, and position of a lexical element within the input expression.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Gets the string value that represents the token.
        /// </summary>
        public string TokenString { get; private set; }

        /// <summary>
        /// Gets the zero-based start position of the token in the expression string.
        /// </summary>
        public int StartPosition { get; private set; }

        /// <summary>
        /// Gets the relative path to the help documentation file for this token type.
        /// </summary>
        public string HelpDocPath { get; private set; }

        /// <summary>
        /// Gets the order of operations precedence for this token type.
        /// Lower values indicate higher precedence in expression evaluation.
        /// </summary>
        public byte OperationOrder { get; private set; }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        public TokenType Type { get; private set; }

        /// <summary>
        /// Gets the broad classification group of the token.
        /// </summary>
        public TokenClass TokenGroup { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Token class with the specified properties.
        /// </summary>
        /// <param name="initialPosition">The zero-based start position of the token in the expression string.</param>
        /// <param name="tokenValue">The string value that represents the token.</param>
        /// <param name="helpPath">The relative path to the help documentation file for this token type.</param>
        /// <param name="order">The order of operations precedence for this token type.</param>
        /// <param name="typeOfToken">The type of the token.</param>
        /// <param name="classOfToken">The broad classification group of the token.</param>
        public Token(int initialPosition, string tokenValue, string helpPath, byte order, TokenType typeOfToken, TokenClass classOfToken)
        {
            StartPosition = initialPosition;
            TokenString = tokenValue;
            HelpDocPath = helpPath;
            OperationOrder = order;
            Type = typeOfToken;
            TokenGroup = classOfToken;
        }
    }

    /// <summary>
    /// Represents the broad classification of a token parsed from an expression string.
    /// </summary>
    public enum TokenClass
    {
        /// <summary>
        /// A constant value such as a number, string, or boolean literal.
        /// </summary>
        Value,

        /// <summary>
        /// A function or keyword that performs an operation or transformation.
        /// </summary>
        Function,

        /// <summary>
        /// An operator that modifies values or controls expression evaluation.
        /// </summary>
        Operator,

        /// <summary>
        /// A miscellaneous token that does not fit into the main categories.
        /// </summary>
        Other
    }
}