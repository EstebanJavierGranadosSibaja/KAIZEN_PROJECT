using System.Drawing.Drawing2D;

namespace KaizenLang.UI;

public static class ControlFactory
{
    // Delegados para eventos personalizados
    public delegate void ButtonStateChangedHandler(Button button, ButtonState state);
    public static event ButtonStateChangedHandler? ButtonStateChanged;

    public enum ButtonState
    {
        Normal,
        Hover,
        Success,
        Error,
        Processing
    }

    public static Panel CreateCodePanel()
    {
        var codePanel = new Panel
        {
            BackColor = UIConstants.Colors.PanelBackground,
            BorderStyle = BorderStyle.None, // Cambiado para bordes personalizados
            Width = UIConstants.CODE_PANEL_WIDTH,
            Height = UIConstants.CODE_PANEL_HEIGHT,
            Top = UIConstants.PANEL_TOP_MARGIN,
            Left = UIConstants.PANEL_LEFT_MARGIN,
            Padding = new Padding(UIConstants.PANEL_PADDING)
        };

        // Efectos visuales mejorados
        codePanel.Paint += (s, e) => PaintCodePanelWithEnhancedEffects(e.Graphics, codePanel);

        // Agregar etiqueta de título
        var titleLabel = CreatePanelTitle("📝 Editor de Código", codePanel);
        codePanel.Controls.Add(titleLabel);

        return codePanel;
    }

    public static TextBox CreateCodeTextBox(Panel parent)
    {
        var codeBox = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Width = parent.Width - UIConstants.PANEL_PADDING * 2,
            Height = parent.Height - UIConstants.PANEL_PADDING * 2 - 25, // Espacio para título
            Top = UIConstants.CONTROL_MARGIN + 25, // Debajo del título
            Left = UIConstants.CONTROL_MARGIN,
            Font = UIConstants.Fonts.CodeFont,
            BackColor = UIConstants.Colors.CodeBackground,
            ForeColor = UIConstants.Colors.CodeForeground,
            BorderStyle = BorderStyle.None,
            WordWrap = false,
            AcceptsTab = true, // Permitir tabs
            HideSelection = false // Mantener selección visible
        };

        // Agregar numeración de líneas visual
        AddLineNumbering(codeBox);

        // Agregar auto-completado básico
        AddBasicAutoComplete(codeBox);

