using System;
using System.Drawing;
using System.Windows.Forms;
using KaizenLang.UI.Theming;

namespace KaizenLang.UI.Components;

public class SidebarPanel : Panel
{
    private Label titleLabel = null!;
    private RichTextBox infoBox = null!;
    private Panel headerPanel = null!;

    public SidebarPanel()
    {
        Dock = DockStyle.Fill;
        Padding = new Padding(10);
        Width = 280;

        InitializeControls();
    }

    private void InitializeControls()
    {
        // Header
        headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            Padding = new Padding(0, 0, 0, 10)
        };

        titleLabel = new Label
        {
            Text = "Guía Rápida",
            Dock = DockStyle.Top,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            Height = 35,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(0, 8, 0, 0)
        };

        headerPanel.Controls.Add(titleLabel);

        // Info box
        infoBox = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            Font = new Font("Consolas", 9F),
            Padding = new Padding(12),
            WordWrap = false,
            ScrollBars = RichTextBoxScrollBars.Vertical
        };

        LoadQuickGuide();

        Controls.Add(infoBox);
        Controls.Add(headerPanel);
    }

        private void LoadQuickGuide()
        {
            var guide = @"
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
  GUÍA RÁPIDA
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

▸ VARIABLES

  integer numero = 10;
  string texto = ""Hola"";
  bool estado = true;

▸ CONDICIONALES

  if (numero > 5) ying
      output(""Mayor"");
  yang

▸ BUCLES

  while (numero < 10) ying
      numero = numero + 1;
  yang

  for (integer i=0; i<5; i++) ying
      output(i);
  yang

▸ FUNCIONES

  integer suma(integer a, integer b) ying
      return a + b;
  yang

▸ ARRAYS

  chainsaw<integer> lista = [1, 2, 3];

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

⌨ ATAJOS

  F5       Ejecutar
  F6       Compilar
  Ctrl+S   Guardar
  Ctrl+O   Abrir

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━";

            infoBox.Text = guide;
            infoBox.SelectionStart = 0;
            infoBox.SelectionLength = 0;
        }    public void UpdateTheme(Theme theme)
    {
        BackColor = theme.PanelBackground;
        titleLabel.ForeColor = theme.Foreground;
        infoBox.BackColor = theme.SecondaryBackground;
        infoBox.ForeColor = theme.SecondaryForeground;
        headerPanel.BackColor = theme.PanelBackground;
    }
}
