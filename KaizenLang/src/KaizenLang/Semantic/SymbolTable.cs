namespace ParadigmasLang
{
    public class Symbol
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object? Value { get; set; }
        public bool IsInitialized { get; set; }
        public int Line { get; set; }

        public Symbol(string name, string type, int line)
        {
            Name = name;
            Type = type;
            Line = line;
            IsInitialized = false;
        }
    }

    public class SymbolTable
    {
        private Dictionary<string, Symbol> symbols;
        private SymbolTable? parent;

        public SymbolTable(SymbolTable? parent = null)
        {
            symbols = new Dictionary<string, Symbol>();
            this.parent = parent;
        }

        public bool DeclareVariable(string name, string type, int line)
        {
            if (symbols.ContainsKey(name))
                return false; // Variable ya declarada en este scope
            
            symbols[name] = new Symbol(name, type, line);
            return true;
        }

        public Symbol? LookupVariable(string name)
        {
            if (symbols.ContainsKey(name))
                return symbols[name];
            
            return parent?.LookupVariable(name);
        }

        public bool SetVariableValue(string name, object value)
        {
            var symbol = LookupVariable(name);
            if (symbol == null)
                return false;
            
            symbol.Value = value;
            symbol.IsInitialized = true;
            return true;
        }

        public List<Symbol> GetAllSymbols()
        {
            return symbols.Values.ToList();
        }
    }

    public class SemanticAnalyzer
    {
        private SymbolTable currentScope;
        private List<string> errors;

        public SemanticAnalyzer()
        {
            currentScope = new SymbolTable();
            errors = new List<string>();
        }

        public List<string> AnalyzeProgram(Node root)
        {
            errors.Clear();
            AnalyzeNode(root);
            return errors;
        }

        private void AnalyzeNode(Node node)
        {
            switch (node.Type)
            {
                case "Program":
                    foreach (var child in node.Children)
                        AnalyzeNode(child);
                    break;

                case "VariableDeclaration":
                    AnalyzeVariableDeclaration(node);
                    break;

                case "Assignment":
                    AnalyzeAssignment(node);
                    break;

                case "Function":
                    AnalyzeFunction(node);
                    break;

                case "If":
                case "While":
                case "DoWhile":
                    AnalyzeConditional(node);
                    break;

                case "For":
                    AnalyzeForLoop(node);
                    break;

                case "Return":
                    AnalyzeReturn(node);
                    break;

                case "FunctionCall":
                    AnalyzeFunctionCall(node);
                    break;

                case "ExpressionStatement":
                    AnalyzeExpressionStatement(node);
                    break;

                case "Expression":
                    AnalyzeExpression(node);
                    break;

                case "Block":
                    // Crear nuevo scope para el bloque
                    var oldScope = currentScope;
                    currentScope = new SymbolTable(currentScope);
                    foreach (var child in node.Children)
                        AnalyzeNode(child);
                    currentScope = oldScope;
                    break;

                default:
                    foreach (var child in node.Children)
                        AnalyzeNode(child);
                    break;
            }
        }

        private void AnalyzeReturn(Node node)
        {
            // Analizar la expresión de retorno si existe
            foreach (var child in node.Children)
                AnalyzeNode(child);
        }

        private void AnalyzeFunctionCall(Node node)
        {
            if (node.Children.Count > 0)
            {
                var nameNode = node.Children[0];
                if (nameNode.Children.Count > 0)
                {
                    var functionName = nameNode.Children[0].Type;
                    var symbol = currentScope.LookupVariable(functionName);
                    if (symbol == null)
                    {
                        errors.Add($"Función '{functionName}' no está declarada");
                    }
                }

                // Analizar argumentos
                if (node.Children.Count > 1)
                {
                    var argsNode = node.Children[1];
                    foreach (var arg in argsNode.Children)
                        AnalyzeNode(arg);
                }
            }
        }

        private void AnalyzeExpressionStatement(Node node)
        {
            foreach (var child in node.Children)
                AnalyzeNode(child);
        }

        private void AnalyzeVariableDeclaration(Node node)
        {
            if (node.Children.Count >= 2)
            {
                var typeNode = node.Children[0];
                var nameNode = node.Children[1];
                
                if (typeNode.Children.Count > 0 && nameNode.Children.Count > 0)
                {
                    var type = typeNode.Children[0].Type;
                    var name = nameNode.Children[0].Type;

                    // Verificar si se está intentando usar una palabra reservada como nombre de variable
                    if (IsReservedWord(name))
                    {
                        errors.Add($"Error: '{name}' es una palabra reservada y no puede usarse como nombre de variable");
                        return;
                    }

                    if (!currentScope.DeclareVariable(name, type, 0))
                    {
                        errors.Add($"Variable '{name}' ya está declarada en este scope");
                    }

                    // Validar tipo
                    var validTypes = new HashSet<string> { "int", "float", "double", "boolean", "char", "string", "array" };
                    if (!validTypes.Contains(type))
                    {
                        errors.Add($"Tipo '{type}' no es válido");
                    }

                    // Si tiene inicialización, verificar compatibilidad
                    if (node.Children.Count > 2)
                    {
                        var valueNode = node.Children[2];
                        AnalyzeNode(valueNode);
                        
                        // Validar compatibilidad de tipos
                        var valueType = GetExpressionType(valueNode);
                        if (!IsTypeCompatible(type, valueType))
                        {
                            errors.Add($"Error de tipo: No se puede asignar valor de tipo '{valueType}' a variable de tipo '{type}'");
                        }
                    }
                }
            }
        }

        private void AnalyzeAssignment(Node node)
        {
            if (node.Children.Count >= 2)
            {
                var varNode = node.Children[0];
                var valueNode = node.Children[1];

                if (varNode.Children.Count > 0)
                {
                    var varName = varNode.Children[0].Type;
                    
                    // Verificar si se está intentando usar una palabra reservada como variable
                    if (IsReservedWord(varName))
                    {
                        errors.Add($"Error: '{varName}' es una palabra reservada y no puede usarse como nombre de variable");
                        return;
                    }
                    
                    var symbol = currentScope.LookupVariable(varName);
                    
                    if (symbol == null)
                    {
                        errors.Add($"Variable '{varName}' no está declarada");
                    }
                    else
                    {
                        AnalyzeNode(valueNode);
                        // Validar compatibilidad de tipos
                        var valueType = GetExpressionType(valueNode);
                        if (!IsTypeCompatible(symbol.Type, valueType))
                        {
                            errors.Add($"Error de tipo: No se puede asignar valor de tipo '{valueType}' a variable '{varName}' de tipo '{symbol.Type}'");
                        }
                    }
                }
            }
        }

        private void AnalyzeFunction(Node node)
        {
            if (node.Children.Count < 2) return;
            
            var typeNode = node.Children[0];
            var nameNode = node.Children[1];
            
            if (typeNode.Children.Count > 0 && nameNode.Children.Count > 0)
            {
                var returnType = typeNode.Children[0].Type;
                var functionName = nameNode.Children[0].Type;
                
                // Registrar la función en el scope actual para permitir recursión
                currentScope.DeclareVariable(functionName, $"function_{returnType}", 0);
            }
            
            // Crear nuevo scope para la función
            var oldScope = currentScope;
            currentScope = new SymbolTable(currentScope);

            // Analizar parámetros
            if (node.Children.Count > 2)
            {
                var paramsNode = node.Children[2];
                foreach (var param in paramsNode.Children)
                {
                    if (param.Type == "Param" && param.Children.Count >= 2)
                    {
                        var paramType = param.Children[0].Type;
                        var paramName = param.Children[1].Type;
                        currentScope.DeclareVariable(paramName, paramType, 0);
                    }
                }
            }

            // Analizar cuerpo de la función
            if (node.Children.Count > 3)
            {
                AnalyzeNode(node.Children[3]);
            }

            currentScope = oldScope;
        }

        private void AnalyzeConditional(Node node)
        {
            // Analizar condición
            if (node.Children.Count > 0)
            {
                var condition = node.Children[0];
                AnalyzeNode(condition);
                // Verificar que la condición sea booleana
            }

            // Analizar bloques
            for (int i = 1; i < node.Children.Count; i++)
            {
                AnalyzeNode(node.Children[i]);
            }
        }

        private void AnalyzeForLoop(Node node)
        {
            // Crear nuevo scope para el for
            var oldScope = currentScope;
            currentScope = new SymbolTable(currentScope);

            // Analizar inicialización, condición e incremento
            foreach (var child in node.Children)
            {
                AnalyzeNode(child);
            }

            currentScope = oldScope;
        }

        private void AnalyzeExpression(Node node)
        {
            foreach (var child in node.Children)
            {
                if (child.Type == "TYPE")
                {
                    // Un tipo no puede aparecer en una expresión como variable
                    if (child.Children.Count > 0)
                    {
                        var typeName = child.Children[0].Type;
                        errors.Add($"Error: '{typeName}' es un tipo de dato y no puede usarse como variable en una expresión");
                    }
                }
                else if (child.Type == "IDENTIFIER")
                {
                    var varName = child.Children[0].Type;
                    
                    // Verificar si se está usando una palabra reservada como identificador
                    if (IsReservedWord(varName))
                    {
                        errors.Add($"Error: '{varName}' es una palabra reservada y no puede usarse como identificador");
                        return;
                    }
                    
                    var symbol = currentScope.LookupVariable(varName);
                    if (symbol == null)
                    {
                        errors.Add($"Variable '{varName}' no está declarada");
                    }
                }
                else
                {
                    AnalyzeNode(child);
                }
            }
        }

        private string GetExpressionType(Node node)
        {
            if (node.Type == "Expression" && node.Children.Count > 0)
            {
                var firstChild = node.Children[0];
                return GetNodeType(firstChild);
            }
            return GetNodeType(node);
        }

        private string GetNodeType(Node node)
        {
            switch (node.Type)
            {
                case "STRING":
                    return "string";
                case "NUMBER":
                    // Determinar si es int, float o double basado en el valor
                    if (node.Children.Count > 0)
                    {
                        var value = node.Children[0].Type;
                        if (value.Contains('.'))
                        {
                            return "float";
                        }
                        return "int";
                    }
                    return "int";
                case "BOOLEAN":
                    return "boolean";
                case "CHARACTER":
                    return "char";
                case "IDENTIFIER":
                    // Buscar el tipo de la variable en la tabla de símbolos
                    if (node.Children.Count > 0)
                    {
                        var varName = node.Children[0].Type;
                        var symbol = currentScope.LookupVariable(varName);
                        return symbol?.Type ?? "unknown";
                    }
                    return "unknown";
                case "Expression":
                    if (node.Children.Count > 0)
                        return GetNodeType(node.Children[0]);
                    return "unknown";
                default:
                    return "unknown";
            }
        }

        private bool IsTypeCompatible(string expectedType, string actualType)
        {
            // Compatibilidad exacta
            if (expectedType == actualType)
                return true;

            // Compatibilidades específicas
            switch (expectedType)
            {
                case "double":
                    return actualType == "int" || actualType == "float";
                case "float":
                    return actualType == "int";
                case "string":
                    return actualType == "char"; // char puede convertirse a string
                default:
                    return false;
            }
        }

        private bool IsReservedWord(string word)
        {
            var reservedWords = new HashSet<string>
            {
                // Tipos de datos
                "int", "float", "double", "boolean", "char", "string", "array",
                // Palabras clave de control
                "if", "else", "while", "for", "do", "return", "void",
                // Valores literales
                "true", "false", "null",
                // Entrada/Salida
                "input", "output"
            };
            
            return reservedWords.Contains(word);
        }
    }
}