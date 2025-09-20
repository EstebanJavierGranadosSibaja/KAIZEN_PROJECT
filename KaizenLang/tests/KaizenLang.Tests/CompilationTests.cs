using KaizenLang.UI;
using Xunit;

public class CompilationTests
{
    [Fact]
    public void InputOutputSnippet_ShouldNotProduceSemanticErrors()
    {
        var source = "integer a;\na = input(\"Ingrese numero: \" );\noutput(a);";
        var service = new CompilationService();
        var result = service.CompileCode(source);

    Assert.True(result.IsSuccessful, result.Output);
    Assert.True(result.SemanticErrors == null || result.SemanticErrors.Count == 0, result.Output);
    Assert.NotNull(result.Tokens);
    Assert.NotNull(result.AST);
    }
}
