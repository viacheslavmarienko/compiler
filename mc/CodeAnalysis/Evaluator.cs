using System;

namespace mc
{
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
            if (node is LiteralExpressionSyntax n)
            {
                return (int)n.SyntaxToken.Value;
            }

            if (node is UnaryExpressionSyntax u)
            {
                var operand = EvaluateExpression(u.Operand);

                if (u.OperationToken.Kind == SyntaxTokenKind.Plus)
                {
                    return operand;
                }
                else if (u.OperationToken.Kind == SyntaxTokenKind.Minus)
                {
                    return -operand;
                }
                else
                {
                    throw new Exception($"Unexpected unary operator: {u.OperationToken.Kind}");
                }
            }

            if (node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if (b.OperationToken.Kind == SyntaxTokenKind.Plus)
                {
                    return left + right;
                }
                if (b.OperationToken.Kind == SyntaxTokenKind.Minus)
                {
                    return left - right;
                }
                if (b.OperationToken.Kind == SyntaxTokenKind.Star)
                {
                    return left * right;
                }
                if (b.OperationToken.Kind == SyntaxTokenKind.BackSlash)
                {
                    return left / right;
                }
                throw new Exception($"Unexpected binary operator {b.OperationToken.Kind}");
            }

            if (node is ParenthesizedExpressionSyntax p)
            {
                return EvaluateExpression(p.Expression);
            }

            throw new Exception($"Unexpected node: {node.Kind}");
        }
    }
}
