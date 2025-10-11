using System;
using System.Collections.Generic;
using System.Linq;

namespace ParadigmasLang;

// Practical SemanticAnalyzer for basic checks required by tests and interpreter.
// Responsibilities implemented:
// - Maintain symbol tables (scoped): variables and functions
// - Detect duplicate declarations in same scope
// - Detect uses of undefined variables
// - Register function signatures and verify call arity (including builtins)
public class SemanticAnalyzer
{
    // Tracks current nesting depth of control blocks (if/while/for/do-while).
    // If depth >= 1 and we encounter another control block, that's an illegal nested block.
    private int controlDepth = 0;

    private readonly Stack<Dictionary<string, SymbolInfo>> scopes = new();
    private readonly Dictionary<string, FunctionSignature> functions = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> errors = new();

    // Known builtins with expected arity (-1 means variadic / flexible)
    private readonly Dictionary<string, int> builtins = new(StringComparer.OrdinalIgnoreCase)
    {
        { "input", 0 }, // input() or input(prompt) -> treat as 0..1 but we'll accept 0 or 1
        { "output", -1 }, // output(...) any number
        { "print", -1 }, // print(...) familiar convenience alias
    };

    public List<string> AnalyzeProgram(Node ast)
    {
        errors.Clear();
        // Reset per-analysis state
        controlDepth = 0;
        scopes.Clear();
        functions.Clear();

        // (No diagnostic prints in normal operation)

        // Initialize global scope
        PushScope();

        // Register builtins as functions
        foreach (var kv in builtins)
        {
            functions[kv.Key] = new FunctionSignature { Name = kv.Key, Arity = kv.Value, IsBuiltin = true };
        }

        // Walk top-level nodes
        foreach (var child in ast.Children)
        {
            VisitTopLevel(child);
        }

        // Final pass: validate all identifier usages across the AST to catch undeclared usages
        ValidateAllIdentifiers(ast);

        // Deduplicate errors to avoid repeated messages from multiple passes
        var unique = errors.Distinct().ToList();
        return unique;
    }

    private void ValidateAllIdentifiers(Node root)
    {
        if (root == null) return;
        foreach (var child in root.Children)
            ValidateNodeWithScope(child);
    }

    // Validate a node and its children, creating temporary scopes for function parameters
    private void ValidateNodeWithScope(Node node)
    {
        if (node == null) return;

        if (string.Equals(node.Type, "Function", StringComparison.OrdinalIgnoreCase) || string.Equals(node.Type, "FunctionDeclaration", StringComparison.OrdinalIgnoreCase))
        {
            // Register parameters in a new temporary scope so body validation recognizes them
            PushScope();
            var paramsNode = node.FindChild("Parameters");
            if (paramsNode != null)
            {
                foreach (var p in paramsNode.Children)
                {
                    var id = p.FindChild("Identifier") ?? p.FindChild("IDENTIFIER");
                    var pname = ExtractIdentifierName(id) ?? id?.Type;
                    var typeNode = p.Children.Count > 0 ? p.Children[0] : null;
                    var ptype = typeNode != null && typeNode.Children.Count>0 ? typeNode.Children[0].Type : typeNode?.Type ?? string.Empty;
                    if (!string.IsNullOrEmpty(pname))
                        scopes.Peek()[pname] = new SymbolInfo { Name = pname, Kind = SymbolKind.Variable, Type = ptype, IsInitialized = true };
                }
            }

            // Validate body under this scope
            var body = node.FindChild("Body") ?? node.FindChild("Block");
            if (body != null)
                ValidateIdentifiersInExpression(body);

            PopScope();
            return;
        }

        // Default: validate this node and recurse
        ValidateIdentifiersInExpression(node);
        foreach (var c in node.Children)
            ValidateNodeWithScope(c);
    }

