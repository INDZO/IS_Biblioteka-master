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
    public partial class FormPregledClanova: Form
    {
        private Repozitorijum repozitorijum;
        private int ClanID;
        public FormPregledClanova(int ClanID)
        {
            InitializeComponent();
            repozitorijum = new Repozitorijum();
            dtgKnjigeIzdavanje.DataSource = repozitorijum.UzmiIzdavanja(ClanID);
            label1.Text = repozitorijum.UzmiImeClana(ClanID);
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
                if(dtgKnjigeIzdavanje.SelectedRows[0].Cells[2].Value is null)
                {
                    repozitorijum.VratiKnjigu(ClanID, (DateTime)dtgKnjigeIzdavanje.SelectedRows[0].Cells[1].Value);
                    this.Close();
                }
            }
        }

        private void btnPretraziIzdavanje_Click(object sender, EventArgs e)
        {
            dtgKnjigeIzdavanje.DataSource = repozitorijum.FiltrirajIzdavanja(ClanID, textBox3_1.Text);
        }
    }
}
