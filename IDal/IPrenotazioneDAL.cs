using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDal
{
    public interface IPrenotazioneDAL
    {
        List<Prenotazione> GetByRistorante(int idRistorante);
        List<Prenotazione> GetByRistoranteEData(int idRistorante, DateTime data);
        List<Prenotazione> GetByPeriodo(DateTime dal, DateTime al);
        Prenotazione GetById(int id);


        void Insert(Prenotazione prenotazione);
        void Update(Prenotazione prenotazione);
        void Delete(int id);


        int GetTotalePrenotazioni(int? idRistorante, DateTime dal, DateTime al);
        int GetTotalePersone(int? idRistorante, DateTime dal, DateTime al);
        List<StatisticaGiornaliera> GetPerGiorno(int? idRistorante, DateTime dal, DateTime al);

    }
}
