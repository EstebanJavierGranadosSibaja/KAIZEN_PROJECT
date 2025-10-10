using System;
using Xunit;
using ParadigmasLang;
using KaizenLang.UI;

namespace KaizenLang.Tests
{
    public class CollectionSemanticTests
    {
        [Fact]
        public void RaggedMatrix_ShouldReportSemanticError()
        {
            var code = @"matrix<integer> ragged = [ [1,2], [3] ];";
            var cs = new CompilationService();
            var res = cs.CompileCode(code);
            Assert.False(res.IsSuccessful, "Compilation should fail for ragged matrix");
            Assert.Contains("Matriz no rectangular", string.Join("|", res.SemanticErrors ?? new System.Collections.Generic.List<string>()));
        }

        [Fact]
        public void ArrayTypeMismatch_ShouldReportSemanticError()
        {
            var code = @"array<string> wrong = [ ""a"", ""b"", 5 ];";
            var cs = new CompilationService();
            var res = cs.CompileCode(code);
            Assert.False(res.IsSuccessful, "Compilation should fail for array type mismatch");
            Assert.Contains("Tipo incompatible en inicialización de array", string.Join("|", res.SemanticErrors ?? new System.Collections.Generic.List<string>()));
        }
    }
}
