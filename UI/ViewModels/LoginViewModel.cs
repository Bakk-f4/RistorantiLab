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
        //manager per la logica di login
        private readonly UtenteManager _manager;

        //evento login
        public event EventHandler<Utente> LoginRiuscito;



        //proprieta' in binding
        private string _userName;
        public string UserName
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

        //comando login
        public ICommand LoginCommand { get; }

        public LoginViewModel(UtenteManager manager)
        {
            _manager = manager
                ?? throw new ArgumentNullException(nameof(manager));

            LoginCommand = new RelayCommand(
                execute: EseguiLogin,
                canExecute: () => !string.IsNullOrWhiteSpace(UserName)
                                  && !string.IsNullOrWhiteSpace(Password));
        }

        private void EseguiLogin()
        {
            try
            {
                MessaggioErrore = null;
                IsLoading = true;

                var utente = _manager.Login(UserName, Password);

                if (utente == null)
                {
                    MessaggioErrore = "Username o password non corretti.";
                    return;
                }

                LoginRiuscito?.Invoke(this, utente);
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
            }
            finally
            {
                IsLoading = false;
            }
        }











    }
}
