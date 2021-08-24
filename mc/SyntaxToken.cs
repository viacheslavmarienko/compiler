namespace mc
{
    public class SyntaxToken
    {
        public SyntaxToken(SyntaxTokenKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public SyntaxTokenKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }
    }
}