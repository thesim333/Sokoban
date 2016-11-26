using System.Windows.Forms;

namespace Sokoban
{
    public partial class StateLoadDialog : Form
    {
        public StateLoadDialog()
        {
            InitializeComponent();
        }

        public void InsertStates(string[] states)
        {
            listBoxStates.Items.AddRange(states);
            btnOK.Enabled = (states.Length > 0);
        }

        public string GetSelected()
        {
            return listBoxStates.SelectedItem.ToString();
        }
    }
}
