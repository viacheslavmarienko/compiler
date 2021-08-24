using System;

namespace mc
{
    class SyntaxNode
    {
        public SyntaxTokenKind Kind { get; private set; }
    }

    class Lexer
    {
        private readonly string text;
        private int position;

        public Lexer(string text)
        {
            this.text = text;
        }

        private char GetCurrentChar()
        {
            if (position >= text.Length)
            {
                return '\0';
            }

            return text[position];
        }

        private void AdvancePosition()
        {
            position++;
        }

        public SyntaxToken GetNextToken()
        {
            if (position >= text.Length)
            {
                return new SyntaxToken(SyntaxTokenKind.EndOfFile, position, "\0", null);
            }

            if (char.IsDigit(GetCurrentChar()))
            {
                var start = position;

                do
                {
                    AdvancePosition();
                } while (char.IsDigit(GetCurrentChar()));

                var tokenLength = position - start;
                var tokenText = text.Substring(start, tokenLength);
                int.TryParse(tokenText, out var value);

                return new SyntaxToken(SyntaxTokenKind.Number, start, tokenText, value);
            }

            if (char.IsWhiteSpace(GetCurrentChar()))
            {
                var start = position;

                do
                {
                    AdvancePosition();
                } while (char.IsDigit(GetCurrentChar()));

                var tokenLength = position - start;
                var tokenText = text.Substring(start, tokenLength);

                return new SyntaxToken(SyntaxTokenKind.Whitespace, start, tokenText, null);
            }

            switch (GetCurrentChar())
            {
                case ('+'):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.Plus, position, "+", null);
                case ('-'):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.Minus, position, "-", null);
                case ('*'):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.Start, position, "*", null);
                case ('/'):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.BackSlash, position, "/", null);
                case ('('):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.OpenParenthesis, position, "(", null);
                case (')'):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.CloseParenthesis, position, ")", null);
            }

            var badTokenStart = position;
            AdvancePosition();
            return new SyntaxToken(SyntaxTokenKind.BadToken, position, text.Substring(badTokenStart, position), null);
        }
    }
}
