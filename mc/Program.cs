using System;

namespace mc
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("!>");

                var line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }

                var lexer = new Lexer(line);

                while (true)
                {
                    var token = lexer.GetNextToken();

                    if (token.Kind == SyntaxTokenKind.EndOfFile)
                    {
                        break;
                    }

                    Console.WriteLine($"{token.Kind}: '{token.Text}'");

                    if (token.Value != null)
                    {
                        Console.WriteLine(token.Value);
                    }
                }
            }
        }
    }
}
