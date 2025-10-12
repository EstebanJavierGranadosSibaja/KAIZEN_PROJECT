using System;
using System.Collections.Generic;
using System.Linq;

namespace ParadigmasLang;

public class DeclarationChecker
{
    private readonly Stack<SymbolTable> scopes;
    private readonly IDictionary<string, FunctionSignature> functions;
    private readonly Dictionary<string, int> builtins;
    private readonly TypeResolver typeResolver;
    private readonly Diagnostics diagnostics;
    private readonly Action<Node> visitNodeCallback;

    public DeclarationChecker(Stack<SymbolTable> scopes,
                              IDictionary<string, FunctionSignature> functions,
                              Dictionary<string,int> builtins,
                              TypeResolver typeResolver,
                              Diagnostics diagnostics,
                              Action<Node> visitNodeCallback)
    {
        this.scopes = scopes;
        this.functions = functions;
        this.builtins = builtins;
        this.typeResolver = typeResolver;
        this.diagnostics = diagnostics;
        this.visitNodeCallback = visitNodeCallback;
    }

    public void RegisterVariable(Node node)
    {
        // Expected structure: VariableDeclaration -> Type, Identifier
        var nameNode = node.FindChild("Identifier") ?? node.FindChild("IDENTIFIER");
        if (nameNode == null)
        {
            diagnostics.Report(node, "Variable declaration missing identifier");
            return;
        }

        var varName = ExtractIdentifierName(nameNode);
        if (string.IsNullOrEmpty(varName))
        {
            diagnostics.Report(node, "Variable declaration has empty name");
            return;
        }

        var current = scopes.Peek();
        if (!current.DeclareVariable(varName, string.Empty, nameNode.Line))
        {
            diagnostics.Report(node, $"Variable '{varName}' already declared in this scope");
            return;
        }

        // determine declared type wrapper (e.g., 'array' or 'matrix') and element type
        var typeNode = node.Children[0];
        string wrapperType = typeNode.Type ?? string.Empty; // e.g., 'array' or 'integer'
        string declaredType = string.Empty; // primary declared type for non-collection (e.g., 'integer')
        string declaredElem = string.Empty; // element type for array/matrix declarations
        if (typeNode.Children.Count > 0)
        {
            declaredElem = typeNode.Children[0].Type ?? string.Empty;
        }
        // If not a wrapper like array/matrix, declaredType is the wrapperType itself
        if (!string.IsNullOrEmpty(wrapperType) && !wrapperType.StartsWith("array", StringComparison.OrdinalIgnoreCase) && !wrapperType.StartsWith("matrix", StringComparison.OrdinalIgnoreCase))
            declaredType = wrapperType;

        // Do not register the variable in the current scope yet: validate initializer first
        // If initializer present, validate identifiers used and types
        bool initializerHasErrors = false;

        // Enforce strict typing: arrays/matrices must include an explicit element type
        if ((wrapperType.StartsWith("array", StringComparison.OrdinalIgnoreCase) || wrapperType.StartsWith("matrix", StringComparison.OrdinalIgnoreCase))
                && string.IsNullOrEmpty(declaredElem))
        {
            diagnostics.Report(node, "Declaración de array/matrix requiere tipo de elemento explícito");
            // Do not proceed with registration or initializer checks for malformed declaration
            return;
        }

        if (node.Children.Count > 2)
        {
            var initWrapper = node.Children[2];
            var initExpr = initWrapper.Children.Count > 0 ? initWrapper.Children[0] : initWrapper;

            // 0) Quick malformed-initializer detection (literal followed by identifier without operator)
            if (initExpr != null && initExpr.Children != null && initExpr.Children.Count >= 2)
            {
                for (int i = 0; i < initExpr.Children.Count - 1; i++)
                {
                    var a = initExpr.Children[i];
                    var b = initExpr.Children[i + 1];
                    if ((a.Type == "INT" || a.Type == "FLOAT") && (b.Type == "IDENTIFIER" || b.Type == "Identifier"))
                    {
                        Node locationNode = a ?? initExpr;
                        if (a != null && a.Children != null && a.Children.Count > 0)
                        {
                            var firstChild = a.Children[0];
                            if (firstChild != null && firstChild.Line > 0)
                                locationNode = firstChild;
                        }

                        diagnostics.Report(locationNode, $"Inicializador inválido: literal seguido de identificador sin operador (posible token pegado)");
                        initializerHasErrors = true;
                        break;
                    }
                }
            }

            // 1) Walk initializer to find identifier usages and ensure they are declared
            if (!initializerHasErrors)
                ValidateIdentifiersInExpression(initExpr);

            // 2) Resolve initializer type and compare to declared type (only if no identifier errors)
            var initType = initializerHasErrors ? null : typeResolver.Resolve(initExpr);
            if (initType != null)
            {
                // If declared as array<T> or matrix<T>
                if (wrapperType.StartsWith("array", StringComparison.OrdinalIgnoreCase) || wrapperType.StartsWith("matrix", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(declaredElem))
                    {
                        if (wrapperType.StartsWith("matrix", StringComparison.OrdinalIgnoreCase))
                        {
                            var expected = $"array<array<{declaredElem}>>";
                            if (string.Equals(initType, expected, StringComparison.OrdinalIgnoreCase))
                            {
                                // ok
                            }
                            else if (initType.StartsWith("array<", StringComparison.OrdinalIgnoreCase))
                            {
                                var inner = TypeResolver.ExtractInnerType(initType);
                                if (string.IsNullOrEmpty(inner) || !inner.StartsWith("array<", StringComparison.OrdinalIgnoreCase))
                                {
                                    diagnostics.Report(node, $"Tipo incompatible en inicialización: se esperaba 'matrix<{declaredElem}>' pero se encontró '{initType}'");
                                }
                                else
                                {
                                    var innerElem = TypeResolver.ExtractInnerType(inner);
                                    if (!string.Equals(innerElem, declaredElem, StringComparison.OrdinalIgnoreCase))
                                        diagnostics.Report(node, $"Tipo incompatible en inicialización: se esperaba 'matrix<{declaredElem}>' pero se encontró '{initType}'");
                                }
                            }
                        }
                        else if (initType.StartsWith("array<") && !string.IsNullOrEmpty(declaredElem))
                        {
                            var initElem = TypeResolver.ExtractInnerType(initType);
                            if (!string.IsNullOrEmpty(initElem) && !string.Equals(initElem, declaredElem, StringComparison.OrdinalIgnoreCase))
                                diagnostics.Report(node, $"Tipo incompatible en inicialización: se esperaba '{declaredElem}' pero se encontró '{initElem}'");
                        }
                    }

                }
                else
                {
                    if (!string.IsNullOrEmpty(declaredType) && !CanAssign(declaredType, initType))
                    {
                        diagnostics.Report(node, $"Tipo incompatible en inicialización: se esperaba '{declaredType}' pero se encontró '{initType}'");
                    }
                }
            }
        }

        // Only register variable if initializer did not have fatal errors
        if (!initializerHasErrors)
        {
            string effectiveType = declaredType;
            if (wrapperType.StartsWith("array", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(declaredElem))
                    effectiveType = $"array<{declaredElem}>";
                else
                    effectiveType = "array";
            }
            else if (wrapperType.StartsWith("matrix", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrEmpty(declaredElem))
                    effectiveType = $"array<array<{declaredElem}>>";
                else
                    effectiveType = "array<array>";
            }

            var sym = current.LookupVariable(varName);
            if (sym != null)
            {
                sym.Type = effectiveType;
                if (node.Children.Count > 2)
                    current.SetVariableValue(varName, new object()); // mark initialized
            }
        }

        // NOTE: collection initializer validation (shape/element checks) remains in the SemanticAnalyzer
    }

