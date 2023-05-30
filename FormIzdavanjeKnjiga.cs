using IS_Biblioteka.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IS_Biblioteka
{
    public partial class FormIzdavanjeKnjiga : Form
    {
        private Repozitorijum repozitorijum;
        private int ClanID;
        public FormIzdavanjeKnjiga(int ClanID)
        {
            InitializeComponent();
            repozitorijum = new Repozitorijum();
            dtgKnjigeIzdavanje.DataSource = repozitorijum.UzmiSveKnjige();
            this.ClanID = ClanID;
        }

        private void btnOtkaziIzdavanjeKnjige_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonIzdajKnjigu_Click(object sender, EventArgs e)
        {
            if(dtgKnjigeIzdavanje.SelectedRows.Count >0)
            {
                repozitorijum.IzdajKnjigu(ClanID, (int)dtgKnjigeIzdavanje.SelectedRows[0].Cells[0].Value, DateTime.Now);
                this.Close();
            }
        }

        private void btnPretraziIzdavanja_Click(object sender, EventArgs e)
        {
            dtgKnjigeIzdavanje.DataSource = repozitorijum.FiltrirajKnjige(textBox3_1.Text);
        }
    }
}
