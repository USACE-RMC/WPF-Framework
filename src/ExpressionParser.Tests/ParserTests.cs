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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionParser.Tests
{
    /// <summary>
    /// Contains unit tests that validate expression parsing, evaluation,
    /// and functional correctness of the ExpressionParser library.
    /// </summary>
    [TestClass()]
    public class ParserTests
    {
        /// <summary>
        /// Tests addition and subtraction expressions, including invalid input handling.
        /// </summary>
        [TestMethod()]
        public void AddSubtractTests()
        {
            Assert.AreEqual(Parser.Parser.Parse("10 + 20").Evaluate().Result, 30);
            Assert.AreEqual(Parser.Parser.Parse("10 - 20").Evaluate().Result, -10);
            Assert.AreEqual(Parser.Parser.Parse("10 + 20 - 40 + 100").Evaluate().Result, 90);
            Assert.AreEqual(Parser.Parser.Parse("1 + 1 1").Evaluate().Type, ResultType.Error);
        }

        /// <summary>
        /// Tests behavior of parenthesis in expressions including malformed and nested forms.
        /// </summary>
        [TestMethod()]
        public void ParenthesisTests()
        {
            Assert.AreEqual(Parser.Parser.Parse("2(3+5)").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("(3+5)2").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("2*(3+5)").Evaluate().Result, 16);
            Assert.AreEqual(Parser.Parser.Parse("1-(1-3+5)*(2-1)+5").Evaluate().Result, 3);
        }

        /// <summary>
        /// Tests evaluation of comparison operators and invalid comparisons.
        /// </summary>
        [TestMethod()]
        public void ComparisonTests()
        {
            Assert.AreEqual(Parser.Parser.Parse("3>cat").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse(">5").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("3>5").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("3>=5").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("3>=3").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("3>3").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("3=5").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("3<5").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("3<>5").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("3!=5").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("5<>5").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("5!=5").Evaluate().Result, false);

        }

        /// <summary>
        /// Tests ROUND, ROUNDUP, and ROUNDDOWN functions across a range of precision values.
        /// </summary>
        [TestMethod()]
        public void RoundingTests()
        {
            Assert.AreEqual(Parser.Parser.Parse("=ROUND(31.1543,-1)").Evaluate().Result, 30);
            Assert.AreEqual(Parser.Parser.Parse("=ROUND(31.1543,0)").Evaluate().Result, 31);
            Assert.AreEqual(Parser.Parser.Parse("=ROUND(31.1543,1)").Evaluate().Result, 31.2d);

            Assert.AreEqual(Parser.Parser.Parse("=ROUNDUP(31.1231,-1)").Evaluate().Result, 40);
            Assert.AreEqual(Parser.Parser.Parse("=ROUNDUP(31.1231,0)").Evaluate().Result, 32);
            Assert.AreEqual(Parser.Parser.Parse("=ROUNDUP(31.1231,1)").Evaluate().Result, 31.2d);

            Assert.AreEqual(Parser.Parser.Parse("=ROUNDDOWN(31.1231,-1)").Evaluate().Result, 30);
            Assert.AreEqual(Parser.Parser.Parse("=ROUNDDOWN(31.1231,0)").Evaluate().Result, 31);
            Assert.AreEqual(Parser.Parser.Parse("=ROUNDDOWN(31.1231,1)").Evaluate().Result, 31.1d);
        }

        /// <summary>
        /// Tests random number functions including deterministic seed-based results.
        /// </summary>
        [TestMethod()]
        public void RandomTests()
        {
            Assert.AreEqual(Parser.Parser.Parse("RAND()").Evaluate().Type, ResultType.Double);
            Assert.AreEqual(Parser.Parser.Parse("RAND(1)").Evaluate().Result, new Random(1).NextDouble());
            Assert.AreEqual(Parser.Parser.Parse("RANDINT()").Evaluate().Type, ResultType.Integer);
            Assert.AreEqual(Parser.Parser.Parse("RANDINT(1)").Evaluate().Result, new Random(1).Next());
            // 
            Assert.AreEqual(Parser.Parser.Parse("RANDBETWEEN(2.3,14)").Evaluate().Type, ResultType.Double);
            Assert.AreEqual(Parser.Parser.Parse("RANDBETWEEN(-4,96,1)").Evaluate().Result, -4 + (96 - (-4)) * new Random(1).NextDouble());
            Assert.AreEqual(Parser.Parser.Parse("RANDINTBETWEEN(1,5)").Evaluate().Type, ResultType.Integer);
            Assert.AreEqual(Parser.Parser.Parse("RANDINTBETWEEN(4,18,1)").Evaluate().Result, new Random(1).Next(4, 18));
        }

        /// <summary>
        /// Tests logical functions AND and OR, including error handling for invalid inputs.
        /// </summary>
        [TestMethod()]
        public void AndOrTests()
        {
            Assert.AreEqual(Parser.Parser.Parse("AND(3>5,1<5)").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("OR(3>5,1<5)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("AND(3<5,1<5,1-1=0,3^2=9,3/3=1)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("OR(3>5,1<5,4-2)").Evaluate().Type, ResultType.Error);
        }

        /// <summary>
        /// Tests the conditional IF function, including various conditions and error handling.
        /// </summary>
        [TestMethod()]
        public void IFTests()
        {
            Assert.AreEqual(Parser.Parser.Parse("IF(3>5,1,2)").Evaluate().Result, 2);
            Assert.AreEqual(Parser.Parser.Parse("If(3>5,True,3)").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("If(3>5,,3)").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("If(3>5,True,)").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("If(3<5,1-(1-3+5)*(2-1)+5,8)").Evaluate().Result, 3);
            Assert.AreEqual(Parser.Parser.Parse("If(AND(3<5,1<5,1-1=0,3^2=9,3/3=1),RAND(1),4-2)").Evaluate().Result, new Random(1).NextDouble());
            Assert.AreEqual(Parser.Parser.Parse("If(AND(3<5, 1<5, 1-1=1, 3^2=9, 3/3=1), RAND(1), 4-2)").Evaluate().Result, 2);
        }

        /// <summary>
        /// Tests the behavior of the INCREMENT function, including error handling for invalid inputs.
        /// </summary>
        [TestMethod()]
        public void IncrementTests()
        {
            Assert.AreEqual(Parser.Parser.Parse("INCREMENT(1, 2, 3)").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("INCREMENT()").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("INCREMENT('1', 2)").Evaluate().Type, ResultType.Error);
            IParserNode parseNode = Parser.Parser.Parse("INCREMENT(1, 2)");
            for (int i = 1; i <= 5; i += 2)
                Assert.AreEqual(parseNode.Evaluate().Result, i);
            // 
            parseNode = Parser.Parser.Parse("INCREMENT(5)");
            for (int i = 5; i <= 10; i++)
                Assert.AreEqual(parseNode.Evaluate().Result, i);
        }

        /// <summary>
        /// Tests string manipulation functions including concatenation, substring, and length checks.
        /// </summary>
        [TestMethod()]
        public void StringTests()
        {
            Assert.AreEqual(Parser.Parser.Parse('"' + "rabbit goat drizzle." + '"').Evaluate().Result, "rabbit goat drizzle.");
            Assert.AreEqual(Parser.Parser.Parse("'rabbit goat drizzle.'").Evaluate().Result, "rabbit goat drizzle.");
            Assert.AreEqual(Parser.Parser.Parse("'rabbit ' & " + "\r" + " & ' goat drizzle.'").Evaluate().Result, "rabbit " + "\r" + " goat drizzle.");
            Assert.AreEqual(Parser.Parser.Parse("rabbit goat drizzle.").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("CONTAINS('rabbit goat drizzle.','goat')").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("CONTAINS('rabbit goat drizzle.','y')").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("CONTAINS(3*4,12)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("CONTAINS(,12)").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("CONTAINS(,)").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("CONTAINS()").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("CONTAINS(,12,'1')").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("LEN('rabbit goat drizzle.')").Evaluate().Result, 20);
            Assert.AreEqual(Parser.Parser.Parse("LENGTH('rabbit goat drizzle.')").Evaluate().Result, 20);
            Assert.AreEqual(Parser.Parser.Parse("LEN('rabbit goat drizzle.')=20").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("LEN('rabbit goat drizzle.')>20").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("LEN('rabbit goat drizzle.')<20").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("LEN('rabbit goat drizzle.')-18").Evaluate().Result, 2);
            Assert.AreEqual(Parser.Parser.Parse("LEFT('rabbit goat drizzle.',3)").Evaluate().Result, "rab");
            Assert.AreEqual(Parser.Parser.Parse("RIGHT('rabbit goat drizzle.',3)").Evaluate().Result, "le.");
            Assert.AreEqual(Parser.Parser.Parse("LEFT('rabbit goat drizzle.',3,1)").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("LEFT('rabbit goat drizzle.')").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("LEFT()").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("RIGHT('rabbit goat drizzle.',3,1)").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("RIGHT('rabbit goat drizzle.')").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("RIGHT()").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("CONCATENATE('rabbit ', goat, ' ', 'drizzle.')").Evaluate().Result, "rabbit goat drizzle.");
            Assert.AreEqual(Parser.Parser.Parse("'rabbit ' & goat & ' ' & 'drizzle.'").Evaluate().Result, "rabbit goat drizzle.");
            Assert.AreEqual(Parser.Parser.Parse("CONCATENATE('rabbit ' & goat, ' ', 'drizzle.')").Evaluate().Result, "rabbit goat drizzle.");
            Assert.AreEqual(Parser.Parser.Parse("INSTRING(abcdef,def)").Evaluate().Result, 3);
            Assert.AreEqual(Parser.Parser.Parse("IndexOf(abcdef,def)").Evaluate().Result, 3);
            Assert.AreEqual(Parser.Parser.Parse("SUBSTRING('rabbit goat drizzle.', 0, 4)").Evaluate().Result, "rabb");
        }

        /// <summary>
        /// Tests variable handling in expressions, including setting and evaluating variable inputs.
        /// </summary>
        [TestMethod()]
        public void VariablesTests()
        {
            var variableTypes = new Dictionary<string, ResultType>() { { "var", ResultType.Integer }, { "var2", ResultType.Integer } };
            IParserNode parseNode = Parser.Parser.Parse("[var]*[var2] + 2 * 8 - ([var] + [var] * 2^([var2]*8-1))", default, variableTypes);
            var varNodes = parseNode.GetVariableNodes();
            // simple way to define values
            int var1;
            int var2 = 2;
            for (int i = 0; i <= 3; i++)
            {
                var1 = i;
                foreach (var @var in varNodes)
                {
                    if (@var.VariableName == "var")
                        @var.SetValue(var1);
                    if (@var.VariableName == "var2")
                        @var.SetValue(var2);
                }
                Assert.AreEqual(parseNode.Evaluate().Result, var1 * var2 + 2 * 8 - (var1 + var1 * Math.Pow(2d, var2 * 8 - 1)));
            }
            // Another way to define values
            string[] varNames = variableTypes.Keys.ToArray();
            int[] varVals = new int[] { 0, 2 };
            var nodeToValueIndices = new int[varNodes.Count];
            for (int i = 0; i < varNodes.Count; i++)
                nodeToValueIndices[i] = Array.IndexOf(varNames, varNodes[i].VariableName);
            for (int i = 0; i <= 3; i++)
            {
                varVals[0] = i;
                for (int j = 0; j < nodeToValueIndices.Length; j++)
                    varNodes[j].SetValue(varVals[nodeToValueIndices[j]]);
                Assert.AreEqual(parseNode.Evaluate().Result, varVals[0] * varVals[1] + 2 * 8 - (varVals[0] + varVals[0] * Math.Pow(2d, varVals[1] * 8 - 1)));
            }
            // Fail due to incorrect input types
            variableTypes = new Dictionary<string, ResultType>() { { "var", ResultType.Integer }, { "var2", ResultType.String } };
            parseNode = Parser.Parser.Parse("[var]*[var2] + 2 * 8 - ([var] + [var] * 2^([var2]*8-1))", default, variableTypes);
            Assert.AreEqual(parseNode.Evaluate().Type, ResultType.Error);

        }

        /// <summary>
        /// Tests type conversion functions including error handling for invalid conversions.
        /// </summary>
        [TestMethod()]
        public void ConverterTests()
        {
            Assert.AreEqual(Parser.Parser.Parse("tostring(3)>=3").Evaluate().Type, ResultType.Error);
            Assert.AreEqual(Parser.Parser.Parse("TOINTEGER(3)>=3").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("int(3)>=3").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("cint(3)>=3").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("TOdouble(3)>=3").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("dbl(3)>=3").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("cdbl(3)>=3").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("tostring(3>=3)").Evaluate().Result, "True");
            Assert.AreEqual(Parser.Parser.Parse("str(3>=3)").Evaluate().Result, "True");
            Assert.AreEqual(Parser.Parser.Parse("cstr(3>=3)").Evaluate().Result, "True");
            Assert.AreEqual(Parser.Parser.Parse("cbool('True')").Evaluate().Result, true);
        }

        /// <summary>
        /// Tests complex logical combinations and application-specific parsing behavior.
        /// </summary>
        [TestMethod()]
        public void ConsequenceToolsTests()
        {
            Assert.AreEqual(Parser.Parser.Parse("contains(" + '"' + "ASDF" + '"' + ", A)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("Fi & sh=Fish").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("and(rand()<1, rand()>0)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("rand(2245)").Evaluate().Result, new Random(2245).NextDouble());
            Assert.AreEqual(Parser.Parser.Parse("IF((OR(1=1, 2=2)), True, False)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("IF((OR(1!=1, 2!=2)), True, False)").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("IF((OR(1+1!=1, 2!=2)), " + '"' + "True" + '"' + ", " + '"' + "False" + '"' + ")").Evaluate().Result, "True");
            Assert.AreEqual(Parser.Parser.Parse("If((Or(LEN('Hello') = 1, 2!=2)), True, False)").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("If((Or(2!=2,LEN('Hello')=1)),True,False)").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("If('Hello'=" + '"' + "Hello" + '"' + "," + '"' + "True" + '"' + "," + '"' + "False" + '"' + ")").Evaluate().Result, "True");
            Assert.AreEqual(Parser.Parser.Parse("If(And(1=1,2=1),True,False)").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("If(And(a=a,b=b),True,False)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("If(2^3>=(5+5),True,False)").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("LEN('hello')").Evaluate().Result, 5);
            Assert.AreEqual(Parser.Parser.Parse("If(2^(6-1)>=(1+5*5),True,False)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("And(abe=a,b=b)").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("And(a=a,b=b)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("And(Or(q=q,w=w),a=a)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("And(And(Or(q=q,w=w),a=a),Or(1=1,5=5))").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("And(And(Or(q=q,w=w),a=a),Or(1+1=2,5-1=4))").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("And(And(Or(q=q,w=w),a=a),Or((LEN('Hi'))=2,(5-1)=4))").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("And(And(Or((LEN('Hi'))=2,(LEN('Hi'))!=2),a=a),Or((LEN('Hi'))=2,(5-1)=4))").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("And(2^2>=4,1=1)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("abc=bd").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("(2^(6-1)-(1+5*5))").Evaluate().Result, 6d);
            Assert.AreEqual(Parser.Parser.Parse("((2+1*2)^(6-3))").Evaluate().Result, 64d);
            Assert.AreEqual(Parser.Parser.Parse("Hello=" + '"' + "Bye" + '"' + "").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("RIGHT(LEFT('hello world',8),5)").Evaluate().Result, "lo wo");
            Assert.AreEqual(Parser.Parser.Parse("LEN(RIGHT(hello,2))=2").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("LEN(k1234)").Evaluate().Result, 5);
            Assert.AreEqual(Parser.Parser.Parse("And(2!=2,'ac'='ac')").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("And(ac=ac,aBc=ac)").Evaluate().Result, false);
            Assert.AreEqual(Parser.Parser.Parse("And(ac=ac,aBc!=ac)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("LEN(RIGHT(wrong,2))=LEN(RIGHT(wrong,2))").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("LEN(hello)=LEN(hello)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("Hello!='Bye'").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("And(abe!=a,b=b)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("If(LEFT(hello,2)='he',True,False)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("If(a!=b,True,False)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("If(And(abe!=a,b=b),True,False)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("LEFT(abc,1)=LEFT(ad,1)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("RIGHT(hello,2)").Evaluate().Result, "lo");
            Assert.AreEqual(Parser.Parser.Parse("'str'=LEFT(string,3)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("LEFT(string,3)='str'").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("LEFT(string,3)=LEFT(string,3)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("RIGHT(string,3)").Evaluate().Result, "ing");
            Assert.AreEqual(Parser.Parser.Parse("ing=RIGHT(string,3)").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("st=st").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("steak!=steel").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("well!='wall'").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("123!=200-10").Evaluate().Result, true);
            Assert.AreEqual(Parser.Parser.Parse("3*5!=3*(2+3)").Evaluate().Result, false);

        }

    }
}