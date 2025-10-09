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

    private object? ExecuteFunctionCall(Node node)
    {
        // Expected shape: FunctionCall -> FunctionName, Arguments
        var fnameNode = node.FindChild("FunctionName") ?? node.FindChild("FunctionName");
        string fname = string.Empty;
        if (fnameNode != null)
        {
            // FunctionName -> Identifier -> <name>
            if (fnameNode.Children.Count > 0)
            {
                var id = fnameNode.Children[0];
                if (id.Children.Count > 0)
                    fname = id.Children[0].Type;
                else
                    fname = id.Type;
            }
        }

        // If user-defined function exists, invoke it
    if (!string.IsNullOrEmpty(fname) && functions.ContainsKey(fname))
        {
            var fnNode = functions[fname];
            var oldScope = currentScope;
            try
            {
                output.Add($"[DBG] Invoking function '{fname}'");
                ParadigmasLang.Logging.Logger.Debug($"Invoking function '{fname}'");
                // Prepare a new scope for function execution
                currentScope = new SymbolTable(oldScope);

                output.Add($"[DBG] Bound new scope for function '{fname}'");
                ParadigmasLang.Logging.Logger.Debug($"Bound new scope for function '{fname}'");

                // Bind parameters: function node structure: Function -> Type, Identifier, Parameters, Body
                var paramsNode = fnNode.FindChild("Parameters");
                var argsNode = node.FindChild("Arguments");
                if (paramsNode != null && argsNode != null)
                {
                    for (int i = 0; i < paramsNode.Children.Count; i++)
                    {
                        var p = paramsNode.Children[i];
                        var paramId = p.FindChild("Identifier");
                        string paramName = string.Empty;
                        if (paramId != null)
                        {
                            if (paramId.Children.Count > 0)
                                paramName = paramId.Children[0].Type;
                            else
                                paramName = paramId.Type;
                        }

                        object? argValue = null;
                        if (argsNode.Children.Count > i)
                        {
                            argValue = ExecuteNode(argsNode.Children[i]);
                        }

                        if (!string.IsNullOrEmpty(paramName))
                        {
                            // Determine declared type if available
                            string declaredType = "";
                            if (p.Children.Count > 0)
                            {
                                var typeNode = p.Children[0];
                                declaredType = typeNode.Type ?? "";
                            }

                            // Attempt simple coercion based on declaredType
                            object? coercedValue = argValue;
                            try
                            {
                                if (!string.IsNullOrEmpty(declaredType) && argValue != null)
                                {
                                    switch (declaredType.ToLower())
                                    {
                                        case "integer":
                                            coercedValue = Convert.ToInt32(argValue);
                                            break;
                                        case "float":
                                        case "real":
                                            coercedValue = Convert.ToDouble(argValue);
                                            break;
                                        case "string":
                                        case "texto":
                                            coercedValue = Convert.ToString(argValue);
                                            break;
                                        case "boolean":
                                        case "bool":
                                            coercedValue = Convert.ToBoolean(argValue);
                                            break;
                                        default:
                                            // For arrays/matrices or unknown types, leave as-is
                                            break;
                                    }
                                }
                            }
                            catch
                            {
                                // If coercion fails, report semantic warning and keep original value
                                output.Add($"Advertencia: no se pudo convertir el argumento para el parámetro '{paramName}' al tipo '{declaredType}'. Usando valor tal cual.");
                            }

                            // Declare parameter in current scope and set value
                            currentScope.DeclareVariable(paramName, declaredType ?? "", 0);
                            currentScope.SetVariableValue(paramName, coercedValue!);
                            output.Add($"[DBG] Param '{paramName}' = {coercedValue} (declared: {declaredType})");
                            ParadigmasLang.Logging.Logger.Debug($"Param '{paramName}' = {coercedValue} (declared: {declaredType})");
                        }
                    }
                }
                output.Add($"[DBG] Executing body of '{fname}'");
                ParadigmasLang.Logging.Logger.Debug($"Executing body of '{fname}'");
                // Execute function body and capture return via exception
                var body = fnNode.FindChild("Block") ?? fnNode.Children.FirstOrDefault(c => c.Type == "Body");
                object? result = null;
                try
                {
                    if (body != null)
                        ExecuteNode(body);
                }
                catch (ReturnException rex)
                {
                    result = rex.Value;
                    output.Add($"[DBG] Function '{fname}' returned: {result}");
                    ParadigmasLang.Logging.Logger.Debug($"Function '{fname}' returned: {result}");
                }

                output.Add($"[DBG] Finished invocation of '{fname}' (returning)");
                ParadigmasLang.Logging.Logger.Debug($"Finished invocation of '{fname}' (returning)");

                return result;
            }
            finally
            {
                // restore to previous scope captured in oldScope
                currentScope = oldScope;
            }
        }

        // Not found: leave to higher-level builtins handling or error
        throw new Exception($"Function '{fname}' not found at runtime");
    }

    private object? ExecuteReturn(Node node)
    {
        // Return may have an expression child
        object? val = null;
        if (node.Children.Count > 0)
            val = ExecuteNode(node.Children[0]);
        // Unwind using ReturnException which is caught by the caller that set up the function scope
        throw new ReturnException(val);
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
