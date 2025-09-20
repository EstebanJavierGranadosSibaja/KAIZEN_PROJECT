namespace ParadigmasLang
{
    public partial class Interpreter
    {
        private SymbolTable globalScope;
        private SymbolTable currentScope;
        private List<string> output;
        private Dictionary<string, object> functions;

        public Interpreter()
        {
            globalScope = new SymbolTable();
            currentScope = globalScope;
            output = new List<string>();
            functions = new Dictionary<string, object>();
        }

        public List<string> Execute(Node root)
        {
            output.Clear();
            try
            {
                ExecuteNode(root);
            }
            catch (Exception ex)
            {
                output.Add($"Error de ejecución: {ex.Message}");
            }
            return output;
        }

        private object? ExecuteNode(Node node)
        {
            switch (node.Type)
            {
                case "Program":
                    foreach (var child in node.Children)
                        ExecuteNode(child);
                    return null;

                case "VariableDeclaration":
                    return ExecuteVariableDeclaration(node);

                case "Assignment":
                    return ExecuteAssignment(node);

                case "Expression":
                    return ExecuteExpression(node);

                case "If":
                    return ExecuteIf(node);

                case "While":
                    return ExecuteWhile(node);

                case "For":
                    return ExecuteFor(node);

                case "Function":
                    return ExecuteFunction(node);

                case "Block":
                    return ExecuteBlock(node);

                // Tipos básicos
                case "INT":
                    return int.Parse(node.Children[0].Type);

                case "FLOAT":
                    return float.Parse(node.Children[0].Type);

                case "STRING":
                    return node.Children[0].Type;

                case "LITERAL":
                    var literal = node.Children[0].Type;
                    if (literal == "true") return true;
                    if (literal == "false") return false;
                    if (literal == "null") return null;
                    return literal;

                case "IDENTIFIER":
                    var varName = node.Children[0].Type;
                    var symbol = currentScope.LookupVariable(varName);
                    if (symbol == null)
                    {
                        // Antes de lanzar una excepción, verificar si es una función predefinida
                        if (varName == "output")
                        {
                            return "output"; // Devolver un identificador especial para la función 'output'
                        }
                        throw new Exception($"Variable o función '{varName}' no está declarada");
                    }
                    if (!symbol.IsInitialized)
                    {
                        // Opcional: advertir o lanzar error si se usa una variable no inicializada
                        // output.Add($"Advertencia: La variable '{varName}' se está usando sin haber sido inicializada.");
                    }
                    return symbol.Value;

                default:
                    // Para nodos como 'ExpressionStatement', simplemente ejecutamos sus hijos
                    foreach (var child in node.Children)
                        ExecuteNode(child);
                    return null;
            }
        }
    }
}
