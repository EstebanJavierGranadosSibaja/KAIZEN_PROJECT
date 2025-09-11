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
            var paramsNode = new Node { Type = "Params" };
            int paramIndex = 0;
            while (tokens[pos].Type != "DELIMITER" || tokens[pos].Value != ")")
            {
                if (tokens[pos].Type == "TYPE")
                {
                    var paramType = tokens[pos].Value;
                    pos++;
                    if (tokens[pos].Type == "IDENTIFIER")
                    {
                        var paramName = tokens[pos].Value;
                        pos++;
                        paramsNode.Children.Add(new Node { Type = "Param", Children = { new Node { Type = paramType }, new Node { Type = paramName } } });
                        paramIndex++;
                        if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ",") pos++;
                    }
                    else
                    {
                        paramsNode.Children.Add(ErrorNode("Falta nombre de parámetro", pos));
                        break;
                    }
                }
                else if (tokens[pos].Type == "IDENTIFIER")
                {
                    paramsNode.Children.Add(ErrorNode("Falta tipo de parámetro", pos));
                    pos++;
                    break;
                }
                else if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ",")
                {
                    pos++;
                }
                else
                {
                    paramsNode.Children.Add(ErrorNode($"Token inesperado '{tokens[pos].Value}' en parámetros", pos));
                    pos++;
                    break;
                }
            }
            return paramsNode;
        }
        public Node Parse(List<Token> tokens)
        {
            int pos = 0;
            Node root = new Node { Type = "Program" };
            while (pos < tokens.Count)
            {
                var node = ParseStatement(tokens, ref pos);
                if (node != null) root.Children.Add(node);
                else pos++;
            }
            return root;
        }

        private Node? ParseStatement(List<Token> tokens, ref int pos)
        {
            if (Match(tokens, pos, "RESERVED", "if"))
                return ParseIf(tokens, ref pos);
            if (Match(tokens, pos, "RESERVED", "for"))
                return ParseFor(tokens, ref pos);
            if (Match(tokens, pos, "RESERVED", "while"))
                return ParseWhile(tokens, ref pos);
            if (Match(tokens, pos, "RESERVED", "do"))
                return ParseDoWhile(tokens, ref pos);
            if (Match(tokens, pos, "RESERVED", "return"))
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

            // Si empieza con TYPE pero mal estructurado
            if (tokens.Count > pos && tokens[pos].Type == "TYPE")
            {
                var invalidNode = new Node { Type = "InvalidDeclaration" };
                invalidNode.Children.Add(new Node
                {
                    Type = "Error",
                    Children = { new Node { Type = "Se esperaba un identificador después del tipo." } }
                });
                pos++;
                return invalidNode;
            }

            return null;
        }

        // Nueva función para manejar return
        private Node ParseReturn(List<Token> tokens, ref int pos)
        {
            Node node = new Node { Type = "Return" };
            pos++; // consumir 'return'
            
            // Si hay una expresión después de return
            if (pos < tokens.Count && !(tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";"))
            {
                node.Children.Add(ParseExpression(tokens, ref pos));
            }
            
            // Consumir el ;
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";")
                pos++;
                
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
                    if (tokens[tempPos].Value == "(") parenLevel++;
                    else if (tokens[tempPos].Value == ")") parenLevel--;
                    else if (tokens[tempPos].Value == ";" && parenLevel == 0) return true;
                    else if (tokens[tempPos].Value == "{" && parenLevel == 0) return false;
                }
                tempPos++;
            }
            return false;
        }

        // Nueva función para parsear expresiones que son sentencias
        private Node ParseExpressionStatement(List<Token> tokens, ref int pos)
        {
            Node node = new Node { Type = "ExpressionStatement" };
            node.Children.Add(ParseExpression(tokens, ref pos));
            
            // Consumir el ;
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";")
                pos++;
                
            return node;
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
            Node node = new Node { Type = "Assignment" };
            node.Children.Add(new Node { Type = "Variable", Children = { new Node { Type = tokens[pos].Value } } });
            pos += 2; // IDENTIFIER =
            node.Children.Add(ParseExpression(tokens, ref pos));

            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";")
                pos++;

            return node;
        }

        // Validación: tipo seguido de identificador
        private bool IsVariableDeclaration(List<Token> tokens, int pos)
        {
            // Solo aceptar tipos válidos
            var validTypes = new HashSet<string> { "int", "float", "double", "boolean", "char", "string", "array" };
            return pos + 1 < tokens.Count && tokens[pos].Type == "TYPE" && validTypes.Contains(tokens[pos].Value) && tokens[pos + 1].Type == "IDENTIFIER";
        }

        private Node ParseVariableDeclaration(List<Token> tokens, ref int pos)
        {
            Node node = new Node { Type = "VariableDeclaration" };

            // Tipo
            node.Children.Add(new Node { Type = "Type", Children = { new Node { Type = tokens[pos].Value } } });
            pos++;
            // Nombre
            node.Children.Add(new Node { Type = "Name", Children = { new Node { Type = tokens[pos].Value } } });
            pos++;

            // Validar lo que sigue
            if (pos < tokens.Count)
            {
                if (tokens[pos].Type == "OPERATOR" && tokens[pos].Value == "=")
                {
                    // inicialización válida
                    pos++;
                    node.Children.Add(new Node
                    {
                        Type = "Value",
                        Children = { ParseExpression(tokens, ref pos) }
                    });
                }
                else if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";")
                {
                    // declaración simple, válido
                    pos++;
                    return node;
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

            // Después de parsear el valor (si hay)
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";")
            {
                pos++; // consumir ;
            }
            else
            {
                // Error: faltó el ;
                var errorNode = new Node { Type = "Error" };
                errorNode.Children.Add(new Node { Type = "Se esperaba ';' al final de la declaración." });
                node.Children.Add(errorNode);
            }

            return node;
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
            // if (condición) { ... } else { ... }
            Node node = new Node { Type = "If" };
            pos++; // 'if'
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(") pos++;
            node.Children.Add(ParseExpression(tokens, ref pos)); // condición
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")") pos++;
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
            {
                node.Children.Add(ParseBlock(tokens, ref pos));
            }
            if (Match(tokens, pos, "RESERVED", "else"))
            {
                pos++;
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
                    node.Children.Add(ParseBlock(tokens, ref pos));
            }
            return node;
        }

        private Node ParseFor(List<Token> tokens, ref int pos)
        {
            // for (init; cond; inc) { ... }
            Node node = new Node { Type = "For" };
            pos++; // 'for'
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(") pos++;
            node.Children.Add(ParseExpression(tokens, ref pos)); // init
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";") pos++;
            node.Children.Add(ParseExpression(tokens, ref pos)); // cond
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";") pos++;
            node.Children.Add(ParseExpression(tokens, ref pos)); // inc
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")") pos++;
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
                node.Children.Add(ParseBlock(tokens, ref pos));
            return node;
        }

        private Node ParseWhile(List<Token> tokens, ref int pos)
        {
            // while (cond) { ... }
            Node node = new Node { Type = "While" };
            pos++; // 'while'
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(") pos++;
            node.Children.Add(ParseExpression(tokens, ref pos)); // cond
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")") pos++;
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
                node.Children.Add(ParseBlock(tokens, ref pos));
            return node;
        }

        private Node ParseDoWhile(List<Token> tokens, ref int pos)
        {
            // do { ... } while (cond);
            Node node = new Node { Type = "DoWhile" };
            pos++; // 'do'
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
                node.Children.Add(ParseBlock(tokens, ref pos));
            if (Match(tokens, pos, "RESERVED", "while"))
            {
                pos++;
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(") pos++;
                node.Children.Add(ParseExpression(tokens, ref pos));
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")") pos++;
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";") pos++;
            }
            return node;
        }

        private Node ParseFunction(List<Token> tokens, ref int pos)
        {
            // tipo nombre (params) { ... }
            Node node = new Node { Type = "Function" };
            node.Children.Add(new Node { Type = "Type", Children = { new Node { Type = tokens[pos].Value } } });
            pos++;
            node.Children.Add(new Node { Type = "Name", Children = { new Node { Type = tokens[pos].Value } } });
            pos++;
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(") pos++;
            node.Children.Add(ParseParams(tokens, ref pos));
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")") pos++;
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
                node.Children.Add(ParseBlock(tokens, ref pos));
            return node;
        }

        private Node ParseBlock(List<Token> tokens, ref int pos)
        {
            // { ... }
            Node node = new Node { Type = "Block" };
            pos++; // '{'
            while (pos < tokens.Count && !(tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "}"))
            {
                var stmt = ParseStatement(tokens, ref pos);
                if (stmt != null) node.Children.Add(stmt);
                else pos++;
            }
            pos++; // '}'
            return node;
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
                     tokens[pos].Value == "," || tokens[pos].Value == "}"))
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
    }
}
