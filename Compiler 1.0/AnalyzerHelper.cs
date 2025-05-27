using System;
using System.Collections.Generic;
using System.Text;
using Compiler_1._0;


namespace Compiler_1._0
{
    public static class AnalyzerHelper
    {
        public static string AnalyzeCode(string source, out bool hasErrors)
        {
            var lexErrors = new List<LexicalException>();
            var tokens = new List<Token>();
            var lexer = new Lexer(source);
            Token tok = null;
            hasErrors = false;

            do
            {
                try
                {
                    tok = lexer.NextToken();
                    tokens.Add(tok);
                }
                catch (LexicalException ex)
                {
                    lexErrors.Add(ex);
                    tok = null;
                }
            }
            while (tok == null || tok.Type != TokenType.EndOfFile);

            var outLog = new StringBuilder();

            if (lexErrors.Count > 0)
            {
                hasErrors = true;
                foreach (var ex in lexErrors)
                {
                    outLog.AppendLine($"Лексическая ошибка: строка {ex.Line}, столбец {ex.Column}: {ex.Message}");
                }
                outLog.AppendLine($"Всего ошибок: {lexErrors.Count}");
                return outLog.ToString();
            }

            var parser = new Parser(tokens);
            parser.ParseFunction();

            foreach (var err in parser.Errors)
            {
                outLog.AppendLine($"Синтаксическая ошибка: строка {err.Line}, столбец {err.Column}: ожидалась '{err.Expected}', но найдено '{err.Found}'");
            }

            int totalErrors = parser.Errors.Count;

            if (totalErrors > 0)
            {
                hasErrors = true;
                outLog.AppendLine($"Всего ошибок: {totalErrors}");
                return outLog.ToString();
            }

            outLog.AppendLine("Функция корректна.");
            outLog.AppendLine("Всего ошибок: 0");
            return outLog.ToString();
        }

        public static string FixCode(string source)
        {
            var fixedSource = new StringBuilder(source);
            var lexErrors = new List<LexicalException>();
            var tokens = new List<Token>();
            var lexer = new Lexer(source);
            Token tok = null;
            int offset = 0;

            do
            {
                try
                {
                    tok = lexer.NextToken();
                    tokens.Add(tok);
                }
                catch (LexicalException ex)
                {
                    lexErrors.Add(ex);
                    int pos = GetAbsoluteCharIndex(source, ex.Line, ex.Column);
                    if (pos >= 0 && pos < fixedSource.Length)
                        fixedSource.Remove(pos - offset, 1);
                    offset++;
                }
            }
            while (tok == null || tok.Type != TokenType.EndOfFile);

            source = fixedSource.ToString();
            lexer = new Lexer(source);
            tokens.Clear();
            do { tok = lexer.NextToken(); tokens.Add(tok); } while (tok.Type != TokenType.EndOfFile);

            var parser = new Parser(tokens);
            parser.ParseFunction();

            var result = new StringBuilder(source);
            offset = 0;
            foreach (var err in parser.Errors)
            {
                int pos = GetAbsoluteCharIndex(result.ToString(), err.Line, err.Column);
                if (pos < 0) continue;

                if (err.Expected == ";" || err.Expected == "," || err.Expected == ")" || err.Expected == "}" || err.Expected == "(")
                {
                    result.Insert(pos + offset, err.Expected);
                    offset += err.Expected.Length;
                }
                else if (err.Expected == "return" && Distance(err.Found, "return") <= 2)
                {
                    result.Remove(pos + offset, err.Found.Length);
                    result.Insert(pos + offset, "return");
                    offset += "return".Length - err.Found.Length;
                }
                else
                {
                    result.Remove(pos + offset, Math.Min(err.Found.Length, result.Length - pos - offset));
                }
            }


            return result.ToString();
        }

        private static int GetAbsoluteCharIndex(string text, int line, int column)
        {
            int currentLine = 1;
            int currentCol = 1;
            for (int i = 0; i < text.Length; i++)
            {
                if (currentLine == line && currentCol == column)
                    return i;
                if (text[i] == '\n') { currentLine++; currentCol = 1; }
                else currentCol++;
            }
            return -1;
        }

        private static int Distance(string a, string b)
        {
            int[,] dp = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) dp[0, j] = j;
            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    if (a[i - 1] == b[j - 1])
                        dp[i, j] = dp[i - 1, j - 1];
                    else
                        dp[i, j] = 1 + Math.Min(dp[i - 1, j - 1], Math.Min(dp[i - 1, j], dp[i, j - 1]));
                }
            }
            return dp[a.Length, b.Length];
        }
    }
}
