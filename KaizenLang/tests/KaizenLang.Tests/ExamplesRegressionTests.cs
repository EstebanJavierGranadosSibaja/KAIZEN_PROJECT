using KaizenLang.UI;
using Xunit;

public class ExamplesRegressionTests
{
    [Theory]
    [InlineData("integer numero = 42;\nstring mensaje = \"Hola KaizenLang\";\n")]
    [InlineData("integer contador = 0;\nwhile (contador < 5) ying\n    contador = contador + 1;\nyang\n")]
    [InlineData("integer suma(integer a, integer b) ying\n    return a + b;\nyang\n")]
    [InlineData("integer factorial(integer n) ying\n    if (n <= 1) ying\n        return 1;\n    yang\n    return n * factorial(n - 1);\nyang\n")]
    [InlineData("matrix<integer> tabla = [[1, 2, 3], [4, 5, 6]];\narray<string> nombres = [\"Ana\", \"Luis\"];\n")]
    public void ExampleSnippets_ShouldCompile(string source)
    {
        var service = new CompilationService();
    var result = service.CompileCode(source);

    Assert.True(result.IsSuccessful, result.Output);
    Assert.True(result.SemanticErrors == null || result.SemanticErrors.Count == 0, result.Output);
    Assert.NotNull(result.AST);
    }
}
