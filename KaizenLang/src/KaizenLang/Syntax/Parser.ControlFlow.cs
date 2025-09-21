using System.Collections.Generic;

namespace ParadigmasLang;

public partial class Parser
{
    private Node ParseIf(List<Token> tokens, ref int pos)
    {
        var ifNode = new Node("If");
        pos++; // Consumir 'if'

        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_OPEN)
            pos++;
        else
            return ErrorNode("Se esperaba '(' después de 'if'.", pos);

        var condition = ParseExpression(tokens, ref pos);
        ifNode.Children.Add(condition);

        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_CLOSE)
            pos++;
        else
            return ErrorNode("Se esperaba ')' después de la condición del if.", pos);

        Node thenBranch;
        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
        {
            pos++; // Consumir 'ying'
            thenBranch = ParseBlock(tokens, ref pos);
        }
        else
        {
            var singleStatement = ParseStatement(tokens, ref pos);
            if (singleStatement != null)
            {
                thenBranch = new Node("Block");
                thenBranch.Children.Add(singleStatement);
            }
            else
            {
                return ErrorNode("Se esperaba un bloque '{...}' o una sentencia después de la condición del if.", pos);
            }
        }
        ifNode.Children.Add(thenBranch);

        // Manejo del 'else'
        if (pos < tokens.Count && Match(tokens, pos, "RESERVED", ReservedWords.ELSE))
        {
            pos++; // Consumir 'else'
            Node elseBranch;

            if (pos < tokens.Count && Match(tokens, pos, "RESERVED", ReservedWords.IF))
            {
                elseBranch = ParseIf(tokens, ref pos);
            }
            else if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
            {
                pos++; // Consumir 'ying'
                elseBranch = ParseBlock(tokens, ref pos);
            }
            else
            {
                var singleStatement = ParseStatement(tokens, ref pos);
                if (singleStatement != null)
                {
                    elseBranch = new Node("Block");
                    elseBranch.Children.Add(singleStatement);
                }
                else
                {
                    return ErrorNode("Se esperaba un bloque '{...}', 'if' o una sentencia después de 'else'.", pos);
                }
            }
            ifNode.Children.Add(elseBranch);
        }
        return ifNode;
    }

    private Node ParseFor(List<Token> tokens, ref int pos)
    {
        pos++; // Consumir 'for'
        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_OPEN)
            pos++;
        else
            return ErrorNode("Se esperaba '(' después de 'for'.", pos);

        // La inicialización puede ser una sentencia que ya consuma el ';'.
        Node? init = ParseStatement(tokens, ref pos); // La inicialización es una sentencia
        if (init == null)
        {
            init = new Node("Empty");
        }

        // Consumir ';' si está presente. Si no está, comprobar si el token anterior fue ';' (ParseStatement lo consumió).
        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON)
        {
            pos++; // Consumir el ';' que separa las partes del for
        }
        else
        {
            // Si el token anterior fue ';', asumimos que ParseStatement ya lo consumió
            if (!(pos > 0 && tokens[pos - 1].Type == "DELIMITER" && tokens[pos - 1].Value == DelimiterWords.SEMICOLON))
            {
                return ErrorNode("Se esperaba ';' después de la inicialización del for.", pos);
            }
        }
        var condition = ParseExpression(tokens, ref pos);
        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON)
            pos++;
        else
            return ErrorNode("Se esperaba ';' después de la condición del for.", pos);
        var increment = ParseExpression(tokens, ref pos);
        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_CLOSE)
            pos++;
        else
            return ErrorNode("Se esperaba ')' después de la expresión de incremento del for.", pos);

        Node body;
        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
        {
            pos++; // Consumir 'ying'
            body = ParseBlock(tokens, ref pos);
        }
        else
        {
            var statementBody = ParseStatement(tokens, ref pos);
            if (statementBody == null)
            {
                return ErrorNode("Se esperaba un cuerpo de bucle (un bloque o una sentencia) después de la cabecera del for.", pos);
            }
            body = new Node("Block", new List<Node> { statementBody });
        }

        return new Node("For", new List<Node> { init, condition, increment, body });
    }

    private Node ParseWhile(List<Token> tokens, ref int pos)
    {
        pos++; // Consumir 'while'
        if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_OPEN)
            pos++;
        else
            return ErrorNode("Se esperaba '(' después de 'while'.", pos);
        var condition = ParseExpression(tokens, ref pos);
        if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_CLOSE)
            pos++;
        else
            return ErrorNode("Se esperaba ')' después de la condición del while.", pos);

        Node body;
        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
        {
            pos++; // Consumir 'ying'
            body = ParseBlock(tokens, ref pos);
        }
        else
        {
            body = ParseStatement(tokens, ref pos) ?? new Node("Block");
        }

        return new Node("While", new List<Node> { condition, body });
    }

    private Node ParseDoWhile(List<Token> tokens, ref int pos)
    {
        pos++; // Consumir 'do'
        Node body;
        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
        {
            pos++; // Consumir 'ying'
            body = ParseBlock(tokens, ref pos);
        }
        else
        {
            body = ParseStatement(tokens, ref pos) ?? new Node("Block");
        }

        if (Match(tokens, pos, "RESERVED", ReservedWords.WHILE))
        {
            pos++; // Consumir 'while'
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_OPEN)
                pos++;
            else
                return ErrorNode("Se esperaba '(' después de 'while' en un do-while.", pos);
            var condition = ParseExpression(tokens, ref pos);
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_CLOSE)
                pos++;
            else
                return ErrorNode("Se esperaba ')' después de la condición del do-while.", pos);
            if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.SEMICOLON)
                pos++;
            else
                return ErrorNode("Se esperaba ';' después del do-while.", pos);
            return new Node("DoWhile", new List<Node> { body, condition });
        }
        return ErrorNode("Se esperaba 'while' después del bloque de un do-while.", pos);
    }

    private Node ParseFunction(List<Token> tokens, ref int pos)
    {
        var typeNode = new Node(tokens[pos].Value);
        pos++;
        var nameNode = new Node("Identifier", new List<Node> { new Node(tokens[pos].Value) });
        pos++;

        if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_OPEN)
            pos++;
        else
            return ErrorNode("Se esperaba '(' después del nombre de la función.", pos);
        var parameters = ParseParams(tokens, ref pos);
        if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.PAREN_CLOSE)
            pos++;
        else
            return ErrorNode("Se esperaba ')' después de los parámetros de la función.", pos);

        Node body;
        if (pos < tokens.Count && tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_START)
        {
            pos++; // Consumir 'ying'
            body = ParseBlock(tokens, ref pos);
        }
        else
        {
            return ErrorNode("Se esperaba un bloque '{...}' para el cuerpo de la función.", pos);
        }

        return new Node("Function", new List<Node> { typeNode, nameNode, parameters, body });
    }

    private Node ParseBlock(List<Token> tokens, ref int pos)
    {
        var blockNode = new Node("Block");

        // Asume que el 'ying' ya fue consumido
        while (pos < tokens.Count && !(tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_END))
        {
            var stmt = ParseStatement(tokens, ref pos);
            if (stmt != null)
                blockNode.Children.Add(stmt);
            else
            {
                blockNode.Children.Add(ErrorNode($"Token inesperado '{tokens[pos].Value}' dentro de un bloque.", pos));
                pos++;
            }
        }

        // If we've reached the end of the token stream, allow implicit block end at EOF
        // (do not add an error node) so snippet-style inputs that omit the closing
        // 'yang' can still be parsed and subsequent top-level declarations are reachable.
        if (pos >= tokens.Count)
        {
            return blockNode;
        }

        // If the next token is the explicit BLOCK_END, consume it
        if (tokens[pos].Type == "DELIMITER" && tokens[pos].Value == DelimiterWords.BLOCK_END)
        {
            pos++; // Consumir 'yang'
        }
        else
        {
            // If the next token looks like the start of a top-level declaration (TYPE) or a function
            // declaration, treat it as an implicit end of the block. This makes the parser forgiving
            // for snippets used in tests that omit the 'yang' closing token.
            if (tokens[pos].Type == "TYPE" || IsFunctionDeclaration(tokens, pos))
            {
                // do not consume token; caller will handle it
            }
            else
            {
                blockNode.Children.Add(ErrorNode($"Bloque no cerrado. Se esperaba '{DelimiterWords.BLOCK_END}'.", pos));
                return blockNode;
            }
        }

        return blockNode;
    }
}
