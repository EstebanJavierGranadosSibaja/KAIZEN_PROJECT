using System.Windows.Forms;
using System.Drawing;
using ParadigmasLang;

namespace KaizenLang.UI
{
	public class MainForm : Form
	{
		private TextBox codeBox;
		private TextBox outputBox;
		private Button compileButton;
		private Button executeButton;
		private MenuStrip menuStrip;

		public MainForm()
		{
			this.Text = "KaizenLang IDE";
			this.Width = 1000;
			this.Height = 700;
			this.BackColor = Color.FromArgb(245, 247, 250);
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;

			// Menú principal moderno
			menuStrip = new MenuStrip
			{
				BackColor = Color.White,
				Font = new Font("Segoe UI", 11, FontStyle.Bold),
				Renderer = new ToolStripProfessionalRenderer()
			};
			var estructurasMenu = new ToolStripMenuItem("☰ Estructuras del Lenguaje") { ForeColor = Color.FromArgb(44, 62, 80) };
			var palabrasReservadas = new ToolStripMenuItem("Palabras reservadas", null, (s, e) => InsertText("output\ninput\nvoid\ndo\nwhile\nfor\nif\nelse\nreturn\ntrue\nfalse\n"));
			var sintaxisMenu = new ToolStripMenuItem("Sintaxis");
			sintaxisMenu.DropDownItems.Add(new ToolStripMenuItem("Control", null, (s, e) => InsertText("if (condicion) {\n    // ...\n}\n")));
			sintaxisMenu.DropDownItems.Add(new ToolStripMenuItem("Funciones", null, (s, e) => InsertText("int suma(int a, int b) {\n    return a + b;\n}\n")));
			sintaxisMenu.DropDownItems.Add(new ToolStripMenuItem("Operaciones", null, (s, e) => InsertText("x = a + b;\n")));
			var semanticaMenu = new ToolStripMenuItem("Semántica", null, (s, e) => InsertText("// Semántica: significado de las instrucciones\n"));
			var tiposDatosMenu = new ToolStripMenuItem("Tipos de datos", null, (s, e) => InsertText("int, float, double, boolean, char, string, array\n"));
			estructurasMenu.DropDownItems.Add(palabrasReservadas);
			estructurasMenu.DropDownItems.Add(sintaxisMenu);
			estructurasMenu.DropDownItems.Add(semanticaMenu);
			estructurasMenu.DropDownItems.Add(tiposDatosMenu);
			menuStrip.Items.Add(estructurasMenu);
			this.Controls.Add(menuStrip);

			// Área de código con panel y borde
			var codePanel = new Panel
			{
				BackColor = Color.White,
				BorderStyle = BorderStyle.FixedSingle,
				Width = 920,
				Height = 320,
				Top = 50,
				Left = 40
			};
			codeBox = new TextBox
			{
				Multiline = true,
				ScrollBars = ScrollBars.Vertical,
				Width = codePanel.Width - 20,
				Height = codePanel.Height - 20,
				Top = 10,
				Left = 10,
				Font = new Font("Fira Code", 13),
				BackColor = Color.FromArgb(250, 250, 250),
				ForeColor = Color.FromArgb(44, 62, 80),
				BorderStyle = BorderStyle.None
			};
			codePanel.Controls.Add(codeBox);
			this.Controls.Add(codePanel);

			// Botones modernos
			compileButton = new Button
			{
				Text = "🛠 Compilar",
				Top = codePanel.Bottom + 20,
				Left = codePanel.Left,
				Width = 140,
				Height = 40,
				BackColor = Color.FromArgb(52, 152, 219),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 12, FontStyle.Bold)
			};
			compileButton.FlatAppearance.BorderSize = 0;
			compileButton.Click += CompileButton_Click;
			this.Controls.Add(compileButton);

			executeButton = new Button
			{
				Text = "▶ Ejecutar",
				Top = codePanel.Bottom + 20,
				Left = compileButton.Right + 20,
				Width = 140,
				Height = 40,
				BackColor = Color.FromArgb(39, 174, 96),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 12, FontStyle.Bold)
			};
			executeButton.FlatAppearance.BorderSize = 0;
			executeButton.Click += ExecuteButton_Click;
			this.Controls.Add(executeButton);

			// Output con fondo oscuro y borde
			var outputPanel = new Panel
			{
				BackColor = Color.FromArgb(44, 62, 80),
				BorderStyle = BorderStyle.FixedSingle,
				Width = 920,
				Height = 200,
				Top = compileButton.Bottom + 30,
				Left = 40
			};
			outputBox = new TextBox
			{
				Multiline = true,
				ScrollBars = ScrollBars.Vertical,
				Width = outputPanel.Width - 20,
				Height = outputPanel.Height - 20,
				Top = 10,
				Left = 10,
				ReadOnly = true,
				Font = new Font("Fira Code", 11),
				BackColor = Color.FromArgb(44, 62, 80),
				ForeColor = Color.White,
				BorderStyle = BorderStyle.None
			};
			outputPanel.Controls.Add(outputBox);
			this.Controls.Add(outputPanel);
		}

		private void InsertText(string text)
		{
			codeBox.SelectedText = text;
		}

		private void CompileButton_Click(object? sender, EventArgs? e)
		{
			string source = codeBox.Text;
			var lexer = new Lexer();
			var tokens = lexer.Tokenize(source);
			var parser = new Parser();
			var root = parser.Parse(tokens);

			// Mostrar tokens
			var output = "TOKENS:\r\n";
			foreach (var token in tokens)
				output += $"{token.Type}: {token.Value}\r\n";

			// Mostrar árbol de sintaxis
			output += "\r\nÁRBOL DE SINTAXIS:\r\n";
			output += PrintNode(root, 0);

			outputBox.Text = output;
		}

		private void ExecuteButton_Click(object? sender, EventArgs? e)
		{
			// Simulación de ejecución: solo muestra mensaje
			outputBox.Text = "Ejecución simulada: (Aquí se mostraría la salida del programa)";
		}

		private string PrintNode(Node node, int indent)
		{
			string result = new string(' ', indent * 2) + node.Type + "\r\n";
			foreach (var child in node.Children)
				result += PrintNode(child, indent + 1);
			return result;
		}
	}
}
