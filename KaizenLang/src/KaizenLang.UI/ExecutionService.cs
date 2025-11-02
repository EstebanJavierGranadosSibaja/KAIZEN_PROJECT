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

    public ExecutionResult ExecuteCode(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return new ExecutionResult
            {
                IsSuccessful = false,
                Output = "error: no hay código para ejecutar.",
                ExecutionTime = TimeSpan.Zero
            };
        }

        var outputBuilder = new StringBuilder();

        try
        {
            // Validar compilación antes de ejecutar
            var compilationResult = compilationService.CompileCode(source);
            outputBuilder.AppendLine(compilationResult.Output.TrimEnd());

            if (!compilationResult.IsSuccessful || compilationResult.AST == null)
            {
                outputBuilder.AppendLine("execution aborted: el código tiene errores de compilación.");
                return new ExecutionResult
                {
                    IsSuccessful = false,
                    Output = outputBuilder.ToString(),
                    CompilationResult = compilationResult
                };
            }

            // Ejecutar código (con registro de tiempo y timeout corto para diagnóstico)
            executionTimer.Restart();
            outputBuilder.AppendLine("Running program...");

            var interpreter = new Interpreter(InputProvider);

            // Ejecutar en una tarea y esperar un timeout razonable para evitar bloqueos indefinidos
            List<string>? executionOutput = null;
            var execTask = Task.Run(() => interpreter.Execute(compilationResult.AST));
            var completed = execTask.Wait(TimeSpan.FromSeconds(5));
            if (!completed)
            {
                // Timeout: report and return a failed execution result (keep background task running for now)
                executionTimer.Stop();
                outputBuilder.AppendLine("error: la ejecución excedió el límite de 5 segundos.");
                return new ExecutionResult
                {
                    IsSuccessful = false,
                    Output = outputBuilder.ToString(),
                    ExecutionTime = executionTimer.Elapsed,
                    CompilationResult = compilationResult,
                    ProgramOutput = new List<string> { "execution timeout (5s)" }
                };
            }

            executionOutput = execTask.Result;
            executionTimer.Stop();

            // Program output: numbered and labeled
            if (executionOutput != null && executionOutput.Any())
            {
                foreach (var line in executionOutput)
                {
                    outputBuilder.AppendLine(line);
                }
            }
            else
            {
                outputBuilder.AppendLine("(sin salida)");
            }

            outputBuilder.AppendLine($"Execution finished in {executionTimer.ElapsedMilliseconds} ms.");

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
            outputBuilder.AppendLine($"runtime error: {ex.Message}");
            if (ex.InnerException != null)
            {
                outputBuilder.AppendLine($"causado por: {ex.InnerException.Message}");
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
