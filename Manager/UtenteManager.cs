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

        public List<Entity.Utente> GetAll()
        {
            return _engine.GetAll();
        }

        public Utente Login(string username, string password)
        {
            return _engine.VerificaCredenziali(username, password);
        }

        public void Salva(Utente utente, string passwordChiaro, bool isNuovo)
        {
            if (isNuovo)
                _engine.Insert(utente, passwordChiaro);
            else
                _engine.Update(utente, passwordChiaro);

        }

        public void Elimina(string username)
        {
            _engine.Delete(username);
        }


    }
}
