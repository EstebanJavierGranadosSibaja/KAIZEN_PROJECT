using System.Collections.Generic;

namespace ParadigmasLang;

public partial class Parser
{
    // Operator precedence levels (higher number = higher precedence)
    private int GetPrecedence(string op)
    {
        switch (op)
        {
            case OperatorWords.OR:
                return 1;
            case OperatorWords.AND:
                return 2;
            case OperatorWords.EQUAL:
            case OperatorWords.NOT_EQUAL:
                return 3;
            case OperatorWords.LESS:
            case OperatorWords.LESS_EQUAL:
            case OperatorWords.GREATER:
            case OperatorWords.GREATER_EQUAL:
                return 4;
            case OperatorWords.ADD:
            case OperatorWords.SUBTRACT:
                return 5;
            case OperatorWords.MULTIPLY:
            case OperatorWords.DIVIDE:
            case OperatorWords.MODULO:
                return 6;
            default:
                return 0; // Unknown operator
        }
    }

    private Node ParseExpression(List<Token> tokens, ref int pos)
    {
        return ParseExpressionWithPrecedence(tokens, ref pos, 0);
    }

    private Node ParseExpressionWithPrecedence(List<Token> tokens, ref int pos, int minPrecedence)
    {
        // Parse left operand (could be unary, primary, etc.)
        var left = ParseUnaryOrPrimary(tokens, ref pos);

        // Parse binary operators with precedence climbing
        while (pos < tokens.Count)
        {
            // Check for stopping delimiters
            if (tokens[pos].Type == "DELIMITER" &&
                (tokens[pos].Value == DelimiterWords.SEMICOLON || tokens[pos].Value == DelimiterWords.PAREN_CLOSE ||
                 tokens[pos].Value == DelimiterWords.COMMA || tokens[pos].Value == DelimiterWords.BRACKET_CLOSE || tokens[pos].Value == DelimiterWords.BLOCK_END))
                break;

            // Check if next token is a binary operator
            if (tokens[pos].Type != "OPERATOR")
                break;

            string op = tokens[pos].Value;

            // Skip assignment operator (handled separately)
            if (op == OperatorWords.ASSIGN)
                break;

            int precedence = GetPrecedence(op);

            // If precedence is lower than minimum, stop (let parent handle it)
            if (precedence < minPrecedence)
                break;

            var opToken = tokens[pos];
            pos++; // consume operator

            // Parse right operand with higher precedence (left-associative)
            var right = ParseExpressionWithPrecedence(tokens, ref pos, precedence + 1);

            // Create binary operation node
            var binaryNode = new Node { Type = "Expression" };
            binaryNode.Line = opToken.Line;
            binaryNode.Column = opToken.Column;
            binaryNode.Children.Add(left);

            var opNode = new Node { Type = "Operator", Children = { new Node { Type = op } } };
            opNode.Line = opToken.Line;
            opNode.Column = opToken.Column;
            binaryNode.Children.Add(opNode);
            binaryNode.Children.Add(right);

            left = binaryNode;
        }

        return left;
    }

