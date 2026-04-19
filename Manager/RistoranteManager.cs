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

        /// <summary>
        /// Restituisce una lista di ristoranti filtrata in base ai parametri specificati. 
        /// Se un parametro è null o vuoto, non viene applicato come filtro.
        /// </summary>
        /// <param name="citta"></param>
        /// <param name="tipologia"></param>
        /// <param name="prezzoMax"></param>
        /// <returns></returns>
        public List<Ristorante> Cerca(
            string citta,
            TipologiaRistorante? tipologia,
            decimal? prezzoMax)
        {
            return _engine.Cerca(citta, tipologia, prezzoMax);
        }

        /// <summary>
        /// Salva un ristorante. 
        /// Se l'ID è 0, viene inserito come nuovo record; 
        /// altrimenti, viene aggiornato il record esistente. 
        /// </summary>
        /// <param name="ristorante"></param>
        public void Salva(Ristorante ristorante)
        {
            if (ristorante.IDRistorante == 0)
                _engine.Insert(ristorante);
            else
                _engine.Update(ristorante);
        }

        /// <summary>
        /// Elimina un ristorante in base all'ID.
        /// </summary>
        /// <param name="id"></param>
        public void Elimina(int id)
        {
            _engine.Delete(id);
        }






    }
}
