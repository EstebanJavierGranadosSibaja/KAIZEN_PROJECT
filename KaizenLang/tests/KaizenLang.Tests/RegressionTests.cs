using KaizenLang.UI;
using Xunit;

public class RegressionTests
{
    [Theory]
    [InlineData("integer a;\na = input(\"Ingrese numero: \" );\noutput(a);")]
    [InlineData("integer x;\nx = 42;\noutput(x);")]
    public void Snippets_ShouldCompileWithoutSemanticErrors(string source)
    {
        var service = new CompilationService();
        var result = service.CompileCode(source);

        Assert.True(result.IsSuccessful, "Expected compilation to succeed");
        Assert.Null(result.SemanticErrors);
        Assert.NotNull(result.Tokens);
        Assert.NotNull(result.AST);
    }
}
