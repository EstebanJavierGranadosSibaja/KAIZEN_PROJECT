namespace ParadigmasLang;

public partial class Interpreter
{
    private object? ExecuteExpression(Node node)
    {
        if (node.Children.Count == 0)
            return null;

        // DEBUG: Log node type
        ParadigmasLang.Logging.Logger.Debug($"ExecuteExpression: node.Type = '{node.Type}', Children = {node.Children.Count}");

        // Handle direct FunctionCall nodes (from new parser)
        if (node.Type == "FunctionCall")
        {
            // Extract function name
            string fname = string.Empty;
            Node? argsNode = null;

            if (node.Children.Count > 0)
            {
                var nameNode = node.Children[0]; // FunctionName node
                ParadigmasLang.Logging.Logger.Debug($"  nameNode.Type = '{nameNode.Type}', Children = {nameNode.Children.Count}");
                fname = SemanticUtils.ExtractIdentifierName(nameNode) ?? string.Empty;
                ParadigmasLang.Logging.Logger.Debug($"  Extracted function name: '{fname}'");
            }

            if (node.Children.Count > 1)
            {
                argsNode = node.Children[1]; // Arguments node
            }

            // Handle builtin functions
            if (string.Equals(fname, ReservedWords.OUTPUT, System.StringComparison.OrdinalIgnoreCase))
            {
                var outputValues = new List<string>();
                if (argsNode != null && argsNode.Children.Count > 0)
                {
                    foreach (var arg in argsNode.Children)
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
                if (argsNode != null && argsNode.Children.Count > 0)
                {
                    var firstArgNode = argsNode.Children[0];
                    // If argument is an identifier, treat as input(varName) -> read and assign to variable
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
                            output.Add($"Variable '{varName}' asignada con valor: {finalVal}");
                        }
                        else
                        {
                            // If variable not declared, just return the token string
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
                return ExecuteLengthBuiltin(argsNode);
            }

            // If not a builtin, try user-defined function
            return ExecuteFunctionCall(node);
        }

        // Prefer canonical FunctionCall node shape for builtins (wrapped in Expression)
        if (node.Children.Count > 0 && node.Children[0].Type == "FunctionCall")
        {
            var fn = node.Children[0];
            if (fn.Children.Count > 0)
            {
                var nameNode = fn.Children[0];
                var fname = SemanticUtils.ExtractIdentifierName(nameNode) ?? string.Empty;

                Node? argsNode = null;
                if (fn.Children.Count > 1)
                    argsNode = fn.Children[1];

                if (string.Equals(fname, ReservedWords.OUTPUT, System.StringComparison.OrdinalIgnoreCase))
                {
                    var outputValues = new List<string>();
                    if (argsNode != null && argsNode.Children.Count > 0)
                    {
                        foreach (var arg in argsNode.Children)
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
                    if (argsNode != null && argsNode.Children.Count > 0)
                    {
                        var firstArgNode = argsNode.Children[0];
                        // If argument is an identifier, treat as input(varName) -> read and assign to variable
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
                                output.Add($"Variable '{varName}' asignada con valor: {finalVal}");
                            }
                            else
                            {
                                // If variable not declared, just return the token string
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
                    return ExecuteLengthBuiltin(argsNode);
                }
            }
        }

        // NOTE: we only support the canonical FunctionCall AST shape now.
        // Older/fallback AST shapes were removed to simplify runtime behavior.

        if (node.Children.Count == 1)
            return ExecuteNode(node.Children[0]);

        // Operaciones binarias
        if (node.Children.Count >= 2)
        {
            var left = ExecuteNode(node.Children[0]);

            for (int i = 1; i < node.Children.Count; i++)
            {
                var operatorNode = node.Children[i];
                if (operatorNode.Type == "Operator" && operatorNode.Children.Count > 0)
                {
                    var op = operatorNode.Children[0].Type;

                    // El operando derecho puede estar anidado
                    Node rightOperandNode;
                    if (i + 1 < node.Children.Count)
                    {
                        rightOperandNode = node.Children[i + 1];
                    }
                    else if (operatorNode.Children.Count > 1)
                    {
                        rightOperandNode = operatorNode.Children[1];
                    }
                    else
                    {
                        throw new Exception("Falta el operando derecho para el operador " + op);
                    }

                    var right = ExecuteNode(rightOperandNode);
                    left = EvaluateOperation(left, op, right);
                    i++; // Skip next operand since we processed it
                }
            }
            return left;
        }

        return null;
    }

    private object EvaluateOperation(object? left, string op, object? right)
    {
        // Convertir valores para operaciones
        if (op == OperatorWords.EQUAL || op == OperatorWords.NOT_EQUAL)
        {
            if (left == null || right == null)
                return op == OperatorWords.EQUAL ? object.Equals(left, right) : !object.Equals(left, right);
        }

        // If either operand is a string and operator is '+', perform concatenation
        if (op == "+" && (left is string || right is string))
        {
            var l = left?.ToString() ?? "null";
            var r = right?.ToString() ?? "null";
            return l + r;
        }

        if (left is string leftStr && right is string rightStr)
        {
            switch (op)
            {
                case OperatorWords.EQUAL:
                    return leftStr == rightStr;
                case OperatorWords.NOT_EQUAL:
                    return leftStr != rightStr;
                default:
                    throw new Exception($"Operación '{op}' no soportada para strings");
            }
        }

        if (TryConvertToDouble(left, out double leftNum) && TryConvertToDouble(right, out double rightNum))
        {
            switch (op)
            {
                case OperatorWords.ADD:
                    return leftNum + rightNum;
                case OperatorWords.SUBTRACT:
                    return leftNum - rightNum;
                case OperatorWords.MULTIPLY:
                    return leftNum * rightNum;
                case OperatorWords.DIVIDE:
                    return rightNum != 0 ? leftNum / rightNum : throw new Exception("División por cero");
                case OperatorWords.MODULO:
                    return leftNum % rightNum;
                case OperatorWords.EQUAL:
                    return leftNum == rightNum;
                case OperatorWords.NOT_EQUAL:
                    return leftNum != rightNum;
                case OperatorWords.LESS:
                    return leftNum < rightNum;
                case OperatorWords.GREATER:
                    return leftNum > rightNum;
                case OperatorWords.LESS_EQUAL:
                    return leftNum <= rightNum;
                case OperatorWords.GREATER_EQUAL:
                    return leftNum >= rightNum;
                default:
                    throw new Exception($"Operación '{op}' no reconocida para números");
            }
        }

        if (left is bool leftBool && right is bool rightBool)
        {
            switch (op)
            {
                case OperatorWords.AND:
                    return leftBool && rightBool;
                case OperatorWords.OR:
                    return leftBool || rightBool;
                case OperatorWords.EQUAL:
                    return leftBool == rightBool;
                case OperatorWords.NOT_EQUAL:
                    return leftBool != rightBool;
                default:
                    throw new Exception($"Operación '{op}' no soportada para booleanos");
            }
        }

        // Operaciones unarias (ej. '!')
        if (op == OperatorWords.NOT && right is bool rightBoolUnary)
        {
            return !rightBoolUnary;
        }

        throw new Exception($"Tipos incompatibles para operación '{op}': {left?.GetType().Name} y {right?.GetType().Name}");
    }

    private bool TryConvertToDouble(object? value, out double result)
    {
        result = 0;
        if (value == null)
            return false;

        if (value is int intVal)
        {
            result = intVal;
            return true;
        }
        if (value is float floatVal)
        {
            result = floatVal;
            return true;
        }
        if (value is double doubleVal)
        {
            result = doubleVal;
            return true;
        }
        if (value is string stringVal)
        {
            return double.TryParse(stringVal, out result);
        }

        return false;
    }

    private object? ExecuteUnaryExpression(Node node)
    {
        // UnaryExpression should have 2 children: Operator and Operand
        if (node.Children.Count < 2)
            throw new Exception("UnaryExpression mal formada");

        var operatorNode = node.Children[0];
        var operandNode = node.Children[1];

        if (operatorNode.Type != "Operator" || operatorNode.Children.Count == 0)
            throw new Exception("Operador unario mal formado");

        var op = operatorNode.Children[0].Type;
        var operand = ExecuteNode(operandNode);

        // Handle unary operators
        switch (op)
        {
            case OperatorWords.SUBTRACT:
                // Unary negation for numeric types
                if (operand is int intVal)
                    return -intVal;
                if (operand is long longVal)
                    return -longVal;
                if (operand is float floatVal)
                    return -floatVal;
                if (operand is double doubleVal)
                    return -doubleVal;
                throw new Exception($"Operador unario '-' no soportado para tipo {operand?.GetType().Name}");

            case OperatorWords.NOT:
                // Logical NOT for boolean
                if (operand is bool boolVal)
                    return !boolVal;
                throw new Exception($"Operador unario '!' no soportado para tipo {operand?.GetType().Name}");

            case OperatorWords.ADD:
                // Unary plus (identity operation)
                if (operand is int || operand is long || operand is float || operand is double)
                    return operand;
                throw new Exception($"Operador unario '+' no soportado para tipo {operand?.GetType().Name}");

            default:
                throw new Exception($"Operador unario '{op}' no reconocido");
        }
    }

    private object? ExecuteLengthBuiltin(Node? argsNode)
    {
        if (argsNode == null || argsNode.Children.Count != 1)
            throw new Exception("Builtin 'length' espera exactamente 1 argumento");

        var value = ExecuteNode(argsNode.Children[0]);
        if (value == null)
            throw new Exception("No se puede calcular length de un valor null");

        if (value is string s)
            return s.Length;

        if (value is System.Collections.ICollection collection)
            return collection.Count;

        if (value is System.Collections.IEnumerable enumerable)
        {
            int count = 0;
            foreach (var _ in enumerable)
                count++;
            return count;
        }

        throw new Exception($"length no soportado para tipo {value.GetType().Name}");
    }
}
