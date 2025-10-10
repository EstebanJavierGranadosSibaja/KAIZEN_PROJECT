using System.Collections.Generic;

namespace ParadigmasLang;

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
                 tokens[pos].Value == "," || tokens[pos].Value == "]" || tokens[pos].Value == "yang"))
                break;

            // Detect function call shapes: NAME(...)
            if (pos + 1 < tokens.Count &&
                (tokens[pos].Type == "IDENTIFIER" || tokens[pos].Type == "RESERVED") &&
                tokens[pos + 1].Type == "DELIMITER" &&
                tokens[pos + 1].Value == "(")
            {
                var funcCall = ParseFunctionCall(tokens, ref pos);
                node.Children.Add(funcCall);
                continue;
            }

            // Detect simple assignment expressions like: IDENTIFIER = <expr>
            // Assignment to identifier or index access: e.g. a = <expr> or a[0] = <expr>
            // We need to detect '=' after an identifier possibly followed by indexers
            if (pos < tokens.Count && tokens[pos].Type == "IDENTIFIER")
            {
                // scan ahead past any indexers to see if next non-index token is '='
                int scan = pos + 1;
                while (scan < tokens.Count && tokens[scan].Type == "DELIMITER" && tokens[scan].Value == "[")
                {
                    // advance to matching ]
                    scan++; // after [
                    // skip until ']' or end
                    while (scan < tokens.Count && !(tokens[scan].Type == "DELIMITER" && tokens[scan].Value == "]"))
                        scan++;
                    if (scan < tokens.Count && tokens[scan].Type == "DELIMITER" && tokens[scan].Value == "]")
                        scan++; // consume ]
                    else
                        break; // malformed, stop scanning
                }

                if (scan < tokens.Count && tokens[scan].Type == "OPERATOR" && tokens[scan].Value == OperatorWords.ASSIGN)
                {
                    // we detected an assignment with LHS possibly containing indexers
                    Node lhs;
                    // parse LHS properly: if there are indexers, use ParseIndexAccess, else simple identifier
                    if (pos + 1 < tokens.Count && tokens[pos+1].Type == "DELIMITER" && tokens[pos+1].Value == "[")
                    {
                        lhs = ParseIndexAccess(tokens, ref pos);
                    }
                    else
                    {
                        var idTok = tokens[pos];
                        lhs = new Node("IDENTIFIER", new List<Node> { new Node(idTok.Value) }) { Line = idTok.Line, Column = idTok.Column };
                        pos++; // consume IDENTIFIER
                    }

                    // now expect '=' operator
                    if (pos < tokens.Count && tokens[pos].Type == "OPERATOR" && tokens[pos].Value == OperatorWords.ASSIGN)
                        pos++; // consume '='

                    var rhs = ParseExpression(tokens, ref pos);
                    var assignNode = new Node("Assignment", new List<Node> { lhs, rhs }) { Line = lhs.Line, Column = lhs.Column };
                    node.Children.Add(assignNode);
                    continue;
                }
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

            // Identifier possibly with indexers (a[0], m[0][1])
            if (tokens[pos].Type == "IDENTIFIER")
            {
                // If next token is '[' then parse index access
                if (pos + 1 < tokens.Count && tokens[pos+1].Type == "DELIMITER" && tokens[pos+1].Value == "[")
                {
                    var idx = ParseIndexAccess(tokens, ref pos);
                    node.Children.Add(idx);
                    continue;
                }
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

    // Parse an identifier possibly followed by one or more indexers: e.g. a, a[0], m[0][1]
    private Node ParseIndexAccess(List<Token> tokens, ref int pos)
    {
        // Expect IDENTIFIER at pos
        if (pos >= tokens.Count || tokens[pos].Type != "IDENTIFIER")
            return new Node("Identifier") { Type = "IDENTIFIER" };

        var idTok = tokens[pos];
        var currentNode = new Node("IDENTIFIER", new List<Node> { new Node(idTok.Value) }) { Line = idTok.Line, Column = idTok.Column };
        pos++; // consume identifier

        // While next token is '[' parse an index expression
        while (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "[")
        {
            pos++; // consume '['
            var idxExpr = ParseExpression(tokens, ref pos);
            // expect ']'
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "]")
            {
                pos++; // consume ']'
            }
            else
            {
                // malformed indexer: produce error node but continue
                var err = ErrorNode("Se esperaba ']' en indexador", pos);
                // attach and break
                var iaErr = new Node { Type = "IndexAccess" };
                iaErr.Children.Add(currentNode);
                iaErr.Children.Add(idxExpr);
                iaErr.Children.Add(err);
                return iaErr;
            }

            // Build a new IndexAccess node with currentNode as target and idxExpr as index
            var idxNode = new Node { Type = "IndexAccess" };
            idxNode.Children.Add(currentNode);
            idxNode.Children.Add(idxExpr);
            currentNode = idxNode;
        }

        return currentNode;
    }
}
