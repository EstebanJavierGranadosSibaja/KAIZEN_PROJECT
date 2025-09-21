namespace KaizenLang.UI.Components
{
    /// <summary>
    /// Ventana Emgerge que carga los archivos de ayuda "Resource/HelpFiles".
    /// La primera linea del archivo es el titulo del boton, al seleccionarlo muestra el contenido.
    /// </summary>
    class HelpWindow : ModernWindowBase
    {
        private FlowLayoutPanel buttonPanel;
        private RichTextBox helpTextBox;
        private const int MarginSize = 10;

        public HelpWindow()
        {
            this.Text = "Ayuda";
            this.Size = new Size(800, 600);
            this.Padding = new Padding(MarginSize);

            buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 200,
                AutoScroll = true,
                Margin = new Padding(0, 0, MarginSize, 0),
                Padding = new Padding(0, MarginSize, 0, MarginSize)
            };

            var borderPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            helpTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0)
            };

            borderPanel.Controls.Add(helpTextBox);
            // this.Controls.Add(helpTextBox);
            this.Controls.Add(borderPanel);
            this.Controls.Add(buttonPanel);


            borderPanel.BackColor = Color.Red;
            configTextBox();

            LoadHelpFiles();
        }

        private void configTextBox()
        {
            helpTextBox.BorderStyle = BorderStyle.None;
            helpTextBox.Margin = new Padding(0);
            helpTextBox.BackColor = Color.Violet;
        }

        private void LoadHelpFiles()
        {
            string helpDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "HelpFiles");
            if (!Directory.Exists(helpDir))
            {
                MessageBox.Show("El directorio de archivos de ayuda no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var helpFiles = Directory.GetFiles(helpDir, "*.txt");
            foreach (var file in helpFiles)
            {
                string title;
                try
                {
                    using (var reader = new StreamReader(file))
                    {
                        title = reader.ReadLine() ?? "Sin título";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al leer el archivo {file}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                var button = new Button
                {
                    Text = title,
                    Width = buttonPanel.Width - 2 * MarginSize,
                    Height = 40,
                    Tag = file,
                    Margin = new Padding(MarginSize, MarginSize, MarginSize, 0)
                };
                button.Click += OnHelpButtonClick;
                buttonPanel.Controls.Add(button);
            }
        }

        private void OnHelpButtonClick(object? sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is string filePath)
            {
                try
                {
                    string content = File.ReadAllText(filePath);
                    helpTextBox.Text = content;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar el contenido del archivo {filePath}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
