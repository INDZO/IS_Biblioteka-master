using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Biblioteka.Models
{
    public class Clan
    {
        public int ID { get; set; }
        public string ImePrezime { get; set; }
        public string MaticniBroj { get; set; }
        public string Adresa { get; set; }
        public DateTime DatumUclanjenja { get; set; }
    }
}
