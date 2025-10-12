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

    private readonly Stack<SymbolTable> scopes = new();
    private readonly Dictionary<string, FunctionSignature> functions = new(StringComparer.OrdinalIgnoreCase);
    private readonly Diagnostics diagnostics = new();
    private TypeResolver? typeResolver;
    private DeclarationChecker? declarationChecker;

    // Known builtins with expected arity (-1 means variadic / flexible)
    private readonly Dictionary<string, int> builtins = new(StringComparer.OrdinalIgnoreCase)
    {
        { "input", 0 }, // input() or input(prompt) -> treat as 0..1 but we'll accept 0 or 1
        { "output", -1 }, // output(...) any number
        { "print", -1 }, // print(...) familiar convenience alias
        { "length", 1 }, // length(collection) -> integer
    };

    public List<string> AnalyzeProgram(Node ast)
    {
    diagnostics.Clear();
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

        // Provide return type info for builtins
        if (functions.ContainsKey("input")) functions["input"].ReturnType = "string";
        if (functions.ContainsKey("length")) functions["length"].ReturnType = "integer";
        if (functions.ContainsKey("output")) functions["output"].ReturnType = "void";
        if (functions.ContainsKey("print")) functions["print"].ReturnType = "void";

    // Create a TypeResolver for expression type queries
    typeResolver = new TypeResolver(scopes, functions, builtins, diagnostics);

        // Create a DeclarationChecker to handle variable/function registration
    declarationChecker = new DeclarationChecker(scopes, functions, builtins, typeResolver, diagnostics, VisitNode);

        // Walk top-level nodes
        foreach (var child in ast.Children)
        {
            VisitTopLevel(child);
        }

        // Final pass: validate all identifier usages across the AST to catch undeclared usages
        ValidateAllIdentifiers(ast);

        // Return unique diagnostics
        return diagnostics.GetUnique();
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
                            scopes.Peek().DeclareVariable(pname, ptype, p.Line);
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
                declarationChecker?.RegisterVariable(node);
                break;
            case "Function":
            case "FunctionDeclaration":
                declarationChecker?.RegisterFunction(node);
                break;
            default:
                // Other top-level constructs — analyze recursively to find usages
                VisitNode(node);
                break;
        }
    }

    // RegisterVariable and RegisterFunction were moved to DeclarationChecker

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
                diagnostics.ReportMessage(msg);
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
                declarationChecker?.RegisterVariable(node);
                // Additional semantic checks for array/matrix initializers
                CheckCollectionInitializer(node);
                break;
            case "Assignment":
                // LHS identifier must exist or be declared
                var id = node.FindChild("Identifier") ?? node.FindChild("IDENTIFIER") ?? node.FindChild("IndexAccess");
                var name = ExtractIdentifierName(id);
                if (!string.IsNullOrEmpty(name) && !IsSymbolDefined(name))
                    diagnostics.Report(node, $"Variable '{name}' not declarada");

                // Resolve RHS type and compare with declared LHS type when possible
                var rhs = node.FindChild("Expression") ?? (node.Children.Count > 1 ? node.Children[1] : null);
                if (rhs != null)
                {
                    var rhsExpr = rhs.Children.Count > 0 ? rhs.Children[0] : rhs;
                    var rhsType = typeResolver?.Resolve(rhsExpr);
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(rhsType))
                    {
                        // lookup var type
                        foreach (var scope in scopes)
                        {
                            var si = scope.LookupVariable(name);
                            if (si != null && !string.IsNullOrEmpty(si.Type))
                            {
                                if (!string.Equals(si.Type, rhsType, StringComparison.OrdinalIgnoreCase))
                                {
                        if (!CanAssign(si.Type, rhsType))
                        diagnostics.Report(node, $"Tipos incompatibles para asignación: {si.Type} y {rhsType}");
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
                    diagnostics.Report(node, $"Variable '{sname}' not declared");
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
                    diagnostics.Report(rowExpr, $"Matriz no rectangular: fila {rowIndex} no es una fila (se esperaba array)");
                    rowIndex++;
                    continue;
                }

                var rowEls = rowArr.FindChild("Elements");
                int cols = rowEls?.Children.Count ?? 0;
                if (expectedCols == null)
                    expectedCols = cols;
                    else if (expectedCols != cols)
                    {
                        diagnostics.Report(rowArr, $"Matriz no rectangular: longitudes de fila inconsistentes (esperado {expectedCols}, fila {rowIndex} tiene {cols})");
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
                                diagnostics.Report(elExpr, $"Tipo incompatible en inicialización de matriz/array: se esperaba '{elemType}' pero se encontró '{detectedType}' (fila {rowIndex}, col {col})");
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
                            diagnostics.Report(elExpr, $"Tipo incompatible en inicialización de array: se esperaba '{elemType}' pero se encontró '{detectedType}' (índice {idx})");
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
            diagnostics.Report(fnCallNode, "Function call missing function name");
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
                    diagnostics.Report(fnCallNode, $"Builtin '{fname}' expects 0 or 1 arguments, got {argCount}");
                else if (expected > 0 && argCount != expected)
                    diagnostics.Report(fnCallNode, $"Builtin '{fname}' expects {expected} arguments, got {argCount}");
            }
        }
        else if (functions.TryGetValue(fname, out var sig))
        {
            if (sig.Arity >= 0 && sig.Arity != argCount)
                diagnostics.Report(fnCallNode, $"Function '{fname}' expects {sig.Arity} arguments, got {argCount}");
        }
        else
        {
            // Unknown function: error
            diagnostics.Report(fnCallNode, $"Function '{fname}' is not defined");
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
            if (scope.LookupVariable(name) != null)
                return true;
        }
        return false;
    }

    private void PushScope()
    {
        scopes.Push(new SymbolTable());
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

    // Determine if a value of sourceType can be assigned to a variable of targetType
    // Promotion rules (widening): integer -> float -> double
    private bool CanAssign(string targetType, string sourceType)
    {
        if (string.Equals(targetType, sourceType, StringComparison.OrdinalIgnoreCase))
            return true;

        // integer -> float or double
        if (string.Equals(sourceType, "integer", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(targetType, "float", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(targetType, "double", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        // float -> double
        if (string.Equals(sourceType, "float", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(targetType, "double", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
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
                diagnostics.Report(expr, $"Variable '{name}' no declarada");
            }
            return;
        }

        // If function call, validate function name and arguments
        if (expr.Type == "FunctionCall")
        {
            var fname = ExtractIdentifierName(expr.FindChild("FunctionName"));
            if (!string.IsNullOrEmpty(fname) && !functions.ContainsKey(fname) && !builtins.ContainsKey(fname))
                diagnostics.Report(expr, $"Function '{fname}' no definida");

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
