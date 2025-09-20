using System.Windows.Forms;

namespace KaizenLang.UI;

public static class Prompt
{
    public static string? Show(string title, string? prompt)
    {
        using (var form = new Form())
        {
            form.Width = 400;
            form.Height = 150;
            form.Text = title ?? "Input";
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterParent;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            var label = new Label() { Left = 10, Top = 10, Width = 360, Text = prompt ?? "" };
            var textBox = new TextBox() { Left = 10, Top = 35, Width = 360 };
            var okButton = new Button() { Text = "OK", Left = 210, Width = 75, Top = 70, DialogResult = DialogResult.OK };
            var cancelButton = new Button() { Text = "Cancel", Left = 295, Width = 75, Top = 70, DialogResult = DialogResult.Cancel };

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
