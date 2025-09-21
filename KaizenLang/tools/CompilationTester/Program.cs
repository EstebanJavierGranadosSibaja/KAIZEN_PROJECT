using KaizenLang.UI;

class Program
{
    static void Main()
    {
        // Use the snippet from the failing unit test to inspect parsing/AST
        var source = "integer suma(integer x, integer y) ying\n    return x + y;\n\ninteger a; a = suma(1, 2); output(a);";
        var service = new CompilationService();
        var result = service.CompileCode(source);
        System.Console.WriteLine(result.Output);
        if (result.AST != null)
        {
            System.Console.WriteLine("--- AST (full):");
            System.Console.WriteLine(result.AST.ToTreeString());
        }
        if (result.Tokens != null)
        {
            System.Console.WriteLine("--- Tokens:");
            foreach (var t in result.Tokens)
                System.Console.WriteLine($"{t.Type}\t'{t.Value}' (l{t.Line}:c{t.Column})");
        }
    }

}
