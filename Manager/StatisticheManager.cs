using Engine;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class StatisticheManager
    {

        private readonly StatisticheEngine _engine;

        public StatisticheManager(StatisticheEngine engine)
        {
            _engine = engine
                ?? throw new ArgumentNullException(nameof(engine));
        }

        public int GetTotalePrenotazioni(
            int? idRistorante, DateTime dal, DateTime al)
            => _engine.GetTotalePrenotazioni(idRistorante, dal, al);

        public int GetTotalePersone(
            int? idRistorante, DateTime dal, DateTime al)
            => _engine.GetTotalePersone(idRistorante, dal, al);

        public decimal GetTassoOccupazione(
            int idRistorante, DateTime dal, DateTime al)
            => _engine.GetTassoOccupazione(idRistorante, dal, al);

        public List<StatisticaGiornaliera> GetPerGiorno(
            int? idRistorante, DateTime dal, DateTime al)
            => _engine.GetPerGiorno(idRistorante, dal, al);

    }
}
