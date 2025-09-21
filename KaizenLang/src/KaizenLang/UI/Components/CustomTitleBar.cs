using System.Runtime.InteropServices;
using System.ComponentModel;

namespace KaizenLang.UI.Components
{
    public class CustomTitleBar : UserControl
    {
        public event EventHandler? CloseClicked;
        public event EventHandler? MinimizeClicked;
        public event EventHandler? MaximizeClicked;

        private Label lblTitle;

        private Button btnMinimize;
        private Button btnMaximize;
        private Button btnClose;

        public CustomTitleBar()
        {
            this.Height = 32;
            this.Dock = DockStyle.Top;
            // this.BackColor = Color.FromArgb(30, 30, 40);

            lblTitle = new Label
            {
                Text = "KaizenLang",
                ForeColor = Color.White,
                Dock = DockStyle.Left,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Width = 200
            };
            this.Controls.Add(lblTitle);

            btnMinimize = new Button
            {
                Text = "—",
                Dock = DockStyle.Right,
                Width = 40,
                FlatStyle = FlatStyle.Flat
            };
            btnMinimize.FlatAppearance.BorderSize = 0;
            btnMinimize.Click += (s, e) => MinimizeClicked?.Invoke(this, EventArgs.Empty);
            this.Controls.Add(btnMinimize);

            btnMaximize = new Button
            {
                Text = "▢",
                Dock = DockStyle.Right,
                Width = 40,
                FlatStyle = FlatStyle.Flat
            };
            btnMaximize.FlatAppearance.BorderSize = 0;
            btnMaximize.Click += (s, e) => MaximizeClicked?.Invoke(this, EventArgs.Empty);
            this.Controls.Add(btnMaximize);

            btnClose = new Button
            {
                Text = "✕",
                Dock = DockStyle.Right,
                Width = 40,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Red
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => CloseClicked?.Invoke(this, EventArgs.Empty);
            this.Controls.Add(btnClose);

            this.MouseDown += CustomTitleBar_MouseDown;
            lblTitle.MouseDown += CustomTitleBar_MouseDown;
        }

        // Permitir mover la ventana
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void CustomTitleBar_MouseDown(object? sender, MouseEventArgs e)
        {
            var form = this.FindForm();
            if (e.Button == MouseButtons.Left && form != null)
            {
                ReleaseCapture();
                SendMessage(form.Handle, 0xA1, 0x2, 0);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title
        {
            get => lblTitle.Text;
            set => lblTitle.Text = value;
        }
    }
}