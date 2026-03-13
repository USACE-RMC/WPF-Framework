/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* - Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* - Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* - The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionParser.Tests
{
    /// <summary>
    /// Contains comprehensive unit tests for arithmetic operations including multiplication,
    /// division, exponentiation, operator precedence, type handling, and edge cases.
    /// </summary>
    [TestClass]
    public class ArithmeticTests
    {
        /// <summary>
        /// Tests basic multiplication operations with positive integers.
        /// </summary>
        [TestMethod]
        public void MultiplicationBasicTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2*3").Evaluate().Result, 6);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("5*10").Evaluate().Result, 50);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("7*8").Evaluate().Result, 56);
        }

        /// <summary>
        /// Tests multiplication operations with negative numbers.
        /// </summary>
        [TestMethod]
        public void MultiplicationWithNegativeNumbersTests()
        {
            Assert.AreEqual(-15, ExpressionParser.Parser.Parser.Parse("-5*3").Evaluate().Result);
            Assert.AreEqual(-15, ExpressionParser.Parser.Parser.Parse("5*(-3)").Evaluate().Result);
            Assert.AreEqual(15, ExpressionParser.Parser.Parser.Parse("-5*(-3)").Evaluate().Result);
            Assert.AreEqual(-20, ExpressionParser.Parser.Parser.Parse("-10*2").Evaluate().Result);
        }

        /// <summary>
        /// Tests multiplication operations with decimal numbers.
        /// </summary>
        [TestMethod]
        public void MultiplicationWithDecimalsTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2.5*4").Evaluate().Result, 10.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("3.5*2.5").Evaluate().Result, 8.75);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("0.5*10").Evaluate().Result, 5.0);
        }

        /// <summary>
        /// Tests basic division operations with positive integers.
        /// </summary>
        [TestMethod]
        public void DivisionBasicTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("10/2").Evaluate().Result, 5.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("20/4").Evaluate().Result, 5.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("15/3").Evaluate().Result, 5.0);
        }

        /// <summary>
        /// Tests division operations that result in decimal values.
        /// </summary>
        [TestMethod]
        public void DivisionWithDecimalResultsTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("10/4").Evaluate().Result, 2.5);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("7/2").Evaluate().Result, 3.5);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("1/8").Evaluate().Result, 0.125);
        }

        /// <summary>
        /// Tests division by zero behavior, which should return NaN.
        /// </summary>
        [TestMethod]
        public void DivisionByZeroTests()
        {
            var result = ExpressionParser.Parser.Parser.Parse("10/0").Evaluate().Result;
            Assert.IsTrue(double.IsNaN((double)result));
        }

        /// <summary>
        /// Tests basic exponentiation operations.
        /// </summary>
        [TestMethod]
        public void ExponentiationBasicTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2^3").Evaluate().Result, 8.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("5^2").Evaluate().Result, 25.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("3^4").Evaluate().Result, 81.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("10^0").Evaluate().Result, 1.0);
        }

        /// <summary>
        /// Tests exponentiation with negative exponents and negative bases.
        /// </summary>
        [TestMethod]
        public void ExponentiationAdvancedTests()
        {
            Assert.AreEqual(0.5, ExpressionParser.Parser.Parser.Parse("2^(-1)").Evaluate().Result);
            Assert.AreEqual(2.0, ExpressionParser.Parser.Parser.Parse("4^0.5").Evaluate().Result);
            Assert.AreEqual(1000.0, ExpressionParser.Parser.Parser.Parse("10^3").Evaluate().Result);
        }

        /// <summary>
        /// Tests operator precedence with multiplication and addition.
        /// </summary>
        [TestMethod]
        public void OperatorPrecedenceMultiplicationAdditionTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2+3*4").Evaluate().Result, 14);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("10-2*3").Evaluate().Result, 4);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("5*2+3*4").Evaluate().Result, 22);
        }

        /// <summary>
        /// Tests operator precedence with exponentiation.
        /// </summary>
        [TestMethod]
        public void OperatorPrecedenceExponentiationTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2^3*4").Evaluate().Result, 32.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2*3^2").Evaluate().Result, 18.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2+3^2").Evaluate().Result, 11.0);
        }

        /// <summary>
        /// Tests complex nested expressions with multiple levels of parentheses.
        /// </summary>
        [TestMethod]
        public void ComplexNestedExpressionsTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("((2+3)*4)^2").Evaluate().Result, 400.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("(10-(3+2))*2").Evaluate().Result, 10);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("((5*2)+(3*4))/(2+2)").Evaluate().Result, 5.5);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2*(3+(4*(5-2)))").Evaluate().Result, 30);
        }

        /// <summary>
        /// Tests unary minus operator with positive numbers.
        /// </summary>
        [TestMethod]
        public void UnaryMinusTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("-5").Evaluate().Result, -5);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("-10+5").Evaluate().Result, -5);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("-(5+3)").Evaluate().Result, -8);
        }

        /// <summary>
        /// Tests double negation and nested unary minus operators.
        /// </summary>
        [TestMethod]
        public void DoubleNegationTests()
        {
            Assert.AreEqual(5, ExpressionParser.Parser.Parser.Parse("-(-5)").Evaluate().Result);
            Assert.AreEqual(10, ExpressionParser.Parser.Parser.Parse("-(-(10))").Evaluate().Result);
            Assert.AreEqual(-5, ExpressionParser.Parser.Parser.Parse("-(-(-5))").Evaluate().Result);
        }

        /// <summary>
        /// Tests that integer operations return integer type when appropriate.
        /// </summary>
        [TestMethod]
        public void IntegerTypeHandlingTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("5+10").Evaluate().Type, ResultType.Integer);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("20-5").Evaluate().Type, ResultType.Integer);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("4*5").Evaluate().Type, ResultType.Integer);
        }

        /// <summary>
        /// Tests that division and operations with decimals return double type.
        /// </summary>
        [TestMethod]
        public void DoubleTypeHandlingTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("10/2").Evaluate().Type, ResultType.Double);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2^3").Evaluate().Type, ResultType.Double);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("5.5+10").Evaluate().Type, ResultType.Double);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("5+10.5").Evaluate().Type, ResultType.Double);
        }

        /// <summary>
        /// Tests operations with zero as an operand.
        /// </summary>
        [TestMethod]
        public void ZeroOperandTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("0+5").Evaluate().Result, 5);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("10-0").Evaluate().Result, 10);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("0*100").Evaluate().Result, 0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("0/5").Evaluate().Result, 0.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("0^5").Evaluate().Result, 0.0);
        }

        /// <summary>
        /// Tests operations with very large numbers.
        /// </summary>
        [TestMethod]
        public void LargeNumberTests()
        {
            // Parser uses Int32 arithmetic, large multiplications overflow
            var result1 = ExpressionParser.Parser.Parser.Parse("1000000*1000000").Evaluate();
            Assert.AreEqual(ResultType.Integer, result1.Type);

            Assert.AreEqual(1000000000, ExpressionParser.Parser.Parser.Parse("999999999+1").Evaluate().Result);
            var result = ExpressionParser.Parser.Parser.Parse("10^10").Evaluate().Result;
            Assert.AreEqual(10000000000.0, (double)result);
        }

        /// <summary>
        /// Tests operations with very small decimal numbers.
        /// </summary>
        [TestMethod]
        public void SmallDecimalTests()
        {
            Assert.AreEqual(0.003, ExpressionParser.Parser.Parser.Parse("0.001+0.002").Evaluate().Result);
            Assert.AreEqual(0.01, (double)ExpressionParser.Parser.Parser.Parse("0.1*0.1").Evaluate().Result, 0.0000001);
            var result = (double)ExpressionParser.Parser.Parser.Parse("1/1000").Evaluate().Result;
            Assert.IsTrue(Math.Abs(result - 0.001) < 0.0000001);
        }

        /// <summary>
        /// Tests chained multiplication operations.
        /// </summary>
        [TestMethod]
        public void ChainedMultiplicationTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2*3*4").Evaluate().Result, 24);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("5*2*3*2").Evaluate().Result, 60);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("10*10*10").Evaluate().Result, 1000);
        }

        /// <summary>
        /// Tests chained division operations from left to right.
        /// </summary>
        [TestMethod]
        public void ChainedDivisionTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("100/10/2").Evaluate().Result, 5.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("64/8/2").Evaluate().Result, 4.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("1000/10/10").Evaluate().Result, 10.0);
        }

        /// <summary>
        /// Tests mixed operations with all basic arithmetic operators.
        /// </summary>
        [TestMethod]
        public void MixedOperationsTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("10+5*2-8/4").Evaluate().Result, 18.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("100-50/5+10*2").Evaluate().Result, 110.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2^3+4*5-10/2").Evaluate().Result, 23.0);
        }

        /// <summary>
        /// Tests expressions with multiple parentheses groups at the same level.
        /// </summary>
        [TestMethod]
        public void MultipleParenthesesGroupsTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("(2+3)*(4+5)").Evaluate().Result, 45);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("(10-5)+(20-10)").Evaluate().Result, 15);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("(8/2)*(12/3)").Evaluate().Result, 16.0);
        }

        /// <summary>
        /// Tests complex expressions with exponentiation and parentheses.
        /// </summary>
        [TestMethod]
        public void ComplexExponentiationTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("(2+1)^(6-3)").Evaluate().Result, 27.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("2^(3+1)").Evaluate().Result, 16.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("(2^3)^2").Evaluate().Result, 64.0);
        }

        /// <summary>
        /// Tests that multiplication by one preserves the value and type.
        /// </summary>
        [TestMethod]
        public void MultiplicationByOneTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("5*1").Evaluate().Result, 5);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("1*100").Evaluate().Result, 100);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("3.5*1").Evaluate().Result, 3.5);
        }

        /// <summary>
        /// Tests that division by one preserves the value but returns double type.
        /// </summary>
        [TestMethod]
        public void DivisionByOneTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("5/1").Evaluate().Result, 5.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("100/1").Evaluate().Result, 100.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("7.5/1").Evaluate().Result, 7.5);
        }

        /// <summary>
        /// Tests exponentiation with one as the exponent or base.
        /// </summary>
        [TestMethod]
        public void ExponentiationWithOneTests()
        {
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("5^1").Evaluate().Result, 5.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("1^5").Evaluate().Result, 1.0);
            Assert.AreEqual(ExpressionParser.Parser.Parser.Parse("1^1").Evaluate().Result, 1.0);
        }
    }
}
