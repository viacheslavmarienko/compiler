namespace mc
{
    public enum SyntaxTokenKind
    {
        Number,
        Whitespace,
        Plus,
        CloseParenthesis,
        OpenParenthesis,
        BackSlash,
        Star,
        Minus,
        BadToken,
        EndOfFile,
        BinaryExpression,
        ParenthesizedExpression
    }
}