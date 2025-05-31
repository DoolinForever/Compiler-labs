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

    class SyntaxException : Exception
    {
        public int Line { get; }
        public int Column { get; }
        public string Lexeme { get; }

        public SyntaxException(string message, int line, int column, string lexeme)
            : base(message)
        {
            Line = line;
            Column = column;
            Lexeme = lexeme;
        }
    }

    class Lexer
    {
        private readonly string _text;
        private int _pos = 0, _line = 1, _column = 1;

        // Разрешённые типы переменных
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

        public (List<Token> Tokens, List<LexicalException> Errors) TokenizeAll()
        {
            var tokens = new List<Token>();
            var errors = new List<LexicalException>();

            while (true)
            {
                try
                {
                    Token token = NextToken();
                    tokens.Add(token);
                    if (token.Type == TokenType.EndOfFile)
                        break;
                }
                catch (LexicalException ex)
                {
                    errors.Add(ex);

                    Advance();
                }
            }

            return (tokens, errors);
        }



        public Token NextToken()
        {
            // Пропускаем пробелы
            while (char.IsWhiteSpace(Current))
                Advance();

            int startLine = _line, startCol = _column;

            // Конец файла
            if (Current == '\0')
                return new Token(TokenType.EndOfFile, string.Empty, startLine, startCol);

            // Специальная обработка типа &str
            if (Current == '&')
            {
                int remaining = _text.Length - _pos;
                if (remaining >= 4 && _text.Substring(_pos, 4) == "&str")
                {
                    // Съедаем '&str'
                    for (int i = 0; i < 4; i++)
                        Advance();
                    return new Token(TokenType.TypeName, "&str", startLine, startCol);
                }
                throw new LexicalException("Unexpected character '&'", startLine, startCol, "&");
            }

            // Однобуквенные символы и операторы
            switch (Current)
            {
                case '(': Advance(); return new Token(TokenType.LParen, "(", startLine, startCol);
                case ')': Advance(); return new Token(TokenType.RParen, ")", startLine, startCol);
                case '{': Advance(); return new Token(TokenType.LBrace, "{", startLine, startCol);
                case '}': Advance(); return new Token(TokenType.RBrace, "}", startLine, startCol);
                case ',': Advance(); return new Token(TokenType.Comma, ",", startLine, startCol);
                case ':': Advance(); return new Token(TokenType.Colon, ":", startLine, startCol);
                case ';': Advance(); return new Token(TokenType.Semicolon, ";", startLine, startCol);
                case '+': Advance(); return new Token(TokenType.Plus, "+", startLine, startCol);
                case '-':
                    Advance();
                    if (Current == '>')
                    {
                        Advance();
                        return new Token(TokenType.Arrow, "->", startLine, startCol);
                    }
                    return new Token(TokenType.Minus, "-", startLine, startCol);
                case '>':  
                    throw new LexicalException("Ожидался '->', найден одиночный '>'", _line, _column, ">");

                case '*': Advance(); return new Token(TokenType.Star, "*", startLine, startCol);
                case '/': Advance(); return new Token(TokenType.Slash, "/", startLine, startCol);
            }

            // Идентификаторы, ключевые слова и типы
            if (char.IsLetter(Current) || Current == '_')
            {
                var sb = new StringBuilder();
                while (char.IsLetterOrDigit(Current) || Current == '_')
                {
                    if (Current > 127) // non-ASCII = ошибка
                        throw new LexicalException("Forbidden character (non-ASCII)", _line, _column, Current.ToString());

                    sb.Append(Current);
                    Advance();
                }

                var lex = sb.ToString();

                if (lex == "fn")
                    return new Token(TokenType.Fn, lex, startLine, startCol);
                if (lex == "return")
                    return new Token(TokenType.Return, lex, startLine, startCol);
                if (ReservedTypes.Contains(lex))
                    return new Token(TokenType.TypeName, lex, startLine, startCol);

                return new Token(TokenType.Identifier, lex, startLine, startCol);
            }


            // Числа
            if (char.IsDigit(Current))
            {
                var sb = new StringBuilder();
                while (char.IsDigit(Current))
                {
                    sb.Append(Current);
                    Advance();
                }
                return new Token(TokenType.Number, sb.ToString(), startLine, startCol);
            }

            // Нераспознанный символ
            char bad = Current;
            Advance();
            throw new LexicalException($"Unexpected character '{bad}'", startLine, startCol, bad.ToString());
        }
    }

    class Parser
    {
        private readonly List<Token> _tokens;
        private int _index = 0;
        public int Position
        {
            get { return _index; }
        }
        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public List<SyntaxException> SyntaxErrors { get; } = new List<SyntaxException>();


        // Корректное определение свойства Current
        private Token Current
        {
            get
            {
                if (_index < _tokens.Count)
                    return _tokens[_index];
                
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

            var ex = new SyntaxException(
                $"Ожидался токен '{type}', найден '{Current.Lexeme}'",
                Current.Line, Current.Column, Current.Lexeme);
            SyntaxErrors.Add(ex);

            Recover(); // вместо _index++;
        }

        private void Recover()
        {
            // Пропускаем токены до конца инструкции или блока
            while (Current.Type != TokenType.Semicolon &&
                   Current.Type != TokenType.RBrace &&
                   Current.Type != TokenType.EndOfFile)
            {
                _index++;
            }

            if (Current.Type != TokenType.EndOfFile)
                _index++; // пропускаем и ; или }
        }


        public void ParseFunctionWithRecovery()
        {
            TryEat(TokenType.Fn);
            TryEat(TokenType.Identifier);
            TryEat(TokenType.LParen);

            // параметры
            if (Current.Type == TokenType.Identifier)
            {
                TryEat(TokenType.Identifier);
                TryEat(TokenType.Colon);
                TryEat(TokenType.TypeName);

                while (Current.Type == TokenType.Comma)
                {
                    _index++; // пропускаем запятую
                    TryEat(TokenType.Identifier);
                    TryEat(TokenType.Colon);
                    TryEat(TokenType.TypeName);
                }
            }

            TryEat(TokenType.RParen);
            TryEat(TokenType.Arrow);
            TryEat(TokenType.TypeName);
            TryEat(TokenType.LBrace);
            TryEat(TokenType.Return);

            ParseExpression();

            TryEat(TokenType.Semicolon);
            TryEat(TokenType.RBrace);
            TryEat(TokenType.EndOfFile);
        }



        private bool TryEat(TokenType type)
        {
            if (Current.Type == type)
            {
                _index++;
                return true;
            }

            // Если токен был вставлен заглушкой
            if (string.IsNullOrEmpty(Current.Lexeme))
            {
                var err = new SyntaxException(
                    $"Ожидался токен '{type}', но в позиции ошибка лексики (пустая лексема)",
                    Current.Line, Current.Column, "<пусто>");
                SyntaxErrors.Add(err);

                _index++; // всё равно двигаемся вперёд
                return false;
            }

            // Специальный случай: '-' вместо '->'
            if (type == TokenType.Arrow && Current.Type == TokenType.Minus)
            {
                SyntaxErrors.Add(new SyntaxException(
                    $"Ожидался '->', найден одиночный '-' (возможно, забыта >)",
                    Current.Line, Current.Column, Current.Lexeme));
                _index++;
                return false;
            }

            // Обычная синтаксическая ошибка
            SyntaxErrors.Add(new SyntaxException(
                $"Ожидался токен '{type}', найден '{Current.Lexeme}'",
                Current.Line, Current.Column, Current.Lexeme));
            _index++;
            return false;
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
                _index++;
            }
            else if (Current.Type == TokenType.LParen)
            {
                TryEat(TokenType.LParen);
                ParseExpression();
                TryEat(TokenType.RParen);
            }
            else
            {
                var ex = new SyntaxException(
                    $"Недопустимый токен '{Current.Lexeme}' в выражении",
                    Current.Line, Current.Column, Current.Lexeme);
                SyntaxErrors.Add(ex);
                _index++;
            }
        }


    }
}
