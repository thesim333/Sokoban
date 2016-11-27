using System.Windows.Forms;

namespace Sokoban
{
    /// <summary>
    /// Dialog for displaying all states in a file for the user to select one.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class StateLoadDialog : Form
    {
        public StateLoadDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Inserts the states.
        /// User cannot click OK if there are no states.
        /// </summary>
        /// <param name="states">The states.</param>
        public void InsertStates(string[] states)
        {
            listBoxStates.Items.AddRange(states);
            btnOK.Enabled = (states.Length > 0);
        }

        /// <summary>
        /// Gets the selected state.
        /// </summary>
        /// <returns>The name of the state as string</returns>
        public string GetSelected()
        {
            return listBoxStates.SelectedItem.ToString();
        }
    }
}
