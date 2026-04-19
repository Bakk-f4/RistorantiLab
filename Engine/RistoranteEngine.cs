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
    /// <summary>
    /// Classe che utilizza un'istanza di IRistoranteDAL per eseguire operazioni sui ristoranti.
    /// </summary>
    public class RistoranteEngine
    {

        private readonly IRistoranteDAL _dal;

        public RistoranteEngine(IRistoranteDAL dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        /// <summary>
        /// Restituisce tutti i ristoranti presenti nel database.
        /// </summary>
        /// <returns></returns>
        public List<Ristorante> GetAll()
        {
            return _dal.GetAll();
        }


        public List<Ristorante> Cerca(
                string citta,
                TipologiaRistorante? tipologia,
                decimal? prezzoMax)
        {
            // Validazioni sui singoli filtri se presenti
            if (!string.IsNullOrWhiteSpace(citta) && citta.Length < 2)
                throw new BusinessException(
                    "Inserisci almeno 2 caratteri per la ricerca per città.");

            if (prezzoMax.HasValue && prezzoMax.Value < 0)
                throw new BusinessException(
                    "Il prezzo massimo non può essere negativo.");

            return _dal.Cerca(citta, tipologia, prezzoMax);
        }


        /// <summary>
        /// Inserisce un nuovo ristorante nel database. 
        /// Prima di inserire, valida l'oggetto Ristorante.
        /// </summary>
        /// <param name="ristorante"></param>
        public void Insert(Ristorante ristorante)
        {
            ValidaRistorante(ristorante);
            _dal.Insert(ristorante);
        }

        /// <summary>
        /// Aggiorna un ristorante esistente nel database.
        /// Prima di aggiornare, valida l'oggetto Ristorante.
        /// </summary>
        /// <param name="ristorante"></param>
        /// <exception cref="BusinessException"></exception>
        public void Update(Ristorante ristorante)
        {
            if (ristorante.IDRistorante <= 0)
                throw new BusinessException("ID ristorante non valido per la modifica.");

            ValidaRistorante(ristorante);
            _dal.Update(ristorante);
        }


        /// <summary>
        /// Cancella un ristorante dal database in base al suo ID. 
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="BusinessException"></exception>
        public void Delete(int id)
        {
            if (id <= 0)
                throw new BusinessException("ID ristorante non valido.");

            _dal.Delete(id);
        }


        /// <summary>
        /// Metodo per validare un oggetto Ristorante prima di inserirlo o aggiornarlo nel database.
        /// </summary>
        /// <param name="r"></param>
        /// <exception cref="BusinessException"></exception>
        private void ValidaRistorante(Ristorante r)
        {
            if (r == null)
                throw new BusinessException("Il ristorante non può essere null.");

            if (string.IsNullOrWhiteSpace(r.RagioneSociale))
                throw new BusinessException("La ragione sociale è obbligatoria.");

            if (r.NumPosti <= 0)
                throw new BusinessException("Il numero di posti deve essere maggiore di zero.");

            if (r.PrezzoMedio < 0)
                throw new BusinessException("Il prezzo medio non può essere negativo.");
        }


        /// <summary>
        /// Restituisce una lista di ristoranti in base alla città.
        /// Se la città è nulla o vuota, viene sollevata una BusinessException.
        /// </summary>
        /// <param name="citta"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public List<Ristorante> GetByCitta(string citta)
        {
            if (string.IsNullOrWhiteSpace(citta))
                throw new BusinessException("Il campo città è obbligatorio.");
            return _dal.GetByCitta(citta);
        }

        /// <summary>
        /// Restituisce una lista di ristoranti in base alla tipologia.
        /// </summary>
        /// <param name="tipologia"></param>
        /// <returns></returns>
        public List<Ristorante> GetByTipologia(TipologiaRistorante tipologia)
        {
            return _dal.GetByTipologia(tipologia);
        }


        /// <summary>
        /// Restituisce una lista di ristoranti con un prezzo medio inferiore o uguale a quello specificato.
        /// </summary>
        /// <param name="prezzoMax"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public List<Ristorante> GetByPrezzo(decimal prezzoMax)
        {
            if (prezzoMax <= 0)
                throw new BusinessException("Il prezzo deve essere maggiore di zero.");
            return _dal.GetByPrezzo(prezzoMax);
        }


        /// <summary>
        /// Restituisce un ristorante in base al suo ID. 
        /// Se l'ID è minore o uguale a 0, viene sollevata una BusinessException.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        public Ristorante GetById(int id)
        {
            if (id <= 0)
                throw new BusinessException("ID non valido.");
            return _dal.GetById(id);
        }
    }
}
