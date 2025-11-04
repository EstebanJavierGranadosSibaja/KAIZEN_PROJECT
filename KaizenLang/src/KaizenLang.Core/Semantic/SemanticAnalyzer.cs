using System;
using System.Collections.Generic;
using System.Linq;

namespace ParadigmasLang;

public class SemanticAnalyzer
{

    private readonly Stack<SymbolTable> scopes = new();
    private readonly Dictionary<string, FunctionSignature> functions = new(StringComparer.OrdinalIgnoreCase);
    private readonly Diagnostics diagnostics = new();
    private TypeResolver? typeResolver;
    private DeclarationChecker? declarationChecker;
    private CollectionValidator? collectionValidator;

    private readonly Dictionary<string, int> builtins = new(StringComparer.OrdinalIgnoreCase)
    {
    { ReservedWords.INPUT, 0 },
    { ReservedWords.OUTPUT, -1 },
    { "print", -1 },
    { "length", 1 },
    };

    public List<string> AnalyzeProgram(Node ast)
    {
    diagnostics.Clear();
        scopes.Clear();
        functions.Clear();
    PushScope();
        foreach (var kv in builtins)
        {
            functions[kv.Key] = new FunctionSignature { Name = kv.Key, Arity = kv.Value, IsBuiltin = true };
        }
    if (functions.ContainsKey(ReservedWords.INPUT)) functions[ReservedWords.INPUT].ReturnType = TypeWords.GRIMOIRE;
    if (functions.ContainsKey("length")) functions["length"].ReturnType = TypeWords.GEAR;
    if (functions.ContainsKey(ReservedWords.OUTPUT)) functions[ReservedWords.OUTPUT].ReturnType = ReservedWords.VOID;
        if (functions.ContainsKey("print")) functions["print"].ReturnType = "void";
    typeResolver = new TypeResolver(scopes, functions, builtins, diagnostics);
    declarationChecker = new DeclarationChecker(scopes, functions, builtins, typeResolver, diagnostics, VisitNode);
    collectionValidator = new CollectionValidator(typeResolver, diagnostics);
        foreach (var child in ast.Children)
        {
            VisitTopLevel(child);
        }
        ValidateAllIdentifiers(ast);
        return diagnostics.GetUnique();
    }

    private void ValidateAllIdentifiers(Node root)
    {
        if (root == null) return;
        foreach (var child in root.Children)
            ValidateNodeWithScope(child);
    }

