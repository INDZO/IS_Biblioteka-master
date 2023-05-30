using IS_Biblioteka.DB;
using IS_Biblioteka.Models;
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
    public partial class FormDodavanjeClanova : Form
    {
        private Repozitorijum repozitorijum;
        public FormDodavanjeClanova()
        {
            InitializeComponent();
            repozitorijum = new Repozitorijum();
        }

        private void btnOtkaziDodavanjeClana_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btdDodajClana_Click(object sender, EventArgs e)
        {
            Clan c = new Clan
            {
                ImePrezime = textBox1.Text + " " + textBox2.Text,
                MaticniBroj = textBox3.Text,
                Adresa = textBox4.Text,
                DatumUclanjenja = DateTime.Now.AddDays(-5)
            };
            repozitorijum.DodajClana(c);
            this.Close();
        }
    }
}
