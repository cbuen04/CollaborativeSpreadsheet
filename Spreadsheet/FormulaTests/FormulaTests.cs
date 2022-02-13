using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        public static double Lookup(string s)
        {
            if (s == "a1")
                return 5;
            if (s == "A2")
                return 8;
            if (s == "a0")
                return 0;
            if (s == "&a")
                return 0;
            if (s == "b5")
                return 2;
            else
                throw new ArgumentException();
        }
        [TestMethod]
        public void TestSimpleExpression()
        {
            Formula simpleFormula = new Formula("3 + 5 + 7");
            Assert.AreEqual(simpleFormula.Evaluate(s => 0), 15d);
        }
        [TestMethod]
        public void TestSimpleExpressionNotDefault()
        {
            Formula simpleFormula = new Formula("3 + 5 + 7", s => s, s => true);
            Assert.AreEqual(simpleFormula.Evaluate(s => 0), 15d);
        }
        [TestMethod]
        public void TestValidVariable()
        {
            Formula simpleFormula = new Formula("3 + a5 + 7");
            Assert.AreEqual(simpleFormula.Evaluate(s => 0), 10d);
        }
        [TestMethod]
        public void TestValidVariable2()
        {
            Formula simpleFormula = new Formula("3 + _a5 + 7");
            Assert.AreEqual(simpleFormula.Evaluate(s => 0), 10d);
        }
        [TestMethod]
        public void TestValidVariable3()
        {
            Formula simpleFormula = new Formula("3 + _a_5_b + 7");
            Assert.AreEqual(simpleFormula.Evaluate(s => 0), 10d);
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidVariable()
        {
            Formula simpleFormula = new Formula("3 + 5_b + 7");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidVariableAfterNormalize()
        {
            Formula simpleFormula = new Formula("3 + b5 + 7", s => s.Remove(0,1), s => true);
        }
        [TestMethod]
        public void TestNormalize()
        {
            Formula simpleFormula = new Formula("3 + B5 + 7", s => s.ToLower(), s => true);
            Assert.AreEqual(simpleFormula.Evaluate(Lookup), 12d);
        }
        [TestMethod]
        public void TestNormalize2()
        {
            Formula simpleFormula = new Formula("3 + b5 + 7", s => s.ToLower(), s => true);
            Assert.AreEqual(simpleFormula.Evaluate(Lookup), 12d);
        }
        [TestMethod]
        public void TestParenthesis()
        {
            Formula simpleFormula = new Formula("(3 + B5) + 7", s => s.ToLower(), s => true);
            Assert.AreEqual(simpleFormula.Evaluate(Lookup), 12d);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidValidator()
        {
            Formula simpleFormula = new Formula("(3 + B5) + 7", s => s.ToLower(), s => s.Contains('a'));
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestCompletelyInvalidToken()
        {
            Formula simpleFormula = new Formula("(3 + @5) + 7", s => s.ToLower(), s => true);
        }
        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUnknownVariable()
        { 
            Formula simpleFormula = new Formula("+3 + B5) + 7", s => s.ToLower(), s => true);
        }
        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestImproperFinalVariable()
        { 
            Formula simpleFormula = new Formula("3 + 7 +", s => s.ToLower(), s => true);
        }
        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestTooManyParenthesis()
        {
            Formula simpleFormula = new Formula("(5 + 6)) + 6", s => s.ToLower(), s => true);
        }
        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestMismatchParenthesis()
        {
            Formula simpleFormula = new Formula("(5 + 6) + 6 + (5", s => s.ToLower(), s => true);
        }
        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidAfterNormalization()
        {
            Formula simpleFormula = new Formula("a5 + b6", s => s.ToUpper(), s => (s.Contains("a")));
        }
        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoTokens()
        {
            Formula simpleFormula = new Formula("", s => s.ToLower(), s => true);
        }
        [TestMethod(), Timeout(5000)]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestOpFollowingOp()
        {
            Formula simpleFormula = new Formula("5++5", s => s.ToLower(), s => true);
        }
        [TestMethod]
        public void TestEquals()
        {
            Formula simpleFormula = new Formula("3 + a_5 + 7");
            Formula simpleFormula2 = new Formula("3 + b_5 + 7");
            Assert.IsFalse(simpleFormula2.Equals(simpleFormula));
        }
        [TestMethod]
        public void TestEqualOverload()
        {
            Formula simpleFormula = new Formula("3 + a_5 + 7");
            Formula simpleFormula2 = new Formula("3 + a_5 + 7");
            Assert.IsTrue(simpleFormula==simpleFormula2);
        }
        [TestMethod]
        public void TestNotEqualOverload()
        {
            Formula simpleFormula = new Formula("3 + b_5 + 7");
            Formula simpleFormula2 = new Formula("3 + a_5 + 7");
            Boolean truth = (simpleFormula == simpleFormula2);
            Assert.IsTrue(simpleFormula!=simpleFormula2);
        }
        [TestMethod]
        public void TestGetVariables()
        {
            Formula simpleFormula = new Formula("a2 + b_5 + b6");
            List<String> expected = new List<String>();
            expected.Add("a2");
            expected.Add("b_5");
            expected.Add("b6");
            List<String> resultList = simpleFormula.GetVariables().ToList();
            for(int j = 0; j < expected.Count(); j++)
            {
                Assert.AreEqual(expected.ElementAt(j), resultList.ElementAt(j));
            }
        }
        [TestMethod]
        public void TestGetVariablesNormalized()
        {
            Formula simpleFormula = new Formula("x+y*z", s => s.ToUpper(), s => true);
            List<String> expected = new List<String>();
            expected.Add("X");
            expected.Add("Y");
            expected.Add("Z");
            List<String> resultList = simpleFormula.GetVariables().ToList();
            for (int j = 0; j < expected.Count(); j++)
            {
                Assert.AreEqual(expected.ElementAt(j), resultList.ElementAt(j));
            }
        }
        [TestMethod]
        public void TestGetVariablesNormalizedDuplicate()
        {
            Formula simpleFormula = new Formula("x+X*z", s => s.ToUpper(), s => true);
            List<String> expected = new List<String>();
            expected.Add("X");
            expected.Add("Z");
            List<String> resultList = simpleFormula.GetVariables().ToList();
            for (int j = 0; j < expected.Count(); j++)
            {
                Assert.AreEqual(expected.ElementAt(j), resultList.ElementAt(j));
            }
        }
        [TestMethod]
        public void TestGetVariablesDuplicate()
        {
            Formula simpleFormula = new Formula("x+X*z");
            List<String> expected = new List<String>();
            expected.Add("x");
            expected.Add("X");
            expected.Add("z");
            List<String> resultList = simpleFormula.GetVariables().ToList();
            for (int j = 0; j < expected.Count(); j++)
            {
                Assert.AreEqual(expected.ElementAt(j), resultList.ElementAt(j));
            }
        }

        // Now implementing tests from PS1 to make sure everything is good
        [TestMethod]
        public void TestSingleVariable()
        {
            Formula simpleFormula = new Formula("X3");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 12d);
        }
        [TestMethod]
        public void TestSSubtraction()
        {
            Formula simpleFormula = new Formula("5-2");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 3d);
        }
        [TestMethod]
        public void TestMultiplication()
        {
            Formula simpleFormula = new Formula("5*2");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 10d);
        }
        [TestMethod]
        public void TestLeftToRight()
        {
            Formula simpleFormula = new Formula("3*2+6");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 12d);
        }
        [TestMethod]
        public void TestRightToLeft()
        {
            Formula simpleFormula = new Formula("5+3*2");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 11d);
        }
        [TestMethod]
        public void TestParenthesis2()
        {
            Formula simpleFormula = new Formula("(2+3)*6");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 30d);
        }
        [TestMethod]
        public void TestOperatorAfterParenth()
        {
            Formula simpleFormula = new Formula("(1*1)-2/2");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 0d);
        }
        [TestMethod]
        public void TestComplexAndParenth()
        {
            Formula simpleFormula = new Formula("2 + 3 * 5 + (3 + 4 * 8) * 5 + 2");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 194d);
        }
        [TestMethod(), Timeout(5000)]
        public void TestDivideByZero2()
        {
            Formula simpleFormula = new Formula("5/0");
            Assert.IsTrue(simpleFormula.Evaluate(s => 0).GetType() == typeof(FormulaError));
        }
        [TestMethod(), Timeout(5000)]
        public void TestNoValueInLookup()
        {
            Formula simpleFormula = new Formula("5/a8");
            Assert.IsTrue(simpleFormula.Evaluate(Lookup).GetType() == typeof(FormulaError));
        }
        [TestMethod]
        public void TestComplexAndParenth2()
        {
            Formula simpleFormula = new Formula("2 + 3 * 5 + (3 * 4 * 8) * 5 + 2");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 499d);
        }
        [TestMethod]
        public void TestComplexAndParenth3()
        {
            Formula simpleFormula = new Formula("2 + 3 * 5 + (3 / 4 / 8) * 5 + 2");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 19.46875d);
        }
        [TestMethod]
        public void TestComplexAndParenth4()
        {
            Formula simpleFormula = new Formula("2 + 3 * 5 + (3 + 4 + 8) * 5 + 2");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 94d);
        }
        [TestMethod]
        public void TestComplexAndParenth5()
        {
            Formula simpleFormula = new Formula("2 + 3 * 5 + (3 - 4 - 8) * 5 + 2");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), -26d);
        }
        [TestMethod]
        public void TestComplexAndParenth6()
        {
            Formula simpleFormula = new Formula("3/(3 - 4 / 8)");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 1.2d);
        }
        [TestMethod]
        public void TestComplexAndParenthDivZero()
        {
            Formula simpleFormula = new Formula("3/(0)");
            Assert.IsTrue(simpleFormula.Evaluate(Lookup).GetType() == typeof(FormulaError));
        }
        [TestMethod]
        public void TestComplexAndParenth7()
        {
            Formula simpleFormula = new Formula("3*(3 - 4 / 8)");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 7.5d);
        }
        [TestMethod]
        public void TestComplexAndParenth8()
        {
            Formula simpleFormula = new Formula("3*(7)");
            Assert.AreEqual(simpleFormula.Evaluate(s => 12), 21d);
        }
        [TestMethod]
        public void TestGivenEqualsTest1()
        {
            Formula test1 = new Formula("x1+y2", s => s.ToUpper(), s => true);
            Formula test2 = new Formula("X1  +  Y2");
            Assert.IsTrue(test1.Equals(test2));
        }

        [TestMethod]
        public void TestGivenEqualsTest2()
        {
            Assert.IsFalse(new Formula("x1+y2").Equals(new Formula("X1+Y2")));
        }
        [TestMethod]
        public void TestGivenEqualsTest2Overload()
        {
            Assert.IsFalse(new Formula("x1+y2") == new Formula("X1+Y2"));
        }
        [TestMethod]
        public void TestGivenEqualsTest2OverloadNegated()
        {
            Assert.IsTrue(new Formula("x1+y2") != new Formula("X1+Y2"));
        }
        [TestMethod]
        public void TestGivenEqualsTest3()
        {
            Assert.IsFalse(new Formula("x1+y2").Equals(new Formula("y2+x1")));
        }
        [TestMethod]
        public void TestGivenEqualsTest3Overload()
        {
            Assert.IsFalse(new Formula("x1+y2") == new Formula("y2+x1"));
        }
        [TestMethod]
        public void TestGivenEqualsTest3OverloadNegated()
        {
            Assert.IsTrue(new Formula("x1+y2") != new Formula("y2+x1"));
        }

        [TestMethod]
        public void TestGivenEqualsTest4()
        {
            Assert.IsTrue(new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")));
        }
        [TestMethod]
        public void TestGivenEqualsTest4Overload()
        {
            Assert.IsTrue(new Formula("2.0 + x7") == new Formula("2.000 + x7"));
        }
        [TestMethod]
        public void TestGivenEqualsTest4OverloadNegated()
        {
            Assert.IsFalse(new Formula("2.0 + x7") != new Formula("2.000 + x7"));
        }
        [TestMethod]
        public void TestScientificNotationEquals()
        {
            Assert.IsTrue(new Formula("2.0 + 2.902e10").Equals(new Formula("2.000 + 2.9020e10")));
        }
        [TestMethod]
        public void TestScientificNotationEqualsOverload()
        {
            Assert.IsTrue(new Formula("2.0 + 2.902e10") == new Formula("2.000 + 2.9020e10"));
        }
        [TestMethod]
        public void TestScientificNotationEqualsOverloadNegated()
        {
            Assert.IsFalse(new Formula("2.0 + 2.902e10") != new Formula("2.000 + 2.9020e10"));
        }
        public void TestNullEqualsOverloadNegated2()
        {
            Assert.IsFalse(null != new Formula("2.000 + 2.9020e10"));
        }
        [TestMethod]
        public void TestHashCode()
        {
            Assert.AreEqual(new Formula("2.0 + 2.902e10").GetHashCode(), new Formula("2.0 + 2.9020e10").GetHashCode());
        }
        [TestMethod]
        public void TestNullEquals()
        {
            Assert.IsFalse(new Formula("2.0 + 2.9020e10").Equals(null));
        }
        [TestMethod]
        public void TestNonFormulaEquals()
        {
            Assert.IsFalse(new Formula("2.0 + 2.9020e10").Equals(7));
        }
        [TestMethod]
        public void TestNullEqualsOverload()
        {
            Formula nullFormula = new Formula("2.0 + 2.9020e10");
            nullFormula = null; 
            Assert.IsTrue(nullFormula == null);
        }
        [TestMethod]
        public void TestNullEqualsOverloadNegated()
        {
            Formula testFormula = new Formula("2.0 + 2.9020e10");
            Assert.IsTrue(testFormula != null);
        }
        [TestMethod]
        public void TestBothNullOverload()
        {
            Formula testFormula = new Formula("2.0 + 2.9020e10");
            testFormula = null;
            Assert.IsFalse(testFormula != null);
        }
        [TestMethod]
        public void TestGetError()
        {
            FormulaError error = new FormulaError("This Failed");
            Assert.AreEqual(error.Reason, "This Failed");
        }

        // Normalizer tests
        [TestMethod(), Timeout(2000)]
        [TestCategory("1")]
        public void TestNormalizerGetVars()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            HashSet<string> vars = new HashSet<string>(f.GetVariables());

            Assert.IsTrue(vars.SetEquals(new HashSet<string> { "X1" }));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("2")]
        public void TestNormalizerEquals()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("2+X1", s => s.ToUpper(), s => true);

            Assert.IsTrue(f.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("3")]
        public void TestNormalizerToString()
        {
            Formula f = new Formula("2+x1", s => s.ToUpper(), s => true);
            Formula f2 = new Formula(f.ToString());

            Assert.IsTrue(f.Equals(f2));
        }

        // Validator tests
        [TestMethod(), Timeout(2000)]
        [TestCategory("4")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorFalse()
        {
            Formula f = new Formula("2+x1", s => s, s => false);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("5")]
        public void TestValidatorX1()
        {
            Formula f = new Formula("2+x", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("6")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX2()
        {
            Formula f = new Formula("2+y1", s => s, s => (s == "x"));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("7")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidatorX3()
        {
            Formula f = new Formula("2+x1", s => s, s => (s == "x"));
        }


        // Simple tests that return FormulaErrors
        [TestMethod(), Timeout(2000)]
        [TestCategory("8")]
        public void TestUnknownVariable2()
        {
            Formula f = new Formula("2+X1");
            Assert.IsInstanceOfType(f.Evaluate(s => { throw new ArgumentException("Unknown variable"); }), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("9")]
        public void TestDivideByZero()
        {
            Formula f = new Formula("5/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("10")]
        public void TestDivideByZeroVars()
        {
            Formula f = new Formula("(5 + X1) / (X1 - 3)");
            Assert.IsInstanceOfType(f.Evaluate(s => 3), typeof(FormulaError));
        }


        // Tests of syntax errors detected by the constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("11")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSingleOperator()
        {
            Formula f = new Formula("+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("12")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOperator()
        {
            Formula f = new Formula("2+5+");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("13")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraCloseParen()
        {
            Formula f = new Formula("2+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("14")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraOpenParen()
        {
            Formula f = new Formula("((3+5*7)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("15")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator()
        {
            Formula f = new Formula("5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("16")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator2()
        {
            Formula f = new Formula("5+5x");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("17")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator3()
        {
            Formula f = new Formula("5+7+(5)8");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("18")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestNoOperator4()
        {
            Formula f = new Formula("5 5");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("19")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestDoubleOperator()
        {
            Formula f = new Formula("5 + + 3");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("20")]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            Formula f = new Formula("");
        }

        // Some more complicated formula evaluations
        [TestMethod(), Timeout(2000)]
        [TestCategory("21")]
        public void TestComplex1()
        {
            Formula f = new Formula("y1*3-8/2+4*(8-9*2)/14*x7");
            Assert.AreEqual(5.14285714285714, (double)f.Evaluate(s => (s == "x7") ? 1 : 4), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("22")]
        public void TestRightParens()
        {
            Formula f = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, (double)f.Evaluate(s => 1), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("23")]
        public void TestLeftParens()
        {
            Formula f = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f.Evaluate(s => 2), 1e-9);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("53")]
        public void TestRepeatedVar()
        {
            Formula f = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, (double)f.Evaluate(s => 3), 1e-9);
        }

        // Test of the Equals method
        [TestMethod(), Timeout(2000)]
        [TestCategory("24")]
        public void TestEqualsBasic()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula("X1+X2");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("25")]
        public void TestEqualsWhitespace()
        {
            Formula f1 = new Formula("X1+X2");
            Formula f2 = new Formula(" X1  +  X2   ");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("26")]
        public void TestEqualsDouble()
        {
            Formula f1 = new Formula("2+X1*3.00");
            Formula f2 = new Formula("2.00+X1*3.0");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("27")]
        public void TestEqualsComplex()
        {
            Formula f1 = new Formula("1e-2 + X5 + 17.00 * 19 ");
            Formula f2 = new Formula("   0.0100  +     X5+ 17 * 19.00000 ");
            Assert.IsTrue(f1.Equals(f2));
        }


        [TestMethod(), Timeout(2000)]
        [TestCategory("28")]
        public void TestEqualsNullAndString()
        {
            Formula f = new Formula("2");
            Assert.IsFalse(f.Equals(null));
            Assert.IsFalse(f.Equals(""));
        }


        // Tests of == operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("29")]
        public void TestEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("30")]
        public void TestEqFalse()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsFalse(f1 == f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("31")]
        public void TestEqNull()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(null == f1);
            Assert.IsFalse(f1 == null);
            Assert.IsTrue(f1 == f2);
        }


        // Tests of != operator
        [TestMethod(), Timeout(2000)]
        [TestCategory("32")]
        public void TestNotEq()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("2");
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("33")]
        public void TestNotEqTrue()
        {
            Formula f1 = new Formula("2");
            Formula f2 = new Formula("5");
            Assert.IsTrue(f1 != f2);
        }


        // Test of ToString method
        [TestMethod(), Timeout(2000)]
        [TestCategory("34")]
        public void TestString()
        {
            Formula f = new Formula("2*5");
            f.ToString();
            Assert.IsTrue(f.Equals(new Formula(f.ToString())));
        }


        // Tests of GetHashCode method
        [TestMethod(), Timeout(2000)]
        [TestCategory("35")]
        public void TestHashCode2()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("2*5");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        // Technically the hashcodes could not be equal and still be valid,
        // extremely unlikely though. Check their implementation if this fails.
        [TestMethod(), Timeout(2000)]
        [TestCategory("36")]
        public void TestHashCodeFalse()
        {
            Formula f1 = new Formula("2*5");
            Formula f2 = new Formula("3/8*2+(7)");
            Assert.IsTrue(f1.GetHashCode() != f2.GetHashCode());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("37")]
        public void TestHashCodeComplex()
        {
            Formula f1 = new Formula("2 * 5 + 4.00 - _x");
            Formula f2 = new Formula("2*5+4-_x");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }


        // Tests of GetVariables method
        [TestMethod(), Timeout(2000)]
        [TestCategory("38")]
        public void TestVarsNone()
        {
            Formula f = new Formula("2*5");
            Assert.IsFalse(f.GetVariables().GetEnumerator().MoveNext());
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("39")]
        public void TestVarsSimple()
        {
            Formula f = new Formula("2*X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("40")]
        public void TestVarsTwo()
        {
            Formula f = new Formula("2*X2+Y3");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "Y3", "X2" };
            Assert.AreEqual(actual.Count, 2);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("41")]
        public void TestVarsDuplicate()
        {
            Formula f = new Formula("2*X2+X2");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X2" };
            Assert.AreEqual(actual.Count, 1);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("42")]
        public void TestVarsComplex()
        {
            Formula f = new Formula("X1+Y2*X3*Y2+Z7+X1/Z8");
            List<string> actual = new List<string>(f.GetVariables());
            HashSet<string> expected = new HashSet<string>() { "X1", "Y2", "X3", "Z7", "Z8" };
            Assert.AreEqual(actual.Count, 5);
            Assert.IsTrue(expected.SetEquals(actual));
        }

        // Tests to make sure there can be more than one formula at a time
        [TestMethod(), Timeout(2000)]
        [TestCategory("43")]
        public void TestMultipleFormulae()
        {
            Formula f1 = new Formula("2 + a1");
            Formula f2 = new Formula("3");
            Assert.AreEqual(2.0, f1.Evaluate(x => 0));
            Assert.AreEqual(3.0, f2.Evaluate(x => 0));
            Assert.IsFalse(new Formula(f1.ToString()) == new Formula(f2.ToString()));
            IEnumerator<string> f1Vars = f1.GetVariables().GetEnumerator();
            IEnumerator<string> f2Vars = f2.GetVariables().GetEnumerator();
            Assert.IsFalse(f2Vars.MoveNext());
            Assert.IsTrue(f1Vars.MoveNext());
        }

        // Repeat this test to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("44")]
        public void TestMultipleFormulaeB()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("45")]
        public void TestMultipleFormulaeC()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("46")]
        public void TestMultipleFormulaeD()
        {
            TestMultipleFormulae();
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("47")]
        public void TestMultipleFormulaeE()
        {
            TestMultipleFormulae();
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("48")]
        public void TestConstructor()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // This test is repeated to increase its weight
        [TestMethod(), Timeout(2000)]
        [TestCategory("49")]
        public void TestConstructorB()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("50")]
        public void TestConstructorC()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        [TestMethod(), Timeout(2000)]
        [TestCategory("51")]
        public void TestConstructorD()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

        // Stress test for constructor
        [TestMethod(), Timeout(2000)]
        [TestCategory("52")]
        public void TestConstructorE()
        {
            Formula f = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2+3.1)-.00000000008)");
        }

    }
}

