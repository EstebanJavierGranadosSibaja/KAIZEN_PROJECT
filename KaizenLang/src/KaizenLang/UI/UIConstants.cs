namespace KaizenLang.UI;

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
        // Dark theme tokens sourced from src/KaizenLang/UI/theme.css
        public static readonly Color MainBackground = ColorTranslator.FromHtml("#0B0F14"); // --kaizen-bg
        public static readonly Color PanelBackground = ColorTranslator.FromHtml("#12181E"); // --kaizen-surface
        public static readonly Color MenuBackground = ColorTranslator.FromHtml("#181E25"); // --kaizen-surface-2
        public static readonly Color CodeBackground = ColorTranslator.FromHtml("#0E1216"); // --kaizen-surface-3
        public static readonly Color CodeForeground = ColorTranslator.FromHtml("#E6E9ED"); // --kaizen-text
        public static readonly Color MenuForeground = ColorTranslator.FromHtml("#AAB0B8"); // --kaizen-muted (used for menu text)
        public static readonly Color PanelBorder = ColorTranslator.FromHtml("#252C36"); // --kaizen-border
        public static readonly Color OutputBackground = ColorTranslator.FromHtml("#0E1319");
        public static readonly Color OutputForeground = ColorTranslator.FromHtml("#AAB0B8"); // muted
                                                                                             // KAIZEN accents (green + gold)
        public static readonly Color CompileButton = ColorTranslator.FromHtml("#1FA044"); // --kaizen-green
        public static readonly Color CompileButtonHover = ColorTranslator.FromHtml("#166C30"); // --kaizen-green-dark
        public static readonly Color ExecuteButton = ColorTranslator.FromHtml("#D49B2D"); // --kaizen-gold
        public static readonly Color ExecuteButtonHover = ColorTranslator.FromHtml("#9E6F1F"); // --kaizen-gold-dark
        public static readonly Color ButtonText = Color.White;
        public static readonly Color Shadow = Color.FromArgb(160, 0, 0, 0);
        public static readonly Color ShadowDark = Color.FromArgb(220, 0, 0, 0);
    }

    // Fuentes
    public static class Fonts
    {
        public static readonly Font MenuFont = new Font("Segoe UI", 12, FontStyle.Bold);
        public static readonly Font CodeFont = new Font("Consolas", 12); // Fuente monospace más segura
        public static readonly Font OutputFont = new Font("Consolas", 11); // Fuente monospace más segura
        public static readonly Font ButtonFont = new Font("Segoe UI", 12, FontStyle.Bold);
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
