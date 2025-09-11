
namespace ParadigmasLang
{
    public class Interpreter
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
                    return literal switch
                    {
                        "true" => true,
                        "false" => false,
                        "null" => null,
                        _ => literal
                    };

                case "IDENTIFIER":
                    var varName = node.Children[0].Type;
                    var symbol = currentScope.LookupVariable(varName);
                    if (symbol == null)
                    {
                        throw new Exception($"Variable '{varName}' no está declarada");
                    }
                    return symbol.Value;

                default:
                    foreach (var child in node.Children)
                        ExecuteNode(child);
                    return null;
            }
        }

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

        private object? ExecuteExpression(Node node)
        {
            if (node.Children.Count == 0)
                return null;

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
                        var right = i + 1 < node.Children.Count ? 
                                   ExecuteNode(node.Children[i + 1]) : 
                                   ExecuteNode(operatorNode.Children[1]);

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
                return op switch
                {
                    "+" => leftStr + rightStr,
                    "==" => leftStr == rightStr,
                    "!=" => leftStr != rightStr,
                    _ => throw new Exception($"Operación '{op}' no soportada para strings")
                };
            }

            if (TryConvertToDouble(left, out double leftNum) && TryConvertToDouble(right, out double rightNum))
            {
                return op switch
                {
                    "+" => leftNum + rightNum,
                    "-" => leftNum - rightNum,
                    "*" => leftNum * rightNum,
                    "/" => rightNum != 0 ? leftNum / rightNum : throw new Exception("División por cero"),
                    "%" => leftNum % rightNum,
                    "==" => leftNum == rightNum,
                    "!=" => leftNum != rightNum,
                    "<" => leftNum < rightNum,
                    ">" => leftNum > rightNum,
                    "<=" => leftNum <= rightNum,
                    ">=" => leftNum >= rightNum,
                    _ => throw new Exception($"Operación '{op}' no reconocida")
                };
            }

            if (left is bool leftBool && right is bool rightBool)
            {
                return op switch
                {
                    "&&" => leftBool && rightBool,
                    "||" => leftBool || rightBool,
                    "==" => leftBool == rightBool,
                    "!=" => leftBool != rightBool,
                    _ => throw new Exception($"Operación '{op}' no soportada para booleanos")
                };
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
}
