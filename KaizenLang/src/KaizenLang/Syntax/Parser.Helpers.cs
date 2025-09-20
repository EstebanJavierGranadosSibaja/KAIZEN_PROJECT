namespace ParadigmasLang
{
    public partial class Parser
    {
        // Utilidad para crear nodos de error con posición (siempre incluir posición approximation)
        private Node ErrorNode(string mensaje, int pos)
        {
            var n = new Node { Type = "Error", Line = 0, Column = pos };
            n.Children.Add(new Node { Type = $"{mensaje} (posición {pos})" });
            return n;
        }

        private bool Match(List<Token> tokens, int pos, string type, string value)
        {
            return pos < tokens.Count && tokens[pos].Type == type && tokens[pos].Value == value;
        }

        private bool IsFunctionDeclaration(List<Token> tokens, int pos)
        {
            // tipo nombre ( ... ) { ... }
            return pos + 3 < tokens.Count && tokens[pos].Type == "TYPE" && tokens[pos + 1].Type == "IDENTIFIER" && tokens[pos + 2].Type == "DELIMITER" && tokens[pos + 2].Value == "(";
        }

        private bool IsAssignment(List<Token> tokens, int pos)
        {
            return pos + 2 < tokens.Count &&
                   tokens[pos].Type == "IDENTIFIER" &&
                   tokens[pos + 1].Type == "OPERATOR" &&
                   tokens[pos + 1].Value == "=";
        }

        private bool IsVariableDeclaration(List<Token> tokens, int pos)
        {
            // Accept declarations of the form:
            //  TYPE IDENTIFIER
            //  array < TYPE > IDENTIFIER
            //  matrix < TYPE > IDENTIFIER
            if (pos + 1 >= tokens.Count) return false;

            // simple base-type declaration: TYPE name
            if (tokens[pos].Type == "TYPE" && tokens[pos + 1].Type == "IDENTIFIER") return true;

            // composite types like array<integer> or matrix<integer> are tokenized as:
            // IDENTIFIER('array'|'matrix') DELIMITER('<') TYPE DELIMITER('>') IDENTIFIER
            if (tokens[pos].Type == "IDENTIFIER" && (tokens[pos].Value == "array" || tokens[pos].Value == "matrix"))
            {
                if (pos + 4 < tokens.Count
                    && (tokens[pos + 1].Type == "DELIMITER" || tokens[pos + 1].Type == "OPERATOR") && tokens[pos + 1].Value == DelimiterWords.ANGLE_OPEN
                    && tokens[pos + 2].Type == "TYPE"
                    && (tokens[pos + 3].Type == "DELIMITER" || tokens[pos + 3].Type == "OPERATOR") && tokens[pos + 3].Value == DelimiterWords.ANGLE_CLOSE
                    && tokens[pos + 4].Type == "IDENTIFIER")
                {
                    return true;
                }
            }

            return false;
        }

        // Buscar si una expresión termina en ; (útil para decidir si es ExpressionStatement)
        private bool IsExpressionStatement(List<Token> tokens, int pos)
        {
            if (pos >= tokens.Count) return false;
            int tempPos = pos;
            int parenLevel = 0;
            while (tempPos < tokens.Count)
            {
                if (tokens[tempPos].Type == "DELIMITER")
                {
                    if (tokens[tempPos].Value == DelimiterWords.PAREN_OPEN) parenLevel++;
                    else if (tokens[tempPos].Value == DelimiterWords.PAREN_CLOSE) parenLevel--;
                    else if (tokens[tempPos].Value == DelimiterWords.SEMICOLON && parenLevel == 0) return true;
                    else if (tokens[tempPos].Value == DelimiterWords.BLOCK_START && parenLevel == 0) return false;
                }
                tempPos++;
            }
            return false;
        }

        // Validación y construcción de parámetros de función
        private Node ParseParams(List<Token> tokens, ref int pos)
        {
            var parameters = new Node("Parameters");
            while (tokens[pos].Type != "DELIMITER" || tokens[pos].Value != DelimiterWords.PAREN_CLOSE)
            {
                // support either base TYPE or composite 'array<type>' / 'matrix<type>'
                if (tokens[pos].Type == "TYPE" || (tokens[pos].Type == "IDENTIFIER" && (tokens[pos].Value == "array" || tokens[pos].Value == "matrix")))
                {
                    Node typeNode = new Node("Unknown");

                    if (tokens[pos].Type == "TYPE")
                    {
                        var typeToken = tokens[pos];
                        typeNode = new Node(typeToken.Value) { Line = typeToken.Line, Column = typeToken.Column };
                        pos++;
                    }
                    else
                    {
                        // composite type starting with 'array' or 'matrix'
                        var wrapper = tokens[pos];
                        typeNode = new Node(wrapper.Value) { Line = wrapper.Line, Column = wrapper.Column };
                        pos++; // consume 'array' or 'matrix'
                        if (!(pos < tokens.Count && (tokens[pos].Type == "DELIMITER" || tokens[pos].Type == "OPERATOR") && tokens[pos].Value == DelimiterWords.ANGLE_OPEN))
                        {
                            parameters.Children.Add(ErrorNode("Se esperaba '<' en tipo compuesto", pos));
                            break;
                        }
                        pos++; // consume '<'
                        if (!(pos < tokens.Count && tokens[pos].Type == "TYPE"))
                        {
                            parameters.Children.Add(ErrorNode("Se esperaba tipo base dentro de '<>'", pos));
                            break;
                        }
                        var inner = new Node(tokens[pos].Value) { Line = tokens[pos].Line, Column = tokens[pos].Column };
                        typeNode.Children.Add(inner);
                        pos++; // consume inner TYPE
                        if (!(pos < tokens.Count && (tokens[pos].Type == "DELIMITER" || tokens[pos].Type == "OPERATOR") && tokens[pos].Value == DelimiterWords.ANGLE_CLOSE))
                        {
                            parameters.Children.Add(ErrorNode("Se esperaba '>' en tipo compuesto", pos));
                            break;
                        }
                        pos++; // consume '>'
                    }

                    if (pos < tokens.Count && tokens[pos].Type == "IDENTIFIER")
                    {
                        var nameToken = tokens[pos];
                        var nameNode = new Node("Identifier", new List<Node> { new Node(nameToken.Value) }) { Line = nameToken.Line, Column = nameToken.Column };
                        var paramNode = new Node("Param", new List<Node> { typeNode, nameNode }) { Line = typeNode.Line, Column = typeNode.Column };
                        parameters.Children.Add(paramNode);
                        pos++;
                        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.COMMA)
                            pos++;
                    }
                    else
                    {
                        parameters.Children.Add(ErrorNode("Falta nombre de parámetro", pos));
                        break;
                    }
                }
                else if (tokens[pos].Type == "IDENTIFIER")
                {
                    parameters.Children.Add(new Node("Param", new List<Node> { new Node("Identifier", new List<Node> { new Node(tokens[pos].Type) }) }));
                    pos++;
                }
                else if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.COMMA)
                {
                    return ErrorNode("Coma inesperada o parámetro faltante.", pos);
                }
                else
                {
                    parameters.Children.Add(ErrorNode($"Token inesperado '{tokens[pos].Value}' en parámetros", pos));
                    pos++;
                    break;
                }
            }
            return parameters;
        }
    }
}
