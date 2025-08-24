namespace ParadigmasLang
{
    public class Lexer
    {
        public List<Token> Tokenize(string source)
        {
            var tokens = new List<Token>();
            var reserved = new HashSet<string> {
                "output", "input", "void", "do", "while", "for", "if", "else", "return"
            };
            var literals = new HashSet<string> { "true", "false", "null" };
            var types = new HashSet<string> {
                "int", "float", "double", "boolean", "char", "string", "array", "list", "matrix"
            };
            var operators = new HashSet<string> {
                "+", "-", "*", "/", "%", "==", "!=", "<", ">", "<=", ">=", "&&", "||", "!",
                "=", "+=", "-=", "*=", "/=", "++", "--"
            };
            var delimiters = new HashSet<string> { "(", ")", "{", "}", ";", ",", "[", "]" };

            int i = 0;
            while (i < source.Length)
            {
                if (char.IsWhiteSpace(source[i])) { i++; continue; }

                // Identificadores, tipos y palabras reservadas
                if (char.IsLetter(source[i]))
                {
                    int start = i;
                    while (i < source.Length && (char.IsLetterOrDigit(source[i]) || source[i] == '_')) i++;
                    string word = source.Substring(start, i - start);

                    if (types.Contains(word))
                        tokens.Add(new Token("TYPE", word));
                    else if (reserved.Contains(word))
                        tokens.Add(new Token("RESERVED", word));
                    else if (literals.Contains(word))
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
                    while (i < source.Length && source[i] != '"') i++;
                    string str = source.Substring(start, i - start);
                    tokens.Add(new Token("STRING", str));
                    i++; // skip closing quote
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
                        tokens.Add(new Token("INVALID", "Invalid char literal"));
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
                        i += 2;
                        while (i + 1 < source.Length && !(source[i] == '*' && source[i + 1] == '/')) i++;
                        i += 2; // skip */
                        continue;
                    }
                }

                // Operadores multi-char
                bool matched = false;
                foreach (var op in operators.OrderByDescending(x => x.Length))
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
                if (delimiters.Contains(source[i].ToString()))
                {
                    tokens.Add(new Token("DELIMITER", source[i].ToString()));
                    i++;
                    continue;
                }

                // Si no se reconoce
                tokens.Add(new Token("INVALID", source[i].ToString()));
                i++;
            }
            return tokens;
        }
    }

    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public Token(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
