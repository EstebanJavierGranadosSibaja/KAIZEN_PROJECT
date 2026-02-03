using System;
using System.Collections.Generic;

namespace ParadigmasLang
{
    // Responsible for resolving static types of expression AST nodes.
    public class TypeResolver
    {
        private readonly IEnumerable<SymbolTable> _scopes;
        private readonly IDictionary<string, FunctionSignature> _functions;
        private readonly IDictionary<string, int> _builtins;
        private readonly Diagnostics _diagnostics;

        public TypeResolver(IEnumerable<SymbolTable> scopes, IDictionary<string, FunctionSignature> functions, IDictionary<string, int> builtins, Diagnostics diagnostics)
        {
            _scopes = scopes;
            _functions = functions;
            _builtins = builtins;
            _diagnostics = diagnostics;
        }

        public string? Resolve(Node? expr)
        {
            if (expr == null) return null;

            switch (expr.Type)
            {
                case "INT": return TypeWords.GEAR;
                case "FLOAT": return TypeWords.SHINKAI;
                case "STRING": return TypeWords.GRIMOIRE;
                case "LITERAL":
                    if (expr.Children.Count > 0)
                    {
                        var lit = expr.Children[0].Type;
                        if (lit == LiteralWords.TRUE || lit == LiteralWords.FALSE) return TypeWords.SHIN;
                        if (lit == LiteralWords.NULL) return LiteralWords.NULL;
                    }
                    return TypeWords.GRIMOIRE;
                case "IDENTIFIER":
                case "Identifier":
                    var name = ExtractIdentifierName(expr);
                    if (string.IsNullOrEmpty(name)) return null;
                    foreach (var scope in _scopes)
                    {
                        var si = scope.LookupVariable(name);
                        if (si != null && !string.IsNullOrEmpty(si.Type))
                            return si.Type;
                    }
                    _diagnostics.Report(expr, $"Variable '{name}' no declarada");
                    return null;
                case "FunctionCall":
                    var fnameNode = expr.FindChild("FunctionName");
                    var fname = ExtractIdentifierName(fnameNode);
                    if (string.IsNullOrEmpty(fname)) return null;
                    if (_functions.TryGetValue(fname, out var sig))
                        return string.IsNullOrEmpty(sig.ReturnType) ? null : sig.ReturnType;
                    if (_builtins.ContainsKey(fname))
                    {
                        if (string.Equals(fname, ReservedWords.INPUT, StringComparison.OrdinalIgnoreCase)) return TypeWords.GRIMOIRE;
                        return null;
                    }
                    _diagnostics.Report(expr, $"Function '{fname}' no definida");
                    return null;
                case "Parentheses":
                case "ExpressionStatement":
                    if (expr.Children.Count > 0) return Resolve(expr.Children[0]);
                    return null;
                case "ArrayLiteral":
                    var elements = expr.FindChild("Elements");
                    if (elements == null || elements.Children.Count == 0) return TypeWords.CHAINSAW;
                    string? eltType = null;
                    var sawNullLiteral = false;
                    foreach (var el in elements.Children)
                    {
                        var et = Resolve(el);
                        if (et == null) return null;
                        if (SemanticUtils.IsNullLiteralType(et))
                        {
                            sawNullLiteral = true;
                            continue;
                        }
                        if (eltType == null) eltType = et;
                        else if (!string.Equals(eltType, et, StringComparison.OrdinalIgnoreCase))
                            return null;
                    }
                    if (eltType == null)
                        eltType = sawNullLiteral ? LiteralWords.NULL : "?";
                    return $"{TypeWords.CHAINSAW}<{eltType}>";
                case "IndexAccess":
                    if (expr.Children != null && expr.Children.Count >= 1)
                    {
                        var target = expr.Children[0];
                        var ttype = Resolve(target);
                        if (string.IsNullOrEmpty(ttype)) return null;
                        if (ttype.StartsWith($"{TypeWords.CHAINSAW}<", StringComparison.OrdinalIgnoreCase))
                        {
                            var inner = ExtractInnerType(ttype);
                            if (!string.IsNullOrEmpty(inner)) return inner;
                            return null;
                        }
                        if (ttype.StartsWith($"{TypeWords.CHAINSAW}<{TypeWords.CHAINSAW}<", StringComparison.OrdinalIgnoreCase))
                        {
                            var inner = ExtractInnerType(ttype);
                            if (!string.IsNullOrEmpty(inner)) return inner;
                        }
                    }
                    return null;
                default:
                    if (expr.Type == "Expression")
                    {
                        if (expr.Children != null && expr.Children.Count == 1)
                            return Resolve(expr.Children[0]);
                    }
                    if (expr.Children != null && expr.Children.Count == 3)
                    {
                        var left = Resolve(expr.Children[0]);
                        var op = expr.Children[1];
                        var right = Resolve(expr.Children[2]);
                        if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) return null;
                        var opSymbol = GetOperatorSymbol(op);
                        var arithOps = new[] { OperatorWords.ADD, OperatorWords.SUBTRACT, OperatorWords.MULTIPLY, OperatorWords.DIVIDE };
                        if (Array.Exists(arithOps, o => o == opSymbol))
                        {
                            if (string.Equals(left, TypeWords.GEAR, StringComparison.OrdinalIgnoreCase) && string.Equals(right, TypeWords.GEAR, StringComparison.OrdinalIgnoreCase))
                                return TypeWords.GEAR;
                            if ((string.Equals(left, TypeWords.GEAR, StringComparison.OrdinalIgnoreCase) && string.Equals(right, TypeWords.SHINKAI, StringComparison.OrdinalIgnoreCase)) ||
                                (string.Equals(left, TypeWords.SHINKAI, StringComparison.OrdinalIgnoreCase) && string.Equals(right, TypeWords.GEAR, StringComparison.OrdinalIgnoreCase)) ||
                                (string.Equals(left, TypeWords.SHINKAI, StringComparison.OrdinalIgnoreCase) && string.Equals(right, TypeWords.SHINKAI, StringComparison.OrdinalIgnoreCase)))
                                return TypeWords.SHINKAI;
                            return null;
                        }
                        var cmpOps = new[] { OperatorWords.GREATER, OperatorWords.LESS, OperatorWords.GREATER_EQUAL, OperatorWords.LESS_EQUAL, OperatorWords.EQUAL, OperatorWords.NOT_EQUAL };
                        if (Array.Exists(cmpOps, o => o == opSymbol)) return TypeWords.SHIN;
                        var boolOps = new[] { OperatorWords.AND, OperatorWords.OR };
                        if (Array.Exists(boolOps, o => o == opSymbol))
                        {
                            if (string.Equals(left, TypeWords.SHIN, StringComparison.OrdinalIgnoreCase) && string.Equals(right, TypeWords.SHIN, StringComparison.OrdinalIgnoreCase))
                                return TypeWords.SHIN;
                            return null;
                        }
                    }
                    return null;
            }
        }

