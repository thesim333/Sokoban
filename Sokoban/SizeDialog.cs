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
