
namespace SpreadsheetGUI
{
    partial class SpreadsheetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpreadsheetForm));
            this.CellNameBox = new System.Windows.Forms.TextBox();
            this.CellValueBox = new System.Windows.Forms.TextBox();
            this.CellContentsBox = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.openFromExistingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.functionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pinkifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CellNameLabel = new System.Windows.Forms.Label();
            this.CellValueLabel = new System.Windows.Forms.Label();
            this.CellContentsLabel = new System.Windows.Forms.Label();
            this.CalcSumButton = new System.Windows.Forms.Button();
            this.CancelSumButton = new System.Windows.Forms.Button();
            this.spreadsheetBasicsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spreadsheetPanel1 = new SS.SpreadsheetPanel();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CellNameBox
            // 
            this.CellNameBox.Location = new System.Drawing.Point(13, 75);
            this.CellNameBox.Name = "CellNameBox";
            this.CellNameBox.ReadOnly = true;
            this.CellNameBox.Size = new System.Drawing.Size(175, 31);
            this.CellNameBox.TabIndex = 1;
            // 
            // CellValueBox
            // 
            this.CellValueBox.Location = new System.Drawing.Point(194, 75);
            this.CellValueBox.Name = "CellValueBox";
            this.CellValueBox.ReadOnly = true;
            this.CellValueBox.Size = new System.Drawing.Size(175, 31);
            this.CellValueBox.TabIndex = 2;
            // 
            // CellContentsBox
            // 
            this.CellContentsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CellContentsBox.Location = new System.Drawing.Point(877, 75);
            this.CellContentsBox.Name = "CellContentsBox";
            this.CellContentsBox.Size = new System.Drawing.Size(175, 31);
            this.CellContentsBox.TabIndex = 3;
            this.CellContentsBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CellContentsBox_KeyDown);
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.functionToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1127, 48);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.openFromExistingToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(72, 40);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(358, 44);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveMenuButton,
            this.saveAsMenuButton});
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(358, 44);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveMenuButton
            // 
            this.saveMenuButton.Name = "saveMenuButton";
            this.saveMenuButton.Size = new System.Drawing.Size(243, 44);
            this.saveMenuButton.Text = "Save ";
            this.saveMenuButton.Click += new System.EventHandler(this.saveMenuButton_Click);
            // 
            // saveAsMenuButton
            // 
            this.saveAsMenuButton.Name = "saveAsMenuButton";
            this.saveAsMenuButton.Size = new System.Drawing.Size(243, 44);
            this.saveAsMenuButton.Text = "Save as...";
            this.saveAsMenuButton.Click += new System.EventHandler(this.saveAsMenuButton_Click);
            // 
            // openFromExistingToolStripMenuItem
            // 
            this.openFromExistingToolStripMenuItem.Name = "openFromExistingToolStripMenuItem";
            this.openFromExistingToolStripMenuItem.Size = new System.Drawing.Size(358, 44);
            this.openFromExistingToolStripMenuItem.Text = "Open From Existing";
            this.openFromExistingToolStripMenuItem.Click += new System.EventHandler(this.openFromExistingToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(358, 44);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // functionToolStripMenuItem
            // 
            this.functionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sumToolStripMenuItem,
            this.pinkifyToolStripMenuItem});
            this.functionToolStripMenuItem.Name = "functionToolStripMenuItem";
            this.functionToolStripMenuItem.Size = new System.Drawing.Size(128, 40);
            this.functionToolStripMenuItem.Text = "Function";
            // 
            // sumToolStripMenuItem
            // 
            this.sumToolStripMenuItem.Name = "sumToolStripMenuItem";
            this.sumToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.sumToolStripMenuItem.Text = "Sum";
            this.sumToolStripMenuItem.Click += new System.EventHandler(this.sumToolStripMenuItem_Click);
            // 
            // pinkifyToolStripMenuItem
            // 
            this.pinkifyToolStripMenuItem.Name = "pinkifyToolStripMenuItem";
            this.pinkifyToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.pinkifyToolStripMenuItem.Text = "Pinkify 🤪💅 ";
            this.pinkifyToolStripMenuItem.Click += new System.EventHandler(this.pinkifyToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spreadsheetBasicsToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(85, 40);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // CellNameLabel
            // 
            this.CellNameLabel.Location = new System.Drawing.Point(13, 50);
            this.CellNameLabel.Name = "CellNameLabel";
            this.CellNameLabel.Size = new System.Drawing.Size(120, 22);
            this.CellNameLabel.TabIndex = 5;
            this.CellNameLabel.Text = "Cell Name";
            // 
            // CellValueLabel
            // 
            this.CellValueLabel.Location = new System.Drawing.Point(194, 50);
            this.CellValueLabel.Name = "CellValueLabel";
            this.CellValueLabel.Size = new System.Drawing.Size(120, 22);
            this.CellValueLabel.TabIndex = 6;
            this.CellValueLabel.Text = "Cell Value";
            // 
            // CellContentsLabel
            // 
            this.CellContentsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CellContentsLabel.Location = new System.Drawing.Point(872, 50);
            this.CellContentsLabel.Name = "CellContentsLabel";
            this.CellContentsLabel.Size = new System.Drawing.Size(142, 22);
            this.CellContentsLabel.TabIndex = 7;
            this.CellContentsLabel.Text = "Cell Contents";
            // 
            // CalcSumButton
            // 
            this.CalcSumButton.BackColor = System.Drawing.Color.MistyRose;
            this.CalcSumButton.Location = new System.Drawing.Point(375, 75);
            this.CalcSumButton.Name = "CalcSumButton";
            this.CalcSumButton.Size = new System.Drawing.Size(180, 44);
            this.CalcSumButton.TabIndex = 8;
            this.CalcSumButton.Text = "Σ = Calculate";
            this.CalcSumButton.UseVisualStyleBackColor = false;
            this.CalcSumButton.Click += new System.EventHandler(this.CalcSumButton_Click);
            // 
            // CancelSumButton
            // 
            this.CancelSumButton.BackColor = System.Drawing.Color.RosyBrown;
            this.CancelSumButton.Location = new System.Drawing.Point(571, 75);
            this.CancelSumButton.Name = "CancelSumButton";
            this.CancelSumButton.Size = new System.Drawing.Size(180, 44);
            this.CancelSumButton.TabIndex = 9;
            this.CancelSumButton.Text = "Cancel Sum";
            this.CancelSumButton.UseVisualStyleBackColor = false;
            this.CancelSumButton.Click += new System.EventHandler(this.CancelSumButton_Click);
            // 
            // spreadsheetBasicsToolStripMenuItem
            // 
            this.spreadsheetBasicsToolStripMenuItem.Name = "spreadsheetBasicsToolStripMenuItem";
            this.spreadsheetBasicsToolStripMenuItem.Size = new System.Drawing.Size(359, 44);
            this.spreadsheetBasicsToolStripMenuItem.Text = "Spreadsheet Basics";
            this.spreadsheetBasicsToolStripMenuItem.Click += new System.EventHandler(this.spreadsheetBasicsToolStripMenuItem_Click);
            // 
            // spreadsheetPanel1
            // 
            this.spreadsheetPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spreadsheetPanel1.Location = new System.Drawing.Point(12, 125);
            this.spreadsheetPanel1.Name = "spreadsheetPanel1";
            this.spreadsheetPanel1.Size = new System.Drawing.Size(1103, 641);
            this.spreadsheetPanel1.TabIndex = 0;
            this.spreadsheetPanel1.Load += new System.EventHandler(this.spreadsheetPanel1_Load);
            // 
            // SpreadsheetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1127, 768);
            this.Controls.Add(this.CancelSumButton);
            this.Controls.Add(this.CalcSumButton);
            this.Controls.Add(this.CellContentsLabel);
            this.Controls.Add(this.CellValueLabel);
            this.Controls.Add(this.CellNameLabel);
            this.Controls.Add(this.CellContentsBox);
            this.Controls.Add(this.CellValueBox);
            this.Controls.Add(this.CellNameBox);
            this.Controls.Add(this.spreadsheetPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SpreadsheetForm";
            this.Text = "eems and char Spreadsheet";
            this.Load += new System.EventHandler(this.Spreadsheet_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel spreadsheetPanel1;
        private System.Windows.Forms.TextBox CellNameBox;
        private System.Windows.Forms.TextBox CellValueBox;
        private System.Windows.Forms.TextBox CellContentsBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFromExistingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMenuButton;
        private System.Windows.Forms.ToolStripMenuItem saveAsMenuButton;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Label CellNameLabel;
        private System.Windows.Forms.Label CellValueLabel;
        private System.Windows.Forms.Label CellContentsLabel;
        private System.Windows.Forms.ToolStripMenuItem functionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sumToolStripMenuItem;
        private System.Windows.Forms.Button CalcSumButton;
        private System.Windows.Forms.Button CancelSumButton;
        private System.Windows.Forms.ToolStripMenuItem pinkifyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spreadsheetBasicsToolStripMenuItem;
    }
}