    private Node ParseUnaryOrPrimary(List<Token> tokens, ref int pos)
    {
        // Handle unary operators (-, !, +)
        if (pos < tokens.Count && tokens[pos].Type == "OPERATOR")
        {
            string opValue = tokens[pos].Value;
            if (opValue == OperatorWords.SUBTRACT || opValue == OperatorWords.NOT || opValue == OperatorWords.ADD)
            {
                var unaryNode = new Node { Type = "UnaryExpression" };
                unaryNode.Line = tokens[pos].Line;
                unaryNode.Column = tokens[pos].Column;

                var opNode = new Node { Type = "Operator", Children = { new Node { Type = opValue } } };
                opNode.Line = tokens[pos].Line;
                opNode.Column = tokens[pos].Column;
                unaryNode.Children.Add(opNode);
                pos++; // consume operator

                // Recursively parse operand (supports chained unary like --x)
                var operand = ParseUnaryOrPrimary(tokens, ref pos);
                unaryNode.Children.Add(operand);
                return unaryNode;
            }
        }

        // Handle postfix increment/decrement: IDENTIFIER++ or IDENTIFIER--
        if (pos + 1 < tokens.Count && tokens[pos].Type == "IDENTIFIER" && tokens[pos + 1].Type == "OPERATOR" &&
            (tokens[pos + 1].Value == OperatorWords.INCREMENT || tokens[pos + 1].Value == OperatorWords.DECREMENT))
        {
            var idTok = tokens[pos];
            var opTok = tokens[pos + 1];

            var targetIdentifier = new Node("IDENTIFIER", new List<Node> { new Node(idTok.Value) })
            {
                Line = idTok.Line,
                Column = idTok.Column
            };

            // Build the arithmetic part: IDENTIFIER +/- 1
            var lhsIdentifier = new Node("IDENTIFIER", new List<Node> { new Node(idTok.Value) })
            {
                Line = idTok.Line,
                Column = idTok.Column
            };

            var opSymbol = opTok.Value == OperatorWords.INCREMENT ? OperatorWords.ADD : OperatorWords.SUBTRACT;
            var opNode = new Node("Operator", new List<Node> { new Node(opSymbol) })
            {
                Line = opTok.Line,
                Column = opTok.Column
            };

            var literalOne = new Node("INT", new List<Node> { new Node("1") })
            {
                Line = opTok.Line,
                Column = opTok.Column
            };

            var arithmeticExpression = new Node("Expression", new List<Node> { lhsIdentifier, opNode, literalOne })
            {
                Line = idTok.Line,
                Column = idTok.Column
            };

            pos += 2; // consume IDENTIFIER and ++/--

            var assignment = new Node("Assignment", new List<Node> { targetIdentifier, arithmeticExpression })
            {
                Line = idTok.Line,
                Column = idTok.Column
            };

            return assignment;
        }

        // Handle assignment expressions: IDENTIFIER = <expr> or IDENTIFIER[...] = <expr>
        if (pos < tokens.Count && tokens[pos].Type == "IDENTIFIER")
        {
            // Scan ahead to check for '=' (assignment operator)
            int scan = pos + 1;
            while (scan < tokens.Count && tokens[scan].Type == "DELIMITER" && tokens[scan].Value == "[")
            {
                // Skip past indexers
                scan++; // after [
                while (scan < tokens.Count && !(tokens[scan].Type == "DELIMITER" && tokens[scan].Value == "]"))
                    scan++;
                if (scan < tokens.Count && tokens[scan].Type == "DELIMITER" && tokens[scan].Value == "]")
                    scan++; // consume ]
                else
                    break;
            }

            // Check if we found assignment operator
            if (scan < tokens.Count && tokens[scan].Type == "OPERATOR" && tokens[scan].Value == OperatorWords.ASSIGN)
            {
                // Parse LHS (identifier or indexed access)
                Node lhs;
                if (pos + 1 < tokens.Count && tokens[pos + 1].Type == "DELIMITER" && tokens[pos + 1].Value == "[")
                {
                    lhs = ParseIndexAccess(tokens, ref pos);
                }
                else
                {
                    var idTok = tokens[pos];
                    lhs = new Node("IDENTIFIER", new List<Node> { new Node(idTok.Value) }) { Line = idTok.Line, Column = idTok.Column };
                    pos++; // consume IDENTIFIER
                }

                // Consume '=' operator
                if (pos < tokens.Count && tokens[pos].Type == "OPERATOR" && tokens[pos].Value == OperatorWords.ASSIGN)
                    pos++; // consume '='

                // Parse RHS expression
                var rhs = ParseExpression(tokens, ref pos);
                var assignNode = new Node("Assignment", new List<Node> { lhs, rhs }) { Line = lhs.Line, Column = lhs.Column };
                return assignNode;
            }
        }

        // Parse primary expression (function call, parentheses, array literal, identifier, literal)
        return ParsePrimaryExpression(tokens, ref pos);
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

    // Helper method to parse a primary expression or unary expression
    private Node ParsePrimaryOrUnaryExpression(List<Token> tokens, ref int pos)
    {
        // Check for unary operator
        if (pos < tokens.Count && tokens[pos].Type == "OPERATOR")
        {
            string opValue = tokens[pos].Value;
            if (opValue == "-" || opValue == "!" || opValue == "+")
            {
                var unaryNode = new Node { Type = "UnaryExpression" };
                unaryNode.Line = tokens[pos].Line;
                unaryNode.Column = tokens[pos].Column;

                var opNode = new Node { Type = "Operator", Children = { new Node { Type = opValue } } };
                opNode.Line = tokens[pos].Line;
                opNode.Column = tokens[pos].Column;
                unaryNode.Children.Add(opNode);
                pos++; // consume operator

                // Recursively parse operand (supports chained unary operators like --x)
                var operand = ParsePrimaryOrUnaryExpression(tokens, ref pos);
                unaryNode.Children.Add(operand);
                return unaryNode;
            }
        }

        // Not a unary operator, parse primary expression
        return ParsePrimaryExpression(tokens, ref pos);
    }

    // Helper method to parse a primary expression (literal, identifier, function call, etc.)
    private Node ParsePrimaryExpression(List<Token> tokens, ref int pos)
    {
        if (pos >= tokens.Count)
            return ErrorNode("Se esperaba una expresión", pos);

        // Function call: IDENTIFIER (
        if (pos < tokens.Count && tokens[pos].Type == "IDENTIFIER" &&
            pos + 1 < tokens.Count && tokens[pos + 1].Type == "DELIMITER" && tokens[pos + 1].Value == "(")
        {
            return ParseFunctionCall(tokens, ref pos);
        }

        // Reserved word function call (like output, input)
        if (pos < tokens.Count && tokens[pos].Type == "RESERVED" &&
            pos + 1 < tokens.Count && tokens[pos + 1].Type == "DELIMITER" && tokens[pos + 1].Value == "(")
        {
            return ParseFunctionCall(tokens, ref pos);
        }

        // Parenthesized expression
        if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "(")
        {
            pos++; // consume (
            var nested = ParseExpression(tokens, ref pos);
            var par = new Node { Type = "Parentheses", Children = { nested } };
            par.Line = tokens[Math.Max(0, pos - 1)].Line;
            par.Column = tokens[Math.Max(0, pos - 1)].Column;
            if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == ")")
                pos++; // consume )
            return par;
        }

