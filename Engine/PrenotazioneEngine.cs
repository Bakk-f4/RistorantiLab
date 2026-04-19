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
    public class PrenotazioneEngine
    {

        private readonly IPrenotazioneDAL _dalPren;
        private readonly IRistoranteDAL _dalRist;
        private readonly IUtenteDAL _dalUtente;


        public PrenotazioneEngine(
            IPrenotazioneDAL dalPren,
            IRistoranteDAL dalRist,
            IUtenteDAL dalUtente)
        {
            _dalPren = dalPren
                ?? throw new ArgumentNullException(nameof(dalPren));
            _dalRist = dalRist
                ?? throw new ArgumentNullException(nameof(dalRist));
            _dalUtente = dalUtente
                ?? throw new ArgumentNullException(nameof(dalUtente));
        }

        public List<Prenotazione> GetByRistorante(int idRistorante)
        {
            if (idRistorante <= 0)
                throw new BusinessException("ID ristorante non valido.");

            var lista = _dalPren.GetByRistorante(idRistorante);

            PopolaNavigazione(lista);

            return lista;
        }


        public List<Prenotazione> GetByRistoranteEData(
            int idRistorante, DateTime data)
        {
            if (idRistorante <= 0)
                throw new BusinessException("ID ristorante non valido.");

            return _dalPren.GetByRistoranteEData(idRistorante, data);
        }

        public List<Prenotazione> GetByPeriodo(DateTime dal, DateTime al)
        {
            if (dal > al)
                throw new BusinessException(
                    "La data di inizio deve essere precedente alla data di fine.");

            var lista = _dalPren.GetByPeriodo(dal, al);
            PopolaNavigazione(lista);
            return lista;
        }



        public int GetPostiDisponibili(int idRistorante, DateTime data)
        {
            if (idRistorante <= 0)
                throw new BusinessException("ID ristorante non valido.");

            var ristorante = _dalRist.GetById(idRistorante);
            if (ristorante == null)
                throw new BusinessException("Ristorante non trovato.");

            //somma le persone di tutte le prenotazioni
            //esistenti per quel ristorante in quella data
            var prenotazioniDelGiorno = _dalPren
                .GetByRistoranteEData(idRistorante, data);

            int postiOccupati = prenotazioniDelGiorno
                .Sum(p => p.NumeroPersone);

            return ristorante.NumPosti - postiOccupati;
        }


        public void Insert(Prenotazione p, Utente utenteCorrente)
        {
            Valida(p);

            if (!utenteCorrente.isAdministrator &&
                p.NomeUtente != utenteCorrente.Username)
                throw new BusinessException(
                    "Non sei autorizzato a creare prenotazioni per altri utenti.");

            //verifica disponibilita' posti prima di inserire
            int disponibili = GetPostiDisponibili(
                p.IDRistorante, p.DataPrenotazione);

            if (p.NumeroPersone > disponibili)
                throw new BusinessException(
                    $"Posti insufficienti. Disponibili: {disponibili}, " +
                    $"richiesti: {p.NumeroPersone}.");

            p.DataRichiesta = DateTime.Now;
            _dalPren.Insert(p);
        }


        public void Update(Prenotazione p, Utente utenteCorrente)
        {
            Valida(p);

            if (p.IDPrenotazione <= 0)
                throw new BusinessException(
                    "ID prenotazione non valido per la modifica.");

            //recupero la prenotazione esistente per verificare il proprietario
            var esistente = _dalPren.GetById(p.IDPrenotazione);
            if (esistente == null)
                throw new BusinessException("Prenotazione non trovata.");

            //controllo autorizzazione:
            //solo il proprietario o un amministratore puo modificare
            if (!utenteCorrente.isAdministrator &&
                esistente.NomeUtente != utenteCorrente.Username)
                throw new BusinessException(
                    "Non sei autorizzato a modificare questa prenotazione.");

            //per il calcolo dei posti disponibili in modifica
            //escludiamo la prenotazione corrente dal conteggio
            //altrimenti i suoi posti verrebbero contati due volte
            var prenotazioniDelGiorno = _dalPren
                .GetByRistoranteEData(p.IDRistorante, p.DataPrenotazione);

            int postiOccupatiEscluso = prenotazioniDelGiorno
                .Where(x => x.IDPrenotazione != p.IDPrenotazione)
                .Sum(x => x.NumeroPersone);

            var ristorante = _dalRist.GetById(p.IDRistorante);
            int disponibili = ristorante.NumPosti - postiOccupatiEscluso;

            if (p.NumeroPersone > disponibili)
                throw new BusinessException(
                    $"Posti insufficienti. Disponibili: {disponibili}, " +
                    $"richiesti: {p.NumeroPersone}.");

            _dalPren.Update(p);
        }


        public void Delete(int id, Utente utenteCorrente)
        {
            if (id <= 0)
                throw new BusinessException(
                    "ID prenotazione non valido.");

            //recupero la prenotazione esistente per verificare il proprietario
            var esistente = _dalPren.GetById(id);
            if (esistente == null)
                throw new BusinessException("Prenotazione non trovata.");

            //controllo autorizzazione:
            //solo il proprietario o un amministratore puo modificare
            if (!utenteCorrente.isAdministrator &&
                esistente.NomeUtente != utenteCorrente.Username)
                throw new BusinessException(
                    "Non sei autorizzato a modificare questa prenotazione.");

            _dalPren.Delete(id);
        }


        private void Valida(Prenotazione p)
        {
            if (p == null)
                throw new BusinessException(
                    "La prenotazione non può essere null.");

            if (p.IDRistorante <= 0)
                throw new BusinessException(
                    "Seleziona un ristorante valido.");

            if (string.IsNullOrWhiteSpace(p.NomeUtente))
                throw new BusinessException(
                    "Seleziona un utente valido.");

            if (p.DataPrenotazione < DateTime.Today)
                throw new BusinessException(
                    "La data prenotazione non può essere nel passato.");

            if (p.NumeroPersone <= 0)
                throw new BusinessException(
                    "Il numero di persone deve essere maggiore di zero.");
        }


        //metodo per popolare le proprieta di navigazione di una lista di prenotazioni
        //usato dalla ui per mostrare i nomi dei ristoranti e degli utenti invece degli id
        private void PopolaNavigazione(List<Prenotazione> lista)
        {
            foreach (var p in lista)
            {
                p.Ristorante = _dalRist.GetById(p.IDRistorante);
                p.Utente = _dalUtente.GetByUserName(p.NomeUtente);
            }
        }











    }
}
