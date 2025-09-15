namespace KaizenLang.UI
{
    public static class UIConstants
    {
        // Dimensiones de la ventana principal
        public const int MAIN_WINDOW_WIDTH = 1400;
        public const int MAIN_WINDOW_HEIGHT = 900;

        // Dimensiones de paneles
        public const int CODE_PANEL_WIDTH = 1200;
        public const int CODE_PANEL_HEIGHT = 400;
        public const int OUTPUT_PANEL_WIDTH = 1200;
        public const int OUTPUT_PANEL_HEIGHT = 250;

        // Posiciones
        public const int MENU_HEIGHT = 40;
        public const int PANEL_TOP_MARGIN = 60;
        public const int PANEL_LEFT_MARGIN = 80;
        public const int BUTTON_TOP_OFFSET = 30;
        public const int BUTTON_SPACING = 40;
        public const int OUTPUT_TOP_OFFSET = 40;

        // Dimensiones de botones
        public const int BUTTON_WIDTH = 180;
        public const int BUTTON_HEIGHT = 50;

        // Padding
        public const int PANEL_PADDING = 20;
        public const int CONTROL_MARGIN = 10;

        // Colores
        public static class Colors
        {
            public static readonly Color MainBackground = Color.FromArgb(240, 243, 250);
            public static readonly Color MenuBackground = Color.FromArgb(255, 255, 255);
            public static readonly Color MenuForeground = Color.FromArgb(44, 62, 80);
            public static readonly Color PanelBackground = Color.White;
            public static readonly Color PanelBorder = Color.FromArgb(200, 200, 220);
            public static readonly Color CodeBackground = Color.FromArgb(250, 250, 255);
            public static readonly Color CodeForeground = Color.FromArgb(44, 62, 80);
            public static readonly Color OutputBackground = Color.FromArgb(44, 62, 80);
            public static readonly Color OutputForeground = Color.White;
            public static readonly Color CompileButton = Color.FromArgb(52, 152, 219);
            public static readonly Color CompileButtonHover = Color.FromArgb(25, 90, 160);
            public static readonly Color ExecuteButton = Color.FromArgb(39, 174, 96);
            public static readonly Color ExecuteButtonHover = Color.FromArgb(30, 132, 73);
            public static readonly Color ButtonText = Color.White;
            public static readonly Color Shadow = Color.FromArgb(30, 44, 62, 80);
            public static readonly Color ShadowDark = Color.FromArgb(60, 44, 62, 80);
        }

        // Fuentes
        public static class Fonts
        {
            public static readonly Font MenuFont = new Font("Segoe UI", 13, FontStyle.Bold);
            public static readonly Font CodeFont = new Font("Consolas", 12); // Fuente monospace más segura
            public static readonly Font OutputFont = new Font("Consolas", 11); // Fuente monospace más segura
            public static readonly Font ButtonFont = new Font("Segoe UI", 14, FontStyle.Bold);
        }

        // Textos de la interfaz
        public static class Text
        {
            public const string WINDOW_TITLE = "KaizenLang IDE";
            public const string MENU_STRUCTURES = "☰ Estructuras del Lenguaje";
            public const string MENU_RESERVED_WORDS = "Palabras reservadas";
            public const string MENU_SYNTAX = "Sintaxis";
            public const string MENU_CONTROL = "Control";
            public const string MENU_FUNCTIONS = "Funciones";
            public const string MENU_OPERATIONS = "Operaciones";
            public const string MENU_SEMANTICS = "Semántica";
            public const string MENU_DATA_TYPES = "Tipos de datos";
            public const string COMPILE_BUTTON = "🛠 Compilar";
            public const string EXECUTE_BUTTON = "▶ Ejecutar";
        }
    }
}
