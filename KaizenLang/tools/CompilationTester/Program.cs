using KaizenLang.UI;

class Program
{
    static void Main()
    {
        // Use the snippet from the failing unit test to inspect parsing/AST
        var source = "integer suma(integer x, integer y) ying\n    return x + y;\n\ninteger a; a = suma(1, 2); output(a);";
        var service = new CompilationService();
        var result = service.CompileCode(source);
    ParadigmasLang.Logging.Logger.Debug(result.Output);
        if (result.AST != null)
        {
            ParadigmasLang.Logging.Logger.Debug("--- AST (full):");
            ParadigmasLang.Logging.Logger.Debug(result.AST.ToTreeString());
        }
        if (result.Tokens != null)
        {
            ParadigmasLang.Logging.Logger.Debug("--- Tokens:");
            foreach (var t in result.Tokens)
                ParadigmasLang.Logging.Logger.Debug($"{t.Type}\t'{t.Value}' (l{t.Line}:c{t.Column})");
        }
    }

}
