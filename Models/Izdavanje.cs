using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS_Biblioteka.Models
{
    public class Izdavanje
    {
        public string NazivKnjige { get; set; }
        public DateTime datumUzimanja { get; set; }
        public DateTime? datumVracanja { get; set; }
    }
}
