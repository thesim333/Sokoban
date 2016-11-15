using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sokoban
{
    public partial class FileSaveNameDialog : Form
    {
        public FileSaveNameDialog()
        {
            InitializeComponent();
        }

        public string GetName()
        {
            return txtBoxName.Text;
        }

        public void SetName(string name)
        {
            txtBoxName.Text = name;
        }

        public void SetLabel(string l)
        {
            lblNameThing.Text = l;
        }

        private void txtBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            SwitchOK();
        }

        private void txtBoxName_TextChanged(object sender, EventArgs e)
        {
            SwitchOK();
        }

        protected void SwitchOK()
        {
            if (String.IsNullOrEmpty(txtBoxName.Text))
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
