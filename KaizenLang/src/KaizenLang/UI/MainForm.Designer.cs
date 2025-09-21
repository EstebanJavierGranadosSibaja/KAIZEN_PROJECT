namespace KaizenLang.UI
{
    partial class MainForm
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
            this.topBarPanel = new System.Windows.Forms.Panel();
            this.executeButton = new System.Windows.Forms.Button();
            this.compileButton = new System.Windows.Forms.Button();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.languageStructuresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reservedWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.syntaxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ifToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.whileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.functionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.operationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.semanticsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataTypesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.editorOutputSplitContainer = new System.Windows.Forms.SplitContainer();
            this.codeRichTextBox = new System.Windows.Forms.RichTextBox();
            this.outputRichTextBox = new System.Windows.Forms.RichTextBox();
            this.sidebarPanel = new System.Windows.Forms.Panel();
            this.topBarPanel.SuspendLayout();
            this.menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.editorOutputSplitContainer)).BeginInit();
            this.editorOutputSplitContainer.Panel1.SuspendLayout();
            this.editorOutputSplitContainer.Panel2.SuspendLayout();
            this.editorOutputSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // topBarPanel
            // 
            this.topBarPanel.Controls.Add(this.executeButton);
            this.topBarPanel.Controls.Add(this.compileButton);
            this.topBarPanel.Controls.Add(this.menuStrip);
            this.topBarPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topBarPanel.Location = new System.Drawing.Point(10, 10);
            this.topBarPanel.Name = "topBarPanel";
            this.topBarPanel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.topBarPanel.Size = new System.Drawing.Size(1244, 50);
            this.topBarPanel.TabIndex = 0;
            // 
            // executeButton
            // 
            this.executeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.executeButton.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.executeButton.Location = new System.Drawing.Point(1118, 8);
            this.executeButton.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(116, 35);
            this.executeButton.TabIndex = 2;
            this.executeButton.Text = "Execute";
            this.executeButton.UseVisualStyleBackColor = true;
            // 
            // compileButton
            // 
            this.compileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.compileButton.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.compileButton.Location = new System.Drawing.Point(996, 8);
            this.compileButton.Name = "compileButton";
            this.compileButton.Size = new System.Drawing.Size(116, 35);
            this.compileButton.TabIndex = 1;
            this.compileButton.Text = "Compile";
            this.compileButton.UseVisualStyleBackColor = true;
            // 
            // menuStrip
            // 
            this.menuStrip.Dock = System.Windows.Forms.DockStyle.Left;
            this.menuStrip.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.languageStructuresToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(10, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(220, 50);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // languageStructuresToolStripMenuItem
            // 
            this.languageStructuresToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.languageStructuresToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reservedWordsToolStripMenuItem,
            this.syntaxToolStripMenuItem,
            this.semanticsToolStripMenuItem,
            this.dataTypesToolStripMenuItem});
            this.languageStructuresToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.languageStructuresToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.languageStructuresToolStripMenuItem.Name = "languageStructuresToolStripMenuItem";
            this.languageStructuresToolStripMenuItem.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.languageStructuresToolStripMenuItem.Size = new System.Drawing.Size(209, 46);
            this.languageStructuresToolStripMenuItem.Text = "Estructuras del lenguaje";
            // 
            // reservedWordsToolStripMenuItem
            // 
            this.reservedWordsToolStripMenuItem.Name = "reservedWordsToolStripMenuItem";
            this.reservedWordsToolStripMenuItem.Size = new System.Drawing.Size(243, 26);
            this.reservedWordsToolStripMenuItem.Text = "Palabras reservadas";
            // 
            // syntaxToolStripMenuItem
            // 
            this.syntaxToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlToolStripMenuItem,
            this.functionsToolStripMenuItem,
            this.operationsToolStripMenuItem});
            this.syntaxToolStripMenuItem.Name = "syntaxToolStripMenuItem";
            this.syntaxToolStripMenuItem.Size = new System.Drawing.Size(243, 26);
            this.syntaxToolStripMenuItem.Text = "Sintaxis";
            // 
            // controlToolStripMenuItem
            // 
            this.controlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ifToolStripMenuItem,
            this.whileToolStripMenuItem,
            this.forToolStripMenuItem});
            this.controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            this.controlToolStripMenuItem.Size = new System.Drawing.Size(169, 26);
            this.controlToolStripMenuItem.Text = "Control";
            // 
            // ifToolStripMenuItem
            // 
            this.ifToolStripMenuItem.Name = "ifToolStripMenuItem";
            this.ifToolStripMenuItem.Size = new System.Drawing.Size(121, 26);
            this.ifToolStripMenuItem.Text = "If";
            // 
            // whileToolStripMenuItem
            // 
            this.whileToolStripMenuItem.Name = "whileToolStripMenuItem";
            this.whileToolStripMenuItem.Size = new System.Drawing.Size(121, 26);
            this.whileToolStripMenuItem.Text = "While";
            // 
            // forToolStripMenuItem
            // 
            this.forToolStripMenuItem.Name = "forToolStripMenuItem";
            this.forToolStripMenuItem.Size = new System.Drawing.Size(121, 26);
            this.forToolStripMenuItem.Text = "For";
            // 
            // functionsToolStripMenuItem
            // 
            this.functionsToolStripMenuItem.Name = "functionsToolStripMenuItem";
            this.functionsToolStripMenuItem.Size = new System.Drawing.Size(169, 26);
            this.functionsToolStripMenuItem.Text = "Funciones";
            // 
            // operationsToolStripMenuItem
            // 
            this.operationsToolStripMenuItem.Name = "operationsToolStripMenuItem";
            this.operationsToolStripMenuItem.Size = new System.Drawing.Size(169, 26);
            this.operationsToolStripMenuItem.Text = "Operaciones";
            // 
            // semanticsToolStripMenuItem
            // 
            this.semanticsToolStripMenuItem.Name = "semanticsToolStripMenuItem";
            this.semanticsToolStripMenuItem.Size = new System.Drawing.Size(243, 26);
            this.semanticsToolStripMenuItem.Text = "Semántica";
            // 
            // dataTypesToolStripMenuItem
            // 
            this.dataTypesToolStripMenuItem.Name = "dataTypesToolStripMenuItem";
            this.dataTypesToolStripMenuItem.Size = new System.Drawing.Size(243, 26);
            this.dataTypesToolStripMenuItem.Text = "Tipos de datos";
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(10, 60);
            this.mainSplitContainer.Name = "mainSplitContainer";
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.editorOutputSplitContainer);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.sidebarPanel);
            this.mainSplitContainer.Size = new System.Drawing.Size(1244, 691);
            this.mainSplitContainer.SplitterDistance = 960;
            this.mainSplitContainer.SplitterWidth = 8;
            this.mainSplitContainer.TabIndex = 1;
            // 
            // editorOutputSplitContainer
            // 
            this.editorOutputSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorOutputSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.editorOutputSplitContainer.Name = "editorOutputSplitContainer";
            this.editorOutputSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // editorOutputSplitContainer.Panel1
            // 
            this.editorOutputSplitContainer.Panel1.Controls.Add(this.codeRichTextBox);
            this.editorOutputSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(10, 10, 10, 5);
            // 
            // editorOutputSplitContainer.Panel2
            // 
            this.editorOutputSplitContainer.Panel2.Controls.Add(this.outputRichTextBox);
            this.editorOutputSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(10, 5, 10, 10);
            this.editorOutputSplitContainer.Size = new System.Drawing.Size(960, 691);
            this.editorOutputSplitContainer.SplitterDistance = 480;
            this.editorOutputSplitContainer.SplitterWidth = 8;
            this.editorOutputSplitContainer.TabIndex = 0;
            // 
            // codeRichTextBox
            // 
            this.codeRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.codeRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeRichTextBox.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.codeRichTextBox.Location = new System.Drawing.Point(10, 10);
            this.codeRichTextBox.Name = "codeRichTextBox";
            this.codeRichTextBox.Size = new System.Drawing.Size(940, 465);
            this.codeRichTextBox.TabIndex = 0;
            this.codeRichTextBox.Text = "";
            // 
            // outputRichTextBox
            // 
            this.outputRichTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.outputRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputRichTextBox.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.outputRichTextBox.Location = new System.Drawing.Point(10, 5);
            this.outputRichTextBox.Name = "outputRichTextBox";
            this.outputRichTextBox.Size = new System.Drawing.Size(940, 183);
            this.outputRichTextBox.TabIndex = 0;
            this.outputRichTextBox.Text = "";
            // 
            // sidebarPanel
            // 
            this.sidebarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sidebarPanel.Location = new System.Drawing.Point(0, 0);
            this.sidebarPanel.Name = "sidebarPanel";
            this.sidebarPanel.Padding = new System.Windows.Forms.Padding(10);
            this.sidebarPanel.Size = new System.Drawing.Size(276, 691);
            this.sidebarPanel.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 761);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.topBarPanel);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "KaizenLang IDE";
            this.topBarPanel.ResumeLayout(false);
            this.topBarPanel.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            this.editorOutputSplitContainer.Panel1.ResumeLayout(false);
            this.editorOutputSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.editorOutputSplitContainer)).EndInit();
            this.editorOutputSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel topBarPanel;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button compileButton;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.SplitContainer editorOutputSplitContainer;
        private System.Windows.Forms.RichTextBox codeRichTextBox;
        private System.Windows.Forms.RichTextBox outputRichTextBox;
        private System.Windows.Forms.Panel sidebarPanel;
        private System.Windows.Forms.ToolStripMenuItem languageStructuresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reservedWordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem syntaxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ifToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem whileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem functionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem operationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem semanticsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataTypesToolStripMenuItem;
    }
}
