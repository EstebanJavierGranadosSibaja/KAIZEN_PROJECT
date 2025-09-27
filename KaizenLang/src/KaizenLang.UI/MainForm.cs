using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using KaizenLang.UI.Theme;

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
            this.ApplyCurrentThemeRecursive();
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
}
