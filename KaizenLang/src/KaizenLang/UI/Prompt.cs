using System.Windows.Forms;

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
                form.BackColor = UIConstants.Colors.PanelBackground;

                var label = new Label()
                {
                    Left = 12,
                    Top = 12,
                    Width = 396,
                    Text = prompt ?? "",
                    ForeColor = UIConstants.Colors.CodeForeground,
                    BackColor = Color.Transparent,
                    Font = new Font(UIConstants.Fonts.MenuFont.FontFamily, 9, FontStyle.Regular)
                };

                var textBox = new TextBox()
                {
                    Left = 12,
                    Top = 40,
                    Width = 396,
                    BackColor = UIConstants.Colors.CodeBackground,
                    ForeColor = UIConstants.Colors.CodeForeground,
                    Font = UIConstants.Fonts.CodeFont
                };

                var okButton = new Button()
                {
                    Text = "OK",
                    Left = 230,
                    Width = 80,
                    Top = 95,
                    DialogResult = DialogResult.OK,
                    BackColor = UIConstants.Colors.CompileButton,
                    ForeColor = UIConstants.Colors.ButtonText,
                    FlatStyle = FlatStyle.Flat,
                    Font = UIConstants.Fonts.ButtonFont
                };

                var cancelButton = new Button()
                {
                    Text = "Cancel",
                    Left = 315,
                    Width = 80,
                    Top = 95,
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.Transparent,
                    ForeColor = UIConstants.Colors.MenuForeground,
                    FlatStyle = FlatStyle.Flat,
                    Font = UIConstants.Fonts.ButtonFont
                };

                okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

                form.Controls.Add(label);
                form.Controls.Add(textBox);
                form.Controls.Add(okButton);
                form.Controls.Add(cancelButton);
                form.AcceptButton = okButton;
                form.CancelButton = cancelButton;

                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                    return textBox.Text;
                return null;
            }
    }
}
