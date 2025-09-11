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
			var palabrasReservadas = new ToolStripMenuItem("Palabras reservadas", null, (s, e) => InsertText("// Palabras reservadas de KaizenLang:\noutput\ninput\nvoid\ndo\nwhile\nfor\nif\nelse\nreturn\ntrue\nfalse\nnull\n"));
			var sintaxisMenu = new ToolStripMenuItem("Sintaxis");
			sintaxisMenu.DropDownItems.Add(new ToolStripMenuItem("Control", null, (s, e) => InsertText("// Estructuras de control:\nif (condicion) {\n    // código si verdadero\n} else {\n    // código si falso\n}\n\nwhile (condicion) {\n    // código del bucle\n}\n\nfor (int i = 0; i < 10; i++) {\n    // código del bucle\n}\n")));
			sintaxisMenu.DropDownItems.Add(new ToolStripMenuItem("Funciones", null, (s, e) => InsertText("// Declaración de funciones:\nint suma(int a, int b) {\n    return a + b;\n}\n\nvoid saludar(string nombre) {\n    output(\"Hola \" + nombre);\n}\n")));
			sintaxisMenu.DropDownItems.Add(new ToolStripMenuItem("Operaciones", null, (s, e) => InsertText("// Operaciones aritméticas:\nint x = 10 + 5;\nint y = x * 2;\nint z = y / 3;\n\n// Operaciones lógicas:\nboolean resultado = (x > y) && (z < 10);\n")));
			var semanticaMenu = new ToolStripMenuItem("Semántica", null, (s, e) => InsertText("// Semántica de KaizenLang:\n// - Tipado estricto obligatorio\n// - Variables deben declararse antes de usarse\n// - No conversiones implícitas peligrosas\n// - Validación de compatibilidad de tipos\n\nint numero = 42;  // ✓ Correcto\n// numero = \"texto\";  // ❌ Error: tipos incompatibles\n"));
			var tiposDatosMenu = new ToolStripMenuItem("Tipos de datos", null, (s, e) => InsertText("// Tipos de datos simples:\nint entero = 42;\nfloat decimal = 3.14;\ndouble precision = 3.141592653589793;\nboolean logico = true;\nchar caracter = 'A';\nstring texto = \"Hola mundo\";\n\n// Tipos de datos compuestos:\narray numeros = [1, 2, 3, 4, 5];\nstring lista = \"elemento1,elemento2,elemento3\";\n"));
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
			if (string.IsNullOrWhiteSpace(source))
			{
				outputBox.Text = "❌ ERROR: No hay código para compilar";
				return;
			}

			var output = "🔧 PROCESO DE COMPILACIÓN INICIADO\r\n";
			output += "═════════════════════════════════════\r\n\r\n";

			try
			{
				// FASE 1: ANÁLISIS LÉXICO
				output += "📍 FASE 1: ANÁLISIS LÉXICO\r\n";
				output += "─────────────────────────────\r\n";
				var lexer = new Lexer();
				var tokens = lexer.Tokenize(source);
				
				// Verificar tokens inválidos
				var invalidTokens = tokens.Where(t => t.Type == "INVALID").ToList();
				if (invalidTokens.Any())
				{
					output += "❌ ERRORES LÉXICOS ENCONTRADOS:\r\n";
					foreach (var invalidToken in invalidTokens)
					{
						output += $"   • {invalidToken.Value}\r\n";
					}
					output += "\r\n❌ COMPILACIÓN DETENIDA\r\n";
					outputBox.Text = output;
					return;
				}

				output += "✅ Análisis léxico completado exitosamente\r\n";
				output += $"📊 Tokens encontrados: {tokens.Count}\r\n\r\n";

				// Mostrar algunos tokens importantes
				output += "🔍 TOKENS PRINCIPALES:\r\n";
				var importantTokens = tokens.Take(10).ToList();
				foreach (var token in importantTokens)
				{
					output += $"   {token.Type}: '{token.Value}'\r\n";
				}
				if (tokens.Count > 10)
					output += $"   ... y {tokens.Count - 10} tokens más\r\n";
				output += "\r\n";

				// FASE 2: ANÁLISIS SINTÁCTICO
				output += "📍 FASE 2: ANÁLISIS SINTÁCTICO\r\n";
				output += "──────────────────────────────\r\n";
				var parser = new Parser();
				var ast = parser.Parse(tokens);

				// Verificar errores sintácticos
				var syntaxErrors = ast.GetAllErrors();
				if (syntaxErrors.Any())
				{
					output += "❌ ERRORES SINTÁCTICOS ENCONTRADOS:\r\n";
					foreach (var error in syntaxErrors)
					{
						output += $"   • {error}\r\n";
					}
					output += "\r\n❌ COMPILACIÓN DETENIDA\r\n";
					outputBox.Text = output;
					return;
				}

				output += "✅ Análisis sintáctico completado exitosamente\r\n";
				output += "🌳 Árbol de Sintaxis Abstracta (AST) generado\r\n\r\n";

				// FASE 3: ANÁLISIS SEMÁNTICO
				output += "📍 FASE 3: ANÁLISIS SEMÁNTICO\r\n";
				output += "─────────────────────────────\r\n";
				var semanticAnalyzer = new SemanticAnalyzer();
				var semanticErrors = semanticAnalyzer.AnalyzeProgram(ast);

				if (semanticErrors.Any())
				{
					output += "❌ ERRORES SEMÁNTICOS ENCONTRADOS:\r\n";
					foreach (var error in semanticErrors)
					{
						output += $"   • {error}\r\n";
					}
					output += "\r\n❌ COMPILACIÓN DETENIDA\r\n";
					outputBox.Text = output;
					return;
				}

				output += "✅ Análisis semántico completado exitosamente\r\n";
				output += "✅ Todas las validaciones de tipos y scope pasaron\r\n\r\n";

				// MOSTRAR AST COMPACTO
				output += "🌳 ESTRUCTURA DEL AST:\r\n";
				output += "────────────────────────\r\n";
				output += ast.ToTreeString();
				output += "\r\n";

				// RESULTADO FINAL
				output += "🎉 COMPILACIÓN EXITOSA\r\n";
				output += "═══════════════════════\r\n";
				output += "✓ Análisis léxico: CORRECTO\r\n";
				output += "✓ Análisis sintáctico: CORRECTO\r\n";
				output += "✓ Análisis semántico: CORRECTO\r\n";
				output += "\r\n💡 El código está listo para ejecutarse\r\n";

				outputBox.Text = output;
			}
			catch (Exception ex)
			{
				output += $"\r\n💥 ERROR INTERNO DEL COMPILADOR:\r\n{ex.Message}\r\n";
				outputBox.Text = output;
			}
		}

		private void ExecuteButton_Click(object? sender, EventArgs? e)
		{
			string source = codeBox.Text;
			if (string.IsNullOrWhiteSpace(source))
			{
				outputBox.Text = "❌ ERROR: No hay código para ejecutar";
				return;
			}

			var output = "🚀 INICIANDO EJECUCIÓN\r\n";
			output += "═════════════════════\r\n\r\n";

			try
			{
				// Primero compilar
				var lexer = new Lexer();
				var tokens = lexer.Tokenize(source);
				var parser = new Parser();
				var ast = parser.Parse(tokens);
				var semanticAnalyzer = new SemanticAnalyzer();
				var semanticErrors = semanticAnalyzer.AnalyzeProgram(ast);

				// Verificar que no hay errores antes de ejecutar
				var lexicalErrors = tokens.Where(t => t.Type == "INVALID").ToList();
				var syntaxErrors = ast.GetAllErrors();

				if (lexicalErrors.Any() || syntaxErrors.Any() || semanticErrors.Any())
				{
					output += "❌ EJECUCIÓN DETENIDA\r\n";
					output += "El código contiene errores. Use 'Compilar' para ver los detalles.\r\n\r\n";
					
					if (lexicalErrors.Any())
						output += $"• {lexicalErrors.Count} errores léxicos\r\n";
					if (syntaxErrors.Any())
						output += $"• {syntaxErrors.Count} errores sintácticos\r\n";
					if (semanticErrors.Any())
						output += $"• {semanticErrors.Count} errores semánticos\r\n";
					
					outputBox.Text = output;
					return;
				}

				// Ejecutar código
				output += "💻 EJECUTANDO CÓDIGO...\r\n";
				output += "─────────────────────────\r\n";
				
				var interpreter = new Interpreter();
				var executionOutput = interpreter.Execute(ast);

				if (executionOutput.Any())
				{
					output += "📤 SALIDA DEL PROGRAMA:\r\n";
					foreach (var line in executionOutput)
					{
						output += $"   {line}\r\n";
					}
				}
				else
				{
					output += "✅ El programa se ejecutó sin salida\r\n";
				}

				output += "\r\n🎯 EJECUCIÓN COMPLETADA EXITOSAMENTE\r\n";
				outputBox.Text = output;
			}
			catch (Exception ex)
			{
				output += $"\r\n💥 ERROR DE EJECUCIÓN:\r\n{ex.Message}\r\n";
				outputBox.Text = output;
			}
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
