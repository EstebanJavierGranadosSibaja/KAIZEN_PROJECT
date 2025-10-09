using KaizenLang.UI;

ParadigmasLang.Logging.Logger.Info("QuickRunner: compiling and running function-call snippet...");

var source = @"void saludar() ying
    output(""¡Hola desde una función!"");
yang

saludar();

integer sumar(integer a, integer b) ying
    return a + b;
yang

integer resultado = sumar(5, 3);
output(""El resultado de la suma es: "" + resultado);
";

var exec = new ExecutionService();
exec.InputProvider = (prompt) => {
    // For tests we don't need interactive input; return null
    return null;
};

// First, compile and print compilation diagnostics immediately
var compilationService = new CompilationService();
var compilationResult = compilationService.CompileCode(source);
            ParadigmasLang.Logging.Logger.Info("--- Compilation Output ---");
            ParadigmasLang.Logging.Logger.Debug(compilationResult.Output);

if (!compilationResult.IsSuccessful || compilationResult.AST == null)
{
    ParadigmasLang.Logging.Logger.Warn("Compilation failed; aborting execution.");
    if (compilationResult.SyntaxErrors != null && compilationResult.SyntaxErrors.Any())
    {
    ParadigmasLang.Logging.Logger.Info("--- Syntax Errors ---");
        foreach (var e in compilationResult.SyntaxErrors)
            ParadigmasLang.Logging.Logger.Info(e);
    }
    if (compilationResult.SemanticErrors != null && compilationResult.SemanticErrors.Any())
    {
    ParadigmasLang.Logging.Logger.Info("--- Semantic Errors ---");
        foreach (var e in compilationResult.SemanticErrors)
            ParadigmasLang.Logging.Logger.Info(e);
    }
    return;
}

ParadigmasLang.Logging.Logger.Info("Starting interpreter.Execute in background task (15s timeout)...");
var interpreter = new ParadigmasLang.Interpreter((prompt) => null);
var execTask = System.Threading.Tasks.Task.Run(() => interpreter.Execute(compilationResult.AST));
if (execTask.Wait(TimeSpan.FromSeconds(15)))
{
    var programOutput = execTask.Result;
    ParadigmasLang.Logging.Logger.Info("--- Interpreter Program Output ---");
    if (programOutput != null && programOutput.Any())
    {
        foreach (var line in programOutput)
            ParadigmasLang.Logging.Logger.Info(line);
    }
    else
    {
    ParadigmasLang.Logging.Logger.Info("(no program output)");
    }
    ParadigmasLang.Logging.Logger.Info("Interpreter execution finished");
}
else
{
    ParadigmasLang.Logging.Logger.Warn("Interpreter.Execute timed out after 15s — likely hanging inside interpreter");
}

// If the compilation produced an ExecutionResult with runtime outputs embedded in result.Output,
// they will already be printed above. If not, try to execute using ExecutionService directly.

return;
