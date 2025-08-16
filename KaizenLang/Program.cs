using System;
using System.IO;
using System.Collections.Generic;

namespace ParadigmasLang
{
    public class Program : Form
    {
        private TextBox codeBox;
        private TextBox outputBox;
        private Button compileButton;
        private Button executeButton;
        private MenuStrip menuStrip;

        public Program()
        {
            this.Text = "KaizenLang IDE";
            this.Width = 900;
            this.Height = 650;

            // Menú principal
            menuStrip = new MenuStrip();
            var estructurasMenu = new ToolStripMenuItem("Estructuras del Lenguaje");
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

            // Área de código
            codeBox = new TextBox {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 850,
                Height = 300,
                Top = 40,
                Left = 20,
                Font = new System.Drawing.Font("Consolas", 12)
            };
            this.Controls.Add(codeBox);

            // Botón Compilar
            compileButton = new Button {
                Text = "Compilar",
                Top = 350,
                Left = 20,
                Width = 100
            };
            compileButton.Click += CompileButton_Click;
            this.Controls.Add(compileButton);

            // Botón Ejecutar
            executeButton = new Button {
                Text = "Ejecutar",
                Top = 350,
                Left = 140,
                Width = 100
            };
            executeButton.Click += ExecuteButton_Click;
            this.Controls.Add(executeButton);

            // Output para errores y resultados
            outputBox = new TextBox {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = 850,
                Height = 200,
                Top = 400,
                Left = 20,
                ReadOnly = true,
                Font = new System.Drawing.Font("Consolas", 10)
            };
            this.Controls.Add(outputBox);
        }

        private void InsertText(string text)
        {
            codeBox.SelectedText = text;
        }

        private void CompileButton_Click(object sender, EventArgs e)
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

        private void ExecuteButton_Click(object sender, EventArgs e)
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

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Program());
        }
    }
}
