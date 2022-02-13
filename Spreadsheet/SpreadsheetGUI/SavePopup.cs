using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// This class implements Form to display a pop up prompting the user to either save, close, or not save the file/popup.
    /// </summary>
    public partial class SavePopup : Form
    {
        // Instance variables to detect which option the user chose
        private bool savePressed;
        private bool dontSavePressed;

        /// <summary>
        /// Constructor to initialize the save popup.
        /// </summary>
        public SavePopup()
        {
            InitializeComponent();
            savePressed = false;
            dontSavePressed = false;
        }

        /// <summary>
        /// If cancel button is clicked, close the instance of the popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Sets the instance variable (accessible by the spreadsheet) to true if
        /// the don't save option is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DontSaveButton_Click(object sender, EventArgs e)
        {
            dontSavePressed = true;
        }

        private void SavePopup_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Sets the instance variable to true saying the save button was clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            savePressed = true;
        }

        /// <summary>
        /// A public getter to access whether the save was pressed
        /// </summary>
        /// <returns>True or false if the button was pressed by the user</returns>
        public bool GetSavedClicked()
        {
            return savePressed;
        }

        /// <summary>
        /// A public getter to access whether the don't save was pressed
        /// </summary>
        /// <returns>True or false if the button was pressed by the user</returns>
        public bool GetDontSaveClicked()
        {
            return dontSavePressed;
        }
    }
}
