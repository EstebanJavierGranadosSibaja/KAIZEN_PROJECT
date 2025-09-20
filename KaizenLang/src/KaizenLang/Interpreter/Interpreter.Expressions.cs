namespace ParadigmasLang;

public partial class Interpreter
{
    private object? ExecuteExpression(Node node)
    {
        if (node.Children.Count == 0)
            return null;

        // Prefer canonical FunctionCall node shape for builtins
        if (node.Children.Count > 0 && node.Children[0].Type == "FunctionCall")
        {
            var fn = node.Children[0];
            if (fn.Children.Count > 0)
            {
                var nameNode = fn.Children[0];
                if (nameNode.Children.Count > 0)
                {
                    var fnameObj = nameNode.Children[0].Type;
                    var fname = fnameObj;

                    // Arguments node is expected as second child
                    Node? argsNode = null;
                    if (fn.Children.Count > 1)
                        argsNode = fn.Children[1];

                    if (fname == "output")
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

                    if (fname == "input")
                    {
                        string? prompt = null;
                        if (argsNode != null && argsNode.Children.Count > 0)
                        {
                            var firstArg = ExecuteNode(argsNode.Children[0]);
                            prompt = firstArg?.ToString();
                        }
                        var token = ReadNextInputToken(prompt);
                        return token;
                    }
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
        if (left is string leftStr && right is string rightStr)
        {
            switch (op)
            {
                case "+":
                    return leftStr + rightStr;
                case "==":
                    return leftStr == rightStr;
                case "!=":
                    return leftStr != rightStr;
                default:
                    throw new Exception($"Operación '{op}' no soportada para strings");
            }
        }

        if (TryConvertToDouble(left, out double leftNum) && TryConvertToDouble(right, out double rightNum))
        {
            switch (op)
            {
                case "+":
                    return leftNum + rightNum;
                case "-":
                    return leftNum - rightNum;
                case "*":
                    return leftNum * rightNum;
                case "/":
                    return rightNum != 0 ? leftNum / rightNum : throw new Exception("División por cero");
                case "%":
                    return leftNum % rightNum;
                case "==":
                    return leftNum == rightNum;
                case "!=":
                    return leftNum != rightNum;
                case "<":
                    return leftNum < rightNum;
                case ">":
                    return leftNum > rightNum;
                case "<=":
                    return leftNum <= rightNum;
                case ">=":
                    return leftNum >= rightNum;
                default:
                    throw new Exception($"Operación '{op}' no reconocida para números");
            }
        }

        if (left is bool leftBool && right is bool rightBool)
        {
            switch (op)
            {
                case "&&":
                    return leftBool && rightBool;
                case "||":
                    return leftBool || rightBool;
                case "==":
                    return leftBool == rightBool;
                case "!=":
                    return leftBool != rightBool;
                default:
                    throw new Exception($"Operación '{op}' no soportada para booleanos");
            }
        }

        // Operaciones unarias (ej. '!')
        if (op == "!" && right is bool rightBoolUnary)
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
}
