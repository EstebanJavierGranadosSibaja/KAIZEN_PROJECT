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
			this.Width = 1400;
			this.Height = 900;
			this.BackColor = Color.FromArgb(240, 243, 250);
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;

			// Menú principal moderno y destacado
			menuStrip = new MenuStrip
			{
				BackColor = Color.FromArgb(255, 255, 255),
				Font = new Font("Segoe UI", 13, FontStyle.Bold),
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
			menuStrip.Height = 40;
			this.Controls.Add(menuStrip);

			// Panel de código con sombra y bordes redondeados simulados
			var codePanel = new Panel
			{
				BackColor = Color.White,
				BorderStyle = BorderStyle.FixedSingle,
				Width = 1200,
				Height = 400,
				Top = 60,
				Left = 80,
				Padding = new Padding(20)
			};
			codePanel.Paint += (s, e) => {
				var g = e.Graphics;
				var rect = new Rectangle(0, 0, codePanel.Width - 1, codePanel.Height - 1);
				using (var pen = new Pen(Color.FromArgb(200, 200, 220), 2))
					g.DrawRectangle(pen, rect);
				// Sombra inferior
				using (var brush = new SolidBrush(Color.FromArgb(30, 44, 62, 80)))
					g.FillRectangle(brush, 0, codePanel.Height - 10, codePanel.Width, 10);
			};
			codeBox = new TextBox
			{
				Multiline = true,
				ScrollBars = ScrollBars.Vertical,
				Width = codePanel.Width - 40,
				Height = codePanel.Height - 40,
				Top = 10,
				Left = 10,
				Font = new Font("Fira Code", 16),
				BackColor = Color.FromArgb(250, 250, 255),
				ForeColor = Color.FromArgb(44, 62, 80),
				BorderStyle = BorderStyle.None
			};
			codePanel.Controls.Add(codeBox);
			this.Controls.Add(codePanel);

			// Botones con efecto hover y aspecto profesional
			compileButton = new Button
			{
				Text = "🛠 Compilar",
				Top = codePanel.Bottom + 30,
				Left = codePanel.Left,
				Width = 180,
				Height = 50,
				BackColor = Color.FromArgb(52, 152, 219),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 14, FontStyle.Bold)
			};
			compileButton.FlatAppearance.BorderSize = 0;
			compileButton.MouseEnter += (s, e) => compileButton.BackColor = Color.FromArgb(25, 90, 160); // Azul más oscuro y saturado
			compileButton.MouseLeave += (s, e) => compileButton.BackColor = Color.FromArgb(52, 152, 219); // Azul original
			compileButton.Click += CompileButton_Click;
			this.Controls.Add(compileButton);

			executeButton = new Button
			{
				Text = "▶ Ejecutar",
				Top = codePanel.Bottom + 30,
				Left = compileButton.Right + 40,
				Width = 180,
				Height = 50,
				BackColor = Color.FromArgb(39, 174, 96),
				ForeColor = Color.White,
				FlatStyle = FlatStyle.Flat,
				Font = new Font("Segoe UI", 14, FontStyle.Bold)
			};
			executeButton.FlatAppearance.BorderSize = 0;
			executeButton.MouseEnter += (s, e) => executeButton.BackColor = Color.FromArgb(30, 132, 73);
			executeButton.MouseLeave += (s, e) => executeButton.BackColor = Color.FromArgb(39, 174, 96);
			executeButton.Click += ExecuteButton_Click;
			this.Controls.Add(executeButton);

			// Output con fondo oscuro, sombra y bordes redondeados simulados
			var outputPanel = new Panel
			{
				BackColor = Color.FromArgb(44, 62, 80),
				BorderStyle = BorderStyle.FixedSingle,
				Width = 1200,
				Height = 250,
				Top = compileButton.Bottom + 40,
				Left = 80,
				Padding = new Padding(20)
			};
			outputPanel.Paint += (s, e) => {
				var g = e.Graphics;
				var rect = new Rectangle(0, 0, outputPanel.Width - 1, outputPanel.Height - 1);
				using (var pen = new Pen(Color.FromArgb(200, 200, 220), 2))
					g.DrawRectangle(pen, rect);
				// Sombra inferior
				using (var brush = new SolidBrush(Color.FromArgb(60, 44, 62, 80)))
					g.FillRectangle(brush, 0, outputPanel.Height - 10, outputPanel.Width, 10);
			};
			outputBox = new TextBox
			{
				Multiline = true,
				ScrollBars = ScrollBars.Vertical,
				Width = outputPanel.Width - 40,
				Height = outputPanel.Height - 40,
				Top = 10,
				Left = 10,
				ReadOnly = true,
				Font = new Font("Fira Code", 14),
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
