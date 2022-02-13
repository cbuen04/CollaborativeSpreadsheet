using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using SpreadsheetUtilities;
using SS;
using System.Linq;
using System.Xml;

namespace SpreadsheetTests
{
    // ps5 im in ps5 in ps5
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        public void TestAddNumberCell()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("a1", "2");
            Assert.AreEqual(spreadsheet.GetCellContents("a1"), 2.0);
        }
        
        [TestMethod]
        public void TestAddTextCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "hello grader! I hope you're having a good day :)");
            Assert.AreEqual(sheet.GetCellContents("a1"), "hello grader! I hope you're having a good day :)");
        }

        [TestMethod]
        public void TestAddFormulaCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=a5 + 1");
            Assert.AreEqual(sheet.GetCellContents("a1"), new Formula("a5 + 1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidNameDouble()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("21", "2");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidNameText()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("21", "hello world!");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestInvalidNameFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("21", "=2+3");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNullNameFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("", "=2+3");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNullNameDouble()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("", "2.3");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNullNameText()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("", "Thank god this assignment is due on saturday instead of friday lol");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullText()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            string str = null;
            sheet.SetContentsOfCell("a2", str);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullFormula() // revamp her
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula formula = new Formula("2");
            formula = null;
            sheet.SetContentsOfCell("a2", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetInvalidNameCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("3");
        }

        [TestMethod]
        public void TestGetEmptyCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Assert.AreEqual(sheet.GetCellContents("a2"), "");
        }

        [TestMethod]
        public void TestGetAllCellNames()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "2.0");
            sheet.SetContentsOfCell("a2", "2.0");
            sheet.SetContentsOfCell("a8", "2.0");
            sheet.SetContentsOfCell("z1", "2.0");
            List<string> occupiedCells = sheet.GetNamesOfAllNonemptyCells().ToList();
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            expectedList.Add("a2");
            expectedList.Add("a8");
            expectedList.Add("z1");
            for(int i = 0; i < expectedList.Count(); i ++)
            {
                Assert.AreEqual(expectedList.ElementAt(i), occupiedCells.ElementAt(i));
            }
        }

        [TestMethod]
        public void TestGetAllCellNamesAfterReassignment()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "2.0");
            sheet.SetContentsOfCell("a2", "2.0");
            sheet.SetContentsOfCell("a8", "2.0");
            sheet.SetContentsOfCell("a1", "2.0");
            List<string> occupiedCells = sheet.GetNamesOfAllNonemptyCells().ToList();
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            expectedList.Add("a2");
            expectedList.Add("a8");
            for (int i = 0; i < expectedList.Count(); i++)
            {
                Assert.AreEqual(expectedList.ElementAt(i), occupiedCells.ElementAt(i));
            }
        }

        [TestMethod]
        public void TestCellNameCaseSensitivity()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "hi");
            sheet.SetContentsOfCell("A1", "hii");
            List<string> occupiedCells = sheet.GetNamesOfAllNonemptyCells().ToList();
            List<string> expectedList = new List<string>();
            expectedList.Add("a1");
            expectedList.Add("A1");
            for (int i = 0; i < expectedList.Count(); i++)
            {
                Assert.AreEqual(expectedList.ElementAt(i), occupiedCells.ElementAt(i));
            }
        }

        [TestMethod]
        public void TestReassignTextCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "heya");
            sheet.SetContentsOfCell("a1", "how are ya");
            Assert.AreEqual(sheet.GetCellContents("a1"), "how are ya");
        }

        [TestMethod]
        public void TestReassignFormulaCell()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=a5 + 1");
            sheet.SetContentsOfCell("a1", "=2+3");
            Assert.AreEqual(sheet.GetCellContents("a1"), new Formula("2+3"));
        }

        [TestMethod]
        public void TestReassignFormulaCellThatWasInitiallyText()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "hey");
            sheet.SetContentsOfCell("a1", "=a5+1");
            Assert.AreEqual(sheet.GetCellContents("a1"), new Formula("a5 + 1"));
        }

        [TestMethod]
        public void TestGivenGetDirectDependentsExample()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "3");
            sheet.SetContentsOfCell("B1", "=A1 * A1");
            sheet.SetContentsOfCell("C1", "=B1 + A1");
            sheet.SetContentsOfCell("D1", "=B1 - C1");
            List<string> dependents = sheet.SetContentsOfCell("A1", "3").ToList();
            List<string> expectedDependents = new List<string>();
            expectedDependents.Add("A1");
            expectedDependents.Add("B1");
            expectedDependents.Add("C1");
            expectedDependents.Add("D1");
            for (int i = 0; i < expectedDependents.Count(); i++)
            {
                Assert.AreEqual(expectedDependents.ElementAt(i), dependents.ElementAt(i));
            }
        }

        [TestMethod]
        public void TestGivenGetDirectDependentWithFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "3");
            sheet.SetContentsOfCell("B1", "=A1 * A1");
            sheet.SetContentsOfCell("C1", "=B1 + A1");
            sheet.SetContentsOfCell("D1", "=B1 - C1");
            List<string> dependents = sheet.SetContentsOfCell("A1", "=b4").ToList();
            List<string> expectedDependents = new List<string>();
            expectedDependents.Add("A1");
            expectedDependents.Add("B1");
            expectedDependents.Add("C1");
            expectedDependents.Add("D1");
            for (int i = 0; i < expectedDependents.Count(); i++)
            {
                Assert.AreEqual(expectedDependents.ElementAt(i), dependents.ElementAt(i));
            }
        }

        [TestMethod]
        public void TestMultipleIndirectDependentWithFormula()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "3");
            sheet.SetContentsOfCell("B1", "=A1 * A1");
            sheet.SetContentsOfCell("C1", "=B1 + A1");
            sheet.SetContentsOfCell("D1", "=B1 - C1");
            sheet.SetContentsOfCell("E1", "=D1 - C1");
            sheet.SetContentsOfCell("F1", "=E1 - C1");
            List<string> dependents = sheet.SetContentsOfCell("A1", "=b4").ToList();
            List<string> expectedDependents = new List<string>();
            expectedDependents.Add("A1");
            expectedDependents.Add("B1");
            expectedDependents.Add("C1");
            expectedDependents.Add("D1");
            expectedDependents.Add("E1");
            expectedDependents.Add("F1");
            for (int i = 0; i < expectedDependents.Count(); i++)
            {
                Assert.AreEqual(expectedDependents.ElementAt(i), dependents.ElementAt(i));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestCircularException()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=a1+2");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestIndirectCircularException()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "=b1+2");
            sheet.SetContentsOfCell("b1", "=a1");
        }
        [TestMethod]
        public void TestGetCellNamesFromEmptySheet()
        {
            AbstractSpreadsheet spreadsheet = new Spreadsheet(); 
            Assert.IsTrue(spreadsheet.GetNamesOfAllNonemptyCells().Count() == 0);
        }

            // EMPTY SPREADSHEETS
            [TestMethod()]
            [TestCategory("1")]
            [ExpectedException(typeof(InvalidNameException))]
            public void TestEmptyGetNull()
            {
                Spreadsheet s = new Spreadsheet();
                s.GetCellContents(null);
            }

            [TestMethod()]
            [TestCategory("2")]
            [ExpectedException(typeof(InvalidNameException))]
            public void TestEmptyGetContents()
            {
                Spreadsheet s = new Spreadsheet();
                s.GetCellContents("1AA");
            }

            [TestMethod()]
            [TestCategory("3")]
            public void TestGetEmptyContents()
            {
                Spreadsheet s = new Spreadsheet();
                Assert.AreEqual("", s.GetCellContents("A2"));
            }

            // SETTING CELL TO A DOUBLE
            [TestMethod()]
            [TestCategory("4")]
            [ExpectedException(typeof(InvalidNameException))]
            public void TestSetNullDouble()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell(null, "1.5");
            }

            [TestMethod()]
            [TestCategory("5")]
            [ExpectedException(typeof(InvalidNameException))]
            public void TestSetInvalidNameDouble()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("1A1A", "1.5");
            }

            [TestMethod()]
            [TestCategory("6")]
            public void TestSimpleSetDouble()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("Z7", "1.5");
                Assert.AreEqual(1.5, (double)s.GetCellContents("Z7"), 1e-9);
            }

            // SETTING CELL TO A STRING
            [TestMethod()]
            [TestCategory("7")]
            [ExpectedException(typeof(ArgumentNullException))]
            public void TestSetNullStringVal()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A8", (string)null);
            }

            [TestMethod()]
            [TestCategory("8")]
            [ExpectedException(typeof(InvalidNameException))]
            public void TestSetNullStringName()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell(null, "hello");
            }

            [TestMethod()]
            [TestCategory("9")]
            [ExpectedException(typeof(InvalidNameException))]
            public void TestSetSimpleString()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("1AZ", "hello");
            }

            [TestMethod()]
            [TestCategory("10")]
            public void TestSetGetSimpleString()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("Z7", "hello");
                Assert.AreEqual("hello", s.GetCellContents("Z7"));
            }

            // SETTING CELL TO A FORMULA
            [TestMethod()]
            [TestCategory("11")]
            [ExpectedException(typeof(ArgumentNullException))]
            public void TestSetNullFormVal()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A8", null);
            }

            [TestMethod()]
            [TestCategory("12")]
            [ExpectedException(typeof(InvalidNameException))]
            public void TestSetNullFormName()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell(null, "2");
            }

            [TestMethod()]
            [TestCategory("13")]
            [ExpectedException(typeof(InvalidNameException))]
            public void TestSetSimpleForm()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("1AZ", "2");
            }

            [TestMethod()]
            [TestCategory("14")]
            public void TestSetGetForm()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("Z7", "=3");
                Formula f = (Formula)s.GetCellContents("Z7");
                Assert.AreEqual(new Formula("3"), f);
                Assert.AreNotEqual(new Formula("2"), f);
            }

            // CIRCULAR FORMULA DETECTION
            [TestMethod()]
            [TestCategory("15")]
            [ExpectedException(typeof(CircularException))]
            public void TestSimpleCircular()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "=A2");
                s.SetContentsOfCell("A2", "=A1");
            }

            [TestMethod()]
            [TestCategory("16")]
            [ExpectedException(typeof(CircularException))]
            public void TestComplexCircular()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A3", "=A4+A5");
                s.SetContentsOfCell("A5", "=A6+A7");
                s.SetContentsOfCell("A7", "=A1+A1");
            }

            [TestMethod()]
            [TestCategory("17")]
            [ExpectedException(typeof(CircularException))]
            public void TestUndoCircular()
            {
                Spreadsheet s = new Spreadsheet();
                try
                {
                    s.SetContentsOfCell("A1", "=A2+A3");
                    s.SetContentsOfCell("A2", "15");
                    s.SetContentsOfCell("A3", "30");
                    s.SetContentsOfCell("A2", "=A3*A1");
                }
                catch (CircularException e)
                {
                    object test = s.GetCellContents("A2");
                    Assert.AreEqual(15, (double)s.GetCellContents("A2"), 1e-9);
                    throw e;
                }
            }
        [TestMethod()]
        [TestCategory("17c")]
        [ExpectedException(typeof(CircularException))]
        public void TestUndoCircularEmptyCell()
        {
            Spreadsheet s = new Spreadsheet();
            try
            {
                s.SetContentsOfCell("A1", "=A1");
                
            }
            catch (CircularException e)
            {
                s.SetContentsOfCell("A1", "1");
                Assert.AreEqual(1, (double)s.GetCellValue("A1"), 1e-9);
                throw e;
            }
        }

        [TestMethod()]
            [TestCategory("17b")]
            [ExpectedException(typeof(CircularException))]
            public void TestUndoCellsCircular()
            {
                Spreadsheet s = new Spreadsheet();
                try
                {
                    s.SetContentsOfCell("A1", "=A2");
                    s.SetContentsOfCell("A2", "=A1");
                }
                catch (CircularException e)
                {
                    List<string> list = new List<string>(s.GetNamesOfAllNonemptyCells());
                    Assert.AreEqual("", s.GetCellContents("A2"));
                    Assert.IsTrue(new HashSet<string> { "A1" }.SetEquals(s.GetNamesOfAllNonemptyCells()));
                    throw e;
                }
            }

            // NONEMPTY CELLS
            [TestMethod()]
            [TestCategory("18")]
            public void TestEmptyNames()
            {
                Spreadsheet s = new Spreadsheet();
                Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
            }

            [TestMethod()]
            [TestCategory("19")]
            public void TestExplicitEmptySet()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("B1", "");
                Assert.IsFalse(s.GetNamesOfAllNonemptyCells().GetEnumerator().MoveNext());
            }

            [TestMethod()]
            [TestCategory("20")]
            public void TestSimpleNamesString()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("B1", "hello");
                Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
            }

            [TestMethod()]
            [TestCategory("21")]
            public void TestSimpleNamesDouble()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("B1", "52.25");
                Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
            }

            [TestMethod()]
            [TestCategory("22")]
            public void TestSimpleNamesFormula()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("B1", "=3.5");
                Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "B1" }));
            }

            [TestMethod()]
            [TestCategory("23")]
            public void TestMixedNames()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "17.2");
                s.SetContentsOfCell("C1", "hello");
                s.SetContentsOfCell("B1", "=3.5");
                Assert.IsTrue(new HashSet<string>(s.GetNamesOfAllNonemptyCells()).SetEquals(new HashSet<string>() { "A1", "B1", "C1" }));
            }

            // RETURN VALUE OF SET CELL CONTENTS
            [TestMethod()]
            [TestCategory("24")]
            public void TestSetSingletonDouble()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("B1", "hello");
                s.SetContentsOfCell("C1", "=5");
                Assert.IsTrue(s.SetContentsOfCell("A1", "17.2").SequenceEqual(new List<string>() { "A1" }));
            }

            [TestMethod()]
            [TestCategory("25")]
            public void TestSetSingletonString()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "17.2");
                s.SetContentsOfCell("C1", "=5");
                Assert.IsTrue(s.SetContentsOfCell("B1", "hello").SequenceEqual(new List<string>() { "B1" }));
            }

            [TestMethod()]
            [TestCategory("26")]
            public void TestSetSingletonFormula()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "17.2");
                s.SetContentsOfCell("B1", "hello");
                Assert.IsTrue(s.SetContentsOfCell("C1", "=5").SequenceEqual(new List<string>() { "C1" }));
            }

            [TestMethod()]
            [TestCategory("27")]
            public void TestSetChain()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A2", "6");
                s.SetContentsOfCell("A3", "=A2+A4");
                s.SetContentsOfCell("A4", "=A2+A5");
                Assert.IsTrue(s.SetContentsOfCell("A5", "82.5").SequenceEqual(new List<string>() { "A5", "A4", "A3", "A1" }));
            }

            // CHANGING CELLS
            [TestMethod()]
            [TestCategory("28")]
            public void TestChangeFtoD()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A1", "2.5");
                Assert.AreEqual(2.5, (double)s.GetCellContents("A1"), 1e-9);
            }

            [TestMethod()]
            [TestCategory("29")]
            public void TestChangeFtoS()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "=A2+A3");
                s.SetContentsOfCell("A1", "Hello");
                Assert.AreEqual("Hello", (string)s.GetCellContents("A1"));
            }

            [TestMethod()]
            [TestCategory("30")]
            public void TestChangeStoF()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "Hello");
                s.SetContentsOfCell("A1", "=23");
                Assert.AreEqual(new Formula("23"), (Formula)s.GetCellContents("A1"));
                Assert.AreNotEqual(new Formula("24"), (Formula)s.GetCellContents("A1"));
            }

            // STRESS TESTS
            [TestMethod()]
            [TestCategory("31")]
            public void TestStress1()
            {
                Spreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "=B1+B2");
                s.SetContentsOfCell("B1", "=C1-C2");
                s.SetContentsOfCell("B2", "=C3*C4");
                s.SetContentsOfCell("C1", "=D1*D2");
                s.SetContentsOfCell("C2", "=D3*D4");
                s.SetContentsOfCell("C3", "=D5*D6");
                s.SetContentsOfCell("C4", "=D7*D8");
                s.SetContentsOfCell("D1", "=E1");
                s.SetContentsOfCell("D2", "=E1");
                s.SetContentsOfCell("D3", "=E1");
                s.SetContentsOfCell("D4", "=E1");
                s.SetContentsOfCell("D5", "=E1");
                s.SetContentsOfCell("D6", "=E1");
                s.SetContentsOfCell("D7", "=E1");
                s.SetContentsOfCell("D8", "=E1");
                IList<String> cells = s.SetContentsOfCell("E1", "0");
                Assert.IsTrue(new HashSet<string>() { "A1", "B1", "B2", "C1", "C2", "C3", "C4", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "E1" }.SetEquals(cells));
            }

            // Repeated for extra weight
            [TestMethod()]
            [TestCategory("32")]
            public void TestStress1a()
            {
                TestStress1();
            }
            [TestMethod()]
            [TestCategory("33")]
            public void TestStress1b()
            {
                TestStress1();
            }
            [TestMethod()]
            [TestCategory("34")]
            public void TestStress1c()
            {
                TestStress1();
            }

            [TestMethod()]
            [TestCategory("35")]
            public void TestStress2()
            {
                Spreadsheet s = new Spreadsheet();
                ISet<String> cells = new HashSet<string>();
                for (int i = 1; i < 200; i++)
                {
                    cells.Add("A" + i);
                    Assert.IsTrue(cells.SetEquals(s.SetContentsOfCell("A" + i, "=A" + (i +1))));
                }
            }
            [TestMethod()]
            [TestCategory("36")]
            public void TestStress2a()
            {
                TestStress2();
            }
            [TestMethod()]
            [TestCategory("37")]
            public void TestStress2b()
            {
                TestStress2();
            }
            [TestMethod()]
            [TestCategory("38")]
            public void TestStress2c()
            {
                TestStress2();
            }

            [TestMethod()]
            [TestCategory("39")]
            public void TestStress3()
            {
                Spreadsheet s = new Spreadsheet();
                for (int i = 1; i < 200; i++)
                {
                    s.SetContentsOfCell("A" + i, "=A" + (i + 1));
                }
                try
                {
                    s.SetContentsOfCell("A150","=A50");
                    Assert.Fail();
                }
                catch (CircularException)
                {
                }
            }

            [TestMethod()]
            [TestCategory("40")]
            public void TestStress3a()
            {
                TestStress3();
            }
            [TestMethod()]
            [TestCategory("41")]
            public void TestStress3b()
            {
                TestStress3();
            }
            [TestMethod()]
            [TestCategory("42")]
            public void TestStress3c()
            {
                TestStress3();
            }

            [TestMethod()]
            [TestCategory("47")]
            public void TestStress5()
            {
                RunRandomizedTest(47, 2519);
            }

            [TestMethod()]
            [TestCategory("48")]
            public void TestStress6()
            {
                RunRandomizedTest(48, 2521);
            }

            [TestMethod()]
            [TestCategory("49")]
            public void TestStress7()
            {
                RunRandomizedTest(49, 2526);
            }

            [TestMethod()]
            [TestCategory("50")]
            public void TestStress8()
            {
                RunRandomizedTest(50, 2521);
            }

            /// <summary>
            /// Sets random contents for a random cell 10000 times
            /// </summary>
            /// <param name="seed">Random seed</param>
            /// <param name="size">The known resulting spreadsheet size, given the seed</param>
            public void RunRandomizedTest(int seed, int size)
            {
                Spreadsheet s = new Spreadsheet();
                Random rand = new Random(seed);
                for (int i = 0; i < 10000; i++)
                {
                    try
                    {
                        switch (rand.Next(3))
                        {
                            case 0:
                                s.SetContentsOfCell(randomName(rand), "3.14");
                                break;
                            case 1:
                                s.SetContentsOfCell(randomName(rand), "hello");
                                break;
                            case 2:
                                s.SetContentsOfCell(randomName(rand), randomFormula(rand));
                                break;
                        }
                    }
                    catch (CircularException)
                    {
                    }
                }
                ISet<string> set = new HashSet<string>(s.GetNamesOfAllNonemptyCells());
                Assert.AreEqual(size, set.Count);
            }

            /// <summary>
            /// Generates a random cell name with a capital letter and number between 1 - 99
            /// </summary>
            /// <param name="rand"></param>
            /// <returns></returns>
            private String randomName(Random rand)
            {
                return "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Substring(rand.Next(26), 1) + (rand.Next(99) + 1);
            }

            /// <summary>
            /// Generates a random Formula
            /// </summary>
            /// <param name="rand"></param>
            /// <returns></returns>
            private String randomFormula(Random rand)
            {
                String f = randomName(rand);
                for (int i = 0; i < 10; i++)
                {
                    switch (rand.Next(4))
                    {
                        case 0:
                            f += "+";
                            break;
                        case 1:
                            f += "-";
                            break;
                        case 2:
                            f += "*";
                            break;
                        case 3:
                            f += "/";
                            break;
                    }
                    switch (rand.Next(2))
                    {
                        case 0:
                            f += 7.2;
                            break;
                        case 1:
                            f += randomName(rand);
                            break;
                    }
                }
                return f;
            }

        // Now onto the PS5 added tests
        [TestMethod()]
        public void TestWriteSimpleSpreadsheet()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "2");
            ss.SetContentsOfCell("A2", "2");
            ss.Save("simplefile2.txt");
            Spreadsheet ss2 = new Spreadsheet("simplefile2.txt", s => true, s => s, "default");
        }

        [TestMethod()]
        public void TestGetDoubleCell()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "2");
            Assert.AreEqual(2.0, ss.GetCellValue("A1"));
        }
        [TestMethod()]
        public void TestGetValueFormulaCell()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=2+3");
            Assert.AreEqual(5.0, ss.GetCellValue("A1"));
        }
        [TestMethod()]
        public void TestGetValueFormulaCell2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "2");
            ss.SetContentsOfCell("A2", "2");
            ss.SetContentsOfCell("A3", "=A1+A2");
            Assert.AreEqual(4.0, ss.GetCellValue("A3"));
        }
        [TestMethod()]
        public void TestGetValueTextCell()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello");
            Assert.AreEqual("hello", ss.GetCellValue("A1"));
        }
        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestInvalidFileName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.Save("C:\\?this-is-not.png");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestInvalidFileNameReadAndLoad()
        {
            Spreadsheet ss = new Spreadsheet("weirdfile2.txt", s=>true, s=>s, "default");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetValueOfInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellValue("Hey");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetValueOfNullName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellValue(null);
        }

        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidFormulat()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=A5++");
        }

        [TestMethod()]
        [ExpectedException(typeof(CircularException))]
        public void TestSimpleCrDependency()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=A1");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNewValidityRules1()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("Z", "hey");
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNewValidityRules2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("X_", "hey");
        }

        [TestMethod()]
        public void TestLookupCantFindValue()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=A2");
            Assert.IsTrue(ss.GetCellValue("A1").GetType() == typeof(FormulaError));
        }

        [TestMethod()]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestNewValidityRules3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("Hello", "hey");
        }

        [TestMethod()]
        public void TestNewValidityRules4()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("X037", "hey");
        }

        [TestMethod()]
        public void TestNormalizer()
        {
            Spreadsheet ss = new Spreadsheet(s=>true, s=>s.ToUpper(), "default");
            ss.SetContentsOfCell("x037", "hey");
            Assert.AreEqual(ss.GetNamesOfAllNonemptyCells().ToList()[0], "X037");
        }

        [TestMethod()]
        public void TestNonExistant()
        {
            Spreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "default");
            ss.SetContentsOfCell("A1", "=A3");
        }

        [TestMethod()]
        public void TestNewValidityRules5()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a7", "hey");
        }
        [TestMethod()]
        public void TestConstructorWithoutFileSave()
        {
            Spreadsheet ss = new Spreadsheet(s=>false, s=>s, "new");
        }
        [TestMethod()]
        public void TestGetChanged()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "3");
            Assert.IsTrue(ss.Changed);
        }
        [TestMethod()]
        public void TestGetChangedAfterSaved()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "3");
            ss.Save("newFile442.txt");
            Assert.IsFalse(ss.Changed);
        }

        [TestMethod()]
        public void TestGetCellValueOfText()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hey");
            Assert.AreEqual(ss.GetCellValue("A1"), "hey");
        }

        [TestMethod()]
        public void TestLookupWorks()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=A2");
            ss.SetContentsOfCell("A2", "3");
            Assert.AreEqual(ss.GetCellValue("A1"), 3.0);
        }

        [TestMethod()]
        public void TestDefaultIsUnchanged()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.IsFalse(ss.Changed);
        }
        [TestMethod()]
        public void TestGetValueOfEmptyCell()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.AreEqual(ss.GetCellValue("A1"), "");
        }
        [TestMethod()]
        public void SaveContentsOfFormula()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "=A2+1");
            ss.Save("newfile22.txt");
        }

        [TestMethod()]
        public void SaveContentsOfText()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hey graders");
            ss.Save("newfile32.txt");
        }

        [TestMethod()]
        public void TestGivenXML()
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create("save2.txt")) // NOTICE the file with no path
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "hello");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
                List<string> cellList = ss.GetNamesOfAllNonemptyCells().ToList();
                Assert.IsTrue(cellList[0] == "A1");
                Assert.AreEqual("hello", ss.GetCellContents("A1"));
            }
            finally
            {
                File.Delete("save2.txt");
            }

           
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestInvalidNameDetectedRead()
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create("save22.txt")) // NOTICE the file with no path
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "Hello");
                    writer.WriteElementString("contents", "hello");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                Spreadsheet ss = new Spreadsheet("save22.txt", s => true, s => s, "");
            }
            finally
            {
                File.Delete("save22.txt");
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestCircularExceptionInRead()
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create("save32.txt")) // NOTICE the file with no path
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "=A1");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                Spreadsheet ss = new Spreadsheet("save32.txt", s => true, s => s, "");
            }
            finally
            {
                File.Delete("save32.txt");
            }
            
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestMisMatchVersion()
        {
            try
            {
                using (XmlWriter writer = XmlWriter.Create("save42.txt")) // NOTICE the file with no path
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "A1");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                Spreadsheet ss = new Spreadsheet("save42.txt", s => true, s => s, "default");
            }
            finally
            {
                File.Delete("save42.txt");
            }
        }
        /// <summary>
        ///This is a test class for SpreadsheetTest and is intended
        ///to contain all SpreadsheetTest Unit Tests
        ///</summary>
        [TestClass()]
        public class GradingTests
        {

            // Verifies cells and their values, which must alternate.
            public void VV(AbstractSpreadsheet sheet, params object[] constraints)
            {
                for (int i = 0; i < constraints.Length; i += 2)
                {
                    if (constraints[i + 1] is double)
                    {
                        Assert.AreEqual((double)constraints[i + 1], (double)sheet.GetCellValue((string)constraints[i]), 1e-9);
                    }
                    else
                    {
                        Assert.AreEqual(constraints[i + 1], sheet.GetCellValue((string)constraints[i]));
                    }
                }
            }


            // For setting a spreadsheet cell.
            public IEnumerable<string> Set(AbstractSpreadsheet sheet, string name, string contents)
            {
                List<string> result = new List<string>(sheet.SetContentsOfCell(name, contents));
                return result;
            }

            // Tests IsValid
            [TestMethod, Timeout(2000)]
            [TestCategory("1")]
            public void IsValidTest1()
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("A1", "x");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("2")]
            [ExpectedException(typeof(InvalidNameException))]
            public void IsValidTest2()
            {
                AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
                ss.SetContentsOfCell("A1", "x");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("3")]
            public void IsValidTest3()
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("B1", "= A1 + C1");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("4")]
            [ExpectedException(typeof(FormulaFormatException))]
            public void IsValidTest4()
            {
                AbstractSpreadsheet ss = new Spreadsheet(s => s[0] != 'A', s => s, "");
                ss.SetContentsOfCell("B1", "= A1 + C1");
            }

            // Tests Normalize
            [TestMethod, Timeout(2000)]
            [TestCategory("5")]
            public void NormalizeTest1()
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("B1", "hello");
                Assert.AreEqual("", s.GetCellContents("b1"));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("6")]
            public void NormalizeTest2()
            {
                AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
                ss.SetContentsOfCell("B1", "hello");
                Assert.AreEqual("hello", ss.GetCellContents("b1"));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("7")]
            public void NormalizeTest3()
            {
                AbstractSpreadsheet s = new Spreadsheet();
                s.SetContentsOfCell("a1", "5");
                s.SetContentsOfCell("A1", "6");
                s.SetContentsOfCell("B1", "= a1");
                Assert.AreEqual(5.0, (double)s.GetCellValue("B1"), 1e-9);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("8")]
            public void NormalizeTest4()
            {
                AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s.ToUpper(), "");
                ss.SetContentsOfCell("a1", "5");
                ss.SetContentsOfCell("A1", "6");
                ss.SetContentsOfCell("B1", "= a1");
                Assert.AreEqual(6.0, (double)ss.GetCellValue("B1"), 1e-9);
            }

            // Simple tests
            [TestMethod, Timeout(2000)]
            [TestCategory("9")]
            public void EmptySheet()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                VV(ss, "A1", "");
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("10")]
            public void OneString()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                OneString(ss);
            }

            public void OneString(AbstractSpreadsheet ss)
            {
                Set(ss, "B1", "hello");
                VV(ss, "B1", "hello");
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("11")]
            public void OneNumber()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                OneNumber(ss);
            }

            public void OneNumber(AbstractSpreadsheet ss)
            {
                Set(ss, "C1", "17.5");
                VV(ss, "C1", 17.5);
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("12")]
            public void OneFormula()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                OneFormula(ss);
            }

            public void OneFormula(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "5.2");
                Set(ss, "C1", "= A1+B1");
                VV(ss, "A1", 4.1, "B1", 5.2, "C1", 9.3);
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("13")]
            public void ChangedAfterModify()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                Assert.IsFalse(ss.Changed);
                Set(ss, "C1", "17.5");
                Assert.IsTrue(ss.Changed);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("13b")]
            public void UnChangedAfterSave()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                Set(ss, "C1", "17.5");
                ss.Save("changed.txt");
                Assert.IsFalse(ss.Changed);
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("14")]
            public void DivisionByZero1()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                DivisionByZero1(ss);
            }

            public void DivisionByZero1(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "0.0");
                Set(ss, "C1", "= A1 / B1");
                Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("14")]
            public void DivisionByZero4()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                DivisionByZero4(ss);
            }

            public void DivisionByZero4(AbstractSpreadsheet ss)
            {
               List<string> list = new List<string>(Set(ss, "A1", "=1/0"));
              
                Assert.IsInstanceOfType(ss.GetCellValue("A1"), typeof(FormulaError));

                list.ToString();
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("15")]
            public void DivisionByZero2()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                DivisionByZero2(ss);
            }

            public void DivisionByZero2(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "5.0");
                Set(ss, "A3", "= A1 / 0.0");
                Assert.IsInstanceOfType(ss.GetCellValue("A3"), typeof(FormulaError));
            }



            [TestMethod, Timeout(2000)]
            [TestCategory("16")]
            public void EmptyArgument()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                EmptyArgument(ss);
            }

            public void EmptyArgument(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "C1", "= A1 + B1");
                Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("17")]
            public void StringArgument()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                StringArgument(ss);
            }

            public void StringArgument(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "hello");
                Set(ss, "C1", "= A1 + B1");
                Assert.IsInstanceOfType(ss.GetCellValue("C1"), typeof(FormulaError));
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("18")]
            public void ErrorArgument()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                ErrorArgument(ss);
            }

            public void ErrorArgument(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "B1", "");
                Set(ss, "C1", "= A1 + B1");
                Set(ss, "D1", "= C1");
                Assert.IsInstanceOfType(ss.GetCellValue("D1"), typeof(FormulaError));
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("19")]
            public void NumberFormula1()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                NumberFormula1(ss);
            }

            public void NumberFormula1(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.1");
                Set(ss, "C1", "= A1 + 4.2");
                VV(ss, "C1", 8.3);
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("20")]
            public void NumberFormula2()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                NumberFormula2(ss);
            }

            public void NumberFormula2(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "= 4.6");
                VV(ss, "A1", 4.6);
            }


            // Repeats the simple tests all together
            [TestMethod, Timeout(2000)]
            [TestCategory("21")]
            public void RepeatSimpleTests()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                Set(ss, "A1", "17.32");
                Set(ss, "B1", "This is a test");
                Set(ss, "C1", "= A1+B1");
                OneString(ss);
                OneNumber(ss);
                OneFormula(ss);
                DivisionByZero1(ss);
                DivisionByZero2(ss);
                StringArgument(ss);
                ErrorArgument(ss);
                NumberFormula1(ss);
                NumberFormula2(ss);
            }

            // Four kinds of formulas
            [TestMethod, Timeout(2000)]
            [TestCategory("22")]
            public void Formulas()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                Formulas(ss);
            }

            public void Formulas(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "4.4");
                Set(ss, "B1", "2.2");
                Set(ss, "C1", "= A1 + B1");
                Set(ss, "D1", "= A1 - B1");
                Set(ss, "E1", "= A1 * B1");
                Set(ss, "F1", "= A1 / B1");
                VV(ss, "C1", 6.6, "D1", 2.2, "E1", 4.4 * 2.2, "F1", 2.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("23")]
            public void Formulasa()
            {
                Formulas();
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("24")]
            public void Formulasb()
            {
                Formulas();
            }


            // Are multiple spreadsheets supported?
            [TestMethod, Timeout(2000)]
            [TestCategory("25")]
            public void Multiple()
            {
                AbstractSpreadsheet s1 = new Spreadsheet();
                AbstractSpreadsheet s2 = new Spreadsheet();
                Set(s1, "X1", "hello");
                Set(s2, "X1", "goodbye");
                VV(s1, "X1", "hello");
                VV(s2, "X1", "goodbye");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("26")]
            public void Multiplea()
            {
                Multiple();
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("27")]
            public void Multipleb()
            {
                Multiple();
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("28")]
            public void Multiplec()
            {
                Multiple();
            }

            // Reading/writing spreadsheets
            [TestMethod, Timeout(2000)]
            [TestCategory("29")]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest1()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                ss.Save(Path.GetFullPath("/missing/save.txt"));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("30")]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest2()
            {
                AbstractSpreadsheet ss = new Spreadsheet(Path.GetFullPath("/missing/save.txt"), s => true, s => s, "");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("31")]
            public void SaveTest3()
            {
                AbstractSpreadsheet s1 = new Spreadsheet();
                Set(s1, "A1", "hello");
                s1.Save("save1.txt");
                s1 = new Spreadsheet("save1.txt", s => true, s => s, "default");
                Assert.AreEqual("hello", s1.GetCellContents("A1"));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("32")]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest4()
            {
                using (StreamWriter writer = new StreamWriter("save2.txt"))
                {
                    writer.WriteLine("This");
                    writer.WriteLine("is");
                    writer.WriteLine("a");
                    writer.WriteLine("test!");
                }
                AbstractSpreadsheet ss = new Spreadsheet("save2.txt", s => true, s => s, "");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("33")]
            [ExpectedException(typeof(SpreadsheetReadWriteException))]
            public void SaveTest5()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                ss.Save("save3.txt");
                ss = new Spreadsheet("save3.txt", s => true, s => s, "version");
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("34")]
            public void SaveTest6()
            {
                AbstractSpreadsheet ss = new Spreadsheet(s => true, s => s, "hello");
                ss.Save("save4.txt");
                new Spreadsheet().GetSavedVersion("save4.txt");
                Assert.AreEqual("hello", new Spreadsheet().GetSavedVersion("save4.txt"));
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("35")]
            public void SaveTest7()
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = "  ";
                using (XmlWriter writer = XmlWriter.Create("save5.txt", settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", "");

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A1");
                    writer.WriteElementString("contents", "hello");
                    writer.WriteEndElement();

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A2");
                    writer.WriteElementString("contents", "5.0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A3");
                    writer.WriteElementString("contents", "4.0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("cell");
                    writer.WriteElementString("name", "A4");
                    writer.WriteElementString("contents", "= A2 + A3");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                AbstractSpreadsheet ss = new Spreadsheet("save5.txt", s => true, s => s, "");
                VV(ss, "A1", "hello", "A2", 5.0, "A3", 4.0, "A4", 9.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("36")]
            public void SaveTest8()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                Set(ss, "A1", "hello");
                Set(ss, "A2", "5.0");
                Set(ss, "A3", "4.0");
                Set(ss, "A4", "= A2 + A3");
                ss.Save("save6.txt");
                using (XmlReader reader = XmlReader.Create("save6.txt"))
                {
                    int spreadsheetCount = 0;
                    int cellCount = 0;
                    bool A1 = false;
                    bool A2 = false;
                    bool A3 = false;
                    bool A4 = false;
                    string name = null;
                    string contents = null;

                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    Assert.AreEqual("default", reader["version"]);
                                    spreadsheetCount++;
                                    break;

                                case "cell":
                                    cellCount++;
                                    break;

                                case "name":
                                    reader.Read();
                                    name = reader.Value;
                                    break;

                                case "contents":
                                    reader.Read();
                                    contents = reader.Value;
                                    break;
                            }
                        }
                        else
                        {
                            switch (reader.Name)
                            {
                                case "cell":
                                    if (name.Equals("A1")) { Assert.AreEqual("hello", contents); A1 = true; }
                                    else if (name.Equals("A2")) { Assert.AreEqual(5.0, Double.Parse(contents), 1e-9); A2 = true; }
                                    else if (name.Equals("A3")) { Assert.AreEqual(4.0, Double.Parse(contents), 1e-9); A3 = true; }
                                    else if (name.Equals("A4")) { contents = contents.Replace(" ", ""); Assert.AreEqual("=A2+A3", contents); A4 = true; }
                                    else Assert.Fail();
                                    break;
                            }
                        }
                    }
                    Assert.AreEqual(1, spreadsheetCount);
                    Assert.AreEqual(4, cellCount);
                    Assert.IsTrue(A1);
                    Assert.IsTrue(A2);
                    Assert.IsTrue(A3);
                    Assert.IsTrue(A4);
                }
            }


            // Fun with formulas
            [TestMethod, Timeout(2000)]
            [TestCategory("37")]
            public void Formula1()
            {
                Formula1(new Spreadsheet());
            }
            public void Formula1(AbstractSpreadsheet ss)
            {
                Set(ss, "a1", "= a2 + a3");
                Set(ss, "a2", "= b1 + b2");
                Assert.IsInstanceOfType(ss.GetCellValue("a1"), typeof(FormulaError));
                Assert.IsInstanceOfType(ss.GetCellValue("a2"), typeof(FormulaError));
                Set(ss, "a3", "5.0");
                Set(ss, "b1", "2.0");
                Set(ss, "b2", "3.0");
                VV(ss, "a1", 10.0, "a2", 5.0);
                Set(ss, "b2", "4.0");
                VV(ss, "a1", 11.0, "a2", 6.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("38")]
            public void Formula2()
            {
                Formula2(new Spreadsheet());
            }
            public void Formula2(AbstractSpreadsheet ss)
            {
                Set(ss, "a1", "= a2 + a3");
                Set(ss, "a2", "= a3");
                Set(ss, "a3", "6.0");
                VV(ss, "a1", 12.0, "a2", 6.0, "a3", 6.0);
                Set(ss, "a3", "5.0");
                VV(ss, "a1", 10.0, "a2", 5.0, "a3", 5.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("39")]
            public void Formula3()
            {
                Formula3(new Spreadsheet());
            }
            public void Formula3(AbstractSpreadsheet ss)
            {
                Set(ss, "a1", "= a3 + a5");
                Set(ss, "a2", "= a5 + a4");
                Set(ss, "a3", "= a5");
                Set(ss, "a4", "= a5");
                Set(ss, "a5", "9.0");
                VV(ss, "a1", 18.0);
                VV(ss, "a2", 18.0);
                Set(ss, "a5", "8.0");
                VV(ss, "a1", 16.0);
                VV(ss, "a2", 16.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("40")]
            public void Formula4()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                Formula1(ss);
                Formula2(ss);
                Formula3(ss);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("41")]
            public void Formula4a()
            {
                Formula4();
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("42")]
            public void MediumSheet()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                MediumSheet(ss);
            }

            public void MediumSheet(AbstractSpreadsheet ss)
            {
                Set(ss, "A1", "1.0");
                Set(ss, "A2", "2.0");
                Set(ss, "A3", "3.0");
                Set(ss, "A4", "4.0");
                Set(ss, "B1", "= A1 + A2");
                Set(ss, "B2", "= A3 * A4");
                Set(ss, "C1", "= B1 + B2");
                VV(ss, "A1", 1.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 3.0, "B2", 12.0, "C1", 15.0);
                Set(ss, "A1", "2.0");
                VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 4.0, "B2", 12.0, "C1", 16.0);
                Set(ss, "B1", "= A1 / A2");
                VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("43")]
            public void MediumSheeta()
            {
                MediumSheet();
            }


            [TestMethod, Timeout(2000)]
            [TestCategory("44")]
            public void MediumSave()
            {
                AbstractSpreadsheet ss = new Spreadsheet();
                MediumSheet(ss);
                ss.Save("save7.txt");
                ss = new Spreadsheet("save7.txt", s => true, s => s, "default");
                VV(ss, "A1", 2.0, "A2", 2.0, "A3", 3.0, "A4", 4.0, "B1", 1.0, "B2", 12.0, "C1", 13.0);
            }

            [TestMethod, Timeout(2000)]
            [TestCategory("45")]
            public void MediumSavea()
            {
                MediumSave();
            }


            // A long chained formula. Solutions that re-evaluate 
            // cells on every request, rather than after a cell changes,
            // will timeout on this test.
            // This test is repeated to increase its scoring weight
            [TestMethod, Timeout(6000)]
            [TestCategory("46")]
            public void LongFormulaTest()
            {
                object result = "";
                LongFormulaHelper(out result);
                Assert.AreEqual("ok", result);
            }

            [TestMethod, Timeout(6000)]
            [TestCategory("47")]
            public void LongFormulaTest2()
            {
                object result = "";
                LongFormulaHelper(out result);
                Assert.AreEqual("ok", result);
            }

            [TestMethod, Timeout(6000)]
            [TestCategory("48")]
            public void LongFormulaTest3()
            {
                object result = "";
                LongFormulaHelper(out result);
                Assert.AreEqual("ok", result);
            }

            [TestMethod, Timeout(6000)]
            [TestCategory("49")]
            public void LongFormulaTest4()
            {
                object result = "";
                LongFormulaHelper(out result);
                Assert.AreEqual("ok", result);
            }

            [TestMethod, Timeout(6000)]
            [TestCategory("50")]
            public void LongFormulaTest5()
            {
                object result = "";
                LongFormulaHelper(out result);
                Assert.AreEqual("ok", result);
            }

            public void LongFormulaHelper(out object result)
            {
                try
                {
                    AbstractSpreadsheet s = new Spreadsheet();
                    s.SetContentsOfCell("sum1", "= a1 + a2");
                    int i;
                    int depth = 100;
                    for (i = 1; i <= depth * 2; i += 2)
                    {
                        s.SetContentsOfCell("a" + i, "= a" + (i + 2) + " + a" + (i + 3));
                        s.SetContentsOfCell("a" + (i + 1), "= a" + (i + 2) + "+ a" + (i + 3));
                    }


                    s.SetContentsOfCell("a" + i, "1");
                    s.SetContentsOfCell("a" + (i + 1), "1");
                    Assert.AreEqual(Math.Pow(2, depth + 1), (double)s.GetCellValue("sum1"), 1.0);
                    s.SetContentsOfCell("a" + i, "0");
                    Assert.AreEqual(Math.Pow(2, depth), (double)s.GetCellValue("sum1"), 1.0);
                    s.SetContentsOfCell("a" + (i + 1), "0");
                    Assert.AreEqual(0.0, (double)s.GetCellValue("sum1"), 0.1);
                    result = "ok";
                }
                catch (Exception e)
                {
                    result = e;
                }
            }

        }
    }
}
