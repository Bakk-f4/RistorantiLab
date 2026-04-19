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
    public class UtenteEngine
    {
        private readonly IUtenteDAL _dal;

        public UtenteEngine(IUtenteDAL dal)
        {
            _dal = dal ?? throw new ArgumentNullException(nameof(dal));
        }


        public List<Utente> GetAll()
        {
            return _dal.GetAll();
        }

        public Utente GetByUserName(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new BusinessException("Username non valido.");
            return _dal.GetByUserName(username);
        }

        public void Insert(Utente utente, string passwordClear)
        {
            ValidaUtente(utente);

            if (string.IsNullOrWhiteSpace(passwordClear))
                throw new BusinessException("La password è obbligatoria.");

            //Verifica che lo username non esista già
            if (_dal.GetByUserName(utente.Username) != null)
                throw new BusinessException(
                    $"Username '{utente.Username}' già in uso.");

            //Hash della password prima di salvare
            utente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordClear);

            _dal.Insert(utente);
        }

        public void Update(Utente utente, string passwordChiaro)
        {
            ValidaUtente(utente);

            var esistente = _dal.GetByUserName(utente.Username);
            if (esistente == null)
                throw new BusinessException(
                    $"Utente '{utente.Username}' non trovato.");

            if (!string.IsNullOrWhiteSpace(passwordChiaro))
                utente.PasswordHash = BCrypt.Net.BCrypt
                    .HashPassword(passwordChiaro);
            else
                //si tiene lo stesso hash se non si vuole cambiare password
                utente.PasswordHash = esistente.PasswordHash;

            _dal.Update(utente);
        }

        public void Delete(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new BusinessException("Username obbligatorio.");

            _dal.Delete(userName);
        }


        private void ValidaUtente(Utente u)
        {
            if (u == null)
                throw new BusinessException("L'utente non può essere null.");

            if (string.IsNullOrWhiteSpace(u.Username))
                throw new BusinessException("Lo username è obbligatorio.");

            if (string.IsNullOrWhiteSpace(u.Email))
                throw new BusinessException("L'email è obbligatoria.");
        }


        //! metodo da riguardare !
        public Utente VerificaCredenziali(string username, string passwordClear)
        {
            if (string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(passwordClear))
                throw new BusinessException("Credenziali non valide.");

            var utente = _dal.GetByUserName(username);

            if (utente == null)
                return null;

            //verifica password con BCrypt di Nuget
            bool passwordCorretta = BCrypt.Net.BCrypt
                .Verify(passwordClear, utente.PasswordHash);

            return passwordCorretta ? utente : null;
        }
    }
}
