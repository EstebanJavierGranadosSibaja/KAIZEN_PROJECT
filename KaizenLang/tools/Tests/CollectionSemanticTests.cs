using System;
using Xunit;
using System.Linq;
using ParadigmasLang;
using KaizenLang.UI;

namespace KaizenLang.Tests
{
    public class CollectionSemanticTests
    {
        [Fact]
        public void ArrayDeclarationWithoutElementType_ShouldBeRejected()
        {
            var code = @"array nums = [1,2,3];";
            var cs = new CompilationService();
            var res = cs.CompileCode(code);
            Assert.False(res.IsSuccessful, "Compilation should fail for array declaration without element type");
            var combinedList = (res.SyntaxErrors ?? new System.Collections.Generic.List<string>()).Concat(res.SemanticErrors ?? new System.Collections.Generic.List<string>()).ToList();
            if (!string.IsNullOrEmpty(res.Output)) combinedList.Add(res.Output);
            var combined = string.Join("|", combinedList);
            Assert.True(combined.Contains("Declaración de array") || combined.Contains("Declaración de matrix"), "Expected parser to report missing element type for array/matrix");
        }
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
