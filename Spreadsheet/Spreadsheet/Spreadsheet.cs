using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SpreadsheetUtilities;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace SS
{
    /// <summary>
    /// This class integrates the Formula class and DependencyGraph class in order to continue to progress towards the completed spreadsheet.
    /// This class has methods which store information in cells to be accessed, as well as maps the dependencies between cells using a DependencyGraph.
    /// PS5 has further implementation of the cell class to get cell values, and this class is also able to read and write XML files. 
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        private Dictionary<string, Cell> cells; // This Dictionary stores cell names and their contents as cell class objects
        private DependencyGraph dependencies; // This Dependency Graph stores the connections between cells as their dependents and dependees
        private bool FilePathExists; // needed to determine if a file path currently exists or if ss was created with no filepath.

        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }

        /// <summary>
        /// This is the default constructor of the Spreadsheet class which creates an empty cell dictionary, empty dependency graph, sets the vesion name to "default" and says that the spreadsheet has not been changed yet. 
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            cells = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
            Changed = false;
            FilePathExists = false;
        }

        /// <summary>
        /// This is the overloaded constructor of spreadsheet which inputs a user's validity and normalize function, as well as their desired version name
        /// </summary>
        /// <param name="isValid">The validity Func as passed by the user. Determines whether a token is valid</param>
        /// <param name="Normalize">The normalize func as passed by the user. Normalizes the function to fit some desired standard</param>
        /// <param name="version">Desired version name to be written in file if saved</param>
        public Spreadsheet(Func<string, Boolean> isValid, Func<string, string> Normalize, string version) : base(isValid, Normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
            Changed = false;
            FilePathExists = false;
        }

        /// <summary>
        /// This overloaded constructor takes in a filepath to be read and populated with cell elements in the spreadsheet. This too takes in a user's isValid func, normalize func, and version name.
        /// </summary>
        /// <param name="filepath">Desired filepath to be read to populate the spreadsheet</param>
        /// <param name="isValid">The validity Func as passed by the user. Determines whether a token is valid</param>
        /// <param name="Normalize">The normalize func as passed by the user. Normalizes the function to fit some desired standard</param>
        /// <param name="version">Desired version name to be written in file if saved</param>
        public Spreadsheet(string filepath, Func<string, Boolean> isValid, Func<string, string> Normalize, string version) : base(isValid, Normalize, version)
        {
            Changed = false;
            cells = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
            FilePathExists = true;
            GetSavedVersion(filepath);
        }

        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            // Checks validity/if there are null content or name references
            if (ReferenceEquals(content, null))
                throw new ArgumentNullException();
            if (ReferenceEquals(name, null) || !checkNameValidity(name))
                throw new InvalidNameException();

            // Sets the name to be the normalized version as dictated by the constructor
            name = Normalize(name);

            // Case where the input is a formula
            if (content.StartsWith("="))
            {
                Changed = true;
                return SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            }
            // Case where the input is a double
            else if (double.TryParse(content, out double value))
            {
                Changed = true;
                return SetCellContents(name, value);
            }
            // Case where the input is text/a string
            else
            {
                Changed = true;
                return SetCellContents(name, content);
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (ReferenceEquals(name, null) || !checkNameValidity(name))
                throw new InvalidNameException();
            name = Normalize(name);
            if (cells.TryGetValue(name, out Cell cell))
                return cell.getCellContents();
            else
                return "";
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            List<string> keyList = new List<string>(cells.Keys);
            return keyList;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            name = Normalize(name);
            // Instance where the cell does not exist in the spreadsheet yet
            if (!cells.ContainsKey(name))
            {
                Cell newCell = new Cell(number, Lookup);
                cells.Add(name, newCell);
            }
            // Instance where the cell does exist in the spreadsheet
            else
            {
                cells.TryGetValue(name, out Cell cell);
                if (cell.getCellContents().GetType() == typeof(Formula))
                {
                    Formula formula = (Formula)cell.getCellContents();
                    List<string> dependees = formula.GetVariables().ToList();
                    // Removing the old depencencies of the cell
                    foreach (string dependee in dependees)
                    {
                        dependencies.RemoveDependency(dependee, name);
                    }
                }
                // Replacing the contents of the cell with the new contents
                cell.setCellContents(number);
            }
            List<string> dependents = new List<string>();
            // Uses the GetCellsToRecalculate method in the abstract class to find all dependent cells
            dependents.AddRange(GetCellsToRecalculate(name));
            return dependents;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            name = Normalize(name);
            List<string> dependents = new List<string>();
            if (text == "")
                return dependents;
            // Instance where the cell does not exist in the spreadsheet yet
            else if (!cells.ContainsKey(name))
            {
                Cell newCell = new Cell(text, Lookup);
                cells.Add(name, newCell);
            }
            // Instance where the cell does exist in the spreadsheet
            else
            {
                cells.TryGetValue(name, out Cell cell);
                if (cell.getCellContents().GetType() == typeof(Formula))
                {
                    Formula formula = (Formula)cell.getCellContents();
                    List<string> dependees = formula.GetVariables().ToList();
                    foreach (string dependee in dependees)
                    {
                        dependencies.RemoveDependency(dependee, name);
                    }
                }
                cell.setCellContents(text);
            }
            dependents.AddRange(GetCellsToRecalculate(name));
            return dependents;
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula) 
        {
            name = Normalize(name);
            Object oldContents = "";
            List<string> oldDependees = new List<string>();
            // Instance where the cell does not exist in the spreadsheet yet
            if (!cells.ContainsKey(name))
            {
                Cell newCell = new Cell(formula, Lookup);
                cells.Add(name, newCell);
                // Finding the variables of the formula to see which cells will depend on it
                List<string> dependees = formula.GetVariables().ToList();
                // Adding the dependencies to the graph
                //if (!(formula.Evaluate(Lookup).GetType() == typeof(FormulaError)))
                {
                    foreach (string dependee in dependees)
                    {
                        dependencies.AddDependency(dependee, name);
                    }
                }
            }
            // Instance where the cell does exist in the spreadsheet
            else
            {
                cells.TryGetValue(name, out Cell cell);
                oldContents = cell.getCellContents();
                if (oldContents.GetType() == typeof(Formula))
                {
                    Formula oldFormula = (Formula)cell.getCellContents();
                    oldDependees = oldFormula.GetVariables().ToList();
                }
                cell.setCellContents(formula);
                //if (!(formula.Evaluate(Lookup).GetType() == typeof(FormulaError)))
                {
                    // Replacing the dependencies in the graph to fit the new formula
                    List<string> dependees = formula.GetVariables().ToList();
                    dependencies.ReplaceDependees(name, dependees);
                }
            }
            // Testing for circular dependencies
            try
            {
                GetCellsToRecalculate(name);
            }
            catch
            {

                    cells.TryGetValue(name, out Cell cell);
                    cell.setCellContents(oldContents);
                    dependencies.ReplaceDependees(name, oldDependees);

                if (GetCellContents(name).ToString() == "")
                {
                    cells.Remove(name);
                }
                throw new CircularException();
            }
            List<string> dependents = new List<string>();
            //if (!(formula.Evaluate(Lookup).GetType() == typeof(FormulaError)))
            {
                dependents.AddRange(GetCellsToRecalculate(name));
            }
            return dependents;
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            name = Normalize(name);
            return dependencies.GetDependents(name);
        }

        ///// <summary>
        ///// Helper method to check cell name validity based on the specifications: Variables for a Spreadsheet are only valid if they are one or more letters followed by one or more digits (numbers)
        ///// </summary>
        ///// <param name="name">The cell name to be tested for validity</param>
        ///// <returns>A boolean stating if valitity is true/valid</returns>
        public Boolean checkNameValidity(string name)
        {
            name = Normalize(name);
            String validVarPattern = "^[a-zA-Z]+[0-9]+$";
            if (Regex.IsMatch(name, validVarPattern))
            {
                // Now, if the token passes standard validity, check if the extra validity
                return IsValid(name);
            }
            else
                return false;
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            // Throws if there is no such file in existence
            if (!File.Exists(filename))
            {
                throw new SpreadsheetReadWriteException("The input filepath could not be found on this system");
            }
            try
            {
                // Create an XmlReader inside this block, and automatically Dispose() it at the end.
                using (XmlReader reader = XmlReader.Create(filename))
                {
                    // Sets the version and cell name/contents initially to nothing to be populated later
                    string detectedVersion = "";
                    string cellName = "";
                    string cellContents = "";
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "cell":
                                    break;
                                case "name":
                                    reader.Read();
                                    // We know name will go next, so storing cellName as the read contents
                                    cellName = reader.Value;
                                    break;
                                case "contents":
                                    reader.Read();
                                    // We know cell contents will go next, so storing cellName as the read contents
                                    cellContents = reader.Value;
                                    break;
                                case "spreadsheet":
                                    detectedVersion = reader.GetAttribute("version");
                                    break;
                            }
                            // Case where both the cell name and contents have been populated
                            if (!(cellName == "") && !(cellContents == ""))
                            {
                                // Checking name validity 
                                cellName = Normalize(cellName);
                                if (!checkNameValidity(cellName))
                                {
                                    throw new SpreadsheetReadWriteException("Invalid name detected");
                                }
                                // Testing for circular dependencies
                                try
                                {
                                    SetContentsOfCell(cellName, cellContents);
                                }
                                catch
                                {
                                    throw new SpreadsheetReadWriteException("Circular Dependency detected");
                                }
                                // setting cell values back to empty strings so next cell can be populated
                                cellName = "";
                                cellContents = "";
                            }
                        }
                    }
                    if (!(detectedVersion == Version) && FilePathExists)
                    {
                        // The version of the saved spreadsheet does not match the version parameter provided to the constructor
                        throw new SpreadsheetReadWriteException("Mismatched versions detected. Tried to access a version that doesn't exist in this file");
                    }
                    return detectedVersion;
                }

            }
            catch(Exception e)
            {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            // We want some non-default settings for our XML writer.
            // Specifically, use indentation to make it more readable.
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "  ";

            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);
                    List<string> occupiedCells = GetNamesOfAllNonemptyCells().ToList();
                    foreach (String cellName in occupiedCells)
                    {
                        writer.WriteStartElement("cell");
                        // Writing into each cell block the name and the contents
                        cells.TryGetValue(cellName, out Cell retrievedCell);
                        writer.WriteElementString("name", cellName);
                        writer.WriteElementString("contents", retrievedCell.ToString());
                        writer.WriteEndElement();
                    }
                    // Setting changed to false since it has now been saved
                    Changed = false;
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            catch
            {
                // the writer naturally throws when the filepath is not valid, so this catches that exception
                throw new SpreadsheetReadWriteException("The filepath input is not valid");
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (ReferenceEquals(name, null) || !checkNameValidity(name))
                throw new InvalidNameException();
            name = Normalize(name);
            // Case where cell has a value in the spreadsheet
            if (cells.TryGetValue(name, out Cell searchedCell))
                return searchedCell.getCellValue();
            // Returns an empty string otherwise, cell value blank for now
            else
                return "";
        }

        /// <summary>
        /// This method is the new designated lookup function for all of the spreadsheet. It either returns an double when the formula is evaluated/the contents are already a double, or an exception
        /// </summary>
        /// <param name="name">The name of the cell to be looked up</param>
        /// <returns></returns>
        private double Lookup(string name)
        {
            if (cells.TryGetValue(name, out Cell searchedCell) && searchedCell.getCellValue().GetType() == typeof(double))
            {
                return (double)searchedCell.getCellValue();
            }
            else
                throw new ArgumentException("The cell value of the variables could not be found");
        }
    }

    /// <summary>
    /// This class works as an object to hold information about cells. It stores the cells contents, and can be get and set. 
    /// </summary>
    class Cell
    {
        private object cellContents; // Holds the contents of the cell as input
        private Func<string, double> Lookup; // The lookup delegate passed in
        /// <summary>
        /// Constructs the cell and allows you to store content inside
        /// </summary>
        /// <param name="contents">Content of any type, but intended to be text, double, or formula</param>
        public Cell(Object contents, Func<string, double> lookup)
        {
            cellContents = contents;
            Lookup = lookup;
        }
        /// <summary>
        /// Allows to get the cell content
        /// </summary>
        /// <returns>The object stored within the cell</returns>
        public object getCellContents()
        {
            return cellContents;
        }
        /// <summary>
        /// Allows to set the cell content
        /// </summary>
        /// <param name="contents">The object you want to reset the cell content to</param>
        public void setCellContents(object contents)
        {
            cellContents = contents;
        }
        /// <summary>
        /// The new method to evaluate the formula/return the content double/return the text contents
        /// </summary>
        /// <returns>Evaluated formula, double as input, or text contents</returns>
        public object getCellValue()
        {
            if (cellContents.GetType() == typeof(Formula))
            {
                Formula cellFormula = (Formula)cellContents;
                return cellFormula.Evaluate(Lookup);
            }
            else if (cellContents.GetType() == typeof(double))
            {
                return cellContents;
            }
            else
                return cellContents;
        }
        /// <summary>
        /// ToString method to help with writing the contents of the cells into the xml file
        /// </summary>
        /// <returns>The string verison of the cell contents</returns>
        public override string ToString()
        {
            if (cellContents.GetType() == typeof(Formula))
            {
                Formula cellFormula = (Formula)cellContents;
                return "=" + cellFormula.ToString();
            }
            else if (cellContents.GetType() == typeof(double))
            {
                return cellContents.ToString();
            }
            else
                return (string)cellContents;
        }
    }
}