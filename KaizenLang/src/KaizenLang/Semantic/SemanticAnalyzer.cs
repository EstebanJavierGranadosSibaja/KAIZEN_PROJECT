using System.Collections.Generic;
using System.Linq;

namespace ParadigmasLang
{
    public class SemanticAnalyzer
    {
        private SymbolTable currentScope;
        private List<string> errors;

        public SemanticAnalyzer()
        {
            currentScope = new SymbolTable();
            errors = new List<string>();
        }

        public List<string> AnalyzeProgram(Node root)
        {
            errors.Clear();
            AnalyzeNode(root);
            return errors;
        }

        private void AnalyzeNode(Node node)
        {
            switch (node.Type)
            {
                case "Program":
                    foreach (var child in node.Children)
                        AnalyzeNode(child);
                    break;

                case "VariableDeclaration":
                    AnalyzeVariableDeclaration(node);
                    break;

                case "Assignment":
                    AnalyzeAssignment(node);
                    break;

                case "Function":
                    AnalyzeFunction(node);
                    break;

                case "If":
                case "While":
                case "DoWhile":
                    AnalyzeConditional(node);
                    break;

                case "For":
                    AnalyzeForLoop(node);
                    break;

                case "Return":
                    AnalyzeReturn(node);
                    break;

                case "FunctionCall":
                    AnalyzeFunctionCall(node);
                    break;

                case "ExpressionStatement":
                    AnalyzeExpressionStatement(node);
                    break;

                case "Expression":
                    AnalyzeExpression(node);
                    break;

                case "ArrayLiteral":
                    AnalyzeArrayLiteral(node);
                    break;

                case "Block":
                    // Crear nuevo scope para el bloque
                    var oldScope = currentScope;
                    currentScope = new SymbolTable(currentScope);
                    foreach (var child in node.Children)
                        AnalyzeNode(child);
                    currentScope = oldScope;
                    break;

                default:
                    foreach (var child in node.Children)
                        AnalyzeNode(child);
                    break;
            }
        }

        private void AnalyzeReturn(Node node)
        {
            // Analizar la expresión de retorno si existe
            foreach (var child in node.Children)
                AnalyzeNode(child);
        }

        private void AnalyzeFunctionCall(Node node)
        {
            if (node.Children.Count > 0)
            {
                var nameNode = node.Children[0];
                if (nameNode.Children.Count > 0)
                {
                    var functionName = nameNode.Children[0].Type;
                    var symbol = currentScope.LookupVariable(functionName);
                    if (symbol == null)
                    {
                        errors.Add($"Función '{functionName}' no está declarada");
                    }
                }

                // Analizar argumentos
                if (node.Children.Count > 1)
                {
                    var argsNode = node.Children[1];
                    foreach (var arg in argsNode.Children)
                        AnalyzeNode(arg);
                }
            }
        }

        private void AnalyzeExpressionStatement(Node node)
        {
            foreach (var child in node.Children)
                AnalyzeNode(child);
        }

        private void AnalyzeVariableDeclaration(Node node)
        {
            if (node.Children.Count >= 2)
            {
                var typeNode = node.Children[0];
                var nameNode = node.Children[1];
                
                if (typeNode.Children.Count > 0 && nameNode.Children.Count > 0)
                {
                    var type = typeNode.Children[0].Type;
                    var name = nameNode.Children[0].Type;

                    // Verificar si se está intentando usar una palabra reservada como nombre de variable
                    if (IsReservedWord(name))
                    {
                        errors.Add($"Error: '{name}' es una palabra reservada y no puede usarse como nombre de variable");
                        return;
                    }

                    // Validar tipo
                    if (!TypeWords.Words.Contains(type))
                    {
                        errors.Add($"Tipo '{type}' no es válido");
                    }

                    // Si tiene inicialización, verificar compatibilidad ANTES de declarar la variable
                    string? valueType = null;
                    if (node.Children.Count > 2)
                    {
                        var valueNode = node.Children[2];
                        AnalyzeNode(valueNode);
                        
                        // Validar compatibilidad de tipos
                        valueType = GetExpressionType(valueNode);
                        if (!IsTypeCompatible(type, valueType))
                        {
                            errors.Add($"Error de tipo: No se puede asignar valor de tipo '{valueType}' a variable de tipo '{type}'");
                        }
                    }

                    // AHORA declarar la variable (después de analizar la inicialización)
                    if (!currentScope.DeclareVariable(name, type, 0))
                    {
                        errors.Add($"Variable '{name}' ya está declarada en este scope");
                    }
                }
            }
        }

