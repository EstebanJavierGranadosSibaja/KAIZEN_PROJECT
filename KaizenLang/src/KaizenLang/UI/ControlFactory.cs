using System.Windows.Forms;
using System.Drawing;

namespace KaizenLang.UI
{
    public static class ControlFactory
    {
        public static Panel CreateCodePanel()
        {
            var codePanel = new Panel
            {
                BackColor = UIConstants.Colors.PanelBackground,
                BorderStyle = BorderStyle.FixedSingle,
                Width = UIConstants.CODE_PANEL_WIDTH,
                Height = UIConstants.CODE_PANEL_HEIGHT,
                Top = UIConstants.PANEL_TOP_MARGIN,
                Left = UIConstants.PANEL_LEFT_MARGIN,
                Padding = new Padding(UIConstants.PANEL_PADDING)
            };

            codePanel.Paint += (s, e) => PaintPanelWithShadow(e.Graphics, codePanel);
            return codePanel;
        }

        public static TextBox CreateCodeTextBox(Panel parent)
        {
            var codeBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = parent.Width - UIConstants.PANEL_PADDING * 2,
                Height = parent.Height - UIConstants.PANEL_PADDING * 2,
                Top = UIConstants.CONTROL_MARGIN,
                Left = UIConstants.CONTROL_MARGIN,
                Font = UIConstants.Fonts.CodeFont,
                BackColor = UIConstants.Colors.CodeBackground,
                ForeColor = UIConstants.Colors.CodeForeground,
                BorderStyle = BorderStyle.None
            };

            return codeBox;
        }

        public static Button CreateCompileButton(Panel codePanel)
        {
            var button = new Button
            {
                Text = UIConstants.Text.COMPILE_BUTTON,
                Top = codePanel.Bottom + UIConstants.BUTTON_TOP_OFFSET,
                Left = codePanel.Left,
                Width = UIConstants.BUTTON_WIDTH,
                Height = UIConstants.BUTTON_HEIGHT,
                BackColor = UIConstants.Colors.CompileButton,
                ForeColor = UIConstants.Colors.ButtonText,
                FlatStyle = FlatStyle.Flat,
                Font = UIConstants.Fonts.ButtonFont
            };

            button.FlatAppearance.BorderSize = 0;
            SetupButtonHoverEffects(button, UIConstants.Colors.CompileButton, UIConstants.Colors.CompileButtonHover);

            return button;
        }

        public static Button CreateExecuteButton(Button compileButton)
        {
            var button = new Button
            {
                Text = UIConstants.Text.EXECUTE_BUTTON,
                Top = compileButton.Top,
                Left = compileButton.Right + UIConstants.BUTTON_SPACING,
                Width = UIConstants.BUTTON_WIDTH,
                Height = UIConstants.BUTTON_HEIGHT,
                BackColor = UIConstants.Colors.ExecuteButton,
                ForeColor = UIConstants.Colors.ButtonText,
                FlatStyle = FlatStyle.Flat,
                Font = UIConstants.Fonts.ButtonFont
            };

            button.FlatAppearance.BorderSize = 0;
            SetupButtonHoverEffects(button, UIConstants.Colors.ExecuteButton, UIConstants.Colors.ExecuteButtonHover);

            return button;
        }

        public static Panel CreateOutputPanel(Button compileButton)
        {
            var outputPanel = new Panel
            {
                BackColor = UIConstants.Colors.OutputBackground,
                BorderStyle = BorderStyle.FixedSingle,
                Width = UIConstants.OUTPUT_PANEL_WIDTH,
                Height = UIConstants.OUTPUT_PANEL_HEIGHT,
                Top = compileButton.Bottom + UIConstants.OUTPUT_TOP_OFFSET,
                Left = UIConstants.PANEL_LEFT_MARGIN,
                Padding = new Padding(UIConstants.PANEL_PADDING)
            };

            outputPanel.Paint += (s, e) => PaintOutputPanelWithShadow(e.Graphics, outputPanel);
            return outputPanel;
        }

        public static TextBox CreateOutputTextBox(Panel parent)
        {
            var outputBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Width = parent.Width - UIConstants.PANEL_PADDING * 2,
                Height = parent.Height - UIConstants.PANEL_PADDING * 2,
                Top = UIConstants.CONTROL_MARGIN,
                Left = UIConstants.CONTROL_MARGIN,
                ReadOnly = true,
                Font = UIConstants.Fonts.OutputFont,
                BackColor = UIConstants.Colors.OutputBackground,
                ForeColor = UIConstants.Colors.OutputForeground,
                BorderStyle = BorderStyle.None
            };

            return outputBox;
        }

        private static void SetupButtonHoverEffects(Button button, Color normalColor, Color hoverColor)
        {
            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = normalColor;
        }

        private static void PaintPanelWithShadow(Graphics graphics, Panel panel)
        {
            var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
            using (var pen = new Pen(UIConstants.Colors.PanelBorder, 2))
                graphics.DrawRectangle(pen, rect);
            // Sombra inferior
            using (var brush = new SolidBrush(UIConstants.Colors.Shadow))
                graphics.FillRectangle(brush, 0, panel.Height - 10, panel.Width, 10);
        }

        private static void PaintOutputPanelWithShadow(Graphics graphics, Panel panel)
        {
            var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
            using (var pen = new Pen(UIConstants.Colors.PanelBorder, 2))
                graphics.DrawRectangle(pen, rect);
            // Sombra inferior
            using (var brush = new SolidBrush(UIConstants.Colors.ShadowDark))
                graphics.FillRectangle(brush, 0, panel.Height - 10, panel.Width, 10);
        }
    }
}
