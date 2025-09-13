using System;
using System.Linq;
using ParadigmasLang;

namespace KaizenLang.UI
{
    public class CompilationService
    {
        public string CompileCode(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return "❌ ERROR: No hay código para compilar";
            }

            var output = "🔧 PROCESO DE COMPILACIÓN INICIADO\r\n";
            output += "═════════════════════════════════════\r\n\r\n";

            try
            {
                // FASE 1: ANÁLISIS LÉXICO
                output += PerformLexicalAnalysis(source, out var tokens);
                if (tokens == null) return output;

                // FASE 2: ANÁLISIS SINTÁCTICO
                output += PerformSyntacticAnalysis(tokens, out var ast);
                if (ast == null) return output;

                // FASE 3: ANÁLISIS SEMÁNTICO
                output += PerformSemanticAnalysis(ast);

                // MOSTRAR AST COMPACTO
                output += "🌳 ESTRUCTURA DEL AST:\r\n";
                output += "────────────────────────\r\n";
                output += ast.ToTreeString();
                output += "\r\n";

                // RESULTADO FINAL
                output += GetSuccessMessage();

                return output;
            }
            catch (Exception ex)
            {
                output += $"\r\n💥 ERROR INTERNO DEL COMPILADOR:\r\n{ex.Message}\r\n";
                return output;
            }
        }

        private string PerformLexicalAnalysis(string source, out List<Token>? tokens)
        {
            var output = "📍 FASE 1: ANÁLISIS LÉXICO\r\n";
            output += "─────────────────────────────\r\n";
            
            var lexer = new Lexer();
            tokens = lexer.Tokenize(source);
            
            // Verificar tokens inválidos
            var invalidTokens = tokens.Where(t => t.Type == "INVALID").ToList();
            if (invalidTokens.Any())
            {
                output += "❌ ERRORES LÉXICOS ENCONTRADOS:\r\n";
                foreach (var invalidToken in invalidTokens)
                {
                    output += $"   • {invalidToken.Value}\r\n";
                }
                output += "\r\n❌ COMPILACIÓN DETENIDA\r\n";
                tokens = null;
                return output;
            }

            output += "✅ Análisis léxico completado exitosamente\r\n";
            output += $"📊 Tokens encontrados: {tokens.Count}\r\n\r\n";

            // Mostrar algunos tokens importantes
            output += ShowImportantTokens(tokens);

            return output;
        }

        private string PerformSyntacticAnalysis(List<Token> tokens, out Node? ast)
        {
            var output = "📍 FASE 2: ANÁLISIS SINTÁCTICO\r\n";
            output += "──────────────────────────────\r\n";
            
            var parser = new Parser();
            ast = parser.Parse(tokens);

            // Verificar errores sintácticos
            var syntaxErrors = ast.GetAllErrors();
            if (syntaxErrors.Any())
            {
                output += "❌ ERRORES SINTÁCTICOS ENCONTRADOS:\r\n";
                foreach (var error in syntaxErrors)
                {
                    output += $"   • {error}\r\n";
                }
                output += "\r\n❌ COMPILACIÓN DETENIDA\r\n";
                ast = null;
                return output;
            }

            output += "✅ Análisis sintáctico completado exitosamente\r\n";
            output += "🌳 Árbol de Sintaxis Abstracta (AST) generado\r\n\r\n";

            return output;
        }

        private string PerformSemanticAnalysis(Node ast)
        {
            var output = "📍 FASE 3: ANÁLISIS SEMÁNTICO\r\n";
            output += "─────────────────────────────\r\n";
            
            var semanticAnalyzer = new SemanticAnalyzer();
            var semanticErrors = semanticAnalyzer.AnalyzeProgram(ast);

            if (semanticErrors.Any())
            {
                output += "❌ ERRORES SEMÁNTICOS ENCONTRADOS:\r\n";
                foreach (var error in semanticErrors)
                {
                    output += $"   • {error}\r\n";
                }
                output += "\r\n❌ COMPILACIÓN DETENIDA\r\n";
                return output;
            }

            output += "✅ Análisis semántico completado exitosamente\r\n";
            output += "✅ Todas las validaciones de tipos y scope pasaron\r\n\r\n";

            return output;
        }

        private string ShowImportantTokens(List<Token> tokens)
        {
            var output = "🔍 TOKENS PRINCIPALES:\r\n";
            var importantTokens = tokens.Take(10).ToList();
            foreach (var token in importantTokens)
            {
                output += $"   {token.Type}: '{token.Value}'\r\n";
            }
            if (tokens.Count > 10)
                output += $"   ... y {tokens.Count - 10} tokens más\r\n";
            output += "\r\n";

            return output;
        }

        private string GetSuccessMessage()
        {
            var output = "🎉 COMPILACIÓN EXITOSA\r\n";
            output += "═══════════════════════\r\n";
            output += "✓ Análisis léxico: CORRECTO\r\n";
            output += "✓ Análisis sintáctico: CORRECTO\r\n";
            output += "✓ Análisis semántico: CORRECTO\r\n";
            output += "\r\n💡 El código está listo para ejecutarse\r\n";

            return output;
        }
    }
}
