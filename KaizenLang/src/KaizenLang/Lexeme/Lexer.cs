// Archivo duplicado eliminado. Mantener solo una definición de Lexer y Token en el proyecto.
namespace ParadigmasLang
{
    public class Lexer
    {
        public List<Token> Tokenize(string source)
        {
            var tokenizer = new Tokenizer(source);
            return tokenizer.Tokenize();
        }
    }

    // Token se encuentra ahora en src/KaizenLang/Tokens/Token.cs
}
