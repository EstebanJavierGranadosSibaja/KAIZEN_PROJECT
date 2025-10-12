using KaizenLang.UI;
using ParadigmasLang;
using System;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0) { Console.WriteLine("Usage: AstDump <file>"); return; }
        var path = args[0];
        if (!System.IO.File.Exists(path)) { Console.WriteLine("File not found"); return; }
        var code = System.IO.File.ReadAllText(path);
        var cs = new CompilationService();
        var res = cs.CompileCode(code);
        // Always print the compiled output so lexical/syntax/semantic errors are visible
        if (!string.IsNullOrEmpty(res.Output))
        {
            Console.WriteLine(res.Output);
        }
        if (res.AST != null)
        {
            Console.WriteLine(res.AST.ToTreeString());
        }
        if (res.SemanticErrors != null && res.SemanticErrors.Count>0)
        {
            Console.WriteLine("SEMANTIC ERRORS:");
            foreach(var e in res.SemanticErrors) Console.WriteLine(e);
        }
    }
}
