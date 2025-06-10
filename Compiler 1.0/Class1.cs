using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

class Class1
{
    public enum TokenType
    {
        Identifier,
        Implication,   // >
        Or,            // +
        And,           // *
        Not,           // !
        LParen,        // (
        RParen,        // )
        EOF,
        Unknown
    }
    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public int Position { get; }

        public Token(TokenType type, string value, int position)
        {
            Type = type;
            Value = value;
            Position = position;
        }

        public override string ToString()
        {
            return $"[{Type}] \"{Value}\" (pos {Position})";
        }
    }
    public class Lexer
    {
        private string input;
        private int position;
        public List<Token> Tokens { get; } = new List<Token>();

        public Lexer(string input)
        {
            this.input = input.Replace(" ", "");
            this.position = 0;
        }

        public void Tokenize()
        {
            while (position < input.Length)
            {
                char ch = input[position];
                int startPos = position;

                if (char.IsLetter(ch))
                {
                    if (ch >= 'А' && ch <= 'я') // Проверка на русские символы
                    {
                        throw new Exception($"Ошибка: недопустимый символ '{ch}' (русская буква) на позиции {startPos}");
                    }

                    string identifier = ReadWhile(char.IsLetterOrDigit);
                    Tokens.Add(new Token(TokenType.Identifier, identifier, startPos));
                }
                else
                {
                    switch (ch)
                    {
                        case '>': Tokens.Add(new Token(TokenType.Implication, ">", position++)); break;
                        case '+': Tokens.Add(new Token(TokenType.Or, "+", position++)); break;
                        case '*': Tokens.Add(new Token(TokenType.And, "*", position++)); break;
                        case '!': Tokens.Add(new Token(TokenType.Not, "!", position++)); break;
                        case '(': Tokens.Add(new Token(TokenType.LParen, "(", position++)); break;
                        case ')': Tokens.Add(new Token(TokenType.RParen, ")", position++)); break;
                        default:
                            throw new Exception($"Ошибка: недопустимый символ '{ch}' на позиции {startPos}");
                    }
                }
            }

            Tokens.Add(new Token(TokenType.EOF, "", position));
        }


        private string ReadWhile(Func<char, bool> condition)
        {
            int start = position;
            while (position < input.Length && condition(input[position]))
                position++;
            return input.Substring(start, position - start);
        }
    }

    public class Parser
    {
        private List<Token> tokens;
        private int position;
        private List<string> trace = new List<string>();  
        private string lastNonTerminal = "";

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            this.position = 0;
        }

        private Token Current => position < tokens.Count ? tokens[position] : null;
        private void Advance() => position++;

        public string Parse()
        {
            try
            {
                E();
                if (Current.Type != TokenType.EOF)
                    return Error($"Неожиданная лексема \"{Current.Value}\"", Current.Position);

                return string.Join("\n", trace) + $"\n\nАнализ завершён. Итоговый нетерминал: {lastNonTerminal}";
            }
            catch (Exception ex)
            {
                return string.Join("\n", trace) + $"\n\n{ex.Message}";
            }
        }

        private string Error(string message, int pos)
        {
            throw new Exception($"Ошибка: {message} на позиции {pos}");
        }

        private void E()
        {
            trace.Add("E");
            lastNonTerminal = "E";
            E1();
            while (Current.Type == TokenType.Implication)
            {
                Advance();
                E1();
            }
        }

        private void E1()
        {
            trace.Add("E1");
            lastNonTerminal = "E1";
            E2();
            while (Current.Type == TokenType.Or)
            {
                Advance();
                E2();
            }
        }

        private void E2()
        {
            trace.Add("E2");
            lastNonTerminal = "E2";
            E3();
            while (Current.Type == TokenType.And)
            {
                Advance();
                E3();
            }
        }

        private void E3()
        {
            trace.Add("E3");
            lastNonTerminal = "E3";
            if (Current.Type == TokenType.Not)
            {
                Advance();
                E4();
            }
            else
            {
                E4();
            }
        }

        private void E4()
        {
            trace.Add("E4");
            lastNonTerminal = "E4";
            if (Current.Type == TokenType.LParen)
            {
                Advance();
                E();
                if (Current.Type == TokenType.RParen)
                {
                    Advance();
                }
                else
                {
                    throw new Exception(Error("Ожидалась \")\"", Current.Position));
                }
            }
            else if (Current.Type == TokenType.Identifier)
            {
                Advance();
            }
            else
            {
                throw new Exception(Error("Ожидался идентификатор или \"(\"", Current.Position));
            }
        }
    }
}
