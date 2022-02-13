using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Displays our text file containing the help instructions if the user asks for it
    /// </summary>
    public partial class HelpBasics : Form
    {
        /// <summary>
        /// Constructor which Initializes form
        /// </summary>
        public HelpBasics()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Helper method to open the help file and display it in the text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpBasics_Load(object sender, EventArgs e)
        {
            string filetext = File.ReadAllText(@"../../../Help.txt");
            richTextBox1.Text = filetext;

        }
    }
}