    public void RegisterFunction(Node node)
    {
        var nameNode = node.FindChild("FunctionName") ?? node.FindChild("Identifier") ?? node.FindChild("IDENTIFIER");
        var fnName = ExtractIdentifierName(nameNode);
        if (string.IsNullOrEmpty(fnName))
        {
            diagnostics.Report(node, "Function declaration missing name");
            return;
        }

        int arity = 0;
        var paramsNode = node.FindChild("Parameters") ?? node.FindChild("Arguments") ?? node.FindChild("PARAMETERS");
        if (paramsNode != null)
            arity = paramsNode.Children.Count;

        if (functions.ContainsKey(fnName))
        {
            diagnostics.Report(node, $"Function '{fnName}' already declared");
            return;
        }

        functions[fnName] = new FunctionSignature { Name = fnName, Arity = arity, IsBuiltin = false };

        // New scope for function body: register parameters as variables
        scopes.Push(new SymbolTable());
        if (paramsNode != null)
        {
            foreach (var p in paramsNode.Children)
            {
                var idNode = p.FindChild("Identifier") ?? p.FindChild("IDENTIFIER");
                var paramName = ExtractIdentifierName(idNode) ?? p.Value?.ToString();
                if (string.IsNullOrEmpty(paramName))
                    continue;
                var cur = scopes.Peek();
                if (cur.LookupVariable(paramName) != null)
                {
                    diagnostics.Report(p, $"Parameter '{paramName}' duplicated");
                }
                else
                {
                    cur.DeclareVariable(paramName, string.Empty, p.Line);
                }
            }
        }

        var body = node.FindChild("Body") ?? node.FindChild("Block");
        if (body != null)
            visitNodeCallback(body);

        if (scopes.Count > 0) scopes.Pop();
    }

    private string? ExtractIdentifierName(Node? idNode)
    {
        if (idNode == null)
            return null;
        if (idNode.Type == "Identifier" || idNode.Type == "IDENTIFIER")
        {
            if (idNode.Value != null)
                return idNode.Value.ToString();
            if (idNode.Children.Count > 0)
                return idNode.Children[0].Value?.ToString() ?? idNode.Children[0].Type;
        }

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

    private void ValidateIdentifiersInExpression(Node? expr)
    {
        if (expr == null) return;

        if (expr.Type == "IDENTIFIER" || expr.Type == "Identifier")
        {
            var name = ExtractIdentifierName(expr);
            if (!string.IsNullOrEmpty(name) && !IsSymbolDefined(name) && !functions.ContainsKey(name) && !builtins.ContainsKey(name))
            {
                diagnostics.Report(expr, $"Variable '{name}' no declarada");
            }
            return;
        }

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

        if (expr.Children != null)
        {
            foreach (var c in expr.Children)
                ValidateIdentifiersInExpression(c);
        }
    }

    // Determine if a value of sourceType can be assigned to a variable of targetType
    // Promotion rules (widening): integer -> float -> double
    private bool CanAssign(string targetType, string sourceType)
    {
        if (string.Equals(targetType, sourceType, StringComparison.OrdinalIgnoreCase))
            return true;

        if (string.Equals(sourceType, "integer", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(targetType, "float", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(targetType, "double", StringComparison.OrdinalIgnoreCase))
                return true;
        }

        if (string.Equals(sourceType, "float", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(targetType, "double", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }
}
