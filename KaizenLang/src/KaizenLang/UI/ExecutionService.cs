using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;
using ParadigmasLang;

namespace KaizenLang.UI
{
    public class ExecutionService
    {
        private readonly CompilationService compilationService;
        private readonly Stopwatch executionTimer;

        public ExecutionService()
        {
            compilationService = new CompilationService();
            executionTimer = new Stopwatch();
        }

        public ExecutionResult ExecuteCode(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return new ExecutionResult
                {
                    IsSuccessful = false,
                    Output = "❌ ERROR: No hay código para ejecutar",
                    ExecutionTime = TimeSpan.Zero
                };
            }

            var outputBuilder = new StringBuilder();
            outputBuilder.AppendLine("🚀 INICIANDO EJECUCIÓN");
            outputBuilder.AppendLine("═════════════════════");
            outputBuilder.AppendLine();

            try
            {
                // Validar compilación antes de ejecutar
                var compilationResult = compilationService.CompileCode(source);
                if (!compilationResult.IsSuccessful || compilationResult.AST == null)
                {
                    outputBuilder.AppendLine(GetValidationErrorMessage(compilationResult));
                    return new ExecutionResult
                    {
                        IsSuccessful = false,
                        Output = outputBuilder.ToString(),
                        CompilationResult = compilationResult
                    };
                }

                // Ejecutar código
                executionTimer.Restart();
                outputBuilder.AppendLine("� EJECUTANDO CÓDIGO...");
                outputBuilder.AppendLine("─────────────────────────");
                
                var interpreter = new Interpreter();
                var executionOutput = interpreter.Execute(compilationResult.AST);
                
                executionTimer.Stop();

                // Mostrar resultados
                if (executionOutput.Any())
                {
                    outputBuilder.AppendLine("📤 SALIDA DEL PROGRAMA:");
                    foreach (var line in executionOutput)
                    {
                        outputBuilder.AppendLine($"   {line}");
                    }
                }
                else
                {
                    outputBuilder.AppendLine("✅ El programa se ejecutó sin salida");
                }

                outputBuilder.AppendLine();
                outputBuilder.AppendLine("🎯 EJECUCIÓN COMPLETADA EXITOSAMENTE");
                outputBuilder.AppendLine($"⏱️ Tiempo de ejecución: {executionTimer.ElapsedMilliseconds}ms");
                outputBuilder.AppendLine($"🔧 Tiempo de compilación: {compilationResult.CompilationTime.TotalMilliseconds}ms");

                return new ExecutionResult
                {
                    IsSuccessful = true,
                    Output = outputBuilder.ToString(),
                    ExecutionTime = executionTimer.Elapsed,
                    CompilationResult = compilationResult,
                    ProgramOutput = executionOutput
                };
            }
            catch (Exception ex)
            {
                executionTimer.Stop();
                outputBuilder.AppendLine($"\r\n💥 ERROR DE EJECUCIÓN:");
                outputBuilder.AppendLine($"Mensaje: {ex.Message}");
                if (ex.InnerException != null)
                {
                    outputBuilder.AppendLine($"Error interno: {ex.InnerException.Message}");
                }
                
                return new ExecutionResult
                {
                    IsSuccessful = false,
                    Output = outputBuilder.ToString(),
                    ExecutionTime = executionTimer.Elapsed,
                    RuntimeError = ex
                };
            }
        }

        private string GetValidationErrorMessage(CompilationResult compilationResult)
        {
            var output = new StringBuilder();
            output.AppendLine("❌ EJECUCIÓN DETENIDA");
            output.AppendLine("El código contiene errores. Use 'Compilar' para ver los detalles.");
            output.AppendLine();
            
            if (compilationResult.LexicalErrors?.Any() == true)
                output.AppendLine($"• {compilationResult.LexicalErrors.Count} errores léxicos");
            if (compilationResult.SyntaxErrors?.Any() == true)
                output.AppendLine($"• {compilationResult.SyntaxErrors.Count} errores sintácticos");
            if (compilationResult.SemanticErrors?.Any() == true)
                output.AppendLine($"• {compilationResult.SemanticErrors.Count} errores semánticos");
            if (compilationResult.InternalError != null)
                output.AppendLine("• Error interno del compilador");
            
            return output.ToString();
        }
    }

    // Clase para encapsular el resultado de la ejecución
    public class ExecutionResult
    {
        public bool IsSuccessful { get; set; }
        public string Output { get; set; } = string.Empty;
        public TimeSpan ExecutionTime { get; set; }
        public CompilationResult? CompilationResult { get; set; }
        public List<string>? ProgramOutput { get; set; }
        public Exception? RuntimeError { get; set; }

        public bool HasErrors => !IsSuccessful || 
                                CompilationResult?.HasErrors == true || 
                                RuntimeError != null;
    }
}
