using System.Collections.Generic;

namespace mc
{
    internal static class SyntaxFacts
    {
        public static int GetBinaryOperatorPrecedence(this SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                    return 1;
                case SyntaxTokenKind.BackSlash:
                case SyntaxTokenKind.Star:
                    return 2;
                default:
                    return 0;
            }
        }

        public static int GetUnaryOperatorPrecedence(this SyntaxTokenKind kind)
        {
            switch (kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                    return 3;
                default:
                    return 0;
            }
        }
    }

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
                token = lexer.Lex();

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

        private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var operatorTokenPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

            if (operatorTokenPrecedence != 0 && operatorTokenPrecedence >= parentPrecedence)
            {
                var operatorToken = GetCurrentSyntaxTokenAndAdvancePosition();
                var operand = ParseExpression(parentPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();

                if (precedence == 0 || precedence <= parentPrecedence)
                {
                    break;
                }

                var operatorToken = GetCurrentSyntaxTokenAndAdvancePosition();
                var right = ParseExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }


        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (Current.Kind == SyntaxTokenKind.OpenParenthesis)
            {
                var left = GetCurrentSyntaxTokenAndAdvancePosition();
                var expression = ParseExpression();
                var right = Match(SyntaxTokenKind.CloseParenthesis);

                return new ParenthesizedExpressionSyntax(left, expression, right);
            }
            var numberToken = Match(SyntaxTokenKind.LiteralExpression);

            return new LiteralExpressionSyntax(numberToken);
        }
    }
}
