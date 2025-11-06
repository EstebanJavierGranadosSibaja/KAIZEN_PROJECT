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
                return ExecuteNode(node.Children[1]);
            }
            else if (!conditionBool && node.Children.Count > 2)
            {
                return ExecuteNode(node.Children[2]);
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
            const int maxIterations = 10000; // Prevenir bucles infinitos

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
            var oldScope = currentScope;
            currentScope = new SymbolTable(currentScope);

            try
            {
                ExecuteNode(node.Children[0]);

                int iterations = 0;
                const int maxIterations = 1000;

                while (iterations < maxIterations)
                {
                    var condition = ExecuteNode(node.Children[1]);
                    if (!Convert.ToBoolean(condition))
                        break;

                    if (node.Children.Count > 3)
                        ExecuteNode(node.Children[3]);

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
        if (node.Children.Count >= 2)
        {
            var nameNode = node.Children[1];
            if (nameNode.Children.Count > 0)
            {
                var functionName = nameNode.Children[0].Type;
                functions[functionName] = node;
                if (VerboseMode)
                    output.Add($"Función '{functionName}' definida");
            }
        }
        return null;
    }

    private object? ExecuteFunctionCall(Node node)
    {
    var fnameNode = node.FindChild("FunctionName");
    string fname = SemanticUtils.ExtractIdentifierName(fnameNode) ?? string.Empty;
    var callArgsNode = node.FindChild("Arguments");

        if (string.Equals(fname, ReservedWords.OUTPUT, System.StringComparison.OrdinalIgnoreCase))
        {
            var outputValues = new List<string>();
            if (callArgsNode != null && callArgsNode.Children.Count > 0)
            {
                foreach (var arg in callArgsNode.Children)
                {
                    var val = ExecuteNode(arg);
                    outputValues.Add(val?.ToString() ?? "null");
                }
            }
            output.Add(string.Join(" ", outputValues));
            return null;
        }

        if (string.Equals(fname, ReservedWords.INPUT, System.StringComparison.OrdinalIgnoreCase))
        {
            string? prompt = null;
            if (callArgsNode != null && callArgsNode.Children.Count > 0)
            {
                var firstArgNode = callArgsNode.Children[0];
                if (firstArgNode.Type == "IDENTIFIER")
                {
                    string varName;
                    if (firstArgNode.Children.Count > 0)
                        varName = firstArgNode.Children[0].Type;
                    else
                        varName = firstArgNode.Type;

                    var tokenLine = ReadNextInputToken(null);
                    if (tokenLine == null)
                        return null;

                    var symbol = currentScope.LookupVariable(varName);
                    object? finalVal = tokenLine;
                    if (symbol != null)
                    {
                        var targetType = ExtractPrimitiveType(symbol.Type);
                        finalVal = ConvertTokenToType(tokenLine, targetType);
                        currentScope.SetVariableValue(varName, finalVal);
                        if (VerboseMode)
                            output.Add($"Variable '{varName}' asignada con valor: {finalVal?.ToString() ?? "null"}");
                    }
                    else
                    {
                        return tokenLine;
                    }

                    return finalVal;
                }

                var firstArg = ExecuteNode(firstArgNode);
                prompt = firstArg?.ToString();
            }

            var token = ReadNextInputToken(prompt);
            return token;
        }

        if (string.Equals(fname, "length", System.StringComparison.OrdinalIgnoreCase))
        {
            return ExecuteLengthBuiltin(callArgsNode);
        }

        if (!string.IsNullOrEmpty(fname) && functions.ContainsKey(fname))
        {
            var fnNode = functions[fname];
            if ((currentCallDepth + 1) > maxCallDepth)
                throw new Exception($"Se excedió la profundidad máxima de recursión permitida ({maxCallDepth}).");

            currentCallDepth++;
            var oldScope = currentScope;
            try
            {
                if (VerboseMode)
                {
                    output.Add($"[DBG] Invoking function '{fname}'");
                    ParadigmasLang.Logging.Logger.Debug($"Invoking function '{fname}'");
                }
                currentScope = new SymbolTable(oldScope);

                if (VerboseMode)
                {
                    output.Add($"[DBG] Bound new scope for function '{fname}'");
                    ParadigmasLang.Logging.Logger.Debug($"Bound new scope for function '{fname}'");
                }

                var paramsNode = fnNode.FindChild("Parameters");
                if (paramsNode != null && callArgsNode != null)
                {
                    var argsNode = callArgsNode;
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
                            string declaredType = string.Empty;
                            string primitiveParamType = string.Empty;
                            if (p.Children.Count > 0)
                            {
                                var typeNode = p.Children[0];
                                var typeInfo = ExtractTypeInfo(typeNode);
                                declaredType = !string.IsNullOrEmpty(typeInfo.fullType) ? typeInfo.fullType : (typeNode.Type ?? string.Empty);
                                primitiveParamType = !string.IsNullOrEmpty(typeInfo.primitiveType) ? typeInfo.primitiveType : declaredType;
                            }

                            object? coercedValue = argValue;
                            try
                            {
                                if (!string.IsNullOrEmpty(primitiveParamType) && argValue != null)
                                {
                                    var declaredTypeLower = primitiveParamType.ToLowerInvariant();
                                    switch (declaredTypeLower)
                                    {
                                        case TypeWords.GEAR:
                                            coercedValue = Convert.ToInt32(argValue);
                                            break;
                                        case TypeWords.SHINKAI:
                                        case TypeWords.BANKAI:
                                        case "real":
                                            coercedValue = Convert.ToDouble(argValue);
                                            break;
                                        case TypeWords.GRIMOIRE:
                                        case "texto":
                                            coercedValue = Convert.ToString(argValue);
                                            break;
                                        case TypeWords.SHIN:
                                        case "boolean":
                                            coercedValue = Convert.ToBoolean(argValue);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            catch
                            {
                                if (VerboseMode)
                                    output.Add($"Advertencia: no se pudo convertir el argumento para el parámetro '{paramName}' al tipo '{declaredType}'. Usando valor tal cual.");
                            }

                            currentScope.DeclareVariable(paramName, declaredType ?? string.Empty, 0);
                            currentScope.SetVariableValue(paramName, coercedValue);
                            if (VerboseMode)
                            {
                                output.Add($"[DBG] Param '{paramName}' = {coercedValue?.ToString() ?? "null"} (declared: {declaredType})");
                                ParadigmasLang.Logging.Logger.Debug($"Param '{paramName}' = {coercedValue} (declared: {declaredType})");
                            }
                        }
                    }
                }
                if (VerboseMode)
                {
                    output.Add($"[DBG] Executing body of '{fname}'");
                    ParadigmasLang.Logging.Logger.Debug($"Executing body of '{fname}'");
                }
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
                    if (VerboseMode)
                    {
                        output.Add($"[DBG] Function '{fname}' returned: {result}");
                        ParadigmasLang.Logging.Logger.Debug($"Function '{fname}' returned: {result}");
                    }
                }

                if (VerboseMode)
                {
                    output.Add($"[DBG] Finished invocation of '{fname}' (returning)");
                    ParadigmasLang.Logging.Logger.Debug($"Finished invocation of '{fname}' (returning)");
                }

                return result;
            }
            finally
            {
                currentCallDepth--;
                // restore to previous scope captured in oldScope
                currentScope = oldScope;
            }
        }

        throw new Exception($"Function '{fname}' not found at runtime");
    }

    private object? ExecuteReturn(Node node)
    {
        // Return may have an expression child
        object? val = null;
        if (node.Children.Count > 0)
            val = ExecuteNode(node.Children[0]);
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
