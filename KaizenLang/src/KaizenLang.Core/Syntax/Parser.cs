namespace ParadigmasLang;

// Analizador sintáctico: convierte tokens en un árbol de sintaxis
public partial class Parser
{
    public Node Parse(List<Token> tokens)
    {
    ParadigmasLang.Logging.Logger.Debug($"Parser.Parse START - tokens: {tokens?.Count ?? 0}");
        int pos = 0;
        Node root = new Node { Type = "Program" };
        if (tokens == null || tokens.Count == 0)
            return root;

        root.Line = tokens[0].Line;
        root.Column = tokens[0].Column;
        while (pos < tokens.Count)
        {
            ParadigmasLang.Logging.Logger.Debug($"Parser.Parse loop pos={pos} token='{tokens[pos].Value}' ({tokens[pos].Type})");
            int beforePos = pos;
            var node = ParseStatement(tokens, ref pos);
            if (node != null)
            {
                root.Children.Add(node);
                // Sanity check: ParseStatement should consume tokens (advance pos). If it doesn't,
                // we surface an exception so the parser implementation can be corrected instead
                // of silently forcing progress.
                if (pos == beforePos)
                {
                    throw new System.Exception($"Parser invariant violated: ParseStatement returned a node but did not advance pos (pos={pos}, token='{tokens[pos].Value}'). Please inspect the corresponding Parse* method.");
                }
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
