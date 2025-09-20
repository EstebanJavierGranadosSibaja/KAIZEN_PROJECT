namespace ParadigmasLang
{
    public partial class Interpreter
    {
        private object? ExecuteExpression(Node node)
        {
            if (node.Children.Count == 0)
                return null;

            // Caso especial: llamada a función como 'output("hola")'
            if (node.Children.Count > 1 && node.Children[0].Type == "IDENTIFIER")
            {
                var functionNameNode = node.Children[0];
                var functionName = ExecuteNode(functionNameNode);

                if (functionName is string name && name == "output")
                {
                    if (node.Children[1].Type == "FunctionCall")
                    {
                        var argsNode = node.Children[1].Children[0]; // El nodo de argumentos
                        var outputValues = new List<string>();
                        foreach (var arg in argsNode.Children)
                        {
                            var val = ExecuteNode(arg);
                            outputValues.Add(val?.ToString() ?? "null");
                        }
                        output.Add(string.Join(" ", outputValues));
                        return null; // La llamada a output no devuelve un valor
                    }
                }
            }

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
                    case "+": return leftStr + rightStr;
                    case "==": return leftStr == rightStr;
                    case "!=": return leftStr != rightStr;
                    default: throw new Exception($"Operación '{op}' no soportada para strings");
                }
            }

            if (TryConvertToDouble(left, out double leftNum) && TryConvertToDouble(right, out double rightNum))
            {
                switch (op)
                {
                    case "+": return leftNum + rightNum;
                    case "-": return leftNum - rightNum;
                    case "*": return leftNum * rightNum;
                    case "/": return rightNum != 0 ? leftNum / rightNum : throw new Exception("División por cero");
                    case "%": return leftNum % rightNum;
                    case "==": return leftNum == rightNum;
                    case "!=": return leftNum != rightNum;
                    case "<": return leftNum < rightNum;
                    case ">": return leftNum > rightNum;
                    case "<=": return leftNum <= rightNum;
                    case ">=": return leftNum >= rightNum;
                    default: throw new Exception($"Operación '{op}' no reconocida para números");
                }
            }

            if (left is bool leftBool && right is bool rightBool)
            {
                switch (op)
                {
                    case "&&": return leftBool && rightBool;
                    case "||": return leftBool || rightBool;
                    case "==": return leftBool == rightBool;
                    case "!=": return leftBool != rightBool;
                    default: throw new Exception($"Operación '{op}' no soportada para booleanos");
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
            if (value == null) return false;

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
}
