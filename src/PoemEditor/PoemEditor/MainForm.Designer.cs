namespace PoemEditor
{
    partial class MainEditor
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.richTextBoxEditor = new System.Windows.Forms.RichTextBox();
            this.panelRight = new System.Windows.Forms.Panel();
            this.tabControlRight = new System.Windows.Forms.TabControl();
            this.Contexual = new System.Windows.Forms.TabPage();
            this.groupBoxSuggestion = new System.Windows.Forms.GroupBox();
            this.listBoxSuggestion = new System.Windows.Forms.ListBox();
            this.groupBoxSelectedWord = new System.Windows.Forms.GroupBox();
            this.comboBoxTheWord = new System.Windows.Forms.ComboBox();
            this.textBoxTheWord = new System.Windows.Forms.TextBox();
            this.Console = new System.Windows.Forms.TabPage();
            this.textBoxConsole = new System.Windows.Forms.TextBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.menuStrip1.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.tabControlRight.SuspendLayout();
            this.Contexual.SuspendLayout();
            this.groupBoxSuggestion.SuspendLayout();
            this.groupBoxSelectedWord.SuspendLayout();
            this.Console.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.optionsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.newToolStripMenuItem.Text = "&New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.openToolStripMenuItem.Text = "&Open";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(113, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(113, 6);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.optionsToolStripMenuItem.Text = "O&ptions";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readWordsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // readWordsToolStripMenuItem
            // 
            this.readWordsToolStripMenuItem.Name = "readWordsToolStripMenuItem";
            this.readWordsToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.readWordsToolStripMenuItem.Text = "&Read Words";
            this.readWordsToolStripMenuItem.Click += new System.EventHandler(this.ReadWordsToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.updateToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            // 
            // updateToolStripMenuItem
            // 
            this.updateToolStripMenuItem.Name = "updateToolStripMenuItem";
            this.updateToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.updateToolStripMenuItem.Text = "&Update";
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.richTextBoxEditor);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(0, 24);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(800, 404);
            this.panelLeft.TabIndex = 2;
            // 
            // richTextBoxEditor
            // 
            this.richTextBoxEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxEditor.Font = new System.Drawing.Font("Book Antiqua", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxEditor.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxEditor.Name = "richTextBoxEditor";
            this.richTextBoxEditor.Size = new System.Drawing.Size(800, 404);
            this.richTextBoxEditor.TabIndex = 0;
            this.richTextBoxEditor.Text = "";
            // 
            // panelRight
            // 
            this.panelRight.Controls.Add(this.tabControlRight);
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelRight.Location = new System.Drawing.Point(600, 24);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(200, 404);
            this.panelRight.TabIndex = 3;
            // 
            // tabControlRight
            // 
            this.tabControlRight.Controls.Add(this.Contexual);
            this.tabControlRight.Controls.Add(this.Console);
            this.tabControlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlRight.Location = new System.Drawing.Point(0, 0);
            this.tabControlRight.Name = "tabControlRight";
            this.tabControlRight.SelectedIndex = 0;
            this.tabControlRight.Size = new System.Drawing.Size(200, 404);
            this.tabControlRight.TabIndex = 0;
            // 
            // Contexual
            // 
            this.Contexual.Controls.Add(this.groupBoxSuggestion);
            this.Contexual.Controls.Add(this.groupBoxSelectedWord);
            this.Contexual.Location = new System.Drawing.Point(4, 22);
            this.Contexual.Name = "Contexual";
            this.Contexual.Padding = new System.Windows.Forms.Padding(3);
            this.Contexual.Size = new System.Drawing.Size(192, 378);
            this.Contexual.TabIndex = 0;
            this.Contexual.Text = "Contexual";
            this.Contexual.UseVisualStyleBackColor = true;
            // 
            // groupBoxSuggestion
            // 
            this.groupBoxSuggestion.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSuggestion.Controls.Add(this.listBoxSuggestion);
            this.groupBoxSuggestion.Location = new System.Drawing.Point(6, 118);
            this.groupBoxSuggestion.Name = "groupBoxSuggestion";
            this.groupBoxSuggestion.Size = new System.Drawing.Size(183, 254);
            this.groupBoxSuggestion.TabIndex = 1;
            this.groupBoxSuggestion.TabStop = false;
            this.groupBoxSuggestion.Text = "Suggested Words";
            // 
            // listBoxSuggestion
            // 
            this.listBoxSuggestion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxSuggestion.FormattingEnabled = true;
            this.listBoxSuggestion.Location = new System.Drawing.Point(3, 16);
            this.listBoxSuggestion.Name = "listBoxSuggestion";
            this.listBoxSuggestion.Size = new System.Drawing.Size(177, 235);
            this.listBoxSuggestion.TabIndex = 0;
            // 
            // groupBoxSelectedWord
            // 
            this.groupBoxSelectedWord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSelectedWord.Controls.Add(this.comboBoxTheWord);
            this.groupBoxSelectedWord.Controls.Add(this.textBoxTheWord);
            this.groupBoxSelectedWord.Location = new System.Drawing.Point(6, 6);
            this.groupBoxSelectedWord.Name = "groupBoxSelectedWord";
            this.groupBoxSelectedWord.Size = new System.Drawing.Size(183, 92);
            this.groupBoxSelectedWord.TabIndex = 0;
            this.groupBoxSelectedWord.TabStop = false;
            this.groupBoxSelectedWord.Text = "The Word";
            // 
            // comboBoxTheWord
            // 
            this.comboBoxTheWord.FormattingEnabled = true;
            this.comboBoxTheWord.Location = new System.Drawing.Point(6, 55);
            this.comboBoxTheWord.Name = "comboBoxTheWord";
            this.comboBoxTheWord.Size = new System.Drawing.Size(171, 21);
            this.comboBoxTheWord.TabIndex = 1;
            // 
            // textBoxTheWord
            // 
            this.textBoxTheWord.Location = new System.Drawing.Point(6, 19);
            this.textBoxTheWord.Name = "textBoxTheWord";
            this.textBoxTheWord.Size = new System.Drawing.Size(171, 20);
            this.textBoxTheWord.TabIndex = 0;
            // 
            // Console
            // 
            this.Console.Controls.Add(this.textBoxConsole);
            this.Console.Location = new System.Drawing.Point(4, 22);
            this.Console.Name = "Console";
            this.Console.Padding = new System.Windows.Forms.Padding(3);
            this.Console.Size = new System.Drawing.Size(192, 378);
            this.Console.TabIndex = 1;
            this.Console.Text = "Console";
            this.Console.UseVisualStyleBackColor = true;
            // 
            // textBoxConsole
            // 
            this.textBoxConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxConsole.Location = new System.Drawing.Point(3, 3);
            this.textBoxConsole.Multiline = true;
            this.textBoxConsole.Name = "textBoxConsole";
            this.textBoxConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxConsole.Size = new System.Drawing.Size(186, 372);
            this.textBoxConsole.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(597, 24);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 404);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // MainEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panelRight);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainEditor";
            this.Text = "Edit it....";
            this.Load += new System.EventHandler(this.MainEditor_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.tabControlRight.ResumeLayout(false);
            this.Contexual.ResumeLayout(false);
            this.groupBoxSuggestion.ResumeLayout(false);
            this.groupBoxSelectedWord.ResumeLayout(false);
            this.groupBoxSelectedWord.PerformLayout();
            this.Console.ResumeLayout(false);
            this.Console.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateToolStripMenuItem;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.RichTextBox richTextBoxEditor;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.TabControl tabControlRight;
        private System.Windows.Forms.TabPage Contexual;
        private System.Windows.Forms.TabPage Console;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.GroupBox groupBoxSuggestion;
        private System.Windows.Forms.ListBox listBoxSuggestion;
        private System.Windows.Forms.GroupBox groupBoxSelectedWord;
        private System.Windows.Forms.ComboBox comboBoxTheWord;
        private System.Windows.Forms.TextBox textBoxTheWord;
        private System.Windows.Forms.TextBox textBoxConsole;
        private System.Windows.Forms.ToolStripMenuItem readWordsToolStripMenuItem;
    }
}

