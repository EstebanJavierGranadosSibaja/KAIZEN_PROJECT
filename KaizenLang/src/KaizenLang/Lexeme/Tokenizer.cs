using System;
using System.Collections.Generic;
using System.Linq;

namespace ParadigmasLang
{
    internal class Tokenizer
    {
        private readonly CharStream stream;

        public Tokenizer(string source)
        {
            stream = new CharStream(source ?? string.Empty);
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            // Usar solo los tipos definidos en TypeWords
            var validTypes = new HashSet<string>(TypeWords.Words);

            while (!stream.EOF)
            {
                var ch = stream.Peek();
                if (ch == null) break;

                if (char.IsWhiteSpace(ch.Value))
                {
                    stream.Read();
                    continue;
                }

                if (char.IsLetter(ch.Value))
                {
                    var word = stream.ReadWhile(c => char.IsLetterOrDigit(c) || c == '_');
                    if (DelimiterWords.MultiCharDelimiters.Contains(word))
                    {
                        tokens.Add(new Token("DELIMITER", word));
                    }
                    else if (validTypes.Contains(word))
                        tokens.Add(new Token("TYPE", word));
                    else if (ReservedWords.Words.Contains(word))
                        tokens.Add(new Token("RESERVED", word));
                    else if (LiteralWords.Words.Contains(word))
                        tokens.Add(new Token("LITERAL", word));
                    else
                        tokens.Add(new Token("IDENTIFIER", word));
                    continue;
                }

                if (char.IsDigit(ch.Value))
                {
                    var number = stream.ReadWhile(c => char.IsDigit(c));
                    if (stream.Peek() == '.')
                    {
                        stream.Read();
                        number += "." + stream.ReadWhile(c => char.IsDigit(c));
                        tokens.Add(new Token("FLOAT", number));
                    }
                    else
                    {
                        tokens.Add(new Token("INT", number));
                    }
                    continue;
                }

                if (ch == '"')
                {
                    stream.Read(); // skip opening quote
                    int startPos = stream.Position;
                    while (!stream.EOF && stream.Peek() != '"') stream.Read();
                    if (!stream.EOF && stream.Peek() == '"')
                    {
                        var len = stream.Position - startPos;
                        var str = stream.ReadWhile(c => true); // read remaining until before closing quote
                        // Move past the closing quote
                        if (stream.Peek() == '"') stream.Read();
                        tokens.Add(new Token("STRING", str));
                    }
                    else
                    {
                        tokens.Add(new Token("INVALID", $"Cadena sin cierre desde posición {startPos - 1}"));
                    }
                    continue;
                }

                if (ch == '\'')
                {
                    stream.Read(); // skip opening '
                    var first = stream.Read();
                    if (first != null && stream.Peek() == '\'')
                    {
                        stream.Read(); // skip closing '
                        tokens.Add(new Token("CHAR", first?.ToString() ?? string.Empty));
                    }
                    else
                    {
                        tokens.Add(new Token("INVALID", $"Literal de char inválido en posición {stream.Position - 1}"));
                    }
                    continue;
                }

                // Comentarios
                if (ch == '/' && stream.Peek(1) == '/')
                {
                    // line comment
                    stream.Read(); stream.Read();
                    while (!stream.EOF && stream.Peek() != '\n') stream.Read();
                    continue;
                }
                if (ch == '/' && stream.Peek(1) == '*')
                {
                    int commentStart = stream.Position;
                    stream.Read(); stream.Read();
                    while (!stream.EOF && !(stream.Peek() == '*' && stream.Peek(1) == '/')) stream.Read();
                    if (!stream.EOF)
                    {
                        stream.Read(); stream.Read(); // skip */
                    }
                    else
                    {
                        tokens.Add(new Token("INVALID", $"Comentario de bloque sin cierre desde posición {commentStart}"));
                    }
                    continue;
                }

                // Operadores multi-char
                bool matched = false;
                foreach (var op in OperatorWords.Words.OrderByDescending(x => x.Length))
                {
                    var candidate = "";
                    for (int i = 0; i < op.Length; i++)
                    {
                        var p = stream.Peek(i);
                        if (p == null) { candidate = null; break; }
                        candidate += p.Value;
                    }
                    if (candidate == op)
                    {
                        // consume op.Length chars
                        for (int i = 0; i < op.Length; i++) stream.Read();
                        tokens.Add(new Token("OPERATOR", op));
                        matched = true;
                        break;
                    }
                }
                if (matched) continue;

                var single = stream.Peek()?.ToString();
                if (single != null && DelimiterWords.SingleCharDelimiters.Contains(single))
                {
                    tokens.Add(new Token("DELIMITER", single));
                    stream.Read();
                    continue;
                }

                // unrecognized
                var invalidChar = stream.Read();
                tokens.Add(new Token("INVALID", $"Carácter no reconocido '{invalidChar}' en posición {stream.Position - 1}"));
            }

            return tokens;
        }
    }
}
