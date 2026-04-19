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
    public class LoginViewModel : ViewModelBase
    {

        private readonly UtenteManager _manager;


        public Utente UtenteAutenticato { get; private set; }

        //Eventi per navigazione
        public EventHandler<Utente> LoginRiuscito;



        //Proprieta' in Bindings
        private string _userName;
        public string Username
        {
            get => _userName;
            set { _userName = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _messaggioErrore;
        public string MessaggioErrore
        {
            get => _messaggioErrore;
            set { _messaggioErrore = value; OnPropertyChanged(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }





        // Comandi
        public ICommand LoginCommand { get; }



        public LoginViewModel(UtenteManager manager)
        {
            _manager = manager
                ?? throw new ArgumentNullException(nameof(manager));
            LoginCommand = new RelayCommand(
                execute: EseguiLogin,
                canExecute: () => !string.IsNullOrEmpty(Username)
                                  && !string.IsNullOrEmpty(Password));
        }


        private void EseguiLogin()
        {
            try
            {
                MessaggioErrore = null;
                IsLoading = true;

                var utente = _manager.Login(Username, Password);

                if (utente == null)
                {
                    MessaggioErrore = "Username o password non corretti.";
                    return;
                }
                UtenteAutenticato = utente;
                LoginRiuscito?.Invoke(this, utente);
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
            }
            //in questo modo isLoading torna sempre a false
            finally
            {
                IsLoading = false;
            }
        }



    }
}
