namespace KaizenLang.UI;

public static class MenuBuilder
{
    public static MenuStrip CreateMainMenu(TextBox codeBox)
    {
        var menuStrip = new MenuStrip
        {
            BackColor = UIConstants.Colors.MenuBackground,
            Font = UIConstants.Fonts.MenuFont,
            Renderer = new ToolStripProfessionalRenderer(new ThemeRenderer.ThemeColorTable()),
            ForeColor = UIConstants.Colors.MenuForeground,
            Height = UIConstants.MENU_HEIGHT
        };

        var estructurasMenu = CreateStructuresMenu(codeBox);
        menuStrip.Items.Add(estructurasMenu);

        return menuStrip;
    }

    private static ToolStripMenuItem CreateStructuresMenu(TextBox codeBox)
    {
        var estructurasMenu = new ToolStripMenuItem(UIConstants.Text.MENU_STRUCTURES)
        {
            ForeColor = UIConstants.Colors.MenuForeground,
            Font = UIConstants.Fonts.MenuFont
        };

        // Palabras reservadas
        var palabrasReservadas = new ToolStripMenuItem(
            UIConstants.Text.MENU_RESERVED_WORDS,
            null,
            (s, e) => InsertText(codeBox, MenuTexts.RESERVED_WORDS_TEXT)
        );

        // Sintaxis
        var sintaxisMenu = CreateSyntaxMenu(codeBox);

        // Semántica
        var semanticaMenu = new ToolStripMenuItem(
            UIConstants.Text.MENU_SEMANTICS,
            null,
            (s, e) => InsertText(codeBox, MenuTexts.SEMANTICS_TEXT)
        );

        // Tipos de datos
        var tiposDatosMenu = new ToolStripMenuItem(
            UIConstants.Text.MENU_DATA_TYPES,
            null,
            (s, e) => InsertText(codeBox, MenuTexts.DATA_TYPES_TEXT)
        );

        estructurasMenu.DropDownItems.Add(palabrasReservadas);
        estructurasMenu.DropDownItems.Add(sintaxisMenu);
        estructurasMenu.DropDownItems.Add(semanticaMenu);
        estructurasMenu.DropDownItems.Add(tiposDatosMenu);

        return estructurasMenu;
    }

    // Create a topbar panel (logo + pill-like menu buttons)
    public static Panel CreateTopBar(TextBox codeBox)
    {
        var topBar = new Panel
        {
            Height = UIConstants.MENU_HEIGHT + 10,
            BackColor = UIConstants.Colors.MenuBackground,
            Dock = DockStyle.Top
        };

        // Logo square
        var logo = new Label
        {
            Text = "K",
            Width = 36,
            Height = 36,
            Left = 10,
            Top = 8,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = UIConstants.Colors.CompileButton,
            ForeColor = UIConstants.Colors.ButtonText,
            Font = new Font(UIConstants.Fonts.MenuFont.FontFamily, 12, FontStyle.Bold),
        };
        logo.BorderStyle = BorderStyle.None;
        logo.Padding = new Padding(6);
        topBar.Controls.Add(logo);

        // Estructuras dropdown button (shows a menu to insert templates into the editor)
        int x = logo.Right + 10;
        var estructurasBtn = ControlFactory.CreateTopBarButton(UIConstants.Text.MENU_STRUCTURES, x, 10);
        var estructurasMenu = BuildStructuresContextMenu(codeBox);
        estructurasBtn.Click += (s, e) => estructurasMenu.Show(estructurasBtn, new System.Drawing.Point(0, estructurasBtn.Height));
        topBar.Controls.Add(estructurasBtn);

        // Keep only Ayuda button on the right side
        var ayudaBtn = ControlFactory.CreateTopBarButton("Ayuda", topBar.Width - 110, 10);
        ayudaBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        topBar.Controls.Add(ayudaBtn);

        // Right side: compile/run buttons placed by MainForm later when needed

        return topBar;
    }

    private static ToolStripMenuItem CreateSyntaxMenu(TextBox codeBox)
    {
        var sintaxisMenu = new ToolStripMenuItem(UIConstants.Text.MENU_SYNTAX)
        {
            ForeColor = UIConstants.Colors.MenuForeground,
            Font = UIConstants.Fonts.MenuFont
        };

        sintaxisMenu.DropDownItems.Add(new ToolStripMenuItem(
            UIConstants.Text.MENU_CONTROL,
            null,
            (s, e) => InsertText(codeBox, MenuTexts.CONTROL_STRUCTURES_TEXT)
        ));

        sintaxisMenu.DropDownItems.Add(new ToolStripMenuItem(
            UIConstants.Text.MENU_FUNCTIONS,
            null,
            (s, e) => InsertText(codeBox, MenuTexts.FUNCTIONS_TEXT)
        ));

        sintaxisMenu.DropDownItems.Add(new ToolStripMenuItem(
            UIConstants.Text.MENU_OPERATIONS,
            null,
            (s, e) => InsertText(codeBox, MenuTexts.OPERATIONS_TEXT)
        ));

        return sintaxisMenu;
    }