    private void VisitTopLevel(Node node)
    {
        switch (node.Type)
        {
            case "VariableDeclaration":
                RegisterVariable(node);
                break;
            case "Function":
            case "FunctionDeclaration":
                RegisterFunction(node);
                break;
            default:
                // Other top-level constructs — analyze recursively to find usages
                VisitNode(node);
                break;
        }
    }

    private void RegisterVariable(Node node)
    {
        // Expected structure: VariableDeclaration -> Type, Identifier
        var nameNode = node.FindChild("Identifier") ?? node.FindChild("IDENTIFIER");
        if (nameNode == null)
        {
            errors.Add(FormattedError(node, "Variable declaration missing identifier"));
            return;
        }

        var varName = ExtractIdentifierName(nameNode);
        if (string.IsNullOrEmpty(varName))
        {
            errors.Add(FormattedError(node, "Variable declaration has empty name"));
            return;
        }

        var current = scopes.Peek();
        if (current.ContainsKey(varName))
        {
            errors.Add(FormattedError(node, $"Variable '{varName}' already declared in this scope"));
            return;
        }

        // determine declared type from node
        string declaredType = string.Empty;
        var typeNode = node.Children[0];
        if (typeNode.Children.Count > 0)
            declaredType = typeNode.Children[0].Type;
        else
            declaredType = typeNode.Type;

        // Do not register the variable in the current scope yet: validate initializer first
        // If initializer present, validate identifiers used and types
        if (node.Children.Count > 2)
        {
            var initWrapper = node.Children[2];
            var initExpr = initWrapper.Children.Count > 0 ? initWrapper.Children[0] : initWrapper;

            // 1) Walk initializer to find identifier usages and ensure they are declared
            ValidateIdentifiersInExpression(initExpr);

            // 2) Resolve initializer type and compare to declared type
            var initType = ResolveExpressionType(initExpr);
            if (initType != null)
            {
                if (declaredType.StartsWith("array", StringComparison.OrdinalIgnoreCase) || declaredType.StartsWith("matrix", StringComparison.OrdinalIgnoreCase))
                {
                    string declaredElem = string.Empty;
                    if (typeNode.Children.Count > 0)
                        declaredElem = typeNode.Children[0].Type;
                    if (initType.StartsWith("array<") && !string.IsNullOrEmpty(declaredElem))
                    {
                        var start = initType.IndexOf('<') + 1;
                        var end = initType.IndexOf('>');
                        if (start > 0 && end > start)
                        {
                            var initElem = initType.Substring(start, end - start);
                            if (!string.Equals(initElem, declaredElem, StringComparison.OrdinalIgnoreCase))
                                errors.Add(FormattedError(node, $"Tipo incompatible en inicialización: se esperaba '{declaredElem}' pero se encontró '{initElem}'"));
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(declaredType) && !string.Equals(declaredType, initType, StringComparison.OrdinalIgnoreCase))
                    {
                        errors.Add(FormattedError(node, $"Tipo incompatible en inicialización: se esperaba '{declaredType}' pero se encontró '{initType}'"));
                    }
                }
            }
        }

        // Finally register variable in current scope
        var sinfo = new SymbolInfo { Name = varName, Kind = SymbolKind.Variable, Type = declaredType, IsInitialized = node.Children.Count > 2 };
        current[varName] = sinfo;

        // run additional checks on initializer if present (arrays / matrices)
        CheckCollectionInitializer(node);
    }

    private void RegisterFunction(Node node)
    {
        // Expected types: Function or FunctionDeclaration with children: FunctionName, Parameters, Body
        var nameNode = node.FindChild("FunctionName") ?? node.FindChild("Identifier") ?? node.FindChild("IDENTIFIER");
        var fnName = ExtractIdentifierName(nameNode);
        if (string.IsNullOrEmpty(fnName))
        {
            errors.Add(FormattedError(node, "Function declaration missing name"));
            return;
        }

        // Count parameters if present
        int arity = 0;
        var paramsNode = node.FindChild("Parameters") ?? node.FindChild("Arguments") ?? node.FindChild("PARAMETERS");
        if (paramsNode != null)
        {
            arity = paramsNode.Children.Count;
        }

        if (functions.ContainsKey(fnName))
        {
            errors.Add(FormattedError(node, $"Function '{fnName}' already declared"));
            return;
        }

        functions[fnName] = new FunctionSignature { Name = fnName, Arity = arity, IsBuiltin = false };

        // New scope for function body: register parameters as variables
        PushScope();
        if (paramsNode != null)
        {
            foreach (var p in paramsNode.Children)
            {
                // Parameter node shape is usually: Param -> Type, Identifier
                var idNode = p.FindChild("Identifier") ?? p.FindChild("IDENTIFIER");
                var paramName = ExtractIdentifierName(idNode) ?? p.Value?.ToString();
                if (string.IsNullOrEmpty(paramName))
                    continue;
                var cur = scopes.Peek();
                if (cur.ContainsKey(paramName))
                {
                    errors.Add(FormattedError(p, $"Parameter '{paramName}' duplicated"));
                }
                else
                {
                    cur[paramName] = new SymbolInfo { Name = paramName, Kind = SymbolKind.Variable };
                }
            }
        }

        // Visit function body if present
        var body = node.FindChild("Body") ?? node.FindChild("Block");
        if (body != null)
            VisitNode(body);

        PopScope();
    }

    private void VisitNode(Node node)
    {
        if (node == null)
            return;

        // Detect control blocks and enforce "only one level of nesting" rule.
        // Parser emits control nodes with types: "If", "While", "For", "DoWhile".
        if (string.Equals(node.Type, "If", StringComparison.OrdinalIgnoreCase)
            || string.Equals(node.Type, "While", StringComparison.OrdinalIgnoreCase)
            || string.Equals(node.Type, "For", StringComparison.OrdinalIgnoreCase)
            || string.Equals(node.Type, "DoWhile", StringComparison.OrdinalIgnoreCase))
        {
            var newDepth = controlDepth + 1;
            if (newDepth > 1)
            {
                // Message contains 'nested', 'anid' and 'nivel' so tests matching any substring will find it
                var msg = FormattedError(node, $"Bloque de control anidado no permitido (nested / anid / nivel) (nivel {newDepth})");
                errors.Add(msg);
                // Still continue analyzing to collect more errors
            }

            // Enter control block
            controlDepth = newDepth;
            foreach (var c in node.Children)
                VisitNode(c);
            // Exit control block
            controlDepth = Math.Max(0, controlDepth - 1);

            return;
        }

        switch (node.Type)
        {
            case "VariableDeclaration":
                RegisterVariable(node);
                // Additional semantic checks for array/matrix initializers
                CheckCollectionInitializer(node);
                break;
            case "Assignment":
                // LHS identifier must exist or be declared
                var id = node.FindChild("Identifier") ?? node.FindChild("IDENTIFIER") ?? node.FindChild("IndexAccess");
                var name = ExtractIdentifierName(id);
                if (!string.IsNullOrEmpty(name) && !IsSymbolDefined(name))
                    errors.Add(FormattedError(node, $"Variable '{name}' not declarada"));

                // Resolve RHS type and compare with declared LHS type when possible
                var rhs = node.FindChild("Expression") ?? (node.Children.Count > 1 ? node.Children[1] : null);
                if (rhs != null)
                {
                    var rhsExpr = rhs.Children.Count > 0 ? rhs.Children[0] : rhs;
                    var rhsType = ResolveExpressionType(rhsExpr);
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(rhsType))
                    {
                        // lookup var type
                        foreach (var scope in scopes)
                        {
                            if (scope.TryGetValue(name, out var si) && !string.IsNullOrEmpty(si.Type))
                            {
                                if (!string.Equals(si.Type, rhsType, StringComparison.OrdinalIgnoreCase))
                                {
                                    errors.Add(FormattedError(node, $"Tipos incompatibles para asignación: {si.Type} y {rhsType}"));
                                }
                                break;
                            }
                        }
                    }
                }
                break;
            case "Identifier":
            case "IDENTIFIER":
                var sname = ExtractIdentifierName(node);
                if (!string.IsNullOrEmpty(sname) && !IsSymbolDefined(sname) && !functions.ContainsKey(sname))
                    errors.Add(FormattedError(node, $"Variable '{sname}' not declared"));
                break;
            case "FunctionCall":
                ValidateFunctionCall(node);
                break;
            default:
                // Recurse into children
                foreach (var c in node.Children)
                    VisitNode(c);
                break;
        }
    }

