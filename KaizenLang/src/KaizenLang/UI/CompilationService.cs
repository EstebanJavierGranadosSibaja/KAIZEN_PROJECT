using System.Diagnostics;
using System.Text;
using ParadigmasLang;

namespace KaizenLang.UI;

public class CompilationService
{
    private readonly Stopwatch compilationTimer;
    private StringBuilder outputBuilder;

    public CompilationService()
    {
        compilationTimer = new Stopwatch();
        outputBuilder = new StringBuilder();
    }

    public CompilationResult CompileCode(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return new CompilationResult
            {
                IsSuccessful = false,
                Output = "❌ ERROR: No hay código para compilar",
                CompilationTime = TimeSpan.Zero
            };
        }

        compilationTimer.Restart();
        outputBuilder.Clear();

        AppendHeader();

        try
        {
            var result = PerformCompilationStages(source);
            compilationTimer.Stop();

            result.CompilationTime = compilationTimer.Elapsed;
            result.Output = outputBuilder.ToString();

            return result;
        }
        catch (Exception ex)
        {
            compilationTimer.Stop();
            AppendError($"� ERROR INTERNO DEL COMPILADOR:\r\n{ex.Message}\r\n{ex.StackTrace}");

            return new CompilationResult
            {
                IsSuccessful = false,
                Output = outputBuilder.ToString(),
                CompilationTime = compilationTimer.Elapsed,
                InternalError = ex
            };
        }
    }

    private CompilationResult PerformCompilationStages(string source)
    {
        var result = new CompilationResult();

        // FASE 1: ANÁLISIS LÉXICO
        if (!PerformLexicalAnalysis(source, out var tokens, result))
            return result;

        // FASE 2: ANÁLISIS SINTÁCTICO
        if (!PerformSyntacticAnalysis(tokens!, out var ast, result))
            return result;

        // Always attach the AST to the result after parsing so tools/debuggers can inspect it even on semantic failures
        result.AST = ast;

        // FASE 3: ANÁLISIS SEMÁNTICO
        if (!PerformSemanticAnalysis(ast!, result))
            return result;

        // MOSTRAR INFORMACIÓN DETALLADA
        ShowCompilationDetails(tokens!, ast!, result);

        // RESULTADO FINAL
        AppendSuccessMessage();
        result.IsSuccessful = true;
        result.Tokens = tokens;
        result.AST = ast;

        return result;
    }

    private bool PerformLexicalAnalysis(string source, out List<Token>? tokens, CompilationResult result)
    {
        AppendPhaseHeader("FASE 1: ANÁLISIS LÉXICO");

        var lexer = new Lexer();
        tokens = lexer.Tokenize(source);

        // Categorizar tokens
        var invalidTokens = tokens.Where(t => t.Type == "INVALID").ToList();
        var reservedTokens = tokens.Where(t => t.Type == "RESERVED_WORD").ToList();
        var identifierTokens = tokens.Where(t => t.Type == "IDENTIFIER").ToList();
        var literalTokens = tokens.Where(t => t.Type.Contains("LITERAL")).ToList();

        if (invalidTokens.Any())
        {
            AppendError("❌ ERRORES LÉXICOS ENCONTRADOS:");
            foreach (var invalidToken in invalidTokens)
            {
                AppendError($"   • Token inválido: '{invalidToken.Value}' (línea {invalidToken.Line}, col {invalidToken.Column})");
            }
            AppendError("\r\n❌ COMPILACIÓN DETENIDA");

            result.LexicalErrors = invalidTokens;
            tokens = null;
            return false;
        }

        AppendSuccess("✅ Análisis léxico completado exitosamente");
        AppendInfo($"📊 Estadísticas de tokens:");
        AppendInfo($"   • Total de tokens: {tokens.Count}");
        AppendInfo($"   • Palabras reservadas: {reservedTokens.Count}");
        AppendInfo($"   • Identificadores: {identifierTokens.Count}");
        AppendInfo($"   • Literales: {literalTokens.Count}");

        ShowTokenSample(tokens);
        AppendNewLine();

        return true;
    }

    private bool PerformSyntacticAnalysis(List<Token> tokens, out Node? ast, CompilationResult result)
    {
        AppendPhaseHeader("FASE 2: ANÁLISIS SINTÁCTICO");

        var parser = new Parser();
        ast = parser.Parse(tokens);

        var syntaxErrors = ast.GetAllErrors();
        if (syntaxErrors.Any())
        {
            AppendError("❌ ERRORES SINTÁCTICOS ENCONTRADOS:");
            foreach (var error in syntaxErrors)
            {
                AppendError($"   • {error}");
            }
            AppendError("\r\n❌ COMPILACIÓN DETENIDA");

            result.SyntaxErrors = syntaxErrors;
            ast = null;
            return false;
        }

        AppendSuccess("✅ Análisis sintáctico completado exitosamente");
        AppendSuccess("🌳 Árbol de Sintaxis Abstracta (AST) generado");

        // Mostrar estadísticas del AST
        var nodeCount = CountNodes(ast);
        var depth = CalculateDepth(ast);
        AppendInfo($"📊 Estadísticas del AST:");
        AppendInfo($"   • Nodos totales: {nodeCount}");
        AppendInfo($"   • Profundidad máxima: {depth}");
        AppendNewLine();

        return true;
    }

    private bool PerformSemanticAnalysis(Node ast, CompilationResult result)
    {
        AppendPhaseHeader("FASE 3: ANÁLISIS SEMÁNTICO");

        var semanticAnalyzer = new SemanticAnalyzer();
        var semanticErrors = semanticAnalyzer.AnalyzeProgram(ast);

        if (semanticErrors.Any())
        {
            AppendError("❌ ERRORES SEMÁNTICOS ENCONTRADOS:");
            foreach (var error in semanticErrors)
            {
                AppendError($"   • {error}");
            }
            AppendError("\r\n❌ COMPILACIÓN DETENIDA");

            result.SemanticErrors = semanticErrors;
            return false;
        }

        AppendSuccess("✅ Análisis semántico completado exitosamente");
        AppendSuccess("✅ Todas las validaciones de tipos y scope pasaron");
        AppendNewLine();

        return true;
    }

    private void ShowCompilationDetails(List<Token> tokens, Node ast, CompilationResult result)
    {
        AppendSectionHeader("DETALLES DE COMPILACIÓN");

        // Mostrar AST compacto
        AppendInfo("🌳 ESTRUCTURA DEL AST:");
        AppendInfo("────────────────────────");
        var astString = ast.ToTreeString();
        if (astString.Length > 1000) // Limitar tamaño para no saturar la salida
        {
            AppendInfo(astString.Substring(0, 1000) + "\r\n... (AST truncado para visualización)");
        }
        else
        {
            AppendInfo(astString);
        }
        AppendNewLine();

        // Mostrar métricas de complejidad
        ShowComplexityMetrics(ast);
    }

    private void ShowComplexityMetrics(Node ast)
    {
        AppendInfo("� MÉTRICAS DE COMPLEJIDAD:");
        AppendInfo("──────────────────────────");

        var functions = CountNodesByType(ast, "FUNCTION");
        var conditionals = CountNodesByType(ast, "IF") + CountNodesByType(ast, "WHILE") + CountNodesByType(ast, "FOR");
        var variables = CountNodesByType(ast, "VARIABLE_DECLARATION");

        AppendInfo($"   • Funciones definidas: {functions}");
        AppendInfo($"   • Estructuras de control: {conditionals}");
        AppendInfo($"   • Variables declaradas: {variables}");
        AppendNewLine();
    }

    private void ShowTokenSample(List<Token> tokens)
    {
        AppendInfo("🔍 MUESTRA DE TOKENS:");
        var importantTokens = tokens.Take(15).ToList();
        foreach (var token in importantTokens)
        {
            AppendInfo($"   {token.Type.PadRight(15)}: '{token.Value}' (l{token.Line}:c{token.Column})");
        }
        if (tokens.Count > 15)
            AppendInfo($"   ... y {tokens.Count - 15} tokens más");
    }

    // Métodos auxiliares para contar nodos
    private int CountNodes(Node node)
    {
        int count = 1;
        foreach (var child in node.Children)
            count += CountNodes(child);
        return count;
    }

    private int CalculateDepth(Node node)
    {
        if (!node.Children.Any())
            return 1;
        return 1 + node.Children.Max(child => CalculateDepth(child));
    }

    private int CountNodesByType(Node node, string type)
    {
        int count = node.Type.Equals(type, StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        foreach (var child in node.Children)
            count += CountNodesByType(child, type);
        return count;
    }

    // Métodos de formateo de salida
    private void AppendHeader()
    {
        outputBuilder.AppendLine("🔧 PROCESO DE COMPILACIÓN INICIADO");
        outputBuilder.AppendLine("═════════════════════════════════════");
        outputBuilder.AppendLine();
    }

    private void AppendPhaseHeader(string phase)
    {
        outputBuilder.AppendLine($"📍 {phase}");
        outputBuilder.AppendLine("─────────────────────────────");
    }

    private void AppendSectionHeader(string section)
    {
        outputBuilder.AppendLine($"📋 {section}");
        outputBuilder.AppendLine("═════════════════════════");
    }

    private void AppendSuccess(string message)
    {
        outputBuilder.AppendLine(message);
    }

    private void AppendInfo(string message)
    {
        outputBuilder.AppendLine(message);
    }

    private void AppendError(string message)
    {
        outputBuilder.AppendLine(message);
    }

    private void AppendNewLine()
    {
        outputBuilder.AppendLine();
    }

    private void AppendSuccessMessage()
    {
        AppendSectionHeader("COMPILACIÓN EXITOSA");
        AppendSuccess("✓ Análisis léxico: CORRECTO");
        AppendSuccess("✓ Análisis sintáctico: CORRECTO");
        AppendSuccess("✓ Análisis semántico: CORRECTO");
        AppendSuccess($"⏱️ Tiempo de compilación: {compilationTimer.ElapsedMilliseconds}ms");
        AppendNewLine();
        AppendSuccess("💡 El código está listo para ejecutarse");
    }
}

// Clase para encapsular el resultado de la compilación
public class CompilationResult
{
    public bool IsSuccessful { get; set; }
    public string Output { get; set; } = string.Empty;
    public TimeSpan CompilationTime { get; set; }
    public List<Token>? Tokens { get; set; }
    public Node? AST { get; set; }
    public List<Token>? LexicalErrors { get; set; }
    // Ensure these lists are non-null to make downstream checks simpler and tests more robust
    public List<string> SyntaxErrors { get; set; } = new List<string>();
    public List<string> SemanticErrors { get; set; } = new List<string>();
    public Exception? InternalError { get; set; }

    public bool HasErrors => (LexicalErrors?.Any() == true) ||
                             SyntaxErrors.Any() ||
                             SemanticErrors.Any() ||
                             InternalError != null;
}
