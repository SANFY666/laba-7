using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace laba_7
{
    public partial class Form1 : Form
    {
        private int openDocuments = 0;
        public Form1()
        {
            InitializeComponent();
            IsMdiContainer = true;
            SetLanguage("English");
            FileNew_Click(this, EventArgs.Empty);
        }

        private void FileNew_Click(object sender, EventArgs e)
        {
            blank frm = new blank();
            frm.DocName = ""; // пусто аби Save() зрозумів що це новий файл
            frm.MdiParent = this;
            frm.Text = "Untitled " + ++openDocuments;
            frm.WindowState = FormWindowState.Maximized;
            frm.Show();
        }

        private void FileOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //підтримка RTF
            openFileDialog1.Filter = "RTF Document (*.rtf)|*.rtf|Text Files (*.txt)|*.txt|All Files(*.*)|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                blank frm = new blank();
                frm.Open(openFileDialog1.FileName);
                frm.MdiParent = this;
                frm.DocName = openFileDialog1.FileName;
                frm.Text = frm.DocName;
                frm.Show();
            }
        }

        private void FileSave_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                // збереження за замовчуванням у RTF
                saveFileDialog1.Filter = "RTF Document (*.rtf)|*.rtf|Text Files (*.txt)|*.txt";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    frm.Save(saveFileDialog1.FileName);
                    frm.DocName = saveFileDialog1.FileName;
                    frm.Text = frm.DocName;
                }
            }
        }

        private void FileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WindowCascade_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void WindowTileHorizontal_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileHorizontal); 
        }

        private void WindowTileVertical_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.TileVertical);
        }

        private void EditCut_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.Cut(); // метод вирізання
            }
        }

        private void EditCopy_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.Copy();
            }
        }

        private void EditPaste_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.Paste();
            }
        }

        private void EditDelete_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.Delete();
            }
        }

        private void EditSelectAll_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.SelectAll();
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                fontDialog1.ShowColor = true;
                fontDialog1.Font = frm.richTextBox1.SelectionFont;
                fontDialog1.Color = frm.richTextBox1.SelectionColor;

                if (fontDialog1.ShowDialog() == DialogResult.OK)
                {
                    var rb = frm.richTextBox1;
                    rb.SelectionFont = fontDialog1.Font;
                    rb.SelectionColor = fontDialog1.Color;
                }
            }
        }

        private void FormatColor_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                colorDialog1.Color = frm.richTextBox1.SelectionColor; 

            if (colorDialog1.ShowDialog() == DialogResult.OK) 
            {   
                frm.richTextBox1.SelectionColor = colorDialog1.Color; 
            }
                }
        }

        private void leftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.SetAlignment(HorizontalAlignment.Left);
            }
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.SetAlignment(HorizontalAlignment.Center);
            }
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.SetAlignment(HorizontalAlignment.Right);
            }
        }

        private void insertImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                OpenFileDialog fd = new OpenFileDialog();

                fd.Filter = "Зображення|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    frm.InsertImage(fd.FileName);
                }
            }
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                FindForm findForm = new FindForm();

                if (findForm.ShowDialog(this) == DialogResult.OK)
                {
                    string searchText = findForm.FindText;
                    RichTextBoxFinds options = findForm.FindCondition;

                    // якщо користувач нічого не ввів, просто виходити
                    if (string.IsNullOrEmpty(searchText)) return;

                    int count = 0;
                    int startIndex = 0;

                    // шукаю слово поки не дійду до кінця тексту
                    while (startIndex < frm.richTextBox1.TextLength)
                    {
                        // наступний збіг
                        int index = frm.richTextBox1.Find(searchText, startIndex, options);

                        if (index != -1) // коли знайшли
                        {
                            count++; // збільшуємо лічильник
                            startIndex = index + searchText.Length;
                        }
                        else
                        {
                            break; // якщо більше не знайшов то виходиш з циклу
                        }
                    }

                    if (count > 0)
                    {
                        MessageBox.Show($"Слово '{searchText}' знайдено!\nКількість повторів у тексті: {count}",
                                        "Результат пошуку",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);

                        frm.richTextBox1.Find(searchText, 0, options);

                        // фокус на текст щоб виділення стало синім
                        frm.Activate();
                        frm.richTextBox1.Focus();
                    }
                    else
                    {
                        // коли слова взагалі немає
                        MessageBox.Show("Такого слова в тексті немає!", "Результат пошуку", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void aboutProgramm_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            FileNew_Click(sender, e); 
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            FileOpen_Click(sender, e);
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            FileSave_Click(sender, e);
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            EditCut_Click(sender, e);
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            EditCopy_Click(sender, e);
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            EditPaste_Click(sender, e);
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            aboutProgramm_Click(sender, e);
        }

        private void SetLanguage(string lang)
        {
            if (lang == "English")
            {
                // Головне меню
                fileToolStripMenuItem.Text = "File";
                editToolStripMenuItem.Text = "Edit";
                FormatFont.Text = "Format";
                findToolStripMenuItem.Text = "Find";
                insertImageToolStripMenuItem.Text = "InsertImage";
                windowsToolStripMenuItem.Text = "Window";
                languageToolStripMenuItem.Text = "Language";

                // Підменю File 
                FileNew.Text = "New";
                FileOpen.Text = "Open";
                FileSave.Text = "Save";
                FileExit.Text = "Exit";

                // Підменю Edit
                EditCut.Text = "Cut";
                EditCopy.Text = "Copy";
                EditPaste.Text = "Paste";
                EditDelete.Text = "Delete";
                EditSelectAll.Text = "SelectAll";

                // Підменю Format
                fontToolStripMenuItem.Text = "Font";
                FormatColor.Text = "Color";
                alignmentToolStripMenuItem.Text = "Alignment";
                boldToolStripMenuItem.Text = "Bold";
                italicToolStripMenuItem.Text = "Italic";
                underlineToolStripMenuItem.Text = "Underline";

                tsbBold.ToolTipText = "Bold";
                tsbItalic.ToolTipText = "Italic";
                tsbUnderline.ToolTipText = "Underline";

                // Підменю Window
                WindowCascade.Text = "Cascade";
                WindowTileHorizontal.Text = "TileHorizontal";
                WindowTileVertical.Text = "TileVertical";

                // Підменю ?
                aboutProgramm.Text = "aboutProgramm";
            }
            else if (lang == "Ukrainian")
            {
                // Головне меню
                fileToolStripMenuItem.Text = "Файл";
                editToolStripMenuItem.Text = "Правка";
                FormatFont.Text = "Формат";
                findToolStripMenuItem.Text = "Шукати";
                insertImageToolStripMenuItem.Text = "Вставити фото";
                windowsToolStripMenuItem.Text = "Вікно";
                languageToolStripMenuItem.Text = "Мова";

                // Підменю Файл
                FileNew.Text = "Створити";
                FileOpen.Text = "Відкрити...";
                FileSave.Text = "Зберегти";
                FileExit.Text = "Вихід";

                // Підменю Edit
                EditCut.Text = "Вирізати";
                EditCopy.Text = "Копіювати";
                EditPaste.Text = "Вставити";
                EditDelete.Text = "Видалити";
                EditSelectAll.Text = "Вибрати все";

                // Підменю Format
                fontToolStripMenuItem.Text = "Шрифт";
                FormatColor.Text = "Колір";
                alignmentToolStripMenuItem.Text = "Розташування";
                boldToolStripMenuItem.Text = "Жирний";
                italicToolStripMenuItem.Text = "Курсив";
                underlineToolStripMenuItem.Text = "Підкреслений";

                tsbBold.ToolTipText = "Жирний текст";
                tsbItalic.ToolTipText = "Курсив";
                tsbUnderline.ToolTipText = "Підкреслений текст";

                // Підменю Window
                WindowCascade.Text = "Каскад";
                WindowTileHorizontal.Text = "Плитка горизонтальна";
                WindowTileVertical.Text = "Плитка вертикальна";

                // Підменю ?
                aboutProgramm.Text = "Про програму";

            }
            foreach (Form childForm in this.MdiChildren)
            {
                if (childForm is blank frm)
                {
                    frm.ChangeLanguage(lang); // Передаємо вибір у blank
                }
            }
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("English");
        }

        private void українськаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLanguage("Ukrainian");
        }

        private void boldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.ToggleBold();
            }
        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.ToggleItalic();
            }
        }

        private void underlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.ToggleUnderline();
            }
        }

        private void tsbBold_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.ToggleBold();
            }
        }

        private void tsbItalic_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.ToggleItalic();
            }
        }

        private void tsbUnderline_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild is blank frm)
            {
                frm.ToggleUnderline();
            }
        }
    }
}