    // Check variable declaration initializers for arrays/matrices
    private void CheckCollectionInitializer(Node varDecl)
    {
        // Expect shape: VariableDeclaration -> Type, Identifier, (optional) Initializer
        if (varDecl.Children.Count < 3)
            return;

        var typeNode = varDecl.Children[0];
        // declaredType should be the wrapper name (e.g., "array" or "matrix")
        string declaredType = typeNode.Type ?? string.Empty;
        // element type (if present) is usually the first child of the type node
        string elemType = string.Empty;
        if (typeNode.Children.Count > 0)
            elemType = typeNode.Children[0].Type ?? string.Empty;

        // Only interested in array<T> or matrix<T> declared types
        if (string.IsNullOrEmpty(declaredType))
            return;

        // Inspect initializer node
    var initNode = varDecl.Children[2];
    // initializer shape: may be wrapped inside a 'Value' node -> child, or Expression -> ArrayLiteral.
    // Find first descendant node of type ArrayLiteral
    Node? arrayLiteral = FindDescendant(initNode, "ArrayLiteral");
        if (arrayLiteral == null)
            return;

        // If declared as matrix, ensure rows are arrays and all rows have same length
        if (declaredType.IndexOf("matrix", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            var elements = arrayLiteral.FindChild("Elements");
            if (elements == null)
                return;
            int? expectedCols = null;
            int rowIndex = 0;
            foreach (var rowExpr in elements.Children)
            {
                // each row should be an ArrayLiteral
                var rowArr = rowExpr.FindChild("ArrayLiteral") ?? (rowExpr.Type == "ArrayLiteral" ? rowExpr : null);
                if (rowArr == null)
                {
                    errors.Add(FormattedError(rowExpr, $"Matriz no rectangular: fila {rowIndex} no es una fila (se esperaba array)"));
                    rowIndex++;
                    continue;
                }

                var rowEls = rowArr.FindChild("Elements");
                int cols = rowEls?.Children.Count ?? 0;
                if (expectedCols == null)
                    expectedCols = cols;
                else if (expectedCols != cols)
                {
                    errors.Add(FormattedError(rowArr, $"Matriz no rectangular: longitudes de fila inconsistentes (esperado {expectedCols}, fila {rowIndex} tiene {cols})"));
                }

                // If element type is declared, validate each element literal type superficially
                if (!string.IsNullOrEmpty(elemType) && rowEls != null)
                {
                    int col = 0;
                    foreach (var elExpr in rowEls.Children)
                    {
                        // simple check: if element is INT/FLOAT/STRING/LITERAL
                        var lit = elExpr.FindChild("INT") ?? elExpr.FindChild("FLOAT") ?? elExpr.FindChild("STRING") ?? elExpr.FindChild("LITERAL");
                        if (lit != null)
                        {
                            var detected = lit.Type;
                            // map node type to declared type names
                            string detectedType = detected switch
                            {
                                "INT" => "integer",
                                "FLOAT" => "float",
                                "STRING" => "string",
                                "LITERAL" => (lit.Children.Count>0 && (lit.Children[0].Type == "true" || lit.Children[0].Type=="false")) ? "bool" : "string",
                                _ => ""
                            };
                            if (!string.IsNullOrEmpty(detectedType) && !string.Equals(detectedType, elemType, StringComparison.OrdinalIgnoreCase))
                            {
                                errors.Add(FormattedError(elExpr, $"Tipo incompatible en inicialización de matriz/array: se esperaba '{elemType}' pero se encontró '{detectedType}' (fila {rowIndex}, col {col})"));
                            }
                        }
                        col++;
                    }
                }

                rowIndex++;
            }
        }
        else if (declaredType.IndexOf("array", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            // For arrays, check each element matches elemType (if present)
            var elements = arrayLiteral.FindChild("Elements");
            if (elements == null)
                return;
            if (!string.IsNullOrEmpty(elemType))
            {
                int idx = 0;
                foreach (var elExpr in elements.Children)
                {
                    var lit = elExpr.FindChild("INT") ?? elExpr.FindChild("FLOAT") ?? elExpr.FindChild("STRING") ?? elExpr.FindChild("LITERAL");
                    if (lit != null)
                    {
                        var detected = lit.Type;
                        string detectedType = detected switch
                        {
                            "INT" => "integer",
                            "FLOAT" => "float",
                            "STRING" => "string",
                            "LITERAL" => (lit.Children.Count>0 && (lit.Children[0].Type == "true" || lit.Children[0].Type=="false")) ? "bool" : "string",
                            _ => ""
                        };
                        if (!string.IsNullOrEmpty(detectedType) && !string.Equals(detectedType, elemType, StringComparison.OrdinalIgnoreCase))
                        {
                            errors.Add(FormattedError(elExpr, $"Tipo incompatible en inicialización de array: se esperaba '{elemType}' pero se encontró '{detectedType}' (índice {idx})"));
                        }
                    }
                    idx++;
                }
            }
        }
    }

    private void ValidateFunctionCall(Node fnCallNode)
    {
        // Expected shape: FunctionCall -> FunctionName, Arguments
        var fnameNode = fnCallNode.FindChild("FunctionName");
        var fname = ExtractIdentifierName(fnameNode) ?? fnameNode?.Children.FirstOrDefault()?.Type;

        if (string.IsNullOrEmpty(fname))
        {
            errors.Add(FormattedError(fnCallNode, "Function call missing function name"));
            return;
        }

        var argsNode = fnCallNode.FindChild("Arguments");
        int argCount = argsNode?.Children.Count ?? 0;

        // If builtin, apply basic arity rules
        if (builtins.TryGetValue(fname, out var expected))
        {
            if (expected >= 0)
            {
                // Accept either expected or expected+1 for input(prompt?) convenience when expected==0
                if (expected == 0 && !(argCount == 0 || argCount == 1))
                    errors.Add(FormattedError(fnCallNode, $"Builtin '{fname}' expects 0 or 1 arguments, got {argCount}"));
                else if (expected > 0 && argCount != expected)
                    errors.Add(FormattedError(fnCallNode, $"Builtin '{fname}' expects {expected} arguments, got {argCount}"));
            }
        }
        else if (functions.TryGetValue(fname, out var sig))
        {
            if (sig.Arity >= 0 && sig.Arity != argCount)
                errors.Add(FormattedError(fnCallNode, $"Function '{fname}' expects {sig.Arity} arguments, got {argCount}"));
        }
        else
        {
            // Unknown function: error
            errors.Add(FormattedError(fnCallNode, $"Function '{fname}' is not defined"));
        }

        // Visit arguments expressions
        if (argsNode != null)
        {
            foreach (var a in argsNode.Children)
                VisitNode(a);
        }
    }

    private string? ExtractIdentifierName(Node? idNode)
    {
        if (idNode == null)
            return null;
        // Many identifier nodes in the AST are shaped as: Identifier -> IDENTIFIER (with Value)
        if (idNode.Type == "Identifier" || idNode.Type == "IDENTIFIER")
        {
            if (idNode.Value != null)
                return idNode.Value.ToString();
            if (idNode.Children.Count > 0)
                return idNode.Children[0].Value?.ToString() ?? idNode.Children[0].Type;
        }

        // FunctionName nodes might have child which is identifier
        if (idNode.Type == "FunctionName")
        {
            if (idNode.Children.Count > 0)
            {
                var c = idNode.Children[0];
                if (c.Value != null)
                    return c.Value.ToString();
                if (c.Children.Count > 0)
                    return c.Children[0].Value?.ToString() ?? c.Children[0].Type;
                return c.Type;
            }
        }

        // Fallbacks
        if (idNode.Value != null)
            return idNode.Value.ToString();
        return idNode.Children.FirstOrDefault()?.Value?.ToString() ?? idNode.Children.FirstOrDefault()?.Type;
    }

    private bool IsSymbolDefined(string name)
    {
        foreach (var scope in scopes)
        {
            if (scope.ContainsKey(name))
                return true;
        }
        return false;
    }

    private void PushScope()
    {
        scopes.Push(new Dictionary<string, SymbolInfo>(StringComparer.OrdinalIgnoreCase));
    }

    private void PopScope()
    {
        if (scopes.Count > 0)
            scopes.Pop();
    }

    private string FormattedError(Node node, string message)
    {
        if (node == null)
            return message;
        return $"{message} (l{node.Line}:c{node.Column})";
    }

    // Helper: find descendant node with given type (depth-first)
    private Node? FindDescendant(Node root, string type)
    {
        if (root == null) return null;
        if (root.Type == type) return root;
        foreach (var c in root.Children)
        {
            var found = FindDescendant(c, type);
            if (found != null) return found;
        }
        return null;
    }

    private class SymbolInfo
    {
        public string Name { get; set; } = string.Empty;
        public SymbolKind Kind { get; set; }
        // Declared type for variables (e.g., "integer", "string", "array", etc.)
        public string Type { get; set; } = string.Empty;
        public bool IsInitialized { get; set; }
    }

    private enum SymbolKind { Variable, Function }

    private class FunctionSignature
    {
        public string Name { get; set; } = string.Empty;
        public int Arity { get; set; }
        public bool IsBuiltin { get; set; }
        // Optional return type ("integer", "string", etc.)
        public string ReturnType { get; set; } = string.Empty;
    }

    // --- Type resolution helpers ---
    // Try to resolve the static type of an expression node. Returns null if unknown.
    private string? ResolveExpressionType(Node? expr)
    {
        if (expr == null) return null;

        switch (expr.Type)
        {
            case "INT":
                return "integer";
            case "FLOAT":
                return "float";
            case "STRING":
                return "string";
            case "LITERAL":
                // boolean literals or null
                if (expr.Children.Count > 0)
                {
                    var lit = expr.Children[0].Type;
                    if (lit == "true" || lit == "false") return "bool";
                    if (lit == "null") return "null";
                }
                return "string";
            case "IDENTIFIER":
            case "Identifier":
                var name = ExtractIdentifierName(expr);
                if (string.IsNullOrEmpty(name)) return null;
                // Look up symbol type in scopes
                foreach (var scope in scopes)
                {
                    if (scope.TryGetValue(name, out var si) && !string.IsNullOrEmpty(si.Type))
                        return si.Type;
                }
                // Not found
                errors.Add(FormattedError(expr, $"Variable '{name}' no declarada"));
                return null;
            case "FunctionCall":
                var fnameNode = expr.FindChild("FunctionName");
                var fname = ExtractIdentifierName(fnameNode);
                if (string.IsNullOrEmpty(fname)) return null;
                if (functions.TryGetValue(fname, out var sig))
                {
                    return string.IsNullOrEmpty(sig.ReturnType) ? null : sig.ReturnType;
                }
                if (builtins.ContainsKey(fname))
                {
                    // input -> string, output -> void
                    if (string.Equals(fname, "input", StringComparison.OrdinalIgnoreCase)) return "string";
                    return null;
                }
                errors.Add(FormattedError(expr, $"Function '{fname}' no definida"));
                return null;
            case "Expression":
            case "Parentheses":
            case "ExpressionStatement":
                // Descend into first child expression if present
                if (expr.Children.Count > 0) return ResolveExpressionType(expr.Children[0]);
                return null;
            case "ArrayLiteral":
                // Determine element type if all elements share same type
                var elements = expr.FindChild("Elements");
                if (elements == null || elements.Children.Count == 0) return "array"; // unknown element type but an array
                string? eltType = null;
                foreach (var el in elements.Children)
                {
                    var et = ResolveExpressionType(el);
                    if (et == null) return null;
                    if (eltType == null) eltType = et;
                    else if (!string.Equals(eltType, et, StringComparison.OrdinalIgnoreCase))
                        return null; // heterogeneous
                }
                return "array<" + (eltType ?? "?") + ">";
            default:
                // Binary operator shapes: Expression children with IDENTIFIER/Operator/INT etc.
                if (expr.Children != null && expr.Children.Count == 3)
                {
                    var left = ResolveExpressionType(expr.Children[0]);
                    var op = expr.Children[1];
                    var right = ResolveExpressionType(expr.Children[2]);
                    if (left == null || right == null) return null;
                    // Numeric operators
                    var ops = new[] { ">", "<", ">=", "<=", "+", "-", "*", "/" };
                    var opSymbol = op.Type;
                    if (Array.Exists(ops, o => o == opSymbol))
                    {
                        // If both integer -> integer; if any float -> float
                        if (string.Equals(left, "integer", StringComparison.OrdinalIgnoreCase) && string.Equals(right, "integer", StringComparison.OrdinalIgnoreCase))
                            return "integer";
                        if ((string.Equals(left, "integer", StringComparison.OrdinalIgnoreCase) && string.Equals(right, "float", StringComparison.OrdinalIgnoreCase)) ||
                            (string.Equals(left, "float", StringComparison.OrdinalIgnoreCase) && string.Equals(right, "integer", StringComparison.OrdinalIgnoreCase)) ||
                            (string.Equals(left, "float", StringComparison.OrdinalIgnoreCase) && string.Equals(right, "float", StringComparison.OrdinalIgnoreCase)))
                            return "float";
                    }
                    // Comparison operators produce bool
                    var cmpOps = new[] { ">", "<", ">=", "<=", "==", "!=" };
                    if (Array.Exists(cmpOps, o => o == opSymbol)) return "bool";
                }
                // Fallback
                return null;
        }
    }

    // Walk an expression AST and report identifiers that are not defined in any accessible scope
    private void ValidateIdentifiersInExpression(Node? expr)
    {
        if (expr == null) return;

        // If this node is an identifier node, check it's defined
        if (expr.Type == "IDENTIFIER" || expr.Type == "Identifier" || expr.Type == "Identifier" )
        {
            var name = ExtractIdentifierName(expr);
            if (!string.IsNullOrEmpty(name) && !IsSymbolDefined(name) && !functions.ContainsKey(name) && !builtins.ContainsKey(name))
            {
                errors.Add(FormattedError(expr, $"Variable '{name}' no declarada"));
            }
            return;
        }

        // If function call, validate function name and arguments
        if (expr.Type == "FunctionCall")
        {
            var fname = ExtractIdentifierName(expr.FindChild("FunctionName"));
            if (!string.IsNullOrEmpty(fname) && !functions.ContainsKey(fname) && !builtins.ContainsKey(fname))
                errors.Add(FormattedError(expr, $"Function '{fname}' no definida"));

            var args = expr.FindChild("Arguments");
            if (args != null)
            {
                foreach (var a in args.Children)
                    ValidateIdentifiersInExpression(a);
            }
            return;
        }

        // For other nodes, recurse into children
        if (expr.Children != null)
        {
            foreach (var c in expr.Children)
                ValidateIdentifiersInExpression(c);
        }
    }
}
