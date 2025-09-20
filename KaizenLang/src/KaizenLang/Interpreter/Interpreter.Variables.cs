namespace ParadigmasLang
{
    public partial class Interpreter
    {
        private object? ExecuteVariableDeclaration(Node node)
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
                        throw new Exception($"Variable '{name}' ya está declarada");
                    }

                    // Si tiene inicialización
                    if (node.Children.Count > 2)
                    {
                        var valueNode = node.Children[2];
                        var value = ExecuteNode(valueNode.Children[0]);
                        currentScope.SetVariableValue(name, value!);
                        output.Add($"Variable '{name}' declarada e inicializada con valor: {value}");
                    }
                    else
                    {
                        output.Add($"Variable '{name}' de tipo '{type}' declarada");
                    }
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

                if (varNode.Children.Count > 0)
                {
                    var varName = varNode.Children[0].Type;
                    var value = ExecuteNode(valueNode);

                    if (!currentScope.SetVariableValue(varName, value!))
                    {
                        throw new Exception($"Variable '{varName}' no está declarada");
                    }

                    output.Add($"Variable '{varName}' asignada con valor: {value}");
                }
            }
            return null;
        }
    }
}
