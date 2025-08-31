// Archivo duplicado eliminado. Mantener solo una definición de Lexer y Token en el proyecto.
namespace ParadigmasLang
{
    public class Lexer
    {
        public List<Token> Tokenize(string source)
        {
            var tokens = new List<Token>();

            int i = 0;
            // Tipos válidos extendidos
            var validTypes = new HashSet<string>(TypeWords.Words.Concat(new[] { "array", "string" }));

            while (i < source.Length)
            {
                if (char.IsWhiteSpace(source[i])) { i++; continue; }

                // Identificadores, tipos y palabras reservadas
                if (char.IsLetter(source[i]))
                {
                    int start = i;
                    while (i < source.Length && (char.IsLetterOrDigit(source[i]) || source[i] == '_')) i++;
                    string word = source.Substring(start, i - start);

                    if (validTypes.Contains(word))
                        tokens.Add(new Token("TYPE", word));
                    else if (ReservedWords.Words.Contains(word))
                        tokens.Add(new Token("RESERVED", word));
                    else if (LiteralWords.Words.Contains(word))
                        tokens.Add(new Token("LITERAL", word));
                    else
                        tokens.Add(new Token("IDENTIFIER", word));
                    continue;
                }

                // Números
                if (char.IsDigit(source[i]))
                {
                    int start = i;
                    while (i < source.Length && char.IsDigit(source[i])) i++;
                    if (i < source.Length && source[i] == '.')
                    {
                        i++;
                        while (i < source.Length && char.IsDigit(source[i])) i++;
                        tokens.Add(new Token("FLOAT", source.Substring(start, i - start)));
                    }
                    else
                    {
                        tokens.Add(new Token("INT", source.Substring(start, i - start)));
                    }
                    continue;
                }

                // Strings
                if (source[i] == '"')
                {
                    int start = ++i;
                    int strStart = start - 1;
                    while (i < source.Length && source[i] != '"') i++;
                    if (i < source.Length)
                    {
                        string str = source.Substring(start, i - start);
                        tokens.Add(new Token("STRING", str));
                        i++; // skip closing quote
                    }
                    else
                    {
                        tokens.Add(new Token("INVALID", $"Cadena sin cierre: {source.Substring(strStart)}"));
                    }
                    continue;
                }

                // Chars
                if (source[i] == '\'')
                {
                    int start = ++i;
                    if (i + 1 < source.Length && source[i + 1] == '\'')
                    {
                        tokens.Add(new Token("CHAR", source[i].ToString()));
                        i += 2; // skip closing '
                    }
                    else
                    {
                        tokens.Add(new Token("INVALID", $"Literal de char inválido en posición {start - 1}"));
                        i++;
                    }
                    continue;
                }

                // Comentarios (línea y bloque)
                if (source[i] == '/' && i + 1 < source.Length)
                {
                    if (source[i + 1] == '/')
                    {
                        while (i < source.Length && source[i] != '\n') i++;
                        continue;
                    }
                    else if (source[i + 1] == '*')
                    {
                        int commentStart = i;
                        i += 2;
                        while (i + 1 < source.Length && !(source[i] == '*' && source[i + 1] == '/')) i++;
                        if (i + 1 < source.Length)
                        {
                            i += 2; // skip */
                        }
                        else
                        {
                            tokens.Add(new Token("INVALID", $"Comentario de bloque sin cierre desde posición {commentStart}"));
                        }
                        continue;
                    }
                }

                // Operadores multi-char
                bool matched = false;
                foreach (var op in OperatorWords.Words.OrderByDescending(x => x.Length))
                {
                    if (source.Substring(i).StartsWith(op))
                    {
                        tokens.Add(new Token("OPERATOR", op));
                        i += op.Length;
                        matched = true;
                        break;
                    }
                }
                if (matched) continue;

                // Delimitadores
                if (DelimiterWords.Words.Contains(source[i].ToString()))
                {
                    tokens.Add(new Token("DELIMITER", source[i].ToString()));
                    i++;
                    continue;
                }

                // Si no se reconoce
                tokens.Add(new Token("INVALID", $"Carácter no reconocido '{source[i]}' en posición {i}"));
                i++;
            }
            return tokens;
        }
    }

    // Token se encuentra ahora en src/KaizenLang/Tokens/Token.cs
}
