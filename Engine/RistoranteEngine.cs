using Entity;
using CoreFramework;
using IDal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class RistoranteEngine
    {
        private readonly IRistoranteDAL _dal;

        public RistoranteEngine(IRistoranteDAL dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }

        public List<Ristorante> GetAll()
        {
            return _dal.GetAll();
        }

        public Ristorante GetById(int id)
        {
            if (id <= 0)
                throw new BusinessException("ID ristorante non valido.");

            return _dal.GetById(id);
        }

        public List<Ristorante> GetByCitta(string citta)
        {
            if (string.IsNullOrWhiteSpace(citta))
                throw new BusinessException("Il campo città è obbligatorio.");

            return _dal.GetByCitta(citta);
        }

        public List<Ristorante> GetByTipologia(TipologiaRistorante tipologia)
        {
            return _dal.GetByTipologia(tipologia);
        }

        public List<Ristorante> GetByPrezzo(decimal prezzoMax)
        {
            if (prezzoMax <= 0)
                throw new BusinessException("Il prezzo massimo deve essere maggiore di zero.");

            return _dal.GetByPrezzo(prezzoMax);
        }

        public void Insert(Ristorante ristorante)
        {
            ValidaRistorante(ristorante);
            _dal.Insert(ristorante);
        }

        public void Update(Ristorante ristorante)
        {
            if (ristorante.IDRistorante <= 0)
                throw new BusinessException("ID ristorante non valido per la modifica.");

            ValidaRistorante(ristorante);
            _dal.Update(ristorante);
        }

        public void Delete(int id)
        {
            if (id <= 0)
                throw new BusinessException("ID ristorante non valido.");

            _dal.Delete(id);
        }

        
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
    }
}
