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
using Compiler_1._0;


namespace Compiler_1._0
{

    public partial class Form1 : Form
    {



        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateLineNumbers();
            richTextBox2.Clear();
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
            this.Text = "Compiler";

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string source = richTextBox1.Text;
            bool hasErrors;
            string result = AnalyzerHelper.AnalyzeCode(source, out hasErrors);

            richTextBox2.ForeColor = hasErrors ? Color.Red : Color.Green;
            richTextBox2.Text = result;
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
            richTextBox2.SelectionStart = 0;
            richTextBox2.ScrollToCaret();
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
            string helpFile = Path.Combine(Application.StartupPath, "help.html");

            if (File.Exists(helpFile))
            {
                System.Diagnostics.Process.Start(helpFile);
            }
            else
            {
                MessageBox.Show("Файл справки не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Reference_Click(object sender, EventArgs e)
        {
            вызовСправкиToolStripMenuItem_Click(sender, e);
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAboutInfo();
        }

        private void пускToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton2_Click(sender, e);
        }

        private void richTextBox1_VScroll(object sender, EventArgs e)
        {
            SyncScroll();
        }

        private void UpdateLineNumbers()
        {
            int firstLine = richTextBox1.GetLineFromCharIndex(richTextBox1.GetCharIndexFromPosition(new Point(0, 0)));
            int lastLine = richTextBox1.GetLineFromCharIndex(richTextBox1.GetCharIndexFromPosition(new Point(0, richTextBox1.ClientSize.Height)));

            var lineNumbers = new StringBuilder();
            for (int i = firstLine + 1; i <= lastLine + 1; i++)
            {
                lineNumbers.AppendLine(i.ToString());
            }

            lineNumbersBox.Text = lineNumbers.ToString();
        }

        private void SyncScroll()
        {
            int d = richTextBox1.GetPositionFromCharIndex(0).Y - lineNumbersBox.GetPositionFromCharIndex(0).Y;
            lineNumbersBox.Location = new Point(lineNumbersBox.Location.X, d);
        }

        private void постановкаЗадачиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string helpFile = Path.Combine(Application.StartupPath, "task.html");

            if (File.Exists(helpFile))
            {
                System.Diagnostics.Process.Start(helpFile);
            }
            else
            {
                MessageBox.Show("Файл task.html не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void грамматикаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "grammar.html");

            if (File.Exists(path))
                System.Diagnostics.Process.Start(path);
            else
                MessageBox.Show("Файл grammar.html не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void классификацияГрамматикиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "classification.html");

            if (File.Exists(path))
                System.Diagnostics.Process.Start(path);
            else
                MessageBox.Show("Файл classification.html не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void методАнализаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "analysis.html");

            if (File.Exists(path))
                System.Diagnostics.Process.Start(path);
            else
                MessageBox.Show("Файл analysis.html не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void тестовыйПримерToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "test.html");

            if (File.Exists(path))
                System.Diagnostics.Process.Start(path);
            else
                MessageBox.Show("Файл test.html не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void списокЛитературыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "literature.html");

            if (File.Exists(path))
                System.Diagnostics.Process.Start(path);
            else
                MessageBox.Show("Файл literature.html не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void исходныйКодПрограммыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "code.html");

            if (File.Exists(path))
                System.Diagnostics.Process.Start(path);
            else
                MessageBox.Show("Файл code.html не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string source = richTextBox1.Text;
            string fixedCode = AnalyzerHelper.FixCode(source);

            richTextBox1.Text = fixedCode;
            MessageBox.Show("Ошибки устранены. Выполнен повторный анализ.", "Исправление завершено", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Повторный запуск анализа
            toolStripButton2_Click(sender, e);
        }


            
    }
}
