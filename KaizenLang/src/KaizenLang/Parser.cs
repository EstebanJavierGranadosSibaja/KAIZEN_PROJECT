namespace ParadigmasLang
{
    // Analizador sintáctico: convierte tokens en un árbol de sintaxis
    public class Parser
    {
        // Utilidad para crear nodos de error con posición
        private Node ErrorNode(string mensaje, int pos)
        {
            return new Node { Type = "Error", Children = { new Node { Type = $"{mensaje} (posición {pos})" } } };
        }

        // Validación y construcción de parámetros de función
        private Node ParseParams(List<Token> tokens, ref int pos)
        {
            var parameters = new Node("Parameters");
            while (tokens[pos].Type != "DELIMITER" || tokens[pos].Value != DelimiterWords.PAREN_CLOSE)
            {
                if (tokens[pos].Type == "TYPE")
                {
                    var typeNode = new Node(tokens[pos].Value);
                    pos++;
                    if (tokens[pos].Type == "IDENTIFIER")
                    {
                        var nameNode = new Node("Identifier", new List<Node> { new Node(tokens[pos].Type) });
                        parameters.Children.Add(new Node("Param", new List<Node> { typeNode, nameNode }));
                        pos++;
                        if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.COMMA)
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

        // Nueva función para verificar si es una expresión que termina en ;
        private bool IsExpressionStatement(List<Token> tokens, int pos)
        {
            if (pos >= tokens.Count) return false;
            
            // Buscar si hay un ; en los próximos tokens (expresión válida)
            int tempPos = pos;
            int parenLevel = 0;
            
            while (tempPos < tokens.Count)
            {
                if (tokens[tempPos].Type == "DELIMITER")
                {
                    if (tokens[tempPos].Value == DelimiterWords.PAREN_OPEN) parenLevel++;
                    else if (tokens[tempPos].Value == DelimiterWords.PAREN_CLOSE) parenLevel--;
                    else if (tokens[tempPos].Value == DelimiterWords.SEMICOLON && parenLevel == 0) return true; // Encontrado
                    else if (tokens[tempPos].Value == DelimiterWords.BLOCK_START && parenLevel == 0) return false; // Es un bloque, no una expresión
                }
                tempPos++;
            }
            return false;
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


        // Nueva validación: asignación
        private bool IsAssignment(List<Token> tokens, int pos)
        {
            return pos + 2 < tokens.Count &&
                   tokens[pos].Type == "IDENTIFIER" &&
                   tokens[pos + 1].Type == "OPERATOR" &&
                   tokens[pos + 1].Value == "=";
        }

        private Node ParseAssignment(List<Token> tokens, ref int pos)
        {
            var varNode = new Node("Identifier", new List<Node> { new Node(tokens[pos].Value) });
            pos++; // Consumir identificador
            pos++; // Consumir '='
            var valueNode = ParseExpression(tokens, ref pos);
            var assignmentNode = new Node("Assignment", new List<Node> { varNode, valueNode });

            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON)
                pos++;
            else
                return ErrorNode("Se esperaba ';' después de la asignación.", pos);
            
            return assignmentNode;
        }

        // Validación: tipo seguido de identificador
        private bool IsVariableDeclaration(List<Token> tokens, int pos)
        {
            // Solo aceptar tipos válidos actualizados
            var validTypes = new HashSet<string> { 
                "integer", "float", "double", "bool", "string", 
                "array_integer", "array_float", "array_string", "array_bool",
                "matrix_integer", "matrix_float", "matrix_string", "matrix_bool"
            };
            return pos + 1 < tokens.Count && tokens[pos].Type == "TYPE" && validTypes.Contains(tokens[pos].Value) && tokens[pos + 1].Type == "IDENTIFIER";
        }

        private Node ParseVariableDeclaration(List<Token> tokens, ref int pos)
        {
            var typeNode = new Node(tokens[pos].Value);
            pos++;
            var nameNode = new Node("Identifier", new List<Node> { new Node(tokens[pos].Value) });
            pos++;

            var declarationNode = new Node("VariableDeclaration", new List<Node> { typeNode, nameNode });

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

        private bool Match(List<Token> tokens, int pos, string type, string value)
        {
            return pos < tokens.Count && tokens[pos].Type == type && tokens[pos].Value == value;
        }

        private bool IsFunctionDeclaration(List<Token> tokens, int pos)
        {
            // tipo nombre ( ... ) { ... }
            return pos + 3 < tokens.Count && tokens[pos].Type == "TYPE" && tokens[pos + 1].Type == "IDENTIFIER" && tokens[pos + 2].Type == "DELIMITER" && tokens[pos + 2].Value == "(";
        }

        private Node ParseIf(List<Token> tokens, ref int pos)
        {
            var ifNode = new Node("If");
            pos++; // Consumir 'if'

            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_OPEN)
                pos++;
            else
                return ErrorNode("Se esperaba '(' después de 'if'.", pos);
            
            var condition = ParseExpression(tokens, ref pos);
            ifNode.Children.Add(condition);

            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_CLOSE)
                pos++;
            else
                return ErrorNode("Se esperaba ')' después de la condición del if.", pos);

            Node thenBranch;
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
            {
                pos++; // Consumir 'ying'
                thenBranch = ParseBlock(tokens, ref pos);
            }
            else
            {
                var singleStatement = ParseStatement(tokens, ref pos);
                if (singleStatement != null)
                {
                    // Creamos un "bloque implícito" para la sentencia única
                    thenBranch = new Node("Block");
                    thenBranch.Children.Add(singleStatement);
                }
                else
                {
                    return ErrorNode("Se esperaba un bloque '{...}' o una sentencia después de la condición del if.", pos);
                }
            }
            ifNode.Children.Add(thenBranch);

            // Manejo del 'else'
            if (pos < tokens.Count && Match(tokens, pos, "RESERVED", ReservedWords.ELSE))
            {
                pos++; // Consumir 'else'
                Node elseBranch;

                if (pos < tokens.Count && Match(tokens, pos, "RESERVED", ReservedWords.IF))
                {
                    // 'else if' - el 'else if' es simplemente un 'if' anidado en el 'else'
                    elseBranch = ParseIf(tokens, ref pos);
                }
                else if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
                {
                    pos++; // Consumir 'ying'
                    elseBranch = ParseBlock(tokens, ref pos);
                }
                else
                {
                    var singleStatement = ParseStatement(tokens, ref pos);
                    if (singleStatement != null)
                    {
                        elseBranch = new Node("Block");
                        elseBranch.Children.Add(singleStatement);
                    }
                    else
                    {
                        return ErrorNode("Se esperaba un bloque '{...}', 'if' o una sentencia después de 'else'.", pos);
                    }
                }
                ifNode.Children.Add(elseBranch);
            }
            return ifNode;
        }

        private Node ParseFor(List<Token> tokens, ref int pos)
        {
            pos++; // Consumir 'for'
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_OPEN) pos++; else return ErrorNode("Se esperaba '(' después de 'for'.", pos);
            
            Node? init = ParseStatement(tokens, ref pos); // La inicialización es una sentencia
            if (init == null)
            {
                // Si la inicialización es opcional y está vacía, se representa con un nodo vacío.
                // El punto y coma sigue siendo obligatorio.
                init = new Node("Empty"); 
            }

            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON) pos++; else return ErrorNode("Se esperaba ';' después de la inicialización del for.", pos);
            var condition = ParseExpression(tokens, ref pos);
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON) pos++; else return ErrorNode("Se esperaba ';' después de la condición del for.", pos);
            var increment = ParseExpression(tokens, ref pos);
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_CLOSE) pos++; else return ErrorNode("Se esperaba ')' después de la expresión de incremento del for.", pos);
            
            Node body;
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
            {
                pos++; // Consumir 'ying'
                body = ParseBlock(tokens, ref pos);
            }
            else
            {
                var statementBody = ParseStatement(tokens, ref pos);
                if (statementBody == null)
                {
                    // Si no hay cuerpo, es un error sintáctico.
                    return ErrorNode("Se esperaba un cuerpo de bucle (un bloque o una sentencia) después de la cabecera del for.", pos);
                }
                // Envolvemos la sentencia única en un bloque para consistencia.
                body = new Node("Block", new List<Node> { statementBody });
            }

            return new Node("For", new List<Node> { init, condition, increment, body });
        }

        private Node ParseWhile(List<Token> tokens, ref int pos)
        {
            pos++; // Consumir 'while'
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_OPEN) pos++; else return ErrorNode("Se esperaba '(' después de 'while'.", pos);
            var condition = ParseExpression(tokens, ref pos);
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_CLOSE) pos++; else return ErrorNode("Se esperaba ')' después de la condición del while.", pos);
            
            Node body;
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
            {
                pos++; // Consumir 'ying'
                body = ParseBlock(tokens, ref pos);
            }
            else
            {
                body = ParseStatement(tokens, ref pos) ?? new Node("Block");
            }

            return new Node("While", new List<Node> { condition, body });
        }

        private Node ParseDoWhile(List<Token> tokens, ref int pos)
        {
            pos++; // Consumir 'do'
            Node body;
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
            {
                pos++; // Consumir 'ying'
                body = ParseBlock(tokens, ref pos);
            }
            else
            {
                body = ParseStatement(tokens, ref pos) ?? new Node("Block");
            }

            if (Match(tokens, pos, "RESERVED", ReservedWords.WHILE))
            {
                pos++; // Consumir 'while'
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_OPEN) pos++; else return ErrorNode("Se esperaba '(' después de 'while' en un do-while.", pos);
                var condition = ParseExpression(tokens, ref pos);
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_CLOSE) pos++; else return ErrorNode("Se esperaba ')' después de la condición del do-while.", pos);
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON) pos++; else return ErrorNode("Se esperaba ';' después del do-while.", pos);
                return new Node("DoWhile", new List<Node> { body, condition });
            }
            return ErrorNode("Se esperaba 'while' después del bloque de un do-while.", pos);
        }

        private Node ParseFunction(List<Token> tokens, ref int pos)
        {
            var typeNode = new Node(tokens[pos].Value);
            pos++;
            var nameNode = new Node("Identifier", new List<Node> { new Node(tokens[pos].Value) });
            pos++;

            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_OPEN) pos++; else return ErrorNode("Se esperaba '(' después del nombre de la función.", pos);
            var parameters = ParseParams(tokens, ref pos);
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_CLOSE) pos++; else return ErrorNode("Se esperaba ')' después de los parámetros de la función.", pos);
            
            Node body;
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
            {
                pos++; // Consumir 'ying'
                body = ParseBlock(tokens, ref pos);
            }
            else
            {
                return ErrorNode("Se esperaba un bloque '{...}' para el cuerpo de la función.", pos);
            }

            return new Node("Function", new List<Node> { typeNode, nameNode, parameters, body });
        }

        private Node ParseBlock(List<Token> tokens, ref int pos)
        {
            var blockNode = new Node("Block");

            // Asume que el 'ying' ya fue consumido
            while (pos < tokens.Count && !(tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_END))
            {
                var stmt = ParseStatement(tokens, ref pos);
                if (stmt != null) blockNode.Children.Add(stmt);
                else 
                {
                    // Si no se pudo parsear y no es el final, es un error.
                    blockNode.Children.Add(ErrorNode($"Token inesperado '{tokens[pos].Value}' dentro de un bloque.", pos));
                    pos++;
                }
            }

            if (pos >= tokens.Count || tokens[pos].Value != DelimiterWords.BLOCK_END)
            {
                // No se encontró 'yang' al final
                // Se puede agregar un nodo de error al AST o lanzar una excepción
                // Por ahora, para mantenerlo simple, agregaremos un nodo de error.
                // Idealmente, se debería tener una mejor gestión de errores.
                // Esto es más informativo que simplemente devolver un único nodo de error.
                // El analizador semántico puede entonces decidir cómo manejarlo.
                blockNode.Children.Add(ErrorNode($"Bloque no cerrado. Se esperaba '{DelimiterWords.BLOCK_END}'.", pos));
                return blockNode; 
            }
            else
            {
                pos++; // Consumir 'yang'
            }

            return blockNode;
        }

        private Node ParseExpression(List<Token> tokens, ref int pos)
        {
            Node node = new Node { Type = "Expression" };
            int startPos = pos;
            
            while (pos < tokens.Count)
            {
                // Terminar en delimitadores que indican fin de expresión
                if (tokens[pos].Type == "DELIMITER" && 
                    (tokens[pos].Value == ";" || tokens[pos].Value == ")" || 
                     tokens[pos].Value == "," || tokens[pos].Value == "yang"))
                    break;

                // Manejar llamadas a función: IDENTIFIER (
                if (pos + 1 < tokens.Count && 
                    tokens[pos].Type == "IDENTIFIER" && 
                    tokens[pos + 1].Type == "DELIMITER" && 
                    tokens[pos + 1].Value == "(")
                {
                    var funcCall = ParseFunctionCall(tokens, ref pos);
                    node.Children.Add(funcCall);
                    continue;
                }

                // Manejar expresiones entre paréntesis
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(")
                {
                    pos++; // consumir (
                    var nested = ParseExpression(tokens, ref pos);
                    node.Children.Add(new Node { Type = "Parentheses", Children = { nested } });
                    if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")")
                        pos++; // consumir )
                    continue;
                }

                // Manejar arrays [1, 2, 3]
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "[")
                {
                    var arrayNode = ParseArrayLiteral(tokens, ref pos);
                    node.Children.Add(arrayNode);
                    continue;
                }

                // Manejar operadores
                if (tokens[pos].Type == "OPERATOR")
                {
                    node.Children.Add(new Node { Type = "Operator", Children = { new Node { Type = tokens[pos].Value } } });
                    pos++;
                    continue;
                }

                // Otros tokens (literales, identificadores, etc.)
                node.Children.Add(new Node { Type = tokens[pos].Type, Children = { new Node { Type = tokens[pos].Value } } });
                pos++;
            }

            if (node.Children.Count == 0)
            {
                node.Children.Add(ErrorNode("Expresión vacía", startPos));
            }

            return node;
        }

        // Nueva función para parsear llamadas a función
        private Node ParseFunctionCall(List<Token> tokens, ref int pos)
        {
            Node node = new Node { Type = "FunctionCall" };
            
            // Nombre de la función
            node.Children.Add(new Node { Type = "FunctionName", Children = { new Node { Type = tokens[pos].Value } } });
            pos++; // IDENTIFIER
            pos++; // (
            
            // Argumentos
            var argsNode = new Node { Type = "Arguments" };
            while (pos < tokens.Count && !(tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")"))
            {
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ",")
                {
                    pos++; // consumir ,
                    continue;
                }
                argsNode.Children.Add(ParseExpression(tokens, ref pos));
            }
            
            node.Children.Add(argsNode);
            
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")")
                pos++; // consumir )
                
            return node;
        }

        // Nueva función para parsear arrays [1, 2, 3]
        private Node ParseArrayLiteral(List<Token> tokens, ref int pos)
        {
            Node arrayNode = new Node { Type = "ArrayLiteral" };
            pos++; // consumir [
            
            var elementsNode = new Node { Type = "Elements" };
            
            while (pos < tokens.Count && !(tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "]"))
            {
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ",")
                {
                    pos++; // consumir ,
                    continue;
                }
                
                // Parsear cada elemento del array
                elementsNode.Children.Add(ParseExpression(tokens, ref pos));
            }
            
            arrayNode.Children.Add(elementsNode);
            
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "]")
                pos++; // consumir ]
            else
            {
                arrayNode.Children.Add(ErrorNode("Se esperaba ']' al final del array", pos));
            }
                
            return arrayNode;
        }
    }
}
