using System;
using System.Collections.Generic;

namespace ParadigmasLang
{
    // Analizador sintáctico: convierte tokens en un árbol de sintaxis
    public class Parser
    {
        public Node Parse(List<Token> tokens)
        {
            int pos = 0;
            Node root = new Node { Type = "Program" };
            while (pos < tokens.Count)
            {
                var node = ParseStatement(tokens, ref pos);
                if (node != null) root.Children.Add(node);
                else pos++;
            }
            return root;
        }

        private Node ParseStatement(List<Token> tokens, ref int pos)
        {
            if (Match(tokens, pos, "RESERVED", "if"))
                return ParseIf(tokens, ref pos);
            if (Match(tokens, pos, "RESERVED", "for"))
                return ParseFor(tokens, ref pos);
            if (Match(tokens, pos, "RESERVED", "while"))
                return ParseWhile(tokens, ref pos);
            if (Match(tokens, pos, "RESERVED", "do"))
                return ParseDoWhile(tokens, ref pos);
            if (IsFunctionDeclaration(tokens, pos))
                return ParseFunction(tokens, ref pos);
            // ...otros tipos de sentencias...
            return null;
        }

        private bool Match(List<Token> tokens, int pos, string type, string value)
        {
            return pos < tokens.Count && tokens[pos].Type == type && tokens[pos].Value == value;
        }

        private bool IsFunctionDeclaration(List<Token> tokens, int pos)
        {
            // tipo nombre ( ... ) { ... }
            return pos + 3 < tokens.Count && tokens[pos].Type == "TYPE" && tokens[pos + 1].Type == "IDENTIFIER" && tokens[pos + 2].Type == "DELIMITER" && tokens[pos + 2].Value == "(";
        }

        private Node ParseIf(List<Token> tokens, ref int pos)
        {
            // if (condición) { ... } else { ... }
            Node node = new Node { Type = "If" };
            pos++; // 'if'
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(") pos++;
            node.Children.Add(ParseExpression(tokens, ref pos)); // condición
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")") pos++;
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
            {
                node.Children.Add(ParseBlock(tokens, ref pos));
            }
            if (Match(tokens, pos, "RESERVED", "else"))
            {
                pos++;
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
                    node.Children.Add(ParseBlock(tokens, ref pos));
            }
            return node;
        }

        private Node ParseFor(List<Token> tokens, ref int pos)
        {
            // for (init; cond; inc) { ... }
            Node node = new Node { Type = "For" };
            pos++; // 'for'
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(") pos++;
            node.Children.Add(ParseExpression(tokens, ref pos)); // init
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";") pos++;
            node.Children.Add(ParseExpression(tokens, ref pos)); // cond
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";") pos++;
            node.Children.Add(ParseExpression(tokens, ref pos)); // inc
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")") pos++;
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
                node.Children.Add(ParseBlock(tokens, ref pos));
            return node;
        }

        private Node ParseWhile(List<Token> tokens, ref int pos)
        {
            // while (cond) { ... }
            Node node = new Node { Type = "While" };
            pos++; // 'while'
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(") pos++;
            node.Children.Add(ParseExpression(tokens, ref pos)); // cond
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")") pos++;
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
                node.Children.Add(ParseBlock(tokens, ref pos));
            return node;
        }

        private Node ParseDoWhile(List<Token> tokens, ref int pos)
        {
            // do { ... } while (cond);
            Node node = new Node { Type = "DoWhile" };
            pos++; // 'do'
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
                node.Children.Add(ParseBlock(tokens, ref pos));
            if (Match(tokens, pos, "RESERVED", "while"))
            {
                pos++;
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(") pos++;
                node.Children.Add(ParseExpression(tokens, ref pos));
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")") pos++;
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ";") pos++;
            }
            return node;
        }

        private Node ParseFunction(List<Token> tokens, ref int pos)
        {
            // tipo nombre (params) { ... }
            Node node = new Node { Type = "Function" };
            node.Children.Add(new Node { Type = "Type", Children = { new Node { Type = tokens[pos].Value } } });
            pos++;
            node.Children.Add(new Node { Type = "Name", Children = { new Node { Type = tokens[pos].Value } } });
            pos++;
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(") pos++;
            // parámetros
            var paramsNode = new Node { Type = "Params" };
            while (tokens[pos].Type != "DELIMITER" || tokens[pos].Value != ")")
            {
                if (tokens[pos].Type == "TYPE")
                {
                    var paramType = tokens[pos].Value;
                    pos++;
                    var paramName = tokens[pos].Value;
                    pos++;
                    paramsNode.Children.Add(new Node { Type = "Param", Children = { new Node { Type = paramType }, new Node { Type = paramName } } });
                    if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ",") pos++;
                }
                else pos++;
            }
            node.Children.Add(paramsNode);
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")") pos++;
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "{")
                node.Children.Add(ParseBlock(tokens, ref pos));
            return node;
        }

        private Node ParseBlock(List<Token> tokens, ref int pos)
        {
            // { ... }
            Node node = new Node { Type = "Block" };
            pos++; // '{'
            while (pos < tokens.Count && !(tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "}"))
            {
                var stmt = ParseStatement(tokens, ref pos);
                if (stmt != null) node.Children.Add(stmt);
                else pos++;
            }
            pos++; // '}'
            return node;
        }

        private Node ParseExpression(List<Token> tokens, ref int pos)
        {
            // Simplificado: solo toma el siguiente token como expresión
            Node node = new Node { Type = "Expression" };
            if (pos < tokens.Count)
            {
                node.Children.Add(new Node { Type = tokens[pos].Type, Children = { new Node { Type = tokens[pos].Value } } });
                pos++;
            }
            return node;
        }
    }

    public class Node
    {
        public string Type { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
    }
}
