using KaizenLang.UI.Utils;

namespace KaizenLang.UI.Theme
{
    /// <summary>
    /// Contiene los valores para los elementos visuales.
    /// </summary>
    public class Theme
    {
        // Main colors
        public Color Background { get; set; }
        public Color Foreground { get; set; }

        // Secondary colors
        // Agrupación por clases según el tipo de componente

        // Colores para botones
        public Color ButtonBackground { get; set; }
        public Color ButtonForeground { get; set; }
        public Color ButtonBorder { get; set; }
        public Color ButtonMouseOver { get; set; }
        public Color ButtonMouseDown { get; set; }

        // Colores para cajas de texto
        public Color TextBoxBackground { get; set; }
        public Color TextBoxForeground { get; set; }
        public Color TextBoxBorder { get; set; }
        public Color TextBoxSelectionBackground { get; set; }
        public Color TextBoxSelectionForeground { get; set; }

        // Colores para listas y combos
        public Color ListBackground { get; set; }
        public Color ListForeground { get; set; }
        public Color ListSelectionBackground { get; set; }
        public Color ListSelectionForeground { get; set; }

        // Colores para etiquetas
        public Color LabelBackground { get; set; }
        public Color LabelForeground { get; set; }

        // Colores para paneles y groupbox
        public Color PanelBackground { get; set; }
        public Color PanelForeground { get; set; }

        // Colores para DataGridView
        public Color GridBackground { get; set; }
        public Color GridForeground { get; set; }
        public Color GridHeaderBackground { get; set; }
        public Color GridHeaderForeground { get; set; }
        public Color GridSelectionBackground { get; set; }
        public Color GridSelectionForeground { get; set; }
        public Color GridBorder { get; set; }

        // Colores para TabControl
        public Color TabBackground { get; set; }
        public Color TabForeground { get; set; }
        public Color TabPageBackground { get; set; }
        public Color TabPageForeground { get; set; }

        // Colores para MenuStrip y ToolStrip
        public Color MenuBackground { get; set; }
        public Color MenuForeground { get; set; }
        public Color ToolStripBackground { get; set; }
        public Color ToolStripForeground { get; set; }
        public Color SecondaryBackground { get; set; }
        public Color SecondaryForeground { get; set; }

        // Accent colors
        public Color PrimaryAccent { get; set; }
        public Color SecondaryAccent { get; set; }

        // Syntax highlighting colors
        public Color Keywords { get; set; }
        public Color Comments { get; set; }
        public Color Strings { get; set; }
        public Color Numbers { get; set; }
        public Color Identifiers { get; set; }
        public Color Functions { get; set; }
        public Color Operators { get; set; }

        // UI element colors
        public Color Selection { get; set; }
        public Color LineNumbers { get; set; }
        public Color CurrentLine { get; set; }
        public Color Error { get; set; }
        public Color Warning { get; set; }
        public Color Info { get; set; }

        // Border colors
        public Color Border { get; set; }
        public Color FocusBorder { get; set; }

        // Font settings
        public string? DefaultFontFamily { get; set; }
        public float DefaultFontSize { get; set; }

        public Theme()
        {
            WhiteTheme();
        }

