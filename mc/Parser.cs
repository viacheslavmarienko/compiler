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
            var expression = ParseTerm();
            var endOfFilteToken = Match(SyntaxTokenKind.EndOfFile);

            return new SyntaxTree(diagnostics, expression, endOfFilteToken);
        }

        public ExpressionSyntax ParseFactor()
        {
            var left = ParsePrimaryExpression();

            while (Current.Kind == SyntaxTokenKind.Star
                || Current.Kind == SyntaxTokenKind.BackSlash)
            {
                var operationToken = GetCurrentSyntaxTokenAndAdvancePosition();
                var right = ParsePrimaryExpression();

                left = new BinaryExpressionSyntax(left, operationToken, right);
            }

            return left;
        }

        public ExpressionSyntax ParseTerm()
        {
            var left = ParseFactor();

            while (Current.Kind == SyntaxTokenKind.Plus 
                || Current.Kind == SyntaxTokenKind.Minus)
            {
                var operationToken = GetCurrentSyntaxTokenAndAdvancePosition();
                var right = ParseFactor();

                left = new BinaryExpressionSyntax(left, operationToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if(Current.Kind == SyntaxTokenKind.OpenParenthesis) 
            {
                var left = GetCurrentSyntaxTokenAndAdvancePosition();
                var expression = ParseTerm();
                var right = Match(SyntaxTokenKind.CloseParenthesis);

                return new ParenthesizedExpressionSyntax(left, expression, right);
            }
            var numberToken = Match(SyntaxTokenKind.Number);

            return new NumberExpressionSyntax(numberToken);
        }
    }

    class Evaluator
    {
        private readonly ExpressionSyntax root;

        public Evaluator(ExpressionSyntax root)
        {
            this.root = root;
        }
        public int Evaluate()
        {
            return EvaluateExpression(root); 
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            if(node is NumberExpressionSyntax n)
            {
                return (int) n.SyntaxToken.Value;
            }

            if(node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if(b.OperationToken.Kind == SyntaxTokenKind.Plus)
                {
                    return left + right;
                }
                if(b.OperationToken.Kind == SyntaxTokenKind.Minus)
                {
                    return left - right;
                }
                if(b.OperationToken.Kind == SyntaxTokenKind.Star)
                {
                    return left * right;
                }
                if(b.OperationToken.Kind == SyntaxTokenKind.BackSlash)
                {
                    return left / right;
                }
                throw new Exception($"Unexpected binary operator {b.OperationToken.Kind}");
            }

            if(node is ParenthesizedExpressionSyntax p) 
            {
                return EvaluateExpression(p.Expression);
            }

            throw new Exception($"Unexpected node: {node.Kind}");
        }
    }
}
