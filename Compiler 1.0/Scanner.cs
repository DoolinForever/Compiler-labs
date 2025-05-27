using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace Compiler_1._0
{
    enum TokenType
    {
        Fn, Return,
        Identifier, TypeName, Number,
        Comma, Colon, Semicolon,
        Arrow, LParen, RParen, LBrace, RBrace,
        Plus, Minus, Star, Slash,
        EndOfFile
    }

    class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public int Line { get; }
        public int Column { get; }

        public Token(TokenType type, string lexeme, int line, int column)
        {
            Type = type;
            Lexeme = lexeme;
            Line = line;
            Column = column;
        }
    }

    class LexicalException : Exception
    {
        public int Line { get; }
        public int Column { get; }
        public string Lexeme { get; }

        public LexicalException(string message, int line, int column, string lexeme)
            : base(message)
        {
            Line = line;
            Column = column;
            Lexeme = lexeme;
        }
    }

    // Новый класс для сбора информации об ошибках синтаксиса
    public class SyntaxErrorInfo
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string Expected { get; set; }
        public string Found { get; set; }
    }

    class Lexer
    {
        private readonly string _text;
        private int _pos = 0, _line = 1, _column = 1;
        private static readonly HashSet<string> ReservedTypes = new HashSet<string>
        {
            "i8", "i16", "i32", "i64", "i128",
            "f32", "f64", "bool", "char"
        };

        public Lexer(string text) => _text = text;

        private char Current => _pos < _text.Length ? _text[_pos] : '\0';

        private void Advance()
        {
            if (Current == '\n') { _line++; _column = 1; }
            else { _column++; }
            _pos++;
        }

        public Token NextToken()
        {
            while (char.IsWhiteSpace(Current)) Advance();
            int startLine = _line, startCol = _column;
            if (Current == '\0') return new Token(TokenType.EndOfFile, string.Empty, startLine, startCol);
            if (Current == '&')
            {
                int remaining = _text.Length - _pos;
                if (remaining >= 4 && _text.Substring(_pos, 4) == "&str")
                {
                    for (int i = 0; i < 4; i++) Advance();
                    return new Token(TokenType.TypeName, "&str", startLine, startCol);
                }
                throw new LexicalException("Unexpected character '&'", startLine, startCol, "&");
            }
            switch (Current)
            {
                case '(':
                    Advance(); return new Token(TokenType.LParen, "(", startLine, startCol);
                case ')': Advance(); return new Token(TokenType.RParen, ")", startLine, startCol);
                case '{': Advance(); return new Token(TokenType.LBrace, "{", startLine, startCol);
                case '}': Advance(); return new Token(TokenType.RBrace, "}", startLine, startCol);
                case ',': Advance(); return new Token(TokenType.Comma, ",", startLine, startCol);
                case ':': Advance(); return new Token(TokenType.Colon, ":", startLine, startCol);
                case ';': Advance(); return new Token(TokenType.Semicolon, ";", startLine, startCol);
                case '+': Advance(); return new Token(TokenType.Plus, "+", startLine, startCol);
                case '-':
                    Advance();
                    if (Current == '>') { Advance(); return new Token(TokenType.Arrow, "->", startLine, startCol); }
                    return new Token(TokenType.Minus, "-", startLine, startCol);
                case '*': Advance(); return new Token(TokenType.Star, "*", startLine, startCol);
                case '/': Advance(); return new Token(TokenType.Slash, "/", startLine, startCol);
            }
            if (char.IsLetter(Current) || Current == '_')
            {
                var sb = new StringBuilder();
                while (char.IsLetterOrDigit(Current) || Current == '_')
                {
                    if (Current > 127) throw new LexicalException("Forbidden character (non-ASCII)", _line, _column, Current.ToString());
                    sb.Append(Current);
                    Advance();
                }
                var lex = sb.ToString();
                if (lex == "fn") return new Token(TokenType.Fn, lex, startLine, startCol);
                if (lex == "return") return new Token(TokenType.Return, lex, startLine, startCol);
                if (ReservedTypes.Contains(lex)) return new Token(TokenType.TypeName, lex, startLine, startCol);
                return new Token(TokenType.Identifier, lex, startLine, startCol);
            }
            if (char.IsDigit(Current))
            {
                var sb = new StringBuilder();
                while (char.IsDigit(Current)) { sb.Append(Current); Advance(); }
                return new Token(TokenType.Number, sb.ToString(), startLine, startCol);
            }
            char bad = Current; Advance();
            throw new LexicalException($"Unexpected character '{bad}'", startLine, startCol, bad.ToString());
        }
    }

    class Parser
    {
        private readonly List<Token> _tokens;
        private int _index = 0;
        public List<SyntaxErrorInfo> Errors { get; } = new List<SyntaxErrorInfo>();

        public Parser(List<Token> tokens) => _tokens = tokens;

        // Старое: private Token Current => _index < _tokens.Count ? _tokens[_index] : _tokens[^1];
        // Новое:
        private Token Current
        {
            get
            {
                if (_index < _tokens.Count)
                    return _tokens[_index];
                // Если вышли за конец списка, возвращаем последний элемент
                return _tokens[_tokens.Count - 1];
            }
        }

        private void Eat(TokenType type)
        {
            if (Current.Type == type)
            {
                _index++;
                return;
            }
            // Регистрируем ошибку и пропускаем
            Errors.Add(new SyntaxErrorInfo
            {
                Line = Current.Line,
                Column = Current.Column,
                Expected = GetExpectedLexeme(type),
                Found = Current.Lexeme
            });
            _index++;
        }

        private string GetExpectedLexeme(TokenType type) => type switch
        {
            TokenType.Fn => "fn",
            TokenType.Return => "return",
            TokenType.LParen => "(",
            TokenType.RParen => ")",
            TokenType.LBrace => "{",
            TokenType.RBrace => "}",
            TokenType.Arrow => "->",
            TokenType.Colon => ":",
            TokenType.Comma => ",",
            TokenType.Semicolon => ";",
            TokenType.Plus => "+",
            TokenType.Minus => "-",
            TokenType.Star => "*",
            TokenType.Slash => "/",
            TokenType.TypeName => "<type>",
            TokenType.Identifier => "<identifier>",
            TokenType.Number => "<number>",
            TokenType.EndOfFile => "<EOF>",
            _ => type.ToString(),
        };

        public void ParseFunction()
        {
            Eat(TokenType.Fn);
            Eat(TokenType.Identifier);
            Eat(TokenType.LParen);
            ParseParamList();
            Eat(TokenType.RParen);
            Eat(TokenType.Arrow);
            Eat(TokenType.TypeName);
            Eat(TokenType.LBrace);
            Eat(TokenType.Return);
            ParseExpression();
            Eat(TokenType.Semicolon);
            Eat(TokenType.RBrace);
            if (Current.Type == TokenType.Semicolon) Eat(TokenType.Semicolon);
            Eat(TokenType.EndOfFile);
        }

        private void ParseParamList()
        {
            if (Current.Type == TokenType.Identifier)
            {
                ParseParam();
                while (Current.Type == TokenType.Comma)
                {
                    Eat(TokenType.Comma);
                    ParseParam();
                }
            }
        }

        private void ParseParam()
        {
            Eat(TokenType.Identifier);
            Eat(TokenType.Colon);
            Eat(TokenType.TypeName);
        }

        private void ParseExpression()
        {
            ParseTerm();
            while (Current.Type == TokenType.Plus || Current.Type == TokenType.Minus)
            {
                Eat(Current.Type);
                ParseTerm();
            }
        }

        private void ParseTerm()
        {
            ParseFactor();
            while (Current.Type == TokenType.Star || Current.Type == TokenType.Slash)
            {
                Eat(Current.Type);
                ParseFactor();
            }
        }

        private void ParseFactor()
        {
            if (Current.Type == TokenType.Number || Current.Type == TokenType.Identifier)
            {
                Eat(Current.Type);
            }
            else if (Current.Type == TokenType.LParen)
            {
                Eat(TokenType.LParen);
                ParseExpression();
                Eat(TokenType.RParen);
            }
            else
            {
                // Регистрируем неожиданную лексему
                Errors.Add(new SyntaxErrorInfo
                {
                    Line = Current.Line,
                    Column = Current.Column,
                    Expected = "<identifier> или <number> или '('",
                    Found = Current.Lexeme
                });
                _index++;
            }
        }
    }
}