        // Array literal
        if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == "[")
        {
            return ParseArrayLiteral(tokens, ref pos);
        }

        // Indexed access: IDENTIFIER[...]
        if (tokens[pos].Type == "IDENTIFIER" && pos + 1 < tokens.Count &&
            tokens[pos + 1].Type == "DELIMITER" && tokens[pos + 1].Value == "[")
        {
            return ParseIndexAccess(tokens, ref pos);
        }

        // Simple identifier
        if (tokens[pos].Type == "IDENTIFIER")
        {
            var idTok = tokens[pos];
            var idNode = new Node("IDENTIFIER", new List<Node> { new Node(idTok.Value) });
            idNode.Line = idTok.Line;
            idNode.Column = idTok.Column;
            pos++;
            return idNode;
        }

        // Literals (INT, FLOAT, BOOL, STRING, TRUE/FALSE/NULL)
        if (tokens[pos].Type == "INT" || tokens[pos].Type == "FLOAT" ||
            tokens[pos].Type == "BOOL" || tokens[pos].Type == "STRING" ||
            tokens[pos].Type == "LITERAL" || tokens[pos].Type == "CHAR")
        {
            var litTok = tokens[pos];
            var litNode = new Node(litTok.Type, new List<Node> { new Node(litTok.Value) });
            litNode.Line = litTok.Line;
            litNode.Column = litTok.Column;
            pos++;
            return litNode;
        }

        // Unknown token - create error node
        return ErrorNode($"Token inesperado: {tokens[pos].Type} '{tokens[pos].Value}'", pos);
    }
}
