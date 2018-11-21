using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace B12___Pozoriste
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        public DataTable GetTable()
        {
            return null;
        }

        private void predstaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TheaterForm predstaveForm = new TheaterForm();
            predstaveForm.Show();
        }

        private void pokomadimaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PartsForm komadiForm = new PartsForm();
            komadiForm.Show();
        }
    }
}
