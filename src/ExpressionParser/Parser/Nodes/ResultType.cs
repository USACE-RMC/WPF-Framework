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
    /// Represents the possible result types of a parse node evaluation.
    /// Bitwise flags are used to allow type grouping (e.g., numeric types).
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// Indicates that the result is invalid due to an error.
        /// </summary>
        Error = 0,

        /// <summary>
        /// Represents a double-precision floating-point number.
        /// </summary>
        Double = 1 << 0,

        /// <summary>
        /// Represents a single-precision floating-point number.
        /// </summary>
        Single = 1 << 1,

        /// <summary>
        /// Represents any floating-point type (Double or Single).
        /// </summary>
        Decimal = Double + Single,

        /// <summary>
        /// Represents a 16-bit integer (short).
        /// </summary>
        Short = 1 << 3,

        /// <summary>
        /// Represents a 32-bit integer (int).
        /// </summary>
        Integer = 1 << 4,

        /// <summary>
        /// Represents an 8-bit integer (byte).
        /// </summary>
        Byte = 1 << 5,

        /// <summary>
        /// Represents any whole number type (Short, Integer, or Byte).
        /// </summary>
        IntegerValue = Short + Integer + Byte,

        /// <summary>
        /// Represents any numeric type (floating point or integer).
        /// </summary>
        Number = Double + Single + Short + Integer + Byte,

        /// <summary>
        /// Represents a string value.
        /// </summary>
        String = 1 << 6,

        /// <summary>
        /// Represents a boolean value (true/false).
        /// </summary>
        Boolean = 1 << 7,

        /// <summary>
        /// Represents an undeclared or unknown result type.
        /// </summary>
        UnDeclared = 1 << 8,

        /// <summary>
        /// Represents any valid result type (excluding Error).
        /// </summary>
        Valid = Double + Single + Short + Integer + Byte + String + Boolean + UnDeclared
    }
}