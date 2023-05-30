using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Biblioteka.Models
{
    public class Knjiga
    {
        public int ID { get; set; }
        public string Naziv { get; set; }
        public int Kolicina { get; set; }
        public string ISBN { get; set; }
    }
}
