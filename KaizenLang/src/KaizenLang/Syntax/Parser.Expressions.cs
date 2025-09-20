using System.Collections.Generic;

namespace ParadigmasLang
{
    public partial class Parser
    {
        private Node ParseExpression(List<Token> tokens, ref int pos)
        {
            Node node = new Node { Type = "Expression" };
            int startPos = pos;
            
            while (pos < tokens.Count)
            {
                if (tokens[pos].Type == "DELIMITER" && 
                    (tokens[pos].Value == ";" || tokens[pos].Value == ")" || 
                     tokens[pos].Value == "," || tokens[pos].Value == "yang"))
                    break;

                if (pos + 1 < tokens.Count && 
                    (tokens[pos].Type == "IDENTIFIER" || tokens[pos].Type == "RESERVED") && 
                    tokens[pos + 1].Type == "DELIMITER" && 
                    tokens[pos + 1].Value == "(")
                {
                    var funcCall = ParseFunctionCall(tokens, ref pos);
                    node.Children.Add(funcCall);
                    continue;
                }

                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(")
                {
                    pos++; // consumir (
                    var nested = ParseExpression(tokens, ref pos);
                    var par = new Node { Type = "Parentheses", Children = { nested } };
                    // use token position from the opening '('
                    par.Line = tokens[Math.Max(0, pos - 1)].Line;
                    par.Column = tokens[Math.Max(0, pos - 1)].Column;
                    node.Children.Add(par);
                    if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")")
                        pos++; // consumir )
                    continue;
                }

                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "[")
                {
                    var arrayNode = ParseArrayLiteral(tokens, ref pos);
                    node.Children.Add(arrayNode);
                    continue;
                }

                if (tokens[pos].Type == "OPERATOR")
                {
                    // Treat operator tokens as Operator nodes with the operator symbol as a child
                    var op = new Node { Type = "Operator", Children = { new Node { Type = tokens[pos].Value } } };
                    op.Line = tokens[pos].Line;
                    op.Column = tokens[pos].Column;
                    node.Children.Add(op);
                    pos++;
                    continue;
                }

                // Literal or identifier token -> create node with position
                var leaf = new Node { Type = tokens[pos].Type, Children = { new Node { Type = tokens[pos].Value } } };
                leaf.Line = tokens[pos].Line;
                leaf.Column = tokens[pos].Column;
                node.Children.Add(leaf);
                pos++;
            }

            if (node.Children.Count == 0)
            {
                node.Children.Add(ErrorNode("Expresión vacía", startPos));
            }

            return node;
        }

        private Node ParseFunctionCall(List<Token> tokens, ref int pos)
        {
            Node node = new Node { Type = "FunctionCall" };
            
            // Nombre de la función
            var fn = new Node { Type = "FunctionName", Children = { new Node { Type = tokens[pos].Value } } };
            fn.Line = tokens[pos].Line;
            fn.Column = tokens[pos].Column;
            node.Children.Add(fn);
            pos++; // IDENTIFIER
            // remember '(' position
            int openParenPos = pos;
            pos++; // (
            
            // Argumentos
            var argsNode = new Node { Type = "Arguments" };
            while (pos < tokens.Count && !(tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")"))
            {
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ",")
                {
                    pos++; // consumir ,
                    continue;
                }
                var arg = ParseExpression(tokens, ref pos);
                argsNode.Children.Add(arg);
            }
            
            node.Children.Add(argsNode);
            
            // set positions for function call using function name or open paren
            node.Line = fn.Line;
            node.Column = fn.Column;

            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")")
                pos++; // consumir )
                
            return node;
        }

        private Node ParseArrayLiteral(List<Token> tokens, ref int pos)
        {
            Node arrayNode = new Node { Type = "ArrayLiteral" };
            // capture '[' position
            int start = pos;
            pos++; // consumir [
            
            var elementsNode = new Node { Type = "Elements" };
            
            while (pos < tokens.Count && !(tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "]"))
            {
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ",")
                {
                    pos++; // consumir ,
                    continue;
                }
                
                var el = ParseExpression(tokens, ref pos);
                elementsNode.Children.Add(el);
            }
            
            arrayNode.Children.Add(elementsNode);
            
            // set position for array from opening '[' token
            arrayNode.Line = tokens[start].Line;
            arrayNode.Column = tokens[start].Column;

            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "]")
                pos++; // consumir ]
            else
            {
                arrayNode.Children.Add(ErrorNode("Se esperaba ']' al final del array", pos));
            }
                
            return arrayNode;
        }
    }
}
