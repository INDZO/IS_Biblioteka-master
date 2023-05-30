using IS_Biblioteka.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Biblioteka.DB
{
    public class Repozitorijum : DBConnection
    {

        public Repozitorijum() { }

        public List<Clan> UzmiSveClanove()
        {
            List<Clan> r = new List<Clan>();
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT CitalacID, Ime, Prezime, Maticni_broj, Adresa, Datum_uclanjenja FROM Citalac";
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        r.Add(new Clan
                        {
                            ID = (int)reader[0],
                            ImePrezime = reader[1] + " " + reader[2],
                            MaticniBroj = reader[3].ToString(),
                            Adresa = reader[4].ToString(),
                            DatumUclanjenja = (DateTime)reader[5]
                        });
                    }

                    reader.Close();
                }
            }
            return r;
        }
        public List<Clan> FiltrirajClanove(string filter)
        {
            List<Clan> r = new List<Clan>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT CitalacID, Ime, Prezime, Maticni_broj, Adresa, Datum_uclanjenja FROM Citalac
                                            WHERE (Ime LIKE(CONCAT(CONCAT('%', @filter), '%' )) OR Prezime LIKE(CONCAT(CONCAT('%', @filter), '%' ) ))
                                            OR (Maticni_broj LIKE(CONCAT(CONCAT('%', @filter), '%' )) OR Adresa LIKE(CONCAT(CONCAT('%', @filter), '%' )))";
                    command.Parameters.Add("@filter", System.Data.SqlDbType.NVarChar).Value = filter;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        r.Add(new Clan
                        {
                            ID = (int)reader[0],
                            ImePrezime = reader[1] + " " + reader[2],
                            MaticniBroj = reader[3].ToString(),
                            Adresa = reader[4].ToString(),
                            DatumUclanjenja = (DateTime)reader[5]
                        });
                    }

                    reader.Close();
                }
            }

            return r;
        }
        public void DodajClana(Clan c)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"INSERT INTO Citalac (Ime, Prezime, Maticni_broj, Adresa, Datum_uclanjenja)
                                            VALUES (@Ime, @Prezime, @Maticni_broj, @Adresa, @Datum_uclanjenja)";
                    command.Parameters.Add("@Ime", System.Data.SqlDbType.NVarChar).Value = c.ImePrezime.Split(' ')[0];
                    command.Parameters.Add("@Prezime", System.Data.SqlDbType.NVarChar).Value = c.ImePrezime.Split(' ')[1];
                    command.Parameters.Add("@Maticni_broj", System.Data.SqlDbType.NVarChar).Value = c.MaticniBroj;
                    command.Parameters.Add("@Adresa", System.Data.SqlDbType.NVarChar).Value = c.Adresa;
                    command.Parameters.Add("@Datum_uclanjenja", System.Data.SqlDbType.DateTime).Value = c.DatumUclanjenja;

                    command.ExecuteScalar();
                }
            }
        }
        public string UzmiImeClana(int ClanID)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT CONCAT(Ime,CONCAT(' ',Prezime)) From Citalac
                                            WHERE CitalacID = @CitalacID";
                    command.Parameters.Add("@CitalacID", System.Data.SqlDbType.Int).Value = ClanID;

                    return command.ExecuteScalar().ToString();

                }
            }
        }
        public List<Knjiga> UzmiSveKnjige()
        {
            List<Knjiga> r = new List<Knjiga>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT K.KnjigaID, K.Naziv, K.ISBN, COUNT(P.PrimerakID) - COUNT(N.PrimerakID) Q from Knjiga K
                                            inner join Primerak P on P.KnjigaID = K.KnjigaID
                                            left join Na_Citanju N on N.PrimerakID = P.PrimerakID AND N.Datum_vracanja IS NULL
                                            group by K.KnjigaID, K.Naziv, K.ISBN
                                            HAVING COUNT(P.PrimerakID) - COUNT(N.PrimerakID) > 0";

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        r.Add(new Knjiga
                        {
                            ID = (int)reader[0],
                            Naziv = reader[1].ToString(),
                            Kolicina = (int)reader[3],
                            ISBN = reader[2].ToString()

                        });
                    }
                    reader.Close();
                }
            }

            return r;
        }
        public void IzdajKnjigu(int KnjigaID, int ClanID, DateTime datum)
        {
            int PrimerakID;
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT TOP 1 P.PrimerakID FROM Primerak P
                                            inner join Knjiga K on K.KnjigaID = P.KnjigaID
                                            WHERE K.KnjigaID = @KnjigaID";
                    command.Parameters.Add("@KnjigaID", System.Data.SqlDbType.Int).Value = KnjigaID;
                    PrimerakID = (int)command.ExecuteScalar();

                    command.CommandText = @"INSERT INTO Na_Citanju (PrimerakID, CitalacID, Datum_uzimanja, Datum_vracanja)
                                            VALUES (@PrimerakID, @CitalacID, @Datum_uzimanja,NULL)";
                    command.Parameters.Add("@PrimerakID", System.Data.SqlDbType.Int).Value = PrimerakID;
                    command.Parameters.Add("@CitalacID", System.Data.SqlDbType.Int).Value = ClanID;
                    command.Parameters.Add("@Datum_uzimanja", System.Data.SqlDbType.DateTime).Value = datum;

                    command.ExecuteScalar();
                }

            }
        }
        public List<Knjiga> FiltrirajKnjige(string filter)
        {
            List<Knjiga> r = new List<Knjiga>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT K.KnjigaID, K.Naziv, K.ISBN, COUNT(P.PrimerakID) - COUNT(N.PrimerakID) Q from Knjiga K
                                            inner join Primerak P on P.KnjigaID = K.KnjigaID
                                            left join Na_Citanju N on N.PrimerakID = P.PrimerakID AND N.Datum_vracanja IS NULL
                                            group by K.KnjigaID, K.Naziv, K.ISBN
                                            HAVING COUNT(P.PrimerakID) - COUNT(N.PrimerakID) > 0 AND
                                            (K.Naziv Like(CONCAT('%', CONCAT(@filter1, '%'))) OR K.ISBN Like(CONCAT('%', CONCAT(@filter1, '%'))))";

                    command.Parameters.Add("@filter1", System.Data.SqlDbType.NVarChar).Value = filter;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        r.Add(new Knjiga
                        {
                            ID = (int)reader[0],
                            Naziv = reader[1].ToString(),
                            Kolicina = (int)reader[3],
                            ISBN = reader[2].ToString()

                        });
                    }
                    reader.Close();
                }
            }

            return r;
        }
        public void DodajKnjigu(string naziv, string ISBN, int AutorID, int IzdavacID)
        {
            using(var connection = GetConnection())
            {
                connection.Open();
                using(var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = @"INSERT INTO Knjiga (Naziv, ISBN)
                                            VALUES (@Naziv, @ISBN)";
                    command.Parameters.Add("@Naziv", System.Data.SqlDbType.NVarChar).Value = naziv;
                    command.Parameters.Add("@ISBN", System.Data.SqlDbType.NVarChar).Value = ISBN;
                    command.ExecuteScalar();

                    command.CommandText = @"SELECT KnjigaID from Knjiga
                                            WHERE ISBN = @ISBN1";
                    command.Parameters.Add("@ISBN1", System.Data.SqlDbType.NVarChar).Value = ISBN;
                    int KnjigaID = (int)command.ExecuteScalar();

                    command.CommandText = @"INSERT INTO Knjiga_Autor(KnjigaID, AutorID)
                                            VALUES( @KnjigaID, @AutorID)";
                    command.Parameters.Add("@KnjigaID", System.Data.SqlDbType.Int).Value = KnjigaID;
                    command.Parameters.Add("@AutorID", System.Data.SqlDbType.Int).Value = AutorID;
                    command.ExecuteScalar();

                    command.CommandText = @"INSERT INTO Knjiga_Izdavac(KnjigaID, IzdavacID)
                                            VALUES( @KnjigaID1, @IzdavacID)";
                    command.Parameters.Add("@KnjigaID1", System.Data.SqlDbType.Int).Value = KnjigaID;
                    command.Parameters.Add("@IzdavacID", System.Data.SqlDbType.Int).Value = IzdavacID;
                    command.ExecuteScalar();

                    command.CommandText = @"DECLARE @i INTEGER = 1
                                            WHILE @i <= 10
                                            	BEGIN
                                            		SET @i = @i + 1
                                            		INSERT INTO Primerak (KnjigaID)
                                            		VALUES(@KnjigaID2)
                                            	END;";
                    command.Parameters.Add("@KnjigaID2", System.Data.SqlDbType.Int).Value = KnjigaID;
                    command.ExecuteScalar();

                }
            }
        }
        public List<Izdavanje> UzmiIzdavanja(int ClanID)
        {
            List<Izdavanje> r = new List<Izdavanje>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT K.Naziv, N.Datum_uzimanja, N.Datum_vracanja FROM Na_Citanju N
                                            inner join Primerak P on P.PrimerakID = N.PrimerakID
                                            inner join Knjiga K on K.KnjigaID = P.KnjigaID
                                            WHERE N.CitalacID = @CitalacID";
                    command.Parameters.Add("@CitalacID", System.Data.SqlDbType.Int).Value = ClanID;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        r.Add(new Izdavanje
                        {
                            NazivKnjige = reader[0].ToString(),
                            datumUzimanja = (DateTime)reader[1],
                            datumVracanja = (DateTime?)(reader.IsDBNull(2) ? null : reader[2])
                        });
                    }
                    reader.Close();
                }
            }

            return r;
        }
        public void VratiKnjigu(int ClanID, DateTime datumUzimanja)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"UPDATE Na_Citanju
                                            SET Datum_vracanja = @Datum_Vracanja
                                            WHERE CitalacID = @CitalacID AND Datum_uzimanja = @Datum_uzimanja";
                    command.Parameters.Add("@CitalacID", System.Data.SqlDbType.Int).Value = ClanID;
                    command.Parameters.Add("@Datum_Vracanja", System.Data.SqlDbType.DateTime).Value = DateTime.Now;
                    command.Parameters.Add("@Datum_uzimanja", System.Data.SqlDbType.DateTime).Value = datumUzimanja;
                    command.ExecuteScalar();
                }
            }
        }
        public List<Izdavanje> FiltrirajIzdavanja(int ClanID, string filter)
        {
            List<Izdavanje> r = new List<Izdavanje>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT K.Naziv, N.Datum_uzimanja, N.Datum_vracanja FROM Na_Citanju N
                                            inner join Primerak P on P.PrimerakID = N.PrimerakID
                                            inner join Knjiga K on K.KnjigaID = P.KnjigaID
                                            WHERE N.CitalacID = @CitalacID  AND 
                                            ( K.Naziv Like(CONCAT('%', CONCAT(@filter1, '%'))) OR N.Datum_uzimanja Like(CONCAT('%', CONCAT(@filter1, '%'))) 
                                            OR N.Datum_vracanja Like(CONCAT('%', CONCAT(@filter1, '%'))))";
                    command.Parameters.Add("@CitalacID", System.Data.SqlDbType.Int).Value = ClanID;
                    command.Parameters.Add("@filter1", System.Data.SqlDbType.NVarChar).Value = filter;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        r.Add(new Izdavanje
                        {
                            NazivKnjige = reader[0].ToString(),
                            datumUzimanja = (DateTime)reader[1],
                            datumVracanja = (DateTime?)(reader.IsDBNull(2) ? null : reader[2])
                        });
                    }
                    reader.Close();
                }
            }

            return r;
        }
        public List<Autor> UzmiSveAutore()
        {
            List<Autor> r = new List<Autor>();

            using(var connection = GetConnection())
            {
                connection.Open();
                using(var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT AutorID, CONCAT(Ime, CONCAT(' ', Prezime)) ImePrezime, Adresa FROM Autor";

                    var reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        r.Add(new Autor
                        {
                            ID = (int)reader[0],
                            ImePrezime = reader[1].ToString(),
                            Adresa = reader[2].ToString()
                        }) ;
                    }
                }
            }

            return r;
        }

        public List<Autor> FiltrirajAutore(string filter)
        {
            List<Autor> r = new List<Autor>();

            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT AutorID, CONCAT(Ime, CONCAT(' ', Prezime)) ImePrezime, Adresa FROM Autor
                                            WHERE Ime LIKE(CONCAT('%', CONCAT(@filter, '%'))) OR Prezime LIKE(CONCAT('%', CONCAT(@filter, '%')))
                                            OR Adresa LIKE(CONCAT('%', CONCAT(@filter, '%')))";
                    command.Parameters.Add("@filter", System.Data.SqlDbType.NVarChar).Value = filter;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        r.Add(new Autor
                        {
                            ID = (int)reader[0],
                            ImePrezime = reader[1].ToString(),
                            Adresa = reader[2].ToString()
                        });
                    }
                }
            }

            return r;
        }
        public List<Izdavac> UzmiSveIzdavace()
        {
            List<Izdavac> r = new List<Izdavac>();

            using(var connection = GetConnection())
            {
                connection.Open();
                using(var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM Izdavac";

                    var reader = command.ExecuteReader();
                    while(reader.Read())
                    {
                        r.Add(new Izdavac
                        {
                            ID = (int)reader[0],
                            Naziv = reader[1].ToString(),
                            Adresa = reader[2].ToString()
                        });
                    }
                    
                }
            }

            return r;
        }
        public List<Izdavac> FiltrirajIzdavace(string filter)
        {
            List<Izdavac> r = new List<Izdavac>();

            using(var connection = GetConnection())
            {
                connection.Open();
                using(var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT * FROM Izdavac
                                            WHERE NazivIzdavaca LIKE(CONCAT('%', CONCAT(@filter, '%'))) OR Adresa LIKE(CONCAT('%', CONCAT(@filter, '%')))";
                    command.Parameters.Add("@filter", System.Data.SqlDbType.NVarChar).Value = filter;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        r.Add(new Izdavac
                        {
                            ID = (int)reader[0],
                            Naziv = reader[1].ToString(),
                            Adresa = reader[2].ToString()
                        });
                    }

                }
            }

            return r;
        }
    }
}
