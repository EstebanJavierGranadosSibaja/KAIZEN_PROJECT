namespace ParadigmasLang;

public partial class Parser
{
    private Node? ParseStatement(List<Token> tokens, ref int pos)
    {
        if (Match(tokens, pos, "RESERVED", ReservedWords.IF))
            return ParseIf(tokens, ref pos);
        if (Match(tokens, pos, "RESERVED", ReservedWords.FOR))
            return ParseFor(tokens, ref pos);
        if (Match(tokens, pos, "RESERVED", ReservedWords.WHILE))
            return ParseWhile(tokens, ref pos);
        if (Match(tokens, pos, "RESERVED", ReservedWords.DO))
            return ParseDoWhile(tokens, ref pos);
        if (Match(tokens, pos, "RESERVED", ReservedWords.RETURN))
            return ParseReturn(tokens, ref pos);
        if (IsFunctionDeclaration(tokens, pos))
            return ParseFunction(tokens, ref pos);

        // Declaración de variable
        if (IsVariableDeclaration(tokens, pos))
            return ParseVariableDeclaration(tokens, ref pos);

        // Asignación de variable existente: IDENTIFIER = valor
        if (IsAssignment(tokens, pos))
            return ParseAssignment(tokens, ref pos);

        // Expresión o llamada a función que termina en ;
        if (IsExpressionStatement(tokens, pos))
            return ParseExpressionStatement(tokens, ref pos);

        // Caso para manejar errores de tokens inesperados que no forman una sentencia válida
        if (tokens.Count > pos && tokens[pos].Type == "TYPE")
        {
            // Esto podría ser una declaración mal formada, como "int;"
            if (pos + 1 < tokens.Count && tokens[pos + 1].Value == DelimiterWords.SEMICOLON)
            {
                var error = ErrorNode($"Declaración de variable incompleta. Falta el nombre de la variable.", pos);
                pos += 2; // Consumir 'tipo' y ';' para evitar bucle infinito
                return error;
            }
        }

        return null; // No se pudo identificar la sentencia
    }

    // Nueva función para manejar return
    private Node ParseReturn(List<Token> tokens, ref int pos)
    {
        var node = new Node("Return");
        pos++; // Consumir 'return'

        // Si hay una expresión de retorno
        if (pos < tokens.Count && !(tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON))
        {
            var expr = ParseExpression(tokens, ref pos);
            node.Children.Add(expr);
        }

        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON)
            pos++;
        else
            return ErrorNode("Se esperaba ';' después de la sentencia return.", pos);

