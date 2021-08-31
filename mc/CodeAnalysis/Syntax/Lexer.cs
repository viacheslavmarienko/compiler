using System;
using System.Collections.Generic;

namespace mc
{

    class Lexer
    {
        private readonly string text;
        private int position;
        private List<string> diagnostics = new List<string>();
        public IEnumerable<string> Diagnostics => diagnostics;

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

        public SyntaxToken Lex()
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
                
                if(!int.TryParse(tokenText, out var value)) {
                    diagnostics.Add($"Error: the number {text} cannot be represented by int32");
                }

                return new SyntaxToken(SyntaxTokenKind.LiteralExpression, start, tokenText, value);
            }

            if (char.IsWhiteSpace(GetCurrentChar()))
            {
                var start = position;

                do
                {
                    AdvancePosition();
                } while (char.IsWhiteSpace(GetCurrentChar()));

                var tokenLength = position - start;
                var tokenText = text.Substring(start, tokenLength);

                return new SyntaxToken(SyntaxTokenKind.Whitespace, start, tokenText, null);
            }

            var current = GetCurrentChar().ToString();

            switch (current)
            {
                case ("+"):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.Plus, position, current, null);
                case ("-"):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.Minus, position, current, null);
                case ("*"):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.Star, position, current, null);
                case ("/"):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.BackSlash, position, current, null);
                case ("("):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.OpenParenthesis, position, current, null);
                case (")"):
                    AdvancePosition();
                    return new SyntaxToken(SyntaxTokenKind.CloseParenthesis, position, current, null);
            }

            diagnostics.Add($"Error: bad token in input: '{current}'");

            var badTokenStart = position;
            AdvancePosition();
            return new SyntaxToken(SyntaxTokenKind.BadToken, position, text.Substring(badTokenStart, position), null);
        }
    }
}
