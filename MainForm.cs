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
    public partial class MainForm : Form
    {
        private Dashboard model;
        private Repozitorijum repozitorijum;
        private Form activeForm;
        private Button currentButton;
        public MainForm()
        {
            InitializeComponent();
            model = new Dashboard();
            repozitorijum = new Repozitorijum();
            btnOvogMeseca_Click(btnOvogMeseca, new EventArgs());
            dataGridView2.DataSource = repozitorijum.UzmiSveClanove();
            dtgKnjige.DataSource = repozitorijum.UzmiSveKnjige();
            dtgAutori.DataSource = repozitorijum.UzmiSveAutore();
            dtgIzdavaci.DataSource = repozitorijum.UzmiSveIzdavace();
        }

        private void Ucitaj(DateTime pocetniDatum, DateTime zavrsniDatum)
        {
            model.UcitajPodatke(pocetniDatum, zavrsniDatum);
            lblBrojNovihClanova.Text = model.brojNovihClanova.ToString();
            lblBrojIzdavanja.Text = model.brojIzdavanja.ToString();

            chartBrojIzdavanja.DataSource = model.IzdavanjaPoVremenskomPeriodu;
            chartBrojIzdavanja.Series[0].XValueMember = "Datum";
            chartBrojIzdavanja.Series[0].YValueMembers = "BrojIzdavanja";
            chartBrojIzdavanja.DataBind();

            chartNajpopularnijiAutori.DataSource = model.NajAutori;
            chartNajpopularnijiAutori.Series[0].XValueMember = "Key";
            chartNajpopularnijiAutori.Series[0].YValueMembers = "Value";
            chartNajpopularnijiAutori.DataBind();

            lblUkupanBrojClanova.Text = model.UkupanBrojClanova.ToString();
            lblUkupanBrojIzdavanja.Text = model.UkupanBrojIzdavanja.ToString();
            lblUkupanBrojNevracenihKnjiga.Text = model.UkupanBrojNevracenihKnjiga.ToString();

            dataGridView1.DataSource = model.KnjigeNiskeZalihe;
        }
        private void OnemoguciCustomDatume()
        {
            dtpPocetniDatum.Enabled = false;
            dtpZavrsniDatum.Enabled = false;
            btnOk.Enabled = false;
            btnOk.Visible = false;
        }
        private void OtvoriFormu(Form form, Panel panel)
        {
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Width = panel.Width;
            form.Height = panel.Height;
            form.Dock = DockStyle.Fill;
            panel.Controls.Add(form);
            panel.Tag = form;
            form.BringToFront();
            form.Show();
        }
        private void btnOvogMeseca_Click(object sender, EventArgs e)
        {
            Ucitaj(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), DateTime.Now);
            OnemoguciCustomDatume();
            SetDateMenuButtonUI(sender);
        }

        private void btnDanas_Click(object sender, EventArgs e)
        {
            Ucitaj(DateTime.Today, DateTime.Now);
            OnemoguciCustomDatume();
            SetDateMenuButtonUI(sender);
        }

        private void btnOveNedelje_Click(object sender, EventArgs e)
        {
            Ucitaj(DateTime.Today.AddDays(-7), DateTime.Now);
            OnemoguciCustomDatume();
            SetDateMenuButtonUI(sender);
        }

        private void btnCustom_Click(object sender, EventArgs e)
        {
            dtpPocetniDatum.Enabled = true;
            dtpZavrsniDatum.Enabled = true;
            btnOk.Enabled = true;
            btnOk.Visible = true;
            SetDateMenuButtonUI(sender);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Ucitaj(dtpPocetniDatum.Value, dtpZavrsniDatum.Value);
        }

        private void btnPretraziClanove_Click(object sender, EventArgs e)
        {
            dataGridView2.DataSource = repozitorijum.FiltrirajClanove(textBox2.Text);
        }

        private void btnDodajClana_Click(object sender, EventArgs e)
        {
            OtvoriFormu(new FormDodavanjeClanova(), panel2_1);
        }

        private void button2_3_Click(object sender, EventArgs e)
        {
            if(dataGridView2.SelectedRows.Count > 0)
            {
                OtvoriFormu(new FormIzdavanjeKnjiga((int)dataGridView2.SelectedRows[0].Cells[0].Value), panel2_1);
            }
        }

        private void button2_4_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                OtvoriFormu(new FormPregledClanova((int)dataGridView2.SelectedRows[0].Cells[0].Value), panel2_1);
            }
        }

        private void btnPretraziKnjige_Click(object sender, EventArgs e)
        {
            dtgKnjige.DataSource = repozitorijum.FiltrirajKnjige(txtBoxKnjige.Text);
        }

        private void btnDodajKnjigu_Click(object sender, EventArgs e)
        {
            FormDodavanjeKnjige f = new FormDodavanjeKnjige();
            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Dock = DockStyle.Fill;
            tabPage3.Controls.Add(f);
            tabPage3.Tag = f;
            f.BringToFront();
            f.Show();
        }

        private void btnPretraziIzdavace_Click(object sender, EventArgs e)
        {
            dtgIzdavaci.DataSource = repozitorijum.FiltrirajIzdavace(textBox1.Text);
        }

        private void btnPretraziAutore_Click(object sender, EventArgs e)
        {
            dtgAutori.DataSource = repozitorijum.FiltrirajAutore(textBox3_1.Text);
        }

        private void SetDateMenuButtonUI(object button)
        {
            var btn = (Button)button;
            btn.BackColor = btnOvogMeseca.FlatAppearance.BorderColor;
            btn.ForeColor = Color.White;

            if(currentButton != null && currentButton != btn)
            {
                currentButton.BackColor = Color.FromArgb(24, 28, 63);
                currentButton.ForeColor = Color.FromArgb(124, 141, 181);
            }
            currentButton = btn;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns[1].Width = 75;
            dataGridView1.Columns[0].HeaderText = "Naziv";
            dataGridView1.Columns[1].HeaderText = "Količina";

            dataGridView2.Columns[0].HeaderText = "Br članske karte";
            dataGridView2.Columns[1].HeaderText = "Ime i Prezime";
            dataGridView2.Columns[2].HeaderText = "Matični broj";
            dataGridView2.Columns[4].HeaderText = "Datum učlanjenja";
        }

    }
}
