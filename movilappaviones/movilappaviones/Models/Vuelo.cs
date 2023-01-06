using System;
using System.Collections.Generic;
using System.Text;

namespace movilappaviones.Models
{
    public class Vuelo
    {

        public int Id { get; set; }
        public DateTime Time { get; set; } 
        public string Destination { get; set; } 
        public string Flight { get; set; }
        public int Gate { get; set; } = 0;
        public string Remarks { get; set; } = "AGENDADO";
        public int? Count { get; set; }

    }
}
