using System;
using System.Linq;
using ParadigmasLang;

namespace KaizenLang.UI
{
    public class ExecutionService
    {
        public string ExecuteCode(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return "❌ ERROR: No hay código para ejecutar";
            }

            var output = "🚀 INICIANDO EJECUCIÓN\r\n";
            output += "═════════════════════\r\n\r\n";

            try
            {
                // Validar compilación antes de ejecutar
                var validationResult = ValidateCode(source);
                if (!validationResult.IsValid || validationResult.Ast == null)
                {
                    output += GetValidationErrorMessage(validationResult);
                    return output;
                }

                // Ejecutar código
                output += ExecuteValidatedCode(validationResult.Ast);

                return output;
            }
            catch (Exception ex)
            {
                output += $"\r\n💥 ERROR DE EJECUCIÓN:\r\n{ex.Message}\r\n";
                return output;
            }
        }

        private ValidationResult ValidateCode(string source)
        {
            try
            {
                var lexer = new Lexer();
                var tokens = lexer.Tokenize(source);
                var parser = new Parser();
                var ast = parser.Parse(tokens);
                var semanticAnalyzer = new SemanticAnalyzer();
                var semanticErrors = semanticAnalyzer.AnalyzeProgram(ast);

                var lexicalErrors = tokens.Where(t => t.Type == "INVALID").ToList();
                var syntaxErrors = ast.GetAllErrors();

                return new ValidationResult
                {
                    IsValid = !lexicalErrors.Any() && !syntaxErrors.Any() && !semanticErrors.Any(),
                    LexicalErrors = lexicalErrors,
                    SyntaxErrors = syntaxErrors,
                    SemanticErrors = semanticErrors,
                    Ast = ast
                };
            }
            catch (Exception)
            {
                return new ValidationResult { IsValid = false };
            }
        }

        private string GetValidationErrorMessage(ValidationResult validationResult)
        {
            var output = "❌ EJECUCIÓN DETENIDA\r\n";
            output += "El código contiene errores. Use 'Compilar' para ver los detalles.\r\n\r\n";
            
            if (validationResult.LexicalErrors.Any())
                output += $"• {validationResult.LexicalErrors.Count} errores léxicos\r\n";
            if (validationResult.SyntaxErrors.Any())
                output += $"• {validationResult.SyntaxErrors.Count} errores sintácticos\r\n";
            if (validationResult.SemanticErrors.Any())
                output += $"• {validationResult.SemanticErrors.Count} errores semánticos\r\n";
            
            return output;
        }

        private string ExecuteValidatedCode(Node ast)
        {
            var output = "💻 EJECUTANDO CÓDIGO...\r\n";
            output += "─────────────────────────\r\n";
            
            var interpreter = new Interpreter();
            var executionOutput = interpreter.Execute(ast);

            if (executionOutput.Any())
            {
                output += "📤 SALIDA DEL PROGRAMA:\r\n";
                foreach (var line in executionOutput)
                {
                    output += $"   {line}\r\n";
                }
            }
            else
            {
                output += "✅ El programa se ejecutó sin salida\r\n";
            }

            output += "\r\n🎯 EJECUCIÓN COMPLETADA EXITOSAMENTE\r\n";
            return output;
        }

        private class ValidationResult
        {
            public bool IsValid { get; set; }
            public List<Token> LexicalErrors { get; set; } = new List<Token>();
            public List<string> SyntaxErrors { get; set; } = new List<string>();
            public List<string> SemanticErrors { get; set; } = new List<string>();
            public Node? Ast { get; set; }
        }
    }
}
