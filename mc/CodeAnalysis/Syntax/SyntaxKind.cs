namespace mc
{
    public enum SyntaxTokenKind
    {
        Whitespace,
        Plus,
        CloseParenthesis,
        OpenParenthesis,
        BackSlash,
        Star,
        Minus,
        BadToken,
        EndOfFile,
        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression,
        UnaryExpression
    }
}