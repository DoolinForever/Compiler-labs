using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using static Class1;

namespace Compiler_1._0
{
    public partial class Form1 : Form
    {
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void ShowHelp()
        {
            string helpText =
        @"СПРАВКА

Файл
• Создать — очищает текстовое поле для нового файла.
• Открыть — загружает текстовый файл в редактор.
• Сохранить — сохраняет текущий файл. Если он ещё не был сохранён, вызывает 'Сохранить как'.
• Сохранить как — позволяет выбрать путь и имя файла для сохранения.
• Выход — завершает работу программы, с предупреждением о несохранённых данных.

Правка
• Отменить — отменяет последнее действие.
• Повторить — возвращает последнее отменённое действие.
• Вырезать — удаляет выделенный текст и копирует его в буфер обмена.
• Копировать — копирует выделенный текст.
• Вставить — вставляет текст из буфера в позицию курсора.
• Удалить — удаляет выделенный текст без копирования.
• Выделить всё — выделяет весь текст в редакторе.

Справка
• Вызов справки — показывает это окно.
• О программе — информация об авторе и версии.";

            MessageBox.Show(helpText, "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void ShowAboutInfo()
        {
            string info =
@"О ПРОГРАММЕ

Название: Компилятор простых функций
Версия: 1.0
Автор: Присекин Сергей
Год: 2025

Описание:
Это небольшое приложение создано для выявления ошибок написанной функции на языке RUST.";

            MessageBox.Show(info, "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }



        private string currentFilePath = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Созранить_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentFilePath)) // Проверяем, есть ли уже сохранённый файл
            {
                try
                {
                    File.WriteAllText(currentFilePath, richTextBox1.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Если путь пустой, вызываем "Сохранить как"
                сохрнаитьКакToolStripMenuItem_Click(sender, e);
            }
        }

        private void О_программе_Click(object sender, EventArgs e)
        {
            ShowAboutInfo();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Create_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            openFileDialog.Title = "Открыть файл";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Сохраняем путь к открытому файлу
                    currentFilePath = openFileDialog.FileName;

                    // Загружаем текст из файла в редактор
                    richTextBox1.Text = File.ReadAllText(currentFilePath);

                    // Обновляем заголовок окна
                    this.Text = "Редактор - " + Path.GetFileName(currentFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при открытии файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            openFileDialog.Title = "Открыть файл";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Сохраняем путь к открытому файлу
                    currentFilePath = openFileDialog.FileName;

                    // Загружаем текст из файла в редактор
                    richTextBox1.Text = File.ReadAllText(currentFilePath);

                    // Обновляем заголовок окна
                    this.Text = "Редактор - " + Path.GetFileName(currentFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при открытии файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void сохрнаитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
            saveFileDialog.Title = "Сохранить файл как";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    currentFilePath = saveFileDialog.FileName;

                    File.WriteAllText(currentFilePath, richTextBox1.Text);

                    this.Text = "Редактор - " + Path.GetFileName(currentFilePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении файла: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(richTextBox1.Text) && string.IsNullOrEmpty(currentFilePath))
            {
                var result = MessageBox.Show("У вас есть несохранённые изменения. Выйти без сохранения?",
                                             "Подтверждение выхода",
                                             MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                    return;
            }

            // Закрываем приложение
            Application.Exit();
        }

        private void отменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo) 
            {
                richTextBox1.Undo();
            }
        }

        private void повторитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanRedo) 
            {
                richTextBox1.Redo(); 
            }
        }

        private void вырезатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                richTextBox1.Cut(); 
            }
        }

        private void копироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                richTextBox1.Copy(); 
            }
        }

        private void вставитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                richTextBox1.Paste(); 
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                richTextBox1.SelectedText = ""; 
            }
        }

        private void выделитьВсеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll(); 
            richTextBox1.Focus();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
            {
                richTextBox1.Undo();
            }
        }

        private void Repeat_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanRedo)
            {
                richTextBox1.Redo();
            }
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                richTextBox1.Copy();
            }
        }

        private void Cut_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength > 0)
            {
                richTextBox1.Cut();
            }
        }

        private void Insert_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                richTextBox1.Paste();
            }
        }

        private void вызовСправкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        private void Reference_Click(object sender, EventArgs e)
        {
            ShowHelp();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutInfo();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
           
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            string input = richTextBox1.Text;

            StringBuilder sb = new StringBuilder();

            try
            {
                Lexer lexer = new Lexer(input);
                lexer.Tokenize();

                sb.AppendLine("Лексемы:");
                foreach (var token in lexer.Tokens)
                    sb.AppendLine(token.ToString());

                Parser parser = new Parser(lexer.Tokens);
                sb.AppendLine("\nАнализ:\n");
                sb.AppendLine(parser.Parse());
            }
            catch (Exception ex)
            {
                sb.AppendLine($"\nОШИБКА: {ex.Message}");
            }

            richTextBox2.Text = sb.ToString();
        }

    }
}
