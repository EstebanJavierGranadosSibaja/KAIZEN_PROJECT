namespace ParadigmasLang
{
    public static class DelimiterWords
    {
        // Constantes para cada delimitador
        public const string PAREN_OPEN = "(";
        public const string PAREN_CLOSE = ")";
        public const string SEMICOLON = ";";
        public const string COMMA = ",";
        public const string BRACKET_OPEN = "[";
        public const string BRACKET_CLOSE = "]";
        public const string BLOCK_START = "ying";
        public const string BLOCK_END = "yang";

        // Delimitadores de un solo carácter
        public static readonly HashSet<string> SingleCharDelimiters = new HashSet<string>
        {
            PAREN_OPEN, PAREN_CLOSE, SEMICOLON, COMMA, BRACKET_OPEN, BRACKET_CLOSE
        };

        // Delimitadores de múltiples caracteres
        public static readonly HashSet<string> MultiCharDelimiters = new HashSet<string>
        {
            BLOCK_START, BLOCK_END
        };

        // Todos los delimitadores (para compatibilidad con código existente)
        public static readonly HashSet<string> Words = new HashSet<string>(
            SingleCharDelimiters.Concat(MultiCharDelimiters)
        );
    }
}
