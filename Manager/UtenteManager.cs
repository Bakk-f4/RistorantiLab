using Engine;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class UtenteManager
    {
        private readonly UtenteEngine _engine;

        public UtenteManager(UtenteEngine engine)
        {
            _engine = engine
                ?? throw new ArgumentNullException(nameof(engine));
        }

        public List<Utente> GetAll()
            => _engine.GetAll();

        //login restituisce l'utente se le credenziali sono corrette
        public Utente Login(string userName, string password)
            => _engine.VerificaCredenziali(userName, password);

        //salva gestisce sia Insert che Update in base allo stato dell'utente
        public void Salva(Utente utente, string passwordChiaro, bool isNuovo)
        {
            if (isNuovo)
                _engine.Insert(utente, passwordChiaro);
            else
                _engine.Update(utente, passwordChiaro);
        }

        public void Elimina(string userName)
            => _engine.Delete(userName);
    }
}
