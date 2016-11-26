using System;
using System.Windows.Forms;

namespace Sokoban
{
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
        }

        public void SetLabel(string message)
        {
            lblWhat.Text = message;
        }

        public string GetResult()
        {
            return txtBoxName.Text;
        }

        private void txtBoxName_TextChanged(object sender, EventArgs e)
        {
            if (txtBoxName.Text.Length == 0)
            {
                btnOK.Enabled = false;
            }
            else
            {
                btnOK.Enabled = true;
            }
        }
    }
}
