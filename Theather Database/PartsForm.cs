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
    public partial class PartsForm : Form
    {
        public PartsForm()
        {
            InitializeComponent();
        }

        private void KomadiForm_Load(object sender, EventArgs e)
        {
            LoadKomad();
        }

        private void LoadKomad()
        {
            DataTable dt = new DataTable();
            dt = Database.ExecuteQuery("select KomadID as ID,Naziv as 'Show Name',Trajanje as Duration,BrojCinova as 'Number of Acts',Opis as Description from Pozorisni_Komad");

            dataGridView1.DataSource = dt;
        }

        private void buttonE_Click(object sender, EventArgs e)
        {
            DateTime dateFrom = dateTimePickerOd.Value;
            DateTime dateTo = dateTimePickerDo.Value;

            DataTable dt = Database.ExecuteQuery(string.Format("select (Predstava.CenaKarte * Predstava.BrojMesta) as Income, Pozorisna_Trupa.NazivTrupe as 'Troop Name' " +
                "from ( Pozorisna_Trupa inner join Predstava on Predstava.TrupaID = Pozorisna_Trupa.TrupaID) " +
                "where Predstava.Datum > '{0}' and Predstava.Datum < '{1}'", dateFrom.ToString("yyyy-MM-dd"), dateTo.ToString("yyyy-MM-dd")));

            listView1.Items.Clear();
            foreach(DataRow dr in dt.Rows)
            {
                ListViewItem item = listView1.Items.Add(dr["Troop Name"].ToString());
                item.SubItems.Add(dr["Income"].ToString());
            }
        }

        private void buttonQ_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
