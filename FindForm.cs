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
    public partial class FindForm : Form
    {
        public FindForm()
        {
            InitializeComponent();
        }
        // властивість яка повертає текст що ввів користувач
        public string FindText
        {
            get { return txtFind.Text; }
        }

        // властивість яка формує умови пошуку
        public RichTextBoxFinds FindCondition
        {
            get
            {
                if (cbMatchCase.Checked && cbMatchWhole.Checked)
                    return RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord;

                if (cbMatchCase.Checked)
                    return RichTextBoxFinds.MatchCase;

                if (cbMatchWhole.Checked)
                    return RichTextBoxFinds.WholeWord;

                return RichTextBoxFinds.None;
            }
        }
    }
}
