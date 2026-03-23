using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laba_7
{
    public partial class blank : Form
    {
        public string DocName = "";
        public bool IsSaved = false;

        private DateTime startTime;
        private Timer workTimer;

        // зберігання слів мовою інтерфейсу
        private string textAmount = "Characters: ";
        private string textTime = "Working: ";

        public blank()
        {
            InitializeComponent();

            sbAmount.Text = textAmount + "0";
            startTime = DateTime.Now;

            sbTime.ToolTipText = DateTime.Now.ToString("M.dd.yyyy");

            workTimer = new Timer();
            workTimer.Interval = 1000; // 1000 мілісекунд = 1 секунда
            workTimer.Tick += new EventHandler(WorkTimer_Tick);
            workTimer.Start();

            lblZoom.Text = "100%"; 

            // рух колеса миші
            richTextBox1.MouseWheel += new MouseEventHandler(richTextBox1_MouseWheel);

            // зміна тексту
            richTextBox1.TextChanged += new EventHandler(richTextBox1_TextChanged);
        }

        private void WorkTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan elapsed = DateTime.Now - startTime;
            // час у форматі Години:Хвилини:Секунди
            sbTime.Text = string.Format("{0}{1:hh\\:mm\\:ss}", textTime, elapsed);
        }

        private void richTextBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            // перевірка нажаття клавіші Ctrl 
            if (Control.ModifierKeys == Keys.Control)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    // Беремо поточний масштаб (1.0 = 100%, 1.5 = 150%) і переводимо в цілі відсотки
                    int percent = (int)(Math.Round(richTextBox1.ZoomFactor, 2) * 100);
                    lblZoom.Text = percent.ToString() + "%";
                });
            }
        }

        // стандартні операції
        public void Cut() { richTextBox1.Cut(); }
        public void Copy() { richTextBox1.Copy(); }
        public void Paste() { richTextBox1.Paste(); }
        public void SelectAll() { richTextBox1.SelectAll(); }
        public void Delete() { richTextBox1.SelectedText = ""; }

        // зробити жирним 
        public void ToggleBold()
        {
            if (richTextBox1.SelectionFont != null)
            {
                FontStyle currentStyle = richTextBox1.SelectionFont.Style;
                // якщо вже жирний забираємо жирність якщо ні то додаємо
                FontStyle newStyle = richTextBox1.SelectionFont.Bold ? (currentStyle & ~FontStyle.Bold) : (currentStyle | FontStyle.Bold);
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, newStyle);
            }
        }

        // зробити курсивом (зігнутим)
        public void ToggleItalic()
        {
            if (richTextBox1.SelectionFont != null)
            {
                FontStyle currentStyle = richTextBox1.SelectionFont.Style;
                FontStyle newStyle = richTextBox1.SelectionFont.Italic ? (currentStyle & ~FontStyle.Italic) : (currentStyle | FontStyle.Italic);
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, newStyle);
            }
        }

        // зробити підкресленим
        public void ToggleUnderline()
        {
            if (richTextBox1.SelectionFont != null)
            {
                FontStyle currentStyle = richTextBox1.SelectionFont.Style;
                FontStyle newStyle = richTextBox1.SelectionFont.Underline ? (currentStyle & ~FontStyle.Underline) : (currentStyle | FontStyle.Underline);
                richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont, newStyle);
            }
        }

        // відкриття файлу з підтримкою RTF та звичайного тексту
        public void Open(string OpenFileName)
        {
            if (string.IsNullOrEmpty(OpenFileName)) return;
            try
            {
                richTextBox1.LoadFile(OpenFileName, RichTextBoxStreamType.RichText);
            }
            catch
            {
                // якщо це не RTF то відкриваємо як звичайний текст
                StreamReader sr = new StreamReader(OpenFileName);
                richTextBox1.Text = sr.ReadToEnd();
                sr.Close();
            }
            DocName = OpenFileName;
            IsSaved = true;
        }

        // збереження файлу у форматі RTF або TXT
        public void Save(string SaveFileName)
        {
            // якщо ім'я порожнє (новий файл) викликаю діалог "Save As"
            if (string.IsNullOrEmpty(SaveFileName))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "RTF файли (*.rtf)|*.rtf|Всі файли (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveFileName = sfd.FileName;
                }
                else
                {
                    return;
                }
            }

            try
            {
                // зберігаю файл
                richTextBox1.SaveFile(SaveFileName, RichTextBoxStreamType.RichText);

                // оновлюю дані форми
                this.DocName = SaveFileName;
                this.Text = SaveFileName;
                this.IsSaved = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при збереженні: " + ex.Message);
            }
        }

        // вирівнювання тексту
        public void SetAlignment(HorizontalAlignment alignment)
        {
            richTextBox1.SelectionAlignment = alignment;
        }

        // додавання зображень у документ
        public void InsertImage(string imagePath)
        {
            try
            {
                Image img = Image.FromFile(imagePath);
                Clipboard.SetImage(img);
                richTextBox1.Paste();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка вставки зображення: " + ex.Message);
            }
        }

        private void HighlightSyntax()
        {
            int selPos = richTextBox1.SelectionStart;
            int selLen = richTextBox1.SelectionLength;
            Color originalColor = richTextBox1.SelectionColor;

            // Скидання форматування
            richTextBox1.Select(0, richTextBox1.Text.Length);
            richTextBox1.SelectionColor = Color.Black;
            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);

            // Ключові слова (синій)
            foreach (Match m in Regex.Matches(richTextBox1.Text, @"\b(public|private|protected|class|void|int|string|bool|if|else|while|for|return|new|static|namespace|using|true|false|null)\b"))
            {
                richTextBox1.Select(m.Index, m.Length);
                richTextBox1.SelectionColor = Color.Blue;
            }

            // Рядки в лапках (коричневий)
            foreach (Match m in Regex.Matches(richTextBox1.Text, "\".*?\""))
            {
                richTextBox1.Select(m.Index, m.Length);
                richTextBox1.SelectionColor = Color.Brown;
            }

            // Коментарі (зелений)
            foreach (Match m in Regex.Matches(richTextBox1.Text, @"//.*"))
            {
                richTextBox1.Select(m.Index, m.Length);
                richTextBox1.SelectionColor = Color.Green;
            }

            // Відновлення курсору
            richTextBox1.Select(selPos, selLen);
            richTextBox1.SelectionColor = originalColor;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            IsSaved = false;

            // оновлюємо кількість символів у рядку стану!
            sbAmount.Text = textAmount + richTextBox1.Text.Length.ToString();
            HighlightSyntax();
        }


        // метод який приймає команду від Form1
        public void ChangeLanguage(string lang)
        {
            if (lang == "Ukrainian")
            {
                textAmount = "Кількість символів: ";
                textTime = "Працює: ";

                cutToolStripMenuItem.Text = "Вирізати";
                copyToolStripMenuItem.Text = "Копіювати";
                pasteToolStripMenuItem.Text = "Вставити";
                deleteToolStripMenuItem.Text = "Видалити";
                selectAllToolStripMenuItem.Text = "Виділити все";
            }
            else if (lang == "English")
            {
                textAmount = "Characters: ";
                textTime = "Working: ";

                cutToolStripMenuItem.Text = "Cut";
                copyToolStripMenuItem.Text = "Copy";
                pasteToolStripMenuItem.Text = "Paste";
                deleteToolStripMenuItem.Text = "Delete";
                selectAllToolStripMenuItem.Text = "Select All";
            }

            // миттєво оновлюю текст на панелі
            sbAmount.Text = textAmount + richTextBox1.Text.Length.ToString();
        }

        private void blank_FormClosing(object sender, FormClosingEventArgs e)
        {
            // перевірка чи є незбережені зміни (IsSaved == false) та чи є текст у документі
            if (!IsSaved && richTextBox1.TextLength > 0)
            {
                string fileName = string.IsNullOrEmpty(DocName) ? "Untitled" : Path.GetFileName(DocName);

                // перевірка яка мова зараз активна 
                string msgText = textTime == "Working: "
                    ? $"Do you want to save changes to {fileName}?"
                    : $"Ви хочете зберегти зміни у файлі {fileName}?";

                string msgTitle = textTime == "Working: " ? "Save" : "Збереження";

                DialogResult result = MessageBox.Show(msgText, msgTitle,
                                                    MessageBoxButtons.YesNoCancel,
                                                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Save(DocName);

                    if (!IsSaved)
                    {
                        e.Cancel = true; 
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    // користувач натиснув Скасувати тоді вікно не закриваємо
                    e.Cancel = true;
                }
            }
         }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Delete();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SelectAll();
        }
    }
}
