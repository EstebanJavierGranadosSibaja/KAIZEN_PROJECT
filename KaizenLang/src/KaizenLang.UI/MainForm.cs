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
        private ToolStripStatusLabel? statusLabel;
        public MainForm()
        {
            InitializeComponent();

            compilationService = new CompilationService();
            executionService = new ExecutionService();
            executionService.InputProvider = prompt => Prompt.Show("Entrada requerida", prompt);

            InitializeCustomComponents();

            // Configurar ícono de la aplicación (si existe en carpeta Resources)
            try
            {
                var iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "icon.ico");
                if (System.IO.File.Exists(iconPath))
                {
                    this.Icon = new Icon(iconPath);
                }
            }
            catch
            {
                // Ignorar fallo al cargar ícono
            }

            this.ApplyCurrentThemeRecursive();
        }

        private void InitializeCustomComponents()
        {
            // Configurar iconos del menú
            SetupMenuIcons();

            // Eventos de botones
            compileButton.Click += async (s, e) => await RunWithUIFeedback(
                compileButton, "Compilando...", "Compilar", async () =>
                {
                    var result = await Task.Run(() => compilationService.CompileCode(codeRichTextBox.Text));
                    DisplayResult(result.Output, result.IsSuccessful);
                    UpdateStatus(result.IsSuccessful ? "Compilación exitosa" : "Compilación fallida", result.IsSuccessful);
                });

            executeButton.Click += async (s, e) => await RunWithUIFeedback(
                executeButton, "Ejecutando...", "Ejecutar", async () =>
                {
                    var result = await Task.Run(() => executionService.ExecuteCode(codeRichTextBox.Text));
                    DisplayResult(result.Output, result.IsSuccessful);
                    UpdateStatus(result.IsSuccessful ? "Ejecución completada" : "Error en ejecución", result.IsSuccessful);
                });

            // Items de menú -> snippets de código
            reservedWordsToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.ReservedWords);
            ifToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.IfStatement);
            whileToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.WhileLoop);
            forToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.ForLoop);
            functionsToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.FunctionDeclaration);
            dataTypesToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.DataTypes);
            operationsToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.Operations);
            semanticsToolStripMenuItem.Click += (s, e) => InsertCodeSnippet(CodeSnippets.Semantics);

            // Barra de estado
            var statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Listo")
            {
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
        }

        private void InsertCodeSnippet(string snippet)
        {
            codeRichTextBox.Clear();
            codeRichTextBox.Text = snippet;
        }

        private void SetupMenuIcons()
        {
            // Configurar iconos de los elementos principales del menú
            languageStructuresToolStripMenuItem.Image = IconFactory.GetIcon("examples", 16, 16);
            reservedWordsToolStripMenuItem.Image = IconFactory.GetIcon("keywords", 16, 16);
            syntaxToolStripMenuItem.Image = IconFactory.GetIcon("syntax", 16, 16);
            semanticsToolStripMenuItem.Image = IconFactory.GetIcon("semantics", 16, 16);
            dataTypesToolStripMenuItem.Image = IconFactory.GetIcon("datatypes", 16, 16);

            // Configurar iconos de submenús
            controlToolStripMenuItem.Image = IconFactory.GetIcon("control", 16, 16);
            functionsToolStripMenuItem.Image = IconFactory.GetIcon("functions", 16, 16);
            operationsToolStripMenuItem.Image = IconFactory.GetIcon("operations", 16, 16);

            // Configurar iconos de los botones principales
            SetupButtonIcons();
        }

        private void SetupButtonIcons()
        {
            // Asignar iconos a los botones principales
            var compileIcon = IconFactory.GetIcon("compile", 16, 16);
            var executeIcon = IconFactory.GetIcon("execute", 16, 16);

            if (compileIcon != null)
            {
                compileButton.Image = compileIcon;
                compileButton.ImageAlign = ContentAlignment.MiddleLeft;
                compileButton.TextAlign = ContentAlignment.MiddleRight;
                compileButton.Text = "  Compilar";
            }

            if (executeIcon != null)
            {
                executeButton.Image = executeIcon;
                executeButton.ImageAlign = ContentAlignment.MiddleLeft;
                executeButton.TextAlign = ContentAlignment.MiddleRight;
                executeButton.Text = "  Ejecutar";
            }
        }        private void UpdateStatus(string message, bool success)
        {
            if (statusLabel == null) return;

            statusLabel.Text = message;
        }

        /// <summary>
        /// Centraliza la lógica de feedback visual: deshabilita botón, cambia texto, ejecuta acción, restaura estado.
        /// </summary>
        private async Task RunWithUIFeedback(Button button, string workingText, string defaultText, Func<Task> action)
        {
            try
            {
                button.Enabled = false;
                button.Text = workingText;
                UpdateStatus("Procesando...", true);

                await action();
            }
            catch (Exception ex)
            {
                DisplayResult($"ERROR inesperado: {ex.Message}", false);
                UpdateStatus("Error inesperado", false);
            }
            finally
            {
                button.Enabled = true;
                button.Text = defaultText;
            }
        }

        /// <summary>
        /// Muestra resultados en el RichTextBox usando el color de primer plano del tema actual.
        /// </summary>
        private void DisplayResult(string output, bool success)
        {
            outputRichTextBox.Clear();
            outputRichTextBox.SelectionColor = ThemeManager.CurrentTheme.Foreground;
            outputRichTextBox.AppendText(output);
            outputRichTextBox.SelectionColor = outputRichTextBox.ForeColor; // reset color
        }
    }
}
