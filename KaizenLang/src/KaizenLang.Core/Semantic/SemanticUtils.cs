using System;
using System.Collections.Generic;
using System.Linq;

namespace ParadigmasLang;

public static class SemanticUtils
{
    public static string? ExtractIdentifierName(Node? idNode)
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

    public static bool IsSymbolDefined(IEnumerable<SymbolTable> scopes, string name)
    {
        foreach (var scope in scopes)
        {
            if (scope.LookupVariable(name) != null)
                return true;
        }
        return false;
    }

    public static bool IsNullLiteralType(string? typeName)
    {
        return string.Equals(typeName, LiteralWords.NULL, StringComparison.OrdinalIgnoreCase);
    }

    public static bool SupportsNullAssignment(string? targetType)
    {
        if (string.IsNullOrWhiteSpace(targetType))
            return false;

        var normalized = targetType.Trim();

        if (normalized.StartsWith(TypeWords.CHAINSAW, StringComparison.OrdinalIgnoreCase) ||
            normalized.StartsWith(TypeWords.HOGYOKU, StringComparison.OrdinalIgnoreCase))
            return true;

        return string.Equals(normalized, TypeWords.GRIMOIRE, StringComparison.OrdinalIgnoreCase);
    }
}
