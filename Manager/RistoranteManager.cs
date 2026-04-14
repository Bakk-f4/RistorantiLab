using Engine;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class RistoranteManager
    {
        private readonly RistoranteEngine _engine;

        public RistoranteManager(RistoranteEngine engine)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        public List<Ristorante> GetAll()
        {
            return _engine.GetAll();
        }

        public List<Ristorante> Cerca(
            string citta,
            TipologiaRistorante? tipologia,
            decimal? prezzoMax)
        {
            //Se sono stati specificati filtri, li applica in sequenza
            if (!string.IsNullOrWhiteSpace(citta))
                return _engine.GetByCitta(citta);

            if (tipologia.HasValue)
                return _engine.GetByTipologia(tipologia.Value);

            if (prezzoMax.HasValue && prezzoMax.Value > 0)
                return _engine.GetByPrezzo(prezzoMax.Value);

            return _engine.GetAll();
        }

        public void Salva(Ristorante ristorante)
        {
            if (ristorante.IDRistorante == 0)
                _engine.Insert(ristorante);
            else
                _engine.Update(ristorante);
        }

        public void Elimina(int id)
        {
            _engine.Delete(id);
        }
    }
}
