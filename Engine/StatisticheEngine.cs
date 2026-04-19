using CoreFramework;
using Entity;
using IDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class StatisticheEngine
    {
        private readonly IPrenotazioneDAL _dalPren;
        private readonly IRistoranteDAL _dalRist;

        public StatisticheEngine(
            IPrenotazioneDAL dalPren,
            IRistoranteDAL dalRist)
        {
            _dalPren = dalPren
                ?? throw new ArgumentNullException(nameof(dalPren));
            _dalRist = dalRist
                ?? throw new ArgumentNullException(nameof(dalRist));
        }

        public int GetTotalePrenotazioni(
            int? idRistorante, DateTime dal, DateTime al)
        {
            ValidaPeriodo(dal, al);
            return _dalPren.GetTotalePrenotazioni(idRistorante, dal, al);
        }

        public int GetTotalePersone(
            int? idRistorante, DateTime dal, DateTime al)
        {
            ValidaPeriodo(dal, al);
            return _dalPren.GetTotalePersone(idRistorante, dal, al);
        }

        public decimal GetTassoOccupazione(
            int idRistorante, DateTime dal, DateTime al)
        {
            ValidaPeriodo(dal, al);

            var ristorante = _dalRist.GetById(idRistorante);
            if (ristorante == null)
                throw new BusinessException("Ristorante non trovato.");

            //+1 perche' prenotare dal 1 al 3, sono 3 giorni, non 2
            int giorniPeriodo = (int)(al - dal).TotalDays + 1;
            int postiTotali = ristorante.NumPosti * giorniPeriodo;
            if (postiTotali == 0) return 0;

            int personeReali = _dalPren
                .GetTotalePersone(idRistorante, dal, al);

            return Math.Round(
                (decimal)personeReali / postiTotali * 100, 1);
        }

        public List<StatisticaGiornaliera> GetPerGiorno(
            int? idRistorante, DateTime dal, DateTime al)
        {
            ValidaPeriodo(dal, al);
            return _dalPren.GetPerGiorno(idRistorante, dal, al);
        }

        private void ValidaPeriodo(DateTime dal, DateTime al)
        {
            if (dal > al)
                throw new BusinessException(
                    "La data inizio deve essere precedente alla data fine.");
        }


    }
}
