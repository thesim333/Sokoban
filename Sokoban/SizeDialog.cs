using System.Windows.Forms;

namespace Sokoban
{
    /// <summary>
    /// Dialog for getting the rows and columns to use as the size of a new level grid.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class SizeDialog : Form
    {
        public SizeDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the rows 10 to 20.
        /// </summary>
        /// <returns>Rows</returns>
        public int GetRows()
        {
            return (int)numUpDownRows.Value;
        }

        /// <summary>
        /// Gets the columns 10 to 20.
        /// </summary>
        /// <returns>Columns</returns>
        public int GetCols()
        {
            return (int)numUpDownCols.Value;
        }
    }
}