    private void ValidateNodeWithScope(Node node)
    {
        if (node == null) return;

        if (string.Equals(node.Type, "Function", StringComparison.OrdinalIgnoreCase) || string.Equals(node.Type, "FunctionDeclaration", StringComparison.OrdinalIgnoreCase))
        {
            PushScope();
            var paramsNode = node.FindChild("Parameters");
            if (paramsNode != null)
            {
                foreach (var p in paramsNode.Children)
                {
                    var id = p.FindChild("Identifier") ?? p.FindChild("IDENTIFIER");
                    var pname = SemanticUtils.ExtractIdentifierName(id) ?? id?.Type;
                    var typeNode = p.Children.Count > 0 ? p.Children[0] : null;
                        var ptype = typeNode != null && typeNode.Children.Count>0 ? typeNode.Children[0].Type : typeNode?.Type ?? string.Empty;
                        if (!string.IsNullOrEmpty(pname))
                            scopes.Peek().DeclareVariable(pname, ptype, p.Line);
                }
            }

            var body = node.FindChild("Body") ?? node.FindChild("Block");
            if (body != null)
                ValidateIdentifiersInExpression(body);

            PopScope();
            return;
        }

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
                collectionValidator?.CheckCollectionInitializer(node);
                break;
            case "Function":
            case "FunctionDeclaration":
                declarationChecker?.RegisterFunction(node);
                break;
            default:
                VisitNode(node);
                break;
        }
    }

    private void VisitNode(Node node)
    {
        if (node == null)
            return;

        if (string.Equals(node.Type, "If", StringComparison.OrdinalIgnoreCase)
            || string.Equals(node.Type, "While", StringComparison.OrdinalIgnoreCase)
            || string.Equals(node.Type, "For", StringComparison.OrdinalIgnoreCase)
            || string.Equals(node.Type, "DoWhile", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var c in node.Children)
                VisitNode(c);
            return;
        }

        switch (node.Type)
        {
            case "VariableDeclaration":
                declarationChecker?.RegisterVariable(node);
                collectionValidator?.CheckCollectionInitializer(node);
                break;
            case "Assignment":
                var id = node.FindChild("Identifier") ?? node.FindChild("IDENTIFIER") ?? node.FindChild("IndexAccess");
                var name = SemanticUtils.ExtractIdentifierName(id);
                if (!string.IsNullOrEmpty(name) && !SemanticUtils.IsSymbolDefined(scopes, name))
                    diagnostics.Report(node, $"Variable '{name}' not declarada");

                var rhs = node.FindChild("Expression") ?? (node.Children.Count > 1 ? node.Children[1] : null);
                if (rhs != null)
                {
                    var rhsExpr = rhs.Children.Count > 0 ? rhs.Children[0] : rhs;
                    var rhsType = typeResolver?.Resolve(rhsExpr);
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(rhsType))
                    {
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
                var sname = SemanticUtils.ExtractIdentifierName(node);
                if (!string.IsNullOrEmpty(sname) && !SemanticUtils.IsSymbolDefined(scopes, sname) && !functions.ContainsKey(sname))
                    diagnostics.Report(node, $"Variable '{sname}' not declared");
                break;
            case "FunctionCall":
                ValidateFunctionCall(node);
                break;
            default:
                foreach (var c in node.Children)
                    VisitNode(c);
                break;
        }
    }

    private void ValidateFunctionCall(Node fnCallNode)
    {
        var fnameNode = fnCallNode.FindChild("FunctionName");
    var fname = SemanticUtils.ExtractIdentifierName(fnameNode) ?? fnameNode?.Children.FirstOrDefault()?.Type;

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
            diagnostics.Report(fnCallNode, $"Function '{fname}' is not defined");
        }

        if (argsNode != null)
        {
            foreach (var a in argsNode.Children)
                VisitNode(a);
        }
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

    private class SymbolInfo
    {
        public string Name { get; set; } = string.Empty;
        public SymbolKind Kind { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsInitialized { get; set; }
    }

    private enum SymbolKind { Variable, Function }
    private bool CanAssign(string targetType, string sourceType)
    {
        if (SemanticUtils.IsNullLiteralType(sourceType))
        {
            if (SemanticUtils.IsNullLiteralType(targetType))
                return true;
            return SemanticUtils.SupportsNullAssignment(targetType);
        }

        if (string.Equals(targetType, sourceType, StringComparison.OrdinalIgnoreCase))
            return true;

        if (string.Equals(sourceType, TypeWords.GEAR, StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(targetType, TypeWords.SHINKAI, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(targetType, TypeWords.BANKAI, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        if (string.Equals(sourceType, TypeWords.SHINKAI, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(targetType, TypeWords.BANKAI, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }
    private void ValidateIdentifiersInExpression(Node? expr)
    {
        if (expr == null) return;

        if (expr.Type == "IDENTIFIER" || expr.Type == "Identifier" || expr.Type == "Identifier" )
        {
            var name = SemanticUtils.ExtractIdentifierName(expr);
                if (!string.IsNullOrEmpty(name) && !SemanticUtils.IsSymbolDefined(scopes, name) && !functions.ContainsKey(name) && !builtins.ContainsKey(name))
            {
                diagnostics.Report(expr, $"Variable '{name}' no declarada");
            }
            return;
        }

        if (expr.Type == "FunctionCall")
        {
            var fname = SemanticUtils.ExtractIdentifierName(expr.FindChild("FunctionName"));
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
}
