namespace ParadigmasLang;

public partial class Interpreter
{
    private SymbolTable globalScope;
    private SymbolTable currentScope;
    private List<string> output;
    // Map function name -> Function AST node (user-defined functions are stored at runtime)
    private Dictionary<string, Node> functions;
    private readonly Func<string?, string?>? inputProvider;
    private readonly Queue<string> inputBuffer = new Queue<string>();
    private readonly object inputLock = new object();

    public Interpreter() : this(null) { }

    // Allow passing an input provider callback (used by UI to prompt user)
    public Interpreter(Func<string?, string?>? inputProvider)
    {
        this.inputProvider = inputProvider;
        globalScope = new SymbolTable();
        currentScope = globalScope;
        output = new List<string>();
        functions = new Dictionary<string, Node>(StringComparer.OrdinalIgnoreCase);
    }

    // Internal exception used to unwind execution when a 'return' is executed inside a function
    private class ReturnException : Exception
    {
        public object? Value { get; }
        public ReturnException(object? value)
        {
            Value = value;
        }
    }

    // Tokenize an input line into whitespace-separated tokens, respecting quoted strings
    private static List<string> TokenizeInputLine(string? line)
    {
        var tokens = new List<string>();
        if (string.IsNullOrEmpty(line))
            return tokens;

        bool inQuote = false;
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                inQuote = !inQuote;
                continue; // drop quote chars
            }
            if (!inQuote && char.IsWhiteSpace(c))
            {
                if (sb.Length > 0)
                {
                    tokens.Add(sb.ToString());
                    sb.Clear();
                }
            }
            else
            {
                sb.Append(c);
            }
        }
        if (sb.Length > 0)
            tokens.Add(sb.ToString());
        return tokens;
    }

    // Ensure inputBuffer has tokens: if empty, ask inputProvider for a line.
    // If no inputProvider is supplied, attempt a Console.ReadLine with a short timeout
    // so running snippets in non-interactive contexts doesn't hang indefinitely.
    private string? ReadNextInputToken(string? prompt)
    {
        lock (inputLock)
        {
            if (inputBuffer.Count == 0)
            {
                try
                {
                    string? line = null;

                    if (inputProvider != null)
                    {
                        // UI-driven provider (modal prompt) - call synchronously
                        line = inputProvider.Invoke(prompt);
                    }
                    else
                    {
                        // No provider: attempt Console.ReadLine but with timeout to avoid blocking.
                        try
                        {
                            var readTask = System.Threading.Tasks.Task.Run(() => Console.ReadLine());
                            // Wait a short timeout (2 seconds) to avoid long blocking during tests
                            if (readTask.Wait(TimeSpan.FromSeconds(2)))
                                line = readTask.Result;
                            else
                                line = null; // timeout
                        }
                        catch
                        {
                            line = null;
                        }
                    }

                    if (line == null)
                        return null;

                    var toks = TokenizeInputLine(line);
                    foreach (var t in toks)
                        inputBuffer.Enqueue(t);
                }
                catch
                {
                    return null;
                }
            }

            if (inputBuffer.Count == 0)
                return null;
            return inputBuffer.Dequeue();
        }
    }

    // Convert a token string to the expected semantic type (e.g., integer, float, bool)
    private object? ConvertTokenToType(string? token, string expectedType)
    {
        if (token == null)
            return null;
        if (string.IsNullOrEmpty(expectedType))
            return token;
        switch (expectedType)
        {
            case "integer":
                if (int.TryParse(token, out var i))
                    return i;
                break;
            case "float":
            case "double":
                if (double.TryParse(token, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var d))
                    return d;
                break;
            case "bool":
                if (bool.TryParse(token, out var b))
                    return b;
                var low = token.ToLowerInvariant();
                if (low == "1" || low == "true")
                    return true;
                if (low == "0" || low == "false")
                    return false;
                break;
            case "string":
                return token;
            default:
                return token;
        }
        return token;
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

            case "FunctionCall":
                // Evaluate a function call expression: either builtin handled in ExecuteExpression
                // or user-defined function lookup and invocation
                return ExecuteFunctionCall(node);

            case "Return":
                return ExecuteReturn(node);

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
                if (literal == "true")
                    return true;
                if (literal == "false")
                    return false;
                if (literal == "null")
                    return null;
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

            case "ArrayLiteral":
                // Construct a runtime list from array literal elements
                var elementsNode = node.Children.FirstOrDefault(c => c.Type == "Elements");
                var list = new List<object?>();
                if (elementsNode != null)
                {
                    foreach (var childExpr in elementsNode.Children)
                    {
                        var v = ExecuteNode(childExpr);
                        list.Add(v);
                    }
                }
                return list;

            case "IndexAccess":
                // children[0] = target (identifier or nested index), children[1] = index expression
                if (node.Children.Count >= 2)
                {
                    var target = ExecuteNode(node.Children[0]);
                    var idx = ExecuteNode(node.Children[1]);
                    if (target is System.Collections.IList listTarget && idx is int i)
                    {
                        if (i < 0 || i >= listTarget.Count)
                            throw new Exception($"Index fuera de rango: {i}");
                        return listTarget[i];
                    }
                    else
                    {
                        throw new Exception($"Operación de indexación no soportada para tipo {target?.GetType().Name}");
                    }
                }
                return null;

            default:
                // Para nodos como 'ExpressionStatement', simplemente ejecutamos sus hijos
                foreach (var child in node.Children)
                    ExecuteNode(child);
                return null;
        }
    }
}
