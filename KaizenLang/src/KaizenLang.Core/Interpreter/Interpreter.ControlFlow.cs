namespace ParadigmasLang;

public partial class Interpreter
{
    private object? ExecuteIf(Node node)
    {
        if (node.Children.Count > 0)
        {
            var condition = ExecuteNode(node.Children[0]);
            var conditionBool = Convert.ToBoolean(condition);

            if (conditionBool && node.Children.Count > 1)
            {
                return ExecuteNode(node.Children[1]); // then block
            }
            else if (!conditionBool && node.Children.Count > 2)
            {
                return ExecuteNode(node.Children[2]); // else block
            }
        }
        return null;
    }

    private object? ExecuteWhile(Node node)
    {
        if (node.Children.Count >= 2)
        {
            var conditionNode = node.Children[0];
            var bodyNode = node.Children[1];

            int iterations = 0;
            const int maxIterations = 1000; // Prevenir bucles infinitos

            while (iterations < maxIterations)
            {
                var condition = ExecuteNode(conditionNode);
                if (!Convert.ToBoolean(condition))
                    break;

                ExecuteNode(bodyNode);
                iterations++;
            }

            if (iterations >= maxIterations)
            {
                output.Add("Advertencia: Bucle while detenido después de 1000 iteraciones");
            }
        }
        return null;
    }

    private object? ExecuteFor(Node node)
    {
        if (node.Children.Count >= 4)
        {
            // Crear nuevo scope para el for
            var oldScope = currentScope;
            currentScope = new SymbolTable(currentScope);

            try
            {
                // init
                ExecuteNode(node.Children[0]);

                int iterations = 0;
                const int maxIterations = 1000;

                while (iterations < maxIterations)
                {
                    // condition
                    var condition = ExecuteNode(node.Children[1]);
                    if (!Convert.ToBoolean(condition))
                        break;

                    // body
                    if (node.Children.Count > 3)
                        ExecuteNode(node.Children[3]);

                    // increment
                    ExecuteNode(node.Children[2]);
                    iterations++;
                }

                if (iterations >= maxIterations)
                {
                    output.Add("Advertencia: Bucle for detenido después de 1000 iteraciones");
                }
            }
            finally
            {
                currentScope = oldScope;
            }
        }
        return null;
    }

    private object? ExecuteFunction(Node node)
    {
        // Por ahora, solo registrar la función
        if (node.Children.Count >= 2)
        {
            var nameNode = node.Children[1];
            if (nameNode.Children.Count > 0)
            {
                var functionName = nameNode.Children[0].Type;
                functions[functionName] = node;
                output.Add($"Función '{functionName}' definida");
            }
        }
        return null;
    }

    private object? ExecuteBlock(Node node)
    {
        // Crear nuevo scope para el bloque
        var oldScope = currentScope;
        currentScope = new SymbolTable(currentScope);

        try
        {
            foreach (var child in node.Children)
                ExecuteNode(child);
        }
        finally
        {
            currentScope = oldScope;
        }
        return null;
    }
}
