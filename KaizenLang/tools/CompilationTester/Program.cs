using System;
using System.IO;
using KaizenLang.UI;

class Program
{
    static int Main(string[] args)
    {
        string source;
        if (args.Length > 0)
        {
            var path = args[0];
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"Archivo no encontrado: {path}");
                return 1;
            }
            source = File.ReadAllText(path);
            ParadigmasLang.Logging.Logger.Info($"== Compilando archivo: {path}");
        }
        else
        {
            Console.WriteLine("Uso: dotnet run --project tools/CompilationTester <ruta-archivo>");
            Console.WriteLine("Se usará un fragmento de ejemplo por defecto.");
            source = "integer suma(integer x, integer y) ying\n    return x + y;\n\ninteger a; a = suma(1, 2); output(a);";
        }

        var service = new CompilationService();
        var result = service.CompileCode(source);

        if (!string.IsNullOrWhiteSpace(result.Output))
            Console.WriteLine(result.Output);

        if (result.SyntaxErrors?.Count > 0)
        {
            Console.WriteLine("-- Errores Sintácticos --");
            foreach (var err in result.SyntaxErrors)
                Console.WriteLine(err);
        }

        if (result.SemanticErrors?.Count > 0)
        {
            Console.WriteLine("-- Errores Semánticos --");
            foreach (var err in result.SemanticErrors)
                Console.WriteLine(err);
        }

        if (result.AST != null)
        {
            ParadigmasLang.Logging.Logger.Debug("--- AST (full):");
            ParadigmasLang.Logging.Logger.Debug(result.AST.ToTreeString());
        }

        if (result.SyntaxErrors?.Count > 0 || result.SemanticErrors?.Count > 0)
            return 2;

        return 0;
    }
}
