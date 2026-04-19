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
    public class UtenteListViewModel : ViewModelBase
    {

        private readonly UtenteManager _manager;


        //Eventi per navigazione
        public event EventHandler RichiestaNuovo;
        public event EventHandler<Utente> RichiestaModifica;




        //Proprieta' in Bindings
        private List<Utente> _listaUtenti;
        public List<Utente> ListaUtenti
        {
            get => _listaUtenti;
            set { _listaUtenti = value; OnPropertyChanged(); }
        }

        private Utente _utenteSelezionato;
        public Utente UtenteSelezionato
        {
            get => _utenteSelezionato;
            set { _utenteSelezionato = value; OnPropertyChanged(); }
        }

        private string _messaggioErrore;
        public string MessaggioErrore
        {
            get => _messaggioErrore;
            set { _messaggioErrore = value; OnPropertyChanged(); }
        }

        private string _messaggioSuccesso;
        public string MessaggioSuccesso
        {
            get => _messaggioSuccesso;
            set { _messaggioSuccesso = value; OnPropertyChanged(); }
        }





        //Comandi
        public ICommand CaricaCommand { get; }
        public ICommand NuovoCommand { get; }
        public ICommand ModificaCommand { get; }
        public ICommand EliminaCommand { get; }



        //Costruttore
        public UtenteListViewModel(UtenteManager manager)
        {
            _manager = manager
                ?? throw new ArgumentNullException(nameof(manager));

            CaricaCommand = new RelayCommand(EseguiCarica);
            NuovoCommand = new RelayCommand(EseguiNuovo);
            ModificaCommand = new RelayCommand(
                execute: EseguiModifica,
                canExecute: () => UtenteSelezionato != null);
            EliminaCommand = new RelayCommand(
                execute: EseguiElimina,
                canExecute: () => UtenteSelezionato != null);

            EseguiCarica();
        }





        //Logica Comandi
        private void EseguiCarica()
        {
            try
            {
                MessaggioErrore = null;
                MessaggioSuccesso = null;
                ListaUtenti = _manager.GetAll();
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
                ListaUtenti = new List<Utente>();
            }
        }

        private void EseguiNuovo()
            => RichiestaNuovo?.Invoke(this, EventArgs.Empty);

        private void EseguiModifica()
            => RichiestaModifica?.Invoke(this, UtenteSelezionato);

        private void EseguiElimina()
        {
            try
            {
                MessaggioErrore = null;
                MessaggioSuccesso = null;
                _manager.Elimina(UtenteSelezionato.Username);
                MessaggioSuccesso = $"Utente '" +
                    $"{UtenteSelezionato.Username}'" +
                    $" eliminato con successo.";
                EseguiCarica();
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
            }
        }













    }
}
