using KaizenLang.UI.Components;
using KaizenLang.UI.Theme;
using KaizenLang.UI.Utils;

namespace KaizenLang.UI
{
	/// <summary>
	/// Venata principal. Sus elementos son:
	/// Menu contiene las opciones principales de la aplicacion
	/// Texto de codigo contiene el codigo fuente a compilar/ejecutar
	/// Texto Salida (contiene un menu para ocultar/mostrar, limpiar)
	/// </summary>
	public class MainWindow : ModernWindowBase
	{
		private ModernTextBoxBase? codeBox;
		private ModernTextBoxBase? outputBox;

		private Panel? contentPanel;
		private Panel? codePanel;
		private Panel? outputPanel;

		private MenuStrip? menuBar;
		private MenuStrip? outputMenuBar;

		private CompilationService? compilationService;
		private ExecutionService? executionService;

		public MainWindow()
		{
			InitializeServices();
			InitializeForm();
			InitializeControls();
			ApplyThemeToAllComponents();

			// Suscribirse a cambios de tema
			ThemeManager.SubscribeToThemeChanges(this);
		}

		private void InitializeServices()
		{
			compilationService = new CompilationService();
			executionService = new ExecutionService();
		}

		private void InitializeForm()
		{
			this.Text = "KaizenLang IDE";
			this.Width = UIConstants.MAIN_WINDOW_WIDTH;
			this.Height = UIConstants.MAIN_WINDOW_HEIGHT;

			// Dimensiones mínimas - al menos 800x600 para un IDE funcional
			this.MinimumSize = new Size(800, 600);

			// Aplicar colores del tema
			this.BackColor = ThemeManager.CurrentTheme.Background;
			this.ForeColor = ThemeManager.CurrentTheme.Foreground;
			this.SetBorderColor(ThemeManager.CurrentTheme.Border);

			// Establecer el ícono de la aplicación desde los recursos
			try
			{
				this.Icon = Properties.Resources.AppIcon;
			}
			catch (Exception)
			{
				// Si no se puede cargar el ícono, continuamos sin él
			}

			// Iniciar maximizado para aprovechar el espacio de la pantalla
			this.WindowState = FormWindowState.Maximized;

			// Establecer propiedades de ventana para un aspecto moderno
			this.FormBorderStyle = FormBorderStyle.Sizable;
			this.StartPosition = FormStartPosition.CenterScreen;
		}

		private void InitializeControls()
		{
			// Configurar la barra de menú en la parte superior
			ConfigMenuBar();

			// Panel principal que contiene todos los demás elementos
			contentPanel = new Panel
			{
				Dock = DockStyle.Fill,
				Padding = new Padding(0), // Sin padding para maximizar espacio
				BackColor = ThemeManager.CurrentTheme.Background
			};
			this.Controls.Add(contentPanel);

			// Crear un SplitContainer para dividir el área de código y la salida
			// En VS Code, el panel de salida está debajo del editor (orientación horizontal)
			var splitContainer = new SplitContainer
			{
				Dock = DockStyle.Fill,
				Orientation = Orientation.Horizontal, // Horizontal para que el panel de salida esté abajo
				SplitterWidth = 5,
				Panel1MinSize = 150, // Tamaño mínimo razonable para el panel de código
				Panel2MinSize = 80,  // Tamaño mínimo para el panel de salida
				BackColor = ThemeManager.CurrentTheme.Border, // Color del splitter
				IsSplitterFixed = false // Permite al usuario ajustar manualmente los tamaños
			};

			// Calcular el tamaño inicial para que el panel de salida tenga un tamaño fijo de 100px
			splitContainer.SplitterDistance = splitContainer.Height - 100;
			contentPanel.Controls.Add(splitContainer);

			// Configurar el panel de código en la parte superior del SplitContainer
			ConfigCodeText(splitContainer.Panel1);

			// Configurar el panel de salida en la parte inferior del SplitContainer
			ConfigOutputText(splitContainer.Panel2);
		}

