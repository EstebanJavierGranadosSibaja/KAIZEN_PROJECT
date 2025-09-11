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
                        // Aquí podrías validar compatibilidad de tipos
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
                    var symbol = currentScope.LookupVariable(varName);
                    
                    if (symbol == null)
                    {
                        errors.Add($"Variable '{varName}' no está declarada");
                    }
                    else
                    {
                        AnalyzeNode(valueNode);
                        // Validar compatibilidad de tipos aquí
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
                if (child.Type == "IDENTIFIER")
                {
                    var varName = child.Children[0].Type;
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
    }
}