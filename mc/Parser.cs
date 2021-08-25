using System;
using System.Collections.Generic;

namespace mc
{
    class Parser
    {
        private readonly SyntaxToken[] tokens;
        private int currentPosition;
        private List<string> diagnostics = new List<string>();
        public IEnumerable<string> Diagnostics => diagnostics;
        public Parser(string text)
        {
            var lexer = new Lexer(text);
            var tokensList = new List<SyntaxToken>();

            SyntaxToken token;

            do
            {
                token = lexer.GetNextToken();

                if (token.Kind != SyntaxTokenKind.Whitespace && token.Kind != SyntaxTokenKind.BadToken)
                {
                    tokensList.Add(token);
                }

            } while (token.Kind != SyntaxTokenKind.EndOfFile);

            tokens = tokensList.ToArray();
            diagnostics.AddRange(lexer.Diagnostics);
        }

        public SyntaxToken GetCurrentSyntaxTokenAndAdvancePosition()
        {
            var current = Current;
            currentPosition++;

            return current;
        }

        public SyntaxToken Match(SyntaxTokenKind kind)
        {
            if (Current.Kind == kind)
            {
                return GetCurrentSyntaxTokenAndAdvancePosition();
            }

            diagnostics.Add($"Error: unexpected token: '<{Current.Kind}>', expected '<{kind}>'");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken Peek(int offset)
        {
            var index = currentPosition + offset;

            if (index >= tokens.Length)
            {
                return tokens[tokens.Length - 1];
            }

            return tokens[index];
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFilteToken = Match(SyntaxTokenKind.EndOfFile);

            return new SyntaxTree(diagnostics, expression, endOfFilteToken);
        }

        public ExpressionSyntax ParseExpression()
        {
            var left = ParsePrimaryExpression();

            while (Current.Kind == SyntaxTokenKind.Plus || Current.Kind == SyntaxTokenKind.Minus)
            {
                var operationToken = GetCurrentSyntaxTokenAndAdvancePosition();
                var right = ParsePrimaryExpression();

                left = new BinaryExpressionSyntax(left, operationToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            var numberToken = Match(SyntaxTokenKind.Number);

            return new NumberExpressionSyntax(numberToken);
        }
    }
}
