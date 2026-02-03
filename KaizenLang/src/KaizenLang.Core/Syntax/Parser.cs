namespace ParadigmasLang;

public partial class Parser
{
    private List<Token>? _currentTokens;

    public Node Parse(List<Token> tokens)
    {
        ParadigmasLang.Logging.Logger.Debug($"Parser.Parse START - tokens: {tokens?.Count ?? 0}");
        _currentTokens = tokens; // Guardar para uso en ErrorNode
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
                if (pos == beforePos)
                {
                    throw new System.Exception($"Parser invariant violated: ParseStatement returned a node but did not advance pos (pos={pos}, token='{tokens[pos].Value}'). Please inspect the corresponding Parse* method.");
                }
            }
            else
            {
                root.Children.Add(ErrorNode($"Token inesperado fuera de lugar: '{tokens[pos].Value}'", pos));
                pos++;
            }
        }
        return root;
    }
}
