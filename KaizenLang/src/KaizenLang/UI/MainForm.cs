using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaizenLang.UI
{
    public partial class MainForm : Form
    {
        private readonly CompilationService compilationService;
        private readonly ExecutionService executionService;

        public MainForm()
        {
            InitializeComponent();
            
            compilationService = new CompilationService();
            executionService = new ExecutionService();
            executionService.InputProvider = prompt => Prompt.Show("Input", prompt);
            
            InitializeCustomComponents();
            ApplyTheme();
        }

        private void InitializeCustomComponents()
        {
            // Event handlers
            compileButton.Click += CompileButton_Click;
            executeButton.Click += ExecuteButton_Click;

            // Menu item handlers
            reservedWordsToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.ReservedWords);
            ifToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.IfStatement);
            whileToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.WhileLoop);
            forToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.ForLoop);
            functionsToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.FunctionDeclaration);
            dataTypesToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.DataTypes);
            operationsToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.Operations);
            semanticsToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.Semantics);
            compileButton.MouseEnter += (s, e) => compileButton.BackColor = KaizenTheme.GreenDark;
            compileButton.MouseLeave += (s, e) => compileButton.BackColor = KaizenTheme.Green;

            executeButton.MouseEnter += (s, e) => executeButton.BackColor = KaizenTheme.GoldDark;
            executeButton.MouseLeave += (s, e) => executeButton.BackColor = KaizenTheme.Gold;
        }

        private void ApplyTheme()
        {
            this.BackColor = KaizenTheme.Background;
            this.ForeColor = KaizenTheme.Text;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(800, 600);


            // Top Bar
            topBarPanel.BackColor = KaizenTheme.Surface;
            topBarPanel.Height = 50;

            // Menu
            menuStrip.BackColor = KaizenTheme.Surface;
            menuStrip.ForeColor = KaizenTheme.Muted;
            menuStrip.Renderer = new ToolStripProfessionalRenderer(new KaizenColorTable());
            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                item.ForeColor = KaizenTheme.Text; // Ensure top-level items are also styled
                item.DropDown.Opening += (s, e) => SetDropDownItemColors(item);
            }

            // Code Editor
            codeRichTextBox.BackColor = KaizenTheme.Surface3;
            codeRichTextBox.ForeColor = KaizenTheme.Text;
            codeRichTextBox.Font = new Font("Consolas", 11);
            codeRichTextBox.BorderStyle = BorderStyle.None;
            codeRichTextBox.Dock = DockStyle.Fill;

            // Output
            outputRichTextBox.BackColor = KaizenTheme.Surface3;
            outputRichTextBox.ForeColor = KaizenTheme.Muted;
            outputRichTextBox.Font = new Font("Consolas", 10);
            outputRichTextBox.ReadOnly = true;
            outputRichTextBox.BorderStyle = BorderStyle.None;
            outputRichTextBox.Dock = DockStyle.Fill;

            // Buttons
            compileButton.BackColor = KaizenTheme.Green;
            compileButton.ForeColor = Color.White;
            compileButton.FlatStyle = FlatStyle.Flat;
            compileButton.FlatAppearance.BorderSize = 0;
            compileButton.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            executeButton.BackColor = KaizenTheme.Gold;
            executeButton.ForeColor = Color.Black;
            executeButton.FlatStyle = FlatStyle.Flat;
            executeButton.FlatAppearance.BorderSize = 0;
            executeButton.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Sidebar
            sidebarPanel.BackColor = KaizenTheme.Surface;

            // Splitters
            mainSplitContainer.BackColor = KaizenTheme.Border;
            mainSplitContainer.SplitterWidth = 2;
            editorOutputSplitContainer.BackColor = KaizenTheme.Border;
            editorOutputSplitContainer.SplitterWidth = 2;
        }

        private void SetDropDownItemColors(ToolStripMenuItem menuItem)
        {
            foreach (ToolStripItem item in menuItem.DropDownItems)
            {
                if (item is ToolStripMenuItem subItem)
                {
                    subItem.BackColor = KaizenTheme.Surface;
                    subItem.ForeColor = KaizenTheme.Text;
                    SetDropDownItemColors(subItem); // Recursive call for nested submenus
                }
            }
        }

        private void InsertCodeSnippet(string snippet)
        {
            codeRichTextBox.Clear();
            codeRichTextBox.Text = snippet;
        }

        private async void CompileButton_Click(object? sender, EventArgs e)
        {
            compileButton.Enabled = false;
            compileButton.Text = "Compiling...";
            
            var result = await Task.Run(() => compilationService.CompileCode(codeRichTextBox.Text));
            
            outputRichTextBox.Text = result.Output;
            
            compileButton.Enabled = true;
            compileButton.Text = "Compile";
        }

        private async void ExecuteButton_Click(object? sender, EventArgs e)
        {
            executeButton.Enabled = false;
            executeButton.Text = "Executing...";

            var result = await Task.Run(() => executionService.ExecuteCode(codeRichTextBox.Text));

            outputRichTextBox.Text = result.Output;

            executeButton.Enabled = true;
            executeButton.Text = "Execute";
        }
    }

    // Custom ColorTable for MenuStrip to override default colors
    public class KaizenColorTable : ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin => KaizenTheme.Surface;
        public override Color MenuStripGradientEnd => KaizenTheme.Surface;
        public override Color MenuItemSelectedGradientBegin => KaizenTheme.Surface2;
        public override Color MenuItemSelectedGradientEnd => KaizenTheme.Surface2;
        public override Color MenuItemPressedGradientBegin => KaizenTheme.GreenDark;
        public override Color MenuItemPressedGradientEnd => KaizenTheme.GreenDark;
        public override Color MenuItemBorder => KaizenTheme.Border;
        public override Color ImageMarginGradientBegin => KaizenTheme.Surface;
        public override Color ImageMarginGradientEnd => KaizenTheme.Surface;
        public override Color ToolStripDropDownBackground => KaizenTheme.Surface;
        public override Color MenuItemSelected => KaizenTheme.Surface2;
    }
}
