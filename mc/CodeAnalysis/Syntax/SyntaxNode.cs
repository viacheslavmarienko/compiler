using System.Collections.Generic;

namespace mc
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxTokenKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    public abstract class ExpressionSyntax : SyntaxNode
    {

    }

    public class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken syntaxToken)
        {
            SyntaxToken = syntaxToken;
        }

        public override SyntaxTokenKind Kind => SyntaxTokenKind.LiteralExpression;

        public SyntaxToken SyntaxToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return SyntaxToken;
        }
    }

    sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operationToken, ExpressionSyntax right)
        {
            Left = left;
            OperationToken = operationToken;
            Right = right;
        }

        public ExpressionSyntax Left { get; }
        public SyntaxToken OperationToken { get; }
        public ExpressionSyntax Right { get; }

        public override SyntaxTokenKind Kind => SyntaxTokenKind.BinaryExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperationToken;
            yield return Right;
        }
    }

    sealed class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax(SyntaxToken operationToken, ExpressionSyntax operand)
        {
            OperationToken = operationToken;
            Operand = operand;
        }

        public SyntaxToken OperationToken { get; }
        public ExpressionSyntax Operand { get; }

        public override SyntaxTokenKind Kind => SyntaxTokenKind.UnaryExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperationToken;
            yield return Operand;
        }
    }

    sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(
            SyntaxToken openParenthesis,
            ExpressionSyntax expression,
            SyntaxToken closeParenthesis)
        {
            OpenParenthesis = openParenthesis;
            Expression = expression;
            CloseParenthesis = closeParenthesis;
        }

        public override SyntaxTokenKind Kind => SyntaxTokenKind.ParenthesizedExpression;

        public SyntaxToken OpenParenthesis { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesis { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesis;
            yield return Expression;
            yield return CloseParenthesis;
        }
    }

    sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFilteToken)
        {
            Diagnostics = diagnostics;
            Root = root;
            EndOfFilteToken = endOfFilteToken;
        }

        public IEnumerable<string> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFilteToken { get; }

        public static SyntaxTree Parse(string text) 
        {
            var parser = new Parser(text);
            
            return parser.Parse();
        }
    }
}
