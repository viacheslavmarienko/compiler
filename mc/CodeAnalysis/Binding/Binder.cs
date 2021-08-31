using System;

namespace mc.CodeAnlysis.Binding
{
    internal sealed class Binder
    {
        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxTokenKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxTokenKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxTokenKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
            }
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.SyntaxToken.Value as int? ?? 0;

            return new BoundLiteralExpression(value);
        }
        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperatorKind = BindUnaryOperatorKind(syntax.OperationToken.Kind, boundOperand.Type);

            return new BoundUnaryExpression(boundOperatorKind, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperatorKind = BindBinaryOperatorKind(syntax.OperationToken.Kind, boundLeft.Type, boundRight.Type);

            return new BoundBinaryExpression(boundLeft, boundOperatorKind, boundRight);
        }

        private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxTokenKind kind, Type operandType)
        {
            if(operandType != typeof(int))
            {
                return null;
            }

            switch (kind)
            {
                case SyntaxTokenKind.Plus:
                    return BoundUnaryOperatorKind.Identity;
                case SyntaxTokenKind.Minus:
                    return BoundUnaryOperatorKind.Negation;
                default:
                    throw new Exception($"Unexpected unary operator {kind}");
            }
        }

        private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxTokenKind kind, Type leftType, Type rightType)
        {
            if(leftType != typeof(int) || rightType != typeof(int))
            {
                return null;
            }

            switch (kind)
            {
                case SyntaxTokenKind.Plus:
                    return BoundBinaryOperatorKind.Addition;
                case SyntaxTokenKind.Minus:
                    return BoundBinaryOperatorKind.Subtraction;
                case SyntaxTokenKind.Star:
                    return BoundBinaryOperatorKind.Multiplication;
                case SyntaxTokenKind.BackSlash:
                    return BoundBinaryOperatorKind.Division;
                default:
                    throw new Exception($"Unexpected binary operator {kind}");
            }
        }
    }

    abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }
    }

    enum BoundNodeKind
    {
        UnaryExpression,
        LiteralExpression,
        BinaryExpression
    }

    abstract class BoundExpression : BoundNode
    {
        public abstract Type Type { get; }
    }

    enum BoundUnaryOperatorKind
    {
        Identity,
        Negation
    }

    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;
        }
        public override Type Type => Value.GetType();

        public override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;

        public object Value { get; }
    }

    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperatorKind operatorKind, BoundExpression operand)
        {
            OperatorKind = operatorKind;
            Operand = operand;
        }

        public override Type Type => Operand.Type;
        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
        public BoundUnaryOperatorKind OperatorKind { get; }
        public BoundExpression Operand { get; }
    }

    internal enum BoundBinaryOperatorKind
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperatorKind operatorKind, BoundExpression right)
        {
            Right = right;
            Left = left;
            OperatorKind = operatorKind;
        }

        public BoundExpression Left { get; }
        public BoundBinaryOperatorKind OperatorKind { get; }
        public BoundExpression Right { get; }

        public override Type Type => Left.Type;

        public override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;
    }
}