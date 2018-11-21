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
    public partial class TheaterForm : Form
    {
        public TheaterForm()
        {
            InitializeComponent();
        }

        private void PredstaveForm_Load(object sender, EventArgs e)
        {
            PrikaziKomade();
        }
        private void button_Izadji_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void UpisiPredstavu(int id, DateTime datum,int brojMesta, int cenaKarte, int pID, int tID)
        {

            Database.ExecuteNonQuery(string.Format("insert into Predstava (KomadID, Datum, BrojMesta, CenaKarte, ProducentID, TrupaID) values (" +
                "{0}, '{1}', {2}, {3}, {4}, {5})", id,datum.ToString("yyyy-MM-dd"), brojMesta, cenaKarte, pID, tID));

            MessageBox.Show(string.Format("Show is sucessfully booked on this date {0}", datum.ToString("yyyy-MM-dd")));
        }
        private void button_Izvrsi_Click(object sender, EventArgs e)
        {
            if (radioButton_Upisi.Checked)
            {
                int id = comboBox_Komad.SelectedIndex+1;
                if (!maskedTextBox_Datum.MaskCompleted && comboBox_Komad.SelectedValue == null && !CheckForUpis())
                    MessageBox.Show("Popunite sva polja pre upisa!");
                else
                {
                    DateTime dt;

                    int brojMesta = int.Parse(textBox_BrojMesta.Text);
                    int cenaKarte = int.Parse(textBox_Cena.Text);
                    int pID = comboBox_Producent.SelectedIndex + 1;
                    int tID = comboBox_Trupa.SelectedIndex + 1;
                    if (DateTime.TryParse(maskedTextBox_Datum.Text, out dt))
                    {
                        UpisiPredstavu(id, dt, brojMesta, cenaKarte, pID, tID);
                    }
                }
            }
            else if (radioButton_Obrisi.Checked)
            {
                int id = 0;
                if (!maskedTextBox_Datum.MaskCompleted && string.IsNullOrEmpty(comboBox_Komad.Text))
                    MessageBox.Show("Upišite barem datum ili šifru komada da bi obrisali nešto.");
                else
                {
                    if(!maskedTextBox_Datum.MaskCompleted)
                    {
                        if (int.TryParse(comboBox_Komad.Text, out id))
                        {
                            DialogResult dialogResult = MessageBox.Show("Obrisaćete sve predstave koje sadrže tu šifru od komada, da li ste sigurni?", "Pitanje", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                            if(dialogResult == DialogResult.Yes)
                            {
                                ClearAll();
                                ObrisiPredstavu(id);
                                MessageBox.Show("Predstave uspešno obrisane.");
                            }
                    
                        }
                
                        else
                            MessageBox.Show("Molim vas unesti ispravnu šifru Komada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        DateTime dt;
                        if (DateTime.TryParse(maskedTextBox_Datum.Text, out dt))
                        {
                            DialogResult dialogResult = MessageBox.Show("Obrisaćete sve predstave koje imaju taj datum, da li ste sigurni?", "Pitanje", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                            if (dialogResult == DialogResult.Yes)
                            {
                                ClearAll();
                                ObrisiPredstavuPoDatumu(dt);
                                MessageBox.Show("Predstave uspešno obrisane.");
                            }

                        }

                        else
                            MessageBox.Show("Molim vas unesti ispravan datum Predstave.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

            }
            else
                MessageBox.Show("Odaberite opciju za upis ili brisanje podataka, pa zatim izvršite!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        private bool CheckForUpis()
        {
            int brojMesta = 0;
            int cena = 0;
            if (!int.TryParse(textBox_BrojMesta.Text, out brojMesta) && !int.TryParse(textBox_Cena.Text, out cena))
                return false;

            return true;
        }

        private void ObrisiPredstavuPoDatumu(DateTime dt)
        {
            Database.ExecuteNonQuery(string.Format("delete from Predstava where Datum = '{0}'", dt.ToString("yyyy-MM-dd")));
        }

        private void comboBox_Komad_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopuniOstalaPolja(comboBox_Komad.SelectedIndex);
        }

        private void ClearAll()
        {
            comboBox_Producent.Text = "";
            comboBox_Trupa.Text = "";
            comboBox_Producent.Items.Clear();
            comboBox_Trupa.Items.Clear();
            textBox_BrojMesta.Clear();
            textBox_Cena.Clear();
            maskedTextBox_Datum.Clear();
        }

        private void ObrisiPredstavu(int id)
        {
            Database.ExecuteNonQuery(string.Format("delete from Predstava where KomadID = {0}", id));
        }
        private void PrikaziKomade()
        {
            DataTable dt = Database.ExecuteQuery("select KomadID,Naziv from Pozorisni_Komad order by KomadID ASC");
            comboBox_Komad.ValueMember = "KomadID";
            foreach (DataRow dr in dt.Rows)
            {
                comboBox_Komad.Items.Add(dr["KomadID"].ToString() + ", " +  dr["Naziv"].ToString());
            }
            comboBox_Komad.SelectedIndex = 0;
        }
        private void PopuniOstalaPolja(int selectedIndex)
        {
            int komadID = selectedIndex + 1;

            //Datum,BrojMesta,CenaKarte
            string queryF = string.Format("select Datum,BrojMesta,CenaKarte from Predstava where KomadID = {0}", komadID);
            DataTable dt = Database.ExecuteQuery(queryF);

            foreach(DataRow dr in dt.Rows)
            {
                //DatumRodjenja
                DateTime datumRodjenja = DateTime.Parse(dr["Datum"].ToString());

                string datumRodjenjaString = datumRodjenja.ToString("dMMyyyy");
                maskedTextBox_Datum.Text = datumRodjenjaString;

                //CenaKarte & BrojMesta
                string cena = dr["CenaKarte"].ToString();
                cena = cena.Replace(" ", "");
                if (cena == "0")
                    cena = "Besplatno";
                textBox_Cena.Text = cena;

                string brojMesta = dr["BrojMesta"].ToString();
                brojMesta = brojMesta.Replace(" ", "");
                textBox_BrojMesta.Text = brojMesta;
            }

            //Producent & Trupa
            string queryS = string.Format("select ProducentID,TrupaID from Predstava");
            dt = Database.ExecuteQuery(queryS);

            foreach(DataRow dr in dt.Rows)
            {
                int producentID = (int)dr["ProducentID"];
                int trupaID = (int)dr["TrupaID"];

                string queryProducent = string.Format("select Ime,Prezime from Producent");
                DataTable producentTable = Database.ExecuteQuery(queryProducent);
                comboBox_Producent.Items.Clear();
                comboBox_Trupa.Items.Clear();

                foreach (DataRow pR in producentTable.Rows)
                {
                    comboBox_Producent.Items.Add(pR["Ime"].ToString() + " " + pR["Prezime"].ToString());
                }
                comboBox_Producent.SelectedIndex = producentID-1;
                string queryTrupa = string.Format("select NazivTrupe from Pozorisna_Trupa");
                DataTable trupaTable = Database.ExecuteQuery(queryTrupa);
                foreach (DataRow tR in trupaTable.Rows)
                {
                    comboBox_Trupa.Items.Add(tR["NazivTrupe"].ToString());
                }
                comboBox_Trupa.SelectedIndex = trupaID-1;
            }


        }

        

    }
}