        private void AnalyzeAssignment(Node node)
        {
            if (node.Children.Count >= 2)
            {
                var varNode = node.Children[0];
                var valueNode = node.Children[1];

                if (varNode.Children.Count > 0)
                {
                    var varName = varNode.Children[0].Type;
                    
                    // Verificar si se está intentando usar una palabra reservada como variable
                    if (IsReservedWord(varName))
                    {
                        errors.Add($"Error: '{varName}' es una palabra reservada y no puede usarse como nombre de variable");
                        return;
                    }
                    
                    var symbol = currentScope.LookupVariable(varName);
                    
                    if (symbol == null)
                    {
                        errors.Add($"Variable '{varName}' no está declarada");
                    }
                    else
                    {
                        AnalyzeNode(valueNode);
                        // Validar compatibilidad de tipos
                        var valueType = GetExpressionType(valueNode);
                        if (!IsTypeCompatible(symbol.Type, valueType))
                        {
                            errors.Add($"Error de tipo: No se puede asignar valor de tipo '{valueType}' a variable '{varName}' de tipo '{symbol.Type}'");
                        }
                    }
                }
            }
        }

        private void AnalyzeFunction(Node node)
        {
            if (node.Children.Count < 2) return;
            
            var typeNode = node.Children[0];
            var nameNode = node.Children[1];
            
            if (typeNode.Children.Count > 0 && nameNode.Children.Count > 0)
            {
                var returnType = typeNode.Children[0].Type;
                var functionName = nameNode.Children[0].Type;
                
                // Registrar la función en el scope actual para permitir recursión
                currentScope.DeclareVariable(functionName, $"function_{returnType}", 0);
            }
            
            // Crear nuevo scope para la función
            var oldScope = currentScope;
            currentScope = new SymbolTable(currentScope);

            // Analizar parámetros
            if (node.Children.Count > 2)
            {
                var paramsNode = node.Children[2];
                foreach (var param in paramsNode.Children)
                {
                    if (param.Type == "Param" && param.Children.Count >= 2)
                    {
                        var paramType = param.Children[0].Type;
                        var paramName = param.Children[1].Type;
                        currentScope.DeclareVariable(paramName, paramType, 0);
                    }
                }
            }

            // Analizar cuerpo de la función
            if (node.Children.Count > 3)
            {
                AnalyzeNode(node.Children[3]);
            }

            currentScope = oldScope;
        }

        private void AnalyzeConditional(Node node)
        {
            // El primer hijo es siempre la condición
            if (node.Children.Count > 0)
            {
                var conditionNode = node.Children[0];
                AnalyzeNode(conditionNode);
                string conditionType = GetExpressionType(conditionNode);

                if (conditionType == "assignment_error")
                {
                    errors.Add($"Error: No se puede usar una asignación ('{OperatorWords.ASSIGN}') como condición en un '{node.Type}'. Use '{OperatorWords.EQUAL}' para comparar.");
                }
                else if (conditionType != "bool")
                {
                    errors.Add($"La condición de un '{node.Type}' debe ser de tipo 'bool', pero se encontró '{conditionType}'");
                }

                // Analizar los bloques 'then' y 'else' (si existen)
                for (int i = 1; i < node.Children.Count; i++)
                {
                    AnalyzeNode(node.Children[i]);
                }
            }
        }

        private void AnalyzeForLoop(Node node)
        {
            // Crear nuevo scope para el for
            var oldScope = currentScope;
            currentScope = new SymbolTable(currentScope);

            // Analizar inicialización, condición e incremento
            foreach (var child in node.Children)
            {
                AnalyzeNode(child);
            }

            currentScope = oldScope;
        }

        private void AnalyzeExpression(Node node)
        {
            foreach (var child in node.Children)
            {
                if (child.Type == "TYPE")
                {
                    // Un tipo no puede aparecer en una expresión como variable
                    if (child.Children.Count > 0)
                    {
                        var typeName = child.Children[0].Type;
                        errors.Add($"Error: '{typeName}' es un tipo de dato y no puede usarse como variable en una expresión");
                    }
                }
                else if (child.Type == "IDENTIFIER")
                {
                    var varName = child.Children[0].Type;
                    
                    // Verificar si se está usando una palabra reservada como identificador
                    if (IsReservedWord(varName))
                    {
                        errors.Add($"Error: '{varName}' es una palabra reservada y no puede usarse como identificador");
                        return;
                    }
                    
                    var symbol = currentScope.LookupVariable(varName);
                    if (symbol == null)
                    {
                        errors.Add($"Variable '{varName}' no está declarada");
                    }
                }
                else
                {
                    AnalyzeNode(child);
                }
            }
        }

