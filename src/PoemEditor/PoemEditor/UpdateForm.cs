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
    public partial class UpdateForm : Form
    {
        public UpdateForm()
        {
            InitializeComponent();
        }

        private void UpdateForm_Load(object sender, EventArgs e)
        {
            textBoxVersion.Text = "Poem Editor, the product created by Pijush Biswas for efficient editing of poem, prose or any article which are in bengali language. Contextual words are built from different sources either downloaded from public website or own collections. Machine Learning are wriiten with python and the GUI is built in C#, DotNet";

            textBoxVersion.Text += Environment.NewLine + "===============================================";
            textBoxVersion.Text += Environment.NewLine + "The first version is released for beta testing. The product is lisence based and would not be free later.";

            textBoxVersion.Text += Environment.NewLine + "===============================================";

        }
    }
}
