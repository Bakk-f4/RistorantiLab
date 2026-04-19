using Entity;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UI.Helpers;

namespace UI.ViewModels
{
    public class UtenteDetailViewModel : ViewModelBase
    {
        private readonly UtenteManager _manager;
        private readonly bool _isNuovo;


        //Eventi per navigazione
        public event EventHandler RichiestaTornaLista;




        //proprieta' in Bindings
        private string _userName;
        public string Username
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        private string _passwordChiaro;
        public string PasswordChiaro
        {
            get => _passwordChiaro;
            set { _passwordChiaro = value; OnPropertyChanged(); }
        }

        private bool _isAdministrator;
        public bool isAdministrator
        {
            get => _isAdministrator;
            set { _isAdministrator = value; OnPropertyChanged(); }
        }

        private string _descrizione;
        public string Descrizione
        {
            get => _descrizione;
            set { _descrizione = value; OnPropertyChanged(); }
        }

        private string _telefono;
        public string Telefono
        {
            get => _telefono;
            set { _telefono = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _citta;
        public string Citta
        {
            get => _citta;
            set { _citta = value; OnPropertyChanged(); }
        }

        private string _messaggioErrore;
        public string MessaggioErrore
        {
            get => _messaggioErrore;
            set { _messaggioErrore = value; OnPropertyChanged(); }
        }

        public bool UserNameAbilitato => _isNuovo;

        public string Titolo => _isNuovo
            ? "Nuovo utente"
            : $"Modifica — {Username}";

        public string LabelPassword => _isNuovo
            ? "Password *"
            : "Nuova password (lascia vuoto per non modificare)";




        //comandi
        public ICommand SalvaCommand { get; }
        public ICommand AnnullaCommand { get; }


        //costruttore
        public UtenteDetailViewModel(UtenteManager manager, Utente utente)
        {
            _manager = manager
                ?? throw new ArgumentNullException(nameof(manager));

            _isNuovo = utente == null;

            if (!_isNuovo)
            {
                Username = utente.Username;
                isAdministrator = utente.isAdministrator;
                Descrizione = utente.Descrizione;
                Telefono = utente.Telefono;
                Email = utente.Email;
                Citta = utente.Citta;
            }

            SalvaCommand = new RelayCommand(EseguiSalva);
            AnnullaCommand = new RelayCommand(EseguiAnnulla);
        }






        //logica comandi
        private void EseguiSalva()
        {
            try
            {
                MessaggioErrore = null;

                var u = new Utente
                {
                    Username = Username,
                    isAdministrator = isAdministrator,
                    Descrizione = Descrizione,
                    Telefono = Telefono,
                    Email = Email,
                    Citta = Citta
                };

                _manager.Salva(u, PasswordChiaro, _isNuovo);
                RichiestaTornaLista?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
            }
        }

        private void EseguiAnnulla()
            => RichiestaTornaLista?.Invoke(this, EventArgs.Empty);











    }
}