		private void ConfigCodeText(Control parent)
		{
			// * Panel de código *
			codePanel = new Panel
			{
				Dock = DockStyle.Fill,
				Padding = new Padding(0), // Sin padding en el panel principal
				BackColor = ThemeManager.CurrentTheme.Background
			};

			// Panel para contener la etiqueta y proporcionar márgenes adecuados
			var headerPanel = new Panel
			{
				Dock = DockStyle.Top,
				Height = 30,
				Padding = new Padding(10, 5, 10, 0),
				BackColor = ThemeManager.CurrentTheme.Background
			};

			// Etiqueta para mostrar "Editor" en la parte superior del panel
			var editorLabel = new Label
			{
				Text = "EDITOR",
				Dock = DockStyle.Fill,
				TextAlign = ContentAlignment.MiddleLeft,
				Font = new Font("Segoe UI", 9f, FontStyle.Bold),
				ForeColor = ThemeManager.CurrentTheme.Foreground,
				BackColor = Color.Transparent
			};
			headerPanel.Controls.Add(editorLabel);
			codePanel.Controls.Add(headerPanel);

			// Panel para contener el editor con márgenes visuales externos
			var editorContainer = new Panel
			{
				Dock = DockStyle.Fill,
				Padding = new Padding(8, 3, 8, 8), // Márgenes externos del contenedor
				BackColor = ThemeManager.CurrentTheme.Background
			};

			// * Texto de código - ahora sin márgenes internos problemáticos *
			codeBox = new ModernTextBoxBase
			{
				Text = "// Escribe tu código KaizenLang aquí...\n",
				BackColor = ThemeManager.CurrentTheme.SecondaryBackground,
				ForeColor = ThemeManager.CurrentTheme.SecondaryForeground,
				Font = new Font("Consolas", 11f),
				Dock = DockStyle.Fill,
				BorderStyle = BorderStyle.None
			};
			editorContainer.Controls.Add(codeBox);
			codePanel.Controls.Add(editorContainer);

			// Agregar el panel de código al contenedor padre (Panel1 del SplitContainer)
			parent.Controls.Add(codePanel);
		}

