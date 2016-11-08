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
    public partial class SizeDialog : Form
    {
        public SizeDialog()
        {
            InitializeComponent();
        }

        public int GetRows()
        {
            return (int)numUpDownRows.Value;
        }

        public int GetCols()
        {
            return (int)numUpDownCols.Value;
        }
    }
}
