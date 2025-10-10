namespace ParadigmasLang;

public partial class Interpreter
{
    private object? ExecuteVariableDeclaration(Node node)
    {
        if (node.Children.Count >= 2)
        {
            var typeNode = node.Children[0];
            var nameNode = node.Children[1];

            // Support both parser shapes:
            // - typeNode.Type == "string" (no children)
            // - typeNode.Children[0].Type == "string"
            string type;
            if (typeNode.Children.Count > 0)
                type = typeNode.Children[0].Type;
            else
                type = typeNode.Type;

            string name = string.Empty;
            if (nameNode.Children.Count > 0)
                name = nameNode.Children[0].Type;
            else
                name = nameNode.Type;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
                return null;

            if (!currentScope.DeclareVariable(name, type, 0))
            {
                throw new Exception($"Variable '{name}' ya está declarada");
            }

            // Si tiene inicialización
            if (node.Children.Count > 2)
            {
                var valueNode = node.Children[2];
                var rawValue = ExecuteNode(valueNode.Children[0]);
                object? finalValue = rawValue;
                // If initialization came from input (string token) and we know the declared type, try to convert
                if (rawValue is string rawStr)
                {
                    finalValue = ConvertTokenToType(rawStr, type);
                }
                currentScope.SetVariableValue(name, finalValue!);
                output.Add($"Variable '{name}' declarada e inicializada con valor: {finalValue}");
            }
            else
            {
                output.Add($"Variable '{name}' de tipo '{type}' declarada");
            }
        }
        return null;
    }

    private object? ExecuteAssignment(Node node)
    {
        if (node.Children.Count >= 2)
        {
            var varNode = node.Children[0];
            var valueNode = node.Children[1];
            // Assignment to index access: e.g. a[0] = expr
            if (varNode.Type == "IndexAccess")
            {
                // Evaluate target (should return IList)
                var targetNode = varNode.Children[0];
                var indexNode = varNode.Children[1];
                var target = ExecuteNode(targetNode);
                var idxVal = ExecuteNode(indexNode);
                var rawValue = ExecuteNode(valueNode);

                if (target is System.Collections.IList listTarget && idxVal is int i)
                {
                    if (i < 0 || i >= listTarget.Count)
                        throw new Exception($"Index fuera de rango: {i}");
                    listTarget[i] = rawValue;
                    output.Add($"Index '{i}' asignado con valor: {rawValue}");
                    return null;
                }
                else
                {
                    throw new Exception("Asignación por índice fallida: target no es lista o índice no es entero");
                }
            }

            if (varNode.Children.Count > 0)
            {
                var varName = varNode.Children[0].Type;
                var rawValue = ExecuteNode(valueNode);
                object? finalValue = rawValue;
                // Try to find the declared type of variable
                var symbol = currentScope.LookupVariable(varName);
                if (rawValue is string rawStr && symbol != null)
                {
                    finalValue = ConvertTokenToType(rawStr, symbol.Type);
                }
                if (!currentScope.SetVariableValue(varName, finalValue!))
                {
                    throw new Exception($"Variable '{varName}' no está declarada");
                }

                output.Add($"Variable '{varName}' asignada con valor: {finalValue}");
            }
        }
        return null;
    }
}
