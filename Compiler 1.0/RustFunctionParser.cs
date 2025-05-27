using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler_1._0
{
    public class Parser
    {
        private List<Lexeme> tokens;
        private int index;
        private Lexeme Current => index < tokens.Count ? tokens[index] : null;

        private List<string> syntaxErrors = new List<string>();
        public int ErrorCount => syntaxErrors.Count;

        public Parser(List<Lexeme> lexemes)
        {
            tokens = lexemes;
            index = 0;
        }
        private void SynchronizeToNext(TokenType type)
        {
            while (Current != null && Current.Type != type)
                Advance();
        }


        public string Result
        {
            get
            {
                var output = new StringBuilder();
                foreach (var error in syntaxErrors)
                    output.AppendLine("❌ " + error);

                if (syntaxErrors.Count == 0)
                    output.AppendLine("✅ Синтаксически корректная функция.");
                else
                    output.AppendLine($"Всего ошибок: {syntaxErrors.Count}");

                return output.ToString();
            }
        }

        public bool Parse()
        {
            ParseFunction();

            if (index < tokens.Count)
                AddError("Лишние символы после конца функции");

            return syntaxErrors.Count == 0;
        }

        private void AddError(string msg)
        {
            string where = Current != null ? $"'{Current.Value}' → {Current.Position}" : "в конце ввода";
            string type = Current != null && Current.Type == TokenType.Unknown ? "ЛЕКСИЧЕСКАЯ" : "СИНТАКСИЧЕСКАЯ";
            syntaxErrors.Add($"[{type} ОШИБКА] {msg} [{where}]");
        }



        private void Advance() => index++;

        private bool Match(string value)
        {
            if (Current != null && Current.Value == value)
            {
                Advance();
                return true;
            }
            return false;
        }

        private bool Match(TokenType type)
        {
            if (Current != null && Current.Type == type)
            {
                Advance();
                return true;
            }
            return false;
        }

        private void ParseFunction()
        {
            bool hasError = false;

            if (!Match("fn"))
            {
                AddError("Ожидалось ключевое слово 'fn'");
                SynchronizeToNext(TokenType.Identifier); // переходим к следующему шагу
            }


            if (!Match(TokenType.Identifier))
            {
                AddError("Ожидалось имя функции");
                hasError = true;
            }

            if (!Match("("))
            {
                AddError("Ожидалась '('");
                hasError = true;
            }

            // Аргументы
            if (!Match(")"))
            {
                do
                {
                    if (!Match(TokenType.Identifier))
                    {
                        AddError("Ожидался идентификатор аргумента");
                        hasError = true;
                        break;
                    }

                    if (!Match(":"))
                    {
                        AddError("Ожидался ':'");
                        hasError = true;
                        break;
                    }

                    if (!Match(TokenType.Type))
                    {
                        AddError("Ожидался тип аргумента (i32, bool, и т.д.)");
                        hasError = true;
                        break;
                    }
                } while (Match(","));

                if (!Match(")"))
                {
                    AddError("Ожидалась ')' после аргументов");
                    hasError = true;
                }
            }

            if (!Match("->"))
            {
                AddError("Ожидался '->' перед возвращаемым типом");
                hasError = true;
            }

            if (!Match(TokenType.Type))
            {
                AddError("Ожидался возвращаемый тип");
                hasError = true;
            }

            if (!Match("{"))
            {
                AddError("Ожидалась '{'");
                hasError = true;
            }

            if (!Match("return"))
            {
                AddError("Ожидалось ключевое слово 'return'");
                hasError = true;
            }

            ParseExpression();

            if (!Match(";"))
            {
                AddError("Ожидался ';' после выражения");
                hasError = true;
            }

            if (!Match("}"))
            {
                AddError("Ожидалась '}'");
                hasError = true;
            }

            if (!Match(";"))
            {
                AddError("Ожидался ';' в конце функции");
                hasError = true;
            }
        }


        private void ParseExpression()
        {
            ParseTerm();
            while (Current != null && (Current.Value == "+" || Current.Value == "-"))
            {
                Advance();
                ParseTerm();
            }
        }

        private void ParseTerm()
        {
            ParseFactor();
            while (Current != null && (Current.Value == "*" || Current.Value == "/"))
            {
                Advance();
                ParseFactor();
            }
        }

        private void ParseFactor()
        {
            if (Match(TokenType.Identifier) || Match(TokenType.Number))
                return;

            if (Match("("))
            {
                ParseExpression();
                if (!Match(")"))
                    AddError("Ожидалась закрывающая скобка ')'");
            }
            else
            {
                AddError("Ожидался идентификатор, число или выражение в скобках");
            }
        }
    }
}

