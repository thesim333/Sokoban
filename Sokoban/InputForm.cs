using System;
using System.Windows.Forms;

namespace Sokoban
{
    /// <summary>
    /// Dialog for getting user input (player name or state name).
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class InputForm : Form
    {
        public InputForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the label denoting what input is required.
        /// </summary>
        /// <param name="message">The message.</param>
        public void SetLabel(string message)
        {
            lblWhat.Text = message;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <returns></returns>
        public string GetResult()
        {
            return txtBoxName.Text;
        }

        /// <summary>
        /// Handles the TextChanged event of the txtBoxName control.
        /// The user cannot click OK unless the textbox contains text.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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
