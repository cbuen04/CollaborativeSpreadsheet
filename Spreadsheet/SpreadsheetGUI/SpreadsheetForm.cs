using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Authors: Daniel Kopta, Charly Bueno, Emma Kerr
    /// 
    /// This class implements Form and uses all spreadsheet classes up to this point to produce a working
    /// spreadsheet GUI. This is capable of calculating and displaying formulas, values, and text, as well
    /// as create dependencies on cells in real time and allows for user input and interaction.
    /// 
    /// The extra functionality impemented is a summation tool which sums together selected cells, and a "pinkify"
    /// option to control color scheme. 
    /// 
    /// </summary>
    public partial class SpreadsheetForm : Form
    {
        private Spreadsheet sheetData;
        public string fileName;
        private bool sumListenerOn;
        private List<string> selectedCells;
        private string sumDisplayCell;
        private bool pink;

        /// <summary>
        /// This is the constructor for the form which displays the actual GUI and initializes all variables and structures
        /// </summary>
        public SpreadsheetForm()
        {
            // file reader and saver
            fileName = "default";

            // initialization of spreadsheet
            InitializeComponent();
            sheetData = new Spreadsheet(isValid, s => s.ToUpper(), fileName);
            spreadsheetPanel1.SelectionChanged += SelectedCell;
            SelectedCell(spreadsheetPanel1);
            CellContentsBox.Focus(); 

            // sum functionality variables
            sumListenerOn = false;
            selectedCells = new List<string>();
            CalcSumButton.Visible = false;
            CancelSumButton.Visible = false;

            // pinkify functionality variables
            pink = false;
        }

        /// <summary>
        /// This method displays all cell information of the current cell selected. Changes the name, cell value, and cell conctents text boxes.
        /// </summary>
        /// <param name="panel">The spreadsheet panel in use to be changed</param>
        public void SelectedCell(SpreadsheetPanel panel)
        {
            // Gets the name and value of the cell selected by the user
            panel.GetSelection(out int col, out int row);
            panel.GetValue(col, row, out string currentValue);
            string CellName = GetCellName(col, row);
           
            // Displays the cell value in the main cell value text box
            CellValueBox.Text = sheetData.GetCellValue(CellNameBox.Text).ToString();

            // Gets the actual contents of the cell and checks what type it is so we can display in the contents box accurately
            object cellContents = sheetData.GetCellContents(CellNameBox.Text);
            if (cellContents.GetType() == typeof(Formula))
            {
                CellContentsBox.Text = "=" + sheetData.GetCellContents(CellNameBox.Text).ToString();
            }
            else
            {
                CellContentsBox.Text = sheetData.GetCellContents(CellNameBox.Text).ToString();
            }

            // Focuses the cursor on the end of the string in the contents editor box
            CellContentsBox.Focus();
            CellContentsBox.Select(CellContentsBox.Text.Length, 0);

            // Extra Functionality feature. Starts collecting cell names clicked after sum button is activated
            if (sumListenerOn)
            {
                spreadsheetPanel1.GetSelection(out int selectedCol, out int selectedRow);
                string selectedCellName = GetCellName(selectedCol, selectedRow);
                selectedCells.Add(selectedCellName);
            }

        }

        /// <summary>
        /// Helper method to translate rows and columns into string cell names
        /// </summary>
        /// <param name="col">Column input</param>
        /// <param name="row">Row input</param>
        /// <returns>Cell name corresponding to column and row</returns>
        private string GetCellName(int col, int row)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            // Mathces the column index to a letter
            string columnName = alphabet.ToCharArray()[col].ToString();
            // Matches the row index to the proper offset 
            string rowName = (row + 1).ToString();

            string cellName = columnName + rowName;
            CellNameBox.Text = cellName;

            return cellName;
        }

        /// <summary>
        /// Helper method that translates a cell name into a row and column 
        /// </summary>
        /// <param name="cellName">The cell name input</param>
        /// <param name="col">Out parameter which outputs the cell letter's corresponding column</param>
        /// <param name="row">Out parameter which outputs the cell number's corresponding row</param>
        private void GetCellLocation(string cellName, out int col, out int row)
        { 
            char colName = cellName.ToCharArray()[0];
            string rowName = cellName.Substring(1);

            // Matches the cut column and cell substrings to their letter and number values
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            col = alphabet.IndexOf(colName);
            row = int.Parse(rowName) - 1;   
        }

        /// <summary>
        /// Activates when a key is pressed. If the key pressed was an arrow key or enter, the cell will evaluate
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="e">The event when a key is pressed</param>
        private void CellContentsBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Detects where the initial cell location is
            spreadsheetPanel1.GetSelection(out int col, out int row);

            // If enter is pressed, moves the cell selected down and evaluates the original cell
            if (e.KeyCode == Keys.Enter)
            {
                evaluateCell(col, row, col, row + 1);
            }

            // If right arrow is pressed, moves the cell selected to the right and evaluates the original cell
            if (e.KeyCode == Keys.Right && col < 25)
            {
                evaluateCell(col, row, col + 1, row);
            }

            // If left arrow is pressed, moves the cell selected to the left and evaluates the original cell
            if (e.KeyCode == Keys.Left && col > 0)
            {
                evaluateCell(col, row, col - 1, row);
            }

            // If up arrow is pressed, moves the cell selected up and evaluates the original cell
            if (e.KeyCode == Keys.Up && row > 0)
            {
                evaluateCell(col, row, col, row - 1);
            }

            // If down arrow is pressed, moves the cell selected down and evaluates the original cell
            if (e.KeyCode == Keys.Down && row < 98)
            {
                evaluateCell(col, row, col, row + 1);
            }
        }

        /// <summary>
        /// Helper method which evaluates the cell at the input location  based on the cell contents and name boxes at those locations
        /// </summary>
        /// <param name="col">The original cell column to be evaluated</param>
        /// <param name="row">The original cell row to be evaluated</param>
        /// <param name="newCol">The new cell column have the selection moved to</param>
        /// <param name="newRow">The new cell row to have the selection moved to</param>
        private void evaluateCell(int col, int row, int newCol, int newRow)
        {
            // Gets the contents and name of the original cell selected
            string cellContents = CellContentsBox.Text;
            string originalCellName = CellNameBox.Text;
            string originalContents = sheetData.GetCellContents(originalCellName).ToString();
            string originalValue = sheetData.GetCellValue(originalCellName).ToString();
            List<string> dependents = new List<string>();

            // Tests for invalid formulas
            try
            {
                dependents = new List<string>(sheetData.SetContentsOfCell(originalCellName, cellContents));
            }
            catch(Exception e)
            {
                // Shows the error message and resets values
                System.Windows.Forms.MessageBox.Show(e.Message);
                spreadsheetPanel1.SetSelection(col, row);
                spreadsheetPanel1.SetValue(col, row, originalValue);
                CellContentsBox.Text = originalContents;
                CellValueBox.Text = originalValue;
                newCol = col;
                newRow = row; 
            }

            // Evaluating the contents at the cell location
            string cellValue = sheetData.GetCellValue(originalCellName).ToString();
            spreadsheetPanel1.SetValue(col, row, cellValue);
            CellValueBox.Text = sheetData.GetCellValue(originalCellName).ToString();

            // Resets the cells which depend on this changed value
            foreach (string dependent in dependents)
            {
                GetCellLocation(dependent, out int col2, out int row2);
                spreadsheetPanel1.SetValue(col2, row2, sheetData.GetCellValue(dependent).ToString());
            }

            // Reset selection
            SelectedCell(spreadsheetPanel1);
            spreadsheetPanel1.SetSelection(newCol, newRow);
            SelectedCell(spreadsheetPanel1);
            
        }

        /// <summary>
        /// Private helper method which serves as our formula variable/token validator to fit the standars of PS6
        /// </summary>
        /// <param name="token">The variable name token</param>
        /// <returns>True if this name fits the standards of the assignment, false otherwise</returns>
        private bool isValid(string token)
        {
            String validVarPattern = "^[a-zA-Z][1-9][0-9]{0,1}$";
            if (Regex.IsMatch(token, validVarPattern))
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// This detects when the user wishes to close the application. This will prompt the user to choose if they want to save the spreadsheet
        /// or not. This event also takes the necessary steps to ensure no data is lost.
        /// 
        /// This form communicates with the SavePopup form class and detects which option the user has selected when prompted to save.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e) // this needs to work
        {
            if (sheetData.Changed)
            {
                // disposable popup menu
                using (SavePopup savePopup = new SavePopup())
                {
                    // listens for a button to be pressed in the form
                    if (savePopup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        // boolean in the savePopup that detects if the save button was clicked
                        if (savePopup.GetSavedClicked())
                        {
                            // extra feature that checks if the user is saving for the first time (or file has already been named)
                            // if so, displays save dialogue
                            if (fileName == "default")
                            {
                                showSaveDialog();
                            }
                            // if not, the file automatically saves to its assigned name without needing user input
                            else
                            {
                                sheetData.Save(fileName + ".sprd");
                            }
                            this.Close();
                        }
                        // boolean that checks if the "don't save" button was selected
                        // if selected, closes without saving
                        else if (savePopup.GetDontSaveClicked())
                            this.Close();
                        // otherwise, user has cancelled the save operation
                        else
                            savePopup.Close();
                    }
                }
            }
            // Closes if the sheet has not been changed and does not need to be saved
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Helper method that displays the save dialog box, that allows the user to save the file to their system.
        /// </summary>
        private void showSaveDialog()
        {
            // Creates and shows a new save dialog and allows the user to only save as a sprd file
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Spreadsheet files (*.sprd)|*";
            saveFileDialog1.Title = "Save this Spreadsheet file";
            saveFileDialog1.ShowDialog();

            // Saves the file under the input filename if the user hasn't input an empty string
            if (saveFileDialog1.FileName != "")
            { 
                System.IO.FileStream fs =
                    (System.IO.FileStream)saveFileDialog1.OpenFile();
                sheetData.Save(saveFileDialog1.FileName + ".sprd");
                fileName = saveFileDialog1.FileName;
                fs.Close();
            }
        }

        /// <summary>
        /// Detects if the user wishes to create a new spreadsheet instance and displays the panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DemoApplicationContext.getAppContext().RunForm(new SpreadsheetForm()); 
        }

        /// <summary>
        /// Detects if the user wishes to open a new spreadsheet from an existing file on their system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFromExistingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // uses documentation code from Microsoft Forms. creates an open file dialog
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                // creates a filter that displays, either spreadsheet files (.sprd) or all files
                openFileDialog.Filter = "Spreadsheet files (*.sprd)|*.sprd|All files (*.*)|*.*"; 
                // sets default filter to .sprd
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                // determines if file attempted to open is an .sprd file
                try
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        filePath = openFileDialog.FileName;
                        string openedFileName = Path.GetFileName(filePath);
                        SpreadsheetForm openedSheet = new SpreadsheetForm();
                        // Creates a new spreadsheet with the specified filepath data and sets the version to defualt
                        openedSheet.sheetData = new Spreadsheet(filePath, isValid, s => s.ToUpper(), "default");
                        // Uses helper method to populate this sheet
                        openedSheet.fillSheet();
                        openedSheet.Show();
                        openedSheet.fileName = openedFileName;
                    }
                }
                // if user attempts to open any file that is not .sprd, an error is thrown
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Invalid File Format \n Cannot open file");
                }
                
            }

        }

        /// <summary>
        /// Helper method to populate sheet with the loaded spreadsheet data from file
        /// </summary>
        private void fillSheet() 
        {
            // Gets the name of all populated cells in the spreadsheet
            List<string> cellNames = new List<string>(sheetData.GetNamesOfAllNonemptyCells());
            foreach(string name in cellNames)
            {
                // Sets the cell boxes to display the proper values
                GetCellLocation(name, out int col, out int row);
                spreadsheetPanel1.SetValue(col, row, sheetData.GetCellValue(name).ToString());
            }
        }

        /// <summary>
        /// Necessary to display GUI 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Spreadsheet_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This button allows the user to save their existing file spreadsheet without the need of the save dialogue,
        /// as file will save under previously entered name.
        /// 
        /// If file has not been previously saved, the save dialoge will show.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveMenuButton_Click(object sender, EventArgs e)
        {
            // logic that automatically saves using current name unless
            // default has not been renamed.
            if (fileName == "default")
            {
                showSaveDialog();
            }
            else
            {
                sheetData.Save(fileName + ".sprd");
            }
        }

        /// <summary>
        /// Detects if the user has clicked to save a file and will trigger the showSaveDialog box to save as a file under a new name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsMenuButton_Click(object sender, EventArgs e)
        {
            showSaveDialog();
        }

        /// <summary>
        /// This is part of our extra feature that allows users to select multiple cells and automatically sum them together
        /// 
        /// This method detects if the user has requested to start a sum calculation and initializes all new necessary variables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Gets the currently selected cell of which the sum will be displayed in
            spreadsheetPanel1.GetSelection(out int col, out int row);
            sumDisplayCell = GetCellName(col, row);
            // Sets the boolean to tell the selected cell listener to start saving which cells are 
            // requested to be summed by the user in the SelectedCells instance variable
            sumListenerOn = true;
            // Displays the buttons which allow the user to click when done or when they want to cancel the operation
            CalcSumButton.Visible = true;
            CancelSumButton.Visible = true;
        }

        /// <summary>
        /// Runs after the user has selected all cells to be calculated and performs the necessary operations to sum 
        /// the requested cells. Displays the final sum value in the sumDisplayCell instance variable name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalcSumButton_Click(object sender, EventArgs e)
        {
            // Tells the selected cell listener to stop saving which cells are being selected
            sumListenerOn = false;
            string sumNames = "="; // if just equals, no sum selected
            bool firstDone = false;

            // Gets the saved listened to selected cells and adds this to a string which makes a formula adding all selected cells
            foreach(string cellName in selectedCells)
            {
                if (!firstDone)
                {
                    sumNames += cellName;
                    firstDone = true;
                }
                else
                    sumNames += "+" + cellName;
            }

            // Gets the location of the initially selected cell for the sum to be displayed and contained and sets 
            // values
            GetCellLocation(sumDisplayCell, out int col, out int row);
            spreadsheetPanel1.SetValue(col, row, sumNames);
            CellContentsBox.Text = sumNames;
            CellNameBox.Text = sumDisplayCell;
            evaluateCell(col, row, col, row);

            // Resets all instance variables so new sum can be made
            selectedCells = new List<string>();
            CalcSumButton.Visible = false;
            CancelSumButton.Visible = false;
        }

        /// <summary>
        /// Runs after user chooses to cancel the sum operation. Resets all sum functionality instance variables to be false
        /// so new sum can be made in the future. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelSumButton_Click(object sender, EventArgs e)
        {
            sumListenerOn = false;
            CalcSumButton.Visible = false;
            CancelSumButton.Visible = false;
            selectedCells = new List<string>();
        }

        /// <summary>
        /// Displays the HelpBasics class form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spreadsheetBasicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpBasics helpInstructions = new HelpBasics();
            helpInstructions.Show();
        }

        /// <summary>
        /// Extra functionality that changes the background color of the sheet to be pink. If pressed again, the 
        /// spreadsheet will revert to standard colors. 
        /// 
        /// This could be changed to darkmode in future releases of spreadsheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pinkifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!pink)
            {
                this.BackColor = Color.MistyRose;
                pink = true;
            }
            else
            {
                this.BackColor = Color.WhiteSmoke;
                pink = false;
            }
        }

        private void spreadsheetPanel1_Load(object sender, EventArgs e)
        {

        }
    }
}