        public void DarkTheme()
        {
            // VS Code Dark Modern theme colors
            // Main colors
            Background = Color.FromArgb(37, 37, 38); // editor.background
            Foreground = Color.FromArgb(212, 212, 212); // editor.foreground

            // Secondary colors
            SecondaryBackground = Color.FromArgb(30, 30, 30); // sideBar.background
            SecondaryForeground = Color.FromArgb(204, 204, 204); // sideBar.foreground

            // Accent colors
            PrimaryAccent = Color.FromArgb(0, 120, 212); // activityBarBadge.background
            SecondaryAccent = Color.FromArgb(97, 175, 239); // editorSuggestWidget.selectedBackground

            // Button colors
            ButtonBackground = Color.FromArgb(45, 45, 45);
            ButtonForeground = Color.FromArgb(212, 212, 212);
            ButtonBorder = Color.FromArgb(80, 80, 80);
            ButtonMouseOver = Color.FromArgb(55, 55, 55);
            ButtonMouseDown = Color.FromArgb(40, 40, 40);

            // TextBox colors
            TextBoxBackground = Color.FromArgb(30, 30, 30);
            TextBoxForeground = Color.FromArgb(212, 212, 212);
            TextBoxBorder = Color.FromArgb(80, 80, 80);
            TextBoxSelectionBackground = Color.FromArgb(38, 79, 120);
            TextBoxSelectionForeground = Color.FromArgb(255, 255, 255);

            // List colors
            ListBackground = Color.FromArgb(30, 30, 30);
            ListForeground = Color.FromArgb(212, 212, 212);
            ListSelectionBackground = Color.FromArgb(38, 79, 120);
            ListSelectionForeground = Color.FromArgb(255, 255, 255);

            // Label colors
            LabelBackground = Color.FromArgb(37, 37, 38);
            LabelForeground = Color.FromArgb(212, 212, 212);

            // Panel colors
            PanelBackground = Color.FromArgb(30, 30, 30);
            PanelForeground = Color.FromArgb(204, 204, 204);

            // DataGridView colors
            GridBackground = Color.FromArgb(37, 37, 38);
            GridForeground = Color.FromArgb(212, 212, 212);
            GridHeaderBackground = Color.FromArgb(45, 45, 45);
            GridHeaderForeground = Color.FromArgb(230, 230, 230);
            GridSelectionBackground = Color.FromArgb(38, 79, 120);
            GridSelectionForeground = Color.FromArgb(255, 255, 255);
            GridBorder = Color.FromArgb(80, 80, 80);

            // TabControl colors
            TabBackground = Color.FromArgb(45, 45, 45);
            TabForeground = Color.FromArgb(212, 212, 212);
            TabPageBackground = Color.FromArgb(37, 37, 38);
            TabPageForeground = Color.FromArgb(212, 212, 212);

            // Menu colors
            MenuBackground = Color.FromArgb(30, 30, 30);
            MenuForeground = Color.FromArgb(204, 204, 204);
            ToolStripBackground = Color.FromArgb(30, 30, 30);
            ToolStripForeground = Color.FromArgb(204, 204, 204);

            // Syntax highlighting colors
            Keywords = Color.FromArgb(86, 156, 214); // keyword
            Comments = Color.FromArgb(106, 153, 85); // comment
            Strings = Color.FromArgb(206, 145, 120); // string
            Numbers = Color.FromArgb(181, 206, 168); // number
            Identifiers = Color.FromArgb(212, 212, 212); // variable
            Functions = Color.FromArgb(220, 220, 170); // function
            Operators = Color.FromArgb(212, 212, 212); // operator

            // UI element colors
            Selection = Color.FromArgb(38, 79, 120); // editor.selectionBackground
            LineNumbers = Color.FromArgb(153, 153, 153); // editorLineNumber.foreground
            CurrentLine = Color.FromArgb(60, 60, 60); // editor.lineHighlightBackground
            Error = Color.FromArgb(255, 85, 85); // editorError.foreground
            Warning = Color.FromArgb(255, 192, 87); // editorWarning.foreground
            Info = Color.FromArgb(97, 175, 239); // editorInfo.foreground

            // Border colors
            Border = Color.FromArgb(80, 80, 80); // editorGroup.border
            FocusBorder = Color.FromArgb(0, 120, 212); // focusBorder

            // Font settings
            DefaultFontFamily = "Consolas";
            DefaultFontSize = 12.0f;
        }

