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
            => _dal.GetAll();

        public Utente GetByUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new BusinessException("Username obbligatorio.");

            return _dal.GetByUserName(userName);
        }

        //verificaCredenziali usato dal Login
        public Utente VerificaCredenziali(string userName, string passwordChiaro)
        {
            if (string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(passwordChiaro))
                throw new BusinessException(
                    "Username e password sono obbligatori.");

            var utente = _dal.GetByUserName(userName);

            if (utente == null)
                return null;

            //BCrypt.Verify confronta la password in chiaro
            //con l'hash salvato nel DB non decripta, ricalcola
            bool passwordCorretta = BCrypt.Net.BCrypt
                .Verify(passwordChiaro, utente.PasswordHash);

            return passwordCorretta ? utente : null;
        }

        public void Insert(Utente utente, string passwordChiaro)
        {
            ValidaUtente(utente);

            if (string.IsNullOrWhiteSpace(passwordChiaro))
                throw new BusinessException("La password è obbligatoria.");

            //verifica che lo username non esista già
            if (_dal.GetByUserName(utente.UserName) != null)
                throw new BusinessException(
                    $"Username '{utente.UserName}' già in uso.");

            //hash della password prima di salvare
            utente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordChiaro);

            _dal.Insert(utente);
        }

        //update con password opzionale:
        //se passwordChiaro e' null o vuota, non aggiorna la password
        public void Update(Utente utente, string passwordChiaro)
        {
            ValidaUtente(utente);

            var esistente = _dal.GetByUserName(utente.UserName);
            if (esistente == null)
                throw new BusinessException(
                    $"Utente '{utente.UserName}' non trovato.");

            if (!string.IsNullOrWhiteSpace(passwordChiaro))
                utente.PasswordHash = BCrypt.Net.BCrypt
                    .HashPassword(passwordChiaro);
            else
                //mantieni l'hash esistente se non si vuole cambiare password
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

            if (string.IsNullOrWhiteSpace(u.UserName))
                throw new BusinessException("Lo username è obbligatorio.");

            if (string.IsNullOrWhiteSpace(u.Email))
                throw new BusinessException("L'email è obbligatoria.");
        }
    }
}
