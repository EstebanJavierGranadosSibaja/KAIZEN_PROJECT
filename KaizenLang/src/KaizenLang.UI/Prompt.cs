using System.Drawing;
using System.Windows.Forms;
using KaizenLang.UI.Theme;

namespace KaizenLang.UI;

public static class Prompt
{
    public static string? Show(string title, string? prompt)
    {
            using (var form = new Form())
            {
                form.Width = 420;
                form.Height = 170;
                form.Text = title ?? "Input";
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.ApplyCurrentTheme();

                var label = new Label()
                {
                    Left = 12,
                    Top = 12,
                    Width = 396,
                    Text = prompt ?? "",
                    Font = new Font("Segoe UI", 9, FontStyle.Regular)
                };

                var textBox = new TextBox()
                {
                    Left = 12,
                    Top = 40,
                    Width = 396,
                    Font = new Font("Consolas", 10)
                };

                var okButton = new Button()
                {
                    Text = "OK",
                    Left = 230,
                    Width = 80,
                    Top = 95,
                    DialogResult = DialogResult.OK,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                var cancelButton = new Button()
                {
                    Text = "Cancel",
                    Left = 315,
                    Width = 80,
                    Top = 95,
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                form.Controls.Add(label);
                form.Controls.Add(textBox);
                form.Controls.Add(okButton);
                form.Controls.Add(cancelButton);
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