        private void AnalyzeArrayLiteral(Node node)
        {
            // Analizar todos los elementos del array para verificar consistencia de tipos
            if (node.Children.Count > 0)
            {
                var elementsNode = node.Children[0]; // Nodo "Elements"
                if (elementsNode.Children.Count > 0)
                {
                    string? firstElementType = null;
                    
                    // Obtener el tipo del primer elemento como referencia
                    foreach (var element in elementsNode.Children)
                    {
                        var elementType = GetExpressionType(element);
                        
                        if (firstElementType == null)
                        {
                            firstElementType = elementType;
                        }
                        else if (firstElementType != elementType)
                        {
                            // Permitir conversiones implícitas entre tipos numéricos
                            if (!AreTypesCompatibleForArray(firstElementType, elementType))
                            {
                                errors.Add($"Error: Inconsistencia de tipos en array literal. Se esperaba '{firstElementType}' pero se encontró '{elementType}'");
                            }
                        }
                        
                        // Analizar recursivamente cada elemento
                        AnalyzeNode(element);
                    }
                }
            }
        }

        private bool AreTypesCompatibleForArray(string type1, string type2)
        {
            if (type1 == type2) return true;
            
            // Permitir mezcla de tipos numéricos en arrays (se promoverá al tipo más general)
            var numericTypes = new HashSet<string> { "integer", "float", "double" };
            return numericTypes.Contains(type1) && numericTypes.Contains(type2);
        }

        private string GetExpressionType(Node node)
        {
            if (node.Type == "Expression" && node.Children.Count > 0)
            {
                // Si la expresión contiene un operador de asignación, es un error en este contexto.
                if (node.Children.Any(child => child.Type == "Operator" && child.Children.Any(op => op.Type == OperatorWords.ASSIGN)))
                {
                    return "assignment_error"; // Tipo especial para detectar asignaciones en condicionales
                }
                // De lo contrario, el tipo de la expresión es el tipo del primer operando (simplificación)
                return GetNodeType(node.Children[0]);
            }
            return GetNodeType(node);
        }

        private string GetNodeType(Node node)
        {
            switch (node.Type)
            {
                case "STRING":
                    return "string";
                case "INT":
                    return "integer";
                case "FLOAT":
                    return "float";
                case "NUMBER":
                    // Determinar si es integer, float o double basado en el valor
                    if (node.Children.Count > 0)
                    {
                        var value = node.Children[0].Type;
                        if (value.Contains('.'))
                        {
                            return "float";
                        }
                        return "integer";
                    }
                    return "integer";
                case "BOOLEAN":
                case "LITERAL":
                    // Verificar si es un literal booleano
                    if (node.Children.Count > 0)
                    {
                        var value = node.Children[0].Type;
                        if (value == "true" || value == "false")
                            return "bool";
                        if (value == "null")
                            return "null";
                    }
                    return "boolean";
                case "CHAR":
                case "CHARACTER":
                    return "char";
                case "IDENTIFIER":
                    // Buscar el tipo de la variable en la tabla de símbolos
                    if (node.Children.Count > 0)
                    {
                        var varName = node.Children[0].Type;
                        var symbol = currentScope.LookupVariable(varName);
                        return symbol?.Type ?? "unknown";
                    }
                    return "unknown";
                case "Expression":
                    if (node.Children.Count > 0)
                        return GetNodeType(node.Children[0]);
                    return "unknown";
                case "Value":
                    // Manejar nodos Value que contienen expresiones
                    if (node.Children.Count > 0)
                        return GetNodeType(node.Children[0]);
                    return "unknown";
                case "ArrayLiteral":
                    // Determinar el tipo de array basado en sus elementos
                    return GetArrayType(node);
                default:
                    return "unknown";
            }
        }

        private string GetArrayType(Node arrayNode)
        {
            if (arrayNode.Children.Count > 0)
            {
                var elementsNode = arrayNode.Children[0]; // Nodo "Elements"
                if (elementsNode.Children.Count > 0)
                {
                    // Obtener el tipo del primer elemento
                    var firstElementType = GetNodeType(elementsNode.Children[0]);
                    
                    // Determinar el tipo de array correspondiente
                    return firstElementType switch
                    {
                        "integer" => "array_integer",
                        "float" => "array_float", 
                        "double" => "array_double",
                        "string" => "array_string",
                        "bool" => "array_bool",
                        _ => "array_unknown"
                    };
                }
            }
            return "array_unknown";
        }

        private bool IsTypeCompatible(string expectedType, string actualType)
        {
            // Compatibilidad exacta
            if (expectedType == actualType)
                return true;

            // Compatibilidades específicas
            switch (expectedType)
            {
                case "double":
                    return actualType == "integer" || actualType == "float";
                case "float":
                    return actualType == "integer";
                case "string":
                    // string puede aceptar cualquier cosa para concatenación
                    return true;
                default:
                    return false;
            }
        }

        private bool IsReservedWord(string word)
        {
            // Utiliza las definiciones centralizadas
            return ReservedWords.Words.Contains(word) || LiteralWords.Words.Contains(word);
        }
    }
}
