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
    public partial class LoadFileFromListDialog : Form
    {
        public LoadFileFromListDialog()
        {
            InitializeComponent();
        }

        public void InsertLevels(string[] levels)
        {
            lstBoxLevels.Items.AddRange(levels);

            btnOK.Enabled = (levels.Length > 0);
        }

        public string GetSelected()
        {
            return lstBoxLevels.SelectedItem.ToString();
        }

        public void SetText(string text)
        {
            this.Text = text;
        }
    }
}
