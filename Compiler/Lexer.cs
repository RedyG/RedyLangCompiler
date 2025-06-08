using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public enum TokenType
    {
        Unknown,

        // Keywords
        Pub,
        Type,
        Alias,
        Struct,
        Trait,
        Impl,
        For,
        While,
        Use,
        Mod,
        Fn,
        If,
        Else,
        Return,
        Var,

        // Characters and strings
        LParen,
        RParen,
        LCurly,
        RCurly,
        LSquare,
        RSquare,
        RArrow,
        Semicolon,
        Colon,
        Comma,
        Dot,
        DoubleColon,

        // Operators
        Mul,
        Div,
        Add,
        Sub,
        Lt,
        Le,
        Gt,
        Ge,
        Assign,
        Amp,


        Identifier,
        IntLiteral,
        StringLiteral
    }

    public static class TokenTypeExtensions
    {
        public static bool IsModuleItem(this TokenType type) => type
            is TokenType.Fn or TokenType.Type or TokenType.Alias or TokenType.Use
            or TokenType.Mod or TokenType.Struct or TokenType.Trait or TokenType.Impl;

        public static string ToFormat(this TokenType type) => type switch
        {
            TokenType.Unknown => "Unknown",
            TokenType.Pub => "pub",
            TokenType.Type => "type",
            TokenType.Alias => "alias",
            TokenType.Struct => "struct",
            TokenType.Trait => "trait",
            TokenType.Impl => "impl",
            TokenType.For => "for",
            TokenType.While => "while",
            TokenType.Use => "use",
            TokenType.Mod => "mod",
            TokenType.Fn => "fn",
            TokenType.If => "if",
            TokenType.Else => "else",
            TokenType.Return => "return",
            TokenType.Var => "var",
            TokenType.LParen => "(",
            TokenType.RParen => ")",
            TokenType.LCurly => "{",
            TokenType.RCurly => "}",
            TokenType.LSquare => "[",
            TokenType.RSquare => "]",
            TokenType.RArrow => "->",
            TokenType.Semicolon => ";",
            TokenType.Colon => ":",
            TokenType.Comma => ",",
            TokenType.Dot => ".",
            TokenType.DoubleColon => "::",
            TokenType.Mul => "*",
            TokenType.Div => "/",
            TokenType.Add => "+",
            TokenType.Sub => "-",
            TokenType.Lt => "<",
            TokenType.Le => "<=",
            TokenType.Gt => ">",
            TokenType.Ge => ">=",
            TokenType.Assign => "=",
            TokenType.Amp => "&",
            TokenType.Identifier => "Identifier",
            TokenType.IntLiteral => "IntLiteral",
            TokenType.StringLiteral => "\"\"",
        };
    }

    public struct TextPos : IEquatable<TextPos>
    {
        public int Pos;
        public int Line;
        public int Col;

        public TextPos AddLength(int length) => new TextPos(Pos + length, Line, Col + length);

        public TextPos(int pos, int line, int col)
        {
            Pos = pos;
            Line = line;
            Col = col;
        }

        public override bool Equals(object obj)
        {
            if (obj is TextPos other)
                Equals(other);

            return false;
        }

        public bool Equals(TextPos other) => Pos == other.Pos && Line == other.Line && Col == other.Col;



        public TextPos() : this(0, 1, 1)
        {
        }

        public static bool operator ==(TextPos left, TextPos right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TextPos left, TextPos right)
        {
            return !(left == right);
        }

        public override int GetHashCode() => HashCode.Combine(Pos, Line, Col);
    }

    public struct Token
    {
        public StringSegment Content;
        public TokenType Type;
        public TextRange Range;



        public Token(StringSegment content, TokenType type, TextRange range)
        {
            Content = content;
            Type = type;
            Range = range;
        }
        public Token(StringSegment content, TokenType type, TextPos start, TextPos end) : this(content, type, new TextRange(start, end))
        {
        }

        public Token(StringSegment content, TokenType type, TextPos start) : this(content, type, start, start.AddLength(content.Length))
        {
        }

        public Token() : this(new StringSegment(), TokenType.Unknown, new TextRange())
        {
        }
    }

    public class Lexer
    {
        private Token _token;
        public Token Token => _token;

        public string FileName { get; }
        public string Input { get; }

        public Lexer(string fileName) : this(File.ReadAllText(fileName), fileName)
        {
        }

        public Lexer(string input, string fileName)
        {
            _token = new Token();
            Input = input;
            FileName = fileName;
        }

        private static bool StartsWithKeyword(StringSegment input, string keyword)
            => input.StartsWith(keyword, StringComparison.CurrentCulture) && (char.IsWhiteSpace(input[keyword.Length]) || input[keyword.Length] == ';');

        public Token Consume()
        {
            if (_token.Range.End.Pos >= Input.Length)
                return _token = new Token();

            while (char.IsWhiteSpace(Input[_token.Range.End.Pos]))
            {
                if (_token.Range.End.Pos == Input.Length - 1)
                {
                    _token.Range.End.Pos += 1;
                    return _token = new Token();
                }

                if (Input[_token.Range.End.Pos] == '\n')
                {
                    _token.Range.End.Line += 1;
                    _token.Range.End.Col = 1;
                }
                else
                    _token.Range.End.Col += 1;

                _token.Range.End.Pos += 1;
            }

            var trimmedInput = new StringSegment(Input).Subsegment(_token.Range.End.Pos);

            if (trimmedInput[0] == '"')
            {
                var end = trimmedInput.Subsegment(1).IndexOf('"');
                 return _token = new Token(trimmedInput.Subsegment(1, end), TokenType.StringLiteral, _token.Range.End.AddLength(1), _token.Range.End.AddLength(end + 2));
            }

            if (trimmedInput.StartsWith("->", StringComparison.CurrentCulture))
                return _token = new Token(trimmedInput.Subsegment(0, 2), TokenType.RArrow, _token.Range.End);
            if (trimmedInput.StartsWith("::", StringComparison.CurrentCulture))
                return _token = new Token(trimmedInput.Subsegment(0, 2), TokenType.DoubleColon, _token.Range.End);

            switch (trimmedInput[0])
            {
                case '(':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.LParen, _token.Range.End);
                case ')':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.RParen, _token.Range.End);
                case '{':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.LCurly, _token.Range.End);
                case '}':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.RCurly, _token.Range.End);
                case '[':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.LSquare, _token.Range.End);
                case ']':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.RSquare, _token.Range.End);
                case ';':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Semicolon, _token.Range.End);
                case ':':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Colon, _token.Range.End);
                case ',':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Comma, _token.Range.End);
                case '.':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Dot, _token.Range.End);
                case '*':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Mul, _token.Range.End);
                case '/':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Div, _token.Range.End);
                case '+':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Add, _token.Range.End);
                case '-':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Sub, _token.Range.End);
                case '<':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Lt, _token.Range.End);
                case '>':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Gt, _token.Range.End);
                case '=':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Assign, _token.Range.End);
                case '&':
                    return _token = new Token(trimmedInput.Subsegment(0, 1), TokenType.Amp, _token.Range.End);
            }

            if (StartsWithKeyword(trimmedInput, "fn"))
                return _token = new Token(trimmedInput.Subsegment(0, 2), TokenType.Fn, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "pub"))
                return _token = new Token(trimmedInput.Subsegment(0, 3), TokenType.Pub, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "type"))
                return _token = new Token(trimmedInput.Subsegment(0, 4), TokenType.Type, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "alias"))
                return _token = new Token(trimmedInput.Subsegment(0, 5), TokenType.Alias, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "struct"))
                return _token = new Token(trimmedInput.Subsegment(0, 6), TokenType.Struct, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "trait"))
                return _token = new Token(trimmedInput.Subsegment(0, 5), TokenType.Trait, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "impl"))
                return _token = new Token(trimmedInput.Subsegment(0, 4), TokenType.Impl, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "for"))
                return _token = new Token(trimmedInput.Subsegment(0, 3), TokenType.For, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "while"))
                return _token = new Token(trimmedInput.Subsegment(0, 5), TokenType.While, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "use"))
                return _token = new Token(trimmedInput.Subsegment(0, 3), TokenType.Use, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "mod"))
                return _token = new Token(trimmedInput.Subsegment(0, 3), TokenType.Mod, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "if"))
                return _token = new Token(trimmedInput.Subsegment(0, 2), TokenType.If, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "else"))
                return _token = new Token(trimmedInput.Subsegment(0, 4), TokenType.Else, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "return"))
                return _token = new Token(trimmedInput.Subsegment(0, 6), TokenType.Return, _token.Range.End);
            if (StartsWithKeyword(trimmedInput, "var"))
                return _token = new Token(trimmedInput.Subsegment(0, 3), TokenType.Var, _token.Range.End);
            if (trimmedInput.StartsWith("<=", StringComparison.CurrentCulture))
                return _token = new Token(trimmedInput.Subsegment(0, 2), TokenType.Le, _token.Range.End);
            if (trimmedInput.StartsWith(">=", StringComparison.CurrentCulture))
                return _token = new Token(trimmedInput.Subsegment(0, 2), TokenType.Ge, _token.Range.End);


            // Identifier
            if (char.IsLetter(trimmedInput[0]) || trimmedInput[0] == '_')
            {
                var i = 1;
                while (i < trimmedInput.Length && (char.IsLetterOrDigit(trimmedInput[i]) || trimmedInput[i] == '_'))
                    i += 1;

                return _token = new Token(trimmedInput.Subsegment(0, i), TokenType.Identifier, _token.Range.End);
            }

            // IntLiteral
            if (char.IsAsciiDigit(trimmedInput[0]))
            {
                var i = 1;
                while (i < trimmedInput.Length && (char.IsAsciiDigit(trimmedInput[i]) || trimmedInput[i] == '_'))
                    i += 1;

                return _token = new Token(trimmedInput.Subsegment(0, i), TokenType.IntLiteral, _token.Range.End);
            }

            return _token = new Token();
        }
    }
}