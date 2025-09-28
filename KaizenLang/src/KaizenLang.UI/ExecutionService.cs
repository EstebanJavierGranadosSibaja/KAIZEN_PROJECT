using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using ParadigmasLang;

namespace KaizenLang.UI;

public class ExecutionService
{
    private readonly CompilationService compilationService;
    private readonly Stopwatch executionTimer;
    // Optional input provider: function that given an optional prompt returns the user input
    public Func<string?, string?>? InputProvider { get; set; }

    public ExecutionService()
    {
        compilationService = new CompilationService();
        executionTimer = new Stopwatch();
    }

    private static int CountSourceLines(string src)
    {
        if (string.IsNullOrEmpty(src)) return 0;
        return src.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
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
            outputBuilder.AppendLine(new string('═', 40));
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

            // Ejecutar código (con registro de tiempo y timeout corto para diagnóstico)
            executionTimer.Restart();
            // Header: timestamps and compilation summary
            outputBuilder.AppendLine($"⏱ Inicio ejecución: {DateTime.UtcNow:O} (UTC)");
            outputBuilder.AppendLine($"• Fuente: <in-memory> | Líneas: {CountSourceLines(source)}");
            var errorsCount = (compilationResult.LexicalErrors?.Count ?? 0) + compilationResult.SyntaxErrors.Count + compilationResult.SemanticErrors.Count + (compilationResult.InternalError != null ? 1 : 0);
            outputBuilder.AppendLine($"• Compilación: {(compilationResult.IsSuccessful ? "OK" : "ERRORS")} | Errores: {errorsCount}");
            outputBuilder.AppendLine();
            outputBuilder.AppendLine("▶ EJECUTANDO PROGRAMA...");
            outputBuilder.AppendLine(new string('─', 40));

            var interpreter = new Interpreter(InputProvider);

            // Ejecutar en una tarea y esperar un timeout razonable para evitar bloqueos indefinidos
            List<string>? executionOutput = null;
            var execTask = Task.Run(() => interpreter.Execute(compilationResult.AST));
            var completed = execTask.Wait(TimeSpan.FromSeconds(5));
            if (!completed)
            {
                // Timeout: report and return a failed execution result (keep background task running for now)
                executionTimer.Stop();
                outputBuilder.AppendLine($"⚠️ EJECUCIÓN DETENIDA POR TIMEOUT a las {DateTime.UtcNow:O} (5s)");
                return new ExecutionResult
                {
                    IsSuccessful = false,
                    Output = outputBuilder.ToString(),
                    ExecutionTime = executionTimer.Elapsed,
                    CompilationResult = compilationResult,
                    ProgramOutput = new List<string> { "ERROR: ejecución excedió timeout diagnóstico (5s)" }
                };
            }

            executionOutput = execTask.Result;
            executionTimer.Stop();
            outputBuilder.AppendLine($"⏱ Fin ejecución: {DateTime.UtcNow:O} (UTC)");

            // Program output: numbered and labeled
            if (executionOutput != null && executionOutput.Any())
            {
                outputBuilder.AppendLine();
                outputBuilder.AppendLine("📤 SALIDA DEL PROGRAMA:");
                int idx = 1;
                foreach (var line in executionOutput)
                {
                    // Allow the UI to style based on leading markers like [OUT], [TRACE], [WARN]
                    var prefix = "[OUT]";
                    outputBuilder.AppendLine($" {idx:00}. {prefix} {line}");
                    idx++;
                }
            }
            else
            {
                outputBuilder.AppendLine();
                outputBuilder.AppendLine("✅ El programa se ejecutó sin salida");
            }

            outputBuilder.AppendLine();
            outputBuilder.AppendLine("🎯 EJECUCIÓN COMPLETADA");
            outputBuilder.AppendLine($"✅ Estado: Éxito");
            outputBuilder.AppendLine($"⏱️ Tiempo de ejecución: {executionTimer.ElapsedMilliseconds}ms");
            outputBuilder.AppendLine($"🔧 Tiempo de compilación: {compilationResult.CompilationTime.TotalMilliseconds:0.0000}ms");

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