        return codeBox;
    }

    public static Button CreateCompileButton(Panel codePanel)
    {
        var button = CreateEnhancedButton(
            UIConstants.Text.COMPILE_BUTTON,
            UIConstants.Colors.CompileButton,
            UIConstants.Colors.CompileButtonHover,
            codePanel.Left,
            codePanel.Bottom + UIConstants.BUTTON_TOP_OFFSET
        );

        button.Tag = "compile"; // Para identificación
        return button;
    }

    public static Button CreateExecuteButton(Button compileButton)
    {
        var button = CreateEnhancedButton(
            UIConstants.Text.EXECUTE_BUTTON,
            UIConstants.Colors.ExecuteButton,
            UIConstants.Colors.ExecuteButtonHover,
            compileButton.Right + UIConstants.BUTTON_SPACING,
            compileButton.Top
        );

        button.Tag = "execute"; // Para identificación
        return button;
    }

    public static Panel CreateOutputPanel(Button compileButton)
    {
        var outputPanel = new Panel
        {
            BackColor = UIConstants.Colors.OutputBackground,
            BorderStyle = BorderStyle.None, // Bordes personalizados
            Width = UIConstants.OUTPUT_PANEL_WIDTH,
            Height = UIConstants.OUTPUT_PANEL_HEIGHT,
            Top = compileButton.Bottom + UIConstants.OUTPUT_TOP_OFFSET,
            Left = UIConstants.PANEL_LEFT_MARGIN,
            Padding = new Padding(UIConstants.PANEL_PADDING)
        };

        // Efectos visuales mejorados
        outputPanel.Paint += (s, e) => PaintOutputPanelWithEnhancedEffects(e.Graphics, outputPanel);

        // Agregar etiqueta de título
        var titleLabel = CreatePanelTitle("📤 Salida de Compilación", outputPanel);
        titleLabel.ForeColor = UIConstants.Colors.OutputForeground;
        outputPanel.Controls.Add(titleLabel);

        // Agregar botón de limpiar salida
        var clearButton = CreateClearButton(outputPanel);
        outputPanel.Controls.Add(clearButton);

        return outputPanel;
    }

    public static TextBox CreateOutputTextBox(Panel parent)
    {
        var outputBox = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Width = parent.Width - UIConstants.PANEL_PADDING * 2,
            Height = parent.Height - UIConstants.PANEL_PADDING * 2 - 25, // Espacio para título
            Top = UIConstants.CONTROL_MARGIN + 25, // Debajo del título
            Left = UIConstants.CONTROL_MARGIN,
            ReadOnly = true,
            Font = UIConstants.Fonts.OutputFont,
            BackColor = UIConstants.Colors.OutputBackground,
            ForeColor = UIConstants.Colors.OutputForeground,
            BorderStyle = BorderStyle.None,
            WordWrap = true
        };

        // Agregar capacidad de selección y copia
        outputBox.Enter += (s, e) => outputBox.SelectAll();

        return outputBox;
    }

    // Método mejorado para crear botones con efectos avanzados
    private static Button CreateEnhancedButton(string text, Color normalColor, Color hoverColor, int left, int top)
    {
        var button = new Button
        {
            Text = text,
            Left = left,
            Top = top,
            Width = UIConstants.BUTTON_WIDTH,
            Height = UIConstants.BUTTON_HEIGHT,
            BackColor = normalColor,
            ForeColor = UIConstants.Colors.ButtonText,
            FlatStyle = FlatStyle.Flat,
            Font = UIConstants.Fonts.ButtonFont,
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };

        button.FlatAppearance.BorderSize = 0;

        // Efectos de hover mejorados con animación
        SetupEnhancedButtonEffects(button, normalColor, hoverColor);

        // Efectos de pintura personalizados con texto incluido
        button.Paint += (s, e) => PaintButtonWithGradient(e.Graphics, button);

        return button;
    }

    private static Label CreatePanelTitle(string title, Panel parent)
    {
        return new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = UIConstants.Colors.CodeForeground,
            BackColor = Color.Transparent,
            AutoSize = true,
            Left = UIConstants.CONTROL_MARGIN,
            Top = 5
        };
    }

    private static Button CreateClearButton(Panel parent)
    {
        var clearButton = new Button
        {
            Text = "🗑️",
            Width = 25,
            Height = 20,
            Top = 3,
            Left = parent.Width - UIConstants.PANEL_PADDING - 30, // Cambiar Right por Left calculado
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.Transparent,
            ForeColor = UIConstants.Colors.OutputForeground,
            Font = new Font("Segoe UI", 8),
            Cursor = Cursors.Hand
        };

        clearButton.FlatAppearance.BorderSize = 0;
        clearButton.Click += (s, e) =>
        {
            var outputBox = parent.Controls.OfType<TextBox>().FirstOrDefault();
            if (outputBox != null)
                outputBox.Clear();
        };

        return clearButton;
    }

    private static void AddLineNumbering(TextBox codeBox)
    {
        // Simulación de numeración de líneas con evento Paint del parent
        codeBox.TextChanged += (s, e) =>
        {
            if (codeBox.Parent != null)
                codeBox.Parent.Invalidate(); // Redibuja el panel para actualizar números
        };
    }

    private static void AddBasicAutoComplete(TextBox codeBox)
    {
        var autoCompleteSource = new AutoCompleteStringCollection();
        autoCompleteSource.AddRange(new[]
        {
            "output", "input", "void", "int", "string", "boolean", "float", "double",
            "if", "else", "while", "for", "do", "return", "true", "false", "null",
            "class", "function", "var", "const", "let"
        });

        codeBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        codeBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
        codeBox.AutoCompleteCustomSource = autoCompleteSource;
    }

    private static void SetupEnhancedButtonEffects(Button button, Color normalColor, Color hoverColor)
    {
        var timer = new System.Windows.Forms.Timer { Interval = 20 };
        var currentColor = normalColor;
        var targetColor = normalColor;
        var animationStep = 0.1f;

        timer.Tick += (s, e) =>
        {
            if (currentColor != targetColor)
            {
                currentColor = InterpolateColor(currentColor, targetColor, animationStep);
                button.BackColor = currentColor;

                if (ColorsAreClose(currentColor, targetColor))
                {
                    button.BackColor = targetColor;
                    currentColor = targetColor;
                    timer.Stop();
                }
            }
        };

        button.MouseEnter += (s, e) =>
        {
            targetColor = hoverColor;
            ButtonStateChanged?.Invoke(button, ButtonState.Hover);
            timer.Start();
        };

        button.MouseLeave += (s, e) =>
        {
            targetColor = normalColor;
            ButtonStateChanged?.Invoke(button, ButtonState.Normal);
            timer.Start();
        };

        button.MouseDown += (s, e) =>
        {
            button.BackColor = Color.FromArgb(
                Math.Max(0, hoverColor.R - 30),
                Math.Max(0, hoverColor.G - 30),
                Math.Max(0, hoverColor.B - 30)
            );
        };
    }

    // Métodos de pintura mejorados
    private static void PaintCodePanelWithEnhancedEffects(Graphics graphics, Panel panel)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Fondo con gradiente sutil
        using (var brush = new LinearGradientBrush(
            panel.ClientRectangle,
            UIConstants.Colors.PanelBackground,
            Color.FromArgb(245, 248, 255),
            LinearGradientMode.Vertical))
        {
            graphics.FillRectangle(brush, panel.ClientRectangle);
        }

        // Borde redondeado
        var borderRect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
        using (var pen = new Pen(UIConstants.Colors.PanelBorder, 2))
        {
            graphics.DrawRoundedRectangle(pen, borderRect, 8);
        }

        // Sombra externa mejorada
        var shadowRect = new Rectangle(3, 3, panel.Width - 1, panel.Height - 1);
        using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
        {
            graphics.FillRoundedRectangle(shadowBrush, shadowRect, 8);
        }

        // Línea de números de línea (simulada)
        var lineNumberArea = new Rectangle(15, 30, 30, panel.Height - 35);
        using (var brush = new SolidBrush(Color.FromArgb(240, 243, 250)))
        {
            graphics.FillRectangle(brush, lineNumberArea);
        }

        using (var pen = new Pen(Color.FromArgb(220, 223, 230), 1))
        {
            graphics.DrawLine(pen, 45, 30, 45, panel.Height - 5);
        }
    }

    private static void PaintOutputPanelWithEnhancedEffects(Graphics graphics, Panel panel)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        // Fondo con gradiente oscuro
        using (var brush = new LinearGradientBrush(
            panel.ClientRectangle,
            UIConstants.Colors.OutputBackground,
            Color.FromArgb(35, 50, 70),
            LinearGradientMode.Vertical))
        {
            graphics.FillRectangle(brush, panel.ClientRectangle);
        }

        // Borde redondeado
        var borderRect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
        using (var pen = new Pen(Color.FromArgb(60, 80, 100), 2))
        {
            graphics.DrawRoundedRectangle(pen, borderRect, 8);
        }

        // Sombra externa
        var shadowRect = new Rectangle(3, 3, panel.Width - 1, panel.Height - 1);
        using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
        {
            graphics.FillRoundedRectangle(shadowBrush, shadowRect, 8);
        }
    }

    private static void PaintButtonWithGradient(Graphics graphics, Button button)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = new Rectangle(0, 0, button.Width, button.Height);

        // Gradiente en el botón
        using (var brush = new LinearGradientBrush(
            rect,
            button.BackColor,
            Color.FromArgb(
                Math.Min(255, button.BackColor.R + 20),
                Math.Min(255, button.BackColor.G + 20),
                Math.Min(255, button.BackColor.B + 20)
            ),
            LinearGradientMode.Vertical))
        {
            graphics.FillRoundedRectangle(brush, rect, 6);
        }

        // Borde sutil
        using (var pen = new Pen(Color.FromArgb(50, 255, 255, 255), 1))
        {
            graphics.DrawRoundedRectangle(pen, new Rectangle(0, 0, button.Width - 1, button.Height - 1), 6);
        }

        // Dibujar el texto del botón
        var textRect = new RectangleF(0, 0, button.Width, button.Height);
        var format = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        using (var textBrush = new SolidBrush(button.ForeColor))
        {
            graphics.DrawString(button.Text, button.Font, textBrush, textRect, format);
        }
    }

    // Métodos auxiliares
    private static Color InterpolateColor(Color color1, Color color2, float amount)
    {
        return Color.FromArgb(
            (int)(color1.R + (color2.R - color1.R) * amount),
            (int)(color1.G + (color2.G - color1.G) * amount),
            (int)(color1.B + (color2.B - color1.B) * amount)
        );
    }

    private static bool ColorsAreClose(Color color1, Color color2, int tolerance = 5)
    {
        return Math.Abs(color1.R - color2.R) <= tolerance &&
               Math.Abs(color1.G - color2.G) <= tolerance &&
               Math.Abs(color1.B - color2.B) <= tolerance;
    }

    // Método público para cambiar estado de botones
    public static void SetButtonState(Button button, ButtonState state)
    {
        var isCompileButton = button.Tag?.ToString() == "compile";
        var normalColor = isCompileButton
            ? UIConstants.Colors.CompileButton
            : UIConstants.Colors.ExecuteButton;

        switch (state)
        {
            case ButtonState.Success:
                button.BackColor = Color.FromArgb(39, 174, 96);
                button.Text = isCompileButton ? "✅ Compilado" : "✅ Ejecutado";
                // Volver al estado normal después de 2 segundos
                var successTimer = new System.Windows.Forms.Timer { Interval = 2000 };
                successTimer.Tick += (s, e) =>
                {
                    SetButtonState(button, ButtonState.Normal);
                    successTimer.Stop();
                    successTimer.Dispose();
                };
                successTimer.Start();
                break;

            case ButtonState.Error:
                button.BackColor = Color.FromArgb(231, 76, 60);
                button.Text = isCompileButton ? "❌ Error" : "❌ Error";
                // Volver al estado normal después de 3 segundos
                var errorTimer = new System.Windows.Forms.Timer { Interval = 3000 };
                errorTimer.Tick += (s, e) =>
                {
                    SetButtonState(button, ButtonState.Normal);
                    errorTimer.Stop();
                    errorTimer.Dispose();
                };
                errorTimer.Start();
                break;

            case ButtonState.Processing:
                button.BackColor = Color.FromArgb(243, 156, 18);
                button.Text = isCompileButton ? "🔄 Compilando..." : "🔄 Ejecutando...";
                break;

            case ButtonState.Normal:
                button.BackColor = normalColor;
                button.Text = isCompileButton
                    ? UIConstants.Text.COMPILE_BUTTON
                    : UIConstants.Text.EXECUTE_BUTTON;
                break;
        }

        ButtonStateChanged?.Invoke(button, state);
    }
}

// Extensiones para Graphics (bordes redondeados)
public static class GraphicsExtensions
{
    public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle rect, int radius)
    {
        using (var path = GetRoundedRectanglePath(rect, radius))
        {
            graphics.DrawPath(pen, path);
        }
    }

    public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rect, int radius)
    {
        using (var path = GetRoundedRectanglePath(rect, radius))
        {
            graphics.FillPath(brush, path);
        }
    }

    private static GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int diameter = radius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}