        public static string ExtractInnerType(string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return string.Empty;
            var open = typeStr.IndexOf('<');
            var close = typeStr.LastIndexOf('>');
            if (open >= 0 && close > open)
            {
                return typeStr.Substring(open + 1, close - open - 1);
            }
            return string.Empty;
        }

        private static string GetOperatorSymbol(Node? opNode)
        {
            if (opNode == null) return string.Empty;
            if (opNode.Value != null) return opNode.Value.ToString() ?? opNode.Type;
            return opNode.Type ?? string.Empty;
        }

        // Local helper to extract identifier text from various node shapes
        private static string? ExtractIdentifierName(Node? idNode)
        {
            if (idNode == null) return null;
            if (idNode.Type == "Identifier" || idNode.Type == "IDENTIFIER")
            {
                if (idNode.Value != null) return idNode.Value.ToString();
                if (idNode.Children.Count > 0) return idNode.Children[0].Value?.ToString() ?? idNode.Children[0].Type;
            }
            if (idNode.Type == "FunctionName")
            {
                if (idNode.Children.Count > 0)
                {
                    var c = idNode.Children[0];
                    if (c.Value != null) return c.Value.ToString();
                    if (c.Children.Count > 0) return c.Children[0].Value?.ToString() ?? c.Children[0].Type;
                    return c.Type;
                }
            }
            if (idNode.Value != null) return idNode.Value.ToString();
            return idNode.Children.FirstOrDefault()?.Value?.ToString() ?? idNode.Children.FirstOrDefault()?.Type;
        }
    }
}
