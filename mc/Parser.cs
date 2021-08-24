using System.Collections.Generic;

namespace mc
{
    class Parser
    {
        public Parser(string text)
        {
            var lexer = new Lexer(text);
            var tokens = new List<SyntaxToken>();

            SyntaxToken token;

            do
            {
                token = lexer.GetNextToken();

                if (token.Kind != SyntaxTokenKind.Whitespace && token.Kind != SyntaxTokenKind.BadToken)
                {
                    tokens.Add(token);
                }

            } while (token.Kind != SyntaxTokenKind.EndOfFile);
        }
    }
}
