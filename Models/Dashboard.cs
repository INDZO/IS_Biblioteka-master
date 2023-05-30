using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Biblioteka.Models
{
    public struct IzdavanjaPoDatumu
    {
        public string Datum { get; set; }
        public int BrojIzdavanja { get; set; }
    }

    class Dashboard : DB.DBConnection
    {
        //Fields and properties
        private DateTime pocetniDatum;
        private DateTime zavrsniDatum;
        private int brojDana;

        public int brojNovihClanova { get; private set; }
        public int brojIzdavanja { get; private set; }

        public List<KeyValuePair<string, int>> NajAutori { get; private set; }
        public List<KeyValuePair<string, int>> KnjigeNiskeZalihe { get; private set; }
        public List<IzdavanjaPoDatumu> IzdavanjaPoVremenskomPeriodu { get; private set; }
        public int UkupanBrojClanova { get; private set; }
        public int UkupanBrojIzdavanja { get; private set; }
        public int UkupanBrojNevracenihKnjiga { get; private set; }

        //Constructor
        public Dashboard() { }

        //Private methods
        private void UzmiUkupneBrojke()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = "SELECT COUNT(CitalacID) FROM Citalac";
                    UkupanBrojClanova = (int)command.ExecuteScalar();

                    command.CommandText = "SELECT COUNT(CitalacID) FROM Na_Citanju";
                    UkupanBrojIzdavanja = (int)command.ExecuteScalar();

                    command.CommandText = @"SELECT COUNT(CitalacID) FROM Na_Citanju
                                             WHERE Datum_vracanja is NULL";
                    UkupanBrojNevracenihKnjiga = (int)command.ExecuteScalar();

                }
            }
        }
        private void Analiza()
        {
            KnjigeNiskeZalihe = new List<KeyValuePair<string, int>>();
            NajAutori = new List<KeyValuePair<string, int>>();

            using(var connection = GetConnection())
            {
                connection.Open();
                using(var command = new SqlCommand())
                {
                    SqlDataReader reader;
                    command.Connection = connection;

                    command.CommandText = @"SELECT TOP 5 A.Ime, A.Prezime, COUNT(N.CitalacID) Q FROM Na_Citanju N
                                            inner join Primerak P on N.PrimerakID = P.PrimerakID
                                            inner join Knjiga_Autor KA on KA.KnjigaID = P.KnjigaID
                                            inner join Autor A on A.AutorID = KA.AutorID
                                            WHERE N.Datum_uzimanja BETWEEN @fromDate AND @toDate
                                            GROUP BY A.Ime, A.Prezime
                                            ORDER BY Q desc";
                    command.Parameters.Add("@fromDate", System.Data.SqlDbType.DateTime).Value = pocetniDatum;
                    command.Parameters.Add("@toDate", System.Data.SqlDbType.DateTime).Value = zavrsniDatum;

                    reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        NajAutori.Add
                            (
                                new KeyValuePair<string, int>(reader[0].ToString() + " " + reader[1].ToString(), (int)reader[2])
                            );
                    }
                    reader.Close();

                    command.CommandText = @"SELECT K.Naziv, COUNT(P.PrimerakID) - COUNT(N.PrimerakID) Q from Knjiga K
                                            inner join Primerak P on P.KnjigaID = K.KnjigaID
                                            left join Na_Citanju N on N.PrimerakID = P.PrimerakID AND N.Datum_vracanja IS NULL
                                            group by K.Naziv
                                            HAVING COUNT(P.PrimerakID) - COUNT(N.PrimerakID) < 10";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        KnjigeNiskeZalihe.Add
                            (
                                new KeyValuePair<string, int>(reader[0].ToString(), (int)reader[1])
                            );
                    }
                    reader.Close();
                }
            }
        }
        private void UzmiIzdavanja()
        {
            IzdavanjaPoVremenskomPeriodu = new List<IzdavanjaPoDatumu>();

            using(var connection = GetConnection())
            {
                connection.Open();
                using(var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT COUNT(CitalacID) FROM Citalac
                                            WHERE Datum_uclanjenja BETWEEN @fromDate AND @toDate";
                    command.Parameters.Add("@fromDate", System.Data.SqlDbType.DateTime).Value = pocetniDatum;
                    command.Parameters.Add("@toDate", System.Data.SqlDbType.DateTime).Value = zavrsniDatum;
                    brojNovihClanova = (int)command.ExecuteScalar();

                    command.CommandText = @"SELECT COUNT(CitalacID) FROM Na_Citanju
                                            WHERE Datum_uzimanja BETWEEN @fromDate1 AND @toDate1";
                    command.Parameters.Add("@fromDate1", System.Data.SqlDbType.DateTime).Value = pocetniDatum;
                    command.Parameters.Add("@toDate1", System.Data.SqlDbType.DateTime).Value = zavrsniDatum;
                    brojIzdavanja = (int)command.ExecuteScalar();

                    command.CommandText = @"SELECT Datum_uzimanja, COUNT(CitalacID) FROM Na_Citanju
                                            WHERE Datum_uzimanja BETWEEN @fromDate2 AND @toDate2
                                            GROUP BY Datum_uzimanja";
                    command.Parameters.Add("@fromDate2", System.Data.SqlDbType.DateTime).Value = pocetniDatum;
                    command.Parameters.Add("@toDate2", System.Data.SqlDbType.DateTime).Value = zavrsniDatum;

                    var reader = command.ExecuteReader();
                    var rezultat = new List<KeyValuePair<DateTime, int>>();
                    while(reader.Read())
                    {
                        rezultat.Add
                            (
                                new KeyValuePair<DateTime, int>((DateTime)reader[0], (int)reader[1])
                            );
                    }
                    reader.Close();

                    if(brojDana <= 1)
                    {
                        IzdavanjaPoVremenskomPeriodu = (from lista in rezultat
                                                        group lista by lista.Key.ToString("hh tt")
                                                       into red
                                                        select new IzdavanjaPoDatumu
                                                        {
                                                            Datum = red.Key,
                                                            BrojIzdavanja = red.Sum(x => x.Value)
                                                        }).ToList();
                    }

                    else if (brojDana <= 30)
                    {
                        IzdavanjaPoVremenskomPeriodu = (from lista in rezultat
                                                        group lista by lista.Key.ToString("dd MMM")
                                                       into red
                                                        select new IzdavanjaPoDatumu
                                                        {
                                                            Datum = red.Key,
                                                            BrojIzdavanja = red.Sum(x => x.Value)
                                                        }).ToList();
                    }
                    else if (brojDana <= 92)
                    {
                        IzdavanjaPoVremenskomPeriodu = (from lista in rezultat
                                                        group lista by CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                                                                       lista.Key, CalendarWeekRule.FirstDay, DayOfWeek.Monday)
                                                       into red
                                                        select new IzdavanjaPoDatumu
                                                        {
                                                            Datum = "Nedelja " + red.Key,
                                                            BrojIzdavanja = red.Sum(x => x.Value)
                                                        }).ToList();
                    }
                    else if(brojDana <= 365 * 2)
                    {
                        bool jelGodina = pocetniDatum.Year == zavrsniDatum.Year ? true : false;
                        IzdavanjaPoVremenskomPeriodu = (from lista in rezultat
                                                        group lista by lista.Key.ToString("MMM yyyy")
                                                     into red
                                                        select new IzdavanjaPoDatumu
                                                        {
                                                            Datum = jelGodina ? red.Key.Substring(0, red.Key.IndexOf(" ")) : red.Key,
                                                            BrojIzdavanja = red.Sum(x => x.Value)
                                                        }).ToList();
                    }
                }
            }

        }
        //Public methods
        public void UcitajPodatke(DateTime pocetniDatum, DateTime zavrsniDatum)
        {
            this.pocetniDatum = pocetniDatum;
            this.zavrsniDatum = zavrsniDatum;
            brojDana = (zavrsniDatum - pocetniDatum).Days;
            UzmiUkupneBrojke();
            Analiza();
            UzmiIzdavanja();
        }
    }
}