        public void WhiteTheme()
        {
            // VS Code Light Modern theme colors (aproximado)
            // Main colors
            Background = Color.FromArgb(255, 255, 255); // editor.background
            Foreground = Color.FromArgb(30, 30, 30); // editor.foreground

            // Secondary colors
            SecondaryBackground = Color.FromArgb(245, 245, 245); // sideBar.background
            SecondaryForeground = Color.FromArgb(60, 60, 60); // sideBar.foreground

            // Accent colors
            PrimaryAccent = Color.FromArgb(0, 120, 212); // activityBarBadge.background
            SecondaryAccent = Color.FromArgb(0, 120, 212); // editorSuggestWidget.selectedBackground

            // Button colors
            ButtonBackground = Color.FromArgb(240, 240, 240);
            ButtonForeground = Color.FromArgb(30, 30, 30);
            ButtonBorder = Color.FromArgb(200, 200, 200);
            ButtonMouseOver = Color.FromArgb(230, 230, 230);
            ButtonMouseDown = Color.FromArgb(210, 210, 210);

            // TextBox colors
            TextBoxBackground = Color.FromArgb(255, 255, 255);
            TextBoxForeground = Color.FromArgb(30, 30, 30);
            TextBoxBorder = Color.FromArgb(200, 200, 200);
            TextBoxSelectionBackground = Color.FromArgb(0, 120, 212);
            TextBoxSelectionForeground = Color.FromArgb(255, 255, 255);

            // List colors
            ListBackground = Color.FromArgb(255, 255, 255);
            ListForeground = Color.FromArgb(30, 30, 30);
            ListSelectionBackground = Color.FromArgb(0, 120, 212);
            ListSelectionForeground = Color.FromArgb(255, 255, 255);

            // Label colors
            LabelBackground = Color.FromArgb(255, 255, 255);
            LabelForeground = Color.FromArgb(30, 30, 30);

            // Panel colors
            PanelBackground = Color.FromArgb(245, 245, 245);
            PanelForeground = Color.FromArgb(60, 60, 60);

            // DataGridView colors
            GridBackground = Color.FromArgb(255, 255, 255);
            GridForeground = Color.FromArgb(30, 30, 30);
            GridHeaderBackground = Color.FromArgb(240, 240, 240);
            GridHeaderForeground = Color.FromArgb(30, 30, 30);
            GridSelectionBackground = Color.FromArgb(0, 120, 212);
            GridSelectionForeground = Color.FromArgb(255, 255, 255);
            GridBorder = Color.FromArgb(200, 200, 200);

            // TabControl colors
            TabBackground = Color.FromArgb(240, 240, 240);
            TabForeground = Color.FromArgb(30, 30, 30);
            TabPageBackground = Color.FromArgb(255, 255, 255);
            TabPageForeground = Color.FromArgb(30, 30, 30);

            // Menu colors
            MenuBackground = Color.FromArgb(245, 245, 245);
            MenuForeground = Color.FromArgb(60, 60, 60);
            ToolStripBackground = Color.FromArgb(245, 245, 245);
            ToolStripForeground = Color.FromArgb(60, 60, 60);

            // Syntax highlighting colors
            Keywords = Color.FromArgb(0, 0, 255); // keyword
            Comments = Color.FromArgb(0, 128, 0); // comment
            Strings = Color.FromArgb(163, 21, 21); // string
            Numbers = Color.FromArgb(43, 145, 175); // number
            Identifiers = Color.FromArgb(30, 30, 30); // variable
            Functions = Color.FromArgb(111, 0, 138); // function
            Operators = Color.FromArgb(30, 30, 30); // operator

            // UI element colors
            Selection = Color.FromArgb(0, 120, 212); // editor.selectionBackground
            LineNumbers = Color.FromArgb(128, 128, 128); // editorLineNumber.foreground
            CurrentLine = Color.FromArgb(232, 242, 254); // editor.lineHighlightBackground
            Error = Color.FromArgb(255, 85, 85); // editorError.foreground
            Warning = Color.FromArgb(255, 192, 87); // editorWarning.foreground
            Info = Color.FromArgb(0, 120, 212); // editorInfo.foreground

            // Border colors
            Border = Color.FromArgb(200, 200, 200); // editorGroup.border
            FocusBorder = Color.FromArgb(0, 120, 212); // focusBorder

            // Font settings
            DefaultFontFamily = "Consolas";
            DefaultFontSize = 13.0f;
        }

