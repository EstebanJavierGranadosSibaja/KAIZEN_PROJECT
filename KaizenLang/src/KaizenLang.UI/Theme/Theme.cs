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
            // Kaizen specific theme colors
            // Main colors
            Background = ColorTranslator.FromHtml("#0B0F14");
            Foreground = ColorTranslator.FromHtml("#E6E9ED");

            // Secondary colors
            SecondaryBackground = ColorTranslator.FromHtml("#12181E");
            SecondaryForeground = ColorTranslator.FromHtml("#AAB0B8");

            // Accent colors
            PrimaryAccent = ColorTranslator.FromHtml("#28C76F");
            SecondaryAccent = ColorTranslator.FromHtml("#D49B2D");

            // Button colors
            ButtonBackground = ColorTranslator.FromHtml("#181E25");
            ButtonForeground = ColorTranslator.FromHtml("#E6E9ED");
            ButtonBorder = ColorTranslator.FromHtml("#252C36");
            ButtonMouseOver = ColorTranslator.FromHtml("#1F262E");
            ButtonMouseDown = ColorTranslator.FromHtml("#166C30");

            // TextBox colors
            TextBoxBackground = ColorTranslator.FromHtml("#0E1216");
            TextBoxForeground = ColorTranslator.FromHtml("#E6E9ED");
            TextBoxBorder = ColorTranslator.FromHtml("#252C36");
            TextBoxSelectionBackground = ColorTranslator.FromHtml("#166C30");
            TextBoxSelectionForeground = Color.White;

            // List colors
            ListBackground = ColorTranslator.FromHtml("#12181E");
            ListForeground = ColorTranslator.FromHtml("#E6E9ED");
            ListSelectionBackground = ColorTranslator.FromHtml("#166C30");
            ListSelectionForeground = Color.White;

            // Label colors
            LabelBackground = Color.Transparent;
            LabelForeground = ColorTranslator.FromHtml("#E6E9ED");

            // Panel colors
            PanelBackground = ColorTranslator.FromHtml("#12181E");
            PanelForeground = ColorTranslator.FromHtml("#E6E9ED");

            // DataGridView colors
            GridBackground = ColorTranslator.FromHtml("#0B0F14");
            GridForeground = ColorTranslator.FromHtml("#E6E9ED");
            GridHeaderBackground = ColorTranslator.FromHtml("#181E25");
            GridHeaderForeground = ColorTranslator.FromHtml("#E6E9ED");
            GridSelectionBackground = ColorTranslator.FromHtml("#166C30");
            GridSelectionForeground = Color.White;
            GridBorder = ColorTranslator.FromHtml("#252C36");

            // TabControl colors
            TabBackground = ColorTranslator.FromHtml("#181E25");
            TabForeground = ColorTranslator.FromHtml("#E6E9ED");
            TabPageBackground = ColorTranslator.FromHtml("#0B0F14");
            TabPageForeground = ColorTranslator.FromHtml("#E6E9ED");

            // Menu colors
            MenuBackground = ColorTranslator.FromHtml("#0E1216");
            MenuForeground = ColorTranslator.FromHtml("#AAB0B8");
            ToolStripBackground = ColorTranslator.FromHtml("#0E1216");
            ToolStripForeground = ColorTranslator.FromHtml("#AAB0B8");

            // Syntax highlighting colors (example mapping)
            Keywords = ColorTranslator.FromHtml("#1FA044"); // Green
            Comments = ColorTranslator.FromHtml("#AAB0B8"); // Muted
            Strings = ColorTranslator.FromHtml("#D49B2D"); // Gold
            Numbers = ColorTranslator.FromHtml("#28C76F"); // Highlight
            Identifiers = ColorTranslator.FromHtml("#E6E9ED"); // Text
            Functions = ColorTranslator.FromHtml("#D49B2D"); // Gold
            Operators = ColorTranslator.FromHtml("#AAB0B8"); // Muted

            // UI element colors
            Selection = ColorTranslator.FromHtml("#166C30"); // GreenDark
            LineNumbers = ColorTranslator.FromHtml("#AAB0B8"); // Muted
            CurrentLine = ColorTranslator.FromHtml("#181E25"); // Surface2
            Error = Color.Firebrick;
            Warning = ColorTranslator.FromHtml("#D49B2D"); // Gold
            Info = Color.DodgerBlue;

            // Border colors
            Border = ColorTranslator.FromHtml("#252C36");
            FocusBorder = ColorTranslator.FromHtml("#28C76F"); // Highlight

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