		private void ConfigOutputText(Control parent)
		{
			// * Panel de salida *
			outputPanel = new Panel
			{
				Dock = DockStyle.Fill,
				Padding = new Padding(0),
				BackColor = ThemeManager.CurrentTheme.Background
			};

			// Configurar la barra de herramientas de salida
			ConfigOutputMenuBar();

			// Panel para contener la barra de menú con márgenes apropiados
			var menuContainer = new Panel
			{
				Dock = DockStyle.Top,
				Height = 30,
				Padding = new Padding(0), // Sin padding porque el MenuStrip ya lo tiene
				BackColor = ThemeManager.CurrentTheme.Background
			};

			// Agregamos la barra de herramientas de salida si fue creada exitosamente
			if (outputMenuBar != null)
			{
				outputMenuBar.Dock = DockStyle.Fill;
				menuContainer.Controls.Add(outputMenuBar);
			}
			outputPanel.Controls.Add(menuContainer);

			// Panel para contener el texto de salida con márgenes visuales externos
			var outputContainer = new Panel
			{
				Dock = DockStyle.Fill,
				Padding = new Padding(8, 3, 8, 8), // Márgenes externos del contenedor
				BackColor = ThemeManager.CurrentTheme.Background
			};

			// * Texto de salida - ahora sin márgenes internos problemáticos *
			outputBox = new ModernTextBoxBase
			{
				Text = "// La salida de compilación y ejecución aparecerá aquí...\n",
				BackColor = ThemeManager.CurrentTheme.SecondaryBackground,
				ForeColor = ThemeManager.CurrentTheme.SecondaryForeground,
				ReadOnly = true,
				Font = new Font("Consolas", 10f),
				Dock = DockStyle.Fill,
				BorderStyle = BorderStyle.None,
				WordWrap = true
			};
			outputContainer.Controls.Add(outputBox);
			outputPanel.Controls.Add(outputContainer);

			// Agregar el panel de salida al contenedor padre (Panel2 del SplitContainer)
			parent.Controls.Add(outputPanel);
		}
		private void ConfigMenuBar()
		{
			// * Barra de menú principal con estilo moderno *
			menuBar = new MenuStrip
			{
				BackColor = ThemeManager.CurrentTheme.Background,
				ForeColor = ThemeManager.CurrentTheme.Foreground,
				Renderer = new ToolStripProfessionalRenderer(new MenuColorTable()),
				Padding = new Padding(3, 2, 0, 2),
				ImageScalingSize = new Size(16, 16)
			};
			this.MainMenuStrip = menuBar;
			this.Controls.Add(menuBar);

			// * Menu Archivo *
			var archivoMenu = new ToolStripMenuItem("Archivo");
			archivoMenu.DropDownItems.Add(new ToolStripMenuItem("Nuevo", null, OnNuevoClick) { ShortcutKeys = Keys.Control | Keys.N });
			archivoMenu.DropDownItems.Add(new ToolStripMenuItem("Abrir", null, OnAbrirClick) { ShortcutKeys = Keys.Control | Keys.O });
			archivoMenu.DropDownItems.Add(new ToolStripMenuItem("Guardar", null, OnGuardarClick) { ShortcutKeys = Keys.Control | Keys.S });
			archivoMenu.DropDownItems.Add(new ToolStripSeparator());
			archivoMenu.DropDownItems.Add(new ToolStripMenuItem("Salir", null, (s, e) => this.Close()) { ShortcutKeys = Keys.Alt | Keys.F4 });

			// * Menu Editar *
			var editarMenu = new ToolStripMenuItem("Editar");
			editarMenu.DropDownItems.Add(new ToolStripMenuItem("Copiar", null, (s, e) => { if (codeBox != null && codeBox.Focused) SendKeys.Send("^c"); }) { ShortcutKeys = Keys.Control | Keys.C });
			editarMenu.DropDownItems.Add(new ToolStripMenuItem("Cortar", null, (s, e) => { if (codeBox != null && codeBox.Focused) SendKeys.Send("^x"); }) { ShortcutKeys = Keys.Control | Keys.X });
			editarMenu.DropDownItems.Add(new ToolStripMenuItem("Pegar", null, (s, e) => { if (codeBox != null && codeBox.Focused) SendKeys.Send("^v"); }) { ShortcutKeys = Keys.Control | Keys.V });
			editarMenu.DropDownItems.Add(new ToolStripSeparator());
			editarMenu.DropDownItems.Add(new ToolStripMenuItem("Seleccionar todo", null, (s, e) => { if (codeBox != null) codeBox.SelectAll(); }) { ShortcutKeys = Keys.Control | Keys.A });

			// * Menu Ejecutar con botones destacados *
			var ejecutarMenu = new ToolStripMenuItem("Ejecutar");

			var compileItem = new ToolStripMenuItem("Compilar", null, CompileButton_Click)
			{
				ShortcutKeys = Keys.F5 | Keys.Control,
				Font = new Font(menuBar.Font, FontStyle.Bold)
			};
			ejecutarMenu.DropDownItems.Add(compileItem);

			var executeItem = new ToolStripMenuItem("Ejecutar", null, ExecuteButton_Click)
			{
				ShortcutKeys = Keys.F5,
				Font = new Font(menuBar.Font, FontStyle.Bold)
			};
			ejecutarMenu.DropDownItems.Add(executeItem);

			// * Botones principales en la barra de menú *
			var compileButton = new ToolStripButton("▶ Compilar", null, CompileButton_Click)
			{
				DisplayStyle = ToolStripItemDisplayStyle.Text,
				BackColor = ThemeManager.CurrentTheme.Info,
				ForeColor = Color.White,
				Font = new Font(menuBar.Font, FontStyle.Bold),
				Padding = new Padding(5, 0, 5, 0),
				Margin = new Padding(10, 0, 0, 0)
			};

			var executeButton = new ToolStripButton("▶ Ejecutar", null, ExecuteButton_Click)
			{
				DisplayStyle = ToolStripItemDisplayStyle.Text,
				BackColor = ThemeManager.CurrentTheme.PrimaryAccent,
				ForeColor = Color.White,
				Font = new Font(menuBar.Font, FontStyle.Bold),
				Padding = new Padding(5, 0, 5, 0),
				Margin = new Padding(5, 0, 0, 0)
			};

			// * Ver Menu *
			var verMenu = new ToolStripMenuItem("Ver");
			verMenu.DropDownItems.Add(new ToolStripMenuItem("Cambiar Tema", null, OnCambiarTemaClick));

			// * Menu Ayuda *
			var ayudaMenu = new ToolStripMenuItem("Ayuda");
			ayudaMenu.DropDownItems.Add(new ToolStripMenuItem("Guía", null, OnGuiaClick) { ShortcutKeys = Keys.F1 });
			ayudaMenu.DropDownItems.Add(new ToolStripSeparator());
			ayudaMenu.DropDownItems.Add(new ToolStripMenuItem("Acerca de", null, OnAcercaDeClick));

			// * Agregar todos los menús a la barra *
			menuBar.Items.Add(archivoMenu);
			menuBar.Items.Add(editarMenu);
			menuBar.Items.Add(ejecutarMenu);
			menuBar.Items.Add(verMenu);
			menuBar.Items.Add(ayudaMenu);

			// * Agregar botones de compilación y ejecución directamente a la barra *
			menuBar.Items.Add(new ToolStripSeparator());
			menuBar.Items.Add(compileButton);
			menuBar.Items.Add(executeButton);
		}

