namespace ParadigmasLang
{
    // Analizador sintáctico: convierte tokens en un árbol de sintaxis
    public partial class Parser
    {
        public Node Parse(List<Token> tokens)
        {
            int pos = 0;
            Node root = new Node { Type = "Program" };
            while (pos < tokens.Count)
            {
                var node = ParseStatement(tokens, ref pos);
                if (node != null)
                {
                    root.Children.Add(node);
                }
                else
                {
                    // Si no se pudo analizar una sentencia, es un token inesperado.
                    root.Children.Add(ErrorNode($"Token inesperado fuera de lugar: '{tokens[pos].Value}'", pos));
                    pos++; // Avanzar para evitar un bucle infinito.
                }
            }
            return root;
        }
    }
}
