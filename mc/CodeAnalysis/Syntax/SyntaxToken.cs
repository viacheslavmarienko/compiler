using System.Collections.Generic;
using System.Linq;

namespace mc
{
    public class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxTokenKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxTokenKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}