        return node;
    }

    // Nueva función para parsear expresiones que son sentencias
    private Node ParseExpressionStatement(List<Token> tokens, ref int pos)
    {
        var exprNode = ParseExpression(tokens, ref pos);
        var stmtNode = new Node("ExpressionStatement");
        stmtNode.Children.Add(exprNode);

        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON)
            pos++;
        else
            return ErrorNode("Se esperaba ';' después de la expresión.", pos);

        return stmtNode;
    }


    private Node ParseAssignment(List<Token> tokens, ref int pos)
    {
        var varToken = tokens[pos];
        var varNode = new Node("Identifier", new List<Node> { new Node(varToken.Value) }) { Line = varToken.Line, Column = varToken.Column };
        pos++; // Consumir identificador
        pos++; // Consumir '='
        var valueNode = ParseExpression(tokens, ref pos);
        var assignmentNode = new Node("Assignment", new List<Node> { varNode, valueNode }) { Line = varNode.Line, Column = varNode.Column };

        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON)
            pos++;
        else
            return ErrorNode("Se esperaba ';' después de la asignación.", pos);

        return assignmentNode;
    }

    private Node ParseVariableDeclaration(List<Token> tokens, ref int pos)
    {
        // Support both simple and composite types for variable declarations:
        // - TYPE IDENTIFIER
        // - array < TYPE > IDENTIFIER
        // - matrix < TYPE > IDENTIFIER

        Node typeNode;
        Node nameNode;

        // Composite type (array/matrix) with explicit element type: array < TYPE > NAME
        if (tokens[pos].Type == "IDENTIFIER" && (tokens[pos].Value == "array" || tokens[pos].Value == "matrix")
            && pos + 4 < tokens.Count
            && (tokens[pos + 1].Type == "DELIMITER" || tokens[pos + 1].Type == "OPERATOR") && tokens[pos + 1].Value == DelimiterWords.ANGLE_OPEN
            && tokens[pos + 2].Type == "TYPE"
            && (tokens[pos + 3].Type == "DELIMITER" || tokens[pos + 3].Type == "OPERATOR") && tokens[pos + 3].Value == DelimiterWords.ANGLE_CLOSE
            && tokens[pos + 4].Type == "IDENTIFIER")
        {
            var wrapper = tokens[pos];
            typeNode = new Node(wrapper.Value) { Line = wrapper.Line, Column = wrapper.Column };
            var inner = new Node(tokens[pos + 2].Value) { Line = tokens[pos + 2].Line, Column = tokens[pos + 2].Column };
            typeNode.Children.Add(inner);

            // consume array/matrix, '<', inner type, '>'
            pos += 4;

            var nameToken = tokens[pos];
            nameNode = new Node("Identifier", new List<Node> { new Node(nameToken.Value) }) { Line = nameToken.Line, Column = nameToken.Column };
            pos++; // consume identifier
        }
        else if (tokens[pos].Type == "IDENTIFIER" && (tokens[pos].Value == "array" || tokens[pos].Value == "matrix")
            && pos + 1 < tokens.Count && tokens[pos + 1].Type == "IDENTIFIER")
        {
            // Explicit element type missing — produce a parse-level error so semantic phase
            // doesn't confuse the tokens as identifier usages. Advance pos to consume the
            // tokens that formed the attempted declaration to avoid parser infinite loop.
            var message = $"Declaración de {tokens[pos].Value} requiere tipo de elemento explícito";
            // consume 'array' and the following identifier to avoid re-parsing the same tokens
            pos += 2;
            return ErrorNode(message, pos - 2);
        }
        else if (tokens[pos].Type == "TYPE" && pos + 1 < tokens.Count && tokens[pos + 1].Type == "IDENTIFIER")
        {
            var typeToken = tokens[pos];
            typeNode = new Node(typeToken.Value) { Line = typeToken.Line, Column = typeToken.Column };
            pos++;
            var nameToken = tokens[pos];
            nameNode = new Node("Identifier", new List<Node> { new Node(nameToken.Value) }) { Line = nameToken.Line, Column = nameToken.Column };
            pos++;
        }
        else
        {
            return ErrorNode("Declaración de variable inválida", pos);
        }

        var declarationNode = new Node("VariableDeclaration", new List<Node> { typeNode, nameNode }) { Line = typeNode.Line, Column = typeNode.Column };

        if (pos < tokens.Count)
        {
            if (tokens[pos].Type == "OPERATOR" && tokens[pos].Value == OperatorWords.ASSIGN)
            {
                pos++; // Consumir '='
                var valueNode = ParseExpression(tokens, ref pos);
                declarationNode.Children.Add(new Node("Value", new List<Node> { valueNode }));
            }
            else if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON)
            {
                // Declaración sin inicialización, solo consumir el ';' al final
            }
            else
            {
                // ❌ cualquier otra cosa es inválida
                var invalidNode = new Node { Type = "InvalidDeclaration" };
                invalidNode.Children.Add(new Node
                {
                    Type = "Error",
                    Children = { new Node { Type = $"Token inesperado '{tokens[pos].Value}' en declaración de variable." } }
                });
                pos++;

                return invalidNode;
            }

        }

        // Todas las declaraciones deben terminar con ;
        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON)
        {
            pos++;
        }
        else
        {
            string errorMessage = (pos >= tokens.Count) ?
                "Se esperaba ';' al final de la declaración de variable." :
                $"Token inesperado '{tokens[pos].Value}' al final de la declaración. Se esperaba ';'.";
            return ErrorNode(errorMessage, pos);
        }

        return declarationNode;
    }
}
