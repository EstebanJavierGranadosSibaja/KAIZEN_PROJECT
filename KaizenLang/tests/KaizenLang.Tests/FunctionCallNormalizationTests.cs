using KaizenLang.UI;
using Xunit;

public class FunctionCallNormalizationTests
{
    [Fact]
    public void BuiltinCalls_ShouldProduce_FunctionCallNodes()
    {
        var source = "integer a; a = input(\"prompt\"); output(a);";
        var service = new CompilationService();
        var result = service.CompileCode(source);

        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.AST);

        // Search AST for any FunctionCall node
        bool hasFunctionCall = ContainsNodeOfType(result.AST, "FunctionCall");
        Assert.True(hasFunctionCall, "Expected AST to contain FunctionCall nodes for builtins");
    }

    [Fact]
    public void UserFunctionCall_ShouldBeRecognized_AsFunctionCall()
    {
        var source = "integer suma(integer x, integer y) ying\n    return x + y;\n\ninteger a; a = suma(1, 2); output(a);";
        var service = new CompilationService();
        var result = service.CompileCode(source);

        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.AST);
        Assert.True(ContainsNodeOfType(result.AST, "FunctionCall"));
    }

    private bool ContainsNodeOfType(ParadigmasLang.Node node, string type)
    {
        if (node == null) return false;
        if (node.Type == type) return true;
        foreach (var child in node.Children)
        {
            if (ContainsNodeOfType(child, type)) return true;
        }
        return false;
    }
}
