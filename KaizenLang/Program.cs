using System;
using System.IO;
using System.Collections.Generic;

namespace ParadigmasLang
{
    class Program
    {
        static void Main(string[] args)
        {
            string source = File.ReadAllText("Examples/example.txt");
            var lexer = new Lexer();
            var tokens = lexer.Tokenize(source);

            // Mostrar tokens generados
            Console.WriteLine("TOKENS:");
            foreach (var token in tokens)
            {
                Console.WriteLine($"{token.Type}: {token.Value}");
            }

            var parser = new Parser();
            var root = parser.Parse(tokens);

            // Mostrar árbol de sintaxis
            Console.WriteLine("\nÁRBOL DE SINTAXIS:");
            PrintNode(root, 0);

            // Ejecutar el intérprete (opcional)
            // var interpreter = new Interpreter();
            // interpreter.Execute(root);
        }

        static void PrintNode(Node node, int indent)
        {
            Console.WriteLine(new string(' ', indent * 2) + node.Type);
            foreach (var child in node.Children)
            {
                PrintNode(child, indent + 1);
            }
        }
    }
}
