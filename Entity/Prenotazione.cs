using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Prenotazione
    {

        public int IDPrenotazione { get; set; }
        public int IDRistorante { get; set; }
        public string NomeUtente { get; set; }
        public DateTime DataRichiesta { get; set; }
        public DateTime DataPrenotazione { get; set; }
        public int NumeroPersone { get; set; }

        //proprieta di navigazione
        public Ristorante Ristorante { get; set; }
        public Utente Utente { get; set; }



    }
}
