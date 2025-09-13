using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using ParadigmasLang;

namespace KaizenLang.UI
{
	public class MainForm : Form
	{
		private TextBox? codeBox;
		private TextBox? outputBox;
		private Button? compileButton;
		private Button? executeButton;
		private MenuStrip? menuStrip;
		private CompilationService? compilationService;
		private ExecutionService? executionService;

		public MainForm()
		{
			InitializeServices();
			InitializeForm();
			InitializeControls();
		}

		private void InitializeServices()
		{
			compilationService = new CompilationService();
			executionService = new ExecutionService();
		}

		private void InitializeForm()
		{
			this.Text = UIConstants.Text.WINDOW_TITLE;
			this.Width = UIConstants.MAIN_WINDOW_WIDTH;
			this.Height = UIConstants.MAIN_WINDOW_HEIGHT;
			this.BackColor = UIConstants.Colors.MainBackground;
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
		}

		private void InitializeControls()
		{
			// Crear panel de código primero
			var codePanel = ControlFactory.CreateCodePanel();
			codeBox = ControlFactory.CreateCodeTextBox(codePanel);
			codeBox.TextChanged += CodeBox_TextChanged; // Agregar evento para resetear colores
			codePanel.Controls.Add(codeBox);
			this.Controls.Add(codePanel);

			// Ahora crear menú con la referencia válida al codeBox
			menuStrip = MenuBuilder.CreateMainMenu(codeBox);
			this.Controls.Add(menuStrip);

			// Crear botones
			compileButton = ControlFactory.CreateCompileButton(codePanel);
			compileButton.Click += CompileButton_Click;
			this.Controls.Add(compileButton);

			executeButton = ControlFactory.CreateExecuteButton(compileButton);
			executeButton.Click += ExecuteButton_Click;
			this.Controls.Add(executeButton);

			// Crear panel de salida
			var outputPanel = ControlFactory.CreateOutputPanel(compileButton);
			outputBox = ControlFactory.CreateOutputTextBox(outputPanel);
			outputPanel.Controls.Add(outputBox);
			this.Controls.Add(outputPanel);
		}

		private async void CompileButton_Click(object? sender, EventArgs? e)
		{
			if (codeBox == null || outputBox == null || compilationService == null || compileButton == null) return;
			
			// Deshabilitar botón durante procesamiento
			compileButton.Enabled = false;
			
			// Mostrar estado de procesamiento
			ControlFactory.SetButtonState(compileButton, ControlFactory.ButtonState.Processing);
			
			try
			{
				// Simular procesamiento asíncrono
				await Task.Run(() =>
				{
					string source = codeBox.Text;
					var result = compilationService.CompileCode(source);
					
					// Actualizar UI en el hilo principal
					this.Invoke(() =>
					{
						outputBox.Text = result.Output;
						
						// Cambiar estado del botón según el resultado
						ControlFactory.SetButtonState(compileButton, 
							result.IsSuccessful ? ControlFactory.ButtonState.Success : ControlFactory.ButtonState.Error);
					});
				});
			}
			finally
			{
				// Rehabilitar botón
				compileButton.Enabled = true;
			}
		}

		private async void ExecuteButton_Click(object? sender, EventArgs? e)
		{
			if (codeBox == null || outputBox == null || executionService == null || executeButton == null) return;
			
			// Deshabilitar botón durante procesamiento
			executeButton.Enabled = false;
			
			// Mostrar estado de procesamiento
			ControlFactory.SetButtonState(executeButton, ControlFactory.ButtonState.Processing);
			
			try
			{
				// Simular procesamiento asíncrono
				await Task.Run(() =>
				{
					string source = codeBox.Text;
					var result = executionService.ExecuteCode(source);
					
					// Actualizar UI en el hilo principal
					this.Invoke(() =>
					{
						outputBox.Text = result.Output;
						
						// Cambiar estado del botón según el resultado
						ControlFactory.SetButtonState(executeButton, 
							result.IsSuccessful ? ControlFactory.ButtonState.Success : ControlFactory.ButtonState.Error);
					});
				});
			}
			finally
			{
				// Rehabilitar botón
				executeButton.Enabled = true;
			}
		}

		private void CodeBox_TextChanged(object? sender, EventArgs? e)
		{
			// Resetear estados de botones cuando se modifica el código
			if (compileButton != null)
				ControlFactory.SetButtonState(compileButton, ControlFactory.ButtonState.Normal);
			if (executeButton != null)
				ControlFactory.SetButtonState(executeButton, ControlFactory.ButtonState.Normal);
		}
	}
}
