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
    public class RistoranteListViewModel : ViewModelBase
    {
        private readonly RistoranteManager _manager;

        //Eventi per navigazione
        public event EventHandler RichiestaNuovo;
        public event EventHandler<Ristorante> RichiestaModifica;


        //Proprieta' in Bindings
        private List<Ristorante> _listaRistoranti;
        public List<Ristorante> ListaRistoranti
        {
            get => _listaRistoranti;
            set
            {
                _listaRistoranti = value;
                OnPropertyChanged();
            }
        }

        private Ristorante _ristoranteSelezionato;
        public Ristorante RistoranteSelezionato
        {
            get => _ristoranteSelezionato;
            set
            {
                _ristoranteSelezionato = value;
                OnPropertyChanged();
            }
        }

        private string _messaggioErrore;
        public string MessaggioErrore
        {
            get => _messaggioErrore;
            set
            {
                _messaggioErrore = value;
                OnPropertyChanged();
            }
        }

        private string _messaggioSuccesso;
        public string MessaggioSuccesso
        {
            get => _messaggioSuccesso;
            set { _messaggioSuccesso = value; OnPropertyChanged(); }
        }

        //utente aggiunto per mostrare/nascondere comandi di modifica/eliminazione
        private Utente _utenteCorrente;
        public Utente UtenteCorrente
        {
            get => _utenteCorrente;
            set
            {
                _utenteCorrente = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAdministrator));
            }
        }

        //aggiunto per mostrare/nascondere comandi di modifica/eliminazione
        private bool _isAdministrator;
        public bool IsAdministrator
        {
            get => _isAdministrator;
            set
            {
                _isAdministrator = value;
                OnPropertyChanged();
            }
        }

        private string _filtroCitta;
        public string FiltroCitta
        {
            get => _filtroCitta;
            set
            {
                _filtroCitta = value;
                OnPropertyChanged();
            }
        }

        private TipologiaRistorante? _filtroTipologia;
        public TipologiaRistorante? FiltroTipologia
        {
            get => _filtroTipologia;
            set
            {
                _filtroTipologia = value;
                OnPropertyChanged();
            }
        }

        private decimal? _filtroPrezzoMax;
        public decimal? FiltroPrezzoMax
        {
            get => _filtroPrezzoMax;
            set
            {
                _filtroPrezzoMax = value;
                OnPropertyChanged();
            }
        }



        // Lista tipologie per il ComboBox
        public List<string> TipologieDisponibili { get; } = new List<string>
        {
            "Tutte",
            "Pizzeria",
            "Pesce",
            "Carne",
            "Vegetariano",
            "Cucina locale"
        };


        private string _tipologiaSelezionata = "Tutte";
        public string TipologiaSelezionata
        {
            get => _tipologiaSelezionata;
            set
            {
                _tipologiaSelezionata = value;
                OnPropertyChanged();

                // Converte la stringa nell'enum corrispondente
                FiltroTipologia = value == "Tutte"
                    ? (TipologiaRistorante?)null
                    : (TipologiaRistorante)TipologieDisponibili.IndexOf(value);
            }
        }




        //Comandi
        public ICommand CaricaCommand { get; }
        public ICommand NuovoCommand { get; }
        public ICommand ModificaCommand { get; }
        public ICommand EliminaCommand { get; }
        public ICommand CercaCommand { get; }
        public ICommand ResetCommand { get; }






        //Costruttore
        public RistoranteListViewModel(RistoranteManager manager, bool isAdministrator)
        {

            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            IsAdministrator = isAdministrator;

            CaricaCommand = new RelayCommand(EseguiCarica);
            NuovoCommand = new RelayCommand(
                execute: EseguiNuovo,
                canExecute: () => IsAdministrator);
            ModificaCommand = new RelayCommand(
                execute: EseguiModifica,
                canExecute: () => IsAdministrator && RistoranteSelezionato != null);
            EliminaCommand = new RelayCommand(
                execute: EseguiElimina,
                canExecute: () => IsAdministrator && RistoranteSelezionato != null);
            CercaCommand = new RelayCommand(EseguiCerca);
            ResetCommand = new RelayCommand(EseguiReset);


            EseguiCarica();
        }




        //Logica Comandi
        private void EseguiCarica()
        {
            try
            {
                MessaggioErrore = null;
                ListaRistoranti = _manager.GetAll();
            }
            catch (Exception ex)
            {
                //per debug, mostra tutta la catena di eccezioni
                MessaggioErrore = ex.Message;
                if (ex.InnerException != null)
                    MessaggioErrore += " -> " + ex.InnerException.Message;

                ListaRistoranti = new List<Ristorante>();
            }
        }

        private void EseguiCerca()
        {
            try
            {
                MessaggioErrore = null;
                MessaggioSuccesso = null;

                ListaRistoranti = _manager.Cerca(
                    FiltroCitta,
                    FiltroTipologia,
                    FiltroPrezzoMax);
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
                ListaRistoranti = new List<Ristorante>();
            }
        }

        private void EseguiReset()
        {
            FiltroCitta = null;
            TipologiaSelezionata = "Tutte";
            FiltroPrezzoMax = null;
            EseguiCarica();
        }

        //metodi per eseguire i comandi, invocano eventi per comunicare con la view e mostrare altre view
        private void EseguiNuovo() => RichiestaNuovo?.Invoke(this, EventArgs.Empty);
        private void EseguiModifica() => RichiestaModifica?.Invoke(this, RistoranteSelezionato);
        private void EseguiElimina()
        {
            try
            {
                MessaggioErrore = null;
                MessaggioSuccesso = null;
                _manager.Elimina(RistoranteSelezionato.IDRistorante);
                MessaggioSuccesso = $"Ristorante {RistoranteSelezionato.RagioneSociale} eliminato con successo.";
                EseguiCarica();
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
            }
        }

    }
}
