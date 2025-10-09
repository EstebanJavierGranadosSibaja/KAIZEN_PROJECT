using System;
using System.IO;
using Xunit;
using ParadigmasLang.Logging;
using ParadigmasLang;
using KaizenLang.UI;

namespace KaizenLang.Tests
{
    public class FunctionTests
    {
        [Fact]
        public void FunctionExecution_LogsAndResult()
        {
            // Arrange: set debug logging so logger prints DBG messages
            Environment.SetEnvironmentVariable("PARADIGMAS_LOG_LEVEL", "Debug");

            var code = @"void saludar() ying
    output(""¡Hola desde una función!"");
yang

saludar();

integer sumar(integer a, integer b) ying
    return a + b;
yang

integer resultado = sumar(5, 3);
output(""El resultado de la suma es: "" + resultado);
";

            // Capture console output
            var sw = new StringWriter();
            var oldOut = Console.Out;
            Console.SetOut(sw);

            try
            {
                // Use CompilationService to compile
                var cs = new CompilationService();
                var res = cs.CompileCode(code);
                Assert.True(res.IsSuccessful, "Compilation should succeed");

                // Execute interpreter and capture returned program outputs
                var interpreter = new Interpreter();
                var programOutputs = interpreter.Execute(res.AST);

                // Examine captured console logs (debug/info messages)
                var consoleOutput = sw.ToString();
                // Debug logs should show function invocation
                Assert.Contains("Invoking function 'saludar'", consoleOutput);
                Assert.Contains("Invoking function 'sumar'", consoleOutput);

                // Program outputs are returned by the interpreter as a list
                var joinedProgramOutput = string.Join("\n", programOutputs);
                Assert.Contains("¡Hola desde una función!", joinedProgramOutput);
                Assert.Contains("El resultado de la suma es: 8", joinedProgramOutput);
            }
            finally
            {
                Console.SetOut(oldOut);
                Environment.SetEnvironmentVariable("PARADIGMAS_LOG_LEVEL", null);
            }
        }
    }
}
