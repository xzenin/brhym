using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoemEditor
{
    public partial class Confirm : Form
    {
        public string Message {
            get
            {
                return textBoxMessage.Text;
            }
            set
            {
                textBoxMessage.Text = value;
            }
        }
        public Confirm()
        {
            InitializeComponent();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