        // Asignación de tema a un elemento según VS Code Dark Modern
        public void ApplyTo(Control control)
        {
            // Colores principales
            control.BackColor = Background;
            control.ForeColor = Foreground;
            control.Font = new Font(DefaultFontFamily ?? "Consolas", DefaultFontSize);

            // Botones
            if (control is ButtonBase button)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = ButtonBorder;
                button.BackColor = ButtonBackground;
                button.ForeColor = ButtonForeground;
                button.FlatAppearance.MouseOverBackColor = ButtonMouseOver;
                button.FlatAppearance.MouseDownBackColor = ButtonMouseDown;
            }

            // TextBox y RichTextBox
            else if (control is TextBoxBase textBox)
            {
                textBox.BackColor = TextBoxBackground;
                textBox.ForeColor = TextBoxForeground;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.Font = new Font(DefaultFontFamily ?? "Consolas", DefaultFontSize);

                if (textBox is RichTextBox richTextBox)
                {
                    richTextBox.SelectionBackColor = TextBoxSelectionBackground;
                    richTextBox.SelectionColor = TextBoxSelectionForeground;
                }
            }

            // ListBox, ComboBox, CheckedListBox
            else if (control is ListControl listControl)
            {
                listControl.BackColor = ListBackground;
                listControl.ForeColor = ListForeground;
                listControl.Font = new Font(DefaultFontFamily ?? "Consolas", DefaultFontSize);

                if (listControl is ComboBox comboBox)
                {
                    comboBox.FlatStyle = FlatStyle.Flat;
                }
            }

            // Label
            else if (control is Label label)
            {
                label.BackColor = LabelBackground;
                label.ForeColor = LabelForeground;
                label.Font = new Font(DefaultFontFamily ?? "Consolas", DefaultFontSize);
            }

            // Panel, GroupBox
            else if (control is Panel || control is GroupBox)
            {
                control.BackColor = PanelBackground;
                control.ForeColor = PanelForeground;
            }

            // DataGridView
            else if (control is DataGridView grid)
            {
                grid.BackgroundColor = GridBackground;
                grid.ForeColor = GridForeground;
                grid.GridColor = GridBorder;
                grid.ColumnHeadersDefaultCellStyle.BackColor = GridHeaderBackground;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = GridHeaderForeground;
                grid.DefaultCellStyle.BackColor = GridBackground;
                grid.DefaultCellStyle.ForeColor = GridForeground;
                grid.DefaultCellStyle.SelectionBackColor = GridSelectionBackground;
                grid.DefaultCellStyle.SelectionForeColor = GridSelectionForeground;
                grid.EnableHeadersVisualStyles = false;
            }

            // TabControl
            else if (control is TabControl tabControl)
            {
                tabControl.BackColor = TabBackground;
                tabControl.ForeColor = TabForeground;
                foreach (TabPage page in tabControl.TabPages)
                {
                    page.BackColor = TabPageBackground;
                    page.ForeColor = TabPageForeground;
                }
            }

            // MenuStrip, ToolStrip
            else if (control is MenuStrip menuStrip)
            {
                menuStrip.BackColor = MenuBackground;
                menuStrip.ForeColor = MenuForeground;
                menuStrip.RenderMode = ToolStripRenderMode.Professional;
            }
            else if (control is ToolStrip toolStrip)
            {
                toolStrip.BackColor = ToolStripBackground;
                toolStrip.ForeColor = ToolStripForeground;
                toolStrip.RenderMode = ToolStripRenderMode.Professional;
            }

            // Recursivo para los hijos
            foreach (Control child in control.Controls)
            {
                ApplyTo(child);
            }
        }
    }
}


