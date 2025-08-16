using System;
using System.Collections.Generic;

namespace ParadigmasLang
{
    // Analizador léxico: convierte el código fuente en tokens
    public class Lexer
    {
        public List<Token> Tokenize(string source)
        {
            // Implementar reglas léxicas aquí
            var tokens = new List<Token>();
            var reserved = new HashSet<string> {
                "output", "input", "void", "do", "while", "for", "if", "else", "return", "true", "false"
            };
            var types = new HashSet<string> {
                "int", "float", "double", "boolean", "char", "string", "array"
            };
            var operators = new HashSet<string> {
                "+", "-", "*", "/", "%", "==", "!=", "<", ">", "<=", ">=", "&&", "||", "!", "=", "++", "--"
            };
            var delimiters = new HashSet<string> { "(", ")", "{", "}", ";", "," };

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
                        tokens.Add(new Token { Type = "TYPE", Value = word });
                    else if (reserved.Contains(word))
                        tokens.Add(new Token { Type = "RESERVED", Value = word });
                    else
                        tokens.Add(new Token { Type = "IDENTIFIER", Value = word });
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
                        tokens.Add(new Token { Type = "FLOAT", Value = source.Substring(start, i - start) });
                    }
                    else
                    {
                        tokens.Add(new Token { Type = "INT", Value = source.Substring(start, i - start) });
                    }
                    continue;
                }

                // Operadores (multi-char primero)
                bool matched = false;
                foreach (var op in new[] { "==", "!=", "<=", ">=", "&&", "||", "++", "--" })
                {
                    if (source.Substring(i).StartsWith(op))
                    {
                        tokens.Add(new Token { Type = "OPERATOR", Value = op });
                        i += op.Length;
                        matched = true;
                        break;
                    }
                }
                if (matched) continue;

                // Operadores y delimitadores de un solo caracter
                if (operators.Contains(source[i].ToString()))
                {
                    tokens.Add(new Token { Type = "OPERATOR", Value = source[i].ToString() });
                    i++;
                    continue;
                }
                if (delimiters.Contains(source[i].ToString()))
                {
                    tokens.Add(new Token { Type = "DELIMITER", Value = source[i].ToString() });
                    i++;
                    continue;
                }

                // Strings
                if (source[i] == '"')
                {
                    int start = ++i;
                    while (i < source.Length && source[i] != '"') i++;
                    string str = source.Substring(start, i - start);
                    tokens.Add(new Token { Type = "STRING", Value = str });
                    i++; // skip closing quote
                    continue;
                }

                // Comentarios (línea)
                if (source[i] == '/' && i + 1 < source.Length && source[i + 1] == '/')
                {
                    while (i < source.Length && source[i] != '\n') i++;
                    continue;
                }

                // Si no se reconoce, marcar como inválido
                tokens.Add(new Token { Type = "INVALID", Value = source[i].ToString() });
                i++;
            }
            return tokens;
        }
    }

    public class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public Token()
        {
            Type = string.Empty;
            Value = string.Empty;
        }
    }
}
