using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{

    public enum TipologiaRistorante
    {
        Pizzeria = 1,
        Pesce = 2,
        Carne = 3,
        Vegetariano = 4,
        CucinaLocale = 5
    }

    public class Ristorante
    {
        public int IDRistorante { get; set; }
        public string RagioneSociale { get; set; }
        public string PartitaIVA { get; set; }
        public string Indirizzo { get; set; }
        public string Citta { get; set; }
        public string Telefono { get; set; }
        public TipologiaRistorante Tipologia { get; set; }
        public int NumPosti { get; set; }
        public decimal PrezzoMedio { get; set; }
    }
}