		// Clase para personalizar los colores del menú
		private class MenuColorTable : ProfessionalColorTable
		{
			public override Color MenuItemSelected => ThemeManager.CurrentTheme.ButtonMouseOver;
			public override Color MenuItemBorder => ThemeManager.CurrentTheme.Border;
			public override Color MenuBorder => ThemeManager.CurrentTheme.Border;
			public override Color MenuItemSelectedGradientBegin => ThemeManager.CurrentTheme.ButtonMouseOver;
			public override Color MenuItemSelectedGradientEnd => ThemeManager.CurrentTheme.ButtonMouseOver;
			public override Color MenuItemPressedGradientBegin => ThemeManager.CurrentTheme.Selection;
			public override Color MenuItemPressedGradientEnd => ThemeManager.CurrentTheme.Selection;
			public override Color ToolStripDropDownBackground => ThemeManager.CurrentTheme.MenuBackground;
			public override Color MenuStripGradientBegin => ThemeManager.CurrentTheme.MenuBackground;
			public override Color MenuStripGradientEnd => ThemeManager.CurrentTheme.MenuBackground;
		}

		private void ConfigOutputMenuBar()
		{
			// * Barra de menu de salida simplificada *
			outputMenuBar = new MenuStrip
			{
				BackColor = ThemeManager.CurrentTheme.Background,
				ForeColor = ThemeManager.CurrentTheme.Foreground,
				Dock = DockStyle.Fill,
				Padding = new Padding(10, 2, 10, 0),
				Height = 30,
				Renderer = new ToolStripProfessionalRenderer(new MenuColorTable()),
				GripStyle = ToolStripGripStyle.Hidden
			};

			// Etiqueta para mostrar "SALIDA" en la parte izquierda
			var outputLabel = new ToolStripLabel
			{
				Text = "SALIDA",
				Font = new Font("Segoe UI", 9f, FontStyle.Bold),
				ForeColor = ThemeManager.CurrentTheme.Foreground,
				Margin = new Padding(0, 0, 15, 0)
			};
			outputMenuBar.Items.Add(outputLabel);

			// * Botón Limpiar *
			var limpiarItem = new ToolStripButton("Limpiar", null, (s, e) =>
			{
				if (outputBox != null)
				{
					outputBox.Text = "// La salida de compilación y ejecución aparecerá aquí...\n";
				}
			})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Text,
				ForeColor = ThemeManager.CurrentTheme.Foreground,
				Font = new Font("Segoe UI", 9f)
			};
			outputMenuBar.Items.Add(limpiarItem);

			// * Botón para maximizar/minimizar panel de salida *
			var toggleButton = new ToolStripButton("Maximizar", null, (s, e) =>
			{
				if (contentPanel?.Controls[0] is SplitContainer splitContainer && s is ToolStripButton button)
				{
					// La lógica simplemente alterna entre dos tamaños fijos
					// Sin afectar la posición de la ventana del editor

					if (button.Text == "Maximizar")
					{
						// Guarda la distancia actual para poder restaurarla
						splitContainer.Tag = splitContainer.SplitterDistance;

						// Establece un tamaño fijo para el panel de salida (250px)
						splitContainer.SplitterDistance = splitContainer.Height - 250;
						button.Text = "Minimizar";
					}
					else
					{
						// Minimiza el panel de salida a un tamaño pequeño (100px)
						splitContainer.SplitterDistance = splitContainer.Height - 100;
						button.Text = "Maximizar";
					}
				}
			})
			{
				DisplayStyle = ToolStripItemDisplayStyle.Text,
				ForeColor = ThemeManager.CurrentTheme.Foreground,
				Alignment = ToolStripItemAlignment.Right,
				Font = new Font("Segoe UI", 9f)
			};
			outputMenuBar.Items.Add(toggleButton);
		}

		private void ApplyThemeToAllComponents()
		{
			// Aplicar tema a todos los controles
			ThemeManager.ApplyThemeToAllControls(this);
		}

