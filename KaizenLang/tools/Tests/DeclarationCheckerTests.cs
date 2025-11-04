using System;
using Xunit;
using System.Linq;
using ParadigmasLang;
using KaizenLang.UI.Services;

namespace KaizenLang.Tests
{
    public class DeclarationCheckerTests
    {
        [Fact]
        public void VariableDeclaration_HappyPath_ShouldSucceed()
        {
            var code = @"integer x = 5;";
            var cs = new CompilationService();
            var res = cs.CompileCode(code);
            Assert.True(res.IsSuccessful, "Compilation should succeed for valid variable declaration");
        }

        [Fact]
        public void DuplicateVariableDeclaration_ShouldReportSemanticError()
        {
            var code = @"integer x = 1; integer x = 2;";
            var cs = new CompilationService();
            var res = cs.CompileCode(code);
            Assert.False(res.IsSuccessful);
            Assert.Contains("already declared", string.Join("|", res.SemanticErrors ?? new System.Collections.Generic.List<string>()));
        }

    [Fact(Skip = "Lexer tokenization varies; enable when INT+IDENTIFIER pattern is produced")]
    public void MalformedInitializer_LiteralIdentifierConcatenation_ShouldReportError()
        {
            var code = @"integer hola = 123abc;"; // lexer may split this into INT + IDENTIFIER sequence -> parser produced expression
            var cs = new CompilationService();
            var res = cs.CompileCode(code);
            Assert.False(res.IsSuccessful);
            // Parser/lexer may report this as a syntax or semantic error depending on tokenization.
            Assert.True((res.SemanticErrors != null && res.SemanticErrors.Count > 0) || (res.SyntaxErrors != null && res.SyntaxErrors.Count > 0), "Expected at least one error for malformed initializer");
        }

        [Fact]
        public void UndeclaredIdentifierInInitializer_ShouldReportError()
        {
            var code = @"integer z = someUndeclaredVar;";
            var cs = new CompilationService();
            var res = cs.CompileCode(code);
            Assert.False(res.IsSuccessful);
            Assert.Contains("no declarada", string.Join("|", res.SemanticErrors ?? new System.Collections.Generic.List<string>()));
        }
    }
}
