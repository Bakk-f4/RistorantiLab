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
    public class PrenotazioneListViewModel : ViewModelBase
    {


        private readonly PrenotazioneManager _prenManager;
        private readonly RistoranteManager _ristManager;

        private readonly Utente _utenteCorrente;

        public event EventHandler RichiestaNuova;
        public event EventHandler<Prenotazione> RichiestaModifica;




        //proprieta viewmodel
        private List<Ristorante> _listaRistoranti;
        public List<Ristorante> ListaRistoranti
        {
            get => _listaRistoranti;
            set { _listaRistoranti = value; OnPropertyChanged(); }
        }

        private Ristorante _ristoranteSelezionato;
        public Ristorante RistoranteSelezionato
        {
            get => _ristoranteSelezionato;
            set
            {
                _ristoranteSelezionato = value;
                OnPropertyChanged();
                //quando cambia il ristorante ricarica le prenotazioni
                if (value != null)
                    EseguiCarica();
            }
        }

        private DateTime _dataFiltro = DateTime.Today;
        public DateTime DataFiltro
        {
            get => _dataFiltro;
            set
            {
                _dataFiltro = value;
                OnPropertyChanged();
                //quando cambia la data aggiorna i posti disponibili
                AggiornaPosti();
            }
        }


        private int _postiTotali;
        public int PostiTotali
        {
            get => _postiTotali;
            set { _postiTotali = value; OnPropertyChanged(); }
        }

        private int _postiOccupati;
        public int PostiOccupati
        {
            get => _postiOccupati;
            set { _postiOccupati = value; OnPropertyChanged(); }
        }

        private int _postiDisponibili;
        public int PostiDisponibili
        {
            get => _postiDisponibili;
            set { _postiDisponibili = value; OnPropertyChanged(); }
        }

        private List<Prenotazione> _listaPrenotazioni;
        public List<Prenotazione> ListaPrenotazioni
        {
            get => _listaPrenotazioni;
            set
            {
                _listaPrenotazioni = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PuoModificareSelezionata));
            }
        }

        private Prenotazione _prenotazioneSelezionata;
        public Prenotazione PrenotazioneSelezionata
        {
            get => _prenotazioneSelezionata;
            set
            {
                _prenotazioneSelezionata = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PuoModificareSelezionata));
            }
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



        public bool IsAmministratore => _utenteCorrente?.isAdministrator ?? false;

        public bool PuoModificareSelezionata =>
            PrenotazioneSelezionata != null &&
            (_utenteCorrente.isAdministrator ||
             PrenotazioneSelezionata.NomeUtente == _utenteCorrente.Username);


        //comandi
        public ICommand CaricaCommand { get; }
        public ICommand NuovaCommand { get; }
        public ICommand ModificaCommand { get; }
        public ICommand EliminaCommand { get; }





        public PrenotazioneListViewModel(
            PrenotazioneManager prenManager,
            RistoranteManager ristManager,
            Utente utenteCorrente)
        {

            _utenteCorrente = utenteCorrente;
            _prenManager = prenManager
                ?? throw new ArgumentNullException(nameof(prenManager));
            _ristManager = ristManager
                ?? throw new ArgumentNullException(nameof(ristManager));

            CaricaCommand = new RelayCommand(EseguiCarica);
            NuovaCommand = new RelayCommand(
                execute: () => RichiestaNuova?.Invoke(this, EventArgs.Empty),
                canExecute: () => RistoranteSelezionato != null);

            ModificaCommand = new RelayCommand(
                execute: () => RichiestaModifica?
                                .Invoke(this, PrenotazioneSelezionata),
                canExecute: () => PrenotazioneSelezionata != null);

            EliminaCommand = new RelayCommand(
                execute: EseguiElimina,
                canExecute: () => PrenotazioneSelezionata != null);

            // Carica la lista ristoranti per il ComboBox
            CaricaRistoranti();
        }



        //logica metodi aux
        private void CaricaRistoranti()
        {
            try
            {
                ListaRistoranti = _ristManager.GetAll();
            }
            catch (Exception ex)
            {
                MessaggioErrore = $"Errore durante il caricamento della lista di ristoranti " + ex.Message;
            }
        }

        private void EseguiCarica()
        {
            if (RistoranteSelezionato == null) return;

            try
            {
                MessaggioErrore = null;
                MessaggioSuccesso = null;

                ListaPrenotazioni = _prenManager
                    .GetByRistorante(RistoranteSelezionato.IDRistorante);

                PostiTotali = RistoranteSelezionato.NumPosti;
                AggiornaPosti();
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
                ListaPrenotazioni = new List<Prenotazione>();
            }
        }

        private void AggiornaPosti()
        {
            if (RistoranteSelezionato == null) return;

            try
            {
                PostiDisponibili = _prenManager.GetPostiDisponibili(
                    RistoranteSelezionato.IDRistorante, DataFiltro);

                PostiOccupati = PostiTotali - PostiDisponibili;
            }
            catch (Exception ex)
            {
                MessaggioErrore = $"Errore durante il calcolo dei posti disponibili: {ex.Message}";
            }
        }

        private void EseguiElimina()
        {
            try
            {
                MessaggioErrore = null;
                MessaggioSuccesso = null;

                _prenManager.Elimina(PrenotazioneSelezionata.IDPrenotazione, _utenteCorrente);
                MessaggioSuccesso = $"Prenotazione eliminata con successo con ID: {PrenotazioneSelezionata.IDPrenotazione}.";
                EseguiCarica();
            }
            catch (Exception ex)
            {
                MessaggioErrore = ex.Message;
            }
        }


































































    }
}
