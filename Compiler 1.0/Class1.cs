using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

class Class1
{
    public static void НайтиSSN(RichTextBox richTextBox)
    {
        string text = richTextBox.Text;
        string pattern = @"\b\d{3}-\d{2}-\d{4}\b";

        Regex regex = new Regex(pattern);
        MatchCollection matches = regex.Matches(text);

        if (matches.Count == 0)
        {
            MessageBox.Show("Номера SSN не найдены.", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Сброс подсветки
        richTextBox.SelectAll();
        richTextBox.SelectionBackColor = richTextBox.BackColor;
        richTextBox.DeselectAll();

        StringBuilder result = new StringBuilder("Найденные SSN:\n\n");

        string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        int globalIndex = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            MatchCollection lineMatches = regex.Matches(line);

            foreach (Match match in lineMatches)
            {
                int localIndex = match.Index;

                result.AppendLine($"Номер строки: {i + 1}");
                result.AppendLine($"Строка: {line}");
                result.AppendLine($"Позиция в строке: {localIndex}");
                result.AppendLine();

                // Подсветка найденного SSN
                richTextBox.Select(globalIndex + localIndex, match.Length);
                richTextBox.SelectionBackColor = Color.Yellow;
            }

            globalIndex += line.Length + 1;
        }

        richTextBox.DeselectAll();
        MessageBox.Show(result.ToString(), "Результат поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }


    public static void НайтиГоды(RichTextBox richTextBox)
    {
        string text = richTextBox.Text;
        string pattern = @"\b(1998|1999|200[0-4])\b";

        Regex regex = new Regex(pattern);
        MatchCollection matches = regex.Matches(text);

        if (matches.Count == 0)
        {
            MessageBox.Show("Годы между 1998 и 2004 не найдены.", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Сброс подсветки
        richTextBox.SelectAll();
        richTextBox.SelectionBackColor = richTextBox.BackColor;
        richTextBox.DeselectAll();

        StringBuilder result = new StringBuilder("Найденные годы:\n\n");

        string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        int globalIndex = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            MatchCollection lineMatches = regex.Matches(line);

            foreach (Match match in lineMatches)
            {
                int localIndex = match.Index;

                result.AppendLine($"Номер строки: {i + 1}");
                result.AppendLine($"Строка: {line}");
                result.AppendLine($"Позиция в строке: {localIndex}");
                result.AppendLine();

                // Подсветка найденного года
                richTextBox.Select(globalIndex + localIndex, match.Length);
                richTextBox.SelectionBackColor = Color.LightGreen;
            }

            globalIndex += line.Length + 1; // учёт символа переноса строки
        }

        richTextBox.DeselectAll();
        MessageBox.Show(result.ToString(), "Результат поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static void НайтиДолготы(RichTextBox richTextBox)
    {
        string text = richTextBox.Text;
        string pattern = @"\b-?(180(\.0+)?|1[0-7]\d(\.\d+)?|[1-9]?\d(\.\d+)?)\b";

        Regex regex = new Regex(pattern);
        MatchCollection matches = regex.Matches(text);

        if (matches.Count == 0)
        {
            MessageBox.Show("Долготы не найдены.", "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        // Сброс подсветки
        richTextBox.SelectAll();
        richTextBox.SelectionBackColor = richTextBox.BackColor;
        richTextBox.DeselectAll();

        StringBuilder result = new StringBuilder("Найденные долготы:\n\n");

        string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        int globalIndex = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            MatchCollection lineMatches = regex.Matches(line);

            foreach (Match match in lineMatches)
            {
                int localIndex = match.Index;

                result.AppendLine($"Номер строки: {i + 1}");
                result.AppendLine($"Строка: {line}");
                result.AppendLine($"Позиция в строке: {localIndex}");
                result.AppendLine();

                // Подсветка найденной долготы
                richTextBox.Select(globalIndex + localIndex, match.Length);
                richTextBox.SelectionBackColor = Color.LightSkyBlue;
            }

            globalIndex += line.Length + 1;
        }

        richTextBox.DeselectAll();
        MessageBox.Show(result.ToString(), "Результат поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
