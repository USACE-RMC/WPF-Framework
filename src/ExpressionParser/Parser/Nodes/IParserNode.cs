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
    // Order of operations uses the following precedence levels (source: https://en.wikipedia.org/wiki/Order_of_operations)
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
    /// Defines the contract for all parse tree nodes in the expression parser.
    /// Parse nodes form a tree structure representing the parsed expression and can be evaluated to produce results.
    /// </summary>
    public interface IParserNode
    {
        /// <summary>
        /// Gets a value indicating whether the node performs case-sensitive string comparisons.
        /// </summary>
        bool IsCaseSensitive { get; }

        /// <summary>
        /// Gets a value indicating whether the parse node contains any errors.
        /// If errors exist, the node should not be evaluated as it may produce incorrect or unexpected results.
        /// </summary>
        bool ContainsErrors { get; }

        /// <summary>
        /// Gets the list of errors associated with the parse node.
        /// Each error contains the token where the error occurred and a description of the error condition.
        /// </summary>
        List<ParseError> GetErrors { get; }

        /// <summary>
        /// Simplifies the parse node and its children by performing constant folding and other optimizations.
        /// This is particularly useful when variables are present and the expression will be evaluated multiple times.
        /// </summary>
        /// <returns>A simplified parse node that produces the same results but evaluates more efficiently.</returns>
        IParserNode Simplify();

        /// <summary>
        /// Determines whether the parse node or any of its child nodes contain variable references.
        /// </summary>
        /// <returns>true if the node or its children contain variables; otherwise, false.</returns>
        bool ContainsVariable();

        /// <summary>
        /// Evaluates the parse node and returns the result of the evaluation.
        /// If the node contains errors, the result will have a ResultType of Error.
        /// </summary>
        /// <returns>A ParseNodeResult containing the evaluation result and its type.</returns>
        ParseNodeResult Evaluate();

        /// <summary>
        /// Gets the expected output type that this parse node will produce when evaluated.
        /// </summary>
        ResultType OutputType { get; }

        /// <summary>
        /// Gets all variable nodes contained within this parse node and its children.
        /// This method recursively traverses the parse tree to collect all variable references.
        /// </summary>
        /// <returns>A list of all VariableNode instances found in the parse tree.</returns>
        List<VariableNode> GetVariableNodes();
    }
}