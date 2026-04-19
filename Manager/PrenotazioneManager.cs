using Engine;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class PrenotazioneManager
    {

        private readonly PrenotazioneEngine _engine;

        public PrenotazioneManager(PrenotazioneEngine engine)
        {
            _engine = engine
                ?? throw new ArgumentNullException(nameof(engine));
        }

        public List<Prenotazione> GetByRistorante(int idRistorante)
            => _engine.GetByRistorante(idRistorante);

        public List<Prenotazione> GetByPeriodo(DateTime dal, DateTime al)
            => _engine.GetByPeriodo(dal, al);

        public int GetPostiDisponibili(int idRistorante, DateTime data)
            => _engine.GetPostiDisponibili(idRistorante, data);

        public void Salva(Prenotazione p, bool isNuova, Utente utenteCorrente)
        {
            if (isNuova)
                _engine.Insert(p, utenteCorrente);
            else
                _engine.Update(p, utenteCorrente);
        }

        public void Elimina(int id, Utente utenteCorrente)
            => _engine.Delete(id, utenteCorrente);






























    }
}
