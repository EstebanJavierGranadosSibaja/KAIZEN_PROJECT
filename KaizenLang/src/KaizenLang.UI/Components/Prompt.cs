using System.Drawing;
using System.Windows.Forms;
using KaizenLang.UI.Theming;

namespace KaizenLang.UI.Components;

public static class Prompt
{
    public static string? Show(string title, string? prompt)
    {
        using (var form = new Form())
        {
            form.Width = 460;
            form.Height = 200;
            form.Text = title ?? "Entrada requerida";
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.Padding = new Padding(12, 15, 12, 12);
            form.ApplyCurrentTheme();

            // Texto descriptivo
            var label = new Label()
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = prompt ?? "",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Padding = new Padding(2, 0, 2, 0)
            };

            // Caja de texto
            var textBox = new TextBox()
            {
                Dock = DockStyle.Top,
                Margin = new Padding(0, 8, 0, 8),
                Font = new Font("Consolas", 11, FontStyle.Regular)
            };

            // Panel inferior para botones
            var buttonPanel = new Panel()
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(0, 5, 0, 0)
            };

            // Botón OK
            var okButton = new Button()
            {
                Text = "Aceptar",
                DialogResult = DialogResult.OK,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Height = 32,
                Width = 100,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Margin = new Padding(6)
            };
            okButton.FlatAppearance.BorderSize = 0;

            // Botón Cancelar
            var cancelButton = new Button()
            {
                Text = "Cancelar",
                DialogResult = DialogResult.Cancel,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Height = 32,
                Width = 100,
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Margin = new Padding(6)
            };
            cancelButton.FlatAppearance.BorderSize = 0;

            // Flujo de botones a la derecha
            var flow = new FlowLayoutPanel()
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false
            };
            flow.Controls.Add(okButton);
            flow.Controls.Add(cancelButton);

            buttonPanel.Controls.Add(flow);

            // Agregar controles
            form.Controls.Add(buttonPanel);
            form.Controls.Add(textBox);
            form.Controls.Add(label);

            // Teclas rápidas
            form.AcceptButton = okButton;
            form.CancelButton = cancelButton;

            form.ApplyCurrentThemeRecursive();

            var result = form.ShowDialog();
            if (result == DialogResult.OK)
                return textBox.Text;
            return null;
        }
    }
}
