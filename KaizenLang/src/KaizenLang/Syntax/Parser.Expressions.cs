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
                    tokens[pos].Type == "IDENTIFIER" && 
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
                    node.Children.Add(new Node { Type = "Parentheses", Children = { nested } });
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
                    node.Children.Add(new Node { Type = "Operator", Children = { new Node { Type = tokens[pos].Value } } });
                    pos++;
                    continue;
                }

                node.Children.Add(new Node { Type = tokens[pos].Type, Children = { new Node { Type = tokens[pos].Value } } });
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
            node.Children.Add(new Node { Type = "FunctionName", Children = { new Node { Type = tokens[pos].Value } } });
            pos++; // IDENTIFIER
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
                argsNode.Children.Add(ParseExpression(tokens, ref pos));
            }
            
            node.Children.Add(argsNode);
            
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")")
                pos++; // consumir )
                
            return node;
        }

        private Node ParseArrayLiteral(List<Token> tokens, ref int pos)
        {
            Node arrayNode = new Node { Type = "ArrayLiteral" };
            pos++; // consumir [
            
            var elementsNode = new Node { Type = "Elements" };
            
            while (pos < tokens.Count && !(tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "]"))
            {
                if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ",")
                {
                    pos++; // consumir ,
                    continue;
                }
                
                elementsNode.Children.Add(ParseExpression(tokens, ref pos));
            }
            
            arrayNode.Children.Add(elementsNode);
            
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