		private async void CompileButton_Click(object? sender, EventArgs? e)
		{
			if (codeBox == null || outputBox == null || compilationService == null) return;

			try
			{
				// Mostrar estado de compilación
				outputBox.Text = "Compilando...\n";
				outputBox.BackColor = ThemeManager.CurrentTheme.SecondaryBackground;

				// Simular procesamiento asíncrono
				await Task.Run(() =>
				{
					string source = codeBox.Text;
					var result = compilationService.CompileCode(source);

					// Actualizar UI en el hilo principal
					this.Invoke(() =>
					{
						outputBox.Text = result.Output;

						// Cambiar color según el resultado
						if (result.Output.Contains("Error"))
						{
							outputBox.ForeColor = ThemeManager.CurrentTheme.Error;
						}
						else if (result.Output.Contains("Warning"))
						{
							outputBox.ForeColor = ThemeManager.CurrentTheme.Warning;
						}
						else
						{
							outputBox.ForeColor = ThemeManager.CurrentTheme.SecondaryForeground;
						}
					});
				});
			}
			catch (Exception ex)
			{
				outputBox.Text = $"Error durante la compilación: {ex.Message}";
				outputBox.ForeColor = ThemeManager.CurrentTheme.Error;
			}
		}

		private async void ExecuteButton_Click(object? sender, EventArgs? e)
		{
			if (codeBox == null || outputBox == null || executionService == null) return;

			try
			{
				// Mostrar estado de ejecución
				outputBox.Text = "Ejecutando...\n";
				outputBox.ForeColor = ThemeManager.CurrentTheme.Info;

				// Simular procesamiento asíncrono
				await Task.Run(() =>
				{
					string source = codeBox.Text;
					var result = executionService.ExecuteCode(source);

					// Actualizar UI en el hilo principal
					this.Invoke(() =>
					{
						outputBox.Text = result.Output;
						outputBox.ForeColor = ThemeManager.CurrentTheme.SecondaryForeground;
					});
				});
			}
			catch (Exception ex)
			{
				outputBox.Text = $"Error durante la ejecución: {ex.Message}";
				outputBox.ForeColor = ThemeManager.CurrentTheme.Error;
			}
		}

		// * Event Handlers *
		private void OnNuevoClick(object? sender, EventArgs? e)
		{
			if (codeBox != null)
			{
				codeBox.Text = "";
				codeBox.Focus();
			}
		}

		private void OnAbrirClick(object? sender, EventArgs? e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "KaizenLang files (*.kz)|*.kz|Text files (*.txt)|*.txt|All files (*.*)|*.*";
				openFileDialog.DefaultExt = "kz";

				if (openFileDialog.ShowDialog() == DialogResult.OK && codeBox != null)
				{
					try
					{
						string content = File.ReadAllText(openFileDialog.FileName);
						codeBox.Text = content;
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Error al abrir el archivo: {ex.Message}", "Error",
							MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}

		private void OnGuardarClick(object? sender, EventArgs? e)
		{
			using (SaveFileDialog saveFileDialog = new SaveFileDialog())
			{
				saveFileDialog.Filter = "KaizenLang files (*.kz)|*.kz|Text files (*.txt)|*.txt|All files (*.*)|*.*";
				saveFileDialog.DefaultExt = "kz";

				if (saveFileDialog.ShowDialog() == DialogResult.OK && codeBox != null)
				{
					try
					{
						File.WriteAllText(saveFileDialog.FileName, codeBox.Text);
						MessageBox.Show("Archivo guardado exitosamente.", "Guardar",
							MessageBoxButtons.OK, MessageBoxIcon.Information);
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Error al guardar el archivo: {ex.Message}", "Error",
							MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}

		private void OnCambiarTemaClick(object? sender, EventArgs? e)
		{
			// Aquí podrías implementar un diálogo para cambiar temas
			MessageBox.Show("Funcionalidad de cambio de tema en desarrollo.", "Cambiar Tema",
				MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void OnAcercaDeClick(object? sender, EventArgs? e)
		{
			MessageBox.Show("KaizenLang IDE\nVersión 1.0\n\nUn IDE moderno para el lenguaje KaizenLang.", "Acerca de");
		}

		private void OnGuiaClick(object? sender, EventArgs? e)
		{
			var helpWindow = new HelpWindow();
			ThemeManager.ApplyThemeToAllControls(helpWindow);
			helpWindow.ShowDialog();
		}
	}
}