    private static void InsertText(TextBox textBox, string text)
    {
        textBox.SelectedText = text;
    }

    // Build a ContextMenuStrip for the topbar 'Estructuras' button
    private static ContextMenuStrip BuildStructuresContextMenu(TextBox codeBox)
    {
        var cms = new ContextMenuStrip
        {
            BackColor = UIConstants.Colors.MenuBackground,
            ForeColor = UIConstants.Colors.MenuForeground,
            Font = UIConstants.Fonts.MenuFont
        };

        // Palabras reservadas
        cms.Items.Add(new ToolStripMenuItem(UIConstants.Text.MENU_RESERVED_WORDS, null, (s, e) => InsertText(codeBox, MenuTexts.RESERVED_WORDS_TEXT)));

        // Sintaxis -> submenu (control, functions, operations)
        var sintaxis = new ToolStripMenuItem(UIConstants.Text.MENU_SYNTAX);
        sintaxis.DropDownItems.Add(new ToolStripMenuItem(UIConstants.Text.MENU_CONTROL, null, (s, e) => InsertText(codeBox, MenuTexts.CONTROL_STRUCTURES_TEXT)));
        sintaxis.DropDownItems.Add(new ToolStripMenuItem(UIConstants.Text.MENU_FUNCTIONS, null, (s, e) => InsertText(codeBox, MenuTexts.FUNCTIONS_TEXT)));
        sintaxis.DropDownItems.Add(new ToolStripMenuItem(UIConstants.Text.MENU_OPERATIONS, null, (s, e) => InsertText(codeBox, MenuTexts.OPERATIONS_TEXT)));
        cms.Items.Add(sintaxis);

        // Semántica
        cms.Items.Add(new ToolStripMenuItem(UIConstants.Text.MENU_SEMANTICS, null, (s, e) => InsertText(codeBox, MenuTexts.SEMANTICS_TEXT)));

        // Tipos de datos
        cms.Items.Add(new ToolStripMenuItem(UIConstants.Text.MENU_DATA_TYPES, null, (s, e) => InsertText(codeBox, MenuTexts.DATA_TYPES_TEXT)));

        return cms;
    }
}

public static class MenuTexts
{
    public const string RESERVED_WORDS_TEXT =
        @"// Palabras reservadas de KaizenLang:
            output
            input
            void
            do
            while
            for
            if
            else
            return
            true
            false
            null
            ";

    public const string CONTROL_STRUCTURES_TEXT =
        @"// Estructuras de control:
            if (condicion) {
                // código si verdadero
            } else {
                // código si falso
            }

            while (condicion) {
                // código del bucle
            }

            for (int i = 0; i < 10; i++) {
                // código del bucle
            }
            ";

    public const string FUNCTIONS_TEXT =
        @"// Declaración de funciones:
            int suma(int a, int b) {
                return a + b;
            }

            void saludar(string nombre) {
                output(""Hola "" + nombre);
            }
            ";

    public const string OPERATIONS_TEXT =
        @"// Operaciones aritméticas:
            int x = 10 + 5;
            int y = x * 2;
            int z = y / 3;

            // Operaciones lógicas:
            boolean resultado = (x > y) && (z < 10);
            ";

    public const string SEMANTICS_TEXT =
        @"// Semántica de KaizenLang:
            // - Tipado estricto obligatorio
            // - Variables deben declararse antes de usarse
            // - No conversiones implícitas peligrosas
            // - Validación de compatibilidad de tipos

            int numero = 42;  // ✓ Correcto
            // numero = ""texto"";  // ❌ Error: tipos incompatibles
            ";

    public const string DATA_TYPES_TEXT =
        @"// Tipos de datos simples:
            int entero = 42;
            float decimal = 3.14;
            double precision = 3.141592653589793;
            boolean logico = true;
            char caracter = 'A';
            string texto = ""Hola mundo"";

            // Tipos de datos compuestos:
            array numeros = [1, 2, 3, 4, 5];
            string lista = ""elemento1,elemento2,elemento3"";
            ";
}
