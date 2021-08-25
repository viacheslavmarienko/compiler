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

    public class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken syntaxToken)
        {
            SyntaxToken = syntaxToken;
        }

        public override SyntaxTokenKind Kind => SyntaxTokenKind.Number;

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
    }
}
