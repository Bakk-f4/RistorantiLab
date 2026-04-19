using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDal
{
    public interface IRistoranteDAL
    {
        List<Ristorante> GetAll();
        Ristorante GetById(int id);
        List<Ristorante> GetByCitta(string citta);
        List<Ristorante> GetByTipologia(TipologiaRistorante tipologia);
        List<Ristorante> GetByPrezzo(decimal prezzoMax);
        List<Ristorante> Cerca(string citta, TipologiaRistorante? tipologia, decimal? prezzoMax);
        void Insert(Ristorante ristorante);
        void Update(Ristorante ristorante);
        void Delete(int id);

    }
}
