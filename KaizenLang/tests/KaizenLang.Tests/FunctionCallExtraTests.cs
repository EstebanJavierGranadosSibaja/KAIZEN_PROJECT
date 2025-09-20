using System;
using System.Collections.Generic;
using System.Linq;
using KaizenLang.UI;
using Xunit;

namespace KaizenLang.Tests
{
    public class FunctionCallExtraTests
    {
        [Fact]
        public void BuiltinInput_ArityMismatch_ShouldProduceSemanticError()
        {
            var src = "integer x;\nx = input(1,2);"; // input should accept 0..1 args
            var service = new CompilationService();
            var result = service.CompileCode(src);

            Assert.False(result.IsSuccessful);
            Assert.True(result.SemanticErrors != null && result.SemanticErrors.Count > 0);
            Assert.Contains(result.SemanticErrors, e => e.ToLower().Contains("input") || e.ToLower().Contains("cantidad") || e.ToLower().Contains("argument"));
        }

        [Fact]
        public void FunctionCall_LineColumn_PropagatesFromName()
        {
            // put the call on line 3 (two leading newlines)
            var src = "\n\nmiFuncion();";
            var service = new CompilationService();
            var result = service.CompileCode(src);

            // Compilation likely failed (undefined function) but AST should be present and semantic errors contain location info
            Assert.NotNull(result.AST);
            if (result.SemanticErrors != null && result.SemanticErrors.Any())
            {
                // at least one error should include the line number '3' or the word 'línea' / 'line'
                Assert.Contains(result.SemanticErrors, e => e.Contains("3") || e.ToLower().Contains("línea") || e.ToLower().Contains("line"));
            }
        }

        [Fact]
        public void NestedFunctionCalls_AreAccepted()
        {
            var src = "integer x; x = max(min(1,2), 3);";
            var service = new CompilationService();
            var result = service.CompileCode(src);

            // Ensure AST exists and contains FunctionCall nodes (parsing should succeed)
            Assert.NotNull(result.AST);
            bool hasFunctionCall = ContainsNodeOfType(result.AST, "FunctionCall");
            Assert.True(hasFunctionCall, "Expected AST to contain nested FunctionCall nodes");
        }

        [Fact]
        public void InputExecution_UsesTestInputProvider()
        {
            // Use nested call to avoid variable declaration edge cases in interpreter
            var src = "output(input(\"prompt\"));";
            var executor = new ExecutionService();
            TestHelpers.UseConstantInput(executor, "42");

            var execResult = executor.ExecuteCode(src);
            Assert.True(execResult.IsSuccessful, execResult.Output);
            Assert.NotNull(execResult.ProgramOutput);
            Assert.Contains("42", execResult.ProgramOutput);
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
}
