using KaizenLang.UI.Services;
using ParadigmasLang;

var testFile = args.Length > 0 ? args[0] : "test-all-snippets.txt";
var basePath = @"c:\Users\esteb\OneDrive\Escritorio\KAIZEN_PROJECT\KaizenLang\";
var filePath = Path.Combine(basePath, testFile);

if (!File.Exists(filePath))
{
    Console.WriteLine($"ERROR: Archivo no encontrado: {filePath}");
    return 1;
}

var source = File.ReadAllText(filePath);
Console.WriteLine($"=== Probando: {testFile} ===\n");

var compilationService = new CompilationService();
var compilationResult = compilationService.CompileCode(source);

Console.WriteLine("--- Compilación ---");
Console.WriteLine(compilationResult.Output);

if (!compilationResult.IsSuccessful || compilationResult.AST == null)
{
    Console.WriteLine("\nERROR: Compilación falló");
    if (compilationResult.SyntaxErrors != null && compilationResult.SyntaxErrors.Any())
    {
        Console.WriteLine("\nErrores de Sintaxis:");
        foreach (var e in compilationResult.SyntaxErrors)
            Console.WriteLine($"  - {e}");
    }
    if (compilationResult.SemanticErrors != null && compilationResult.SemanticErrors.Any())
    {
        Console.WriteLine("\nErrores Semánticos:");
        foreach (var e in compilationResult.SemanticErrors)
            Console.WriteLine($"  - {e}");
    }
    return 1;
}

Console.WriteLine("\n--- Ejecución ---");
var interpreter = new Interpreter((prompt) => null);
try
{
    var output = interpreter.Execute(compilationResult.AST);
    if (output != null && output.Any())
    {
        foreach (var line in output)
            Console.WriteLine(line);
    }
    else
    {
        Console.WriteLine("(sin salida)");
    }
    Console.WriteLine("\n✓ Ejecución completada exitosamente");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"\nERROR en ejecución: {ex.Message}");
    Console.WriteLine($"Stack: {ex.StackTrace}");
    return 1;
}
